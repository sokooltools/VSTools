using System.Windows.Forms;
using System.ComponentModel;

namespace SokoolTools.VsTools.FindAndReplace
{
	public partial class SearchForm
	{

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		//.............................................................................................................

		#region Windows Form Designer generated code

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
			this.lblSearchPattern = new System.Windows.Forms.Label();
			this.cboSearchPattern = new System.Windows.Forms.ComboBox();
			this.btnFindNext = new System.Windows.Forms.Button();
			this.chkSingleLine = new System.Windows.Forms.CheckBox();
			this.chkMultipleLine = new System.Windows.Forms.CheckBox();
			this.chkECMAScript = new System.Windows.Forms.CheckBox();
			this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
			this.lblSearchScope = new System.Windows.Forms.Label();
			this.cboSearchScope = new System.Windows.Forms.ComboBox();
			this.chkIncludeSubFolders = new System.Windows.Forms.CheckBox();
			this.cboFileTypes = new System.Windows.Forms.ComboBox();
			this.lblFileTypes = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.lblContextLines = new System.Windows.Forms.Label();
			this.btnReplace = new System.Windows.Forms.Button();
			this.cboReplacePattern = new System.Windows.Forms.ComboBox();
			this.lblReplacePattern = new System.Windows.Forms.Label();
			this.btnReplaceAll = new System.Windows.Forms.Button();
			this.txtSearchPattern = new System.Windows.Forms.TextBox();
			this.txtReplacePattern = new System.Windows.Forms.TextBox();
			this.btnReplaceWithImage = new System.Windows.Forms.Button();
			this.updContextLines = new System.Windows.Forms.NumericUpDown();
			this.btnFileTypesUnlock = new System.Windows.Forms.Button();
			this.lnkRegexHelp = new System.Windows.Forms.LinkLabel();
			this.lnkReset = new System.Windows.Forms.LinkLabel();
			this.btnFindAll = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.updContextLines)).BeginInit();
			this.SuspendLayout();
			// 
			// lblSearchPattern
			// 
			this.lblSearchPattern.Location = new System.Drawing.Point(14, 9);
			this.lblSearchPattern.Name = "lblSearchPattern";
			this.lblSearchPattern.Size = new System.Drawing.Size(73, 15);
			this.lblSearchPattern.TabIndex = 0;
			this.lblSearchPattern.Text = "Find what:";
			// 
			// cboSearchPattern
			// 
			this.cboSearchPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboSearchPattern.Location = new System.Drawing.Point(17, 27);
			this.cboSearchPattern.Name = "cboSearchPattern";
			this.cboSearchPattern.Size = new System.Drawing.Size(383, 21);
			this.cboSearchPattern.TabIndex = 2;
			// 
			// btnFindNext
			// 
			this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindNext.Location = new System.Drawing.Point(324, 210);
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Size = new System.Drawing.Size(84, 22);
			this.btnFindNext.TabIndex = 17;
			this.btnFindNext.Text = "Find &Next";
			// 
			// chkSingleLine
			// 
			this.chkSingleLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkSingleLine.Location = new System.Drawing.Point(17, 166);
			this.chkSingleLine.Name = "chkSingleLine";
			this.chkSingleLine.Size = new System.Drawing.Size(86, 22);
			this.chkSingleLine.TabIndex = 4;
			this.chkSingleLine.Text = "SingleLine";
			// 
			// chkMultipleLine
			// 
			this.chkMultipleLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkMultipleLine.Location = new System.Drawing.Point(103, 166);
			this.chkMultipleLine.Name = "chkMultipleLine";
			this.chkMultipleLine.Size = new System.Drawing.Size(95, 22);
			this.chkMultipleLine.TabIndex = 5;
			this.chkMultipleLine.Text = "MultipleLine";
			// 
			// chkECMAScript
			// 
			this.chkECMAScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkECMAScript.Location = new System.Drawing.Point(290, 166);
			this.chkECMAScript.Name = "chkECMAScript";
			this.chkECMAScript.Size = new System.Drawing.Size(93, 22);
			this.chkECMAScript.TabIndex = 7;
			this.chkECMAScript.Text = "ECMAScript";
			// 
			// chkIgnoreCase
			// 
			this.chkIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkIgnoreCase.Location = new System.Drawing.Point(199, 166);
			this.chkIgnoreCase.Name = "chkIgnoreCase";
			this.chkIgnoreCase.Size = new System.Drawing.Size(86, 22);
			this.chkIgnoreCase.TabIndex = 6;
			this.chkIgnoreCase.Text = "IgnoreCase";
			// 
			// lblSearchScope
			// 
			this.lblSearchScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSearchScope.Location = new System.Drawing.Point(14, 192);
			this.lblSearchScope.Name = "lblSearchScope";
			this.lblSearchScope.Size = new System.Drawing.Size(73, 18);
			this.lblSearchScope.TabIndex = 8;
			this.lblSearchScope.Text = "Look in:";
			// 
			// cboSearchScope
			// 
			this.cboSearchScope.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboSearchScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSearchScope.Location = new System.Drawing.Point(17, 211);
			this.cboSearchScope.Name = "cboSearchScope";
			this.cboSearchScope.Size = new System.Drawing.Size(262, 21);
			this.cboSearchScope.TabIndex = 9;
			// 
			// chkIncludeSubFolders
			// 
			this.chkIncludeSubFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkIncludeSubFolders.Enabled = false;
			this.chkIncludeSubFolders.Location = new System.Drawing.Point(16, 239);
			this.chkIncludeSubFolders.Name = "chkIncludeSubFolders";
			this.chkIncludeSubFolders.Size = new System.Drawing.Size(128, 17);
			this.chkIncludeSubFolders.TabIndex = 11;
			this.chkIncludeSubFolders.Text = "Include sub-folders";
			// 
			// cboFileTypes
			// 
			this.cboFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboFileTypes.Enabled = false;
			this.cboFileTypes.Items.AddRange(new object[] {
            "*.cs;*.resx;*.xsd;*.wsdl;*.htm;*.html;*.css;*.xml;*.xaml",
            "*.vb;*.resx;*.xsd;*.wsdl;*.htm;*.html;*.css;*.xml;*.xaml",
            "*.vb;*.resx;*.xsd;*.wsdl;*.htm;*.html;*.css;*.xml;*.aspx;*.ascx;*.asmx;*.svc;*.as" +
                "ax;*.config;*.asp;*.asa",
            "*.vbs; *.js;*.xml;*.act;*.actproj",
            "*.c;*.cpp;*.cxx;*.cc;*.tli;*.tlh;*.h;*.hpp;*.hxx;*.hh;*.inl;*.rc;*.resx;*.idl;*.a" +
                "sm;*.inc",
            "*.srf;*.htm;*.html;*.xml;*.gif;*.jpg;*.png;*.css;*.disco",
            "*.txt;*.htm;*.html",
            "*.ora",
            "*.rpt",
            "*.sql",
            "*.txt",
            "*.*"});
			this.cboFileTypes.Location = new System.Drawing.Point(17, 285);
			this.cboFileTypes.Name = "cboFileTypes";
			this.cboFileTypes.Size = new System.Drawing.Size(262, 21);
			this.cboFileTypes.TabIndex = 13;
			// 
			// lblFileTypes
			// 
			this.lblFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblFileTypes.Location = new System.Drawing.Point(14, 268);
			this.lblFileTypes.Name = "lblFileTypes";
			this.lblFileTypes.Size = new System.Drawing.Size(228, 16);
			this.lblFileTypes.TabIndex = 12;
			this.lblFileTypes.Text = "Look at these file types:";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(280, 210);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnBrowse.TabIndex = 10;
			this.btnBrowse.Text = "&...";
			// 
			// lblContextLines
			// 
			this.lblContextLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblContextLines.Enabled = false;
			this.lblContextLines.Location = new System.Drawing.Point(13, 318);
			this.lblContextLines.Name = "lblContextLines";
			this.lblContextLines.Size = new System.Drawing.Size(159, 16);
			this.lblContextLines.TabIndex = 15;
			this.lblContextLines.Text = "Show this many context lines:";
			// 
			// btnReplace
			// 
			this.btnReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReplace.Location = new System.Drawing.Point(324, 284);
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Size = new System.Drawing.Size(84, 21);
			this.btnReplace.TabIndex = 20;
			this.btnReplace.Text = "&Replace";
			this.btnReplace.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cboReplacePattern
			// 
			this.cboReplacePattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboReplacePattern.Location = new System.Drawing.Point(16, 104);
			this.cboReplacePattern.MaxDropDownItems = 10;
			this.cboReplacePattern.Name = "cboReplacePattern";
			this.cboReplacePattern.Size = new System.Drawing.Size(383, 21);
			this.cboReplacePattern.TabIndex = 5;
			// 
			// lblReplacePattern
			// 
			this.lblReplacePattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblReplacePattern.Location = new System.Drawing.Point(17, 86);
			this.lblReplacePattern.Name = "lblReplacePattern";
			this.lblReplacePattern.Size = new System.Drawing.Size(130, 15);
			this.lblReplacePattern.TabIndex = 2;
			this.lblReplacePattern.Text = "Replace with:";
			// 
			// btnReplaceAll
			// 
			this.btnReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReplaceAll.Location = new System.Drawing.Point(324, 311);
			this.btnReplaceAll.Name = "btnReplaceAll";
			this.btnReplaceAll.Size = new System.Drawing.Size(84, 22);
			this.btnReplaceAll.TabIndex = 20;
			this.btnReplaceAll.Text = "Replace &All";
			// 
			// txtSearchPattern
			// 
			this.txtSearchPattern.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSearchPattern.Location = new System.Drawing.Point(17, 27);
			this.txtSearchPattern.Multiline = true;
			this.txtSearchPattern.Name = "txtSearchPattern";
			this.txtSearchPattern.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSearchPattern.Size = new System.Drawing.Size(364, 53);
			this.txtSearchPattern.TabIndex = 1;
			// 
			// txtReplacePattern
			// 
			this.txtReplacePattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtReplacePattern.Location = new System.Drawing.Point(16, 104);
			this.txtReplacePattern.Multiline = true;
			this.txtReplacePattern.Name = "txtReplacePattern";
			this.txtReplacePattern.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtReplacePattern.Size = new System.Drawing.Size(365, 53);
			this.txtReplacePattern.TabIndex = 3;
			// 
			// btnReplaceWithImage
			// 
			this.btnReplaceWithImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReplaceWithImage.Image = ((System.Drawing.Image)(resources.GetObject("btnReplaceWithImage.Image")));
			this.btnReplaceWithImage.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnReplaceWithImage.Location = new System.Drawing.Point(324, 284);
			this.btnReplaceWithImage.Name = "btnReplaceWithImage";
			this.btnReplaceWithImage.Size = new System.Drawing.Size(84, 22);
			this.btnReplaceWithImage.TabIndex = 19;
			this.btnReplaceWithImage.Text = " &Replace";
			// 
			// updContextLines
			// 
			this.updContextLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.updContextLines.Enabled = false;
			this.updContextLines.Location = new System.Drawing.Point(175, 316);
			this.updContextLines.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.updContextLines.Name = "updContextLines";
			this.updContextLines.Size = new System.Drawing.Size(37, 22);
			this.updContextLines.TabIndex = 16;
			this.updContextLines.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// btnFileTypesUnlock
			// 
			this.btnFileTypesUnlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFileTypesUnlock.Enabled = false;
			this.btnFileTypesUnlock.Location = new System.Drawing.Point(280, 285);
			this.btnFileTypesUnlock.Name = "btnFileTypesUnlock";
			this.btnFileTypesUnlock.Size = new System.Drawing.Size(24, 22);
			this.btnFileTypesUnlock.TabIndex = 14;
			this.btnFileTypesUnlock.Text = "&...";
			this.btnFileTypesUnlock.Visible = false;
			// 
			// lnkRegexHelp
			// 
			this.lnkRegexHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkRegexHelp.AutoSize = true;
			this.lnkRegexHelp.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.lnkRegexHelp.Location = new System.Drawing.Point(334, 9);
			this.lnkRegexHelp.Name = "lnkRegexHelp";
			this.lnkRegexHelp.Size = new System.Drawing.Size(74, 13);
			this.lnkRegexHelp.TabIndex = 22;
			this.lnkRegexHelp.TabStop = true;
			this.lnkRegexHelp.Text = "Regex Help...";
			// 
			// lnkReset
			// 
			this.lnkReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkReset.AutoSize = true;
			this.lnkReset.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.lnkReset.Location = new System.Drawing.Point(241, 9);
			this.lnkReset.Name = "lnkReset";
			this.lnkReset.Size = new System.Drawing.Size(44, 13);
			this.lnkReset.TabIndex = 21;
			this.lnkReset.TabStop = true;
			this.lnkReset.Text = "Reset...";
			// 
			// btnFindAll
			// 
			this.btnFindAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindAll.Location = new System.Drawing.Point(324, 238);
			this.btnFindAll.Name = "btnFindAll";
			this.btnFindAll.Size = new System.Drawing.Size(84, 22);
			this.btnFindAll.TabIndex = 18;
			this.btnFindAll.Text = "&Find All";
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Location = new System.Drawing.Point(1000, 1000);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(10, 12);
			this.btnClose.TabIndex = 0;
			this.btnClose.TabStop = false;
			this.btnClose.Text = "Close";
			this.btnClose.Visible = false;
			// 
			// SearchForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(420, 351);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.lnkReset);
			this.Controls.Add(this.lnkRegexHelp);
			this.Controls.Add(this.txtReplacePattern);
			this.Controls.Add(this.cboReplacePattern);
			this.Controls.Add(this.lblReplacePattern);
			this.Controls.Add(this.txtSearchPattern);
			this.Controls.Add(this.lblSearchPattern);
			this.Controls.Add(this.cboSearchPattern);
			this.Controls.Add(this.updContextLines);
			this.Controls.Add(this.btnReplaceWithImage);
			this.Controls.Add(this.btnReplaceAll);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.lblContextLines);
			this.Controls.Add(this.btnFileTypesUnlock);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.cboFileTypes);
			this.Controls.Add(this.lblFileTypes);
			this.Controls.Add(this.chkIncludeSubFolders);
			this.Controls.Add(this.cboSearchScope);
			this.Controls.Add(this.lblSearchScope);
			this.Controls.Add(this.chkECMAScript);
			this.Controls.Add(this.chkIgnoreCase);
			this.Controls.Add(this.chkMultipleLine);
			this.Controls.Add(this.chkSingleLine);
			this.Controls.Add(this.btnFindAll);
			this.Controls.Add(this.btnFindNext);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Regex Find and Replace";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.updContextLines)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Container components = null;

		private Button btnBrowse;
		private Button btnClose;
		private Button btnFileTypesUnlock;
		private Button btnFindAll;
		private Button btnFindNext;
		private Button btnReplace;
		private Button btnReplaceAll;
		private Button btnReplaceWithImage;
		private CheckBox chkECMAScript;
		private CheckBox chkIgnoreCase;
		private CheckBox chkIncludeSubFolders;
		private CheckBox chkMultipleLine;
		private CheckBox chkSingleLine;
		private ComboBox cboFileTypes;
		private ComboBox cboReplacePattern;
		private ComboBox cboSearchPattern;
		private ComboBox cboSearchScope;
		private Label lblContextLines;
		private Label lblFileTypes;
		private Label lblReplacePattern;
		private Label lblSearchPattern;
		private Label lblSearchScope;
		private LinkLabel lnkRegexHelp;
		private LinkLabel lnkReset;
		private NumericUpDown updContextLines;
		private TextBox txtReplacePattern;
		private TextBox txtSearchPattern;

	}
}
