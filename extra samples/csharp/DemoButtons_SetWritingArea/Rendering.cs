/******************************************************* 

  Rendering.cs
  
  This file contains utilities which are related to the
  rendering of the signature, either on the modal
  window or on the main DemoButtonsForm at the end
  
  Copyright (c) 2024 Wacom Ltd. All rights reserved.
  
********************************************************/
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoButtons
{
	 class Rendering
	 {
			public struct Button
			{
				 public Rectangle Bounds; // in Screen coordinates
				 public String Text;
				 public EventHandler Click;

				 public void PerformClick()
				 {
						Click(this, null);
				 }
			};

			public static Graphics SetQualityGraphics(Form thisForm)
			{
				 Graphics gfx = thisForm.CreateGraphics();
				 gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				 gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
				 gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				 gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				 return gfx;
			}

			public static Bitmap CreateScreenBitmap(wgssSTU.ICapability tabletDimensions, bool useColor, Button[] btns)
      {
				 Bitmap bitmap;

				 bitmap = new Bitmap(tabletDimensions.screenWidth, tabletDimensions.screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				 {
						Graphics gfx = Graphics.FromImage(bitmap);
						gfx.Clear(Color.White);

						// Uses pixels for units as DPI won't be accurate for tablet LCD.
						Font font = new Font(FontFamily.GenericSansSerif, btns[0].Bounds.Height / 2F, GraphicsUnit.Pixel);
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
						for (int i = 0; i < btns.Length; ++i)
						{
							 if (useColor)
							 {
									gfx.FillRectangle(Brushes.LightGray, btns[i].Bounds);
							 }
							 gfx.DrawRectangle(Pens.Black, btns[i].Bounds);
							 gfx.DrawString(btns[i].Text, font, Brushes.Black, btns[i].Bounds, sf);
						}

						gfx.Dispose();
						font.Dispose();
				 }
				 return bitmap;
			}

			public static Point ConvertTabletToScreen(wgssSTU.IPenData penData, wgssSTU.ICapability tabletCapability)
			{
				 // Screen means LCD screen of the tablet.
				 return Point.Round(new PointF((float)penData.x * tabletCapability.screenWidth / tabletCapability.tabletMaxX, (float)penData.y * tabletCapability.screenHeight / tabletCapability.tabletMaxY));
			}

			public static Bitmap GetSigImage(SignatureForm sigForm)
			{
				 Bitmap bitmap;
				 SolidBrush brush;
				 Point p1, p2;
				 Pen penInk;

				 Rectangle rect = new Rectangle(0, 0, sigForm.m_capability.screenWidth, sigForm.m_capability.screenHeight);

				 try
				 {
						bitmap = new Bitmap(rect.Width, rect.Height);
						Graphics gfx = Graphics.FromImage(bitmap);
						SizeF s = sigForm.AutoScaleDimensions;
						Single inkWidthMM = 1.0F;
						penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2.0F));
						penInk.StartCap = penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
						penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

						gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
						gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
						gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
						gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

						brush = new SolidBrush(Color.White);
						gfx.FillRectangle(brush, 0, 0, rect.Width, rect.Height);

						if (sigForm.penDataType == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
						{
							 for (int i = 1; i < sigForm.m_penTimeData.Count; ++i)
							 {
									p1 = ConvertTabletToScreen(sigForm.m_penTimeData[i - 1], sigForm.m_capability);
									p2 = ConvertTabletToScreen(sigForm.m_penTimeData[i], sigForm.m_capability);

									if (SignatureForm.inkingInHandwritingArea(sigForm.m_penTimeData[i - 1], sigForm.m_penTimeData[i], sigForm.m_capability, sigForm.rectWritingArea))
									{
										if (sigForm.m_penTimeData[i - 1].sw > 0 || sigForm.m_penTimeData[i].sw > 0)
										{
											gfx.DrawLine(penInk, p1, p2);
										}
									}
							 }
						}
						// This else section isn't actually relevant because the sample only works with
						// the 430, 530 and 540, all of which generate the TimeCountSequence, therefore
						// this section of code should never be called
						else
						{
							 for (int i = 1; i < sigForm.m_penData.Count; ++i)
							 {
									p1 = ConvertTabletToScreen(sigForm.m_penData[i - 1], sigForm.m_capability);
									p2 = ConvertTabletToScreen(sigForm.m_penData[i], sigForm.m_capability);

									if (sigForm.m_penData[i - 1].sw > 0 || sigForm.m_penData[i].sw > 0)
									{
										 gfx.DrawLine(penInk, p1, p2);
									}
							 }
						}
				 }
				 catch (Exception ex)
				 {
						//print("Exception: " + ex.Message);
						MessageBox.Show("Exception: " + ex.Message);
						bitmap = null;
				 }
				 return bitmap;
			}

			public class ButtonPanel
			{
				 public Button[] buttonList { get; set; }

				 public ButtonPanel(wgssSTU.ICapability tabletDimensions, ushort productID, SignatureForm sigForm)
				 {
						buttonList = new Button[3];

						if (productID != 0x00a2)
						{
							 // Place the buttons across the bottom of the screen.

							 int w2 = tabletDimensions.screenWidth / 3;
							 int w3 = tabletDimensions.screenWidth / 3;
							 int w1 = tabletDimensions.screenWidth - w2 - w3;
							 int y = tabletDimensions.screenHeight * 6 / 7;
							 int h = tabletDimensions.screenHeight - y;

							 buttonList[0].Bounds = new Rectangle(0, y, w1, h);
							 buttonList[1].Bounds = new Rectangle(w1, y, w2, h);
							 buttonList[2].Bounds = new Rectangle(w1 + w2, y, w3, h);
						}
						else
						{
							 // The STU-300 is very shallow, so it is better to utilise
							 // the buttons to the side of the display instead.

							 int x = tabletDimensions.screenWidth * 3 / 4;
							 int w = tabletDimensions.screenWidth - x;

							 int h2 = tabletDimensions.screenHeight / 3;
							 int h3 = tabletDimensions.screenHeight / 3;
							 int h1 = tabletDimensions.screenHeight - h2 - h3;

							 buttonList[0].Bounds = new Rectangle(x, 0, w, h1);
							 buttonList[1].Bounds = new Rectangle(x, h1, w, h2);
							 buttonList[2].Bounds = new Rectangle(x, h1 + h2, w, h3);
						}
						buttonList[0].Text = "OK";
						buttonList[1].Text = "Clear";
						buttonList[2].Text = "Cancel";
						buttonList[0].Click = new EventHandler(sigForm.btnOk_Click);
						buttonList[1].Click = new EventHandler(sigForm.btnClear_Click);
						buttonList[2].Click = new EventHandler(sigForm.btnCancel_Click);
				 }
			}
	 }
}