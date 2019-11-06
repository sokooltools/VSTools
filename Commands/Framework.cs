using System;
using System.Windows;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for accessing and controlling the 'Visual Studio Framework'.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal class Framework
	{
		private static readonly Lazy<Framework> Lazy = new Lazy<Framework>(() => new Framework());

		public static Framework Instance => Lazy.Value;

		private Framework() { }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Toggles the publish button located on the status bar.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void TogglePublish()
		{
			Logging.Log();
			FrameworkElement e = FindElement(Application.Current.MainWindow, "PublishCompartment");
			if (e != null)
				e.Visibility = e.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Toggles the publish button located on the status bar.
		/// </summary>
		/// <param name="isCollapsed">
		/// if set to <c>true</c> visibility is collapsed otherwise visibility is visible.
		/// </param>
		//----------------------------------------------------------------------------------------------------
		public void HidePublish(bool isCollapsed)
		{
			Logging.Log();
			FrameworkElement e = FindElement(Application.Current.MainWindow, "PublishCompartment");
			if (e != null)
				e.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Hides the sign in smiley button.
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public bool HideSignIn()
		{
			Logging.Log();
			FrameworkElement e = FindElement(Application.Current.MainWindow, "PART_MenuBarFrameControlContainer");
			if (e == null)
				return false;
			e.Visibility = Visibility.Collapsed;
			return true;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Hides the feedback smilely button.
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public bool HideFeedback()
		{
			Logging.Log();
			FrameworkElement e = FindElement(Application.Current.MainWindow, "PART_TitleBarFrameControlContainer");
			if (e == null)
				return false;
			DependencyObject o1 = System.Windows.Media.VisualTreeHelper.GetChild(e, 0);
			DependencyObject o2 = System.Windows.Media.VisualTreeHelper.GetChild(o1, 0);
			DependencyObject o3 = System.Windows.Media.VisualTreeHelper.GetChild(o2, 0);
			if (System.Windows.Media.VisualTreeHelper.GetChildrenCount(o3) != 3)
				return false;
			DependencyObject o4 = System.Windows.Media.VisualTreeHelper.GetChild(o3, 1);
			e = o4 as FrameworkElement;
			if (e != null)
				e.Visibility = Visibility.Collapsed;
			return true;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Hides the title bar.
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public bool HideTitleBar()
		{
			Logging.Log();
			FrameworkElement e = FindElement(Application.Current.MainWindow, "MainWindowTitleBar");
			if (e == null)
				return false;
			e.Visibility = Visibility.Collapsed;
			return true;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the framework element.
		/// </summary>
		/// <param name="parent">The dependency object.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		private static FrameworkElement FindElement(DependencyObject parent, string name)
		{
			if (parent == null)
				return null;
			for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i) as System.Windows.Media.Visual;
				var fe = child as FrameworkElement;
				if (fe != null && fe.Name == name)
					return fe;
				fe = FindElement(child, name);
				if (fe != null)
					return fe;
			}
			return null;
		}

		//public static void SetSite(EnvDTE80.DTE2 dte, Microsoft.VisualStudio.Shell.Package package)
		//{
		//	DTE = dte;
		//	events = DTE.Events;
		//	debuggerEvents = events.DebuggerEvents;
		//	debuggerEvents.OnEnterRunMode += OnEnterRunMode;
		//}

		//public static void Close()
		//{
		//	debuggerEvents.OnEnterRunMode -= OnEnterRunMode;
		//}

		//private static void OnEnterRunMode(EnvDTE.dbgEventReason reason)
		//{
		//	try
		//	{
		//		if (reason == EnvDTE.dbgEventReason.dbgEventReasonGo)
		//			DTE.MainWindow.WindowState = EnvDTE.vsWindowState.vsWindowStateMinimize;
		//	}
		//	catch
		//	{
		//	}
		//}

		//private static EnvDTE80.DTE2 DTE;
		//private static  EnvDTE.Events events;
		//private static  EnvDTE.DebuggerEvents debuggerEvents;
	}
}
