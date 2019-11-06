using System.Windows.Forms;

namespace SokoolTools.VsTools
{
	internal partial class AboutDialog
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private System.ComponentModel.IContainer components = null;

		//------------------------------------------------------------------------------------------
		/// <summary>
		///    Clean up any resources being used.
		/// </summary>
		//------------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				components?.Dispose();
			base.Dispose(disposing);
		}


		#region Windows Form Designer generated code

		//------------------------------------------------------------------------------------------
		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		//------------------------------------------------------------------------------------------
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.tableLayoutPanel = new System.Windows.Forms.Panel();
			this.txtFilePath = new SokoolTools.VsTools.EllipsisTextBox();
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.lblProductName = new System.Windows.Forms.Label();
			this.lblModDate = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblCompanyName = new System.Windows.Forms.Label();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.Controls.Add(this.txtFilePath);
			this.tableLayoutPanel.Controls.Add(this.logoPictureBox);
			this.tableLayoutPanel.Controls.Add(this.lblProductName);
			this.tableLayoutPanel.Controls.Add(this.lblModDate);
			this.tableLayoutPanel.Controls.Add(this.lblVersion);
			this.tableLayoutPanel.Controls.Add(this.lblCopyright);
			this.tableLayoutPanel.Controls.Add(this.lblCompanyName);
			this.tableLayoutPanel.Controls.Add(this.txtDescription);
			this.tableLayoutPanel.Controls.Add(this.okButton);
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Size = new System.Drawing.Size(435, 327);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// txtFilePath
			// 
			this.txtFilePath.AllowDrop = false;
			this.txtFilePath.EllipsisType = SokoolTools.VsTools.EllipsisTextBox.EllipsisLocation.Path;
			this.txtFilePath.ExpansionLineCount = 1;
			this.txtFilePath.Location = new System.Drawing.Point(9, 153);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.ReadOnly = true;
			this.txtFilePath.Size = new System.Drawing.Size(415, 22);
			this.txtFilePath.TabIndex = 25;
			// 
			// logoPictureBox
			// 
			this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			this.logoPictureBox.Location = new System.Drawing.Point(8, 8);
			this.logoPictureBox.Name = "logoPictureBox";
			this.logoPictureBox.Size = new System.Drawing.Size(144, 138);
			this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.logoPictureBox.TabIndex = 12;
			this.logoPictureBox.TabStop = false;
			// 
			// lblProductName
			// 
			this.lblProductName.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProductName.Location = new System.Drawing.Point(158, 8);
			this.lblProductName.Name = "lblProductName";
			this.lblProductName.Size = new System.Drawing.Size(266, 16);
			this.lblProductName.TabIndex = 19;
			this.lblProductName.Text = "Product Name";
			this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblModDate
			// 
			this.lblModDate.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblModDate.Location = new System.Drawing.Point(158, 128);
			this.lblModDate.Name = "lblModDate";
			this.lblModDate.Size = new System.Drawing.Size(266, 16);
			this.lblModDate.TabIndex = 0;
			this.lblModDate.Text = "ModDate";
			this.lblModDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblVersion
			// 
			this.lblVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersion.Location = new System.Drawing.Point(158, 38);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(266, 16);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "Version";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCopyright
			// 
			this.lblCopyright.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCopyright.Location = new System.Drawing.Point(158, 69);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(266, 16);
			this.lblCopyright.TabIndex = 21;
			this.lblCopyright.Text = "Copyright";
			this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCompanyName
			// 
			this.lblCompanyName.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCompanyName.Location = new System.Drawing.Point(158, 100);
			this.lblCompanyName.Name = "lblCompanyName";
			this.lblCompanyName.Size = new System.Drawing.Size(266, 16);
			this.lblCompanyName.TabIndex = 22;
			this.lblCompanyName.Text = "Company Name";
			this.lblCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtDescription
			// 
			this.txtDescription.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDescription.Location = new System.Drawing.Point(8, 180);
			this.txtDescription.Multiline = true;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.ReadOnly = true;
			this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtDescription.Size = new System.Drawing.Size(416, 95);
			this.txtDescription.TabIndex = 23;
			this.txtDescription.TabStop = false;
			this.txtDescription.Text = "Description";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(349, 292);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 24;
			this.okButton.Text = "&OK";
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size(435, 327);
			this.Controls.Add(this.tableLayoutPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AboutBox1";
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Button okButton;
		private Label lblCompanyName;
		private Label lblCopyright;
		private Label lblModDate;
		private Label lblProductName;
		private Label lblVersion;
		private Panel tableLayoutPanel;
		private PictureBox logoPictureBox;
		private TextBox txtDescription;
		private EllipsisTextBox txtFilePath;
	}
}
