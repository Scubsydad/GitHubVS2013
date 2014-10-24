using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Settlers_of_Catan
{
	public	enum	UTIL
	{
		LOGIC_BLUE,
		LOGIC_ORANGE,
		LOGIC_RED,
		LOGIC_SILVER,
		SYSTEM,
		_size
	};

	public enum CONTROL
	{
		CPU,
		USER,
		NA,
		_size,
	};

	public enum COORD_TYPE
	{
		A,
		B,
		C,
		D,
		E,
		F,
		_size
	};

	public enum ASSET
	{
		INVALID = -1,
		CITY,
		ROAD,
		SETTLEMENT,
		DEV_CARD,
		_size
	};

	public enum RESOURCE
	{
		INVALID = -1,
		BRICK,
		GRAIN,
		LUMBER,
		ORE,
		WOOL,
        _size,	//	in the case of a 'port resource type' this is a 3-1 location for trading
	};

    public enum TERRAIN
    {
		INVALID = -1,
        DESERT,
        FIELDS,
        FOREST,
        HILLS,
        MOUNTAINS,
        PASTURE,
        _size,
    };

	public enum OWNER
	{
		INVALID = -1,
        BLUE,
        ORANGE,
        RED,
        SILVER,
        _size,
		MANAGER,	//	special enum for 'System' calls
	};

	public enum PORT
	{
		INVALID = -1,
		ZERO,
		ONE,
		TWO,
		THREE,
		FOUR,
		FIVE,
		SIX,
		SEVEN,
		EIGHT,
		_size,
	};

	public	enum	DIR
	{
		INVALID = -1,
		NORTH,
        NORTH_EAST,
        SOUTH_EAST,
		SOUTH,
        SOUTH_WEST,
        NORTH_WEST,
        _size,
	};

	public enum CITY_DIR
	{
		INVALID = -1,
		NORTH_WEST,
		NORTH_EAST,
		EAST,
		SOUTH_EAST,
		SOUTH_WEST,
		WEST,
		_size,
	};
	
	public partial class Form1 : Form
	{
		private	UtilityMgr		mUtilMgr;
		private ResourceKernel  mSysDefsResKernel;
		private SysDefsKernel	mSysDefsKernel;
		private CONTROL[]		mSideCtrl = new CONTROL[(int)OWNER._size];
		private ComboBox[]		mSideCtrlCombos;
		private	bool			mGameActive = false;
		private int				mNumOwnersActive;
		private MapManager		mMapManager;
		private	MessageCenter	mMessageCenter;
		private	PlayGameMgr		mPlayGameMgr;
		private	MessageHistory	mMessageHistory;
		private LogicUtil[]		mLogicUtils = new LogicUtil[(int)UTIL._size];
		private Support			mSupport = new Support();

		static public string GetBrkFileFolderPath(string fileName) {  return ( fileName ); }

		public Form1()
		{
			InitializeComponent();
	
			mSideCtrlCombos = new ComboBox[] { SideCtrlCombo0, SideCtrlCombo1, SideCtrlCombo2, SideCtrlCombo3 };

			new TrackBarAssociation(new TrackBar[] { FirstPlayerTrackBar }, new Label[] { FirstPlayerTrackBarLbl }, new string[] { "Random", "Blue", "Orange", "Red", "Silver" } ); 
			mUtilMgr = new UtilityMgr( UTIL.SYSTEM, "SystemDefs.brk", false );

			mUtilMgr.RegisterDataSegments("DieRollPct", new DataSegments( new RichTextBox[] { DieRollPct }, 10, 250 ) );

			if ( mUtilMgr.CheckLoadBrkFile() )
			{
				RefreshInternals();
			}

			_InitLogicUtil( OWNER.BLUE,		"Blue.brk" );
			_InitLogicUtil( OWNER.ORANGE,	"Orange.brk" );
			_InitLogicUtil( OWNER.RED,		"Red.brk" );
			_InitLogicUtil( OWNER.SILVER,	"Silver.brk" );
		}

		public	void		RefreshInternals()
		{
			mSysDefsResKernel = mUtilMgr.GetResourceKernel();
			mSysDefsKernel = new SysDefsKernel( mSysDefsResKernel );
		}

		public  void UpdateUtilDirtyStatus( )
		{
			RefreshInternals();
		}

		private void _ToggleControlStatusUponStartStop( )
		{
			bool	beginGameDisable = ( !mGameActive );
			bool	beginGameEnable = mGameActive;
		// these controls should be disabled once game begins
			foreach ( ComboBox comboBox in mSideCtrlCombos )
			{
				comboBox.Enabled = beginGameDisable;
			}
			FirstPlayerTrackBar.Enabled = beginGameDisable;
		// these controls should be active when game begins


		}

		private void _StartStopGameRequest(object sender, EventArgs e)
		{
			if ( StartStopButton.Text == "Start Game" )
			{
				CONTROL	control;
				int		numUsers = 0;
				for ( int i = 0; i < (int)OWNER._size; ++i )
				{
					control = (CONTROL)mSideCtrlCombos[i].SelectedIndex;
					mSideCtrl[i] = control;
					if ( control == CONTROL.USER )
					{
						++numUsers;
					}
				}
				mNumOwnersActive = (int)OWNER._size;		//	assume four owners by default
				if ( mSideCtrl[(int)OWNER.SILVER] == CONTROL.NA )
				{
					--mNumOwnersActive;						//	decrease to 3 active owners if silver is 'n/a'
				}
				if ( numUsers == 0 )
				{
					MessageBox.Show("You must activate at least ONE 'User' controlled team to play.", "Error", MessageBoxButtons.OK );
				}
				else
				{
//					DialogResult result = MessageBox.Show("Are you sure you wish to begin the game with these settings?", "Please Confirm...", MessageBoxButtons.YesNo );
//					if ( result == System.Windows.Forms.DialogResult.Yes )
					{
						_StartGamePrep( );
					}
				}

			}
			else
			{
				_PostGameCleanup();
			}
		}

		private void _PostGameCleanup()
		{
			mGameActive = false;
			_ToggleControlStatusUponStartStop();
			StartStopButton.Text = "Start Game";
			mMessageHistory.Hide();
			mMessageHistory.Dispose();
			mMapManager = null;
			mMessageHistory = null;
			mPlayGameMgr = mPlayGameMgr.ShutDown();
			mMessageCenter = null;
		}

		private void _StartGamePrep()
		{
			mGameActive = true;
			_ToggleControlStatusUponStartStop();
			StartStopButton.Text = "Quit Game";

			mMessageCenter = Support.InitMessageCenter( 10000 );

			mMessageHistory = new MessageHistory( mMessageCenter, mPlayGameMgr );
//	figure out the random seed & broadcast it out to stakeholders
			int randSeed = Support.InitRandNumGen( UtilData.GetShortValue( RandomSeedBox ) );
			RandSeedLbl.Text = string.Format("Random # Seed :               {0}", randSeed );

			mMessageCenter.SendMsgRandomNumSeed( randSeed );

			mMapManager = new MapManager( mMessageCenter );
			mPlayGameMgr = new PlayGameMgr( mMessageCenter, mNumOwnersActive, mSideCtrl, mMapManager, mMapPictBox, StateExplainTabs, mMessageHistory, mSysDefsKernel );


			mMessageCenter.SendMsgInitTerrainRequest( MapTypeCombo.SelectedIndex );
			mMessageCenter.SendMsgInitDieRollRequest( ResourceRollCombo.SelectedIndex );
			mMessageCenter.SendMsgInitPortLocRequest( PortLocationsCombo.SelectedIndex );

			int playerStartIndex = FirstPlayerTrackBar.Value;
			while ( playerStartIndex == 0 )	//	is the track bar set to 'random' starting player?
			{
				playerStartIndex = Support.GetRand( FirstPlayerTrackBar.Maximum );
			}

			mMessageHistory.Show();

			mMapPictBox.Image = mMapManager.GenerateMapGfx();

			mPlayGameMgr.InitTurnOrder( (OWNER)( playerStartIndex - 1 ), ( TurnOrderCombo.SelectedIndex == 1) );

		}

		private void _SilverOwnerChangeCallBack(object sender, EventArgs e)
		{
			CONTROL control = (CONTROL)mSideCtrlCombos[(int)OWNER.SILVER].SelectedIndex;
			if ( control == CONTROL.NA )			//	if 'silver' is set to 'n/a', then don't allow their selection as 'first player'
			{
				if ( FirstPlayerTrackBar.Value == (int)OWNER._size )// this == 'silver' for the track bar as 'zero' == 'random'
				{
					--FirstPlayerTrackBar.Value;	//	then convert 'silver' to 'red' before we cap the maximum
				}
				FirstPlayerTrackBar.Maximum = 3;
			}
			else
			{
				FirstPlayerTrackBar.Maximum = 4;
			}
		}

		private LogicUtil	_InitLogicUtil( OWNER whichSide, string utilFileName )
		{
			UTIL whichUtil = (UTIL)((int)UTIL.LOGIC_RED + (int)whichSide);
			LogicUtil logicUtil = new LogicUtil(whichSide, utilFileName, whichUtil );
			Control.ControlCollection utilControls = logicUtil.Controls;

			Control[] controlArray = new Control[utilControls.Count];
			utilControls.CopyTo(controlArray, 0);
			SideLogicTabs.TabPages[(int)whichSide].Controls.AddRange( controlArray );

			logicUtil.GetUtilMgr().SubmitClassCreator( this );								//	create association so utility can call out and let us know its 'dirty'

			mLogicUtils[(int)whichSide] = logicUtil;

			return ( logicUtil );
		}

		private void _SaveFileRequest(object sender, EventArgs e)
		{
			mUtilMgr.SaveFileRequest( true );
		}

		private void _ValidationRequest(object sender, EventArgs e)
		{
			mUtilMgr.ValidateAllData();
		}
	}
}
