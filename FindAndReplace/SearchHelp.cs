using System;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public partial class SearchHelp : Form
	{
		private PrintDocument printDocument1;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchHelp"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public SearchHelp()
		{
			InitializeComponent();
		}

		private void SearchHelp_Load(object sender, EventArgs e)
		{
			printDocument1 = new PrintDocument();
			printDocument1.PrintPage += printDocument1_PrintPage;
		}

		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			var myPrintDialog = new PrintDialog { Document = printDocument1 };
			if (myPrintDialog.ShowDialog(this) == DialogResult.OK)
				printDocument1.Print();
		}

		private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
		}
	}
}