using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace SelectPad
{

    //This class controls an Image so that it plays a gif
    //We could use a MediaElement but that's a much heavier control
    public class RadioGifControl
    {
        RadioButton button;

        Image image;
        DispatcherTimer dispatcherTimer;
        Stream imageStreamSource;
        GifBitmapDecoder decoder;
        List<BitmapSource> bitmapSources;
        int index;

        public RadioGifControl(String path, RadioButton but)
        {
            if (null == but)
            {
                throw new Exception("Error: null button");
            }
            bitmapSources = new List<BitmapSource>();
            button = but;
            imageStreamSource = new FileStream(@path, FileMode.Open, FileAccess.Read, FileShare.Read);
            decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                bitmapSources.Add(decoder.Frames[i]);
            }
            if (decoder.Frames.Count < 1)
            {
                throw new Exception("Error gif with no frames: " + path);
            }
            index = decoder.Frames.Count - 1;
            image = new Image();
            image.SnapsToDevicePixels = true;
            image.UseLayoutRounding = true;
            image.Source = bitmapSources[index];
            but.Content = image;
            //image.Width = bitmapSources[index].Width;
            //image.Height = bitmapSources[index].Height;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimer.Tick += delegate { flip(); };
            //dispatcherTimer.Start();
        }

        private void flip()
        {
            index++;
            if (index >= bitmapSources.Count)
            {
                index = 0;
            }
            image.Source = bitmapSources[index];
        }

        public void Play()
        {
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
            index = decoder.Frames.Count - 1;
            image.Source = bitmapSources[index];
        }

        public bool IsPlaying()
        {
            return dispatcherTimer.IsEnabled;
        }
    }

    //This class controls an Image so that it plays a gif
    //We could use a MediaElement but that's a much heavier control
    public class RibbonGifControl
    {
        RibbonRadioButton m_ribbonButton;
        DispatcherTimer m_dispatcherTimer;
        Stream m_imageStreamSource;
        GifBitmapDecoder m_decoder;
        List<BitmapSource> m_bitmapSources;
        int m_index;

        public RibbonGifControl(String path, RibbonRadioButton im)
        {
            if (null == im)
            {
                throw new Exception("Error: null Image");
            }
            m_bitmapSources = new List<BitmapSource>();
            m_ribbonButton = im;
            m_imageStreamSource = new FileStream(@path, FileMode.Open, FileAccess.Read, FileShare.Read);
            m_decoder = new GifBitmapDecoder(m_imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            for (int i = 0; i < m_decoder.Frames.Count; i++)
            {
                m_bitmapSources.Add(m_decoder.Frames[i]);
            }
            if (m_decoder.Frames.Count < 1)
            {
                throw new Exception("Error gif with no frames: " + path);
            }
            m_index = m_decoder.Frames.Count - 1;
            m_ribbonButton.SmallImageSource = m_bitmapSources[m_index];
            //image.Width = bitmapSources[index].Width;
            //image.Height = bitmapSources[index].Height;

            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            m_dispatcherTimer.Tick += delegate { flip(); };
        }

        private void flip()
        {
            m_index++;
            if (m_index >= m_bitmapSources.Count)
            {
                m_index = 0;
            }
            m_ribbonButton.SmallImageSource = m_bitmapSources[m_index];
        }

        public void Play()
        {
            m_dispatcherTimer.Start();
        }

        public void Stop()
        {
            m_dispatcherTimer.Stop();
            m_index = m_decoder.Frames.Count - 1;
            m_ribbonButton.SmallImageSource = m_bitmapSources[m_index];
        }

        public bool IsPlaying()
        {
            return m_dispatcherTimer.IsEnabled;
        }
    }

}
