using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Options dialog form.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public partial class OptionsDialog : Form
	{
		private bool _isLoaded;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="OptionsDialog"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public OptionsDialog()
		{
			InitializeComponent();
			AttachMiscEvents();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Load event of this dialog.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void FrmOptions_Load(object sender, EventArgs e)
		{
			AddToolTips();
			ReadRegistry();
			_isLoaded = true;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the OK button.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnOK_Click(object sender, EventArgs e)
		{
			WriteRegistry();
			Close();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Cancel button.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void ChkRegionDividerLinesInsert_CheckedChanged(object sender, EventArgs e)
		{
			bool bEnabled = (chkRegionDividerLinesInsert.Checked);
			txtRegionDividerLineChar.Enabled = bEnabled;
			updRegionDividerLineRepeat.Enabled = bEnabled;
			btnOK.Enabled = _isLoaded;
		}

		private void Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = _isLoaded;
			if (sender == chkIsLoggingEnabled)
				updLogLevel.Enabled = btnBrowseLogFile.Enabled = txtLogFile.Enabled = chkIsLoggingEnabled.Checked;
		}

		private void NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = _isLoaded;
		}

		private void TextBox_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = _isLoaded;
		}

		private static void TxtExternalToolPath_Validating(object sender, CancelEventArgs e)
		{
			var tbx = (EllipsisTextBox)sender;
			if (string.IsNullOrEmpty(tbx.Text) || File.Exists(Utilities.GetExpandedPath(tbx.Text)))
				return;
			MessageBox.Show(Resources.OptionsDialog_FileDoesNotExist);
			e.Cancel = true;
		}

		private void BtnBrowseLogFile_Click(object sender, EventArgs e)
		{
			EllipsisTextBox tbx = txtLogFile;
			using (var dlg = new SaveFileDialog())
			{
				dlg.Title = @"Log File";
				if (!string.IsNullOrEmpty(tbx.Text))
				{
					dlg.FileName = Path.GetFileName(tbx.Text);
					dlg.InitialDirectory = Path.GetDirectoryName(Utilities.GetExpandedPath(tbx.Text));
				}
				dlg.DefaultExt = ".log";
				dlg.Filter = @"Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*";
				if (dlg.ShowDialog(this) == DialogResult.OK)
					tbx.Text = Path.GetFullPath(dlg.FileName);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised when the Browse... button is clicked.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnBrowseExternalTool_Click(object sender, EventArgs e)
		{
			EllipsisTextBox tbx = sender == btnBrowseExternalTool1 ? txtExternalToolPath1 : txtExternalToolPath2;
			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = Resources.OptionsDialog_SelectFile;
				if (!string.IsNullOrEmpty(tbx.Text))
				{
					dlg.FileName = Path.GetFileName(tbx.Text);
					dlg.InitialDirectory = Path.GetDirectoryName(Utilities.GetExpandedPath(tbx.Text));
				}
				dlg.DefaultExt = ".exe";
				dlg.Filter = @"Executable files (*.exe)|*.exe|All files (*.*)|*.*";
				if (dlg.ShowDialog(this) == DialogResult.OK)
					tbx.Text = Path.GetFullPath(dlg.FileName);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Attaches the misc events.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void AttachMiscEvents()
		{
			Load += FrmOptions_Load;
			btnBrowseExternalTool1.Click += BtnBrowseExternalTool_Click;
			btnBrowseExternalTool2.Click += BtnBrowseExternalTool_Click;
			btnBrowseLogFile.Click += BtnBrowseLogFile_Click;
			btnCancel.Click += BtnCancel_Click;
			btnOK.Click += BtnOK_Click;
			chkCommentDividerLineIndentText.CheckedChanged += Checkbox_CheckedChanged;
			chkCommentDividerLineRightAligned.CheckedChanged += Checkbox_CheckedChanged;
			chkJavascriptStripComments.CheckedChanged += Checkbox_CheckedChanged;
			chkRegionDividerLinesInsert.CheckedChanged += ChkRegionDividerLinesInsert_CheckedChanged;
			txtCommentDividerLineChar.TextChanged += TextBox_TextChanged;
			txtExternalToolPath1.TextChanged += TextBox_TextChanged;
			txtExternalToolPath2.TextChanged += TextBox_TextChanged;
			chkIsLoggingEnabled.CheckedChanged += Checkbox_CheckedChanged;
			txtLogFile.TextChanged += TextBox_TextChanged;
			txtExternalToolPath1.Validating += TxtExternalToolPath_Validating;
			txtExternalToolPath2.Validating += TxtExternalToolPath_Validating;
			updAutoCloseSeconds.ValueChanged += NumericUpDown_ValueChanged;
			updCommentDividerLineRepeat.ValueChanged += NumericUpDown_ValueChanged;
			updPasteCommentsMaxLineLength.ValueChanged += NumericUpDown_ValueChanged;
			updRegionDividerLineRepeat.ValueChanged += NumericUpDown_ValueChanged;
			updVariableAlignment.ValueChanged += NumericUpDown_ValueChanged;
			updLogLevel.ValueChanged += NumericUpDown_ValueChanged;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Reads information from the registry and loads it into the respective controls.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void ReadRegistry()
		{
			chkRegionDividerLinesInsert.Checked = OptionsHelper.IsRegionDividerLinesInserted;
			updRegionDividerLineRepeat.Value = OptionsHelper.RegionDividerLineRepeat;
			updCommentDividerLineRepeat.Value = OptionsHelper.CommentDividerLineRepeat;
			txtRegionDividerLineChar.Text = OptionsHelper.RegionDividerLineChar.ToString(CultureInfo.InvariantCulture);
			txtCommentDividerLineChar.Text = OptionsHelper.CommentDividerLineChar.ToString(CultureInfo.InvariantCulture);
			chkCommentDividerLineIndentText.Checked = OptionsHelper.IsCommentIndented;
			chkCommentDividerLineRightAligned.Checked = OptionsHelper.IsCommentDividerLineRightAligned;
			updPasteCommentsMaxLineLength.Value = OptionsHelper.PasteCommentMaxLineLength;
			chkJavascriptStripComments.Checked = OptionsHelper.IsJavascriptCommentsStripped;
			updVariableAlignment.Value = OptionsHelper.AlignVariablesMaxIndent;
			chkIsLoggingEnabled.Checked = OptionsHelper.IsLogEnabled;

			txtLogFile.Text = OptionsHelper.LogFile;
			updLogLevel.Value = OptionsHelper.LogLevel;
			txtExternalToolPath1.Text = OptionsHelper.ExternalToolPath1;
			txtExternalToolPath2.Text = OptionsHelper.ExternalToolPath2;
			updAutoCloseSeconds.Value = OptionsHelper.AutoCloseSeconds;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes information to the registry which has been obtained from the respective controls.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void WriteRegistry()
		{
			OptionsHelper.IsRegionDividerLinesInserted = chkRegionDividerLinesInsert.Checked;
			OptionsHelper.RegionDividerLineChar = Convert.ToChar(txtRegionDividerLineChar.Text ?? ".");
			OptionsHelper.RegionDividerLineRepeat = (updRegionDividerLineRepeat.Value <= 0 ? 109 : (int)updRegionDividerLineRepeat.Value);
			OptionsHelper.CommentDividerLineChar = Convert.ToChar(txtCommentDividerLineChar.Text ?? "-");
			OptionsHelper.CommentDividerLineRepeat = (updCommentDividerLineRepeat.Value <= 0 ? 100 : (int)updCommentDividerLineRepeat.Value);
			OptionsHelper.IsCommentIndented = chkCommentDividerLineIndentText.Checked;
			OptionsHelper.IsCommentDividerLineRightAligned = chkCommentDividerLineRightAligned.Checked;
			OptionsHelper.PasteCommentMaxLineLength = (updPasteCommentsMaxLineLength.Value <= 0 ? 100 : (int)updPasteCommentsMaxLineLength.Value);
			OptionsHelper.IsJavascriptCommentsStripped = chkJavascriptStripComments.Checked;
			OptionsHelper.AlignVariablesMaxIndent = (updVariableAlignment.Value <= 0 ? 50 : (int)updVariableAlignment.Value);
			OptionsHelper.IsLogEnabled = chkIsLoggingEnabled.Checked;

			OptionsHelper.LogFile = txtLogFile.Text;
			OptionsHelper.LogLevel = (int)updLogLevel.Value;
			OptionsHelper.ExternalToolPath1 = txtExternalToolPath1.Text;
			OptionsHelper.ExternalToolPath2 = txtExternalToolPath2.Text;
			OptionsHelper.AutoCloseSeconds = (int)updAutoCloseSeconds.Value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds tootips to some of the controls.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void AddToolTips()
		{
			toolTip1 = new ToolTip();
			toolTip1.SetToolTip(chkJavascriptStripComments,
				"When checked, indicates that comments should be removed from javascript code when it gets formatted.");
			toolTip1.SetToolTip(chkRegionDividerLinesInsert,
				"When checked, indicates that region divider lines should be inserted into the code window during a format operation.");
			toolTip1.SetToolTip(lblPasteCommentsMaxLineLength,
				"Maximum length of each line of a pasted text before it word-wraps to the next line.");
			toolTip1.SetToolTip(lblRegionDividerLineChar,
				"Character to repeat in the creation of a region divider line.");
			toolTip1.SetToolTip(lblRegionDividerLineRepeat,
				"Number of times the character should be repeated to create a region divider line.");
			toolTip1.SetToolTip(lblCommentDividerLineChar,
				"Character to repeat for creation of a comment divider line.");
			toolTip1.SetToolTip(lblCommentDividerLineRepeat,
				"Number of times the character should be repeated to create a comment divider line.");
			toolTip1.SetToolTip(chkCommentDividerLineIndentText,
				"When checked, indicates that each word-wrapped line of commented text should be indented.");
			toolTip1.SetToolTip(chkCommentDividerLineRightAligned,
				"When checked, indicates that comment divider lines should be aligned on the right side.");
			toolTip1.SetToolTip(lblMaximumIndent,
				"The maximum indentation to be used when aligning variables.");
			toolTip1.SetToolTip(chkIsLoggingEnabled,
				"When checked, indicates that logging is enabled and all messages will be written to the log file defined below.");

			toolTip1.SetToolTip(lblLogFile,
				"Full path to the log file.");
			toolTip1.SetToolTip(lblLogLevel,
				"Log Level (1=Terse; 2=Normal; 3=Verbose.");
			toolTip1.SetToolTip(btnBrowseLogFile,
				"Click to select path to the log file.");
			toolTip1.SetToolTip(lblExternalTool1,
				"Full path to the external tool 1's executable file.");
			toolTip1.SetToolTip(lblExternalTool2,
				"Full path to the external tool 2's executable file.");
			toolTip1.SetToolTip(lblConsoleWindowTimeoutSeconds,
				"The number of seconds to wait before automatically closing the console window following a copy operation.");
		}
	}
}