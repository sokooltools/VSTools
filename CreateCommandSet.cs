using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SokoolTools.VsTools.FindAndReplace;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Command handler
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal sealed class CreateCommandSet : IDisposable
	{
		//..................................................................................................................................

		#region Private Fields

		private static readonly Guid VsToolsCmdSetGuid = new Guid("19492bcb-32b3-4ec3-8826-d67cd5526653");
		private readonly OleMenuCommandService _mcs;

		private static MySolutionEventsHandler MySolutionEventsHandler { get; set; }

		private static OleMenuCommand _dynamicVisibilityCommand1;
		private static OleMenuCommand _dynamicVisibilityCommand2;

		#endregion

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private readonly AsyncPackage _package;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private IServiceProvider ServiceProvider => _package;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the instance of this object.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static CreateCommandSet Instance { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <remarks>
		/// Gets the OleCommandService object provided by the MPF; this object is the one responsible for handling the 
		/// collection of commands implemented by the package.
		/// </remarks>
		/// <param name="package">Owner package, not null.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in the constructor requires the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			var menuCommandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new CreateCommandSet(package, menuCommandService);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCommandSet" /> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="menuCommandService">The menu command service.</param>
		/// <exception cref="ArgumentNullException">package or commandService</exception>
		//------------------------------------------------------------------------------------------------------------------------
		private CreateCommandSet(AsyncPackage package, OleMenuCommandService menuCommandService)
		{
			_package = package ?? throw new ArgumentNullException(nameof(package));

			_mcs = menuCommandService ?? throw new ArgumentNullException(nameof(menuCommandService));

			//--------------------------------------------------------------------------------------------------------------------
			// Create one object derived from OleMenuCommand for each command defined in the VSCT file and add it to the menu 
			// command service.
			// For each command define an id that is a unique menu group guid along with a command integer.
			// Now create the OleMenuCommand object for this command. The EventHandler object is the function that will be called 
			// when the user selects the command.
			//--------------------------------------------------------------------------------------------------------------------
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
			Type type = typeof(PkgIds);
			IEnumerable<FieldInfo> fields = type.GetFields(flags).Where(f => f.IsLiteral && !f.IsInitOnly);
			foreach (FieldInfo f in fields)
			{
				var mnuCmdId = new CommandID(VsToolsCmdSetGuid, (int)f.GetValue(null));
				var menuItem = new OleMenuCommand(MenuCommandCallback, null, MenuItem_BeforeQueryStatus, mnuCmdId);
				_mcs.AddCommand(menuItem);
			}
			
			ThreadHelper.ThrowIfNotOnUIThread();

			AddDynamicCommand();

			SetupSolutionEvents();
		}

		//..................................................................................................................................

		#region Private Helper Methods

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets up the solution events.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void SetupSolutionEvents()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// Create the handler used to receive the solution events.
			if (ServiceProvider.GetService(typeof(SVsSolution)) is IVsSolution solution)
				MySolutionEventsHandler = new MySolutionEventsHandler(solution);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the BeforeQueryStatus event of the MenuItem control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (!(sender is OleMenuCommand myCommand))
				return;
			bool isActiveTextView = TextView.GetActiveTextView() != null;
			switch (myCommand.CommandID.ID)
			{
				case PkgIds.VSToolsMenu:
					break;
				case PkgIds.TogglePublish:
				case PkgIds.SolutionExplorerCollapse:
				case PkgIds.ShowProjectReferences:
				case PkgIds.ShowSolutionBuild:
				case PkgIds.GenerateTranslationReport:
				case PkgIds.RegexFindAndReplace:
				case PkgIds.ExternalTools1:
				case PkgIds.ExternalTools2:
				case PkgIds.Options:
				case PkgIds.OptimizeUsings:
				case PkgIds.About:
					break;
				case PkgIds.FormatComments:
				case PkgIds.RemoveAllDividerLines:
				case PkgIds.RegionsToggleCurrent:
				case PkgIds.RegionsExpandAll:
				case PkgIds.RegionsCollapseAll:
				case PkgIds.BookmarksCut:
				case PkgIds.BookmarksCopy:
				case PkgIds.BookmarksDelete:
				case PkgIds.SummariesCollapseAll:
				case PkgIds.SummariesExpandAll:
				case PkgIds.CodeBlockCollapse:
				case PkgIds.CodeBlockExpand:
				case PkgIds.NunitToMsTest:
				case PkgIds.MsTestToNunit:
				case PkgIds.UrlsEncode:
				case PkgIds.UrlsDecode:
				case PkgIds.JavascriptCompact:
				case PkgIds.JavascriptFormat:
				case PkgIds.PasteAsComments:
				case PkgIds.LineupDeclarations:
				case PkgIds.SortSelectedLines:
					myCommand.Enabled = isActiveTextView;
					break;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		void IDisposable.Dispose()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			MySolutionEventsHandler?.Dispose();
		}

		#endregion

		//..................................................................................................................................

		#region Menu Callbacks

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Event handler called when the user selects a command.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void MenuCommandCallback(object sender, EventArgs args)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			
			// Check that the type of caller is expected.
			if (!(sender is OleMenuCommand command))
				return;

			// Now check that its guid corresponds to this command set.
			if (command.CommandID.Guid != VsToolsCmdSetGuid)
				return;

			switch (command.CommandID.ID)
			{
				case PkgIds.FormatComments:
					Comments.FormatComments();
					break;
				case PkgIds.RemoveAllDividerLines:
					Dividers.RemoveAllDividerLines();
					break;
				case PkgIds.SolutionExplorerCollapse:
					SolutionExplorer.CollapseAll();
					break;
				case PkgIds.RegionsToggleCurrent:
					Regions.RegionsToggleCurrent();
					break;
				case PkgIds.RegionsExpandAll:
					Regions.RegionsExpandAll();
					break;
				case PkgIds.RegionsCollapseAll:
					Regions.RegionsCollapseAll();
					break;
				case PkgIds.BookmarksCut:
					Bookmarks.ProcessBookmarks(Bookmarks.ProcessBookmarksEnum.Cut);
					break;
				case PkgIds.BookmarksCopy:
					Bookmarks.ProcessBookmarks(Bookmarks.ProcessBookmarksEnum.Copy);
					break;
				case PkgIds.BookmarksDelete:
					Bookmarks.ProcessBookmarks(Bookmarks.ProcessBookmarksEnum.Delete);
					break;
				case PkgIds.SummariesCollapseAll:
					Regions.CollapseAllSummaries();
					break;
				case PkgIds.SummariesExpandAll:
					Regions.ExpandAllSummaries();
					break;
				case PkgIds.CodeBlockCollapse:
					PropertyBlock.Collapse();
					break;
				case PkgIds.CodeBlockExpand:
					PropertyBlock.Expand();
					break;
				case PkgIds.NunitToMsTest:
					NUnitToMSTest.Perform();
					break;
				case PkgIds.MsTestToNunit:
					MSTestToNUnit.Perform();
					break;
				case PkgIds.UrlsEncode:
					UrlCode.UrlEncodeOrDecode(UrlCode.UrlEncodeOrDecodeEnum.Encode);
					break;
				case PkgIds.UrlsDecode:
					UrlCode.UrlEncodeOrDecode(UrlCode.UrlEncodeOrDecodeEnum.Decode);
					break;
				case PkgIds.JavascriptFormat:
					Javascript.FormatOrCompact(Javascript.FormatOrCompactEnum.Format);
					break;
				case PkgIds.JavascriptCompact:
					Javascript.FormatOrCompact(Javascript.FormatOrCompactEnum.Compact);
					break;
				case PkgIds.OptimizeUsings:
					Optimize.OptimizeUsings();
					break;
				case PkgIds.ShowProjectReferences:
					ProjectStuff.ShowProjectReferences();
					break;
				case PkgIds.ShowSolutionBuild:
					SolutionStuff.ShowSolutionBuildConfigurations();
					break;
				case PkgIds.TogglePublish:
					Framework.TogglePublish();
					break;
				case PkgIds.GenerateTranslationReport:
					Translation.GenerateReport();
					break;
				case PkgIds.PasteAsComments:
					TextEditor.PasteTextAsComments();
					break;
				case PkgIds.LineupDeclarations:
					Docs.LineUpVariableDeclarations();
					break;
				case PkgIds.SortSelectedLines:
					Docs.SortSelectedLines();
					break;
				case PkgIds.RegexFindAndReplace:
					new SearchForm(Connect.ApplicationObject).Show();
					break;
				case PkgIds.ExternalTools1:
					ExternalTools.Run(1);
					break;
				case PkgIds.ExternalTools2:
					ExternalTools.Run(2);
					break;
				case PkgIds.Options:
					using (var frm = new OptionsDialog())
						frm.ShowDialog();
					break;
				case PkgIds.About:
					using (var frm = new AboutDialog())
						frm.ShowDialog();
					break;
				case PkgIds.MyGraph:
					OutputString("Graph Command Callback.");
					break;
				case PkgIds.MyZoom:
					OutputString("Zoom Command Callback.");
					break;
				case DynPkgIds.DynVisibility1:
					// The user clicked on the first one; make it invisible and show the second one.
					_dynamicVisibilityCommand1.Visible = false;
					_dynamicVisibilityCommand2.Visible = true;
					break;
				case DynPkgIds.DynVisibility2:
					// The user clicked on the second one; make it invisible and show the first one.
					_dynamicVisibilityCommand2.Visible = false;
					_dynamicVisibilityCommand1.Visible = true;
					break;
			}
		}

		#endregion

		//..................................................................................................................................

		#region Dynamic Command Menu

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds a dynamic command to the VSTools menu.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void AddDynamicCommand()
		{
			// Create the DynamicMenuCommand object for the command defined with the TextChanges flag.
			var mnuCmdId = new CommandID(VsToolsCmdSetGuid, DynPkgIds.DynamicTxt);
			OleMenuCommand menuItem = new DynamicTextCommand(mnuCmdId, Resources.DynamicTextBaseText);
			menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
			_mcs.AddCommand(menuItem);

			// Now create two OleMenuCommand objects for the two commands with dynamic visibility.
			mnuCmdId = new CommandID(VsToolsCmdSetGuid, DynPkgIds.DynVisibility1);
			_dynamicVisibilityCommand1 = new OleMenuCommand(MenuCommandCallback, mnuCmdId);
			_mcs.AddCommand(_dynamicVisibilityCommand1);

			// This command is the one that is invisible by default, so its visble property must be set to false   
			// because the default value of this property for every object which is derived from MenuCommand is true.
			mnuCmdId = new CommandID(VsToolsCmdSetGuid, DynPkgIds.DynVisibility2);
			_dynamicVisibilityCommand2 = new OleMenuCommand(MenuCommandCallback, mnuCmdId) { Visible = false };
			_mcs.AddCommand(_dynamicVisibilityCommand2);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// This function prints text on the debug output and on the generic pane of the Output window.
		/// </summary>
		/// <param name="text"></param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void OutputString(string text)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			OutputPane.WriteLine(" ================================================");
			OutputPane.WriteLine($"  VSTools: {text}");
			OutputPane.WriteLine(" ================================================");
			OutputPane.WriteLine("");
		}

		#endregion

		//..................................................................................................................................

		#region Currently Unused

		////------------------------------------------------------------------------------------------------------------------------
		///// <summary>
		///// Enables (or disables) a command corresponding to the specified command id.
		///// </summary>
		///// <param name="cmdId">The command identifier.</param>
		///// <param name="isEnabled">
		///// When set to <c>false</c> the command is disabled; otherwise the command is enabled by default.
		///// </param>
		///// <exception cref="KeyNotFoundException"></exception>
		////------------------------------------------------------------------------------------------------------------------------
		//public void EnableCommand(int cmdId, bool isEnabled = true)
		//{
		//	MenuCommand mc = _mcs.FindCommand(new CommandID(VsToolsCmdSetGuid, cmdId));
		//	if (mc == null)
		//		throw new KeyNotFoundException($"The menu associated with CommandID: {cmdId} could not be found.");
		//	mc.Enabled = isEnabled;
		//}

		//private static string GetActiveDocumentFilePath()
		//{
		//	ThreadHelper.ThrowIfNotOnUIThread();
		//	return Connect.ApplicationObject?.ActiveDocument.FullName;
		//}

		//private static void SetupWindowEvents()
		//{
		//	Connect.ApplicationObject?.Events.WindowEvents.WindowActivated += OnWindowActivated;
		//}

		//private static void OnWindowActivated(Window gotFocus, Window lostFocus)
		//{
		//  ThreadHelper.ThrowIfNotOnUIThread();
		//  if (gotFocus != null && lostFocus != null)
		//		OutputPane.WriteLine($@"gotFocus: {gotFocus.Caption}; lostFocus: {lostFocus.Caption}");
		//}

		//private void ShowAddDocumentationWindow(string activeDocumentPath, TextViewSelection selection)
		//{
		//	OutputPane.Activate();
		//	OutputPane.Clear();
		//	OutputPane.Write(activeDocumentPath);
		//	OutputPane.Write(selection.Text);
		//	//Debug.WriteLine(activeDocumentPath);
		//	//Debug.WriteLine(selection.Text);
		//	//var documentationControl = new AddDocumentationWindow();
		//	//documentationControl.DataContext = new EditDocumentationViewModel(activeDocumentPath, selection);
		//	//documentationControl.ShowDialog();
		//}

		//private TextViewSelection GetSelection(IServiceProvider serviceProvider)
		//{
		//	object service = serviceProvider.GetService(typeof(SVsTextManager));
		//	if (!(service is IVsTextManager2 textManager))
		//		return new TextViewSelection();
		//	textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out IVsTextView view);
		//	view.GetSelection(out int startLine, out int startColumn, out int endLine, out int endColumn); // end could be before beginning
		//	var start = new TextViewPosition(startLine, startColumn);
		//	var end = new TextViewPosition(endLine, endColumn);
		//	view.GetSelectedText(out string selectedText);
		//	var selection = new TextViewSelection(start, end, selectedText);
		//	return selection;
		//}

		#endregion
	}
}
