using System;
using System.Drawing;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for MessageHandler.
	/// </summary>
	public class MessageHandler
	{
		private	MessageCenter			mMsgCenter;
		private	OWNER					mPlayer;
		private int						mCurrMsgId;
		private bool					mAssertIfNotHandled;

		public MessageHandler( OWNER whichSide, bool assertIfNotHandled )
		{
			mPlayer = whichSide;
			mAssertIfNotHandled = assertIfNotHandled;

			mMsgCenter = Support.MessageCenter();
			if ( mMsgCenter != null )
			{
				mMsgCenter.RequestMessageNotification( new MessageBroadcastHandler( ReceiveMessage ) );
			}
		}

		private OWNER	 _GetMessageSender( )
		{
			return (  (OWNER)mMsgCenter.GetMessageData( mCurrMsgId, MsgParam.SenderSide ) );
		}

		private OWNER	 _GetMessageOwner( )
		{
			return (  (OWNER)mMsgCenter.GetMessageData( mCurrMsgId, MsgParam.WhichSide ) );
		}

		private int	 _GetMessageData( MsgParam msgParam )
		{
			return (  mMsgCenter.GetMessageData( mCurrMsgId, msgParam ) );
		}

		public	void ReceiveMessage( MessageType msgType, int uniqueMsgId )
		{
			OWNER whichSide = mMsgCenter.GetMessageSide( uniqueMsgId );		//	get which side the message is for...
			if ( ( whichSide == OWNER._size ) ||							//	if message is for all listeners
				 ( mPlayer == OWNER._size ) ||								//	if class listens to ALL messages ('MessageHistory' class...)
				 ( whichSide == mPlayer ) )									//	or is expected to listen to messages from SPECIFIC side...
			{
				mCurrMsgId = uniqueMsgId;
				int		timeStamp = _GetMessageData( MsgParam.TimeStamp );

				switch ( msgType )												//	don't use 'mPlayer' below, use 'whichSide'
				{
					case	MessageType.RenderMap			:	MsgRenderMap( timeStamp );												break;

					case	MessageType.AddRoadWay			:	MsgAddRoadWay( timeStamp, _GetMessageSender(), _GetMessageData( MsgParam.UniqueId ), (CITY_DIR)_GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.AnimateStart		:	MsgAnimateStart( timeStamp, (OWNER)_GetMessageData( MsgParam.MiscVal ) );		break;
					case	MessageType.AnimateUpdate		:	MsgAnimateUpdate( timeStamp, ( _GetMessageData( MsgParam.MiscVal ) == 1 ) );	break;
					case	MessageType.AnimateFinish		:	MsgAnimateFinish( timeStamp );													break;

					case	MessageType.AddSettlement		:	MsgAddSettlement( timeStamp, _GetMessageSender(), _GetMessageData( MsgParam.UniqueId ), _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.GameTurnInit		:	MsgGameTurnInit( timeStamp, _GetMessageOwner(), _GetMessageData( MsgParam.MiscVal ) );					break;

					case	MessageType.InitDieRollSet		:	MsgInitDieRollSet( timeStamp, _GetMessageData( MsgParam.UniqueId ), _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.InitDieRollRequest	:	MsgInitDieRollRequest( timeStamp, (_GetMessageData( MsgParam.MiscVal ) != 0 ) );	break;

					case	MessageType.AddStartResources	:	MsgAddStartResources( timeStamp, _GetMessageOwner() );							break;
					case	MessageType.InitGameSide		:	MsgInitGameSide( timeStamp, _GetMessageOwner() );	break;

					case	MessageType.InitPortLocRequest	:	MsgInitPortLocRequest( timeStamp, (_GetMessageData( MsgParam.MiscVal ) != 0 ) );	break;

					case	MessageType.InitTerrainRequest	:	MsgInitTerrainRequest( timeStamp, (_GetMessageData( MsgParam.MiscVal ) != 0 ) );	break;

					case	MessageType.LogicStateRequest	:	MsgLogicStateRequest( timeStamp, _GetMessageOwner(), (SideLogic.LOGIC_STATE)_GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.MessageHandled		:	MsgMessageHandled( timeStamp, _GetMessageSender(), (MessageType)_GetMessageData( MsgParam.UniqueId ), _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.StateRequest		:	MsgStateRequest( timeStamp, _GetMessageSender(), (PlayGameMgr.STATE)_GetMessageData( MsgParam.UniqueId ), _GetMessageData( MsgParam.SettlementId ), _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.RandomNumSeed		:	MsgRandomNumSeed( timeStamp, _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.ResourceDieRoll		:	MsgResourceDieRoll( timeStamp, _GetMessageData( MsgParam.MiscVal ) );	break;
					case	MessageType.ResourceUpdate		:	MsgResourceUpdate( timeStamp, _GetMessageSender(), (RESOURCE)_GetMessageData( MsgParam.Resource ), _GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.InitTerrainSet		:	MsgInitTerrainSet( timeStamp, _GetMessageData( MsgParam.UniqueId ), (TERRAIN)_GetMessageData( MsgParam.MiscVal ) );	break;

					case	MessageType.PickRoadWay			:	MsgPickRoadWay( timeStamp,  _GetMessageOwner() );	break;

					case	MessageType.PickSettlement		:	MsgPickSettlement( timeStamp,  _GetMessageOwner() );	break;

					case	MessageType.InitPortLocSet		:	MsgInitPortLocSet( timeStamp, (PORT)_GetMessageData( MsgParam.PortId ), (RESOURCE)_GetMessageData( MsgParam.Resource ) );	break;

				}
			}
		}

		private void	_ConfirmHandled()
		{
Debug.Assert( !mAssertIfNotHandled );
		}
		//public	virtual	void	MsgCoordOccupied( int msgTime, OWNER side, Point coord, int unitId ) { _ConfirmHandled(); }
		//public	virtual	void	MsgCoordLost( int msgTime, OWNER side, Point coord ) { _ConfirmHandled(); }
		//public	virtual	void	MsgCoordBuildCheck( int msgTime, OWNER side, Point coord ) { _ConfirmHandled(); }
		//public	virtual	void	MsgTurnNumber( int msgTime, int miscVal ) { _ConfirmHandled(); }
		public	virtual	void	MsgRenderMap( int msgTime ) { _ConfirmHandled(); }
		public	virtual	void	MsgAddRoadWay( int msgTime, OWNER side, int settlementId, CITY_DIR whichDir ) { _ConfirmHandled(); }
		public	virtual	void	MsgAddSettlement( int msgTime, OWNER side, int settlementId, int numActive ) { _ConfirmHandled(); }
		public	virtual	void	MsgAnimateStart( int msgTime, OWNER whoFor ) { _ConfirmHandled(); }
		public	virtual	void	MsgAnimateUpdate( int msgTime, bool wantThinkingGfx ) { _ConfirmHandled(); }
		public	virtual	void	MsgAnimateFinish( int msgTime ) { _ConfirmHandled(); }
		public	virtual	void	MsgGameTurnInit( int msgTime, OWNER whichSide, int turnNumber ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitDieRollRequest( int msgTime, bool wantRandom ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitPortLocRequest( int msgTime, bool wantRandom ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitTerrainRequest( int msgTime, bool wantRandom ) { _ConfirmHandled(); }
		public	virtual	void	MsgLogicStateRequest( int msgTime, OWNER sender, SideLogic.LOGIC_STATE stateEnum ) { _ConfirmHandled(); }
		public	virtual	void	MsgRandomNumSeed( int msgTime, int miscVal ) { _ConfirmHandled(); }
		public	virtual	void	MsgResourceDieRoll( int msgTime, int resourceDieRoll ) { _ConfirmHandled(); }
		public	virtual	void	MsgResourceUpdate( int msgTime, OWNER sender, RESOURCE resource, int quantityMod ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitTerrainSet( int msgTime, int uniqueId, TERRAIN terrain ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitDieRollSet( int msgTime, int uniqueId, int dieRoll ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitPortLocSet( int msgTime, PORT portId, RESOURCE portResource ) { _ConfirmHandled(); }
		public	virtual	void	MsgInitGameSide( int msgTime, OWNER side ) { _ConfirmHandled(); }
		public	virtual	void	MsgAddStartResources( int msgTime, OWNER side ) { _ConfirmHandled(); }
		public	virtual	void	MsgPickRoadWay( int msgTime, OWNER side ) { _ConfirmHandled(); }
		public	virtual	void	MsgPickSettlement( int msgTime, OWNER side ) { _ConfirmHandled(); }
		public	virtual	void	MsgMessageHandled( int msgTime, OWNER sender, MessageType whichMessage, int miscVal ) { _ConfirmHandled(); }
		public	virtual	void	MsgStateRequest( int msgTime, OWNER sender, PlayGameMgr.STATE whichState, int settlementId, int miscVal ) { _ConfirmHandled(); }

	
	}
}
