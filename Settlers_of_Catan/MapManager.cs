using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	public class MapManager	:	MessageHandler
	{
		private Bitmap[][]		mTerrainArt = new Bitmap[(int)TERRAIN._size][];
		private Bitmap[]		mDieRollArt;
		private Bitmap[]		mPortArt;
		private Bitmap			mOceanBmap = new Bitmap( "art//ocean.png");
		private int[]			mDieRollHits = new int[13];	//	store # of times die rolls 2-12 are used for map
		private int[]			mPortResCount = new int[(int)RESOURCE._size + 1];	//	add '1' so we can store the '3-1' hits also
		private MessageCenter	mMessageCtr;
		private HexInfo[]		mHexInfo;
		private bool			mDidRandomTerrain;
		private Point[]			mPortGfxLocs = new Point[] {  new Point( 310, 1 ), new Point( 500, 109 ), new Point( 609, 252 ), new Point( 609, 490 ), new Point( 400, 600 ), new Point( 222, 600 ), new Point( 13, 492 ), new Point( 13, 289 ), new Point( 120, 109 ) };
		private RESOURCE[]		mPortResOrig = new RESOURCE[] { RESOURCE._size,   RESOURCE.WOOL,		  RESOURCE._size,        RESOURCE._size,        RESOURCE.BRICK,        RESOURCE.LUMBER,       RESOURCE._size,       RESOURCE.GRAIN,       RESOURCE.ORE };
		static RESOURCE[]		mPortResActive = new RESOURCE[] { RESOURCE._size,   RESOURCE.WOOL,		  RESOURCE._size,        RESOURCE._size,        RESOURCE.BRICK,        RESOURCE.LUMBER,       RESOURCE._size,       RESOURCE.GRAIN,       RESOURCE.ORE };

		static public	RESOURCE	GetPortResource( PORT pordId )	{ return ( mPortResActive[(int)pordId] ); }
	
		public	MapManager( MessageCenter msgCenter)	:	base ( OWNER._size, false )
		{
			mMessageCtr = msgCenter;
			mHexInfo = Support.GetHexInfoSource();

			mTerrainArt[(int)TERRAIN.DESERT] = new Bitmap[] { new Bitmap( "art//desert.bmp") };
			mTerrainArt[(int)TERRAIN.FIELDS] = new Bitmap[] { new Bitmap( "art//fields.bmp") };
			mTerrainArt[(int)TERRAIN.FOREST] = new Bitmap[] { new Bitmap( "art//forest.bmp") };
			mTerrainArt[(int)TERRAIN.HILLS] = new Bitmap[] { new Bitmap( "art//hill.bmp") };
			mTerrainArt[(int)TERRAIN.MOUNTAINS] = new Bitmap[] { new Bitmap( "art//mountain.bmp") };
			mTerrainArt[(int)TERRAIN.PASTURE] = new Bitmap[] { new Bitmap( "art//pasture.bmp") };
			foreach ( Bitmap[] bmapArray in mTerrainArt )
			{
				_GfxArrayTrans( bmapArray );
			}
			mDieRollArt = new Bitmap[] { null, null, new Bitmap( "art//roll2.bmp"), new Bitmap( "art//roll3.bmp"), new Bitmap( "art//roll4.bmp"), 
										 new Bitmap( "art//roll5.bmp"), new Bitmap( "art//roll6.bmp"), null, 
										 new Bitmap( "art//roll8.bmp"), new Bitmap( "art//roll9.bmp"), new Bitmap( "art//roll10.bmp"), 
										 new Bitmap( "art//roll11.bmp"), new Bitmap( "art//roll12.bmp") };
			_GfxArrayTrans( mDieRollArt );
			mPortArt = new Bitmap[] { new Bitmap( "art//port21brick.bmp") , new Bitmap( "art//port21grain.bmp"), new Bitmap( "art//port21lumber.bmp"), new Bitmap( "art//port21ore.bmp"), new Bitmap( "art//port21wool.bmp"), new Bitmap( "art//port31all.bmp") }; 
			_GfxArrayTrans( mPortArt );

			int				dirLoop, adjLocId, dieRoll, locId;
			DIR				dirEnum, oppositeDir;
			HexInfo			adjHexLoc;

			foreach ( RESOURCE resource in mPortResActive )
			{
				++mPortResCount[(int)resource];
			}
			foreach( HexInfo hexInfo in mHexInfo )
			{
				locId = hexInfo.GetUniqueId();

				dieRoll = hexInfo.GetDieRoll();
				++mDieRollHits[dieRoll];

				for ( dirLoop = 0; dirLoop < (int)DIR._size; ++dirLoop )
				{
					dirEnum = (DIR)dirLoop;
					oppositeDir = Support.GetOppositeDir( dirEnum );
					adjLocId = hexInfo.GetAdjacentHexId( dirEnum );
					if ( adjLocId != -1 )
					{
						adjHexLoc = mHexInfo[adjLocId];
						adjLocId = adjHexLoc.GetAdjacentHexId( oppositeDir );
						Debug.Assert( adjLocId == locId );

					}
				}
			}
			mDieRollHits[7] = 0;	//	turn this off, we don't want to ever assign 'desert' via our randomization/set logics
		}

		private void	_GfxArrayTrans( Bitmap[] bmaps )
		{
			foreach ( Bitmap bmap in bmaps )
			{
				if ( bmap != null )
				{
					bmap.MakeTransparent();
				}
			}
		}

		public Bitmap	GenerateMapGfx( )
		{
			Bitmap		baseBmap = new Bitmap( 680, 680 );
			Graphics	gfx = Graphics.FromImage( baseBmap );
			gfx.DrawImage( mOceanBmap,   0,   0 );
			gfx.DrawImage( mOceanBmap, 400,   0 );
			gfx.DrawImage( mOceanBmap,   0, 400 );
			gfx.DrawImage( mOceanBmap, 400, 400 );
			gfx.FillRectangle( Brushes.Black, 192, 113, 290, 485 );
			gfx.FillRectangle( Brushes.Black,  82, 174, 510, 363 );

			TERRAIN			whichTerrain;
			Point			gfxOffset;
			int				dieRoll;
			bool[]			drewPort = new bool[(int)PORT._size];
			int				i = 0, numHexes = mHexInfo.Length;
			HexBase	hexBase;
			for ( ; i < numHexes; ++i )
			{
				hexBase = Support.GetHexBase( i );
				whichTerrain = hexBase.GetTerrain();
				gfxOffset = hexBase.GetGfxLoc();
				gfx.DrawImage( mTerrainArt[(int)whichTerrain][0], gfxOffset.X, gfxOffset.Y , 142, 120 );
				dieRoll = hexBase.GetDieRoll();
				if ( mDieRollArt[dieRoll] != null )
				{
					gfx.DrawImage( mDieRollArt[dieRoll], gfxOffset.X + 42, gfxOffset.Y + 35, 50, 50 );
				}
			}
			i = 0;
			RESOURCE		resource;
			foreach ( Point gfxLoc in mPortGfxLocs )
			{
				resource = mPortResActive[i++];
				gfx.DrawImage( mPortArt[(int)resource], gfxLoc.X, gfxLoc.Y, 50, 50 );
			}
					
			return ( baseBmap );
		}

		private void _GeneratePortLocs( bool wantRandomPortRes )
		{
			ArrayList	storageList = new ArrayList();
			for ( int i = 0; i < mPortResOrig.Length; ++i )
			{
				mPortResActive[i] = mPortResOrig[i];
				storageList.Add( mPortResActive[i] );
			}
			if ( wantRandomPortRes )
			{
				int		randPortId, stompIndex = 0, numPortsToAssign = storageList.Count;
				bool[]	portAssigned = new bool[numPortsToAssign];

				while ( numPortsToAssign-- != 0 )
				{
					randPortId = Support.GetRand( storageList.Count );		
					mPortResActive[stompIndex++] = 	(RESOURCE)storageList[randPortId];
					storageList.RemoveAt( randPortId );
				};
			}
		}

		public  HexInfo[]	GenerateTempMap( bool wantRandomTerrain, bool wantRandomDieRolls, bool wantRandomPorts )
		{
			_GenerateTerrain( wantRandomTerrain );
			_GenerateDieRolls( wantRandomDieRolls );
			_GeneratePortLocs( wantRandomPorts );

			HexInfo[]	newHexInfo = new HexInfo[19];
			for ( int i = 0; i < 19; ++i )
			{
				newHexInfo[i] = new HexInfo( Support.GetHexBase( i ) );
			}
			Support.LinkSettlementDataToHexInfo( newHexInfo );

			return ( newHexInfo );
		}

		private void _GenerateDieRolls( bool wantRandomDieRolls )
		{
			int		i, numHexes = mHexInfo.Length;
			if ( !wantRandomDieRolls )
			{ 
				if ( !mDidRandomTerrain )	//	don't touch the die rolls if we did random terrain, the die follows the terrain type by default
				{ 
					for ( i = 0; i < numHexes; ++i )
					{
						Support.GetHexBase( i ).RestoreOrigDieRoll();
					}
				}
			}
			else // if we did random terrain, we can ALSO do 'random die roll' so the dies are not the default (except 7 == desert )
			{
				int[]	dieRollCounts = (int[])mDieRollHits.Clone();
				int		numToAssign = numHexes; 
				int		randDieRoll, randHexIndex = 0;
				bool[]	hexLocUsed = new bool[numToAssign--];	//	 post decrement so we effectively ignore the spot with desert in it

				while ( numToAssign-- != 0 )
				{
					HexBase	hexBase;
					do
					{ 
						randHexIndex = Support.GetRand( numHexes );	
						hexBase = Support.GetHexBase(randHexIndex);
					} while ( ( hexLocUsed[randHexIndex] ) ||
							  ( hexBase.GetTerrain() == TERRAIN.DESERT ) );
					hexLocUsed[randHexIndex] = true;
					do
					{
						randDieRoll = Support.GetRand( 13 );
					} while ( dieRollCounts[randDieRoll] == 0 );
					--dieRollCounts[randDieRoll];
					hexBase.SetActiveDieRoll( randDieRoll );
				};
			}
		}
	
		private void	_GenerateTerrain( bool wantRandomTerrain )
		{
			mDidRandomTerrain = wantRandomTerrain;
			int				i = 0, numHexes = mHexInfo.Length;
			ArrayList		terrainInfo = new ArrayList();
			HexBase	hexBase;
			for ( ; i < numHexes; ++i )
			{
				hexBase = Support.GetHexBase( i );
				hexBase.RestoreOrigTerrain();
				hexBase.RestoreOrigDieRoll();
				terrainInfo.Add( new Point( (int)hexBase.GetTerrain(), hexBase.GetDieRoll() ) );
			}			
			if ( wantRandomTerrain )
			{
				int		numToAssign = numHexes;
				int		randTerrain, randHexIndex;
				bool[]	hexLocUsed = new bool[numToAssign];
				Point	terrainPoint;
				while ( numToAssign-- != 0 )
				{
					do
					{ 
						randHexIndex = Support.GetRand( numHexes );					
					} while ( hexLocUsed[randHexIndex] );
					hexLocUsed[randHexIndex] = true;

					randTerrain = Support.GetRand( terrainInfo.Count );
					terrainPoint = (Point)terrainInfo[randTerrain];
					terrainInfo.RemoveAt( randTerrain );

					hexBase = Support.GetHexBase( randHexIndex );

					hexBase.SetActiveTerrain( (TERRAIN)terrainPoint.X );
					hexBase.SetActiveDieRoll( terrainPoint.Y );
				};
			}
		}

		public override void	MsgInitTerrainRequest( int msgTime, bool wantRandom )
		{
			_GenerateTerrain( wantRandom );

			TERRAIN			terrain;
			int				uniqueId;
			int				numHexes = mHexInfo.Length;
			HexBase	hexBase;
			for ( int i = 0; i < numHexes; ++i )
			{
				hexBase = Support.GetHexBase( i );
				uniqueId = hexBase.GetUniqueId();
				terrain = hexBase.GetTerrain();

				mMessageCtr.SendMsgInitTerrainSet( uniqueId, terrain );
			}
		}

		public override void	MsgInitDieRollRequest( int msgTime, bool wantRandom )
		{
			_GenerateDieRolls( wantRandom );

			int				uniqueId, dieRoll;
			int				numHexes = mHexInfo.Length;
			HexBase	hexBase;
			for ( int i = 0; i < numHexes; ++i )
			{
				hexBase = Support.GetHexBase( i );
				uniqueId = hexBase.GetUniqueId();
				dieRoll = hexBase.GetDieRoll();

				mMessageCtr.SendMsgInitDieRollSet( uniqueId, dieRoll );
			}
		}

		public override void	MsgInitPortLocRequest( int msgTime, bool wantRandom )
		{
			_GeneratePortLocs( wantRandom );

			int		portIndex = 0;
			PORT	portEnum;
			foreach ( RESOURCE resource in mPortResActive )
			{
				portEnum = (PORT)portIndex++;
				mMessageCtr.SendMsgInitPortLocSet( portEnum, resource );
			}
		}
	}
}
