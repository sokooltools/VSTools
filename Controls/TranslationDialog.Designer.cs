namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	partial class TranslationDialog
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private System.ComponentModel.IContainer components = null;

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		//----------------------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		//.............................................................................................................

		#region Windows Form Designer generated code

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private void InitializeComponent()
		{
			this.lblCaption = new System.Windows.Forms.Label();
			this.ChkOutputUntranslatedOnly = new System.Windows.Forms.CheckBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblCaption
			// 
			this.lblCaption.AutoSize = true;
			this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCaption.Location = new System.Drawing.Point(12, 18);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(314, 17);
			this.lblCaption.TabIndex = 0;
			this.lblCaption.Text = "Ok to generate a translation report for this solution?";
			// 
			// ChkOutputUntranslatedOnly
			// 
			this.ChkOutputUntranslatedOnly.AutoSize = true;
			this.ChkOutputUntranslatedOnly.Location = new System.Drawing.Point(15, 61);
			this.ChkOutputUntranslatedOnly.Name = "ChkOutputUntranslatedOnly";
			this.ChkOutputUntranslatedOnly.Size = new System.Drawing.Size(161, 17);
			this.ChkOutputUntranslatedOnly.TabIndex = 3;
			this.ChkOutputUntranslatedOnly.Text = "Output Untranslated Only";
			this.ChkOutputUntranslatedOnly.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(208, 57);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(299, 57);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// TranslationDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(397, 96);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.ChkOutputUntranslatedOnly);
			this.Controls.Add(this.lblCaption);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TranslationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generate Translation Report";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.CheckBox ChkOutputUntranslatedOnly;
		private System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}