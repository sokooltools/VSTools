using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Management;
using System.Net.NetworkInformation;

// ReSharper disable StringLiteralTypo

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides code for displaying an AboutBox dialog as well as publicly available methods
	/// </summary>
	/// <seealso cref="System.Windows.Forms.Form" />
	//----------------------------------------------------------------------------------------------------------------------------
	internal partial class AboutBox : Form
	{
		//private static Assembly _assembly;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="AboutBox"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public AboutBox()
		{
			InitializeComponent();
			//_assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
		}

		//..................................................................................................................................

		#region Assembly Attribute Accessors

		private static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private static string AssemblyTitle
		{
			get
			{
				// Get all Title attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				// If there is at least one Title attribute
				if (!attributes.Any())
					return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
				// Select the first one
				var titleAttribute = (AssemblyTitleAttribute)attributes[0];
				// If it is not an empty string, return it
				return
					String.IsNullOrEmpty(titleAttribute.Title)
					? Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase)
					: titleAttribute.Title;
				// If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
			}
		}

		private static string AssemblyDescription
		{
			get
			{
				// Get all Description attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				// If there aren't any Description attributes, return an empty string, otherwise return its value.
				return attributes.Any() ? ((AssemblyDescriptionAttribute)attributes[0]).Description : String.Empty;
			}
		}

		internal static string AssemblyProduct
		{
			get
			{
				// Get all Product attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				// If there aren't any Product attributes, return an empty string
				return attributes.Any() ? ((AssemblyProductAttribute)attributes[0]).Product : String.Empty;
				// If there is a Product attribute, return its value
			}
		}

		private static string AssemblyCopyright
		{
			get
			{
				// Get all Copyright attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				// If there aren't any Copyright attributes, return an empty string
				return attributes.Any() ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : String.Empty;
				// If there is a Copyright attribute, return its value
			}
		}

		private static string AssemblyCompany
		{
			get
			{
				// Get all Company attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				// If there aren't any Company attributes, return an empty string
				return attributes.Any() ? ((AssemblyCompanyAttribute)attributes[0]).Company : String.Empty;
				// If there is a Company attribute, return its value
			}
		}

		private static string AssemblyLocation => Assembly.GetExecutingAssembly().Location;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the file path of the assembly.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static string FilePath => new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the date and time the assembly was created.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public new static DateTime Created => File.GetCreationTime(FilePath);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the date and time the assembly was last modified.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static DateTime Modified => File.GetLastWriteTime(FilePath);

		#endregion

		//..................................................................................................................................

		#region Form Event Handlers

		private void AboutBox_Load(object sender, EventArgs e)
		{
			MinimumSize = Size;

			Debug.WriteLine($@"AssemblyLocation: {AssemblyLocation}");

			Text = $@"About {AssemblyTitle} {(SystemInfo.IsWindowsAdministrator() ? "(Admin)" : String.Empty)}";

			webBrowser1.DocumentText = GetTabPage1Text();
		}

		private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedIndex != 1 || webBrowser2.DocumentText != String.Empty)
				return;
			Cursor = Cursors.WaitCursor;
			webBrowser2.DocumentText = SystemInfo.GetTabPage2Text();
			Cursor = Cursors.Default;
		}

		#endregion

		//..................................................................................................................................

		#region Helper Methods

		private static string GetTabPage1Text()
		{
			string html = Resources.About1;
			html = html.Replace(@"#Product", AssemblyProduct);
			html = html.Replace(@"#Description", AssemblyDescription);
			html = html.Replace(@"#Version", AssemblyVersion);
			html = html.Replace(@"#Copyright", AssemblyCopyright);
			html = html.Replace(@"#Company", AssemblyCompany);
			html = html.Replace(@"#LastModified", $"{ Modified.ToLongDateString() } { Modified.ToLongTimeString() }");
			html = html.Replace(@"#PluginPath", FilePath);
			return html;
		}

		#endregion
	}

	//..................................................................................................................................

	#region SystemInfo

	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Helper class used for getting information related to the computer system.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class SystemInfo
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns an indication as to whether the current user belongs to the Windows user group with an administrator role.
		/// </summary>
		/// <returns><c>true</c> if the current user is a Windows administrator; otherwise, <c>false</c>.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static bool IsWindowsAdministrator()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a summary of all pertinent system information in HTML format.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetTabPage2Text()
		{
			string html = Resources.About2;
			html = html.Replace(@"#OSInfo", GetOSInfo());
			html = html.Replace(@"#DotNetData", GetDotNetData().Replace(Environment.NewLine, "<br />"));
			html = html.Replace(@"#LanNames", GetLanNames().Replace(Environment.NewLine, "<br />"));
			html = html.Replace(@"#MacAddresses", GetMacAddresses());
			html = html.Replace(@"#CpuData", GetCpuData().Replace(Environment.NewLine, "<br />"));
			return html;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets Operating System Name, Service Pack, and Architecture using WMI (with the legacy methods as a fallback).
		/// </summary>
		/// <returns>
		/// String containing the name of the operating system followed by its service pack (if any) and architecture.
		/// </returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetOSInfo()
		{
			using var objMOS = new ManagementObjectSearcher("SELECT * FROM  Win32_OperatingSystem");
			// Variables to hold our return value.
			string os = String.Empty;
			int osArch = 0;
			try
			{
				foreach (ManagementBaseObject objManagement in objMOS.Get())
				{
					// Get OS version from WMI - This also gives us the edition
					object osCaption = objManagement.GetPropertyValue("Caption");
					if (osCaption == null)
						continue;

					// Remove all non-alphanumeric characters so that only letters, numbers, and spaces are left.
					string osC = Regex.Replace(osCaption.ToString(), "[^A-Za-z0-9 ]", "");
					//string osC = osCaption.ToString();

					// If the OS starts with "Microsoft," remove it.  We know that already.
					if (osC.StartsWith("Microsoft"))
						osC = osC.Substring(9);

					// If the OS now starts with "Windows," again... useless.  Remove it.
					if (osC.Trim().StartsWith("Windows"))
						osC = osC.Trim().Substring(7);

					// Remove any remaining beginning or ending spaces.
					os = osC.Trim();

					// Only proceed if we actually have an OS version - service pack is useless without the OS version.
					if (!String.IsNullOrEmpty(os))
					{
						try
						{
							// Get OS service pack from WMI
							object osSP = objManagement.GetPropertyValue("ServicePackMajorVersion");
							if (osSP != null && osSP.ToString() != "0")
							{
								os += " Service Pack " + osSP;
							}
							else
							{
								// Service Pack not found.  Try built-in Environment class.
								os += GetLegacyOSServicePack();
							}
						}
						catch (Exception)
						{
							// There was a problem getting the service pack from WMI.  Try built-in Environment class.
							os += GetLegacyOSServicePack();
						}
					}
					try
					{
						// Get OS architecture from WMI
						object osA = objManagement.GetPropertyValue("OSArchitecture");
						if (osA != null)
						{
							string osAString = osA.ToString();
							// If "64" is anywhere in there, it's a 64-bit architectore.
							osArch = (osAString.Contains("64") ? 64 : 32);
						}
					}
					catch (Exception)
					{
						Console.Write("");
					}
				}
			}
			catch (Exception)
			{
				Console.Write("");
			}

			// If WMI couldn't tell us the OS, use our legacy method.
			// We won't get the exact OS edition, but something is better than nothing.
			if (os == String.Empty)
			{
				os = GetLegacyOS();
			}

			// If WMI couldn't tell us the architecture, use our legacy method.
			if (osArch == 0)
			{
				osArch = GetLegacyOSArchitecture();
			}
			return os + " " + osArch.ToString(CultureInfo.InvariantCulture) + "-bit";
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the operating system name using .Net's Environment class.
		/// </summary>
		/// <returns>String containing the name of the operating system followed by its service pack (if any)</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetLegacyOS()
		{
			// Get Operating system information.
			OperatingSystem os = Environment.OSVersion;

			// Get version information about the os.
			Version vs = os.Version;

			// Variable to hold our return value.
			string operatingSystem = String.Empty;

			switch (os.Platform)
			{
				case PlatformID.Win32Windows:
					operatingSystem = vs.Minor switch
					{
						0 => "95",
						10 => vs.Revision.ToString(CultureInfo.InvariantCulture) == "2222A" ? "98SE" : "98",
						90 => "Me",
						_ => operatingSystem
					};
					break;
				case PlatformID.Win32NT:
					operatingSystem = vs.Major switch
					{
						3 => "NT 3.51",
						4 => "NT 4.0",
						5 => vs.Minor == 0 ? "2000" : "XP",
						6 => vs.Minor == 0 ? "Vista" : "7",
						10 => "Win10",
						_ => operatingSystem
					};
					break;
				case PlatformID.Win32S:
					break;
				case PlatformID.WinCE:
					break;
				case PlatformID.Unix:
					break;
				case PlatformID.Xbox:
					break;
				case PlatformID.MacOSX:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			// Make sure we actually got something in our OS check that we don't want to just return " Service Pack 2"
			// That information is useless without the OS version.
			if (!String.IsNullOrEmpty(operatingSystem))
			{
				// Got something.  Let's see if there's a service pack installed.
				operatingSystem += GetLegacyOSServicePack();
			}
			// Return the information we've gathered.
			return operatingSystem;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the installed Operating System Service Pack using .Net's Environment class.
		/// </summary>
		/// <returns>String containing the operating system's installed service pack (if any)</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetLegacyOSServicePack()
		{
			// Get service pack from Environment Class
			string sp = Environment.OSVersion.ServicePack;
			if (!String.IsNullOrEmpty(sp) && sp != " ")
			{
				// If there's a service pack, return it with a space in front (for formatting)
				return " " + sp;
			}
			// No service pack.  Return an empty string
			return String.Empty;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets Operating System Architecture.  This does not tell you if the program is running in 32 or 64-bit mode or if the 
		/// CPU is 64-bit capable.  It tells you whether the actual Operating System is 32 or 64-bit.
		/// </summary>
		/// <returns>Int containing 32 or 64 representing the number of bits in the OS Architecture</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static int GetLegacyOSArchitecture()
		{
			string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			return String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0 ? 32 : 64;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the CPU data as an html string.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetCpuData()
		{
			var sb = new StringBuilder();

			using var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
			string cpuName = String.Empty;
			foreach (ManagementBaseObject managementBaseObject in searcher.Get())
			{
				var o = (ManagementObject)managementBaseObject;
				sb.AppendFormat("{0}{1}{1}", o, Environment.NewLine);
				foreach (PropertyData prop in o.Properties)
				{
					sb.AppendFormat("{0} : {1}{2}", prop.Name, prop.Value, Environment.NewLine);

					if (prop.Name == "Name")
						cpuName = (string)prop.Value;
				}
			}
			sb.Insert(0, $"{cpuName}{Environment.NewLine}{Environment.NewLine}");
			return sb.ToString();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the .Net versons installed on this machine as an html string.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetDotNetData()
		{
			var sb = new StringBuilder();
			IEnumerable<string> dotNetVersionList = GetDotNetVersionList();
			foreach (string s in dotNetVersionList)
				sb.AppendLine(s);
			return sb.ToString();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the .Net framework/versions installed on this machine as an enumerable list.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static IEnumerable<string> GetDotNetVersionList()
		{
			static void ProcessKids(RegistryKey node, Action<RegistryKey, string> action)
			{
				foreach (string childName in node.GetSubKeyNames())
					using (RegistryKey child = node.OpenSubKey(childName))
						action(child, childName);
			}
			var versionList = new List<string>();
			RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
			{
				Action<RegistryKey, Func<RegistryKey, bool>>[] visitDescendants = { null };
				visitDescendants[0] = (regKey, isDone) =>
				{
					if (!isDone(regKey))
						ProcessKids(regKey, (subKey, _) => visitDescendants[0](subKey, isDone));
				};
				ProcessKids(ndpKey, (versionKey, versionKeyName) =>
				{
					if (Regex.IsMatch(versionKeyName, @"^v\d"))
					{
						visitDescendants[0](versionKey, key =>
						{
							bool isInstallationNode =
								Equals(key.GetValue("Install"), 1) && key.GetValue("Version") != null;
							if (!isInstallationNode) return false;
							if (ndpKey != null)
								versionList.Add(key.Name.Substring(ndpKey.Name.Length + 1)
												+ (key.GetValue("SP") != null
													? ", service pack " + key.GetValue("SP")
													: "")
												+ " (" + key.GetValue("Version") + ") "
								);
							return true;
						});
					}
				});
			}
			return versionList;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the MAC Addresses.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetMacAddresses()
		{
			string macAddress = String.Empty;
			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				// Only consider Ethernet network interfaces, thereby ignoring any loopback devices etc.
				if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet || nic.OperationalStatus != OperationalStatus.Up)
					continue;
				macAddress += nic.GetPhysicalAddress().ToString();
				break;
			}
			var sb = new StringBuilder();
			int index = 0;
			foreach (char c in macAddress)
			{
				sb.AppendFormat("{0}{1}", c, (index++ & 1) == 0 ? "" : "-");
			}
			macAddress = sb.ToString().Trim('-');
			return macAddress;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a list of LAN names as Html text.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetLanNames()
		{
			var sb = new StringBuilder();
			using var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection mgObjCollection = managementClass.GetInstances();
			foreach (ManagementObject mgObject in mgObjCollection.Cast<ManagementObject>().Where(mgObject => Convert.ToBoolean(mgObject["IPEnabled"])))
			{
				try
				{
					sb.AppendFormat("{0} : {1}{2}", ((string[])mgObject["IPAddress"])[0], mgObject["Description"], Environment.NewLine);
				}
				catch (Exception ex)
				{
					Console.WriteLine(@"An error occured: " + ex.Message);
				}
			}
			return sb.ToString();
		}
		#endregion

		//..................................................................................................................................

		#region Not Used

		////------------------------------------------------------------------------------------------------------------------------
		///// <summary>
		///// Gets the latest version number of .NET installed on this computer.
		///// </summary>
		////------------------------------------------------------------------------------------------------------------------------
		//public static string DotNetVersion
		//{
		//	get
		//	{
		//		const string REGISTRY_KEY = @"SOFTWARE\Microsoft\.NETFramework\";
		//		using RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY, false);
		//		return key == null
		//			? Environment.Version.ToString()
		//			: key.GetSubKeyNames().Where(m => m.StartsWith("v")).OrderByDescending(m => m).FirstOrDefault();
		//	}
		//}

		////------------------------------------------------------------------------------------------------------------------------
		///// <summary>
		///// Gets the standard ip config data as a string of text.
		///// </summary>
		///// <returns></returns>
		////------------------------------------------------------------------------------------------------------------------------
		//private static string GetIpConfigData()
		//{
		//	const string MY_COMMAND = "IPCONFIG/ALL";
		//	var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + MY_COMMAND)
		//	{
		//		CreateNoWindow = true,
		//		RedirectStandardOutput = true,
		//		UseShellExecute = false
		//	};
		//	using (var proc = new System.Diagnostics.Process())
		//	{
		//		proc.StartInfo = procStart;
		//		proc.Start();
		//		string result = proc.StandardOutput.ReadToEnd();
		//		return result;
		//	}
		//}

	}

	#endregion
}
