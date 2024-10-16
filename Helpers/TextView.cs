using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SokoolTools.VsTools
{
	internal static class TextView
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the active view.
		/// </summary>
		/// <param name="windowFrame">The window frame.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">windowFrame</exception>
		//------------------------------------------------------------------------------------------------------------------------
		public static IVsTextView GetActiveView(IVsWindowFrame windowFrame)
		{
			if (windowFrame == null)
				throw new ArgumentException(nameof(windowFrame));

            ThreadHelper.ThrowIfNotOnUIThread();

			ErrorHandler.ThrowOnFailure(windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object pVar));

			var textView = pVar as IVsTextView;
			if (textView != null)
				return textView;

			if (pVar is IVsCodeWindow codeWin)
				ErrorHandler.ThrowOnFailure(codeWin.GetLastActiveView(out textView));
			return textView;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Find the active text view (if any) in the active document.
		/// </summary>  
		/// <returns>
		/// The IVsTextView of the active view, or null if there is no active document or the active view if the active document 
		/// is not a text view.
		/// </returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static IVsTextView GetActiveTextView()
		{
            ThreadHelper.ThrowIfNotOnUIThread();

			object frameObj = null;
			if (ServiceProvider.GlobalProvider.GetService(typeof(IVsMonitorSelection)) is IVsMonitorSelection selection)
				ErrorHandler.ThrowOnFailure(selection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_DocumentFrame, out frameObj));

			return frameObj is IVsWindowFrame frame ? GetActiveView(frame) : null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the current editor column.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static int GetCurrentEditorColumn()
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView view = GetActiveTextView();
			if (view == null)
				return -1;
			try
			{
				IWpfTextView textView = GetTextViewFromVsTextView(view);
				int column = GetCaretColumn(textView);
				//----------------------------------------------------------------------------------------------------------------
				// Note: GetCaretColumn returns 0-based positions. Guidelines are 1-based positions. However, do not subtract one 
				// here since the caret is positioned to the left of the given column and the guidelines are positioned to the 
				// right. We want the guideline to line up with the current caret position. e.g. When the caret is at position 1 
				// (zero-based), the status bar says column 2. We want to add a guideline for column 1 since that will place the 
				// guideline where the caret is. 
				//----------------------------------------------------------------------------------------------------------------
				return column;
			}
			catch (InvalidOperationException)
			{
				return -1;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the IWpfTextView from the IVsTextView.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">view</exception>
		/// <exception cref="InvalidOperationException"></exception>
		//------------------------------------------------------------------------------------------------------------------------
		private static IWpfTextView GetTextViewFromVsTextView(IVsTextView view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));

			// ReSharper disable once SuspiciousTypeConversion.Global
			if (!(view is IVsUserData userData))
				throw new InvalidOperationException();

			if (userData.GetData(DefGuidList.guidIWpfTextViewHost, out object objTextViewHost) != VSConstants.S_OK)
				throw new InvalidOperationException();

			if (!(objTextViewHost is IWpfTextViewHost textViewHost))
				throw new InvalidOperationException();

			return textViewHost.TextView;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Given an IWpfTextView, find the position of the caret and report its column number. The column number is 0-based
		/// </summary>  
		/// <param name="textView">The text view containing the caret</param>  
		/// <returns>
		/// The column number of the caret's position. When the caret is at the leftmost column, the return value is zero.
		/// </returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static int GetCaretColumn(IWpfTextView textView)
		{
			// This is the code the editor uses to populate the status bar.  
			ITextViewLine caretViewLine = textView.Caret.ContainingTextViewLine;
			double columnWidth = textView.FormattedLineSource.ColumnWidth;
			return (int)Math.Round((textView.Caret.Left - caretViewLine.Left) / columnWidth);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determine the applicable column number for an add or remove command. The column is parsed from command arguments, if 
		/// present, otherwise the current position of the caret is used to determine the column.
		/// </summary>  
		/// <param name="e">Event args passed to the command handler.</param>  
		/// <returns>The column number. May be negative to indicate the column number is unavailable.</returns>  
		/// <exception cref="ArgumentException">The column number parsed from event args was not a valid integer.</exception>
		//------------------------------------------------------------------------------------------------------------------------
#pragma warning disable IDE0051
		// ReSharper disable once UnusedMember.Local
		private static int GetApplicableColumn(EventArgs e)
#pragma warning restore IDE0051
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            string inValue = ((OleMenuCmdEventArgs)e).InValue as string;
			if (String.IsNullOrEmpty(inValue))
				return GetCurrentEditorColumn();
			if (!Int32.TryParse(inValue, out int column) || column < 0)
				throw new ArgumentException("Invalid column");
			return column;
		}
	}
}
