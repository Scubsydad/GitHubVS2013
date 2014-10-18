namespace Settlers_of_Catan
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.Tabs = new System.Windows.Forms.TabControl();
			this.PlayGameTab = new System.Windows.Forms.TabPage();
			this.StateExplainTabs = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.SettlementHexSelectTab = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.richTextBox4 = new System.Windows.Forms.RichTextBox();
			this.SettlementCornerTab = new System.Windows.Forms.TabPage();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.RoadwayPlotTab = new System.Windows.Forms.TabPage();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.richTextBox3 = new System.Windows.Forms.RichTextBox();
			this.TurnOrderCombo = new System.Windows.Forms.ComboBox();
			this.RandomSeedBox = new System.Windows.Forms.TextBox();
			this.RandSeedLbl = new System.Windows.Forms.Label();
			this.mMapPictBox = new System.Windows.Forms.PictureBox();
			this.PortLocationsCombo = new System.Windows.Forms.ComboBox();
			this.ResourceRollCombo = new System.Windows.Forms.ComboBox();
			this.MapTypeCombo = new System.Windows.Forms.ComboBox();
			this.FirstPlayerTrackBarLbl = new System.Windows.Forms.Label();
			this.FirstPlayerTrackBar = new System.Windows.Forms.TrackBar();
			this.StartStopButton = new System.Windows.Forms.Button();
			this.SideCtrlCombo3 = new System.Windows.Forms.ComboBox();
			this.SideCtrlCombo2 = new System.Windows.Forms.ComboBox();
			this.SideCtrlCombo1 = new System.Windows.Forms.ComboBox();
			this.SideCtrlCombo0 = new System.Windows.Forms.ComboBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SideLogicTab = new System.Windows.Forms.TabPage();
			this.SideLogicTabs = new System.Windows.Forms.TabControl();
			this.LogicTabBlue = new System.Windows.Forms.TabPage();
			this.LogicTabOrange = new System.Windows.Forms.TabPage();
			this.LogicTabRed = new System.Windows.Forms.TabPage();
			this.LogicTabSilver = new System.Windows.Forms.TabPage();
			this.Tabs.SuspendLayout();
			this.PlayGameTab.SuspendLayout();
			this.StateExplainTabs.SuspendLayout();
			this.SettlementHexSelectTab.SuspendLayout();
			this.SettlementCornerTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.RoadwayPlotTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mMapPictBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.FirstPlayerTrackBar)).BeginInit();
			this.SideLogicTab.SuspendLayout();
			this.SideLogicTabs.SuspendLayout();
			this.SuspendLayout();
			// 
			// Tabs
			// 
			this.Tabs.Controls.Add(this.PlayGameTab);
			this.Tabs.Controls.Add(this.SideLogicTab);
			this.Tabs.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Tabs.Location = new System.Drawing.Point(0, -1);
			this.Tabs.Name = "Tabs";
			this.Tabs.SelectedIndex = 0;
			this.Tabs.Size = new System.Drawing.Size(1152, 744);
			this.Tabs.TabIndex = 1;
			// 
			// PlayGameTab
			// 
			this.PlayGameTab.BackColor = System.Drawing.SystemColors.Control;
			this.PlayGameTab.Controls.Add(this.StateExplainTabs);
			this.PlayGameTab.Controls.Add(this.TurnOrderCombo);
			this.PlayGameTab.Controls.Add(this.RandomSeedBox);
			this.PlayGameTab.Controls.Add(this.RandSeedLbl);
			this.PlayGameTab.Controls.Add(this.mMapPictBox);
			this.PlayGameTab.Controls.Add(this.PortLocationsCombo);
			this.PlayGameTab.Controls.Add(this.ResourceRollCombo);
			this.PlayGameTab.Controls.Add(this.MapTypeCombo);
			this.PlayGameTab.Controls.Add(this.FirstPlayerTrackBarLbl);
			this.PlayGameTab.Controls.Add(this.FirstPlayerTrackBar);
			this.PlayGameTab.Controls.Add(this.StartStopButton);
			this.PlayGameTab.Controls.Add(this.SideCtrlCombo3);
			this.PlayGameTab.Controls.Add(this.SideCtrlCombo2);
			this.PlayGameTab.Controls.Add(this.SideCtrlCombo1);
			this.PlayGameTab.Controls.Add(this.SideCtrlCombo0);
			this.PlayGameTab.Controls.Add(this.richTextBox1);
			this.PlayGameTab.Location = new System.Drawing.Point(4, 22);
			this.PlayGameTab.Name = "PlayGameTab";
			this.PlayGameTab.Size = new System.Drawing.Size(1144, 718);
			this.PlayGameTab.TabIndex = 0;
			this.PlayGameTab.Text = "Play Game";
			// 
			// StateExplainTabs
			// 
			this.StateExplainTabs.Controls.Add(this.tabPage1);
			this.StateExplainTabs.Controls.Add(this.SettlementHexSelectTab);
			this.StateExplainTabs.Controls.Add(this.SettlementCornerTab);
			this.StateExplainTabs.Controls.Add(this.RoadwayPlotTab);
			this.StateExplainTabs.Location = new System.Drawing.Point(671, 154);
			this.StateExplainTabs.Name = "StateExplainTabs";
			this.StateExplainTabs.SelectedIndex = 0;
			this.StateExplainTabs.Size = new System.Drawing.Size(470, 140);
			this.StateExplainTabs.TabIndex = 16;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(462, 114);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "0";
			// 
			// SettlementHexSelectTab
			// 
			this.SettlementHexSelectTab.BackColor = System.Drawing.SystemColors.Control;
			this.SettlementHexSelectTab.Controls.Add(this.label4);
			this.SettlementHexSelectTab.Controls.Add(this.label5);
			this.SettlementHexSelectTab.Controls.Add(this.label3);
			this.SettlementHexSelectTab.Controls.Add(this.label2);
			this.SettlementHexSelectTab.Controls.Add(this.label1);
			this.SettlementHexSelectTab.Controls.Add(this.richTextBox4);
			this.SettlementHexSelectTab.Location = new System.Drawing.Point(4, 22);
			this.SettlementHexSelectTab.Name = "SettlementHexSelectTab";
			this.SettlementHexSelectTab.Padding = new System.Windows.Forms.Padding(3);
			this.SettlementHexSelectTab.Size = new System.Drawing.Size(462, 114);
			this.SettlementHexSelectTab.TabIndex = 1;
			this.SettlementHexSelectTab.Tag = "Settlement Hex Selection";
			this.SettlementHexSelectTab.Text = "1";
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(322, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(134, 23);
			this.label4.TabIndex = 68;
			this.label4.Text = "Lowest Quality Choice";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.Orange;
			this.label5.Location = new System.Drawing.Point(322, 58);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(134, 23);
			this.label5.TabIndex = 67;
			this.label5.Text = "Lower Quality Pick";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Yellow;
			this.label3.Location = new System.Drawing.Point(322, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(134, 23);
			this.label3.TabIndex = 66;
			this.label3.Text = "Middle of Road Pick";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.YellowGreen;
			this.label2.Location = new System.Drawing.Point(322, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 23);
			this.label2.TabIndex = 65;
			this.label2.Text = "Secondary Preference";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Chartreuse;
			this.label1.Location = new System.Drawing.Point(322, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 23);
			this.label1.TabIndex = 64;
			this.label1.Text = "Top Preference";
			// 
			// richTextBox4
			// 
			this.richTextBox4.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox4.Location = new System.Drawing.Point(3, 4);
			this.richTextBox4.Name = "richTextBox4";
			this.richTextBox4.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox4.Size = new System.Drawing.Size(324, 93);
			this.richTextBox4.TabIndex = 63;
			this.richTextBox4.Text = resources.GetString("richTextBox4.Text");
			this.richTextBox4.WordWrap = false;
			// 
			// SettlementCornerTab
			// 
			this.SettlementCornerTab.BackColor = System.Drawing.SystemColors.Control;
			this.SettlementCornerTab.Controls.Add(this.pictureBox1);
			this.SettlementCornerTab.Controls.Add(this.richTextBox2);
			this.SettlementCornerTab.Location = new System.Drawing.Point(4, 22);
			this.SettlementCornerTab.Name = "SettlementCornerTab";
			this.SettlementCornerTab.Size = new System.Drawing.Size(462, 114);
			this.SettlementCornerTab.TabIndex = 2;
			this.SettlementCornerTab.Tag = "Settlement Build Location";
			this.SettlementCornerTab.Text = "2";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(333, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(126, 107);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 65;
			this.pictureBox1.TabStop = false;
			// 
			// richTextBox2
			// 
			this.richTextBox2.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox2.Location = new System.Drawing.Point(3, 4);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox2.Size = new System.Drawing.Size(324, 107);
			this.richTextBox2.TabIndex = 64;
			this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
			this.richTextBox2.WordWrap = false;
			// 
			// RoadwayPlotTab
			// 
			this.RoadwayPlotTab.BackColor = System.Drawing.SystemColors.Control;
			this.RoadwayPlotTab.Controls.Add(this.pictureBox2);
			this.RoadwayPlotTab.Controls.Add(this.richTextBox3);
			this.RoadwayPlotTab.Location = new System.Drawing.Point(4, 22);
			this.RoadwayPlotTab.Name = "RoadwayPlotTab";
			this.RoadwayPlotTab.Size = new System.Drawing.Size(462, 114);
			this.RoadwayPlotTab.TabIndex = 3;
			this.RoadwayPlotTab.Tag = "Early Road Building";
			this.RoadwayPlotTab.Text = "3";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(333, 4);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(126, 107);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 65;
			this.pictureBox2.TabStop = false;
			// 
			// richTextBox3
			// 
			this.richTextBox3.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox3.Location = new System.Drawing.Point(3, 4);
			this.richTextBox3.Name = "richTextBox3";
			this.richTextBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox3.Size = new System.Drawing.Size(324, 107);
			this.richTextBox3.TabIndex = 64;
			this.richTextBox3.Text = resources.GetString("richTextBox3.Text");
			this.richTextBox3.WordWrap = false;
			// 
			// TurnOrderCombo
			// 
			this.TurnOrderCombo.FormattingEnabled = true;
			this.TurnOrderCombo.Items.AddRange(new object[] {
            "By Color Order",
            "Randomized"});
			this.TurnOrderCombo.Location = new System.Drawing.Point(733, 52);
			this.TurnOrderCombo.Name = "TurnOrderCombo";
			this.TurnOrderCombo.Size = new System.Drawing.Size(95, 21);
			this.TurnOrderCombo.TabIndex = 15;
			this.TurnOrderCombo.Text = "By Color Order";
			// 
			// RandomSeedBox
			// 
			this.RandomSeedBox.Location = new System.Drawing.Point(762, 26);
			this.RandomSeedBox.Name = "RandomSeedBox";
			this.RandomSeedBox.Size = new System.Drawing.Size(34, 21);
			this.RandomSeedBox.TabIndex = 7;
			this.RandomSeedBox.Text = "-1";
			// 
			// RandSeedLbl
			// 
			this.RandSeedLbl.AutoSize = true;
			this.RandSeedLbl.Location = new System.Drawing.Point(671, 29);
			this.RandSeedLbl.Name = "RandSeedLbl";
			this.RandSeedLbl.Size = new System.Drawing.Size(141, 13);
			this.RandSeedLbl.TabIndex = 14;
			this.RandSeedLbl.Text = "Random # Seed :               ?";
			// 
			// mMapPictBox
			// 
			this.mMapPictBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.mMapPictBox.Location = new System.Drawing.Point(0, 0);
			this.mMapPictBox.Name = "mMapPictBox";
			this.mMapPictBox.Size = new System.Drawing.Size(665, 665);
			this.mMapPictBox.TabIndex = 13;
			this.mMapPictBox.TabStop = false;
			// 
			// PortLocationsCombo
			// 
			this.PortLocationsCombo.FormattingEnabled = true;
			this.PortLocationsCombo.Items.AddRange(new object[] {
            "Default",
            "Randomized"});
			this.PortLocationsCombo.Location = new System.Drawing.Point(751, 131);
			this.PortLocationsCombo.Name = "PortLocationsCombo";
			this.PortLocationsCombo.Size = new System.Drawing.Size(89, 21);
			this.PortLocationsCombo.TabIndex = 12;
			this.PortLocationsCombo.Text = "Default";
			// 
			// ResourceRollCombo
			// 
			this.ResourceRollCombo.FormattingEnabled = true;
			this.ResourceRollCombo.Items.AddRange(new object[] {
            "Default",
            "Randomized"});
			this.ResourceRollCombo.Location = new System.Drawing.Point(808, 105);
			this.ResourceRollCombo.Name = "ResourceRollCombo";
			this.ResourceRollCombo.Size = new System.Drawing.Size(89, 21);
			this.ResourceRollCombo.TabIndex = 11;
			this.ResourceRollCombo.Text = "Default";
			// 
			// MapTypeCombo
			// 
			this.MapTypeCombo.FormattingEnabled = true;
			this.MapTypeCombo.Items.AddRange(new object[] {
            "Beginner Map",
            "Randomized"});
			this.MapTypeCombo.Location = new System.Drawing.Point(726, 79);
			this.MapTypeCombo.Name = "MapTypeCombo";
			this.MapTypeCombo.Size = new System.Drawing.Size(89, 21);
			this.MapTypeCombo.TabIndex = 10;
			this.MapTypeCombo.Text = "Beginner Map";
			// 
			// FirstPlayerTrackBarLbl
			// 
			this.FirstPlayerTrackBarLbl.AutoSize = true;
			this.FirstPlayerTrackBarLbl.Location = new System.Drawing.Point(1031, 56);
			this.FirstPlayerTrackBarLbl.Name = "FirstPlayerTrackBarLbl";
			this.FirstPlayerTrackBarLbl.Size = new System.Drawing.Size(35, 13);
			this.FirstPlayerTrackBarLbl.TabIndex = 9;
			this.FirstPlayerTrackBarLbl.Text = "label1";
			// 
			// FirstPlayerTrackBar
			// 
			this.FirstPlayerTrackBar.AutoSize = false;
			this.FirstPlayerTrackBar.Location = new System.Drawing.Point(945, 56);
			this.FirstPlayerTrackBar.Maximum = 4;
			this.FirstPlayerTrackBar.Name = "FirstPlayerTrackBar";
			this.FirstPlayerTrackBar.Size = new System.Drawing.Size(74, 17);
			this.FirstPlayerTrackBar.TabIndex = 8;
			this.FirstPlayerTrackBar.Value = 2;
			// 
			// StartStopButton
			// 
			this.StartStopButton.Location = new System.Drawing.Point(1020, 126);
			this.StartStopButton.Name = "StartStopButton";
			this.StartStopButton.Size = new System.Drawing.Size(75, 23);
			this.StartStopButton.TabIndex = 6;
			this.StartStopButton.Text = "Start Game";
			this.StartStopButton.UseVisualStyleBackColor = true;
			this.StartStopButton.Click += new System.EventHandler(this._StartStopGameRequest);
			// 
			// SideCtrlCombo3
			// 
			this.SideCtrlCombo3.FormattingEnabled = true;
			this.SideCtrlCombo3.Items.AddRange(new object[] {
            "CPU",
            "User",
            "n/a"});
			this.SideCtrlCombo3.Location = new System.Drawing.Point(1088, 3);
			this.SideCtrlCombo3.Name = "SideCtrlCombo3";
			this.SideCtrlCombo3.Size = new System.Drawing.Size(47, 21);
			this.SideCtrlCombo3.TabIndex = 5;
			this.SideCtrlCombo3.Text = "CPU";
			this.SideCtrlCombo3.SelectedIndexChanged += new System.EventHandler(this._SilverOwnerChangeCallBack);
			// 
			// SideCtrlCombo2
			// 
			this.SideCtrlCombo2.FormattingEnabled = true;
			this.SideCtrlCombo2.Items.AddRange(new object[] {
            "CPU",
            "User"});
			this.SideCtrlCombo2.Location = new System.Drawing.Point(993, 3);
			this.SideCtrlCombo2.Name = "SideCtrlCombo2";
			this.SideCtrlCombo2.Size = new System.Drawing.Size(47, 21);
			this.SideCtrlCombo2.TabIndex = 4;
			this.SideCtrlCombo2.Text = "CPU";
			// 
			// SideCtrlCombo1
			// 
			this.SideCtrlCombo1.FormattingEnabled = true;
			this.SideCtrlCombo1.Items.AddRange(new object[] {
            "CPU",
            "User"});
			this.SideCtrlCombo1.Location = new System.Drawing.Point(905, 3);
			this.SideCtrlCombo1.Name = "SideCtrlCombo1";
			this.SideCtrlCombo1.Size = new System.Drawing.Size(47, 21);
			this.SideCtrlCombo1.TabIndex = 3;
			this.SideCtrlCombo1.Text = "CPU";
			// 
			// SideCtrlCombo0
			// 
			this.SideCtrlCombo0.FormattingEnabled = true;
			this.SideCtrlCombo0.Items.AddRange(new object[] {
            "CPU",
            "User"});
			this.SideCtrlCombo0.Location = new System.Drawing.Point(793, 3);
			this.SideCtrlCombo0.Name = "SideCtrlCombo0";
			this.SideCtrlCombo0.Size = new System.Drawing.Size(47, 21);
			this.SideCtrlCombo0.TabIndex = 2;
			this.SideCtrlCombo0.Text = "User";
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Location = new System.Drawing.Point(673, 4);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(463, 543);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "Side Control\tBlue\t\tOrange\t\tRed\t\tSilver\n\n\t\t\t\t  \t\t\t\t\t\n\nTurn Order\t\t\tWho Goes First?" +
    "\n\nMap Type\t\t\t \t\t\t\n\n Resource Die Roll Location\n\nPort Locations";
			// 
			// SideLogicTab
			// 
			this.SideLogicTab.Controls.Add(this.SideLogicTabs);
			this.SideLogicTab.Location = new System.Drawing.Point(4, 22);
			this.SideLogicTab.Name = "SideLogicTab";
			this.SideLogicTab.Size = new System.Drawing.Size(1144, 718);
			this.SideLogicTab.TabIndex = 1;
			this.SideLogicTab.Text = "Logics";
			this.SideLogicTab.UseVisualStyleBackColor = true;
			// 
			// SideLogicTabs
			// 
			this.SideLogicTabs.Controls.Add(this.LogicTabBlue);
			this.SideLogicTabs.Controls.Add(this.LogicTabOrange);
			this.SideLogicTabs.Controls.Add(this.LogicTabRed);
			this.SideLogicTabs.Controls.Add(this.LogicTabSilver);
			this.SideLogicTabs.Location = new System.Drawing.Point(0, 0);
			this.SideLogicTabs.Name = "SideLogicTabs";
			this.SideLogicTabs.SelectedIndex = 0;
			this.SideLogicTabs.Size = new System.Drawing.Size(1144, 718);
			this.SideLogicTabs.TabIndex = 0;
			// 
			// LogicTabBlue
			// 
			this.LogicTabBlue.BackColor = System.Drawing.SystemColors.Control;
			this.LogicTabBlue.Location = new System.Drawing.Point(4, 22);
			this.LogicTabBlue.Name = "LogicTabBlue";
			this.LogicTabBlue.Padding = new System.Windows.Forms.Padding(3);
			this.LogicTabBlue.Size = new System.Drawing.Size(1136, 692);
			this.LogicTabBlue.TabIndex = 0;
			this.LogicTabBlue.Text = "Blue";
			// 
			// LogicTabOrange
			// 
			this.LogicTabOrange.BackColor = System.Drawing.SystemColors.Control;
			this.LogicTabOrange.Location = new System.Drawing.Point(4, 22);
			this.LogicTabOrange.Name = "LogicTabOrange";
			this.LogicTabOrange.Padding = new System.Windows.Forms.Padding(3);
			this.LogicTabOrange.Size = new System.Drawing.Size(1136, 692);
			this.LogicTabOrange.TabIndex = 1;
			this.LogicTabOrange.Text = "Orange";
			// 
			// LogicTabRed
			// 
			this.LogicTabRed.BackColor = System.Drawing.SystemColors.Control;
			this.LogicTabRed.Location = new System.Drawing.Point(4, 22);
			this.LogicTabRed.Name = "LogicTabRed";
			this.LogicTabRed.Size = new System.Drawing.Size(1136, 692);
			this.LogicTabRed.TabIndex = 2;
			this.LogicTabRed.Text = "Red";
			// 
			// LogicTabSilver
			// 
			this.LogicTabSilver.BackColor = System.Drawing.SystemColors.Control;
			this.LogicTabSilver.Location = new System.Drawing.Point(4, 22);
			this.LogicTabSilver.Name = "LogicTabSilver";
			this.LogicTabSilver.Size = new System.Drawing.Size(1136, 692);
			this.LogicTabSilver.TabIndex = 3;
			this.LogicTabSilver.Text = "Silver";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1152, 743);
			this.Controls.Add(this.Tabs);
			this.Name = "Form1";
			this.Text = "BK\'s Settlers of Catan";
			this.Tabs.ResumeLayout(false);
			this.PlayGameTab.ResumeLayout(false);
			this.PlayGameTab.PerformLayout();
			this.StateExplainTabs.ResumeLayout(false);
			this.SettlementHexSelectTab.ResumeLayout(false);
			this.SettlementCornerTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.RoadwayPlotTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mMapPictBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.FirstPlayerTrackBar)).EndInit();
			this.SideLogicTab.ResumeLayout(false);
			this.SideLogicTabs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl Tabs;
		private System.Windows.Forms.TabPage PlayGameTab;
		private System.Windows.Forms.ComboBox TurnOrderCombo;
		private System.Windows.Forms.TextBox RandomSeedBox;
		private System.Windows.Forms.Label RandSeedLbl;
		private System.Windows.Forms.PictureBox mMapPictBox;
		private System.Windows.Forms.ComboBox PortLocationsCombo;
		private System.Windows.Forms.ComboBox ResourceRollCombo;
		private System.Windows.Forms.ComboBox MapTypeCombo;
		private System.Windows.Forms.Label FirstPlayerTrackBarLbl;
		private System.Windows.Forms.TrackBar FirstPlayerTrackBar;
		private System.Windows.Forms.Button StartStopButton;
		private System.Windows.Forms.ComboBox SideCtrlCombo3;
		private System.Windows.Forms.ComboBox SideCtrlCombo2;
		private System.Windows.Forms.ComboBox SideCtrlCombo1;
		private System.Windows.Forms.ComboBox SideCtrlCombo0;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.TabPage SideLogicTab;
		private System.Windows.Forms.TabControl SideLogicTabs;
		private System.Windows.Forms.TabPage LogicTabBlue;
		private System.Windows.Forms.TabPage LogicTabOrange;
		private System.Windows.Forms.TabPage LogicTabRed;
		private System.Windows.Forms.TabPage LogicTabSilver;
		private System.Windows.Forms.TabControl StateExplainTabs;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage SettlementHexSelectTab;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox4;
		private System.Windows.Forms.TabPage SettlementCornerTab;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.TabPage RoadwayPlotTab;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.RichTextBox richTextBox3;
	}
}

