using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System;
using System.Windows.Controls.Ribbon;
using Utility;

namespace SelectPad
{

    //UIPadHandle is the main class that contains all the graphic elements and the state variables associated with of a single Pad
    //That includes the mouse events
    public class UIPadHandle
    {
        public event MouseButtonEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseButtonEventHandler MouseUp;

        private readonly Brush MarineBlue = new SolidColorBrush(Color.FromRgb(98, 163, 229));

        private class SelectionStateOpacity
        {
            public const double Selected = 1.0;
            public const double Unselected = 0.4;
        }

        public enum ProductId : ushort
        {
            ProductId_500 = 0x00a1,  ///< STU-500 
            ProductId_300 = 0x00a2,  ///< STU-300 
            ProductId_520A = 0x00a3,  ///< STU-520
            ProductId_430 = 0x00a4,  ///< STU-430 
            ProductId_530 = 0x00a5   ///< STU-530   
        }

        private bool m_mouseCapture = false;
        public bool MouseCapture
        {
            get { return m_mouseCapture; }
            set
            {
                m_mouseCapture = value;
                if (true == m_mouseCapture)
                {
                    Mouse.Capture(m_rect);
                }
                else
                {
                    Mouse.Capture(null);
                }
            }
        }

        private bool m_isOnIdentifyState = false;
        public bool IdentifyState
        {
            get { return m_isOnIdentifyState; }
            set
            {
                m_isOnIdentifyState = value;
                UpdateGraphics();
            }
        }

        private Canvas m_canvas;

        private double offset_x, offset_y; //this offset is a percentage 
        public double XOffset  
        {
            get { return offset_x; }
            set { SetOffsetX(value); }
        }

        private double addx, addy; //This is another offset, because of how a TextBlock works

        public double YOffset//this offset is a percentage
        {
            get {
                return offset_y;
            }
            set { SetOffsetY(value); }
        }

        private String m_fileName;
        public String Filename
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        private Image m_img;

        public ImageSource ImgSource
        {
            get { return m_img.Source; }
            set
            {
                m_img.Source = value;
                m_img.Width = m_width;
                m_img.Height = m_height;
            }
        }

        private Image m_warning;
        public ImageSource WarningSource
        {
            get { return m_warning.Source; }
            set
            {
                m_warning.Source = value;
                m_warning.Width = 20;
                m_warning.Height = 17;
            }
        }

        private bool m_hasWarning = false;
        public bool HasWarning
        {
            get { return m_hasWarning; }
            set
            {
                m_hasWarning = value;
                UpdateGraphics();
            }
        }

        private TextBlock m_text;
        private Rectangle m_rect;
        private int m_idIndex = 0;

        public ushort ProductID
        {
            set { SetIdIndex(value); }
        }

        public int IDIndex
        {
            get { return m_idIndex; }
        }

        private int m_num;
        public int Num
        {
            get { return m_num; }
            set { SetNum(value); }
        }

        private double m_xpos;
        public double XPos
        {
            get { return m_xpos; }
            set { SetX(value); }
        }

        private double m_ypos;
        public double YPos
        {
            get { return m_ypos; }
            set { SetY(value); }
        }

        private double m_borderThickness = 0.0f;
        public double BorderThickness
        {
            get { return m_borderThickness; }
            set { SetBorderThickness(value); }
        }

        private double m_width;
        public double Width
        {
            get { return m_width; }
            set { SetWidth(value); }
        }

        private double m_height;
        public double Height
        {
            get { return m_height; }
            set { SetHeight(value); }
        }

        private bool m_isSelected = false;
        public bool Selected
        {
            get { return m_isSelected; }
            set { SetSelected(value); }
        }

        public UIPadHandle(String fileName, int num, Canvas c)
        {
            if (null == c)
            {
                throw new Exception("Error: null Canvas");
            }
            m_canvas = c;
            m_num = num;
            m_fileName = fileName;
            m_img = new Image();
            m_text = new TextBlock();
            m_text.Text = "";
            m_text.FontFamily = new FontFamily("Tahoma");
            m_text.VerticalAlignment = VerticalAlignment.Center;
            m_text.HorizontalAlignment = HorizontalAlignment.Center;

            m_rect = new Rectangle();
            m_img.SnapsToDevicePixels = true;
            m_img.UseLayoutRounding = true;

            m_warning = new Image();
            m_warning.SnapsToDevicePixels = true;
            m_warning.UseLayoutRounding = true;
            m_warning.Visibility = Visibility.Hidden;

            m_img.MouseDown += OnMouseLeftButtonDown;
            m_img.MouseMove += OnMouseMove;
            m_img.MouseUp += OnMouseLeftButtonUp;

            m_rect.MouseDown += OnMouseLeftButtonDown;
            m_rect.MouseMove += OnMouseMove;
            m_rect.MouseUp += OnMouseLeftButtonUp;

            m_text.MouseDown += OnMouseLeftButtonDown;
            m_text.MouseMove += OnMouseMove;
            m_text.MouseUp += OnMouseLeftButtonUp;

            m_rect.Visibility = Visibility.Visible;
            m_rect.Stroke = MarineBlue;
            m_rect.Fill = Brushes.White;

            SetSelected(false);

            m_canvas.Children.Add(m_rect);
            m_canvas.Children.Add(m_img);
            m_canvas.Children.Add(m_text);
            m_canvas.Children.Add(m_warning);
        }

        private void UpdateGraphics()
        {

            m_warning.Visibility = m_hasWarning ? Visibility.Visible : Visibility.Hidden;
            if (m_isOnIdentifyState)
            {
                m_rect.Stroke = MarineBlue;
                m_text.Visibility = Visibility.Visible;
                m_img.Opacity = SelectionStateOpacity.Selected;
            }
            else
            {
                m_text.Visibility = Visibility.Hidden;
                m_rect.Stroke = Brushes.Transparent;
                if (m_isSelected)
                {
                    m_img.Opacity = SelectionStateOpacity.Selected;
                }
                else
                {
                    m_img.Opacity = SelectionStateOpacity.Unselected;
                }

            }
        }

        public void RemoveFromCanvas()
        {
            m_canvas.Children.Remove(m_rect);
            m_canvas.Children.Remove(m_img);
            m_canvas.Children.Remove(m_text);
            m_canvas.Children.Remove(m_warning);
        }

        public void showPadNumberOnMonitor(Point ScreenSize)
        {
            //Show number on monitor
            double dpi = DpiInfo.GetDpi();
            double imgWidth = m_img.Width * ScreenSize.X;

            BitmapSource bmps = (BitmapSource)m_img.Source;
            double realHeight = null != bmps ? bmps.PixelHeight : 0;
            double imgHeight = m_img.Height * ScreenSize.Y;
            if (realHeight != 0)
            {
                imgHeight = realHeight * ScreenSize.Y;
            }

            m_text.Text = "" + m_num;

            m_text.FontSize = (double)new FontSizeConverter().ConvertFrom(imgHeight * 0.8f + "pt");

            Size sizes = TextHelper.MeasureString(m_text);
            double x = (imgWidth - sizes.Width) / 2;
            double y = (imgHeight - sizes.Height) / 2;
            addx = x;
            addy = y;

            Canvas.SetLeft(m_text, m_xpos + m_borderThickness + offset_x * m_img.Width + addx);
            Canvas.SetTop(m_text, m_ypos + offset_y * realHeight + addy + (m_height - realHeight) / 2);

            IdentifyState = true;
            m_text.InvalidateVisual();
        }

        private void SetOffsetX(double x)
        {
            offset_x = x;
            Canvas.SetLeft(m_text, m_xpos + m_borderThickness + offset_x * m_img.Width + addx);
        }

        private void SetOffsetY(double y)
        {
            offset_y = y;
            BitmapSource bmps = (BitmapSource)m_img.Source;
            double realHeight = null != bmps ? bmps.PixelHeight : 0;
            Canvas.SetTop(m_text, m_ypos + offset_y * realHeight + addy + (m_height - realHeight) / 2);
        }

        private void SetNum(int n)
        {
            m_num = n;
        }

        private void SetX(double x)
        {
            m_xpos = x;
            Canvas.SetLeft(m_text, m_xpos + m_borderThickness + offset_x * m_img.Width + addx);
            Canvas.SetLeft(m_rect, m_xpos);
            Canvas.SetLeft(m_img, m_xpos + m_borderThickness);
            Canvas.SetLeft(m_warning, m_xpos + m_borderThickness);
        }

        private void SetY(double y)
        {
            m_ypos = y;

            BitmapSource bmps = (BitmapSource)m_img.Source;
            double realHeight = null != bmps ? bmps.PixelHeight : 0;
            Canvas.SetTop(m_text, m_ypos + offset_y * realHeight + addy + (m_height - realHeight) / 2);
            Canvas.SetTop(m_rect, m_ypos);
            Canvas.SetTop(m_img, m_ypos + m_borderThickness);
            Canvas.SetTop(m_warning, m_ypos + m_borderThickness);
        }

        private void SetBorderThickness(double d)
        {
            m_borderThickness = d;
            m_rect.StrokeThickness = m_borderThickness;
            SetWidth(m_width);
            SetHeight(m_height);
            SetX(m_xpos);
            SetY(m_ypos);
        }

        private void SetWidth(double w)
        {
            double offx = XOffset;
            m_width = w;
            m_rect.Width = m_width;
            m_img.Width = m_width - 2 * m_borderThickness;
            m_warning.Width = 20;
            SetOffsetX(offx);
        }

        private void SetHeight(double h)
        {
            double offy = this.YOffset;
            m_height = h;
            m_rect.Height = Height;
            m_img.Height = m_height - 2 * m_borderThickness;
            m_warning.Height = 17;
            SetOffsetY(offy);
        }

        private void SetSelected(bool s)
        {
            m_isSelected = s;
            UpdateGraphics();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = MouseDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = MouseUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseEventHandler handler = MouseMove;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SetIdIndex(ushort id)
        {
            m_idIndex = 0;
            switch (id)
            {
                case (ushort)ProductId.ProductId_300:
                    m_idIndex = 0;
                    break;
                case (ushort)ProductId.ProductId_430:
                    m_idIndex = 1;
                    break;
                case (ushort)ProductId.ProductId_500:
                    m_idIndex = 2;
                    break;
                case (ushort)ProductId.ProductId_520A:
                    m_idIndex = 3;
                    break;
                case (ushort)ProductId.ProductId_530:
                    m_idIndex = 4;
                    break;
                default:
                    break;
            }
        }
    }

}
