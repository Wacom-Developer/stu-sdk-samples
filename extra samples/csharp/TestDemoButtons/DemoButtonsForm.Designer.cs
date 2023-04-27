namespace DemoButtons
{
  partial class DemoButtonsForm
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
			this.btnSign = new System.Windows.Forms.Button();
			this.txtDisplay = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.chkCalcSpeed = new System.Windows.Forms.CheckBox();
			this.chkReverseImage = new System.Windows.Forms.CheckBox();
			this.chkSaveImage = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtSTUModel = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSign
			// 
			this.btnSign.Location = new System.Drawing.Point(410, 274);
			this.btnSign.Margin = new System.Windows.Forms.Padding(5);
			this.btnSign.Name = "btnSign";
			this.btnSign.Size = new System.Drawing.Size(131, 52);
			this.btnSign.TabIndex = 0;
			this.btnSign.Text = "Sign";
			this.btnSign.UseVisualStyleBackColor = true;
			this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
			// 
			// txtDisplay
			// 
			this.txtDisplay.Location = new System.Drawing.Point(14, 357);
			this.txtDisplay.Margin = new System.Windows.Forms.Padding(5);
			this.txtDisplay.Multiline = true;
			this.txtDisplay.Name = "txtDisplay";
			this.txtDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDisplay.Size = new System.Drawing.Size(746, 366);
			this.txtDisplay.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Location = new System.Drawing.Point(21, 21);
			this.panel1.Margin = new System.Windows.Forms.Padding(5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(370, 305);
			this.panel1.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(142, 77);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(102, 98);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(629, 274);
			this.btnClose.Margin = new System.Windows.Forms.Padding(5);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(131, 52);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// chkCalcSpeed
			// 
			this.chkCalcSpeed.AutoSize = true;
			this.chkCalcSpeed.Location = new System.Drawing.Point(422, 94);
			this.chkCalcSpeed.Name = "chkCalcSpeed";
			this.chkCalcSpeed.Size = new System.Drawing.Size(217, 29);
			this.chkCalcSpeed.TabIndex = 4;
			this.chkCalcSpeed.Text = "Calculate pen speed";
			this.chkCalcSpeed.UseVisualStyleBackColor = true;
			// 
			// chkReverseImage
			// 
			this.chkReverseImage.AutoSize = true;
			this.chkReverseImage.Location = new System.Drawing.Point(422, 146);
			this.chkReverseImage.Name = "chkReverseImage";
			this.chkReverseImage.Size = new System.Drawing.Size(168, 29);
			this.chkReverseImage.TabIndex = 5;
			this.chkReverseImage.Text = "Reverse image";
			this.chkReverseImage.UseVisualStyleBackColor = true;
			// 
			// chkSaveImage
			// 
			this.chkSaveImage.AutoSize = true;
			this.chkSaveImage.Location = new System.Drawing.Point(422, 205);
			this.chkSaveImage.Name = "chkSaveImage";
			this.chkSaveImage.Size = new System.Drawing.Size(192, 29);
			this.chkSaveImage.TabIndex = 6;
			this.chkSaveImage.Text = "Save image to file";
			this.chkSaveImage.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(416, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 25);
			this.label1.TabIndex = 7;
			this.label1.Text = "Signature pad:";
			// 
			// txtSTUModel
			// 
			this.txtSTUModel.BackColor = System.Drawing.SystemColors.Control;
			this.txtSTUModel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtSTUModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.857143F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSTUModel.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.txtSTUModel.Location = new System.Drawing.Point(568, 35);
			this.txtSTUModel.Name = "txtSTUModel";
			this.txtSTUModel.ReadOnly = true;
			this.txtSTUModel.Size = new System.Drawing.Size(121, 27);
			this.txtSTUModel.TabIndex = 8;
			// 
			// DemoButtonsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(824, 750);
			this.Controls.Add(this.txtSTUModel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chkSaveImage);
			this.Controls.Add(this.chkReverseImage);
			this.Controls.Add(this.chkCalcSpeed);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.txtDisplay);
			this.Controls.Add(this.btnSign);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "DemoButtonsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DemoButtons (C#)";
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSign;
    private System.Windows.Forms.TextBox txtDisplay;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.CheckBox chkCalcSpeed;
		private System.Windows.Forms.CheckBox chkReverseImage;
		private System.Windows.Forms.CheckBox chkSaveImage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSTUModel;
	}
}

