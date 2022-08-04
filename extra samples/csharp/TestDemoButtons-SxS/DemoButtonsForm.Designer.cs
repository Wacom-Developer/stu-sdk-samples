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
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.SuspendLayout();
        // 
        // btnSign
        // 
        this.btnSign.Location = new System.Drawing.Point(293, 42);
        this.btnSign.Name = "btnSign";
        this.btnSign.Size = new System.Drawing.Size(75, 30);
        this.btnSign.TabIndex = 0;
        this.btnSign.Text = "Sign";
        this.btnSign.UseVisualStyleBackColor = true;
        this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
        // 
        // txtDisplay
        // 
        this.txtDisplay.Location = new System.Drawing.Point(12, 182);
        this.txtDisplay.Multiline = true;
        this.txtDisplay.Name = "txtDisplay";
        this.txtDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtDisplay.Size = new System.Drawing.Size(428, 93);
        this.txtDisplay.TabIndex = 1;
        // 
        // panel1
        // 
        this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.panel1.Controls.Add(this.pictureBox1);
        this.panel1.Location = new System.Drawing.Point(12, 12);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(212, 152);
        this.panel1.TabIndex = 2;
        // 
        // pictureBox1
        // 
        this.pictureBox1.Location = new System.Drawing.Point(81, 44);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(58, 56);
        this.pictureBox1.TabIndex = 3;
        this.pictureBox1.TabStop = false;
        // 
        // btnClose
        // 
        this.btnClose.Location = new System.Drawing.Point(293, 96);
        this.btnClose.Name = "btnClose";
        this.btnClose.Size = new System.Drawing.Size(75, 30);
        this.btnClose.TabIndex = 3;
        this.btnClose.Text = "Close";
        this.btnClose.UseVisualStyleBackColor = true;
        this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
        // 
        // DemoButtonsForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(471, 287);
        this.Controls.Add(this.btnClose);
        this.Controls.Add(this.panel1);
        this.Controls.Add(this.txtDisplay);
        this.Controls.Add(this.btnSign);
        this.Name = "DemoButtonsForm";
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
  }
}

