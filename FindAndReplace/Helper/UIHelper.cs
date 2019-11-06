namespace SokoolTools.VsTools.FindAndReplace.Helper
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// UIHelper
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class UIHelper
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the info.
		/// </summary>
		/// <param name="info">The info.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public static void ShowInfo(string info)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            VsStatusBar.ColorText = info;
		}
	}
}