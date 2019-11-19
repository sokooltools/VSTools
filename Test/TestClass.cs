// ReSharper disable UnusedMember.Global

using System;
using System.Text.RegularExpressions;

namespace VsToolsTest
{
	/// <summary>
	/// Summary description for TestClass.
	/// </summary>
	[System.ComponentModel.Description(@"code")]
	[System.ComponentModel.DesignerCategory(@"code")]
	public class TestClass
	{
		#region Region1

		public TestClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private bool _something;

		public bool Something
		{
			get { if (true) _something = false; return _something; }
			set { if (true) _something = true; _something = value; }
		}

		// ReSharper disable UnusedMember.Global
		private static string GetStringTest
		{
			get;
			set;
		}

		#endregion
		#region Region2

		/// <summary>
		/// Implements the Exec method of the IDTCommandTarget interface.
		/// This is called when the command is invoked.
		/// </summary>
		/// <param name="sTxt">This is the text parameter that is used for the text for this example.</param>
		/// <param name="iMaxLength">This is the maximum length parameter for text.</param>
		/// <returns>This thing returns a whole lot of stuff and then some more stuff.</returns>
		public void Test1(string sTxt, int iMaxLength)
		{
			// An internal comment
			Console.WriteLine(Regex.Replace("abc", "b", "x", RegexOptions.IgnoreCase));

			// This is a great big long summary comment that shows how wrapping comments works.
			// 
			// A separate paragraph.
		}

		/// <summary>
		/// This is a great big long summary description that shows how wrapping comments works.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when an argument is not supplied.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when too many business rules are found.</exception>
		/// <exception cref="Exception">Thrown when the reference data is not available.</exception>
		public void Test2()
		{
		}

		#endregion
		#region Region3

		/// <summary>
		/// Utility function to get the region name from a text line which is assumed to be a start region compiler definition.
		/// </summary>
		/// <param name="sPar1">The first parameter to use.</param>
		/// <param name="sPar2">The second parameter to use.</param>
		/// <param name="sPar3">The 3rd parameter to use.</param>
		/// <returns>The name following the region definition (e.g., "#regionx").</returns>
		/// <remarks>
		/// Note that for visual basic, the region name is bracketted by quotes.
		/// </remarks>
		public void Test3(string sPar1, string sPar2, string sPar3)
		{
		}
		#region Nested Region3-a

		// Inside 3-a
		#region Nested Region3a-1

		// Inside 3a-1

		#endregion
		#region Nested Region3a-2

		// Inside 3a-2

		#endregion

		#endregion

		/// <summary>
		/// 1) This is a comment containing a hard return at the end of this line.
		/// 
		/// 2) This is the beginning of a new line which follows the previous line.
		/// </summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void Test4()
		{
			// Utility function to get the region name from a text line which is assumed to be a start region compiler definition.
		}

		#endregion

		/// <summary>
		/// Updates the network adapter corresponding to the specified old IP address with the new IP address specified.
		/// </summary>
		/// <exception cref="System.InvalidProgramException"></exception>
		/// <exception cref="System.FormatException"></exception>
		public void Test5()
		{
		}
	}
}
