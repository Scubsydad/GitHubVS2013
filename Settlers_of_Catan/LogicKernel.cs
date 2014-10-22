using System;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for LogicKernel.
	/// </summary>
	public class LogicKernel
	{
		public enum TRACKING_INDEX
		{
			RESOURCES_GAINED,
			RESOURCES_LOST,
		}
		private ResourceKernel		mResourceKernel;
		private string				mPickLocData	= "PickLocData";
		private string				mRoadData0		= "RoadData0";
		private string				mTrackingAcc	= "TrackingAcc";

		public LogicKernel(	ResourceKernel resKernel )
		{
			mResourceKernel = resKernel;
		}

		private string	_GetStartLocDesc( int settlementCount )
		{
			string objectDesc = "StartLocs0";		// by default, assume FOR settlement 1
			if ( settlementCount == 1 )				//	then we are picking for our second settlement
			{
				objectDesc = "StartLocs1";
			}
			return ( objectDesc );
		}

		public	bool	GetStartSettlementHexTypeVal( COORD_TYPE coordType, int settlementCount, ref int value )
		{
			value = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 0, (int)coordType );
			return ( ( value != 0 ) );
		}

		public	bool		ScaleStartSettlementByPips( int settlementCount, int numPips, ref int value )
		{
			int	pipsScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 1, ( 6 - numPips ) );
			value = ( ( value * pipsScale ) / 100 );
			return ( ( value != 0 ) );
		}

		public	bool		ScaleStartSettlementByResource( int settlementCount, RESOURCE earnResource, RESOURCE hex1Resource, ref int value )
		{
			string objectDesc =  _GetStartLocDesc( settlementCount );
			int	resourceScale = mResourceKernel.GetValueFromSegment(objectDesc, 2, (int)earnResource );
			if ( ( settlementCount == 1 ) && ( earnResource == hex1Resource ) )		//	if resource matches first settlement, scale by alternate...
			{
				resourceScale = mResourceKernel.GetValueFromSegment(objectDesc, 9, 0 );	//	use different value if match
			}
			value = ( ( value * resourceScale ) / 100 );
			return ( ( value != 0 ) );
		}

		public	bool		ScaleStartSettlementByAvailLocs( int settlementCount, int availLocs, ref int value )
		{
			int	availLocsScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 3, ( 6 - availLocs ) );
			value = ( ( value * availLocsScale ) / 100 );
			return ( ( value != 0 ) );
		}

		public	void		AdjustStartSettlementByAdjPips( int settlementCount, int adjPips, ref int value )
		{
			int	adjPipsMod = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 4, ( 6 - adjPips ) );
			value += adjPipsMod;
		}

		public	bool		ScaleStartSettlementByAvailPorts( int settlementCount, int maxPorts, int availPorts, ref int value )
		{
			if ( maxPorts != 0 )
			{
				int	valueIndex = 3;					//	assume 1 port and its available by default...
				if ( maxPorts == 2 )				//	does hex support two different ports instead?
				{
					valueIndex = ( 2 - availPorts );//	0 = 2/2, 1 = 1/2, 2 = 0/2
				}
				else // # ports == 1
				{
					if ( availPorts == 0 )			//	if only 1 port is in hex, and its not available...
					{
						valueIndex = 4;				//	use 0/1 value instead
					}
				}
				int	availPortsScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 5, valueIndex );
				value = ( ( value * availPortsScale ) / 100 );
			}
			return ( ( value != 0 ) );
		}

		public	bool		ScaleStartSettlementByPortResource( int settlementCount, RESOURCE hexResource, RESOURCE hex1Resource, RESOURCE portResource, ref int value )
		{
			int	resourceScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 6, (int)portResource );
			if ( hexResource == portResource )	//	if the resource matches the hex resource, use the 'bonus %' instead of standard value
			{
				resourceScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 7, 0 );
			}
			else if ( hex1Resource == portResource )
			{
				resourceScale = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 7, 1 );
			}
			value = ( ( value * resourceScale ) / 100 );
			return ( ( value != 0 ) );
		}

		public	int			GetStartSettlementSortListMax( int settlementCount )
		{
			int	sortSizeMax = mResourceKernel.GetValueFromSegment( _GetStartLocDesc( settlementCount ), 8, 0 );
			return ( sortSizeMax );
		}


		public	int	GetAdjacentHexValue( RESOURCE hexResource, RESOURCE adjResource, int adjPips )
		{
			int scalePct = mResourceKernel.GetValueFromSegment( mPickLocData, 5, ( adjPips - 1 ) );
			int adjResourceVal = mResourceKernel.GetValueFromSegment( mPickLocData, (int)hexResource, (int)adjResource );
			int adjHexValue = ( ( ( scalePct * adjResourceVal ) / 100 ) );
			return ( adjHexValue );
		}

		public	int	GetPortCornerEval( int numSharedHexes )
		{
			int portValue = mResourceKernel.GetValueFromSegment( mPickLocData, 6, ( 2 - numSharedHexes ) );
			return ( portValue );	//	if shared, use first value in segment, if not, use second (should only be 1 or 2)
		}

		public	int	GetPortResourceEval( RESOURCE hexResource, RESOURCE portResource )
		{
			int portValue = 0;			//	by default, assume no match...
			if ( hexResource == portResource )
			{
				portValue = mResourceKernel.GetValueFromSegment( mPickLocData, 7, 0 );
			}
			RESOURCE compareResource = (RESOURCE)mResourceKernel.GetValueFromSegment( mPickLocData, 8, 0 );
			if ( portResource == compareResource )	//	if it matches the wanted resource?
			{
				portValue += mResourceKernel.GetValueFromSegment( mPickLocData, 7, 1 );	//	then use additional modifer
			}
			return ( portValue );
		}

		public PATHWAY_TYPE	GetInitialSettlementPathTarget( RESOURCE hexResource, out RESOURCE targetResource, out int numPips )
		{
			targetResource = RESOURCE.INVALID;
			numPips = -1;
			PATHWAY_TYPE roadWayTarget = (PATHWAY_TYPE)mResourceKernel.GetPercentageIntercept( mRoadData0, 0, Support.GetRandPct() );
			if ( roadWayTarget == PATHWAY_TYPE.CLOSEST_PORT_TYPE )
			{
				targetResource = (RESOURCE)mResourceKernel.GetValueFromSegment( mRoadData0, 1, 0 );
				if ( targetResource == hexResource )
				{
					 targetResource = (RESOURCE)mResourceKernel.GetValueFromSegment( mRoadData0, 1, 1 );
				}
			}
			else if ( roadWayTarget == PATHWAY_TYPE.CLOSEST_RESOURCE )
			{
				targetResource = (RESOURCE)mResourceKernel.GetValueFromSegment( mRoadData0, 2, 0 );
				if ( targetResource == hexResource )
				{
					 targetResource = (RESOURCE)mResourceKernel.GetValueFromSegment( mRoadData0, 2, 1 );
				}
			}
			else if ( roadWayTarget == PATHWAY_TYPE.DIE_ROLL_PIPS )
			{
				numPips = mResourceKernel.GetValueFromSegment( mRoadData0, 3, 0 );
			}
			return ( roadWayTarget );
		}

		public	bool	ConfirmAccurate( OWNER forSide, TRACKING_INDEX whichIndex )	// support function in case we ever need to know if it has a chance of failure or not
		{
			int	trackingPctVal = mResourceKernel.GetValueFromSegment( mTrackingAcc, (int)whichIndex, (int)forSide );
			bool isAccurate = ( trackingPctVal == 100 );
			return ( isAccurate );
		}

		public	bool	CanTrackLogic( OWNER forSide, TRACKING_INDEX whichIndex )
		{
			int	trackingPctVal = mResourceKernel.GetValueFromSegment( mTrackingAcc, (int)whichIndex, (int)forSide );
			bool canTrackLogic = ( Support.GetRandPct() < trackingPctVal );
			return ( canTrackLogic );
		}



	}
}
