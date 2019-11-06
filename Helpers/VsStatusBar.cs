using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable UnusedMember.Global

namespace SokoolTools.VsTools
{
    //--------------------------------------------------------------------------------------------------------
    /// <summary>
    /// VsStatusBar
    /// </summary>
    //--------------------------------------------------------------------------------------------------------
    internal static class VsStatusBar
	{
		// Connect.ApplicationObject.StatusBar.Animate(true, (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_General);
		// Connect.ApplicationObject.StatusBar.Text = "Generating Translation Report... ";
		// Connect.ApplicationObject.StatusBar.Highlight(true);
		// ... ...
		// Connect.ApplicationObject.StatusBar.Text = "Done";
		// Connect.ApplicationObject.StatusBar.Animate(false, (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_General);

		private static object _animationIcon = (short)Constants.SBAI_General;

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the Visual Studio Status bar object.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private static IVsStatusbar StatusBar { get; }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="VsStatusBar"/> class.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		static VsStatusBar()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var oleServiceProvider = Connect.ApplicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
			var svc = new ServiceProvider(oleServiceProvider);
			StatusBar = svc.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows or hides the animated Visual Studio icon in the Animation region.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static bool Animation
		{
			set { ThreadHelper.ThrowIfNotOnUIThread(); StatusBar.Animation(value ? 1 : 0, ref _animationIcon); }
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the text displayed in the Visual Studio Status Bar.
		/// </summary>
		/// <value>The text.</value>
		//----------------------------------------------------------------------------------------------------
		public static string Text
		{
			set
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				if (!IsFrozen)
					StatusBar.SetText(value);
			}
			get
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				StatusBar.GetText(out string text);
				return text;
			}
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the text of the status bar to either be the 'full name of the current document' or the 
		/// 'caption of the current window'.
		/// </summary>
		/// <param name="window">The window.</param>
		//----------------------------------------------------------------------------------------------------
		public static void SetWindowText(EnvDTE.Window window)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Text = window.Document != null ? window.Document.FullName : window.Caption;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the text displayed in the Visual Studio status bar. (The text always being white 
		/// text on a dark blue background.)
		/// </summary>
		/// <value>The color text.</value>
		//----------------------------------------------------------------------------------------------------
		public static string ColorText
		{
			set { ThreadHelper.ThrowIfNotOnUIThread(); StatusBar.SetColorText(value, 0, 0); }
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an indication as to whether the Visual Studio Status bar object is currently frozen.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static bool IsFrozen
		{
			get
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				StatusBar.IsFrozen(out int isFrozen);
				return isFrozen != 0;
			}
		}

		///// private static uint _cookie;
		 
		///// ----------------------------------------------------------------------------------------------------
		///// <summary>
		///// Indicates whether the meter is displayed. Shows or hides the progress meter embedded inside the
		///// Visual Studio status bar.
		///// </summary>
		///// ----------------------------------------------------------------------------------------------------
		///// <param name="inProgress">
		///// If set to <c>true</c>, the meter is turned on. Otherwise, the meter is hidden.
		///// </param>
		///// ----------------------------------------------------------------------------------------------------
		///// public static void Progress(bool inProgress)
		///// {
		/////	StatusBar.Progress(ref _cookie, inProgress ? 1 : 0, "", 0, 0);
		///// }
		 
		///// ----------------------------------------------------------------------------------------------------
		///// <summary>
		///// Shows or hides the progress meter embedded inside the Visual Studio status bar and updates the meter
		///// using the specified parameters.
		///// </summary>
		///// <param name="inProgress">
		///// Indicates whether the meter is displayed. If set to <c>true</c>, the meter is turned on. Otherwise, 
		///// the meter is hidden.
		///// </param>
		///// <param name="label">The label to display together with the meter control.</param>
		///// <param name="stepsCompleted">
		///// The number of steps of the operation that have completed.
		///// </param>
		///// <param name="totalSteps">The total number of steps in the operation.</param>
		///// ----------------------------------------------------------------------------------------------------
		//public static void Progress(bool inProgress, string label, int stepsCompleted, int totalSteps)
		//{
		//    StatusBar.Progress(ref _cookie, inProgress ? 1 : 0, label, (uint)stepsCompleted, (uint)totalSteps);
		//}
	}
}