using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SokoolTools.VsTools
{
	//---------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//---------------------------------------------------------------------------------------------------------------------------
	internal static class JSFormatter
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Compacts javascript by removing all whitespace whereever possible.
		/// </summary>
		/// <param name="sTxt">javascript to compact</param>
		/// <param name="bStripComments"></param>
		/// <returns>compacted javascript</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetCompactedScript(string sTxt, bool bStripComments)
		{
			bool bDoubleQuote = false;
			bool bSingleQuote = false;

			// Remove double-empty lines
			sTxt = sTxt.Replace("\r\n", "\n");
			sTxt = sTxt.Replace("\n\n", "\n");

			// Remove trailing whitespace from each line.
			sTxt = Regex.Replace(sTxt, @"[\s]+$", "", RegexOptions.Multiline);

			// Remove leading whitespace from each line.
			sTxt = Regex.Replace(sTxt, @"^[\s]+", "", RegexOptions.Multiline);

			//// Temporary substitution for linefeeds inside block comments  "/*   */"
			//string sTmp;
			//Regex regexObj = new Regex(@"[\n]?/\*.*?\*/[\n]?", RegexOptions.Multiline | RegexOptions.Singleline);
			//Match matchObj = regexObj.Match(sTxt);
			//while (matchObj.Success)
			//{
			//   sTmp = matchObj.Value;
			//   sTmp = sTmp.Replace('\n', '¶');
			//   sTxt = sTxt.Remove(matchObj.Index, matchObj.Length);
			//   sTxt = sTxt.Insert(matchObj.Index, sTmp);
			//   matchObj = matchObj.NextMatch();
			//}

			// Maintain linefeed for end-of-line comments.
			sTxt = Regex.Replace(sTxt, @"(\/\/[^\n]*)\n", "$1¤", RegexOptions.Multiline);

			// Maintain linefeed for beginning-of-line comments.
			sTxt = Regex.Replace(sTxt, @"^[\s]*(\/\/)", "¤$1", RegexOptions.Multiline);

			// Maintain spaces following certain keywords.
			sTxt = Regex.Replace(sTxt, @"\b(function|var|new|return|case|sizeof|typeof)+[\s\n]+", "$1¥");

			// Maintain 'else' not followed by any whitespace and a '{'
			//sTxt = Regex.Replace(sTxt, @"(\n)(\t)([\t]*)else return", @"$2$3else$1$2$2$3return", RegexOptions.Multiline);

			// Remove extra whitespace surrounding 'else'.
			sTxt = Regex.Replace(sTxt, @"\b(else)\b[\s\n]+", "$1 ", RegexOptions.Multiline);

			// First or last character in a word
			// [1]: A numbered capture group. [else]
			//     else
			// First or last character in a word
			// Match if prefix is absent. [[\s\n]+{]
			//     [\s\n]+{
			//         Any character in this class: [\s\n], one or more repetitions

			// Maintain space following 'else' when 'else' not followed a '{'
			sTxt = Regex.Replace(sTxt, @"\b(else)\b(?![\s\n]+{)", "$1¥", RegexOptions.Multiline);

			// Restore linefeed to beginning-of-line and end-of-line comments.
			sTxt = Regex.Replace(sTxt, @"¤", "\n", RegexOptions.Multiline);

			// Remove extra linefeed at beginning of entire string.
			if (sTxt.StartsWith("\n"))
				sTxt = sTxt.Remove(0, 1);


			// Start a new Stringbuilder using the current length of the text.
			var sb = new StringBuilder(sTxt.Length);

			// Convert the text into a character array.
			char[] aTxt = sTxt.ToCharArray();

			for (int i = 0; i < aTxt.Length; i++)
			{
				try
				{
					char chr = aTxt[i];

					// Ignore when char appears inside "'" (single or double-quotes).
					if (bSingleQuote && chr != '\'' || bDoubleQuote && chr != '"')
					{
						sb.Append(chr);
						continue;
					}

					switch (chr)
					{
						case '/': // Forward-slash
						{
							if (IsFollowedBy(aTxt, i, '/')) // It's a comment: Look for ending "\n".
							{
								string str = GetCommentString(aTxt, i);
								i += str.Length;
								if (!bStripComments)
								{
									sb.Append(str);
									if (i + 1 < aTxt.Length)
										sb.Append("\n");
									else
										i -= 1;
								}
								else
								{
									while (sb.ToString().EndsWith("\n"))
										sb.Remove(sb.Length - 1, 1);
									if (i + 1 < aTxt.Length && aTxt[i + 1] == '\n')
										i += 1;
								}
							}
							else if (IsFollowedBy(aTxt, i, '*')) // It's a block comment: Look for the ending "*/".
							{
								string str = GetBlockCommentString(aTxt, i);
								i += str.Length - 1;
								if (!bStripComments)
									sb.Append(str);
								else
								{
									while (sb.ToString().EndsWith("\n"))
										sb.Remove(sb.Length - 1, 1);
									if (i + 1 < aTxt.Length && aTxt[i + 1] == '\n')
										i += 1;
								}
							}
							else if (IsPrecededByAny(aTxt, i, "(,+=&|")) // It's a literal string: Look for the next "/"
							{
								string str = GetLiteralString(aTxt, i);
								sb.Append(str);
								i += str.Length - 1;
							}
							else
								sb.Append(chr);
							break;
						}
						case '\'': // Single-quote
						{
							sb.Append(chr);
							if (!IsPrecededBy(aTxt, i, '\\') || IsPrecededBy(aTxt, i - 1, '\\'))
								// Not preceded by an escape preceded by escape.
								bSingleQuote = !bSingleQuote;
							break;
						}
						case '"': // Double-quote
						{
							sb.Append(chr);
							if (!IsPrecededBy(aTxt, i, '\\') || IsPrecededBy(aTxt, i - 1, '\\'))
								bDoubleQuote = !bDoubleQuote;

							break;
						}
						case '\n': // Linefeed
						{
							if (!IsPrecededByAny(aTxt, i, '{', '}', ';', ':', ',', ')', '+', '|', '&')
								&& !IsFollowedByAny(aTxt, i, '{', '}', ';', ':', ',', '?'))
								sb.Append(chr);
						}
							break;
						case ' ': // Space
						case '\t': // Tab
						{
							break;
						}
						default:
						{
							sb.Append(chr);
							break;
						}
					}
				}
				catch (Exception e)
				{
					string sMessage = LogResults(e, sTxt, sb, i);
					throw new ApplicationException(sMessage);
				}
			}

			// Restore space char following certain keywords.
			sTxt = Regex.Replace(sb.ToString(), @"(function|var|new|return|case|sizeof|typeof|else)+¥", "$1 ");

			// Restore linefeeds back into block comment.
			//sTxt = sTxt.Replace('¶', '\n');

			return sTxt;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Formats Javascript using standard indents and spacing.
		/// </summary>
		/// <param name="sTxt">unformatted javascript</param>
		/// <param name="bStripComments"></param>
		/// <returns>formatted javascript</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetFormattedScript(string sTxt, bool bStripComments)
		{
			bool bDoubleQuote = false;
			bool bSingleQuote = false;
			bool bIsIfStatement = false;
			int iLeftParen = 0;
			int iIndent = 0;
			int iHoldIndent = 0;

			sTxt = GetCompactedScript(sTxt, bStripComments);

			// Replace parts of case statements with metacharacters.
			sTxt = Regex.Replace(sTxt, @"([{};:]+)[\s]*\b(case)\b([^:]*)[\s]*(:)", "$1ß$3ð", RegexOptions.Multiline);
			sTxt = Regex.Replace(sTxt, @"([{};:]+)[\s]*\b(default)\b([^:]*)[\s]*(:)", "$1Þ$3ð", RegexOptions.Multiline);

			// Start a new stringbuilder using the current length of the text.
			var sb = new StringBuilder(sTxt.Length);

			char[] a = sTxt.ToCharArray();

			for (int i = 0; i < a.Length; i++)
			{
				try
				{
					char chr = a[i];

					// Ignore when char appears inside "'" (single-quotes).
					if (bSingleQuote && chr != '\'')
					{
						sb.Append(chr);
						continue;
					}

					// Ignore when char appears inside '"' (double-quotes).
					if (bDoubleQuote && chr != '"')
					{
						sb.Append(chr);
						continue;
					}

					switch (chr)
					{
						case '/':
						{
							if (IsFollowedBy(a, i, '/')) // It's a comment: Look for ending "\n".
							{
								string str = GetCommentString(a, i);
								sb.Append(str);
								i += str.Length - 1;
							}
							else if (IsFollowedBy(a, i, '*')) // It's a block comment: Look for the ending "*/".
							{
								string str = GetBlockCommentString(a, i);
								i += str.Length - 1;
								if (iIndent > 0)
								{
									// Put Tabs into the beginning of each line corresponding to the indent.
									str = Regex.Replace(str, "^", new string('\t', iIndent), RegexOptions.Multiline);
									// Remove any tabs found at beginning of entire string.
									str = Regex.Replace(str, "^[\t]*", "");
								}
								sb.Append(str);
							}
							else if (IsPrecededByAny(a, i, "(,+=&")) // It's a literal string: Look for the next "/"
							{
								string str = GetLiteralString(a, i);
								sb.Append(str);
								i += str.Length - 1;
							}
							else
							{
								if (IsPrecededBy(a, i, ' ') || !IsPrecededByAny(a, i, '\\', '/', '\n', '}', '*'))
									sb.Append(' ');
								if (!IsPrecededBy(a, i, '\\'))
								{
									sb.Append(chr);
									sb.Append(' ');
								}
								else
									sb.Append(chr);
							}
							break;
						}
						case '\n': // New-line
						{
							sb.Append(chr);
							if (!IsFollowedBy(a, i, '}'))
								sb.Append(new string('\t', iIndent));
							break;
						}
						case '\'': // Single-quote
						{
							sb.Append(chr);
							if (!IsPrecededBy(a, i, '\\') || IsPrecededBy(a, i - 1, '\\')) // Not preceded by an escape preceded by escape.
								bSingleQuote = !bSingleQuote;
							break;
						}
						case '"': // Double-quote
						{
							sb.Append(chr);
							if (!IsPrecededBy(a, i, '\\') || IsPrecededBy(a, i - 1, '\\')) // Not preceded by an escape preceded by escape.
								bDoubleQuote = !bDoubleQuote;
							break;
						}
						case ',': // Comma
						{
							sb.Append(chr);
							sb.Append(' ');
							break;
						}
						case ';': // Semi-colon
						{
							sb.Append(chr);
							if (iLeftParen == 0 && !(IsFollowedBy(a, i, '/') && IsFollowedBy(a, i + 1, '/')))
							{
								sb.Append('\n');
								if (IsFollowedBy(a, i, '*')) // Case statement.
									sb.Append(new string('\t', iIndent - 1));
								else if (!IsFollowedBy(a, i, '}'))
									sb.Append(new string('\t', iIndent));
							}
							else
								sb.Append(' ');
							break;
						}
						case '(':
						{
							sb.Append(chr);
							iLeftParen += 1;
							break;
						}
						case ')':
						{
							sb.Append(chr);
							iLeftParen -= 1;
							if (iLeftParen == 0)
							{
								if (bIsIfStatement || IsFollowedByAlpha(a, i))
								{
									// Add linefeed to end of "if" or "for" statement.
									i = SkipLinefeeds(a, i);
									if (!IsFollowedByAny(a, i, "{;+-*/|&"))
									{
										sb.Append('\n');
										sb.Append(new string('\t', iIndent + 1));
									}
									else
										sb.Append(' ');
									bIsIfStatement = false;
								}
							}
							break;
						}
						case '+':
						case '-':
						{
							if (IsPrecededBy(a, i, ';'))
							{
								sb.Append(' ');
								sb.Append(chr);
							}
							else if (!IsPrecededByAny(a, i, "+-"))
							{
								if (!IsFollowedByAny(a, i, '+', '-', ')'))
									sb.Append(' ');
								sb.Append(chr);
								if (!IsPrecededByAny(a, i, '=', '>', '<', '"'))
								{
									if (!IsFollowedByAny(a, i, '=', '+', '-'))
										sb.Append(' ');
								}
								else if (IsPrecededByAny(a, i, '"', '\''))
									sb.Append(' ');
							}
							else
							{
								sb.Append(chr);
								if (!IsFollowedByAny(a, i, ')', ';', '=') && !IsPrecededByAny(a, i, '+', '-'))
									sb.Append(' ');
							}
							break;
						}
						case '=':
						case '|':
						case '&':
						{
							if (!IsPrecededByAny(a, i, "!=+-*/<>|&"))
							{
								if (!IsFollowedByAny(a, i, "+-*"))
									sb.Append(' ');
								sb.Append(chr);
								if (!IsFollowedByAny(a, i, "=+-*)|&"))
									sb.Append(' ');
							}
							else
							{
								sb.Append(chr);
								if (!IsFollowedByAny(a, i, "=-;)"))
									sb.Append(' ');
							}
							break;
						}
						case '*':
						{
							if (!IsPrecededByAny(a, i, "*/") && !IsFollowedByAny(a, i, "*/"))
							{
								sb.Append(' ');
								sb.Append(chr);
								sb.Append(' ');
							}
							else
							{
								if (IsFollowedBy(a, i, '/'))
									sb.Append(' ');
								sb.Append(chr);
							}
							break;
						}
						case '!':
						{
							if (IsFollowedBy(a, i, '='))
								sb.Append(' ');
							sb.Append(chr);
							break;
						}
						case '<':
						{
							if (!IsPrecededByAny(a, i, "\n<"))
								sb.Append(' ');
							sb.Append(chr);
							if (!IsFollowedByAny(a, i, "!=<-+"))
								sb.Append(' ');
							break;
						}
						case '>':
						case '%':
						{
							if (!IsPrecededByAny(a, i, "\n>"))
								sb.Append(' ');
							sb.Append(chr);
							if (!IsFollowedByAny(a, i, "!=>-+"))
								sb.Append(' ');
							break;
						}
						case '{':
						{
							sb.Append('\n');
							sb.Append(new string('\t', iIndent));
							sb.Append(chr);
							sb.Append('\n');
							iIndent += 1;
							if (!IsFollowedBy(a, i, '}'))
								sb.Append(new string('\t', iIndent));
							break;
						}
						case '}':
						{
							if (iIndent > 0)
								iIndent -= 1;
							if (iHoldIndent > 0 && iHoldIndent == iIndent - 1)
							{
								iHoldIndent = 0;
								iIndent -= 1;
							}
							sb.Append('\n');
							sb.Append(new string('\t', iIndent));
							sb.Append(chr);
							if (!IsFollowedByAny(a, i, ';'))
							{
								if (!IsFollowedByAny(a, i, "}\n"))
								{
									sb.Append('\n');
									sb.Append(new string('\t', iIndent));
								}
							}
							break;
						}
						case ':':
						case '?':
						{
							sb.Append(' ');
							sb.Append(chr);
							sb.Append(' ');
							break;
						}
						case 'e': // handle 'else'
						{
							sb.Append(chr);
							break;
						}
						case 'f': // handle 'if' statement
						{
							sb.Append(chr);
							if (i > 1 && a[i - 1] == 'i' && IsFollowedBy(a, i, '('))
							{
								sb.Append(' ');
								bIsIfStatement = true;
							}
							break;
						}
						case 'r': // handle 'for' loop
						{
							sb.Append(chr);
							if (i > 1 && a[i - 2] == 'f' && a[i - 1] == 'o' && IsFollowedBy(a, i, '('))
							{
								sb.Append(' ');
								bIsIfStatement = true;
							}
							break;
						}
						case 'ð': // Handle 'case:' statement.
						{
							if (iHoldIndent == 0)
							{
								iHoldIndent = iIndent - 1;
								iIndent += 1;
							}
							sb.Append(':');
							sb.Append('\n');
							sb.Append(new string('\t', iIndent));
							break;
						}
						case 'ß': // Handle 'case:' statement.
						{
							if (iHoldIndent != 0)
								sb.Remove(sb.Length - 1, 1); // Remove last tab
							sb.Append("case");
							break;
						}
						case 'Þ': // Handle 'default:' case statement.
						{
							sb.Remove(sb.Length - 1, 1); // Remove last tab
							sb.Append("default");
							break;
						}
						default:
						{
							sb.Append(chr);
							break;
						}
					}
				}
				catch (Exception e)
				{
					string sMessage = LogResults(e, sTxt, sb, i);
					throw new ApplicationException(sMessage);
				}
			}

			sTxt = sb.ToString();

			// Remove all empty lines.
			sTxt = Regex.Replace(sTxt, @"^[ \t\r]*\n", @"", RegexOptions.Multiline);

			// Add an empty line at the end of each function.
			sTxt = Regex.Replace(sTxt, "^(}[;]*)", "$1\n", RegexOptions.Multiline);

			if (!bStripComments)
			{
				// Add space between semi-colons and inline comments.
				sTxt = sTxt.Replace(";//", "; //");
			}

			// Replace case statement metacharacters.
			sTxt = sTxt.Replace("ß", "case").Replace("Þ", "default").Replace("ð", ":");

			if (sTxt.EndsWith("\n"))
				sTxt = sTxt.Remove(sTxt.Length - 1, 1);

			return sTxt;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Logs the results.
		/// </summary>
		/// <param name="e">The exception.</param>
		/// <param name="s">The string.</param>
		/// <param name="sb">The string builder object.</param>
		/// <param name="i">The i.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string LogResults(Exception e, string s, StringBuilder sb, int i)
		{
			string sFileName = Path.Combine(Path.GetTempPath(), "JSFormatter_Error.log");
			try
			{
				string sDivideLine = new string('*', 70) + "\n";
				s += $"\n\n{sDivideLine}An error occured at character {i} of the raw text above.\n{sDivideLine}";
				s += "Following is the formatted text prior to encountering the error:\n";
				s += sDivideLine + "\n";
				s += sb.ToString();
				using (var sw = new StreamWriter(sFileName, false))
					sw.Write(s);
			}
			catch
			{
			}
			return $"Error: {e.Message}\n\nPlease see the following results file for more info:\n   '{sFileName}'.";
		}

		private static bool IsPrecededBy(IList<char> a, int i, char c)
		{
			for (int n = i - 1; n > -1; n--)
			{
				if (a[n] != ' ')
					return a[n] == c;
			}
			return false;
		}

		private static bool IsPrecededByAny(char[] a, int i, params char[] anyOf)
		{
			for (int n = i - 1; n > -1; n--)
			{
				if (a[n] != ' ')
					return a[n].ToString(CultureInfo.InvariantCulture).IndexOfAny(anyOf) > -1;
			}
			return false;
		}

		private static bool IsPrecededByAny(IList<char> a, int i, string pattern)
		{
			for (int n = i - 1; n > -1; n--)
			{
				if (a[n] != ' ')
					return pattern.IndexOf(a[n]) > -1;
			}
			return false;
		}

		private static bool IsFollowedBy(IList<char> a, int i, char c)
		{
			for (int n = i + 1; n < a.Count; n++)
			{
				if (a[n] != ' ')
					return a[n] == c;
			}
			return false;
		}

		private static bool IsFollowedByAlpha(char[] a, int i)
		{
			for (int n = i + 1; n < a.Length; n++)
			{
				if (a[n] != ' ')
					return Regex.IsMatch(a[n].ToString(CultureInfo.InvariantCulture), "[a-z]", RegexOptions.IgnoreCase);
			}
			return false;
		}

		private static bool IsFollowedByAny(char[] a, int i, params char[] anyOf)
		{
			for (int n = i + 1; n < a.Length; n++)
			{
				if (a[n] != ' ')
					return a[n].ToString(CultureInfo.InvariantCulture).IndexOfAny(anyOf) > -1;
			}
			return false;
		}

		private static bool IsFollowedByAny(IList<char> a, int i, string pattern)
		{
			for (int n = i + 1; n < a.Count; n++)
			{
				if (a[n] != ' ')
					return pattern.IndexOf(a[n]) > -1;
			}
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Skips the linefeeds.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="i">The index into the array.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static int SkipLinefeeds(IList<char> a, int i)
		{
			for (int n = i + 1; n < a.Count; n++)
			{
				if (a[n] != '\n')
					return n - 1;
			}
			return i;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the comment string.
		/// </summary>
		/// <param name="a">Array of characters</param>
		/// <param name="i">The index into the array.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetCommentString(IList<char> a, int i)
		{
			var sb = new StringBuilder();
			while (i + 1 < a.Count)
			{
				sb.Append(a[i]);
				if (a[i + 1] == '\n')
					break;
				i++;
			}
			return sb.ToString();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the block comment string.
		/// </summary>
		/// <param name="a">Array of characters</param>
		/// <param name="i">The index into the array.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetBlockCommentString(IList<char> a, int i)
		{
			int iCnt = 0;
			var sb = new StringBuilder();
			while (i + 1 < a.Count)
			{
				if (a[i] == '/' && a[i + 1] == '*')
					iCnt += 1;
				else if (a[i] == '*' && a[i + 1] == '/')
				{
					if (iCnt == 1)
					{
						sb.Append("*/");
						break;
					}
					iCnt -= 1;
				}
				sb.Append(a[i]);
				i++;
			}
			return sb.ToString();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the literal string.
		/// </summary>
		/// <param name="a">Array of characters</param>
		/// <param name="i">The index into the array.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string GetLiteralString(IList<char> a, int i)
		{
			var sb = new StringBuilder();
			while (i < a.Count)
			{
				sb.Append(a[i]);
				if (a[i + 1] == '/' && (a[i] != '\\' || a[i] == '\\' && a[i - 1] == '\\'))
				{
					sb.Append(a[i + 1]);
					break;
				}
				i++;
			}
			return sb.ToString();
		}

	}
}