// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

using System;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// MatchInfo
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class MatchInfo
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="MatchInfo"/> class.
		/// </summary>
		/// <param name="fullFilename">The full filename.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		/// <param name="matchContent">Content of the match.</param>
		/// <param name="matchContext">The match context.</param>
		/// <param name="startLineOffset">The start line offset.</param>
		/// <param name="matchIndex">Index of the match.</param>
		/// <param name="matchLength">Length of the match.</param>
		/// <param name="endLineOffset">The end line offset.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public MatchInfo(string fullFilename, int startLine, int endLine, string matchContent, string matchContext,
						 int startLineOffset = 0, int matchIndex = 0, int matchLength = 0, int endLineOffset = 0)
		{
			FullFilename = fullFilename;
			StartLine = startLine;
			EndLine = endLine;
			MatchContent = matchContent;
			MatchContext = matchContext;
			StartLineOffset = startLineOffset;

			MatchIndex = matchIndex;
			MatchLength = matchLength;
			EndLineOffset = endLineOffset;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="MatchInfo"/> class.
		/// </summary>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		/// <param name="matchContent">Content of the match.</param>
		/// <param name="startLineOffset">The start line offset.</param>
		/// <param name="matchIndex">Index of the match.</param>
		/// <param name="matchLength">Length of the match.</param>
		/// <param name="endLineOffset">The end line offset.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public MatchInfo(int startLine, int endLine, string matchContent, int startLineOffset, int matchIndex, int matchLength,
						 int endLineOffset)
			: this(String.Empty, startLine, endLine, matchContent, String.Empty, startLineOffset, matchIndex, matchLength,
				endLineOffset)
		{
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full filename.
		/// </summary>
		/// <value>The full filename.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public string FullFilename { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the start line.
		/// </summary>
		/// <value>The start line.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int StartLine { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the end line.
		/// </summary>
		/// <value>The end line.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int EndLine { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the content of the match.
		/// </summary>
		/// <value>The content of the match.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public string MatchContent { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match context.
		/// </summary>
		/// <value>The match context.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public string MatchContext { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the start line offset.
		/// </summary>
		/// <value>The start line offset.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int StartLineOffset { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of the match.
		/// </summary>
		/// <value>The index of the match.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int MatchIndex { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the length of the match.
		/// </summary>
		/// <value>The length of the match.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int MatchLength { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the end line offset.
		/// </summary>
		/// <value>The end line offset.</value>
		//------------------------------------------------------------------------------------------------------------------------
		public int EndLineOffset { get; private set; }
	}
}