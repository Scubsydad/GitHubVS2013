using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for MessageCenter.
	/// </summary>
	/// 
	public	enum	MessageType
	{
		NA =			-1,

		AddRoadWay,
		AddSettlement,
		AddStartResources,

		AnimateFinish,
		AnimateStart,
		AnimateUpdate,

		GameTurnInit,

		InitDieRollRequest,
		InitPortLocRequest,
		InitTerrainRequest,				//	message to initialize the map
		InitGameSide,
		InitTurnSide,
		InitDieRollSet,
		InitTerrainSet,
		InitPortLocSet,

		LogicStateRequest,

		MessageHandled,				//	confirmation that a particular message has been handled, if we don't want to create specifics for each type.

		PickSettlement,
		PickRoadWay,


		RandomNumSeed,
		RenderMap,
		ResourceUpdate,

		StateRequest,


		SettlementBuilt,

		_size,
	};

	public	enum	MsgParam
	{
		Dir,

		MiscVal,
		PortId,
		Resource,
		SettlementId,
		TimeStamp,		//	NOT actually used, special accessor for Message Time post/get
		UniqueId, 

		SenderSide,		//	who is sending this message, not who its for
		WhichSide,
	}

	public	delegate void MessageBroadcastHandler( MessageType msgType, int uniqueId );

	public class MessageCenter
	{
		private	Message[]				mMessageBuffer;
		private	Message					mFirstInList, mLastInList;
		private	int						mMaxMessagesStored;
		private	int[,]					mParamIndex = new int[(int)MessageType._size, 2];
		private	MsgParameter[]			mParamTable = new MsgParameter[500];
		private int						mCurrParamTblIndex = 0;
		private int						mMaxParamBytesStorage = 0;
		private	MessageType				mMaxParamMessageType = MessageType.NA;
		private int[]					mDailyRemovalList = new int[300];
		private	int						mDailyRemovalIndex = 0;
		private int						mMsgStompIndex = -1;
		public	MessageBroadcastHandler	mBroadcastHandler;
		private int						mLastTimerTick = 0;
		private int						mLastUnpackId = -1;
		private int[]					mUnpackedMsg = new int[(int)Message.STORAGE._size];	//	unpack optimized space
		private	bool					mIsBroadCasting = false;
		private	Message					mCurrMessage;
		private	int						mLastAddedMsgId = -1;
		private float					mTimerTicks = 0.0f;

		public MessageCenter(int maxMessages)
		{
			mMaxMessagesStored = maxMessages;
			mMessageBuffer = new Message[mMaxMessagesStored];
			int i = 0;
			for ( ; i < mMaxMessagesStored; )
			{
				mMessageBuffer[i] = new Message( i++ );
			}

			for ( i = 0; i < (int)MessageType._size; ++i )
				mParamIndex[i,0] = -1;

			for ( i = 0; i < mDailyRemovalList.Length; ++i )
				mDailyRemovalList[i] = -1;

			mLastInList = mFirstInList = mMessageBuffer[0];

			MsgParam[] whichSideDesc = new MsgParam[] { MsgParam.WhichSide };	// 'whichSide' added automatically by '_AddMessage' if not present, but make it obvious here
			_AddMessageType( MessageType.InitGameSide, whichSideDesc );
			_AddMessageType( MessageType.PickRoadWay, whichSideDesc );
			_AddMessageType( MessageType.PickSettlement, whichSideDesc );
			_AddMessageType( MessageType.InitTurnSide, whichSideDesc );
			_AddMessageType( MessageType.RenderMap, whichSideDesc );
			_AddMessageType( MessageType.AddStartResources, whichSideDesc );

			MsgParam[] miscValDesc = new MsgParam[] { MsgParam.MiscVal };
			_AddMessageType( MessageType.RandomNumSeed, miscValDesc );
			_AddMessageType( MessageType.InitTerrainRequest, miscValDesc );
			_AddMessageType( MessageType.InitDieRollRequest, miscValDesc );
			_AddMessageType( MessageType.InitPortLocRequest, miscValDesc );
			_AddMessageType( MessageType.GameTurnInit, miscValDesc );
			_AddMessageType( MessageType.LogicStateRequest, miscValDesc );
			
			MsgParam[] hexLocVallDesc = new MsgParam[] { MsgParam.UniqueId, MsgParam.MiscVal };
			_AddMessageType( MessageType.InitDieRollSet, hexLocVallDesc );
			_AddMessageType( MessageType.InitTerrainSet, hexLocVallDesc );

			MsgParam[] senderValDesc = new MsgParam[] { MsgParam.MiscVal, MsgParam.SenderSide };
			_AddMessageType( MessageType.AnimateStart, senderValDesc );
			_AddMessageType( MessageType.AnimateUpdate, senderValDesc );
			_AddMessageType( MessageType.AnimateFinish, senderValDesc );

			MsgParam[] senderValResourceDesc = new MsgParam[] { MsgParam.MiscVal, MsgParam.Resource,  MsgParam.SenderSide };
			_AddMessageType( MessageType.ResourceUpdate, senderValResourceDesc );

			MsgParam[] senderUniqueValDesc = new MsgParam[] { MsgParam.UniqueId, MsgParam.MiscVal, MsgParam.SenderSide };
			_AddMessageType( MessageType.AddRoadWay, senderUniqueValDesc );
			_AddMessageType( MessageType.AddSettlement, senderUniqueValDesc );
			_AddMessageType( MessageType.MessageHandled, senderUniqueValDesc );

			MsgParam[] senderVal2Desc = new MsgParam[] { MsgParam.UniqueId, MsgParam.SettlementId, MsgParam.MiscVal, MsgParam.SenderSide };
			_AddMessageType( MessageType.StateRequest, senderVal2Desc );

			_AddMessageType( MessageType.InitPortLocSet, new MsgParam[] { MsgParam.PortId, MsgParam.Resource } );

	
		}

		public	MessageType[]	GetAllMessageTypes()
		{
			int maxTypes = (int)MessageType._size;
			MessageType[] allTypes = new MessageType[maxTypes];
			for ( int i = 0; i < maxTypes; ++i )
			{
				allTypes[i] = (MessageType)i;
			}
			return ( allTypes );
		}

		private void	_AddMessageType( MessageType msgType, MsgParam[] accessIds )
		{
Debug.Assert(( mParamIndex[(int)msgType,1] == 0 ), "Already items registered against this MessageType");

			bool	foundWhichSide = false;

			foreach ( MsgParam accessId in accessIds )
			{
				if ( accessId == MsgParam.WhichSide )
				{
					foundWhichSide = true;
				}
				switch ( accessId )
				{
/* short data */	default						:	_RegisterShortParam( msgType, accessId );		break;

/* byte data */		case	MsgParam.WhichSide	:	_RegisterByteParam( msgType, accessId );		break;

/* int data */		case	MsgParam.UniqueId	:	_RegisterIntParam( msgType, accessId );			break;
				}
			}
			if ( !foundWhichSide )									//	if param list did not contain 'whichSide' already...
			{
				_RegisterByteParam( msgType, MsgParam.WhichSide );	//	add it manually here so EVERY message has a 'whichSide' param
			}
			_ValidateCurrStorageSize(msgType) ;
		}

		public	void RequestMessageNotification(MessageBroadcastHandler function)
		{
			mBroadcastHandler += function;
		}

		private  bool		_ValidateCurrStorageSize(MessageType msgType)
		{
			int numBytes = 0;
			int scanIndex =  mParamIndex[(int)msgType,0];
			int numToScan = mParamIndex[(int)msgType,1];
			for ( int i = 0; i < numToScan; ++i, ++scanIndex )
			{
				numBytes += mParamTable[scanIndex].GetNumBytes();
			}
			bool haveSpace =( numBytes <= 24 );			//	10 shorts are 160 bits
Debug.Assert(haveSpace,"too many items/bytes for storage space");
			if ( numBytes > mMaxParamBytesStorage )		//	keep track of maximum registeres size, so we can trim it at 'final'
			{
				mMaxParamBytesStorage = numBytes;		//	store max # of bytes any one registered item uses
				mMaxParamMessageType = msgType;			//	store enum of item, in case we want to refactor it to reduce it...
			}
			return ( haveSpace );
		}
			
		private bool	_AddParamToTable(MessageType msgType, MsgParam desc, bool isSigned, int numBytes)
		{
			MsgParameter param = new MsgParameter(desc, numBytes, isSigned );

			if ( mParamIndex[(int)msgType,0] == -1 )				//	just starting a fresh registration...
			{
				mParamIndex[(int)msgType,0] = mCurrParamTblIndex;	//	base address to start reading from...
				mParamIndex[(int)msgType,1] = 1;					//	we must have at least one param if hitting this point
			}
			else
			{
				++mParamIndex[(int)msgType,1];						//	increment # of paramaters this item has associated
			}
			mParamTable[mCurrParamTblIndex++] = param;				//	store the param...
			bool allOk = ( mCurrParamTblIndex <= mParamTable.Length );	// protect against memory stomp
Debug.Assert(allOk, "ran out of parameter storage space!");
			return ( allOk );
		}

		public	bool	_RegisterIntParam(MessageType msgType, MsgParam desc )
		{
			return ( _AddParamToTable( msgType, desc, true, 4) );
		}

		public	bool	_RegisterUIntParam(MessageType msgType, MsgParam desc )
		{
			return ( _AddParamToTable( msgType, desc, false, 4) );
		}

		public	bool	_RegisterShortParam(MessageType msgType, MsgParam desc)
		{
			return ( _AddParamToTable( msgType, desc, true, 2) );
		}

		public	bool	_RegisterUShortParam(MessageType msgType, MsgParam desc)
		{
			return ( _AddParamToTable( msgType, desc, false, 2) );
		}

		public	bool	_RegisterByteParam(MessageType msgType, MsgParam desc)
		{
			return ( _AddParamToTable( msgType, desc, true, 1 ) );
		}

		public	bool	_RegisterUByteParam(MessageType msgType, MsgParam desc)
		{
			return ( _AddParamToTable( msgType, desc, false, 1) );
		}

		private Message		_GetMessageByUniqueId(int uniqueId, bool doOptimizedUnpack )
		{
Debug.Assert( ( uniqueId >= 0 ) && ( uniqueId < mMaxMessagesStored ), "Error, invalid 'unique Id' message ID!" );
			Message message =  mMessageBuffer[uniqueId];
			if ( ( doOptimizedUnpack ) && ( mLastUnpackId != uniqueId ))
			{
				mLastUnpackId = uniqueId;
				MessageType msgType = message.GetMessageType();
				int scanIndex =  mParamIndex[(int)msgType,0];
				int numToScan = mParamIndex[(int)msgType,1];
				int totalBytesIn = 0, i = 0, sizeInBytes;
				bool isSigned;

				for ( ; i < numToScan; ++i, ++scanIndex )
				{
					sizeInBytes = mParamTable[scanIndex].GetNumBytes();
					isSigned = mParamTable[scanIndex].IsSigned();
					mUnpackedMsg[i] = message.GetValue( totalBytesIn, sizeInBytes, isSigned );
					totalBytesIn += sizeInBytes;
				}
				if ( i < (int)Message.STORAGE._size )
				{
					mUnpackedMsg[i] = 12345678;
				}
			}
			return ( message );			//	also store message in cache
		}
		
		private int			_GetStorageIndex( MessageType msgType, MsgParam paramDesc )
		{
			int storageIndex = -1;								//	assume not found by default
			int scanIndex =  mParamIndex[(int)msgType,0];
			int numToScan = mParamIndex[(int)msgType,1];
			for ( int i = 0; i < numToScan; ++i, ++scanIndex )
			{
				if ( mParamTable[scanIndex].IsMatchingParam( paramDesc ) )
				{
					storageIndex = i;
					break;
				}
			}
			return ( storageIndex );
		}

		private bool		_GetBytesInAndSize(MessageType msgType, MsgParam paramDesc, out int totalBytes, out int sizeInBytes, out bool isSigned)
		{
			bool foundItem = isSigned = false;
			totalBytes = 0;
			sizeInBytes = 0;
			int scanIndex =  mParamIndex[(int)msgType,0];
			int numToScan = mParamIndex[(int)msgType,1];
			for ( int i = 0; i < numToScan; ++i, ++scanIndex )
			{
				if ( mParamTable[scanIndex].IsMatchingParam( paramDesc ) )
				{
					sizeInBytes = mParamTable[scanIndex].GetNumBytes();
					isSigned = mParamTable[scanIndex].IsSigned();
					foundItem = true;
					break;
				}
				else
					totalBytes += mParamTable[scanIndex].GetNumBytes();
			}
			
			return ( foundItem );
		}

		private void		_BroadcastMessageToHandlers(Message message)
		{
			int uniqueId = message.GetUniqueId();
			MessageType msgType = message.GetMessageType();

			//BRK in c++ call virtual functions, in C#, fire off 'event' so event handlers can query the details
			if ( mBroadcastHandler != null )
			{
				mBroadcastHandler( msgType, uniqueId );
			}
			message.BroadCasted();

Debug.Assert( ( mDailyRemovalIndex < mDailyRemovalList.Length ), "about to stomp memory!");
			mDailyRemovalList[mDailyRemovalIndex++] = uniqueId;		//	add the unique ID to be removed 'tomorrow' when its processed by all the handlers
		}

		private int			_PackThreeVals(int minutes, int seconds, int hundredths)
		{
			int packedVal = ( minutes * 10000 ) + ( seconds * 100 ) + hundredths;
			return ( packedVal );
		}

		private int			_ConvertToPackedTime( int timerTicks )
		{
			int minutes = 0;
			int hundredths = ( timerTicks % 100 );
			int seconds = ( timerTicks / 100 );
			if ( seconds >= 60 )
			{
				minutes = ( seconds / 60 );
				seconds = ( seconds - ( minutes * 60 ));
			}
			int packedTime = _PackThreeVals( minutes, seconds, hundredths);

			return ( packedTime );
		}

		private int			_ConvertToPackedTime( int addSeconds, int addHundredths )
		{
			int	packedTime = 0;
			if ( ( addSeconds != 0 ) || ( addHundredths != 0 ))
			{
				int timerTicks = mLastTimerTick;
				timerTicks += ( addSeconds * 100 ) + addHundredths;
				int minutes = 0;
				int hundredths = ( timerTicks % 100 );
				int seconds = ( timerTicks / 100 );
				if ( seconds >= 60 )
				{
					minutes = ( seconds / 60 );
					seconds = ( seconds - ( minutes * 60 ));
				}
				packedTime = _PackThreeVals( minutes, seconds, hundredths);
			}
			return ( packedTime );
		}

		public	int			GetLastTimerTickVal()
		{
			return ( mLastTimerTick ); 
		}


		public	int		TimerTask( float timerTicksMod )
		{
			mTimerTicks += timerTicksMod;
			int timerTicks = (int)mTimerTicks;
			mLastTimerTick = timerTicks;
			int packedTime = _ConvertToPackedTime( timerTicks );
			for ( int i = 0; i < mDailyRemovalIndex; ++i )
			{
				_GetMessageByUniqueId( mDailyRemovalList[i], false ).SetExpired();
				mDailyRemovalList[i] = -1;					//	clear out any old ids to keep it 'clean' each day
			}
			mDailyRemovalIndex = 0;							//	now zero the variable to receive new broadcasted messages for 'today'

			Message	next = mFirstInList;
			while ( next.IsNewerMessage( packedTime ) )
			{
				mIsBroadCasting = true;						//	track that message is being broadcast...
				mCurrMessage = next;						//	track which message is being broadcast currently
				try
				{
					_BroadcastMessageToHandlers( mCurrMessage );	//	broadcast message, 'today' is the day
					next = mCurrMessage.GetNext();
				}
				catch ( Exception ex )
				{
					MessageBox.Show( ex.StackTrace, ex.Message );
				}
				mIsBroadCasting = false;					//	not actively broadcasting at this time
			}
			mFirstInList = next;							//	new 'first in list' now that we've traversed older ones...

			return ( mLastTimerTick );
		}

		private int			_AddMessage( OWNER whoFor, MessageType msgType, int fullSecondsAndPct )
		{
			int packedTime =  _ConvertToPackedTime( ( fullSecondsAndPct / 100 ) , ( fullSecondsAndPct % 100 ) );
			if ( ( packedTime == 0 ) && ( mIsBroadCasting ) )
			{
				packedTime = mCurrMessage.GetTimeStamp();	//	get time stamp of message being broadcast so we get proper inclusion slot
			}
Debug.Assert( mLastAddedMsgId == -1, "Potentially destroying queued message id!" );
			mLastAddedMsgId = _AddMessage( msgType, packedTime );

			_SetMessageData( mLastAddedMsgId, MsgParam.WhichSide, (int)whoFor );

			return ( mLastAddedMsgId );
		}
		
		private int			_AddMessage( MessageType msgType, int packedTime )
		{
			Message	next = mFirstInList;
			Message prev = null;
			while ( next.IsNewerMessage( packedTime ) )
			{
				prev = next;
				next = next.GetNext();
			}
			do
			{
				if ( ++mMsgStompIndex == mMaxMessagesStored )		//	is next try default 'too big'?
				{
					mMsgStompIndex = 0;								//	reset at start index if so
				}
	
			} while ( !mMessageBuffer[mMsgStompIndex].IsEmptyItem() );

			Message newMessage = mMessageBuffer[mMsgStompIndex] = new Message( msgType, mMsgStompIndex, packedTime, prev, next );

			if ( ( prev == null ) ||			//	if no previous message...
				 ( prev.WasBroadCasted() ) )	//	or 'previous message' has already been broadcasted...
			{
				mFirstInList = newMessage;		//	then this message is the new 'firstInList'
			}
			if ( next.IsEmptyItem() ) 
			{
				mLastInList = newMessage;
			}
			return ( mMsgStompIndex );
		}


		public	OWNER		GetMessageSide( int uniqueMsgId )
		{
			return ( (OWNER)GetMessageData( uniqueMsgId, MsgParam.WhichSide ) );
		}

		//public	System.Drawing.Point		GetMessageCoord( int uniqueMsgId )
		//{
		//	return ( new System.Drawing.Point( GetMessageData( uniqueMsgId, MsgParam.CoordX ),
		//									   GetMessageData( uniqueMsgId, MsgParam.CoordY ) ) );
		//}

		public	int			GetMessageData(int uniqueIdIndex, MsgParam paramDesc)
		{
			Message		message = _GetMessageByUniqueId( uniqueIdIndex, true );
			int returnVal = -1;
			if ( paramDesc == MsgParam.TimeStamp )
			{
				returnVal = message.GetTimeStamp();
				if ( returnVal == 0 )
				{
					returnVal = mLastTimerTick;
				}
			}
			else
			{
				int			storageIndex = _GetStorageIndex( message.GetMessageType(), paramDesc );
				bool		foundItem = ( storageIndex != -1 );
Debug.Assert( foundItem, "Attempting to extract invalid unqiue param MsgParam from message!" );
				if ( foundItem )
				{
					returnVal = mUnpackedMsg[storageIndex];
				}
			}
			return ( returnVal );
		}

		private  void		_PostMessage( )
		{
			_PostMessage( mLastAddedMsgId );
		}

		private  void		_PostMessage( int uniqueIdIndex )
		{
			Message		message = _GetMessageByUniqueId(uniqueIdIndex, false );
			mLastAddedMsgId = -1;	//	destroy this immediately upon post request, so its not left 'dangling' during broadcasts
			if ( message.SetPostedStatus() )
			{
				_BroadcastMessageToHandlers(message);
			}
		}

		private	bool		_SetMessageData( MsgParam paramDesc, int valToSet)
		{
			return ( _SetMessageData( mLastAddedMsgId, paramDesc, valToSet ) );
		}

		private	bool		_SetMessageData( MsgParam paramDesc, bool boolParam )
		{
			int intVal = 0;	//assume false by default
			if ( boolParam ) { intVal = 1; }
			return ( _SetMessageData( mLastAddedMsgId, paramDesc, intVal ) );
		}

		public	bool		_SetMessageData(int uniqueIdIndex, MsgParam paramDesc, int valToSet)
		{
			Message		message = _GetMessageByUniqueId( uniqueIdIndex, false );
			int totalBytesIn, storageBytes;
			bool isSigned;
			bool foundItem = _GetBytesInAndSize( message.GetMessageType(), paramDesc, out totalBytesIn, out storageBytes, out isSigned);
			if ( storageBytes == 1 )
			{
				if ( isSigned )
				{
Debug.Assert( ( ( valToSet >= -128 ) && ( valToSet <= 127 )), "invalid 'signed byte' parameter passed" );
				}
				else
				{
Debug.Assert( ( ( valToSet >= 0 ) && ( valToSet <= 255 )), "invalid 'unsigned byte' parameter passed" );
				}
			}
			else if ( storageBytes == 2 )
			{
				if ( isSigned )
				{
Debug.Assert( ( ( valToSet >= -32768 ) && ( valToSet <= 32767 )), "invalid 'signed short' parameter passed" );
				}
				else
				{
Debug.Assert( ( ( valToSet >= 0 ) && ( valToSet <= 65535 )), "invalid 'unsigned short' parameter passed" );
				}
			}					   
			message.SetValue(totalBytesIn, storageBytes, valToSet);

			return ( foundItem );
		}

		private class	Message	
		{
			public enum STORAGE
			{
				_size = 24,
			};
			
			private int					mStorageId;										//	4 bytes
			private MessageType			mMsgType;										//	4 bytes			
			private	int					mTime;											//	4 bytes					
			private	byte[]				mContentBuffer = new byte[(int)STORAGE._size];	//	24 bytes			( 6 ints , or 12 shorts  )
			private	Message				mNext, mPrev;									//	8 bytes (2 * 4)		
			private byte				mBeenPosted;									//	1 byte
			private byte				mBroadCasted;									//	1 byte
			//	50 bytes for each Message Structure

			public	Message(int storageId)
			{
				mStorageId = storageId;
				mTime = -1;
				mMsgType = MessageType.NA;
				mPrev = mNext = null;
				mBroadCasted = mBeenPosted = 0;
			}

			public	Message(MessageType type, int storageId, int packedTime, Message prev, Message next)
			{
				mStorageId = storageId;
				mTime = packedTime;
				mMsgType = type;

				mPrev = prev;
				if ( prev != null )
				{
					prev.SetNextLink( this );
				}

				mNext = next;
				if ( next != null )
				{
					next.SetPrevLink( this );
				}
				mBroadCasted = mBeenPosted = 0;
			}

			public	int		GetTimeStamp()
			{
				return ( mTime );
			}

			public	bool	WasPostedAlready()
			{
				return ( ( mBeenPosted == 1 ) );
			}

			public	void	BroadCasted()
			{
				mBroadCasted = 1;
			}

			public	bool	WasBroadCasted()
			{
				return ( mBroadCasted == 1 );
			}

			public  bool	SetPostedStatus()
			{
Debug.Assert( ( mBeenPosted == 0 ), "Already posted message" );
				mBeenPosted = 1;
				bool broadCastImmediately = ( mTime == 0 );		//	if no real date or time, message needs to be broadcast immediately
				return ( broadCastImmediately );
			}

			public  void	SetPrevLink(Message msg)
			{
				mPrev = msg;
			}

			public  void	SetNextLink(Message msg)
			{
				mNext = msg;
			}

			public	bool	IsEmptyItem()
			{
				bool	isEmpty =  ( mTime < 0 );
				return ( isEmpty );
			}

			public	void	SetExpired()
			{
				mTime = -1;						//	flag item is no longer valid & can be stomped in future

				Message next = mNext;			//	get the 'next message' link...
				Message prev = mPrev;			//	get the 'prev message' link...
				if ( next != null )				//	if its not a null pointer
				{
					next.SetPrevLink( prev );	//	tell the 'next guy' about a new 'previous' link
				}
				if ( prev != null )				//	if its not a null pointer
				{
					prev.SetNextLink( next );	//	tell the 'prev guy' about a new 'next' link
				}
			}

			public	int		GetUniqueId()
			{
				return ( mStorageId );
			}

			public  MessageType  GetMessageType()
			{
				return ( mMsgType );
			}

			public	bool	IsNewerMessage( int packedTime )
			{
				bool isNewer = false;											//	assume not by default
				if ( ( mTime >= 0 ) && ( packedTime >= mTime ))					//	is the time stamp greater? (or same)
				{
					isNewer = true;												//	then our parame stuff is still newer
				}
				return ( isNewer );
			}

			public	Message		GetNext()
			{
				return ( mNext );
			}

			public	Message		GetPrev()
			{
				return ( mPrev );
			}

			public	void	SetValue(int numBytesIn, int bytesToUse, int valToSet )
			{
				if ( bytesToUse == 1 )
				{
					mContentBuffer[numBytesIn] = (byte)valToSet;
				}
				else if ( bytesToUse == 2 )
				{
					mContentBuffer[numBytesIn++] = (byte)( ( valToSet & 0xff00 ) >> 8 );
					mContentBuffer[numBytesIn] =   (byte)  ( valToSet & 0x00ff );
				}
				else // bytesToUse == 4 )
				{
					mContentBuffer[numBytesIn++] = (byte)( ( valToSet & 0xff000000 ) >> 24 );
					mContentBuffer[numBytesIn++] = (byte)( ( valToSet & 0x00ff0000 ) >> 16 );
					mContentBuffer[numBytesIn++] = (byte)( ( valToSet & 0x0000ff00 ) >> 8 );
					mContentBuffer[numBytesIn] =   (byte)  ( valToSet & 0x000000ff );
				}
			}

			public	int		GetValue(int numBytesIn, int bytesToUse, bool isSigned )
			{
				int returnVal = 0;
				sbyte negVal = -1;
				short negShort = -1;
				if ( bytesToUse == 1 )
				{
					returnVal = mContentBuffer[numBytesIn];
					if (( isSigned ) && (( returnVal & 0x00000080 ) != 0 ) )
					{
						returnVal |= ( negShort << 16 );
						returnVal |= ( negVal << 8 );
					}
				}
				else if ( bytesToUse == 2 )
				{
					returnVal |= ( mContentBuffer[numBytesIn++] << 8 );
					returnVal |=  mContentBuffer[numBytesIn++];
					if (( isSigned ) && (( returnVal & 0x00008000 ) != 0 ) )
					{
						returnVal |= ( negShort << 16 );
					}
				}
				else // bytesToUse == 4 )
				{
					returnVal |= ( mContentBuffer[numBytesIn++] << 24 );
					returnVal |= ( mContentBuffer[numBytesIn++] << 16 );
					returnVal |= ( mContentBuffer[numBytesIn++] << 8 );
					returnVal |=  mContentBuffer[numBytesIn++];
				}
				return ( returnVal );
			}
		};

		private struct	MsgParameter
		{
			private	MsgParam			mMsgParam;
			private	short				mNumBytes;
			private short				mIsSigned;

			public	MsgParameter(MsgParam desc, int numBytes, bool isSigned)
			{
				mMsgParam = desc;
				mNumBytes = (short)numBytes;
				mIsSigned = 0;
				if ( isSigned )
					mIsSigned = 1;
			}

			public	bool	IsSigned()
			{
				return ( mIsSigned == 1 );
			}

			public	int		GetNumBytes()
			{
				return ( mNumBytes );
			}

			public	bool	IsMatchingParam(MsgParam compare)
			{
				bool sameParam = ( compare == mMsgParam );
				return ( sameParam );
			}
		}

		public void	SendMsgRenderMap( ) 
		{
			_AddMessage( OWNER.MANAGER, MessageType.RenderMap, 0 );	//	render the map to reflect the last choice made by settlement placement
			_PostMessage();
		}

		public void SendMsgAddRoadWay( OWNER side, int settlementId, CITY_DIR whichDir )  
		{
			_AddMessage( OWNER._size, MessageType.AddRoadWay, 0 );
			_SetMessageData( MsgParam.UniqueId, settlementId );
			_SetMessageData( MsgParam.MiscVal, (int)whichDir );
			_SetMessageData( MsgParam.SenderSide, (int)side );		//	we need to broadcast who sent it, because everybody should listen...
			_PostMessage();
		}

		public void SendMsgAddSettlement( OWNER side, int settlementId, int numSettlements ) 
		{
			_AddMessage( OWNER._size, MessageType.AddSettlement, 0 );
			_SetMessageData( MsgParam.UniqueId, settlementId );
			_SetMessageData( MsgParam.MiscVal, numSettlements );
			_SetMessageData( MsgParam.SenderSide, (int)side );		//	we need to broadcast who sent it, because everybody should listen...
			_PostMessage();
		}

		public void SendMsgInitDieRollRequest( int wantRandomOnOff ) 
		{
			_AddMessage( OWNER._size, MessageType.InitDieRollRequest, 0 );
			_SetMessageData( MsgParam.MiscVal, wantRandomOnOff );
			_PostMessage();
		}

		public void SendMsgInitPortLocRequest( int wantRandomOnOff ) 
		{
			_AddMessage( OWNER._size, MessageType.InitPortLocRequest, 0 );
			_SetMessageData( MsgParam.MiscVal, wantRandomOnOff );
			_PostMessage();
		}

		public void SendMsgInitTerrainRequest( int wantRandomOnOff ) 
		{
			_AddMessage( OWNER._size, MessageType.InitTerrainRequest, 0 );
			_SetMessageData( MsgParam.MiscVal, wantRandomOnOff );
			_PostMessage();
		}

		public void SendMsgRandomNumSeed( int randSeedVal ) 
		{
			_AddMessage( OWNER._size, MessageType.RandomNumSeed, 0 );
			_SetMessageData( MsgParam.MiscVal, randSeedVal );
			_PostMessage();
		}

		public void SendMsgInitTerrainSet( int uniqueId, TERRAIN terrain ) 
		{
			_AddMessage( OWNER._size, MessageType.InitTerrainSet, 0 );
			_SetMessageData( MsgParam.UniqueId, uniqueId );
			_SetMessageData( MsgParam.MiscVal, (int)terrain );
			_PostMessage();
		}

		public void SendMsgInitDieRollSet( int uniqueId, int dieRoll ) 
		{
			_AddMessage( OWNER._size, MessageType.InitDieRollSet, 0 );
			_SetMessageData( MsgParam.UniqueId, uniqueId );
			_SetMessageData( MsgParam.MiscVal, dieRoll );
			_PostMessage();
		}

		public void SendMsgInitPortLocSet( PORT portEnum, RESOURCE portResource ) 
		{
			_AddMessage( OWNER._size, MessageType.InitPortLocSet, 0 );
			_SetMessageData( MsgParam.PortId, (int)portEnum );
			_SetMessageData( MsgParam.Resource, (int)portResource );
			_PostMessage();
		}

		public void SendMsgInitGameSide( OWNER side ) 
		{
			_AddMessage( side, MessageType.InitGameSide, 0 );
			_PostMessage();
		}

		public void SendMsgPickRoadWay( OWNER side ) 
		{
			_AddMessage( side, MessageType.PickRoadWay, 0 );		//	each settlement placed should fire off the next guy in line
			_PostMessage();
		}

		public void SendMsgGameTurnInit( OWNER whichSide, int turnNum )
		{
			_AddMessage( whichSide, MessageType.GameTurnInit, 0 );
			_SetMessageData( MsgParam.MiscVal, turnNum );
			_PostMessage();
		}

		public void SendMsgPickSettlement( OWNER side ) 
		{
			_AddMessage( side, MessageType.PickSettlement, 0 );	//	each settlement placed should fire off the next guy in line
			_PostMessage();
		}

		public void SendMsgMessageHandledDelayed( int msgTime, OWNER sender, MessageType whichMessage, int miscAssocVal ) 
		{
			_AddMessage( OWNER.MANAGER, MessageType.MessageHandled, msgTime );	//	supports delayed acknowledgement if applicable
			_SetMessageData( MsgParam.UniqueId, (int)whichMessage );
			_SetMessageData( MsgParam.MiscVal, miscAssocVal );
			_SetMessageData( MsgParam.SenderSide, (int)sender );		//	we need to broadcast who sent it, because everybody should listen...
			_PostMessage();
		}

		public void SendMsgMessageHandled( OWNER sender, MessageType whichMessage, int miscAssocVal ) 
		{
			SendMsgMessageHandledDelayed( 0, sender, whichMessage, miscAssocVal );	// delay of zero so it goes out 'automatically'
		}

		public void SendMsgStateRequest( OWNER sender, PlayGameMgr.STATE whichState, int settlementid, int miscAssocVal ) 
		{
			_AddMessage( OWNER.MANAGER, MessageType.StateRequest, 0 );
			_SetMessageData( MsgParam.UniqueId, (int)whichState );
			_SetMessageData( MsgParam.SettlementId, settlementid );
			_SetMessageData( MsgParam.MiscVal, miscAssocVal );
			_SetMessageData( MsgParam.SenderSide, (int)sender );		//	we need to broadcast who sent it, because everybody should listen...
			_PostMessage();
		}

		public void	MsgAddStartResources( int msgTime, OWNER whoFor )
		{
			_AddMessage( whoFor, MessageType.AddStartResources, 0 );
			_PostMessage();
		}

		public void SendMsgAnimateStart( OWNER whoFor ) 
		{
			_AddMessage( OWNER.MANAGER, MessageType.AnimateStart, 0 );
			_SetMessageData( MsgParam.MiscVal, (int)whoFor );
			_PostMessage();
		}

		public void SendMsgAnimateUpdate( int msgTime, bool wantThinkingGfx )
		{
			_AddMessage( OWNER.MANAGER, MessageType.AnimateUpdate, msgTime );
			_SetMessageData( MsgParam.MiscVal, wantThinkingGfx );
			_PostMessage();
		}

		public void SendMsgAnimateFinish( int msgTime )
		{
			_AddMessage( OWNER.MANAGER, MessageType.AnimateFinish, msgTime );
			_PostMessage();
		}

		public void SendMsgLogicStateRequest( int msgTime, OWNER sender, SideLogic.LOGIC_STATE stateEnum )
		{
			_AddMessage( sender, MessageType.LogicStateRequest, msgTime );
			_SetMessageData( MsgParam.SenderSide, (int)sender );		//	we need to broadcast who sent it, because everybody should listen...
			_SetMessageData( MsgParam.MiscVal, (int)stateEnum );
			_PostMessage();
		}

		public void SendMsgResourceUpdate( OWNER sender, RESOURCE resource, int quantityMod )
		{
			_AddMessage( OWNER._size, MessageType.ResourceUpdate, 0 );
			_SetMessageData( MsgParam.SenderSide, (int)sender );		//	we need to broadcast who sent it, because everybody should listen...
			_SetMessageData( MsgParam.MiscVal, quantityMod );
			_SetMessageData( MsgParam.Resource, (int)resource );
			_PostMessage();
		}
	}

	public	class	MsgFilter
	{
		public	MessageCenter			Ptr;
		private	bool[]					mWantedMsg = null;
		private	MessageBroadcastHandler	mParentHandler;

		public	MsgFilter( MessageCenter msgCtr, MessageBroadcastHandler parentReceiveFunction, MessageType[] wantedMsg )
		{
			Ptr = msgCtr;
			Ptr.RequestMessageNotification( new MessageBroadcastHandler( LocalReceiveFunction ) );
			mParentHandler = parentReceiveFunction;

			if ( wantedMsg != null )
			{
				mWantedMsg = new bool[(int)MessageType._size];

				foreach ( MessageType msgType in wantedMsg )
				{
					mWantedMsg[ (int)msgType ] = true;
				}
			}
		}

		public	void LocalReceiveFunction( MessageType msgType, int uniqueMsgId )
		{
			if (( mWantedMsg == null ) || ( mWantedMsg[ (int)msgType] ))
			{
				mParentHandler( msgType, uniqueMsgId );
			}
		}
	}
}
