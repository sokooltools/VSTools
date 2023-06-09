using System;
using EnvDTE;

namespace SokoolTools.VsTools
{
	/// <summary>
	/// Provides methods for processing the current 'document' inside Visual Studio such as sorting selected lines or lining up 
	/// variables.
	/// </summary>
	internal static class Docs
	{
		//..................................................................................................................................

		#region SortSelectedLines

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sorts the selected lines.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void SortSelectedLines()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Get the current selection.
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// This should never happen but just make sure some text is selected.
			if (sel == null)
				throw new ApplicationException("No text was selected!");

			// Make sure the selection begins and ends at the end of a line
			if (!sel.IsActiveEndGreater)
				sel.SwapAnchor();
			if (!sel.ActivePoint.AtEndOfLine)
				sel.EndOfLine(true);

			sel.SwapAnchor();
			if (!sel.ActivePoint.AtStartOfLine)
				sel.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, true);
			if (!sel.IsActiveEndGreater)
				sel.SwapAnchor();

			//sel.MoveToAbsoluteOffset(sel.ActivePoint.AbsoluteCharOffset + sel.ActivePoint.LineLength - sel.ActivePoint.LineCharOffset + 1, true);
			//// Make sure the selection is not at the very beginning of a line.
			//if (sel.ActivePoint.AtStartOfLine && sel.ActivePoint.AtEndOfLine)
			//    sel.MoveToAbsoluteOffset(sel.ActivePoint.AbsoluteCharOffset - 1, true);

			// Make sure indents are either all tabs or all spaces.
			if (Utilities.IsCsDocument || Utilities.IsVbDocument)
				sel.SmartFormat();

			// Get the text of the selection for processing.
			string sTxt = sel.Text;

			// Remove any extra CRLFs from the end of the text before sorting.
			bool endsWithReturn = false;
			while (sTxt.EndsWith("\r") || sTxt.EndsWith("\n"))
			{
				endsWithReturn = true;
				sTxt = sTxt.Remove(sTxt.Length - 1);
			}

			// Split the text into an array and sort it.
			string[] aTxt = sTxt.Split('\n');
			Array.Sort(aTxt);

			// Convert the array back into a string adding CRLFs.
			sTxt = String.Empty;
			foreach (string line in aTxt)
				sTxt += line.Replace("\r", "") + "\r\n";
			// Remove the last CRLF
			if (!endsWithReturn && sTxt.EndsWith("\r\n"))
				sTxt = sTxt.Remove(sTxt.Length - 2);

			// Get the virtual edit points.
			EditPoint topEditPt = sel.TopPoint.CreateEditPoint();
			EditPoint botEditPt = sel.BottomPoint.CreateEditPoint();

			// Replace the selected text with the given text
			topEditPt.ReplaceText(botEditPt, sTxt, (int)vsEPReplaceTextOptions.vsEPReplaceTextTabsSpaces);
		}

		#endregion

		//..................................................................................................................................

		#region LineUpVariableDeclarations

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Lines up variable declarations for all selected lines.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void LineUpVariableDeclarations()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			const string COMPARE = "=!><";
			const string ASSIGN = "+-|";
			string sLine;
			int iPos;
			int iMaxLength = 0;
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;
			EditPoint botEditPt = sel.BottomPoint.CreateEditPoint();
			EditPoint topEditPt = sel.TopPoint.CreateEditPoint();

			// Make sure indents are either all tabs or all spaces.
			if (Utilities.IsCsDocument || Utilities.IsVbDocument)
				sel.SmartFormat();

			topEditPt.StartOfLine();
			while (topEditPt.Line <= botEditPt.Line)
			{
				sLine = topEditPt.GetLines(topEditPt.Line, topEditPt.Line + 1);
				iPos = sLine.IndexOf("=", StringComparison.Ordinal);
				if (iPos != -1 && iPos > iMaxLength)
					iMaxLength = iPos;
				topEditPt.LineDown();
			}

			int maxIndent = OptionsHelper.AlignVariablesMaxIndent;
			if (iMaxLength > maxIndent)
				iMaxLength = maxIndent;

			botEditPt = sel.BottomPoint.CreateEditPoint();
			topEditPt = sel.TopPoint.CreateEditPoint();
			topEditPt.StartOfLine();
			while (topEditPt.Line <= botEditPt.Line)
			{
				sLine = topEditPt.GetLines(topEditPt.Line, topEditPt.Line + 1);
				iPos = sLine.IndexOf("=", StringComparison.Ordinal);
				if (iPos > 0 && iPos < iMaxLength)
				{
					// Check whether the equal sign is used for comparison as opposed to assignment
					if (!COMPARE.Contains(sLine.Substring(iPos - 1, 1)) && !COMPARE.Contains(sLine.Substring(iPos + 1, 1)))
					{
						string sNewLine;
						if (ASSIGN.Contains(sLine.Substring(iPos - 1, 1)))
							sNewLine = sLine.Substring(0, iPos - 2) + new string(' ', iMaxLength - iPos + 1) + sLine.Substring(iPos - 1);
						else
							sNewLine = sLine.Substring(0, iPos - 1) + new string(' ', iMaxLength - iPos + 1) + sLine.Substring(iPos);

						topEditPt.ReplaceText(sLine.Length, sNewLine, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
					}
				}

				topEditPt.LineDown();
			}
		}

		#endregion

		//..................................................................................................................................

		#region Format Document

		////----------------------------------------------------------------------------------------------------
		///// <summary>
		///// Formats the entire document without losing the current selection.
		///// </summary>
		///// <remarks>
		///// (Prior to VS2005, there was only a 'Format Selection' command).
		///// </remarks>
		////----------------------------------------------------------------------------------------------------
		//public static void FormatDocument()
		//{
		//    // Hold onto the current selection.
		//    TextSelection sel = MyTextSelection.Current();
		//    sel.SelectAll();
		//    Connect.ApplicationObject.ExecuteCommand("Edit.FormatSelection", string.Empty);
		//    // Restore the original selection.
		//    MyTextSelection.Restore();
		//}

		#endregion
	}
}