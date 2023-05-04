/*
 SignatureForm.cs

 Allows user to input a signature on an STU and reproduces it on a Window on the PC
 The user can choose between HID and serial input from the signature pad.

 HID mode works with the STU-300, STU-430, STU-500, STU-530 and STU-540.
 Serial mode works satisfactorily with the STU-430V (no longer manufactured)
 but not with the STU-540 in serial mode because it's too slow

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

enum PenDataOptionMode
{
    PenDataOptionMode_None,
    PenDataOptionMode_TimeCount,
    PenDataOptionMode_SequenceNumber,
    PenDataOptionMode_TimeCountSequence
};

namespace DemoButtons
{


  public partial class SignatureForm : Form
  {
    public TabletLib.STU_Tablet stu_Tablet;

    DemoButtonsForm m_parent;   // give access to calling form

    private GraphicsLib.Buttons STUButtons;

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


    private void btnOk_Click(object sender, EventArgs e)
    {
      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        closeProgramOK(stu_Tablet.penTimeData.Count);
      }
      else
      {
        closeProgramOK(stu_Tablet.penData.Count);
      }

    }

    private void closeProgramOK(int penDataCount)
    {
      if (penDataCount > 0)
      {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      else
      {
        MessageBox.Show("No signature provided");
      }
    }


    private void btnCancel_Click(object sender, EventArgs e)
    {
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
      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        clearSignature(stu_Tablet.penTimeData.Count);
      }
      else
      {
        clearSignature(stu_Tablet.penData.Count);
      }

    }


    private void clearSignature(int penDataCount)
    {
      if (penDataCount != 0)
      {
        clearScreen();
      }
    }

    // Pass in the device you want to connect to!
    public SignatureForm(DemoButtonsForm parent, wgssSTU.IUsbDevice usbDevice, wgssSTU.SerialInterface serialInterface, string fileNameCOMPort, string baudRate, bool HIDMode)
    {
      int currentPenDataOptionMode;

      m_parent = parent;

      stu_Tablet = new TabletLib.STU_Tablet(this, parent, this.ClientSize.Height, this.ClientSize.Width);

      // This is a DPI aware application, so ensure you understand how .NET client coordinates work.
      // Testing using a Windows installation set to a high DPI is recommended to understand how
      // values get scaled or not.

      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

      InitializeComponent();

      wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();

      // A more sophisticated applications should cycle for a few times as the connection may only be
      // temporarily unavailable for a second or so. 
      // For example, if a background process such as Wacom STU Display
      // is running, this periodically updates a slideshow of images to the device.

      wgssSTU.IErrorCode ec = null;
      if (HIDMode)
      {
        ec = stu_Tablet.tablet.usbConnect(usbDevice, true);
      }
      else
      {
        uint padBaudRate = uint.Parse(baudRate);
        ec = stu_Tablet.tablet.serialConnect(fileNameCOMPort, padBaudRate, true);
      }

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

      ushort productId = stu_Tablet.tablet.getProductId();

      STUButtons = new GraphicsLib.Buttons(productId, stu_Tablet.capability, false, btnOk_Click, btnClear_Click, btnCancel_Click);
      stu_Tablet.btns = STUButtons.btns;

      // Calculate the encodingMode that will be used to update the image
      stu_Tablet.SetEncodingMode();
      stu_Tablet.bitmap = GraphicsLib.GraphicFunctions.CreateBitmap(stu_Tablet.capability, stu_Tablet.useColor, STUButtons.btns);

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
      foreach (GraphicsLib.Buttons.Button btn in STUButtons.btns)
      {
        if (btn.Bounds.Contains(pt))
        {
          btn.PerformClick();
          break;
        }
      }
    }
  }
}
