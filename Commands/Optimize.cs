using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods to remove any unused 'Using's and sorts the remaining 'Using's in one or more projects of the current 
	/// solution.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class Optimize
	{
		private static readonly string DIV = new string('-', 90);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes any unused usings and sorts the remaining usings.
		/// </summary>
		/// <remarks>
		/// Select the solution node in the Solution Explorer window to remove and sort all the usings in all projects of the 
		/// solution.
		/// Selecting any other node in the Solution Explorer window will remove and sort the usings at that particular level and 
		/// below.
		/// All results are displayed in the Output window pane.
		/// </remarks>
		//------------------------------------------------------------------------------------------------------------------------
		public static void OptimizeUsings()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Logging.Log(2);

			SelectedItems selectedItems = Connect.DteService.SelectedItems;
			Project project = selectedItems.Item(1).Project;
			OutputPane.Clear();
			int hWnd = NativeMethods.GetTextWindowHandle();
			NativeMethods.LockWindowUpdate(hWnd);
			try
			{
				//var docTable = new RunningDocumentTable(ServiceProvider);
				//List<string> alreadyOpenFiles = docTable.Select(info => info.Moniker).ToList();

				if (project == null)
				{
					ProjectItem projectItem = selectedItems.Item(1).ProjectItem;
					if (projectItem == null)
					{
						if (MessageBox.Show(Resources.OptimizeEntireSolution, @"Optimize Usings",
								MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
							return;
						VsStatusBar.Animation = true;
						VsStatusBar.ColorText = "Optimizing Solution...";
						Write(DIV);

						//--------------------------------------------------------------------------------------------------------
						// Solution Level
						//--------------------------------------------------------------------------------------------------------
						Write("Starting 'Optimize Usings' on entire solution...");
						int totalProjects = Connect.DteService.Solution.Projects.Count;
						for (int i = 1; i <= totalProjects; i++)
						{
							//VsStatusBar.Progress(true, "Optimizing Solution...", i, totalProjects - 1);
							RemoveAndSort(Connect.DteService.Solution.Projects.Item(i));
						}
						Write("Finished 'Optimize Usings' on entire solution...");
					}
					else
					{
						Write(DIV);
						switch (projectItem.Kind)
						{
							//----------------------------------------------------------------------------------------------------
							// Folder Level
							//----------------------------------------------------------------------------------------------------
							case Constants.vsProjectItemKindPhysicalFolder:
								Write("Starting 'Optimize Usings' on folder : " + projectItem.Name);
								RemoveAndSortProjectItem(projectItem);
								Write("Finished 'Optimize Usings' on folder : " + projectItem.Name);
								break;

							//----------------------------------------------------------------------------------------------------
							// File Level
							//----------------------------------------------------------------------------------------------------
							case Constants.vsProjectItemKindPhysicalFile:
								RemoveAndSortProjectItem(projectItem);
								break;
						}
					}
				}
				else
				{
					if (
						MessageBox.Show(Resources.OptimizeEntireProject, @"Optimize Usings", MessageBoxButtons.OKCancel,
							MessageBoxIcon.Question) == DialogResult.Cancel)
						return;
					VsStatusBar.Animation = true;
					VsStatusBar.ColorText = "'Optimize Usings' on entire project...";
					Write(DIV);

					//------------------------------------------------------------------------------------------------------------
					// Project Level
					//------------------------------------------------------------------------------------------------------------
					if (project.Kind == PrjKind.prjKindCSharpProject.ToUpper())
						RemoveAndSort(project);
				}
				Write(DIV);
				OutputPane.Activate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				NativeMethods.LockWindowUpdate(0);
				//VsStatusBar.Progress(false);
				VsStatusBar.Animation = false;
				VsStatusBar.Text = "Done";
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the and sort.
		/// </summary>
		/// <param name="proj">The proj.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void RemoveAndSort(Project proj)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (proj.Name == "Miscellaneous Files") return;
			Write("Starting 'Optimize Usings' on project : " + proj.Name);
			RemoveAndSortProject(proj);
			Write("Finished 'Optimize Usings' on project : " + proj.Name);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the and sort project.
		/// </summary>
		/// <param name="proj">The project.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void RemoveAndSortProject(Project proj)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            for (int i = 1; i <= proj.ProjectItems.Count; i++)
				RemoveAndSortProjectItem(proj.ProjectItems.Item(i));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the and sort project item.
		/// </summary>
		/// <param name="projectItem">The project item.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void RemoveAndSortProjectItem(ProjectItem projectItem)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
			{
				if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFile)
				{
					// If this is a c# file
					if (projectItem.Name.LastIndexOf(".cs", StringComparison.Ordinal) == projectItem.Name.Length - 3)
					{
						// Set flag to true if file is already open
						bool fileIsOpen = projectItem.IsOpen[Constants.vsViewKindCode];

						//Window window = projectItem.Open(Constants.vsViewKindCode)
						Window window = Connect.DteService.OpenFile(Constants.vsViewKindTextView, projectItem.FileNames[0]);
						try
						{
							//DTE.Windows.Item(projectItem.Document.Name).Activate()
							//projectItem.Document.Activate()
							window.Activate();

							Application.DoEvents();

							try
							{
								projectItem.Document.DTE.ExecuteCommand("Edit.RemoveAndSort", String.Empty);
								Write("Finished 'Optimize Usings' on document: " + projectItem.Document.Name);
							}
							catch (COMException)
							{
								Write("Couldn't 'Optimize Usings' on document: " + projectItem.Document.Name);
							}
							// Only close the file if it was not already open
							if (!fileIsOpen)
							{
								try
								{
									window.Close(vsSaveChanges.vsSaveChangesYes);
								}
								catch (Exception)
								{
									Write("Couldn't close document: " + projectItem.Document.Name);
								}
							}
						}
						catch (Exception)
						{
							Write("Couldn't activate document: " + projectItem.Document.Name);
						}
					}
				}
				// Be sure to apply RemoveAndSort on all of the ProjectItems.
				if (projectItem.ProjectItems != null)
				{
					for (int i = 1; i <= projectItem.ProjectItems.Count; i++)
						RemoveAndSortProjectItem(projectItem.ProjectItems.Item(i));
				}
				// Apply RemoveAndSort on a sub-project if it exists.
				if (projectItem.SubProject != null)
					RemoveAndSortProject(projectItem.SubProject);
			}
			catch (Exception)
			{
				Write("Couldn't process document.");
			}
		}

		//..................................................................................................................................

		#region Helper Method

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgText"></param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void Write(string msgText)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var buffer = new StringBuilder();
			buffer.Append(DateTime.Now.ToLongTimeString());
			buffer.Append(" ");
			buffer.Append(msgText);
			buffer.Append("\r\n");
			OutputPane.Write(buffer.ToString());
		}

		#endregion

		//.............................................................................................................
	}
}