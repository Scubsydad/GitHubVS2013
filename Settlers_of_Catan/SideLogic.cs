using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Settlers_of_Catan
{
	public class SideLogic	:	MessageHandler
	{

		private OWNER			mWhichSide;
		private	bool			mIsUserControlled;
		private int				mNumPlayers;
		private MessageCenter	mMessageCtr;
		private HexInfo[]		mHexInfo;
		private int[]			mNumAssetsAvail;
		private int[]			mNumAssetsUsed;
		private LogicKernel		mLogicKernel;
		private ArrayList		mBuildLocs = new ArrayList();
		private PathFinder		mPathFinder;
		public	SideLogic( OWNER whichSide, LogicKernel logicKernel, HexInfo[] hexInfos )	:	base ( whichSide, false )
		{
			mWhichSide = whichSide;
			mLogicKernel = logicKernel;
			mHexInfo = hexInfos;
		}

		public	SideLogic( OWNER whichSide, bool isUserControlled, int totalPlayers, MessageCenter msgCenter, PathFinder pathFinder )	:	base ( whichSide, false )
		{
			mWhichSide = whichSide;
			mIsUserControlled = isUserControlled;
			mNumPlayers = totalPlayers;
			mMessageCtr = msgCenter;
			mHexInfo = Support.GetHexInfoCopy();
			mPathFinder = pathFinder;
		}

		public	override	void	MsgInitDieRollSet( int msgTime, int uniqueId, int dieRoll )
		{
			mHexInfo[uniqueId].SetDieRoll( dieRoll );
		}

		public	override	void	MsgInitTerrainSet( int msgTime, int uniqueId, TERRAIN terrain )
		{
			mHexInfo[uniqueId].SetTerrain( terrain );
		}

		public	override	void	MsgInitPortLocSet( int msgTime, PORT portId, RESOURCE portResource )
		{		
			int			index, numPorts;
			CITY_DIR	cityDir;
			foreach ( HexInfo hexInfo in mHexInfo )
			{
				numPorts = hexInfo.GetNumPorts();
				if ( numPorts != 0 )
				{
					for ( index = 0; index < (int)CITY_DIR._size; ++index )
					{
						cityDir = (CITY_DIR)index;
						if ( hexInfo.GetBuildLoc( cityDir ).GetSettlementLoc().GetPortId() == portId )
						{
							 hexInfo.GetBuildLoc( cityDir ).SetPortResource( portResource );
						}
					}
				}
			}
		}

		public  int			GetNumAssets( ASSET assetType )
		{
			return ( mNumAssetsUsed[(int)assetType] );
		}

		public  int 		AdjustAssetCount( ASSET assetType, int modifier )
		{
			mNumAssetsUsed[(int)assetType] += modifier;
			mNumAssetsAvail[(int)assetType] -= modifier;

			return ( mNumAssetsUsed[(int)assetType] );
		}

		public	override	void	MsgInitGameSide( int msgTime, OWNER side )
		{
			mNumAssetsUsed = new int[(int)ASSET._size];		//	 by default, no used assets at start of game

			mNumAssetsAvail = new int[(int)ASSET._size];	//	init array
			mNumAssetsAvail[(int)ASSET.SETTLEMENT] = 5;		//	5 settlements by default
			mNumAssetsAvail[(int)ASSET.CITY] = 4;			//	4 cities by default
			mNumAssetsAvail[(int)ASSET.ROAD] = 15;			//	15 roads by default

			mLogicKernel = Support.GetLogicKernel( mWhichSide );
		}

		public  HexInfo[]	GetHexInfoArray()
		{
			return ( mHexInfo );
		}


		private int			_DoValueEntitlement( int[] hexValues, int numToConsider, Point[] optionalList )
		{
			ArrayList	arrayList = new ArrayList();
			int	weightValue, scanIndex, insertIndex;
			Point		vals;
			for ( int index = 0; index < hexValues.Length; ++index )
			{
				weightValue = hexValues[index];
				vals = new Point( weightValue, index );
				insertIndex = -1;
				for ( scanIndex = 0; scanIndex < arrayList.Count; ++scanIndex )
				{
					if ( weightValue > ((Point)arrayList[scanIndex]).X )
					{
						insertIndex = scanIndex;
						break;
					}
				}
				if ( insertIndex == -1 )
				{
					arrayList.Add( vals );
				}
				else
				{
					arrayList.Insert( insertIndex, vals );
				}
			}
			while ( arrayList.Count > numToConsider )
			{
				arrayList.RemoveAt( numToConsider );
			}
			int sumValue = 0;
			foreach ( Point val in arrayList )
			{
				sumValue += val.X;
			}
			int selectedIndex = -1;
			int pct, randVal = Support.GetRand( sumValue );
			for ( scanIndex = 0; scanIndex < arrayList.Count; ++scanIndex )
			{
				vals = (Point)arrayList[scanIndex];
				if ( ( selectedIndex == -1 ) && ( randVal < vals.X ) )
				{
					selectedIndex = vals.Y;
				}
				randVal -= vals.X;
				pct = ( ( vals.X * 100 ) / sumValue );
				vals = new Point( pct, vals.Y );
				arrayList[scanIndex] = vals;
				if ( optionalList != null )
				{
					optionalList[scanIndex] = vals;
				}
			}
			return ( selectedIndex );
		}

		public	void		SubmitNewHexInfo( HexInfo[] hexInfoArray )
		{
			mHexInfo = hexInfoArray;
		}
		
		private RESOURCE _GetHex1Resource()
		{
			RESOURCE	hex1Resource = RESOURCE.INVALID;
			if ( mNumAssetsUsed[(int)ASSET.SETTLEMENT] != 0 )
			{
				hex1Resource = mHexInfo[_GetBuildLoc(0).GetHexAssocId()].GetEarnResource();
			}
			return ( hex1Resource );
		}

		private int		_InferNumSettlementsByHex1Resource( RESOURCE hex1Resource )
		{
			int			numSettlements = 0;				//	assume we are doing for the initial settlement by default
			if ( hex1Resource != RESOURCE.INVALID )		//	did we pass in a valid resource for 'hex 1' (if so that means we are picking 'settlement2' instead
			{
				numSettlements = 1;
			}
			return ( numSettlements );
		}

		public	Point[]	GetSettlementTopChoices( int maxCandidatesSize )
		{
			Point[]	candidatesSort = new Point[maxCandidatesSize];
			RESOURCE hex1Resource = _GetHex1Resource();
			int numSettlements = _InferNumSettlementsByHex1Resource( hex1Resource );
			_DoSettlementWeighting( mHexInfo, hex1Resource, numSettlements, maxCandidatesSize, null, candidatesSort );
			return ( candidatesSort );
		}

		public  int		DoSettlementWeighting( HexInfo[] hexInfoArray, RESOURCE hex1Resource, int[] destinationVals, System.Drawing.Point[] optionalList )
		{
			int numSettlements = _InferNumSettlementsByHex1Resource( hex1Resource );
			int topCandidatesListSize = mLogicKernel.GetStartSettlementSortListMax( numSettlements );

			return ( _DoSettlementWeighting( hexInfoArray, hex1Resource, numSettlements, topCandidatesListSize, destinationVals, optionalList ) );
		}

		public  int		_DoSettlementWeighting( HexInfo[] hexInfoArray, RESOURCE hex1Resource, int numSettlements, int topCandidatesListSize, int[] destinationVals, System.Drawing.Point[] optionalList )
		{
			int[]		hexValues = new int[mHexInfo.Length];
			int			portIndex, adjHexId, availPorts, maxPorts, hexId, hexValueSum = 0, numBuildLocs, hexValue, resourceDieRoll, numPips, spineLoop;
			COORD_TYPE	coordType;
			RESOURCE	hexResource, tempResource, portResource;
			DIR			tempDir;
			BuildLoc	portBuildLoc;
			CITY_DIR	dir1, dir2;
			HexInfo		adjHexInfo;

			foreach( HexInfo hexInfo in hexInfoArray )
			{
				hexId = hexInfo.GetUniqueId();
				coordType = hexInfo.GetCoordType();
				resourceDieRoll = hexInfo.GetDieRoll();
				numPips = Support.GetDieRollPips( resourceDieRoll );
				if ( ( hexInfo.IsDesert() ) ||						//	never contemplate a desert location 
				    ( hexInfo.HasOccupiedHex( mWhichSide ) ) )		//	or put a second initial settlement where the first one is
				{
					continue;		//	can never select desert to build on...
				}
				hexResource = hexInfo.GetEarnResource();
				hexValue = 0;		//	set to zero by default...
				if ( mLogicKernel.GetStartSettlementHexTypeVal( coordType, numSettlements, ref hexValue ) )			//	get base value for hexType, continue if not zero
				{
				  if ( mLogicKernel.ScaleStartSettlementByPips( numSettlements, numPips, ref hexValue ) )			//	scale by die roll, continue if not zero
				  {
				    if ( mLogicKernel.ScaleStartSettlementByResource( numSettlements, hexResource, hex1Resource, ref hexValue ) )	//	scale by resource, continue if not zero
				    {
						bool[]	availBuildLocs = new bool[(int)CITY_DIR._size];
						for ( spineLoop = 0; spineLoop < (int)DIR._size; ++spineLoop )
						{
							tempDir = (DIR)spineLoop;														//	convert loop into adjacent hex dir
							Support.GetAssocCityDir( tempDir, out dir1, out dir2 );							//	get two associated build corners for hex dir
							availBuildLocs[(int)dir1] = hexInfo.GetBuildLoc( dir1 ).IsValidBuildLoc();		//	build up table whether those locations are valid
							availBuildLocs[(int)dir2] = hexInfo.GetBuildLoc( dir2 ).IsValidBuildLoc();		//	write to both corners
							if ( ( availBuildLocs[(int)dir1] ) || ( availBuildLocs[(int)dir2] ) )			//	if EITHER build loc is valid, then consider pips
							{
								adjHexId = hexInfo.GetAdjacentHexId( tempDir );								//	get adjacent hex id in that direction
								if ( adjHexId != -1 )														//	if adjacent hex is ocean, we don't apply the mod
								{
									adjHexInfo = mHexInfo[adjHexId];										//	get adjacent hex info
									if ( adjHexInfo.GetTerrain() != TERRAIN.DESERT )						//	don't process desert for pips, there are non
									{
										resourceDieRoll = adjHexInfo.GetDieRoll();							//	get die roll for adjacent hex
										numPips = Support.GetDieRollPips( resourceDieRoll );				//	convert THAT adjacent die roll to # of pips
										mLogicKernel.AdjustStartSettlementByAdjPips( numSettlements, numPips, ref hexValue );
									}
								}
							}
						}
						numBuildLocs = 0;																	//	clear counter before we search the booleans
						for ( spineLoop = 0; spineLoop < (int)DIR._size; ++spineLoop )						//	now see how many of the build corners are valid
						{
							if ( availBuildLocs[spineLoop] )
							{
								++numBuildLocs;																//	each valid corner increases count to maximum '6'
							}
						}
						if ( mLogicKernel.ScaleStartSettlementByAvailLocs( numSettlements, numBuildLocs, ref hexValue ) )	//	scale by valid corners, continue if not zero
						{
							maxPorts = hexInfo.GetNumPorts();												//	find out how many ports maximum are supported
							availPorts = 0;																	//	assume all 'taken' by default
							tempResource = RESOURCE.INVALID;												//	ensure this has invalid setting first time through loop
							for ( portIndex = 0; portIndex < maxPorts; ++portIndex )						//	traverse through the available ports
							{
								portBuildLoc = hexInfo.GetPortBuildLoc( portIndex );						//	grab the port build location
								portResource = portBuildLoc.GetPortResource();								//	get port resource even if its not available
								if ( availBuildLocs[(int)portBuildLoc.GetCityDir()] )						//	can we still build on this port location?
								{
Debug.Assert( portResource != RESOURCE.INVALID );
									if ( portResource == tempResource )										//	if this matches, we only have ONE port on two shared hexes, vs two different ports
									{
										--maxPorts;															//	reduce the # of maximum ports from 2 to 1 if its a shared edge port
									}
									else
									{
										++availPorts;														//	if so, track how many ports are valid
										mLogicKernel.ScaleStartSettlementByPortResource( numSettlements, hexResource, hex1Resource, portResource, ref hexValue );	// scale by port resources
									}
								}
								tempResource = portResource;												//	set this for second time through loop, if applicable
							}
							if ( mLogicKernel.ScaleStartSettlementByAvailPorts( numSettlements, maxPorts, availPorts, ref hexValue ) )	//	scale by # avail ports, continue if not zero
							{
								hexValues[hexId] = hexValue;	//	if we got all the way here, we have a non zero value for the hex evaluation
								hexValueSum += hexValue;		//	build sum of all values
							}
						}
				    }
				  }
				}
			}
			int	selectedIndex = _DoValueEntitlement( hexValues, topCandidatesListSize, optionalList );
Debug.Assert( selectedIndex != -1 );

			if ( destinationVals != null )
			{
				for ( int i = 0; i < hexValues.Length; ++i )
				{
					destinationVals[i] = hexValues[i];
				}
			}
			return ( selectedIndex );
		}

		public	CITY_DIR	GetSettlementLoc( int hexUniqueId, int[] optionalAdjVals, int[] optionalCityDirVals )
		{
			int			loop = 0, adjHexId, adjHexVal, adjNumPips, dirSumVal, halfVal;
			DIR			dirEnum;
			HexInfo		adjHexInfo, hexInfo = mHexInfo[hexUniqueId];
			RESOURCE	portResource, adjHexResource, hexResource = hexInfo.GetEarnResource();
			int[]		cornerVals = new int[(int)CITY_DIR._size];
			CITY_DIR	dir1, dir2, cityDir;
			BuildLoc	builLoc;

			for ( ; loop < (int)DIR._size; ++loop)
			{
				dirEnum = (DIR)loop;
				cityDir = (CITY_DIR)loop;
				builLoc = hexInfo.GetBuildLoc( cityDir );
				if ( ( builLoc.IsValidBuildLoc() ) &&
				     ( !builLoc.IsOccupied() ) )
				{
					adjHexId = hexInfo.GetAdjacentHexId( dirEnum );
					if ( adjHexId != -1 )
					{
						adjHexInfo = mHexInfo[adjHexId];
						adjHexResource = adjHexInfo.GetEarnResource();
						if ( adjHexResource != RESOURCE.INVALID )	//	don't process check if 'Desert'
						{
							adjNumPips = Support.GetDieRollPips( adjHexInfo.GetDieRoll() );
							adjHexVal = mLogicKernel.GetAdjacentHexValue( hexResource, adjHexResource, adjNumPips );
							Support.GetAssocCityDir( dirEnum, out dir1, out dir2 );
							halfVal = ( adjHexVal / 2 );
							dirSumVal = 0;
							builLoc = hexInfo.GetBuildLoc( dir1 );
							if ( builLoc.IsValidBuildLoc() )
							{
								cornerVals[(int)dir1] += adjHexVal;
								dirSumVal += halfVal;
							}
							builLoc = hexInfo.GetBuildLoc( dir2 );
							if ( builLoc.IsValidBuildLoc() )
							{
								cornerVals[(int)dir2] += adjHexVal;
								dirSumVal += halfVal;
							}
							if ( optionalAdjVals != null )	
							{ 
								optionalAdjVals[loop] = dirSumVal;
							}
						}
					}
				}
			}

			int				cityEval, hexesSharingPort, numPorts = hexInfo.GetNumPorts();
			BuildLoc		portLoc;
			SettlementLoc	portSettlementLoc;
			for ( loop = 0; loop < numPorts; ++loop )
			{
				portLoc = hexInfo.GetPortBuildLoc( loop );
				if ( portLoc.IsValidBuildLoc() )			//	is this port still available for settlements?
				{ 
					portSettlementLoc = portLoc.GetSettlementLoc();
					portResource = portLoc.GetPortResource();
					hexesSharingPort = portSettlementLoc.GetNumHexesShareSettlement();
					cityEval = mLogicKernel.GetPortCornerEval( hexesSharingPort );
					cityEval += mLogicKernel.GetPortResourceEval( hexResource, portResource );

					cityDir = portLoc.GetCityDir();
					cornerVals[(int)cityDir] += cityEval;
				}
			}
			int largestVal = 0;
			int largestIndex = -1;
			for ( loop = 0; loop < (int)CITY_DIR._size; ++loop)
			{
				cityEval = cornerVals[loop];
				if ( cityEval > largestVal )
				{
					largestIndex = loop;
					largestVal = cityEval;
				}
				if ( optionalCityDirVals != null )	
				{ 
					optionalCityDirVals[loop] = cityEval;
				}
			}

			CITY_DIR	buildDir = (CITY_DIR)largestIndex;
			return ( buildDir );
		}

		private	BuildLoc	_GetBuildLoc( int wantedIndex )
		{
			BuildLoc	buildLoc = null;
			if ( mBuildLocs.Count != 0 )
			{
				buildLoc = (BuildLoc)mBuildLocs[wantedIndex];
			}
			return ( buildLoc );
		}

		public	void	ConfirmRoadway( int settlementId, CITY_DIR roadDir )
		{
			AdjustAssetCount( ASSET.ROAD, 1 );

			mMessageCtr.SendMsgAddRoadWay( mWhichSide, settlementId, roadDir );
			mMessageCtr.SendMsgMessageHandled( mWhichSide, MessageType.PickRoadWay, -1 );
		}

		public	void	ConfirmSettlement( int hexLocId, CITY_DIR cityDir )
		{
			HexInfo		hexInfo = mHexInfo[hexLocId];
			BuildLoc	buildLoc = hexInfo.GetBuildLoc( cityDir );
Debug.Assert( buildLoc.IsValidBuildLoc() );
			int			settlementId = buildLoc.GetSettlementLoc().GetUniqueId();

			mBuildLocs.Add( buildLoc );		//	
			int			numSettlements = AdjustAssetCount( ASSET.SETTLEMENT, 1 );
//Remember, settlements may be shared across hex sides, so it makes no sense to broadcast 'hex' and 'corner' since there may be several
			mMessageCtr.SendMsgAddSettlement( mWhichSide, settlementId, numSettlements );
			mMessageCtr.SendMsgMessageHandled( mWhichSide, MessageType.PickSettlement, numSettlements );
		}

		public	override	void	MsgPickSettlement( int msgTime, OWNER side )
		{
			if ( mIsUserControlled )
			{
				mMessageCtr.SendMsgStateRequest( mWhichSide, PlayGameMgr.STATE.PICK_BUILD_HEX, ( mNumAssetsUsed[(int)ASSET.SETTLEMENT] + 1 ) );
			}
			else
			{
				RESOURCE	hex1Resource = _GetHex1Resource();
				int			hexLocId = DoSettlementWeighting( mHexInfo, hex1Resource, null, null );
				CITY_DIR	cityDir = GetSettlementLoc( hexLocId, null, null );

				ConfirmSettlement( hexLocId, cityDir );
			}
		}

		public	override	void	MsgPickRoadWay( int msgTime, OWNER side )
		{
			int				roadwayIndex = GetNumAssets( ASSET.ROAD );
			BuildLoc		initialBuildLoc = _GetBuildLoc( roadwayIndex );		//	get build loc based on roads built (only works for first two roads)
			SettlementLoc	settlementLoc = initialBuildLoc.GetSettlementLoc();	//	get settlement loc attached
			if ( mIsUserControlled )
			{
				mMessageCtr.SendMsgStateRequest( mWhichSide, PlayGameMgr.STATE.PICK_ROAD_WAY_LOC, settlementLoc.GetUniqueId() );
			}
			else
			{
				RESOURCE	hexResoure = mHexInfo[initialBuildLoc.GetHexAssocId()].GetEarnResource();
				RESOURCE	targetResource;
				int			minDiePips;
				PATHWAY_TYPE roadWayPath = mLogicKernel.GetInitialSettlementPathTarget( hexResoure, out targetResource, out minDiePips );
				int			pathWayParamVal = (int)targetResource;	//	assume its a port resource or hex resource call by default
				if ( roadWayPath == PATHWAY_TYPE.DIE_ROLL_PIPS )
				{
					pathWayParamVal = minDiePips;
				}
				PathWay	pathWay = mPathFinder.PlotPathway( mHexInfo, settlementLoc.GetUniqueId(), roadWayPath, pathWayParamVal );
				ConfirmRoadway( pathWay.GetSourceLocId(), pathWay.GetRoadDir() );
			}

		}

		public	override	void	MsgAddRoadWay( int msgTime, OWNER whichSide, int settlementId, CITY_DIR whichDir )
		{
			_AddRoadWay( whichSide, settlementId, whichDir );
		}

		public	override	void	MsgAddSettlement( int msgTime, OWNER whichSide, int settlementId, int numActive )
		{
			_AddSettlement( whichSide, settlementId );
		}

		private void	_SetBuildLocRoadWayOwner( HexInfo[] hexInfos, SettlementLoc settlementLoc, CITY_DIR exitDir, OWNER whichSide )
		{
			int			i;
			CITY_DIR	cityDir;
			BuildLoc	buildLoc;
			bool		foundLoc;
			foreach ( HexInfo hexInfo in hexInfos )
			{
				foundLoc = false;
				for ( i = 0; i < (int)CITY_DIR._size; ++i )
				{
					cityDir = (CITY_DIR)i;
					buildLoc = hexInfo.GetBuildLoc( cityDir );
					if ( buildLoc.GetSettlementLoc() == settlementLoc )
					{
						buildLoc.SetRoadWayOwner( exitDir, whichSide );
						foundLoc = true;
						break;
					}
				}
Debug.Assert( foundLoc );
			}
		}

		private void	_SetBuildLocOwner( HexInfo[] hexInfos, SettlementLoc settlementLoc, OWNER whichSide )
		{
			int			i;
			CITY_DIR	cityDir;
			BuildLoc	buildLoc;
			bool		foundLoc;
			foreach ( HexInfo hexInfo in hexInfos )
			{
				foundLoc = false;
				for ( i = 0; i < (int)CITY_DIR._size; ++i )
				{
					cityDir = (CITY_DIR)i;
					buildLoc = hexInfo.GetBuildLoc( cityDir );
					if ( buildLoc.GetSettlementLoc() == settlementLoc )
					{
						buildLoc.SetOwner( whichSide );
						foundLoc = true;
						break;
					}
				}
Debug.Assert( foundLoc );
			}
		}

		private HexInfo[]	_GetHexInfoForSettlement( SettlementLoc settlementLoc )
		{
			int numAdjSettlements = settlementLoc.GetNumHexesShareSettlement();
			HexInfo[]	hexInfos = new HexInfo[numAdjSettlements];
			for ( int i = 0; i < numAdjSettlements; ++i )
			{
				hexInfos[i] = mHexInfo[ settlementLoc.GetShareSettlementHexId( i ) ];
			}
			return ( hexInfos );
		}

		private SettlementLoc	_SetSettlementLocOwner( OWNER whichSide, int settlementId )
		{
			SettlementLoc	settlementLoc = MapDefs.GetSettlementLoc( settlementId );
			HexInfo[]		hexes = _GetHexInfoForSettlement( settlementLoc );
			_SetBuildLocOwner( hexes, settlementLoc, whichSide );
			return ( settlementLoc );
		}

		private	void	_AddRoadWay( OWNER whichSide, int settlementId, CITY_DIR roadDir)
		{
// set all the road links (in each distinct buildloc attached to <x> adjacent hexes) based on source values
			SettlementLoc	settlementLoc = MapDefs.GetSettlementLoc( settlementId );
			HexInfo[]		hexes = _GetHexInfoForSettlement( settlementLoc );
			_SetBuildLocRoadWayOwner( hexes, settlementLoc, roadDir, whichSide );
// now grab all the settlements on the END of the connection, reverse the road dir, and connect them to 'source'
			int				adjSettlementId = settlementLoc.GetAdjacentLocId( roadDir );
			SettlementLoc	adjSettlementLoc = MapDefs.GetSettlementLoc( adjSettlementId );
			HexInfo[]		adjHexes = _GetHexInfoForSettlement( adjSettlementLoc );
			CITY_DIR		oppositeDir = Support.GetOppositeCityDir( roadDir );
			_SetBuildLocRoadWayOwner( adjHexes, adjSettlementLoc, oppositeDir, whichSide );
		}

		private	void	_AddSettlement( OWNER whichSide, int settlementId )
		{
// settlement locations gets true 'ownership' of 'whichSide'
			SettlementLoc	settlementLoc = _SetSettlementLocOwner( whichSide, settlementId );
// all (2 or 3) adjacent settlement locations get 'ownership' of "_size' to put them off limits to buildings.
			int				adjSettlementId, numAdjacentSettlements = settlementLoc.GetNumAdjacentLocIds();
			CITY_DIR		ignoreDir;
			for ( int i = 0; i < numAdjacentSettlements; ++i )
			{
				adjSettlementId = settlementLoc.GetAdjacentLocId( i, out ignoreDir );
				_SetSettlementLocOwner( OWNER._size, adjSettlementId );
			}
		}


	}
}
