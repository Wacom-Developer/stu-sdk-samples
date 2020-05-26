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
         this.txtPenDataCount = new System.Windows.Forms.TextBox();
         this.lblPenDataCount = new System.Windows.Forms.Label();
         this.chkUseEncryption = new System.Windows.Forms.CheckBox();
         this.SuspendLayout();
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(92, 29);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(75, 23);
         this.button1.TabIndex = 0;
         this.button1.Text = "Demo";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // txtPenDataCount
         // 
         this.txtPenDataCount.Location = new System.Drawing.Point(102, 73);
         this.txtPenDataCount.Name = "txtPenDataCount";
         this.txtPenDataCount.ReadOnly = true;
         this.txtPenDataCount.Size = new System.Drawing.Size(51, 20);
         this.txtPenDataCount.TabIndex = 1;
         // 
         // lblPenDataCount
         // 
         this.lblPenDataCount.AutoSize = true;
         this.lblPenDataCount.Location = new System.Drawing.Point(12, 76);
         this.lblPenDataCount.Name = "lblPenDataCount";
         this.lblPenDataCount.Size = new System.Drawing.Size(83, 13);
         this.lblPenDataCount.TabIndex = 2;
         this.lblPenDataCount.Text = "Pen data count:";
         // 
         // chkUseEncryption
         // 
         this.chkUseEncryption.AutoSize = true;
         this.chkUseEncryption.Location = new System.Drawing.Point(79, 6);
         this.chkUseEncryption.Name = "chkUseEncryption";
         this.chkUseEncryption.Size = new System.Drawing.Size(103, 17);
         this.chkUseEncryption.TabIndex = 3;
         this.chkUseEncryption.Text = "Use encryption?";
         this.chkUseEncryption.UseVisualStyleBackColor = true;
         // 
         // DemoButtonsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
         this.ClientSize = new System.Drawing.Size(284, 98);
         this.Controls.Add(this.chkUseEncryption);
         this.Controls.Add(this.lblPenDataCount);
         this.Controls.Add(this.txtPenDataCount);
         this.Controls.Add(this.button1);
         this.Name = "DemoButtonsForm";
         this.Text = "DemoButtons (C#)";
         this.ResumeLayout(false);
         this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
      private System.Windows.Forms.TextBox txtPenDataCount;
      private System.Windows.Forms.Label lblPenDataCount;
      private System.Windows.Forms.CheckBox chkUseEncryption;
   }
}

