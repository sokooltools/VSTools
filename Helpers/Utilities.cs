using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class Utilities
	{
        //.............................................................................................................

        #region Helper Methods

        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets an indication as to whether the current document represents CSharp code.
        /// </summary>
        //----------------------------------------------------------------------------------------------------
        public static bool IsCsDocument
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return Connect.objDte2.ActiveDocument.Language.Equals("CSharp");
            }
        }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an indication as to whether the current document represents Visual Basic code.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static bool IsVbDocument
		{
			get
			{
				Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
				return Connect.objDte2.ActiveDocument.Language.Equals("Basic");
			}
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an indication as to whether the specified extension is valid for the active window.
		/// </summary>
		/// <param name="extension">The extension including the period (e.g. ".cs").</param>
		/// <returns><c>true</c> if the extension is valid; otherwise, <c>false</c>.</returns>
		//----------------------------------------------------------------------------------------------------
		public static bool GetIsValidExtension(string extension)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (Connect.objDte2 == null || Connect.objDte2.ActiveWindow.Document == null)
				return false;
			return
				String.Compare(Path.GetExtension(Connect.objDte2.ActiveWindow.Document.FullName),
					extension, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Helper function to insert text back into the document window's selection.
		/// </summary>
		/// <param name="sel">The selection to insert into.</param>
		/// <param name="txt">The text to insert into the selection.</param>
		//----------------------------------------------------------------------------------------------------
		public static void PasteTextIntoSelection(TextSelection sel, string txt)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Get our current location
            EditPoint active = sel.ActivePoint.CreateEditPoint();
			var pane = (TextPane)Connect.objDte2.ActiveDocument.Object("TextPane");
			EditPoint corner = pane.StartPoint.CreateEditPoint();

			// Hold onto text currently on the clipboard.
			string sTemp = null;
			IDataObject clip = Clipboard.GetDataObject();
			if (clip != null && clip.GetDataPresent(DataFormats.Text))
				sTemp = clip.GetData(DataFormats.Text).ToString();

			// Place newly formatted text onto the clipboard.
			Clipboard.SetDataObject(txt, false);
			// Paste text from clipboard into the selection.
			sel.Paste();

			// Put the original text back onto the clipboard.
			if (sTemp != null)
				Clipboard.SetDataObject(sTemp, true);

			// Reselect the original selection.
			sel.MoveToPoint(active, true);
			pane.TryToShow(corner, vsPaneShowHow.vsPaneShowTop);
		}

		#endregion

		//.............................................................................................................

		#region Path Related

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the specified folder path resolved with all environment variables and relative paths 
		/// expanded.
		/// </summary>
		/// <param name="folderPath">The original path.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public static string GetExpandedPath(string folderPath)
		{
			folderPath = Environment.ExpandEnvironmentVariables(folderPath.Trim());
			int countBack = 0;
			while (folderPath.StartsWith(@"..\"))
			{
				folderPath = folderPath.Remove(0, 3);
				countBack++;
			}
			string parentPath = Path.GetDirectoryName(Application.ExecutablePath);
			for (int i = 0; i < countBack; i++)
			{
				if (parentPath == null) continue;
				DirectoryInfo di = Directory.GetParent(parentPath);
				if (di == null)
					break;
				parentPath = di.ToString();
			}
			if (countBack > 0)
				if (parentPath != null) folderPath = Path.Combine(parentPath, folderPath);
			if (folderPath.StartsWith(@"\") && !folderPath.StartsWith(@"\\"))
				folderPath = Path.GetDirectoryName(Application.ExecutablePath) + folderPath;
			folderPath = folderPath.TrimEnd('\\');
			return folderPath;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the path collapsed to a relative path.
		/// </summary>
		/// <param name="folderPath">The original folder path.</param>
		/// <returns>The path collapsed to a relative path.</returns>
		//----------------------------------------------------------------------------------------------------
		public static string GetRelativePath(string folderPath)
		{
			string parentPath = Path.GetDirectoryName(Application.ExecutablePath);
			if (parentPath == null) 
				return folderPath;
			string relativePath = @"";
			while (parentPath.Length > 0)
			{
				if (folderPath.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
				{
					folderPath = folderPath.Remove(0, parentPath.Length);
					if (folderPath.StartsWith(@"\"))
						folderPath = folderPath.Remove(0, 1);
					if (relativePath.Length == 0)
						relativePath = @"\";
					return relativePath + folderPath;
				}
				DirectoryInfo di = Directory.GetParent(parentPath);
				if (di == null)
					break;
				parentPath = di.ToString();
				relativePath += @"..\";
			}
			return folderPath;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the the specified folder path right-trimmed to contain the path to the first existing folder 
		/// path which may or may not be the original folder path.
		/// </summary>
		/// <param name="folderPath">The file/folder path to examine.</param>
		/// <returns>The existing part of the folder path.</returns>
		//----------------------------------------------------------------------------------------------------
		public static string GetTrimToExistingPath(string folderPath)
		{
			folderPath = Environment.ExpandEnvironmentVariables(folderPath.Trim());
			while (!Directory.Exists(folderPath))
			{
				int index = folderPath.LastIndexOf('\\');
				if (index <= 0)
					break;
				folderPath = folderPath.Substring(0, index);
			}
			return folderPath;
		}

		#endregion

		////----------------------------------------------------------------------------------------------------
		///// <summary>
		///// Determines whether this instance can be executed.
		///// </summary>
		///// <returns><c>true</c> if this instance can be executed; otherwise, <c>false</c>.</returns>
		////----------------------------------------------------------------------------------------------------
		//internal static bool CanExecute()
		//{
		//    Project project = Connect.ApplicationObject.SelectedItems.Item(1).Project;
		//    if (project == null)
		//    {
		//        // Executed at the solution level
		//        return (new ProjectIterator(Connect.ApplicationObject.Solution).FirstOrDefault(prj => prj.Kind == VSLangProj.PrjKind.prjKindCSharpProject) != null);
		//    }
		//    // Executed at the project level
		//    return project.Kind == VSLangProj.PrjKind.prjKindCSharpProject;
		//}

		////----------------------------------------------------------------------------------------------------
		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		////----------------------------------------------------------------------------------------------------
		//private static bool IsOpenSolution()
		//{
		//    if (Connect.ApplicationObject.Solution == null || Connect.ApplicationObject.Solution.Count == 0)
		//    {
		//        MessageBox.Show("Sorry.. There must be an open solution for this command to execute.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		//        return false;
		//    }
		//    return true;
		//}
	}
}