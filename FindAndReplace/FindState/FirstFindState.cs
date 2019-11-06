using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools.FindAndReplace.FindState
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Summary description for FirstFindState.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class FirstFindState : IFindState
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
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the next.
		/// </summary>
		/// <param name="windowSearcher">The window searcher.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void FindNext(WindowSearcher windowSearcher)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			windowSearcher.SetSearchStartPoint();
			if (windowSearcher.SearchInTextWindowOnce())
			{
				windowSearcher.SelectMatchedText();
				windowSearcher.SetState(new FirstFindSucceededState());
			}
			else
			{
				windowSearcher.MoveToStartInTextWindow();
				windowSearcher.SetState(new FirstFindFailedState());
				windowSearcher.SetLatestFindStartPosToStartOfDoc();
				windowSearcher.FindNext();
			}
		}

		#endregion
	}
}