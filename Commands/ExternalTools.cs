using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Process = System.Diagnostics.Process;

namespace SokoolTools.VsTools
{
	internal static class ExternalTools
	{
		/// <summary>
		/// Launches an external application used for copying target files to a 'Special' assemblies folder and then to 
		/// optionally check them into TFS.
		/// </summary>
		/// <param name="isCheckin">if set to <c>true</c> the files are automatically checked into TFS.</param>
		public static void CopyTargetFiles(bool isCheckin)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			try
			{
				if (!(Connect.objDte2.ActiveSolutionProjects is Array activeProjects) || activeProjects.Length == 0)
				{
					MessageBox.Show(Resources.ProjectMustBeSelected, @"Copy Target Files", MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation);
					return;
				}

				string vsToolsPath = OptionsHelper.ExternalToolPath1;
				string timeoutSecs = OptionsHelper.AutoCloseSeconds.ToString(CultureInfo.InvariantCulture);

				for (int i = 0; i < activeProjects.GetLength(0); i++)
				{
					var project = (Project)activeProjects.GetValue(i);

					Configuration config = project.ConfigurationManager.ActiveConfiguration;

					string fullPath = project.Properties.Item("FullPath").Value.ToString();
					string outputPath = config.Properties.Item("OutputPath").Value.ToString();
					string binDir = Path.Combine(fullPath, outputPath);
					string targetName = project.Properties.Item("AssemblyName").Value.ToString();

					string args = "\"" + binDir.Replace(@"\", @"\\") + "\" \"" + targetName + "\" " + (isCheckin ? "0 Y" : timeoutSecs);

					var process = new Process
					{
						StartInfo =
						{
							FileName = vsToolsPath,
							Arguments = args
						}
					};
					process.Start();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex.Message, 0);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Runs the external tool.
		/// </summary>
		/// <param name="index">The index number.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public static void Run(int index)
		{
			Logging.Log(2);
			try
			{
				var process = new Process
				{
					StartInfo = { FileName = index == 1 ? OptionsHelper.ExternalToolPath1 : OptionsHelper.ExternalToolPath2 }
				};
				process.Start();
			}
			catch (Exception ex)
			{
				Logging.Log(ex.Message, 0);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Opens an external application used for updating the 'Spring' and 'Entities' properties files.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void UpdatePropertiesFiles()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);
			try
			{
				string args = String.Empty;

				Project project = ProjectStuff.GetSelectedProject();
				if (project != null)
				{
					//Configuration config = project.ConfigurationManager.ActiveConfiguration;

					string fullPath = project.Properties.Item("FullPath").Value.ToString();

					string springPropertiesFile = Path.Combine(fullPath, "Spring.properties");
					string entityPropertiesFile = Path.Combine(fullPath, "Entity.properties");


					if (File.Exists(springPropertiesFile))
						args += "\"" + springPropertiesFile + "\" ";

					if (File.Exists(entityPropertiesFile))
						args += "\"" + entityPropertiesFile + "\"";
				}

				string vsToolsPath = Path.Combine(Path.GetDirectoryName(OptionsHelper.ExternalToolPath1) ?? "", "UpdatePropertiesFiles.exe");
				var process = new Process
				{
					StartInfo =
					{
						FileName = vsToolsPath,
						Arguments = args
					}
				};
				process.Start();
			}
			catch (Exception ex)
			{
				Logging.Log(ex.Message, 0);
			}
		}
	}
}