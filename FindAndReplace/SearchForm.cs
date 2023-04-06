using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using SokoolTools.VsTools.FindAndReplace.Common;
using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// SearchForm
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public partial class SearchForm : Form
	{
		private const int ReplacePartHeight = 68;
		private static BackgroundWorker _bw;
		private static SearchHelp _searchHelp;
		private readonly _DTE _dte;

		private readonly OpenFilesSearcher _openFilesSearcher;
		//private readonly RecentCache _recentCache = new RecentCache();
		private readonly StaticFilesSearcher _staticFilesSearcher;
		private readonly WindowSearcher _windowSearcher;
		private Size _minimumSize;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchForm"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public SearchForm()
		{
			InitializeComponent();
			_bw = new BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			AttachMiscEventHandlers();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchForm"/> class.
		/// </summary>
		/// <param name="dte">The DTE.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public SearchForm(_DTE dte)
			: this()
		{
			_windowSearcher = new WindowSearcher(dte);
			_openFilesSearcher = new OpenFilesSearcher(dte);
			_staticFilesSearcher = new StaticFilesSearcher(dte);
			_dte = dte;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this is find in text window.
		/// </summary>
		/// <value><c>true</c> if this is find in text window; otherwise, <c>false</c>.</value>
		//------------------------------------------------------------------------------------------------------------------------
		private bool IsFindInTextWindow { set; get; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Attaches miscellaneous event handlers.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void AttachMiscEventHandlers()
		{
			Load += This_Load;
			Activated += This_Activated;
			btnBrowse.Click += BtnBrowse_Click;
			btnClose.Click += BtnClose_Click;
			btnFileTypesUnlock.Click += BtnFileTypesUnlock_Click;
			btnFindAll.Click += BtnFind_Click;
			btnFindNext.Click += BtnFind_Click;
			btnReplace.Click += BtnReplace_Click;
			btnReplaceAll.Click += BtnReplaceAll_Click;
			btnReplaceWithImage.Click += BtnReplaceWithImage_Click;
			cboFileTypes.SelectedIndexChanged += CboFileTypes_SelectedIndexChanged;
			cboReplacePattern.SelectedIndexChanged += CboReplacePattern_SelectedIndexChanged;
			cboSearchPattern.SelectedIndexChanged += CboSearchPattern_SelectedIndexChanged;
			cboSearchScope.SelectedIndexChanged += CboSearchScope_SelectedIndexChanged;
			FormClosing += This_FormClosing;
			lnkRegexHelp.LinkClicked += LnkRegexHelp_Click;
			lnkReset.LinkClicked += LnkReset_LinkClicked;
			_bw.DoWork += Bw_DoWork;
			_bw.ProgressChanged += Bw_ProgressChanged;
			_bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Detaches miscellaneous event handlers.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void DetachMiscEventHandlers()
		{
			Load -= This_Load;
			Activated -= This_Activated;
			btnBrowse.Click -= BtnBrowse_Click;
			btnClose.Click -= BtnClose_Click;
			btnFileTypesUnlock.Click -= BtnFileTypesUnlock_Click;
			btnFindAll.Click -= BtnFind_Click;
			btnFindNext.Click -= BtnFind_Click;
			btnReplace.Click -= BtnReplace_Click;
			btnReplaceAll.Click -= BtnReplaceAll_Click;
			btnReplaceWithImage.Click -= BtnReplaceWithImage_Click;
			cboFileTypes.SelectedIndexChanged -= CboFileTypes_SelectedIndexChanged;
			cboReplacePattern.SelectedIndexChanged -= CboReplacePattern_SelectedIndexChanged;
			cboSearchPattern.SelectedIndexChanged -= CboSearchPattern_SelectedIndexChanged;
			cboSearchScope.SelectedIndexChanged -= CboSearchScope_SelectedIndexChanged;
			FormClosing -= This_FormClosing;
			lnkRegexHelp.LinkClicked -= LnkRegexHelp_Click;
			lnkReset.LinkClicked -= LnkReset_LinkClicked;
			_bw.DoWork -= Bw_DoWork;
			_bw.ProgressChanged -= Bw_ProgressChanged;
			_bw.RunWorkerCompleted -= Bw_RunWorkerCompleted;
		}

		//..................................................................................................................................

		#region Form Event Handlers

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Load event of the SearchForm control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void This_Load(object sender, EventArgs e)
		{
			ShowReplaceControls(false);
			MinimumSize = Size;
			_minimumSize = Size;

			CacheBind(cboSearchPattern, CacheType.Find);
			CacheBind(cboReplacePattern, CacheType.Replace);

			ReadRegistry();

			ThreadHelper.ThrowIfNotOnUIThread();
			string text = _windowSearcher.GetSelectionText();
			if (!String.IsNullOrEmpty(text))
				txtSearchPattern.Text = text;

			cboFileTypes.Select(0, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the FormClosing event of the SearchForm control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			WriteRegistry();
			DetachMiscEventHandlers();
			_searchHelp?.Close();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Activated event of the SearchForm control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void This_Activated(object sender, EventArgs e)
		{
			string originalText = cboSearchScope.Text;

			if (originalText.Length == 0)
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Info.REG_MAIN_HIVE, false) ??
										 Registry.CurrentUser.CreateSubKey(Info.REG_MAIN_HIVE))
				{
					if (key != null)
						originalText = key.GetValue("cboSearchScope", "").ToString();
				}
			}

			ThreadHelper.ThrowIfNotOnUIThread();
			LoadSearchScope();

			// Select the first item when none is selected.
			cboSearchScope.SelectedItem = originalText;
			if (cboSearchScope.SelectedIndex < 0 && cboSearchScope.Items.Count > 0)
				cboSearchScope.SelectedIndex = 0;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Truncates the specified text using an ellipsis at the end if necessary.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string Trunc(string text)
		{
			if (String.IsNullOrEmpty(text))
				return String.Empty;
			return text.Substring(0, Math.Min(text.Length, 40)) + (text.Length > 40 ? "..." : "");
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnFind control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnFind_Click(object sender, EventArgs e)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            VsStatusBar.Text = "Find \"" + Trunc(txtSearchPattern.Text) + "\", '" + cboSearchScope.Text +
							   (chkIncludeSubFolders.Checked ? "', SubFolders" : "'");

			if (!IsControlsValid())
				return;

			if (!RecentCache.IsCached(CacheType.Find, txtSearchPattern.Text))
				RecentCache.Add(txtSearchPattern.Text, CacheType.Find);

			IsFindInTextWindow = sender == btnFindNext;

			try
			{
				Cursor = Cursors.WaitCursor;
				if (IsFindInTextWindow)
				{
					if (IsHandleOpenFiles())
					{
						_openFilesSearcher.Pattern = txtSearchPattern.Text;
						_openFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
							chkECMAScript.Checked);
						_openFilesSearcher.FindNext();
						return;
					}
					_windowSearcher.Pattern = txtSearchPattern.Text;
					_windowSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_windowSearcher.FindNext();
				}
				else
				{
					if (btnFindAll.Text == @"&Find All")
					{
						//btnFindAll.Text = @"&Cancel";
						//_bw.RunWorkerAsync("Hello to worker");

						_staticFilesSearcher.Pattern = txtSearchPattern.Text;
						_staticFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
							chkECMAScript.Checked);
						_staticFilesSearcher.IsSearchInSubDir = IsSearchInSubDir;
						_staticFilesSearcher.SearchScope = cboSearchScope.Text;
						_staticFilesSearcher.FileTypes = cboFileTypes.Text;

						if (!_staticFilesSearcher.IsFindInWindow)
						{
							if (updContextLines.Value < 2)
							{
								_staticFilesSearcher.FrontLines = 0;
								_staticFilesSearcher.BackLines = 0;
							}
							else
							{
								_staticFilesSearcher.FrontLines = (int)((updContextLines.Value - 1) / 2);
								_staticFilesSearcher.BackLines = (int)(updContextLines.Value - 1) - _staticFilesSearcher.FrontLines;
							}
						}
						_staticFilesSearcher.Search();
					}
					else
					{
						btnFindAll.Text = @"&Find All";

						//if (_bw.IsBusy)
						//    _bw.CancelAsync();
					}
				}
			}
			catch (ApplicationException appEx)
			{
				UIHelper.ShowInfo(appEx.Message);
			}
			finally
			{
				CacheBind(cboSearchPattern, CacheType.Find);
				Cursor = Cursors.Default;
			}
		}

		private void Bw_DoWork(object sender, DoWorkEventArgs e)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            _staticFilesSearcher.Pattern = txtSearchPattern.Text;
			_staticFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
				chkECMAScript.Checked);
			_staticFilesSearcher.IsSearchInSubDir = IsSearchInSubDir;
			_staticFilesSearcher.SearchScope = cboSearchScope.Text;
			_staticFilesSearcher.FileTypes = cboFileTypes.Text;

			//if (_bw.CancellationPending)
			//{
			//    e.Cancel = true;
			//    return;
			//}

			//_bw.ReportProgress(1);

			if (!_staticFilesSearcher.IsFindInWindow)
			{
				if (updContextLines.Value < 2)
				{
					_staticFilesSearcher.FrontLines = 0;
					_staticFilesSearcher.BackLines = 0;
				}
				else
				{
					_staticFilesSearcher.FrontLines = (int)((updContextLines.Value - 1) / 2);
					_staticFilesSearcher.BackLines = (int)(updContextLines.Value - 1) - _staticFilesSearcher.FrontLines;
				}
			}
			_staticFilesSearcher.Search();
		}

		private static void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
				Console.WriteLine(@"Find All canceled!");
			else if (e.Error != null)
				Console.WriteLine(@"Find All exception: " + e.Error);
			else
				Console.WriteLine(@"Find All completed: " + e.Result); // from DoWork
		}

		private static void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Console.WriteLine(@"Reached " + e.ProgressPercentage + @"%");
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnReplace control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnReplace_Click(object sender, EventArgs e)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            VsStatusBar.Text = "Replace \"" + Trunc(txtSearchPattern.Text) + "\", \"" + Trunc(txtReplacePattern.Text) + "\", '" +
							   cboSearchScope.Text + (chkIncludeSubFolders.Checked ? ", SubFolders" : "'");
			if (!IsControlsValid())
				return;
			if (!RecentCache.IsCached(CacheType.Replace, txtReplacePattern.Text))
				RecentCache.Add(txtReplacePattern.Text, CacheType.Replace);
			try
			{
				Cursor = Cursors.WaitCursor;
				if (IsHandleOpenFiles())
				{
					_openFilesSearcher.Pattern = txtSearchPattern.Text;
					_openFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_openFilesSearcher.ReplaceNext(txtReplacePattern.Text);
					return;
				}
				if (IsFindInTextWindow)
				{
					_windowSearcher.Pattern = txtSearchPattern.Text;
					_windowSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_windowSearcher.ReplaceNext(txtReplacePattern.Text);
				}
				else
				{
					btnFindNext.Enabled = true;
					_staticFilesSearcher.Pattern = txtSearchPattern.Text;
					_staticFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_staticFilesSearcher.IsSearchInSubDir = IsSearchInSubDir;
					_staticFilesSearcher.SearchScope = cboSearchScope.Text;
					_staticFilesSearcher.FileTypes = cboFileTypes.Text;
					_staticFilesSearcher.IsReplaceNext = true;
					_staticFilesSearcher.ReplaceNext(txtReplacePattern.Text);
				}
			}
			finally
			{
				CacheBind(cboReplacePattern, CacheType.Replace);
				Cursor = Cursors.Default;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnReplaceAll control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnReplaceAll_Click(object sender, EventArgs e)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!IsControlsValid())
				return;
			if (!RecentCache.IsCached(CacheType.Replace, txtReplacePattern.Text))
				RecentCache.Add(txtReplacePattern.Text, CacheType.Replace);
			try
			{
				Cursor = Cursors.WaitCursor;
				if (IsHandleOpenFiles())
				{
					_openFilesSearcher.Pattern = txtSearchPattern.Text;
					_openFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_openFilesSearcher.ReplaceAll(txtReplacePattern.Text);
					return;
				}
				if (IsFindInTextWindow)
				{
					_windowSearcher.Pattern = txtSearchPattern.Text;
					_windowSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_windowSearcher.ReplaceAll(txtReplacePattern.Text, false);
				}
				else
				{
					_staticFilesSearcher.Pattern = txtSearchPattern.Text;
					_staticFilesSearcher.SetRegexOptions(chkSingleLine.Checked, chkMultipleLine.Checked, chkIgnoreCase.Checked,
						chkECMAScript.Checked);
					_staticFilesSearcher.IsSearchInSubDir = IsSearchInSubDir;
					_staticFilesSearcher.SearchScope = cboSearchScope.Text;
					_staticFilesSearcher.FileTypes = cboFileTypes.Text;
					_staticFilesSearcher.IsReplaceNext = false;
					_staticFilesSearcher.ReplaceAll(txtReplacePattern.Text);
				}
			}
			finally
			{
				CacheBind(cboReplacePattern, CacheType.Replace);
				Cursor = Cursors.Default;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnBrowse control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnBrowse_Click(object sender, EventArgs e)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            using (var folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = @"Select directory to look in:";
				folderBrowserDialog.SelectedPath = Directory.Exists(cboSearchScope.Text)
					? cboSearchScope.Text
					: _dte.Solution.FileName.Length > 0
						? Path.GetDirectoryName(_dte.Solution.FileName)
						: Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					cboSearchScope.DropDownStyle = ComboBoxStyle.DropDown;
					if (!RecentCache.IsCached(CacheType.Directory, folderBrowserDialog.SelectedPath))
					{
						RecentCache.Add(folderBrowserDialog.SelectedPath, CacheType.Directory);
						int index = cboSearchScope.Items.Count < 4 ? 0 : 4;
						cboSearchScope.Items.Insert(index, folderBrowserDialog.SelectedPath);
						cboSearchScope.SelectedIndex = index;
					}
					else
						cboSearchScope.Text = folderBrowserDialog.SelectedPath;
					cboFileTypes.Enabled = true;
					chkIncludeSubFolders.Enabled = true;
					lblContextLines.Enabled = true;
					updContextLines.Enabled = true;
				}
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnReplaceWithImage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnReplaceWithImage_Click(object sender, EventArgs e)
		{
			ShowReplaceControls(true);
			MinimumSize = new Size(_minimumSize.Width, _minimumSize.Height + ReplacePartHeight);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event of the cboSearchScope control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CboSearchScope_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (((ComboBox)sender).Text)
			{
				case Info.SEARCH_SCOPE_CURRENT_DOCUMENT:
				case Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS:
					//IsFindInTextWindow = true;
					btnFindNext.Enabled = true;
					btnFindAll.Enabled = true;
					lblContextLines.Enabled = false;
					updContextLines.Enabled = false;
					lblFileTypes.Enabled = false;
					cboFileTypes.Enabled = false;
					btnFileTypesUnlock.Enabled = false;
					cboSearchScope.DropDownStyle = ComboBoxStyle.DropDownList;
					chkIncludeSubFolders.Enabled = false;
					break;
				case Info.SEARCH_SCOPE_CURRENT_PROJECT:
				case Info.SEARCH_SCOPE_ENTIRE_SOLUTION:
					btnFindNext.Enabled = true;
					btnFindAll.Enabled = true;
					lblContextLines.Enabled = true;
					updContextLines.Enabled = true;
					lblFileTypes.Enabled = true;
					cboFileTypes.Enabled = true;
					btnFileTypesUnlock.Enabled = true;
					cboSearchScope.DropDownStyle = ComboBoxStyle.DropDownList;
					chkIncludeSubFolders.Enabled = true;
					break;
				default:
					btnFindNext.Enabled = false;
					btnFindAll.Enabled = true;
					lblContextLines.Enabled = true;
					updContextLines.Enabled = true;
					lblFileTypes.Enabled = true;
					cboFileTypes.Enabled = true;
					btnFileTypesUnlock.Enabled = true;
					cboSearchScope.DropDownStyle = ComboBoxStyle.DropDownList;
					chkIncludeSubFolders.Enabled = true;
					break;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event of the cboFileTypes control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void CboFileTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			//cboFileTypes.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnFileTypesUnlock control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnFileTypesUnlock_Click(object sender, EventArgs e)
		{
			cboFileTypes.DropDownStyle = ComboBoxStyle.DropDown;
			cboFileTypes.Focus();
			cboFileTypes.SelectAll();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event of the cboSearchPattern control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CboSearchPattern_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtSearchPattern.Text = RecentCache.GetCachePattern(CacheType.Find, cboSearchPattern.SelectedIndex);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event of the cboReplacePattern control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CboReplacePattern_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtReplacePattern.Text = RecentCache.GetCachePattern(CacheType.Replace, cboReplacePattern.SelectedIndex);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the lnkRegexHelp control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void LnkRegexHelp_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_searchHelp = new SearchHelp { WindowState = FormWindowState.Maximized };
			_searchHelp.Show();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the LinkClicked event of the lnkReset control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void LnkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (
				MessageBox.Show(this, @"Ok to reset the form back to the default?", @"Reset", MessageBoxButtons.OKCancel,
					MessageBoxIcon.Question) == DialogResult.OK)
				ResetSearchForm();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the clicked event of the 'Close' button. (Used here to handle the ESC key being pressed
		/// by the user).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		//..................................................................................................................................

		#region Private Helper Methods

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether to search in the sub-directories has been indicated. (True always returned for
		/// project and entire solution)
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private bool IsSearchInSubDir
		{
			get
			{
				switch (cboSearchScope.Text)
				{
					case Info.SEARCH_SCOPE_CURRENT_PROJECT:
					case Info.SEARCH_SCOPE_ENTIRE_SOLUTION:
						return true;
				}
				return chkIncludeSubFolders.Checked;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the search scope combobox with the values according to the current state.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void LoadSearchScope()
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            cboSearchScope.Items.Clear();
			if (_dte.ActiveDocument != null)
				cboSearchScope.Items.Add(Info.SEARCH_SCOPE_CURRENT_DOCUMENT);
			if (_dte.ActiveDocument != null && _dte.ActiveDocument.Collection.Count > 0)
				cboSearchScope.Items.Add(Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS);
			if (_dte.Solution.FileName.Length > 0 && _dte.ActiveSolutionProjects != null)
				cboSearchScope.Items.Add(Info.SEARCH_SCOPE_CURRENT_PROJECT);
			if (_dte.Solution.FileName.Length > 0)
				cboSearchScope.Items.Add(Info.SEARCH_SCOPE_ENTIRE_SOLUTION);
			StringCollection cacheList = RecentCache.GetRecentList(CacheType.Directory);
			for (int i = cacheList.Count - 1; i >= 0; i--)
				cboSearchScope.Items.Add(cacheList[i]);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Resets the form.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void ResetSearchForm()
		{
			MinimumSize = _minimumSize;

			ThreadHelper.ThrowIfNotOnUIThread();
			RecentCache.ClearRecentList();
			CacheBind(cboSearchPattern, CacheType.Find);
			CacheBind(cboReplacePattern, CacheType.Replace);

			cboFileTypes.SelectedIndex = 0;
			LoadSearchScope();
			if (cboSearchScope.SelectedIndex < 0 && cboSearchScope.Items.Count > 0)
				cboSearchScope.SelectedIndex = 0;

			btnReplaceWithImage.Visible = true;
			btnReplaceAll.Visible = false;
			lblReplacePattern.Visible = false;
			cboReplacePattern.Visible = false;
			txtReplacePattern.Visible = false;
			txtSearchPattern.Height += ReplacePartHeight;
			txtReplacePattern.Text = String.Empty;
			txtSearchPattern.Text = String.Empty;

			chkECMAScript.Checked = false;
			chkIgnoreCase.Checked = false;
			chkMultipleLine.Checked = false;
			chkSingleLine.Checked = false;
			chkIncludeSubFolders.Checked = false;
			updContextLines.Value = 0;

			Size = _minimumSize;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the replace controls.
		/// </summary>
		/// <param name="isShow">if set to <c>true</c> the various controls are shown.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void ShowReplaceControls(bool isShow)
		{
			if (isShow && btnReplaceWithImage.Visible == false)
				return;

			btnReplaceWithImage.Visible = !isShow;
			btnReplaceAll.Visible = isShow;
			lblReplacePattern.Visible = isShow;
			cboReplacePattern.Visible = isShow;
			txtReplacePattern.Visible = isShow;

			int searchPatternHeight = txtSearchPattern.Height;

			if (isShow)
				Height += ReplacePartHeight;
			else
				Height -= ReplacePartHeight;

			txtSearchPattern.Height = searchPatternHeight;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Validates the controls.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private bool IsControlsValid()
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            if (cboSearchScope.Text.Length > 0)
				return true;
			UIHelper.ShowInfo("Please specify a search scope");
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether [is handle open files].
		/// </summary>
		/// <returns><c>true</c> if [is handle open files]; otherwise, <c>false</c>.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private bool IsHandleOpenFiles()
		{
			return cboSearchScope.Text == Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Caches the bind.
		/// </summary>
		/// <param name="comboBox">The combo box.</param>
		/// <param name="cacheType">Type of the cache.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void CacheBind(ComboBox comboBox, CacheType cacheType)
		{
			StringCollection cacheList = RecentCache.GetRecentList(cacheType);
			comboBox.Items.Clear();
			for (int i = cacheList.Count - 1; i >= 0; i--)
				comboBox.Items.Add(cacheList[i]);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Write information to the registry.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void WriteRegistry()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Info.REG_MAIN_HIVE, true) ??
									 Registry.CurrentUser.CreateSubKey(Info.REG_MAIN_HIVE))
			{
				if (key == null)
					return;
				key.SetValue("txtSearchPattern", txtSearchPattern.Text);
				key.SetValue("txtReplacePattern", txtReplacePattern.Text);
				key.SetValue("chkECMAScript", chkECMAScript.Checked);
				key.SetValue("chkIgnoreCase", chkIgnoreCase.Checked);
				key.SetValue("chkMultipleLine", chkMultipleLine.Checked);
				key.SetValue("chkSingleLine", chkSingleLine.Checked);
				key.SetValue("chkIncludeSubFolders", chkIncludeSubFolders.Checked);
				key.SetValue("updContextLines", updContextLines.Value);
				key.SetValue("cboSearchScope", cboSearchScope.Text);
				key.SetValue("cboFileTypes", cboFileTypes.Text);

				key.SetValue("Height", !btnReplaceAll.Visible ? Height : Height - ReplacePartHeight);
				key.SetValue("Width", Width);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Read information from the registry.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void ReadRegistry()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Info.REG_MAIN_HIVE, false) ??
									 Registry.CurrentUser.CreateSubKey(Info.REG_MAIN_HIVE))
			{
				if (key == null)
					return;
				txtSearchPattern.Text = key.GetValue("txtSearchPattern", "").ToString();
				txtReplacePattern.Text = key.GetValue("txtReplacePattern", "").ToString();
				chkECMAScript.Checked = Convert.ToBoolean(key.GetValue("chkECMAScript", false));
				chkIgnoreCase.Checked = Convert.ToBoolean(key.GetValue("chkIgnoreCase", false));
				chkMultipleLine.Checked = Convert.ToBoolean(key.GetValue("chkMultipleLine", false));
				chkSingleLine.Checked = Convert.ToBoolean(key.GetValue("chkSingleLine", false));
				chkIncludeSubFolders.Checked = Convert.ToBoolean(key.GetValue("chkIncludeSubFolders", false));
				updContextLines.Value = Convert.ToInt32(key.GetValue("updContextLines", 0));

				cboSearchScope.SelectedText = key.GetValue("cboSearchScope", "").ToString();
				if (cboSearchScope.SelectedIndex < 0 && cboSearchScope.Items.Count > 0)
					cboSearchScope.SelectedIndex = 0;

				cboFileTypes.Text = key.GetValue("cboFileTypes", "").ToString();
				if (cboFileTypes.SelectedIndex < 0 && cboFileTypes.Items.Count > 0)
					cboFileTypes.SelectedIndex = 0;

				Height = Convert.ToInt32(key.GetValue("Height", _minimumSize.Height));
				Width = Convert.ToInt32(key.GetValue("Width", _minimumSize.Width));
			}
		}

		#endregion
	}
}