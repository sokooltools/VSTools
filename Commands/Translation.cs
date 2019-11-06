using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class Translation
	{
		private const string FR_EXT = ".fr";
		private const string EN_EXT = ".resx";
		private static string _outputFile;

		private static bool OutputUntranslatedOnly { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Generates the report.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void GenerateReport()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

			Logging.Log();

			try
			{
				if (!Connect.ApplicationObject.Solution.IsOpen)
					MessageBox.Show(Resources.CommandOnlyWorksOnActiveSolution, @"Generate Translation Report",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				else
				{
					using (var dialog = new TranslationDialog())
					{
						if (dialog.ShowDialog() != DialogResult.OK)
							return;
						OutputUntranslatedOnly = dialog.ChkOutputUntranslatedOnly.Checked;
					}
					Cursor.Current = Cursors.WaitCursor;
					VsStatusBar.ColorText = "Generating Translation Report... ";
					VsStatusBar.Animation = true;

					_outputFile = Path.Combine(Path.GetTempPath(),
						Path.GetFileNameWithoutExtension(Connect.ApplicationObject.Solution.FileName) + "_Translate.xml");
					if (File.Exists(_outputFile))
						File.Delete(_outputFile);
					using (var sw = new StreamWriter(_outputFile, true))
					{
						sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
						sw.WriteLine("<document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
					}
					foreach (Project objProject in Connect.ApplicationObject.Solution.Projects)
					{
						if (objProject.Kind == PrjKind.prjKindCSharpProject)
							NavigateProjectItems(objProject, objProject.ProjectItems);
					}
					using (var sw = new StreamWriter(_outputFile, true))
						sw.WriteLine("</document>");
					System.Diagnostics.Process.Start("EXCEL.EXE", "\"" + _outputFile + "\""); // "C:\Program Files\Microsoft Office\Office12\EXCEL.EXE"
																		   //if (File.Exists(_outputFile)) File.Delete(_outputFile);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				VsStatusBar.Text = "Done";
				VsStatusBar.Animation = false;
				Cursor.Current = Cursors.Default;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Navigates the project items.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <param name="projectItems">The project items.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void NavigateProjectItems(Project project, ProjectItems projectItems)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItems == null) return;
			const string OTHER_LANG_EXT = FR_EXT + EN_EXT;
			foreach (ProjectItem projectItem in projectItems)
			{
				string englishFile = projectItem.FileNames[0];
				if (englishFile.EndsWith(EN_EXT) && !englishFile.EndsWith(OTHER_LANG_EXT))
				{
					string dir = Path.GetDirectoryName(englishFile) ?? "";
					string fil = Path.GetFileNameWithoutExtension(englishFile);
					string otherLangFile = Path.Combine(dir, fil) + OTHER_LANG_EXT;
					if (File.Exists(otherLangFile))
						ProcessResourceFiles(project.Name, englishFile, otherLangFile, _outputFile);
				}
				NavigateProjectItems(project,
					projectItem.SubProject != null
						? projectItem.SubProject.ProjectItems
						: projectItem.ProjectItems);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Compares two resource files creating a single xml file in the process which will eventually be read into Excel.
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="fileEnglish"></param>
		/// <param name="fileOtherLang"></param>
		/// <param name="fileOutput"></param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void ProcessResourceFiles(string projectName, string fileEnglish, string fileOtherLang, string fileOutput)
		{
			try
			{
				// Process the OtherLang resource file.
				var dictOtherLang = new Dictionary<string, string>();
				using (var rsxr = new ResXResourceReader(fileOtherLang))
				{
					rsxr.BasePath = Path.GetDirectoryName(fileOtherLang);
					// Iterate through the french resources and add them to the dictionary.
					foreach (DictionaryEntry d in rsxr)
					{
						string key = d.Key.ToString();
						if ((!(d.Value is string) || key.StartsWith(">>") || key.StartsWith("$") ||
							 !Regex.IsMatch(key, @"(Text|Caption[0-9]*)$") && key.LastIndexOf('.') != -1) && key != "$this.Text")
							continue;
						if (d.Value.ToString().Length > 0 &&
							!Regex.IsMatch(d.Value.ToString(), @"^([ \t0-9=\<\>\+\-\.\$#%]*|[&]*OK|[&]*Ok)$"))
							dictOtherLang.Add(key, d.Value.ToString());
					}
				}

				// Process the English resource file.
				var dictEnglish = new SortedDictionary<string, string>();
				using (var rsxr = new ResXResourceReader(fileEnglish))
				{
					rsxr.BasePath = Path.GetDirectoryName(fileEnglish);
					// Iterate through the english resources and add them to the dictionary.
					foreach (DictionaryEntry d in rsxr)
					{
						string key = d.Key.ToString();
						if ((!(d.Value is string) || key.StartsWith(">>") || key.StartsWith("$") ||
							 !Regex.IsMatch(key, @"(Text|Caption[0-9]*)$") && key.LastIndexOf('.') != -1) && key != "$this.Text")
							continue;
						if (d.Value.ToString().Length > 0 &&
							!Regex.IsMatch(d.Value.ToString(), @"^([ \t0-9=\<\>\+\-\.\$#%]*|[&]*OK|[&]*Ok)$"))
							dictEnglish.Add(key, d.Value.ToString());
					}
				}

				string fileName = Path.GetFileNameWithoutExtension(fileEnglish);
				using (var sw = new StreamWriter(fileOutput, true))
				{
					foreach (KeyValuePair<string, string> en in dictEnglish)
					{
						string frValue;
						if (dictOtherLang.ContainsKey(en.Key))
						{
							frValue = dictOtherLang[en.Key];
							if (OutputUntranslatedOnly && !frValue.StartsWith("#"))
								continue;
						}
						else
							frValue = "#" + en.Value;
						sw.WriteLine("<row>");
						sw.WriteLine("\t<Project>" + projectName + "</Project>");
						sw.WriteLine("\t<Control>" + fileName + "</Control>");
						sw.WriteLine("\t<Item>" + en.Key + "</Item>");
						sw.WriteLine("\t<English>" + HttpUtility.HtmlEncode(en.Value) + "</English>");
						sw.WriteLine("\t<French>" + HttpUtility.HtmlEncode(frValue) + "</French>");
						sw.WriteLine("</row>");
					}
				}
			}
			catch (Exception ex)
			{
				Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
				OutputPane.WriteLine("Error processing: \"" + fileEnglish + "\"");
				OutputPane.WriteLine("\t\t(" + ex.Message + ")");
			}
		}

		////----------------------------------------------------------------------------------------------------
		///// <summary>
		///// This is a test.
		///// </summary>
		///// <param name="dte"></param>
		////----------------------------------------------------------------------------------------------------
		//public static void slnExplUIHierarchyExample(DTE2 dte)
		//{
		//    UIHierarchy uih = dte.ToolWindows.SolutionExplorer;
		//    // Set a reference to the first level nodes in Solution Explorer.
		//    // Automation collections are one-based.
		//    UIHierarchyItem uihItem = uih.UIHierarchyItems.Item(1);
		//    StringBuilder sb = new StringBuilder();
		//    // Iterate through first level nodes.
		//    foreach (UIHierarchyItem fid in uihItem.UIHierarchyItems)
		//    {
		//        sb.AppendLine(fid.Name);
		//        // Iterate through second level nodes (if they exist).
		//        foreach (UIHierarchyItem subitem in fid.UIHierarchyItems)
		//        {
		//            sb.AppendLine("   " + subitem.Name);
		//            //UIHierarchyItem item = (UIHierarchyItem) subitem.Object;
		//            // Iterate through third level nodes (if they exist).
		//            foreach (UIHierarchyItem subSubItem in subitem.UIHierarchyItems)
		//            {
		//                sb.AppendLine("        " + subSubItem.Name);
		//            }
		//        }
		//    }
		//    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "test.txt"), true))
		//    {
		//        sw.Write(sb.ToString());
		//    }
		//    //Utilities.WriteToOutputPane(sb.ToString());
		//}

	}
}