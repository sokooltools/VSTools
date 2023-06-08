using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SokoolTools.VsTools
{
	public class WildCard
	{
		//..................................................................................................................................

		#region Private Declarations

		private const char Delimiter = ';';

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The regexes
		/// </summary>
		//--------------------------------------------------------------------------------------------------------------
		private readonly Regex[] _regexes;

		#endregion

		//..................................................................................................................................

		#region Public Methods

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="WildCard"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="options">The options.</param>
		//--------------------------------------------------------------------------------------------------------------
		public WildCard(string pattern, WildCardOptions options = WildCardOptions.SinglePattern) =>
			_regexes = ConvertRegexPatterns(pattern, options).Select(x => new Regex(x)).ToArray();

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified input is a match.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns><c>true</c> if the specified input is match; otherwise, <c>false</c>.</returns>
		//--------------------------------------------------------------------------------------------------------------
		public bool IsMatch(string input) => _regexes.Any(x => x.IsMatch(input));

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified input is match.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="options">The options.</param>
		/// <returns><c>true</c> if the specified input is match; otherwise, <c>false</c>.</returns>
		//--------------------------------------------------------------------------------------------------------------
		public static bool IsMatch(string input, string pattern, WildCardOptions options = WildCardOptions.SinglePattern) =>
			ConvertRegexPatterns(pattern, options).Any(x => Regex.IsMatch(input, x));

		#endregion

		//..................................................................................................................................

		#region Private Methods

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Converts the specified wildcard pattern into a valid regex pattern.
		/// </summary>
		/// <param name="wildCardPattern">The wild card pattern.</param>
		/// <returns></returns>
		//--------------------------------------------------------------------------------------------------------------
		private static string ConvertRegexPattern(string wildCardPattern) =>
			"^" + Regex.Escape(wildCardPattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";

		//--------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Converts the specified wildcard pattern according to the specified option and returns an enumerable 
		/// containing valid regex patterns.
		/// </summary>
		/// <param name="wildCardPattern">The pattern containing zero or more wildcards.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		//--------------------------------------------------------------------------------------------------------------
		private static IEnumerable<string> ConvertRegexPatterns(string wildCardPattern, WildCardOptions options)
		{
			if (String.IsNullOrEmpty(wildCardPattern))
				return Enumerable.Empty<string>();

			return options.HasFlag(WildCardOptions.MultiPattern) 
				? wildCardPattern.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(WildCard.ConvertRegexPattern) 
				: new[] { WildCard.ConvertRegexPattern(wildCardPattern)};
		}

		#endregion
	}
}
