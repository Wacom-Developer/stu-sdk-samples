/******************************************************* 

  DemoButtonsForm.cs
  
  Controlling function for C# Demobuttons program allowing
  user to capture a signature with 3 control buttons
  
  Copyright (c) 2023 Wacom Ltd. All rights reserved.
  
********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

enum PenDataOptionMode
{
  PenDataOptionMode_None,
  PenDataOptionMode_TimeCount,
  PenDataOptionMode_SequenceNumber,
  PenDataOptionMode_TimeCountSequence
};

namespace DemoButtons
{
    public partial class DemoButtonsForm : Form
    {
        public DemoButtonsForm()
        {
            string STU_ModelName;

            InitializeComponent();
            STU_ModelName = TabletLib.STU_Tablet.FindSTUModelName();
            txtSTUModel.Text = STU_ModelName;

            if (STU_ModelName == "STU-500" || STU_ModelName == "STU-300")
              chkCalcSpeed.Enabled = false;

            if (STU_ModelName == "STU-300")
              chkReverseImage.Enabled = false;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {

            wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
            if (usbDevices.Count != 0)
            {
                try
                {
                    wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                    SignatureForm demo = new SignatureForm(this, usbDevice, chkSaveImage.Checked, chkCalcSpeed.Checked, chkReverseImage.Checked);
                    DialogResult res = demo.ShowDialog();
                    print("SignatureForm returned: " + res.ToString());
                    if (res == DialogResult.OK)
                    {
                        txtSTUModel.Text = demo.stu_Tablet.information.modelName;

                        if (demo.stu_Tablet.information.modelName == "STU-500" || demo.stu_Tablet.information.modelName == "STU-300")
                          chkCalcSpeed.Enabled = false;

                        if (demo.stu_Tablet.information.modelName == "STU-300")
                          chkReverseImage.Enabled = false;

                        DisplaySignature(demo);
                    }
                    demo.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                print("No STU devices attached");
            }
        }

        public void print(string txt)
        {
            txtDisplay.Text += txt + "\r\n";
            txtDisplay.SelectionStart = txtDisplay.Text.Length; // scroll to end
            txtDisplay.ScrollToCaret();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void DisplaySignature(SignatureForm demo )
        {
          Bitmap bitmap;

          bitmap = GraphicsLib.GraphicFunctions.GetSigImage(this, demo, demo.stu_Tablet);
          // resize the image to fit the screen
          int scale = 2;       // halve or quarter the image size
          if( bitmap.Width > 400 )
              scale = 4;
          pictureBox1.Size = new Size(bitmap.Width / scale, bitmap.Height / scale);
          pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
          pictureBox1.Image = bitmap;
          pictureBox1.Parent = this;
          //centre the image in the panel
          int x, y;
          x = panel1.Location.X + ((panel1.Width - pictureBox1.Width) / 2);
          y = panel1.Location.Y + ((panel1.Height - pictureBox1.Height) / 2);
          this.pictureBox1.Location = new Point(x, y);
          pictureBox1.BringToFront();
        }
    }
    
}

