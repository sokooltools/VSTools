using System;
using System.Text.RegularExpressions;
using EnvDTE;

namespace SokoolTools.VsTools.FindAndReplace
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Searcher
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	public abstract class Searcher
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// The top-level Visual Studio automation object.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		protected readonly _DTE Dte;

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Searcher"/> class.
		/// </summary>
		/// <param name="dte">The DTE.</param>
		//----------------------------------------------------------------------------------------------------
		protected Searcher(_DTE dte)
		{
			Dte = dte;
			SearchScope = String.Empty;
			//Pattern = string.Empty;
			RegexOptions = RegexOptions.None;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the search scope.
		/// </summary>
		/// <value>The search scope.</value>
		//----------------------------------------------------------------------------------------------------
		public string SearchScope { get; set; }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the search pattern.
		/// </summary>
		/// <value>The pattern.</value>
		//----------------------------------------------------------------------------------------------------
		public virtual string Pattern { get; set; }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the regex options.
		/// </summary>
		/// <value>The regex options.</value>
		//----------------------------------------------------------------------------------------------------
		public RegexOptions RegexOptions { set; get; }

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the regex options.
		/// </summary>
		/// <param name="isSingleLine">if set to <c>true</c> [is single line].</param>
		/// <param name="isMultipleLine">if set to <c>true</c> [is multiple line].</param>
		/// <param name="isIgnoreCase">if set to <c>true</c> [is ignore case].</param>
		/// <param name="isECMAScript">if set to <c>true</c> [is ECMA script].</param>
		//----------------------------------------------------------------------------------------------------
		public virtual void SetRegexOptions(bool isSingleLine, bool isMultipleLine, bool isIgnoreCase, bool isECMAScript)
		{
			RegexOptions = RegexOptions.None;
			if (isSingleLine) RegexOptions |= RegexOptions.Singleline;
			if (isMultipleLine) RegexOptions |= RegexOptions.Multiline;
			if (isIgnoreCase) RegexOptions |= RegexOptions.IgnoreCase;
			if (isECMAScript) RegexOptions |= RegexOptions.ECMAScript;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the regex options.
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public RegexOptions GetRegexOptions()
		{
			return RegexOptions;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public string Replace(string source, string replacement)
		{
			var rgx = new Regex(Pattern, GetRegexOptions());
			return rgx.Replace(source, replacement);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the string in the specified source that match the replacement pattern.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <param name="startPos">The start pos.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public string Replace(string source, string replacement, int startPos)
		{
			var rgx = new Regex(Pattern, GetRegexOptions());
			return rgx.Replace(source, replacement, 1, startPos);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces all the strings in the specified source that match the replacement pattern.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <param name="repeatCount">The repeat count.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		protected string ReplaceAll(string source, string replacement, out int repeatCount)
		{
			var rgx = new Regex(Pattern, GetRegexOptions());
			repeatCount = rgx.Matches(source).Count;
			return repeatCount > 0 ? rgx.Replace(source, replacement) : String.Empty;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the next.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="replacement">The replace pattern.</param>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		public string ReplaceNext(string source, string replacement)
		{
			var rgx = new Regex(Pattern, GetRegexOptions());
			return rgx.Replace(source, replacement, 1);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the search scope path.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		protected virtual void GetSearchScopePath()
		{
		}
	}
}