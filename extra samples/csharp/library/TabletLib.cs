/******************************************************* 

  TabletLib.cs
  
  Library of functions related to the use of the STU tablet
  
  Copyright (c) 2023 Wacom Ltd. All rights reserved.
  
********************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace TabletLib
{
	public class STU_Tablet
	{
    public bool useColor;
    public int clickEventCount;
    public int sigFormHeight;
    public int sigFormWidth;
    public int m_isDown;
    public int penDataOptionMode;
    public List<wgssSTU.IPenData> penData;
    public List<wgssSTU.IPenDataTimeCountSequence> penTimeData;
    public wgssSTU.Tablet tablet;
    public wgssSTU.ICapability capability;
    public wgssSTU.IInformation information;
    public Pen penInk;  // cached object.
    public wgssSTU.encodingMode encodingMode;
    public Bitmap bitmap;
    public byte[] bitmapData;
    public GraphicsLib.Buttons STUButtons;
    public GraphicsLib.Buttons.Button[] btns;
    public Graphics gfx;
    public Form signatureForm;
    public Form demoButtonsForm;

    public STU_Tablet(Form signatureForm, Form demoForm, int clientHeight, int clientWidth)
    {
      this.signatureForm = signatureForm;
      this.demoButtonsForm = demoForm;
      this.penData = new List<wgssSTU.IPenData>();
      this.penDataOptionMode = -1;
      this.tablet = new wgssSTU.Tablet();
      this.sigFormHeight = clientHeight;
      this.sigFormWidth = clientWidth;
      this.clickEventCount = 0;

      // Calculate the size and cache the inking pen.

      SizeF s = signatureForm.AutoScaleDimensions;
      float inkWidthMM = 0.7F;
      this.penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2F));
      this.penInk.StartCap = this.penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
      this.penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
    }
    
    public static string FindSTUModelName()
		{
      string STU_Model = "Unknown";
      wgssSTU.IInformation information;

      wgssSTU.Tablet tablet = new wgssSTU.Tablet();

      wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
      if (usbDevices.Count != 0)
      {
        try
        {
          wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device
          wgssSTU.IErrorCode ec = tablet.usbConnect(usbDevice, true);
          if (ec.value == 0)
          {
            information = tablet.getInformation();
            STU_Model = information.modelName;
            tablet.disconnect();
          }
        }
        catch
				{
          STU_Model = "No STU found";
				}
      }

      return STU_Model;
		}
    
    public PointF TabletToClient(wgssSTU.IPenData penDataPoint)
    {
      // Client means the Windows Form coordinates.
      return new PointF((float)penDataPoint.x * this.sigFormWidth / this.capability.tabletMaxX, (float)penDataPoint.y * this.sigFormHeight / this.capability.tabletMaxY);
    }

    public PointF TabletToClientTimed(wgssSTU.IPenDataTimeCountSequence penData)
    {
      // Client means the Windows Form coordinates.
      return new PointF((float)penData.x * this.sigFormWidth / this.capability.tabletMaxX, (float)penData.y * this.sigFormHeight / this.capability.tabletMaxY);
    }

    public Point TabletToScreen(wgssSTU.IPenData penData)
    {
      // Screen means LCD screen of the tablet.
      return Point.Round(new PointF((float)penData.x * this.capability.screenWidth / this.capability.tabletMaxX, (float)penData.y * this.capability.screenHeight / this.capability.tabletMaxY));
    }

    public Point ClientToScreen(Point pt)
    {
      // client (window) coordinates to LCD screen coordinates. 
      // This is needed for converting mouse coordinates into LCD bitmap coordinates as that's
      // what this application uses as the coordinate space for buttons.
      return Point.Round(new PointF((float)pt.X * this.capability.screenWidth / this.sigFormWidth, (float)pt.Y * this.capability.screenHeight / this.sigFormHeight));
    }

    public wgssSTU.ICapability getCapability()
    {
      if (this.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
        return this.penTimeData != null ? this.capability : null;
      else
        return this.penData != null ? this.capability : null;
    }

    public wgssSTU.IInformation getInformation()
    {
      if (this.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
        return this.penTimeData != null ? this.information : null;
      else
        return this.penData != null ? this.information : null;
    }

    public void SetEncodingMode()
    {
      this.useColor = false;
      wgssSTU.encodingMode encodingMode;
      wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();

      // Calculate the encodingMode that will be used to update the image

      ushort idP = tablet.getProductId();
      wgssSTU.encodingFlag encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(idP, 0);

      if ((encodingFlag & (wgssSTU.encodingFlag.EncodingFlag_16bit | wgssSTU.encodingFlag.EncodingFlag_24bit)) != 0)
      {
        if (tablet.supportsWrite())
          this.useColor = true;
      }

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
      this.encodingMode = encodingMode;
    }

    public int GetCurrentPenDataOptionMode()
    {
      int penDataOptionMode;

      try
      {
        penDataOptionMode = this.tablet.getPenDataOptionMode();
      }
      catch (Exception optionModeException)
      {
        penDataOptionMode = -1;
      }
      return penDataOptionMode;
    }

    public void SetCurrentPenDataOptionMode(int currentPenDataOptionMode)
    {
      // If the current option mode is TimeCount then this is a 520 so we must reset the mode
      // to basic data only as there is no handler for TimeCount

      switch (currentPenDataOptionMode)
      {
        case -1:
          // THis must be the 300 which doesn't support getPenDataOptionMode at all so only basic data
          this.penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
          break;

        case (int)PenDataOptionMode.PenDataOptionMode_None:
          // If the current option mode is "none" then it could be any pad so try setting the full option
          // and if it fails or ends up as TimeCount then set it to none
          try
          {
            tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_TimeCountSequence);
            this.penDataOptionMode = tablet.getPenDataOptionMode();
            if (this.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCount)
            {
              tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
              this.penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
            }
            else
            {
              this.penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence;
            }
          }
          catch (Exception ex)
          {
            // THis shouldn't happen but just in case...
            this.penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
          }
          break;

        case (int)PenDataOptionMode.PenDataOptionMode_TimeCount:
          tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
          this.penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
          break;

        case (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence:
          // If the current mode is timecountsequence then leave it at that
          this.penDataOptionMode = currentPenDataOptionMode;
          break;
      }

      switch ((int)this.penDataOptionMode)
      {
        case (int)PenDataOptionMode.PenDataOptionMode_None:
          this.penData = new List<wgssSTU.IPenData>();
          break;
        case (int)PenDataOptionMode.PenDataOptionMode_TimeCount:
          this.penData = new List<wgssSTU.IPenData>();
          break;
        case (int)PenDataOptionMode.PenDataOptionMode_SequenceNumber:
          this.penData = new List<wgssSTU.IPenData>();
          break;
        case (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence:
          this.penTimeData = new List<wgssSTU.IPenDataTimeCountSequence>();
          break;
        default:
          this.penData = new List<wgssSTU.IPenData>();
          break;
      }
    }

    // Once the bitmap has been created, it needs to be converted to device-native format.
    public void ConvertBitmap()
    {
      // Unfortunately it is not possible for the native COM component to
      // understand .NET bitmaps. We have therefore convert the .NET bitmap
      // into a memory blob that will be understood by COM.

      wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();
      System.IO.MemoryStream stream = new System.IO.MemoryStream();

      this.bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
      this.bitmapData = (byte[])protocolHelper.resizeAndFlatten(stream.ToArray(), 0, 0, (uint)this.bitmap.Width, (uint)this.bitmap.Height, this.capability.screenWidth, this.capability.screenHeight, (byte)this.encodingMode, wgssSTU.Scale.Scale_Fit, 0, 0);
      protocolHelper = null;

      stream.Dispose();
    }

    public int buttonClicked(Point pt)
    {
      int btn = 0; // will be +ve if the pen is over a button.
      {
        for (int i = 0; i < this.btns.Length; ++i)
        {
          if (this.btns[i].Bounds.Contains(pt))
          {
            btn = i + 1;
            break;
          }
        }
      }
      return btn;
    }

    public void addDelegates()
    {
      // Add the delegates that receive pen data.
      this.tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

      this.tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
      this.tablet.onPenDataEncrypted += new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

      this.tablet.onPenDataTimeCountSequence += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
      this.tablet.onPenDataTimeCountSequenceEncrypted += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
    }

    public void removeDelegates()
		{
      this.tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

      this.tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
      this.tablet.onPenDataEncrypted -= new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

      this.tablet.onPenDataTimeCountSequence -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
      this.tablet.onPenDataTimeCountSequenceEncrypted -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);


      this.tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);
    }

    public void onGetReportException(wgssSTU.ITabletEventsException tabletEventsException)
    {
      try
      {
        tabletEventsException.getException();
      }
      catch (Exception e)
      {
        MessageBox.Show("Error: " + e.Message);
        this.tablet.disconnect();
        this.tablet = null;
        this.penData = null;
        this.penTimeData = null;
        this.signatureForm.Close();
      }
    }

    public void onPenDataTimeCountSequenceEncrypted(wgssSTU.IPenDataTimeCountSequenceEncrypted penTimeCountSequenceDataEncrypted) // Process incoming pen data
    {
      onPenDataTimeCountSequence(penTimeCountSequenceDataEncrypted);
    }

    private void onPenDataTimeCountSequence(wgssSTU.IPenDataTimeCountSequence penTimeData)
    {
      Point pt = this.TabletToScreen(penTimeData);
      int btn = buttonClicked(pt); // Check if a button was clicked

      bool isDown = (penTimeData.sw != 0);

      // This code uses a model of four states the pen can be in:
      // down or up, and whether this is the first sample of that state.

      if (isDown)
      {
        if (this.m_isDown == 0)
        {
          // transition to down
          if (btn > 0)
          {
            // We have put the pen down on a button.
            // Track the pen without inking on the client.

            this.m_isDown = btn;
          }
          else
          {
            // We have put the pen down somewhere else.
            // Treat it as part of the signature.

            this.m_isDown = -1;
          }
        }
        else
        {
          // already down, keep doing what we're doing!
        }

        // draw
        if (this.penTimeData.Count != 0 && this.m_isDown == -1)
        {
          // Draw a line from the previous down point to this down point.
          // This is the simplist thing you can do; a more sophisticated program
          // can perform higher quality rendering than this!

          wgssSTU.IPenDataTimeCountSequence prevPenData = this.penTimeData[this.penTimeData.Count - 1];
          PointF prev = this.TabletToClientTimed(prevPenData);

          Graphics gfx = GraphicsLib.GraphicFunctions.SetQualityGraphics(this.signatureForm);
          gfx.DrawLine(this.penInk, prev, this.TabletToClientTimed(penTimeData));
          gfx.Dispose();
        }

        // The pen is down, store it for use later.
        if (this.m_isDown == -1)
          this.penTimeData.Add(penTimeData);
      }
      else
      {
        if (this.m_isDown != 0)
        {
          // transition to up
          if (btn > 0)
          {
            // The pen is over a button

            if (btn == this.m_isDown)
            {
              // The pen was pressed down over the same button as is was lifted now. 
              // Consider that as a click!
              this.btns[btn - 1].PerformClick();
            }
          }
          this.m_isDown = 0;
        }
        else
        {
          // still up
        }

        // Add up data once we have collected some down data.
        if (this.penTimeData != null)
        {
          if (this.penTimeData.Count != 0)
            this.penTimeData.Add(penTimeData);
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
      Point pt = this.TabletToScreen(penData);

      int btn = 0; // will be +ve if the pen is over a button.
      {
        for (int i = 0; i < this.btns.Length; ++i)
        {
          if (this.btns[i].Bounds.Contains(pt))
          {
            btn = i + 1;
            break;
          }
        }
      }

      bool isDown = (penData.sw != 0);

      // This code uses a model of four states the pen can be in:
      // down or up, and whether this is the first sample of that state.

      if (isDown)
      {
        if (this.m_isDown == 0)
        {
          // transition to down
          if (btn > 0)
          {
            // We have put the pen down on a button.
            // Track the pen without inking on the client.

            this.m_isDown = btn;
          }
          else
          {
            // We have put the pen down somewhere else.
            // Treat it as part of the signature.

            this.m_isDown = -1;
          }
        }
        else
        {
          // already down, keep doing what we're doing!
        }

        // draw
        if (this.penData.Count != 0 && this.m_isDown == -1)
        {
          // Draw a line from the previous down point to this down point.
          // This is the simplist thing you can do; a more sophisticated program
          // can perform higher quality rendering than this!

          wgssSTU.IPenData prevPenData = this.penData[this.penData.Count - 1];

          PointF prev = this.TabletToClient(prevPenData);

          Graphics gfx = GraphicsLib.GraphicFunctions.SetQualityGraphics(this.signatureForm);
          gfx.DrawLine(this.penInk, prev, this.TabletToClient(penData));
          gfx.Dispose();
        }

        // The pen is down, store it for use later.
        if (this.m_isDown == -1)
          this.penData.Add(penData);
      }
      else
      {
        if (this.m_isDown != 0)
        {
          // transition to up
          if (btn > 0)
          {
            // The pen is over a button

            if (btn == this.m_isDown)
            {
              // The pen was pressed down over the same button as is was lifted now. 
              // Consider that as a click!
              this.btns[btn - 1].PerformClick();
            }
          }
          this.m_isDown = 0;
        }
        else
        {
          // still up
        }

        // Add up data once we have collected some down data.
        if (this.penData.Count != 0)
          this.penData.Add(penData);
      }
    }

    // Count no of pixels traversed between 2 co-ordinates
    private int countPixels(UInt16 lastX, UInt16 lastY, UInt16 currX, UInt16 currY)
    {
      int pixelDistance = 0;
      int tempCurrX = 0;
      int tempCurrY = 0;
      int tempLastX = 0;
      int tempLastY = 0;

      int tempX = 0;
      int tempY = 0;
      int tempZ = 0;
      string logText;

      // Possibilities are straight line along x co-ordinate, straight line along y co-ordinate or diagonal

      if (lastX == currX)
      {
        if (lastY != currY)
        {
          // Only y co-ordinate has changed
          if (currY > lastY)
          {
            pixelDistance = currY - lastY;
          }
          else
            pixelDistance = lastY - currY;
        }
        else
        {
          // No change in either co-ordinate
          pixelDistance = 0;
        }
      }
      else
      {
        // x has changed - what about y?
        if (lastY == currY)
        {
          // y has not changed so just use the difference in the x co-ordinate
          if (currX > lastX)
          {
            pixelDistance = currX - lastX;
          }
          else
            pixelDistance = lastX - currX;
        }
        else
        {
          // Both x and y have changed so we have to calculate the length of a diagonal line between the 2
          // Convert x and y to positive values and then use standard Pythagorean theorem calculation for the diagonal
          tempLastX = Math.Abs(lastX);
          tempCurrX = Math.Abs(currX);
          tempLastY = Math.Abs(lastY);
          tempCurrY = Math.Abs(currY);

          // We just want a positive difference value to calculate the diagonal distance (3rd side of the triangle)
          tempX = Math.Abs(tempLastX - tempCurrX);
          tempY = Math.Abs(tempLastY - tempCurrY);

          tempZ = (tempX * tempX) + (tempY * tempY);
          pixelDistance = (int)Math.Round(Math.Sqrt(tempZ), 0);
        }
      }
      logText = "Distance from " + lastX + "/" + lastY + " to " + currX + "/" + currY + " = " + pixelDistance;
      logFile(logText);

      return pixelDistance;
    }


    // Calculate average speed of the pen while creating the signature
    public String calcSpeed()
    {
      int i;
      int totalPixels = 0;
      UInt16 endTime, startTime;
      UInt16 lastX, lastY;
      decimal averageSpeed;
      decimal roundedSpeed;
      float kmPerHour;
      float metresPerHour;
      float metresPerSecond;
      float mmPerSecond;
      float penPointsPerSecond;
      float roundedPixelSpeed;
      String logText;

      // Store the start and end time of the pen data input
      startTime = this.penTimeData[0].timeCount;
      endTime = this.penTimeData[this.penTimeData.Count - 1].timeCount;

      lastX = this.penTimeData[0].x;
      lastY = this.penTimeData[0].y;

      // Count the number of pixels traversed by the pen by using the data in the array
      for (i = 0; i < this.penTimeData.Count; i++)
      {
        // If both co-ordinates are zero for either pair then no calculation can be made
        if ((lastX > 0 || lastY > 0) && (this.penTimeData[i].x > 0 || this.penTimeData[i].y > 0))
        {
          totalPixels += countPixels(lastX, lastY, this.penTimeData[i].x, this.penTimeData[i].y);
          lastX = this.penTimeData[i].x;
          lastY = this.penTimeData[i].y;
        }
      }
      averageSpeed = (decimal)totalPixels / (endTime - startTime);
      roundedSpeed = Math.Round(averageSpeed, 2);

      logText = "Time lapse from " + startTime + " to " + endTime + ": " + (endTime - startTime) + ". Pixels: " + totalPixels + ". Average pen points per ms: " + roundedSpeed.ToString();
      logFile(logText);

      // The 530 is 800 x 480 pixels but the # of pen points is 10800 x 6480 which is a ratio of 13.5 pen points to 1 pixel
      // Therefore the average speed in pixels is lower
      roundedPixelSpeed = (float)roundedSpeed;
      roundedPixelSpeed /= 13.5f;
      logText = "Average pixel speed per ms = " + roundedPixelSpeed;
      logFile(logText);

      // There are 100 pen points per millimeter (the pad measures 10.8 cm x 6.48cm) so we can now calculate metres per second and km per hour
      // First multiply the average no of pen points by 1000 to get the number per second
      penPointsPerSecond = (float)(roundedSpeed * 1000);

      // Divide by 100 to get the number of millimetres covered per second, then by 1000 to get the number of metres
      mmPerSecond = penPointsPerSecond / 100;
      metresPerSecond = mmPerSecond / 1000;

      // Multiple by 3600 to get the number of metres per hour
      metresPerHour = metresPerSecond * 3600;
      // Finally divide by 1000 to get the number of km per hour
      kmPerHour = metresPerHour / 1000;

      logText = "Metres per second: " + metresPerSecond.ToString() + ".  Km per hour: " + kmPerHour.ToString();
      logFile(logText);

      if (this.clickEventCount == 1)
      {
        logText = "Average pen points per ms: " + roundedSpeed.ToString() + "\r\nPixel speed per ms: " + roundedPixelSpeed + "\r\nMetres/sec: " + metresPerSecond.ToString() + "\r\nKm/h: " + kmPerHour.ToString();
      }
      return logText;
    }


    // Save the image in a local file
    public String saveImage(bool reverseImage)
    {
      String msgText = "";

      try
      {
        Bitmap bitmap = GraphicsLib.GraphicFunctions.GetSigImage(this.demoButtonsForm, this.signatureForm, this);

        if (reverseImage)
          bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

        string saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output.jpg";
        bitmap.Save(saveLocation, ImageFormat.Jpeg);
        msgText = "Image saved to " + saveLocation;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Exception: " + ex.Message);
        msgText = "Exception: " + ex.Message;
      }
      return msgText;
    }

    public void logFile(string logMessage)
    {
      StreamWriter log;

      if (!File.Exists("logfile.txt"))
      {
        log = new StreamWriter("logfile.txt");
      }
      else
      {
        log = File.AppendText("logfile.txt");
      }
      // Write to the file:
      log.WriteLine("Date/Time:" + DateTime.Now);
      log.WriteLine(logMessage);
      // Close the stream:
      log.Close();
    }
  }
}