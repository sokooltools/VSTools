using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
// ReSharper disable EmptyGeneralCatchClause

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for writing messages to a log file.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class Logging
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes a message to the log file containing the criteria specifed as part of a find and replace 
		/// action, but only when Logging is enabled and the specified logging level is greater than or equal 
		/// to the logging level specified in the options dialog.
		/// </summary>
		/// <param name="find">The find.</param>
		/// <param name="replace">The replace.</param>
		/// <param name="description">The description.</param>
		/// <param name="logLevel">The log level.</param>
		//----------------------------------------------------------------------------------------------------
		public static void LogReplace(string find, string replace, string description, int logLevel = 3)
		{
			if (!OptionsHelper.IsLogEnabled || OptionsHelper.LogLevel < logLevel)
				return;
			try
			{
				string message = $"\t//--{description}\n\t\t Find:\t\t\"{find}\"\n\t\t Replace:\t\"{replace.Replace("\n", @"\n")}\"";
				using (var sw = new StreamWriter(OptionsHelper.LogFile, true))
					sw.WriteLine(message);
			}
			catch { }
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the name of the calling method to the log file, but only when Logging is enabled and the 
		/// specified logging level is greater than or equal to the logging level specified in the options 
		/// dialog.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void Log(int logLevel = 1)
		{
			if (!OptionsHelper.IsLogEnabled || OptionsHelper.LogLevel < logLevel)
				return;
			MethodBase mb = new StackTrace().GetFrame(1).GetMethod();
			Write(mb.ReflectedType != null ? $"{mb.ReflectedType.Name}.{mb.Name}()" : $"{mb.Name}()");
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the specified message to the log file, but only if the specified log level is greater than 
		/// or equal to the logging level specified in the options dialog.
		/// </summary>
		/// <param name="message">Text of the message to write.</param>
		/// <param name="logLevel">The log level.</param>
		//----------------------------------------------------------------------------------------------------
		public static void Log(string message, int logLevel = 3)
		{
			if (OptionsHelper.IsLogEnabled && OptionsHelper.LogLevel >= logLevel)
				Write(message);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the specified message to the log file.
		/// </summary>
		/// <param name="message">The message.</param>
		//----------------------------------------------------------------------------------------------------
		private static void Write(string message)
		{
			try
			{
				using (var sw = new StreamWriter(OptionsHelper.LogFile, true))
					sw.WriteLine("{0:MM/dd/yyyy HH:mm:ss}: {1}", DateTime.Now, message);
			}
			catch { }
		}
	}
}