namespace RPIconn
	{
	partial class TextBoxOutput
		{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
			{
			if(disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
			{
			this.tbInnerText = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbInnerText
			// 
			this.tbInnerText.Location = new System.Drawing.Point(97, 51);
			this.tbInnerText.Multiline = true;
			this.tbInnerText.Name = "tbInnerText";
			this.tbInnerText.Size = new System.Drawing.Size(250, 115);
			this.tbInnerText.TabIndex = 0;
			// 
			// TextBoxOutput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(738, 464);
			this.Controls.Add(this.tbInnerText);
			this.Name = "TextBoxOutput";
			this.Text = "TextBoxOutput";
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.TextBox tbInnerText;
		}
	}