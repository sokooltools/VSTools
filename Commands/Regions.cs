using System;
using System.Text.RegularExpressions;
using EnvDTE;
// ReSharper disable EmptyGeneralCatchClause

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for managing the Regions inside Visual Studio text edit control such as Expand, Collapse, Toggle, etc.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class Regions
	{
		//..................................................................................................................................

		#region Region Methods

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses the current region (i.e., the region containing the cursor).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void RegionsToggleCurrent()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			
			Logging.Log(2);

			// Make sure the current document can be processed.
			if (!(Utilities.IsCsDocument || Utilities.IsVbDocument))
				return;

			StartAutomaticOutlining();

			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Selection;
			EditPoint active = sel.ActivePoint.CreateEditPoint();
			while (!active.AtStartOfDocument)
			{
				active.StartOfLine();
				string line = active.GetText(active.LineLength).Trim().Replace(" ", "").Replace("\t", "").ToLower();
				if (line.StartsWith("#region"))
				{
					sel.MoveToLineAndOffset(active.Line, 1);
					Connect.objDte2.ExecuteCommand("Edit.ToggleOutliningExpansion", String.Empty);
					return;
				}
				active.LineUp();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses all regions. (Only regions are collapsed - i.e. all other outlining is maintained).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void RegionsCollapseAll()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Make sure the current document can be processed.
			if (!(Utilities.IsCsDocument || Utilities.IsVbDocument)) return;

			RegionsExpandAll();

			// Hold onto the currently selected cursor position.
			TextSelection sel = MyTextSelection.Current();
			sel.SelectAll();
			string sTxt = sel.Text;
			const string S_PATTERN = @"^[\s]*\#[\s]*region";
			MatchCollection mc = Regex.Matches(sTxt, S_PATTERN, RegexOptions.Multiline);
			if (mc.Count > 0)
			{
				// Work backwards through the text collapsing each region as it is encountered.
				for (int i = mc.Count - 1; i >= 0; i--)
				{
					int iLine = Regex.Matches(sTxt.Substring(0, mc[i].Index + 5), @"\n", RegexOptions.Multiline).Count + 1;
					sel.MoveToLineAndOffset(iLine, 1);
					Connect.objDte2.ExecuteCommand("Edit.ToggleOutliningExpansion", String.Empty);
				}
			}
			sel.StartOfDocument();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Expands all regions. (Only regions are expanded - i.e., all other outlining is maintained).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void RegionsExpandAll()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Set the search pattern depending on the document type.
			string sPattern;
			if (Utilities.IsCsDocument)
				sPattern = @"^[\s]*\#[\s]*endregion";
			else if (Utilities.IsVbDocument)
				sPattern = @"^[\s]*\#[\s]*end region";
			else
				return;

			StartAutomaticOutlining();

			// Hold onto the currently selected point.
			TextSelection sel = MyTextSelection.Current();

			sel.SelectAll();
			string sTxt = sel.Text;
			foreach (Match m in Regex.Matches(sTxt, sPattern, RegexOptions.Multiline))
			{
				int iLine = Regex.Matches(sTxt.Substring(0, m.Index + 5), @"\n", RegexOptions.Multiline).Count + 1;
				sel.MoveToLineAndOffset(iLine, 1);
			}

			// Reselect the original edit point.
			MyTextSelection.Restore();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses all regions. (Only regions are collapsed - i.e. all other outlining is maintained).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void CollapseAllSummaries()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Make sure the current document can be processed.
			if (!(Utilities.IsCsDocument || Utilities.IsVbDocument)) return;

			StopOutlining();
			//StartAutomaticOutlining();
            try
            {
                Connect.objDte2.ExecuteCommand("Edit.StartAutomaticOutlining", String.Empty);
            }
            catch
            {
            }

            // Hold onto the currently selected cursor position.
            TextSelection sel = MyTextSelection.Current();
			sel.SelectAll();
			string sTxt = sel.Text;
			const string S_PATTERN = @"^[\s]*///[\s]*\<summary\>";
			MatchCollection mc = Regex.Matches(sTxt, S_PATTERN, RegexOptions.Multiline);
			if (mc.Count > 0)
			{
				// Work backwards through the text collapsing each region as it is encountered.
				for (int i = mc.Count - 1; i >= 0; i--)
				{
					int iLine = Regex.Matches(sTxt.Substring(0, mc[i].Index + 5), @"\n", RegexOptions.Multiline).Count + 1;
					sel.MoveToLineAndOffset(iLine, 1);
					Connect.objDte2.ExecuteCommand("Edit.ToggleOutliningExpansion", String.Empty);
				}
			}
			sel.StartOfDocument();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Expands all summaries. (Only summaries are expanded - i.e., all other outlining is maintained).
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void ExpandAllSummaries()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Set the search pattern depending on the document type.
			string sPattern;
			if (Utilities.IsCsDocument)
				sPattern = @"^[\s]*///[\s]*\</summary\>";
			else if (Utilities.IsVbDocument)
				sPattern = @"^[\s]*///[\s]*\</summary\>";
			else
				return;

			StartAutomaticOutlining();

			// Hold onto the currently selected point.
			TextSelection sel = MyTextSelection.Current();

			sel.SelectAll();
			string sTxt = sel.Text;
			foreach (Match m in Regex.Matches(sTxt, sPattern, RegexOptions.Multiline))
			{
				int iLine = Regex.Matches(sTxt.Substring(0, m.Index + 5), @"\n", RegexOptions.Multiline).Count + 1;
				sel.MoveToLineAndOffset(iLine, 1);
			}

			// Reselect the original edit point.
			MyTextSelection.Restore();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Executes the command to Start Automatic Outlining catching the exception which occurs
		/// should the command not be available.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void StartAutomaticOutlining()
		{
			try
			{
                //Connect.ApplicationObject.ExecuteCommand("Edit.ToggleAllOutlining", string.Empty);
                //Connect.ApplicationObject.ExecuteCommand("Edit.ToggleOutliningExpansion", string.Empty);
                Connect.objDte2.ExecuteCommand("Edit.StartAutomaticOutlining", String.Empty);
			}
            catch
            {
            }
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Executes the command to Stop Automatic Outlining catching the exception which occurs
		/// should the command not be available.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void StopOutlining()
		{
			try
			{
				Connect.objDte2.ExecuteCommand("Edit.StopOutlining", String.Empty);
			}
			catch
			{
			}
		}

		#endregion

		//....................................................................................................
	}
}