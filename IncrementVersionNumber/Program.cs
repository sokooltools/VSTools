using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IncrementVersionNumber
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Increments the version number(s) inside the following four files:
	/// <list type="bullet">
	/// <item><description>AssemblyInfo.cs</description></item>
	/// <item><description>VsToolsPackage.cs</description></item>
	/// <item><description>source.extension.vsixmanifest</description></item>
	/// <item><description>source.extension.cs</description></item>
	/// </list>
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class Program
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The main entry point of this application.
		/// </summary>
		/// <param name="args">
		/// The arguments: 1st argument is required and is the solution folder; 2nd argument is optional and is the increment 
		/// (which can also be a negative number)
		/// </param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">args - At least one argument must be specified!</exception>
		/// <exception cref="System.IO.DirectoryNotFoundException">Specified Solution folder was not found</exception>
		/// <exception cref="Exception"></exception>
		//------------------------------------------------------------------------------------------------------------------------
		public static int Main(string[] args)
		{
			try
			{
				if (args.Length < 1)
					throw new ArgumentNullException(nameof(args), "At least one argument must be specified!");
				string solutionFolder = Environment.ExpandEnvironmentVariables(args[0]);
				if (Directory.Exists(solutionFolder))
				{
					int increment = args.Length > 1 ? int.Parse(args[1]) : 1;
					IncrementVersionNumbers(solutionFolder, increment);
				}
				else
					throw new DirectoryNotFoundException($"Specified solution folder: \"{solutionFolder}\" was not found!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return -1;
			}

			return 0;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Increments the version number(s) inside four files.
		/// </summary>
		/// <param name="solutionFolder">The solution folder.</param>
		/// <param name="increment">The increment (default = 1).</param>
		/// <exception cref="Exception"></exception>
		//------------------------------------------------------------------------------------------------------------------------
		private static void IncrementVersionNumbers(string solutionFolder, int increment = 1)
		{
			string filePath1 = Path.Combine(solutionFolder, @"Properties\AssemblyInfo.cs");
			string text1 = File.ReadAllText(filePath1);
			const string pattern1 = "\\(\"([0-9]+\\.[0-9]+\\.[0-9]+\\.)([0-9]+)\"\\)";
			Match m = Regex.Match(text1, "AssemblyVersion" + pattern1);
			if (!m.Success)
				throw new Exception($"Could not find 'AssemblyVersion' in file: \"{filePath1}\"");

			string version1A = m.Groups[1].Value + (int.Parse(m.Groups[2].Value) + increment);
			text1 = text1.Substring(0, m.Index) + "AssemblyVersion(\"" + version1A + "\")" + text1.Substring(m.Index + m.Length);
			m = Regex.Match(text1, "AssemblyFileVersion" + pattern1);
			if (!m.Success)
				throw new Exception($"Could not find 'AssemblyFileVersion' in file: \"{filePath1}\"");
			string version1B = m.Groups[1].Value + (int.Parse(m.Groups[2].Value) + increment);
			text1 = text1.Substring(0, m.Index) + "AssemblyFileVersion(\"" + version1B + "\")" + text1.Substring(m.Index + m.Length);

			string filePath2 = Path.Combine(solutionFolder, @"VsToolsPackage.cs");
			string text2 = File.ReadAllText(filePath2);
			const string pattern2 = "(InstalledProductRegistration\\(\"#110\",[\\s]*\"#112\",[\\s]*\")([0-9]+\\.[0-9]+\\.[0-9]+\\.)([0-9]+)\"";
			m = Regex.Match(text2, pattern2);
			if (!m.Success)
				throw new Exception($"Could not find 'InstalledProductRegistration' in file: \"{filePath2}\"");
			string version2 = m.Groups[2].Value + (int.Parse(m.Groups[3].Value) + increment);
			text2 = text2.Substring(0, m.Index) + m.Groups[1].Value + version2 + "\"" + text2.Substring(m.Index + m.Length);

			string filePath3 = Path.Combine(solutionFolder, @"source.extension.vsixmanifest");
			string text3 = File.ReadAllText(filePath3);
			const string pattern3 = "(Identity Id=\"7a2977ea-8c89-4d9f-bbb6-96b2040e23b6\"[\\s]+Version=\")([0-9]+\\.[0-9]+\\.[0-9]+\\.)([0-9]+)\"";
			m = Regex.Match(text3, pattern3);
			if (!m.Success)
				throw new Exception($"Could not find 'Identity' in file: \"{filePath3}\"");
			string version3 = m.Groups[2].Value + (int.Parse(m.Groups[3].Value) + increment);
			text3 = text3.Substring(0, m.Index) + m.Groups[1].Value + version3 + "\"" + text3.Substring(m.Index + m.Length);

			string filePath4 = Path.Combine(solutionFolder, @"source.extension.cs");
			string text4 = File.ReadAllText(filePath4);
			const string pattern4 = "([\\s]+Version = \")([0-9]+\\.[0-9]+\\.[0-9]+\\.)([0-9]+)\"";
			m = Regex.Match(text4, pattern4);
			if (!m.Success)
				throw new Exception($"Could not find 'Version' in file: \"{filePath4}\"");
			string version4 = m.Groups[2].Value + (int.Parse(m.Groups[3].Value) + increment);
			text4 = text4.Substring(0, m.Index) + m.Groups[1].Value + version4 + "\"" + text4.Substring(m.Index + m.Length);

			File.WriteAllText(filePath1, text1, Encoding.UTF8);
			File.WriteAllText(filePath2, text2, Encoding.UTF8);
			File.WriteAllText(filePath3, text3, Encoding.UTF8);
			File.WriteAllText(filePath4, text4, Encoding.UTF8);

			// Write the version number (from the manifest file) to a file.
			File.WriteAllText(Path.Combine(solutionFolder, @"IncrementVersionNumber\bin\IncrementVersionNumber.dat"), version3, Encoding.ASCII);

			if (version1A == version1B && version1B == version2 && version2 == version3 && version3 == version4)
				return;

			// Create an error file when the version numbers do not all match.
			string message = "The four files do not all contain the same version number!\n\n" +
							 "Version     File\n" +
							 "----------- -----------------------------------------------------------------------------------------------\n" +
							 $"{version1A,-12} {filePath1}\n" +
							 $"{version1B,-12} {filePath1}\n" +
							 $"{version2,-12} {filePath2}\n" +
							 $"{version3,-12} {filePath3}\n" +
							 $"{version4,-12} {filePath4}";
			File.WriteAllText(Path.Combine(solutionFolder, @"IncrementVersionNumber\bin\IncrementVersionNumber.err"), message, Encoding.ASCII);
		}
	}
}