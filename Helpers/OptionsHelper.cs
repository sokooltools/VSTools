using System;
using System.IO;
using Microsoft.Win32;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides helper methods for accessing the properties (stored in the Windows Registry) associated with 
	/// this extension and accessible to the user via the options dialog.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class OptionsHelper
	{
		private static readonly string DefaultLogFile = Path.Combine(Path.GetTempPath(), "VsTools.log");
		private static readonly string DefaultExternalToolPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe");
		
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Represents the path in the HKEY_CURRENT_USER part the registry containing the options specified in this class:
		/// "Software\DevTools\VsTools\14.0\Options"
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private const string REGISTRY_HIVE = @"Software\DevTools\VsTools\14.0\Options";

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether debug logging is enabled. (Default = false)
		/// </summary>
		/// <value><c>true</c> if debug logging is enabled; otherwise, <c>false</c>.</value>
		//----------------------------------------------------------------------------------------------------
		public static bool IsLogEnabled
		{
			get => Convert.ToBoolean(GetValue("IsLogEnabled", false));
			set => SetValue("IsLogEnabled", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the path to the debug log file (Default = %TEMP%\VsTools.log)
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static string LogFile
		{
			get => Convert.ToString(GetValue("LogFile", DefaultLogFile));
			set => SetValue("LogFile", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the Log Level. (Default = 2)
		/// </summary>
		/// <value>The Log Level.</value>
		//----------------------------------------------------------------------------------------------------
		public static int LogLevel
		{
			get => Convert.ToInt32(GetValue("LogLevel", 2));
			set => SetValue("LogLevel", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether region divider lines are inserted. (Default = false)
		/// </summary>
		/// <value><c>true</c> if region divider lines are inserted; otherwise, <c>false</c>.</value>
		//----------------------------------------------------------------------------------------------------
		public static bool IsRegionDividerLinesInserted
		{
			get => Convert.ToBoolean(GetValue("RegionDividerLinesInsert", false));
			set => SetValue("RegionDividerLinesInsert", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the maximum indentation of aligned variables. (Default = 50)
		/// </summary>
		/// <value>The maximum indentation of aligned variables.</value>
		//----------------------------------------------------------------------------------------------------
		public static int AlignVariablesMaxIndent
		{
			get => Convert.ToInt32(GetValue("AlignVariablesMaxIndent", 50));
			set => SetValue("AlignVariablesMaxIndent", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the max length of a pasted comment line before it wraps. (Default = 100)
		/// </summary>
		/// <value>The max length of a pasted comment line before it wraps.</value>
		//----------------------------------------------------------------------------------------------------
		public static int PasteCommentMaxLineLength
		{
			get => Convert.ToInt32(GetValue("PasteCommentMaxLineLength", 100));
			set => SetValue("PasteCommentMaxLineLength", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the region divider line character. (Default = '.')
		/// </summary>
		/// <value>The region divider line character.</value>
		//----------------------------------------------------------------------------------------------------
		public static char RegionDividerLineChar
		{
			get => Convert.ToChar(GetValue("RegionDividerLineChar", '.'));
			set => SetValue("RegionDividerLineChar", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the region divider line character repeat amount. (Default = 109)
		/// </summary>
		/// <value>The region divider line repeat.</value>
		//----------------------------------------------------------------------------------------------------
		public static int RegionDividerLineRepeat
		{
			get => Convert.ToInt32(GetValue("RegionDividerLineRepeat", 109));
			set => SetValue("RegionDividerLineRepeat", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the comment divider line character. (Default = '-')
		/// </summary>
		/// <value>The comment divider line character.</value>
		//----------------------------------------------------------------------------------------------------
		public static char CommentDividerLineChar
		{
			get => Convert.ToChar(GetValue("CommentDividerLineChar", '-'));
			set => SetValue("CommentDividerLineChar", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the comment divider line repeat. (Default = 100)
		/// </summary>
		/// <value>The comment divider line repeat.</value>
		//----------------------------------------------------------------------------------------------------
		public static int CommentDividerLineRepeat
		{
			get => Convert.ToInt32(GetValue("CommentDividerLineRepeat", 100));
			set => SetValue("CommentDividerLineRepeat", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether comments are indented. (Default = false)
		/// </summary>
		/// <value><c>true</c> if comments are indented; otherwise, <c>false</c>.</value>
		//----------------------------------------------------------------------------------------------------
		public static bool IsCommentIndented
		{
			get => Convert.ToBoolean(GetValue("IsCommentIndented", false));
			set => SetValue("IsCommentIndented", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether comments divider lines are right aligned. (Default = true)
		/// </summary>
		/// <value><c>true</c> if comment divider lines are right aligned; otherwise, <c>false</c>.</value>
		//----------------------------------------------------------------------------------------------------
		public static bool IsCommentDividerLineRightAligned
		{
			get => Convert.ToBoolean(GetValue("IsCommentDividerLineRightAligned", true));
			set => SetValue("IsCommentDividerLineRightAligned", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether javascript comments are stripped. (Default = false)
		/// </summary>
		/// <value><c>true</c> if javascript comments are stripped; otherwise, <c>false</c>.</value>
		//----------------------------------------------------------------------------------------------------
		public static bool IsJavascriptCommentsStripped
		{
			get => Convert.ToBoolean(GetValue("IsJavascriptCommentsStripped", false));
			set => SetValue("IsJavascriptCommentsStripped", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the external tool path 1.  (Default = notepad.exe)
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static string ExternalToolPath1
		{
			get => Convert.ToString(GetValue("ExternalToolPath1", DefaultExternalToolPath));
			set => SetValue("ExternalToolPath1", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the external tool path 2.  (Default = notepad.exe)
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static string ExternalToolPath2
		{
			get => Convert.ToString(GetValue("ExternalToolPath2", DefaultExternalToolPath));
			set => SetValue("ExternalToolPath2", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the console window auto timeout in seconds following a copy procedure. (Default = 5)
		/// </summary>
		/// <value>The console window timeout in seconds.</value>
		//----------------------------------------------------------------------------------------------------
		public static int AutoCloseSeconds
		{
			get => Convert.ToInt32(GetValue("AutoCloseSeconds", 5));
			set => SetValue("AutoCloseSeconds", value);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the value in the registry corresponding to the specified key name (or the default value if 
		/// the key doesn't exist).
		/// </summary>
		/// <param name="keyName">Name of the key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		private static object GetValue(string keyName, object defaultValue)
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_HIVE) ??
									 Registry.CurrentUser.CreateSubKey(REGISTRY_HIVE))
				return key != null ? key.GetValue(keyName, defaultValue) : defaultValue;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the value in the registry corresponding to the specified key.
		/// </summary>
		/// <param name="keyName">Name of the key.</param>
		/// <param name="value">The value.</param>
		//----------------------------------------------------------------------------------------------------
		private static void SetValue(string keyName, object value)
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_HIVE, true) ??
									 Registry.CurrentUser.CreateSubKey(REGISTRY_HIVE))
			{
				key?.SetValue(keyName, value);
			}
		}

		////------------------------------------------------------------------------------------------
		///// <summary>
		///// Gets or sets a value indicating whether debug is on. (Default = false)
		///// </summary>
		///// <value><c>true</c> if this debug is on; otherwise, <c>false</c>.</value>
		////------------------------------------------------------------------------------------------
		//public static bool IsDebugOn
		//{
		//    get { return Convert.ToBoolean(GetValue("IsDebugOn", false)); }
		//    set { SetValue("IsDebugOn", value); }
		//}
	}
}