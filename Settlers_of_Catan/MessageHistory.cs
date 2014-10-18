using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Settlers_of_Catan
{
	public partial class MessageHistory : Form
	{
		private bool				mAllowRefresh;
		private CheckBox[]			mSideChecks;
		private	CheckBox[]			mMsgChecks;
		private TrackBar[]			mSecondsRange;
		private TrackBar[]			mMessageIdRange;
		private TrackBar[]			mUnitIdRange;
		private MessageDisplay		mMsgDisplay;
		private ArrayList			mMessages = new ArrayList();
		private	bool[]				mDidAssert;
		private int					mValidTasks;
		private PlayGameMgr			mPlayGameMgr;

		public MessageHistory( MessageCenter msgCenter, PlayGameMgr	playGameMgr )
		{
			InitializeComponent();

			mPlayGameMgr = playGameMgr;

			mSideChecks = new CheckBox[] { SideCheck0, SideCheck1, SideCheck2, SideCheck3, SideCheck4, SideCheck5 };
			mMsgChecks = new CheckBox[] { MsgCheck0, MsgCheck1, MsgCheck2, MsgCheck3, MsgCheck4, MsgCheck5, MsgCheck6, MsgCheck7, MsgCheck8, MsgCheck9, 
										  MsgCheck10, MsgCheck11, MsgCheck12, MsgCheck13, MsgCheck14, MsgCheck15, MsgCheck16, MsgCheck17, MsgCheck18, MsgCheck19, 
										  MsgCheck20, MsgCheck21, MsgCheck22, MsgCheck23, MsgCheck24, MsgCheck25, MsgCheck26, MsgCheck27, MsgCheck28, MsgCheck29,
										  MsgCheck30, MsgCheck31, MsgCheck32, MsgCheck33, MsgCheck34, MsgCheck35, MsgCheck36, MsgCheck37, MsgCheck38, MsgCheck39,
										  MsgCheck40, MsgCheck41, MsgCheck42, MsgCheck43, MsgCheck44, MsgCheck45, MsgCheck46, MsgCheck47, MsgCheck48, MsgCheck49 };


			mSecondsRange = new TrackBar[] { SecMinSlider, SecMaxSlider };
			mMessageIdRange = new TrackBar[] { MsgMinSlider, MsgMaxSlider };
			mUnitIdRange = new TrackBar[] { IdMinSlider, IdMaxSlider };

			new TrackBarAssociation( mSecondsRange, new Label[] { SecMinSliderLbl, SecMaxSliderLbl } );
			new TrackBarAssociation( mMessageIdRange, new Label[] { MsgMinSliderLbl, MsgMaxSliderLbl } );
			new TrackBarAssociation( mUnitIdRange, new Label[] { IdMinSliderLbl, IdMaxSliderLbl } );

			MessageType[] allTypes = msgCenter.GetAllMessageTypes();
			mMsgDisplay = new MessageDisplay( MessageOutput, MsgNumPanel, mSideChecks, mMsgChecks, mMessageIdRange , mSecondsRange, mUnitIdRange);

			mValidTasks = allTypes.Length;
			mDidAssert = new bool[mValidTasks];
Debug.Assert( mValidTasks <= mMsgChecks.Length );
			int i = 0;
			for ( ; i < mValidTasks; ++i )
			{
				mMsgChecks[i].Text = allTypes[i].ToString();
			}
			for ( ; i < mMsgChecks.Length; ++i )
			{
				mMsgChecks[i].Hide();
			}
		}

	public	void	ToggleAllowInteraction( bool allow )
		{
			mMsgDisplay.ToggleAllowInteraction( allow );
			AllOnButton.Enabled = AllOffButton.Enabled = 
			mAllowRefresh =	allow;
			TogglePauseButton( !allow );
		}

		private void MessagesToggle(object sender, System.EventArgs e)
		{
			mAllowRefresh = false;
			bool wantChecked = ( sender == AllOnButton );
			
			for ( int i = 0; i < mValidTasks; ++i )
			{
				mMsgChecks[i].Checked = wantChecked;
			}
			mAllowRefresh = true;
			mMsgDisplay.UpdateMessageHistory();
		}

		private void PauseGameRequest(object sender, System.EventArgs e)
		{
			mPlayGameMgr.PauseGameRequest();
			TogglePauseButton( false );
		}

		public	void	TogglePauseButton( bool isEnabled )
		{
			PauseButton.Enabled = isEnabled;
		}

	}
}
