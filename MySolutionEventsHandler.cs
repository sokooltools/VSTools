using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal class MySolutionEventsHandler : IVsSelectionEvents, IVsSolutionEvents, IDisposable
	{
		//..................................................................................................................................

		#region Private Fields

		private readonly ServiceProvider _serviceProvider;

		private readonly IVsMonitorSelection _monitorSelectionService;

		private readonly IVsSolution _solution;

		private readonly uint _solutionExistsCookie;

		private uint _solutionSubscriptionCookie;

		private uint _solutionEventsCookie; // Cookie used to remove our events client.

		#endregion

		//..................................................................................................................................

		#region Public Constructor

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="MySolutionEventsHandler"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">serviceProvider</exception>
		//------------------------------------------------------------------------------------------------------------------------
		public MySolutionEventsHandler(IVsSolution solution)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            _solution = solution;

			// ReSharper disable once SuspiciousTypeConversion.Global
			var oleServiceProvider = Connect.ApplicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
			_serviceProvider = new ServiceProvider(oleServiceProvider);

			if (_serviceProvider == null)
				throw new ArgumentNullException(nameof(_serviceProvider));

			_monitorSelectionService = _serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

			Guid solutionExists = VSConstants.UICONTEXT_SolutionExists;
			_monitorSelectionService?.GetCmdUIContextCookie(ref solutionExists, out _solutionExistsCookie);

			solution.AdviseSolutionEvents(this, out _solutionEventsCookie);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public void Dispose()
		{
            ThreadHelper.ThrowIfNotOnUIThread();

            _serviceProvider?.Dispose();

			if (_solutionEventsCookie == 0) 
				return;

			// Disable client from receiving notifications about solution events.
			_solution.UnadviseSolutionEvents(_solutionEventsCookie);
			_solutionEventsCookie = 0;
		}

		#endregion

		//..................................................................................................................................

		#region IVsSelectionEvents Implementation

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Reports that the project hierarchy, item and/or selection container has changed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pHierOld">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project hierarchy 
		/// for the previous selection.
		/// </param>
		/// <param name="itemidOld">
		/// Identifier of the project item for previous selection. For valid <paramref name="itemidOld" /> values, see VSITEMID.
		/// </param>
		/// <param name="pMisOld">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsMultiItemSelect" /> interface to access a 
		/// previous multiple selection.
		/// </param>
		/// <param name="pScOld">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.ISelectionContainer" /> interface to access 
		/// Properties window data for the previous selection.
		/// </param>
		/// <param name="pHierNew">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project hierarchy 
		/// for the current selection.
		/// </param>
		/// <param name="itemidNew">
		/// Identifier of the project item for the current selection. For valid <paramref name="itemidNew" /> values, see 
		/// VSITEMID.
		/// </param>
		/// <param name="pMisNew">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsMultiItemSelect" /> interface for the current 
		/// selection.
		/// </param>
		/// <param name="pScNew">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.ISelectionContainer" /> interface for the current 
		/// selection.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMisOld,
										ISelectionContainer pScOld, IVsHierarchy pHierNew, uint itemidNew,
										IVsMultiItemSelect pMisNew, ISelectionContainer pScNew)
		{
			return HandleSolutionEvent(nameof(OnSelectionChanged));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Reports that an element value has changed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="elementid">
		/// DWORD value representing a particular entry in the array of element values associated with the selection context. For 
		/// valid <paramref name="elementid" /> values, see <see cref="T:Microsoft.VisualStudio.VSConstants.VSSELELEMID" />.
		/// </param>
		/// <param name="varValueOld">
		/// VARIANT that contains the previous element value. This parameter contains element-specific data, such as a pointer to 
		/// the <see cref="T:Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget" /> interface if <paramref name="elementid" /> 
		/// is set to SEID_ResultsList or a pointer to the <see cref="T:Microsoft.VisualStudio.OLE.Interop.IOleUndoManager" /> 
		/// interface if <paramref name="elementid" /> is set to SEID_UndoManager.
		/// </param>
		/// <param name="varValueNew">
		/// VARIANT that contains the new element value. This parameter contains element-specific data, such as a pointer to the 
		/// IOleCommandTarget interface if <paramref name="elementid" /> is set to SEID_ResultsList or a pointer to the 
		/// IOleUndoManager interface if <paramref name="elementid" /> is set to SEID_UndoManager.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
		{
			return HandleSolutionEvent(nameof(OnElementValueChanged));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Reports that the command UI context has changed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="dwCmdUICookie">
		/// DWORD representation of the GUID identifying the command UI context passed in as the rguidCmdUI parameter in the call 
		/// to <see 
		/// cref="M:Microsoft.VisualStudio.Shell.Interop.IVsMonitorSelection.GetCmdUIContextCookie(System.Guid@,System.UInt32@)"/>.
		/// </param>
		/// <param name="fActive">
		/// Flag that is set to true if the command UI context identified by <paramref name="dwCmdUICookie" /> has become active 
		/// and false if it has become inactive.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
		{
            ThreadHelper.ThrowIfNotOnUIThread();
           
            //var svc1 = ServiceProvider.GetService(typeof(SVsCodeDefView)) as IVsCodeDefView;

            int active = 0;
			_monitorSelectionService?.IsCmdUIContextActive(_solutionExistsCookie, out active);

			Debug.WriteLine(@"Solution Exists: {0}", active);

			if (active == 0 || _solutionSubscriptionCookie != 0)
				return VSConstants.S_OK;

			Debug.Assert(_solution != null);

			// The solution is loaded. Now we can get hold of our selection events. 
			_solution.AdviseSolutionEvents(this, out _solutionSubscriptionCookie);

			return VSConstants.S_OK;
		}

		#endregion

		//..................................................................................................................................

		#region IVsSolutionEvents Implementation

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that the project has been opened.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project being 
		/// loaded.
		/// </param>
		/// <param name="fAdded">
		/// true if the project is added to the solution after the solution is opened. false if the project is added to the 
		/// solution while the solution is being opened.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return HandleSolutionEvent(nameof(OnAfterOpenProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Queries listening clients as to whether the project can be closed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project to be 
		/// closed.
		/// </param>
		/// <param name="fRemoving">
		/// true if the project is being removed from the solution before the solution is closed. false if the project is being 
		/// removed from the solution while the solution is being closed.
		/// </param>
		/// <param name="pfCancel">
		/// true if the client vetoed the closing of the project. false if the client approved the closing of the project.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return HandleSolutionEvent(nameof(OnQueryCloseProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that the project is about to be closed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project being 
		/// closed.
		/// </param>
		/// <param name="fRemoved">
		/// true if the project was removed from the solution before the solution was closed. false if the project was removed 
		/// from the solution while the solution was being closed.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return HandleSolutionEvent(nameof(OnBeforeCloseProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that project has been loaded.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pStubHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the placeholder 
		/// hierarchy for the unloaded project.
		/// </param>
		/// <param name="pRealHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project that was 
		/// loaded.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return HandleSolutionEvent(nameof(OnAfterLoadProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Queries listening clients as to whether the project can be unloaded.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pRealHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project to be 
		/// unloaded.
		/// </param>
		/// <param name="pfCancel">
		/// true if the client vetoed unloading the project. false if the client approved unloading the project.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return HandleSolutionEvent(nameof(OnQueryUnloadProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that the project is about to be unloaded.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pRealHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project that will 
		/// be unloaded.
		/// </param>
		/// <param name="pStubHierarchy">
		/// Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the placeholder 
		/// hierarchy for the project being unloaded.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return HandleSolutionEvent(nameof(OnBeforeUnloadProject));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that the solution has been opened.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pUnkReserved">Reserved for future use.</param>
		/// <param name="fNewSolution">
		/// true if the solution is being created. false if the solution was created previously or is being loaded.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			return HandleSolutionEvent(nameof(OnAfterOpenSolution));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Queries listening clients as to whether the solution can be closed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pUnkReserved">Reserved for future use.</param>
		/// <param name="pfCancel">
		/// true if the client vetoed closing the solution. false if the client approved closing the solution.
		/// </param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return HandleSolutionEvent(nameof(OnQueryCloseSolution));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that the solution is about to be closed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pUnkReserved">Reserved for future use.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return HandleSolutionEvent(nameof(OnBeforeCloseSolution));
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies listening clients that a solution has been closed.
		/// </summary>
		/// <returns>
		/// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns 
		/// an error code.
		/// </returns>
		/// <param name="pUnkReserved">Reserved for future use.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public int OnAfterCloseSolution(object pUnkReserved)
		{
			return HandleSolutionEvent(nameof(OnAfterCloseSolution));
		}

		#endregion

		//..................................................................................................................................

		#region Private Helper Methods

		private static int HandleSolutionEvent(string eventName)
		{
			Debug.WriteLine(eventName);
			return VSConstants.S_OK;
		}

		#endregion
	}
}