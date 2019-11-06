using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace.FindState
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// FirstSearchFailedAndSecondSucceededState
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class FirstSearchFailedAndSecondSucceededState : IFindState
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
            UIHelper.ShowInfo("Back to the start point");
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
				windowSearcher.SelectMatchedText();
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