using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	public class GeneralOption
	{
		//private const string FORMAT_DOCUMENT_COMMAND = "Edit.FormatDocument";
		//private const string REMOVE_AND_SORT_COMMAND = "Edit.RemoveAndSort";
		//private const string FORMAT_COMMENTS_COMMAND = "VSTools.FormatComments";

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the pattern or patterns which define the file name or names that should have commands executed 
		/// on them,
		/// </summary>
		/// <value>The inclusion file pattern.</value>
		//--------------------------------------------------------------------------------------------------------------
		[DefaultValue("*.cs")]
		public  string InclusionFilePattern { get; set; }

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the pattern or patterns which define the file name or names that should NOT have commands  
		/// executed on them, even if they are defined in the 'Inclusion Pattern'.
		/// </summary>
		/// <value>The exclusion file pattern.</value>
		//--------------------------------------------------------------------------------------------------------------
		[DefaultValue("")]
		public  string ExclusionFilePattern { get; set; }


		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether [exclude generated t4].
		/// </summary>
		/// <value> <c>true</c> if [exclude generated t4]; otherwise, <c>false</c>. </value>
		//--------------------------------------------------------------------------------------------------------------
		[DefaultValue(true)]
		public bool ExcludeGeneratedT4 { get; set; }

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the command (e.g. 'Edit.FormatDocument', 'VSTools.FormatComments', etc.) which should be
		/// executed on each file.
		/// </summary>
		/// <value>The other command.</value>
		//--------------------------------------------------------------------------------------------------------------
		[DefaultValue("")]
		public string Command { get; set; }

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the <see cref="GeneralOption"/> class.
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		public GeneralOption()
		{
			InclusionFilePattern = "*.cs";
			ExclusionFilePattern = "*.vb";
			ExcludeGeneratedT4 = true;
			Command = "VSTools.FormatComments";
		}

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the file filter function.
		/// </summary>
		/// <returns>An indication as to whether the string passed into this function is included, but not excluded;</returns>
		//--------------------------------------------------------------------------------------------------------------
		public Func<string, bool> CreateFileFilterFunc()
		{
			Func<string, bool> inclusionFilter;
			if (String.IsNullOrWhiteSpace(InclusionFilePattern))
				inclusionFilter = name => true;
			else
				inclusionFilter = new WildCard(InclusionFilePattern, WildCardOptions.MultiPattern).IsMatch;
			Func<string, bool> exclusionFilter;
			if (String.IsNullOrWhiteSpace(ExclusionFilePattern))
				exclusionFilter = name => false;
			else
				exclusionFilter = new WildCard(ExclusionFilePattern, WildCardOptions.MultiPattern).IsMatch;
			return name => !exclusionFilter(name) && inclusionFilter(name);
		}

		public Func<string, bool> CreateHierarchyFilter() => ExcludeGeneratedT4 
			? path => Path.GetExtension(path) != ".tt" 
			: (Func<string, bool>)(path => true);

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a list of commands to execute.
		/// </summary>
		/// <returns>A list of commands</returns>
		//--------------------------------------------------------------------------------------------------------------
		public IList<string> GetCommands()
		{
			var commands = new List<string>();
			if (!String.IsNullOrWhiteSpace(Command))
				commands.Add(Command);
			return commands;
		}
	}
}
