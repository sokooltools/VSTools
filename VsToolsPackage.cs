using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

// ReSharper disable UnusedMember.Local

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// This is the class that implements the package. This is the class that Visual Studio will create when one of the 
	/// commands is selected by the user, and so it can be considered the main entry point for the integration with the 
	/// IDE.
	/// Notice that this implementation derives from Microsoft.VisualStudio.Shell.Package that is the basic 
	/// implementation of a package provided by the Managed Package Framework (MPF).
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(PackageGuidString)]
	[InstalledProductRegistration("#110", "#112", "3.0.2.19", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	// These following two lines make our Command initialize at Visual Studio startup.
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
	public sealed class VsToolsPackage : AsyncPackage
	{
		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// VsToolsPackage GUID string.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		public const string PackageGuidString = "8CCE8876-C155-4A94-95A3-EF619E2D2736";

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="VsToolsPackage"/> class.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		public VsToolsPackage()
		{
			// Inside this method you can place any initialization code that does not require any Visual Studio service because 
			// at this point the package object is created but not sited yet inside Visual Studio environment.
			// The place to do all the other initialization is the Initialize method.
		}

		//..................................................................................................................................

		#region Package Members

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place 
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">
		/// A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.
		/// </param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>
		/// A task representing the async work of package initialization, or an already completed task if there is 
		/// none. Do not return null from this method.
		/// </returns>
		//--------------------------------------------------------------------------------------------------------------
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			await CreateCommandSet.InitializeAsync(this);
		}

		#endregion
	}
}