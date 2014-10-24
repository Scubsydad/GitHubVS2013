using System;
using System.Collections;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	partial class SideLogic
	{
		private void	_DoResourceAnalysis()
		{
			int[]		ourCurrentResources = mNumRescources[(int)mWhichSide];			//	extract resources out of our tracking array for 'ease of use'
			ASSET		buildType;
			Asset[]		assetCost;
			int			havePct, sumPct, costLoop, numDiffResources, numNeeded;
			bool		haveSufficientResources = false;
			RESOURCE	requiredResource;
			mBuildPcts = new int[(int)ASSET._size][];

			for ( int assetLoop = 0; assetLoop < (int)ASSET._size; ++assetLoop )
			{
				buildType = (ASSET)assetLoop;											//	convert loop to enum type for function call
				mBuildPcts[assetLoop] = new int[] { -1, -1, -1, -1, -1, 0 };			//	5 -1 indicating resource not applicable, and zero for '% have resources' sum
				assetCost = Support.GetCostToBuild( buildType );						//	ask support class to deliver array of cost items
				haveSufficientResources = true;											//	assume we DO have enough of this type by default
				numDiffResources = assetCost.Length;
				for ( sumPct = costLoop = 0; costLoop < numDiffResources; ++costLoop )
				{
					requiredResource = assetCost[costLoop].GetResource();				//	ask for resource type
					numNeeded = assetCost[costLoop].GetQuantity();						//	how many do you need?
					havePct = ( ( ourCurrentResources[(int)requiredResource * 100 ) / numNeeded );
					if ( havePct < 100 )
					{
						haveSufficientResources = false;
					}
					else																//	if enough resources exist, track the 100% + value
					{
						sumPct += havePct;
					}
					mBuildPcts[assetLoop][(int)requiredResource] = havePct;				//	
				}
				if ( haveSufficientResources )											//	if we have enough, then track that we can purchase it and see what else we can do...
				{
					sumPct /= numDiffResources;											//	divide the sum by the quantity present
					mBuildPcts[assetLoop][(int)RESOURCE._size] = sumPct;				//	store how many may be built with the current resources
				}
			}
		}

		private void	_DoBuildAnalysis()
		{
		
		}

		private void	_DetermineTradePriority()
		{
		
		}

		private void	_DoVictoryAnalysis()
		{
		
		}

		private void	_DoRoadWayPlotting()
		{
		
		}	
	}
}