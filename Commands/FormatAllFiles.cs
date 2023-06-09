using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools
{
	internal static class FormatAllFiles
	{
		#region Public Methods

		public static void Execute()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var generalOption = new GeneralOption();

			IEnumerable<ProjectItem> allItems = GetSelectedProjectItems(Connect.objDte2.ToolWindows.SolutionExplorer, generalOption.CreateHierarchyFilter());

			Func<string, bool> fileFilter = generalOption.CreateFileFilterFunc();

			var projectItems = new List<ProjectItem>();
			foreach (ProjectItem item in allItems)
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				if (item.Kind == VSConstants.ItemTypeGuid.PhysicalFile_string && fileFilter(arg: item.Name))
					projectItems.Add(item);
			}

			int numFiles = projectItems.Count;
			int numFailed = 0;
			IList<string> commands = generalOption.GetCommands();
			StatusBar statusBar = Connect.objDte2.StatusBar;
			OutputPane.Clear();
			OutputPane.WriteLine($"{DateTime.Now:T} Started. ({numFiles} files)");
			for (int index = 0; index < numFiles; index++)
			{
				ProjectItem projectItem = projectItems[index];
				OutputPane.WriteLine($"Formatting: {(projectItem.FileCount != 0 ? projectItem.FileNames[1] : projectItem.Name)}");
				statusBar.Progress(true, String.Empty, index + 1, numFiles);
				if (!ExecuteCommand(projectItem, commands))
					++numFailed;
			}
			OutputPane.WriteLine($"{DateTime.Now:T} Finished. ({numFiles - numFailed} success. {numFailed} failure.)");
			statusBar.Progress(false);
			statusBar.Text = $"Format All Files is finished. ({numFiles} files)";
		}

		#endregion

		#region Private Methods

		private static bool ExecuteCommand(ProjectItem item, IEnumerable<string> commands)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			bool result = false;
			bool isOpen = item.IsOpen[ViewKind:"{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}"];
			if (!isOpen)
			{
				// Try to open the text document.
				try
				{
					item.Open("{7651A703-06E5-11D1-8EBD-00A0C90F26EA}"); // LogicalView.Text
				}
				catch
				{
					OutputPane.WriteLine("This is not text file.");
				}
			}
			Document document = item.Document;
			if (document == null)
				return false;
			try
			{
				document.Activate();
				foreach (string command in commands)
				{
					try
					{
						item.DTE.ExecuteCommand(command);
						result = true;
					}
					catch (COMException ex)
					{
						OutputPane.WriteLine(ex.Message);
					}
				}
			}
			finally
			{
				// Just save the document when it was already open.
				if (isOpen)
					document.Save();
				// Otherwise save the document then close it.
				else
					document.Close(vsSaveChanges.vsSaveChangesYes);
			}
			return result;
		}

		private static IEnumerable<ProjectItem> GetProjectItems(Project project, Func<string, bool> filter)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return GetProjectItems(project.ProjectItems, filter);
		}

		private static IEnumerable<ProjectItem> GetProjectItems(ProjectItem item, Func<string, bool> filter)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IEnumerable<ProjectItem> projectItems = GetProjectItems(item.ProjectItems, filter);
			return item.Kind != VSConstants.ItemTypeGuid.PhysicalFile_string ? projectItems : new[] { item }.Concat(projectItems);
		}

		private static  IEnumerable<ProjectItem> GetProjectItems(IEnumerable items, Func<string, bool> filter)
			=> items.OfType<ProjectItem>().Recursive(x =>
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				ProjectItems projectItems = x.ProjectItems;
				if (projectItems != null)
				{
					if (filter(x.Name))
						return projectItems.OfType<ProjectItem>();
				}
				else
				{
					Project subProject = x.SubProject;
					if (subProject != null)
						return GetProjectItems(subProject, filter);
				}
				return Enumerable.Empty<ProjectItem>();
			});

		private static IEnumerable<ProjectItem> GetSelectedProjectItems(UIHierarchy solutionExplorer, Func<string, bool> filter)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return solutionExplorer.SelectedItems is object[] selectedItems
				? selectedItems.OfType<UIHierarchyItem>().SelectMany(x => GetSelectedProjectItems(x, filter))
				: Enumerable.Empty<ProjectItem>();
		}

		private static IEnumerable<ProjectItem> GetSelectedProjectItems(UIHierarchyItem hierarchyItem, Func<string, bool> filter)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			switch (hierarchyItem.Object)
			{
				case Solution solution:
					return solution.Projects.OfType<Project>().SelectMany(x => GetProjectItems(x, filter));
				case Project project:
					return GetProjectItems(project, filter);
				case ProjectItem projectItem:
					return GetProjectItems(projectItem, filter);
				default:
					return Enumerable.Empty<ProjectItem>();
			}
		}

		#endregion
	}
}
