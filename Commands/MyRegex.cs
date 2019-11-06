using System.Text.RegularExpressions;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides method which overloads the standard Regex 'Replace' method by optionally logging the text of what is being 
	/// searched and replaced and according to a specified logging level.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class MyRegex
	{
		public const RegexOptions OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// In a specified input string, replaces all strings that match a specified regular expression with a specified
		/// replacement string. Specified options modify the matching operation. When a desciption is specified a message 
		/// depicting what is being searched and replaced will be output to the log file.
		/// </summary>
		/// <param name="input">The string to search for a match.</param>
		/// <param name="pattern">The regular expression pattern to match.</param>
		/// <param name="replacement">The replacement string.</param>
		/// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
		/// <param name="description">The description of the current replacement. (Default = null)</param>
		/// <param name="logLevel">The log level. (Default = 3)</param>
		/// <returns>
		/// A new string that is identical to the input string, except that the replacement string takes the place of each
		/// matched string. If <paramref name="pattern" /> is not matched in the current instance, the method returns the
		/// current instance unchanged.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">A regular expression parsing error occurred.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="input" />, <paramref name="pattern" />, or <paramref name="replacement" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="options" /> is not a valid bitwise combination of <see 
		/// cref="T:System.Text.RegularExpressions.RegexOptions" /> values.
		/// </exception>
		/// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">
		/// A time-out occurred. For more information about time-outs, see the Remarks section.
		/// </exception>
		//------------------------------------------------------------------------------------------------------------------------
		public static string Replace(string input, string pattern, string replacement, RegexOptions options, string description = null,
									 int logLevel = 3)
		{
			if (description != null)
				Logging.LogReplace(pattern, replacement, description, logLevel);
			return Regex.Replace(input, pattern, replacement, options);
		}
	}
}
