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

namespace SelectPad
{

    //UsbNotification class is used to listen to notifications when USB devices are plugged or unplugged.
    internal static class UsbNotification
    {
        public const int DbtDevicearrival = 0x8000; // system detected a new device        
        public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
        public const int WmDevicechange = 0x0219; // device change event      
        private const int DbtDevtypDeviceinterface = 5;
        private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
        private static IntPtr notificationHandle;

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = GuidDevinterfaceUSBDevice,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        /// <summary>
        /// Unregisters the window for USB device notifications
        /// </summary>
        public static void UnregisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
    }


    public partial class MainWindow : Window
    {
        UIPadsController m_padGui;
        DispatcherTimer timer;
        wgssSTU.UsbDevices m_usbDevices;
        double DPI = 1.0f;
        CONFIG config;

        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_OnClosed;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            m_padGui.SaveConfig(windowHandle, "config.txt");
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            DPI = DpiInfo.GetDpi();
            DPI = 1.0f;

            timer = new DispatcherTimer();
            timer.IsEnabled = false;
            timer.Interval = TimeSpan.FromMilliseconds(350);
            timer.Tick += OnEndResize;

            m_usbDevices = new wgssSTU.UsbDevices();
            m_padGui = new UIPadsController(m_usbDevices, canvas, LayoutRoot, tbar, config, this);

            m_padGui.setMinSizesCanvas();
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            CONFIG config = WindowPlacement.ReadConfig("config.txt");
            WindowPlacement.SetPlacement(windowHandle, config.placement);

            // Adds the windows message processing hook and registers USB device add/removal notification.
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null)
            {
                windowHandle = source.Handle;
                source.AddHook(HwndHandler);
                UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }

        public void RemoveDevice(wgssSTU.IUsbDevice remDev)
        {
            m_padGui.RemoveDevice(remDev);
        }

        public void AddDevice(wgssSTU.IUsbDevice addDev)
        {
            m_padGui.AddDevice(addDev);
        }

        /// <summary>
        /// Method that receives window messages.
        /// </summary>
        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == UsbNotification.WmDevicechange)
            {
                Action fetcher = new Action(updateDevicesList);
                switch ((int)wparam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        fetcher.BeginInvoke(null, null);
                        break;
                    case UsbNotification.DbtDevicearrival:
                        fetcher.BeginInvoke(null, null);
                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }
        
        void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.NewSize.Width < 300)
            {
                Title = "";
            }
            else
            {
                Title = "Pad Select";
            }
            if (null != timer)
            {
                timer.IsEnabled = true;
                timer.Stop();
                timer.Start();
            }
        }

        //The canvas will be resized once the user stops resizing the window
        void OnEndResize(object sender, EventArgs args)
        {
            timer.Stop();
            timer.IsEnabled = false;
            if (null != m_padGui)
            {
                m_padGui.OnResizeCanvas();
            }
        }

        public void updateDevicesList()
        {
            Thread.Sleep(50);
            var newUsbDevices = new wgssSTU.UsbDevices();
            for (int i = 0; i < newUsbDevices.Count; i++)
            {
                bool isNew = true;
                for (int j = 0; j < m_usbDevices.Count; j++)
                {
                    if (m_usbDevices[j].fileName == newUsbDevices[i].fileName)
                    {
                        isNew = false;
                        break;
                    }
                }
                if (isNew)
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action<wgssSTU.IUsbDevice>(AddDevice), newUsbDevices[i]);
                }
            }
            for (int j = 0; j < m_usbDevices.Count; j++)
            {
                bool isRemoved = true;
                for (int i = 0; i < newUsbDevices.Count; i++)
                {
                    if (m_usbDevices[j].fileName == newUsbDevices[i].fileName)
                    {
                        isRemoved = false;
                        break;
                    }
                }
                if (isRemoved)
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action<wgssSTU.IUsbDevice>(RemoveDevice), m_usbDevices[j]);
                }
            }
            m_usbDevices = newUsbDevices;
        }

    }
}