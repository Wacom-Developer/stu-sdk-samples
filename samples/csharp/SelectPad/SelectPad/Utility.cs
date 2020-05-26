using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Interop;
using System.Globalization;

namespace Utility
{
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }



    // TABLETINFO
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TABLETINFO
    {
        public uint uid;
        public bool isSelected;
        public POINT position;
    }

    // CONFIG
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct CONFIG
    {
        public WINDOWPLACEMENT placement;
        public bool isOnIdentifyMode;
        public bool isOnSingleSelectionMode;
        public List<TABLETINFO> tablets;
    }


    public static class WindowPlacement
    {
        public static Encoding encoding = new UTF8Encoding();
        public static XmlSerializer serializer = new XmlSerializer(typeof(CONFIG));

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
        
        public static CONFIG ReadConfig(string filePath)
        {

            CONFIG config = new CONFIG();
            config.placement = new WINDOWPLACEMENT();
            config.tablets = new List<TABLETINFO>();
            try
            {
                StreamReader streamReader = new StreamReader(filePath);
                String text = streamReader.ReadToEnd();
                streamReader.Close();

                byte[] xmlBytes = encoding.GetBytes(text);
                using (MemoryStream memoryStream = new MemoryStream(xmlBytes))
                {
                    config = (CONFIG)serializer.Deserialize(memoryStream);
                }

                config.placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                config.placement.flags = 0;
                config.placement.showCmd = (config.placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : config.placement.showCmd);
                /*
                String message = "isOnIdentifyMode " + config.isOnIdentifyMode + "\n";
                message += "isOnSingleSelectionMode " + config.isOnSingleSelectionMode + "\n";
                message += "Count " + config.tablets.Count + ": ";
                foreach (TABLETINFO j in config.tablets)
                {
                    message += "uid " + j.uid + " isSelected " + j.isSelected + " position " + j.position.X + " " + j.position.Y + "\n";
                }
                MessageBox.Show(message, "List", MessageBoxButton.OK, MessageBoxImage.Question);*/
            }
            catch (Exception)
            {
                // Fail silently.
            }

            return config;
        }

        public static void SetPlacement(IntPtr windowHandle, WINDOWPLACEMENT oldPlacement)
        {
            try
            {
                WINDOWPLACEMENT presentPlacement = new WINDOWPLACEMENT();
                GetWindowPlacement(windowHandle, out presentPlacement);

                presentPlacement.normalPosition = oldPlacement.normalPosition;

                SetWindowPlacement(windowHandle, ref oldPlacement);
            }
            catch (Exception) { }
        }
    }


    //Static class that allows us to know if we are debugging
    public static class DebugInfo
    {
        public static bool IsDebuggerAttached()
        {
            bool ret = false;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ret = true;
            }
            ret = false;
            return ret;
        }
    }

    //Static helper class for the radio buttons
    public static class ButtonsHelper
    {
        private static readonly Brush LightBlue = new SolidColorBrush(Color.FromRgb(205, 230, 247));
        private static readonly Brush MediumBlue = new SolidColorBrush(Color.FromRgb(143, 200, 237));

        private static readonly Brush MarineBlue = new SolidColorBrush(Color.FromRgb(98, 163, 229));
        private static readonly Brush MediumGrey = new SolidColorBrush(Color.FromRgb(177, 177, 177));

        private static LinearGradientBrush lchecked, lmouseover;

        public static void RadioButtonUpdate(RadioButton button)
        {
            Border b = (Border)button.Template.FindName("Border", button);
            if (null != b)
            {
                if (true == button.IsEnabled)
                {
                    if (true == button.IsChecked)
                    {
                        if (true == button.IsMouseOver)
                        {
                            b.BorderBrush = MarineBlue;
                            b.Background = Brushes.White;//MediumBlue;
                        }
                        else
                        {
                            b.BorderBrush = MarineBlue;
                            b.Background = LightBlue;
                        }
                    }
                    else
                    {
                        if (true == button.IsMouseOver)
                        {
                            b.BorderBrush = LightBlue;
                            b.Background = LightBlue;
                        }
                        else
                        {
                            b.BorderBrush = Brushes.Transparent;
                            b.Background = Brushes.Transparent;
                        }
                    }
                }
                else
                {
                    if (true == button.IsChecked)
                    {
                        b.BorderBrush = MediumGrey;
                        b.Background = Brushes.Transparent;
                    }
                    else
                    {
                        b.BorderBrush = Brushes.Transparent;
                        b.Background = Brushes.Transparent;
                    }
                }
            }
        }

        public static void InitializeHooksRibbonRadioButton(RibbonRadioButton button)
        {
            if (null == lchecked)
            {

                lchecked = new LinearGradientBrush();
                lchecked.EndPoint = new Point(0, 1);
                lchecked.StartPoint = new Point(0, 0);
                lchecked.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFDCA0"), 0));
                lchecked.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD692"), 0.181f));
                lchecked.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFC45D"), 0.39f));
                lchecked.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFD178"), 1));


                lmouseover = new LinearGradientBrush();
                lmouseover.EndPoint = new Point(0, 1);
                lmouseover.StartPoint = new Point(0, 0);
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFEFBF4"), 0));
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFDE7CE"), 0.181f));
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFDDEB8"), 0.39f));
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFCE6B"), 0.39f));
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFDE9A"), 0.79));
                lmouseover.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFEBAA"), 1));
            }

            button.MouseEnter += delegate
            {
                if (true == button.IsChecked)
                {
                    button.CheckedBackground = lmouseover;
                }
                else
                {
                    button.CheckedBackground = lchecked;
                }
            };
            button.MouseLeave += delegate
            {
                button.CheckedBackground = lchecked;
            };

            button.MouseMove += delegate
            {
                if (true == button.IsChecked)
                {
                    button.CheckedBackground = lmouseover;
                }
                else
                {
                    button.CheckedBackground = lchecked;
                }
            };


            button.Click += delegate
            {
                if (true == button.IsChecked)
                {
                    button.CheckedBackground = lmouseover;
                }
                else
                {
                    button.CheckedBackground = lchecked;
                }
            };

        }

    } //End of ButtonsHelper class

    //Static class that gives us the Dpi info 
    public static class DpiInfo
    {
        private static bool IsInitialized = false;
        private static double dpi = 1.0f;
        public static double GetDpi()
        {
            double ret = dpi;
            if (!IsInitialized)
            {
                Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;

                double dpiX = 1.0f, dpiY = 0;
                dpiX = m.M11;  //it will normally be 1.0f 
                dpiY = m.M22;
                if (DebugInfo.IsDebuggerAttached() && dpiX != 1.0f)
                {
                    MessageBoxResult result = MessageBox.Show("Dpi X: " + dpiX + " Dpi Y: " + dpiY, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                }
                dpi = dpiX;
                ret = dpi;
                IsInitialized = true;
            }
            return ret;
        }
    }

    //Static class that gives info about the size of a given Textblock
    public static class TextHelper
    {
        public static Size MeasureString(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                textBlock.Foreground);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }


}
