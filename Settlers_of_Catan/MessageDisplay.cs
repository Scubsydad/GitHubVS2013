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
		private	bool				mAllowRefresh = false;
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
					if ( ( message.messageId >= mMessageIdRange[0].Value ) &&
						 ( message.messageId <= mMessageIdRange[1].Value ) )
					{
						if ( ( message.timeStamp >= mSecondsRange[0].Value ) &&
							 ( message.timeStamp <= mSecondsRange[1].Value ) )
						{
							if ( ( message.uniqueId >= mUnitIdRange[0].Value ) &&
								 ( message.uniqueId <= mUnitIdRange[1].Value ) )
							{
								isValidMessage = true;
							}
						}
					}
				}
			}
			return ( isValidMessage );
		}

		private void _CheckUpdateSliders( int val, TrackBar[] sliders )
		{
			if ( val > sliders[0].Maximum )
			{
				lock ( sliders[0] )
				{
					sliders[0].Maximum = val;
				}
				lock ( sliders[1] )
				{
					sliders[1].Maximum = val;	//	must set maximum manually first, to ensure 'value' is in range below
					sliders[1].Value = val;
				}
			}
		}

		private	void	__AddMessage( Message message )
		{
			mMessageStorage.Add( message );

			_CheckUpdateSliders( message.timeStamp, mSecondsRange );
			_CheckUpdateSliders( message.messageId, mMessageIdRange );
			_CheckUpdateSliders( message.uniqueId, mUnitIdRange );

			if ( _IsValidMessage( message ) )
			{
				lock ( mMessageOutput )
				{
					mMessageOutput.Text += message.desc;
				}
			}
		}


		//public	override	void	MsgRender( int msgTime )
		//{
		//	__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.Render ) );
		//}

		//public	override	void	MsgCoordOccupied( int msgTime, OWNER side, Point coord, int uniqueId )
		//{
		//	__AddMessage( new Message( side, mMessageStorage.Count, msgTime, MessageType.CoordOccupied, coord, string.Format("Unit : {0}", uniqueId ) ) );
		//}

		public	override	void	MsgRenderMap( int msgTime )
		{
			__AddMessage( new Message( OWNER.MANAGER, mMessageStorage.Count, msgTime, MessageType.RenderMap ) );
		}

		public	override	void	MsgInitGameSide( int msgTime, OWNER whichSide ) 
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.InitGameSide ) );
		}

		public	override	void	MsgGameTurnInit( int msgTime, OWNER whichSide, int turnNumber )
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.GameTurnInit, string.Format("Turn # {0}", turnNumber ) ) );
		}

		public	override	void	MsgPickRoadWay( int msgTime, OWNER whichSide ) 
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.PickRoadWay ) );
		}

		public	override	void	MsgPickSettlement( int msgTime, OWNER whichSide ) 
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.PickSettlement ) );
		}

		public	override	void	MsgRandomNumSeed( int msgTime, int miscVal )
		{
			__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.RandomNumSeed, miscVal.ToString() ) );
		}

		private void _InitMsgParse( int msgTime, bool wantRandom, MessageType msgType, string regDesc, string randomDesc )
		{
			string miscDesc = regDesc;
			if ( wantRandom )
			{
				miscDesc = randomDesc;
			}
			__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, msgType, miscDesc ) );
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
			__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitDieRollSet, uniqueId, dieRoll.ToString() ) );
		}

		public	override	void	MsgInitTerrainSet( int msgTime, int uniqueId, TERRAIN terrain )
		{
			__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitTerrainSet, uniqueId, terrain.ToString() ) );
		}

		public	override	void	MsgInitPortLocSet( int msgTime, PORT portId, RESOURCE portResource )
		{
			string resDesc = portResource.ToString();
			if ( portResource == RESOURCE._size )
			{
				resDesc = "3-1 ?";
			}
			resDesc = string.Format("Port {0} : {1}", (int)portId, resDesc );
			__AddMessage( new Message( OWNER.INVALID, mMessageStorage.Count, msgTime, MessageType.InitPortLocSet, resDesc ) );
		}

		public	override	void	MsgAddRoadWay( int msgTime, OWNER whichSide, int settlementId, CITY_DIR whichDir )
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.AddRoadWay, -1, whichDir.ToString(), string.Format("LocId {0}", settlementId ) ) );
		}


		public	override	void	MsgAddSettlement( int msgTime, OWNER whichSide, int settlementId, int numActive )
		{
			__AddMessage( new Message( whichSide, mMessageStorage.Count, msgTime, MessageType.AddSettlement, string.Format("LocId {0} : # {1}", settlementId, numActive ) ) );
		}

		public	override	void	MsgMessageHandled( int msgTime, OWNER sender, MessageType whichMessage, int miscVal )
		{
			string miscDesc = "";
			if ( miscVal != 0 )
			{
				miscDesc = miscVal.ToString();
			}
			__AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.MessageHandled, whichMessage.ToString() ) );
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
			__AddMessage( new Message( sender, mMessageStorage.Count, msgTime, MessageType.StateRequest, string.Format("{0}{1}{2}", whichState.ToString(), settlementDesc, miscDesc ) ) );
		}

	}
}
