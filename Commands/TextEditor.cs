using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class TextEditor
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Pastes the text currently located on the clipboard into the selection as commented text.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void PasteTextAsComments()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Maximum number of characters to be displayed on a line before automatically word-wrapping it to the next line
			int pasteCommentMaxLineLength = OptionsHelper.PasteCommentMaxLineLength;

			bool bIsJavascript = Utilities.GetIsValidExtension(".js");

			string sComment;
			string docLanguage = Connect.objDte2.ActiveDocument.Language;

			// Determine comment character to use based on the language.
			if (docLanguage.Equals("CSharp") || bIsJavascript)
				sComment = "// ";
			else if (docLanguage.Equals("Basic"))
				sComment = "' ";
			else
				return;

			// Another method for grabbing whats on the clipboard.
			//sel.Paste();
			//sel.MoveToPoint(sp, true);
			//string sTxt = sel.Text;
			//sel.Delete(1);

			// Get the text off the clipboard.
			string sClipTxt = String.Empty;
			IDataObject clip = Clipboard.GetDataObject();
			if (clip != null && clip.GetDataPresent(DataFormats.Text))
				sClipTxt = clip.GetData(DataFormats.Text).ToString();

			// Remove carriage returns.
			sClipTxt = sClipTxt.Replace("\r\n", "\n");

			// Word-wrap the text based on the maximum length allowed for a line.
			string sNewtxt = String.Empty;
			int currLineLength = 0;
			foreach (Match m in Regex.Matches(sClipTxt, @"\S+\s*"))
			{
				if (currLineLength + m.Length > pasteCommentMaxLineLength)
				{
					// Add a linefeed to the end of the string when the length surpasses the maximum length allowed.
					if (!Regex.IsMatch(m.Value, @"[\n]+[\s]*$") && !Regex.IsMatch(sNewtxt, @"[\n]+[\s]*$"))
						sNewtxt += "\n";
					currLineLength = 0;
				}

				// Keep appending each word one at a time to the new string.
				sNewtxt += m.Value;
				currLineLength += m.Length;
				// Reset the linelength if the word contains a linefeed.
				if (m.Value.IndexOf("\n", StringComparison.Ordinal) > 0)
					currLineLength = 0;
			}

			// Get the indent located at the beginning of the string.
			string sIndent = Regex.Match(sClipTxt, @"^\s*").Value;

			// 
			const string desc = "Add the comment characters to the beginning of each line.";
			sNewtxt = MyRegex.Replace(sIndent + sNewtxt.TrimEnd(), "^", sComment, RegexOptions.Multiline, desc, 2);

			// Get the current selection in the active window.
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// Move the cursor to the beginning of the line.
			sel.StartOfLine();

			// Remember the current position.
			EditPoint activePoint = sel.ActivePoint.CreateEditPoint();

			// Insert the new text.
			sel.Insert(sNewtxt, (int)vsInsertFlags.vsInsertFlagsContainNewText);

			// Return to the position remembered above.
			sel.MoveToPoint(activePoint, true);

			// Have Visual Studio format the text (as long as it isn't javascript).
			if (!bIsJavascript)
				sel.SmartFormat();
		}
	}
}
