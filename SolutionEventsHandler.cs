using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="IVsSolutionEvents" />
	//----------------------------------------------------------------------------------------------------------------------------
	internal class SolutionEventsHandler : IVsSolutionEvents
	{
		public int OnAfterCloseSolution(object pUnkReserved)
		{
			HandleSolutionEvent("OnAfterCloseSolution");
			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			HandleSolutionEvent("OnAfterLoadProject");
			return VSConstants.S_OK;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			HandleSolutionEvent("OnAfterOpenProject");
			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			HandleSolutionEvent("OnAfterOpenSolution");
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			HandleSolutionEvent("OnBeforeCloseProject");
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			HandleSolutionEvent("OnBeforeCloseSolution");
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			HandleSolutionEvent("OnBeforeUnloadProject");
			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			HandleSolutionEvent("OnQueryCloseProject");
			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			HandleSolutionEvent("OnQueryCloseSolution");
			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			HandleSolutionEvent("OnQueryUnloadProject");
			return VSConstants.S_OK;
		}


		private static void HandleSolutionEvent(string eventName)
		{
			Debug.WriteLine(eventName);
		}

	}
}
