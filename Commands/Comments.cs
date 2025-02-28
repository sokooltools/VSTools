using System;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for processing 'Comments' in Visual Studio.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class Comments
	{
		private static int _tabSize;
		private static bool _isCommentsIndented;
		private static bool _isRightAligned;
		private static bool _isNamespaced;
		private static char _commentDividerLineChar;
		private static int _commentDividerLineRepeat;

		public static void FormatCommentsInAllFiles()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

			Logging.Log(2);

			// TODO: Add body here 
			FormatAllFiles.Execute();
			//SolutionStuff.ShowSolutionProperties(); // <-- Just for testing!
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Formats the Comments.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void FormatComments()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			
			Logging.Log(2);

			Document doc = Connect.DteService.ActiveWindow.Document;

			if (!(doc?.Selection is TextSelection sel))
				return;

			// Hold onto the current line number.
			int iLine = sel.AnchorPoint.Line;
			int iLineCharOffset = sel.AnchorPoint.LineCharOffset;

			sel.SelectAll();

			// Need to change to insert tabs before doing smart formatting.
			bool bInsertTabs =
				Convert.ToBoolean(Connect.DteService.DTE.Properties["TextEditor", "CSharp"].Item("InsertTabs").Value);

			if (!bInsertTabs)
				Connect.DteService.DTE.Properties["TextEditor", "CSharp"].Item("InsertTabs").Value = true;

			// Always start out by formatting the entire text.
			sel.SmartFormat();

			sel.Tabify();

			string text = sel.Text;

			_commentDividerLineChar = OptionsHelper.CommentDividerLineChar;
			_commentDividerLineRepeat = Math.Max(2, OptionsHelper.CommentDividerLineRepeat);
			_isCommentsIndented = OptionsHelper.IsCommentIndented;
			_isRightAligned = OptionsHelper.IsCommentDividerLineRightAligned;

			// Get the actual tab size used for this document type.
			_tabSize = Convert.ToInt32(Connect.DteService.DTE.Properties["TextEditor", "CSharp"].Item("TabSize").Value);

			// Determine if code is surrounded by a namespace.
			_isNamespaced = Regex.IsMatch(text, @"^[\s]*namespace ", RegexOptions.Multiline | RegexOptions.IgnoreCase);

			// Remove carriage-returns leaving just linefeeds.
			text = text.Replace("\r", String.Empty);

			text = GetWrappedInternalText(text);

			text = GetWrappedCommentText(text);

			text = GetWrappedCommentTextForTags(text);

			text = Dividers.RemoveRegionDividerLines(text);

			if (OptionsHelper.IsRegionDividerLinesInserted)
				text = AddRegionDividerLines(text);

			text = Dividers.RemoveCommentDividerLines(text);

			text = AddTopMiniDividerLines(text);

			text = AddAttributeDividerLines(text);

			text = AddResharperDividerLines(text);

			text = AddBottomMiniDividerLines(text);

			//text = Dividers.ReplaceDoubleCommentDividerLines(text);

			text = text.Replace("¤¤", "--");

			text = AdjustLengthOfDividerLines(text);

			text = RemoveTabsAndSpacesFromBlankLines(text);

			text = RemoveExtraBlankLinesPrecedingCommentLines(text);

			// Replace linefeeds with carriage-return/linefeeds.
			text = text.Replace("\n", "\r\n");

			if (!bInsertTabs)
				text = ReplaceTabsWithSpaces(text);

			text = RemoveMultipleLinesFollowingLeftBracket(text);

			text = RemoveErrantSummaryTagDividerLines(text);

			// Insert the newly formatted text back into the selection.
			sel.Insert(text, (int)vsInsertFlags.vsInsertFlagsContainNewText);

			if (sel.BottomLine >= iLine)
			{
				// Go to the original line number and offset.
				sel.MoveToLineAndOffset(iLine, iLineCharOffset);
				sel.AnchorPoint.TryToShow();
				sel.CharRight();
				sel.CharLeft();
			}
			else
				sel.StartOfDocument();

			// //Try to find the specified text.
			// TextRanges tRng = null;
			// bool wasFound = sel.FindPattern(sLine, (int) vsFindOptions.vsFindOptionsRegularExpression, ref tRng);
			// sel.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, false);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the text internal to classes, methods, properties (i.e., the text not placed onto the header) after it has been 
		/// wrapped accordingly.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>Text with comments wrapped according to the maximum length.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetWrappedInternalText(string text)
		{
			Logging.Log(3);

			string sIndent = " ";

			if (_isCommentsIndented)
				sIndent = "\t";

			//--------------------------------------------------------------------------------------------------------------------
			// ^[\s]*//\-[\-]+[\n]?
			// Beginning of line or string
			// Whitespace, any number of repetitions
			// 
			// -
			// -, one or more repetitions
			// New line, zero or one repetitions
			// [1]: A numbered capture group. [([\s]*)//[^/-][^\n]*], one or more repetitions
			// ([\s]*)//[^/-][^\n]*
			// [2]: A numbered capture group. [[\s]*]
			// Whitespace, any number of repetitions
			// 
			// Any character that is not in this class: [/-]
			// Any character other than New line, any number of repetitions
			// [\s]*//\-[\-]+[\n]?
			// Whitespace, any number of repetitions
			// 
			// -
			// -, one or more repetitions
			// New line, zero or one repetitions
			//--------------------------------------------------------------------------------------------------------------------
			string sPattern = String.Format(@"^[\s]*//\{0}[\{0}]+[\n]?(([\s]*)//[^/\{0}][^\n]*)+[\s]*//\{0}[\{0}]+[\n]?",
				_commentDividerLineChar);

			MatchCollection mc = Regex.Matches(text, sPattern, RegexOptions.Multiline | RegexOptions.Singleline);
			if (mc.Count == 0)
				return text;

			var sRetTxt = new StringBuilder(text);
			for (int j = mc.Count - 1; j >= 0; j--)
			{
				Match match = mc[j];
				string sMidTxt = match.Value;
				string sLinBeg = match.Groups[2].Value.Replace("\n", "");
				int iIdx = match.Index;
				int iLen = match.Length;

				string desc = "Remove any tabs, spaces and comment chars at the end of the match.";
				sMidTxt = MyRegex.Replace(sMidTxt, @"[/ \t]*$", "", RegexOptions.Singleline, desc).Trim();

				desc = "Remove any tabs, spaces and comment chars (///) at the beginning of each line.";
				sMidTxt = MyRegex.Replace(sMidTxt, @"^[ \t]*//[/ \t]*", "", RegexOptions.Multiline | RegexOptions.Singleline, desc);

				desc = "Remove the comment divider line off the end.";
				sMidTxt = MyRegex.Replace(sMidTxt, String.Format(@"\{0}[\{0}]+$", _commentDividerLineChar), "", RegexOptions.Multiline, desc);

				desc = "Concatenate the internal text by replacing 'one or more spaces followed by a linefeed' with a 'single space'.";
				sMidTxt = MyRegex.Replace(sMidTxt, @"[ ]+[\n]", " ", RegexOptions.Multiline, desc);

				desc = "Remove the comment divider line off the beginning.";
				sMidTxt = MyRegex.Replace(sMidTxt, String.Format(@"^\{0}[\{0}]+", _commentDividerLineChar), "", RegexOptions.Multiline, desc);

				// Get the wrapped comment text.
				int iAdjLineLength = GetAdjLineLength(sLinBeg, false);
				string sWrappedTxt = GetWrappedTxt(sMidTxt, iAdjLineLength);

				// Remove the last newline from the string.
				if (sWrappedTxt.EndsWith("\n"))
					sWrappedTxt = sWrappedTxt.Remove(sWrappedTxt.Length - 1, 1);

				desc = "Indent the beginning of each comment line using tabs and/or space.";
				sWrappedTxt = MyRegex.Replace(sWrappedTxt, "^", $"{sLinBeg}//{sIndent}", RegexOptions.Multiline, desc);

				// Remove the old text.
				sRetTxt = sRetTxt.Remove(iIdx, iLen);

				// Concatenate the wrapped text in between the two comment lines meta characters.
				sWrappedTxt = String.Format("\n{0}//{2}\n{1}\n{0}//{2}\n", sLinBeg, sWrappedTxt, "¤¤"); //<--10/31/06

				// Insert the new text.
				sRetTxt = sRetTxt.Insert(iIdx, sWrappedTxt);
			}
			return sRetTxt.ToString();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns comment text word-wrapped.
		/// </summary>
		/// <param name="text"></param>
		/// <returns>Text with comments wrapped according to the max length.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetWrappedCommentText(string text)
		{
			Logging.Log(3);

			// Process text between each tag independently.
			string[] aTags = { "summary", "param", "remarks", "exception", "returns", "example" };
			foreach (string t in aTags)
			{
				string sPattern = t == "example"
					? @"(^[ \t]*//[/]*[ \t]*)(<example>)(.*?)(</example>|<code>)" // TODO: example or code
					: String.Format(@"(^[ \t]*//[/]*[ \t]*)(<{0}[^/>]*>)(.*?)(</{0}>)", t);

				MatchCollection mc = Regex.Matches(text, sPattern,
					RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

				if (mc.Count <= 0)
					continue;

				var sRetTxt = new StringBuilder(text);

				for (int j = mc.Count - 1; j >= 0; j--)
				{
					Match match = mc[j];
					string sLinBeg = match.Groups[1].Value; // "\t///"
					string sTagBeg = match.Groups[2].Value; // "<summary>"
					string sMidTxt = match.Groups[3].Value; // "\n\t/// Provides methods for processing the current 'document' inside Visual Studio such as sorting..."
					string sTagEnd = match.Groups[4].Value; // "</summary>"
					int iIdx = match.Groups[0].Index;       // 32
					int iLen = match.Groups[0].Length;      // 428

					string desc = "Remove any tabs, spaces and comment chars at the end of a line.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"[/ \t]*$", "", RegexOptions.Singleline, desc).Trim();

					desc = "Remove any tabs, spaces and comment chars (///) at the beginning of each line.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"^[ \t]*//[/ \t]*", "", RegexOptions.Multiline | RegexOptions.Singleline, desc);

					desc = "Replace one or more spaces followed by a linefeed with a single space.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"[ ]+[\n]", " ", RegexOptions.Multiline, desc);

					// Get the wrapped comment text.
					int iAdjLineLength = GetAdjLineLength(sLinBeg, _isCommentsIndented) - 2; // 118
					string sWrappedTxt = GetWrappedTxt(sMidTxt, iAdjLineLength);

					if (_isCommentsIndented)
					{
						desc = "Prepend a tab to the beginning of each line when indented is indicated.";
						sWrappedTxt = MyRegex.Replace(sWrappedTxt, "^", "\t", RegexOptions.Multiline, desc);
					}

					desc = "Concatenate the wrapped text between the two tags.";
					sWrappedTxt = $"{sTagBeg}\n{sWrappedTxt}\n{sTagEnd}";
					sWrappedTxt = MyRegex.Replace(sWrappedTxt, "^", sLinBeg, RegexOptions.Multiline, desc);

					// Remove the current text.
					sRetTxt = sRetTxt.Remove(iIdx, iLen);

					// Insert the new text.
					sRetTxt = sRetTxt.Insert(iIdx, sWrappedTxt);
				}
				text = sRetTxt.ToString();
			}
			return text;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Attempts to fit each comment parameter and its tags onto a single line.
		/// </summary>
		/// <param name="text">The original text</param>
		/// <returns>Text with comments wrapped according to the max length.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetWrappedCommentTextForTags(string text)
		{
			Logging.Log(3);

			// Process text between each tag independently.
			string[] tagNames = { "param", "returns", "value", "exception" };
			foreach (string tagName in tagNames)
			{
				string sPattern = String.Format(@"(^[ \t]*//[/]*[ \t]*)(<{0}[^/>]*>)(.*?)(</{0}>)", tagName);

				MatchCollection mc = Regex.Matches(text, sPattern, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (mc.Count == 0)
					continue;

				var sRetTxt = new StringBuilder(text);
				for (int j = mc.Count - 1; j >= 0; j--)
				{
					Match match = mc[j];
					string sLinBeg = match.Groups[1].Value;
					string sTagBeg = match.Groups[2].Value;
					string sMidTxt = match.Groups[3].Value;
					string sTagEnd = match.Groups[4].Value;
					int iIdx = match.Groups[0].Index;
					int iLen = match.Groups[0].Length;

					string desc = "Remove any tabs, spaces and comment chars at the end of a line.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"[/ \t]*$", "", RegexOptions.Singleline, desc).Trim();

					desc = "Remove any tabs, spaces and comment chars (///) at the beginning of each line.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"^[ \t]*//[/ \t]*", "", RegexOptions.Multiline | RegexOptions.Singleline, desc);

					desc = "Replace one or more spaces followed by a linefeed with a single space.";
					sMidTxt = MyRegex.Replace(sMidTxt, @"[ ]+[\n]", " ", RegexOptions.Multiline, desc);

					// Concatenate the wrapped text between the two tags.
					string sWrappedTxt = $"{sTagBeg}{sMidTxt}{sTagEnd}";

					int iAdjLineLength = GetAdjLineLength(sLinBeg, _isCommentsIndented) - 1;

					if (sWrappedTxt.Length > iAdjLineLength) 
						continue;

					desc = "";
					sWrappedTxt = MyRegex.Replace(sWrappedTxt, "^", sLinBeg, RegexOptions.Multiline, desc);

					// Remove the current text.
					sRetTxt = sRetTxt.Remove(iIdx, iLen);
					// Insert the new text into its position.
					sRetTxt = sRetTxt.Insert(iIdx, sWrappedTxt);
				}
				text = sRetTxt.ToString();
			}
			return text;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns maximum line length after being adjusted because of several boolean conditions.
		/// </summary>
		/// <param name="sLinBeg"></param>
		/// <param name="isCommentsIndented"></param>
		/// <returns>Adjusted length value depending on indentation and other settings.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static int GetAdjLineLength(string sLinBeg, bool isCommentsIndented)
		{
			Logging.Log(3);

			int iMaxLineLength = _commentDividerLineRepeat;

			// Adjust maximum length when indented.
			if (isCommentsIndented)
				iMaxLineLength -= _tabSize;

			// Set the maximum length according to number of tabs in the beginning of the line.
			if (!_isRightAligned)
				return iMaxLineLength;

			int iAdjust = sLinBeg.Replace("/// ", "").Length;

			// Slight variation needed when the code is surrounded by 'namespace'.
			iAdjust = _tabSize * (iAdjust - (_isNamespaced ? 2 : 1));

			return iMaxLineLength - iAdjust;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Word-wraps text based on specified maximum length of a line.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="iMaxLineLength"></param>
		/// <returns>Word-wrapped text</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetWrappedTxt(string text, int iMaxLineLength)
		{
			Logging.Log(3);

			int currLineLength = 0;
			string sWrappedTxt = String.Empty;
			// Split the text according to the specified max length.
			foreach (Match m in Regex.Matches(text, @"\S+\s*"))
			{
				if (currLineLength + m.Length > iMaxLineLength)
				{
					if (sWrappedTxt != String.Empty && !Regex.IsMatch(sWrappedTxt, @"[\n]+[\s]*$"))
						sWrappedTxt += "\n";
					currLineLength = 0;
				}
				sWrappedTxt += m.Value;
				currLineLength += m.Length;
				if (m.Value.IndexOf("\n", StringComparison.Ordinal) > 0)
					currLineLength = 0;
			}
			return sWrappedTxt;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the region divider lines to the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with region divider lines added.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AddRegionDividerLines(string text)
		{
			string regionDividerLine = new string(OptionsHelper.RegionDividerLineChar, Math.Max(2, OptionsHelper.RegionDividerLineRepeat));
			const string desc = "Add region divider lines.";
			const string findWhat = @"[\t]*[\r\n]+([\t ]*)\#region";
			string replWith = "\n\n$1//" + regionDividerLine + "\n\n$1#region";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the top minimum divider lines to the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with top divider lines added.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AddTopMiniDividerLines(string text)
		{
			const string desc = @"Add top mini divider lines.";
			const string findWhat = @"(?![ \t]*//[=\-]+\n[ \t]*)^([ \t]*)(///[ \t]*\<summary\>)";
			const string replWith = "$1//--\n$1$2";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the mini divider lines before Attributes in the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with mid divider lines added.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AddAttributeDividerLines(string text)
		{
			const string desc = @"Add attribute mini divider lines1.";
			const string findWhat = @"(\>)([\r\n]*)([ \t]*)([/]*[\s]*\[)";

			// 
			const string replWith = "$1$2$3//--$2$3$4";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the mini divider lines before 'Resharper disable' comments in the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with mid divider lines added.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AddResharperDividerLines(string text)
		{
			const string desc = @"Add resharper mini divider lines2.";
			const string findWhat = @"([\r\n]*)([ \t]*)(//[ \t]*ReSharper disable[^\n]*[\s\r\n]*)(public|private|protected|internal|static|void)";
			const string replWith = "$1$2//--\n$2$3$4";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the bottom minimum divider lines to the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with bottom divider lines added.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AddBottomMiniDividerLines(string text)
		{
			//--------------------------------------------------------------------------------------------------------------------
			//  [1]: A numbered capture group. [\</summary\>|\</param\>|\</typeparam\>|\</returns\>
			//								   |\</remarks\>|\</exception\>|\</example\>|\</value\>
			//								   |[^\n]+\]
			//								   |(\<seealso[^/]+/\>)]
			//      Select from 10 alternatives
			//          \</summary\>
			//          \</param\>
			//          \</typeparam\>
			//          \</returns\>
			//          \</remarks\>
			//          \</exception\>
			//          \</example\>
			//          \</value\>
			//          [2]: A numbered capture group. [\<seealso[^/]+/\>]
			//              \<seealso[^/]+/\>
			//                  Literal <
			//                  seealso
			//                  Any character that is NOT in this class: [/], one or more repetitions
			//                  /
			//                  Literal >
			//  Any character in this class: [\s\r], any number of repetitions
			//  New line
			//  [3]: A numbered capture group. [[ \t]*]
			//      Any character in this class: [ \t], any number of repetitions
			//  Match if suffix is absent. [\s]
			//      Whitespace
			//  Match if suffix is absent. [/]
			//      /
			//--------------------------------------------------------------------------------------------------------------------
			const string desc = "Add bottom divider lines.";
			const string findWhat = @"(\</summary\>|\</param\>|\</typeparam\>|\</returns\>" +
									@"|\</remarks\>|\</exception\>|\</example\>|\</value\>" +
									@"|(\<seealso[^/]+/\>)" +
									@")" +
									@"[\s\r]*\n([ \t]*)(?!\s)(?!/)";
			const string replWith = "$0//--\n$3";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adjusts the length of all divider lines to the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with the length of each divider line adjusted.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string AdjustLengthOfDividerLines(string text)
		{
			//const string desc = "Adjust length of divider lines.";
			Logging.Log(3);
			MatchCollection m = Regex.Matches(text, "^([\t]*)(//--[\\-]*)\\s*\n", RegexOptions.Multiline);
			for (int i = m.Count - 1; i >= 0; i--)
			{
				int numTabs = m[i].Groups[1].Captures[0].Length - 2;
				Capture cap = m[i].Groups[2].Captures[0];
				string newString = "//" + new string(_commentDividerLineChar, _commentDividerLineRepeat - numTabs * _tabSize);
				text = text.Substring(0, cap.Index) + newString + text.Substring(cap.Index + cap.Length);
			}
			return text;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes all tabs and spaces from the blank lines of the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with tabs and spaces removed from all blank lines.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string RemoveTabsAndSpacesFromBlankLines(string text)
		{
			const string desc = "Remove tabs and spaces from blank lines.";
			const string findWhat = @"^[ \t]+$";
			const string replWith = "";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes all extra blank lines preceding comment lines in the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns>The original text with extra blank lines removed preceding comment lines.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string RemoveExtraBlankLinesPrecedingCommentLines(string text)
		{
			const string desc = "Remove extra blank lines preceding comment lines.";
			string findWhat = String.Format(@"[\n]{{3,}}([ \t]*//\{0}\{0})", _commentDividerLineChar);
			const string replWith = "\n\n$1";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes all errant summary tag divider lines from the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		internal static string RemoveErrantSummaryTagDividerLines(string text)
		{
			const string desc = "Remove all errant summary tag divider lines.";
			string findWhat = String.Format(@"(\<summary\>[ \t\r]*\n)[ \t]*//\{0}[\{0}]+[ \t\r]*\n", OptionsHelper.CommentDividerLineChar);
			const string replWith = "$1";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces tabs in the specified text with spaces.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string ReplaceTabsWithSpaces(string text)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            const string desc = "Replace tabs with spaces";
			Connect.DteService.DTE.Properties["TextEditor", "CSharp"].Item("InsertTabs").Value = false;
			return MyRegex.Replace(text, @"\t", new string(' ', _tabSize), RegexOptions.Multiline, desc);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes multiple lines following a left bracket from the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string RemoveMultipleLinesFollowingLeftBracket(string text)
		{
			const string desc = "Remove multiple lines following a left bracket.";
			return MyRegex.Replace(text, @"{\r\n\r\n", "{\r\n", RegexOptions.Multiline, desc);
		}
	}
}