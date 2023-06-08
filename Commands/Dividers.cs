using System;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for adding and/or removing "Comment" and/or "Region" divider lines.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class Dividers
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes all divider lines (i.e., 'Comment' and 'Region') from all the text.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		internal static void RemoveAllDividerLines()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log();

			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// Hold onto the current line number.
			int iLine = sel.AnchorPoint.Line;
			int iLineCharOffset = sel.AnchorPoint.LineCharOffset;

			sel.SelectAll();

			// Always format the entire text to start
			sel.SmartFormat();

			string sTxt = sel.Text;

			sTxt = RemoveCommentDividerLines(sTxt);

			sTxt = RemoveRegionDividerLines(sTxt);

			// Delete the specified text.
			sel.Delete();

			// Insert the newly formatted text.
			sel.Insert(sTxt, (int)vsInsertFlags.vsInsertFlagsContainNewText);

			if (sel.BottomLine >= iLine)
			{
				// Go to the original line number and offset.
				sel.MoveToLineAndOffset(iLine, iLineCharOffset);
				sel.AnchorPoint.TryToShow();
				sel.CharRight();
				sel.CharLeft();
			}
			else
				sel.EndOfDocument();//StartOfDocument();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes all of the comment divider lines from the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		internal static string RemoveCommentDividerLines(string text)
		{
			const string DESC = "Remove existing comment divider lines.";
			string findWhat = String.Format(@"^[ \t]*//\{0}[\{0}]+[ \t\r]*\n", OptionsHelper.CommentDividerLineChar);
			string replWith = String.Empty;
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, DESC);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces double comment divider lines with single comment divider lines.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		internal static string ReplaceDoubleCommentDividerLines(string text)
		{
			const string DESC = "Replace double comment divider lines.";
			string findWhat = String.Format(@"^([ \t]*//\{0}[\{0}]+[ \t\r]*\n)[ \t]*//\{0}[\{0}]+[ \t\r]*\n", OptionsHelper.CommentDividerLineChar);
			string replWith = "$1";
			return MyRegex.Replace(text, findWhat, replWith, MyRegex.OPTIONS, DESC);
		}
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the region divider lines from the specified text.
		/// </summary>
		/// <param name="text">The original text.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		internal static string RemoveRegionDividerLines(string text)
		{
			const string DESC = "Remove existing region divider lines.";
			string findWhat = String.Format(@"[\n]+[\s]*//\{0}[\{0}]+[\s]*[\n]+([\s]*\#region)", OptionsHelper.RegionDividerLineChar);
			const string REPL_WITH = "\n$1";
			return MyRegex.Replace(text, findWhat, REPL_WITH, MyRegex.OPTIONS, DESC);
		}
	}
}
