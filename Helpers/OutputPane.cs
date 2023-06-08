using System;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for activating and/or writing to the Visual Studio Output window Pane.
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	internal static class OutputPane
	{
		private static readonly OutputWindowPane _oWp;

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the <see cref="OutputPane"/> class.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		static OutputPane()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			var ow = (OutputWindow)Connect.objDte2.Windows.Item(Constants.vsWindowKindOutput).Object;
			try
			{
				_oWp = ow.OutputWindowPanes.Item(Vsix.Name);
			}
			catch
			{
				_oWp = ow.OutputWindowPanes.Add(Vsix.Name);
			}
		}

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the specified text to the end of the output window pane.
		/// </summary>
		/// <param name="msgText"></param>
		//--------------------------------------------------------------------------------------------------------------
		internal static void Write(string msgText)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.OutputString(msgText);
		}

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the specified line of text to the end of the output window pane.
		/// </summary>
		/// <param name="msgText"></param>
		//--------------------------------------------------------------------------------------------------------------
		internal static void WriteLine(string msgText)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.OutputString(msgText + Environment.NewLine);
		}

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Clears all the text from the output window pane.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		internal static void Clear()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Activate();
			_oWp.Clear();
		}

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		internal static void Activate()
		{
			const string StandardCommandSet2K_guid_string = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";
			const int OutputPaneCombo = 1627;
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.Activate();
			object customIn = Vsix.Name;
			object customOut = null;
			Connect.objDte2.Windows.Item(Constants.vsWindowKindOutput).Activate();
			Connect.objDte2.Commands.Raise(StandardCommandSet2K_guid_string, OutputPaneCombo, ref customIn, ref customOut);
		}
	}
}