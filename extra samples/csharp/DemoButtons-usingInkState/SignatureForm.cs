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
using System.Security.Cryptography;

namespace DemoButtons
{


    public partial class SignatureForm : Form
    {
        private wgssSTU.Tablet m_tablet;
        private wgssSTU.ICapability m_capability;
        private wgssSTU.IInformation m_information;
        private wgssSTU.IInkingState m_inkingState;

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

        private Button[] m_btns; // The array of buttons that we are emulating.

        private Bitmap m_bitmap; // This bitmap that we display on the screen.
        private wgssSTU.encodingMode m_encodingMode; // How we send the bitmap to the device.
        private byte[] m_bitmapData; // This is the flattened data of the bitmap that we send to the device.
        private int pointCount = 0;

        // As per the file comment, there are three coordinate systems to deal with.
        // To help understand, we have left the calculations in place rather than optimise them.

        DemoButtonsForm m_parent;   // give access to calling form

        private PointF tabletToClient(wgssSTU.IPenData penData)
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
            m_tablet.writeImage((byte)m_encodingMode, m_bitmapData);

            m_penData.Clear();
            m_isDown = 0;
            this.Invalidate();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            // Save the image to a file on disk
            SaveImage();
            this.Close();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            // You probably want to add additional processing here.
            this.m_penData.Clear();
            this.Close();
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            if (m_penData.Count != 0)
            {
                clearScreen();
            }
        }


        // Pass in the device you want to connect to!
        public SignatureForm(DemoButtonsForm parent, wgssSTU.IUsbDevice usbDevice)
        {
            m_parent = parent;

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

                m_inkingState = new wgssSTU.InkingState();
                m_inkingState.setInkThreshold(m_tablet.getInkThreshold());
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


            if (m_tablet.isSupported((byte)wgssSTU.ReportId.ReportId_EncryptionStatus))
            {
                MyEncryptionHandler2 e = new MyEncryptionHandler2();
                m_tablet.encryptionHandler2 = e;
                m_tablet.startCapture(0xc0ffee);
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
            float inkWidthMM = 0.5F;
            m_penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2F));
            m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;


            // Add the delagate that receives pen data.
            m_tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
            m_tablet.onPenDataEncrypted += new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);
            m_tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);


            // Initialize the screen
            clearScreen();

            // Enable the pen data on the screen (if not already)
            m_tablet.setInkingMode(0x01);
        }



        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Ensure that you correctly disconnect from the tablet, otherwise you are 
            // likely to get errors when wanting to connect a second time.
            if (m_tablet != null)
            {
                m_tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
                m_tablet.onPenDataEncrypted -= new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);
                m_tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);
                m_tablet.setInkingMode(0x00);
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
                this.Close();
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

                //print("Inkstate " + inkState);

                // draw
                if ((inkState == wgssSTU.InkState.InkState_Inking || inkState == wgssSTU.InkState.InkState_First) && m_isDown == -1)
                {
                    // Draw a line from the previous down point to this down point.
                    // This is the simplist thing you can do; a more sophisticated program
                    // can perform higher quality rendering than this!

                    //print("Drawing point " + ++pointCount);

                    Graphics gfx = this.CreateGraphics();
                    gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    wgssSTU.IPenData prevPenData = m_penData[m_penData.Count - 1];

                    PointF prev;
                    PointF curr;

                    if (inkState == wgssSTU.InkState.InkState_First)
                    {
                        prev = tabletToClient(penData);
                        curr = tabletToClient(penData);
                    }
                    else
                    {
                        prev = tabletToClient(prevPenData);
                        curr = tabletToClient(penData);
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
                        gfx.DrawLine(m_penInk, prev, curr);
                        gfx.Dispose();
                    }
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
                if (m_penData.Count != 0)
                    m_penData.Add(penData);
            }
        }




        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (m_penData.Count != 0)
            {
                // Redraw all the pen data up until now!

                Graphics gfx = e.Graphics;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


                wgssSTU.IInkingState inkingState = new wgssSTU.InkingState();
                inkingState.setInkThreshold(m_inkingState.getInkThreshold());

                PointF prev = tabletToClient(m_penData[0]);
                for (int i = 0; i < m_penData.Count; ++i)
                {
                    PointF curr = tabletToClient(m_penData[i]);

                    switch (inkingState.nextState(m_penData[i].pressure))
                    {
                        case wgssSTU.InkState.InkState_Inking:
                            gfx.DrawLine(m_penInk, prev, curr);
                            break;

                        case wgssSTU.InkState.InkState_Last:
                            if (!m_penData[i].rdy)
                                curr = prev;
                            gfx.DrawLine(m_penInk, prev, curr);
                            break;
                    }
                    prev = curr;
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
        private void print(string txt)
        {
            m_parent.print(txt);            // update parent form
        }


        public List<wgssSTU.IPenData> getPenData()
        {
            return m_penData;
        }

        public wgssSTU.ICapability getCapability()
        {
            return m_penData != null ? m_capability : null;
        }

        public wgssSTU.IInformation getInformation()
        {
            return m_penData != null ? m_information : null;
        }


        // Save the image in a local file
        private void SaveImage()
        {
            try
            {
                Bitmap bitmap = GetImage(new Rectangle(0, 0, m_capability.screenWidth, m_capability.screenHeight));
                string saveLocation = System.Environment.CurrentDirectory + "\\" + "signature_output.jpg";
                bitmap.Save(saveLocation, System.Drawing.Imaging.ImageFormat.Jpeg);
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
            Brush aBrush = (Brush)Brushes.Black;
            try
            {
                bitmap = new Bitmap(rect.Width, rect.Height);
                Graphics graphics = Graphics.FromImage(bitmap);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                wgssSTU.IInkingState inkingState = new wgssSTU.InkingState();
                inkingState.setInkThreshold(m_inkingState.getInkThreshold());

                brush = new SolidBrush(Color.White);
                graphics.FillRectangle(brush, 0, 0, rect.Width, rect.Height);
                PointF prev = new PointF();

                for (int i = 1; i < m_penData.Count; i++)
                {
                    PointF curr = tabletToClient(m_penData[i]);

                    if (i > 1)
                    {
                        prev = tabletToClient(m_penData[i - 1]);
                    }
                    
                    switch (inkingState.nextState(m_penData[i].pressure))
                    {
                        case wgssSTU.InkState.InkState_First:
                            graphics.FillRectangle(aBrush, curr.X, curr.Y, 2, 2);
                            break;

                        case wgssSTU.InkState.InkState_Inking:
                            graphics.DrawLine(m_penInk, prev, curr);
                            break;
                    }
                    prev = curr;
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

}
