using System;
using System.IO;
using System.Reflection;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for obtaining information about the entry or executing assembly by default, or a specified assembly.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class AssemblyInfo
	{
		private readonly Assembly _assembly;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyInfo"/> class using either the entry assembly or the 
		/// executing assembly
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public AssemblyInfo()
		{
			_assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyInfo"/> class using the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public AssemblyInfo(Assembly assembly)
		{
			_assembly = assembly;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the title information pertaining to the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Title
		{
			get
			{
				string defaultTitle = Path.GetFileNameWithoutExtension(_assembly.CodeBase);
				object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length == 0) return defaultTitle;
				var titleAttribute = (AssemblyTitleAttribute)attributes[0];
				return titleAttribute.Title != String.Empty ? titleAttribute.Title : defaultTitle;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the version information (major, minor, build, and revision number) of the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Version => _assembly.GetName().Version.ToString();

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the description of the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Description
		{
			get
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the product name information pertaining to the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Product
		{
			get
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the copyright information pertaining to the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Copyright
		{
			get
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the company name information pertaining to the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string Company
		{
			get
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the file path of the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public string FilePath => new Uri(_assembly.GetName().CodeBase).LocalPath;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the date and time the assembly was created.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public DateTime Created => File.GetCreationTime(FilePath);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the date and time the assembly was last modified.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public DateTime Modified => File.GetLastWriteTime(FilePath);
	}
}
