using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods for obtaining information about the current solution.
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class SolutionStuff
	{
		public static void ShowSolutionProperties()  
		{    
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			Logging.Log();
			try  
			{   // Open a solution before running this example  
				Properties props = Connect.ApplicationObject.Solution.Properties;
				var sb = new StringBuilder();
				sb.AppendLine( "Number of properties in the current solution: " + props.Count);  
				sb.AppendLine( "The application containing this Properties collection is " + props.DTE.Name);  
				sb.AppendLine( "The parent object of the Properties collection is a " + ((Solution)props.Parent).FullName);  
				sb.AppendLine( "The application property returns : " + ((DTE2)props.Application).Name);
				for (int i = 1; i <= props.Count; i++)
				{
					sb.Append($"[{i}] Name: " + props.Item(i).Name);
					try
					{
						sb.AppendLine(", Value: " + props.Item(i).Value);
					}
					catch
					{
						sb.AppendLine(", Value: Error!");
					}
				}
				OutputPane.Activate();
				OutputPane.Clear();
				OutputPane.Write(sb.ToString());
			}  
			catch(Exception ex)  
			{  
				MessageBox.Show(ex.Message);  
			}  
		}  

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the solution build configurations.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void ShowSolutionBuildConfigurations()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log();
			OutputPane.Activate();
			OutputPane.Clear();
			try
			{
				const string divLine0 =
					"---------------------------------------------------------------------------------------------------------------";
				const string divLine1 =
					"==============+===============================================================================================";
				const string hrdLine1 = " Config       | Platform";

				const string divLine2 =
					"--------------+----------------------------------------------------+-------------+----------+-------+--------+";
				const string hdrLine2 =
					"              | Project Name                                       | Config      | Platform | Build | Deploy |";
				const string divLine3 =
					"              +----------------------------------------------------+-------------+----------+-------+--------+";

				var sb = new StringBuilder();

				var usedNames = new List<string>();

				Solution solution2 = Connect.ApplicationObject.Solution;
				var solutionBuild2 = (SolutionBuild2)solution2.SolutionBuild;

				var activeSolutionConfiguration2 = (SolutionConfiguration2)solutionBuild2.ActiveConfiguration;

				sb.AppendLine(divLine0);
				sb.AppendLine("Solution Configurations");
				sb.AppendLine(divLine0);
				usedNames.Clear();

				foreach (string solutionConfigurationName in solutionBuild2.SolutionConfigurations.Cast<SolutionConfiguration2>()
																		   .Select(solutionConfiguration2 => solutionConfiguration2.Name)
																		   .Where(solutionConfigurationName => !usedNames.Contains(solutionConfigurationName)))
				{
					sb.AppendLine(
						$"   - {solutionConfigurationName} {(solutionConfigurationName == activeSolutionConfiguration2.Name ? "***" : "")}");
					usedNames.Add(solutionConfigurationName);
				}

				sb.AppendLine();
				sb.AppendLine(divLine0);
				sb.AppendLine("Solution Platforms");
				sb.AppendLine(divLine0);
				usedNames.Clear();

				foreach (string solutionPlatformName in solutionBuild2.SolutionConfigurations.Cast<SolutionConfiguration2>()
																	  .Select(solutionConfiguration2 => solutionConfiguration2.PlatformName)
																	  .Where(solutionPlatformName => !usedNames.Contains(solutionPlatformName)))
				{
					sb.AppendLine(
						$"   - {solutionPlatformName} {(solutionPlatformName == activeSolutionConfiguration2.PlatformName ? "***" : "")}");
					usedNames.Add(solutionPlatformName);
				}

				sb.AppendLine();
				sb.AppendLine(divLine0);
				sb.AppendLine("*** = \"Active\" ");

				foreach (SolutionConfiguration2 solutionConfiguration2 in solutionBuild2.SolutionConfigurations)
				{
					sb.AppendLine();
					sb.AppendLine(divLine1);
					sb.AppendLine(hrdLine1);
					sb.AppendLine(divLine1);
					sb.AppendLine($" {solutionConfiguration2.Name,-12} | {solutionConfiguration2.PlatformName}");
					sb.AppendLine(divLine2);
					sb.AppendLine(hdrLine2);
					sb.AppendLine(divLine3);

					List<SolutionContext> scList = solutionConfiguration2.SolutionContexts.Cast<SolutionContext>().ToList();
					scList.Sort((x, y) => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); 
						return String.Compare(x.ProjectName, y.ProjectName, StringComparison.Ordinal); });

					Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
					foreach (SolutionContext solutionContext in scList)
					{
						sb.Append("              |");
						sb.Append($" {Path.GetFileNameWithoutExtension(solutionContext.ProjectName),-50} |");
						sb.Append($" {solutionContext.ConfigurationName,-11} |");
						sb.Append($" {solutionContext.PlatformName,-8} |");
						sb.Append($" {GetCheckmark(solutionContext.ShouldBuild),-5} |");
						sb.Append($" {GetCheckmark(solutionContext.ShouldDeploy),-6} |");
						sb.AppendLine();
					}
				}
				OutputPane.Write(sb.ToString());
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a literal value representing a checkmark.
		/// </summary>
		/// <param name="value">if set to <c>true</c> returns '[X]'; otherwise returns '[ ]'.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		private static string GetCheckmark(bool value)
		{
			return value ? " [X]" : " [ ]";
		}
	}
}