using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace Settlers_of_Catan
{
	public	enum PATHWAY_TYPE
	{
		CLOSEST_PORT_ANY,
		CLOSEST_PORT_TYPE,
		CLOSEST_RESOURCE,
		MIDDLE_OF_MAP,
		COASTAL_EDGE,
		DIE_ROLL_PIPS,
	};

	public class PathWay
	{


		public PathWay( PATHWAY_TYPE pathType, int sourceLocId, int destLocId, int distance, CITY_DIR initialDir )
		{
			mPathType = pathType;
			mSourceId = sourceLocId;
			mTargetId = destLocId;
			mDistance = distance;
			mInitialDir = initialDir;
		}

		public	CITY_DIR	GetRoadDir()		{ return ( mInitialDir ); }
		public	int			GetSourceLocId()	{ return ( mSourceId ); }

		private int				mSourceId;
		private	int				mTargetId;
		private int				mDistance;
		private CITY_DIR		mInitialDir;
		private PATHWAY_TYPE	mPathType;
	};

	public class PathFinder
	{
		private int[]		mCoastalIds = new int[] {  0,  1, 14, 15, 25, 26, 27, 38, 39, 47, 48, 49, 50, 51, 52, 
													  53, 45, 46, 35, 36, 37, 23, 24, 11, 12, 13,  8,  9,  4,  5 };
		private int[]		mGenericPortIds;
		private int[]		mCenterMapIds = new int[] { 18, 30, 31, 32, 20, 19 };
		private int[][]		mResourceLocIds = new int[(int)RESOURCE._size][];
		private int[][]		mDieRollPipsIds = new int[6][];		//	this is 'one based' from '1' to '5' (zero ignored)
		private int[][]		mResourcePortIds = new int[6][];	//	varies by map generation, 5 ports have specific resources
		private int[]       mEmptySearchBuffer = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };	//	init the buffer once so we can reproduce it quickly over and over

		public	PathFinder( HexInfo[] hexLocInfo )
		{
			mResourceLocIds[(int)RESOURCE.BRICK] = new int[18];		//	3 hill hexes containing 'brick' resource
			mResourceLocIds[(int)RESOURCE.GRAIN] = new int[24];		//	4 fields hexes containing 'grain' resource
			mResourceLocIds[(int)RESOURCE.LUMBER] = new int[24];	//	4 forest hexes containing 'lumber' resource
			mResourceLocIds[(int)RESOURCE.ORE] = new int[18];		//	3 mountain hexes containing 'ore' resource
			mResourceLocIds[(int)RESOURCE.WOOL] = new int[24];		//	4 pasture hexes containing 'wool' resource

			mGenericPortIds = new int[8];							//	4 ports are 'generic'
			mResourcePortIds[(int)RESOURCE.BRICK] = new int[2];		//	there is only 1 port per resource type
			mResourcePortIds[(int)RESOURCE.GRAIN] = new int[2];		
			mResourcePortIds[(int)RESOURCE.LUMBER] = new int[2];	
			mResourcePortIds[(int)RESOURCE.ORE] = new int[2];		
			mResourcePortIds[(int)RESOURCE.WOOL] = new int[2];		
			mResourcePortIds[(int)RESOURCE._size] = new int[10];	//	all port ids regardless of type tracked in this one array

			mDieRollPipsIds[1] = new int[12];						//	only 2 hexes contain 1 pip
			mDieRollPipsIds[2] = new int[24];						//	all other pips contain 4 hexes
			mDieRollPipsIds[3] = new int[24];
			mDieRollPipsIds[4] = new int[24];
			mDieRollPipsIds[5] = new int[24];

			HexInfo		hexInfo;
			RESOURCE	hexResource, portResource;
			BuildLoc[]	buildLocs;
			int[]		resourceHexFound = new int[(int)RESOURCE._size];	// just used as counters to seed the various 'hexId' vals
			int[]		resourcePortsFound = new int[(int)RESOURCE._size];	// just used as counters to seed the various 'hexId' vals
			int[]		pipHexFound = new int[6];							//	track pip hexes as 'one based', there is no 'zero pip' hex
			int			genericPortsFound = 0, numResourcePortsFound = 0;
			int			dieRollPips, settlementId;

			for ( int i = 0; i < hexLocInfo.Length; ++i )
			{ 
				hexInfo = hexLocInfo[i];
				hexResource = hexInfo.GetEarnResource();			//	get resource for this location
				buildLocs = hexInfo.GetBuildLocs();					//	grab the build locs once at start of loop

				if ( hexResource != RESOURCE.INVALID )				//	only process this particular code if its NOT the 'desert' item
				{
Debug.Assert( hexInfo.GetTerrain() != TERRAIN.DESERT );				//	just ensure we never have a 'desert' with valid terrain
					dieRollPips = Support.GetDieRollPips( hexInfo.GetDieRoll() );	//	get die roll pips from 1 to 5
					foreach( BuildLoc buildLoc in buildLocs )
					{
						settlementId = buildLoc.GetSettlementLoc().GetUniqueId();	//	grab settlement id
// store settlement id based on it's resource type
						mResourceLocIds[(int)hexResource][resourceHexFound[(int)hexResource]++] = settlementId;
// store settlement id based on die roll pips
						mDieRollPipsIds[dieRollPips][pipHexFound[dieRollPips]++] = settlementId;
					}
				}
				foreach( BuildLoc buildLoc in buildLocs )					//	second loop to process ports seperately from resources
				{
					portResource = buildLoc.GetPortResource();						//	ask if this location has a port resource
					if ( portResource != RESOURCE.INVALID )
					{
						settlementId = buildLoc.GetSettlementLoc().GetUniqueId();	//	grab settlement id
						if ( portResource == RESOURCE._size )						//	is this a generic port location?
						{
							_CheckAddSettlementId( settlementId, ref mGenericPortIds, ref genericPortsFound );
						}
						else
						{
							_CheckAddSettlementId( settlementId, ref mResourcePortIds[(int)portResource], ref resourcePortsFound[(int)portResource] );
							_CheckAddSettlementId( settlementId, ref mResourcePortIds[(int)RESOURCE._size], ref numResourcePortsFound );
						}
					}
				}
			}
		}

		private void	_CheckAddSettlementId( int uniqueId, ref int[] intArray, ref int numInBufferCount )
		{
			for ( int i = 0; i < numInBufferCount; ++i )
			{
				if ( intArray[i] == uniqueId )
				{
					return;
				}
			}
			intArray[numInBufferCount++] = uniqueId;
		}

		public	PathWay	PlotPathway( HexInfo[] hexInfos, int targetSettlementId, PATHWAY_TYPE pathWayType, int pathWayParamVal )
		{
			int[]	originSettlementIds = null;		//	define var to contain copy of integer array for settlementIds
			switch ( pathWayType )
			{
				case PATHWAY_TYPE.CLOSEST_PORT_ANY	:	originSettlementIds = mGenericPortIds;						break;
				case PATHWAY_TYPE.CLOSEST_PORT_TYPE	:	originSettlementIds = mResourcePortIds[pathWayParamVal];	break;
				case PATHWAY_TYPE.CLOSEST_RESOURCE	:	originSettlementIds = mResourceLocIds[pathWayParamVal];		break;
				case PATHWAY_TYPE.COASTAL_EDGE		:	originSettlementIds = mCoastalIds;							break;
				case PATHWAY_TYPE.DIE_ROLL_PIPS		:	originSettlementIds = mDieRollPipsIds[pathWayParamVal];		break;
				case PATHWAY_TYPE.MIDDLE_OF_MAP		:	originSettlementIds = mCenterMapIds;						break;
				default								:	Debug.Assert( false );										break;
			}
			SettlementLoc	settlementLoc = MapDefs.GetSettlementLoc( targetSettlementId );	//	get location we want to plot pathway to
			int				numTravelDirs = settlementLoc.GetNumAdjacentLocIds();			//	find out how many basic dirs this loc has
			int[]			adjSettlementIds = new int[numTravelDirs];
			CITY_DIR[]		adjTravelDirs = new CITY_DIR[numTravelDirs];					//	define storage for adj info
			int				i = 0;
			for ( ; i < numTravelDirs; ++i )												//	extract basic details about adjacent locs
			{
				adjSettlementIds[i] = settlementLoc.GetAdjacentLocId( i, out adjTravelDirs[i] );
			}
// the purpose of these items is just to simplify/make legible the 'while loop' condition more readily
			int				adjId1 = adjSettlementIds[0];									//	all hexes automatically have at least two adjacent spots
			int				adjId2 = adjSettlementIds[1];
			int				adjId3 = adjSettlementIds[0];									//	in cases of only 2, '3' and '1' will point to same location for loop purposes
			if ( numTravelDirs == 3 )
			{
				adjId3 = adjSettlementIds[2];												//	in cases of 3, use real value
			}
// now run the search algorithm on all the available locations
			ArrayList		distanceSummaries = new ArrayList();							//	tracking for each of the origin locations
			int				numItemsToTest = originSettlementIds.Length;					//	get var for loop size
			int				j, distance, originSettlementId;
			int				searchMin, searchIndex, sourceIndex, destIndex;
			int[]			tempSearchBuffer;
			SettlementLoc	scanSettlementLoc;
			ArrayList[]		expansionLocs;													//	initialize two array lists, one for source, one for dest

			for ( j = 0; j < numItemsToTest; ++j )
			{
				originSettlementId = originSettlementIds[j];								//	grab the settlement id out of the origin buffer
				if ( originSettlementId == targetSettlementId )								//	if one of the searching items is the destination, then skip it
				{
					distanceSummaries.Add( new Point( 1000, -1 ) );							//	add a bogus plot so we can have a 1 to 1 correlation between array & source origins
					continue;
				}
				scanSettlementLoc = MapDefs.GetSettlementLoc( originSettlementId );
				tempSearchBuffer = (int[])mEmptySearchBuffer.Clone();						//	clone the search buffer we'll initialize

				tempSearchBuffer[targetSettlementId] = 1000;								//	put a large obvious # in the source location so we don't go through it
				tempSearchBuffer[originSettlementId] = 1;									//	the origin always has a distance of 1
				distance = 2;																//	always begin our expansion distance at 2 (adjacent to one)
				expansionLocs = Support.InitMultiDimensionArrayList( 2 );
				sourceIndex = 1;
				destIndex = 0;
				_CheckAddExpansionIds( originSettlementId, distance++, ref expansionLocs[sourceIndex], ref tempSearchBuffer );
				do
				{
					foreach ( int adjLocId in expansionLocs[sourceIndex] )					//	traverse all previously added adj ids
					{
						_CheckAddExpansionIds( adjLocId, distance, ref expansionLocs[destIndex], ref tempSearchBuffer );
					}
					++distance;																//	increment our distance counter
					if ( destIndex == 0 )	{ destIndex = 1; sourceIndex = 0; }				//	always swap these two for each loop
					else					{ destIndex = 0; sourceIndex = 1; }
					expansionLocs[destIndex] = new ArrayList();								//	always erase the expansion buffer for the next time through loup
				} while ( ( tempSearchBuffer[adjId1] == -1 ) ||		//	all 3 adjacent locs need to be written to before we exit loop
						  ( tempSearchBuffer[adjId2] == -1 ) ||
						  ( tempSearchBuffer[adjId3] == -1 ) );		//	remember that 'adj3' & 'adj1' same if only 2 adj locs

				searchMin = 1000;									//	impossibly large value by default
				searchIndex = -1;									//	clear...
				for ( i = 0; i < numTravelDirs; ++i )
				{
					if ( tempSearchBuffer[adjSettlementIds[i]] < searchMin )	//	does this adjacent id have lowest # so far?
					{
						searchMin = tempSearchBuffer[adjSettlementIds[i]];		//	track newest min
						searchIndex = i;										//	track loop iterator
					}
				}
				distanceSummaries.Add( new Point( searchMin, (int)adjTravelDirs[searchIndex] ) );
			}

			searchMin = 1000;									//	impossibly large value by default
			Point		searchVals;
			CITY_DIR	shortestDir = adjTravelDirs[0];			//	by default, pick the first dir in the list
			numItemsToTest = distanceSummaries.Count;
			searchIndex = -1;									//	set to -1 by default before loop
			for ( i = 0; i < numItemsToTest; ++i )
			{
				searchVals = (Point)distanceSummaries[i];		//	extract the data we kept for each origin point
				if ( searchVals.X < searchMin )					//	does this value have the lowest min so far?
				{
					searchIndex = i;							//	track which location we used as the destination for plot
					searchMin = searchVals.X;					//	track the current min value
					shortestDir = (CITY_DIR)searchVals.Y;		//	extract the direction associated with this 'min'
				}
			}
			PathWay	pathWay = new PathWay( pathWayType, targetSettlementId, originSettlementIds[searchIndex], searchMin, shortestDir );
			return ( pathWay );									//	return path info to caller
		}

		private void	_CheckAddExpansionIds( int settlementId, int distance, ref ArrayList expansionList, ref int[] tempSearchBuffer )
		{
			SettlementLoc	settlementLoc = MapDefs.GetSettlementLoc( settlementId );		//	get location we want to plot pathway to
			int				i = 0, numTravelDirs = settlementLoc.GetNumAdjacentLocIds();	//	find out how many basic dirs this loc has
			int				adjTravelId;
			CITY_DIR		ignoreDir;
			for ( ; i < numTravelDirs; ++i )												//	extract basic details about adjacent locs
			{
				adjTravelId = settlementLoc.GetAdjacentLocId( i, out ignoreDir );			//	get adjacent ids next to the input param loc
				if ( tempSearchBuffer[adjTravelId] == -1 )									//	does this value still have a -1 in it?
				{
					tempSearchBuffer[adjTravelId] = distance;								//	then replace with a valid distance
					expansionList.Add( adjTravelId );										//	add this id to be processed for next loop of expansion
				}
			}
		}
	}
}
