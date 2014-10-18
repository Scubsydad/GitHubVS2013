using System.IO;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

namespace Settlers_of_Catan
{
	public class ResourceKernel
	{
		private			AccessQueue				mAccessQueue;
		private			DataStorage[]			mStorageItems;
		private			string[]				mObjectStrings;
		private			UTIL					mWhichUtil;
		
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileHeader
        {
            public sbyte mVersion;
            public short mNumObjects;
        }

        public FileHeader mFileHeader;

		public	ResourceKernel( UTIL whichUtil, string brkFilePath, bool isEaSharpConsoleRun )
		{
			mWhichUtil = whichUtil;
			
			if ( !File.Exists( brkFilePath ) )
			{
                brkFilePath = Path.GetFileName(brkFilePath);
			}
			if ( File.Exists( brkFilePath ) )
			{
				FileStream myStream = File.OpenRead( brkFilePath );
				BinaryReader myFile = new BinaryReader( myStream );

                short headerSize = myFile.ReadInt16();
                Debug.Assert(headerSize > 0, "Header size is wrong.");

                mFileHeader.mVersion = myFile.ReadSByte();
                //TODO: Check for file version and do proper header loading
                mFileHeader.mNumObjects = myFile.ReadInt16();						//	num objects in the save file...

                mObjectStrings = new string[mFileHeader.mNumObjects];					//	create storage for the access strings
                mStorageItems = new DataStorage[mFileHeader.mNumObjects];				//	allocate space to unpack the objects

                for (int i = 0; i < mFileHeader.mNumObjects; ++i)
				{
                    mStorageItems[i] = new DataStorage(myFile, isEaSharpConsoleRun, mFileHeader);			//	parse the file pointer and create a DataStorage class for each item in the file
					mObjectStrings[i] = mStorageItems[i].GetObjectName();	//	get name out of object and store in array for access optimization
				}
				mAccessQueue = new AccessQueue( mObjectStrings, 3 );		//	relatively small queue size of 3 for speed of accessing items
				
				char shouldBeB = myFile.ReadChar();
				char shouldBeR = myFile.ReadChar();
				char shouldBeK = myFile.ReadChar();
Debug.Assert((( shouldBeB == 'B' ) && ( shouldBeR == 'R' ) && ( shouldBeK == 'K' ) ), "Supplied file is not in correct format." );
								
				myFile.Close();
				myStream.Close();	
			}	
		}
		
		//public	void	SetRandom(Random rand)
		//{
		//	Form1.SetRandom( mWhichUtil, rand );
		//}
				
		//public int	GetRand( int maxVal )
		//{
		//	return ( Form1.GetRand( mWhichUtil, maxVal ) );	//	use 'FeBinFile::GetRandNum' when incorporated into C++
		//}
		
		//public int	GetRandPct( )
		//{
		//	return ( GetRand( 100 ) );
		//}
		
		public int   GetValueByRangePct( int minVal, int maxVal, int wantedPct )
		{
			return ( MathOps.GetValueByRangePct(minVal, maxVal, wantedPct ) );	//	return out pct on range to caller...
		}
		
		public int	CapValWithinRange( int val, int minRange, int maxRange )
		{
			return ( MathOps.CapValWithinRange( val, minRange, maxRange ) );
		}

		public int	CapModValWithinRange( int val, int mod, int minRange, int maxRange )
		{
			val += mod;
			return ( CapValWithinRange( val, minRange, maxRange ) );
		}
		
		private	DataStorage	_GetMatchingObject( string objectDesc )
		{
			int itemIndex = mAccessQueue.GetSegmentIndex( objectDesc );	//	ask access queue for the associated index
Debug.Assert( itemIndex != -1, "Invalid item label submitted!");
			return ( mStorageItems[itemIndex] );
		}
				
		public bool	IsValidSegmentIndex( string objectDesc, int segmentIndex )
		{
			return ( _GetMatchingObject(objectDesc).IsValidSegmentIndex( segmentIndex ) );
		}

		public int	GetSegmentSize( string objectDesc, int segmentIndex )
		{
			return ( _GetMatchingObject(objectDesc).GetSegmentSize( segmentIndex ) );
		}

		public	int			GetNumSegments( string objectDesc )
		{
			return ( _GetMatchingObject(objectDesc).GetNumSegments() );
		}

		/// <summary>
		/// public function that takes 'arrayIndex' and 'rowIndex' to build a [y][x] matrix access to get value stored in the class
		/// </summary>

		public int GetValueFromSegment( string objectDesc, int segmentIndex, int storageIndex )
		{
			return ( _GetMatchingObject(objectDesc).GetValueFromSegment( segmentIndex, storageIndex ));
		}
		
		public int GetBoundaryIntercept( string objectDesc, int segmentIndex, int interceptVal )
		{
			return ( GetBoundaryIntercept( objectDesc, segmentIndex, interceptVal, false ));
		}

		public int GetBoundaryIntercept( string objectDesc, int segmentIndex, int interceptVal, bool autoCapToRange )
		{
			return ( _GetMatchingObject(objectDesc).GetBoundaryIntercept( segmentIndex, interceptVal, autoCapToRange ));
		}

		public int GetInterpolationIntercept( string objectDesc, int listSegmentIndex, int testVal )
		{
			return ( GetInterpolationIntercept( objectDesc, listSegmentIndex, testVal, false ));
		}

		public int GetInterpolationIntercept(string objectDesc, int listSegmentIndex, int testVal, bool autoCapToRange )
		{
			return ( _GetMatchingObject( objectDesc ).GetInterpolationIntercept( listSegmentIndex, testVal, autoCapToRange ) );
		}

		public int GetValBasedOnInterpolation( string objectDesc, int listSegmentIndex, int valueSegmentIndex, int testVal )
		{
			return ( GetValBasedOnInterpolation( objectDesc, listSegmentIndex, valueSegmentIndex, testVal, false ));
		}

		public int GetValBasedOnInterpolation( string objectDesc, int listSegmentIndex, int valueSegmentIndex, int testVal, bool autoCapToRange )
		{
			return ( _GetMatchingObject(objectDesc).GetValBasedOnInterpolation( listSegmentIndex, valueSegmentIndex, testVal, autoCapToRange ));
		}

        public int GetPercentageInterpolation( string objectDesc, int percentSegment, int valueSegmentIndex, int testVal )
		{
			return ( _GetMatchingObject(objectDesc).GetPercentageInterpolation( percentSegment, valueSegmentIndex, testVal ));
		}

		public int GetPercentageIntercept( string objectDesc, int segmentIndex, int interceptVal )
		{
			return ( _GetMatchingObject(objectDesc).GetPercentageIntercept( segmentIndex, interceptVal ));
		}

		public	int		GetValuePairByPercent( string objectDesc, int segmentIndex, int pairIndex, int entitlementPct )
		{
			return ( _GetMatchingObject(objectDesc).GetValuePairByPercent( segmentIndex, pairIndex, entitlementPct ));
		}
	
		
		public	int		GetRangePct(int minVal, int maxVal, int value)
		{
			return ( MathOps.GetRangePct( minVal, maxVal, value ) ) ;
		}

		public	int	GetNumObjectsPresent()
		{
			int numObjects = 0;
			if ( mObjectStrings != null )
			{
				numObjects = mObjectStrings.Length;
			}
			return ( numObjects );
		}
		
		public	string	GetObjectDescByIndex( int wantedIndex )
		{
Debug.Assert((( wantedIndex >= 0 ) && ( wantedIndex < GetNumObjectsPresent() ))	, "Invalid 'wantedIndex' supplied" );
			return ( mObjectStrings[wantedIndex] );	
		}
		
		public	void	UtilSendUpdatedValue( string objectDesc, int segmentIndex, int storageIndex, short value )
		{
			_GetMatchingObject(objectDesc).UtilSendUpdatedValue( segmentIndex, storageIndex, value );
		}
		
		public int  GetArraySumIntercept( int[] array, int arraySize, int knownSumOfArray )
		{
			int intercept = Support.GetRand( knownSumOfArray );	// don't use 'array.length' in case array is padded at end, use 'arraySize' param
			for ( int i = 0; i < arraySize; ++i )
			{
				if ( intercept < array[i] )
				{
					intercept = i;
					break;
				}
				else
				{
					intercept -= array[i];
				}
			}
Debug.Assert( intercept < arraySize );
			return ( intercept );
		}
		
		public int	GetArraySumIntercept( int[] array )
		{
			int sum = 0, val, length = array.Length;
			for ( int i = 0; i < length; ++i )
			{
				val = array[i];
				if ( val < 0 )				//	if values passed in contain a negative #, correct val to zero
				{
					array[i] = val = 0;
				}
				sum += val;
			}
			return ( GetArraySumIntercept( array, length, sum ) );
		}
	
		private	class AccessQueue
		{
			private	string[]			mAccessStrings;
			private	string[]			mLastStrings;
			private int[]				mLastIndexes;
			private int[]				mQueueHitAge;
			private int					mQueueSize;
			private int					mNumStrings;
			
			public	AccessQueue( string[] accessDesc, int queueSize )
			{
				mAccessStrings = accessDesc;								//	store the initial string array
				mNumStrings = mAccessStrings.Length;						//	track # of strings in array
				mQueueSize = queueSize;										//	store size of requested queue
				
				mLastStrings = new string[mQueueSize];						//	create a string buffer of requested size...
				mLastIndexes = new int[mQueueSize];							//	storage buffer for associated index for found strings
				mQueueHitAge = new int[mQueueSize];							//	storage for age of item tracked in optimized queue
				
				for ( int i = 0; i < mQueueSize; ++i )
				{
					mLastStrings[i] = "****";								//	invalid string to ensure it won't match anything in the bin file
				}
			}
			
			public	int		GetSegmentIndex( string requestedDesc )
			{
				int foundIndex = -1;										//	by default, assume not found...
				int loop = 0;
				for ( ; loop < mQueueSize; ++loop )
				{
					if ( mLastStrings[loop] == requestedDesc )				//	did we find the item in our queue
					{
						foundIndex = mLastIndexes[loop];					//	track that we hit it...
						mQueueHitAge[loop] = 0;								//	clear the age tracking, so we know that this item was recently used
						break;												//	stop when we find the match...
					}
				}
				if ( foundIndex == -1 )										//	did we fail to find a match?
				{
					int oldestAge = -1, oldestIndex = -1;					//	storage for finding oldest item...
					for ( loop = 0; loop < mQueueSize; ++loop )				//	increment everything in the queue so we know how old it is
					{
						++mQueueHitAge[loop];								//	age this entry...
						if ( mQueueHitAge[loop] > oldestAge )				//	is this item the oldest so far?
						{
							oldestAge = mQueueHitAge[loop];					//	track the largest value thus far...
							oldestIndex = loop;								//	and which entry it is
						}
					}
					mQueueHitAge[oldestIndex] = 0;							//	clear the age tracking, so we know that this item was recently added
					for ( loop = 0; loop < mNumStrings; ++loop )
					{
						if ( mAccessStrings[loop] == requestedDesc )		//	does the string match?
						{
							break;											//	stop when we find it...		
						}
					}
					mLastStrings[oldestIndex] = mAccessStrings[loop];		//	track the comparison string for future matches
					mLastIndexes[oldestIndex] = foundIndex = loop;			//	track the location/index of the object associated with the string
				}	
				return ( foundIndex );										//	return out the index of the item
			}
		}
	
		private	class DataStorage
		{
			private string		mName;
			private int			mNumItems;
			private int			mArraySize;
			private short[]		mOffsets;
			private short[]		mStorage;
			private bool		mIsShort;
			private bool		mSigned;

            public DataStorage(BinaryReader fileReader, bool isEaSharpConsoleRun, FileHeader fileHeader)
			{
				uint[] sentinalData = new uint[4];											//	storage for 4 unsigned ints that were packed by the 'save'
				bool	expandedRegistrationLabel = false;									//	assume saved with '12 byte' limit as opposed to 17
				int		numIntsRead = 0;													//	storage for writing out array

                if (fileHeader.mVersion == 1)
                {
                    fileReader.ReadByte(); // skip first byte cause its not used here
                }
             
                sentinalData[numIntsRead++] = _CheckConvert32Bit( fileReader.ReadUInt32(), isEaSharpConsoleRun );	//	extract out the int 1 of 3 (or 4)
				sentinalData[numIntsRead++] = _CheckConvert32Bit( fileReader.ReadUInt32(), isEaSharpConsoleRun );	//	extract out the int 2 of 3 (or 4)
				if ( ( sentinalData[1] & 0x00000001 ) != 0 )								//	is the 'expanded label' bit set?
				{
					expandedRegistrationLabel = true;
					sentinalData[numIntsRead++] = _CheckConvert32Bit( fileReader.ReadUInt32(), isEaSharpConsoleRun );	//	extract out the int 2 of 3 (or 4)
				}
				sentinalData[numIntsRead++] = _CheckConvert32Bit( fileReader.ReadUInt32(), isEaSharpConsoleRun );	//	extract out the int 3 of 3rd (if old) or 4th of 4th if new
				
				_UnpackSentinalData( sentinalData, expandedRegistrationLabel );				//	extracts out the class/format information

				mStorage = new short[mArraySize];											//	all data is stored internally as 'shorts', regardless of whether it lives (on disk/bin file) as byte/sbyte etc
				mOffsets = new short[( mNumItems + 1 )];									//	offsets for each item inside the 'mStorage' array
				mOffsets[mNumItems] = (short)mArraySize;									//	store the total size in the extra last entry as well

				int i = 0;
				for ( ; i < mNumItems; ++i )												//	read in the data offsets for each component
				{
					mOffsets[i] = (short)_CheckConvert16Bit( fileReader.ReadUInt16(), isEaSharpConsoleRun );//	copy over each of the 16 bit offsets for the 'start address' of the data
				}
				if ( mIsShort )																//	is the 'array data' 'short' sized?
				{
					for ( i = 0; i < mArraySize; ++i )
					{
						mStorage[i] = (short)_CheckConvert16Bit( fileReader.ReadUInt16(), isEaSharpConsoleRun );//	copy over each of the 16 bit data items into the array
					}
				}
				else																		//	data is '8 bit' instead of '16 bit'
				{
					if ( mSigned )															//	was this saved out as 8 bit signed value?
					{
						for ( i = 0; i < mArraySize; ++i )
						{
							mStorage[i] = (short)fileReader.ReadSByte();					//	read in a signed byte, extend it out to a short
						}
					}
					else
					{
						for ( i = 0; i < mArraySize; ++i )
						{
							mStorage[i] = (short)fileReader.ReadByte();						//	read in a unsigned byte, extend it out to a short
						}
					}
				}
			}
	
			private uint _CheckConvert32Bit( uint origVal, bool isEaSharpConsoleRun )
			{
				uint adjustedVal = origVal;													//	do straight copy from param to return value assuming it's native pc		
				if ( isEaSharpConsoleRun )														//	is this the console version?
				{
					uint highShort = origVal;												//	copy the original value over to 'high short'
					highShort >>= 16;														//	shift everything down 16 bits
					highShort &= 0x0000ffff;												//	now mask out anything that was left over in the upper short of the int
					uint lowShort = ( origVal & 0x0000ffff );								//	get the 'lower short' value out of int
					ushort highShort16bit = (ushort)highShort;								//	convert what was originally the 'int' to a short
					ushort lowShort16bit = (ushort)lowShort;								//	ditto...
					ushort swappedHighShort = _CheckConvert16Bit(highShort16bit, true );	//	swap the bytes inside the short
					ushort swappedLowShort = _CheckConvert16Bit(lowShort16bit, true );		//	swap the bytes inside the short
					adjustedVal = 0;														//	ensure this is all cleared out for now...
					adjustedVal = swappedLowShort;											//	copy swapped low short to bottom of int
					adjustedVal <<= 16;														//	now shift that up into the upper 16 bits of the int
					adjustedVal |= swappedHighShort;										//	now push 'adjusted high bytes' into the bottom 16 bits of the int
				}
				return ( adjustedVal );														//	pass out the (potentially adjusted) 32 bit value...
			}	

			/// <summary>
			/// public support function to convert 16 bit shorts into hi byte/low byte format of the xbox 360 & ps3, as opposed to pc 16 bit format...
			/// </summary>

			private		ushort	_CheckConvert16Bit( ushort origVal, bool isEaSharpConsoleRun )
			{
				ushort adjustedVal = origVal;												//	do straight copy from param to return value assuming it's native pc		
				if ( isEaSharpConsoleRun )														//	is this the console version?
				{
					int highByte = ( origVal & 0xff00 );									//	isolate the current 'high byte' in bits 15 to 8
					highByte >>= 8;															//	shift it down to bits 7 to 0
					int lowByte = ( origVal & 0x00ff );										//	isolate the original bits 0 to 7
					lowByte <<= 8;															//	now shift those bits up to 8 to 15
					adjustedVal = (ushort)lowByte;											//	adjusted val becomes the new 8 to 15 bit value
					adjustedVal |= (ushort)highByte;										//	now 'or' in the original 'high byte' into the new 0 to 7 location
				}
				return ( adjustedVal );														//	pass the new converted 16 bit value out for the console verison...
			}		
		
			/// <summary>
			/// private function that reverse engineers the name/label and 16 bit/signed bools from the packed data
			/// </summary>

			private void _UnpackSentinalData( uint[] packedData, bool expandedRegistration )
			{
				char[] name = new char[13];													//	expect the name buffer to be 13 chars big by default
				int[] numChars = new int[3] { 5, 5, 2 };									//	how many chars to extract from each int...
				int   numInts = 3;															//	by default, assume 3 integers...
				if ( expandedRegistration )
				{
					numChars = new int[4] { 5, 5, 5, 2 };									//	resize for extra registration unpack
					name = new char[18];													//	resize for extra storage
					numInts = 4;															//	# of ints to read and unpack
				}
				int finalIndex = ( numInts - 1 );											//	var for extracting size/width info
				uint charIndex, intIndex = 0;												//	set all of these to zero by default...
				uint packedVal, bitMask, stringVal;
				int bitShift, readIndex = 0;
				for ( ; intIndex < numInts; ++intIndex )									//	set this up to process 3 int, extracting 5 packed characters per short
				{
					packedVal = packedData[intIndex];
					bitShift = 26;															//	shift packed letter over by a decreasing amount of bits each loop
					bitMask = 0xfc000000;													//	mask value to isolate specific bits...
					for ( charIndex = 0; charIndex < numChars[intIndex]; ++charIndex )		//	traverse string in groups of 3 chars
					{
						stringVal = packedVal & bitMask;									//	isolate specific bits
						stringVal >>= bitShift;												//	shift value down to zero bit location
						if ( stringVal == 63 )												//	did we find our 'underscore' value?
						{
							stringVal = '_';												//	just do a straight stomp for this entry
						}
						else if ( ( stringVal >= 1 ) && ( stringVal <= 26 ) )				//	did we find an upper case letter index?
						{
							stringVal -= 1;													//	zero base it...
							stringVal += 'A';												//	then convert it to 'A' to 'Z' 
						}
						else if ( ( stringVal >= 27 ) && ( stringVal <= 52 ) )				//	did we find an lower case letter index?
						{
							stringVal -= 27;												//	zero base it...
							stringVal += 'a';												//	then convert it to 'a' to 'z'
						}
						else if ( ( stringVal >= 53 ) && ( stringVal <= 62 ) )				//	did we find an numeric value index?
						{
							stringVal -= 53;												//	zero base it...
							stringVal += '0';												//	then convert it to '0' to '9'
						}
						else
						{
							intIndex = charIndex = 5;										//	break both inner and outer loops
							break;
						}
						bitMask >>= 6;														//	shift the mask value items down 5 bits
						bitShift -= 6;														//	reduce the # of bits we'll shift in the futrue
						name[readIndex++] = (char)stringVal;
					}
				}

				mIsShort = mSigned = false;													//	assume both of these are false by default...
				uint packedInt = packedData[0];												//	extract a (packed) int out of the array
				if ( ( packedInt & 1 ) != 0 )												//	was the 1st bit on in the first int?
				{
					mIsShort = true;														//	then the array is 'short sized'
				}
				if ( ( packedInt & 2 ) != 0 )												//	was the 2nd bit on in the first int?
				{
					mSigned = true;															//	then we have 'signed char' data
	Debug.Assert( !mIsShort, "Can't have 'signed short' data???" );
				}

				packedInt = packedData[finalIndex];											//	extract out the final packed int...
				mArraySize = (int)( packedInt & 0x000fff00 );								//	isolate our # of discrete values
				mArraySize >>= 8;															//	now shift the value down into the bottom 8 bits of the storage
				mNumItems = (int)( packedInt & 0x000000ff );								//	isolate the bottom 8 bits of the packed short to be our 'columns' value
				mName = new string( name, 0, readIndex );									//	now convert char array to system string
			}

			public	int			GetNumSegments()
			{
				return ( mNumItems );
			}
			
			public	int			GetSegmentSize( int segmentIndex )	
			{
Debug.Assert( ( segmentIndex >= 0 ) && ( segmentIndex < mNumItems ), "Invalid object index" );
				return ( mOffsets[segmentIndex + 1] -  mOffsets[segmentIndex] );
			}
			
			/// <summary>
			/// public function to allow the access queue to quickly retrieve information from the objects
			/// </summary>

			public	string		GetObjectName()
			{
				return ( mName );
			}	

			public	bool		IsValidSegmentIndex( int segmentIndex )
			{
				return ( ( segmentIndex < mNumItems ) );
			}

			/// <summary>
			/// private function to extract data out from a specifically prepared segment
			/// </summary>
	
			public int _GetValueFromSegment( int segmentIndex, int storageIndex )
			{
				return ( mStorage[( mOffsets[segmentIndex] + storageIndex )] );
			}		
				
			/// <summary>
			/// public function to extract data out from a specifically prepared segment (does assert on storage index, 'GetSegmentSize' will assert on object index too)
			/// </summary>

			public int GetValueFromSegment( int segmentIndex, int storageIndex )
			{
Debug.Assert( ( storageIndex >= 0 ) && ( storageIndex < GetSegmentSize(segmentIndex) ), "Invalid index within object segment" );
				return ( _GetValueFromSegment( segmentIndex, storageIndex ) );
			}
			
			public int GetBoundaryIntercept( int segmentIndex, int interceptVal, bool autoCapToRange )
			{
	            int bounds1, bounds2, valIndex = 0, intercept = 0;
	            bool foundMatch = false;
	            int segmentSize = GetSegmentSize( segmentIndex);							//	convert array width into # of value pairs
Debug.Assert( ( ( segmentSize % 2 ) == 0 ), "ResourceKernel::_GetBoundaryIntercept() passed an odd sized segment for 'bounds'!!!" );

	            if ( autoCapToRange )														//	in situations where we may have unpredictable inputs, this will be set to 'true'
	            {
		            int minVal = _GetValueFromSegment( segmentIndex, 0 );
		            int maxVal = _GetValueFromSegment( segmentIndex, segmentSize - 1 );		//	get bound edges out of segment
					MathOps.VerifyMinMax( ref minVal, ref maxVal );								//	swap min and max list values, and corresponding lookup values
		            interceptVal = MathOps.CapValWithinRange( interceptVal, minVal, maxVal );//	cap the incoming intercept based on our allowable min/max
	            }
	            int numPairs = ( segmentSize / 2 );											//	# of pairs is half the size of the segment...
            	
	            for ( ; intercept < numPairs; ++intercept )
	            {
		            bounds1 = _GetValueFromSegment( segmentIndex, valIndex++ );				//	get both consecutive pair values at once...
		            bounds2 = _GetValueFromSegment( segmentIndex, valIndex++ );

		            if ( bounds1 < bounds2 )
		            {
			            if ( ( interceptVal >= bounds1 ) && ( interceptVal <= bounds2 ) )
			            {
				            foundMatch = true;
				            break;
			            }
		            }
		            else if ( ( interceptVal >= bounds2 ) && ( interceptVal <= bounds1 ) )
		            {
			            foundMatch = true;
			            break;
		            }
	            }
	            if ( !foundMatch )
	            {
		            intercept = 0;
Debug.Assert( foundMatch, "ResourceKernel::_GetBoundaryIntercept() failed to find match" );
	            }
	            return ( intercept );
			}
			
			/// <summary>
			/// private function for interpolation support
			/// </summary>

			private int _GetInterpolationKeys( int listSegmentIndex, out int lowBoxIndex, out int highBoxIndex, out int scaleLowVal, out int scaleHighVal )
			{
Debug.Assert( ( listSegmentIndex >= 0 ) && ( listSegmentIndex < mNumItems ), "Invalid list segment index" );
				lowBoxIndex = 0;							        						//	by default, assume the first box is the 'lowest' box... (ie: list 'ascends')
				highBoxIndex = 1;							        						//	ditto

				scaleLowVal = _GetValueFromSegment( listSegmentIndex, lowBoxIndex );		//	extract the low/high box vals by default...
				scaleHighVal = _GetValueFromSegment( listSegmentIndex, highBoxIndex );

				int tieBreakRead = 0;														//	get temporary 'scan' value based on 'low box index' 2nd pair
				while ( scaleLowVal == scaleHighVal )										//	is the initial value pair 'tied'?
				{
					tieBreakRead += 2;
					scaleLowVal = _GetValueFromSegment( listSegmentIndex, lowBoxIndex + tieBreakRead );		//	get adjacent pair vals till they are not the same/tied
					scaleHighVal = _GetValueFromSegment( listSegmentIndex, highBoxIndex + tieBreakRead );
				};
	            if ( MathOps.VerifyMinMax( ref scaleLowVal, ref scaleHighVal ) )									//	swap min and max list values, and corresponding lookup values
	            {
					lowBoxIndex = 1;							        					//	swap the order of these since data 'descends'
					highBoxIndex = 0;
				}
				return ( GetSegmentSize( listSegmentIndex) - 1 );							//	one less than segment size since interpolation into adjacent entry...			
			}
			
			public int GetPercentageInterpolation( int percentSegment, int valueSegmentIndex, int randNum )
			{
Debug.Assert( ( percentSegment >= 0 ) && ( percentSegment < mNumItems ), "Invalid percentage segment index" );
Debug.Assert( ( valueSegmentIndex >= 0 ) && ( valueSegmentIndex < mNumItems ), "Invalid value segment index" );
			int segmentSize = GetSegmentSize( percentSegment );									//	get size of percent segment
Debug.Assert( ( segmentSize == GetSegmentSize( valueSegmentIndex ) ), "Interpolation error!: list & value segments must be same size" );

				int val = 0, index = 0;
				for ( ; index < segmentSize; ++index )											//	loop through segment looking for intercept
				{
					val = _GetValueFromSegment( percentSegment, index );						//	extract value from segment
					if ( randNum < val )														//	is random # less than table value?
					{
						break;																	//	stop when we find the intercept
					}
					else																		//	if not...
					{
						randNum -= val;															//	subtract table value from random val
					}
				}
				int returnVal = _GetValueFromSegment( valueSegmentIndex, index );				//	get initial value out of value table
				if ( index != 0 )																//	is index == 0?
				{
					int rangePct = ( ( randNum * 100 ) / val );									//	get range of pct segment intercept used
					val = _GetValueFromSegment(valueSegmentIndex, --index );					//	get previous value table index			
					returnVal = MathOps.GetValueByRangePct( val, returnVal, rangePct );					//	get value based on range of % table intercept
				}
				return ( returnVal );															//	pass out result 
			}
			
			/// <summary>
			/// public function for interpolated value support that Chuie/Ray's utils used
			/// </summary>

			public int GetValBasedOnInterpolation( int listSegmentIndex, int valueSegmentIndex, int testVal, bool autoCapToRange )
			{
Debug.Assert( ( valueSegmentIndex >= 0 ) && ( valueSegmentIndex < mNumItems ), "Invalid value segment index" );
Debug.Assert( ( GetSegmentSize( listSegmentIndex ) == GetSegmentSize( valueSegmentIndex ) ), "GetValBasedOnInterpolation has mis-matched size 'list' & 'value' segments!" );

	            bool foundMatch = false;													//	set up value for whether we were successful or not...
	            int lowBoxIndex, highBoxIndex, scaleLowVal, scaleHighVal;
	            int segmentSizeM1 = _GetInterpolationKeys( listSegmentIndex, out lowBoxIndex, out highBoxIndex, out scaleLowVal, out scaleHighVal );
	            int returnVal = _GetValueFromSegment( valueSegmentIndex, 0 );				//	get initial value out of segment...

	            for ( int intercept = 0; intercept < segmentSizeM1; ++intercept )
	            {
                    scaleLowVal = _GetValueFromSegment(listSegmentIndex, lowBoxIndex);	//	extract values out as pairs...
                    scaleHighVal = _GetValueFromSegment(listSegmentIndex, highBoxIndex);

		            if ( ( testVal >= scaleLowVal ) && ( testVal <= scaleHighVal ) )
		            {
                        int startVal = _GetValueFromSegment(valueSegmentIndex, lowBoxIndex);		//	extract out the associated...
                        int endVal = _GetValueFromSegment(valueSegmentIndex, highBoxIndex);		//	interpolated values in same 'spot' as list pair
			            returnVal = _GetInterpolatedValue( testVal, scaleLowVal, scaleHighVal, startVal, endVal, false );
			            foundMatch = true;
			            break;
		            }

		            ++lowBoxIndex;
		            ++highBoxIndex;
	            }

	            if ( !foundMatch && autoCapToRange )
	            {
                    int minListVal = _GetValueFromSegment(listSegmentIndex, 0);
                    int minValVal = _GetValueFromSegment(valueSegmentIndex, 0);			//	value that corresponds to the minimum list value
                    int maxListVal = _GetValueFromSegment(listSegmentIndex, GetSegmentSize(listSegmentIndex) - 1);
                    int maxValVal = _GetValueFromSegment(valueSegmentIndex, GetSegmentSize(valueSegmentIndex) - 1);		//	value that corresponds to the maximum list value

		            if ( MathOps.VerifyMinMax( ref minListVal, ref maxListVal ) )											//	swap min and max list values, and corresponding lookup values
		            {
			            int temp = minValVal;
			            minValVal = maxValVal;
			            maxValVal = temp;
		            }

		            if ( testVal >= maxListVal )
		            {
			            returnVal = maxValVal;												//	capping value, no need to interpolate
			            foundMatch = true;
		            }
		            else if ( testVal <= minListVal )
		            {
			            returnVal = minValVal;												//	capping value, no need to interpolate
			            foundMatch = true;
		            }
	            }

Debug.Assert( foundMatch, "ResourceKernel::GetValBasedOnInterpolation() failed to find match" );
	            return ( returnVal );							//	return interpolated value
			}

			/// <summary>
			/// public function for interpolated value support that Chuie/Ray's utils used
			/// </summary>

			public int GetInterpolationIntercept( int listSegmentIndex, int testVal, bool autoCapToRange )
			{
Debug.Assert( ( listSegmentIndex >= 0 ) && ( listSegmentIndex < mNumItems ), "Invalid list segment index" );

				int lowBoxIndex, highBoxIndex, scaleLowVal, scaleHighVal;
				int segmentSizeM1 = _GetInterpolationKeys( listSegmentIndex, out lowBoxIndex, out highBoxIndex, out scaleLowVal, out scaleHighVal );
				bool foundMatch = false;													//	set up value for whether we were successful or not...
				int returnVal = 0;															//	get initial 'loop count' intercept if we fail to find a match

				for ( int intercept = returnVal; intercept < segmentSizeM1; ++intercept )
				{
					if ( ( testVal >= scaleLowVal ) && ( testVal <= scaleHighVal ) )
					{
						int startVal = intercept;											//	current intercept loop val is our 'start val'
						int endVal = ( intercept + 1 );										//	one greater than our loop is the 'end val'
						returnVal = _GetInterpolatedValue( testVal, scaleLowVal, scaleHighVal, startVal, endVal, ( lowBoxIndex > highBoxIndex ) );
						foundMatch = true;
						break;
					}
					scaleLowVal = _GetValueFromSegment( listSegmentIndex, ++lowBoxIndex );	//	extract values out as pairs...
					scaleHighVal = _GetValueFromSegment( listSegmentIndex, ++highBoxIndex );
				}
	            if ( !foundMatch && autoCapToRange )
	            {
                    int minIndexVal = 0;			//	value that corresponds to the minimum list value
                    int minListVal = _GetValueFromSegment(listSegmentIndex, minIndexVal);
                    int maxIndexVal = ( GetSegmentSize(listSegmentIndex) - 1);
                    int maxListVal = _GetValueFromSegment(listSegmentIndex, maxIndexVal);

		            if ( MathOps.VerifyMinMax( ref minListVal, ref maxListVal ) )											//	swap min and max list values, and corresponding lookup values
		            {
			            int temp = minIndexVal;
			            minIndexVal = maxIndexVal;
			            maxIndexVal = temp;
		            }

		            if ( testVal >= maxListVal )
		            {
			            returnVal = maxIndexVal;											//	capping value, no need to interpolate
			            foundMatch = true;
		            }
		            else if ( testVal <= minListVal )
		            {
			            returnVal = minIndexVal;											//	capping value, no need to interpolate
			            foundMatch = true;
		            }
				}
Debug.Assert( foundMatch, "GetInterpolationIntercept() failed to find match" );
				return ( returnVal );							//	return interpolated value
			}
			
			private int		_GetInterpolatedValue( int searchVal, int searchMin, int searchMax, int valMin, int valMax, bool isDescending )
			{
				int pctRange = MathOps.GetRangePct(searchMin, searchMax, searchVal);
				if ( MathOps.VerifyMinMax( ref valMin, ref valMax ) )
				{
					isDescending = !isDescending;
				}
				if ( isDescending )									//	if we are 'descending' in our order...
				{
					pctRange = 100 - pctRange;										//	then flip the order so we grab the intercept based on inverse pct
				}
				int returnVal = MathOps.GetValueByRangePct( valMin, valMax, pctRange );
				return ( returnVal );
			}
			
			/// <summary>
			/// public function that takes a 'row' and finds the applicable 'entry' based on the intercept val. Typically used to find a 'percentage chance' from a table
			/// </summary>

			public int GetPercentageIntercept( int segmentIndex, int interceptVal )
			{
Debug.Assert( ( segmentIndex >= 0 ) && ( segmentIndex < mNumItems ), "Invalid object index" );

				int		segmentSize =  GetSegmentSize( segmentIndex);						//	get # of items in the segment
				int		arrayVal, intercept = 0;
				bool	foundMatch = false;
				for ( ; intercept < segmentSize; ++intercept )								//	traverse through the segment looking for a match
				{
					arrayVal = _GetValueFromSegment( segmentIndex, intercept );				//	pass in our requested segment details & index, to get out value
					if ( interceptVal < arrayVal )
					{
						foundMatch = true;
						break;
					}
					else
					{
						interceptVal -= arrayVal;
					}
				}
Debug.Assert( foundMatch, "GetPercentageIntercept() failed to find match" );
				return ( intercept );
			}	
			
			
			public	int		GetValuePairByPercent( int segmentIndex, int pairIndex, int entitlementPct )
			{
				if ( entitlementPct < 0 )													//	cap the incoming pct to ensure proper in range results
				{
					entitlementPct = 0;
				}
				else if ( entitlementPct > 100 )
				{
					entitlementPct = 100;
				}
				pairIndex *= 2;																//	multiply the pair index to get pair boundary within segment
				int pairLowVal = _GetValueFromSegment( segmentIndex, pairIndex++ );			//	extract the low/high box vals by default...
				int pairHighVal = _GetValueFromSegment( segmentIndex, pairIndex++ );

				MathOps.VerifyMinMax( ref pairLowVal, ref pairHighVal );

				return ( MathOps.GetValueByRangePct( pairLowVal, pairHighVal, entitlementPct ) );
			}
			
			public	void	UtilSendUpdatedValue( int segmentIndex, int storageIndex, short value )
			{
Debug.Assert( ( storageIndex >= 0 ) && ( storageIndex < GetSegmentSize(segmentIndex) ), "Invalid index within object segment" );
				mStorage[( mOffsets[segmentIndex] + storageIndex )] = value;
			}
		}
	}
}
