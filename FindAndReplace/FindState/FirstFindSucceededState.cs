using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools.FindAndReplace.FindState
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// FirstFindSucceededState
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class FirstFindSucceededState : IFindState
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
			if (windowSearcher.SearchInTextWindowOnce())
				windowSearcher.SelectMatchedText();
			else
			{
				windowSearcher.SetState(new FirstSearchSucceededAndBackToStartPointState());
				windowSearcher.MoveToStartInTextWindow();
				windowSearcher.SetLatestFindStartPosToStartOfDoc();
				windowSearcher.FindNext();
			}
		}

		#endregion
	}
}