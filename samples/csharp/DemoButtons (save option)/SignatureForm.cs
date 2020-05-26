/*
 SignatureForm.cs

 Allows user to input a signature on an STU and reproduces it on a Window on the PC
 Signature can also be saved to disk as a JPEG image
 
 Copyright (c) 2018 Wacom GmbH. All rights reserved.

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
using System.Security.Cryptography;

namespace DemoButtons
{

   class MyEncryptionHandler2 : wgssSTU.ITabletEncryptionHandler2
   {
      RSACryptoServiceProvider rsaProvider;
      RSAParameters rsaParameters;
      RijndaelManaged aes; // for .NET 2 compatibility.

      public void reset()
      {
         if (rsaProvider != null)
            rsaProvider.Clear();
         rsaProvider = null;
         if (aes != null)
            aes.Clear();
         aes = null;
      }


      public void clearKeys()
      {
         if (aes != null)
            aes.Clear();
         aes = null;
      }


      public byte getAsymmetricKeyType()
      {
         return (byte)wgssSTU.asymmetricKeyType.AsymmetricKeyType_RSA2048;
      }


      public byte getAsymmetricPaddingType()
      {
         return (byte)wgssSTU.asymmetricPaddingType.AsymmetricPaddingType_OAEP;
      }


      public byte getSymmetricKeyType()
      {
         return (byte)wgssSTU.symmetricKeyType.SymmetricKeyType_AES256;
      }

      private void create()
      {
         rsaProvider = new RSACryptoServiceProvider(2048, new CspParameters());
         rsaParameters = rsaProvider.ExportParameters(false);
      }

      public Array generatePublicKey()
      {
         if (rsaProvider == null)
            create();

         return rsaParameters.Modulus;
      }


      public Array getPublicExponent()
      {
         if (rsaProvider == null)
            create();

         return rsaParameters.Exponent;
      }


      public void computeSessionKey(Array data)
      {
         byte[] arr = rsaProvider.Decrypt((byte[])data, true);
         byte[] key = new byte[32];
         Array.Copy(arr, arr.Length - 32, key, 0, 32);

         aes = new RijndaelManaged();
         aes.Key = key;
         aes.IV = new byte[16];
         aes.Mode = CipherMode.CBC;
         aes.Padding = PaddingMode.None;
      }


      public Array decrypt(Array data)
      {
         var dec = aes.CreateDecryptor();
         byte[] arr = new byte[data.Length];
         dec.TransformBlock((byte[])data, 0, data.Length, arr, 0);
         return arr;
      }
   }
   public partial class SignatureForm : Form
   {
      public int penDataType;
      private wgssSTU.Tablet m_tablet;
      private wgssSTU.ICapability m_capability;
      private wgssSTU.IInformation m_information;

      // In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
      // Using an array of these makes it easy to add or remove buttons as desired.
      private delegate void ButtonClick();
      private struct Button
      {
         public Rectangle Bounds; // in Screen coordinates
         public String Text;
         public EventHandler Click;

         public void PerformClick()
         {
            Click(this, null);
         }
      };

      private Pen m_penInk;  // cached object.

      // The isDown flag is used like this:
      // 0 = up
      // +ve = down, pressed on button number
      // -1 = down, inking
      // -2 = down, ignoring
      private int m_isDown;

      private List<wgssSTU.IPenData> m_penData; // Array of data being stored. This can be subsequently used as desired.
      private List<wgssSTU.IPenDataTimeCountSequence> m_penTimeData; // Array of data being stored. This can be subsequently used as desired.

      private Button[] m_btns; // The array of buttons that we are emulating.

      private Bitmap m_bitmap; // This bitmap that we display on the screen.
      private wgssSTU.encodingMode m_encodingMode; // How we send the bitmap to the device.
      private byte[] m_bitmapData; // This is the flattened data of the bitmap that we send to the device.
      private int m_penDataOptionMode;  // The pen data option mode flag - basic or with time and sequence counts

      private bool m_useEncryption; 

      // As per the file comment, there are three coordinate systems to deal with.
      // To help understand, we have left the calculations in place rather than optimise them.


      private PointF tabletToClient(wgssSTU.IPenData penData)
      {
         // Client means the Windows Form coordinates.
         return new PointF((float)penData.x * this.ClientSize.Width / m_capability.tabletMaxX, (float)penData.y * this.ClientSize.Height / m_capability.tabletMaxY);
      }

      private PointF tabletToClientTimed(wgssSTU.IPenDataTimeCountSequence penData)
      {
         // Client means the Windows Form coordinates.
         return new PointF((float)penData.x * this.ClientSize.Width / m_capability.tabletMaxX, (float)penData.y * this.ClientSize.Height / m_capability.tabletMaxY);
      }


      private Point tabletToScreen(wgssSTU.IPenData penData)
      {
         // Screen means LCD screen of the tablet.
         return Point.Round(new PointF((float)penData.x * m_capability.screenWidth / m_capability.tabletMaxX, (float)penData.y * m_capability.screenHeight / m_capability.tabletMaxY));
      }



      private Point clientToScreen(Point pt)
      {
         // client (window) coordinates to LCD screen coordinates. 
         // This is needed for converting mouse coordinates into LCD bitmap coordinates as that's
         // what this application uses as the coordinate space for buttons.
         return Point.Round(new PointF((float)pt.X * m_capability.screenWidth / this.ClientSize.Width, (float)pt.Y * m_capability.screenHeight / this.ClientSize.Height));
      }


      private void clearScreen()
      {
         // note: There is no need to clear the tablet screen prior to writing an image.

         if (m_useEncryption)
            m_tablet.endCapture();

         m_tablet.writeImage((byte)m_encodingMode, m_bitmapData);

         if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
         {
            m_penTimeData.Clear();
         }
         else
         {
            m_penData.Clear();
         }

         if (m_useEncryption)
            m_tablet.startCapture(0xc0ffee);

         m_isDown = 0;
         this.Invalidate();
      }



      private void btnOk_Click(object sender, EventArgs e)
      {
         SaveImage();
         penDataType = m_penDataOptionMode;
         this.Close();
      }


      private void btnCancel_Click(object sender, EventArgs e)
      {
         if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
         {
            this.m_penTimeData = null;
         }
         else
         {
            this.m_penData = null;
         }

         this.Close();
      }


      private void btnClear_Click(object sender, EventArgs e)
      {
         if (m_penData.Count != 0 || m_penTimeData.Count != 0)
         {
            clearScreen();
         }
      }

      private Graphics setQualityGraphics(Form thisForm)
      {
         Graphics gfx = thisForm.CreateGraphics();
         gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
         gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
         gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
         gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

         return gfx;
      }


      // Pass in the device you want to connect to!
      public SignatureForm(wgssSTU.IUsbDevice usbDevice, bool encryptionWanted)
      {
         int currentPenDataOptionMode;

         m_penDataOptionMode = -1;

         // This is a DPI aware application, so ensure you understand how .NET client coordinates work.
         // Testing using a Windows installation set to a high DPI is recommended to understand how
         // values get scaled or not.

         this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

         InitializeComponent();

         m_penData = new List<wgssSTU.IPenData>();

         m_tablet = new wgssSTU.Tablet();
         wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();

         // A more sophisticated applications should cycle for a few times as the connection may only be
         // temporarily unavailable for a second or so. 
         // For example, if a background process such as Wacom STU Display
         // is running, this periodically updates a slideshow of images to the device.

         wgssSTU.IErrorCode ec = m_tablet.usbConnect(usbDevice, true);
         if (ec.value == 0)
         {
            m_capability = m_tablet.getCapability();
            m_information = m_tablet.getInformation();

            // First find out if the pad supports the pen data option mode (the 300 doesn't)
            currentPenDataOptionMode = getPenDataOptionMode();

            // Set up the tablet to return time stamp with the pen data or just basic data
            setPenDataOptionMode(currentPenDataOptionMode);
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

         Size clientSize = new Size((int)(m_capability.tabletMaxX / 2540F * 96F), (int)(m_capability.tabletMaxY / 2540F * 96F));
         this.ClientSize = clientSize;
         this.ResumeLayout();

         m_btns = new Button[3];
         if (usbDevice.idProduct != 0x00a2)
         {
            // Place the buttons across the bottom of the screen.

            int w2 = m_capability.screenWidth / 3;
            int w3 = m_capability.screenWidth / 3;
            int w1 = m_capability.screenWidth - w2 - w3;
            int y = m_capability.screenHeight * 6 / 7;
            int h = m_capability.screenHeight - y;

            m_btns[0].Bounds = new Rectangle(0, y, w1, h);
            m_btns[1].Bounds = new Rectangle(w1, y, w2, h);
            m_btns[2].Bounds = new Rectangle(w1 + w2, y, w3, h);
         }
         else
         {
            // The STU-300 is very shallow, so it is better to utilise
            // the buttons to the side of the display instead.

            int x = m_capability.screenWidth * 3 / 4;
            int w = m_capability.screenWidth - x;

            int h2 = m_capability.screenHeight / 3;
            int h3 = m_capability.screenHeight / 3;
            int h1 = m_capability.screenHeight - h2 - h3;

            m_btns[0].Bounds = new Rectangle(x, 0, w, h1);
            m_btns[1].Bounds = new Rectangle(x, h1, w, h2);
            m_btns[2].Bounds = new Rectangle(x, h1 + h2, w, h3);
         }
         m_btns[0].Text = "OK";
         m_btns[1].Text = "Clear";
         m_btns[2].Text = "Cancel";
         m_btns[0].Click = new EventHandler(btnOk_Click);
         m_btns[1].Click = new EventHandler(btnClear_Click);
         m_btns[2].Click = new EventHandler(btnCancel_Click);


         // Disable color if the STU-520 bulk driver isn't installed.
         // This isn't necessary, but uploading colour images with out the driver
         // is very slow.

         // Calculate the encodingMode that will be used to update the image
         ushort idP = m_tablet.getProductId();
         wgssSTU.encodingFlag encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(idP, 0);
         bool useColor = false;
         if ((encodingFlag & (wgssSTU.encodingFlag.EncodingFlag_16bit | wgssSTU.encodingFlag.EncodingFlag_24bit)) != 0)
         {
            if (m_tablet.supportsWrite())
               useColor = true;
         }
         if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_24bit) != 0)
         {
            m_encodingMode = m_tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_24bit_Bulk : wgssSTU.encodingMode.EncodingMode_24bit;
         }
         else if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_16bit) != 0)
         {
            m_encodingMode = m_tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_16bit_Bulk : wgssSTU.encodingMode.EncodingMode_16bit;
         }
         else
         {
            // assumes 1bit is available
            m_encodingMode = wgssSTU.encodingMode.EncodingMode_1bit;
         }       

         // Size the bitmap to the size of the LCD screen.
         // This application uses the same bitmap for both the screen and client (window).
         // However, at high DPI, this bitmap will be stretch and it would be better to 
         // create individual bitmaps for screen and client at native resolutions.
         m_bitmap = new Bitmap(m_capability.screenWidth, m_capability.screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         {
            Graphics gfx = Graphics.FromImage(m_bitmap);
            gfx.Clear(Color.White);

            // Uses pixels for units as DPI won't be accurate for tablet LCD.
            Font font = new Font(FontFamily.GenericSansSerif, m_btns[0].Bounds.Height / 2F, GraphicsUnit.Pixel);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (useColor)
            {
               gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
               // Anti-aliasing should be turned off for monochrome devices.
               gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            }

            // Draw the buttons
            for (int i = 0; i < m_btns.Length; ++i)
            {
               if (useColor)
               {
                  gfx.FillRectangle(Brushes.LightGray, m_btns[i].Bounds);
               }
               gfx.DrawRectangle(Pens.Black, m_btns[i].Bounds);
               gfx.DrawString(m_btns[i].Text, font, Brushes.Black, m_btns[i].Bounds, sf);
            }

            gfx.Dispose();
            font.Dispose();

            // Finally, use this bitmap for the window background.
            this.BackgroundImage = m_bitmap;
            this.BackgroundImageLayout = ImageLayout.Stretch;
         }

         // Now the bitmap has been created, it needs to be converted to device-native
         // format.
         {

            // Unfortunately it is not possible for the native COM component to
            // understand .NET bitmaps. We have therefore convert the .NET bitmap
            // into a memory blob that will be understood by COM.

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            m_bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            m_bitmapData = (byte[])protocolHelper.resizeAndFlatten(stream.ToArray(), 0, 0, (uint)m_bitmap.Width, (uint)m_bitmap.Height, m_capability.screenWidth, m_capability.screenHeight, (byte)m_encodingMode, wgssSTU.Scale.Scale_Fit, 0, 0);
            protocolHelper = null;
            stream.Dispose();
         }

         // If you wish to further optimize image transfer, you can compress the image using 
         // the Zlib algorithm.

         bool useZlibCompression = false;
         if (!useColor && useZlibCompression)
         {
            // m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
            m_encodingMode |= wgssSTU.encodingMode.EncodingMode_Zlib;
         }

         // Calculate the size and cache the inking pen.

         SizeF s = this.AutoScaleDimensions;
         float inkWidthMM = 0.7F;
         m_penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2F));
         m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
         m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

         addDelegates();

         // Initialize the screen
         clearScreen();

         if (encryptionWanted)
         {
            if (m_tablet.isSupported((byte)wgssSTU.ReportId.ReportId_EncryptionStatus))
            {
               MyEncryptionHandler2 e = new MyEncryptionHandler2();
               m_tablet.encryptionHandler2 = e;
               m_useEncryption = true;
            }

            if (m_useEncryption)
               m_tablet.startCapture(0xc0ffee);

         }

         // Enable the pen data on the screen (if not already)
         m_tablet.setInkingMode(0x01);
      }

      private int getPenDataOptionMode()
      {
         int penDataOptionMode;

         try
         {
            penDataOptionMode = m_tablet.getPenDataOptionMode();
         }
         catch (Exception optionModeException)
         {
            //m_parent.print("Tablet doesn't support getPenDataOptionMode");
            penDataOptionMode = -1;
         }
         //m_parent.print("Pen data option mode: " + m_penDataOptionMode);

         return penDataOptionMode;
      }

      private void setPenDataOptionMode(int currentPenDataOptionMode)
      {
         // If the current option mode is TimeCount then this is a 520 so we must reset the mode
         // to basic data only as there is no handler for TimeCount

         //m_parent.print("current mode: " + currentPenDataOptionMode);

         switch (currentPenDataOptionMode)
         {
            case -1:
               // THis must be the 300 which doesn't support getPenDataOptionMode at all so only basic data
               m_penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
               break;

            case (int)PenDataOptionMode.PenDataOptionMode_None:
               // If the current option mode is "none" then it could be any pad so try setting the full option
               // and if it fails or ends up as TimeCount then set it to none
               try
               {
                  m_tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_TimeCountSequence);
                  m_penDataOptionMode = m_tablet.getPenDataOptionMode();
                  if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCount)
                  {
                     m_tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
                     m_penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
                  }
                  else
                  {
                     m_penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence;
                  }
               }
               catch (Exception ex)
               {
                  // THis shouldn't happen but just in case...
                  //m_parent.print("Using basic pen data");
                  m_penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
               }
               break;

            case (int)PenDataOptionMode.PenDataOptionMode_TimeCount:
               m_tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
               m_penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
               break;

            case (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence:
               // If the current mode is timecountsequence then leave it at that
               m_penDataOptionMode = currentPenDataOptionMode;
               break;
         }

         switch ((int)m_penDataOptionMode)
         {
            case (int)PenDataOptionMode.PenDataOptionMode_None:
               m_penData = new List<wgssSTU.IPenData>();
               //m_parent.print("None");
               break;
            case (int)PenDataOptionMode.PenDataOptionMode_TimeCount:
               m_penData = new List<wgssSTU.IPenData>();
               //m_parent.print("Time count");
               break;
            case (int)PenDataOptionMode.PenDataOptionMode_SequenceNumber:
               m_penData = new List<wgssSTU.IPenData>();
               //m_parent.print("Seq number");
               break;
            case (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence:
               m_penTimeData = new List<wgssSTU.IPenDataTimeCountSequence>();
               //m_parent.print("Time count + seq");
               break;
            default:
               m_penData = new List<wgssSTU.IPenData>();
               break;
         }
      }

      private void addDelegates()
      {
         // Add the delegates that receive pen data.
         m_tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

         m_tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
         m_tablet.onPenDataEncrypted += new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

         m_tablet.onPenDataTimeCountSequence += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
         m_tablet.onPenDataTimeCountSequenceEncrypted += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
      }

      private void Form2_FormClosed(object sender, FormClosedEventArgs e)
      {
         // Ensure that you correctly disconnect from the tablet, otherwise you are 
         // likely to get errors when wanting to connect a second time.
         if (m_tablet != null)
         {
            m_tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

            m_tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
            m_tablet.onPenDataEncrypted -= new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

            m_tablet.onPenDataTimeCountSequence -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
            m_tablet.onPenDataTimeCountSequenceEncrypted -= new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
           
            
            m_tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

            m_tablet.setInkingMode(0x00);

            // We have to disable encryption before sending any image to the pad (even just to clear it)
            if (m_useEncryption)
               m_tablet.endCapture();

            m_tablet.setClearScreen();
            m_tablet.disconnect();
         }

         m_penInk.Dispose();
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
            m_tablet.disconnect();
            m_tablet = null;
            m_penData = null;
            m_penTimeData = null;
            this.Close();
         }
      }

      private void onPenDataTimeCountSequenceEncrypted(wgssSTU.IPenDataTimeCountSequenceEncrypted penTimeCountSequenceDataEncrypted) // Process incoming pen data
      {
        onPenDataTimeCountSequence(penTimeCountSequenceDataEncrypted);
        //onPenDataTimeCountSequence(penTimeCountSequenceDataEncrypted);
      }

      private void onPenDataTimeCountSequence(wgssSTU.IPenDataTimeCountSequence penTimeData)
      {
         UInt16 penSequence;
         UInt16 penTimeStamp;
         UInt16 penPressure;
         UInt16 x;
         UInt16 y;

         penPressure = penTimeData.pressure;
         penTimeStamp = penTimeData.timeCount;
         penSequence = penTimeData.sequence;
         x = penTimeData.x;
         y = penTimeData.y;

         Point pt = tabletToScreen(penTimeData);
         int btn = buttonClicked(pt); // Check if a button was clicked

         bool isDown = (penTimeData.sw != 0);

         //m_parent.print("Handling pen data timed");

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
            if (m_penTimeData.Count != 0 && m_isDown == -1)
            {
               // Draw a line from the previous down point to this down point.
               // This is the simplist thing you can do; a more sophisticated program
               // can perform higher quality rendering than this!

               Graphics gfx = setQualityGraphics(this);
               wgssSTU.IPenDataTimeCountSequence prevPenData = m_penTimeData[m_penTimeData.Count - 1];
               PointF prev = tabletToClient(prevPenData);

               gfx.DrawLine(m_penInk, prev, tabletToClientTimed(penTimeData));
               gfx.Dispose();
            }

            // The pen is down, store it for use later.
            if (m_isDown == -1)
               m_penTimeData.Add(penTimeData);
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
                     m_btns[btn - 1].PerformClick();
                  }
               }
               m_isDown = 0;
            }
            else
            {
               // still up
            }

            // Add up data once we have collected some down data.
            if (m_penTimeData != null)
            {
               if (m_penTimeData.Count != 0)
                  m_penTimeData.Add(penTimeData);
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
         Point pt = tabletToScreen(penData);

         int btn = 0; // will be +ve if the pen is over a button.
         {
            for (int i = 0; i < m_btns.Length; ++i)
            {
               if (m_btns[i].Bounds.Contains(pt))
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
            if (m_penData.Count != 0 && m_isDown == -1)
            {
               // Draw a line from the previous down point to this down point.
               // This is the simplist thing you can do; a more sophisticated program
               // can perform higher quality rendering than this!

               Graphics gfx = this.CreateGraphics();
               gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
               gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
               gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
               gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

               wgssSTU.IPenData prevPenData = m_penData[m_penData.Count - 1];

               PointF prev = tabletToClient(prevPenData);

               gfx.DrawLine(m_penInk, prev, tabletToClient(penData));
               gfx.Dispose();
            }

            // The pen is down, store it for use later.
            if (m_isDown == -1)
               m_penData.Add(penData);
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
                     m_btns[btn - 1].PerformClick();
                  }
               }
               m_isDown = 0;
            }
            else
            {
               // still up
            }

            // Add up data once we have collected some down data.
            if (m_penData != null)
            {
               if (m_penData.Count != 0)
                  m_penData.Add(penData);
            }
         }
      }

      private int buttonClicked(Point pt)
      {
         int btn = 0; // will be +ve if the pen is over a button.
         {
            for (int i = 0; i < m_btns.Length; ++i)
            {
               if (m_btns[i].Bounds.Contains(pt))
               {
                  btn = i + 1;
                  //m_parent.print("Pressed button " + btn);
                  break;
               }
            }
         }
         return btn;
      }

      private void Form2_Paint(object sender, PaintEventArgs e)
      {
         // Call the appropriate routine to render the pen strokes on the client window
         // depending on what type of pen data is being received

         if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
         {
            renderPenTimeData(e);
         }
         else
         {
            renderPenData(e);
         }
      }

      private void renderPenData(PaintEventArgs e)
      {
         if (m_penData.Count != 0)
         {
            // Redraw all the pen data up until now!

            Graphics gfx = e.Graphics;
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            bool isDown = false;
            PointF prev = new PointF();
            for (int i = 0; i < m_penData.Count; ++i)
            {
               if (m_penData[i].sw != 0)
               {
                  if (!isDown)
                  {
                     isDown = true;
                     prev = tabletToClient(m_penData[i]);
                  }
                  else
                  {
                     PointF curr = tabletToClient(m_penData[i]);
                     gfx.DrawLine(m_penInk, prev, curr);
                     prev = curr;
                  }
               }
               else
               {
                  if (isDown)
                  {
                     isDown = false;
                  }
               }
            }
         }
      }
      private void renderPenTimeData(PaintEventArgs e)
      {
         if (m_penTimeData.Count != 0)
         {
            // Redraw all the pen data up until now!

            Graphics gfx = e.Graphics;
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            bool isDown = false;
            PointF prev = new PointF();
            for (int i = 0; i < m_penTimeData.Count; ++i)
            {
               if (m_penTimeData[i].sw != 0)
               {
                  if (!isDown)
                  {
                     isDown = true;
                     prev = tabletToClientTimed(m_penTimeData[i]);
                  }
                  else
                  {
                     PointF curr = tabletToClientTimed(m_penTimeData[i]);
                     gfx.DrawLine(m_penInk, prev, curr);
                     prev = curr;
                  }
               }
               else
               {
                  if (isDown)
                  {
                     isDown = false;
                  }
               }
            }
         }
      }

      private void Form2_Paint_old(object sender, PaintEventArgs e)
    {
      if (m_penData.Count != 0)
      {
        // Redraw all the pen data up until now!

        Graphics gfx = e.Graphics;
        gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        bool isDown = false;
        PointF prev = new PointF();
        for (int i = 0; i < m_penData.Count; ++i)
        {
          if (m_penData[i].sw != 0)
          {
            if (!isDown)
            {
              isDown = true;
              prev = tabletToClient(m_penData[i]);
            }
            else
            {
              PointF curr = tabletToClient(m_penData[i]);
              gfx.DrawLine(m_penInk, prev, curr);
              prev = curr;
            }
          }
          else
          {
            if (isDown)
            {
              isDown = false;
            }
          }
        }
      }
          
    }

    private void Form2_MouseClick(object sender, MouseEventArgs e)
    {      
      // Enable the mouse to click on the simulated buttons that we have displayed.
      
      // Note that this can add some tricky logic into processing pen data
      // if the pen was down at the time of this click, especially if the pen was logically
      // also 'pressing' a button! This demo however ignores any that.

      Point pt = clientToScreen(e.Location);
      foreach (Button btn in m_btns)
      {
        if (btn.Bounds.Contains(pt))
        {
          btn.PerformClick();
          break;
        }
      }
    }



    public List<wgssSTU.IPenData> getPenData()
    {
       return m_penData;
    }

    public List<wgssSTU.IPenDataTimeCountSequence> getPenTimeData()
    {
          return m_penTimeData;
    }

    public wgssSTU.ICapability getCapability()
    {
         if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
            return m_penTimeData != null ? m_capability : null;
         else
            return m_penData != null ? m_capability : null;
    }

    public wgssSTU.IInformation getInformation()
    {
         if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
            return m_penTimeData != null ? m_information : null;
         else
            return m_penData != null ? m_information : null;
    }
    // Save the image in a local file
    private void SaveImage()
    {
        try
        {
            Bitmap bitmap = GetImage(new Rectangle(0, 0, m_capability.screenWidth, m_capability.screenHeight));
            string saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output.jpg";
            bitmap.Save(saveLocation, ImageFormat.Jpeg);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Exception: " + ex.Message);
        }
    }

    // Draw an image with the existed points.
    public Bitmap GetImage(Rectangle rect)
    {
        Bitmap retVal = null;
        Bitmap bitmap = null;
        SolidBrush brush = null;
        try
        {
            bitmap = new Bitmap(rect.Width, rect.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            brush = new SolidBrush(Color.White);
            graphics.FillRectangle(brush, 0, 0, rect.Width, rect.Height);

            if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
            {
               for (int i = 1; i < m_penTimeData.Count; i++)
               {
                  PointF p1 = tabletToScreen(m_penTimeData[i - 1]);
                  PointF p2 = tabletToScreen(m_penTimeData[i]);

                  if (m_penTimeData[i - 1].sw > 0 || m_penTimeData[i].sw > 0)
                  {
                     graphics.DrawLine(m_penInk, p1, p2);
                  }
               }
            }
            else
            {
               for (int i = 1; i < m_penData.Count; i++)
               {
                  PointF p1 = tabletToScreen(m_penData[i - 1]);
                  PointF p2 = tabletToScreen(m_penData[i]);

                  if (m_penData[i - 1].sw > 0 || m_penData[i].sw > 0)
                  {
                     graphics.DrawLine(m_penInk, p1, p2);
                  }
               }
            }

            retVal = bitmap;
            bitmap = null;
        }
        finally
        {
            if (brush != null)
                brush.Dispose();
            if (bitmap != null)
                bitmap.Dispose();
        }
        return retVal;
    }
      
  }
}
