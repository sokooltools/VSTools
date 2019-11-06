using System;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// VsProperties
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class VsProperties
	{
		// Open a Visual C# project before running this add-in.
		public static void ProjectConfigProperties()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				string msg = null;

				Project project = Connect.ApplicationObject.Solution.Projects.Item(1);
				Configuration config = project.ConfigurationManager.ActiveConfiguration;

				//string fullPath = project.Properties.Item("FullPath").Value.ToString();
				//string outputPath = config.Properties.Item("OutputPath").Value.ToString();
				//string binDir = System.IO.Path.Combine(fullPath, outputPath);
				//string targetName = project.Properties.Item("AssemblyName").Value.ToString();

				Properties projProps = project.Properties;

				for (int i = 1; i <= projProps.Count; i++)
				{
					try
					{
						msg += projProps.Item(i).Name + ": " + projProps.Item(i).Value + "\n";
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				MessageBox.Show(msg);

				msg = null;

				//Configuration config = project.ConfigurationManager.ActiveConfiguration;
				Properties configProps = config.Properties;

				for (int i = 1; i <= configProps.Count; i++)
					msg += configProps.Item(i).Name + ": " + configProps.Item(i).Value + "\n";
				MessageBox.Show(msg);

				msg = null;

				Property prop = configProps.Item("PlatformTarget");
				msg += @"The platform target for this project is: " + prop.Value;

				prop = project.Properties.Item("AssemblyName");
				msg += "\nThe assembly name is: " + prop.Value;

				prop = configProps.Item("OutputPath");
				msg += "\nThe output path of this project is set to: " + prop.Value;

				//msg += (@"Changing the warning level to 3...");
				//prop.Value = "3";
				//msg += (@"The warning level for this project is now set to: " + prop.Value);

				if (project.Kind == PrjKind.prjKindCSharpProject)
				{
					msg += "\nThe project is a Visual C# Project";

					prop = configProps.Item("LanguageVersion");
					msg += "\nThe language version value is : " + prop.Value;

					//msg += (@"Setting the language version to ISO-1");
					//prop.Value = "ISO-1";
					//msg += (@"The language version value is now: " + prop.Value);
				}
				MessageBox.Show(msg);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Open a Visual C# project before running this add-in.
		public static void ProjectFolderProps2()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				string msg = null;

				Project project = Connect.ApplicationObject.Solution.Projects.Item(1);

				// Add a new folder to the project.
				//msg += (@"Adding a new folder to the project.");
				//ProjectItem folder = project.ProjectItems.AddFolder("MyFolder");

				ProjectItem folder = project.ProjectItems.Item(1);

                Properties folderProps = folder.Properties;

				Property prop = folderProps.Item("FullPath");
				msg += "\nThe full path of the new folder is: " + prop.Value;

				prop = folderProps.Item("FileName");
				msg += "\nThe file name of the new folder is:" + prop.Value;

				prop = folderProps.Item("URL");
				msg += "\nThe new folder has the following URL:" + prop.Value;

				MessageBox.Show(msg);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Open a Visual C# before running this add-in.
		public static void ProjectFileProps2()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
			{
				string msg = null;
				Project project = Connect.ApplicationObject.Solution.Projects.Item(1);
				ProjectItems projItems = project.ProjectItems;
				for (int i = 1; i <= projItems.Count; i++)
				{
					ProjectItem projItem = projItems.Item(i);
					Property prop = projItem.Properties.Item("FileName");
					msg += @"The file name of item " + i + @" is: " + prop.Value;
					if (prop.Value.ToString().Contains(".cs") || prop.Value.ToString().Contains(".vb"))
					{
						prop = projItem.Properties.Item("FileSize");
						msg += @"\nThe file size of item " + i + @" is: " + prop.Value;
						prop = projItem.Properties.Item("DateCreated");
						msg += @"\nThe creation date of item " + i + @" is: " + prop.Value;
					}
				}
				MessageBox.Show(msg);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}