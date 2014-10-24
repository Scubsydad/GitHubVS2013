using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Settlers_of_Catan
{
	/// <summary>
	/// Summary description for Support.
	/// </summary>
	/// 

	public class Asset
	{
		public	Asset( RESOURCE res, int num )
		{
			mWhichResource = res;
			mQuantity = num;
		}
		public	RESOURCE	GetResource()	{ return ( mWhichResource ); }
		public	int			GetQuantity()	{ return ( mQuantity ); }

		private	RESOURCE	mWhichResource;
		private	int			mQuantity;
	}

	public class Support
	{
		static	private	MessageCenter		mMessageCenter;
		static	private	Random				mRandom = new Random();
		static	private LogicKernel[]		mLogicKernels = new LogicKernel[(int)OWNER._size];
		static  private Bitmap[]			mSettlementBmaps = new Bitmap[] {  new Bitmap( "art//SettlementBlue.png"), new Bitmap( "art//SettlementOrange.png"), new Bitmap( "art//SettlementRed.png"), new Bitmap( "art//SettlementSilver.png"), new Bitmap( "art//SettlementInvalid.png") };
		static	private	HexInfo[]			mHexInfoSource;

		public Support()
		{
			TransparentBmaps( mSettlementBmaps );

			TestSettlmentLocs();
			_LinkSettlementDataToHexInfo();
		}

		static  public  void		TransparentBmaps( Bitmap[] bmaps )
		{
			foreach ( Bitmap bmap in bmaps )
			{
				if ( bmap != null )
				{
					bmap.MakeTransparent();
				}
			}
		}

		static  public  bool		IsQuestionTrue( string question, string heading )
		{
			DialogResult	dialogResult = MessageBox.Show( question, heading, MessageBoxButtons.YesNo );
			bool			isQuestionTrue = ( dialogResult == DialogResult.Yes );
			return ( isQuestionTrue );
		}

		static	public	Color		GetSideColor( OWNER whichSide )
		{
			switch ( whichSide )
			{
				case	OWNER.BLUE	:	return ( Color.Blue );
				case	OWNER.ORANGE:	return ( Color.Orange );
				case	OWNER.RED	:	return ( Color.Red );
				case	OWNER.SILVER:	return ( Color.Silver);
			}
			return ( Color.HotPink );
		}

		static  private void		_LinkSettlementDataToHexInfo()
		{
			mHexInfoSource = new HexInfo[MapDefs.mHexBaseData.Length];
			int destIndex = 0;
			foreach ( HexBase hexBase in MapDefs.mHexBaseData )
			{
				mHexInfoSource[destIndex++] = new HexInfo( hexBase );
			}
			LinkSettlementDataToHexInfo( mHexInfoSource );
		}

		static  public void		LinkSettlementDataToHexInfo( HexInfo[] hexInfo )
		{
			int			i, hexId;
			CITY_DIR	cityDir;
			foreach ( SettlementLoc settlementLoc in MapDefs.mSettlementLocs )
			{
				for ( i = 0; i < settlementLoc.GetNumHexesShareSettlement(); ++i )
				{
					hexId = settlementLoc.GetShareSettlementHexId( i );
					cityDir = settlementLoc.GetShareSettlementHexDir( i );
					hexInfo[hexId].SetSettlementLocLink( cityDir, settlementLoc );
				}
			}
		}

		static  public	HexInfo[]	GetHexInfoSource()
		{
			return ( mHexInfoSource );
		}

		static  public	HexInfo[]	GetHexInfoCopy()
		{
			HexInfo[] hexInfoCopy = new HexInfo[mHexInfoSource.Length];
			int destIndex = 0;
			foreach ( HexInfo hexInfo in mHexInfoSource )
			{
				hexInfoCopy[destIndex++] = new HexInfo( hexInfo );
			}
			return ( hexInfoCopy );
		}
		
		static  public Bitmap  		GetSettlementBitmap( OWNER whichSide )
		{
			return ( mSettlementBmaps[(int)whichSide] );
		}

		static  public	void		TestSettlmentLocs()
		{
			int[,] tempDirs = new int[19,(int)CITY_DIR._size];
			int i, numAdjHex, numAdjacentSettlements, adjSettlementId2, adjSettlementId, uniqueId;
			CITY_DIR	dir1, dir2;

			foreach ( SettlementLoc settlementLoc in MapDefs.mSettlementLocs )
			{
				numAdjHex = settlementLoc.GetNumHexesShareSettlement();
				for ( i = 0; i < numAdjHex; ++i )
				{
Debug.Assert( tempDirs[settlementLoc.GetShareSettlementHexId( i ), (int)settlementLoc.GetShareSettlementHexDir( i )] == 0 );
					++tempDirs[settlementLoc.GetShareSettlementHexId( i ), (int)settlementLoc.GetShareSettlementHexDir( i )];
				}
				uniqueId = settlementLoc.GetUniqueId();
				numAdjacentSettlements = settlementLoc.GetNumAdjacentLocIds();
				for ( i = 0; i < numAdjacentSettlements; ++i )
				{
					adjSettlementId = settlementLoc.GetAdjacentLocId( i, out dir1 );
					dir2 = Support.GetOppositeCityDir( dir1 );
					adjSettlementId2 = MapDefs.mSettlementLocs[adjSettlementId].GetAdjacentLocId( dir2 );
Debug.Assert( uniqueId == adjSettlementId2 );
				}
			}
		}

		static  public	HexBase[]	GetHexLocArray()
		{
			return ( MapDefs.mHexBaseData );
		}

		static  public	HexBase	GetHexBase( int byId )
		{
			return ( MapDefs.mHexBaseData[byId] );
		}

		static	public	int[][]		InitMultiDimensionArray( int numDeep, int numAcross )
		{
			int[][]	arrayDefs = new int[numDeep][];
			for ( int i = 0; i < numDeep; ++i )
			{
				arrayDefs[i] = new int[numAcross];
			}
			return ( arrayDefs );
		}

		static  public	ArrayList[]	InitMultiDimensionArrayList( int numDeep )
		{
			ArrayList[] mdArrayList = new ArrayList[numDeep];
			for ( int i = 0; i < numDeep; ++i )
			{
				mdArrayList[i] = new ArrayList();
			}
			return ( mdArrayList );
		}

		static	public	LogicKernel	GetLogicKernel( OWNER whichSide )
		{
			return ( mLogicKernels[(int)whichSide] );
		}

		static	public	void	SubmitLogicKernel( OWNER whichSide, LogicKernel logicKernel )
		{
			mLogicKernels[(int)whichSide] = logicKernel;
		}

		static	public	MessageCenter	InitMessageCenter( int numMsg )
		{
			mMessageCenter = new MessageCenter( numMsg );

			return ( MessageCenter() );
		}

		static	public	MessageCenter	MessageCenter()
		{
			return ( mMessageCenter );
		}
		
		static	public	MessageCenter	RequestFilteredMessages( MessageBroadcastHandler parentReceiveFunction, MessageType[] wantedMsg )
		{
			new MsgFilter( mMessageCenter, parentReceiveFunction, wantedMsg );
			return ( MessageCenter() );
		}
	
		static	public	int	InitRandNumGen( int randomSeed )
		{
			if ( randomSeed == -1 )
			{
				randomSeed = new Random().Next(1000);
			}
			mRandom = new Random( randomSeed );

			return ( randomSeed );
		}

		static	public	int		GetRandPct(  )
		{
			return ( mRandom.Next( 100 ) );
		}

		static	public	int		GetRand( int max )
		{
			return ( mRandom.Next( max ) );
		}

		static  public	Asset[]	GetCostToBuild( ASSET buildType )
		{
			Asset[]	requirements;
			if ( buildType == ASSET.ROAD )
			{
				requirements = new Asset[] { new Asset( RESOURCE.BRICK, 1 ),
											 new Asset( RESOURCE.LUMBER, 1 ) };
			}
			else if ( buildType == ASSET.SETTLEMENT )
			{
				requirements = new Asset[] { new Asset( RESOURCE.BRICK, 1 ),
											 new Asset( RESOURCE.LUMBER, 1 ),
											 new Asset( RESOURCE.GRAIN, 1 ),
											 new Asset( RESOURCE.WOOL, 1 ) };
			}
			else if ( buildType == ASSET.CITY )
			{
				requirements = new Asset[] { new Asset( RESOURCE.GRAIN, 2 ),
											 new Asset( RESOURCE.ORE, 3 ) };
			}
			else // if ( buildType == ASSET.DEV_CARD )
			{
Debug.Assert( buildType == ASSET.DEV_CARD );
				requirements = new Asset[] { new Asset( RESOURCE.WOOL, 1 ),
											 new Asset( RESOURCE.GRAIN, 1 ),
											 new Asset( RESOURCE.ORE, 1 ) };
			}
			return ( requirements );
		}

		static  public	string		GetIntDesc( int value )
		{
			string valDesc = value.ToString();
			if ( value < 10 )		{ valDesc = "  " + valDesc; }
			else if ( value < 100 ) { valDesc = " " + valDesc; }
			else if ( value == 0 )	{ valDesc = "-"; }
			return ( valDesc );
		}

		static	public	CITY_DIR	GetOppositeCityDir( CITY_DIR whichDir )
		{
			CITY_DIR otherDir = CITY_DIR._size;
			switch ( whichDir )
			{
				case	CITY_DIR.NORTH_WEST	:	otherDir = CITY_DIR.SOUTH_EAST;	break;
				case	CITY_DIR.NORTH_EAST	:	otherDir = CITY_DIR.SOUTH_WEST;	break;
				case	CITY_DIR.EAST		:	otherDir = CITY_DIR.WEST;		break;
				case	CITY_DIR.SOUTH_EAST	:	otherDir = CITY_DIR.NORTH_WEST;	break;
				case	CITY_DIR.SOUTH_WEST	:	otherDir = CITY_DIR.NORTH_EAST;	break;
				case	CITY_DIR.WEST		:	otherDir = CITY_DIR.EAST;		break;
				default						:	Debug.Assert( false );			break;
			}
			return ( otherDir );
		}

		static  public	Point		GetCityDirGfxOffset( CITY_DIR whichDir )
		{
			int	xMod = 0, yMod = 0;
			switch ( whichDir )
			{
				case	CITY_DIR.NORTH_WEST	:	xMod = 35;				break;
				case	CITY_DIR.NORTH_EAST	:	xMod = 106;				break;
				case	CITY_DIR.EAST		:	xMod = 141; yMod = 60;	break;
				case	CITY_DIR.SOUTH_EAST	:	xMod = 106;	yMod = 120;	break;
				case	CITY_DIR.SOUTH_WEST	:	xMod = 35;	yMod = 120;	break;
				case	CITY_DIR.WEST		:				yMod = 60;	break;
				default						:	Debug.Assert( false );	break;
			}
			return ( new Point( xMod, yMod ) );
		}

		static	public	void		GetAssocCityDir( DIR whichDir, out CITY_DIR dir1, out CITY_DIR dir2 )
		{
			switch ( whichDir )
			{
				case	DIR.NORTH		:	dir1 = CITY_DIR.NORTH_EAST;	dir2 = CITY_DIR.NORTH_WEST;	break;
				case	DIR.NORTH_EAST	:	dir1 = CITY_DIR.NORTH_EAST;	dir2 = CITY_DIR.EAST;		break;
				case	DIR.SOUTH_EAST	:	dir1 = CITY_DIR.SOUTH_EAST;	dir2 = CITY_DIR.EAST;		break;
				case	DIR.SOUTH		:	dir1 = CITY_DIR.SOUTH_EAST;	dir2 = CITY_DIR.SOUTH_WEST;	break;
				case	DIR.SOUTH_WEST	:	dir1 = CITY_DIR.SOUTH_WEST;	dir2 = CITY_DIR.WEST;		break;
				case	DIR.NORTH_WEST	:	dir1 = CITY_DIR.NORTH_WEST;	dir2 = CITY_DIR.WEST;		break;
				default					:	Debug.Assert( false ); dir1 = dir2 = CITY_DIR.INVALID;	break;
			}
		}

		static	public	DIR		GetOppositeDir( DIR whichDir )
		{
			DIR otherDir = DIR._size;
			switch ( whichDir )
			{
				case	DIR.NORTH		:	otherDir = DIR.SOUTH;		break;
				case	DIR.NORTH_EAST	:	otherDir = DIR.SOUTH_WEST;	break;
				case	DIR.SOUTH_EAST	:	otherDir = DIR.NORTH_WEST;	break;
				case	DIR.SOUTH		:	otherDir = DIR.NORTH;		break;
				case	DIR.SOUTH_WEST	:	otherDir = DIR.NORTH_EAST;	break;
				case	DIR.NORTH_WEST	:	otherDir = DIR.SOUTH_EAST;	break;
				default					:	Debug.Assert( false );		break;
			}
			return ( otherDir );
		}

		static public	RESOURCE	GetResourceFromTerrain( TERRAIN terrain )
		{
			RESOURCE	whichResource = RESOURCE.INVALID;		//	assume nothing by default (desert)
			switch ( terrain )
			{
				case	TERRAIN.DESERT		:	/*default var init */				break;
				case	TERRAIN.FIELDS		:	whichResource = RESOURCE.GRAIN;		break;
				case	TERRAIN.FOREST		:	whichResource = RESOURCE.LUMBER;	break;
				case	TERRAIN.HILLS		:	whichResource = RESOURCE.BRICK;		break;
				case	TERRAIN.MOUNTAINS	:	whichResource = RESOURCE.ORE;		break;
				case	TERRAIN.PASTURE		:	whichResource = RESOURCE.WOOL;		break;
				default						:	Debug.Assert( false );				break;
			}
			return ( whichResource );
		}

		static public	Point	GetDirOffset( DIR whichDir )
		{
			int xMod = 0;
			int yMod = 0;

			//switch ( whichDir )
			//{
			//	case	DIR.NORTH		:	yMod = -1;				break;
			//	case	DIR.EAST		:				xMod = 1;	break;
			//	case	DIR.SOUTH		:	yMod = 1;				break;
			//	case	DIR.WEST		:				xMod = -1;	break;
			//	default					:	Debug.Assert( false );	break;
			//}
			return ( new Point( xMod, yMod ) );
		}

		static	public	Bitmap	ColorizeBmap( Bitmap origBmap, Color newColor )
		{
			Bitmap bmap = new Bitmap ( origBmap );
			bmap.MakeTransparent();
			int x = 0, width = bmap.Width;
			int y, height = bmap.Height;
			Color	rgb;
			for ( ; x < width; ++x )
			{
				for ( y = 0; y < height; ++y )
				{
					rgb = bmap.GetPixel(x, y);
					if ( ( rgb.R == 255 ) && ( rgb.G == 0 ) && ( rgb.B == 0 ) )
					{
						bmap.SetPixel( x, y, newColor );
					}
				}
			}
			return ( bmap );
		}

		static	public int		GetDieRollChance( int dieRoll )
		{
			int chance = 17;			//	default for 7 (robber loc)
			switch ( dieRoll )
			{
				case	2	:
				case	12	:	chance = 3;		break;
				case	3	:
				case	11	:	chance = 6;		break;
				case	4	:	
				case	10	:	chance = 8;		break;
				case	5	:
				case	9	:	chance = 11;	break;
				case	6	:
				case	8	:	chance = 14;	break;
				case	7	: /*def var init*/  break;
				default		:Debug.Assert(false);break;
			}
			return ( chance );
		}

		static	public	int		GetDieRollPips( int dieRoll )
		{
			int numPips = 5;			//	default for 6, 7 & 8 die roll
			switch ( dieRoll )
			{
				case	2	:
				case	12	:	numPips = 1;		break;
				case	3	:
				case	11	:	numPips = 2;		break;
				case	4	:	
				case	10	:	numPips = 3;		break;
				case	5	:
				case	9	:	numPips = 4;		break;
				case	6	:
				case	8	:	/*def var init */	break;
				case	7	:	numPips = -1;		break;	// 7 as a die roll is never converted to pips as its for the 'robber'
				default		:	Debug.Assert(false);break;
			}
			return ( numPips );
		}
	}
}
