using System.Drawing;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	public class BuildLoc
	{
		public	BuildLoc( CITY_DIR cityDir, int hexId )
		{
			mCityDir = cityDir;
			mHexAssocId = hexId;
		}

		public	void			SetOwner( OWNER whichSide )
		{
			mWhichSide = whichSide;
			if ( mWhichSide != OWNER._size )		//	ownership sent to '_size' means that it can not contain buildings
			{
				mAsset = ASSET.SETTLEMENT;
			}
			else if ( mWhichSide == OWNER.INVALID )
			{
				mAsset = ASSET.INVALID;
			}
		}

		public	int				GetHexAssocId()
		{
			return ( mHexAssocId );
		}

		public	bool			IsValidBuildLoc()
		{
			return ( mWhichSide == OWNER.INVALID );
		}

		public	SettlementLoc	GetSettlementLoc()
		{
			return ( mSettlementLoc );
		}

		public	void		SetSettlementLoc( SettlementLoc settlementLoc )
		{
			mSettlementLoc = settlementLoc;
		}

		public	bool		HasPort()
		{
			bool hasPort = ( GetPortResource() != RESOURCE.INVALID );
			return ( hasPort );
		}

		public	void		SetPortResource( RESOURCE portResource )
		{
			mPortResource = portResource;
		}

		public	RESOURCE	GetPortResource()
		{
			return ( mPortResource );
		}

		public	OWNER		GetOwner()
		{
			return ( mWhichSide );
		}

		public	bool		IsOccupied()
		{
			bool isOccupied = ( ( mWhichSide >= OWNER.BLUE ) && ( mWhichSide < OWNER._size ) );
			return ( isOccupied );
		}

		public	CITY_DIR	GetCityDir()
		{
			return ( mCityDir );
		}

		public	void		SetRoadWayOwner( CITY_DIR exitDir, OWNER whichSide )
		{
			mRoadWayOwner[(int)exitDir] = whichSide;
		}

		public	OWNER		GetRoadWayOwner( CITY_DIR exitDir )
		{
			return ( mRoadWayOwner[(int)exitDir] );
		}

		public	BuildLoc( BuildLoc origSource )
		{
			mSettlementLoc = origSource.mSettlementLoc;
			mWhichSide = origSource.mWhichSide;
			mAsset = origSource.mAsset;
			mPortResource = origSource.mPortResource;
			mCityDir = origSource.mCityDir;
			mHexAssocId = origSource.mHexAssocId;
			mRoadWayOwner = (OWNER[])origSource.mRoadWayOwner.Clone();
		}
//BRK don't forget to update the copy constructor above if you add any member variables to the class!
		private	SettlementLoc	mSettlementLoc = null;
		private	OWNER			mWhichSide = OWNER.INVALID;
		private	ASSET			mAsset = ASSET.INVALID;
		private RESOURCE		mPortResource = RESOURCE.INVALID;
		private CITY_DIR		mCityDir;
		private int				mHexAssocId;
		private OWNER[]			mRoadWayOwner = new OWNER[(int)CITY_DIR._size] { OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID };
	};

	public class DirLink
	{
		public DirLink(int settlementId1, CITY_DIR whichDir1, int settlementId2, CITY_DIR whichDir2)
		{
			_SetAdjLink( whichDir1, settlementId1 );
			_SetAdjLink( whichDir2, settlementId2 );
		}
		public DirLink(int settlementId1, CITY_DIR whichDir1, int settlementId2, CITY_DIR whichDir2, int settlementId3, CITY_DIR whichDir3)
		{
			_SetAdjLink( whichDir1, settlementId1 );
			_SetAdjLink( whichDir2, settlementId2 );
			_SetAdjLink( whichDir3, settlementId3 );
		}

		private void _SetAdjLink( CITY_DIR whichDir, int settlementId )
		{
Debug.Assert( ( whichDir >= CITY_DIR.NORTH_WEST ) && ( whichDir <= CITY_DIR.WEST ) );
Debug.Assert(( settlementId >= 0 ) && (settlementId <= 53 ) );
Debug.Assert( mLinkLocs[(int)whichDir] == -1 );
			mLinkLocs[(int)whichDir] = settlementId;
			++mNumAdjacentLinks;
		}

		public  int		GetAdjacentLocId( CITY_DIR whichDir )
		{
			return ( mLinkLocs[(int)whichDir] );
		}

		public	int		GetNumAdjacentLocIds()	{ return ( mNumAdjacentLinks ); }
		public  int		GetAdjacentLocId( int index, out CITY_DIR whichDir )
		{
			int		linkId = -1;
			whichDir = CITY_DIR.INVALID;

			for ( int i = 0; i < (int)CITY_DIR._size; ++i )
			{
				if ( mLinkLocs[i] != -1 )
				{
					if ( index == 0 )
					{
						linkId = mLinkLocs[i];
						whichDir = (CITY_DIR)i;
						break;
					}
					else
					{
						--index;
					}
				}
			}
			return ( linkId );
		}

		private int[] mLinkLocs = new int[] { -1, -1, -1, -1, -1, -1 };
		private int		mNumAdjacentLinks = 0;
		
	}
	
	public class SettlementLoc
	{
		public	SettlementLoc( int id, DirLink dirLink, int hexIndex, CITY_DIR cityDir )
		{
			_ClassInit( id, dirLink, PORT.INVALID, new int[] { hexIndex }, new CITY_DIR[] { cityDir } ); 
		}
		public	SettlementLoc( int id, DirLink dirLink, int hexIndex, CITY_DIR cityDir, int hexIndex2, CITY_DIR cityDir2 )
		{
			_ClassInit( id, dirLink, PORT.INVALID, new int[] { hexIndex, hexIndex2 }, new CITY_DIR[] { cityDir, cityDir2 } ); 
		}
		public	SettlementLoc( int id, DirLink dirLink, int hexIndex, CITY_DIR cityDir, int hexIndex2, CITY_DIR cityDir2, int hexIndex3, CITY_DIR cityDir3 )
		{
			_ClassInit( id, dirLink, PORT.INVALID, new int[] { hexIndex, hexIndex2, hexIndex3 }, new CITY_DIR[] { cityDir, cityDir2, cityDir3 } ); 
		}
		public	SettlementLoc( int id, DirLink dirLink, PORT portId, int hexIndex, CITY_DIR cityDir )
		{
			_ClassInit( id, dirLink, portId, new int[] { hexIndex }, new CITY_DIR[] { cityDir } ); 
		}
		public	SettlementLoc( int id, DirLink dirLink, PORT portId, int hexIndex, CITY_DIR cityDir, int hexIndex2, CITY_DIR cityDir2 )
		{
			_ClassInit( id, dirLink, portId, new int[] { hexIndex, hexIndex2 }, new CITY_DIR[] { cityDir, cityDir2 } ); 
		}
		public	SettlementLoc( int id, DirLink dirLink, PORT portId, int hexIndex, CITY_DIR cityDir, int hexIndex2, CITY_DIR cityDir2, int hexIndex3, CITY_DIR cityDir3 )
		{
			_ClassInit( id, dirLink, portId, new int[] { hexIndex, hexIndex2, hexIndex3 }, new CITY_DIR[] { cityDir, cityDir2, cityDir3 } ); 
		}

		private void _ClassInit( int uniqueId, DirLink dirLink, PORT portId, int[] hexes, CITY_DIR[] dirs )
		{
			mUniqueId = uniqueId;
			mPortId = portId;
			mHexIndex = hexes;
			mCityDir = dirs;
			mDirLink = dirLink;
		}
		//public	SettlementLoc( SettlementLoc origSource )
		//{
		//	_ClassInit( origSource.mUniqueId, (int[])origSource.mAdjIds.Clone(), origSource.mPortId, origSource.mScales, (int[])origSource.mHexIndex.Clone(), (CITY_DIR[])origSource.mCityDir.Clone() ); 
		//}

		private	int			mUniqueId;
		private	int[]		mHexIndex;
		private	CITY_DIR[]	mCityDir;
		private	DirLink		mDirLink;
		private  PORT		mPortId;

		public	int			GetUniqueId()							{ return ( mUniqueId ); }
		public  PORT		GetPortId()								{ return ( mPortId ); }
		public	int			GetNumHexesShareSettlement()			{ return ( mHexIndex.Length ); }
		public	int			GetShareSettlementHexId( int index )	{ return ( mHexIndex[index] ); }
		public	CITY_DIR	GetShareSettlementHexDir( int index )	{ return ( mCityDir[index] ); }

		public  int		GetAdjacentLocId( CITY_DIR whichDir )		{ return ( mDirLink.GetAdjacentLocId( whichDir ) ); }
		public	int		GetNumAdjacentLocIds()						{ return ( mDirLink.GetNumAdjacentLocIds() ); }
		public  int		GetAdjacentLocId( int index, out CITY_DIR whichDir )	{ return ( mDirLink.GetAdjacentLocId( index, out whichDir ) ); }

		public	Point		GetSettlementGfxLoc( Size drawingScale )
		{
			return ( GetSettlementGfxLoc( drawingScale, -1 ) );
		}

		public	Point		GetSettlementGfxLoc( Size drawingScale, int targetHexId )
		{
			int hexArrayIndex = 0;
			if ( targetHexId != -1 )		//	if we are attached to a specific hex draw, use that one's coordinates
			{
				for ( ; hexArrayIndex < GetNumHexesShareSettlement(); ++hexArrayIndex )
				{
					if ( GetShareSettlementHexId( hexArrayIndex ) == targetHexId )
					{
						break;
					}
				}				
			}
			Point	hexDrawLoc = Support.GetHexBase( GetShareSettlementHexId( hexArrayIndex ) ).GetGfxLoc();
			Point	cityDirOffset = Support.GetCityDirGfxOffset( GetShareSettlementHexDir( hexArrayIndex ) );
			int		combinedX = ( hexDrawLoc.X + cityDirOffset.X );
			int		combinedY = ( hexDrawLoc.Y + cityDirOffset.Y );
			int		scaleX = ( ( drawingScale.Width * 100 ) / 680 );
			int		scaleY = ( ( drawingScale.Height * 100 ) / 680 );
			int		adjustedX = ( ( combinedX * scaleX ) / 100 );
			int		adjustedY = ( ( combinedY * scaleY ) / 100 );

			return ( new Point( adjustedX, adjustedY ) );
		}

		public	void		DrawSettlement( Bitmap destBmap, OWNER whichSide )
		{
			Graphics gfx = Graphics.FromImage( destBmap );
			Point	gfxLoc = GetSettlementGfxLoc( destBmap.Size );

			int scale = (int)((float)destBmap.Width  * 0.0561f);
			int halfScale = ( scale / 2 );
			gfx.DrawImage( Support.GetSettlementBitmap( whichSide ), gfxLoc.X - halfScale, gfxLoc.Y - halfScale, scale, scale );
			//int portId = (int)mPortId;
			//if ( portId != -1 )
			{
				gfx.DrawString( mUniqueId.ToString(), new Font("Sylfaen", 10.0f ), Brushes.White, gfxLoc.X - 5, gfxLoc.Y - 5 );
			}
		}
	}

	public class HexInfo
	{
		public	HexInfo( HexInfo copySource)
		{
			mUniqueId = copySource.mUniqueId;
			mResourceDieRoll = copySource.mResourceDieRoll;
			mTerrain = copySource.mTerrain;
			mHexSpineRoads = new OWNER[] { OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID };
			mEarnResource = copySource.mEarnResource;
			mCoordType = copySource.mCoordType;
			mAdjacentHexes = (int[])copySource.mAdjacentHexes.Clone();
			mBuildLocs = new BuildLoc[(int)CITY_DIR._size];
			for ( int i = 0; i < mBuildLocs.Length; ++i )
			{
				mBuildLocs[i] = new BuildLoc( copySource.mBuildLocs[i] );
			}
		}

		public	HexInfo( HexBase hexBase)
		{
			mUniqueId = hexBase.GetUniqueId();
			mResourceDieRoll = hexBase.GetDieRoll();
			mTerrain = hexBase.GetTerrain();
			mHexSpineRoads = new OWNER[] { OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID, OWNER.INVALID };
			mEarnResource = Support.GetResourceFromTerrain( mTerrain );
			mCoordType = hexBase.GetCoordType();
			mAdjacentHexes = hexBase.GetAdjLocIds();
			mBuildLocs = new BuildLoc[(int)CITY_DIR._size] { new BuildLoc( CITY_DIR.NORTH_WEST, mUniqueId ), new BuildLoc( CITY_DIR.NORTH_EAST, mUniqueId ), 
															 new BuildLoc( CITY_DIR.EAST, mUniqueId ), new BuildLoc( CITY_DIR.SOUTH_EAST, mUniqueId ), 
															 new BuildLoc( CITY_DIR.SOUTH_WEST, mUniqueId ), new BuildLoc( CITY_DIR.WEST, mUniqueId ) };
		}
		public	int				GetUniqueId()					{ return ( mUniqueId ); }
		public	void			SetDieRoll( int dieRoll )		{ mResourceDieRoll = dieRoll; }
		public	int				GetDieRoll()					{ return ( mResourceDieRoll ); }		
		public	int				GetAdjacentHexId( DIR whichDir ){ return ( mAdjacentHexes[(int)whichDir] ); }
		public  bool			IsDesert()						{ return ( mTerrain == TERRAIN.DESERT ); }
		public	void			SetTerrain( TERRAIN newVal )	{ mTerrain = newVal; mEarnResource = Support.GetResourceFromTerrain( mTerrain ); }
		public	TERRAIN			GetTerrain()					{ return ( mTerrain ); }
		public	OWNER			GetRoadOwnership( DIR whichDir ){ return ( mHexSpineRoads[(int)whichDir] ); }
		public	COORD_TYPE		GetCoordType()					{ return ( mCoordType ); }
		public	void			SetEarnResource( RESOURCE res ) { mEarnResource = res; }
		public	RESOURCE		GetEarnResource()				{ return ( mEarnResource ); }
		public	BuildLoc		GetBuildLoc( CITY_DIR cityDir ) { return ( mBuildLocs[(int)cityDir] ); }
		public	BuildLoc		GetPortBuildLoc( int index )	{ return ( GetBuildLoc( _GetPortDir( index ) ) ); }
		private	int				mUniqueId;
		private	int[]			mAdjacentHexes;
		private	int				mResourceDieRoll;
		private	TERRAIN			mTerrain;
		private	OWNER[]			mHexSpineRoads;
		private COORD_TYPE		mCoordType;
		private	RESOURCE		mEarnResource;
		private	BuildLoc[]		mBuildLocs;

		private bool			_MatchesOwner( OWNER whichSide, OWNER compareOwner )
		{
			bool matches = (  (compareOwner == whichSide ) || 
							( ( compareOwner == OWNER._size ) && ( whichSide != OWNER.INVALID ) ) );
			return ( matches );
		}

		public	void			ClearOwnership( OWNER whichSide )
		{
			for ( int i = 0; i < mHexSpineRoads.Length; ++i )
			{
				if ( _MatchesOwner( mHexSpineRoads[i], whichSide ) )
				{
					mHexSpineRoads[i] = OWNER.INVALID;
				}
			}
			foreach ( BuildLoc buildLoc in mBuildLocs )
			{
				if ( _MatchesOwner( buildLoc.GetOwner(), whichSide ) )
				{
					buildLoc.SetOwner( OWNER.INVALID );
				}
			}
		}

		private CITY_DIR		_GetPortDir( int index )
		{
			CITY_DIR	cityDir = CITY_DIR.INVALID;
			int	numPorts = 0;
			for ( int i = 0; i < mBuildLocs.Length; ++i )
			{
				if ( mBuildLocs[i].GetSettlementLoc().GetPortId() != PORT.INVALID )
				{
					if ( numPorts++ == index )
					{
						cityDir = (CITY_DIR)i;
						break;
					}
				}
			}
			return ( cityDir );
		}

		public	BuildLoc[]		GetBuildLocs()
		{
			return ( mBuildLocs );
		}

		public	BuildLoc		GetBuildLoc( SettlementLoc settlementLoc )
		{
			BuildLoc	buildLoc = null;
			for ( int i = 0; i < mBuildLocs.Length; ++i )
			{
				if ( mBuildLocs[i].GetSettlementLoc() == settlementLoc  )
				{
					buildLoc = mBuildLocs[i];
				}
			}
			return ( buildLoc );
		}

		public  bool			HasOccupiedHex( OWNER whichSide )
		{
			bool hasOccupied = false;
			foreach ( BuildLoc buildLoc in mBuildLocs )
			{
				if ( buildLoc.GetOwner() == whichSide )
				{
					hasOccupied = true;
					break;
				}
			}
			return ( hasOccupied);
		}

		public	int				GetNumPorts()
		{
			int	numPorts = 0;
			for ( int i = 0; i < mBuildLocs.Length; ++i )
			{
				if ( mBuildLocs[i].GetSettlementLoc().GetPortId() != PORT.INVALID )
				{
					++numPorts;
				}
			}
			return ( numPorts );
		}

		public	void		SetSettlementLocLink( CITY_DIR whichDir, SettlementLoc settlementLoc)
		{
			mBuildLocs[(int)whichDir].SetSettlementLoc( settlementLoc );
			PORT	portId = settlementLoc.GetPortId();
			if ( portId != PORT.INVALID )
			{
				mBuildLocs[(int)whichDir].SetPortResource( MapManager.GetPortResource( portId ) );
			}
		}
	}

	public class HexBase
	{
		public HexBase ( int id, int dieRoll, TERRAIN defaultTerrain, COORD_TYPE type, int[] adjId , Point gfxLoc )
		{
			mUniqueId = id;
			mDefTerrain = defaultTerrain;
			mDefDieRoll = dieRoll;
			mCoordType = type;
			mAdjLocId = adjId;
			mGfxLoc = gfxLoc;

			RestoreOrigTerrain();
			RestoreOrigDieRoll();
		}


		public	TERRAIN	GetTerrain()
		{
			return ( mCurrTerrain );
		}

		public	int		GetDieRoll()
		{
			return ( mCurrDieRoll );
		}

		public	void	RestoreOrigDieRoll( )
		{
			_SetActiveDieRoll( mDefDieRoll );
		}

		public	void	SetActiveDieRoll( int dieRoll )
		{
			_SetActiveDieRoll( dieRoll );
		}

		public	void	_SetActiveDieRoll( int dieRoll )
		{
			mCurrDieRoll = dieRoll ;
		}

		public	void	RestoreOrigTerrain( )
		{
			SetActiveTerrain( mDefTerrain );
		}

		public	void	SetActiveTerrain( TERRAIN newTerrain )
		{
			mCurrTerrain = newTerrain ;
			if ( mCurrTerrain == TERRAIN.DESERT )
			{
				_SetActiveDieRoll( 7 );	//	call the private function to ensure the die roll is set to 'desert' value by default
			}
		}

		public	int		GetUniqueId( )
		{
			return ( mUniqueId );
		}

		public	int[]	GetAdjLocIds()
		{
			return ( mAdjLocId );
		}
		public	int		GetAdjLocId( DIR whichDir )
		{
			return ( mAdjLocId[(int)whichDir] );
		}

		public	Point	GetGfxLoc( )
		{
			return ( mGfxLoc );
		}

		public COORD_TYPE	GetCoordType()
		{
			return ( mCoordType );
		}


		private	int				mUniqueId;
		private int				mDefDieRoll, mCurrDieRoll;
		private	int[]			mAdjLocId;
		private Point			mGfxLoc;	
		private TERRAIN			mDefTerrain, mCurrTerrain;
		private COORD_TYPE		mCoordType;
		private SettlementLoc[]	mCityLocs = new SettlementLoc[(int)CITY_DIR._size];
	}
}