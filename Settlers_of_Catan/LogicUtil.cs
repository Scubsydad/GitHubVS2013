using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	public partial class LogicUtil : Form
	{
		private	UtilityMgr			mUtilMgr;
		private LogicKernel			mLogicKernel;
		private MapManager			mMapManager;
		private Bitmap				mMapBmap;
		private HexInfo[]			mHexInfo = Support.GetHexInfoCopy();
		private SideLogic			mSideLogic;
		private OWNER				mWhichSide, mTempSettlementOwner = OWNER.INVALID;
		private Font				mTextFont = new Font( "Sylfaen", 10.0f );
		private Font				mCityFont = new Font( "Sylfaen", 12.0f );
		private bool				mPlaceSettlementMessageShown = false;
		private Color				mWhichColor;
		private int					mSideSettlementHexIndex;
		private Bitmap				mHexAreaBmap, mHexCityDirBmap = new Bitmap( "art//settlement_legend_hex.png");
		private Point[]				mAdjHexOffsets = new Point[] { new Point( 70, 2 ),  new Point( 135, 32 ),  new Point( 135, 100 ),  new Point( 70, 130 ),  new Point( 12, 100 ),  new Point( 12, 32 ) };
		private Point[]				mCityOffsets = new Point[] { new Point( 30, 0 ),  new Point( 115, 0 ),  new Point( 146, 68 ),  new Point( 115, 120 ),  new Point( 30, 120 ),  new Point( 0, 68 ) };
		private Size				mAdjHexValSize = new Size( 25, 13 );
		private Size				mEllipseValSize = new Size( 26, 24 );
		private Pen					mEllipsePen = new Pen( Brushes.Cyan, 4.0f );
		private ArrayList			mSettlementPickOrder = new ArrayList();
		private CITY_DIR			mSettlementHexCityDir = CITY_DIR.INVALID;
		private BuildLoc[]			mSettlementLocs = new BuildLoc[2];

		public LogicUtil( OWNER whichSide, string utilFileName, UTIL utilOwner )
		{
			InitializeComponent();

			TerrainTemplate.Hide();

			mWhichSide = whichSide;

			ArrayList	colors = new ArrayList();
			ArrayList	owners = new ArrayList();
			OWNER		ownerEnum;
			for ( int i = 0; i < (int)OWNER._size; ++i )
			{
				ownerEnum = (OWNER)i;
				owners.Add( ownerEnum );
				colors.Add( Support.GetSideColor( ownerEnum ) );
			}

			mWhichColor = (Color)colors[(int)whichSide];		//	extract our color before we remove it
			colors.RemoveAt( (int)whichSide );

			PlayerAddSettlementButton0.BackColor = mWhichColor;

			owners.RemoveAt( (int)whichSide );

			Button[] settlementButtons = new Button[] { SettlementButton00, SettlementButton01, SettlementButton02, 
														SettlementButton10, SettlementButton11, SettlementButton12 };
			for ( int i = 0; i < 3; ++i )
			{
				settlementButtons[i].BackColor = settlementButtons[( i + 3 )].BackColor = (Color)colors[i];
				settlementButtons[i].Tag = settlementButtons[( i + 3 )].Tag = (OWNER)owners[i];
			}

			Size	box2416 = new Size(24,16);

			mUtilMgr = new UtilityMgr( utilOwner, utilFileName, false );

			object[] translation = new object[] { box2416, box2416, box2416, box2416, box2416, box2416, box2416, box2416, box2416, box2416, };
			int[]    minVals = new int[]		{ 0,       0,       0,       0,       0,       0,       0,       100,     1,       0,       };
			int[]    maxVals = new int[]		{ 100,     100,     100,     100,     10,      150,     100,     150,     10,      100,     };

			ArrayList[] controls = mUtilMgr.RegisterDataSegments( "StartLocs0", new DataSegments( translation, new RichTextBox[] { PickStartLoc00, PickStartLoc01, PickStartLoc02, PickStartLoc03, PickStartLoc04, PickStartLoc05, PickStartLoc06, PickStartLoc07, PickStartLoc08, PickStartLoc09 }, minVals, maxVals ) );
			((TextBox)controls[9][0]).Hide();
			mUtilMgr.RegisterDataSegments("StartLocs1", new DataSegments( translation, new RichTextBox[] { PickStartLoc10, PickStartLoc11, PickStartLoc12, PickStartLoc13, PickStartLoc14, PickStartLoc15, PickStartLoc16, PickStartLoc17, PickStartLoc18, PickStartLoc19 }, minVals, maxVals ) );

			translation = new object[]  { box2416, box2416, box2416, box2416, box2416, box2416, box2416, box2416, PortResourceTemplate };
			minVals = new int[]			{ 5,       5,       5,       5,       5,       1,       10,      10,	  0  };
			maxVals = new int[]			{ 100,     100,     100,     100,     100,     100,     100,     100,	  0  };
			mUtilMgr.RegisterDataSegments("PickLocData", new DataSegments( translation, new RichTextBox[] { PickLocData0, PickLocData1, PickLocData2, PickLocData3, PickLocData4, PickLocData5, PickLocData6, PickLocData7, PickLocData8 }, minVals, maxVals ) );

//			mUtilMgr.AddIgnoreUponLoadForSegments( "PickLocData", new int[] { 0, 1, 2, 3, 4 } );

			translation = new object[] {  box2416, PortResourceTemplate, HexResourceTemplate, box2416 };
			minVals = new int[]		   {  0,       0,					 0,					  1       };
			maxVals = new int[]		   {  70,      0,					 0,					  5		  };
			mUtilMgr.RegisterDataSegments("RoadData0", new DataSegments( translation, new RichTextBox[] { RoadData00, RoadData01, RoadData02, RoadData03 }, minVals, maxVals ) );

			if ( mUtilMgr.CheckLoadBrkFile() )
			{
				RefreshInternals();
			}

			mMapManager = new MapManager( null );

			_NewMapRequest( null, null );
		}


		
		public	UtilityMgr	GetUtilMgr()
		{
			return ( mUtilMgr );
		}

		public	LogicKernel		RefreshInternals()
		{
			mLogicKernel = new LogicKernel( mUtilMgr.GetResourceKernel() );
			mSideLogic = new SideLogic( mWhichSide, mLogicKernel, mHexInfo );
			Support.SubmitLogicKernel( mWhichSide, mLogicKernel );
			return ( mLogicKernel );
		}

		private void _ValidationRequest(object sender, EventArgs e)
		{
			mUtilMgr.ValidateAllData();
		}

		private void _SaveFileRequest(object sender, EventArgs e)
		{
			if ( mUtilMgr.SaveFileRequest( true ) )
			{
				RefreshInternals();
			}
		}

		private void _NewMapRequest(object sender, EventArgs e)
		{
			mHexInfo = mMapManager.GenerateTempMap(  ( TerrainCombo.SelectedIndex == 1), // random terrain
													 ( DieRollCombo.SelectedIndex == 1), // random die rolls
													 ( PortsCombo.SelectedIndex == 1) ); // random ports
			mSideLogic.SubmitNewHexInfo( mHexInfo );

			mMapBmap = mMapManager.GenerateMapGfx();
			_NewValuesRequest( null, null );
		}

		private void _NewValuesRequest(object sender, EventArgs e)
		{
			if ( mUtilMgr.ValidateAllData() )
			{ 
				int numSettlements = PickStartLocTabs.SelectedIndex;
				Bitmap bmap = _DrawGameMap( false );
				Graphics gfx = Graphics.FromImage( bmap );
				RESOURCE	hex1Resource = RESOURCE.INVALID;
				if ( sender == RunLogicButton2 )
				{
					hex1Resource = RESOURCE._size;	//	fix me when we track the resource of 'settlement 1'!!!
				}
				Point[] optionalList = new Point[mHexInfo.Length];
				int[]	hexVals = new int[mHexInfo.Length];
				int	chosenIndex = mSideLogic.DoSettlementWeighting( mHexInfo, hex1Resource, hexVals, optionalList );
				mMapPictBox.Image = bmap;

				int	numHexesToConsider = mLogicKernel.GetStartSettlementSortListMax( numSettlements );

				Point	coordGfx;
				int		x, y, pct = 0;
				Brush	textBrush;
				Brush	rectBrush;
				for ( int i = 0; i < mHexInfo.Length; ++i )
				{
					coordGfx = Support.GetHexBase( i ).GetGfxLoc();
					x = ( ( coordGfx.X * 53 ) / 100 ) + 25;
					y = ( ( coordGfx.Y * 53 ) / 100 ) + 4;
					textBrush = Brushes.Black;
					rectBrush = Brushes.White;
					
					if ( i == chosenIndex )
					{
						textBrush = Brushes.Red;
						rectBrush = Brushes.Yellow;
					}
					if ( numHexesToConsider != 1 )
					{
						pct = 0;
						foreach ( Point vals in optionalList )
						{
							if ( vals.Y == i )
							{
								pct = vals.X;
								break;
							}
						}
					}
					if ( pct == 0 )
					{
						gfx.FillRectangle( rectBrush, x, y, 30, 13 );
						if ( i == chosenIndex )
						{
							gfx.DrawRectangle( Pens.Red, x, y, 30, 13 );
						}
					}
					else
					{
						gfx.FillRectangle( rectBrush, x, y, 30, 26 );
						gfx.DrawRectangle( Pens.Red, x, y, 30, 26 );
						gfx.DrawString( string.Format("{0}%", pct ), mTextFont, Brushes.Purple, x + 2 , y + 10 );
					}
					gfx.DrawString( hexVals[i].ToString(), mTextFont, textBrush, x + 4 , y - 3 );
				}
			}
		}

		private void _RemoveSettlements( bool allowInitialSettlements )
		{
			foreach ( HexInfo hexInfo in mHexInfo )
			{
				hexInfo.ClearOwnership( OWNER._size );
			}
			mSettlementPickOrder = new ArrayList();
			PlayerAddSettlementButton0.Enabled = SettlementButton00.Enabled = SettlementButton01.Enabled = SettlementButton02.Enabled = true;
			mSettlementLocs = new BuildLoc[2];
		}

		private void _SettlementButtonClick(object sender, EventArgs e)
		{
			if ( !mPlaceSettlementMessageShown )
			{
				mPlaceSettlementMessageShown = true;
				MessageBox.Show("You may place up to three different 'Settlements' to reflect CPU\ndecisions that occured before your turn. This allows you to test\nscenarios if you will build too close to other opponents etc.\n\nClick on a button for the other color you wish to add then click\non the map in the area you want the settlement.\n\nClick one of the '+Settlment' buttons again to continue." );
			}
			else
			{
				if ( ( sender == SettlementButtonRemove0 ) ||
					 ( sender == SettlementButtonRemove1 ) ||
					 ( sender == SettlementButtonRemove2 ) )
				{
					mTempSettlementOwner = OWNER.INVALID;
					_RemoveSettlements( ( sender == SettlementButtonRemove2 ) );
				}
				else
				{
					Button button = (Button)sender;
					button.Enabled = false;
					mTempSettlementOwner = (OWNER)button.Tag;
				}
				_NewValuesRequest( null, null );
			}
		}

		private void _DropSettlementMouseMove(object sender, MouseEventArgs e)
		{
			if ( mTempSettlementOwner != OWNER.INVALID )
			{ 
				SettlementLoc settlementLoc = MapDefs.GetSettlmentLocIndex( e.X, e.Y, mMapPictBox.Size, 15 );
				if ( settlementLoc != null )
				{
					HexInfo		hexInfo = mHexInfo[settlementLoc.GetShareSettlementHexId(0)];
					BuildLoc	buildLoc = hexInfo.GetBuildLoc( settlementLoc.GetShareSettlementHexDir(0) );
					if ( ( !buildLoc.IsOccupied() ) &&
						 ( buildLoc.IsValidBuildLoc() ) )
					{ 
						Bitmap bmap = _DrawGameMap( true );
						settlementLoc.DrawSettlement( bmap, mTempSettlementOwner );
					}
				}
			}
		}

		private Bitmap _DrawGameMap( bool wantInvalidLocHighlights )
		{
			Bitmap bmap = new Bitmap( mMapBmap, mMapPictBox.Size );
			foreach ( HexInfo hexInfo in mHexInfo )
			{
				foreach ( BuildLoc buildLoc in hexInfo.GetBuildLocs() )
				{
					if ( ( buildLoc.IsOccupied() ) ||
					     ( ( wantInvalidLocHighlights ) &&
							  ( !buildLoc.IsValidBuildLoc() ) ) )
					{
						buildLoc.GetSettlementLoc().DrawSettlement( bmap, buildLoc.GetOwner() );
					}
				}
			}
			mMapPictBox.Image = bmap;
			return ( bmap );
		}

		private void _DropSettlementMouseDown(object sender, MouseEventArgs e)
		{
			if ( mTempSettlementOwner != OWNER.INVALID )
			{ 
				SettlementLoc settlementLoc = MapDefs.GetSettlmentLocIndex( e.X, e.Y, mMapPictBox.Size, 15 );
				if ( settlementLoc != null )
				{
					HexInfo		hexInfo = mHexInfo[settlementLoc.GetShareSettlementHexId(0)];
					BuildLoc	buildLoc = hexInfo.GetBuildLoc( settlementLoc.GetShareSettlementHexDir(0) );
					if ( ( !buildLoc.IsOccupied() ) &&
						 ( buildLoc.IsValidBuildLoc() ) )
					{ 
						int		settlementId = settlementLoc.GetUniqueId();
						mSideLogic.MsgAddSettlement( 0, mTempSettlementOwner, settlementId, 0 );
						_IncrementSettlementPlacedCount( mTempSettlementOwner );
						_NewValuesRequest( null, null );
						mTempSettlementOwner = OWNER.INVALID;
					}
				}
			}
		}

		private void _PickStartLocTabPageChanged(object sender, EventArgs e)
		{
			if ( ( PickStartLocTabs.SelectedIndex == 1 ) && 
					( !ConfirmSettlementButton2.Enabled ) )
			{
				MessageBox.Show("You must place all three CPU and the 'Current Side'\nsettlements before you may access/tune this data.", "Please Note!" );
				PickStartLocTabs.SelectTab( 0 );
			}
		}

		private void _SideSettlementButtonClick(object sender, EventArgs e)
		{
			if ( mUtilMgr.ValidateAllData() )
			{
				RESOURCE	hex1Resource = RESOURCE.INVALID;
				if ( mSettlementLocs[0] != null )
				{
					hex1Resource = mHexInfo[mSettlementLocs[0].GetHexAssocId()].GetEarnResource();
				}
				mSideSettlementHexIndex  = mSideLogic.DoSettlementWeighting( mHexInfo, hex1Resource, null, null );
				mSettlementHexCityDir = CITY_DIR.INVALID;									//	clear this until we confirm a settlement by clicking on 'confirm'
				HexBase	hexBase = Support.GetHexBase( mSideSettlementHexIndex );	//	'hexBase' is a subset of 'Info' and ordered in same manner
				Point	gfxLoc	= hexBase.GetGfxLoc();
				int		clipDelta = 15;
				Point   readStart =  new Point( gfxLoc.X - clipDelta, gfxLoc.Y - clipDelta );
				Size	clipSize = new Size( ( 142 + ( clipDelta * 2 ) ), ( 120 + ( clipDelta * 2 ) ) );
				Rectangle	clipRect = new Rectangle( readStart, clipSize );
				mHexAreaBmap = mMapBmap.Clone( clipRect, System.Drawing.Imaging.PixelFormat.DontCare );
				SettlementLegendPictBox.Image = mHexAreaBmap;

				ConfirmSettlementButton1.Enabled = SettlementValButton.Enabled = true;

				_SettlementValButtonClick( null, null );
			}
		}

		private void _SettlementValButtonClick(object sender, EventArgs e)
		{
			if ( mUtilMgr.ValidateAllData() )
			{
				int[]		adjHexVals = new int[(int)DIR._size];
				int[]		cornerVals = new int[(int)CITY_DIR._size];
				Point		drawLoc;
				Bitmap		bmap = new Bitmap( mHexAreaBmap );
				Graphics	gfx = Graphics.FromImage( bmap );
				mSettlementHexCityDir = mSideLogic.GetSettlementLoc( mSideSettlementHexIndex, adjHexVals, cornerVals );

				for ( int loop = 0; loop < (int)DIR._size; ++loop)
				{
					drawLoc = mAdjHexOffsets[loop];
					gfx.FillRectangle( Brushes.White, new Rectangle( drawLoc, mAdjHexValSize ));
					gfx.DrawString( Support.GetIntDesc( adjHexVals[loop] ), mTextFont, Brushes.Black, ( drawLoc.X + 1 ), ( drawLoc.Y - 2 ) );

					drawLoc = mCityOffsets[loop];
					gfx.FillEllipse( Brushes.WhiteSmoke, new Rectangle( drawLoc, mEllipseValSize ));
					gfx.DrawString( Support.GetIntDesc( cornerVals[loop] ), mCityFont, Brushes.Green, ( drawLoc.X - 3 ), ( drawLoc.Y  ) );

				}
				drawLoc = mCityOffsets[(int)mSettlementHexCityDir];
				gfx.DrawEllipse(mEllipsePen, new Rectangle( new Point( ( drawLoc.X - 2 ), ( drawLoc.Y - 2 ) ), mEllipseValSize ));
				gfx.DrawString( Support.GetIntDesc( cornerVals[(int)mSettlementHexCityDir] ), mCityFont, Brushes.Red, ( drawLoc.X - 3 ), ( drawLoc.Y  ) );

				SettlementLegendPictBox.Image = bmap;
			}
		}

		private void _IncrementSettlementPlacedCount( OWNER whichSide )
		{
			mSettlementPickOrder.Add( whichSide );
			if ( mSettlementPickOrder.Count == 4 )
			{
				ConfirmSettlementButton2.Enabled = true;
				MessageBox.Show("You may now make edits to 'Second Settlement' and\nplace the second initial group of CPU Settlements.\n\nSecond Settlements are placed in 'reverse order' of the first.", "Please Note" );
			}
		}
		private void _ConfirmSideSettlementClicked(object sender, EventArgs e)
		{
			if ( mUtilMgr.ValidateAllData() )
			{
				((Button)sender).Enabled = false;
				BuildLoc buildLoc = mHexInfo[mSideSettlementHexIndex].GetBuildLoc( mSettlementHexCityDir );
				int		settlementIndex = 0;				//	assume first by default
				if ( sender == ConfirmSettlementButton1 )
				{
					_IncrementSettlementPlacedCount( mWhichSide );
				}
				else // sender == confirmButton2
				{
					settlementIndex = 1;
				}
				mSettlementLocs[settlementIndex++] = buildLoc;		//	post increment array index so we pass in 'count value' to this function here
				mSideLogic.MsgAddSettlement( 0, mWhichSide, buildLoc.GetSettlementLoc().GetUniqueId(), settlementIndex );
				_NewValuesRequest( null, null );
			}
		}

	}
}
