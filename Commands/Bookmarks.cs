using System;
using System.Text;
using System.Windows.Forms;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for processing bookmarks in Visual Studio.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class Bookmarks
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Provides the actions used in conjunction with bookmarked items.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		[Flags]
		public enum ProcessBookmarksEnum
		{
			Cut = 0,
			Copy = 1,
			Delete = 2
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Cuts, copies, or deletes the current bookmarked items.
		/// </summary>
		/// <param name="eAction">One of 'cut', 'copy', or 'delete' actions to perform.</param>
		//----------------------------------------------------------------------------------------------------
		public static void ProcessBookmarks(ProcessBookmarksEnum eAction)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Logging.Log();

			// Get the current selection in the active window.
			TextSelection sel = MyTextSelection.Current();

			// Move the cursor to the beginning of the line.
			sel.StartOfLine();

			// Remember the current cursor position.
			EditPoint activePoint = sel.ActivePoint.CreateEditPoint();

			//Connect.ApplicationObject.ExecuteCommand("Edit.StopOutlining", string.Empty);

			int iFirstBookmarkLine = 0;
			var sb = new StringBuilder();

			// Start at the beginning of the document.
			sel.StartOfDocument();
			if (sel.NextBookmark())
			{
				iFirstBookmarkLine = sel.ActivePoint.Line;
				sel.SelectLine();

				//Console.WriteLine(sel.IsActiveEndGreater);
				//Console.WriteLine(sel.IsEmpty);

				// Escape double-quotes inside the text.
				sb.Append(sel.Text.Replace("\"", "\"\""));
				if (eAction == ProcessBookmarksEnum.Cut || eAction == ProcessBookmarksEnum.Delete)
					sel.Delete();
				sel.CharLeft();
			}

			// Continue looking for more bookmarks.
			while (sel.NextBookmark())
			{
				if (eAction == ProcessBookmarksEnum.Cut || eAction == ProcessBookmarksEnum.Delete)
				{
					sel.SelectLine();
					sb.Append(sel.Text.Replace("\"", "\"\""));
					sel.Delete();
				}
				else
				{
					// Determine if we've wrapped around to the beginning.
					if (sel.ActivePoint.Line == iFirstBookmarkLine)
						break;
					sel.SelectLine();
					sb.Append(sel.Text.Replace("\"", "\"\""));
				}
				sel.CharLeft();
			}

			string sTxt = sb.ToString();

			if (sTxt.Length > 0 &&
				(eAction == ProcessBookmarksEnum.Cut || eAction == ProcessBookmarksEnum.Copy))
			{
				sTxt = sTxt.TrimEnd();

				// Restore double-quotes in the text.
				sTxt = sTxt.Replace("\"\"", "\""); //.Replace("\r\n", "\n");

				// Copy the text to the clipboard.
				Clipboard.SetDataObject(sTxt, true);
			}
			// Return to the position remembered above.
			sel.MoveToPoint(activePoint);
		}

	}
}