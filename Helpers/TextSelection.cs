using EnvDTE;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for maintaining and/or restoring the text selection in the Visual Studio editor window.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class MyTextSelection
	{
		private static EditPoint _anchor;
		private static EditPoint _active;
		private static EditPoint _corner;
		private static TextPane _pane;
		private static TextSelection _sel;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the current text selection.
		/// </summary>
		/// <returns>The TextSelection</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static TextSelection Current()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            _pane = (TextPane)Connect.objDte2.ActiveDocument.Object("TextPane");
			_corner = _pane.StartPoint.CreateEditPoint();
			_sel = _pane.Selection;
			_anchor = _sel.AnchorPoint.CreateEditPoint();
			_active = _sel.ActivePoint.CreateEditPoint();
			return _sel;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Restores the original text selection.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void Restore()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            _sel.MoveToPoint(_anchor);
			_sel.MoveToPoint(_active, true);
			_pane.TryToShow(_corner, vsPaneShowHow.vsPaneShowTop);
			_sel.CharRight();
			_sel.CharLeft();
		}
	}
}