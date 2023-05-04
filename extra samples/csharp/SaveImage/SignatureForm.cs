/*
 SignatureForm.cs

 Allows user to input a signature on an STU and reproduces it on a Window on the PC.
 The signature is then saved as an image in any of 4 formats (JPEG, PNG, BMP, TIFF) as chosen by the user
 by buttons on the pad.  
 Supports the STU-300, STU-430, STU-500, STU-530 but not the STU-541

 Copyright (c) 2023 Wacom Ltd. All rights reserved.

*/
// Notes:
// There are three coordinate spaces to deal with that are named:
//   tablet: the raw tablet coordinate
//   screen: the tablet LCD screen
//   client: the Form window client area

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DemoButtons
{
  public partial class SignatureForm : Form
  {
    public TabletLib.STU_Tablet stu_Tablet;
    public string msg;

    // In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
    // Using an array of these makes it easy to add or remove buttons as desired.
    private delegate void ButtonClick();

    private GraphicsLib.Buttons.Button[] m_btns;

    // As per the file comment, there are three coordinate systems to deal with.
    // To help understand, we have left the calculations in place rather than optimise them.

    private void clearScreen()
    {
      // note: There is no need to clear the tablet screen prior to writing an image.

      stu_Tablet.tablet.writeImage((byte)stu_Tablet.encodingMode, stu_Tablet.bitmapData);

      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        stu_Tablet.penTimeData.Clear();
      }
      else
      {
        stu_Tablet.penData.Clear();
      }

      stu_Tablet.m_isDown = 0;
      this.Invalidate();
    }


    private void btnJpeg_Click(object sender, EventArgs e)
    {
      // Save the image.
      SaveImage("JPEG");
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnPng_Click(object sender, EventArgs e)
    {
      // Save the image.
      SaveImage("PNG");
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnBmp_Click(object sender, EventArgs e)
    {
      // Save the image.
      SaveImage("BMP");
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnGif_Click(object sender, EventArgs e)
    {
      // Save the image.
      SaveImage("GIF");
      this.DialogResult = DialogResult.OK;
      this.Close();
    }


    private void btnCancel_Click(object sender, EventArgs e)
    {
      // You probably want to add additional processing here.
      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        this.stu_Tablet.penTimeData = null;
      }
      else
      {
        this.stu_Tablet.penData = null;
      }

      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      if (stu_Tablet.penData.Count != 0 || stu_Tablet.penTimeData.Count != 0)
      {
        clearScreen();
      }
    }

    // Pass in the device you want to connect to!
    public SignatureForm(Form mainForm, wgssSTU.IUsbDevice usbDevice)
    {
      int currentPenDataOptionMode;

      stu_Tablet = new TabletLib.STU_Tablet(this, mainForm, this.ClientSize.Height, this.ClientSize.Width);

      // This is a DPI aware application, so ensure you understand how .NET client coordinates work.
      // Testing using a Windows installation set to a high DPI is recommended to understand how
      // values get scaled or not.

      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

      InitializeComponent();

      // A more sophisticated applications should cycle for a few times as the connection may only be
      // temporarily unavailable for a second or so. 
      // For example, if a background process such as Wacom STU Display
      // is running, this periodically updates a slideshow of images to the device.

      wgssSTU.IErrorCode ec = stu_Tablet.tablet.usbConnect(usbDevice, true);
      if (ec.value == 0)
      {
        stu_Tablet.capability = stu_Tablet.tablet.getCapability();
        stu_Tablet.information = stu_Tablet.tablet.getInformation();

        // First find out if the pad supports the pen data option mode (the 300 doesn't)
        currentPenDataOptionMode = stu_Tablet.GetCurrentPenDataOptionMode();

        // Set up the tablet to return time stamp with the pen data or just basic data
        stu_Tablet.SetCurrentPenDataOptionMode(currentPenDataOptionMode);
      }
      else
      {
        throw new Exception(ec.message);
      }

      this.SuspendLayout();
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

      // Set the size of the client window to be actual size, 
      // based on the reported DPI of the monitor.

      Size clientSize = new Size((int)(stu_Tablet.capability.tabletMaxX / 2540F * 96F), (int)(stu_Tablet.capability.tabletMaxY / 2540F * 96F));
      this.ClientSize = clientSize;
      this.ResumeLayout();

      // Place the buttons across the bottom of the screen.
      m_btns = new GraphicsLib.Buttons.Button[6];

      int btnWidth = stu_Tablet.capability.screenWidth / m_btns.Length;
      int y = stu_Tablet.capability.screenHeight * 6 / 7;
      int h = stu_Tablet.capability.screenHeight - y;

      for (int j = 0; j < m_btns.Length; j++)
      {
        if (j == 0)
        {
          m_btns[0].Bounds = new Rectangle(0, y, btnWidth, h);
        }
        else
        {
          m_btns[j].Bounds = new Rectangle(btnWidth * j, y, btnWidth, h);
        }
      }

      m_btns[0].Text = "JPEG";
      m_btns[1].Text = "PNG";
      m_btns[2].Text = "BMP";
      m_btns[3].Text = "GIF";
      m_btns[4].Text = "Clear";
      m_btns[5].Text = "Canc";

      m_btns[0].Click = new EventHandler(btnJpeg_Click);
      m_btns[1].Click = new EventHandler(btnPng_Click);
      m_btns[2].Click = new EventHandler(btnBmp_Click);
      m_btns[3].Click = new EventHandler(btnGif_Click);
      m_btns[4].Click = new EventHandler(btnClear_Click);
      m_btns[5].Click = new EventHandler(btnCancel_Click);

      stu_Tablet.btns = m_btns;

      // Calculate the encodingMode that will be used to update the image
      stu_Tablet.SetEncodingMode();

      stu_Tablet.bitmap = GraphicsLib.GraphicFunctions.CreateBitmap(stu_Tablet.capability, stu_Tablet.useColor, m_btns);

      // Finally, use this bitmap for the window background.
      this.BackgroundImage = stu_Tablet.bitmap;
      this.BackgroundImageLayout = ImageLayout.Stretch;

      stu_Tablet.ConvertBitmap();

      // If you wish to further optimize image transfer, you can compress the image using 
      // the Zlib algorithm.

      bool useZlibCompression = false;
      if (!stu_Tablet.useColor && useZlibCompression)
      {
        // m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
        stu_Tablet.encodingMode |= wgssSTU.encodingMode.EncodingMode_Zlib;
      }

      stu_Tablet.addDelegates();

      // Initialize the screen
      clearScreen();

      // Enable the pen data on the screen (if not already)
      stu_Tablet.tablet.setInkingMode(0x01);

      stu_Tablet.sigFormWidth = this.ClientSize.Width;
      stu_Tablet.sigFormHeight = this.ClientSize.Height;
    }

    private void Form2_FormClosed(object sender, FormClosedEventArgs e)
    {
      // Ensure that you correctly disconnect from the tablet, otherwise you are 
      // likely to get errors when wanting to connect a second time.
      if (stu_Tablet.tablet != null)
      {
        stu_Tablet.removeDelegates();
        stu_Tablet.tablet.setInkingMode(0x00);
        stu_Tablet.tablet.setClearScreen();
        stu_Tablet.tablet.disconnect();
      }

      stu_Tablet.penInk.Dispose();
    }

    private void Form2_Paint(object sender, PaintEventArgs e)
    {
      // Call the appropriate routine to render the pen strokes on the client window
      // depending on what type of pen data is being received

      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        GraphicsLib.GraphicFunctions.renderPenTimeData(this, stu_Tablet);
      }
      else
      {
        GraphicsLib.GraphicFunctions.renderPenData(this, stu_Tablet);
      }
    }

    private void Form2_MouseClick(object sender, MouseEventArgs e)
    {
      // Enable the mouse to click on the simulated buttons that we have displayed.

      // Note that this can add some tricky logic into processing pen data
      // if the pen was down at the time of this click, especially if the pen was logically
      // also 'pressing' a button! This demo however ignores any that.

      Point pt = stu_Tablet.ClientToScreen(e.Location);
      foreach (GraphicsLib.Buttons.Button btn in stu_Tablet.btns)
      {
        if (btn.Bounds.Contains(pt))
        {
          btn.PerformClick();
          break;
        }
      }
    }

    // Save the image in a local file
    private void SaveImage(String ImageType)
    {
      String fileExtension;
      String saveLocation;

      try
      {
        Bitmap bitmap = GraphicsLib.GraphicFunctions.GetSigImage(stu_Tablet.demoButtonsForm, this, stu_Tablet);

        switch (ImageType)
        {
          case "JPEG":
            fileExtension = "jpg";
            saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output." + fileExtension;
            bitmap.Save(saveLocation, ImageFormat.Jpeg);
            break;
          case "PNG":
            fileExtension = "png";
            saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output." + fileExtension;
            bitmap.Save(saveLocation, ImageFormat.Png);
            break;
          case "BMP":
            fileExtension = "bmp";
            saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output." + fileExtension;
            bitmap.Save(saveLocation, ImageFormat.Bmp);
            break;
          case "GIF":
            fileExtension = "gif";
            saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output." + fileExtension;
            bitmap.Save(saveLocation, ImageFormat.Gif);
            break;
          default:
            fileExtension = "jpg";
            saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output." + fileExtension;
            bitmap.Save(saveLocation, ImageFormat.Jpeg);
            break;
        }
        this.msg = "Signature image saved to " + saveLocation;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Exception: " + ex.Message);
      }
    }
  }
}
