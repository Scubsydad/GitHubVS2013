using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

public class ComboBoxControlsToggle
{
	private	ComboBox	mComboBox;
	private	string		mDisableDesc;
	private ArrayList[]	mControls;
	private Color[]		mComboColors;
	
//BRK when we receive an array of controls, simply put them all in a single array list, it doesn't matter if they are different groupings...	
//	  we are not passing in any 'ComboControls' which means that the combo will NOT be colorized if its the 'n/a' entry or not...

	public	ComboBoxControlsToggle( ComboBox comboBox, string disableDesc, object[] toggleControls, bool trueForReadOnly )
	{
		_InternalInit( comboBox, disableDesc, _BuildArrayListArray( toggleControls ), trueForReadOnly, null );
	}

//BRK when we receive an array of controls, simply put them all in a single array list, it doesn't matter if they are different groupings...	
//	  we ARE passing in combo colors, pass them through...
	public	ComboBoxControlsToggle( ComboBox comboBox, string disableDesc, object[] toggleControls, bool trueForReadOnly , Color[] comboColors)
	{
		_InternalInit( comboBox, disableDesc, _BuildArrayListArray( toggleControls ), trueForReadOnly, comboColors  );
	}
	
//BRK when we receive a pre-existing array of array lits, just pass it through 'as is' to the private init	
//	  we are not passing in any 'ComboControls' which means that the combo will NOT be colorized if its the 'n/a' entry or not...
	public	ComboBoxControlsToggle( ComboBox comboBox, string disableDesc, ArrayList[] controls, bool trueForReadOnly )
	{
		_InternalInit( comboBox, disableDesc, controls, trueForReadOnly, null );
	}

//BRK when we receive a pre-existing array of array lits, just pass it through 'as is' to the private init	
//	  we ARE passing in combo colors, pass them through...
	public	ComboBoxControlsToggle( ComboBox comboBox, string disableDesc, ArrayList[] controls, bool trueForReadOnly , Color[] comboColors)
	{
		_InternalInit( comboBox, disableDesc, controls, trueForReadOnly, comboColors );
	}
		
	private ArrayList[]	_BuildArrayListArray( object[] toggleControls )
	{
		ArrayList controlArrayList = new ArrayList();		
		for ( int i = 0; i < toggleControls.Length; ++i )
		{
			controlArrayList.Add( toggleControls[i] );
		}	
		return ( new ArrayList[] { controlArrayList } );
	}
	
	private void	_InternalInit( ComboBox comboBox, string disableDesc, ArrayList[] controls, bool trueForReadOnly, Color[] comboColors )
	{
		mComboBox = comboBox;
		mDisableDesc = disableDesc;
		mControls = controls;
		mComboColors = comboColors;
		
		if ( trueForReadOnly )
		{
			comboBox.SelectedIndexChanged += new EventHandler( _ComboBoxReadOnlyCallBack );
			_ComboBoxReadOnlyCallBack( null, null );
		}
		else
		{
			comboBox.SelectedIndexChanged += new EventHandler( _ComboBoxEnableDisableCallBack );
			_ComboBoxEnableDisableCallBack( null, null );
		}
	}

	private void	_DoComboBoxColorCheck( Control control, bool isValidChoice )
	{
		if ( mComboColors != null )
		{
			int colorId = 0;											//	assume disabled by default
			if ( isValidChoice )
			{
				colorId = 1;											//	use 'combo valid' index...
			}
			control.BackColor = mComboColors[colorId];
		}
	}
	
	void _ComboBoxEnableDisableCallBack(object sender, EventArgs e)
	{
		bool controlsEnabled = ( mComboBox.Text != mDisableDesc );		//	see if the combo box text is NOT our 'n/a' type entry...
		foreach ( ArrayList arrayList in mControls )					//	process each of the array lists in the storage
		{
			foreach ( Control control in arrayList )					//	now process each individual control in a single array list
			{
				control.Enabled = controlsEnabled;						//	enable/disable the control based on the text combo
			}
		}
		_DoComboBoxColorCheck( mComboBox, controlsEnabled );
	}
	
	void _ComboBoxReadOnlyCallBack(object sender, EventArgs e)
	{
		bool wantReadOnly = ( mComboBox.Text == mDisableDesc );		//	see if the combo box text is NOT our 'n/a' type entry...
		foreach ( ArrayList arrayList in mControls )					//	process each of the array lists in the storage
		{
			foreach ( Control control in arrayList )					//	now process each individual control in a single array list
			{
				if ( control.GetType().ToString() == "System.Windows.Forms.TextBox" )
				{
					((TextBox)control).ReadOnly = wantReadOnly;			//	unfortunately, there doesn't seem to be a 'read only' for combo boxes :(
				}
				else
				{
					_DoComboBoxColorCheck( control, !wantReadOnly );	//	if a combo box entry, use the 'back color' to denote its not editable
				}
			}
		}
		_DoComboBoxColorCheck( mComboBox, !wantReadOnly );
	}	
}

class MathOps
{
	static public int   GetValueByRangePct( int minVal, int maxVal, int wantedPct )
	{
		if ( VerifyMinMax( ref minVal, ref maxVal ) )
		{
			wantedPct = ( 100 - wantedPct );		//	if we flipped the min/max, then take the opposite orientation of the 'wanted %' to get correct #
		}
Debug.Assert( ( wantedPct >= 0 ) && ( wantedPct <= 100 ), string.Format( "invalid scale num supplied (%d)!", wantedPct ) );
		int	diff = ( maxVal - minVal ) + 1;										//	get range between values...
		int diffRange = ( ( diff * wantedPct ) / 100 );							//	get value based on percentage of diff =
		int val = ( minVal + diffRange );										//	get value within pair range
		if ( val > maxVal )
		{
			val = maxVal;														//	cap it at the high range if we used '100' % of diff
		}
		return ( val );															//	return out pct on range to caller...
	}
		
	static public int	CapValWithinRange( int val, int minRange, int maxRange )
	{
		if ( val < minRange )
		{
			val = minRange;
		}
		else if ( val > maxRange )
		{
			val = maxRange;
		}
		return ( val );
	}

	static public	int		GetRangePct(int minVal, int maxVal, int value)
	{
		VerifyMinMax( ref minVal, ref maxVal );						//	ensure that the min val is greater than the max val
		value = CapValWithinRange( value, minVal, maxVal );				//	ensure that the val is within range
		int diff = ( maxVal - minVal );									//	get range between min/max ranges we intercepted
		if ( diff == 0 )
			diff = 1;
		int zbVal = ( value - minVal);									//	zero base the intercept value
		int pctRange = ( ( zbVal * 100 ) / diff );
		return ( pctRange );
	}

	static public bool   VerifyMinMax( ref int minVal, ref int maxVal )
	{
		bool madeChange = ( minVal > maxVal );
		if ( madeChange )			//	sometimes called via interpolation segments, which can't guarantee order of data, so double check
		{
			int temp = minVal;
			minVal = maxVal;
			maxVal = temp;
		}
		return ( madeChange );
	}
}

public class BackColorToggle
{
	private ArrayList	mBackColorStorage = new ArrayList();

	private class	 ControlBackColor
	{
		private Control		mControl;
		private Color		mOrigColor;
		
		public	ControlBackColor( Control control, Color backColor )
		{
			mControl = control;
			mOrigColor = mControl.BackColor;
			mControl.BackColor = backColor;
		}
		
		public void Dispose()
		{
			mControl.BackColor = mOrigColor;
		}
	}
	
	public	BackColorToggle( ArrayList controls, Color backColor )
	{
		foreach ( Control control in controls )
		{
			mBackColorStorage.Add( new ControlBackColor( control, backColor ) );
		}
	}

	public	BackColorToggle( ArrayList[] controls, Color backColor )
	{
		foreach ( ArrayList arrayList in controls )
		{
			foreach ( Control control in arrayList )
			{
				mBackColorStorage.Add( new ControlBackColor( control, backColor ) );
			}
		}
	}
	
	public void Dispose()
	{
		foreach ( ControlBackColor cbc in mBackColorStorage )
		{
			cbc.Dispose();
		}
	}
}

public class RegistrationBackup
{
	private	ArrayList[]				mControls;
	private string					mObjectDesc;
	private ArrayList				mControlsBackup = new ArrayList();
	private Settlers_of_Catan.UtilityMgr	mUtilMgr;
	
	public RegistrationBackup( Settlers_of_Catan.UtilityMgr utilMgr, string objectDesc, ArrayList[] controls )
	{
		mObjectDesc = objectDesc;
		mControls = controls;
		mUtilMgr = utilMgr;
		
		foreach ( ArrayList controlArray in mControls )
		{
			foreach ( Control control in controlArray )
			{
				control.MouseDoubleClick += new MouseEventHandler( _ControlMouseDoubleClick );
			}
		}
	}

	void _ControlMouseDoubleClick(object sender, MouseEventArgs e)
	{
		int numBackups = mControlsBackup.Count;
		if ( numBackups == 0 )
		{
			_CreateBackup();
		}
		else
		{
			DialogResult result = MessageBox.Show(string.Format( "Please confirm your wanted action based on this 'double click'...\n\nYes\t: Set current values as 'Backup Version {0}'\n\nNo\t: Do Nothing\n\nCancel\t: Restore 'Backup Version {1}' overtop of existing controls", ( numBackups + 1 ), numBackups ), "Please Confirm", MessageBoxButtons.YesNoCancel );
			if ( result == DialogResult.Yes )
			{
				_CreateBackup();
			}
			else if ( result == DialogResult.Cancel )
			{
				int restoreIndex = ( mControlsBackup.Count - 1 );
				((ControlsBackup)mControlsBackup[restoreIndex]).RestoreBackup();
				mControlsBackup.RemoveAt( restoreIndex );
			}
		}
	}	
	
	private void _CreateBackup( )
	{
		if ( mUtilMgr.ValidateAllData() )
		{
			mControlsBackup.Add( new ControlsBackup( mControls, mObjectDesc ) );
			BackColorToggle bct = new BackColorToggle( mControls, Color.Chartreuse );
			MessageBox.Show( string.Format( "The current values in all controls for registration '{0}' have now been 'Backed up'", mObjectDesc ), string.Format("Backup Version : {0}", mControlsBackup.Count ) );
			bct.Dispose();
		}
	}
}

public class ControlsBackup
{
	private	ArrayList[]		mControls;
	private int[][]			mOrigValues;
	private string			mDataDesc;
	private	int				mNumSegments;
	
	public	ControlsBackup( ArrayList[] controls )
	{
		_InternalInit( controls, "Data" );
	}
	

	public	ControlsBackup( ArrayList[] controls, string dataDescription )
	{
		_InternalInit( controls, mDataDesc );
	}
	
	private void		_InternalInit( ArrayList[] controls, string dataDescription )
	{
		mControls = controls;
		mDataDesc = dataDescription;
		mNumSegments = mControls.Length;
		CommitCurrentEdits();	
	}
	

	public	void		CommitCurrentEdits()
	{
		mOrigValues = GetControlVals();
	}
	
	public	int[][]		GetControlVals()
	{
		int		j, numControls;
		int[][] ctrlVals = new int[mNumSegments][];		//	storage int array to contain vals

		for ( int i = 0; i < mNumSegments; ++i )
		{
			numControls = mControls[i].Count;			//	number of controls in this specific segment/array list
			ctrlVals[i] = new int[numControls];			//	storage to track the vals
			if ( ((Control)mControls[i][0]).GetType().ToString() == "System.Windows.Forms.ComboBox" )
			{
				for ( j = 0; j < numControls; ++j )
				{
					ctrlVals[i][j] = ((ComboBox)mControls[i][j]).SelectedIndex;
				}					
			}
			else
			{
				for ( j = 0; j < numControls; ++j )
				{
					ctrlVals[i][j] = Settlers_of_Catan.UtilData.GetShortValue(mControls[i][j]);
				}
			}
		}
		return ( ctrlVals );
	}

	public	Point		GetValueRange( int wantedSegment )
	{
		int segmentSize = mControls[wantedSegment].Count;
		int minVal = 10000;
		int maxVal = 0;
		int ctrlVal;
		for ( int i = 0; i < segmentSize; ++i )
		{
			ctrlVal = Settlers_of_Catan.UtilData.GetShortValue( mControls[wantedSegment][i] );
			if ( ctrlVal < minVal )
			{
				minVal = ctrlVal;
			}
			if ( ctrlVal > maxVal )
			{
				maxVal = ctrlVal;
			}
		}
		return ( new Point( maxVal, minVal ) );
	}
	
	public	void	RestoreBackup()
	{
		SetControlVals( mOrigValues );
	}
	
	public	void	SetControlVals( int[][] newVals )
	{
		int		j, numControls;

		for ( int i = 0; i < mNumSegments; ++i )
		{
			numControls = mControls[i].Count;			//	number of controls in this specific segment/array list

			if ( ((Control)mControls[i][0]).GetType().ToString() == "System.Windows.Forms.ComboBox" )
			{
				for ( j = 0; j < numControls; ++j )
				{
					((ComboBox)mControls[i][j]).SelectedIndex = mOrigValues[i][j];
				}					
			}
			else
			{
				for ( j = 0; j < numControls; ++j )
				{
					((TextBox)mControls[i][j]).Text = mOrigValues[i][j].ToString();
				}
			}
		}
	}
	
	public		bool			ConfirmCancelEdits()
	{
		bool	didCancel = false;
		if ( MessageBox.Show(string.Format( "Are you sure you want to lose any potential edits you made to this {0}?", mDataDesc ), "Please Confirm", MessageBoxButtons.YesNo ) == DialogResult.Yes )
		{
			didCancel = true;
			RestoreBackup();		//	reset any controls to the 'orig' values we created in the past
		}
		return ( didCancel );
	}
	
	public		DialogResult	VerifyAndConfirmAnyChanges( )
	{
		DialogResult result = DialogResult.Yes;
		
		bool	changesMade = false;					//	assume no changes by default...
		
		int[][]	currVals = GetControlVals( );			//	get values as they are right now...

		int		j, numIntsToCheck;

		for ( int i = 0; i < mNumSegments; ++i )
		{
			numIntsToCheck = mOrigValues[i].Length;		//	get size of each array horizontally
			for ( j = 0; j < numIntsToCheck; ++j )
			{
				if ( mOrigValues[i][j] != currVals[i][j] )//	compare 'original' value to 'current' to see if any change
				{
					changesMade = true;					//	stop when we find changes
					i = j = 100;						//	break both loops
				}
			}
		}
		if ( changesMade )
		{
			result = MessageBox.Show( String.Format( "You have made changes to the {0},\nbut have not saved them...\n\nDo you still wish to leave this {0}?\n\n'Yes'\t- Keep changes & go to other {0}.\n\n'No'\t- Throw away changes & go to other {0}.\n\n'Cancel'\t- Stay on this {0}.", mDataDesc ), "Please Confirm", MessageBoxButtons.YesNoCancel );
			if ( result == DialogResult.No )
			{
				RestoreBackup( );
			}
			else if ( result == DialogResult.Yes )
			{
				CommitCurrentEdits();
			}
		}
		return ( result );
	}
}

public class TabActivations
{
	private Size			mMinSize, mMaxSize;
	private TabControl		mTabControl;
	
	public	TabActivations( TabControl tabControl, Size minSize, Size maxSize, bool startAsMax )
	{
		mTabControl = tabControl;
		mMinSize = minSize;
		mMaxSize = maxSize;
		mTabControl.BringToFront();
		
		_AddControlCallBacks( mTabControl );

		foreach ( TabPage tabPage in mTabControl.TabPages )
		{
			_AddControlCallBacks( tabPage );
			foreach ( Control control in tabPage.Controls )
			{
				control.MouseEnter += new EventHandler( _WantMaxSize );
			}
		}
		if ( startAsMax )	{	_WantMaxSize( null, null );	}
		else				{	_WantMinSize( null, null );	}
	}
	
	private void _AddControlCallBacks( Control control )
	{
		control.MouseEnter += new EventHandler( _WantMaxSize );
		control.MouseLeave += new EventHandler( _WantMinSize );
	}
			
	private void _WantMaxSize(object sender, EventArgs e)
	{
		mTabControl.Size = mMaxSize;
	}

	private void _WantMinSize(object sender, EventArgs e)
	{
		mTabControl.Size = mMinSize;
	}
}

public class TrackBarAssociation
{
	private ArrayList		mTrackBars;
	private Label[]			mLabels;
	private string[]		mLabelDesc;
	
	public	TrackBarAssociation( TrackBar[] sliders, Label[] labels )
	{
		_InternalInit( sliders, labels, null );
	}

	public	TrackBarAssociation( TrackBar[] sliders, Label[] labels, string[] labelDesc )
	{
		_InternalInit( sliders, labels, labelDesc );
	}
	
	private void _InternalInit( TrackBar[] sliders, Label[] labels, string[] labelDesc )
	{
		mTrackBars = new ArrayList();
		foreach ( TrackBar trackBar in sliders )
		{
			mTrackBars.Add( trackBar );
		}
		mLabels = labels;
		mLabelDesc = labelDesc;
		
		foreach ( TrackBar trackBar in mTrackBars )
		{
			trackBar.ValueChanged += new EventHandler( _TrackBarValueChanged );
			_TrackBarValueChanged( trackBar, null );
		}
	}

	private void _TrackBarValueChanged( object sender, EventArgs e )
	{
		TrackBar	trackBar = (TrackBar)sender;									//	convert caller to TrackBar object
		int			trackBarIndex = mTrackBars.IndexOf( sender );					//	find out which track bar is calling in...
		Label		label = mLabels[trackBarIndex];									//	grab specific label associated with this slider
		int			sliderValue = trackBar.Value;									//	get value of slider...
		if ( mLabelDesc == null )													//	are there no associated strings with the label?
		{
			label.Text = sliderValue.ToString();									//	then display the numeric value of the slider
		}
		else
		{
			label.Text = mLabelDesc[sliderValue - trackBar.Minimum];				//	if strings, display associated string assigned to slider
		}	
	}
}