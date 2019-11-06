using System;
using System.Text.RegularExpressions;
using EnvDTE;
using SokoolTools.VsTools.FindAndReplace.FindState;
using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// WindowSearcher.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class WindowSearcher : Searcher
	{
		private int _matchedEndPoint;
		private int _matchedStartPoint;
		private int _nextMatchTextEndLine;
		private int _nextMatchTextEndLineCharOffset;
		private int _nextMatchTextStartLine;
		private int _nextMatchTextStartLineCharOffset;
		private int _nextSearchStartPos;
		private EditPoint _searchLatestEditPoint;
		private EditPoint _searchStartEditPoint;
		private IFindState _state;
		private string _windowSource;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="WindowSearcher"/> class.
		/// </summary>
		/// <param name="dte">The DTE.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public WindowSearcher(_DTE dte)
			: base(dte)
		{
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the regular expression pattern to match.
		/// </summary>
		/// <value>The pattern.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public override string Pattern
		{
			get => base.Pattern;
			set
			{
				if (base.Pattern == value)
					return;
				base.Pattern = value;
				_state = new FirstFindState();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the window source.
		/// </summary>
		/// <value>The window source.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public string WindowSource
		{
			set => _windowSource = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the position where to begin the next search.
		/// </summary>
		/// <value>The next search start position.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int NextSearchStartPos
		{
			set => _nextSearchStartPos = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the regular expression options.
		/// </summary>
		/// <param name="isSingleLine">if set to <c>true</c> [is single line].</param>
		/// <param name="isMultipleLine">if set to <c>true</c> [is multiple line].</param>
		/// <param name="isIgnoreCase">if set to <c>true</c> [is ignore case].</param>
		/// <param name="isECMAScript">if set to <c>true</c> [is ECMA script].</param>
		//------------------------------------------------------------------------------------------------------------------------
		public override void SetRegexOptions(bool isSingleLine, bool isMultipleLine, bool isIgnoreCase, bool isECMAScript)
		{
			var regexOptions = RegexOptions.None;
			if (isSingleLine)
				regexOptions |= RegexOptions.Singleline;
			if (isMultipleLine)
				regexOptions |= RegexOptions.Multiline;
			if (isIgnoreCase)
				regexOptions |= RegexOptions.IgnoreCase;
			if (isECMAScript)
				regexOptions |= RegexOptions.ECMAScript;
			if (RegexOptions == regexOptions) return;
			RegexOptions = regexOptions;
			_state = new FirstFindState();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the state.
		/// </summary>
		/// <param name="newState">The new state.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void SetState(IFindState newState)
		{
			_state = newState;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void FindNext()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetLatestFindStartPos();
			_state.FindNext(this);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the search start point.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void SetSearchStartPoint()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (Dte.ActiveDocument.Selection is TextSelection textSelection)
				_searchStartEditPoint = textSelection.ActivePoint.CreateEditPoint();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Moves to start in text window.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void MoveToStartInTextWindow()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextSelection textSelection = GetTextSelection();
			textSelection.MoveToPoint(GetTextDocument().StartPoint);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match info.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public MatchInfo GetMatch()
		{
			var rgx = new Regex(Pattern, GetRegexOptions());
			Match match = rgx.Match(_windowSource, _nextSearchStartPos);
			if (!match.Success)
				return null;
			int startLine = StringHelper.GetLineCount(ref _windowSource, match.Index);
			int startLineOffset = StringHelper.GetPosInLine(ref _windowSource, match.Index - 1);
			int endLine = StringHelper.GetLineCount(ref _windowSource, match.Index + match.Length - 1);
			int endLineOffset = StringHelper.GetPosInLine(ref _windowSource, match.Index + match.Length - 1);
			return new MatchInfo(startLine, endLine, match.Value, startLineOffset, match.Index, match.Length, endLineOffset);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the in text window once.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public bool SearchInTextWindowOnce()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextSelection textSelection = GetTextSelection();
			int activePointLine = textSelection.ActivePoint.Line;
			int activePointLineCharOffset = textSelection.ActivePoint.LineCharOffset;
			_windowSource = textSelection.ActivePoint.CreateEditPoint().GetText(GetTextDocument().EndPoint);
			MatchInfo match = GetMatch();

			if (!(match?.MatchContent.Length > 0))
				return false;

			if (match.StartLine == 1 && match.EndLine == match.StartLine)
			{
				_nextMatchTextStartLine = activePointLine;
				_nextMatchTextStartLineCharOffset = activePointLineCharOffset + match.StartLineOffset;
				_nextMatchTextEndLine = activePointLine + match.EndLine - 1;
				_nextMatchTextEndLineCharOffset = activePointLineCharOffset + match.EndLineOffset;
			}
			else
			{
				_nextMatchTextStartLine = activePointLine + match.StartLine - 1;
				_nextMatchTextStartLineCharOffset = match.StartLineOffset + 1;
				_nextMatchTextEndLine = activePointLine + match.EndLine - 1;
				_nextMatchTextEndLineCharOffset = match.EndLineOffset + 1;
			}
			return true;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Selects the matched text.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void SelectMatchedText()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextSelection textSelection = GetTextSelection();
			textSelection.MoveToLineAndOffset(_nextMatchTextStartLine, _nextMatchTextStartLineCharOffset);
			textSelection.MoveToLineAndOffset(_nextMatchTextEndLine, _nextMatchTextEndLineCharOffset, true);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether to go across start point.
		/// </summary>
		/// <returns><c>true</c> to go across start point; otherwise, <c>false</c>.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public bool IsGoAcrossStartPoint()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            //TextSelection textSelection = _dte.ActiveDocument.Selection as TextSelection;
            return _nextMatchTextStartLine > _searchStartEditPoint.Line ||
				   _nextMatchTextStartLine == _searchStartEditPoint.Line &&
				   _nextMatchTextStartLineCharOffset >= _searchStartEditPoint.LineCharOffset;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the next occurence.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ReplaceNext(string replacePattern)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (_searchLatestEditPoint == null)
			{
				SetLatestFindStartPos();
				FindNext();
			}
			else
			{
				TryToReplaceSelectedText(replacePattern);
				FindNext();
			}
			SaveMatchedPosition();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the matched position.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void SaveMatchedPosition()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextSelection textSelection = GetTextSelection();
			_matchedStartPoint = textSelection.AnchorPoint.AbsoluteCharOffset;
			_matchedEndPoint = textSelection.ActivePoint.AbsoluteCharOffset;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces all occurences of the text.
		/// </summary>
		/// <param name="replacement">The replacement pattern.</param>
		/// <param name="isReplaceInOpenFiles">if set to <c>true</c> [is replace in open files].</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public int ReplaceAll(string replacement, bool isReplaceInOpenFiles)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            SetSearchStartPoint();
			MoveToStartInTextWindow();
			TextSelection textSelection = GetTextSelection();
			TextDocument textDocument = GetTextDocument();

			string wholeWindowText = textSelection.ActivePoint.CreateEditPoint().GetText(GetTextDocument().EndPoint);

			EditPoint editPoint = textSelection.ActivePoint.CreateEditPoint();
			TextPoint endPoint = textDocument.EndPoint.CreateEditPoint();
			const int VS_EP_REPLACE_TEXT_AUTOFORMAT = 3; // vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
			ReplaceAll(wholeWindowText, replacement, out int repeatCount);
			if (repeatCount > 0)
			{
				editPoint.ReplaceText(endPoint, Replace(wholeWindowText, replacement), VS_EP_REPLACE_TEXT_AUTOFORMAT);
				if (!isReplaceInOpenFiles)
					UIHelper.ShowInfo(repeatCount + " replacements made");
			}
			textSelection.MoveToPoint(_searchStartEditPoint);
			return repeatCount;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the text selection.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private TextSelection GetTextSelection()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextDocument textDocument = GetTextDocument();
			if (textDocument != null)
				return Dte.ActiveDocument.Selection as TextSelection;
			return null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the text document.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private TextDocument GetTextDocument()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextDocument activeDocument = null;
			try
			{
				activeDocument = Dte.ActiveDocument.Object(string.Empty) as TextDocument;
				Dte.ActiveDocument.Activate();
			}
			catch
			{
			}

			if (activeDocument == null)
				throw new ApplicationException("There is not an active document.");
			return activeDocument;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Tries to replace the selected text with the specified replacement pattern.
		/// </summary>
		/// <param name="replacePattern">The replacement pattern.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private void TryToReplaceSelectedText(string replacePattern)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextSelection textSelection = GetTextSelection();
			if (_matchedStartPoint == textSelection.AnchorPoint.AbsoluteCharOffset &&
				_matchedEndPoint == textSelection.ActivePoint.AbsoluteCharOffset)
				textSelection.Text = GetReplaceText(_searchLatestEditPoint.GetText(GetTextDocument().EndPoint), replacePattern);
			SetLatestFindStartPos();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the next occurence of the searched text with the specified replacement text in a file.
		/// </summary>
		/// <param name="replacePattern">The replace pattern.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public bool ReplaceNextInSearchInFiles(string replacePattern)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (_searchLatestEditPoint == null)
			{
				SetLatestFindStartPosToStartOfDoc();
				return FindNextInSearchInFiles();
			}
			TryToReplaceSelectedText(replacePattern);
			return FindNextInSearchInFiles();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next occurence of the search text in the file.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public bool FindNextInSearchInFiles()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetLatestFindStartPos();
			if (SearchInTextWindowOnce())
			{
				SelectMatchedText();
				SaveMatchedPosition();
				return true;
			}
			ResetLatestFindStartPos();
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the latest start position.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void SetLatestFindStartPos()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextDocument textDocument = GetTextDocument();
			if (textDocument == null)
				return;
			TextSelection textSelection = GetTextSelection();
			_searchLatestEditPoint = textSelection.ActivePoint.CreateEditPoint();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Resets the latest find start pos.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void ResetLatestFindStartPos()
		{
			_searchLatestEditPoint = null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the latest find start pos to start of doc.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void SetLatestFindStartPosToStartOfDoc()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            GetTextSelection().StartOfDocument();
			_searchLatestEditPoint = GetTextSelection().ActivePoint.CreateEditPoint();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the selection text.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public string GetSelectionText()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string selectionText = string.Empty;
			try
			{
				TextSelection textSelection = GetTextSelection();
				selectionText = textSelection.Text;
			}
			catch (ApplicationException)
			{
			}
			return selectionText;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the replace text.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="replacePattern">The replace pattern.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private string GetReplaceText(string source, string replacePattern)
		{
			string replacingSource = source;
			var rgx = new Regex(Pattern, GetRegexOptions());
			Match match = rgx.Match(replacingSource);
			if (!match.Success)
				return string.Empty;
			string changedSource = rgx.Replace(replacingSource, replacePattern, 1);
			int replaceTextStartPos = match.Index; //position in changedSource
			int replaceTextEndPos = changedSource.Length - (replacingSource.Length - match.Index - match.Length);
			//position in changedSource
			return changedSource.Substring(replaceTextStartPos, replaceTextEndPos - replaceTextStartPos);
		}
	}
}