/*
 DemoButtonsForm.cs

 Controlling program for the DemoButtons program which allow user to input a signature on an STU
 and reproduces it on a Window on the PC

 Copyright (c) 2015 Wacom GmbH. All rights reserved.

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DemoButtons
{
    public partial class DemoButtonsForm : Form
    {
        public void print(string txt)
        {
            txtDisplay.Text += txt + "\r\n";
            txtDisplay.SelectionStart = txtDisplay.Text.Length; // scroll to end
            txtDisplay.ScrollToCaret();

        }

        public DemoButtonsForm()
        {
            InitializeComponent();
            radHID.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (radHID.Checked)
            {
                captureSignatureHID();
            }
            else
            {
                captureSignatureSerial();
            }

        }

        private void captureSignatureHID()
        {
            wgssSTU.SerialInterface serialInterface = new wgssSTU.SerialInterface();
            wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();

            if (usbDevices.Count != 0)
            {
                try
                {
                    wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                    SignatureForm demo = new SignatureForm(this, usbDevice, serialInterface, txtCOM.Text, txtBaudRate.Text, true);
                    DialogResult res = demo.ShowDialog();
                    //print("SignatureForm returned: " + res.ToString());
                    if (res == DialogResult.OK)
                    {
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
                MessageBox.Show("No STU devices attached");
            }
        }

        private void captureSignatureSerial()
        {
            int baudRate;
            string fileNameCOMPort;
            wgssSTU.IUsbDevice usbDevice;
            wgssSTU.SerialInterface serialInterface;

            usbDevice = null;
            serialInterface = new wgssSTU.SerialInterface();

            fileNameCOMPort = txtCOM.Text;
            baudRate = int.Parse(txtBaudRate.Text);

            try
            {
                SignatureForm demo = new SignatureForm(this, usbDevice, serialInterface, txtCOM.Text, txtBaudRate.Text, false);
                DialogResult res = demo.ShowDialog();
                print("SignatureForm returned: " + res.ToString());
                if (res == DialogResult.OK)
                {
                    DisplaySignature(demo);
                }
                demo.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No STU devices attached");
            }
        }

        private void radHID_CheckedChanged(object sender, EventArgs e)
        {
            radioSelectionCheck();
        }

        private void radSerial_CheckedChanged(object sender, EventArgs e)
        {
            radioSelectionCheck();
        }

        private void radioSelectionCheck()
        {
            if (radHID.Checked == true)
            {
                txtCOM.Enabled = false;
                txtBaudRate.Enabled = false;
            }
            else
            {
                txtCOM.Enabled = true;
                txtBaudRate.Enabled = true;
                txtBaudRate.Text = "128000";
                txtCOM.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DisplaySignature(SignatureForm demo)
        {
            Bitmap bitmap;

            bitmap = demo.GetSigImage();
            // resize the image to fit the screen if needed
            int scale = 1;       
            if (bitmap.Width > 400)
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
            //bitmap.Save("C:\\temp\\sig.png", System.Drawing.Imaging.ImageFormat.Png); // to save the image to disk
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
