using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using SokoolTools.VsTools.FindAndReplace.Common;
using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// FilesSearcher
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class StaticFilesSearcher : Searcher
	{
		//private readonly OutputWindowPaneWrapper _oWp;
		private readonly StringCollection _replacedFileList = new StringCollection();
		private string _fileTypes = String.Empty;
		private bool _isReplaceNext;
		private int _searchFileCount;
		private string _searchScopePath = String.Empty;
		private WindowSearcher _windowSearcher;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="StaticFilesSearcher"/> class.
		/// </summary>
		/// <param name="dte">The DTE.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public StaticFilesSearcher(_DTE dte)
			: base(dte)
		{
			//if (dte != null)
			//	_oWp = new OutputWindowPaneWrapper(dte, Info.APP_NAME);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this instance is find in window.
		/// </summary>
		/// <value><c>true</c> if this instance is find in window; otherwise, <c>false</c>.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public bool IsFindInWindow { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets a value indicating whether this instance is replace next.
		/// </summary>
		/// <value><c>true</c> if this instance is replace next; otherwise, <c>false</c>.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public bool IsReplaceNext
		{
			set => _isReplaceNext = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match info list.
		/// </summary>
		/// <value>The match info list.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public ArrayList MatchInfoList { get; } = new ArrayList();

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the file search pattern.
		/// </summary>
		/// <value>The file search pattern.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public string FileTypes
		{
			set => _fileTypes = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether [search in sub dir].
		/// </summary>
		/// <value><c>true</c> if [search in sub dir]; otherwise, <c>false</c>.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public bool IsSearchInSubDir { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the back lines.
		/// </summary>
		/// <value>The back lines.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int BackLines { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the front lines.
		/// </summary>
		/// <value>The front lines.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int FrontLines { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches this instance.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void Search()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_searchFileCount = 0;
			GetSearchScopePath();
			SetFileTypes();
			if (IsFindInWindow)
			{
				Trace.Assert(_windowSearcher != null);
				if (_windowSearcher != null)
				{
					_windowSearcher.Pattern = Pattern;
					_windowSearcher.RegexOptions = RegexOptions;
					if (!_windowSearcher.FindNextInSearchInFiles())
						ReplaceNextInFiles(_searchScopePath, _fileTypes, String.Empty, true);
				}
				IsFindInWindow = false;
			}
			else
			{
				MatchInfoList.Clear();
				switch (SearchScope)
				{
					case Info.SEARCH_SCOPE_CURRENT_PROJECT:
						SearchCurrentProject(_fileTypes);
						break;
					case Info.SEARCH_SCOPE_ENTIRE_SOLUTION:
						SearchEntireSolution(_fileTypes);
						break;
					case Info.SEARCH_SCOPE_CURRENT_DOCUMENT:
						SearchInOneFile(_searchScopePath);
						break;
					case Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS:
						SearchInOpenFiles(_fileTypes);
						break;
					default:
						SearchInFiles(_searchScopePath, _fileTypes);
						break;
				}
				ShowResults(false);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches all the files in each project of the entire solution.
		/// </summary>
		/// <param name="fileTypes">The file types.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchEntireSolution(string fileTypes)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			foreach (Project project in Connect.DteService.Solution.Projects)
			{
				if (project.Kind != EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
					SearchProjectFiles(project.ProjectItems, fileTypes);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches all the files in the entire project.
		/// </summary>
		/// <param name="fileTypes">The file types.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchCurrentProject(string fileTypes)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Project currentProject = GetCurrentProject();
			if (currentProject != null)
				SearchProjectFiles(currentProject.ProjectItems, fileTypes);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches all the files in the entire project.
		/// </summary>
		/// <param name="projectItems">The project items.</param>
		/// <param name="fileTypes">The file types to limit the search to.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchProjectFiles(ProjectItems projectItems, string fileTypes)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (projectItems == null)
				return;

			foreach (ProjectItem projectItem in projectItems)
			{
				if (projectItem.FileCount == 0)
					continue;

				string fileName = null;
				try
				{
					fileName = projectItem.FileNames[0];
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				if (String.IsNullOrEmpty(fileName))
					continue;

				if (fileTypes == "*.*" || (fileTypes + ";").Contains(Path.GetExtension(fileName) + ";"))
					SearchInOneFile(fileName);

				//// If supports cancelation, cancel work.
				//if (backgroundWorker1.CancellationPending)
				//{
				//    e.Cancel = true;
				//    return;
				//}

				if (projectItem.SubProject != null)
					SearchProjectFiles(projectItem.SubProject.ProjectItems, fileTypes);
				else if (projectItem.ProjectItems.Count > 0)
					SearchProjectFiles(projectItem.ProjectItems, fileTypes);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches all the files in the specified directory and optionally its sub-directories belonging to the specified file
		/// types.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="fileTypes">The file types to search on.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchInFiles(string dirPath, string fileTypes)
		{
			string[] fileTypesList = fileTypes.Split(';');
			var dirInfo = new DirectoryInfo(dirPath);
			var myfileinfos = new ArrayList();
			foreach (string fileTypesItem in fileTypesList)
			{
				myfileinfos.AddRange(dirInfo.GetFiles(fileTypesItem,
					IsSearchInSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			}
			var fileList = (FileInfo[])myfileinfos.ToArray(typeof(FileInfo));
			var query = from fi in fileList orderby fi.FullName select new { fi.FullName };
			foreach (var v in query)
			{
				// TODO: Detect cancel here
				SearchInOneFile(v.FullName);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches all open files belonging to the specified file types.
		/// </summary>
		/// <param name="fileTypes">The file types.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchInOpenFiles(string fileTypes)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_ = fileTypes.Split(';');
			foreach (Document document in Connect.DteService.Documents)
			{
				// TODO: Detect cancel here
				SearchInOneFile(document.FullName);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the in one file.
		/// </summary>
		/// <param name="fullFilename">The full filename.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void SearchInOneFile(string fullFilename)
		{
			try
			{
				string source;
				using (var sr = new StreamReader(fullFilename)) //, System.Text.Encoding.GetEncoding("utf-8")
					source = sr.ReadToEnd();
				_searchFileCount++;
				foreach (Match match in Regex.Matches(source, Pattern, GetRegexOptions()))
				{
					int lineNum = StringHelper.GetLineCount(ref source, match.Index);
					var matchInfo = new MatchInfo(fullFilename, lineNum, 0, match.Value,
						StringHelper.GetStringContext(ref source, match.Index, match.Index + match.Length,
							FrontLines, BackLines));
					MatchInfoList.Add(matchInfo);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the string in the files of the specified directory path based on the replacement pattern.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="fileTypes">The file search pattern.</param>
		/// <param name="replacement">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceInFiles(string dirPath, string fileTypes, string replacement)
		{
			string[] fileTypesList = fileTypes.Split(';');
			var dirInfo = new DirectoryInfo(Path.GetDirectoryName(dirPath) ?? "");
			var myfileinfos = new ArrayList();
			foreach (string fileTypesItem in fileTypesList)
			{
				myfileinfos.AddRange(dirInfo.GetFiles(fileTypesItem,
					IsSearchInSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			}
			var fileList = (FileInfo[])myfileinfos.ToArray(typeof(FileInfo));
			var query = from fi in fileList orderby fi.FullName select new { fi.FullName };

			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			foreach (var v in query)
				ReplaceInOneFile(v.FullName, replacement);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces all occurences of the .
		/// </summary>
		/// <param name="replacement">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceAll(string replacement)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			GetSearchScopePath();
			SetFileTypes();
			MatchInfoList.Clear();
			ReplaceInFiles(_searchScopePath, _fileTypes, replacement);
			ShowResults(true);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Records the replaced information.
		/// </summary>
		/// <param name="source">The input source.</param>
		/// <param name="replacedAllSource">The replaced all source.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <param name="fullFilename">The full filename.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void RecordReplacedInformation(string source, string replacedAllSource, string replacement,
											   string fullFilename)
		{
			string input = source;
			int startPos = 0;
			while (true)
			{
				var reg = new Regex(Pattern, GetRegexOptions());
				Match match = reg.Match(input, startPos);
				if (match.Success)
				{
					string changedSource = reg.Replace(input, replacement, 1, startPos);
					startPos = match.Index + match.Length;
					int replaceTextStartPos = match.Index; // position in changedSource
					int replaceTextEndPos = changedSource.Length - (input.Length - startPos); // position in changedSource
					string replaceText = changedSource.Substring(replaceTextStartPos, replaceTextEndPos - replaceTextStartPos);

					int lineNum = StringHelper.GetLineCount(ref replacedAllSource, replaceTextStartPos) + 1;
					var matchInfo = new MatchInfo(fullFilename, lineNum, 0, replaceText,
						StringHelper.GetStringContext(ref replacedAllSource, replaceTextStartPos,
							replaceTextEndPos, FrontLines, BackLines));
					MatchInfoList.Add(matchInfo);
					input = changedSource;
				}
				else
					break;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces in one file.
		/// </summary>
		/// <param name="fullFilename">The full filename.</param>
		/// <param name="replacement">The replace string.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private bool ReplaceInOneFile(string fullFilename, string replacement)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			string source;
			using (var sr = new StreamReader(fullFilename)) //, System.Text.Encoding.GetEncoding("utf-8")
				source = sr.ReadToEnd();
			string replacedAllSource = ReplaceAll(source, replacement, out int _);
			if (replacedAllSource.Length <= 0) return false;
			if (_isReplaceNext)
			{
				Dte.ItemOperations.OpenFile(fullFilename, Constants.vsViewKindTextView);
				return true;
			}
			RecordReplacedInformation(source, replacedAllSource, replacement, fullFilename);
			using (var sw = new StreamWriter(fullFilename, false, Encoding.Default))
				sw.Write(replacedAllSource);
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces in the next file.
		/// </summary>
		/// <param name="replacement">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceNext(string replacement)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			GetSearchScopePath();
			SetFileTypes();
			if (IsFindInWindow)
			{
				Trace.Assert(_windowSearcher != null);
				if (_windowSearcher == null) 
					return;
				_windowSearcher.Pattern = Pattern;
				_windowSearcher.RegexOptions = RegexOptions;
				if (!_windowSearcher.ReplaceNextInSearchInFiles(replacement))
					ReplaceNextInFiles(_searchScopePath, _fileTypes, replacement, true);
			}
			else
				ReplaceNextInFiles(_searchScopePath, _fileTypes, replacement, true);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces in the next series of files.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="fileTypes">The file search pattern.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <param name="isRoot">if set to <c>true</c> [is root].</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public bool ReplaceNextInFiles(string dirPath, string fileTypes, string replacement, bool isRoot)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			string[] fileTypesList = fileTypes.Split(';');
			foreach (string fileTypesItem in fileTypesList)
			{
				foreach (string fullFilename in
					Directory.GetFiles(dirPath, fileTypesItem).Where(fullFilename => !_replacedFileList.Contains(fullFilename)))
				{
					_replacedFileList.Add(fullFilename);
					if (!ReplaceInOneFile(fullFilename, replacement)) continue;
					IsFindInWindow = true;
					CheckWindowSearcher();
					Trace.Assert(_windowSearcher.ReplaceNextInSearchInFiles(replacement));
					return true;
				}
			}
			if (IsSearchInSubDir)
			{
				if (fileTypesList.Any(fileTypesItem => Directory.GetDirectories(dirPath, fileTypesItem).Any(
					dirFullName => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return ReplaceNextInFiles(dirFullName, fileTypes, replacement, false); })))
				{
					return true;
				}
			}
			if (isRoot)
				UIHelper.ShowInfo(Info.UI_PROMPT_FIND_TO_END);
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the results.
		/// </summary>
		/// <param name="isReplace">if set to <c>true</c> [is replace].</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void ShowResults(bool isReplace)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

			string title1;
			string title2;
			OutputPane.Clear();

			if (isReplace)
			{
				title1 = "Replace all";
				title2 = "Replaced lines:";
			}
			else
			{
				title1 = "Find all";
				title2 = "Matching lines:";
			}

			OutputPane.WriteLine(SearchScope == Info.SEARCH_SCOPE_CURRENT_DOCUMENT
				? $"{title1} \"{Pattern}\", in {SearchScope}"
				: $"{title1} \"{Pattern}\", in {SearchScope}{(IsSearchInSubDir ? ", Subfolders" : "")}, \"{(SearchScope == Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS ? "" : _fileTypes)}\"");

			int matchingFileCount = 0;
			int matchingLineCount = MatchInfoList.Count;
			string prevFullFileName = String.Empty;
			int prevStartLine = 0;
			foreach (MatchInfo matchInfo in MatchInfoList)
			{
				if (matchInfo.FullFilename + matchInfo.StartLine != prevFullFileName + prevStartLine)
				{
					string showInfo = StringHelper.IsMultipleLine(matchInfo.MatchContext)
						? String.Format("{4}{3}{0}({1}):{3}{5}{3}{2}{3}", matchInfo.FullFilename, matchInfo.StartLine,
							matchInfo.MatchContext, Environment.NewLine, new string('_', 132),
							new string('¯', 132))
						: $"{matchInfo.FullFilename}({matchInfo.StartLine}):   {matchInfo.MatchContext.TrimStart('\t', ' ')}";
					if (showInfo.EndsWith("\r") || showInfo.EndsWith("\n"))
						OutputPane.Write(showInfo);
					else
						OutputPane.WriteLine(showInfo);
					if (matchInfo.FullFilename != prevFullFileName)
					{
						matchingFileCount++;
						prevFullFileName = matchInfo.FullFilename;
					}
					prevStartLine = matchInfo.StartLine;
				}
				else
					matchingLineCount--;
			}
			OutputPane.WriteLine(
				$"{title2} {matchingLineCount}, Matching files: {matchingFileCount}, Total files searched: {_searchFileCount}");
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the search scope path.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void GetSearchScopePath()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			switch (SearchScope)
			{
				// TODO: accomodate Current Project and Solution different from full path.
				case Info.SEARCH_SCOPE_CURRENT_PROJECT:
					if (Dte.ActiveSolutionProjects is Array activeProjects && activeProjects.Length > 0)
					{
						if (activeProjects.GetValue(0) is Project currentProject)
							_searchScopePath = Path.GetDirectoryName(currentProject.FullName);
						return;
					}
					break;
				case Info.SEARCH_SCOPE_ENTIRE_SOLUTION:
					_searchScopePath = Path.GetDirectoryName(Dte.Solution.FullName);
					return;
				case Info.SEARCH_SCOPE_CURRENT_DOCUMENT:
					_searchScopePath = Dte.ActiveDocument.FullName;
					return;
				case Info.SEARCH_SCOPE_ALL_OPEN_DOCUMENTS:
					_searchScopePath = Dte.ActiveDocument.FullName;
					return;
			}
			if (Directory.Exists(SearchScope))
				_searchScopePath = SearchScope;
			else
				throw new ApplicationException(String.Format("The path does not exist, {0}", SearchScope));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the file search pattern when none has been specified.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void SetFileTypes()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (_fileTypes.Length == 0)
			{
				switch (SearchScope)
				{
					case Info.SEARCH_SCOPE_CURRENT_PROJECT:
					case Info.SEARCH_SCOPE_ENTIRE_SOLUTION:
						_fileTypes = Info.DEFAULT_FILE_SEARCH_PATTERN;
						break;
					default:
						UIHelper.ShowInfo("Please input the file type");
						break;
				}
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the current project.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private Project GetCurrentProject()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (!(Dte.ActiveSolutionProjects is Array activeProjects) || activeProjects.Length == 0)
				return null;
			return activeProjects.GetValue(0) as Project;
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
		/// Initializes the window searcher.
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
	}
}