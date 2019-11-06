using System.Text.RegularExpressions;

namespace SokoolTools.VsTools.FindAndReplace.Helper
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides helper methods for working with strings.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public static class StringHelper
	{
		private const char NEW_LINE_CHARACTER = '\n';
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the string consists of multiple lines.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns><c>true</c> if the string consists of multiple lines; otherwise, <c>false</c>.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static bool IsMultipleLine(string source)
		{
			return GetLineCount(ref source) > 1;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of lines in the specified string from the beginning of the string up to the specified length.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="length">The length of the string.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static int GetLineCount(ref string source, int length)
		{
			if (string.IsNullOrEmpty(source))
				return 1;
			return Regex.Matches(source.Substring(0, length), "\n").Count + 1;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of lines contained in the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static int GetLineCount(ref string source)
		{
			return GetLineCount(ref source, source.Length - 1);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets just the single line a string token is found on in order to provide a display context back
		/// to the user.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="startIndex">The starting index within the string.</param>
		/// <param name="endIndex">The end index within the string.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetStringContext(ref string source, int startIndex, int endIndex)
		{
			return GetStringContext(ref source, startIndex, endIndex, 0, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the line a string token is found on along with the specified number of lines before and
		/// number of lines after it in order to provide a display context back to the user.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="endIndex">The end index.</param>
		/// <param name="frontLineCount">The front line count.</param>
		/// <param name="backLineCount">The back line count.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetStringContext(ref string source, int startIndex, int endIndex, int frontLineCount,
											  int backLineCount)
		{
			int contextStartPos = source.LastIndexOf(NEW_LINE_CHARACTER, startIndex);
			if (contextStartPos < 0) contextStartPos = 0;

			if (frontLineCount > 0 && contextStartPos > 0)
			{
				for (int i = 1; i <= frontLineCount; i++)
				{
					int newLinePos = source.LastIndexOf(NEW_LINE_CHARACTER, contextStartPos - 1);
					if (newLinePos > 0)
						contextStartPos = newLinePos;
					else
					{
						contextStartPos = 0;
						break;
					}
				}
			}

			int contextEndPos = source.IndexOf(NEW_LINE_CHARACTER, endIndex);
			if (contextEndPos < 0) contextEndPos = source.Length;

			if (backLineCount > 0 && contextEndPos < source.Length)
			{
				for (int i = 1; i <= backLineCount; i++)
				{
					int newLinePos = source.IndexOf(NEW_LINE_CHARACTER, contextEndPos + 1);
					if (newLinePos > 0)
						contextEndPos = newLinePos;
					else
					{
						contextEndPos = source.Length;
						break;
					}
				}
			}
			return contextStartPos == 0
				? source.Substring(0, contextEndPos)
				: source.Substring(contextStartPos + 1, contextEndPos - contextStartPos - 1);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the position in the line.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="absolutePos">The absolute position.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static int GetPosInLine(ref string source, int absolutePos)
		{
			int newlinePos = source.LastIndexOf(NEW_LINE_CHARACTER, absolutePos);
			if (newlinePos == -1)
				return absolutePos + 1;

			// The end position is at "\r\n"
			if (absolutePos == newlinePos)
				newlinePos = source.LastIndexOf(NEW_LINE_CHARACTER, absolutePos - 1);
			return absolutePos - newlinePos;
		}
	}
}