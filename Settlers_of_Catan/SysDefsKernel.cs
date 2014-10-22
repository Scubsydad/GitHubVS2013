using System;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	public class SysDefsKernel
	{
		private ResourceKernel		mResourceKernel;
		private string				mDieRollPct	= "DieRollPct";

		public SysDefsKernel(	ResourceKernel resKernel )
		{
			mResourceKernel = resKernel;
		}

		public	int	GetResourceDieRoll( )
		{
			int resourceDieRoll;
			do 
			{
				int dieRollVal = Support.GetRand( 1000 );	//	get a die roll from 0 to 1000
				int zeroBasedIndex = mResourceKernel.GetPercentageIntercept( mDieRollPct, 0, dieRollVal );
				resourceDieRoll = ( 2 + zeroBasedIndex );
			} while ( resourceDieRoll == 7 );	//	for now, don't include the 'move robber' die roll until later
			return ( resourceDieRoll );
		}
	}
}
