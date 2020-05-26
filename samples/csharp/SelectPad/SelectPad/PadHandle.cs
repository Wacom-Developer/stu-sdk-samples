using System;
using System.Windows;
using System.Reflection;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace SelectPad
{

    //PadHandle is the main class that deals with the Pad device itself, the hardware
    //The public methods are the ones that can be called by the UI/Main thread
    //The private methods are executed by the thread/dispatcher associated to the corresponding Pad ,
    //The public methods invoke the private ones in a transparent way to the user of this class
    //The private methods end with the letter B, indicating that they are executed in a background thread
    public class PadHandle
    {
        private BackgroundTaskManager background;
        private wgssSTU.Tablet m_tablet;
        private wgssSTU.IUsbDevice m_device;
        private wgssSTU.encodingMode m_encodingMode;
        private UIPadsController m_controller;
        private int m_num = 0;
        //This timer is used to try to execute an order again when it fails the first time
        //We only allow one order to be the queue of this timer
        private DispatcherTimer dispatcherTimer;

        public PadHandle(UIPadsController c, wgssSTU.IUsbDevice device, AppState s, int i)
        {
            if (null == c)
            {
                throw new Exception("Error: no pad controller on PadHandle constructor");
            }
            m_controller = c;
            background = new BackgroundTaskManager();
            //This has to be synchronous/blocking:
            background.Dispatcher.Invoke(new Action<wgssSTU.IUsbDevice, AppState, int>(ConstructorB), device, s, i);
            if (AppState.Identification_Mode == s)
            {
                background.Dispatcher.BeginInvoke(new Action<int>(waitandIdB), i);
            }
        }

        private void ConstructorB(wgssSTU.IUsbDevice device, AppState s, int i)
        {
            m_num = i;
            m_tablet = new wgssSTU.Tablet();
            m_device = device;
            m_encodingMode = wgssSTU.encodingMode.EncodingMode_1bit;
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.DataBind, background.Dispatcher);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
        }

        private void waitandIdB(int i)
        {
            int millisec = 200;
            switch ((UIPadHandle.ProductId)m_device.idProduct)
            {
                case UIPadHandle.ProductId.ProductId_300:
                    millisec = 700;
                    break;
                case UIPadHandle.ProductId.ProductId_430:
                    millisec = 700;
                    break;
                case UIPadHandle.ProductId.ProductId_530:
                    millisec = 1500;
                    break;
                default:
                    break;
            }
            System.Threading.Thread.Sleep(millisec);
            IdentifyPadB(i);
        }

        //RemoveHandlers removes all the elements on the dispatchTimer execution queue
        private void RemoveHandlers(DispatcherTimer dispatchTimer)
        {
            var eventField = dispatchTimer.GetType().GetField("Tick",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            var eventDelegate = (Delegate)eventField.GetValue(dispatchTimer);
            if (null != eventDelegate)
            {
                var invocatationList = eventDelegate.GetInvocationList();
                if (null != invocatationList)
                {
                    foreach (var handler in invocatationList)
                    {
                        if (null != handler)
                        {
                            dispatchTimer.Tick -= ((EventHandler)handler);
                        }
                    }
                }
            }
        }

        public void IdentifyPad(int i)
        {
            background.Dispatcher.BeginInvoke(new Action<int>(IdentifyPadB), i);
        }

        private void IdentifyPadB(int i)
        {
            bool previousWarning = false;
            m_num = i;
            if (dispatcherTimer.IsEnabled)
            {
                previousWarning = true;
                dispatcherTimer.Stop();
            }
            if (!isConnected())
            {
                connectB(); //Connect synchronously
            }
            if (showPadNumberOnPad(i))
            {
                /*if (true == isSelected) //we keep all pads connected on Identifying mode
                {
                    disconnectB(); //Disconnect synchronously
                }*/
                if (previousWarning)
                {
                    SetWarning(false);
                }
            }
            else
            {
                if (DebugInfo.IsDebuggerAttached())
                {
                    MessageBoxResult result = MessageBox.Show("Error showing Id on Pad",
                        "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                }
                RemoveHandlers(dispatcherTimer);
                dispatcherTimer.Tick += delegate { IdentifyPadB(i); };
                dispatcherTimer.Start();
                if (!previousWarning)
                {
                    SetWarning(true);
                }
                //IdentifyPadB(i);
            }
        }

        public bool isConnected()
        {
            bool ret = false;
            if (null != m_tablet)
            {
                ret = m_tablet.isConnected();
            }
            return ret;
        }


        public void connect()
        {
            if (null != m_tablet)
            {
                background.Dispatcher.BeginInvoke(new Func<bool, bool>(connectB), true);
            }
        }

        private bool connectB(bool repeat = false)
        {
            bool previousWarning = false;
            bool ret = false; //return false means everything is OK, we have connected (if the tablet exists)
            if (dispatcherTimer.IsEnabled)
            {
                previousWarning = true;
                dispatcherTimer.Stop();
            }
            if (null != m_tablet)
            {
                if (!m_tablet.isConnected())
                {
                    wgssSTU.IErrorCode ec = null;
                    for (int n = 0; n < 5; n++)
                    {
                        try
                        {
                            ec = m_tablet.usbConnect(m_device, true);
                            if (ec != null && ec.value == 0)
                            {
                                n = 5;
                                break;
                            }
                            else if (ec != null && ec.value != 0 && n >= 4)
                            {
                                if (DebugInfo.IsDebuggerAttached())
                                {
                                    MessageBoxResult result = MessageBox.Show("Error connecting to a Pad. Error value: " + ec.value + " " + ec.message,
                                        "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            if (ec != null && ec.value != 0 && n == 4) //ec.value == 32 means it's already being used by another program.
                            {
                                if (DebugInfo.IsDebuggerAttached())
                                {
                                    MessageBoxResult result = MessageBox.Show("Error connecting to a Pad. Error value: " + ec.value + " " + ec.message,
                                        "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                                }
                            }
                        }
                    }
                }

                if (!m_tablet.isConnected())
                {
                    ret = true; //We couldn't connect
                    if (repeat)
                    {
                        RemoveHandlers(dispatcherTimer);
                        dispatcherTimer.Tick += delegate { connectB(repeat); };
                        dispatcherTimer.Start();

                        if (!previousWarning)
                        {
                            SetWarning(true);
                        }
                    }
                }
                else
                {
                    if (previousWarning)
                    {
                        SetWarning(false);
                    }
                }
            }
            return ret;
        }

        private void SetWarning(bool set)
        {
            background.OwnerDispatcher.BeginInvoke(new Action<int, bool>(m_controller.SetWarning), m_num, set);
        }


        public void disconnect()
        {
            if (null != m_tablet)
            {
                background.Dispatcher.BeginInvoke(new Func<bool, bool>(disconnectB), true);
            }
        }

        private bool disconnectB(bool repeat = false)
        {
            bool previousWarning = false;
            if (dispatcherTimer.IsEnabled)
            {
                previousWarning = true;
                dispatcherTimer.Stop();
            }
            bool ret = false; //this means everything is OK
            if (null != m_tablet)
            {
                if (m_tablet.isConnected())
                {
                    try
                    {
                        m_tablet.disconnect();
                        if (previousWarning)
                        {
                            SetWarning(false);
                        }
                    }
                    catch (Exception e)
                    {
                        ret = true;
                        if (DebugInfo.IsDebuggerAttached())
                        {
                            MessageBoxResult result = MessageBox.Show("Error disconnecting from a Pad. Exception: " + e.ToString(),
                                "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        if (repeat)
                        {
                            RemoveHandlers(dispatcherTimer);
                            dispatcherTimer.Tick += delegate { disconnectB(repeat); };
                            dispatcherTimer.Start();
                            if (!previousWarning)
                            {
                                SetWarning(true);
                            }
                        }
                    }

                }
                else
                {
                    if (previousWarning)
                    {
                        SetWarning(false);
                    }
                }
            }
            return ret;
        }

        private bool showPadNumberOnPad(int i)
        {
            bool ret = false; //return false means everything went OK
            //Show number on the Pad
            try
            {
                if (isConnected())
                {
                    //Create bitmap
                    wgssSTU.ICapability m_capability = m_tablet.getCapability();
                    wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();
                    System.Drawing.Bitmap m_bitmap = new System.Drawing.Bitmap(m_capability.screenWidth, m_capability.screenHeight);

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(m_bitmap);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    g.FillRectangle(System.Drawing.Brushes.White, 0, 0, m_bitmap.Width, m_bitmap.Height);

                    System.Drawing.StringFormat strFormat = new System.Drawing.StringFormat();
                    strFormat.Alignment = System.Drawing.StringAlignment.Center;
                    strFormat.LineAlignment = System.Drawing.StringAlignment.Center;

                    double dpi = DpiInfo.GetDpi();

                    g.DrawString("" + i,
                                    new System.Drawing.Font("Tahoma", (int)(m_bitmap.Height / (1.2 * dpi))), System.Drawing.Brushes.Black,
                                    new System.Drawing.RectangleF(0, 0, m_bitmap.Width, m_bitmap.Height),
                                    strFormat);
                    g.Dispose();

                    //Convert bitmap to device-native format
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    m_bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] m_bitmapData = (byte[])protocolHelper.resizeAndFlatten(stream.ToArray(), 0, 0, (uint)m_bitmap.Width, (uint)m_bitmap.Height, m_capability.screenWidth, m_capability.screenHeight, (byte)m_encodingMode, wgssSTU.Scale.Scale_Fit, 0, 0);

                    // Write image. note: There is no need to clear the tablet screen prior to writing an image.
                    ret = writeImageB(i, (byte)m_encodingMode, m_bitmapData);

                    protocolHelper = null;
                    stream.Dispose();
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }

        private bool writeImageB(int i, byte encodingMode, Array bitmapData)
        {
            bool ret = false; //return false means everything went OK
            if (null != m_tablet)
            {
                if (m_tablet.isConnected())
                {
                    try
                    {
                        m_tablet.writeImage(encodingMode, bitmapData);
                        ret = true;
                    }
                    catch (Exception e)
                    {
                        ret = false;
                        if (DebugInfo.IsDebuggerAttached())
                        {
                            MessageBoxResult result = MessageBox.Show("Error writing Image " + e.ToString(), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                    }
                }
            }
            return ret;
        }

        public void ClearPad(bool isSelected)
        {
            if (null != m_tablet)
            {
                background.Dispatcher.BeginInvoke(new Action<bool>(ClearPadB), isSelected);
            }
        }

        private void ClearPadB(bool isSelected)
        {
            bool previousWarning = false;
            bool ret = false; //false means everything went OK
            if (dispatcherTimer.IsEnabled)
            {
                previousWarning = true;
                dispatcherTimer.Stop();
            }
            if (!m_tablet.isConnected())
            {
                ret |= connectB();
            }
            try
            {
                m_tablet.setClearScreen();
            }
            catch (Exception e)
            {
                ret = true;
                if (DebugInfo.IsDebuggerAttached())
                {
                    MessageBoxResult result = MessageBox.Show("Error clearing Pad screen. Error value: " + e.ToString(), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                }
            }
            if (isSelected)
            {
                ret |= disconnectB();
            }
            if (ret)
            {
                RemoveHandlers(dispatcherTimer);
                dispatcherTimer.Tick += delegate { ClearPadB(isSelected); };
                dispatcherTimer.Start();
                if (!previousWarning)
                {
                    SetWarning(true);
                }
            }
            else
            {
                if (previousWarning)
                {
                    SetWarning(false);
                }
            }
        }

        public uint getUID() //be careful, this is a blocking operation
        {
            return (uint)background.Dispatcher.Invoke(new Func<uint>(getUIDB), null);
        }

        private uint getUIDB() //be careful, this is a blocking operation
        {
            uint ret = 0;
            try
            {
                ret = m_tablet.getUid();
            }
            catch (Exception) { }
            return ret;
        }

        public ushort getProductId()
        {
            ushort ret = 0;
            if (null != m_device)
            {
                ret = m_device.idProduct;
            }
            else
            {
                if (DebugInfo.IsDebuggerAttached())
                {
                    MessageBoxResult result = MessageBox.Show("Error getting getProductId ", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                }
            }
            return ret;
        }

        public void removeDevice(int i, List<PadHandle> list)
        {
            if (null != m_tablet)
            {
                background.Dispatcher.BeginInvoke(new Action<int, List<PadHandle>>(removeDeviceB), i, list);
            }
        }

        private void removeDeviceB(int i, List<PadHandle> list)
        {
            if (null != dispatcherTimer)
            {
                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                }
            }
            RemoveHandlers(dispatcherTimer);
            if (null != m_tablet)
            {
                if (m_tablet.isConnected())
                {
                    m_tablet.disconnect();
                }
                list.RemoveAt(i);
                background.Dispose();
            }
        }
    }
}
