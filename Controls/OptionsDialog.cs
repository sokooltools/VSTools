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
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Load event of this dialog.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void FrmOptions_Load(object sender, EventArgs e)
		{
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
			bool bEnabled                      = chkRegionDividerLinesInsert.Checked;
			txtRegionDividerLineChar.Enabled   = bEnabled;
			updRegionDividerLineRepeat.Enabled = bEnabled;
			btnOK.Enabled                      = _isLoaded;
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

		private void TxtExternalToolPath_Validating(object sender, CancelEventArgs e)
		{
			var tbx = (EllipsisTextBox)sender;
			if (String.IsNullOrEmpty(tbx.Text) || File.Exists(Utilities.GetExpandedPath(tbx.Text)))
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
				if (!String.IsNullOrEmpty(tbx.Text))
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
				if (!String.IsNullOrEmpty(tbx.Text))
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
	}
}