/******************************************************* 

  SignatureForm.cs
  
  Display signature capture form on STU pad and on Windows
  with OK, Clear and Cancel buttons

  The inking on the pad is restricted to an area in the top-left corner.
  This sample only works with pads which support this i.e. 430, 530, 540.
  The 541 would support it in theory but there is no .NET support yet for the 541.

  The purpose of this sample is to illustrate how to restrict inking
  on the pad itself.
  
  Copyright (c) 2024 Wacom Ltd. All rights reserved.
  
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
using System.Security.Cryptography;
using wgssSTU;

namespace DemoButtons
{
	public partial class SignatureForm : Form
	{
		public int penDataType;
		public wgssSTU.ICapability m_capability;

		private wgssSTU.Tablet       m_tablet;
		private wgssSTU.IInformation m_information;

		// In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
		// Using an array of these makes it easy to add or remove buttons as desired.
		private delegate void ButtonClick();
		private Pen m_penInk;  // cached object.

		// The isDown flag is used like this:
		// 0 = up
		// +ve = down, pressed on button number
		// -1 = down, inking
		// -2 = down, ignoring
		private int m_isDown;

		public List<wgssSTU.IPenData> m_penData; // Array of data being stored. This can be subsequently used as desired.
		public List<wgssSTU.IPenDataTimeCountSequence> m_penTimeData; // Array of data being stored. This can be subsequently used as desired.

	  private Rendering.Button[] m_btns;

		private Bitmap m_bitmap; // The bitmap that we display on the screen.
		private wgssSTU.encodingMode m_encodingMode; // How we send the bitmap to the device.
		private byte[] m_bitmapData; // This is the flattened data of the bitmap that we send to the device.
		private int m_penDataOptionMode;  // The pen data option mode flag - basic or with time and sequence counts

		private bool m_useEncryption;
		public wgssSTU.Rectangle rectWritingArea;

		// As per the file comment, there are three coordinate systems to deal with.
		// To help understand, we have left the calculations in place rather than optimise them.

		DemoButtonsForm m_parent;   // give access to calling form

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

		public Point tabletToScreen(wgssSTU.IPenData penData)
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

		public static bool inkingInHandwritingArea(wgssSTU.IPenDataTimeCountSequence lastPoint, wgssSTU.IPenDataTimeCountSequence nextPoint, wgssSTU.ICapability capability, wgssSTU.Rectangle writingArea)
		{
			if (pointInHandwritingArea(lastPoint, capability, writingArea) && pointInHandwritingArea(nextPoint, capability, writingArea))
				return true;
			else
				return false;
		}

		private static bool pointInHandwritingArea(wgssSTU.IPenDataTimeCountSequence point, wgssSTU.ICapability capability, wgssSTU.Rectangle writingArea)
		{
			int xPos, yPos;
			double tempX, tempY;

			tempX = point.x * capability.screenWidth / capability.tabletMaxX;
			tempY = point.y * capability.screenHeight / capability.tabletMaxY;

			xPos = (int)Math.Round(tempX);
			yPos = (int)Math.Round(tempY);

			return (xPos >= writingArea.upperLeftXPixel && xPos <= writingArea.lowerRightXPixel && yPos >= writingArea.upperLeftYPixel && yPos <= writingArea.lowerRightYPixel);
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


		public void btnOk_Click(object sender, EventArgs e)
		{
			// You probably want to add additional processing here.
			penDataType = m_penDataOptionMode;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		public void btnCancel_Click(object sender, EventArgs e)
		{
			if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
			{
				this.m_penTimeData = null;
			}
			else
			{
				this.m_penData = null;
			}
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		public void btnClear_Click(object sender, EventArgs e)
		{
			if (m_penData.Count != 0 || m_penTimeData.Count != 0)
			{
				clearScreen();
			}
		}

		// Pass in the device you want to connect to!
		public SignatureForm(DemoButtonsForm parent, wgssSTU.IUsbDevice usbDevice, bool encryptionWanted)
		{
			rectWritingArea = new wgssSTU.Rectangle();

			int currentPenDataOptionMode;

			m_penDataOptionMode = -1;
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
				 print("Connected: " + m_information.modelName);

				 if (m_information.modelName == "STU-430")
				 {
					 rectWritingArea.upperLeftXPixel = 20;
					 rectWritingArea.upperLeftYPixel = 20;

					 rectWritingArea.lowerRightXPixel = 100;
					 rectWritingArea.lowerRightYPixel = 100;
				 }
				 else
				 {
				  	rectWritingArea.upperLeftXPixel = 100;
				    rectWritingArea.upperLeftYPixel = 100;

				    rectWritingArea.lowerRightXPixel = 300;
				    rectWritingArea.lowerRightYPixel = 300;
				 }

				 // First find out if the pad supports the pen data option mode (the 300 doesn't)
				 currentPenDataOptionMode = PenDataMode.getPenDataOptionMode(m_tablet);

				 // Set up the tablet to return time stamp with the pen data or just basic data
				 m_penDataOptionMode = PenDataMode.setPenDataOptionMode(currentPenDataOptionMode, m_tablet);
				
				 if (m_penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
         {
						m_penTimeData = new List<wgssSTU.IPenDataTimeCountSequence>();
				 }
				 else
         {
					 m_penData = new List<wgssSTU.IPenData>();
				 }
			}
			else
			{
				throw new Exception(ec.message);
			}

			this.SuspendLayout();
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

			// Set the size of the client window to be actual size based on the reported DPI of the monitor.

			Size clientSize = new Size((int)(m_capability.tabletMaxX / 2540F * 96F), (int)(m_capability.tabletMaxY / 2540F * 96F));
			this.ClientSize = clientSize;
			this.ResumeLayout();

			// Set up the OK, Clear and Cancel buttons
		  Rendering.ButtonPanel btnPanel = new Rendering.ButtonPanel(m_capability, usbDevice.idProduct, this);
			m_btns = btnPanel.buttonList;

			// Disable color if the STU-520 bulk driver isn't installed.
			// This isn't necessary, but uploading colour images with out the driver
			// is very slow.

			// Calculate the encodingMode that will be used to update the image
			EncodingSettings encodingSettings = new EncodingSettings(m_tablet, protocolHelper);
			m_encodingMode = encodingSettings.encodingMode;
		  bool useColor = encodingSettings.useColor;

			// Size the bitmap to the size of the LCD screen.
			// This application uses the same bitmap for both the LCD screen and client (window).
			// However, at high DPI, this bitmap will be stretched and it would be better to 
			// create individual bitmaps for screen and client at native resolutions.

			m_bitmap = Rendering.CreateScreenBitmap(m_capability, useColor, m_btns);

			// Finally, use this bitmap for the window background.
			this.BackgroundImage = m_bitmap;
			this.BackgroundImageLayout = ImageLayout.Stretch;

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

			addDelegates(); // Add delegates that receive pen data.

			m_tablet.setHandwritingDisplayArea(rectWritingArea);

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

		private void Form2_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Ensure that you correctly disconnect from the tablet, otherwise you are 
			// likely to get errors when wanting to connect a second time.
			if (m_tablet != null)
			{
				m_tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
				m_tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

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

	  public void onGetReportException(wgssSTU.ITabletEventsException tabletEventsException)
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

		public void addDelegates()
		{
			// Add the delegates that receive pen data.
			m_tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);

			m_tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
			m_tablet.onPenDataEncrypted += new wgssSTU.ITabletEvents2_onPenDataEncryptedEventHandler(onPenDataEncrypted);

			m_tablet.onPenDataTimeCountSequence += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(onPenDataTimeCountSequence);
			m_tablet.onPenDataTimeCountSequenceEncrypted += new wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEncryptedEventHandler(onPenDataTimeCountSequenceEncrypted);
		}

		private void onPenDataTimeCountSequenceEncrypted(wgssSTU.IPenDataTimeCountSequenceEncrypted penTimeCountSequenceDataEncrypted) // Process incoming pen data
		{
			onPenDataTimeCountSequence(penTimeCountSequenceDataEncrypted);
		}

		private void onPenDataTimeCountSequence(wgssSTU.IPenDataTimeCountSequence penTimeData)
		{
			Point pt = tabletToScreen(penTimeData);
			int btn = buttonClicked(pt); // Check if a button was clicked

			bool isDown = (penTimeData.sw != 0);

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
					// Only draw a line on the modal window if the corresponding inking on the pad is within the handwriting area
					// as delimited by the rectangle coordinates
					if (inkingInHandwritingArea(m_penTimeData[m_penTimeData.Count - 1], penTimeData, m_capability, rectWritingArea))
					{
						// Draw a line from the previous down point to this down point.
						// This is the simplest thing you can do; a more sophisticated program
						// can perform higher quality rendering than this!

						Graphics gfx = Rendering.SetQualityGraphics(this);
						wgssSTU.IPenDataTimeCountSequence prevPenData = m_penTimeData[m_penTimeData.Count - 1];
						PointF prev = tabletToClientTimed(prevPenData);

						gfx.DrawLine(m_penInk, prev, tabletToClientTimed(penTimeData));
						gfx.Dispose();
					}
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
						btn = i+1;
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
					if (inkingInHandwritingArea(m_penTimeData[m_penData.Count - 1], m_penTimeData[m_penData.Count], m_capability, rectWritingArea))
					{
						// Draw a line from the previous down point to this down point.
						// This is the simplist thing you can do; a more sophisticated program
						// can perform higher quality rendering than this!
						Graphics gfx = Rendering.SetQualityGraphics(this);
						wgssSTU.IPenData prevPenData = m_penData[m_penData.Count - 1];
						PointF prev = tabletToClient(prevPenData);

						gfx.DrawLine(m_penInk, prev, tabletToClient(penData));
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

		}

		private void Form2_MouseClick(object sender, MouseEventArgs e)
		{      
			// Enable the mouse to click on the simulated buttons that we have displayed.
			
			// Note that this can add some tricky logic into processing pen data
			// if the pen was down at the time of this click, especially if the pen was logically
			// also 'pressing' a button! This demo however ignores any that.

			Point pt = clientToScreen(e.Location);
			foreach (Rendering.Button btn in m_btns)
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
