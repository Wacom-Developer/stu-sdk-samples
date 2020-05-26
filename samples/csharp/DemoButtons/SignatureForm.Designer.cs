namespace DemoButtons
{
  partial class SignatureForm
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
      if (disposing && (m_tablet != null))
      {
        m_tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
        m_tablet.disconnect();
        m_tablet = null;
      }
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
      this.SuspendLayout();
      // 
      // SignatureForm
      // 
      this.ClientSize = new System.Drawing.Size(284, 261);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SignatureForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Signature";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form2_Paint);
      this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseClick);
      this.ResumeLayout(false);

    }

    #endregion

  }
}