namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// This class is used to expose the list of the IDs of all the commands implemented by this package.
	/// </summary>
	/// <remarks>
	/// This list of 'IDs' must match the set of IDs defined inside the 'Buttons' section of the VSCT file.
	/// </remarks>
	//------------------------------------------------------------------------------------------------------------------
	internal static class PkgIds
	{
		public const int VSToolsMenu = 8100;
		// 1 -----------------------------------------------------
		public const int FormatComments = 8210;
		public const int FormatCommentsInAllFiles = 8215;
		public const int RemoveAllDividerLines = 8220;
		// 2 -----------------------------------------------------
		public const int SolutionExplorerCollapse = 8230;
		// 3 -----------------------------------------------------
		public const int RegionsToggleCurrent = 8241;
		public const int RegionsExpandAll = 8242;
		public const int RegionsCollapseAll = 8243;
		//4  -----------------------------------------------------
		//public const int BookmarksSubMenuGroup = 8250;
		//public const int BookmarksSubMenu = 8251;
		public const int BookmarksCut = 8252;
		public const int BookmarksCopy = 8253;
		public const int BookmarksDelete = 8254;
		// 5 -----------------------------------------------------
		//public const int SummariesSubMenuGroup = 8270;
		//public const int SummariesSubMenu = 8271;
		public const int SummariesCollapseAll = 8272;
		public const int SummariesExpandAll = 8273;
		// 6 -----------------------------------------------------
		//public const int CodeBlockSubMenuGroup = 8280;
		//public const int CodeBlockSubMenu = 8281;
		public const int CodeBlockCollapse = 8282;
		public const int CodeBlockExpand = 8283;
		// 7 -----------------------------------------------------
		//public const int ConvertUnitTestsSubMenuGroup = 8290;
		//public const int ConvertUnitTestsSubMenu = 8291;
		public const int MsTestToNunit = 8292;
		public const int NunitToMsTest = 8293;
		// 8 -----------------------------------------------------
		//public const int UrlSubMenuGroup = 8300;
		//public const int UrlsMenu = 8301;
		public const int UrlsEncode = 8302;
		public const int UrlsDecode = 8303;
		// 9 -----------------------------------------------------
		//public const int JavascriptSubMenuGroup = 8310;
		//public const int JavascriptSubMenu = 8311;
		public const int JavascriptFormat = 8312;
		public const int JavascriptCompact = 8313;
		// 10 ----------------------------------------------------
		public const int OptimizeUsings = 8320;
		public const int ShowProjectReferences = 8330;
		public const int ShowSolutionBuild = 8340;
		public const int TogglePublish = 8341;
		// 11 ----------------------------------------------------
		public const int GenerateTranslationReport = 8350;
		// 12 ----------------------------------------------------
		public const int PasteAsComments = 8360;
		public const int LineupDeclarations = 8370;
		public const int SortSelectedLines = 8380;
		// 13 ----------------------------------------------------
		public const int RegexFindAndReplace = 8390;
		// 14 ----------------------------------------------------
		//public const int ExternalToolsSubMenuGroup = 8400;
		//public const int ExternalToolsSubMenu = 8401;
		public const int ExternalTools1 = 8402;
		public const int ExternalTools2 = 8403;
		// 15 ----------------------------------------------------
		public const int Options = 8500;
		// 16 ----------------------------------------------------
		public const int About = 8510;
		// 17 ----------------------------------------------------
		public const int MyGraph = 8520;
		public const int MyZoom = 8530;
	}

	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Contains the Dynamic Package Ids.
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	internal static class DynPkgIds
	{
		public const int DynamicTxt = 8784;
		public const int DynVisibility1 = 8785;
		public const int DynVisibility2 = 8786;
	}
}
