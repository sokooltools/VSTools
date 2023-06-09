using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;
using VSLangProj80;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for obtaining information regarding each project in the current solution. 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class ProjectStuff
	{
		private static readonly string NL = Environment.NewLine;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows each project in the entire solution and all its related references in a report in the output window.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void ShowProjectReferences()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			var projList = new List<Project>();
			OutputPane.Clear();
			try
			{
				string divLine = "+" + new string('-', 51) + "+" + new string('-', 16) + "+" + new string('-', 7) + "+" +
								  new string('-', 7) + "+" +
								  new string('-', 88) + "-";

				string header = divLine + NL + "| Project" + new string(' ', 43) + "|" + new string(' ', 16) + "| Spec. | Copy  |     " +
								new string(' ', 75) + "" + NL
								+ "|   Resources" + new string(' ', 39) + "| Version " + new string(' ', 7) + "| Vers. | Local | Path" +
								new string(' ', 75) + "" + NL
								+ divLine;

				OutputPane.Write(header);

				// Get all the projects into a generic list of projects so it can be sorted.
				foreach (Project prj in Connect.objDte2.Solution.Projects)
				{
					if (prj.Object is VSProject)
						projList.Add(prj);
					else
						IterateProjItems(prj, ref projList);
				}

				// Sort the list of projects in the order they appear in the solution.
				projList.Sort(
					(x, y) => {
                        Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                        return String.Compare(x.UniqueName.Replace("\\", "."), y.UniqueName.Replace("\\", "."), StringComparison.Ordinal);
                    });

				const int MAX_LINES = 60; // 6pt = 85; 7pt = 73; 8pt = 60; 9pt = 54; 10pt = 50; // TODO: read from registry

				// Iterate all the references for each project.
				int iCntTotal = 0;

				foreach (Project prj in projList)
				{
					string sRow = NL + new string(' ', 2) + prj.Name + new string(' ', 85 - prj.Name.Length) + GetOutputBuildPath(prj) +
								  NL;

					sRow += new string(' ', 2) + new string('.', 170) + NL;
					List<string> referenceList = GetProjectReferenceList(prj);

					sRow = referenceList.Aggregate(sRow, (current, reference) => current + new string(' ', 4) + reference + NL);
					int iCnt = referenceList.Count + 3; // '3' = '1' project name + '2' divider lines.

					if (iCntTotal + iCnt > MAX_LINES)
					{
						OutputPane.Write("\f" + NL + header);
						iCntTotal = iCnt;
					}
					else
						iCntTotal += iCnt;

					sRow += divLine;
					OutputPane.Write(sRow);
				}
				OutputPane.Write(NL + "*** For optimal printing use: 8pt mono-spaced font; landscape mode; no line wrap...");
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Iterates through all the project items (i.e., folders) adding projects inside them to the project list.
		/// </summary>
		/// <param name="prj">The project.</param>
		/// <param name="prjList">The project list.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void IterateProjItems(Project prj, ref List<Project> prjList)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Check whether the project is inside another folder.
            if (prj.ProjectItems == null)
				return;
			foreach (ProjectItem prjItem in prj.ProjectItems.Cast<ProjectItem>()
			    .Where(prjItem => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return prjItem.SubProject != null; }))
			{
				if (prjItem.SubProject.Kind == PrjKind.prjKindCSharpProject)
					prjList.Add(prjItem.SubProject);
				IterateProjItems(prjItem.SubProject, ref prjList);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a list of all the assemblies referenced by the specified project.
		/// </summary>
		/// <param name="prj">The project.</param>
		/// <returns>List of all the assemblies referenced by the specified project.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static List<string> GetProjectReferenceList(Project prj)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var referenceList = new List<string>();
			try
			{
				var objVsProject2 = (VSProject2)prj.Object;

				foreach (Reference3 objRef in objVsProject2.References)
				{
					string sRow = objRef.Name;
					bool isProjectReference = Regex.IsMatch(sRow, "\\(.*\\)") | (objRef.SourceProject != null);

				    if (!isProjectReference)
				        sRow = sRow.Length > 48 ? sRow.Substring(0, 45) + "..." : sRow.PadRight(48);
				    else
				    {
				        sRow = Regex.Replace(sRow, "\\(.*\\)", "");
				        // Indicates the reference is to another project.
				        string sToken = "<P>";

				        if (objRef.SourceProject == null)
				            sToken = "<!>"; // Indicates the reference does not exist!
				        if (sRow.Length > 48)
				            sRow = sRow.Substring(0, 41) + "... " + sToken;
				        else
				            sRow = sRow.PadRight(45) + sToken;
				    }
				    sRow += "  " + objRef.Version.PadRight(17);

					if (objRef.SpecificVersion & !isProjectReference)
						sRow += " [X]" + new string(' ', 4);
					else
						sRow += new string(' ', 8);

					if (objRef.CopyLocal)
						sRow += " [X]" + new string(' ', 4);
					else
						sRow += new string(' ', 8);
					sRow += CollapseEnvironmentVariables(objRef.Path);
					referenceList.Add(sRow);
				}
				referenceList.Sort();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return referenceList;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the specified path collapsed to contain environment variables where applicable in place of the expanded text.
		/// </summary>
		/// <param name="fullPath">The full path.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string CollapseEnvironmentVariables(string fullPath)
		{
			string[] variables = { "%ProgramFiles(x86)%", "%SystemRoot%", "%UserProfile%", "%Public%", "%AppData%", "%ProgramData%", "%Temp%" };
			fullPath = variables.Aggregate(fullPath, (current, v) 
				=> Regex.Replace(current, Regex.Escape(Environment.ExpandEnvironmentVariables(v)), v, RegexOptions.IgnoreCase));
			fullPath = Regex.Replace(fullPath, Regex.Escape(@"C:\Program Files\"), @"%ProgramFiles%\", RegexOptions.IgnoreCase);
			return fullPath;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full output build path (e.g."C:\Parker\Tucson\PadarnWebCF\Deployment\PadarnServer.exe")
		/// corresponding to the specified project.
		/// </summary>
		/// <param name="prj">The project.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetOutputBuildPath(Project prj)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string outputFullName = String.Empty;
			string absoluteOutputPath = GetOutputBuildFolder(prj);

			if (String.IsNullOrEmpty(absoluteOutputPath))
				return outputFullName;
			try
			{
				Property prop = prj.Properties.Item("OutputFileName");
				if (prop != null)
				{
					string outputFileName = prop.Value.ToString();
					outputFullName = Path.Combine(absoluteOutputPath, outputFileName);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return outputFullName;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the output build folder (e.g."C:\Parker\Tucson\PadarnWebCF\Deployment") corresponding to the
		/// specified project.
		/// </summary>
		/// <param name="prj">The project.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetOutputBuildFolder(Project prj)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string absoluteOutputPath = String.Empty;
            try
			{
				// Get the configuration manager of the project.
				ConfigurationManager configManager = prj.ConfigurationManager;

				if (configManager != null)
				{
					// Get the active project configuration.
					Configuration ac = configManager.ActiveConfiguration;
					// Get the output folder
					string outputPath = ac.Properties.Item("OutputPath").Value.ToString();
					// The output folder can have these patterns:
					// 1) "\\server\folder"
					// 2) "drive:\folder"
					// 3) "..\..\folder"
					// 4) "folder"
					if (outputPath.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)
											  + Path.DirectorySeparatorChar))
					{
						// This is case 1: "\\server\folder"
						absoluteOutputPath = outputPath;
					}
					else if (outputPath.Length >= 2 && outputPath[1] == Path.VolumeSeparatorChar)
					{
						// This is case 2: "drive:\folder"
						absoluteOutputPath = outputPath;
					}
					else
					{
						string projectFolder;

						if (outputPath.IndexOf("..\\", StringComparison.Ordinal) != -1)
						{
							// This is case 3: "..\..\folder"
							projectFolder = Path.GetDirectoryName(prj.FullName);
							while (outputPath.StartsWith("..\\"))
							{
								outputPath = outputPath.Substring(3);
								projectFolder = Path.GetDirectoryName(projectFolder);
							}
							if (projectFolder != null)
								absoluteOutputPath = Path.Combine(projectFolder, outputPath);
						}
						else
						{
							// This is case 4: "folder"
							projectFolder = Path.GetDirectoryName(prj.FullName);
							if (projectFolder != null)
								absoluteOutputPath = Path.Combine(projectFolder, outputPath);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return absoluteOutputPath;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the first project out of all the currently selected projects or null if no projects are selected.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static Project GetSelectedProject()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (Connect.objDte2.ActiveSolutionProjects is Array activeProjects && activeProjects.Length > 0)
				return activeProjects.GetValue(0) as Project;
			return null;
		}
	}
}