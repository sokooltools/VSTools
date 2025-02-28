using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Class containing the DTE2 object.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------------------------
	public static class Connect
	{
		//----------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The service containing implementations of the main Design Time Environment interface to a Visual Studio Add-In.
		/// </summary>
		//----------------------------------------------------------------------------------------------------------------------
		public static readonly DTE2 DteService;

		//----------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the <see cref="Connect"/> class.
		/// </summary>
		//----------------------------------------------------------------------------------------------------------------------
		static Connect()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			DteService = (DTE2)Package.GetGlobalService(typeof(DTE));
		}
	}
}
