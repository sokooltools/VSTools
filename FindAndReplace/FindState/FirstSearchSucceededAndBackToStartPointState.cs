using SokoolTools.VsTools.FindAndReplace.Helper;

namespace SokoolTools.VsTools.FindAndReplace.FindState
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// FirstSearchSucceededAndBackToStartPointState
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class FirstSearchSucceededAndBackToStartPointState : IFindState
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
            UIHelper.ShowInfo("Back to the start point.");
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
				if (windowSearcher.IsGoAcrossStartPoint())
				{
					windowSearcher.SetState(new FirstFindState());
					windowSearcher.ResetLatestFindStartPos();
					ShowMessage();
				}
				else
					windowSearcher.SelectMatchedText();
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