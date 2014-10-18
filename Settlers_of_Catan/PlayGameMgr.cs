using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for PlayGameMgr.
	/// </summary>
	public class PlayGameMgr	:	MessageHandler
	{
		public enum STATE
		{
			DORMANT,
			PICK_BUILD_HEX,
			PICK_BUILD_CORNER,
			PICK_ROAD_WAY_LOC,
			_size
		};

		SideLogic[]		mPlayerLogics;
		MessageCenter	mMessageCtr;
		int				mTurnNumber = 0, mNumPlayers, mHexNumChosen, mSettlementBuildIndex, mSettlementId;
		int[]			mClickLocRadius;
		Point[]			mClickLocCenterPoint;
		ArrayList		mTabStorage = new ArrayList(), mPlacementOrder = new ArrayList(), mTurnOrder = new ArrayList();
		PictureBox		mPlayGamePictBox;
		MapManager		mMapManager;
		STATE			mCurrentState = STATE.DORMANT;
		OWNER			mStateOwner = OWNER.INVALID;
		Color[]			mHexBuildHelpColors = new Color[] { Color.Chartreuse, Color.YellowGreen, Color.Yellow, Color.Orange, Color.Red };
		HexInfo[]		mGfxLocHexArray = Support.GetHexInfoCopy();
		TabControl		mStateExplainTabs;
		Point[][]		mBuilLocs = new Point[19][];
		Point[][]		mHexDrawGfxLocs = new Point[19][];
		PathFinder		mPathFinder;

		public PlayGameMgr( MessageCenter msgCenter, int numPlayers, CONTROL[] control, MapManager mapMgr, PictureBox pictBox, TabControl stateExplainTabs )	:	base ( OWNER.MANAGER, false )
		{
			mMessageCtr = msgCenter;
			mNumPlayers = numPlayers;
			mPlayerLogics = new SideLogic[mNumPlayers];
			mMapManager = mapMgr;
			mPlayGamePictBox = pictBox;
			mPlayGamePictBox.MouseDown += _PlayGamePictMouseDown;
			mStateExplainTabs = stateExplainTabs;
			mPathFinder = new PathFinder( mGfxLocHexArray );	//	this initializes default location tracking based on map (which will be shared across all logics moving forwards)

			int j, i = 0;
			for ( ; i < mNumPlayers; ++i )
			{
				mPlayerLogics[i] = new SideLogic( (OWNER)i, ( control[i] == CONTROL.USER ), mNumPlayers, mMessageCtr, mPathFinder );
			}
			int		nunOrigTabPages = mStateExplainTabs.TabCount;
			TabPage tabPage;
			for ( i = 0; i < nunOrigTabPages; ++i )
			{
				tabPage = mStateExplainTabs.TabPages[0];
				tabPage.Text = (string)tabPage.Tag;
				mTabStorage.Add( tabPage );
				mStateExplainTabs.TabPages.Remove( tabPage );
			}
			Point[]		drawArray, buildArray;
			Size		bmapSize = new Size(680,680);
			Point		buildLoc;
			int			xMod = 0, yMod = 0;
			CITY_DIR	cityDir;
			for ( i = 0; i < mGfxLocHexArray.Length; ++i )
			{
				drawArray = new Point[(int)CITY_DIR._size];
				buildArray = new Point[(int)CITY_DIR._size];
				for ( j = 0; j < (int)CITY_DIR._size; ++j )
				{
					cityDir =  (CITY_DIR)j;
					buildLoc = mGfxLocHexArray[i].GetBuildLoc( cityDir ).GetSettlementLoc().GetSettlementGfxLoc( bmapSize, i );
					buildArray[j] = buildLoc;
					switch ( cityDir )
					{
						case	CITY_DIR.NORTH_WEST	:	xMod = 3;	yMod = 3;	break;
						case	CITY_DIR.NORTH_EAST	:	xMod = -3;	yMod = 3;	break;
						case	CITY_DIR.EAST		:	xMod = -3;	yMod = 0;	break;
						case	CITY_DIR.SOUTH_EAST	:	xMod = -3;	yMod = -3;	break;
						case	CITY_DIR.SOUTH_WEST	:	xMod = 3;	yMod = -3;	break;
						default /*CITY_DIR.WEST*/	:	xMod = 3;	yMod = 0;	break;
					}
					drawArray[j] = new Point( ( buildLoc.X + xMod ), ( buildLoc.Y + yMod ) );
				}
				mBuilLocs[i] = buildArray;
				mHexDrawGfxLocs[i] = drawArray;

			}
		}

		void _SetState( STATE newState, OWNER whichOwner )
		{
			if ( mStateExplainTabs.TabCount != 0 )
			{
				mStateExplainTabs.TabPages.RemoveAt( 0 );
			}
			mCurrentState = newState;
			mStateOwner = whichOwner;
			if ( mCurrentState != STATE.DORMANT )
			{
				mStateExplainTabs.TabPages.Add( (TabPage)mTabStorage[(int)mCurrentState] );
			}
			else
			{
				mClickLocCenterPoint = null;	//	invalidate these when we go into dormant state so they aren't left dangling
				mClickLocRadius = null;
			}
		}

		public PlayGameMgr ShutDown()
		{
			int i = 0;
			for ( ; i < mNumPlayers; ++i )
			{
				mPlayerLogics[i] = null;
			}
			if ( mStateExplainTabs.TabPages.Count != 0 )
			{
				 mStateExplainTabs.TabPages.RemoveAt( 0 );
			}
			int		nunOrigTabPages = mTabStorage.Count;
			for ( i = 0; i < nunOrigTabPages; ++i )
			{
				mStateExplainTabs.TabPages.Add( (TabPage)mTabStorage[i] );
			}
			return ( null );
		}

		private Bitmap	_DoMapDraw()
		{
			Bitmap			bmap = mMapManager.GenerateMapGfx();
			ArrayList		settlementsDrawn = new ArrayList();
			ArrayList		roadWaysDrawn = new ArrayList();
			int				settlementId, indexFound, dirLoop;
			HexInfo[]		hexInfos = mPlayerLogics[(int)OWNER.BLUE].GetHexInfoArray();	//	every side should have their own copy, but just grab blues
			SettlementLoc	settlementLoc;
			CITY_DIR		dirEnum;
			OWNER			roadOwner;
			Point			adjRoadPoint, roadPoint, gfxLoc, offset1, offset2;
			Size			bmapSize = bmap.Size;
			Graphics		gfx = Graphics.FromImage( bmap );

			foreach ( HexInfo hexInfo in hexInfos )
			{
				foreach ( BuildLoc buildLoc in hexInfo.GetBuildLocs() )
				{
					settlementLoc = buildLoc.GetSettlementLoc();
					settlementId = settlementLoc.GetUniqueId();
					if ( buildLoc.IsOccupied() )
					{
						indexFound = settlementsDrawn.IndexOf( settlementId );
						if ( indexFound == -1 )
						{
							settlementsDrawn.Add( settlementId );
							settlementLoc.DrawSettlement( bmap, buildLoc.GetOwner() );
						}
					}
					gfxLoc = settlementLoc.GetSettlementGfxLoc( bmapSize );	//	grab the center point of the settlement, so we can offset the road graphics
					for ( dirLoop = 0; dirLoop < (int)CITY_DIR._size; ++dirLoop )
					{
						dirEnum = (CITY_DIR)dirLoop;
						roadOwner = buildLoc.GetRoadWayOwner( dirEnum );
						if ( roadOwner != OWNER.INVALID )
						{
							roadPoint = new Point( settlementId, dirLoop );	//	build a point based on settlement id & direction
							indexFound = roadWaysDrawn.IndexOf( roadPoint );//	see if roadway already drawn by other hex/settlement link?
							if ( indexFound == -1 )							//	if not, we'll draw it and track it
							{
								roadWaysDrawn.Add( roadPoint );				//	track so we know we've done it
								adjRoadPoint =  new Point( settlementLoc.GetAdjacentLocId( dirEnum ), (int)Support.GetOppositeCityDir( dirEnum ) );
								roadWaysDrawn.Add( adjRoadPoint );			//	flag the adjacent location as drawn, so we don't draw the same road 'back to us'
								offset1 = new Point( 0, 0 );
								offset2 = new Point( 0, 0 );
								switch ( dirEnum )
								{
									case	CITY_DIR.NORTH_WEST	:	offset1 = new Point(  -5, -12 );	offset2 = new Point( -25, -42 ); break;
									case	CITY_DIR.NORTH_EAST	:	offset1 = new Point( +13, -19 );	offset2 = new Point( +33, -49 ); break;	
									case	CITY_DIR.EAST		:	offset1 = new Point( +20,   0 );	offset2 = new Point( +60,   0 ); break;
									case	CITY_DIR.SOUTH_EAST	:	offset1 = new Point( +15, +22 );	offset2 = new Point( +35, +52 ); break;		
									case	CITY_DIR.SOUTH_WEST	:	offset1 = new Point(  -4, +10 );	offset2 = new Point( -24, +40 ); break;
									default /*CITY_DIR.WEST*/	:	offset1 = new Point( -11,   0 );	offset2 = new Point( -51,   0 ); break;	
								}
								offset1 = new Point( ( gfxLoc.X + offset1.X ), ( gfxLoc.Y + offset1.Y ) );
								offset2 = new Point( ( gfxLoc.X + offset2.X ), ( gfxLoc.Y + offset2.Y ) );
								gfx.DrawLine( new Pen( Support.GetSideColor( roadOwner ), 7 ), offset1, offset2 );
							}
						}
					}
				}
			}
			mPlayGamePictBox.Image = bmap;

			return ( bmap );
		}

		public	override	void	MsgRenderMap( int msgTime )
		{
			_DoMapDraw();
		}

		private void    _AddPlayerTurnOrder( OWNER whichOwner, bool[] tracking )
		{
			mTurnOrder.Add( whichOwner );
			tracking[(int)whichOwner] = true;
		}

		public	void	InitTurnOrder( OWNER firstPlayer, bool wantRandomizedOrder )
		{
			bool[]	usedPlayer = new bool[mNumPlayers];
			_AddPlayerTurnOrder( firstPlayer, usedPlayer );
			int i = 1, index;
			if ( wantRandomizedOrder )
			{
				for ( ; i < mNumPlayers; ++i )
				{
					do
					{
						index = Support.GetRand( mNumPlayers );
					} while ( usedPlayer[index] );
					_AddPlayerTurnOrder( (OWNER)index, usedPlayer );
				}
			}
			else
			{
				index = (int)firstPlayer + 1;
				for ( ; i < mNumPlayers; ++i, ++index )
				{
					if ( index == mNumPlayers )
					{
						index = 0;
					}
					_AddPlayerTurnOrder( (OWNER)index, usedPlayer );
				}
			}
// prepare for game, choose list of settlements to select from
			for ( i = 0; i < mNumPlayers; ++i )
			{
				mMessageCtr.SendMsgInitGameSide( (OWNER)mTurnOrder[i] );
			}
// players first available settlement in default turn order
			for ( i = 0; i < mNumPlayers; ++i )
			{
				mPlacementOrder.Add( (OWNER)mTurnOrder[i] );
			}
// players pick second initial settlement in REVERSE default turn order
			for ( i = ( mNumPlayers - 1 ); i >= 0 ; --i )
			{
				mPlacementOrder.Add( (OWNER)mTurnOrder[i] );
			}

			_SendPickSettlementMessage();
		}

		public	void	PauseGameRequest()
		{
		}

		private void _RoadWayPlacementUpdate( OWNER whichSide )
		{
// once a settlement AND a roadway are placed for a side, this is 'one cycle' per side, so advance the placement owner list
			if ( whichSide != OWNER.INVALID )
			{
				mMessageCtr.SendMsgRenderMap( );	//	render the map to reflect the last choice made by settlement placement

				OWNER firstInQueue = (OWNER)mPlacementOrder[0];
Debug.Assert( firstInQueue == whichSide );				//	always ensure the first one in the queue is who we want to hear back from
				mPlacementOrder.RemoveAt( 0 );			//	remove the first item in the list one at a time over 8 calls
			}
			if ( mPlacementOrder.Count != 0 )			//	do we still have more settlement placements to go through?
			{
				_SendPickSettlementMessage();
			}
			else
			{
				mMessageCtr.SendMsgGameTurnInit( OWNER.MANAGER, mTurnNumber );
			}
		}

		private void _SendPickSettlementMessage()
		{
			OWNER whichSide = (OWNER)mPlacementOrder[0];	//	grab next side that is supposed to pick their settlement
			mMessageCtr.SendMsgPickSettlement( whichSide );	//	each settlement placed should fire off the next guy in line
		}

		public	override	void	MsgMessageHandled( int msgTime, OWNER sender, MessageType whichMessage, int miscVal )
		{
			if ( whichMessage == MessageType.PickSettlement )		{	mMessageCtr.SendMsgPickRoadWay( sender );	}
			else if ( whichMessage == MessageType.PickRoadWay )		{	_RoadWayPlacementUpdate( sender );			}
			else if ( whichMessage == MessageType.GameTurnInit )	{	_GameTurnInitUpdate( sender, miscVal );		}
		}

		private void        _NewStateChangeRequest( PlayGameMgr.STATE whichState, OWNER forSide, int settlementId, int miscVal )
		{
			string		explainMsg = "", miscStr1 = "", headerMsg = string.Format( "Attention {0}!", forSide.ToString() ) ;
			Bitmap		bmap = _DoMapDraw();
			Size		bmapSize = bmap.Size;
			Graphics	gfx = Graphics.FromImage( bmap );
			SideLogic	sideLogic = mPlayerLogics[(int)forSide];
			bool		allowStateChange = true;		//	assume yes by default
			Pen			drawPen = new Pen( Support.GetSideColor( forSide ), 3 );

			if ( whichState == STATE.PICK_BUILD_HEX )
			{
				mHexNumChosen = -1;
				miscStr1 = "first";
				mSettlementBuildIndex = miscVal;
				if ( mSettlementBuildIndex == 2 )
				{
					miscStr1 = "second";
				}
				explainMsg = string.Format( "Attention, it is now your turn to place your {0} settlement.", miscStr1 );

				Point[]			topCandidatesList = sideLogic.GetSettlementTopChoices( mHexBuildHelpColors.Length );
				int				candidateLoop;
				Point[]			gfxLocs = new Point[(int)CITY_DIR._size];

// highlight coordinates in reverse order, so the highest quality outline draws overtop of the lowest, if conflict
				for ( candidateLoop = ( topCandidatesList.Length - 1); candidateLoop >= 0; --candidateLoop )
				{
					gfx.DrawPolygon( new Pen( mHexBuildHelpColors[candidateLoop], 5 ), mHexDrawGfxLocs[topCandidatesList[candidateLoop].Y] );
				}
			}
			else if ( whichState == STATE.PICK_BUILD_CORNER )
			{
				mHexNumChosen = miscVal;
				int[]	cityDirVals = new int[(int)CITY_DIR._size];
				CITY_DIR bestCorner = sideLogic.GetSettlementLoc( mHexNumChosen, null, cityDirVals );
				if ( bestCorner == CITY_DIR.INVALID )
				{
					MessageBox.Show( "This hex has no allowable build locations to use.", "Attention!" );
					_NewStateChangeRequest( STATE.PICK_BUILD_HEX, forSide, -1, mSettlementBuildIndex );	//	pick a new hex...
					allowStateChange = false;
				}
				else
				{
					int		pct, halfSize, elipseSize, largestVal = cityDirVals[(int)bestCorner];
					Point	gfxLoc;
					mClickLocRadius = new int[(int)CITY_DIR._size];
					for ( int i = 0; i < (int)CITY_DIR._size; ++i )
					{
						pct = ( ( cityDirVals[i] * 100 ) / largestVal );		//	convert to percentage of max
						elipseSize = ( ( 30 * pct ) / 100 );					//	then convert to percentage of 30
						if ( elipseSize >= 4 )
						{
							if ( elipseSize < 8 ) { elipseSize = 8; }			//	make circles a minimize size for legibility	
							halfSize = ( elipseSize / 2 );						//	get half of size to center point it
							gfxLoc = mBuilLocs[mHexNumChosen][i];				//	get graphics loc center point
							gfx.DrawEllipse( drawPen, ( gfxLoc.X - halfSize ),
													  ( gfxLoc.Y - halfSize ),
													  elipseSize, elipseSize );	//	track how big the click radius is for selection purposes
							mClickLocRadius[i] = halfSize;
						}

					}
				}
			}
			else if ( whichState == STATE.PICK_ROAD_WAY_LOC )
			{
				if ( settlementId != -1 )
				{
					mSettlementId = settlementId;
					SettlementLoc	adjLoc, settlementLoc = MapDefs.GetSettlementLoc( mSettlementId );
					int				numAdjSettlements = settlementLoc.GetNumAdjacentLocIds();
					int				i = 0, adjSettlementId, radius = 18;
					CITY_DIR		adjRoadDir;
					Point			gfxPoint, settlementGfxLoc = settlementLoc.GetSettlementGfxLoc(bmapSize ), adjGfxLoc;
// define 6 adjacent click locs/radia, but settlements will have max of 3. Use active index to determine clicked 'CITY_DIR'
					mClickLocCenterPoint = new Point[(int)CITY_DIR._size];
					mClickLocRadius = new int[(int)CITY_DIR._size];

					for ( ; i < numAdjSettlements; ++i )
					{
						adjSettlementId = settlementLoc.GetAdjacentLocId( i, out adjRoadDir );
						adjLoc = MapDefs.GetSettlementLoc( adjSettlementId );
						adjGfxLoc = adjLoc.GetSettlementGfxLoc( bmapSize );
						gfxPoint = new Point( ( ( settlementGfxLoc.X + adjGfxLoc.X ) / 2 ), ( ( settlementGfxLoc.Y + adjGfxLoc.Y ) / 2 ) );
						gfx.DrawEllipse( drawPen, ( gfxPoint.X - radius ),
													( gfxPoint.Y - radius ),
													radius * 2, radius * 2 );	

						mClickLocCenterPoint[(int)adjRoadDir] = gfxPoint;
						mClickLocRadius[(int)adjRoadDir]  = radius;
					}
					if ( miscVal == 0 )	//	is this the first time we are getting a plot road request?
					{
						explainMsg = "It is now time for you to place your first roadway.\n\nChoose from the available highlighted locations on the map.";
					}
				}
				else
				{
Debug.Assert( false ); // we need a solution to plot all 'roadway ends' in the future
				}
			}
			if ( explainMsg != "" )
			{
				MessageBox.Show(explainMsg, headerMsg );
			}
			if ( allowStateChange )					//	we might have to transition out of a state request in some instances
			{
				_SetState( whichState, forSide );	//	so ensure we get confirmation before we change
			}
		}

		public	override	void	MsgStateRequest( int msgTime, OWNER sender, PlayGameMgr.STATE whichState, int settlementId, int miscVal )
		{
			_NewStateChangeRequest( whichState, sender, settlementId, miscVal );
		}

		private void _MouseClickPicHex( int mouseX, int mouseY )
		{
			Point[]			pointArray;
			for ( int i = 0; i < mGfxLocHexArray.Length; ++i )
			{
				pointArray = mHexDrawGfxLocs[i];	// use the draw locs rather than the build locs to reduce the click size
				if ( ( mouseX >= pointArray[(int)CITY_DIR.NORTH_WEST].X ) &&	//	left edge...
					 ( mouseX <= pointArray[(int)CITY_DIR.NORTH_EAST].X ) &&	//	right edge...
					 ( mouseY >= pointArray[(int)CITY_DIR.NORTH_WEST].Y ) &&	//	top edge
					 ( mouseY <= pointArray[(int)CITY_DIR.SOUTH_EAST].Y ) )		//	bottom edge
				{
					HexInfo hexInfo = mGfxLocHexArray[i];
					if ( Support.IsQuestionTrue( string.Format("Are you sure you want to choose hex # {0}?\n\nTerrain\t : {1}\nResource\t : {2}\nDie Roll\t : {3}", i, hexInfo.GetTerrain().ToString(), hexInfo.GetEarnResource().ToString(), hexInfo.GetDieRoll() ), "Please Confirm" ) )
					{
						_NewStateChangeRequest( STATE.PICK_BUILD_CORNER, mStateOwner, -1, i );
						break;
					}
				}
			}
		}
		private void _MouseClickPickCorner( int mouseX, int mouseY )
		{
			Point		clickLoc;
			int			xDelta, yDelta, dist;
			CITY_DIR	clickDir = CITY_DIR.INVALID;
			for ( int i = 0; i < (int)CITY_DIR._size; ++i )
			{
				clickLoc = mBuilLocs[mHexNumChosen][i];
				xDelta = ( clickLoc.X - mouseX );
				yDelta = ( clickLoc.Y - mouseY );
				dist = (int)Math.Sqrt( (double)(( xDelta * xDelta ) + ( yDelta * yDelta )) );
				if ( dist <= mClickLocRadius[i] )
				{
					clickDir = (CITY_DIR)i;
					break;
				}
			}
			if ( clickDir != CITY_DIR.INVALID )
			{
				if ( Support.IsQuestionTrue( string.Format("Please confirm you wish to place your settlement in the\n{0} corner of hex # {1}?", clickDir.ToString(), mHexNumChosen ), "Please Confirm" ) )
				{
					OWNER currOwner = mStateOwner;				//	back up the owner before we erase the state
					_SetState( STATE.DORMANT, OWNER.INVALID );	//	disable the state BEFORE we send the message, or we'll lose the results of the 'confirm settlement' call
					mPlayerLogics[(int)currOwner].ConfirmSettlement( mHexNumChosen, clickDir );
				}
			}
		}
		private void _MouseClickPickRoad( int mouseX, int mouseY )
		{
			Point		clickLoc;
			int			xDelta, yDelta, dist;
			CITY_DIR	clickDir = CITY_DIR.INVALID;
			for ( int i = 0; i < (int)CITY_DIR._size; ++i )
			{
				clickLoc = mClickLocCenterPoint[i];
				xDelta = ( clickLoc.X - mouseX );
				yDelta = ( clickLoc.Y - mouseY );
				dist = (int)Math.Sqrt( (double)(( xDelta * xDelta ) + ( yDelta * yDelta )) );
				if ( dist <= mClickLocRadius[i] )
				{
					clickDir = (CITY_DIR)i;
					break;
				}
			}
			if ( clickDir != CITY_DIR.INVALID )
			{
				if ( Support.IsQuestionTrue( string.Format("Please confirm you wish to place your ROAD off of\nSettlement ID {0} going in the {1} direction?", mSettlementId, clickDir.ToString() ), "Please Confirm" ) )
				{
					OWNER currOwner = mStateOwner;				//	back up the owner before we erase the state
						_SetState( STATE.DORMANT, OWNER.INVALID );	//	disable the state BEFORE we send the message, or we'll lose the results of the 'confirm settlement' call
						mPlayerLogics[(int)currOwner].ConfirmRoadway( mSettlementId, clickDir );
				}
			}
		}

		void _PlayGamePictMouseDown(object sender, MouseEventArgs e)
		{
			switch ( mCurrentState )
			{
				case	STATE.PICK_BUILD_HEX		:	_MouseClickPicHex( e.X, e.Y );		break;
				case	STATE.PICK_BUILD_CORNER		:	_MouseClickPickCorner( e.X, e.Y );	break;
				case	STATE.PICK_ROAD_WAY_LOC		:	_MouseClickPickRoad( e.X, e.Y );	break;
				default								:	Debug.Assert( false );				break;
			}
		}

		public	override	void	MsgGameTurnInit( int msgTime, OWNER whichSide, int turnNumber )
		{
			mMessageCtr.SendMsgGameTurnInit( (OWNER)mTurnOrder[0], turnNumber );	//	fire off the first turn init message for the side who goes first
		}

		private	void	_GameTurnInitUpdate( OWNER whichSide, int turnNumber )
		{
			int		playerIndex = mTurnOrder.IndexOf( whichSide );
Debug.Assert( playerIndex != -1 );																	//	ensure we got a valid index out of above array
Debug.Assert( turnNumber == mTurnNumber );
			if ( ++playerIndex == mNumPlayers )				//	was that the final player that needs to call back and confirm they've initialized their turn?
			{
				++playerIndex;
			}
			else
			{
				mMessageCtr.SendMsgGameTurnInit( (OWNER)mTurnOrder[playerIndex], turnNumber );		//	daisy chain each init based on completion of previous player
			}		
		}

	}
}
