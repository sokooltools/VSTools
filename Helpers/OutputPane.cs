using System;
using EnvDTE;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for activating and/or writing to the Visual Studio Output window Pane.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class OutputPane
	{
		private static OutputWindowPane _oWp = GetOutputWindowPane();

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgText"></param>
		//----------------------------------------------------------------------------------------------------
		internal static void Write(string msgText)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.OutputString(msgText);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgText"></param>
		//----------------------------------------------------------------------------------------------------
		internal static void WriteLine(string msgText)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.OutputString(msgText + Environment.NewLine);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		internal static void Clear()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.Clear();
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		internal static void Activate()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_oWp.Activate();
			object customIn = "VsTools";
			object customOut = null;
			Connect.ApplicationObject.Windows.Item(Constants.vsWindowKindOutput).Activate();
			Connect.ApplicationObject.Commands.Raise("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 1627, ref customIn, ref customOut);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		private static OutputWindowPane GetOutputWindowPane()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			var ow = (OutputWindow)Connect.ApplicationObject.Windows.Item(Constants.vsWindowKindOutput).Object;
			try
			{
				_oWp = ow.OutputWindowPanes.Item("VsTools");
			}
			catch
			{
				_oWp = ow.OutputWindowPanes.Add("VsTools");
			}
			return _oWp;
		}
	}
}