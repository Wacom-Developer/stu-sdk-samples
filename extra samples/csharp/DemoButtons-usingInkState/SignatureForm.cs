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
using System.Text;
using System.Windows.Forms;

namespace DemoButtons
{
  public partial class SignatureForm : Form
  {
    public TabletLib.STU_Tablet stu_Tablet;
    public int penDataType;
    private wgssSTU.IInkingState m_inkingState;

    private Pen m_penInk;  // cached object.
    
    // The isDown flag is used like this:
    // 0 = up
    // +ve = down, pressed on button number
    // -1 = down, inking
    // -2 = down, ignoring
    private int m_isDown;

    // As per the file comment, there are three coordinate systems to deal with.
    // To help understand, we have left the calculations in place rather than optimise them.

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

      m_isDown = 0;
      this.Invalidate();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      // You probably want to add additional processing here.
      penDataType = stu_Tablet.penDataOptionMode;

      if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
      {
        if (stu_Tablet.penTimeData.Count > 0)
        {
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      else
      {
        if (stu_Tablet.penData.Count > 0)
        {
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
        clearScreen();
      }
    }

    // Pass in the device you want to connect to!
    public SignatureForm(DemoButtonsForm parent, wgssSTU.IUsbDevice usbDevice)
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

      // A more sophisticated applications should cycle for a few times as the connection may only be
      // temporarily unavailable for a second or so. 
      // For example, if a background process such as Wacom STU Display
      // is running, this periodically updates a slideshow of images to the device.

      wgssSTU.IErrorCode ec = stu_Tablet.tablet.usbConnect(usbDevice, true);
      if (ec.value == 0)
      {
        stu_Tablet.capability = stu_Tablet.tablet.getCapability();
        stu_Tablet.information = stu_Tablet.tablet.getInformation();
        print("Connected: " + stu_Tablet.information.modelName);

        m_inkingState = new wgssSTU.InkingState();
        m_inkingState.setInkThreshold(stu_Tablet.tablet.getInkThreshold());

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

      STUButtons = new GraphicsLib.Buttons(usbDevice.idProduct, stu_Tablet.capability, false, btnOk_Click, btnClear_Click, btnCancel_Click);
      stu_Tablet.btns = STUButtons.btns;

      // Calculate the encodingMode that will be used to update the image
      stu_Tablet.SetEncodingMode();
      stu_Tablet.bitmap = GraphicsLib.GraphicFunctions.CreateBitmap(stu_Tablet.capability, stu_Tablet.useColor, STUButtons.btns);

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
        // m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
        stu_Tablet.encodingMode |= wgssSTU.encodingMode.EncodingMode_Zlib;
      }

      addDelegates();

      // Initialize the screen
      clearScreen();

      // Enable the pen data on the screen (if not already)
      stu_Tablet.tablet.setInkingMode(0x01);

      stu_Tablet.sigFormWidth = this.ClientSize.Width;
      stu_Tablet.sigFormHeight = this.ClientSize.Height;
    }

    public void addDelegates()
    {
      // Add the delegates that receive pen data.
      stu_Tablet.tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

      stu_Tablet.tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
      stu_Tablet.tablet.onPenDataEncrypted += new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);
      
      stu_Tablet.tablet.onPenDataTimeCountSequence += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
      stu_Tablet.tablet.onPenDataTimeCountSequenceEncrypted += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
    }

    public void removeDelegates()
    {
      // Add the delegates that receive pen data.
      stu_Tablet.tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

      stu_Tablet.tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
      stu_Tablet.tablet.onPenDataEncrypted -= new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

      stu_Tablet.tablet.onPenDataTimeCountSequence -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
      stu_Tablet.tablet.onPenDataTimeCountSequenceEncrypted -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
    }

    private void onGetReportException(wgssSTU.ITabletEventsException tabletEventsException)
    {
      try
      {
        tabletEventsException.getException();
      }
      catch (Exception e)
      {
        MessageBox.Show("Error: " + e.Message);
        stu_Tablet.tablet.disconnect();
        stu_Tablet.tablet = null;
        this.Close();
      }
    }

    private void Form2_FormClosed(object sender, FormClosedEventArgs e)
    {
      // Ensure that you correctly disconnect from the tablet, otherwise you are 
      // likely to get errors when wanting to connect a second time.
      if (stu_Tablet.tablet != null)
      {
        removeDelegates();
        stu_Tablet.tablet.setInkingMode(0x00);
        stu_Tablet.tablet.setClearScreen();
        stu_Tablet.tablet.disconnect();
      }

      stu_Tablet.penInk.Dispose();
    }

    private void onPenDataTimeCountSequenceEncrypted(wgssSTU.IPenDataTimeCountSequenceEncrypted penTimeCountSequenceDataEncrypted) // Process incoming pen data
    {
      onPenDataTimeCountSequence(penTimeCountSequenceDataEncrypted);
    }

    private void onPenDataTimeCountSequence(wgssSTU.IPenDataTimeCountSequence penTimeData)
    {
      Point pt = stu_Tablet.TabletToScreen(penTimeData);
      int btn = stu_Tablet.buttonClicked(pt); // Check if a button was clicked

      wgssSTU.InkState inkState = m_inkingState.nextState(penTimeData.pressure);
      bool isDown = ((int)inkState != (int)wgssSTU.InkState_.InkState_isOff);

      // This code uses a model of four states the pen can be in:
      // down or up, and whether this is the first sample of that state.

      if (isDown)
      {
        if (m_isDown == 0)
        {
          // transition to down
          if (btn > 0)
          {
            // We have put the pen down on a button.
            // Track the pen without inking on the client.

            m_isDown = btn;
          }
          else
          {
            // We have put the pen down somewhere else.
            // Treat it as part of the signature.

            m_isDown = -1;
          }
        }
        else
        {
          // already down, keep doing what we're doing!
        }

        // draw
        if ((inkState == wgssSTU.InkState.InkState_Inking || inkState == wgssSTU.InkState.InkState_First) && m_isDown == -1)
        {
          // Draw a line from the previous down point to this down point.
          // This is the simplist thing you can do; a more sophisticated program
          // can perform higher quality rendering than this!

          Graphics gfx = GraphicsLib.GraphicFunctions.SetQualityGraphics(this);
          wgssSTU.IPenDataTimeCountSequence prevPenData = stu_Tablet.penTimeData[stu_Tablet.penTimeData.Count - 1];

          PointF prev;
          PointF curr;

          if (inkState == wgssSTU.InkState.InkState_First)
          {
            prev = stu_Tablet.TabletToClient(penTimeData);
            curr = stu_Tablet.TabletToClient(penTimeData);
          }
          else
          {
            prev = stu_Tablet.TabletToClient(prevPenData);
            curr = stu_Tablet.TabletToClient(penTimeData);
          }

          if (!penTimeData.rdy)
            curr = prev;

          if (inkState == wgssSTU.InkState.InkState_First)
          {
            // Drawline won't work if the pen point hasn't changed so draw a 2-pixel rectangle instead
            // 1 pixel isn't enough to be in proportion with the ink on the pad
            // You might want to use a different colour brush

            Brush aBrush = (Brush)Brushes.Black;
            gfx.FillRectangle(aBrush, curr.X, curr.Y, 2, 2);
          }
          else
          {
            gfx.DrawLine(stu_Tablet.penInk, prev, curr);
            gfx.Dispose();
          }
        }

        // The pen is down, store it for use later.
        if (m_isDown == -1)
          stu_Tablet.penTimeData.Add(penTimeData);
      }
      else
      {
        if (m_isDown != 0)
        {
          // transition to up
          if (btn > 0)
          {
            // The pen is over a button

            if (btn == m_isDown)
            {
              // The pen was pressed down over the same button as is was lifted now. 
              // Consider that as a click!
              //m_parent.print("Performing button " + btn);
              stu_Tablet.btns[btn - 1].PerformClick();
            }
          }
          m_isDown = 0;
        }
        else
        {
          // still up
        }

        // Add up data once we have collected some down data.
        if (stu_Tablet.penTimeData != null)
        {
          if (stu_Tablet.penTimeData.Count != 0)
            stu_Tablet.penTimeData.Add(penTimeData);
        }
      }
    }

    private void onPenDataEncrypted(wgssSTU.IPenDataEncrypted penData) // Process incoming pen data
    {
      onPenData(penData.penData1);
      onPenData(penData.penData2);
    }

    private void onPenData(wgssSTU.IPenData penData) // Process incoming pen data
    {
      Point pt = stu_Tablet.TabletToScreen(penData);

      int btn = 0; // will be +ve if the pen is over a button.
      {        
        for (int i = 0; i < stu_Tablet.btns.Length; ++i)
        {
          if (stu_Tablet.btns[i].Bounds.Contains(pt))
          {
            btn = i+1;
            break;
          }          
        }
      }

      wgssSTU.InkState inkState = m_inkingState.nextState(penData.pressure);

      bool isDown = ((int)inkState != (int)wgssSTU.InkState_.InkState_isOff);

      // This code uses a model of four states the pen can be in:
      // down or up, and whether this is the first sample of that state.

      if (isDown)
      {
        if (m_isDown == 0)
        {
          // transition to down
          if (btn > 0)
          {
            // We have put the pen down on a button.
            // Track the pen without inking on the client.

            m_isDown = btn; 
          }
          else
          {
            // We have put the pen down somewhere else.
            // Treat it as part of the signature.

            m_isDown = -1;
          }
        }
        else
        {
          // already down, keep doing what we're doing!
        }

        // draw
        if ((inkState == wgssSTU.InkState.InkState_Inking || inkState == wgssSTU.InkState.InkState_First) && m_isDown == -1)
        {
          // Draw a line from the previous down point to this down point.
          // This is the simplist thing you can do; a more sophisticated program
          // can perform higher quality rendering than this!
          
          Graphics gfx = this.CreateGraphics();
          gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
          gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
          gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
          gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          
          wgssSTU.IPenData prevPenData = stu_Tablet.penData[stu_Tablet.penData.Count - 1];

          PointF prev;
          PointF curr;

          if (inkState == wgssSTU.InkState.InkState_First)
          {
            prev = stu_Tablet.TabletToClient(penData);
            curr = stu_Tablet.TabletToClient(penData);
          }
          else
          {
            prev = stu_Tablet.TabletToClient(prevPenData);
            curr = stu_Tablet.TabletToClient(penData);
          }

          if (!penData.rdy)
            curr = prev;

          if (inkState == wgssSTU.InkState.InkState_First)
          {
            // Drawline won't work if the pen point hasn't changed so draw a 2-pixel rectangle instead
            // 1 pixel isn't enough to be in proportion with the ink on the pad
            // You might want to use a different colour brush

            Brush aBrush = (Brush)Brushes.Black;
            gfx.FillRectangle(aBrush, curr.X, curr.Y, 2, 2);
          }
          else
          {
            gfx.DrawLine(stu_Tablet.penInk, prev, curr);
            gfx.Dispose();
          }
        }

        // The pen is down, store it for use later.
        if (m_isDown == -1)
          stu_Tablet.penData.Add(penData);
      }
      else
      {
        if (m_isDown != 0)
        {
          // transition to up
          if (btn > 0)
          {
            // The pen is over a button

            if (btn == m_isDown)
            {
              // The pen was pressed down over the same button as is was lifted now. 
              // Consider that as a click!
              stu_Tablet.btns[btn - 1].PerformClick();
            }
          }
          m_isDown = 0;
        }
        else
        {
           // still up
        }

        // Add up data once we have collected some down data.
        if (stu_Tablet.penData.Count != 0)
          stu_Tablet.penData.Add(penData);
      }
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

    public Bitmap GetSigImage() {

        Bitmap bitmap;
        SolidBrush brush;
        Brush aBrush = (Brush)Brushes.Black;

        Rectangle rect = new Rectangle(0, 0, stu_Tablet.capability.screenWidth, stu_Tablet.capability.screenHeight);

        try
        {
            bitmap = new Bitmap(rect.Width, rect.Height);
            Graphics gfx = Graphics.FromImage(bitmap);
            SizeF s = this.AutoScaleDimensions;
            //            Dim inkWidthMM = 0.7F
            Single inkWidthMM = 1.0F;
            m_penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2.0F));
            m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            wgssSTU.IInkingState inkingState = new wgssSTU.InkingState();
            inkingState.setInkThreshold(m_inkingState.getInkThreshold());

            brush = new SolidBrush(Color.White);
            gfx.FillRectangle(brush, 0, 0, rect.Width, rect.Height);

            PointF prev = new PointF();

            if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
            {
              for (int i = 1; i < stu_Tablet.penTimeData.Count; i++)
              {
                PointF curr = stu_Tablet.TabletToScreen(stu_Tablet.penTimeData[i]);

                if (i > 1)
                {
                  prev = stu_Tablet.TabletToScreen(stu_Tablet.penTimeData[i - 1]);
                }

                switch (inkingState.nextState(stu_Tablet.penTimeData[i].pressure))
                {
                  case wgssSTU.InkState.InkState_First:
                    gfx.FillRectangle(aBrush, curr.X, curr.Y, 2, 2);
                    break;

                  case wgssSTU.InkState.InkState_Inking:
                    gfx.DrawLine(m_penInk, prev, curr);
                    break;
                }
                prev = curr;
              }
            }
            else
				    {
              for (int i = 1; i < stu_Tablet.penData.Count; i++)
              {
                PointF curr = stu_Tablet.TabletToScreen(stu_Tablet.penData[i]);

                if (i > 1)
                {
                  prev = stu_Tablet.TabletToScreen(stu_Tablet.penData[i - 1]);
                }

                switch (inkingState.nextState(stu_Tablet.penData[i].pressure))
                {
                  case wgssSTU.InkState.InkState_First:
                    gfx.FillRectangle(aBrush, curr.X, curr.Y, 2, 2);
                    break;

                  case wgssSTU.InkState.InkState_Inking:
                    gfx.DrawLine(m_penInk, prev, curr);
                    break;
                }
                prev = curr;
              }
            }
          
        }
        catch (Exception ex)
        {
            print("Exception: " + ex.Message);
            MessageBox.Show("Exception: " + ex.Message);
            bitmap = null;
        }
        return bitmap;
      }
  }
}
