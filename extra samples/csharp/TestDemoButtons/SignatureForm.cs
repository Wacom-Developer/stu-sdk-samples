/******************************************************* 

  SignatureForm.cs
  
  Display signature capture form on STU pad and on Windows
  with OK, Clear and Cancel buttons. After capturing the signature
  displays it on the main form as an image.

  Compatible with STU-300, STU-430, STU-500, STU-530 and STU-540
  
  Copyright (c) 2023 Wacom Ltd. All rights reserved.
  
********************************************************/
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
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DemoButtons
{
  public partial class SignatureForm : Form
  {
    public TabletLib.STU_Tablet stu_Tablet;
    private int penDataType;
    private bool calcPenSpeed;
    private bool reverseSigImage;
    private bool saveSigImage;

    DemoButtonsForm m_parent;   // give access to calling form

    private GraphicsLib.Buttons STUButtons;

    public void updateScreen()
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
      String msgText;

      // You probably want to add additional processing here.
      penDataType = stu_Tablet.penDataOptionMode;

      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        if (stu_Tablet.penTimeData.Count > 0)
        {
          ++stu_Tablet.clickEventCount;
          if (stu_Tablet.clickEventCount == 1)
          {
            if (saveSigImage)
            {
              msgText = stu_Tablet.saveImage(reverseSigImage);
              print(msgText);
            }
            if (calcPenSpeed)
            {
              //  Calculate the average speed of the pen
              msgText = stu_Tablet.calcSpeed();
              print(msgText);
            }
          }
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      else
      {
        if (stu_Tablet.penData.Count > 0)
        {
          ++stu_Tablet.clickEventCount;
          if (stu_Tablet.clickEventCount == 1)
          {
            if (saveSigImage)
            {
              msgText = stu_Tablet.saveImage(reverseSigImage);
              print(msgText);
            }
            /*
            if (calcPenSpeed)
            {
              // Calculate the average speed of the pen
              msgText = stu_Tablet.calcSpeed();
              print(msgText);
            }
            */
          }
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
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
        updateScreen();
      }
    }

    // Pass in the device you want to connect to!
    // The 3 boolean values relate to the check boxes on the main form.
    // These are to enable the options for saving the image to disk, calculating the pen speed and reversing the image on the STU
    public SignatureForm(DemoButtonsForm parent, wgssSTU.IUsbDevice usbDevice, bool chkSaveImage, bool chkCalcSpeed, bool chkReverseImage)
    {
      int currentPenDataOptionMode;

      saveSigImage = chkSaveImage;
      calcPenSpeed = chkCalcSpeed;
      reverseSigImage = chkReverseImage;
      m_parent = parent;

      stu_Tablet = new TabletLib.STU_Tablet(this, parent, this.ClientSize.Height, this.ClientSize.Width);

      // This is a DPI aware application, so ensure you understand how .NET client coordinates work.
      // Testing using a Windows installation set to a high DPI is recommended to understand how
      // values get scaled or not.

      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

      InitializeComponent();

      // A more sophisticated application should cycle for a few times as the connection may only be
      // temporarily unavailable for a second or so. 
      // For example, if a background process such as Wacom STU Display
      // is running, this periodically updates a slideshow of images to the device.

      wgssSTU.IErrorCode ec = stu_Tablet.tablet.usbConnect(usbDevice, true);
      if (ec.value == 0)
      {
        stu_Tablet.capability = stu_Tablet.tablet.getCapability();
        stu_Tablet.information = stu_Tablet.tablet.getInformation();
        print("Connected: " + stu_Tablet.information.modelName);

        // First find out if the pad supports the pen data option mode (the 300 doesn't)
        currentPenDataOptionMode = stu_Tablet.GetCurrentPenDataOptionMode();

        // Set up the tablet to return time stamp with the pen data or just basic data
        stu_Tablet.SetCurrentPenDataOptionMode(currentPenDataOptionMode);
      }
      else
      {
        throw new Exception(ec.message);
      }

      // Set the size of the client window to be actual size, 
      // based on the reported DPI of the monitor.

      this.SuspendLayout();
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

      Size clientSize = new Size((int)(stu_Tablet.capability.tabletMaxX / 2540F * 96F), (int)(stu_Tablet.capability.tabletMaxY / 2540F * 96F));
      this.ClientSize = clientSize;
      this.ResumeLayout();

      STUButtons = new GraphicsLib.Buttons(usbDevice.idProduct, stu_Tablet.capability, reverseSigImage, btnOk_Click, btnClear_Click, btnCancel_Click);
      stu_Tablet.btns = STUButtons.btns;

      // Calculate the encodingMode that will be used to update the image
      stu_Tablet.SetEncodingMode();
      stu_Tablet.bitmap = GraphicsLib.GraphicFunctions.CreateBitmap(stu_Tablet.capability, stu_Tablet.useColor, STUButtons.btns);

      if (reverseSigImage)
      {
        /* Flip the bitmap so the image appears upside down on the pad.
         * This necessitates resetting the button rectangle boundary information
        */
        // The Y position for all 3 buttons is 0
        int btnYPos = 0;
        int btnWidth = stu_Tablet.capability.screenWidth / 3;
        int btnHeight = stu_Tablet.capability.screenHeight - (stu_Tablet.capability.screenHeight * 6 / 7);

        STUButtons.btns[0].Bounds = new Rectangle(0, btnYPos, btnWidth, btnHeight);
        STUButtons.btns[1].Bounds = new Rectangle(btnWidth, btnYPos, btnWidth, btnHeight);
        STUButtons.btns[2].Bounds = new Rectangle((btnWidth * 2) + 1, btnYPos, btnWidth, btnHeight);
        stu_Tablet.bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
      }

      stu_Tablet.STUButtons = STUButtons;

      // Finally, use this bitmap for the window background.
      this.BackgroundImage = stu_Tablet.bitmap;
      this.BackgroundImageLayout = ImageLayout.Stretch;

      stu_Tablet.ConvertBitmap();

      // If you wish to further optimize image transfer, you can compress the image using 
      // the Zlib algorithm.
      
      bool useZlibCompression = false;
      if (!stu_Tablet.useColor && useZlibCompression)
      {
        // stu_Tablet.bitmapData = compress_using_zlib(stu_Tablet.bitmapData); // insert compression here!
        stu_Tablet.encodingMode |= wgssSTU.encodingMode.EncodingMode_Zlib;
      }

      stu_Tablet.addDelegates();  // Add the delegates for receiving pen data

      // Initialize the screen
      updateScreen();

      // Enable the pen data on the screen (if not already)
      stu_Tablet.tablet.setInkingMode(0x01);

      // Recalculate the signature form size after the changes above
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
    private void print(string txt)
    {
        m_parent.print(txt);            // update parent form
    }
  }
}
