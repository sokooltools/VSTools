using System;
using System.Text.RegularExpressions;
using EnvDTE;
// ReSharper disable InconsistentNaming

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Provides methods to convert 'NUnit' unit tests to 'MSTest' unit tests.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal static class NUnitToMSTest
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Converts tests from nUnit to Microsoft Test.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void Perform()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log();

			// Select all the text in the window as long as it is c-sharp code (ie., document has a .cs extension).
			if (!Utilities.IsCsDocument)
				throw new ApplicationException("The document must be native C# code in order to convert it from nUnit to MS Test.");

			TextSelection sel = MyTextSelection.Current();
			sel?.SelectAll();

			// Make sure selection is not empty.
			if (sel == null || sel.Text == String.Empty)
				throw new ApplicationException("The document must not be empty.");

			string sTxt = sel.Text;

			string pattern = "using NUnit.Framework;";
			string replace = "using Microsoft.VisualStudio.TestTools.UnitTesting;";
			sTxt = Regex.Replace(sTxt, pattern, replace);

			pattern = @"^([\s/]*)\[TestFixture\]";
			replace = @"$1[TestClass]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			if (!sTxt.Contains("public TestContext TestContext"))
			{
				pattern = @"^([\s]*public class [a-zA-z0-9]+[\s\n]*{)";
				replace = "$1\n" +
						  "\t\t/// <summary>\n" +
						  "\t\t///Gets or sets the test context which provides \n" +
						  "\t\t///information about and functionality for the current test run.\n" +
						  "\t\t///</summary>\n" +
						  "\t\tpublic TestContext TestContext { get; set; }\n";
				sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);
			}

			pattern = @"^([\s/]*)\[SetUp[\(\)]*\]";
			replace = @"$1[TestInitialize]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s\/]*)\[TearDown[\(\)]*\]";
			replace = @"$1[TestCleanup]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s/]*)\[TestFixtureSetUp[\(\)]*\]";
			replace = @"$1[ClassInitialize]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s/]*)\[TestFixtureTearDown[\(\)]*\]";
			replace = @"$1[ClassCleanup]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"public void MyClassInitialize\(\)";
			replace = @"public void MyClassInitialize(TestContext testContext)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"public void MyClassCleanup\(\)";
			replace = @"public void MyClassCleanup()";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s/]*)\[Test[\(\)]*\]";
			replace = @"$1[TestMethod]";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^([\s]*)//(\[DeploymentItem\()";
			replace = @"$1$2";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.Greater\(([^,]+),([^\)]+),([^\)]+)\)";
			replace = @"Assert.IsTrue($1 >$2,$3)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.Greater\(([^,]+),([^\),]+)\)";
			replace = @"Assert.IsTrue($1 >$2)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.GreaterOrEqual\(([^,]+),([^\)]+),([^\)]+)\)";
			replace = @"Assert.IsTrue($1 >=$2,$3)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.GreaterOrEqual\(([^,]+),([^\),]+)\)";
			replace = @"Assert.IsTrue($1 >=$2)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.Less\(([^,]+),([^\)]+),([^\)]+)\)";
			replace = @"Assert.IsTrue($1 <$2,$3)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.Less\(([^,]+),([^\),]+)\)";
			replace = @"Assert.IsTrue($1 <$2)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.LessOrEqual\(([^,]+),([^\)]+),([^\)]+)\)";
			replace = @"Assert.IsTrue($1 <=$2,$3)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.LessOrEqual\(([^,]+),([^\),]+)\)";
			replace = @"Assert.IsTrue($1 <=$2)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.Null\(";
			replace = @"Assert.IsNull(";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.NotNull\(";
			replace = @"Assert.IsNotNull(";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.True\(";
			replace = @"Assert.IsTrue(";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.False\(";
			replace = @"Assert.IsFalse(";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.IsEmpty\(([^\)]+)\)";
			replace = @"Assert.IsTrue($1 == String.Empty)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"Assert.IsNotEmpty\(([^\)]+)\)";
			replace = @"Assert.IsFalse($1 == String.Empty)";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//   Assert.AreEqual
			//   Assert.AreNotEqual
			//   Assert.AreSame
			//   Assert.Fail
			// * Assert.Greater
			// * Assert.GreaterOrEqual
			//   Assert.Inconclusive
			// * Assert.IsEmpty
			//   Assert.IsFalse
			//   Assert.IsInstanceOfType
			// * Assert.IsNotEmpty
			//   Assert.IsNotNull
			//   Assert.IsNull
			//   Assert.IsTrue
			// * Assert.Less
			// * Assert.LessOrEqual
			// * Assert.NotNull
			// * Assert.Null
			// * Assert.True

			// --  TODO: Still need to take a look at the following.

			//pattern = @"actual;[\s\r\n]*(actual)";
			//replace = "$1";
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = @"actual;[\s\r\n]*([^;]*;)([\s\r\n]*)(actual[^;]*;)";
			//replace = "$3$2$1";
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = @"^([\s]*//\})[\s\r\n]*//";
			//replace = "$1\n";
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = "and is intended";
			//replace = "and is intended ";
			//sTxt = sTxt.Replace(pattern, replace);

			//pattern = @"Additional test attributes[\s\r\n]*" +
			//@"//[\s\r\n]*" +
			//@"//You can use the following additional attributes as you write your tests:[\s\r\n]*" +
			//@"//([\s\r\n]*)";
			//replace = "Initialize and Cleanup\n$1";
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//const string SUM_BEG = "/// <summary>";
			//const string SUM_END = "/// </summary>";

			//pattern = @"^([\s]*)//Use ClassInitialize[^\n]+$";
			//replace = "$1" + SUM_BEG + @"$1/// Executed before running the first test method in the class.$1" + SUM_END;
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = @"^([\s]*)//Use ClassCleanup[^\n]+$";
			//replace = "$1" + SUM_BEG + @"$1/// Executed after all test methods in this class have run.$1" + SUM_END;
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = @"^([\s]*)//Use TestInitialize[^\n]+$";
			//replace = "$1//" + SUM_BEG + @"$1///// Executed before each test is run.$1//" + SUM_END;
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			//pattern = @"^([\s]*)//Use TestCleanup[^\n]+$";
			//replace = "$1//" + SUM_BEG + @"$1///// Executed after each test has run.$1//" + SUM_END;
			//sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			pattern = @"^[\s]*$[\n]+";
			replace = "\n";
			sTxt = Regex.Replace(sTxt, pattern, replace, RegexOptions.Multiline);

			// Paste the modified text back into the selection.
			Utilities.PasteTextIntoSelection(sel, sTxt);

			sel.StartOfDocument();
		}
	}
}