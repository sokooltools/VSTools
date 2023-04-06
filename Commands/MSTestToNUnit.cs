using System;
using System.Text.RegularExpressions;
using EnvDTE;
// ReSharper disable InconsistentNaming

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods to convert 'MSTest' unit tests to 'NUnit' unit tests.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class MSTestToNUnit
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Converts tests from Microsoft Visual Studio to nUnit.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void Perform()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log();

			// Select all the text in the window as long as it is c-sharp code (ie., document has a .cs extension).
			if (!Utilities.IsCsDocument)
				throw new ApplicationException("The document must be native C# code in order to convert it from MS Tests to nUnit.");

			TextSelection sel = MyTextSelection.Current();
			sel?.SelectAll();

			// Make sure selection is not empty.
			if (sel == null || sel.Text == String.Empty)
				throw new ApplicationException("There is no text to select.");

			string sTxt = sel.Text;

			string pattern = "using Microsoft.VisualStudio.TestTools.UnitTesting;";
			string replace = "using NUnit.Framework;";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"^([\s]*)\[TestClass[\(\)]*\]";
			replace = "$1[Ignore]\r\n$1[TestFixture]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"\[TestInitialize[\(\)]*\]";
			replace = "[SetUp]";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"\[TestCleanup[\(\)]*\]";
			replace = "[TearDown]";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"\[TestMethod[\(\)]*\]";
			replace = "[Test]";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"\[TestMethod[\(\)]*,[\s\r\n]*";
			replace = "[Test, ";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"//\[ClassInitialize[\(\)]*\]([\s\r\n]*)" +
					  @"//public static void MyClassInitialize\(TestContext testContext\)[\s\r\n]*" +
					  @"//\{[\s\r\n]*//\}[\s\r\n]*//";
			replace = "[TestFixtureSetUp]" +
					  "$1public void MyClassInitialize()" +
					  "$1{" +
					  "$1}\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"//\[ClassCleanup[\(\)]*\]([\s\r\n]*)" +
					  @"//public static void MyClassCleanup\(\)[\s\r\n]*" +
					  @"//\{[\s\r\n]*//\}[\s\r\n]*//";
			replace = "[TestFixtureTearDown]" +
					  "$1public void MyClassCleanup()" +
					  "$1{" +
					  "$1}\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"\[TestInitialize[\(\)]*\][\s\r\n]*" +
					  @"public void MyTestInitialize\(\)[\s\r\n]*" +
					  @"\{[\s\r\n]*\}";
			replace = "//[TestInitialize]\r\n\t" +
					  "//public void MyTestInitialize()\r\n\t" +
					  "//{\r\n\t//}";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"\[TestCleanup[\(\)]*\][\s\r\n]*" +
					  @"public void MyTestCleanup\(\)[\s\r\n]*" +
					  @"\{[\s\r\n]*\}";
			replace = "//[TestCleanup]\r\n\t" +
					  "//public void MyTestCleanup()\r\n\t" +
					  "//{\r\n\t//}";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)(public|private)( TestContext testContextInstance;[\s]*)$[\s\r\n]*";
			//replace = "$1//$2$3";
			replace = "";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^[\s]*///[\s]*<summary>[\s\r\n]*" +
					  @"///Gets or sets the test context which provides[\s\r\n]*" +
					  @"///information about and functionality for the current test run.[\s\r\n]*" +
					  @"///[\s]*</summary>[\s\r\n]*";
			replace = "\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern =
				@"[\s\r\n]*(public|private) TestContext TestContext[\s\r\n]*" +
				@"\{[\s\r\n]*" +
				@"get[\s\r\n]*" +
				@"\{[\s\r\n]*" +
				@"return testContextInstance;[\s\r\n]*" +
				@"\}[\s\r\n]*" +
				@"set[\s\r\n]*" +
				@"\{[\s\r\n]*" +
				@"testContextInstance = value;[\s\r\n]*" +
				@"\}[\s\r\n]*\}";
			replace = "\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern =
				@"[\s\r\n]*(public|private) TestContext TestContext[\s\r\n]*" +
				@"\{[\s\r\n]*" +
				@"get[\s\r\n]*" +
				@";[\s\r\n]*" +
				@"set[\s\r\n]*" +
				@";[\s\r\n]*" +
				@"\}[\s\r\n]*\}";
			replace = "\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)(public|private)( TestContext TestContext \{ get; set; \}[\s]*)$[\s\r\n]*";
			//replace = "$1//$2$3";
			replace = "";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)(\[DeploymentItem\()";
			replace = "$1//$2";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"actual;[\s\r\n]*(actual)";
			replace = "$1";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"actual;[\s\r\n]*([^;]*;)([\s\r\n]*)(actual[^;]*;)";
			replace = "$3$2$1";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*//\})[\s\r\n]*//";
			replace = "$1\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = "and is intended";
			replace = "and is intended ";
			sTxt = sTxt.Replace(pattern, replace);

			pattern = @"Additional test attributes[\s\r\n]*" +
					  @"//[\s\r\n]*" +
					  @"//You can use the following additional attributes as you write your tests:[\s\r\n]*" +
					  @"//([\s\r\n]*)";
			replace = "Initialize and Cleanup\n$1";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			const string SUM_BEG = "/// <summary>";
			const string SUM_END = "/// </summary>";

			pattern = @"^([\s]*)//Use ClassInitialize[^\n]+$";
			replace = "$1" + SUM_BEG + @"$1/// Executed before running the first test method in the class.$1" + SUM_END;
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)//Use ClassCleanup[^\n]+$";
			replace = "$1" + SUM_BEG + @"$1/// Executed after all test methods in this class have run.$1" + SUM_END;
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)//Use TestInitialize[^\n]+$";
			replace = "$1//" + SUM_BEG + @"$1///// Executed before each test is run.$1//" + SUM_END;
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)//Use TestCleanup[^\n]+$";
			replace = "$1//" + SUM_BEG + @"$1///// Executed after each test has run.$1//" + SUM_END;
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^[\s]*$[\n]+";
			replace = "\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			// Paste the modified text back into the selection.
			Utilities.PasteTextIntoSelection(sel, sTxt);

			sel.StartOfDocument();
		}
	}
}