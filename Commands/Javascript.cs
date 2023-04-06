using System;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//---------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for formatting javascript code.
	/// </summary>
	//---------------------------------------------------------------------------------------------------------------------------
	internal static class Javascript
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		[Flags]
		public enum FormatOrCompactEnum
		{
			//------------------------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Format = 0
			/// </summary>
			//------------------------------------------------------------------------------------------------------------------------
			Format = 0,
			//------------------------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Compact = 1
			/// </summary>
			//------------------------------------------------------------------------------------------------------------------------
			Compact = 1
		}

		//.................................................................................................................................

		#region FormatOrCompact

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Formats javascript using standard indents and spacing or compacts javascript by
		/// removing all extraneous spacing and linefeeds.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void FormatOrCompact(FormatOrCompactEnum eFormatOrCompact)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Get the selection.
            var sel = (TextSelection) Connect.ApplicationObject.ActiveWindow.Document.Selection;

			// Check that the selection is not empty.
			if (sel != null && sel.Text == String.Empty)
			{
				// Select all the text in the window as long as it is all Javascript (ie., document has a .js extension).
				if (Utilities.GetIsValidExtension(".js"))
				{
					sel.SelectAll();
					//sel = (TextSelection)Connect.ApplicationObject.ActiveWindow.Document.Selection;
				}
			}

			// Make sure selection is not empty.
			if (sel == null || sel.Text == String.Empty)
				throw new ApplicationException("No text was selected.");

			string sTxt = sel.Text;

			bool bStripComments = OptionsHelper.IsJavascriptCommentsStripped;
			sTxt = eFormatOrCompact == FormatOrCompactEnum.Compact
				? JSFormatter.GetCompactedScript(sTxt, bStripComments)
				: JSFormatter.GetFormattedScript(sTxt, bStripComments);

			//	// (For some reason cannot get the following to work when there are multiple lines!)
			//	// Activate the document and insert the modified text.
			//	applicationObject.ActiveWindow.Document.Activate();
			//	sel.Insert(sTxt, (int)EnvDTE.vsInsertFlags.vsInsertFlagsContainNewText); // 4
			//	// Cannot get this to work either!
			//	sel.Text = sTxt.Replace("\n", Environment.NewLine);

			// Paste the modified text back into the selection.
			Utilities.PasteTextIntoSelection(sel, sTxt);
		}

		#endregion

		//.............................................................................................................
	}
}