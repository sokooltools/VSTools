using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Class containing the DTE2 object.
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	public static class Connect
	{
		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The DTE2 object containg implementations of the main interface used by the Visual Studio Add-In.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		public static readonly DTE2 objDte2;

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the <see cref="Connect"/> class.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		static Connect()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			objDte2 = (DTE2)Package.GetGlobalService(typeof(DTE));
		}
	}
}
