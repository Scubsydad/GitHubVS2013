using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for MessageDisplay.
	/// </summary>
	public class MessageDisplay	:	MessageHandler
	{
		public class	Message
		{
			public	int			timeStamp;
			public	int			messageId;
			public	OWNER		whichSide;
			public	MessageType	msgType; 
			public	int			uniqueId;
			public	string		desc;
			
			public Message( OWNER side, int msgId, int time, MessageType whatType )
			{
				_InitMsg( side, msgId, time, whatType, new Point( -1, -1 ), -1, "", "" );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType , Point coord )
			{
				_InitMsg( side, msgId, time, whatType, coord, -1, "", ""  );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType, Point coord, string miscDesc )
			{
				_InitMsg( side, msgId, time, whatType, coord, -1, "", miscDesc  );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType,int uniqueId, string miscDesc )
			{
				_InitMsg( side, msgId, time, whatType, new Point( -1, -1 ), uniqueId, "", miscDesc  );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType,int uniqueId, string dirString, string miscDesc )
			{
				_InitMsg( side, msgId, time, whatType, new Point( -1, -1 ), uniqueId, dirString, miscDesc  );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType, Point coord, int uniqueId, string miscDesc )
			{
				_InitMsg( side, msgId, time, whatType, coord, uniqueId, "", miscDesc  );
			}

			public Message( OWNER side, int msgId, int time, MessageType whatType, string miscDesc )
			{
				_InitMsg( side, msgId, time, whatType, new Point( -1, -1 ), -1, "", miscDesc );
			}

			private void	_InitMsg(  OWNER side, int msgId, int time, MessageType whatType, Point coord, int unitIndex, string dirString, string miscDesc )
			{
				messageId = msgId;
				timeStamp = time;
				msgType = whatType;
				whichSide = side;
                uniqueId = unitIndex;

				string msgDesc = msgType.ToString();
				if ( msgDesc.Length < 25 )
				{
					msgDesc += "\t";
					if ( msgDesc.Length < 15 )
					{
						msgDesc += "\t";
						if ( msgDesc.Length <= 8 )
						{
							msgDesc += "\t";
						}
					}
				}

				desc = string.Format("{0}\t{1}\t{2}", messageId, timeStamp, msgDesc );
				desc += MessageDisplay.OwnerDisplay( whichSide );
				desc += "\t";

				if ( uniqueId != -1 )
				{
					desc += uniqueId.ToString();
				}
				desc += "\t";

				if ( coord.X == -1 )
				{
					desc += "\t";
				}
				else
				{
					desc += string.Format("{0},{1}\t", coord.X, coord.Y );
				}

				_ParseFacing( dirString );

				desc += miscDesc;

				desc += "\n";
			}


			private void	_ParseFacing( string dirString )
			{
				switch ( dirString )
				{
                    case	"NORTH"			:	desc += "N ";	break;
				    case	"NORTH_EAST"	:	desc += "NE";	break;
                    case	"EAST"			:	desc += "E ";	break;
				    case	"SOUTH_EAST"	:	desc += "SE";	break;
				    case	"SOUTH"			:	desc += "S ";	break;
				    case	"SOUTH_WEST"	:	desc += "SW";	break;
				    case	"WEST"			:	desc += "W ";	break;
				    case	"NORTH_WEST"	:	desc += "NW";	break;
					default					:	desc += "  ";	break;
				}
				desc += "\t";
			}
		}

		private ArrayList			mMessageStorage = new ArrayList();
		private RichTextBox			mMessageOutput;
		private CheckBox[]			mSideChecks;
		private CheckBox[]			mMsgChecks;
		private TrackBar[]			mMessageIdRange;
		private TrackBar[]			mSecondsRange;
		private TrackBar[]			mUnitIdRange;
		private Panel				mMsgNumPanel;
		private	bool				mTrackBarValInRange, mAllowRefresh = false;
		private int					mValidTasks;
		private	int					mBaseX, mBaseY;
		private Font				mMsgFont;

		public MessageDisplay( RichTextBox outputBox, Panel msgNumPanel, CheckBox[] sideChecks, CheckBox[] msgChecks , TrackBar[] idRange , TrackBar[] timeRange, TrackBar[] unitRange )	:	base ( OWNER._size, true )
		{
			mMessageOutput = outputBox;
			mMsgNumPanel = msgNumPanel;
			mSideChecks = sideChecks;
			mMsgChecks = msgChecks;
			mMessageIdRange = idRange;
			mSecondsRange = timeRange;
			mUnitIdRange = unitRange;
			mValidTasks = msgChecks.Length;

		
			mBaseX = mMsgChecks[0].Location.X;
			mBaseY = mMsgChecks[0].Location.Y;
			mMsgFont = mMsgNumPanel.Font;

			_CheckBoxCallBacks( mSideChecks );
			_CheckBoxCallBacks( mMsgChecks );

			foreach ( TrackBar trackBar in mMessageIdRange )
			{
				trackBar.ValueChanged += new System.EventHandler( _MessageIdRefreshRequest );
			}
			foreach ( TrackBar trackBar in mSecondsRange )
			{
				trackBar.ValueChanged += new System.EventHandler( _TimeStampRefreshRequest );
			}
			foreach ( TrackBar trackBar in mUnitIdRange )
			{
				trackBar.ValueChanged += new System.EventHandler( _UnitIdRefreshRequest );
			}
			mMessageOutput.Text = "";
		}

		static	public	string	OwnerDisplay( OWNER owner )
		{
			string ownerDesc = "";
			if ( owner != OWNER.INVALID )
			{
				if ( owner == OWNER.MANAGER )
				{
					ownerDesc = "System";
				}
				else
				{
					ownerDesc = owner.ToString();
				}
			}
			return ( ownerDesc );
		}

		private void _MessageIdRefreshRequest(object sender, System.EventArgs e)
		{
			if ( mAllowRefresh )
			{	
				_SliderRangeMatch( (TrackBar)sender, mMessageIdRange );
				UpdateMessageHistory();
			}						  
		}

		private void _TimeStampRefreshRequest(object sender, System.EventArgs e)
		{
			if ( mAllowRefresh )
			{	
				_SliderRangeMatch( (TrackBar)sender, mSecondsRange );
				UpdateMessageHistory();
			}						  
		}

		private void _UnitIdRefreshRequest(object sender, System.EventArgs e)
		{
			if ( mAllowRefresh )
			{	
				_SliderRangeMatch( (TrackBar)sender, mUnitIdRange );
				UpdateMessageHistory();
			}						  
		}

		private void _SliderRangeMatch( TrackBar active, TrackBar[] minMaxPair )
		{
			mAllowRefresh = false;						//	stop recursive call backs
			int sliderVal = active.Value;				//	get value...
			if ( active == minMaxPair[0] )				//	is the 'min value' changing?
			{
				if ( sliderVal > minMaxPair[1].Value )	//	is min greater than max?
				{
					minMaxPair[1].Value = sliderVal;	//	then set 'max' to same value
				}
			}
			else
			{
				if ( sliderVal < minMaxPair[0].Value )	//	is 'max' greater than current min?
				{
					minMaxPair[0].Value = sliderVal;	//	then reset 'min' to same value
				}
			}
			mAllowRefresh = true;						//	turn call backs back on...
		}

		private void	_CheckBoxCallBacks( CheckBox[] checkBoxes )
		{
			foreach ( CheckBox checkBox in checkBoxes )
			{
				checkBox.CheckedChanged += new System.EventHandler( _UpdateMessageHistory );
			}
		}

		public	void	ToggleAllowInteraction( bool allow )
		{
			mSecondsRange[0].Enabled = mSecondsRange[1].Enabled = 
			mMessageIdRange[0].Enabled = mMessageIdRange[1].Enabled = 
			mUnitIdRange[0].Enabled = mUnitIdRange[1].Enabled = allow;
			mAllowRefresh =	allow;
			if ( allow )
			{
				_UpdateMessageHistory( null, null );
			}
		}

		public	void	UpdateMessageHistory()
		{
			int[]	numOfType = new int[mValidTasks];
			string eventSummary = "";
			foreach ( Message message in mMessageStorage )
			{
				if ( _IsValidMessage( message ) )
				{
					eventSummary += message.desc;
					++numOfType[(int)message.msgType];
				}
			}
			mMessageOutput.Text = eventSummary;
			if ( mMsgNumPanel.BackgroundImage != null )
			{
				mMsgNumPanel.BackgroundImage.Dispose();
			}
			Bitmap bmap = new Bitmap( mMsgNumPanel.Width, mMsgNumPanel.Height );
			Graphics gfx = Graphics.FromImage( bmap );
			gfx.Clear( System.Drawing.SystemColors.Control );
			int i = 0, x, y;
			for (; i < mValidTasks; ++i )
			{
				if ( numOfType[i] != 0 )
				{
					x = mMsgChecks[i].Location.X - mBaseX;
					y = mMsgChecks[i].Location.Y - mBaseY;
					gfx.DrawString( numOfType[i].ToString(), mMsgFont, Brushes.Black, x, y );
				}
			}
			mMsgNumPanel.BackgroundImage = bmap;
		}

		private void _UpdateMessageHistory(object sender, System.EventArgs e)
		{
			if ( mAllowRefresh )
			{	
				UpdateMessageHistory();
			}
		}

		delegate bool _IsValueInRangeDelegate ( int compareValue, TrackBar[] trackBars );
		
		private bool _IsValueInRange( int compareValue, TrackBar[] trackBars )
		{
			if ( trackBars[0].InvokeRequired )
			{
				trackBars[0].Invoke( new _IsValueInRangeDelegate (_IsValueInRange), new object[]{ compareValue, trackBars} );
			}
			else
			{ 
				mTrackBarValInRange = ( ( compareValue >= trackBars[0].Value ) &&
										( compareValue <= trackBars[1].Value ) );
			}
			return ( mTrackBarValInRange );	//	since I can't figure out how to return stuff from delegate calls, just use a class bool to store results of check
		}

		private bool _IsValidMessage( Message message )
		{
			bool	isValidMessage = false;						//	assume not valid by default...
			int whichSide = (int)message.whichSide;
			if ( whichSide == -1 )
			{
				whichSide = (int)OWNER._size;
			}
			if ( mSideChecks[whichSide].Checked )	//	valid 'side'?
			{
				if ( mMsgChecks[(int)message.msgType].Checked )	//	message type is wanted?
				{
					if ( ( _IsValueInRange( message.messageId, mMessageIdRange ) ) &&
					     ( _IsValueInRange( message.timeStamp, mSecondsRange ) ) &&
					     ( _IsValueInRange( message.uniqueId, mUnitIdRange ) ) )
					{
						isValidMessage = true;
					}
				}
			}
			return ( isValidMessage );
		}

		delegate void _CheckUpdateSliderDelegate ( int val, TrackBar slider, bool setVal );

		private void _CheckUpdateSlider( int val, TrackBar slider, bool setVal )
		{
			if ( slider.InvokeRequired )
			{
				slider.Invoke( new _CheckUpdateSliderDelegate (_CheckUpdateSlider), new object[]{ val, slider, setVal} );
			}
			else
			{
				slider.Maximum = val;
				if ( setVal )
				{
					slider.Value = val;
				}
			}
		}
		private void _CheckUpdateSliders( int val, TrackBar[] sliders )
		{
			if ( val > sliders[0].Maximum )
			{
				_CheckUpdateSlider( val, sliders[0], false );
				_CheckUpdateSlider( val, sliders[1], true );
			}
		}

		delegate void _AddMessageTextDelegate ( RichTextBox rtb, string msgToAdd );

		private void    _AddMessageText( RichTextBox rtb, string msgToAdd )
		{
			if ( rtb.InvokeRequired )
			{
				rtb.Invoke( new _AddMessageTextDelegate (_AddMessageText), new object[]{ rtb, msgToAdd } );
			}
			else
			{ 
				rtb.Text += msgToAdd;
			}
		}

		private	void	_AddMessage( Message message )
		{
			mMessageStorage.Add( message );

			_CheckUpdateSliders( message.timeStamp, mSecondsRange );
			_CheckUpdateSliders( message.messageId, mMessageIdRange );
			_CheckUpdateSliders( message.uniqueId, mUnitIdRange );

			if ( _IsValidMessage( message ) )
			{
				_AddMessageText( mMessageOutput, message.desc );
			}
		}

		public	override	void	MsgRenderMap( int msgTime )
		{
			_AddMessage( new Message( OWNER.MANAGER, mMessageStorage.Count, msgTime, MessageType.RenderMap ) );
		}

		public	override	void	MsgInitGameSide( int msgTime, OWNER whichSide ) 
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.InitGameSide ) );
		}

		public	override	void	MsgGameTurnInit( int msgTime, OWNER whichSide, int turnNumber )
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.GameTurnInit, string.Format("Turn # {0}", turnNumber ) ) );
		}

		public	override	void	MsgPickRoadWay( int msgTime, OWNER whichSide ) 
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.PickRoadWay ) );
		}

		public	override	void	MsgPickSettlement( int msgTime, OWNER whichSide ) 
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.PickSettlement ) );
		}

		public	override	void	MsgAnimateStart( int msgTime, OWNER whoFor ) 
		{
			_AddMessage( new Message( OWNER.MANAGER, mMessageStorage.Count, msgTime, MessageType.AnimateStart, whoFor.ToString() ) );
		}

		public	override	void	MsgAddStartResources( int msgTime, OWNER whoFor )
		{
			_AddMessage( new Message( whoFor, mMessageStorage.Count, msgTime, MessageType.AddStartResources, whoFor.ToString() ) );
		}

		public	override	void	MsgAnimateUpdate( int msgTime, bool wantThinkingGfx )
		{
			string gfxDesc = "Hide Gfx";
			if ( wantThinkingGfx ) { gfxDesc = "SHOW Gfx"; }
			_AddMessage( new Message( OWNER.MANAGER, mMessageStorage.Count, msgTime, MessageType.AnimateUpdate, gfxDesc ) );
		}
		public	override	void	MsgAnimateFinish( int msgTime )
		{
			_AddMessage( new Message( OWNER.MANAGER, mMessageStorage.Count, msgTime, MessageType.AnimateFinish ) );
		}

		public	override	void	MsgLogicStateRequest( int msgTime, OWNER sender, SideLogic.LOGIC_STATE stateEnum )
		{
			_AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.LogicStateRequest, stateEnum.ToString() ) );
		}

		public	override	void	MsgResourceUpdate( int msgTime, OWNER sender, RESOURCE resource, int quantityMod )
		{
			string signDec = "";	//	assume negative by default
			if ( quantityMod >= 1 )	{ signDec = "+"; }
			_AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.ResourceUpdate, string.Format("{0} {1}{2}",  resource.ToString(), signDec, quantityMod ) ) );
		}

		public	override	void	MsgRandomNumSeed( int msgTime, int miscVal )
		{
			_AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.RandomNumSeed, miscVal.ToString() ) );
		}

		private void _InitMsgParse( int msgTime, bool wantRandom, MessageType msgType, string regDesc, string randomDesc )
		{
			string miscDesc = regDesc;
			if ( wantRandom )
			{
				miscDesc = randomDesc;
			}
			_AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, msgType, miscDesc ) );
		}

		public override void	MsgInitPortLocRequest( int msgTime, bool wantRandom )
		{
			_InitMsgParse( msgTime, wantRandom, MessageType.InitPortLocRequest, "Default", "Randomized" );
		}

		public override void	MsgInitTerrainRequest( int msgTime, bool wantRandom )
		{
			_InitMsgParse( msgTime, wantRandom, MessageType.InitTerrainRequest, "Beginner Map", "Randomized" );
		}

		public override void	MsgInitDieRollRequest( int msgTime, bool wantRandom )
		{
			_InitMsgParse( msgTime, wantRandom, MessageType.InitDieRollRequest, "Default", "Randomized" );
		}

		public	override	void	MsgInitDieRollSet( int msgTime, int uniqueId, int dieRoll )
		{
			_AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitDieRollSet, uniqueId, dieRoll.ToString() ) );
		}

		public	override	void	MsgInitTerrainSet( int msgTime, int uniqueId, TERRAIN terrain )
		{
			_AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitTerrainSet, uniqueId, terrain.ToString() ) );
		}

		public	override	void	MsgInitPortLocSet( int msgTime, PORT portId, RESOURCE portResource )
		{
			string resDesc = portResource.ToString();
			if ( portResource == RESOURCE._size )
			{
				resDesc = "3-1 ?";
			}
			resDesc = string.Format("Port {0} : {1}", (int)portId, resDesc );
			_AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitPortLocSet, resDesc ) );
		}

		public	override	void	MsgAddRoadWay( int msgTime, OWNER whichSide, int settlementId, CITY_DIR whichDir )
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.AddRoadWay, -1, whichDir.ToString(), string.Format("LocId {0}", settlementId ) ) );
		}


		public	override	void	MsgAddSettlement( int msgTime, OWNER whichSide, int settlementId, int numActive )
		{
			_AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.AddSettlement, string.Format("LocId {0} : # {1}", settlementId, numActive ) ) );
		}

		public	override	void	MsgMessageHandled( int msgTime, OWNER sender, MessageType whichMessage, int miscVal )
		{
			string miscDesc = "";
			if ( miscVal != 0 )
			{
				miscDesc = miscVal.ToString();
			}
			_AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.MessageHandled, whichMessage.ToString() ) );
		}

		public	override	void	MsgStateRequest( int msgTime, OWNER sender, PlayGameMgr.STATE whichState, int settlementId, int miscVal )
		{
			string settlementDesc = "";
			if ( settlementId != -1 )
			{
				settlementDesc = string.Format( "[{0}]", settlementId.ToString() );
			}
			string miscDesc = "";
			if ( miscVal != 0 )
			{
				miscDesc = string.Format( "({0})", miscVal.ToString() );
			}
			_AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.StateRequest, string.Format("{0}{1}{2}", whichState.ToString(), settlementDesc, miscDesc ) ) );
		}

	}
}
