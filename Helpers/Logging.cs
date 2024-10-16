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
		/// Initializes the <see cref="Logging"/> class.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		static Logging()
		{
			if (!File.Exists(OptionsHelper.LogFile))
				return;
			try { File.Delete(OptionsHelper.LogFile); } catch { }
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the name of the calling method to the log file, but only when Logging is enabled and the 
		/// specified logging level is greater than or equal to the logging level specified in the options 
		/// dialog.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void Log(int logLevel)
		{
			if (!OptionsHelper.IsLogEnabled || logLevel > OptionsHelper.LogLevel)
				return;
			MethodBase mb = new StackTrace().GetFrame(1).GetMethod();
			Write(mb.ReflectedType != null ? $"{mb.ReflectedType.Name}.{mb.Name}()" : $"{mb.Name}()");

		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the specified message to the log file, but only if the specified log level is greater than 
		/// or equal to the logging level specified in the options dialog.
		/// </summary>
		/// <param name="logLevel">The log level.</param>
		/// <param name="message">Text of the message to write.</param>
		//----------------------------------------------------------------------------------------------------
		public static void Log(int logLevel, string message)
		{
			if (OptionsHelper.IsLogEnabled && OptionsHelper.LogLevel <= logLevel)
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
				using var sw = new StreamWriter(OptionsHelper.LogFile, true);
				sw.WriteLine("{0:MM/dd/yyyy HH:mm:ss}: {1}", DateTime.Now, message);
			}
			catch { }
		}
	}
}