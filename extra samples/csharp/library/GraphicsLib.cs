/******************************************************* 

  GraphicsLib.cs
  
  Library of functions related to drawing or generating
  graphics
  
  Copyright (c) 2023 Wacom Ltd. All rights reserved.
  
********************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GraphicsLib
{
  public class Buttons
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
    public Button[] btns;

    public Buttons(Int32 prodID, wgssSTU.ICapability capability, bool reverseImage, EventHandler btnOk_Click, EventHandler btnClear_Click, EventHandler btnCancel_Click)
    {
      btns = new Button[3];

      if (prodID != 0x00a2)
      {
        // Place the buttons across the bottom of the screen.

        int w2 = capability.screenWidth / 3;
        int w3 = capability.screenWidth / 3;
        int w1 = capability.screenWidth - w2 - w3;
        int y = capability.screenHeight * 6 / 7;
        int h = capability.screenHeight - y;

        btns[0].Bounds = new Rectangle(0, y, w1, h);
        btns[1].Bounds = new Rectangle(w1, y, w2, h);
        btns[2].Bounds = new Rectangle(w1 + w2, y, w3, h);
      }
      else
      {
        // The STU-300 is very shallow, so it is better to utilise
        // the buttons to the side of the display instead.

        int x = capability.screenWidth * 3 / 4;
        int w = capability.screenWidth - x;

        int h2 = capability.screenHeight / 3;
        int h3 = capability.screenHeight / 3;
        int h1 = capability.screenHeight - h2 - h3;

        btns[0].Bounds = new Rectangle(x, 0, w, h1);
        btns[1].Bounds = new Rectangle(x, h1, w, h2);
        btns[2].Bounds = new Rectangle(x, h1 + h2, w, h3);
      }
      btns[0].Text = "OK";
      btns[1].Text = "Clear";
      btns[2].Text = "Cancel";

      if (reverseImage)
      {
        btns[0].Click = new EventHandler(btnCancel_Click);
        btns[1].Click = new EventHandler(btnClear_Click);
        btns[2].Click = new EventHandler(btnOk_Click);
      }
      else
      {
        btns[0].Click = new EventHandler(btnOk_Click);
        btns[1].Click = new EventHandler(btnClear_Click);
        btns[2].Click = new EventHandler(btnCancel_Click);
      }
    }
  }

  public class GraphicFunctions
  {
    public static Graphics SetQualityGraphics(Form thisForm)
    {
      Graphics gfx = thisForm.CreateGraphics();
      gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
      gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
      gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
      gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

      return gfx;
    }

    public static Bitmap CreateBitmap(wgssSTU.ICapability capability, bool useColor, Buttons.Button[] btns)
    {
      Bitmap bitmap;

      // Size the bitmap to the size of the LCD screen.
      // This application uses the same bitmap for both the screen and client (window).
      // However, at high DPI, this bitmap will be stretch and it would be better to 
      // create individual bitmaps for screen and client at native resolutions.
      bitmap = new Bitmap(capability.screenWidth, capability.screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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

    public static void renderPenData(Form sigForm, TabletLib.STU_Tablet stu_Tablet)
    {
      if (stu_Tablet.penData.Count != 0)
      {
        // Redraw all the pen data up until now!

        Graphics gfx = GraphicFunctions.SetQualityGraphics(sigForm);
        bool isDown = false;
        PointF prev = new PointF();
        for (int i = 0; i < stu_Tablet.penData.Count; ++i)
        {
          if (stu_Tablet.penData[i].sw != 0)
          {
            if (!isDown)
            {
              isDown = true;
              prev = stu_Tablet.TabletToClient(stu_Tablet.penData[i]);
            }
            else
            {
              PointF curr = stu_Tablet.TabletToClient(stu_Tablet.penData[i]);
              gfx.DrawLine(stu_Tablet.penInk, prev, curr);
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

    public static void renderPenTimeData(Form sigForm, TabletLib.STU_Tablet stu_Tablet)
    {
      if (stu_Tablet.penTimeData.Count != 0)
      {
        // Redraw all the pen data up until now!
        Graphics gfx = GraphicFunctions.SetQualityGraphics(sigForm);
        bool isDown = false;
        PointF prev = new PointF();
        for (int i = 0; i < stu_Tablet.penTimeData.Count; ++i)
        {
          if (stu_Tablet.penTimeData[i].sw != 0)
          {
            if (!isDown)
            {
              isDown = true;
              prev = stu_Tablet.TabletToClientTimed(stu_Tablet.penTimeData[i]);
            }
            else
            {
              PointF curr = stu_Tablet.TabletToClientTimed(stu_Tablet.penTimeData[i]);
              gfx.DrawLine(stu_Tablet.penInk, prev, curr);
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

    public static Bitmap GetSigImage(Form parent, Form sigForm,TabletLib.STU_Tablet stu_Tablet)
    {
      Bitmap bitmap;
      SolidBrush brush;
      Point p1, p2;
      Pen penInk;

      Rectangle rect = new Rectangle(0, 0, stu_Tablet.capability.screenWidth, stu_Tablet.capability.screenHeight);

      try
      {
        bitmap = new Bitmap(rect.Width, rect.Height);
        Graphics gfx = Graphics.FromImage(bitmap);
        SizeF s = sigForm.AutoScaleDimensions;
        //            Dim inkWidthMM = 0.7F
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

        if (stu_Tablet.penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence)
        {
          for (int i = 1; i < stu_Tablet.penTimeData.Count; ++i)
          {
            p1 = stu_Tablet.TabletToScreen(stu_Tablet.penTimeData[i - 1]);
            p2 = stu_Tablet.TabletToScreen(stu_Tablet.penTimeData[i]);

            if (stu_Tablet.penTimeData[i - 1].sw > 0 || stu_Tablet.penTimeData[i].sw > 0)
            {
              gfx.DrawLine(penInk, p1, p2);
            }
          }
        }
        else
        {
          for (int i = 1; i < stu_Tablet.penData.Count; ++i)
          {
            p1 = stu_Tablet.TabletToScreen(stu_Tablet.penData[i - 1]);
            p2 = stu_Tablet.TabletToScreen(stu_Tablet.penData[i]);

            if (stu_Tablet.penData[i - 1].sw > 0 || stu_Tablet.penData[i].sw > 0)
            {
              gfx.DrawLine(penInk, p1, p2);
            }
          }
        }
      }
      catch (Exception ex)
      {
        // parent.print("Exception: " + ex.Message);
        MessageBox.Show("Exception: " + ex.Message);
        bitmap = null;
      }
      return bitmap;
    }
  }
}
