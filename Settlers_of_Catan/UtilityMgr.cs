using	System;
using	System.Collections;
using	System.Windows.Forms;
using	System.Drawing;
using	System.IO;
using	System.Diagnostics;
using	System.Collections.Generic;

namespace Settlers_of_Catan
{
	public class DataSegments
	{
		private Object[]		mDataTypes;
		private RichTextBox[]	mSourceData;
		private int[]			mMinVals;
		private int[]			mMaxVals;
		private int				mNumSegments;
		private bool[]			mComboDisableDuplicates;
		private string[]		mComboFinalStrings;

		private Size			mDefaultBoxSize = new Size( 24, 16 );
		private int				mDefaultMinVal = 0;
		private int				mDefaultMaxVal = 100;
		private bool			mDefaultComboDup = false;
		private string			mDefaultComboStr = "***";
		

/// <summary>
/// All segments will default to 24x16 TextBox, with a min/max range of 0-100. No 'ComboBox' support
/// </summary>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>

		public	DataSegments( RichTextBox[] sourceData )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( mDefaultBoxSize ),	//	no supplied source object, assume all items are 24,16 box size
						_CreateValArray( mDefaultMinVal ),			//	by default, assume min of zero if not supplied
						_CreateValArray( mDefaultMaxVal ),			//	max of 100 if not supplied
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// All items displayed will be of type 'translationOrig' when unpacked/formatted for display
/// </summary>
/// <param name="sourceData">An array of RichTextBox items that will be unpacked as 'ComboBox'</param>
/// <param name="translationOrig">A object used to 'format' all entries in 'sourceData'. May be a 'Size', 'TextBox', or 'ComboBox' entry</param>

		public	DataSegments( RichTextBox[] sourceData, object translationOrig )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( translationOrig ),	//	user has supplied a specific object to format the text boxes
						_CreateValArray( mDefaultMinVal ),			//	by default, assume min of zero if not supplied
						_CreateValArray( mDefaultMaxVal ),			//	max of 100 if not supplied
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// Assumes a 'ComboBox' bulk registration, by exposing the 'comboDisableDups' param
/// </summary>
/// <param name="sourceData">An array of RichTextBox items that will be unpacked as 'ComboBox'</param>
/// <param name="translationOrig">A ComboBox control which is used to display all the default entries applied to 'RichTextBox'</param>
/// <param name="comboDisableDups">A bool which indicates whether all combo box segments should enable or disable duplicate entries within one segment</param>

		public	DataSegments( RichTextBox[] sourceData, ComboBox translationOrig, bool comboDisableDups )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( translationOrig ),	//	user has supplied a specific object to format the controls
						_CreateValArray( mDefaultMinVal ),			//	by default, assume min of zero if not supplied
						_CreateValArray( mDefaultMaxVal ),			//	max of 100 if not supplied
						_CreateBoolDupArray( comboDisableDups ),	//	convert individual param to a bulk reproduction of same value as an array
						_CreateStringDupArray( mDefaultComboStr) );	//	user didn't supply default string
		}	

/// <summary>
/// Assumes a 'ComboBox' bulk registration, by exposing the 'comboDisableDups' and 'final string' params
/// </summary>
/// <param name="sourceData">An array of RichTextBox items that will be unpacked as 'ComboBox'</param>
/// <param name="translationOrig">A ComboBox control which is used to display all the default entries applied to 'RichTextBox'</param>
/// <param name="comboDisableDups">A bool which indicates whether all combo box segments should enable or disable duplicate entries within one segment</param>
/// <param name="comboLastString">A string that requires that IF the supplied param is present in an array of combos, it must be the final (or last several) entries</param>
		
		public	DataSegments( RichTextBox[] sourceData, ComboBox translationOrig, bool comboDisableDups, string comboLastString )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( translationOrig ),	//	user has supplied a specific object to format the controls
						_CreateValArray( mDefaultMinVal ),			//	by default, assume min of zero if not supplied
						_CreateValArray( mDefaultMaxVal ),			//	max of 100 if not supplied
						_CreateBoolDupArray( comboDisableDups ),	//	convert individual param to a bulk reproduction of same value as an array
						_CreateStringDupArray( comboLastString ) );	//	ditto for final string entry for all combo related segments
		}	
			
/// <summary>
/// Assumes all registered data are TextBoxes (default size 24x16) and all entries are capped between min/max params
/// </summary>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVal">The min value for validation for all TextBox entries spawned</param>
/// <param name="maxVal">The max value for validation for all TextBox entries spawned</param>

		public	DataSegments( RichTextBox[] sourceData, int minVal, int maxVal )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( mDefaultBoxSize ),	//	no supplied source object, assume all items are 24,16 box size
						_CreateValArray( minVal ),					//	duplicate min value for each segment
						_CreateValArray( maxVal ),					//	ditto max
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// All spawned controls are of type 'translationOrig' (param) and have a min/max value of 'minVal/maxVal' params.
/// This registration does NOT make sense for ComboBox registrations, as the min/max params would be ignored in this case by the default init
/// </summary>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVal">The min value for validation for all TextBox entries spawned (if 'translationOrig' is a ComboBox, this will be ignored!)</param>
/// <param name="maxVal">The max value for validation for all TextBox entries spawned (if 'translationOrig' is a ComboBox, this will be ignored!)</param>
/// <param name="translationOrig">Either a 'Size' or 'TextBox' entry to format all the text boxes present in the registration</param>

		public	DataSegments( RichTextBox[] sourceData, int minVal, int maxVal, object translationOrig )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( translationOrig ),	//	user has supplied a specific object to format the text boxes
						_CreateValArray( minVal ),					//	duplicate min value for each segment
						_CreateValArray( maxVal ),					//	ditto max
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// Assumes all registered data are TextBoxes (default size 24x16) and each registered segment has its own unique min/max range
/// </summary>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of TextBox entries spawned</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of TextBox entries spawned</param>

		public	DataSegments( RichTextBox[] sourceData, int[] minVals, int[] maxVals )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( mDefaultBoxSize ),	//	no supplied source object, assume all items are 24,16 box size
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// All spawned controls are of type 'translationOrig' (param) and each registered segment has its own unique min/max range
/// This registration does NOT make sense for ComboBox registrations, as the min/max params would be ignored in this case by the default init
/// </summary>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of TextBox entries spawned</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of TextBox entries spawned</param>
/// <param name="translationOrig">Either a 'Size' or 'TextBox' entry to format all the text boxes present in the registration</param>

		public	DataSegments( RichTextBox[] sourceData, int[] minVals, int[] maxVals, object translationOrig )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( _CreateTranslationArray( translationOrig ),	//	user has supplied a specific object to format the text boxes
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}
				
/// <summary>
/// Every 'sourceData' segment has its own 'translationOrig' to unpack it, and its own unique min/max entries
/// </summary>
/// <param name="translation">An array of 'unpack templates' for each segment that is being registered. May be 'ComboBox', 'Size' or 'TextBox'</param>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>

		public	DataSegments( Object[] translation, RichTextBox[] sourceData, int[] minVals, int[] maxVals )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( translation,								//	user supplies translation information for each individual segment
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						null,										//	no (combo) duplicates for this entry
						null );										//	no (combo) last strings for this entry
		}

/// <summary>
/// Every 'sourceData' segment has its own 'translationOrig' to unpack it, and its own unique min/max entries
/// This registration assumes there are SOME 'ComboBox' entries as part of the registration, otherwise the 'comboDisableDups' param is meaningless
/// </summary>
/// <param name="translation">An array of 'unpack templates' for each segment that is being registered. May be 'ComboBox', 'Size' or 'TextBox'</param>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="comboDisableDups">A bool which indicates whether all combo box segments should enable or disable duplicate entries within one segment</param>

		public	DataSegments( Object[] translation, RichTextBox[] sourceData, int[] minVals, int[] maxVals, bool comboDisableDups )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( translation,								//	user supplies translation information for each individual segment
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						_CreateBoolDupArray( comboDisableDups ),	//	convert individual param to a bulk reproduction of same value as an array
						null );										//	no (combo) last strings for this entry
		}
		
/// <summary>
/// Every 'sourceData' segment has its own 'translationOrig' to unpack it, and its own unique min/max entries
/// This registration assumes there are SOME 'ComboBox' entries as part of the registration, otherwise the 'combo' params are meaningless
/// </summary>
/// <param name="translation">An array of 'unpack templates' for each segment that is being registered. May be 'ComboBox', 'Size' or 'TextBox'</param>
/// <param name="sourceData">An array of RichTextBox items which contain the default initial values to be placed in all text boxes</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="comboDisableDups">A bool which indicates whether all combo box segments should enable or disable duplicate entries within one segment</param>
/// <param name="comboLastString">A string that requires that IF the supplied param is present in an array of combos, it must be the final (or last several) entries</param>

		public	DataSegments( Object[] translation, RichTextBox[] sourceData, int[] minVals, int[] maxVals, bool comboDisableDups, string comboLastString )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( translation,								//	user supplies translation information for each individual segment
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						_CreateBoolDupArray( comboDisableDups ),	//	convert individual param to a bulk reproduction of same value as an array
						_CreateStringDupArray( comboLastString ) );	//	ditto for final string entry for all combo related segments
		}

/// <summary>
/// Every 'sourceData' segment has its own 'translationOrig' to unpack it, and its own unique min/max entries
/// This registration assumes there are SOME 'ComboBox' entries as part of the registration, otherwise the 'combo' params are meaningless
/// </summary>
/// <param name="translation">An array of 'unpack templates' for each segment that is being registered. May be 'ComboBox', 'Size' or 'TextBox'</param>
/// <param name="minVals">A 1 to 1 correlation of 'segment == min' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="maxVals">A 1 to 1 correlation of 'segment == max' array for validation for each segment of segments spawned (ComboBox segments will ignore param)</param>
/// <param name="comboDups">An array of bools to correlate 1 to 1 for segments, so that EACH ComboBox segment may have its own unique 'disableDups' flag</param>
/// <param name="comboLastString">An array of strings that requires that IF the supplied param is present combo box segment array, it must be the final (or last several) entries</param>

		public	DataSegments( Object[] translation, RichTextBox[] sourceData, int[] minVals, int[] maxVals, bool[] comboDups, string[] comboLastString  )
		{
			_SourceDataInit( sourceData );							//	register source data (only, so far) and determine how many segments present
			_ClassInit( translation,								//	user supplies translation information for each individual segment
						minVals,									//	pass in supplied min val array
						maxVals,									//	ditto max array
						comboDups,									//	user supplied array of whether combo segments should disable duplicate settings
						comboLastString );							//	user supplied array of what the 'final string' (if any) should be for each combo segment
		}

		private void	_SourceDataInit( RichTextBox[] sourceData )
		{
			mSourceData = sourceData;
			mNumSegments = mSourceData.Length;
		}
		
		private void	_ClassInit( Object[] translation, int[] minVals, int[] maxVals, bool[] comboDupFlag, string[] comboLastStrings )
		{
			mDataTypes = translation;
			mMinVals = minVals;
			mMaxVals = maxVals;
			mComboDisableDuplicates = comboDupFlag;
			mComboFinalStrings = comboLastStrings;

			if ( mComboDisableDuplicates == null )
			{
				mComboDisableDuplicates = _CreateBoolDupArray( mDefaultComboDup );	//	assume there may be some combo segments, if so, do NOT disable duplicates			
			}
			
			if ( mComboFinalStrings == null )
			{
				mComboFinalStrings = _CreateStringDupArray( mDefaultComboStr );		//	assume there may be some combo segments, if so, assign (invalid) 'final string'
			}

Debug.Assert( ( mDataTypes.Length == mSourceData.Length ), "Mismatch between translation & source data sizes" );			
Debug.Assert( ( mSourceData.Length == mMinVals.Length ), "Mismatch between source & min val data sizes" );			
Debug.Assert( ( mMinVals.Length == mMaxVals.Length ), "Mismatch between min & max val data sizes" );			
Debug.Assert( ( mSourceData.Length == mComboDisableDuplicates.Length ), "Mismatch between source & combo duplication array sizes" );			
Debug.Assert( ( mSourceData.Length == mComboFinalStrings.Length ), "Mismatch between source & combo final string array sizes" );			
		}
	
		private int[]		_CreateValArray( int valToSet )
		{
			int[]	intArray = new int[mNumSegments];					//	create an array of 'wantedSize' to duplicate the min/max vals
			for ( int i = 0; i < mNumSegments; ++i )
			{
				intArray[i] = valToSet;									//	duplicate value in every entry of array
			}
			return ( intArray );
		}

		private string[]	_CreateStringDupArray( string source )
		{
			string[] dupArray = new string[mNumSegments];
			for ( int i = 0; i < mNumSegments; ++i )
			{
				dupArray[i] = source;
			}
			return ( dupArray );
		}
		
		private bool[]	_CreateBoolDupArray( bool source )
		{
			bool[] dupArray = new bool[mNumSegments];
			for ( int i = 0; i < mNumSegments; ++i )
			{
				dupArray[i] = source;
			}
			return ( dupArray );
		}
		
		private Object[]	_CreateTranslationArray( Object source )
		{
			Object[] translation = new Object[mNumSegments];
			for ( int i = 0; i < mNumSegments; ++i )
			{
				translation[i] = source;
			}
			return ( translation );
		}

/// <summary>
/// Function to alter an existing DataSegments class and override a specific 'ComboDisableDup' flag
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="disableDupFlag">The new value to be stomped into the existing array</param>
				
		public	void	SetComboDisableDuplication( int segmentId, bool disableDupFlag )
		{
Debug.Assert( ( mComboDisableDuplicates != null ), "array was not submitted upon class creation!" );			
			mComboDisableDuplicates[segmentId] = disableDupFlag;
		}
		
/// <summary>
/// Function to alter an existing DataSegments class and override a specific 'ComboFinalString' entry
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="finalString">The new value to be stomped into the existing array</param>
	
		public	void	SetComboFinalString( int segmentId, string finalString )
		{
Debug.Assert( ( mComboFinalStrings != null ), "array was not submitted upon class creation!" );			
			mComboFinalStrings[segmentId] = finalString;
		}

		public	void	SetSegmentsMin( int[] segmentIds, int minVal )
		{
			foreach ( int segmentId in segmentIds )
			{
				SetSegmentMin( segmentId, minVal );
			}
		}
		
		
/// <summary>
/// Function to alter an existing DataSegments class and override a specific 'MinValue' value range for validation
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="finalString">The new value to be stomped into the existing array</param>
	
		public	void	SetSegmentMin( int segmentId, int minVal )
		{
			mMinVals[segmentId] = minVal;
		}

		public	void	SetSegmentsMax( int[] segmentIds, int maxVal )
		{
			foreach ( int segmentId in segmentIds )
			{
				SetSegmentMax( segmentId, maxVal );
			}
		}
				
/// <summary>
/// Function to alter an existing DataSegments class and override a specific 'MaxValue' value range for validation
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="finalString">The new value to be stomped into the existing array</param>
	
		public	void	SetSegmentMax( int segmentId, int maxVal )
		{
			mMaxVals[segmentId] = maxVal;
		}

/// <summary>
/// Function to alter an existing DataSegments class to override the 'SourceRtb' to unpack the data for that segment
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="sourceRtb">The new RichTextBox entry to replace the existing one, this allows for new validation for an previously registered segment</param>
		
		public	void	SetSegmentSource( int segmentId, RichTextBox sourceRtb )
		{
			mSourceData[segmentId] = sourceRtb;
		}
		
/// <summary>
/// Function to alter an existing DataSegments class to override the 'SourceRtb' to unpack the data for that segment
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be 'stomped'</param>
/// <param name="sourceRtb">The new RichTextBox entry to replace the existing one, this allows for new validation for an previously registered segment</param>
/// <param name="translation">The new 'Object' entry to replace the existing one, allows for a new display/font/size to be registered for a previously created segment</param>
	
		public	void	SetSegmentSource( int segmentId, RichTextBox sourceRtb, Object translation )
		{
			mSourceData[segmentId] = sourceRtb;
			mDataTypes[segmentId] = translation;
		}	

/// <summary>
/// Function allows caller to determine the current 'minValue' range for an existing DataSegment class
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be used as an index to supply the result</param>
/// <returns></returns>
		
		public	int		GetSegmentMin( int segmentId )
		{
			return ( mMinVals[segmentId] );
		}
		
/// <summary>
/// Function allows caller to determine the current 'maxValue' range for an existing DataSegment class
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be used as an index to supply the result</param>
/// <returns></returns>
		
		public	int		GetSegmentMax( int segmentId )
		{
			return ( mMaxVals[segmentId] );
		}

/// <summary>
/// Function supplies how many unique segments have been registered within the existing DataSegment class
/// </summary>
/// <returns>An Integer, representing the total # of segments registered associated with this item</returns>
/// 
		public	int		GetNumSegments( )
		{
			return ( mNumSegments );
		}

/// <summary>
/// Function allows caller to determine the 'DisableDup' flag for any registered segment in the class
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be used as an index to supply the result</param>
/// <returns>A bool, 'true' if multiple combo entries (in one segment) may NOT share the same string, or 'false' if they can</returns>
		
		public bool		GetComboDisableDup( int segmentId )
		{
			return ( mComboDisableDuplicates[segmentId] );
		}

/// <summary>
/// Function allows caller to determine the 'FinalString' entry for any registered segment in the class
/// </summary>
/// <param name="segmentId">The segment (zero based) which is to be used as an index to supply the result</param>
/// <returns>A string ("***" if not supplied/defined) representing the string that MUST terminate the array of Combos, if applicable</returns>
	
			public  string	GetComboSegmentFinalString( int segmentId )
		{
			return ( mComboFinalStrings[segmentId] );
		}

/// <summary>
/// Function used to support 'UtilData' registration, not necessary for anyone else to call.
/// </summary>
/// <returns>An Array of RichTextBox items, which were the 'sourceData' param when class was constructed.</returns>
	
		public	RichTextBox[]	GetSegmentSource()
		{
			return ( mSourceData );
		}
		
/// <summary>
/// Function used to support 'UtilData' registration, not necessary for anyone else to call.
/// </summary>
/// <returns>A single RichTextBox item, which was part of the original 'sourceData' param when class was constructed.</returns>

		public	RichTextBox	GetSegmentSourceData( int segmentId )
		{
			return ( mSourceData[segmentId] );
		}
		
/// <summary>
/// Function used to support 'UtilData' registration, not necessary for anyone else to call.
/// </summary>
/// <returns>Returns a 'Size', 'TextBox', or 'ComboBox' depending on the original 'translation' param supplied to class</returns>

		public	Object	GetSegmentDataType( int segmentId )
		{
			return ( mDataTypes[segmentId] );
		}
	}
	
	public class UtilityMgr
	{
		private		ResourceKernel			mResourceKernel = null ;
		private		ArrayList				mUtilObjects = new ArrayList();		//	each entry contains a 'DataObject' but also extra stuff to support C# utility save/load/validity checks
		private		string					mBinFileName;
		private		Form1					mClassCreator;
		private		Font					mInterpolatePlotFont = new Font("Sylfaen", 8.0f);
		private		int						mSubmittedSaveFileVersion = 0;
		private		int						mMinValidFileVersion = -1;
        private		List<string>			mRegisteredNamesList = new List<string>();        
		private		UTIL					mWhichUtil;
		private		bool					mExportFileCall;
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//	basic constructor, takes a bin file name only
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	UtilityMgr( UTIL whichUtil, string binFile, bool isExportCall )
		{
			mWhichUtil = whichUtil;
			mBinFileName = binFile;
			mExportFileCall = isExportCall;
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	provides 'ResourceKernel' link so external callers can access registered data
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	ResourceKernel		GetResourceKernel()
		{
			return ( mResourceKernel );
		}
	
////////////////////////////////////////////////////////////////////////////////////////
//		
//	support function to 'Form1' (which spawns all utilities) can be attached, 
//	so 'DirtyData' call backs can be initiated
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	void	SubmitClassCreator( Form1 form1 )
		{
			mClassCreator = form1;												//	register the creator...
		}
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//	called from 'UtilData' objects when they are dirty, so that the 'Form1' can be updated
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	void	ComponentDirtyNotification()							//	notification for dirty/clean status
		{
			if ( mClassCreator != null )
			{
				mClassCreator.UpdateUtilDirtyStatus();							//	tell 'Form1' (master page) to update it's refresh status
			}
		}
	
////////////////////////////////////////////////////////////////////////////////////////
//		
//	support function called by 'Form1' to see if any registered data has NOT been copied
//	from the Utility TextBox/ComboBox controls to their associated 'Kernel' data values
//			
////////////////////////////////////////////////////////////////////////////////////////

		public  bool	CheckHaveDirtySegments()
		{
			bool		haveDirtySegments = false;										//	assume all clean by default...
			foreach ( UtilData utilData in mUtilObjects )								//	traverse though all registered items
			{
				if ( utilData.CheckIfDirtySegments() )									//	is the current item 'dirty'?
				{
					haveDirtySegments = true;											//	flag that we found dirty segment...
					break;																//	stop when we find any, one is enough
				}
			}
			return ( haveDirtySegments );												//	return 'dirty' status...
		}

		public	ArrayList[]	RegisterDataSegments( string objectDesc, DataSegments dataSegment )
		{		
			return ( _ClassInit1( objectDesc, dataSegment, null, new Point( 0, 0 ) ) );
		}

		public	ArrayList[]	RegisterDataSegments( string objectDesc, DataSegments dataSegment, Control tabPageParent )
		{
			return ( _ClassInit1( objectDesc, dataSegment, tabPageParent, new Point( 0, 0 ) ) );
		}
		
		public	ArrayList[]	RegisterDataSegments( string objectDesc, DataSegments dataSegment, Control tabPageParent, Point displayOffset )
		{
			return ( _ClassInit1( objectDesc, dataSegment, tabPageParent, displayOffset ) );
		}
		
		private ArrayList[]	_ClassInit1( string objectDesc, DataSegments dataSegments, Control tabPageParent, Point xyOffset )
		{
			int			numSegments = dataSegments.GetNumSegments();
			Control[]	segmentParents = new Control[numSegments];
			
			if ( tabPageParent == null )			//	did the caller NOT supply a 'Parent' to attach the data segments to?
			{
				for ( int i = 0; i < numSegments; ++i )
				{
					segmentParents[i] = dataSegments.GetSegmentSourceData( i ).Parent;
				}
			}
			else
			{
				for ( int i = 0; i < numSegments; ++i )
				{
					segmentParents[i] = tabPageParent;
				}
			}
			return ( _ClassInit2( objectDesc, dataSegments, segmentParents, xyOffset ) );
		}

		public	ArrayList[]	RegisterDataSegments( string objectDesc, DataSegments dataSegment, Control[] segmentParents )
		{
			return ( _ClassInit2( objectDesc, dataSegment, segmentParents, new Point( 0, 0 ) ) );
		}
		
		public	ArrayList[]	RegisterDataSegments( string objectDesc, DataSegments dataSegment, Control[] segmentParents, Point displayOffset )
		{
			return ( _ClassInit2( objectDesc, dataSegment, segmentParents, displayOffset ) );
		}
		
		private ArrayList[]	_ClassInit2( string objectDesc, DataSegments dataSegments, Control[] segmentParents, Point xyOffset )
		{
			ArrayList[] arrayListArray = null;
			if ( _IsValidNameLabel( objectDesc, true ) )										//	is name valid? and we have submitted 'boundary groupings'?
			{
				UtilData utilData = new UtilData( this, objectDesc, dataSegments, segmentParents, xyOffset );//	create a UtilData based on the 'DataSegments' param
				if ( utilData.IsValid() )
				{
		            mRegisteredNamesList.Add( objectDesc );
					mUtilObjects.Add( utilData );										//	add the combined data & util info object to it's array as well
					arrayListArray = utilData.GetGeneratedControls();					//	get the array list array as return param
				}
			}
			new RegistrationBackup( this, objectDesc, arrayListArray );
			return ( arrayListArray );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	Support call to ensure that all data is translated to Kernel, IF they all survive their 'Validate' calls.
//	If any item is NOT validated, the operation will return 'false'
//			
////////////////////////////////////////////////////////////////////////////////////////

		public bool ValidateAllData( )
		{
			return ( _ValidateAllData( true ) );
		}
			
////////////////////////////////////////////////////////////////////////////////////////
//		
//	function that allows C# util to ignore an entire registration found in a BRK save file
//  if/when the file is found on disk, when utility is launched. This is typically used
//	when a implementation has been completely re-factored, and any existing 'defaults' data
//	should be disregarded. EVERYTHING with this 'name' is jettisoned. 
//
//	PLEASE NOTE: You should only ever use the call ONCE when adding new data, and remove the 
//				 call as soon as the new data has been integrated, failure to do so will mean
//				 the (even new) data is NEVER loaded off disk, ever.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public  bool	AddIgnoreUponLoadAllSegments( string label )
		{
			return ( AddIgnoreUponLoadForSegment( label, -1 ) );						//	pass in -1 so all aspects of the component are disabled
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	As above, except that only the 'xth' segment index is ignored. This will be used
//	when only SOME of a registration has been re-vamped. All other segments are used.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public  bool	AddIgnoreUponLoadForSegment( string label, int segmentIndex )
		{
			return ( AddIgnoreUponLoadForSegments( label, new int[] { segmentIndex } ) );//	pass in value as an array of 1, to be compatible with multiple registers
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	as above, except that multiple segments can be removed, not just one, or all.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public  bool	AddIgnoreUponLoadForSegments( string label, int[] whichSegments )
		{
			return( _AddIgnoreUponLoadForSegments( label, whichSegments ) );
		}

		public  bool	AddIgnoreUponLoadForSegments( string label, int startIgnore, int endIgnore )
		{
			int size = ( 1 + ( endIgnore - startIgnore ) );
			int[] whichSegments = new int[size];
			for ( int i = 0; startIgnore <= endIgnore; ++i )
			{
				whichSegments[i] = startIgnore++;
			}
			return( _AddIgnoreUponLoadForSegments( label, whichSegments ) );
		}
////////////////////////////////////////////////////////////////////////////////////////
//		
//	allows caller to know the 'SaveFileVersion' used, generally, this is NOT really
//  used on NHL, but Fight Night wanted this in case they ever implemented some kind of
//  robust file formatting.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public  int		GetSaveFileVersion()
		{
			return ( mSubmittedSaveFileVersion );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  Simplest Save file request, supplies whether a confirmation popup is required or not upon success/failure.
//			
////////////////////////////////////////////////////////////////////////////////////////

        public bool SaveFileRequest(bool wantSaveConfirmation)
		{
            return ( SaveFileRequest(wantSaveConfirmation, mBinFileName, 0));
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  Save File request. Supply params whether a confirmation is required, and which 'file version' is
//	to be used. Defaults to 'mBinFileName' of constructor argument only.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public bool SaveFileRequest(bool wantSaveConfirmation, int submittedSaveFileVersion)
		{
			return ( SaveFileRequest( wantSaveConfirmation, mBinFileName ) );			//	call next lower function that doesn't care about save file
		}
				
////////////////////////////////////////////////////////////////////////////////////////
//		
//  alternate save file request, supply whether confirmation is wanted, and new alternate 
//	save file name. Cannot specify 'file version', will default to 'one'.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool  SaveFileRequest( bool wantSaveConfirmation, string newFileName )
		{
			bool	validStatus = SaveFileRequest(wantSaveConfirmation, newFileName, 0 );
			if ( !validStatus )
			{
				MessageBox.Show("File was not valid for saving, new save file name <{0}> was ignored.", newFileName );
			}
			return ( validStatus );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  Most robust save file request, allows caller to specify all params, confirmation, name, and file version
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool  SaveFileRequest( bool wantSaveConfirmation, string newFileName, int submittedSaveFileVersion )        		
		{
			return ( _SaveFileRequest( wantSaveConfirmation, newFileName, submittedSaveFileVersion ) );
		}

		
////////////////////////////////////////////////////////////////////////////////////////
//		
//  specialized BRK file load function, supply NEW bin file name, if valid. NOT currently
//  used in NHL, was support for Fight Night.
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool	CheckLoadBrkFile( string newBinFileName )
		{
			bool	loadedStatus = false;												//	assume no load by default...

			if ( File.Exists( newBinFileName ) )										//	verify this alternate name exists...
			{
				mBinFileName = newBinFileName;											//	stomp our class bin file name...
				loadedStatus = _CheckLoadBrkFile( );
			}
			else
			{
				MessageBox.Show(string.Format("File <{0}> does not exist, load file request ignored!" ));
			}
			return ( loadedStatus );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  specialized bin file load function. Not used on NHL, supported for Fight Night
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool	CheckLoadBrkFile( int minAcceptableFileVersion )
		{
Debug.Assert( minAcceptableFileVersion >= 0 );
			mMinValidFileVersion = minAcceptableFileVersion;
			return ( _CheckLoadBrkFile( ) );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  Most robust of alternate load file functions. Supplies alternate file name and file version.
//	not used by NHL utils, provided for Fight Night
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool	CheckLoadBrkFile( string newBinFileName, int minAcceptableFileVersion )
		{
Debug.Assert( minAcceptableFileVersion >= 0 );
			mMinValidFileVersion = minAcceptableFileVersion;
			return ( CheckLoadBrkFile( newBinFileName ) );
		}
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//  Standard Load BRK File, uses default file version & save file name as supplied by constructor
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	bool	CheckLoadBrkFile( )
		{
			return ( _CheckLoadBrkFile() );
		}
	

////////////////////////////////////////////////////////////////////////////////////////
//		
//  Support function to graphically plot out an interpolation segment visually
//	Version below is used if you want to plot TWO interpolations in a single Panel.
//	Use the item BELOW if you only want a single plot.
//			
////////////////////////////////////////////////////////////////////////////////////////

//BRK please note, the format of the rectangle param should be as follows...
//		ranges = new Rectangle( bottom left X val, bottom left Y val, bottom right X val, top LEFT Y val )
//
//		 D +----------------+
//       ^ |                |
//       | |                |
//       C +----------------+
//         A      ->       B   
//      ranges = new Rectangle( A, C , B, D );			//	this allows the interpolation to know where the 'empty' space is, if any, in the plot

		public	Bitmap	InterpolationPlot( Bitmap bmap, Color penColor, string objectDesc, int listSegment, int valueSegment, int highlightVal, bool isListSegmentHorizontal, Rectangle ranges)
		{
			int			bWidth = ( bmap.Width - 4 );
			int			result, bHeight = ( bmap.Height - 4 );
			float		xMod, yMod, xCoord = 2, yCoord;
			Graphics	gfx = Graphics.FromImage( bmap );													//	get graphics class from bmap
			bool		firstTimeThrough = true;
			Pen			linePen = new Pen(penColor);
			Point		currPoint, prevLineCoord = new Point();
			
			int leftX = ranges.Location.X;
			int bottomY = ranges.Location.Y;
			int rightX = ranges.Size.Width;
			int topY = ranges.Size.Height;

			xMod = _GetInterpolationDrawMod( bWidth, leftX, rightX );	//	get width data
			yMod = _GetInterpolationDrawMod( bHeight, bottomY, topY );	//	get height data
			int			listVal = 0;
			int			listMaxVal = 99;
			ResourceKernel	validKernel = GetResourceKernel();									//	call this, in case 'mResourceKernel' is null due to 'read only' bin file
			_GetInterpolationRange( validKernel, objectDesc, listSegment, out listVal, out listMaxVal );					//	get list range data
			
			if ( isListSegmentHorizontal )																	//	do we have a horizontally oriented display?
			{
				if ( leftX > rightX )
				{
					xCoord = ( bmap.Width - 2 );
					xMod = -xMod;
				}
											
				for ( ; listVal <= listMaxVal; ++listVal, xCoord += xMod )
				{
					result = validKernel.GetValBasedOnInterpolation( objectDesc, listSegment, valueSegment, listVal );
					yCoord = ( bHeight - ( ( result - bottomY ) * yMod ) );												//	get associated x coord
					yCoord = validKernel.CapValWithinRange( (int)yCoord, 0, bHeight - 1 );
					currPoint = new Point((int)xCoord, (int)yCoord);									//	get current coordinate
					if ( !firstTimeThrough )
					{
						gfx.DrawLine( linePen, currPoint, prevLineCoord );								//	draw line connecting spots instead					
					}
					prevLineCoord = currPoint;															//	make current coordinate 'prev coordinate'
					if ( listVal == highlightVal )
					{
						_DisplayInterpolationIntercept( gfx, xCoord, yCoord, result, bWidth, bHeight );
					}
					firstTimeThrough = false;	
				}
			}
			else
			{
				yCoord = bHeight;

				for ( ; listVal <= listMaxVal; ++listVal, yCoord -= yMod )
				{
					result = validKernel.GetValBasedOnInterpolation( objectDesc, listSegment, valueSegment, listVal );
					xCoord = ( 2 + ( ( result - leftX ) * xMod ) );														//	get associated x coord
					xCoord = validKernel.CapValWithinRange( (int)xCoord, 0, bWidth - 1 );
					currPoint = new Point((int)xCoord, (int)yCoord);									//	get current coordinate
					if ( !firstTimeThrough )
					{
						gfx.DrawLine( linePen, currPoint, prevLineCoord );								//	draw line connecting spots instead					
					}
					prevLineCoord = currPoint;															//	make current coordinate 'prev coordinate'
					if ( listVal == highlightVal )
					{
						_DisplayInterpolationIntercept( gfx, xCoord, yCoord, result, bWidth, bHeight );
					}
					firstTimeThrough = false;
				}
			}
			return ( bmap );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//  interpolation plot support function. Displays a 'interpolation' graphically, based on the 'Rectangle' ranges supplied
//			
////////////////////////////////////////////////////////////////////////////////////////

		public	Bitmap	InterpolationPlot( Panel drawPanel, string objectDesc, int listSegment, int valueSegment, int highlightVal, bool isListSegmentHorizontal, Rectangle ranges)
		{
			Bitmap bmap = new Bitmap( drawPanel.Width, drawPanel.Height );									//	create bitmap from panel dimensions
			Graphics.FromImage( bmap ).Clear( Color.Black );
			bmap = InterpolationPlot( bmap, drawPanel.ForeColor, objectDesc, listSegment, valueSegment, highlightVal, isListSegmentHorizontal, ranges);//	do plot
			drawPanel.BackgroundImage = bmap;
			return ( bmap );
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	STATIC PUBLIC functions that provide helper functionality for Utilities, but don't need the a class
//			
////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//		
//	'Graphics' optimization, since every utility has some custom graphical harness plots etc.
//			
//	simplest version, takes in a 'Windows.Panel', and sets up a bitmap based on that sized control, returns out a Graphics object
//
////////////////////////////////////////////////////////////////////////////////////////

		static public	Graphics	PreparePanelGfx( Panel panel )
		{
			Bitmap ignoreBmap;
			return ( PreparePanelGfx( panel, null, out ignoreBmap ) );
		}
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//	'Graphics' optimization, since every utility has some custom graphical harness plots etc.
//			
//	takes in a windows.panel, plus a default bitmap to place as the foundation for the bitmap object
//
////////////////////////////////////////////////////////////////////////////////////////

		static public	Graphics	PreparePanelGfx( Panel panel, Bitmap sourceBmapToUse )
		{
			Bitmap ignoreBmap;
			return ( PreparePanelGfx( panel, sourceBmapToUse, out ignoreBmap ) );
		}
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//	'Graphics' optimization, since every utility has some custom graphical harness plots etc.
//			
//	version allows caller to have access to bitmap that was created for the panel, if required seperately from "Graphics" object
//
////////////////////////////////////////////////////////////////////////////////////////

		static public	Graphics	PreparePanelGfx( Panel panel, out Bitmap createdBmap )
		{
			return ( PreparePanelGfx( panel, null, out createdBmap ) );
		}
		
////////////////////////////////////////////////////////////////////////////////////////
//		
//	'Graphics' optimization, since every utility has some custom graphical harness plots etc.
//			
//	most robust version, combination of above 3 versions
//
////////////////////////////////////////////////////////////////////////////////////////

		static public	Graphics	PreparePanelGfx( Panel panel, Bitmap sourceCopyBmap, out Bitmap createdBmap )
		{
			Graphics gfx;
			if ( sourceCopyBmap == null )
			{
				createdBmap = new Bitmap( panel.Width, panel.Height );	//	create new bitmap of height/width of passed in panel
				gfx = Graphics.FromImage( createdBmap );				//	create a graphical object for the bitmap
				gfx.Clear( panel.BackColor );							//	automatically clear the bitmap to the background color of the source panel
			}
			else
			{
				createdBmap = new Bitmap( sourceCopyBmap );				//	create a bmap based on the optional supplied 'source' bitmap
				gfx = Graphics.FromImage( createdBmap );				//	create a graphical object for the bitmap
			}
			panel.BackgroundImage = createdBmap;						//	now set the bmap to the panel's background image, 'gfx' will draw to the panel from now on
			
			return ( gfx );												//	return gfx to caller, so they can draw/plot to panel bmap
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	Support function that allows caller to display an error message, and snap the utility to a tab page containing the offending controls that are invalid
//			
////////////////////////////////////////////////////////////////////////////////////////

		static public void	DisplayErrorMessage( string errorMsg, string objectDesc, ArrayList controls, int badSegment, int badControlIndex )
		{
			// first, traverse through the 'parents' of the controls to find out which tab page that this textbox/combo box lives on...
			//BRK discovered that controls inside 'group box' have THAT as the parent, so check against group boxes first
			Control control = (Control)controls[0];										//	get the first control out of the supplied array list
			System.Windows.Forms.GroupBox groupBox = null;
			bool isInsideGroupBox = true;
			try
			{
				groupBox = (GroupBox)control.Parent;									//	see if we get a successful cast to a 'Group Box'
			}
			catch
			{
				isInsideGroupBox = false;												//	if NOT, then we'll leave the 'control' param as is
			}
			if ( isInsideGroupBox )
			{
				control = groupBox;														//	otherwise, make the 'GroupBox' the 'control' instead to get its parent...
			}

			// BRK old beginning of function
			System.Windows.Forms.TabPage tabPage = new TabPage();						//	fill with empty control to start by default
			System.Windows.Forms.TabControl tabControl;
			bool moreParentTabs;
			bool isInsideTab = true;
			int tabPageIndex;
			try
			{
				tabPage = (TabPage)control.Parent;
			}
			catch
			{
				isInsideTab = false;
			}
			if ( isInsideTab )
			{
				do
				{
					moreParentTabs = true;
					tabControl = (TabControl)tabPage.Parent;
					for ( tabPageIndex = 0; tabPageIndex < tabControl.TabPages.Count; ++tabPageIndex )
					{
						if ( tabPage == tabControl.TabPages[tabPageIndex] )
							break;
					}
					tabControl.SelectedIndex = tabPageIndex;
					try
					{
						tabPage = (TabPage)tabControl.Parent;
					}
					catch
					{
						moreParentTabs = false;
					}
				} while ( moreParentTabs );
			}
			ArrayList controlsCopy = controls;											//	by default, make a copy
			if ( badControlIndex != -1 )
			{
				controlsCopy = new ArrayList();
				controlsCopy.Add( controls[badControlIndex] );
			}
			BackColorToggle bct = new BackColorToggle( controlsCopy, Color.Salmon );
			MessageBox.Show(errorMsg, string.Format("Error with '{0}', segment {1}", objectDesc, badSegment ));
			bct.Dispose();
		}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	PRIVATE functions for class reside below this point
//			
////////////////////////////////////////////////////////////////////////////////////////

		private void _DisplayInterpolationIntercept( Graphics gfx, float xCoord, float yCoord, int result, int width, int height )
		{
			gfx.DrawRectangle(Pens.YellowGreen, xCoord - 2, yCoord - 2, 5, 5 );
			int textX = (int)xCoord;
			int textY = (int)yCoord;
			if ( textX >= ( width - 10 ))
			{
				textX -= 10;
			}
			if ( textY >= ( height - 8 ))
			{
				textY -= 14;
			}
			gfx.DrawString(result.ToString(), mInterpolatePlotFont, Brushes.White, textX, textY );
		}

		private	bool	_CheckLoadBrkFile( )
		{
			string fileName = Form1.GetBrkFileFolderPath( mBinFileName );						//	add bin file to folder path
			bool fileExists = File.Exists( fileName );
			bool doSaveRequest = true;															//	assume we always want a merged save based on kernel file & utility defaults
			if ( fileExists )
			{
				FileAttributes fileAttrib = File.GetAttributes( fileName );			//	get attributes for the file...
				if ( ( fileAttrib & FileAttributes.ReadOnly ) != 0 )
				{
					doSaveRequest = false;														//	don't do save file operation if bin file is 'read only'
					if ( !mExportFileCall )
					{
						MessageBox.Show(string.Format("Attention, you must SIGN OUT\n\n'{0}'\n\nfrom perforce in order to fully tune this utility\n\nUtility will operate, but no 'save file' option is valid.\n\nEnsure you RE-LAUNCH utility if BRK file was modified by anybody else.", fileName ), "Warning! Non writeable file!" );
					}
				}
			}
			if ( fileExists )
			{
				ResourceKernel tempResourceKernel = new ResourceKernel( mWhichUtil, fileName, false );		//	instantiate a file wrapper on the SAVED bin file (but it won't be permanent)
				int			saveFileVersionNum = tempResourceKernel.mFileHeader.mVersion;			//	get the version # from the save file...
				DialogResult	result = DialogResult.Yes;										//	by default, assume it will be ok...
				if (( mMinValidFileVersion != -1 ) &&											//	if the user has NOT specified a min save file version...
					( saveFileVersionNum < mMinValidFileVersion ))								//	or they have, and the save file version doesn't match...
				{
					string query = string.Format(	"You've specified version '{0}' of '{1}' as \n" + 
													"the minimum valid save Version allowed.  The \n" + 
													"current save file is Version '{2}'.\n\n"+
													"Do you want to use this file, or not?", mMinValidFileVersion, mBinFileName, saveFileVersionNum );	
					result = MessageBox.Show(query, "Possible File Version Conflict!", MessageBoxButtons.YesNo );
				}
				if ( result == DialogResult.Yes )
				{
					if ( saveFileVersionNum > mSubmittedSaveFileVersion )						//	is the save file of the load greater than the default one active?
					{
						mSubmittedSaveFileVersion = saveFileVersionNum;							//	then update the internal class version with the save file one
					}
					int			numObjectsInWrapper = tempResourceKernel.GetNumObjectsPresent();
					
					string		objectDesc;
					for ( int i = 0; i < numObjectsInWrapper; ++i )
					{
						objectDesc = tempResourceKernel.GetObjectDescByIndex(i);			//	get description of item present
						foreach ( UtilData utilData in mUtilObjects )						//	traverse through util objects to see
						{
							if ( utilData.IsMatchingItem( objectDesc ))						//	does this item have an associated 'util object'?
							{
								utilData.RefreshControlsFromBinFile( tempResourceKernel, objectDesc );	//	refresh the visible controls via the bin file loaded data
								break;														//	there will be only on match 
							}
						}
					}
				}
			}
			bool	mergedDataValid = false;												//	assume we are not going to be able to save file
			if ( doSaveRequest )
			{
				mergedDataValid = SaveFileRequest( false );									//	is the attempt to merge util & bin file save successful
			}
			else
			{
				mResourceKernel = new ResourceKernel(  mWhichUtil, fileName, false );					//	instantiate a file wrapper based on the new safe file
			}
			if ( !mergedDataValid )															//	if not...
			{
				if ( !fileExists )
				{
					if ( File.Exists( mBinFileName ) )										//	do we have a default copy of the file in the root folder of the executable?
					{
						mResourceKernel = new ResourceKernel(  mWhichUtil, mBinFileName, false );		//	instantiate a file wrapper based on the new safe file
					}
				}
				mergedDataValid = ( mResourceKernel != null );								//	if not, return out whether we have a valid 'util defaults' kernal to seed the logic files
			}
			return ( mergedDataValid );										//	now that we've refreshed everything from the load, save the updated/current controls & re-init the file wrapper
		}

		private	bool  _SaveFileRequest( bool wantSaveConfirmation, string newFileName, int submittedSaveFileVersion )        		
		{
            Debug.Assert(mSubmittedSaveFileVersion >= 0);
            mSubmittedSaveFileVersion = submittedSaveFileVersion;

			bool	isValid = _ValidateAllData( false );							//	call function to verify all valid data (don't update file wrapper yet)
			if ( isValid )															//	is it ok to save the collection?
			{
				string fileName = Form1.GetBrkFileFolderPath( newFileName );
                isValid = File.Exists( fileName );
				if ( isValid )
				{
					FileAttributes fileAttrib = File.GetAttributes( fileName );			//	get attributes for the file...
					isValid = ( fileAttrib & FileAttributes.ReadOnly ) == 0;
                    if ( !isValid )
					{
						MessageBox.Show(string.Format("Cannot Save out file!\n\n'{0}'\n\nMust be checked out of perforce to be made 'writeable'.", fileName ), "Invalid 'Read Only' file!" );
					}
				}
				else
				{
					isValid = true;		//	if file doesn't exist, mark it as 'valid' so that we now create it
					if ( !Directory.Exists( Form1.GetBrkFileFolderPath( "" ) ) )
					{
						fileName = mBinFileName;	//	only create a 'local' copy in the same folder as exeuctable
					}
				}

                if (isValid)
				{
                    mBinFileName = newFileName;

					FileStream myStream = File.Create( fileName );
					BinaryWriter myFile = new BinaryWriter( myStream );

                    ResourceKernel.FileHeader hdr;
                    hdr.mVersion = (sbyte)submittedSaveFileVersion;
                    hdr.mNumObjects = (short)mUtilObjects.Count;
					int headerStructSize = System.Runtime.InteropServices.Marshal.SizeOf(hdr);
					
                    myFile.Write((short)headerStructSize);
                    myFile.Write(hdr.mVersion);
                    myFile.Write(hdr.mNumObjects);

                    mRegisteredNamesList.Sort();

					foreach ( UtilData utilData in mUtilObjects )
					{                        
                        utilData.WriteToDisk(myFile, mRegisteredNamesList, hdr);
						utilData.ClearDirtyStatus();				//	clear any dirty flags, as we're going to re-create all of the transfer items
					}
					myFile.Write( (char)'B' );
					myFile.Write( (char)'R' );
					myFile.Write( (char)'K' );
					
					myFile.Close();
					myStream.Close();
					
					mResourceKernel = new ResourceKernel( mWhichUtil, fileName, false );			//	instantiate a file wrapper based on the new safe file

					ComponentDirtyNotification();											//	tell our creator we are 'clean/updated'
					
					if ( wantSaveConfirmation )
					{
//						MessageBox.Show(string.Format(	"Successfully wrote out :\n\n< {0} >\n\n\t    ( file version : {1} )\n\nDON'T FORGET to copy file to 'runtime\\ps3' folder path as well!", fileName, mSubmittedSaveFileVersion), "Save Complete" );
						MessageBox.Show(string.Format(	"Successfully wrote out :\n\n< {0} >", fileName), "Save Complete" );
					}
				}
			}
			return ( isValid );
		}

		private  bool	_AddIgnoreUponLoadForSegments( string label, int[] whichSegments )
		{
			bool	foundMatch = false;													//	assume we'll not find a match by default
			foreach ( UtilData utilData in mUtilObjects )								//	traverse though all registered items
			{
				if ( utilData.IsMatchingItem( label ) )									//	is the current item the registered item?
				{
					utilData.AddIgnoreUponLoadSegment( whichSegments );					//	pass on the ignore status to the specific component
					foundMatch = true;													//	track that we found the item
					break;																//	stop when we find it
				}
			}
			return ( foundMatch );
		}

		
		/// <summary>
		/// public support function to convert 16 bit shorts into hi byte/low byte format of the xbox 360 & ps3, as opposed to pc 16 bit format...
		/// </summary>

		private		bool		_IsValidNameLabel( string name, bool wantErrorPopup )
		{
			bool	isValid = true;															//	assume it's valid by default...
			int		nameLength = name.Length;												//	get length...
			string	errorMsg = "";															//	declaration for error message string
			foreach ( UtilData utilData in mUtilObjects )									//	traverse though all registered items
			{
				if ( utilData.IsMatchingItem( name ) )										//	is the current item the registered item?
				{
					isValid = false;
					errorMsg = string.Format("Invalid sentinel registration! <{0}> already previously used!", name );
				}
			}
			if ( isValid )											//	only process this if not already registered...
			{
				if (( nameLength == 0 ) || ( nameLength > 17 ))		//	make sure name isn't too small or too big
				{
					isValid = false;								//	not valid size
					errorMsg = string.Format("Invalid 'name' <{0}>. String must be between 1 and 17 characters long!", name );
				}
				else
				{
					char	stringVal;								//	declaration for variable...
					for ( int i = 0; i < nameLength; ++i )			//	process each char in the string to ensure it's one of our allowable chars
					{
						stringVal = name[i];						//	get character one at a time...
						if ((( stringVal >= 'A' ) && ( stringVal <= 'Z' )) ||	//	is it a valid upper case letter?
							(( stringVal >= 'a' ) && ( stringVal <= 'z' )) ||	//	is it a valid lower case letter?
							( stringVal == '_' ) ||								//	or its our 'underscore'?
							(( stringVal >= '0' ) && ( stringVal <= '9' )))		//	or a digit from '0' to '9' numerically?
						{
							continue;								//	these characters are authorized...
						}
						else
						{
							isValid = false;						//	not valid size
							errorMsg = string.Format("Invalid 'name' <{0}>. Only 'A' to 'Z' and '0' to '4' allowed in label!!", name );
							break;
						}
					}
				}
			}
			if (( wantErrorPopup ) && ( !isValid ))										//	did we have a format problem, and we want a popup?
			{
				MessageBox.Show(errorMsg, "'DataAccess' name format error!" );
			}
			return ( isValid );															//	let caller know whether name was valid or not...
		}

		private bool _ValidateAllData( bool updateResourceKernel )
		{
			bool	validToSave = true;									//	assume no problems at first...
			foreach ( UtilData utilData in mUtilObjects )
			{
				if ( !utilData.RefreshArrayValues( true ) )				//	ensure all items are refreshed from the last text box/combo values
				{
					validToSave = false;								//	stop when we encounter a problem do NOT save...
					break;
				}
				else if ( ( updateResourceKernel ) &&					//	if item was valid, copy the items from the controls to the file wrapper class
				          ( mResourceKernel != null ) )					//	dynamically re-initialized combos may trigger this call back before kernel is initialized
				{
					utilData.TransferModifiedValues( mResourceKernel );	//	transfer any modified values over to the file wrapper class
				}
			}
			return ( validToSave );
		}

		private void	_GetInterpolationRange( ResourceKernel validKernel, string objectDesc, int segmentIndex, out int min, out int max )
		{
			int segmentSize = validKernel.GetSegmentSize( objectDesc, segmentIndex );			//	get maximum size
			min = validKernel.GetValueFromSegment( objectDesc, segmentIndex, 0 );				//	get first value in list
			max = validKernel.GetValueFromSegment( objectDesc, segmentIndex, segmentSize - 1 );	//	get max
			if ( max < min )
			{
				int temp = max;
				max = min;
				min = temp;
			}
		}

		private float	_GetInterpolationDrawMod( int numPixels, int min, int max )
		{
			int valDiff = max - min;													//	get range between...
			if ( valDiff < 0 )
			{
				valDiff = min;
				min = max;
				max = valDiff;
				valDiff = max - min;
			}
			float mod = (float)numPixels / (float)valDiff;								//	get # of pixels in this dimension
			return ( mod );
		}



        public string GetCodeString(string localDef)
        {
            string res = "";
            mRegisteredNamesList.Sort();
            foreach (string s in mRegisteredNamesList)
            {
                res += localDef + "( prefix, " + s.ToUpper() + " ) \\\n";
            }
            return res.Remove(res.Length-2)+"\n";
        }
        
        static public void	PrepNumberPanel( Panel panel, string numberFormat, int minVal, int maxVal )
        {
			int height = panel.Height;
			Bitmap bmap = new Bitmap( panel.Width, height );
			Graphics gfx = Graphics.FromImage( bmap );
			gfx.Clear( panel.BackColor );
			
			int dist = (( maxVal - minVal ) + 1 );
			float yDist = (float)height / (float)dist;
			float yCoord = 0;
			Brush textBrush = new SolidBrush( panel.ForeColor );
			for ( ; minVal <= maxVal; ++minVal, yCoord += yDist )
			{
				gfx.DrawString( string.Format(numberFormat, minVal ), panel.Font, textBrush, 0, yCoord );
			}
			panel.BackgroundImage = bmap;
        }

		static public void	LayoutCtrlsInGrid(ArrayList controls, bool isHorizontal, int numCtrlsPerSubList, int gapBetweenCtrlsInSubList, int gapBetweenSubLists)
		{
			if ( ( controls != null ) && ( controls.Count > 0 ) )
			{
				Point startPoint = ( ( Control )controls[0] ).Location;
				LayoutCtrlsInGrid( controls, startPoint, isHorizontal, numCtrlsPerSubList, gapBetweenCtrlsInSubList, gapBetweenSubLists );
			}
		}

		static public void	LayoutCtrlsInGrid(ArrayList controls, Point startPoint, bool isHorizontal, int numCtrlsPerSubList, int gapBetweenCtrlsInSubList, int gapBetweenSubLists)
		{
			if ( ( controls != null ) && ( controls.Count > 0 ) )
			{
				int numControls = controls.Count;
				Control currentControl;
				Point currentPoint = startPoint;
				Size ctrlSize = ( ( Control )controls[0] ).Size;
				int currentNumCtrls = 0;
				if (numCtrlsPerSubList < 1)
				{
					numCtrlsPerSubList = 1;
				}

				for (int i = 0; i < numControls; ++i)
				{
					currentControl = ((Control)controls[i]);
					currentControl.Location = currentPoint;
					
					if (++currentNumCtrls == numCtrlsPerSubList)
					{
						// new row/column
						if (isHorizontal)
						{
							currentPoint.Y += (ctrlSize.Height + gapBetweenSubLists);
							currentPoint.X = startPoint.X;
						}
						else
						{
							currentPoint.X += (ctrlSize.Width + gapBetweenSubLists);
							currentPoint.Y = startPoint.Y;
						}
						currentNumCtrls = 0;
					}
					else
					{
						// continue along the same row/column
						if (isHorizontal)
						{
							currentPoint.X += (ctrlSize.Width + gapBetweenCtrlsInSubList);
						}
						else
						{
							currentPoint.Y += (ctrlSize.Height + gapBetweenCtrlsInSubList);
						}
					}
				}
			}
		}
	}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	This is a (essentially private, but public due to some required connections) support class for the utility manager
//			
////////////////////////////////////////////////////////////////////////////////////////

	public class UtilData
	{
		public enum ControlType
		{
			TextBox,
			ComboBox,
		};
		
		public enum Validity
		{
			NONE					= 0,		//			no special logic (OTHER THAN 'min/max' enforcement)
			Ascending				= 1,		//	A		values must ascend (only) as array is traversed
			AscendingTied			= 2,		//	AT		values must ascend (or stay same) as array is traversed
			Descending				= 4,		//	D		values must descend (only) as array is traversed
			DescendingTied			= 8,		//	DT		values must descend (or stay same) as array is traversed
			FallAndRise				= 16,		//	F		combination of 'descending' with 'ascending', each # must be unique
			FallAndRiseTied			= 32,		//  FT		same as 'FallAndRise' but allows tied #s
			RiseAndFall				= 64,		//  R		a combination of 'ascending' & 'descending' in one group of #s, each # must be unique
			RiseAndFallTied			= 128,		//  RT		same as 'rise and fall' but allows tied #s
			Sum100					= 256, 		//	%		values within array must total 100 as a sum
			SumSuppliedVal			= 512,		//	S###	values inside components must add up to supplied value with component
			ValuePairAscend			= 1024,		//	VA		special 'ascending' validity for text pair groupings 
			ValuePairAscendTied		= 2048,		//	VAT		special 'ascending' validity for text pair groupings 
			ValuePairDescend		= 4096,		//	VD		special 'descending' validity for text pair groupings
			ValuePairDescendTied	= 8192, 	//	VDT		special 'descending' validity for text pair groupings
			ValuePairNoOrder		= 16384,	//	VN		special value pair with NO ordering within pair grouping itself (for coordinates, etc)
			Boundaries				= 32768,	//	BC		a grouping of boxes denoting several layers of boundaries ( ALL values present == 'COMPLETE' )
			BoundariesPartial		= 65536,	//	B		a grouping of boxes denoting several layers of boundaries, but start/end boxes are disabled
			//	if another enum is added, ensure that the 'for loop' in 'ValidateComponent' is updated to replace 'BoundariesPartial'!!!
			_size,								//	final entry
		};
	
		private string					mObjectDesc;
		private ArrayList[]				mLists;
		private ArrayList				mDisplayLabels = new ArrayList();
		private int[]					mMinValues, mMaxValues;
		private int						mNumSegments;
		private UtilData.Validity[]		mValidity;
		private int[]					mSpecificSums;
		private bool					mStoreAsShorts;
		private bool					mIsSignedBytes;
		private bool[]					mIgnoreUponLoad;
		private	ulong[]					mRowValModBits;
		private ControlType[]			mControlTypes;
		private bool[]					mComboDisableDups;
		private int[]					mComboFinalIndex;
		private UtilityMgr				mUtilMgr;
		private bool					mIsValid = true;
		public	bool					IsValid( )				{ return ( mIsValid ); }

		public UtilData( UtilityMgr utilMgr, string objectDesc, DataSegments dataSegments, Control[] segmentParents, Point xyOffset )
		{
			mNumSegments = dataSegments.GetNumSegments();									//	ask for how many segments registered in this item

			mValidity = new UtilData.Validity[mNumSegments];								//	create a default array to store all segment id validation rules
			mComboDisableDups = new bool[mNumSegments];
			mComboFinalIndex = new int[mNumSegments];
			mMinValues = new int[mNumSegments];
			mMaxValues = new int[mNumSegments];
			mSpecificSums = new int[mNumSegments];
			mIsSignedBytes = false;
			
			int[]				segmentSizes = new int[mNumSegments];						//	build array to set up how wide each array is...

			UtilData.ControlType[] controlTypes = new UtilData.ControlType[mNumSegments];

			ArrayList[]			arrayListArray = new ArrayList[mNumSegments];				//	init here to receive pre-set ComboBox templates

			Size[]  			boxSizes = new Size[mNumSegments];

			ComboBox	template = null;
			bool		validTranslation;
			string[]	comboStrings;
			int			minMaxVal, segmentId = 0;
			string		controlText;
			string		errorDesc =  string.Format("Error with '{0}' registration!", objectDesc );	//	parse this on the assumption there will be a problem
			int			lowestLow = 1000000000;
			int			maxiestMax = 0;
			string		segmentDesc;
			Object		dataType;
			RichTextBox	sourceDataRtb;
			bool		isTextBox;
			
			for ( ; segmentId < mNumSegments; ++segmentId )
			{
				mComboFinalIndex[segmentId] = -1;									//	invalid index by default
				mSpecificSums[segmentId] = -1;
				validTranslation = true;												//	assume valid match by default...
				
				sourceDataRtb = dataSegments.GetSegmentSourceData( segmentId );
				
				dataType = dataSegments.GetSegmentDataType( segmentId );				//	get data type for this segment
				
				segmentDesc = dataType.GetType().ToString();							//	get string description of translation object type

				isTextBox = true;														//	assume tuning component is text box by default
				
				if ( segmentDesc == "System.Windows.Forms.ComboBox" )					//	is item a combo box entry
				{
					isTextBox = false;													//	not a text box segment
					template = (ComboBox)dataType;										//	try to convert the translation entry to a ComboBox
					controlTypes[segmentId] = UtilData.ControlType.ComboBox;
				}
				else if ( segmentDesc == "System.Windows.Forms.TextBox" )
				{
					boxSizes[segmentId] = ((TextBox)dataType).Size;						//	get size, so we can share same init as other 'size' translation
					controlTypes[segmentId] = UtilData.ControlType.TextBox;
				}
				else if ( segmentDesc == "System.Drawing.Size" )						//	size item?
				{
					boxSizes[segmentId] = (Size)dataType;									//	is this a box size entry for a text box?
					controlTypes[segmentId] = UtilData.ControlType.TextBox;
				}
				else
				{
					validTranslation = false;
					mIsValid = false;											//	flag that we could not initialize everything supplied
					Debug.Assert(false, string.Format("'translation[{0}] is not a 'ComboBox', 'TextBox', or 'Size' entry.", segmentId ), "Invalid translation item!" );
				}
				if ( validTranslation )												//	if we got here, and it's true, then it's a valid translation object
				{
					if ( isTextBox )
					{
						controlText = _ExtractValidtyAndNumericEntries( sourceDataRtb.Text, ref mValidity[segmentId], ref mSpecificSums[segmentId] );		//	get only the numeric entries (remove any validity/special chars)
						segmentSizes[segmentId] = UtilData.GetNumUniqueEntries( controlText );	//	find out how big entry is across...
						if (( mValidity[segmentId] & ( UtilData.Validity.Sum100 )) != 0)
						{
							if ( dataSegments.GetSegmentMin( segmentId ) < 0 )		//	do NOT allow negative #s for 'Sum100' registration items!
							{
								dataSegments.SetSegmentMin( segmentId, 0 );		//	'sum100' registrations must have a minimum of zero, stomp user supplied negative #
							}
						}
						
						if ( UtilData.IsBoundaryOrValuePair( mValidity[segmentId] )  )		//	this segment in component is a 'bounds group'
						{
							if ( ( segmentSizes[segmentId] % 2) != 0 )
							{
								mIsValid = false;											//	item is not valid if numSegments don't match size in matrix
								MessageBox.Show( string.Format("Boundary/Pair Grouping lists ( segment '{0}' ) must have an EVEN amount of boxes in bounds group!", segmentId ), errorDesc );
							}
						}
					}
					else
					{
						mValidity[segmentId] = UtilData.Validity.NONE;					//	will be set to 'None' in array constructor, but do this to be clear it's not applicable
						mComboDisableDups[segmentId] = dataSegments.GetComboDisableDup( segmentId );

						dataSegments.SetSegmentMin( segmentId, 0 );					//	set the min/max array properly based on the template
						dataSegments.SetSegmentMax( segmentId, template.Items.Count );

						comboStrings = _GetStringListAndTerminatedIndex( template, dataSegments.GetComboSegmentFinalString( segmentId ), out mComboFinalIndex[segmentId] );				//	instantiate a string array to hold all of the strings in the main template
						arrayListArray[segmentId] = ( _InitComboTemplateList( segmentParents[segmentId], template, comboStrings, segmentId, sourceDataRtb, xyOffset ));
					}
					minMaxVal = dataSegments.GetSegmentMin( segmentId );
					if ( minMaxVal < lowestLow )
					{
						lowestLow = minMaxVal;
					}
					mMinValues[segmentId] = minMaxVal;

					minMaxVal = dataSegments.GetSegmentMax( segmentId );
					if ( minMaxVal > maxiestMax )
					{
						maxiestMax = minMaxVal;
					}
					mMaxValues[segmentId] = minMaxVal;
				}
			}
			if ( mIsValid )
			{
				bool wantByteArray = ( ( lowestLow >= 0 ) && ( maxiestMax <= 255 ) );	//	check to see if value is within standard 'unsigned byte' range
				if ( !wantByteArray )												//	if we failed the 0 to 255 check, try the signed byte check
				{
					wantByteArray = ( ( lowestLow >= -128 ) && ( maxiestMax <= 127 ) );	//	check against signed byte range
					if ( wantByteArray )											//	was this range successful?
					{
						mIsSignedBytes = true;										//	then this is a signed byte array
					}
				}
				mStoreAsShorts = !wantByteArray;									//	flag whether we want the short or byte data...
		
				mUtilMgr = utilMgr;
				mControlTypes = controlTypes;
				mObjectDesc = objectDesc;		
// text box specific (if needed) init here
		
				mLists = _InitInternalArrays( segmentParents, arrayListArray, dataSegments.GetSegmentSource(), boxSizes, segmentSizes, xyOffset );

				_CommonClassInit();

				for ( segmentId = 0; segmentId < mNumSegments; ++segmentId )
				{
					if ( mControlTypes[segmentId] == ControlType.ComboBox )
					{
						foreach ( ComboBox comboBox in mLists[segmentId] )
						{
							comboBox.SelectedIndexChanged += new EventHandler( _SegmentIndexDirtyRefresh );
						}
					}
					else if ( mDisplayLabels[segmentId] != null )
					{
						_TextChangedDisplayLabelRefresh( mLists[segmentId][0], null );
					}
				}
			}
		}

		private string[]	_GetStringListAndTerminatedIndex( ComboBox template, string finalString, out int terminationStringIndex )
		{
			int		numStrings = template.Items.Count;								//	get # of strings in the template's list
			string[]	comboStrings = new string[numStrings];						//	instantiate a string array to hold all of the strings in the main template
			terminationStringIndex = -1;											//	assume no matching string index by default
			for ( int row = 0; row < numStrings; ++row )
			{
				comboStrings[row] = template.Items[row].ToString();
				if ( comboStrings[row] == finalString )								//	did the string match our supplied param?
				{
					terminationStringIndex = row;									//	track what row was present for the match... (if any)
				}
			}
			return ( comboStrings );
		}

		private ArrayList	_InitComboTemplateList( Control parent, ComboBox original, string[] origStringArray, int segmentIndex, RichTextBox config, Point locOffset )
		{
			bool	wider = true;									//	by default, assume 'horizontal' orientation (look for 'carriage return' to make vertical instead)
			ArrayList	copies = new ArrayList();					//	array list to store the #s we are going to retrieve out..
			string	tempStr, sourceText = config.Text + ' ';		//	gets out the text as a string (add ascii char at end to ensure 'end int' parsing succeeds
			int		index = 0;										//  read location
			int		strLen = sourceText.Length;						//	end location
			int		numStartIndex = 0, tempNum, tempLen = 0;		//	variable declaration...
			bool	disable, inString = false;						//	set to false to start...
			while ( index != strLen )								//	outer control loop to traverse text
			{
				if ( sourceText[index] == '\"' )
				{
					if ( !inString )								//	did we JUST encounter a new # parse?
					{
						inString = true;							//	set flag to true... (indicate start parse)
						numStartIndex = index + 1;					//	keep track of the starting index of the string
						tempLen = 0;								//	automatically count the first digit of the #
					}
					else
					{
						inString = false;							//	turn off the flag...
						tempStr = sourceText.Substring(numStartIndex, tempLen);	//	isolate the string that is our #
						copies.Add( tempStr );						//	add the # to our ArrayList of submitted copies...
					}
				}
				else // treat anything OTHER than a '%' or digit as a 'end string' or 'empty space'
				{
					if ( sourceText[index] == '\n' )				//	did we find a carriage return?
						wider = false;								//	if so, then orient the combo list downwards...
					else if ( inString )							//	were we parsing a #?
						++tempLen;									//	if NOT, increase the digits of the # we are parsing...
				}
				++index;											//	next item into string...
			};
			if ( inString )
			{
				string errorMsg = string.Format("Error! There is a 'format' problem with :\n\nText   \t      : < {0} > !\nMapDefRow  : < {1} >. \n\nAll 'copies' must be enclosed in quotes.", sourceText, config.Name);
				MessageBox.Show(errorMsg, "ERROR!");
			}
			int numCopies = copies.Count;							//	# of ComboBoxes to create, and a loop size variable
			ArrayList		comboArray = new ArrayList();
			Point coord = new Point(( config.Location.X + locOffset.X ), ( config.Location.Y + locOffset.Y ) );
			int width = original.Width;
			int height = original.Height;							//	dimensions of original

			Size  comboDimension = new Size(width,height);			//	make a size copy based on dimensions of the original

			int divisor = numCopies - 1;							//  variable defs...
			if ( divisor == 0 )
				divisor = 1;

			int coordMod = config.Height - height;					//	by default, assume vertical orientation, set up 'coord mod' for that
			tempNum =  height;										//	part two of veritical assumption

			if ( wider )
			{
				coordMod = config.Width - width;					//	# of pixels in rich text box (across) minus one boxes worth
				tempNum =  width;									//	ensure coordinate offset is at least 'width' in size
			}

			coordMod /= divisor;									//	divide that remaining area by the # of boxes in it
			if ( coordMod < tempNum )
				coordMod = tempNum;									//	make sure mod is at least as wide as our text box
			tempLen = original.Items.Count;							//	get # of items...
			for ( index = 0; index < numCopies; ++index )			//	loop through array list and create a textBox for each #
			{
				ComboBox combo = new ComboBox();					//	create a new text box that exists while the utility is running
				combo.DropDownStyle = ComboBoxStyle.DropDownList;	//	set the style to be only selections present in the list, no 'typing'
				combo.Items.AddRange( origStringArray );
				combo.Location = coord;

				combo.Size = comboDimension;						//	set the size
				combo.Font = original.Font;
				combo.Tag = new Point( index, segmentIndex );		//	store item's unique id in the 'Tag' component of control
				
				tempStr = ((string)copies[index]);					//	get string to copy...
				disable = ( tempStr[0] == '*' );					//	is asterix present?
				if ( disable)										//	are we disabling?
				{
					tempStr = tempStr.Substring(1, tempStr.Length - 1);
					combo.Enabled = false;
				}
				combo.Text = tempStr;								//	seed the text box with the appropriate # in it...
				Debug.Assert(( combo.SelectedIndex != -1 ), string.Format("Invalid combo text default supplied! < {0} >", tempStr ));
				if ( wider )
					coord.X += coordMod;							//	add coordinate to x to align the box...
				else
					coord.Y += coordMod;							//	adjust y coordinate instead if vertically aligned
				parent.Controls.Add(combo);							//	add combo box to the supplied parent control

				comboArray.Add( combo );							//	store the box in the array...
				combo.BringToFront();								//	ensure its brought to the front of the tab/page its on
			}
			original.Hide();										//	get rid of the ComboBox 'original template' now that we've used it to init the boxes....
			config.Hide();											//	get rid of the 'RichTextBox' config too...

			return ( comboArray );
		}

		private string	_CheckAndRemoveValidity( string controlText, string validityDesc, UtilData.Validity validityEnum, ref UtilData.Validity validityBuild )
		{
			int	stringIndex = controlText.IndexOf(validityDesc);						//	check to see if the string is present...
			if ( stringIndex != -1 )													//	was the string found?
			{
				controlText = controlText.Remove( stringIndex, validityDesc.Length );	//	remove the validation key from the source string and keep modified string
				validityBuild |= validityEnum;											//	'or' in the validity flag that is associated with the text entry
			}
			return ( controlText );														//	return (potentially modified) string...
		}
		

		private string _ExtractValidtyAndNumericEntries( string text, ref UtilData.Validity validityFlags, ref int sumVal )
		{
			Validity	tempValidity = Validity.NONE;				//	by default, assume no special validity

			text = text.ToUpper();														//	convert to upper case to ensure pattern matching	
			text = _CheckAndRemoveValidity( text, "VN", Validity.ValuePairNoOrder, ref tempValidity  );		//	check if 'value pair' with no ordering
			text = _CheckAndRemoveValidity( text, "VAT",Validity.ValuePairAscendTied, ref tempValidity  );	//	check if 'value pair ascending tied' present
			text = _CheckAndRemoveValidity( text, "VA",	Validity.ValuePairAscend, ref tempValidity  );		//	check if 'value pair ascending' present
			text = _CheckAndRemoveValidity( text, "AT",	Validity.AscendingTied, ref tempValidity  );		//	check if 'ascending tied' present
			text = _CheckAndRemoveValidity( text, "A",	Validity.Ascending, ref tempValidity  );			//	check if 'ascending' present
			text = _CheckAndRemoveValidity( text, "VDT",Validity.ValuePairDescendTied, ref tempValidity  );	//	check if 'value pair descending tied' present
			text = _CheckAndRemoveValidity( text, "VD",	Validity.ValuePairDescend, ref tempValidity  );		//	check if 'value pair descending' present
			text = _CheckAndRemoveValidity( text, "DT",	Validity.DescendingTied, ref tempValidity  );		//	check if 'descending tied' present
			text = _CheckAndRemoveValidity( text, "D",	Validity.Descending, ref tempValidity  );			//	check if 'descending' present
			text = _CheckAndRemoveValidity( text, "FT",	Validity.FallAndRiseTied, ref tempValidity  );		//	check if 'fall and rise tied' present
			text = _CheckAndRemoveValidity( text, "F",	Validity.FallAndRise, ref tempValidity  );			//	check if 'fall and rise' present
			text = _CheckAndRemoveValidity( text, "RT",	Validity.RiseAndFallTied, ref tempValidity  );		//	check if 'rise and fall tied' present
			text = _CheckAndRemoveValidity( text, "R",	Validity.RiseAndFall, ref tempValidity  );			//	check if 'rise and fall' present
			text = _CheckAndRemoveValidity( text, "BC",	Validity.Boundaries, ref tempValidity  );			//	check if 'boundaries (full)' present
			text = _CheckAndRemoveValidity( text, "B",	Validity.BoundariesPartial, ref tempValidity  );	//	check if 'boundaries (partial)' present
			
			int stringIndex = text.IndexOf("%");										//	ask if there is a percent sign present
			if ( stringIndex != -1 )													//	did we find one?
			{
				text = text.Remove( stringIndex, 1 );									//	remove the "%" source string and keep modified string
				tempValidity |= Validity.Sum100;									//	if so, turn on the 'sum100' validity flag
				sumVal = 100;
			}
			stringIndex = text.IndexOf("S");											//	check to see if 'sum' flag is there
			if ( stringIndex != -1 )													//	was the 'supplied sum' flag present?
			{
				text = text.Remove( stringIndex, 1 );									//	remove the validation key from the source string and keep modified string
				int startIndex = stringIndex;											//	keep track of initial location
				int maxLength = text.Length;											//	get maximum length...
				while ( ( text[stringIndex] >= '0' ) && ( text[stringIndex] <= '9' ))	//	is the first char after the 'S' part of a #?
				{
					if ( ++stringIndex == maxLength )									//	traversed till end of string?
					{
						break;															//	stop if so...
					}
				}
				int length = ( stringIndex - startIndex );								//	get length of sum string
				if ( length >= 1 )
				{
					sumVal = int.Parse(text.Substring(startIndex, length ) );			//	get sum value...
					text = text.Remove( startIndex, length );							//	remove the validation key from the source string and keep modified string	
					tempValidity |= Validity.SumSuppliedVal;							//	if so, turn on the 'sum supplied val' validity flag
				}
			}
			if ( tempValidity != Validity.NONE )										//	did we assign any special validity by reviewing the text?
			{
				validityFlags = tempValidity;											//	if so, replace the previous validity with the embedded validity
			}
			return ( text );
		}
				
		public  ArrayList[]		GetGeneratedControls()
		{
			return ( mLists );
		}
		
		private void	_CommonClassInit()
		{
			mNumSegments = mLists.Length;
			mIgnoreUponLoad = new bool[mNumSegments];
			ClearDirtyStatus();
		}
		
		public void	AddIgnoreUponLoadSegment( int[] segmentIndexes )
		{
			if ( segmentIndexes[0] == -1 )
			{
				for ( int i = 0; i < mNumSegments; ++i )
				{
					mIgnoreUponLoad[i] = true;
				}
			}
			else
			{
				foreach ( int segmentIndex in segmentIndexes )
				{
					mIgnoreUponLoad[segmentIndex] = true;
				}
			}
		}
		
		public void ClearDirtyStatus()
		{
			mRowValModBits = new ulong[mNumSegments];
		}

		public bool CheckIfDirtySegments()
		{
			bool	isDirty = false;
			for ( int i = 0; i < mNumSegments; ++i )
			{
				if ( mRowValModBits[i] != 0 )
				{
					isDirty = true;
					break;
				}
			}
			return ( isDirty );
		}

        public void WriteToDisk(BinaryWriter fileWriter, List<string> strList, ResourceKernel.FileHeader hdr)
		{
            if (hdr.mVersion == 1)
            {
                fileWriter.Write((byte)strList.BinarySearch(mObjectDesc));
            }

			uint[] packedData = _PackSentinalData();
			fileWriter.Write( (uint)packedData[0] );									//	write out 1 of 3 (maybe 4)packed ints  (contains packed name & short/signed bools)
			fileWriter.Write( (uint)packedData[1] );									//	write out 2 of 3 (maybe 4)packed ints
			fileWriter.Write( (uint)packedData[2] );									//	write out 3 of 3 (maybe 4)packed ints
			if ( ( packedData[1] & 0x00000001 ) != 0 )
			{
				fileWriter.Write( (uint)packedData[3] );								//	write out 4th of 4 packed ints, if special 13+ long registration
			}
			int i = 0;
			short baseAddress = 0;
			for ( ; i < mNumSegments; ++i )
			{
				fileWriter.Write( baseAddress );										//	save out 'numItems' memory offsets
				baseAddress += (short)mLists[i].Count;									//	add the size of each sub component as the 'offset' running total
			}
			short boxVal;
			for ( i = 0; i < mNumSegments; ++i )
			{
				if ( mControlTypes[i] == ControlType.TextBox )
				{
					foreach ( TextBox textBox in mLists[i] )
					{
						boxVal = GetShortValue(textBox);
						if ( mStoreAsShorts )												//	is this 'short' data on disk?
							fileWriter.Write( boxVal );										//	save our our storage buffer as signed shorts to disk
						else if ( mIsSignedBytes )
							fileWriter.Write( (sbyte)boxVal );								//	save our our storage buffer as signed byte to disk
						else 
							fileWriter.Write( (byte)boxVal );								//	save our our storage buffer as unsigned byte to disk
					}
				}
				else if ( mControlTypes[i] == ControlType.ComboBox )
				{
					foreach ( ComboBox comboBox in mLists[i] )
					{
						if ( mStoreAsShorts )
						{
							fileWriter.Write( (short)comboBox.SelectedIndex );					//	save our our storage buffer as unsigned byte to disk
						}
						else
						{
							fileWriter.Write( (byte)comboBox.SelectedIndex );					//	save our our storage buffer as unsigned byte to disk
						}
					}
				}
			}
		}
		
		public	bool		IsMatchingItem( string descLabel )
		{
			bool	matchingItem = ( mObjectDesc == descLabel ) ;
			return ( matchingItem );
		}
		
		public	void		RefreshControlsFromBinFile( ResourceKernel tempResourceKernel, string objectDesc )
		{
			int		row = 0, x, val;
			int		activeSegmentSize, tempSegmentSize;
			bool	isTextBoxList;
			for ( ; row < mNumSegments; ++row )
			{
				if (( !mIgnoreUponLoad[row] )	&&										//	don't do update/read if the row's been 'disable' for load
					( tempResourceKernel.IsValidSegmentIndex( objectDesc, row ) ))			//	or the wrapper object does not contain this number of segments/rows
				{
					activeSegmentSize = mLists[row].Count;								//	get how big the active segment we are going to update is...
					tempSegmentSize = tempResourceKernel.GetSegmentSize( objectDesc, row );//	ask the temp file wrapper how bit it's segment with this name is...
					if ( activeSegmentSize == tempSegmentSize )							//	only do the transfer if both arrays are the same size, otherwise one set of data is obsolete
					{
						isTextBoxList = _IsTextBoxSegment(row);
						for ( x = 0; x < activeSegmentSize; ++ x)
						{
							val = tempResourceKernel.GetValueFromSegment( objectDesc, row, x );
							if ( isTextBoxList )
							{
								((TextBox)mLists[row][x]).Text = val.ToString();
							}
							else
							{
								((ComboBox)mLists[row][x]).SelectedIndex = val;
							}
						}
					}
				}
			}
		}
		
		public	bool		RefreshArrayValues( bool wantErrorPopup )
		{
			bool	validItem = true;													//	assume valid component by default...
			int		lastIndex, y = 0, x, i, size, badRow = -1;
			short	minVal, maxVal;
			short	val;
			string	errorMsg = "";
			
			for ( ; y < mNumSegments; ++y )
			{
				size = mLists[y].Count;													//	get variable for size of array, we'll use it multiple times
				short[] vals = new short[size];											//	create short array to store the datra

				if ( _IsTextBoxSegment(y) )
				{
					minVal = (short)mMinValues[y];										//	grab the min value from the segment
					maxVal = (short)mMaxValues[y];
					
					for ( x = 0; x < size; ++x )
					{
						vals[x] = GetShortValue( (TextBox)mLists[y][x] );
					}
					
					if ( _IsBoundaryPairSegment( y ) )				//	check if this segment is a bounds pair
					{
						lastIndex = ( size - 1 );										//	int index for final box/entry in segment
						if ( vals[0] > vals[lastIndex] )								//	is this a 'descending' group?
						{
							vals[0] = maxVal;											//	stomp the first extracted array value
							((TextBox)mLists[y][0]).Text = maxVal.ToString();			//	stomp the first text box string value
							vals[lastIndex] = minVal;									//	stomp the last extracted array value
							((TextBox)mLists[y][lastIndex]).Text = minVal.ToString();	//	stomp the last text box string value
						}
						else															//	then its a 'ascending' pair grouping
						{
							vals[lastIndex] = maxVal;									//	stomp the last extracted array value
							((TextBox)mLists[y][lastIndex]).Text = maxVal.ToString();	//	stomp the last text box string value
							vals[0] = minVal;											//	stomp the first extracted array value
							((TextBox)mLists[y][0]).Text = minVal.ToString();			//	stomp the first text box string value
						}
					}
					for ( x = 0; x < size; ++x )
					{
						val = vals[x];
						if (( val < minVal ) || ( val > maxVal ))
						{
							validItem = false;											//	flag that component was not valid due to min/max value outside of range
							errorMsg = string.Format("Invalid entry of '{0}, values must be between '{1}' and '{2}'.", val, mMinValues[y], mMaxValues[y] );
							badRow = y;													//	store the index of the array that is invalid
							y = mNumSegments;											//	break 'outer' y loop
							break;														//	break 'inner' x loop
						}
					}
					if (( validItem ) && ( mValidity[y] != Validity.NONE ) & ( vals.Length >= 2 ))//	are we still valid, and the object has a validity other than 'min/max' enforcement?
					{
						validItem = _VerifyNumericValidity( ref errorMsg, vals, mValidity[y], y );	//	verify validity enforcement, set 'errorMsg' if applicable
					}
				}
				else
				{
					for ( x = 0; x < size; ++x )
					{
						vals[x] = val = (short)((ComboBox)mLists[y][x]).SelectedIndex;
						if ( val == -1 )												//	is the current combo setting NOT a valid item in the drop down?
						{
							validItem = false;											//	flag that component was not valid due to min/max value outside of range
							errorMsg = string.Format("Invalid combo setting, selection of '{0}' is not allowed.", ((ComboBox)mLists[y][x]).Text );
							break;														//	break 'inner' x loop
						}
					}
					if (( validItem ) && (( mComboDisableDups[y] ) || ( mComboFinalIndex[y] != -1 ) ))	//	do we have any special combo validation rules?
					{
						if ( mComboDisableDups[y] )										//	did we set the 'ignore duplicate entry' flags?
						{
							for ( i = 0; i < size; ++i )								//	outer loop...
							{
								val = vals[i];											//	get value out of list...
								if ( val == mComboFinalIndex[y] )						//	is the value == to our 'combo final' index?
								{
									continue;											//	if so, allow this item to be duplicated automatically
								}
								for ( x = 0; x < size; ++x )							//	inner loop
								{
									if ( i == x )										//	if loop indexes match, don't process entry
									{
										continue;
									}
									if ( vals[x] == val )								//	do values match? If so, we have duplicated entries, no-no
									{
										validItem = false;	
										errorMsg = string.Format("Invalid combo duplication, selection '{0}' appears more than once in list.", ((ComboBox)mLists[y][x]).Text );
										i = x = size;									//	terminate both outer and inner loop
									}
								}
							}
						}
						if (( validItem ) && ( mComboFinalIndex[y] != -1 ))				//	are we still valid? Do we have 'bottom entry' enforced?
						{
							bool	foundEntry = false;									//	by default, assume we'll NOT find the entry specified
							errorMsg = string.Format("Invalid text ordering, selection '{0}' should only appear as final entry(s).", (string)((ComboBox)mLists[y][0]).Items[mComboFinalIndex[y]]);
							for ( x = 0; x < size; ++x )								//	logic loop...
							{
								val = vals[x];											//	get value out of list...
								if ( val == mComboFinalIndex[y] )						//	did we find the matching entry?
								{
									foundEntry = true;									//	flag that we found it...
								}
								else if ( foundEntry )
								{
									validItem = false;	
									break;
								}
							}
						}
					}	
				}
				if ( !validItem )
				{
					if ( badRow == -1 )
					{
						badRow = y;														//	store the index of the array that is invalid				
					}
					y = mNumSegments;													//	break 'outer' y loop
				}
			}
			if (( wantErrorPopup) && ( !validItem ))									//	did this item fail it's validity check?
			{
				UtilityMgr.DisplayErrorMessage( errorMsg, mObjectDesc, mLists[badRow], badRow, -1 );		//	call function to display error/validation issue
			}
			return ( validItem );														//	return out whether component is valid or not
		}
	
		/// <summary>
		/// private function that packs up the Label name and inserts the '16 bit' and 'signed' bools into the packed data
		/// </summary>

		private uint[] _PackSentinalData()
		{
			int stringLength = mObjectDesc.Length;										//	get size of string
			uint[]	packedData = new uint[3] { 0, 0, 0 };								//	3 unsigned ints to store the packed class data
			int[]	numChars = new int[3] { 5, 5, 2 };									//	how many chars to extract from each int...
			int		packedInts = 3;														//	assume standard 12 byte registration by default
			bool	expandedRegistration = ( stringLength >= 13 );						//	flag for expanded 13 to 17 registration
			if ( expandedRegistration )													//	is this an extended registration label segment?
			{
				packedData = new uint[4] { 0, 0, 0, 0 };								//	4 unsigned ints to store the packed class data
				numChars = new int[4] { 5, 5, 5, 2 };									//	how many chars to extract from each int...
				packedInts = 4;															//	expand registration write logic
			}
			int lastItem = ( packedInts - 1 );											//	access for final item
			int charIndex = 0, intIndex = 0, readIndex = 0;								//	set all of these to zero by default...
			int bitShift;
			uint stringVal;
			for ( ; intIndex < packedInts; ++intIndex )									//	set this up to process 3 int, extracting 5 packed characters per short
			{
				bitShift = 26;															//	shift packed letter over by a decreasing amount of bits each loop
				for ( charIndex = 0; charIndex < numChars[intIndex]; ++charIndex )		//	traverse string in groups of 3 chars
				{
					if ( readIndex == stringLength )									//	have we traversed the entire string already?
					{
						charIndex = intIndex = 5;										//	stop both inner and outer loops
						break;
					}
					stringVal = mObjectDesc[readIndex++];								//	extract out the string value one 'char' at a time
					if ( ( stringVal >= 'A' ) && ( stringVal <= 'Z' ) )					//	is it a upper case letter?
					{
						stringVal -= 'A';												//	then convert to 'zero base' of upper case letter (result will be zero to 25)
						stringVal += 1;													//	convert this letter to '1' to '26' numerically
					}
					else if ( ( stringVal >= 'a' ) && ( stringVal <= 'z' ) )			//	is it a lower case letter?
					{
						stringVal -= 'a';												//	then convert to 'zero base' of lower case letter (result will be zero to 25)
						stringVal += 27;												//	convert this letter to '27' to '52' numerically
					}
					else if ( ( stringVal >= '0' ) && ( stringVal <= '9' ) )			//	is it a digit from 0 to 9?
					{
						stringVal -= '0';												//	then convert to 'zero base' version of the digits, will result in '0 to 9' as integer
						stringVal += 53;												//	convert this letter to '53' to '62' numerically
					}
					else if ( stringVal == '_' )										//	is this an 'underscore'?
					{
						stringVal = 63;													//	then convert to 63 numerically
					}
					else 																//	if it's anything else, skip it, don't put it in label...
					{
						Debug.Assert( false, "Invalid label entry, must be 'A' to 'Z', 'a' to 'z', '_', or '0' to '9' only" );
						continue;
					}
					stringVal <<= bitShift;												//	shift the 'string val' items over 'x' bits
					bitShift -= 6;														//	reduce the # of bits we'll shift in the futrue
					packedData[intIndex] |= stringVal;									//	or in the characters per short using 6 bits per letter
				}
			}
			if ( mStoreAsShorts )														//	is this short data?
			{
				packedData[0] |= 1;														//	then turn off 'bit 1' of integer 0
			}
			if ( mIsSignedBytes )
			{
				packedData[0] |= 2;														//	then turn off 'bit 1' of integer 0
			}
			
			if ( expandedRegistration )
			{
				packedData[1] |= 1;														//	store that this is a special expanded registration
			}
			
			// extra 2 bits are available at the end of 'packedData[1]' since we don't care about 'NumBoundsGroups' anymore
			int totalSize = 0;
			foreach ( ArrayList controlList in mLists )									//	traverse each array of controls...
			{
				totalSize += controlList.Count;											//	add the # of entries in this component to the sum of all the components
			}
			
			uint widthHeight = (uint)totalSize;											//	copy the total # of unique items (NOT # of 'bytes' if its short data) in the class over
			widthHeight <<= 8;															//	shift the 'tall' over 8 bits
			widthHeight |= (uint)mNumSegments;											//	now 'or' in the # of separate components
			packedData[lastItem] |= widthHeight;										//	now put the packed width/height into the bottom 16 bits of the last int

			return ( packedData );
		}
		
		private	bool		_IsTextBoxSegment( int segmentIndex )
		{
			return ( mControlTypes[segmentIndex] == ControlType.TextBox );														//	let caller know whether this array is a text box array or not...
		}
		
		static  public  bool	IsBoundaryOrValuePair( Validity validity )
		{
			bool pairBasedValidity = (( validity & ( Validity.Boundaries|Validity.BoundariesPartial|Validity.ValuePairAscend|Validity.ValuePairAscendTied|Validity.ValuePairDescend|Validity.ValuePairDescendTied|Validity.ValuePairNoOrder )) != 0);
			
			return ( pairBasedValidity );
		}

		private bool	_IsBoundaryPairSegment( int row )
		{
			bool isPairGroupingRow = (( mValidity[row] & ( Validity.Boundaries | Validity.BoundariesPartial )) != 0);
			
			return ( isPairGroupingRow );
		}

		private bool	_IsValuePairGrouping( Validity validity )
		{
			bool isValuePairRow = (( validity & ( Validity.ValuePairAscend|Validity.ValuePairAscendTied|Validity.ValuePairDescend|Validity.ValuePairDescendTied|Validity.ValuePairNoOrder )) != 0);
			
			return ( isValuePairRow );
		}

		private bool	_IsValuePairRow( int row )
		{
			return ( _IsValuePairGrouping( mValidity[row] ));
		}

		private class HideShowLbl
		{
			private TextBox		mTextBox;
			private Label		mLabel;
			
			public HideShowLbl( TextBox textBox, Label label )
			{
				mTextBox = textBox;
				mLabel = label;

				mTextBox.VisibleChanged += new EventHandler( _TextBoxVisibleChanged );
			}

			void _TextBoxVisibleChanged(object sender, EventArgs e)
			{
				if ( mTextBox.Visible )
				{
					mLabel.Show();
					mLabel.BringToFront();
					mTextBox.BringToFront();
				}
				else
				{
					mLabel.Hide();
				}
			}
			
		}
		
		private ArrayList[]	_InitInternalArrays( Control[] parents, ArrayList[] preInitArrays, RichTextBox[] arrays, Size[] boxSizes, int[] segmentSizes, Point xyOffset )
		{
			int			rowsTall = arrays.Length;
			ArrayList[] arrayListArray = new ArrayList[rowsTall];
			
			int			numValidBoxes, sizeUsageBoxes, sizeInPixels, boxUsedSpace, i, row = 0;
			int			boxX, boxY, modX, modY;
			RichTextBox	sourceBox;
			short[]		values;
			string		arrayString;													//	string for item being parsed/processed...
			bool		havePairAssociation = false;									//	by default, assume no pair grouping, straight int array display
			bool		wantSumDisplay = false;
			int			pairGroupXMod;													//	build var for how much we'll offset pair grouping pair boxes
			int			remainingPairs;
			ArrayList	enabledDisabled;
			
			for ( ; row < rowsTall; ++row )
			{
				if ( mControlTypes[row] == ControlType.ComboBox )
				{
					mDisplayLabels.Add( null );											//	add a null in order of the rows, so we know it's not valid
					arrayListArray[row] = preInitArrays[row];							//	simply copy over combo box pre-init that's been supplied
					continue;
				}
				pairGroupXMod = boxSizes[row].Width + 4;								//	build var for how much we'll offset pair grouping pair boxes
				sourceBox = arrays[row];												//	get item out of the array...
				arrayString = sourceBox.Text;											//	grab the string, we'll use it more than once
				enabledDisabled = new ArrayList();
				values = AsShortArray( arrayString, enabledDisabled );					//	get all the source values out of the text boxes
				sourceBox.Hide();														//	remove the 'source' item from the visible control list of the util
				numValidBoxes = segmentSizes[row];										//	get how many items user wants visible (grouped items need to be stored as width/height square, but visibly, some boxes may not be there)
				sizeUsageBoxes = numValidBoxes;											//	by default, make this the same value...
				havePairAssociation = false;											//	assume this is NOT a 'pair grouping' by default...
				
				if ( IsBoundaryOrValuePair( mValidity[row] ) )							//	if either a pair grouping or value grouping, do special placement logic
				{
					havePairAssociation = true;
					sizeUsageBoxes /= 2;												//	for pair groupings, half this value...
				}
				modX = modY = 0;														//	reset these each loop so they aren't left dangling per row
				if ( sourceBox.Width > ( sourceBox.Height * 2 ) )						//	is this a 'horizontally' oriented control list?
				{
					sizeInPixels = sourceBox.Width;										//	how wide is the total space used by the unpack pattern?
					if ( havePairAssociation )											//	is it a pair grouping?
					{
						sizeInPixels -= ( pairGroupXMod + boxSizes[row].Width );		//	remove how much one complete pair group will be in pixels across
						remainingPairs = ( (numValidBoxes / 2 ) - 1 );
						if (remainingPairs > 0)
							sizeInPixels /= remainingPairs;								//	now divide that remainder by the # of OTHER remaining pairs
					}
					else if ( numValidBoxes >= 2)										//	are multiple boxes being unpacked from one 'rtb'?
					{
						boxUsedSpace = ( numValidBoxes * boxSizes[row].Width);			//	determine how much space is needed for the visible area of the boxes
						sizeInPixels -= boxUsedSpace;									//	subtract the space used to display 'x' boxes horizontally
						sizeInPixels /= ( numValidBoxes - 1 );							//	now create value that exists between each box
						sizeInPixels += boxSizes[row].Width;								//	add the width of one box, so that this value is 'skip x' for next box
					}
					modX = sizeInPixels;												//	set 'modX' to be the X pixel offset...
				}
				else
				{
					sizeInPixels = sourceBox.Height;									//	how wide is the total space used by the unpack pattern?
					boxUsedSpace = ( sizeUsageBoxes * boxSizes[row].Height );			//	determine how much space is needed for the visible area of the boxes
					if ( numValidBoxes >= 2 )											//	are multiple boxes being unpacked from one 'rtb'?
					{
						sizeInPixels -= boxUsedSpace;									//	subtract the space used to display 'x' boxes horizontally
						sizeInPixels /= ( sizeUsageBoxes - 1 );							//	now create value that exists between each box
						sizeInPixels += boxSizes[row].Height;							//	add the width of one box, so that this value is 'skip x' for next box
						modY = sizeInPixels;											//	set 'modX' to be the pixel offset...
					}
				}
				boxX = ( sourceBox.Location.X + xyOffset.X );							//	get x coordinate of master unpack control (add offset if exists)
				boxY = ( sourceBox.Location.Y + xyOffset.Y );							//	ditto y
				int prevX = boxX;
				ArrayList textBoxes = new ArrayList();									//	create an array list to save the boxes we create...
				bool	inPairGroup = false;											//	by default, assume not inside a pair grouping...
				TextBox textBox = null;													//	define here, so we can have access to it later, if required
				for ( i = 0; i < numValidBoxes; ++i )									//	traverse through each (visible) box in the unpack buffer
				{
					textBox = new TextBox();											//	create a new text box that exists while the utility is running
					textBox.AutoSize = false;											//	set auto size to 'false', we want a custom dimension to it
					textBox.Size = boxSizes[row];										//	set the size
					textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;//	set the text alignment for the box
					textBox.Location = new Point(boxX, boxY);							//	set the boxes coordinate...
					textBox.Text = values[i].ToString();								//	seed the text box with the appropriate # in it...
					textBox.Font = sourceBox.Font;										//	copy the font over...
					textBox.BackColor = sourceBox.BackColor;							//	copy the background color...
					parents[row].Controls.Add(textBox);									//	add it to the 'config' boxes parent, so it also shows up
					textBox.BringToFront();												//	ensure its brought to the front of the tab/page its on	
					textBox.Tag = new Point( i, row );									//	have each item store the row/across index of each box that is present
					textBoxes.Add( textBox );											//	save the box in our own array list...
					
					textBox.TextChanged += new EventHandler(_SegmentIndexDirtyRefresh);		//	add call back so we know when a row changes...
					
					if ( !havePairAssociation )											//	if not a pair grouping, automatically update each box's coord by default...
					{
						boxX += modX;													//	adjust both x/y coords if not a pair grouping...
						boxY += modY;
					}
					else																//	otherwise we are in a pair grouping...
					{
						inPairGroup = !inPairGroup;										//	alternate this flag for every other box...
						if ( inPairGroup )												//	are we doing the 2nd box in a group?
						{
							prevX = boxX;												//	track the previous 'base point'...
							boxX += ( pairGroupXMod );									//	2nd box automatically adjacent to first box in pair (horizontally)
						}
						else															//	if we are finished 2nd box in group...
						{
							boxX = prevX;												//	re-set the x coord for new pair location...
							if ( modY == 0 )											//	if we have NO y modifier...
							{
								boxX += modX ; 											//	shift it over into a new pair area
							}
							else
							{
								boxY += modY;
							}
						}
					}
					if ( !(bool)enabledDisabled[i] )
					{
						textBox.ReadOnly = true;										//	if any 'asterix' submitted with values, disable box entry
						textBox.MouseWheel += new MouseEventHandler( EnableBoxMouseWheel );
					}
				}
				arrayListArray[row] = textBoxes;										//	save each array list for each row of boxes
				Label displayLbl = null;												//	create a empty/null control by default...
				wantSumDisplay = ( arrayString.IndexOf("#") != -1 );					//	check string to see if the '#' is present...
				
				if ((( mValidity[row] & ( Validity.Sum100 | Validity.SumSuppliedVal )) != 0 ) ||	//	is this a special 'percentage sum' item?
					( wantSumDisplay ) )												//	or, the user asked for a manual sum to be displayed
				{
					System.Drawing.ContentAlignment alignment = System.Drawing.ContentAlignment.MiddleLeft;
					if ( modX != 0 )													//	were we creating offsets based on the x width?
					{
						boxX -= modX;													//	then reduce the offset for the label by the width of the box
						boxX += ( boxSizes[row].Width );
						boxY += 1;
					}
					else
					{
						alignment = System.Drawing.ContentAlignment.TopLeft;
						boxY -= modY;													//	reduce the offset by the height of the last box added
						boxY += ( boxSizes[row].Height - 3 );							//	skip down a bit so it's not too close to the last box
						boxX -= 2;
					}
					displayLbl = new Label();											//	initialize display label item
					displayLbl.Location = new Point(boxX, boxY);						//	place at next text box location (if there was one)
					displayLbl.Font = new Font( "Sylfaen", 7.0f );						//	use the small 'Sylfaen' font for this display
					displayLbl.Size = new Size(30,11);
					displayLbl.TextAlign = alignment;
					displayLbl.BackColor = parents[row].BackColor;
					parents[row].Controls.Add( displayLbl );								//	add it to the 'config' boxes parent, so it also shows up
					displayLbl.BringToFront();
					textBox.BringToFront();												//	ensure its brought to the front of the tab/page its on	

					foreach ( TextBox tb in textBoxes )
					{
						tb.TextChanged += new EventHandler( _TextChangedDisplayLabelRefresh );
					}
					new HideShowLbl( textBox, displayLbl );
				}
				else if ( _IsBoundaryPairSegment(row) )
				{
					bool isFullValidityBounds = (( mValidity[row] & Validity.Boundaries ) != 0 );
					if ( ( mValidity[row] & Validity.Ascending|Validity.AscendingTied|Validity.Descending|Validity.DescendingTied ) == 0 )	// NO descending or ascending rules set by user, but they MUST be there for bounds to be valid
					{
						short firstBoxVal = GetShortValue((TextBox)textBoxes[0]);					//	get first value in bounds grouping list
						short lastBoxVal = GetShortValue((TextBox)textBoxes[( numValidBoxes - 1 )]);//	get last value in bounds grouping list
						if ( lastBoxVal > firstBoxVal )												//	the last current box is > than the current first
						{
							mValidity[row] |= Validity.AscendingTied;								//	force 'ascending tied' validation
						}
						else																		//	otherwise, if not greater than, must be descending
						{
							mValidity[row] |= Validity.DescendingTied;								//	force 'descending tied' validation
						}
					}
					for ( i = 0; i < numValidBoxes; ++i )									//	traverse through each (visible) box in the unpack buffer
					{
						textBox = (TextBox)textBoxes[i];
						if (( i == 0 ) || ( i == ( numValidBoxes - 1 )) || (( i % 2) == 0 ))
						{
							if ( !isFullValidityBounds )
							{
								textBox.ReadOnly = true;									//	only disable the key/alternate boxes if its a 'partial bounds' component
							}
						}
						else
						{
							textBox.TextChanged += new EventHandler( _BoundaryPairRefresh );
						}
					}				
				}
				mDisplayLabels.Add( displayLbl );										//	add null/valid item in order of rows registered
			}
			return ( arrayListArray );
		}

		void EnableBoxMouseWheel(object sender, MouseEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			
			if ( e.Delta > 0 )
			{
				if ( textBox.ReadOnly )
				{
					DialogResult result = MessageBox.Show("Would you like to activate this control in order to edit the value?\n\nDo NOT do this unless you fully understand valid ranges of this control sequence.", "Please Confirm", MessageBoxButtons.YesNo );
					if ( result == DialogResult.Yes )
					{
						((TextBox)sender).ReadOnly = false;
					}			
				}
			}
			else
			{
				if ( !textBox.ReadOnly )
				{
					textBox.ReadOnly = true;
				}
			
			}
		}

		bool _IsBoundaryGroupAscending( int segmentIndex )
		{
			int storageIndex = 0;
			int firstVal = GetShortValue( (TextBox)mLists[segmentIndex][storageIndex++] );		//	get 'first' value in segment...
			int secondVal = GetShortValue( (TextBox)mLists[segmentIndex][storageIndex++] );		//	get 'second' value in segment...
			while ( firstVal == secondVal )
			{
				firstVal = GetShortValue( (TextBox)mLists[segmentIndex][storageIndex++] );		//	continue to traverse segment until we find NON matching values
				secondVal = GetShortValue((TextBox) mLists[segmentIndex][storageIndex++] );		//	ditto
			}
			bool isAscendingPairs = ( secondVal > firstVal );							//	is 'second' val greater than 'first' val?, if so, 'ascending'
			return ( isAscendingPairs );
		}
		
		void _BoundaryPairRefresh( object sender, EventArgs e )
		{
			TextBox	changedBox = (TextBox)sender;										//	convert the sender to a 'TextBox'
			Point	rowAndAcross = (Point)changedBox.Tag;								//	get the 'y' and 'x' of which box was updated
			short	boundsVal = GetShortValue( changedBox );							//	get (incremented) value that was typed in the visible/enabled box
			int     segmentIndex = rowAndAcross.Y;
			int		storageIndex = rowAndAcross.X;
			
			TextBox nextBox = (TextBox)mLists[segmentIndex][( storageIndex + 1) ];		//	get adjacent box

			if ( ( ( mValidity[segmentIndex] & Validity.Ascending ) != 0 ) ||
			     ( ( mValidity[segmentIndex] & Validity.AscendingTied ) != 0 ) )
			{
				nextBox.Text = ( boundsVal + 1 ).ToString();							//	set the next adjacent value to be HIGHER then...
			}
			else
			{
				nextBox.Text = ( boundsVal - 1 ).ToString();							//	set the next adjacent value to be LOWER then...
			}
		}

		private void _SegmentIndexDirtyRefresh( object sender, EventArgs e )
		{
			Point	rowAndAcross = (Point)((Control)sender).Tag;						//	get the 'y' and 'x' of which box was updated
			int		segmentIndex = rowAndAcross.Y;										//	get row...
			int		whichBoxIndex = rowAndAcross.X;										//	box index within the row...
			if ( whichBoxIndex < 64 )
			{
				mRowValModBits[ segmentIndex ] |= ((ulong)1 << whichBoxIndex );			//	track which items within the row are dirty/changed
			}
			mUtilMgr.ComponentDirtyNotification();										//	tell our creator we are 'dirty'
		}
		
		private void _TextChangedDisplayLabelRefresh( object sender, EventArgs e )
		{
			TextBox changedBox = (TextBox)sender;										//	convert the sender to a 'TextBox'
			Point rowAndAcross = (Point)changedBox.Tag;									//	get the 'y' and 'x' of which box was updated
			short		sumVal = 0;
			int		controlRowIndex = rowAndAcross.Y;									//	get row/index of control group
			foreach ( TextBox textBox in mLists[controlRowIndex] )
			{
				sumVal += GetShortValue(textBox);										//	traverse each item in the visible list, extracting out it's data
			}
			Label displayLabel = (Label)mDisplayLabels[controlRowIndex];				//	extract item from array
			int wantedSum = mSpecificSums[controlRowIndex];								//	get the sum we are expecting to compare against
			displayLabel.Text = sumVal.ToString();										//	set the string...
			if ( wantedSum == 100 )														//	was this a 'SUM100/%" validity?
			{
				displayLabel.Text += "%";												//	if so, add the '%' sign afterwards
			}
			if (( sumVal == wantedSum ) || ( wantedSum == -1 ))							//	if no sum is supplied, print in black...
			{
				displayLabel.ForeColor = Color.Black;
			}
			else
			{
				displayLabel.ForeColor = Color.Red;
			}
		}
		
		static public int GetNumUniqueEntries( string text )
		{
			int entries = 0;
			if ( text.IndexOf("\"") == -1  )											//	if no quotes in string, use special 'int array' support
			{
				entries =  AsShortArray( text, new ArrayList() ).Length;				//	call function, return only size of array
			}
			return ( entries );
		}
		
		static public short[]	AsShortArray( string text, ArrayList enabledTracking )
		{
			int charIndex = -1;
			int startIndex = -1;
			int charVal = 0;
			int stringLength = text.Length;												//	get size of string...
			bool insideString = false;													//	by default, not inside string at start...
			bool insideSum = false;
			ArrayList strings = new ArrayList();										//	storage for string index items...
			
			while ( ++charIndex < stringLength )										//	traverse through entire string...
			{
				charVal = text[charIndex];												//	extract item from string
				if ((( charVal >= '0' ) && ( charVal <= '9' ) )	|| ( charVal == '-'))	//	did we find an ascii digit?
				{
					if ( !insideString )												//	if we are not currently 'in a string'...
					{
						startIndex = charIndex;											//	track the starting location of this 'string'
						insideString = true;											//	then flag that we are...
					}
				}
				else if ( ( charVal == ' ' ) ||											//	space?
					( charVal == '\t' )||												//	tab?
					( charVal == '\n' )||												//	carriage return?
					( charVal == '*'  ) )												//	asterix (means disable box after activated)
				{
					if ( insideString )													//	are we flagged as being 'in a string'?
					{
						insideString = false;											//	then turn it off...
						if ( !insideSum )
						{
							strings.Add( text.Substring( startIndex, charIndex - startIndex ) );
							enabledTracking.Add( ( charVal != '*'  ) );					//	each time we end a number, track whether we requested it be disabled or not	
						}
						else
						{
							insideSum = false;											//	turn this off now...
						}
					}
				}
				else if (( charVal == 'S' ) || ( charVal == 's' ))
				{
					insideSum = true;													//	we are traversing a sum now, don't add to list of strings...
				}
			}
			if ( insideString )															//	are we flagged as being 'in a string'?
			{
				if ( !insideSum )
				{
					strings.Add( text.Substring( startIndex, charIndex - startIndex ) );
					enabledTracking.Add( ( charVal != '*'  ) );								//	each time we end a number, track whether we requested it be disabled or not	
				}
			}
			short[] shortArray = new short[strings.Count];								//	get # of unique strings in the array list, make array...
			int numEntries = 0;
			foreach ( string valStr in strings )
			{
				shortArray[numEntries++] = short.Parse( valStr );						//	parse each string into an int and store in 'short' array
			}
			
			return ( shortArray );
		}

		public void	 TransferModifiedValues( ResourceKernel resourceKernel )
		{
			int			i, listSize, cappedList;
			ulong		bitVal, rowBits;
			ArrayList	controlList;
			bool		didTransfer = false;							//	assume no transfers by default...
			
			for ( int row = 0; row < mNumSegments; ++row )				//	traverse each control list in object
			{
				rowBits = mRowValModBits[row];							//	get row changed bits flags...
				if ( rowBits != 0 )										//	check to see if row/controls have been modified
				{
					didTransfer = true;									//	flag that at least one component was dirty
					bitVal = 1;											//	set bit field to 1 in anticipation of masking
					controlList = mLists[row];							//	get control array for this particular array
					cappedList = listSize = controlList.Count;			//	size of this particular set of boxes/controls
					if ( listSize > 64 )								//	is there more than 64 controls?
					{
						cappedList = 64;								//	cap it at 64, since thats all the bits we've found
					}
					if ( mControlTypes[row] == ControlType.TextBox )
					{
						for ( i = 0; i < cappedList; ++i, bitVal += bitVal )//	loop through all available bits...
						{
							if (( rowBits & bitVal ) != 0 )				//	did this box/control get changed?
							{
								resourceKernel.UtilSendUpdatedValue(mObjectDesc, row, i, GetShortValue((TextBox)controlList[i]));
							}
						}
						for ( ; i < listSize; ++i )				//	if more than 64 items, just automatically copy over the rest...
						{
							resourceKernel.UtilSendUpdatedValue(mObjectDesc, row, i, GetShortValue((TextBox)controlList[i]));
						}
					}
					else if ( mControlTypes[row] == ControlType.ComboBox )
					{
						for ( i = 0; i < cappedList; ++i, bitVal += bitVal )//	loop through all available bits...
						{
							if (( rowBits & bitVal ) != 0 )					//	did this box/control get changed?
							{
								resourceKernel.UtilSendUpdatedValue(mObjectDesc, row, i, (short)((ComboBox)controlList[i]).SelectedIndex );
							}
						}
						for ( ; i < listSize; ++i )				//	if more than 32 items, just automatically copy over the rest...
						{
							resourceKernel.UtilSendUpdatedValue(mObjectDesc, row, i, (short)((ComboBox)controlList[i]).SelectedIndex );
						}
					}
					mRowValModBits[row] = 0;								//	once we've transferred the data, set all flags back to off/zero
				}
			}
			if ( didTransfer )
			{
				mUtilMgr.ComponentDirtyNotification();							//	tell our creator we are 'clean'
			}
		}

		static public short GetShortValue( object textBox )
		{
			return ( GetShortValue((TextBox)textBox ) );
		}
				
		static public short GetShortValue( TextBox textBox )
		{
			string text = textBox.Text;

			short parsedShort = 0;
			bool validNumber = true;	//	set to true by default
			bool hadProblem = false;

			do
			{
				if (text.Length == 0)
				{
					hadProblem = true;
					parsedShort = 0;
					break;
				}
				validNumber = true;		//	re-init to 'true' upon each loop attempt

				try
				{
					parsedShort = (short)int.Parse(text);
				}

				catch
				{
					validNumber = false;
					int length = text.Length;

					for (int i = 0; i < text.Length; ++i)
					{
						if ((text[i] < '0') || (text[i] > '9'))
						{
							text = text.Remove(i, 1);
						}
					}
					hadProblem = true;
				}
			} while (!validNumber);

			if (hadProblem)
				textBox.Text = string.Format("{0}", parsedShort);

			return ( parsedShort );
		}
		

		private bool _VerifyNumericValidity( ref string errorMsg, short[] vals, Validity validityFlags, int row )
		{
			bool		allowTies, ascendingPairs, isValid = true;						//	assume valid by default
			int			i, validityLoop = (int)Validity.Ascending;						//	start with the first valid validity...
			int			validityBits = validityLoop;									//	set bit field equivalent
			Validity	validityEnum;
			string		errorFormat;
			short		sum;
			
			bool		isValuePairing = _IsValuePairGrouping( validityFlags );			//	check validity for incoming group to see if a 'ValuePair' association

			for ( ; validityLoop < (int)Validity._size; ++validityLoop, validityBits += validityBits )				//	traverse through the validity values, some may be 'or'd together
			{
				if (( (int)validityFlags & validityBits ) != 0 )						//	is this enum activated?
				{
					validityEnum = (Validity)validityBits;								//	convert loop value to enum for switch statement
					errorFormat = string.Format( "Invalid number formatting, set to '{0}',", validityEnum.ToString().ToUpper());	//	initial default error message...
					
					if (( validityEnum == Validity.Ascending) || ( validityEnum == Validity.AscendingTied ))						
					{
						allowTies = ( validityEnum == Validity.AscendingTied );		//	special bool for checking whether we are allowing 'ties' or not
						if ( allowTies )
						{
							errorFormat += "  and values drop.";
						}
						else
						{
							errorFormat += "  and values drop (or stay the same).";
						}
						isValid = _VerifyNumberProgression(vals, allowTies, +1, +1, isValuePairing, false, ref errorMsg, errorFormat );
					}
					else if (( validityEnum == Validity.Descending ) || ( validityEnum == Validity.DescendingTied ))
					{
						allowTies = ( validityEnum == Validity.DescendingTied );		//	special bool for checking whether we are allowing 'ties' or not
						if ( allowTies )
						{
							errorFormat += "  and values rise.";
						}
						else
						{
							errorFormat += "  and values rise (or stay the same).";
						}
						isValid = _VerifyNumberProgression(vals, allowTies, -1, -1, isValuePairing, false, ref errorMsg, errorFormat );
					}
					else if (( validityEnum == Validity.FallAndRise ) || ( validityEnum == Validity.FallAndRiseTied ))
					{
						errorFormat += "  and values do not conform.";
						isValid = _VerifyNumberProgression( vals, ( validityEnum == Validity.FallAndRiseTied ), -1, +1, isValuePairing, false, ref errorMsg, errorFormat );
					}
					else if (( validityEnum == Validity.RiseAndFall ) || ( validityEnum == Validity.RiseAndFallTied ))
					{
						errorFormat += "  and values do not conform.";
						isValid = _VerifyNumberProgression( vals, ( validityEnum == Validity.RiseAndFallTied ), +1, -1, isValuePairing, false, ref errorMsg, errorFormat );
					}
					else if ( validityEnum == Validity.ValuePairNoOrder )
					{
						isValid = true;		//	'value pair no order' has no rules, other than 'must be within range' which is standard for all groups
					}
					else if (( validityEnum == Validity.ValuePairAscend) || ( validityEnum == Validity.ValuePairAscendTied ))						
					{
						allowTies = ( validityEnum == Validity.ValuePairAscendTied );		//	special bool for checking whether we are allowing 'ties' or not
						if ( allowTies )
						{
							errorFormat += "  and values drop.";
						}
						else
						{
							errorFormat += "  and values drop (or stay the same).";
						}
						isValid = _VerifyNumberProgression(vals, allowTies, +1, +1, false, true, ref errorMsg, errorFormat );
					}
					else if (( validityEnum == Validity.ValuePairDescend ) || ( validityEnum == Validity.ValuePairDescendTied ))
					{
						allowTies = ( validityEnum == Validity.ValuePairDescendTied );		//	special bool for checking whether we are allowing 'ties' or not
						if ( allowTies )
						{
							errorFormat += "  and values rise.";
						}
						else
						{
							errorFormat += "  and values rise (or stay the same).";
						}
						isValid = _VerifyNumberProgression(vals, allowTies, -1, -1, false, true, ref errorMsg, errorFormat );
					}
					else if (( validityEnum == Validity.Sum100 ) || ( validityEnum == Validity.SumSuppliedVal ))
					{
						sum = 0;
						foreach ( short val in vals )
						{
							sum += val;
						}
						if ( sum != mSpecificSums[row] )
						{
							isValid = false;
							errorMsg += string.Format("{1} and values do not add up to {0}!", mSpecificSums[row], errorFormat );
						}
					}
					else if (( validityEnum == Validity.Boundaries ) || ( validityEnum == Validity.BoundariesPartial ))
					{
						ascendingPairs = _IsBoundaryGroupAscending( row );				//	is this an 'ascending' boundary grouping?
						for ( i = 1; i < ( vals.Length - 1 ); i += 2 )
						{
							if ( ascendingPairs )										//	check if we want ascending validation?
							{
								if ( vals[i] != ( vals[i + 1] - 1 ) )
								{
									errorMsg += string.Format("{0} and pair groupings do not ascend properly!", errorFormat );
									isValid = false;
									break;
								}
							}
							else														//	otherwise this is a descending pair grouping
							{
								if ( vals[i] != ( vals[i + 1] + 1 ) )
								{
									errorMsg += string.Format("{0} and pair groupings do not descend properly!", errorFormat );
									isValid = false;
									break;
								}
							}
						}
					}
					if ( !isValid )														//	did we find some kind of failure criteria?
					{
						break;															//	stop the validity enforcement loop
					}
				}
			}
			return ( isValid );
		}

		private bool _VerifyNumberProgression( short[] vals, bool allowTies, int startMod, int endMod, bool isValuePairing, bool checkPairOrder, ref string errorMsg, string explanation )
		{
			int 	loopCount = vals.Length;											//	get # of items to traverse...
			int		diff, currVal, prevVal = vals[0];									//	get first value out of array...
			bool	expectingPeak = ( startMod != endMod );								//	if these two don't match, then we are expecting (or allow for )a direction change...
			bool	foundPeak = false;													//	track that we didn't find a direction change yet
			bool	isValid = true;														//	assume #s are valid by default...
			int		increaseVal = 1;													//	by default, assume we are processing each entry in the array
			if ( isValuePairing )														//	are we doing value pairs?
			{
				increaseVal = 2;														//	process every 2nd item in the list instead...
			}
			int		i = increaseVal;													//	set loop val to pairing (or not) related entry
			for ( ; i < loopCount; i += increaseVal )									//	traverse through the array of values...
			{
				currVal = vals[i];														//	get next value out of array...
				if ( ( checkPairOrder ) && (( i % 2 ) == 0 ))							//	if we are doing internal pair ordering ONLY, and we are in 'byte 0' of 2, then ignore first byte
				{
					prevVal = currVal;													//	'current' becomes 'previous' for next loop execution
					continue;
				}
				diff = currVal - prevVal;												//	use 'current value' minus 'prev value' to get 'difference'
				if (( diff == 0 ) && ( !allowTies ))									//	did we find a difference of zero, and were told to expect NO tied values?
				{
					isValid = false;													//	stop, tied values are not valid...
				}
				else if ( !foundPeak )													//	are we in the 'startMod' range of the values?
				{
					if ((( diff >= +1 ) && ( startMod == -1 )) ||						//	found increase and expected decrease??? OR...
						(( diff <= -1 ) && ( startMod == +1 )))							//	found decrease and expecting increase
					{
						if (( !expectingPeak ) || (( expectingPeak ) && ( foundPeak )))	//	if not expecting change, or already found one, we are invalid...
						{
							isValid = false;											//	no longer valid...
						}
						else if (( expectingPeak ) && ( !foundPeak ))					//	if we are expecting a change, and now have it, set 'foundPeak'
						{
							foundPeak = true;											//	track that direction change was found now...
						}
					}
				}			
				else																	//	otherwise we hit the 'direction change', use 'endMod'
				{
					if ((( diff >= +1 ) && ( endMod == -1 )) ||							//	found increase and expected decrease??? OR...
						(( diff <= -1 ) && ( endMod == +1 )))							//	found decrease and expecting increase
					{
						isValid = false;												//	no longer valid...
					}
				}
				if ( !isValid )															//	did we encounter some kind of problem?
				{
					errorMsg = explanation;												//	copy over explanation of what is wrong...
					break;																//	then stop the loop if so, no need to search further...
				}
				else
				{
					prevVal = currVal;													//	'current' becomes 'previous' for next loop execution
				}
			}
			return ( isValid );
		}	
	}

////////////////////////////////////////////////////////////////////////////////////////
//		
//	This is a support class that does 'bar graph' displays based on different inputs.
//	The NHL RookieGen (Mock Draft) is the best example of usage of this class/system.
//			
////////////////////////////////////////////////////////////////////////////////////////

	public class GraphicArray
	{
		private	ComboBox		mCombo;
		private Panel			mPanel;
		private int				mTall;
		private int				mWide;
		private int				mExtraWide;
		private int				mMin;
		private int				mMax;
		private bool			mWantText;
		private bool			mScaleToLargest;
		private int[][]			mArray;
		private	ArraySliders	mArraySliders;
		private EventHandler	mDisplayCallBack = null;
		
		public GraphicArray( ComboBox combo, Panel panel, int rowsTall, int minVal, int maxVal, bool wantText, bool scaleToLargest, ArraySliders sliders )
		{
			_InitData( combo, panel, ( rowsTall + 1 ), minVal, maxVal, wantText, scaleToLargest, sliders );
		}

		public GraphicArray( ComboBox combo, Panel panel, int rowsTall, int minVal, int maxVal, bool wantText, bool scaleToLargest )
		{
			_InitData( combo, panel, ( rowsTall + 1 ), minVal, maxVal, wantText, scaleToLargest, null );
		}

		public GraphicArray( Panel panel, int minVal, int maxVal, bool wantText, bool scaleToLargest, ArraySliders sliders )
		{
			_InitData( null, panel, 1, minVal, maxVal, wantText, scaleToLargest, sliders );
		}
		public GraphicArray( Panel panel, int minVal, int maxVal, bool wantText, bool scaleToLargest )
		{
			_InitData( null, panel, 1, minVal, maxVal, wantText, scaleToLargest, null );
		}
		
		public	void	RequestDisplayCallBack( EventHandler functionName )
		{
			if ( functionName != null )
			{
				mDisplayCallBack = functionName;
			}
		}

		private void	_InitData( ComboBox combo, Panel panel, int rowsTall, int minVal, int maxVal, bool wantText, bool scaleToLargest, ArraySliders slider )
		{
			mCombo = combo;
			mPanel = panel;
			mTall = rowsTall;
			mMin = minVal;
			mMax = maxVal;
			mWide = ( mMax - mMin ) + 1;			//	add one to encompass entire range between min/max
			mExtraWide = mWide + 1;					//  add another to hold total hits in that array
			mWantText = wantText;
			mScaleToLargest = scaleToLargest;
			mArraySliders = slider;
			
			InitArray();							//	initialize internal array automatically

			if ( mCombo != null )
			{
				mCombo.SelectedIndexChanged += new EventHandler( ComboIndexChanged );
			}
			
			if ( mArraySliders != null )
			{
				mArraySliders.mSliders[0].ValueChanged += new EventHandler( ComboIndexChanged );
				mArraySliders.mSliders[1].ValueChanged += new EventHandler( ComboIndexChanged );
			}
		}
		
		public	void	InitArray()
		{
			mArray = Support.InitMultiDimensionArray( mTall, mExtraWide );									//	create a two dimensional array, 'mTall' deep
		}
		
		public	int[]	GetArrayByRow( int wantedRow )
		{
			return ( mArray[wantedRow] );
		}
		
		private void	_CapRowAndVal( ref int whichRow, ref int valToAdd )
		{
			if ( whichRow < 0 )
			{
				whichRow = 0;
			}
			else if ( whichRow >= mTall )
			{
				whichRow = ( mTall - 1 );
			}

			if ( valToAdd < mMin )
			{
				valToAdd = mMin;
			}
			else if ( valToAdd > mMax )
			{
				valToAdd = mMax;
			}
		}
		
		private void    _AddValueHit( int whichRow, int zeroBasedVal )
		{
			++mArray[whichRow][zeroBasedVal];							//	add a hit to the specific entry...
			++mArray[whichRow][mWide];									//	add a hit to the total # of hits per this 'row', allows us to build a '%' total for each hit
		}

		public	void	AddValueHits( int whichRow, int valToAdd, int numTimesToAdd, bool capToRange )
		{
			while ( numTimesToAdd-- > 0 )
			{
				AddValueHit( whichRow, valToAdd, capToRange );
			}
		}

		public	void	AddValueHit( int whichRow, int valToAdd, bool capToRange )
		{
			if ( capToRange )
			{
				_CapRowAndVal( ref whichRow, ref valToAdd );
			}
			else
			{
Debug.Assert( ( whichRow >= 0 ) && ( whichRow < mTall ) );				//	assert on valid array 'deep'
Debug.Assert( ( valToAdd >= mMin ) && ( valToAdd <= mMax ) );			//	assert on valid array 'accross'			
			}
			valToAdd -= mMin;											//	zero base the value we were supplied...
			_AddValueHit( whichRow, valToAdd );							//	track a 'hit' to a certain row/across segment
			_AddValueHit( mTall - 1, valToAdd );						//	track a 'hit' to the 'entire list' summary array
		}

		public	void	AddValueHit( int whichRow, int valToAdd )
		{
			AddValueHit( whichRow, valToAdd, false );					//	call function but assert if 'valToAdd' outside of range
		}

		public	void	AddSingleArrayHit( int valToAdd )
		{
			AddSingleArrayHit( valToAdd, false );
		}

		public	void	AddSingleArrayHits( int valToAdd, int timesToAdd )
		{
			while ( timesToAdd-- > 0 )
			{
				AddSingleArrayHit( valToAdd, false );
			}
		}


		public	void	AddSingleArrayHit( int valToAdd, bool capToRange )
		{
Debug.Assert( ( mCombo == null ), "Use 'AddValueHit' if ComboBox is attached!" );
			int whichRow = 0;											//	always write to row zero...
			if ( capToRange )
			{
				_CapRowAndVal( ref whichRow, ref valToAdd );
			}
			else
			{
Debug.Assert( ( valToAdd >= mMin ) && ( valToAdd <= mMax ) );			//	assert on valid array 'accross'			
			}
			valToAdd -= mMin;											//	zero base the value we were supplied...
			_AddValueHit( whichRow, valToAdd );							//	track a 'hit' to a certain row/across segment
		}

		private void ComboIndexChanged(object sender, EventArgs e)
		{
			DisplayRefresh();
		}

		static public	Bitmap ArrayDisplay( int[] array, Panel panel, bool arrayHasExtraSumAppended, bool wantText, bool scaleToLargest  )
		{
			return ( ArrayDisplay( array, panel, arrayHasExtraSumAppended, wantText, scaleToLargest, null ) );
		}
		
		public class ArraySliders
		{
			public	TrackBar[]	mSliders;
			public	Point		mDisplayRange;
			
			public	ArraySliders( TrackBar[] sliders, Point dispRange )
			{
				mSliders = sliders;
				mDisplayRange = dispRange;
			}
		}
		
		static public	Bitmap ArrayDisplay( int[] array, Panel panel, bool arrayHasExtraSumAppended, bool wantText, bool scaleToLargest, ArraySliders arraySliders )
		{
			int fontHeight = (int)panel.Font.GetHeight();
			int widthPixels = panel.Width;								//	get # of pixels across for bmap/display
			int heightPixels = panel.Height;							//	get # of pixels down for bmap/display
			Bitmap bmap = new Bitmap( widthPixels, heightPixels );		//	create a bmap big enough to draw panel
			Graphics gfx = Graphics.FromImage( bmap );					//	init a graphics class based on the bmap handle
			gfx.Clear( Color.Black );									//	clear background to receive gfx

			Font valFont = new Font("Sylfaen", 9.0f );
			Font smallFont = new Font("Sylfaen", 7.0f );
			
			Pen drawPen = new Pen(panel.BackColor);
			Brush textBrush = new SolidBrush(panel.ForeColor);

			float yCoord = 2.0f;										//	offset to start drawing format
			heightPixels -= ( (int)yCoord * 2 );						//	subtract a bit of space (equivalent of 2 from top & bottom)

			int endVal = array.Length;
			int startVal = 0;
			int topVal = 0, botVal;
			bool haveSliderParam = ( arraySliders != null );
			bool wantDescending = false;
			int  firstDispVar = 0;
			int  lastDispVar = 0;
			if ( haveSliderParam )
			{
				int minSliderVal = arraySliders.mSliders[0].Value;
				int maxSliderVal = arraySliders.mSliders[1].Value;
				
				wantDescending = ( arraySliders.mDisplayRange.X > arraySliders.mDisplayRange.Y );
				if ( wantDescending )
				{
					startVal = ( arraySliders.mSliders[1].Maximum - maxSliderVal );
					endVal = ( ( arraySliders.mSliders[0].Maximum - minSliderVal ) + 1 );	//	add one since we want an inclusion of the final rating

					firstDispVar = topVal = maxSliderVal;
					lastDispVar = botVal = minSliderVal;
				}
				else
				{
					startVal = ( minSliderVal - arraySliders.mSliders[0].Minimum );
					endVal = ( ( maxSliderVal - arraySliders.mSliders[1].Minimum ) + 1 );	//	add one since we want an inclusion of the final rating

					firstDispVar = topVal = minSliderVal;
					lastDispVar = botVal = maxSliderVal;
				}

				gfx.DrawString( topVal.ToString(), valFont, Brushes.White, ( widthPixels - 20 ), 0 );
				gfx.DrawString( botVal.ToString(), valFont, Brushes.White, ( widthPixels - 20 ), ( heightPixels - 10 ) );

				if ( wantDescending )
				{
					botVal = ( topVal - endVal );
					topVal += startVal;
				}
				
				arrayHasExtraSumAppended = false;						//	ignore this bool if we have sliders, since we are extracting from a sub-set of vals
			}

			int i = 0, sampleSize = 0;									//	assume some hits by default
			if ( arrayHasExtraSumAppended )
			{
				--endVal;												//	if passed with extra 'sum' padding, then remove that from processing
				sampleSize = array[endVal];								//	get total # of hits for that row...
			}
			else
			{
				for ( i = startVal; i < endVal; ++i )
				{
					sampleSize += array[i];
				}
			}
			widthPixels -= 4;											//	subtract a bit to allow for full width display bar
			if ( panel.BorderStyle == BorderStyle.Fixed3D )
			{
				widthPixels -= 4;										//	subtract a bit more to take into account the '3d' lines around the edge
			}
			float hitPixels = (float)heightPixels / (float)( endVal - startVal );	//	find out how many pixels we we need for each 'hit' display
Debug.Assert( hitPixels >= 1 , "Insufficient Panel height to display storage!" );
			int textOffset = (( ( (int)hitPixels - 2 ) - fontHeight ) / 2 );
			int val, width, pct, highestVal = 1;
			if ( scaleToLargest )										//	only do this step if we want everything scaled to the largest single hit entry
			{
				for ( i = startVal; i < endVal; ++i )
				{
					val = array[i];										//	grab val out of array, we'll use it more than once
					if ( val > highestVal )
					{
						highestVal = val;								//	track the highest value...
					}
				}
			}
			if ( sampleSize == 0 )
			{
				sampleSize = 1;											//	protect against divide by zero, if no hits in this array
			}

			if (( wantText ) && ( haveSliderParam ))					//	if text requested by default, and sliders activated...
			{
				wantText = ( hitPixels > panel.Font.Height );			//	turn off text if not enough pixels to display
			}
			int ratingDispCounter = 0;
			bool doVarDisp;
			bool flipVertically = ( panel.RightToLeft == RightToLeft.Yes );
			int  j = ( endVal - 1 );
			for ( i = startVal; i < endVal; i++, yCoord += hitPixels )
			{
				if ( flipVertically )
				{
					val = array[j--];
				}
				else
				{
					val =  array[i];									//	grab val...
				}
				if ( !scaleToLargest )
				{
					pct = ( ( val * 100 ) / sampleSize );				//	we want percentage of entire sample display?	
				}
				else
				{
					pct = ( ( val * 100 ) / highestVal );				//	we want % of highest value in the sample?
				}
				width = ( ( widthPixels * pct ) / 100 );
				if ( hitPixels >= 4 )
				{
					gfx.DrawRectangle( drawPen, 0, yCoord, width, hitPixels - 3 );
				}
				else
				{
					gfx.DrawLine( drawPen, 0, yCoord, width, yCoord );
				}
				if ( ( wantText ) && ( val != 0 ) )
				{
					if ( !scaleToLargest )
					{
						val = pct;
					}
					gfx.DrawString( val.ToString(), panel.Font, textBrush, 0, yCoord + textOffset );
				}
				if ( haveSliderParam )
				{
					++ratingDispCounter;
					if ( ( ratingDispCounter % 5 ) == 0 )
					{
						if ( wantDescending )
						{
							val = firstDispVar - ratingDispCounter;
							doVarDisp = ( val > lastDispVar );
						}
						else
						{
							val = firstDispVar + ratingDispCounter;			
							doVarDisp = ( val < lastDispVar );	
						}
						if ( doVarDisp )
						{
							gfx.DrawString( val.ToString(), smallFont, Brushes.LightGray, ( widthPixels - 14 ), yCoord + textOffset );	
						}
					}
				}
			}
			if ((( !wantText ) && ( scaleToLargest ) ) ||
				( haveSliderParam ) )
			{
				gfx.DrawString(string.Format("max hits : {0}", highestVal.ToString()), smallFont, Brushes.Cyan, 12, heightPixels - 10 );
			}
			panel.BackgroundImage = bmap;
			
			return ( bmap );
		}
		
		public	Bitmap	DisplayRefresh()
		{
			int wantedRow = 0;											//	assume no combo box by default, single array only...
			if ( mCombo != null )
			{
				wantedRow =	mCombo.SelectedIndex;						//	get row of array we want to print
			}
			Bitmap bmap = ArrayDisplay( mArray[wantedRow], mPanel, true, mWantText, mScaleToLargest, mArraySliders );
			if ( mDisplayCallBack != null )
			{
				mPanel.Tag = new Bitmap( bmap );
				mDisplayCallBack( bmap, null );
			}
			return ( bmap );
		}
	}	
	
	public	class	ControlMasks
	{
		private class MaskCtrl
		{
			public	TrackBar		mSlider;
			public	Label			mLbl0, mLbl1;
			public	int				mSizeVal;
			
			public MaskCtrl( TrackBar slider, Label lbl0, Label lbl1, int val, bool vertMask )
			{
				mSlider = slider;
				mLbl0 = lbl0;
				mLbl1 = lbl1;
				mSizeVal = val;
				
				mLbl0.BringToFront();
				mLbl1.BringToFront();
				
				if ( vertMask )
				{
					mLbl1.Width = mLbl0.Width;
					mSlider.ValueChanged += new EventHandler( VertMaskRequest );

					VertMaskRequest( null, null );
				}
				else
				{
					mLbl1.Height = mLbl0.Height;
					mSlider.ValueChanged += new EventHandler( HorzMaskRequest );

					HorzMaskRequest( null, null );	
				}
			}
			
			private bool	_PrepSliderVal( out int val )
			{
				val = mSlider.Value;
				
				if ( val-- == 0 )
				{
					mLbl0.Hide();
					mLbl1.Hide();
				}
				else
				{
					mLbl0.Show();
					mLbl1.Show();
				}
				return ( val >= 0 );
			}
			
			void VertMaskRequest(object sender, EventArgs e)
			{
				int sliderVal;
				if ( _PrepSliderVal( out sliderVal ) )
				{		
					mLbl0.Size = new Size( mLbl0.Size.Width, ( sliderVal++ * mSizeVal ) );
					mLbl1.Location = new Point( mLbl0.Location.X, ( mLbl0.Location.Y + ( sliderVal * mSizeVal ) ) );
					mLbl1.Size = new Size( mLbl1.Size.Width, (( mSlider.Maximum - sliderVal ) * mSizeVal ) );
				}
			}
			
			void HorzMaskRequest(object sender, EventArgs e)
			{
				int sliderVal;
				if ( _PrepSliderVal( out sliderVal ) )
				{		
					mLbl0.Size = new Size( ( sliderVal++ * mSizeVal ), mLbl0.Size.Height );
					mLbl1.Location = new Point( ( mLbl0.Location.X + ( sliderVal * mSizeVal ) ), mLbl0.Location.Y );
					mLbl1.Size = new Size( (( mSlider.Maximum - sliderVal ) * mSizeVal ), mLbl1.Size.Height );
				}
			}
		}

		public	ControlMasks( TrackBar vertMaskSlider, Label[] vertMaskLbl, TrackBar horzMaskSlider, Label[] horzMaskLbl )
		{
			new MaskCtrl( vertMaskSlider, vertMaskLbl[0], vertMaskLbl[1], vertMaskLbl[0].Size.Height, true );
			new MaskCtrl( horzMaskSlider, horzMaskLbl[0], horzMaskLbl[1], horzMaskLbl[0].Size.Width, false );
		}

		public	ControlMasks( TrackBar slider, Label[] labels )
		{
			if ( labels[0].Size.Width > labels[0].Size.Height )
			{
				new MaskCtrl( slider, labels[0], labels[1], labels[0].Size.Height, true );	//	label is wider than tall, indicating we want to block ROWS of horizontal data
			}
			else
			{
				new MaskCtrl( slider, labels[0], labels[1], labels[0].Size.Width, false );	//	label is taller than wide, indicating we want to block COLUMNS of vertical data
			}
		}

		
		

	}
}
