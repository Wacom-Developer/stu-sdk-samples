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
            this.button1 = new System.Windows.Forms.Button();
            this.radHID = new System.Windows.Forms.RadioButton();
            this.radSerial = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCOM = new System.Windows.Forms.TextBox();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 209);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 39);
            this.button1.TabIndex = 0;
            this.button1.Text = "Sign";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radHID
            // 
            this.radHID.AutoSize = true;
            this.radHID.Location = new System.Drawing.Point(32, 26);
            this.radHID.Name = "radHID";
            this.radHID.Size = new System.Drawing.Size(44, 17);
            this.radHID.TabIndex = 1;
            this.radHID.TabStop = true;
            this.radHID.Text = "HID";
            this.radHID.UseVisualStyleBackColor = true;
            this.radHID.CheckedChanged += new System.EventHandler(this.radHID_CheckedChanged);
            // 
            // radSerial
            // 
            this.radSerial.AutoSize = true;
            this.radSerial.Location = new System.Drawing.Point(30, 88);
            this.radSerial.Name = "radSerial";
            this.radSerial.Size = new System.Drawing.Size(51, 17);
            this.radSerial.TabIndex = 2;
            this.radSerial.TabStop = true;
            this.radSerial.Text = "Serial";
            this.radSerial.UseVisualStyleBackColor = true;
            this.radSerial.CheckedChanged += new System.EventHandler(this.radSerial_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(308, 209);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 39);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "COM number:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Baud rate:";
            // 
            // txtCOM
            // 
            this.txtCOM.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCOM.Enabled = false;
            this.txtCOM.Location = new System.Drawing.Point(105, 19);
            this.txtCOM.Name = "txtCOM";
            this.txtCOM.Size = new System.Drawing.Size(72, 20);
            this.txtCOM.TabIndex = 6;
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Enabled = false;
            this.txtBaudRate.Location = new System.Drawing.Point(104, 56);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.Size = new System.Drawing.Size(72, 20);
            this.txtBaudRate.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBaudRate);
            this.groupBox1.Controls.Add(this.txtCOM);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(133, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 98);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // txtDisplay
            // 
            this.txtDisplay.Location = new System.Drawing.Point(452, 25);
            this.txtDisplay.Multiline = true;
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.Size = new System.Drawing.Size(416, 601);
            this.txtDisplay.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(30, 311);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 315);
            this.panel1.TabIndex = 10;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(376, 283);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // DemoButtonsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(880, 638);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.radSerial);
            this.Controls.Add(this.radHID);
            this.Controls.Add(this.button1);
            this.Name = "DemoButtonsForm";
            this.Text = "DemoButtons (C#)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.RadioButton radHID;
    private System.Windows.Forms.RadioButton radSerial;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtCOM;
    private System.Windows.Forms.TextBox txtBaudRate;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox txtDisplay;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}

