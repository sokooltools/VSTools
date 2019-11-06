using System.Linq;
using System.Windows.Forms;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Used for performing actions (such as expanding or collapsing all nodes) in the Visual Studi Solution Explorer window.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class SolutionExplorer
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses all the nodes in the solution explorer window.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void CollapseAll()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Logging.Log();

			// Get the the Solution Explorer tree
			var solutionExplorer =
				(UIHierarchy) Connect.ApplicationObject.Windows.Item(Constants.vsWindowKindSolutionExplorer).Object;

			// Check if there is an open solution.
			if (solutionExplorer.UIHierarchyItems.Count == 0)
			{
				MessageBox.Show(Resources.NothingToCollapse);
				return;
			}
			// Get the top node (the name of the solution).
			UIHierarchyItem solutionRootNode = solutionExplorer.UIHierarchyItems.Item(1);
			solutionRootNode.DTE.SuppressUI = true;
			// Collapse each project node
			foreach (UIHierarchyItem uihItem in solutionRootNode.UIHierarchyItems)
			{
				if (uihItem.UIHierarchyItems.Expanded)
					Collapse(uihItem, ref solutionExplorer);
				uihItem.UIHierarchyItems.Expanded = false;
			}
			// Select the solution node, or else when you click on the solution window
			// scrollbar, it will synchronize the open document with the tree and pop
			// out the corresponding node which is probably not what we want.
			solutionRootNode.Select(vsUISelectionType.vsUISelectionTypeSelect);
			solutionRootNode.DTE.SuppressUI = false;

			if (solutionRootNode.DTE.ActiveDocument == null)
				return;

			solutionRootNode.DTE.ExecuteCommand("View.TrackActivityInSolutionExplorer", string.Empty);
			solutionRootNode.DTE.ExecuteCommand("View.TrackActivityInSolutionExplorer", string.Empty);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses the specified UI Hierarchy Item and all of its children.
		/// </summary>
		/// <param name="uihItem">Hierarchical tree data</param>
		/// <param name="solutionExplorer"></param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void Collapse(UIHierarchyItem uihItem, ref UIHierarchy solutionExplorer)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (UIHierarchyItem eItem in uihItem.UIHierarchyItems.Cast<UIHierarchyItem>()
						.Where(eItem => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return eItem.UIHierarchyItems.Count > 0; }))
			{
				Collapse(eItem, ref solutionExplorer);
				if (!eItem.UIHierarchyItems.Expanded) continue;
				eItem.UIHierarchyItems.Expanded = false;
				if (!eItem.UIHierarchyItems.Expanded) continue;
				// Resolves a _bug in VS 2005-2008!
				eItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
				solutionExplorer.DoDefaultAction();
			}
		}
	}
}