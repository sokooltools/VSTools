using System.Drawing;
using System.Runtime.InteropServices;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides access to the Native Methods part of the standard Win32 Api.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class NativeMethods
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the window.
		/// </summary>
		/// <param name="lpClassName">Name of the class.</param>
		/// <param name="lpWindowName">Name of the window.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		[DllImport("user32", EntryPoint = "FindWindowA", CharSet = CharSet.Unicode)]
		private static extern int FindWindow(string lpClassName, string lpWindowName);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the window (extended version).
		/// </summary>
		/// <param name="hWnd1">The 1st window handle.</param>
		/// <param name="hWnd2">The 2nd window handle.</param>
		/// <param name="lpsz1">The 1st string.</param>
		/// <param name="lpsz2">The 2nd string.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		[DllImport("user32", EntryPoint = "FindWindowExA", CharSet = CharSet.Unicode)]
		private static extern int FindWindowEx(int hWnd1, int hWnd2, string lpsz1, string lpsz2);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Locks the window update.
		/// </summary>
		/// <param name="hwndLock">The HWND lock.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		[DllImport("user32")]
		public static extern int LockWindowUpdate(int hwndLock);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCaretPos(out Point lpPoint);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the handle of the current text window.
		/// </summary>
		/// <returns>the window handle</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static int GetTextWindowHandle()
		{
			int hwd = FindWindow("wndclass_desked_gsk", null);
			hwd = FindWindowEx(hwd, 0, "MDIClient", null);
			return hwd;
		}
	}
}