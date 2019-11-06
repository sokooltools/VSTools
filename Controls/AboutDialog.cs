using System.Windows.Forms;

namespace SokoolTools.VsTools
{
	internal partial class AboutDialog : Form
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize the AboutBox to display the product information from the assembly information.
		/// <para>
		/// Change assembly information settings for your application through either:
		/// - Project -> Properties -> Application -> Assembly Information
		///		or
		/// - AssemblyInfo.cs
		/// </para>
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public AboutDialog()
		{
			InitializeComponent();

			var assm = new AssemblyInfo();

			base.Text = string.Format(Resources.About_Text, assm.Title);
			lblProductName.Text = assm.Product;
			lblVersion.Text = string.Format(Resources.About_Version, assm.Version);
			lblModDate.Text = string.Format(Resources.About_ModDate, assm.Modified);
			lblCopyright.Text = assm.Copyright;
			lblCompanyName.Text = assm.Company;
			txtFilePath.Text = assm.FilePath;
			txtDescription.Text = assm.Description;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The text associated with this control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public sealed override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}
	}
}