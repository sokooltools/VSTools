using System.Windows.Forms;

namespace SokoolTools.VsTools
{
	partial class OptionsDialog
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private System.ComponentModel.IContainer components = null;

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
					components.Dispose();
			base.Dispose(disposing);
		}

		//.............................................................................................................

		#region Windows Form Designer generated code

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.chkJavascriptStripComments = new System.Windows.Forms.CheckBox();
			this.lblCommentDividerLineChar = new System.Windows.Forms.Label();
			this.grpFormatComments = new System.Windows.Forms.GroupBox();
			this.chkCommentDividerLineRightAligned = new System.Windows.Forms.CheckBox();
			this.chkCommentDividerLineIndentText = new System.Windows.Forms.CheckBox();
			this.grpInsertRegionDividerLines = new System.Windows.Forms.GroupBox();
			this.txtRegionDividerLineChar = new System.Windows.Forms.TextBox();
			this.updRegionDividerLineRepeat = new System.Windows.Forms.NumericUpDown();
			this.lblRegionDividerLineChar = new System.Windows.Forms.Label();
			this.lblRegionDividerLineRepeat = new System.Windows.Forms.Label();
			this.chkRegionDividerLinesInsert = new System.Windows.Forms.CheckBox();
			this.txtCommentDividerLineChar = new System.Windows.Forms.TextBox();
			this.updCommentDividerLineRepeat = new System.Windows.Forms.NumericUpDown();
			this.lblCommentDividerLineRepeat = new System.Windows.Forms.Label();
			this.grpFormatOrCompactJavascript = new System.Windows.Forms.GroupBox();
			this.grpPasteAsComments = new System.Windows.Forms.GroupBox();
			this.updPasteCommentsMaxLineLength = new System.Windows.Forms.NumericUpDown();
			this.lblPasteCommentsMaxLineLength = new System.Windows.Forms.Label();
			this.grpVariableAlignment = new System.Windows.Forms.GroupBox();
			this.updVariableAlignment = new System.Windows.Forms.NumericUpDown();
			this.lblMaximumIndent = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.tabPageExternalTool = new System.Windows.Forms.TabPage();
			this.txtExternalToolPath2 = new SokoolTools.VsTools.EllipsisTextBox();
			this.lblExternalTool2 = new System.Windows.Forms.Label();
			this.txtExternalToolPath1 = new SokoolTools.VsTools.EllipsisTextBox();
			this.btnBrowseExternalTool2 = new System.Windows.Forms.Button();
			this.lblExternalTool1 = new System.Windows.Forms.Label();
			this.btnBrowseExternalTool1 = new System.Windows.Forms.Button();
			this.lblConsoleWindowTimeoutSeconds = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblConsoleWindowTimeoutNote = new System.Windows.Forms.Label();
			this.updAutoCloseSeconds = new System.Windows.Forms.NumericUpDown();
			this.tabPageLogFile = new System.Windows.Forms.TabPage();
			this.updLogLevel = new System.Windows.Forms.NumericUpDown();
			this.lblLogLevel = new System.Windows.Forms.Label();
			this.chkIsLoggingEnabled = new System.Windows.Forms.CheckBox();
			this.txtLogFile = new SokoolTools.VsTools.EllipsisTextBox();
			this.lblLogFile = new System.Windows.Forms.Label();
			this.btnBrowseLogFile = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.grpFormatComments.SuspendLayout();
			this.grpInsertRegionDividerLines.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updRegionDividerLineRepeat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.updCommentDividerLineRepeat)).BeginInit();
			this.grpFormatOrCompactJavascript.SuspendLayout();
			this.grpPasteAsComments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updPasteCommentsMaxLineLength)).BeginInit();
			this.grpVariableAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updVariableAlignment)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.tabPageExternalTool.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updAutoCloseSeconds)).BeginInit();
			this.tabPageLogFile.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updLogLevel)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(135, 349);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(223, 349);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			// 
			// chkJavascriptStripComments
			// 
			this.chkJavascriptStripComments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkJavascriptStripComments.Location = new System.Drawing.Point(16, 16);
			this.chkJavascriptStripComments.Name = "chkJavascriptStripComments";
			this.chkJavascriptStripComments.Size = new System.Drawing.Size(160, 24);
			this.chkJavascriptStripComments.TabIndex = 0;
			this.chkJavascriptStripComments.Text = "Strip Javascript Comments";
			this.toolTip1.SetToolTip(this.chkJavascriptStripComments, "When checked, indicates that comments should be removed from javascript code when" +
        " it gets formatted.");
			this.chkJavascriptStripComments.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// lblCommentDividerLineChar
			// 
			this.lblCommentDividerLineChar.AutoSize = true;
			this.lblCommentDividerLineChar.Location = new System.Drawing.Point(15, 28);
			this.lblCommentDividerLineChar.Name = "lblCommentDividerLineChar";
			this.lblCommentDividerLineChar.Size = new System.Drawing.Size(174, 13);
			this.lblCommentDividerLineChar.TabIndex = 0;
			this.lblCommentDividerLineChar.Text = "Comment Divider Line Character:";
			this.toolTip1.SetToolTip(this.lblCommentDividerLineChar, "Character to repeat for creation of a comment divider line.");
			// 
			// grpFormatComments
			// 
			this.grpFormatComments.Controls.Add(this.chkCommentDividerLineRightAligned);
			this.grpFormatComments.Controls.Add(this.chkCommentDividerLineIndentText);
			this.grpFormatComments.Controls.Add(this.grpInsertRegionDividerLines);
			this.grpFormatComments.Controls.Add(this.txtCommentDividerLineChar);
			this.grpFormatComments.Controls.Add(this.updCommentDividerLineRepeat);
			this.grpFormatComments.Controls.Add(this.lblCommentDividerLineChar);
			this.grpFormatComments.Controls.Add(this.lblCommentDividerLineRepeat);
			this.grpFormatComments.Location = new System.Drawing.Point(6, 15);
			this.grpFormatComments.Name = "grpFormatComments";
			this.grpFormatComments.Size = new System.Drawing.Size(392, 159);
			this.grpFormatComments.TabIndex = 0;
			this.grpFormatComments.TabStop = false;
			this.grpFormatComments.Text = "Format Comments";
			// 
			// chkCommentDividerLineRightAligned
			// 
			this.chkCommentDividerLineRightAligned.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkCommentDividerLineRightAligned.Location = new System.Drawing.Point(188, 56);
			this.chkCommentDividerLineRightAligned.Name = "chkCommentDividerLineRightAligned";
			this.chkCommentDividerLineRightAligned.Size = new System.Drawing.Size(201, 18);
			this.chkCommentDividerLineRightAligned.TabIndex = 5;
			this.chkCommentDividerLineRightAligned.Text = "Right Align Comment Divider Lines";
			this.toolTip1.SetToolTip(this.chkCommentDividerLineRightAligned, "When checked, indicates that comment divider lines should be aligned on the right" +
        " side.");
			this.chkCommentDividerLineRightAligned.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// chkCommentDividerLineIndentText
			// 
			this.chkCommentDividerLineIndentText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkCommentDividerLineIndentText.Location = new System.Drawing.Point(15, 56);
			this.chkCommentDividerLineIndentText.Name = "chkCommentDividerLineIndentText";
			this.chkCommentDividerLineIndentText.Size = new System.Drawing.Size(158, 18);
			this.chkCommentDividerLineIndentText.TabIndex = 4;
			this.chkCommentDividerLineIndentText.Text = "Indent Commented Text";
			this.toolTip1.SetToolTip(this.chkCommentDividerLineIndentText, "When checked, indicates that each word-wrapped line of commented text should be i" +
        "ndented.");
			this.chkCommentDividerLineIndentText.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// grpInsertRegionDividerLines
			// 
			this.grpInsertRegionDividerLines.Controls.Add(this.txtRegionDividerLineChar);
			this.grpInsertRegionDividerLines.Controls.Add(this.updRegionDividerLineRepeat);
			this.grpInsertRegionDividerLines.Controls.Add(this.lblRegionDividerLineChar);
			this.grpInsertRegionDividerLines.Controls.Add(this.lblRegionDividerLineRepeat);
			this.grpInsertRegionDividerLines.Controls.Add(this.chkRegionDividerLinesInsert);
			this.grpInsertRegionDividerLines.Location = new System.Drawing.Point(6, 87);
			this.grpInsertRegionDividerLines.Name = "grpInsertRegionDividerLines";
			this.grpInsertRegionDividerLines.Size = new System.Drawing.Size(380, 56);
			this.grpInsertRegionDividerLines.TabIndex = 6;
			this.grpInsertRegionDividerLines.TabStop = false;
			// 
			// txtRegionDividerLineChar
			// 
			this.txtRegionDividerLineChar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtRegionDividerLineChar.Enabled = false;
			this.txtRegionDividerLineChar.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtRegionDividerLineChar.Location = new System.Drawing.Point(184, 21);
			this.txtRegionDividerLineChar.MaxLength = 1;
			this.txtRegionDividerLineChar.Name = "txtRegionDividerLineChar";
			this.txtRegionDividerLineChar.Size = new System.Drawing.Size(20, 22);
			this.txtRegionDividerLineChar.TabIndex = 2;
			this.txtRegionDividerLineChar.Text = ".";
			this.txtRegionDividerLineChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtRegionDividerLineChar.WordWrap = false;
			this.txtRegionDividerLineChar.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
			// 
			// updRegionDividerLineRepeat
			// 
			this.updRegionDividerLineRepeat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updRegionDividerLineRepeat.Enabled = false;
			this.updRegionDividerLineRepeat.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updRegionDividerLineRepeat.Location = new System.Drawing.Point(264, 22);
			this.updRegionDividerLineRepeat.Maximum = new decimal(new int[] {
            132,
            0,
            0,
            0});
			this.updRegionDividerLineRepeat.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.updRegionDividerLineRepeat.Name = "updRegionDividerLineRepeat";
			this.updRegionDividerLineRepeat.Size = new System.Drawing.Size(42, 20);
			this.updRegionDividerLineRepeat.TabIndex = 4;
			this.updRegionDividerLineRepeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updRegionDividerLineRepeat.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.updRegionDividerLineRepeat.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// lblRegionDividerLineChar
			// 
			this.lblRegionDividerLineChar.AutoSize = true;
			this.lblRegionDividerLineChar.Location = new System.Drawing.Point(20, 25);
			this.lblRegionDividerLineChar.Name = "lblRegionDividerLineChar";
			this.lblRegionDividerLineChar.Size = new System.Drawing.Size(162, 13);
			this.lblRegionDividerLineChar.TabIndex = 1;
			this.lblRegionDividerLineChar.Text = "Region Divider Line Character:";
			this.toolTip1.SetToolTip(this.lblRegionDividerLineChar, "Character to repeat in the creation of a region divider line.");
			// 
			// lblRegionDividerLineRepeat
			// 
			this.lblRegionDividerLineRepeat.AutoSize = true;
			this.lblRegionDividerLineRepeat.Location = new System.Drawing.Point(216, 25);
			this.lblRegionDividerLineRepeat.Name = "lblRegionDividerLineRepeat";
			this.lblRegionDividerLineRepeat.Size = new System.Drawing.Size(46, 13);
			this.lblRegionDividerLineRepeat.TabIndex = 3;
			this.lblRegionDividerLineRepeat.Text = "Repeat:";
			this.toolTip1.SetToolTip(this.lblRegionDividerLineRepeat, "Number of times the character should be repeated to create a region divider line." +
        "");
			// 
			// chkRegionDividerLinesInsert
			// 
			this.chkRegionDividerLinesInsert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkRegionDividerLinesInsert.Location = new System.Drawing.Point(8, 0);
			this.chkRegionDividerLinesInsert.Name = "chkRegionDividerLinesInsert";
			this.chkRegionDividerLinesInsert.Size = new System.Drawing.Size(160, 18);
			this.chkRegionDividerLinesInsert.TabIndex = 0;
			this.chkRegionDividerLinesInsert.Text = "Insert Region Divider Lines";
			this.toolTip1.SetToolTip(this.chkRegionDividerLinesInsert, "When checked, indicates that region divider lines should be inserted into the cod" +
        "e window during a format operation.");
			this.chkRegionDividerLinesInsert.CheckedChanged += new System.EventHandler(this.ChkRegionDividerLinesInsert_CheckedChanged);
			// 
			// txtCommentDividerLineChar
			// 
			this.txtCommentDividerLineChar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtCommentDividerLineChar.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCommentDividerLineChar.Location = new System.Drawing.Point(191, 24);
			this.txtCommentDividerLineChar.MaxLength = 1;
			this.txtCommentDividerLineChar.Name = "txtCommentDividerLineChar";
			this.txtCommentDividerLineChar.Size = new System.Drawing.Size(20, 22);
			this.txtCommentDividerLineChar.TabIndex = 1;
			this.txtCommentDividerLineChar.Text = "-";
			this.txtCommentDividerLineChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtCommentDividerLineChar.WordWrap = false;
			// 
			// updCommentDividerLineRepeat
			// 
			this.updCommentDividerLineRepeat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updCommentDividerLineRepeat.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updCommentDividerLineRepeat.Location = new System.Drawing.Point(269, 24);
			this.updCommentDividerLineRepeat.Maximum = new decimal(new int[] {
            132,
            0,
            0,
            0});
			this.updCommentDividerLineRepeat.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.updCommentDividerLineRepeat.Name = "updCommentDividerLineRepeat";
			this.updCommentDividerLineRepeat.Size = new System.Drawing.Size(42, 20);
			this.updCommentDividerLineRepeat.TabIndex = 3;
			this.updCommentDividerLineRepeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updCommentDividerLineRepeat.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.updCommentDividerLineRepeat.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// lblCommentDividerLineRepeat
			// 
			this.lblCommentDividerLineRepeat.AutoSize = true;
			this.lblCommentDividerLineRepeat.Location = new System.Drawing.Point(221, 27);
			this.lblCommentDividerLineRepeat.Name = "lblCommentDividerLineRepeat";
			this.lblCommentDividerLineRepeat.Size = new System.Drawing.Size(46, 13);
			this.lblCommentDividerLineRepeat.TabIndex = 2;
			this.lblCommentDividerLineRepeat.Text = "Repeat:";
			this.toolTip1.SetToolTip(this.lblCommentDividerLineRepeat, "Number of times the character should be repeated to create a comment divider line" +
        ".");
			// 
			// grpFormatOrCompactJavascript
			// 
			this.grpFormatOrCompactJavascript.Controls.Add(this.chkJavascriptStripComments);
			this.grpFormatOrCompactJavascript.Location = new System.Drawing.Point(6, 184);
			this.grpFormatOrCompactJavascript.Name = "grpFormatOrCompactJavascript";
			this.grpFormatOrCompactJavascript.Size = new System.Drawing.Size(197, 48);
			this.grpFormatOrCompactJavascript.TabIndex = 1;
			this.grpFormatOrCompactJavascript.TabStop = false;
			this.grpFormatOrCompactJavascript.Text = "Format or Compact Javascript";
			// 
			// grpPasteAsComments
			// 
			this.grpPasteAsComments.Controls.Add(this.updPasteCommentsMaxLineLength);
			this.grpPasteAsComments.Controls.Add(this.lblPasteCommentsMaxLineLength);
			this.grpPasteAsComments.Location = new System.Drawing.Point(6, 242);
			this.grpPasteAsComments.Name = "grpPasteAsComments";
			this.grpPasteAsComments.Size = new System.Drawing.Size(197, 48);
			this.grpPasteAsComments.TabIndex = 2;
			this.grpPasteAsComments.TabStop = false;
			this.grpPasteAsComments.Text = "Paste As Comments";
			// 
			// updPasteCommentsMaxLineLength
			// 
			this.updPasteCommentsMaxLineLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updPasteCommentsMaxLineLength.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updPasteCommentsMaxLineLength.Location = new System.Drawing.Point(140, 16);
			this.updPasteCommentsMaxLineLength.Maximum = new decimal(new int[] {
            132,
            0,
            0,
            0});
			this.updPasteCommentsMaxLineLength.Name = "updPasteCommentsMaxLineLength";
			this.updPasteCommentsMaxLineLength.Size = new System.Drawing.Size(42, 20);
			this.updPasteCommentsMaxLineLength.TabIndex = 1;
			this.updPasteCommentsMaxLineLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updPasteCommentsMaxLineLength.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// lblPasteCommentsMaxLineLength
			// 
			this.lblPasteCommentsMaxLineLength.AutoSize = true;
			this.lblPasteCommentsMaxLineLength.Location = new System.Drawing.Point(16, 19);
			this.lblPasteCommentsMaxLineLength.Name = "lblPasteCommentsMaxLineLength";
			this.lblPasteCommentsMaxLineLength.Size = new System.Drawing.Size(122, 13);
			this.lblPasteCommentsMaxLineLength.TabIndex = 0;
			this.lblPasteCommentsMaxLineLength.Text = "Maximum Line Length:";
			this.toolTip1.SetToolTip(this.lblPasteCommentsMaxLineLength, "Maximum length of each line of a pasted text before it word-wraps to the next lin" +
        "e.");
			// 
			// grpVariableAlignment
			// 
			this.grpVariableAlignment.Controls.Add(this.updVariableAlignment);
			this.grpVariableAlignment.Controls.Add(this.lblMaximumIndent);
			this.grpVariableAlignment.Location = new System.Drawing.Point(218, 242);
			this.grpVariableAlignment.Name = "grpVariableAlignment";
			this.grpVariableAlignment.Size = new System.Drawing.Size(180, 48);
			this.grpVariableAlignment.TabIndex = 3;
			this.grpVariableAlignment.TabStop = false;
			this.grpVariableAlignment.Text = "Variable Alignment";
			// 
			// updVariableAlignment
			// 
			this.updVariableAlignment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updVariableAlignment.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updVariableAlignment.Location = new System.Drawing.Point(126, 16);
			this.updVariableAlignment.Maximum = new decimal(new int[] {
            132,
            0,
            0,
            0});
			this.updVariableAlignment.Name = "updVariableAlignment";
			this.updVariableAlignment.Size = new System.Drawing.Size(42, 20);
			this.updVariableAlignment.TabIndex = 1;
			this.updVariableAlignment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updVariableAlignment.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// lblMaximumIndent
			// 
			this.lblMaximumIndent.AutoSize = true;
			this.lblMaximumIndent.Location = new System.Drawing.Point(28, 19);
			this.lblMaximumIndent.Name = "lblMaximumIndent";
			this.lblMaximumIndent.Size = new System.Drawing.Size(96, 13);
			this.lblMaximumIndent.TabIndex = 0;
			this.lblMaximumIndent.Text = "Maximum Indent:";
			this.toolTip1.SetToolTip(this.lblMaximumIndent, "The maximum indentation to be used when aligning variables.");
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageGeneral);
			this.tabControl1.Controls.Add(this.tabPageExternalTool);
			this.tabControl1.Controls.Add(this.tabPageLogFile);
			this.tabControl1.Location = new System.Drawing.Point(8, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(417, 331);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Controls.Add(this.grpFormatComments);
			this.tabPageGeneral.Controls.Add(this.grpVariableAlignment);
			this.tabPageGeneral.Controls.Add(this.grpFormatOrCompactJavascript);
			this.tabPageGeneral.Controls.Add(this.grpPasteAsComments);
			this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageGeneral.Size = new System.Drawing.Size(409, 305);
			this.tabPageGeneral.TabIndex = 0;
			this.tabPageGeneral.Text = "General";
			this.tabPageGeneral.UseVisualStyleBackColor = true;
			// 
			// tabPageExternalTool
			// 
			this.tabPageExternalTool.Controls.Add(this.txtExternalToolPath2);
			this.tabPageExternalTool.Controls.Add(this.lblExternalTool2);
			this.tabPageExternalTool.Controls.Add(this.txtExternalToolPath1);
			this.tabPageExternalTool.Controls.Add(this.btnBrowseExternalTool2);
			this.tabPageExternalTool.Controls.Add(this.lblExternalTool1);
			this.tabPageExternalTool.Controls.Add(this.btnBrowseExternalTool1);
			this.tabPageExternalTool.Controls.Add(this.lblConsoleWindowTimeoutSeconds);
			this.tabPageExternalTool.Controls.Add(this.label1);
			this.tabPageExternalTool.Controls.Add(this.lblConsoleWindowTimeoutNote);
			this.tabPageExternalTool.Controls.Add(this.updAutoCloseSeconds);
			this.tabPageExternalTool.Location = new System.Drawing.Point(4, 22);
			this.tabPageExternalTool.Name = "tabPageExternalTool";
			this.tabPageExternalTool.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageExternalTool.Size = new System.Drawing.Size(409, 305);
			this.tabPageExternalTool.TabIndex = 1;
			this.tabPageExternalTool.Text = "External Tool";
			this.tabPageExternalTool.UseVisualStyleBackColor = true;
			// 
			// txtExternalToolPath2
			// 
			this.txtExternalToolPath2.EllipsisType = SokoolTools.VsTools.EllipsisTextBox.EllipsisLocation.Path;
			this.txtExternalToolPath2.ExpansionLineCount = 0;
			this.txtExternalToolPath2.Location = new System.Drawing.Point(10, 86);
			this.txtExternalToolPath2.Name = "txtExternalToolPath2";
			this.txtExternalToolPath2.Size = new System.Drawing.Size(310, 22);
			this.txtExternalToolPath2.TabIndex = 4;
			this.txtExternalToolPath2.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
			this.txtExternalToolPath2.Validating += new System.ComponentModel.CancelEventHandler(this.TxtExternalToolPath_Validating);
			// 
			// lblExternalTool2
			// 
			this.lblExternalTool2.AutoSize = true;
			this.lblExternalTool2.Location = new System.Drawing.Point(10, 69);
			this.lblExternalTool2.Name = "lblExternalTool2";
			this.lblExternalTool2.Size = new System.Drawing.Size(85, 13);
			this.lblExternalTool2.TabIndex = 3;
			this.lblExternalTool2.Text = "External Tool 2:";
			this.toolTip1.SetToolTip(this.lblExternalTool2, "Full path to the external tool 2\'s executable file.");
			// 
			// txtExternalToolPath1
			// 
			this.txtExternalToolPath1.EllipsisType = SokoolTools.VsTools.EllipsisTextBox.EllipsisLocation.Path;
			this.txtExternalToolPath1.ExpansionLineCount = 0;
			this.txtExternalToolPath1.Location = new System.Drawing.Point(10, 44);
			this.txtExternalToolPath1.Name = "txtExternalToolPath1";
			this.txtExternalToolPath1.Size = new System.Drawing.Size(310, 22);
			this.txtExternalToolPath1.TabIndex = 4;
			this.txtExternalToolPath1.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
			this.txtExternalToolPath1.Validating += new System.ComponentModel.CancelEventHandler(this.TxtExternalToolPath_Validating);
			// 
			// btnBrowseExternalTool2
			// 
			this.btnBrowseExternalTool2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseExternalTool2.Location = new System.Drawing.Point(326, 85);
			this.btnBrowseExternalTool2.Name = "btnBrowseExternalTool2";
			this.btnBrowseExternalTool2.Size = new System.Drawing.Size(62, 23);
			this.btnBrowseExternalTool2.TabIndex = 5;
			this.btnBrowseExternalTool2.Text = "Browse...";
			this.btnBrowseExternalTool2.UseVisualStyleBackColor = true;
			this.btnBrowseExternalTool2.Click += new System.EventHandler(this.BtnBrowseExternalTool_Click);
			// 
			// lblExternalTool1
			// 
			this.lblExternalTool1.AutoSize = true;
			this.lblExternalTool1.Location = new System.Drawing.Point(10, 27);
			this.lblExternalTool1.Name = "lblExternalTool1";
			this.lblExternalTool1.Size = new System.Drawing.Size(85, 13);
			this.lblExternalTool1.TabIndex = 3;
			this.lblExternalTool1.Text = "External Tool 1:";
			this.toolTip1.SetToolTip(this.lblExternalTool1, "Full path to the external tool 1\'s executable file.");
			// 
			// btnBrowseExternalTool1
			// 
			this.btnBrowseExternalTool1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseExternalTool1.Location = new System.Drawing.Point(326, 43);
			this.btnBrowseExternalTool1.Name = "btnBrowseExternalTool1";
			this.btnBrowseExternalTool1.Size = new System.Drawing.Size(62, 23);
			this.btnBrowseExternalTool1.TabIndex = 5;
			this.btnBrowseExternalTool1.Text = "Browse...";
			this.btnBrowseExternalTool1.UseVisualStyleBackColor = true;
			this.btnBrowseExternalTool1.Click += new System.EventHandler(this.BtnBrowseExternalTool_Click);
			// 
			// lblConsoleWindowTimeoutSeconds
			// 
			this.lblConsoleWindowTimeoutSeconds.AutoSize = true;
			this.lblConsoleWindowTimeoutSeconds.Location = new System.Drawing.Point(7, 244);
			this.lblConsoleWindowTimeoutSeconds.Name = "lblConsoleWindowTimeoutSeconds";
			this.lblConsoleWindowTimeoutSeconds.Size = new System.Drawing.Size(271, 13);
			this.lblConsoleWindowTimeoutSeconds.TabIndex = 0;
			this.lblConsoleWindowTimeoutSeconds.Text = "Close console windows automatically after waiting:";
			this.toolTip1.SetToolTip(this.lblConsoleWindowTimeoutSeconds, "The number of seconds to wait before automatically closing the console window fol" +
        "lowing a copy operation.");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(296, 266);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "(0 = stay open)";
			// 
			// lblConsoleWindowTimeoutNote
			// 
			this.lblConsoleWindowTimeoutNote.AutoSize = true;
			this.lblConsoleWindowTimeoutNote.Location = new System.Drawing.Point(329, 244);
			this.lblConsoleWindowTimeoutNote.Name = "lblConsoleWindowTimeoutNote";
			this.lblConsoleWindowTimeoutNote.Size = new System.Drawing.Size(52, 13);
			this.lblConsoleWindowTimeoutNote.TabIndex = 2;
			this.lblConsoleWindowTimeoutNote.Text = "seconds.";
			// 
			// updAutoCloseSeconds
			// 
			this.updAutoCloseSeconds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updAutoCloseSeconds.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updAutoCloseSeconds.Location = new System.Drawing.Point(284, 242);
			this.updAutoCloseSeconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.updAutoCloseSeconds.Name = "updAutoCloseSeconds";
			this.updAutoCloseSeconds.Size = new System.Drawing.Size(42, 20);
			this.updAutoCloseSeconds.TabIndex = 1;
			this.updAutoCloseSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updAutoCloseSeconds.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// tabPageLogFile
			// 
			this.tabPageLogFile.Controls.Add(this.updLogLevel);
			this.tabPageLogFile.Controls.Add(this.lblLogLevel);
			this.tabPageLogFile.Controls.Add(this.chkIsLoggingEnabled);
			this.tabPageLogFile.Controls.Add(this.txtLogFile);
			this.tabPageLogFile.Controls.Add(this.lblLogFile);
			this.tabPageLogFile.Controls.Add(this.btnBrowseLogFile);
			this.tabPageLogFile.Location = new System.Drawing.Point(4, 22);
			this.tabPageLogFile.Name = "tabPageLogFile";
			this.tabPageLogFile.Size = new System.Drawing.Size(409, 305);
			this.tabPageLogFile.TabIndex = 2;
			this.tabPageLogFile.Text = "LogFile";
			this.tabPageLogFile.UseVisualStyleBackColor = true;
			// 
			// updLogLevel
			// 
			this.updLogLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.updLogLevel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updLogLevel.Location = new System.Drawing.Point(186, 22);
			this.updLogLevel.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.updLogLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.updLogLevel.Name = "updLogLevel";
			this.updLogLevel.Size = new System.Drawing.Size(42, 20);
			this.updLogLevel.TabIndex = 11;
			this.updLogLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.updLogLevel.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.updLogLevel.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
			// 
			// lblLogLevel
			// 
			this.lblLogLevel.AutoSize = true;
			this.lblLogLevel.Location = new System.Drawing.Point(152, 25);
			this.lblLogLevel.Name = "lblLogLevel";
			this.lblLogLevel.Size = new System.Drawing.Size(35, 13);
			this.lblLogLevel.TabIndex = 10;
			this.lblLogLevel.Text = "Level:";
			this.toolTip1.SetToolTip(this.lblLogLevel, "Log Level (1=Terse; 2=Normal; 3=Verbose.");
			// 
			// chkIsLoggingEnabled
			// 
			this.chkIsLoggingEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkIsLoggingEnabled.Location = new System.Drawing.Point(17, 22);
			this.chkIsLoggingEnabled.Name = "chkIsLoggingEnabled";
			this.chkIsLoggingEnabled.Size = new System.Drawing.Size(107, 18);
			this.chkIsLoggingEnabled.TabIndex = 9;
			this.chkIsLoggingEnabled.Text = "Enable Logging";
			this.toolTip1.SetToolTip(this.chkIsLoggingEnabled, "When checked, indicates that logging is enabled and all messages will be written " +
        "to the log file defined below.");
			this.chkIsLoggingEnabled.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// txtLogFile
			// 
			this.txtLogFile.EllipsisType = SokoolTools.VsTools.EllipsisTextBox.EllipsisLocation.Path;
			this.txtLogFile.Enabled = false;
			this.txtLogFile.ExpansionLineCount = 0;
			this.txtLogFile.Location = new System.Drawing.Point(15, 69);
			this.txtLogFile.Name = "txtLogFile";
			this.txtLogFile.Size = new System.Drawing.Size(313, 22);
			this.txtLogFile.TabIndex = 7;
			this.txtLogFile.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
			// 
			// lblLogFile
			// 
			this.lblLogFile.AutoSize = true;
			this.lblLogFile.Location = new System.Drawing.Point(15, 52);
			this.lblLogFile.Name = "lblLogFile";
			this.lblLogFile.Size = new System.Drawing.Size(50, 13);
			this.lblLogFile.TabIndex = 6;
			this.lblLogFile.Text = "Log File:";
			this.toolTip1.SetToolTip(this.lblLogFile, "Full path to the log file.");
			// 
			// btnBrowseLogFile
			// 
			this.btnBrowseLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseLogFile.Enabled = false;
			this.btnBrowseLogFile.Location = new System.Drawing.Point(334, 68);
			this.btnBrowseLogFile.Name = "btnBrowseLogFile";
			this.btnBrowseLogFile.Size = new System.Drawing.Size(62, 23);
			this.btnBrowseLogFile.TabIndex = 8;
			this.btnBrowseLogFile.Text = "Browse...";
			this.toolTip1.SetToolTip(this.btnBrowseLogFile, "Click to select path to the log file.");
			this.btnBrowseLogFile.UseVisualStyleBackColor = true;
			this.btnBrowseLogFile.Click += new System.EventHandler(this.BtnBrowseLogFile_Click);
			// 
			// toolTip2
			// 
			this.toolTip1.ToolTipTitle = "When checked, indicates that comments should be removed from javascript code when" +
    " it gets formatted.";
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(432, 382);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "VsTools Options";
			this.Load += new System.EventHandler(this.FrmOptions_Load);
			this.grpFormatComments.ResumeLayout(false);
			this.grpFormatComments.PerformLayout();
			this.grpInsertRegionDividerLines.ResumeLayout(false);
			this.grpInsertRegionDividerLines.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.updRegionDividerLineRepeat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.updCommentDividerLineRepeat)).EndInit();
			this.grpFormatOrCompactJavascript.ResumeLayout(false);
			this.grpPasteAsComments.ResumeLayout(false);
			this.grpPasteAsComments.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.updPasteCommentsMaxLineLength)).EndInit();
			this.grpVariableAlignment.ResumeLayout(false);
			this.grpVariableAlignment.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.updVariableAlignment)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageExternalTool.ResumeLayout(false);
			this.tabPageExternalTool.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.updAutoCloseSeconds)).EndInit();
			this.tabPageLogFile.ResumeLayout(false);
			this.tabPageLogFile.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.updLogLevel)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		
		private Button btnBrowseExternalTool1;
		private Button btnBrowseExternalTool2;
		private Button btnBrowseLogFile;
		private Button btnCancel;
		private Button btnOK;
		private CheckBox chkCommentDividerLineIndentText;
		private CheckBox chkCommentDividerLineRightAligned;
		private CheckBox chkIsLoggingEnabled;
		private CheckBox chkJavascriptStripComments;
		private CheckBox chkRegionDividerLinesInsert;
		private EllipsisTextBox txtExternalToolPath1;
		private EllipsisTextBox txtExternalToolPath2;
		private EllipsisTextBox txtLogFile;
		private GroupBox grpFormatComments;
		private GroupBox grpFormatOrCompactJavascript;
		private GroupBox grpInsertRegionDividerLines;
		private GroupBox grpPasteAsComments;
		private GroupBox grpVariableAlignment;
		private Label label1;
		private Label lblCommentDividerLineChar;
		private Label lblCommentDividerLineRepeat;
		private Label lblConsoleWindowTimeoutNote;
		private Label lblConsoleWindowTimeoutSeconds;
		private Label lblExternalTool1;
		private Label lblExternalTool2;
		private Label lblLogFile;
		private Label lblLogLevel;
		private Label lblMaximumIndent;
		private Label lblPasteCommentsMaxLineLength;
		private Label lblRegionDividerLineChar;
		private Label lblRegionDividerLineRepeat;
		private NumericUpDown updAutoCloseSeconds;
		private NumericUpDown updCommentDividerLineRepeat;
		private NumericUpDown updLogLevel;
		private NumericUpDown updPasteCommentsMaxLineLength;
		private NumericUpDown updRegionDividerLineRepeat;
		private NumericUpDown updVariableAlignment;
		private TabControl tabControl1;
		private TabPage tabPageExternalTool;
		private TabPage tabPageGeneral;
		private TabPage tabPageLogFile;
		private TextBox txtCommentDividerLineChar;
		private TextBox txtRegionDividerLineChar;
		private ToolTip toolTip1;
	}
}
