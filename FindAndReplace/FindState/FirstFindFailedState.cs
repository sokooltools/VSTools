using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace.FindState
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// FirstFindFailedState
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class FirstFindFailedState : IFindState
	{

		//..................................................................................................................................

		#region IFindState Members

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the message.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void ShowMessage()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            UIHelper.ShowInfo("Didn't find the specified text.");
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next.
		/// </summary>
		/// <param name="windowSearcher">The window searcher.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void FindNext(WindowSearcher windowSearcher)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (windowSearcher.SearchInTextWindowOnce())
			{
				windowSearcher.SelectMatchedText();
				windowSearcher.SetState(new FirstSearchFailedAndSecondSucceededState());
			}
			else
			{
				windowSearcher.ResetLatestFindStartPos();
				windowSearcher.SetState(new FirstFindState());
				ShowMessage();
			}
		}

		#endregion

	}
}