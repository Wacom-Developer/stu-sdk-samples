/******************************************************* 

  Form1.cs
  
  Allow user to select an image and send it to the STU
  Compatible with STU-300, STU-430, STU-500, STU-530 and STU-540
  but not the obsolete STU-541

  Copyright (c) 2015 Wacom GmbH. All rights reserved.
  
********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SendToSTU
{
    public partial class Form1 : Form
    {
        private wgssSTU.Tablet tablet;
        private wgssSTU.ICapability capability;
        private wgssSTU.ICapability2 capability2;
        private wgssSTU.IInformation information;


        public Form1()
        {
            InitializeComponent();
            DisplayImage( Properties.Resources._640x480 );  // use resource to simplify and avoid locating a folder
            print("Ready");
        }

        private void btnSendToSTU_Click(object sender, EventArgs e)
        {
            print("Send image to STU display");
            SendToSTU();
        }
        private void print(string txt)
        {
            txtDisplay.Text += txt + "\r\n";
            txtDisplay.SelectionStart = txtDisplay.Text.Length; // scroll to end
            txtDisplay.ScrollToCaret();

        }
        private bool Connect()
        {
            bool connected = false;
            try
            {
                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                if (usbDevices.Count == 0)
                {
                    print("No devices found");
                }
                else
                {
                    connected = true;
                    tablet = new wgssSTU.Tablet();
                    wgssSTU.IErrorCode ec = tablet.usbConnect(usbDevices[0], true);
                    if (ec.value == 0)
                    {
                        capability = tablet.getCapability();
                        capability2 = tablet.getCapability_2();
                        information = tablet.getInformation();
                        print("Tablet: " + information.modelName);
                    }
                    else
                    {
                        throw new Exception(ec.message);
                    }
                }
            }
            catch (Exception ex)
            {
                print("Exception: " + ex.Message);
            }
            return connected;
        }
        private void Disconnect()
        {
            try
            {
                if (tablet != null)
                    tablet.disconnect();
            }
            catch (Exception ex)
            {
                print("Exception: " + ex.Message);
            }
        }
        private void SendToSTU() {
            try
            {
                if (!Connect())
                {
                    return;
                }
                print("Connected: " + information.modelName);
                wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();
                wgssSTU.encodingFlag encodingFlag = 0;
                wgssSTU.encodingMode encodingMode = 0;

                ushort idP = tablet.getProductId();

                encodingFlag = (wgssSTU.encodingFlag)capability2.encodingFlag;
                encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(idP, (byte)encodingFlag);

                print("Encoding flag: " + encodingFlag);

                if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_24bit) != 0)
                {
                    encodingMode = tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_24bit_Bulk : wgssSTU.encodingMode.EncodingMode_24bit;
                }
                else if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_16bit) != 0)
                {
                    encodingMode = tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_16bit_Bulk : wgssSTU.encodingMode.EncodingMode_16bit;
                }
                else
                {
                    // assumes 1bit is available
                    encodingMode = wgssSTU.encodingMode.EncodingMode_1bit;
                }
                print("encodingMode: " + encodingMode);

                byte[] m_bitmapData; // This is the flattened data of the bitmap that we send to the device.
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                pictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                m_bitmapData = (byte[])protocolHelper.resizeAndFlatten(
                    stream.ToArray(),
                    0, 0, 0, 0,
                    capability.screenWidth, capability.screenHeight,
                    (byte)encodingMode,
                    wgssSTU.Scale.Scale_Fit, 0, 0);
                tablet.writeImage((byte)encodingMode, m_bitmapData);	  // uses the colour mode flags in encodingMode



                Disconnect();
            }
            catch (Exception ex)
            {
                print("Exception: " + ex.Message);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Connect())
                {
                    return;
                }
                tablet.setClearScreen();
                print("Cleared " + information.modelName);
                Disconnect();
            }
            catch (Exception ex)
            {
                print("Exception: " + ex.Message);
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtFilename.Text = openFileDialog1.FileName;
                DisplayImage(Image.FromFile(openFileDialog1.FileName));
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void DisplayImage(Image img)
        {
        // resize the image to fit the panel
        // STU displays: 300:396x100 430:320x200 500:640x480 520:800x480 530:800x480
        // 300/420 scale by 2, else scale by 4. Also handle unexpected size.
        int scale = 1;       
        if (img.Width > 400)
            scale = 4;
        else if (img.Width > panel1.Width)
            scale = 2;

        pictureBox1.Size = new Size(img.Width / scale, img.Height / scale);
        // don't exceed the panel size:
        if (pictureBox1.Size.Width > panel1.Size.Width || pictureBox1.Size.Height > panel1.Size.Height)
                    pictureBox1.Size = panel1.Size;     

        //centre the image in the panel
        int x, y;
        x = panel1.Location.X + ((panel1.Width - pictureBox1.Width) / 2);
        y = panel1.Location.Y + ((panel1.Height - pictureBox1.Height) / 2);
        pictureBox1.Location = new Point(x, y);

        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.Image = img;
        pictureBox1.Parent = this;
        pictureBox1.BringToFront();
        }
              
    }
}
