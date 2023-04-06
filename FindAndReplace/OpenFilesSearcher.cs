using System;
using System.Collections.Specialized;
using EnvDTE;
using SokoolTools.VsTools.FindAndReplace.Common;
using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// OpenFilesSearcher
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class OpenFilesSearcher : Searcher
	{
		private readonly StringCollection _unhandledOpenFileList = new StringCollection();
		private string _activeOpenFullFilename = String.Empty;
		private WindowSearcher _windowSearcher;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenFilesSearcher"/> class.
		/// </summary>
		/// <param name="dte">The DTE.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public OpenFilesSearcher(_DTE dte) : base(dte)
		{
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next in open files.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void FindNextInOpenFiles()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            while (true)
			{
				if (_unhandledOpenFileList.Count > 0)
				{
					CheckWindowSearcher();
					if (_activeOpenFullFilename != _unhandledOpenFileList[0])
					{
						Dte.ItemOperations.OpenFile(_unhandledOpenFileList[0], Constants.vsViewKindTextView);
						_activeOpenFullFilename = _unhandledOpenFileList[0];
						_windowSearcher.MoveToStartInTextWindow();
					}

					if (!_windowSearcher.FindNextInSearchInFiles())
					{
						_unhandledOpenFileList.RemoveAt(0);
						continue;
					}
				}
				else
					UIHelper.ShowInfo(Info.UI_PROMPT_FIND_TO_END);
				break;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void FindNext()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            GetSearchScopePath();
			if (_unhandledOpenFileList.Count > 0)
				FindNextInOpenFiles();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Checks the window searcher.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CheckWindowSearcher()
		{
			if (_windowSearcher == null)
			{
				InitWindowSearcher();
				return;
			}
			if (!(_windowSearcher.Pattern == Pattern && _windowSearcher.RegexOptions == RegexOptions))
				InitWindowSearcher();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Inits the window searcher.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void InitWindowSearcher()
		{
			_windowSearcher = new WindowSearcher(Dte)
			{
				Pattern = Pattern,
				RegexOptions = RegexOptions
			};
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces all in open files.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void ReplaceAllInOpenFiles(string replacePattern)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (_unhandledOpenFileList.Count == 0) return;
			int replaceCount = 0;
			CheckWindowSearcher();
			foreach (string fullFilename in _unhandledOpenFileList)
			{
				Dte.ItemOperations.OpenFile(fullFilename, Constants.vsViewKindTextView);
				replaceCount += _windowSearcher.ReplaceAll(replacePattern, true);
			}
			UIHelper.ShowInfo(replaceCount + " replacements made");
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the next in open files.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void ReplaceNextInOpenFiles(string replacePattern)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            while (true)
			{
				if (_unhandledOpenFileList.Count > 0)
				{
					CheckWindowSearcher();
					if (_activeOpenFullFilename != _unhandledOpenFileList[0])
					{
						Dte.ItemOperations.OpenFile(_unhandledOpenFileList[0], Constants.vsViewKindTextView);
						_activeOpenFullFilename = _unhandledOpenFileList[0];
						_windowSearcher.MoveToStartInTextWindow();
					}
					if (_windowSearcher.ReplaceNextInSearchInFiles(replacePattern)) return;
					_unhandledOpenFileList.RemoveAt(0);
					continue;
				}
				UIHelper.ShowInfo(Info.UI_PROMPT_FIND_TO_END);
				break;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the next.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceNext(string replacePattern)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            GetSearchScopePath();
			if (_unhandledOpenFileList.Count == 0) return;
			ReplaceNextInOpenFiles(replacePattern);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the search scope path.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void GetSearchScopePath()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            //if (_unhandledOpenFileList.Count == 0) return;
            foreach (Document document in Dte.Documents)
				_unhandledOpenFileList.Add(document.FullName);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces all.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceAll(string replacePattern)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            GetSearchScopePath();
			if (_unhandledOpenFileList.Count > 0)
				ReplaceAllInOpenFiles(replacePattern);
		}
	}
}