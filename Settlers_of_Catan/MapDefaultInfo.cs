using System.Drawing;

namespace Settlers_of_Catan
{

	public class MapDefs
	{

	static public	SettlementLoc	GetSettlementLoc( int index )
	{
		return ( mSettlementLocs[index] );
	}

	static public	SettlementLoc	GetSettlmentLocIndex( int mouseX, int mouseY, Size bmapSize, int allowableDelta )
	{
		SettlementLoc settlementLoc = null;
		Point		settlementCenterPt;
		int			xDelta, yDelta;
		for ( int i = 0; i < mSettlementLocs.Length; ++i )
		{
			settlementCenterPt = mSettlementLocs[i].GetSettlementGfxLoc( bmapSize ); 
			xDelta = settlementCenterPt.X - mouseX;
			yDelta = settlementCenterPt.Y - mouseY;
			if ( ( ( xDelta >= -allowableDelta ) && ( xDelta <= allowableDelta ) ) &&
					( ( yDelta >= -allowableDelta ) && ( yDelta <= allowableDelta ) ) )
			{
				settlementLoc = mSettlementLocs[i];
				break;
			}
		}
		return ( settlementLoc );
	}
			
	static public SettlementLoc[] mSettlementLocs = new SettlementLoc[] {
	//                                NW NE  E SE SW  W
	new SettlementLoc(  0, new DirLink( 1, CITY_DIR.EAST, 5, CITY_DIR.SOUTH_WEST ),										0, CITY_DIR.NORTH_WEST ),
	new SettlementLoc(  1, new DirLink( 0, CITY_DIR.WEST, 2, CITY_DIR.SOUTH_EAST, 14, CITY_DIR.NORTH_EAST ),PORT.EIGHT,	0, CITY_DIR.NORTH_EAST,	3, CITY_DIR.WEST ),
	new SettlementLoc(  2, new DirLink( 1, CITY_DIR.NORTH_WEST, 3, CITY_DIR.SOUTH_WEST, 17, CITY_DIR.EAST ),			0, CITY_DIR.EAST,		3, CITY_DIR.SOUTH_WEST,	4, CITY_DIR.NORTH_WEST ),
	new SettlementLoc(  3, new DirLink( 2, CITY_DIR.NORTH_EAST, 4, CITY_DIR.WEST, 6, CITY_DIR.SOUTH_EAST ),				0, CITY_DIR.SOUTH_EAST,	4, CITY_DIR.WEST,		1, CITY_DIR.NORTH_EAST ),
	new SettlementLoc(  4, new DirLink( 3, CITY_DIR.EAST, 5, CITY_DIR.NORTH_WEST, 9, CITY_DIR.SOUTH_WEST ),	PORT.SEVEN,	0, CITY_DIR.SOUTH_WEST,	1, CITY_DIR.NORTH_WEST ),
	new SettlementLoc(  5, new DirLink( 0, CITY_DIR.NORTH_EAST, 4, CITY_DIR.SOUTH_EAST ),								0, CITY_DIR.WEST ),

	new SettlementLoc(  6, new DirLink( 3, CITY_DIR.NORTH_WEST, 7, CITY_DIR.SOUTH_WEST, 19, CITY_DIR.EAST ),			1, CITY_DIR.EAST,		4, CITY_DIR.SOUTH_WEST,	5, CITY_DIR.NORTH_WEST ),
	new SettlementLoc(  7, new DirLink( 6, CITY_DIR.NORTH_EAST, 8, CITY_DIR.WEST, 10, CITY_DIR.SOUTH_EAST ),			1, CITY_DIR.SOUTH_EAST,	5, CITY_DIR.WEST,		2, CITY_DIR.NORTH_EAST ),
	new SettlementLoc(  8, new DirLink( 7, CITY_DIR.EAST, 9, CITY_DIR.NORTH_WEST, 13, CITY_DIR.SOUTH_WEST ),			1, CITY_DIR.SOUTH_WEST,	2, CITY_DIR.NORTH_WEST ),
	new SettlementLoc(  9, new DirLink( 4, CITY_DIR.NORTH_EAST, 8, CITY_DIR.SOUTH_EAST ),					PORT.SEVEN,	1, CITY_DIR.WEST ),
													  
	new SettlementLoc( 10, new DirLink( 7, CITY_DIR.NORTH_WEST, 11, CITY_DIR.SOUTH_WEST, 21, CITY_DIR.EAST ),			2, CITY_DIR.EAST,		5, CITY_DIR.SOUTH_WEST,	6, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 11, new DirLink( 10, CITY_DIR.NORTH_EAST, 12, CITY_DIR.WEST, 24, CITY_DIR.SOUTH_EAST ),			2, CITY_DIR.SOUTH_EAST,	6, CITY_DIR.WEST ),
	new SettlementLoc( 12,new DirLink( 11, CITY_DIR.EAST, 13, CITY_DIR.NORTH_WEST ),						PORT.SIX,	2, CITY_DIR.SOUTH_WEST ),
	new SettlementLoc( 13,new DirLink( 8, CITY_DIR.NORTH_EAST, 12, CITY_DIR.SOUTH_EAST ),					PORT.SIX,	2, CITY_DIR.WEST ),

	new SettlementLoc( 14, new DirLink( 1, CITY_DIR.SOUTH_WEST, 15, CITY_DIR.EAST ),						PORT.EIGHT, 3, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 15, new DirLink( 14, CITY_DIR.WEST, 16, CITY_DIR.SOUTH_EAST, 25, CITY_DIR.NORTH_EAST ),			3, CITY_DIR.NORTH_EAST,	7, CITY_DIR.WEST ),
	new SettlementLoc( 16, new DirLink( 15, CITY_DIR.NORTH_WEST, 17, CITY_DIR.SOUTH_WEST, 28, CITY_DIR.EAST ),			3, CITY_DIR.EAST,		7, CITY_DIR.SOUTH_WEST,	8, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 17, new DirLink( 2, CITY_DIR.WEST, 16, CITY_DIR.NORTH_EAST, 18, CITY_DIR.SOUTH_EAST ),			3, CITY_DIR.SOUTH_EAST,	8, CITY_DIR.WEST,		4, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 18, new DirLink( 17, CITY_DIR.NORTH_WEST, 19, CITY_DIR.SOUTH_WEST, 30, CITY_DIR.EAST ),			4, CITY_DIR.EAST,		8, CITY_DIR.SOUTH_WEST,	9, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 19, new DirLink( 6, CITY_DIR.WEST, 18, CITY_DIR.NORTH_EAST, 20, CITY_DIR.SOUTH_EAST ),			4, CITY_DIR.SOUTH_EAST,	9, CITY_DIR.WEST,		5, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 20, new DirLink( 19, CITY_DIR.NORTH_WEST, 21, CITY_DIR.SOUTH_WEST, 32, CITY_DIR.EAST ),			5, CITY_DIR.EAST,		9, CITY_DIR.SOUTH_WEST,	10, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 21, new DirLink( 10, CITY_DIR.WEST, 20, CITY_DIR.NORTH_EAST, 22, CITY_DIR.SOUTH_EAST ),			5, CITY_DIR.SOUTH_EAST,	10, CITY_DIR.WEST,		6, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 22, new DirLink( 21, CITY_DIR.NORTH_WEST, 23, CITY_DIR.SOUTH_WEST, 34, CITY_DIR.EAST ),			6, CITY_DIR.EAST,		10, CITY_DIR.SOUTH_WEST,11, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 23, new DirLink( 22, CITY_DIR.NORTH_EAST, 24, CITY_DIR.WEST, 37, CITY_DIR.SOUTH_EAST ),PORT.FIVE,6, CITY_DIR.SOUTH_EAST,	11, CITY_DIR.WEST ),
	new SettlementLoc( 24, new DirLink( 11, CITY_DIR.NORTH_WEST, 23, CITY_DIR.EAST ),						PORT.FIVE,	6, CITY_DIR.SOUTH_WEST ),

	new SettlementLoc( 25, new DirLink( 15, CITY_DIR.SOUTH_WEST, 26, CITY_DIR.EAST ),						PORT.ZERO,	7, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 26, new DirLink( 25, CITY_DIR.WEST, 27, CITY_DIR.SOUTH_EAST ),						PORT.ZERO,	7, CITY_DIR.NORTH_EAST ),
	new SettlementLoc( 27, new DirLink( 26, CITY_DIR.NORTH_WEST, 28, CITY_DIR.SOUTH_WEST, 38, CITY_DIR.EAST ),			7, CITY_DIR.EAST,		12, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 28, new DirLink( 16, CITY_DIR.WEST, 27, CITY_DIR.NORTH_EAST, 29, CITY_DIR.SOUTH_EAST ),			7, CITY_DIR.SOUTH_EAST,	12, CITY_DIR.WEST,		8, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 29, new DirLink( 28, CITY_DIR.NORTH_WEST, 30, CITY_DIR.SOUTH_WEST, 40, CITY_DIR.EAST ),			8, CITY_DIR.EAST,		12, CITY_DIR.SOUTH_WEST,13, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 30, new DirLink( 18, CITY_DIR.WEST, 29, CITY_DIR.NORTH_EAST, 31, CITY_DIR.SOUTH_EAST ),			8, CITY_DIR.SOUTH_EAST,	13, CITY_DIR.WEST,		9, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 31, new DirLink( 30, CITY_DIR.NORTH_WEST, 32, CITY_DIR.SOUTH_WEST, 42, CITY_DIR.EAST ),			9, CITY_DIR.EAST,		13, CITY_DIR.SOUTH_WEST,14, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 32, new DirLink( 20, CITY_DIR.WEST, 31, CITY_DIR.NORTH_EAST, 33, CITY_DIR.SOUTH_EAST ),			9, CITY_DIR.SOUTH_EAST,	14, CITY_DIR.WEST,		10, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 33, new DirLink( 32, CITY_DIR.NORTH_WEST, 34, CITY_DIR.SOUTH_WEST, 44, CITY_DIR.EAST ),			10, CITY_DIR.EAST,		14, CITY_DIR.SOUTH_WEST,15, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 34, new DirLink( 22, CITY_DIR.WEST, 33, CITY_DIR.NORTH_EAST, 35, CITY_DIR.SOUTH_EAST ),			10, CITY_DIR.SOUTH_EAST,	15, CITY_DIR.WEST,		11, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 35, new DirLink( 34, CITY_DIR.NORTH_WEST, 36, CITY_DIR.SOUTH_WEST, 46, CITY_DIR.EAST ),PORT.FOUR,11, CITY_DIR.EAST,		15, CITY_DIR.SOUTH_WEST  ),
	new SettlementLoc( 36, new DirLink( 35, CITY_DIR.NORTH_EAST, 37, CITY_DIR.WEST ),									11, CITY_DIR.SOUTH_EAST ),
	new SettlementLoc( 37, new DirLink( 23, CITY_DIR.NORTH_WEST, 36, CITY_DIR.EAST ),									11, CITY_DIR.SOUTH_WEST ),

	new SettlementLoc( 38, new DirLink( 27, CITY_DIR.WEST, 39, CITY_DIR.SOUTH_EAST ),						PORT.ONE, 	12, CITY_DIR.NORTH_EAST ),
	new SettlementLoc( 39, new DirLink( 38, CITY_DIR.NORTH_WEST, 40, CITY_DIR.SOUTH_WEST, 47, CITY_DIR.EAST ),PORT.ONE, 12, CITY_DIR.EAST,		16, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 40, new DirLink( 29, CITY_DIR.WEST, 39, CITY_DIR.NORTH_EAST, 41, CITY_DIR.SOUTH_EAST ),			12, CITY_DIR.SOUTH_EAST,	16, CITY_DIR.WEST,		13, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 41, new DirLink( 40, CITY_DIR.NORTH_WEST, 42, CITY_DIR.SOUTH_WEST, 49, CITY_DIR.EAST ),			13, CITY_DIR.EAST,		16, CITY_DIR.SOUTH_WEST,17, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 42, new DirLink( 31, CITY_DIR.WEST, 41, CITY_DIR.NORTH_EAST, 43, CITY_DIR.SOUTH_EAST ),			13, CITY_DIR.SOUTH_EAST,	17, CITY_DIR.WEST,		14, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 43, new DirLink( 42, CITY_DIR.NORTH_WEST, 44, CITY_DIR.SOUTH_WEST, 51, CITY_DIR.EAST ),			14, CITY_DIR.EAST,		17, CITY_DIR.SOUTH_WEST,18, CITY_DIR.NORTH_WEST ),
	new SettlementLoc( 44, new DirLink( 33, CITY_DIR.WEST, 43, CITY_DIR.NORTH_EAST, 45, CITY_DIR.SOUTH_EAST ),			14, CITY_DIR.SOUTH_EAST,	18, CITY_DIR.WEST,		15, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 45, new DirLink( 44, CITY_DIR.NORTH_WEST, 46, CITY_DIR.SOUTH_WEST, 53, CITY_DIR.EAST ),			15, CITY_DIR.EAST,		18, CITY_DIR.SOUTH_WEST  ),
	new SettlementLoc( 46, new DirLink( 35, CITY_DIR.WEST, 45, CITY_DIR.NORTH_EAST ),						PORT.FOUR, 	15, CITY_DIR.SOUTH_EAST ),
		
	new SettlementLoc( 47, new DirLink( 39, CITY_DIR.WEST, 48, CITY_DIR.SOUTH_EAST ),									16, CITY_DIR.NORTH_EAST ),
	new SettlementLoc( 48, new DirLink( 47, CITY_DIR.NORTH_WEST, 49, CITY_DIR.SOUTH_WEST ),								16, CITY_DIR.EAST ),
	new SettlementLoc( 49, new DirLink( 41, CITY_DIR.WEST, 48, CITY_DIR.NORTH_EAST, 50, CITY_DIR.SOUTH_EAST ),PORT.TWO, 16, CITY_DIR.SOUTH_EAST,	17, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 50, new DirLink( 49, CITY_DIR.NORTH_WEST, 51, CITY_DIR.SOUTH_WEST ),					PORT.TWO, 	17, CITY_DIR.EAST ),
	new SettlementLoc( 51, new DirLink( 43, CITY_DIR.WEST, 50, CITY_DIR.NORTH_EAST, 52, CITY_DIR.SOUTH_EAST ),			17, CITY_DIR.SOUTH_EAST,	18, CITY_DIR.NORTH_EAST ),

	new SettlementLoc( 52, new DirLink( 51, CITY_DIR.NORTH_WEST, 53, CITY_DIR.SOUTH_WEST ),					PORT.THREE, 18, CITY_DIR.EAST ),
	new SettlementLoc( 53, new DirLink( 45, CITY_DIR.WEST, 52, CITY_DIR.NORTH_EAST ),						PORT.THREE, 18, CITY_DIR.SOUTH_EAST ),
	};

	public static HexBase[]	mHexBaseData = new HexBase[] {					  //   N  NE  SE   S  SW  NW                 
	new HexBase(  0,  7, TERRAIN.DESERT,	COORD_TYPE.A, new int[] { -1,  3,  4,  1, -1, -1 }, new Point(  46,	173 ) ),
	new HexBase(  1,  8, TERRAIN.HILLS,		COORD_TYPE.B, new int[] {  0,  4,  5,  2, -1, -1 }, new Point(  46,	295 ) ),
	new HexBase(  2,  5, TERRAIN.MOUNTAINS,	COORD_TYPE.C, new int[] {  1,  5,  6, -1, -1, -1 }, new Point(  46,	417 ) ),
	new HexBase(  3,  4, TERRAIN.HILLS,		COORD_TYPE.B, new int[] { -1,  7,  8,  4,  0, -1 }, new Point( 156,	113 ) ),
	new HexBase(  4,  3, TERRAIN.FOREST,	COORD_TYPE.D, new int[] {  3,  8,  9,  5,  1,  0 }, new Point( 156,	235 ) ),
	new HexBase(  5, 10, TERRAIN.PASTURE,	COORD_TYPE.E, new int[] {  4,  9, 10,  6,  2,  1 }, new Point( 156,	357 ) ),
	new HexBase(  6,  2, TERRAIN.FIELDS,	COORD_TYPE.B, new int[] {  5, 10, 11, -1, -1,  2 }, new Point( 156,	479 ) ),
	new HexBase(  7, 11, TERRAIN.FOREST,	COORD_TYPE.C, new int[] { -1, -1, 12,  8,  3, -1 }, new Point( 266,	51 ) ),
	new HexBase(  8,  6, TERRAIN.MOUNTAINS,	COORD_TYPE.E, new int[] {  7, 12, 13,  9,  4,  3 }, new Point( 266,	173 ) ),
	new HexBase(  9, 11, TERRAIN.FIELDS,	COORD_TYPE.F, new int[] {  8, 13, 14, 10,  5,  4 }, new Point( 266,	295 ) ),
	new HexBase( 10,  9, TERRAIN.PASTURE,	COORD_TYPE.D, new int[] {  9, 14, 15, 11,  6,  5 }, new Point( 266,	417 ) ),
	new HexBase( 11,  6, TERRAIN.FOREST,	COORD_TYPE.A, new int[] { 10, 15, -1, -1, -1,  6 }, new Point( 266,	539 ) ),
	new HexBase( 12, 12, TERRAIN.PASTURE,	COORD_TYPE.B, new int[] { -1, -1, 16, 13,  8,  7 }, new Point( 376,	113 ) ),
	new HexBase( 13,  5, TERRAIN.HILLS,		COORD_TYPE.D, new int[] { 12, 16, 17, 14,  9,  8 }, new Point( 376,	235 ) ),
	new HexBase( 14,  4, TERRAIN.FOREST,	COORD_TYPE.E, new int[] { 13, 17, 18, 15, 10,  9 }, new Point( 376,	357 ) ),
	new HexBase( 15,  3, TERRAIN.MOUNTAINS,	COORD_TYPE.B, new int[] { 14, 18, -1, -1, 11, 10 }, new Point( 376,	479 ) ),
	new HexBase( 16,  9, TERRAIN.FIELDS,	COORD_TYPE.A, new int[] { -1, -1, -1, 17, 13, 12 }, new Point( 486,	173 ) ),
	new HexBase( 17, 10, TERRAIN.PASTURE,	COORD_TYPE.B, new int[] { 16, -1, -1, 18, 14, 13 }, new Point( 486,	295 ) ),
	new HexBase( 18,  8, TERRAIN.FIELDS,	COORD_TYPE.C, new int[] { 17, -1, -1, -1, 15, 14 }, new Point( 486,	417 ) ),
	};
	}
}
