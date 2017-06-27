namespace GenericBrowser
{
	partial class BrowserWindow
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
			if (disposing && (components != null))
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
			this.goButton = new System.Windows.Forms.Button();
			this.inputBox = new System.Windows.Forms.TextBox();
			this.renderBox = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// goButton
			// 
			this.goButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.goButton.BackColor = System.Drawing.Color.LimeGreen;
			this.goButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.goButton.Location = new System.Drawing.Point(740, 5);
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(35, 23);
			this.goButton.TabIndex = 1;
			this.goButton.Text = "Go";
			this.goButton.UseVisualStyleBackColor = false;
			this.goButton.Click += new System.EventHandler(this.goButton_Click);
			// 
			// inputBox
			// 
			this.inputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.inputBox.Location = new System.Drawing.Point(10, 5);
			this.inputBox.Name = "inputBox";
			this.inputBox.Size = new System.Drawing.Size(725, 20);
			this.inputBox.TabIndex = 2;
			this.inputBox.Text = "http://";
			// 
			// renderBox
			// 
			this.renderBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.renderBox.AutoSize = true;
			this.renderBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.renderBox.Location = new System.Drawing.Point(10, 34);
			this.renderBox.Name = "renderBox";
			this.renderBox.Padding = new System.Windows.Forms.Padding(2);
			this.renderBox.Size = new System.Drawing.Size(765, 525);
			this.renderBox.TabIndex = 3;
			this.renderBox.TabStop = false;
			this.renderBox.Enter += new System.EventHandler(this.renderBox_Enter);
			// 
			// BrowserWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.renderBox);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.goButton);
			this.MaximizeBox = false;
			this.Name = "BrowserWindow";
			this.Text = "Web Browser";
			this.Load += new System.EventHandler(this.BrowserWindow_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button goButton;
		private System.Windows.Forms.TextBox inputBox;
		private System.Windows.Forms.GroupBox renderBox;
	}
}

