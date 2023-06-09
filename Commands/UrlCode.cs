using System;
using System.Text.RegularExpressions;
using System.Web;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for encoding or decoding the selected text (URL) in the current Visual Studio editor window.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class UrlCode
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Enumeration
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public enum UrlEncodeOrDecodeEnum
		{
			Encode = 0,
			Decode = 1
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// URL encodes or decodes the selected text.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void UrlEncodeOrDecode(UrlEncodeOrDecodeEnum eCode)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			
			Logging.Log(2);

			// Get the selection.
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			string sTxt = sel.Text;

			// Make sure some text is selected.
			if (sel == null || sTxt == String.Empty)
				throw new ApplicationException("No text was selected!");

			if (eCode == UrlEncodeOrDecodeEnum.Decode)
				sTxt = HttpUtility.UrlDecode(sTxt);
			else
			{
				// Hold onto the beg-of-line and end-of-line characters.
				string sBeg = String.Empty;
				string sEnd = String.Empty;
				Match oMatch = Regex.Match(sTxt, @"^(\s*)");
				if (oMatch.Length > 0)
					sBeg = oMatch.Value;
				if (sTxt.EndsWith("\r\n"))
					sEnd = "\r\n";
				else if (sTxt.EndsWith("\r"))
					sEnd = "\r";
				else if (sTxt.EndsWith("\n"))
					sEnd = "\n";
				// Reattach the beg-of-line / end-of-line characters.
				sTxt = sBeg + HttpUtility.UrlEncode(sTxt.Trim()) + sEnd;
			}

			// Paste the modified text back into the selection.
			Utilities.PasteTextIntoSelection(sel, sTxt);
		}
	}
}