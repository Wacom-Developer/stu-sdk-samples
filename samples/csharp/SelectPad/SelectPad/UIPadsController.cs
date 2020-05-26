using System;
using System.Collections.Generic;
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
using System.Windows.Controls.Ribbon;
using Utility;
using System.Xml;
using System.Xml.Serialization;

namespace SelectPad
{

    //SpaceFinder class is used to find void space in the App's window where we can add a new Pad
    public class SpaceFinder
    {

        public class Block
        {
            public Point topleft, bottomright;
        }

        private List<Block> m_blocks;
        private Point m_canvasSize;
        private bool[,] m_spaceArray; //this array represents the Canvas/app's window space divided in a grid with each grid element the size of a Pad
        private int PadPixelHeight = 117, PadPixelWidth = 100;

        public SpaceFinder(double mx, double my, List<UIPadHandle> m_pads, int padWidth, int padHeight)
        {
            PadPixelHeight = padHeight;
            PadPixelWidth = padWidth;
            m_blocks = new List<Block>();
            m_canvasSize = new Point(mx, my);

            int ny = (int)my / PadPixelHeight;
            int nx = (int)mx / PadPixelWidth;
            m_spaceArray = new bool[nx, ny];

            for (int i = 0; i < m_pads.Count; i++)
            {
                Block b = new Block();
                b.topleft = new Point(m_pads[i].XPos, m_pads[i].YPos);
                b.bottomright = new Point(m_pads[i].Width + b.topleft.X, m_pads[i].Height + b.topleft.Y);
                m_blocks.Add(b);
            }
            CheckSpace();
        }

        public SpaceFinder(double mx, double my, List<UIPadHandle> m_pads, int dontInclude, int padWidth, int padHeight)
        {
            PadPixelHeight = padHeight;
            PadPixelWidth = padWidth;
            m_blocks = new List<Block>();
            m_canvasSize = new Point(mx, my);

            int ny = (int)my / PadPixelHeight;
            int nx = (int)mx / PadPixelWidth;
            m_spaceArray = new bool[nx, ny];

            for (int i = 0; i < m_pads.Count; i++)
            {
                if (i != dontInclude)
                {
                    Block b = new Block();
                    b.topleft = new Point(m_pads[i].XPos, m_pads[i].YPos);
                    b.bottomright = new Point(m_pads[i].Width + b.topleft.X, m_pads[i].Height + b.topleft.Y);
                    m_blocks.Add(b);
                }
            }
            CheckSpace();
        }

        private void CheckSpace()
        {
            for (int n = 0; n < m_blocks.Count; n++)
            {
                int imin = (int)Math.Floor(m_blocks[n].topleft.X / PadPixelWidth);
                int imax = (int)Math.Ceiling(m_blocks[n].bottomright.X / PadPixelWidth);

                int jmin = (int)Math.Floor(m_blocks[n].topleft.Y / PadPixelHeight);
                int jmax = (int)Math.Ceiling(m_blocks[n].bottomright.Y / PadPixelHeight);

                for (int i = imin; i < m_spaceArray.GetLength(0) && i < imax; i++)
                {
                    for (int j = jmin; j < m_spaceArray.GetLength(1) && j < jmax; j++)
                    {
                        m_spaceArray[i, j] = true; //This space is being used
                    }
                }
            }
        }

        public Point getFreeSpace()
        {
            Point p = new Point(0, 0);
            for (int j = 0; j < m_spaceArray.GetLength(1); j++)
            {
                for (int i = 0; i < m_spaceArray.GetLength(0); i++)
                {
                    if (!m_spaceArray[i, j])
                    {
                        p.X = PadPixelWidth * i;
                        p.Y = PadPixelHeight * j;
                        return p;
                    }
                }
            }
            return p;
        }
    }

    //AppState enum indicates the overall state/mode of the program
    public enum AppState
    {
        Selection_Mode,
        Identification_Mode
    }

    //UIPadsController is the main class that deals with user input and graphic elements
    //It deals with the Pads at a higher level of abstraction than the UIPandHandle and PadHandle classes
    public class UIPadsController//: IDisposable
    {

        private static Encoding encoding = new UTF8Encoding();
        private static XmlSerializer serializer = new XmlSerializer(typeof(CONFIG));

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        enum ProductId : ushort
        {
            ProductId_500 = 0x00a1,  ///< STU-500 
            ProductId_300 = 0x00a2,  ///< STU-300 
            ProductId_520A = 0x00a3,  ///< STU-520
            ProductId_430 = 0x00a4,  ///< STU-430 
            ProductId_530 = 0x00a5   ///< STU-530   
        }

        enum SelectionMode
        {
            Single,
            Multiple
        }

        //Contains the info about the size and position of the pad screen, relative to the picture of that pad.
        class UIPadScreen
        {
            public Point m_pos, m_size;
            public UIPadScreen(Point pos, Point size)
            {
                m_pos = pos;
                m_size = size;
            }
            public UIPadScreen()
            {
                m_pos = new Point(0, 0);
                m_size = new Point(0, 0);
            }
        }


        //Contains the picture of the pad and the position/size of its screen in that picture
        class BitmapResource
        {
            public BitmapImage m_img;
            public UIPadScreen m_screen;

            public BitmapResource()
            {
                m_img = null;
                Point p = new Point(0, 0);
                m_screen = new UIPadScreen();
            }

            public BitmapResource(BitmapImage img, UIPadScreen screen)
            {
                m_img = img;
                m_screen = screen;
            }
        }

        //These vars define the size of the pictures of the pads
        public int PadPixelHeight = 117;
        public int PadPixelWidth = 100;

        AppState m_state = AppState.Selection_Mode;
        SelectionMode m_mode = SelectionMode.Single;
        List<BitmapResource> m_bitmapRes;
        BitmapImage m_warning;
        List<UIPadHandle> m_padsUI;
        List<PadHandle> m_padsControl;
        Canvas canvas; //The Canvas object where the pictures of the pads are presented
        Grid LayoutRoot;

        double m_selectThickness = 3.0f; //thickness of the border surrounding the pads
        bool m_captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIPadHandle m_MouseOverSource = null, m_lastSelected = null;

        RadioButton m_identifyButton, m_singleModeButton, m_multipleModeButton;

        DispatcherTimer timer; //this is a hack to be able to update the buttons correctly

        public UIPadsController(wgssSTU.UsbDevices m_usbDevices, Canvas lcanvas, Grid root, StackPanel tbar,
            CONFIG config, Window w)
        {


            timer = new DispatcherTimer();
            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += UpdateButtons;
            
            PadPixelHeight = 117;
            PadPixelWidth = 100;
            canvas = lcanvas;
            LayoutRoot = root;
            m_bitmapRes = new List<BitmapResource>();
            m_padsUI = new List<UIPadHandle>();
            m_padsControl = new List<PadHandle>();

            tbar.Height = 60;
            //////////////////////////////////////////////
            m_identifyButton = new RadioButton();
            m_identifyButton.GroupName = "Identify";
            m_identifyButton.Height = tbar.Height - 6;

            Image im = new Image();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(@"pictures/move.png", UriKind.Relative);
            //bmp.DecodePixelHeight = (int)(m_identifyButton.Height * 2*DpiInfo.GetDpi());
            bmp.EndInit();

            im.SnapsToDevicePixels = true;
            im.UseLayoutRounding = true;
            im.Source = bmp;

            m_identifyButton.Content = im;
            m_identifyButton.MouseEnter += delegate { ButtonsHelper.RadioButtonUpdate(m_identifyButton); };
            m_identifyButton.MouseLeave += delegate { ButtonsHelper.RadioButtonUpdate(m_identifyButton); };

            tbar.Children.Add(m_identifyButton);

            m_identifyButton.Click += delegate { IdentifyButtonClick(); };
            m_identifyButton.ToolTip = "Position and identify pads";

            ////////////////////////////////////////////////////////////////

            Separator sep = new Separator();
            sep.SetResourceReference(Control.StyleProperty, "VerticalSeparatorStyle");

            tbar.Children.Add(sep);


            /////////////////////////////////////////////
            m_singleModeButton = new RadioButton();
            //m_identifyButton.GroupName = "Selection Mode";
            m_singleModeButton.Height = tbar.Height - 6;


            m_singleModeButton.MouseEnter += delegate
            {
                ButtonsHelper.RadioButtonUpdate(m_singleModeButton);
                ButtonsHelper.RadioButtonUpdate(m_multipleModeButton);
            };
            m_singleModeButton.MouseLeave += delegate
            {
                ButtonsHelper.RadioButtonUpdate(m_singleModeButton);
                ButtonsHelper.RadioButtonUpdate(m_multipleModeButton);
            };

            RadioGifControl gifSingle = new RadioGifControl("pictures/select_singleBIG.gif", m_singleModeButton);

            m_singleModeButton.MouseEnter += delegate { gifSingle.Play(); };
            m_singleModeButton.MouseLeave += delegate { gifSingle.Stop(); };

            tbar.Children.Add(m_singleModeButton);

            m_singleModeButton.ToolTip = "Select single pad";
            m_singleModeButton.Click += delegate
            {
                ChangeSelectionMode(m_singleModeButton, new EventArgs());
            };


            //////////////////////////////////////////////

            m_multipleModeButton = new RadioButton();
            //m_multipleModeButton.GroupName = "Selection Mode";
            m_multipleModeButton.Height = tbar.Height - 6;

            m_multipleModeButton.MouseEnter += delegate
            {
                ButtonsHelper.RadioButtonUpdate(m_multipleModeButton);
                ButtonsHelper.RadioButtonUpdate(m_singleModeButton);
            };
            m_multipleModeButton.MouseLeave += delegate
            {
                ButtonsHelper.RadioButtonUpdate(m_multipleModeButton);
                ButtonsHelper.RadioButtonUpdate(m_singleModeButton);
            };


            RadioGifControl gifMultiple = new RadioGifControl("pictures/select_multipleBIG.gif", m_multipleModeButton);

            m_multipleModeButton.MouseEnter += delegate { gifMultiple.Play(); };
            m_multipleModeButton.MouseLeave += delegate { gifMultiple.Stop(); };

            tbar.Children.Add(m_multipleModeButton);
            m_multipleModeButton.ToolTip = "Select multiple pads";


            m_multipleModeButton.Click += delegate
            {
                ChangeSelectionMode(m_multipleModeButton, new EventArgs());
            };

            ///////////////////////////////////////////////

            for (int i = 0; i < m_usbDevices.Count; i++)
            {
                m_padsControl.Add(new PadHandle(this, m_usbDevices[i], m_state, i));
            }

            loadBitmaps();

            if (m_usbDevices.Count != 0)
            {
                loadPads(m_usbDevices);
                noOverlappingOnStart();
            }

            m_state = AppState.Selection_Mode;
            m_singleModeButton.IsChecked = true;
            IdentifyButtonClick();
            SetConfig(config);

        }

        private void SetConfig(CONFIG config)
        {
           
        }

        void UpdateButtons(object sender, EventArgs args)
        {
            timer.Stop();
            ButtonsHelper.RadioButtonUpdate(m_identifyButton);
            ButtonsHelper.RadioButtonUpdate(m_singleModeButton);
            ButtonsHelper.RadioButtonUpdate(m_multipleModeButton);
        }

        //When a new pad is connected, this function gives us the first available number to be used to id the pad
        private int GetNextNum()
        {
            int i = 0;
            bool getOut = false;
            while (!getOut)
            {
                bool used = false;
                foreach (UIPadHandle p in m_padsUI)
                {
                    if (p.Num == i)
                    {
                        used = true;
                        i++;
                        break;
                    }
                }
                if (!used)
                {
                    getOut = true;
                }
            }
            return i;
        }

        //returns true if the pad on position i in the pads list m_padsUI is selected
        public bool isSelected(int i)
        {

            bool ret = false;
            if (i < m_padsUI.Count)
            {
                ret = m_padsUI[i].Selected;
            }
            return ret;
        }

        public void ChangeSelectionMode(object sender, EventArgs e)
        {
            if ((sender == m_singleModeButton) && SelectionMode.Multiple == m_mode)
            {
                m_mode = SelectionMode.Single;
                m_singleModeButton.IsChecked = true;
                if (AppState.Selection_Mode == m_state)
                {
                    select(m_lastSelected, false);
                }
            }
            else if ((sender == m_multipleModeButton) && SelectionMode.Single == m_mode)
            {
                m_mode = SelectionMode.Multiple;
                m_multipleModeButton.IsChecked = true;
            }
            timer.Start();
        }

        private void setNumberVisible(int i, bool visible)
        {
            if (m_padsUI.Count > i)
            {
                m_padsUI[i].IdentifyState = visible;
            }
        }

        //Clears the pads' screens
        private void ClearPads(bool alsoClearSelected)
        {
            for (int i = 0; i < m_padsControl.Count; i++)
            {
                bool setVisibleB = false;

                if (!isSelected(i) || alsoClearSelected)
                {
                    m_padsControl[i].ClearPad(isSelected(i));
                    setVisibleB = false;
                }
                setNumberVisible(i, setVisibleB);
            }
        }

        private void showPadNumberOnMonitor(int i)
        {
            m_padsUI[i].showPadNumberOnMonitor(m_bitmapRes[m_padsUI[i].IDIndex].m_screen.m_size);
        }

        public void IdentifyButtonClick()
        {

            if (AppState.Selection_Mode == m_state)
            {
                m_state = AppState.Identification_Mode;

                m_singleModeButton.IsEnabled = false;
                m_multipleModeButton.IsEnabled = false;
                m_identifyButton.IsChecked = true;
                IdentifyPads();
                for (int i = 0; i < m_padsUI.Count; i++)
                {
                    m_padsUI[i].IdentifyState = true;
                }
            }
            else
            {
                m_state = AppState.Selection_Mode;

                m_identifyButton.IsChecked = false;
                m_singleModeButton.IsEnabled = true;
                m_multipleModeButton.IsEnabled = true;
                ClearPads(true);
                for (int i = 0; i < m_padsUI.Count; i++)
                {
                    m_padsUI[i].IdentifyState = false;
                }

                EventArgs e = new EventArgs();
                if (SelectionMode.Single == m_mode)
                {
                    m_singleModeButton.IsChecked = true;
                }
                else
                {
                    m_multipleModeButton.IsChecked = true;
                }
            }

            timer.Start();
        }

        //Each pad is assigned a different number
        //this number is shown both on the real pad and in the picture of the pad on the monitor
        private void IdentifyPads()
        {
            for (int i = 0; i < m_padsControl.Count; i++)
            {
                m_padsControl[i].IdentifyPad(m_padsUI[i].Num);
            }

            for (int i = 0; i < m_padsUI.Count; i++)
            {
                showPadNumberOnMonitor(i);
            }
        }

        //When the app starts, this method sets the position of each pad on screen so that they do not overlap
        private void noOverlappingOnStart()
        {
            if (m_padsUI.Count > 0)
            {
                int ih = (int)Math.Ceiling(Math.Sqrt((double)m_padsUI.Count));

                for (int j = 0, jh = 0, jv = 0; j < m_padsUI.Count; j++)
                {
                    m_padsUI[j].XPos = jh * PadPixelWidth;
                    m_padsUI[j].YPos = jv * PadPixelHeight;

                    if (jh == ih - 1)
                    {
                        jh = 0;
                        jv++;
                    }
                    else
                    {
                        jh++;
                    }
                }
            }
        }

        //Sets the minimum size of the canvas to a size where there is enough space for
        //all pads without overlapping
        public void setMinSizesCanvas()
        {
            int nmaxh = (int)Math.Floor(canvas.ActualHeight / PadPixelWidth);
            int nmaxv = (int)Math.Floor(canvas.ActualWidth / PadPixelHeight);
            int ih = 0;
            int iv = 0;

            if (m_padsUI.Count > 0)
            {
                ih = (int)Math.Ceiling(Math.Sqrt((double)m_padsUI.Count));
                iv = (int)Math.Ceiling(((double)m_padsUI.Count) / ((double)ih));
            }

            canvas.MinHeight = PadPixelHeight * iv + 10;
            canvas.MinWidth = PadPixelWidth * ih + 10;
        }

        private void loadBitmaps()
        {
            BitmapResource bres;
            BitmapImage bpic;

            bpic = new BitmapImage();
            int decodeWidth = (int)((PadPixelWidth - 2 * m_selectThickness) * DpiInfo.GetDpi());

            bpic.BeginInit();
            bpic.UriSource = new Uri(@"pictures/STU-300.png", UriKind.Relative);
            bpic.DecodePixelWidth = decodeWidth;
            bpic.EndInit();
            bres = new BitmapResource(bpic, new UIPadScreen(new Point(0.18666, 0.29707), new Point(0.6333, 0.2008)));
            m_bitmapRes.Add(bres);

            bpic = new BitmapImage();

            bpic.BeginInit();
            bpic.UriSource = new Uri(@"pictures/STU-430.png", UriKind.Relative);
            bpic.DecodePixelWidth = decodeWidth;
            bpic.EndInit();
            bres = new BitmapResource(bpic, new UIPadScreen(new Point(0.20333, 0.22876), new Point(0.59, 0.3398)));
            m_bitmapRes.Add(bres);

            bpic = new BitmapImage();

            bpic.BeginInit();
            bpic.UriSource = new Uri(@"pictures/STU-500.png", UriKind.Relative);
            bpic.DecodePixelWidth = decodeWidth;
            bpic.EndInit();
            bres = new BitmapResource(bpic, new UIPadScreen(new Point(0.18666, 0.22287), new Point(0.63, 0.4047)));
            m_bitmapRes.Add(bres);

            bpic = new BitmapImage();

            bpic.BeginInit();
            bpic.UriSource = new Uri(@"pictures/STU-520.png", UriKind.Relative);
            bpic.DecodePixelWidth = decodeWidth;
            bpic.EndInit();
            bres = new BitmapResource(bpic, new UIPadScreen(new Point(0.17333, 0.21511), new Point(0.6566, 0.3372)));
            m_bitmapRes.Add(bres);

            bpic = new BitmapImage();

            bpic.BeginInit();
            bpic.UriSource = new Uri(@"pictures/STU-530.png", UriKind.Relative);
            bpic.DecodePixelWidth = decodeWidth;
            bpic.EndInit();
            bres = new BitmapResource(bpic, new UIPadScreen(new Point(0.16333, 0.20779), new Point(0.6733, 0.3766)));
            m_bitmapRes.Add(bres);

            m_warning = new BitmapImage();

            m_warning.BeginInit();
            m_warning.UriSource = new Uri(@"pictures/warning.png", UriKind.Relative);
            m_warning.DecodePixelWidth = (int)(20.0f * DpiInfo.GetDpi());
            m_warning.DecodePixelHeight = (int)(17.0f * DpiInfo.GetDpi());
            m_warning.EndInit();

        }

        private void loadPads(wgssSTU.UsbDevices usbDevices)
        {
            for (int i = 0; i < m_padsControl.Count; i++)
            {
                UIPadHandle ph = new UIPadHandle(usbDevices[i].fileName, i, canvas);

                ph.ProductID = m_padsControl[i].getProductId();

                ph.BorderThickness = 0;
                ph.Width = PadPixelWidth;
                ph.Height = PadPixelHeight;
                ph.XOffset = m_bitmapRes[ph.IDIndex].m_screen.m_pos.X;
                ph.YOffset = m_bitmapRes[ph.IDIndex].m_screen.m_pos.Y;
                ph.MouseDown += shape_MouseLeftButtonDown;
                ph.MouseMove += shape_MouseMove;
                ph.MouseUp += shape_MouseLeftButtonUp;

                ph.ImgSource = m_bitmapRes[ph.IDIndex].m_img;
                ph.WarningSource = m_warning;
                ph.Selected = false;
                ph.IdentifyState = false;

                ph.BorderThickness = m_selectThickness;

                m_padsUI.Add(ph);
            }
        }

        private void shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) //&& e.ClickCount == 1) // Single Click
            {
                if (AppState.Selection_Mode == m_state)
                {
                    if (m_padsUI.Count > 0)
                    {
                        select((UIPadHandle)sender);
                    }
                }
                else
                {
                    if (m_padsUI.Count > 0)
                    {
                        m_MouseOverSource = (UIPadHandle)sender;
                        m_MouseOverSource.MouseCapture = true;
                        m_captured = true;
                        x_shape = m_MouseOverSource.XPos;
                        x_canvas = e.GetPosition(canvas).X;
                        y_shape = m_MouseOverSource.YPos;
                        y_canvas = e.GetPosition(canvas).Y;
                    }
                }
            }
        }

        //Selects a new pad that has just been added/connected
        private void selectNew(UIPadHandle selectThis)
        {
            for (int i = 0; i < m_padsUI.Count; i++)
            {
                if (selectThis == m_padsUI[i])
                {
                    if (false == m_padsUI[i].Selected)
                    {
                        m_lastSelected = selectThis;
                        m_padsUI[i].Selected = true;
                        m_padsUI[i].IdentifyState = (AppState.Identification_Mode == m_state) ? true : false;

                    }
                    else if (true == m_padsUI[i].Selected)
                    {
                        m_padsControl[i].connect();
                        m_padsUI[i].Selected = false;
                        m_lastSelected = null;
                    }
                }
                else
                {
                    if ((SelectionMode.Single == m_mode && !m_padsControl[i].isConnected()) ||
                        (SelectionMode.Multiple == m_mode && !m_padsControl[i].isConnected() && false == m_padsUI[i].Selected)
                        )
                    {
                        m_padsControl[i].connect();
                        m_padsUI[i].Selected = false;
                    }
                }
            }
        }

        private void select(UIPadHandle selectThis, bool flip = true)
        {
            for (int i = 0; i < m_padsUI.Count; i++)
            {
                if (selectThis == m_padsUI[i])
                {
                    if (false == m_padsUI[i].Selected || !flip)
                    {
                        //Try to disconnect even when not connected so that the program knows that it doesn't need to connect anymore
                        m_padsControl[i].disconnect();
                        m_lastSelected = selectThis;
                        m_padsUI[i].Selected = true;
                    }
                    else if (true == m_padsUI[i].Selected && flip)
                    {
                        m_padsControl[i].connect();
                        m_padsUI[i].Selected = false;
                        if (m_lastSelected == selectThis) //if the last selected is the one we are unselecting,  we don't know which one was selected before that
                        {
                            m_lastSelected = null;
                        }
                    }
                }
                else
                {
                    if ((SelectionMode.Single == m_mode && !m_padsControl[i].isConnected()) ||
                        (SelectionMode.Multiple == m_mode && !m_padsControl[i].isConnected() && false == m_padsUI[i].Selected)
                        )
                    {
                        m_padsControl[i].connect();
                        m_padsUI[i].Selected = false;
                    }
                }
            }
        }

        private void shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_padsUI.Count > 0 && AppState.Identification_Mode == m_state)
            {
                if (m_captured && m_MouseOverSource != null)
                {
                    double x = e.GetPosition(canvas).X;
                    double y = e.GetPosition(canvas).Y;
                    x_shape += x - x_canvas;
                    x_shape = Math.Min(Math.Max(0, x_shape), canvas.ActualWidth - m_MouseOverSource.Width);
                    m_MouseOverSource.XPos = x_shape;

                    x_canvas = x;
                    y_shape += y - y_canvas;
                    y_shape = Math.Min(Math.Max(0, y_shape), canvas.ActualHeight - m_MouseOverSource.Height);
                    m_MouseOverSource.YPos = y_shape;
                    y_canvas = y;
                }
            }
        }

        private void shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_captured = false;
            if (null != m_MouseOverSource)
            {
                m_MouseOverSource.MouseCapture = false;
            }
            else
            {
                Mouse.Capture(null);
            }
        }

        public void RemoveDevice(wgssSTU.IUsbDevice remDev)
        {
            for (int j = 0; j < m_padsUI.Count; j++)
            {
                if (m_padsUI[j].Filename == remDev.fileName)
                {
                    if (m_MouseOverSource != null && m_MouseOverSource.Filename == remDev.fileName)
                    {
                        m_MouseOverSource = null;
                    }
                    if (m_lastSelected != null && m_lastSelected.Filename == remDev.fileName)
                    {
                        m_lastSelected = null;
                    }

                    m_padsUI[j].RemoveFromCanvas();
                    m_padsUI.RemoveAt(j);
                    m_padsControl[j].removeDevice(j, m_padsControl);
                    break;
                }
            }
            setMinSizesCanvas();
        }

        public void AddDevice(wgssSTU.IUsbDevice addDev)
        {
            SpaceFinder sf = new SpaceFinder(canvas.ActualWidth, canvas.ActualHeight, m_padsUI, PadPixelWidth, PadPixelHeight);

            Point pos = sf.getFreeSpace();

            int jn = GetNextNum();
            UIPadHandle ph = new UIPadHandle(addDev.fileName, jn, canvas);

            m_padsControl.Add(new PadHandle(this, addDev, m_state, ph.Num));

            ph.ProductID = addDev.idProduct;

            ph.Width = m_bitmapRes[ph.IDIndex].m_img.PixelWidth + 2 * m_selectThickness;
            ph.Height = m_bitmapRes[ph.IDIndex].m_img.PixelHeight + 2 * m_selectThickness;
            ph.XOffset = m_bitmapRes[ph.IDIndex].m_screen.m_pos.X;
            ph.YOffset = m_bitmapRes[ph.IDIndex].m_screen.m_pos.Y;
            ph.MouseDown += shape_MouseLeftButtonDown;
            ph.MouseMove += shape_MouseMove;
            ph.MouseUp += shape_MouseLeftButtonUp;
            ph.XPos = pos.X;
            ph.YPos = pos.Y;

            ph.ImgSource = m_bitmapRes[ph.IDIndex].m_img;
            ph.WarningSource = m_warning;


            ph.Width = PadPixelWidth;
            ph.Height = PadPixelHeight;

            ph.Selected = false;
            ph.IdentifyState = false;

            ph.BorderThickness = m_selectThickness;

            m_padsUI.Add(ph);

            selectNew(ph);

            if (AppState.Identification_Mode == m_state)
            {
                showPadNumberOnMonitor(m_padsUI.Count - 1);
            }

            setMinSizesCanvas();
        }

        //When the window is resized, this method changes the position of the pads in screen
        //in the case it's necessary
        public void OnResizeCanvas()
        {
            if (null != m_padsUI && m_padsUI.Count > 0)
            {
                for (int i = 0; i < m_padsUI.Count; i++)
                {
                    if (canvas.ActualHeight < m_padsUI[i].YPos + m_padsUI[i].Height
                        || canvas.ActualWidth < m_padsUI[i].XPos + m_padsUI[i].Width)
                    {
                        RelocatePadUI(i);
                    }
                }
            }
        }

        public void RelocatePadUI(int i)
        {
            SpaceFinder sf = new SpaceFinder(canvas.ActualWidth, canvas.ActualHeight, m_padsUI, i, PadPixelWidth, PadPixelHeight);

            Point pos = sf.getFreeSpace();

            m_padsUI[i].XPos = pos.X;
            m_padsUI[i].YPos = pos.Y;
        }

        public void SetWarning(int num, bool hasWarning)
        {
            int j = -1;
            for (int i = 0; i < m_padsUI.Count; i++)
            {
                if (m_padsUI[i].Num == num)
                {
                    j = i;
                    break;
                }
            }
            if (j != -1)
            {
                m_padsUI[j].HasWarning = hasWarning;
            }
        }

        public void SaveConfig(IntPtr windowHandle, String filePath)
        {
            try
            {
                CONFIG config = new CONFIG();
                config.placement = new WINDOWPLACEMENT();
                config.tablets = new List<TABLETINFO>();
                config.isOnIdentifyMode = (AppState.Identification_Mode == m_state ? true : false);
                config.isOnSingleSelectionMode = (SelectionMode.Single == m_mode ? true : false); 

                for(int i = 0; i < m_padsUI.Count; i++)
                {
                    TABLETINFO t = new TABLETINFO();

                    t.uid = (uint)m_padsControl[i].getUID(); 
                    t.position = new POINT((int)m_padsUI[i].XPos, (int)m_padsUI[i].YPos);
                    t.isSelected = m_padsUI[i].Selected;

                    config.tablets.Add(t);
                }

                String s = "";

                GetWindowPlacement(windowHandle, out config.placement);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                    {
                        serializer.Serialize(xmlTextWriter, config);
                        byte[] xmlBytes = memoryStream.ToArray();
                        s = encoding.GetString(xmlBytes);
                    }
                }

                StreamWriter file = new StreamWriter(filePath);
                file.WriteLine(s);

                file.Close();
            }
            catch (Exception) { }
        }

    }
}
