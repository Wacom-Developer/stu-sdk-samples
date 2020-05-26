#include "DemoButtons.h"
#include <WacomGSS/STU/Tablet_OpenSSL.hpp>
#include <WacomGSS/STU/ReportHandler.hpp>
#include <WacomGSS/STU/getUsbDevices.hpp>

#include <WacomGSS/Win32/com.hpp>
#include <WacomGSS/Win32/d2d1.hpp>
#include <WacomGSS/Win32/dwrite.hpp>
#include <WacomGSS/setThreadName.hpp>


#include <sstream>

#ifndef WacomGSS_NoZLIB
#include <zlib.h>
#endif

//   tablet: the raw tablet coordinate
//   screen: the tablet LCD screen
//   client: the Form window client area

#define SIG_IMAGE_NUM 1


typedef enum tagMONITOR_DPI_TYPE
{
  MDT_Effective_DPI = 0
} MONITOR_DPI_TYPE;

typedef HRESULT (WINAPI *pfn_GetDPIForMonitor)(HMONITOR, MONITOR_DPI_TYPE, UINT *, UINT *);
#define WM_DPICHANGED       0x02E0



float clamp(float v, float min, float max)
{
  if (v<min) v = min;
  if (v>max) v = max;
  return v;
}



D2D1_COLOR_F blend(D2D1_COLOR_F src, D2D1_COLOR_F dst)
{
  D2D1_COLOR_F out;
  if (dst.a == 1.0f)
  {
    float da = dst.a * (1 - src.a);
    out.a = 1.0f;
    out.r = src.r * src.a + dst.r * da;
    out.g = src.g * src.a + dst.g * da;
    out.b = src.b * src.a + dst.b * da;
  }
  else
  {
    out.a = src.a + dst.a*(1 - src.a);
    if (out.a != 0)
    {
      float da = dst.a * (1 - src.a);
      out.r = (src.r * src.a + dst.r * da) / out.a;
      out.g = (src.g * src.a + dst.g * da) / out.a;
      out.b = (src.b * src.a + dst.b * da) / out.a;
    }

    else
    {
      out.r = out.g = out.b = 0;
    }
  }
  return out;
}



/*constexpr*/ uint16_t rgb16_565(D2D1_COLOR_F value)
{
  value.r = clamp(value.r, 0.0f, 1.0f);
  value.g = clamp(value.g, 0.0f, 1.0f);
  value.b = clamp(value.b, 0.0f, 1.0f);
  uint16_t r = static_cast<uint16_t>(value.r * 31.0f);
  uint16_t g = static_cast<uint16_t>(value.g * 63.0f);
  uint16_t b = static_cast<uint16_t>(value.b * 31.0f);
  return
    static_cast<uint16_t>
    (
        (r << 11) // red
      | (g <<  5) // green
      | (b      ) // blue
    )
  ;
}



//==============================================================================


struct Factory
{
  WacomGSS::Win32::com_ptr<ID2D1Factory>       m_pID2D1Factory; // Factory for creating all D2D1 objects
  WacomGSS::Win32::com_ptr<IWICImagingFactory> m_pIWICImagingFactory; // Factory for creating WIC bitmaps
  WacomGSS::Win32::com_ptr<IDWriteFactory>     m_pIDWriteFactory;
};

class SignatureForm : Factory, WacomGSS::STU::ProtocolHelper::ReportHandler
{
  struct TextArea
  {
    D2D1_RECT_F boundsScreen;
    D2D1_RECT_F boundsClient;
    std::wstring text;
    void (SignatureForm::* onClick)();
  };

  struct Button : TextArea
  {
    void (SignatureForm::* onClick)();
  };
  
  struct RenderData
  {
    WacomGSS::STU::ProtocolHelper::InkState inkState; // determined by original pressure and ink threshold
    uint16_t x;
    uint16_t y;
    float    pressure; // normalized pressure value

    D2D1_POINT_2F client; // calculated pixel coordinates
  };


  HWND                                       m_hwnd;
  HINSTANCE                                  m_hInstance;
  bool                                       m_isTls;
  WacomGSS::STU::Tablet                      m_tablet;
  WacomGSS::STU::Protocol::Capability        m_capability;
  WacomGSS::STU::ProtocolHelper::InkingState m_inkingState;
  
  std::vector<Button>                        m_btns;
  
  TextArea                              m_why;
  std::vector<uint8_t>                  m_bitmapWhy;
  WacomGSS::STU::Protocol::Rectangle    m_bitmapWhyArea;
  WacomGSS::STU::Protocol::EncodingMode m_bitmapWhyEncodingMode;
  bool                                  m_forceMonoWhy;

  // bitmap sent to screen
  WacomGSS::STU::Protocol::EncodingMode m_encodingMode;
  std::vector<uint8_t>                  m_bitmap;
  
  WacomGSS::Win32::com_ptr<IWICBitmap>  m_pBitmap540;

  // pen data collected
  std::deque<RenderData> m_penData;  // The stored pendata for rendering

  size_t                 m_renderIndex; // Currently rendered up to this point
  int                    m_isDown;

  // data needed to render to client.
  D2D1_SIZE_F                                     m_renderDataScale;  // cached size of client window
  FLOAT                                           m_renderInkScale;       // used for inkwidth  
  //WacomGSS::Win32::com_ptr<IDWriteTextFormat>     m_textFormatWhy;
 // WacomGSS::Win32::com_ptr<IDWriteTextFormat>     m_textFormatBtn;

  // D2D1 device dependent data.
  WacomGSS::Win32::com_ptr<ID2D1HwndRenderTarget> m_renderTarget;
  WacomGSS::Win32::com_ptr<ID2D1SolidColorBrush>  m_penInk;
  WacomGSS::Win32::com_ptr<ID2D1SolidColorBrush>  m_textBrush;

  // data used for receiving the data from the tablet and queuing it for processing on the primary thread.  
  std::uint32_t                     m_sessionId;  
  std::thread                       m_reader;
  WacomGSS::STU::InterfaceQueue     m_queue; // buffer between Tablet class and this application
  WacomGSS::mutex                                               m_mutex;
  std::deque<WacomGSS::STU::Protocol::PenDataTimeCountSequence> m_readerQueue; // buffer between our background and primary threads.
  std::exception_ptr                                            m_readerException;
  bool                          m_useSigMode;

  // Windows 8.1 per-monitor DPI 
  WacomGSS::Win32::HModule      m_shcore;
  pfn_GetDPIForMonitor          m_pGetDPIForMonitor; 
   

  void getDPIForMonitor(FLOAT & dpiX, FLOAT & dpiY)
  {
    if (m_pGetDPIForMonitor)
    {
      // Use Windows 8.1 API

      HMONITOR hMonitor = MonitorFromWindow(m_hwnd, MONITOR_DEFAULTTONEAREST);
  
      using namespace WacomGSS::Win32;
      
      UINT ui_dpiX = 0, ui_dpiY = 0;
      hresult_succeeded(m_pGetDPIForMonitor(hMonitor, MDT_Effective_DPI, &ui_dpiX, &ui_dpiY), "GetDPIForMonitor");  
      
      dpiX = static_cast<FLOAT>(ui_dpiX);
      dpiY = static_cast<FLOAT>(ui_dpiY);
    }
    else
    {
      HDC hdc = GetDC(m_hwnd);
      int i_dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
      int i_dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
      ReleaseDC(m_hwnd, hdc);
  
      dpiX = static_cast<FLOAT>(i_dpiX);
      dpiY = static_cast<FLOAT>(i_dpiY);
    }
  }


  void discardDeviceDependentResources()
  {
    m_renderTarget = nullptr;    
    m_penInk       = nullptr;
    m_textBrush    = nullptr;    
  }


  template<typename T>
  void calculateWhyPosition(D2D1_RECT_F TextArea::* bounds)
  {
    FLOAT r, b;
 
    if (m_tablet.getProductId() == WacomGSS::STU::ProductId_300)
    {
      r = static_cast<FLOAT>((m_btns[0].*bounds).left) - 1.0f;
      b = static_cast<FLOAT>((m_btns[2].*bounds).bottom);
    }
    else
    {
      r = static_cast<FLOAT>((m_btns[2].*bounds).right);
      b = static_cast<FLOAT>((m_btns[0].*bounds).top) -1.0f;
    }

    m_why.*bounds = D2D1::RectF(0, 0, r, b);
  }
  

  template<typename T>
  void calculateButtonPositions(D2D1_SIZE_F renderTargetSize, D2D1_RECT_F TextArea::* bounds)
  {
    // While we want the button positions to look similar, ensure that 
    // the screen positions are integer while the client positions can be float.
    using namespace WacomGSS::Win32;
        
    T width  = static_cast<T>(renderTargetSize.width);
    T height = static_cast<T>(renderTargetSize.height);

    if (m_useSigMode)
    {
      // 800x480
      float xr = width / 800.0f;
      float yr = height / 480.0f;

      float y0 = 431 * yr;
      float x1 = 265 * xr;
      float y1 = 48 * yr;
      
      m_btns[0].*bounds = D2D1::RectF(  0*xr, y0, x1, y1);
      m_btns[1].*bounds = D2D1::RectF(266*xr, y0, x1, y1);
      m_btns[2].*bounds = D2D1::RectF(532*xr, y0, x1, y1);
    }
    else if (m_tablet.getProductId() != WacomGSS::STU::ProductId_300)
    {
      // Place the buttons across the bottom of the screen.

      FLOAT w2 = static_cast<FLOAT>(width / static_cast<T>(3));
      FLOAT w3 = static_cast<FLOAT>(width / static_cast<T>(3));
      FLOAT w1 = width - w2 - w3;
      FLOAT y  = static_cast<FLOAT>(height * 6 / static_cast<T>(7));
      FLOAT h  = height - y;
            
      m_btns[0].*bounds = D2D1::RectF(0    , y, w1, h);
      m_btns[1].*bounds = D2D1::RectF(w1   , y, w2, h);
      m_btns[2].*bounds = D2D1::RectF(w1+w2, y, w3, h);      
    }
    else
    {
      // The STU-300 is very shallow, so it is better to utilise
      // the buttons to the side of the display instead.

      FLOAT x = static_cast<FLOAT>(width * 3 / static_cast<T>(4));
      FLOAT w = width - x;

      FLOAT h2 = static_cast<FLOAT>(height / static_cast<T>(3));
      FLOAT h3 = h2;
      FLOAT h1 = height - h2 - h3;

      m_btns[0].*bounds = D2D1::RectF(x, 0 , w, h1);
      m_btns[1].*bounds = D2D1::RectF(x, h1, w, h2);
      m_btns[2].*bounds = D2D1::RectF(x, h1+h2, w, h3);
    }
    
    // Convert width/height to right/bottom
    // Note that -0.5 is required in order to deal with the +0.5 that 
    // is added for pixel alignment for the screen bitmap.
    for (auto i = m_btns.begin(); i != m_btns.end(); ++i)
    {
      (*i.*bounds).right  = static_cast<FLOAT>(static_cast<T>((*i.*bounds).left + (*i.*bounds).right  - 0.5f));
      (*i.*bounds).bottom = static_cast<FLOAT>(static_cast<T>((*i.*bounds).top  + (*i.*bounds).bottom - 0.5f));
    }       
  }



  void renderWhy_Screen(ID2D1RenderTarget * renderTarget)
  {
    using namespace WacomGSS::Win32;
    //auto b = D2D1::RectF(m_why.boundsScreen.left+0.5f, m_why.boundsScreen.top+0.5f, m_why.boundsScreen.right+0.5f, m_why.boundsScreen.bottom+0.5f);
    auto b = D2D1::RectF(m_why.boundsScreen.left, m_why.boundsScreen.top, m_why.boundsScreen.right, m_why.boundsScreen.bottom);
    auto w = b.right - b.left +1;
    auto h = b.bottom - b.top +1;

    auto textFormat = createWhyFont(&Button::boundsScreen);

    com_ptr<IDWriteTextLayout> textLayout;
    hresult_succeeded
    (
      m_pIDWriteFactory->CreateTextLayout(
        m_why.text.data(),      // The string to be laid out and formatted.
        m_why.text.size(),  // The length of the string.
        textFormat.get(),  // The text format to apply to the string (contains font information, etc).
        w,         // The width of the layout box.
        h,        // The height of the layout box.
        &textLayout  // The IDWriteTextLayout interface pointer.
      )
    );

    DWRITE_TEXT_METRICS textMetrics;
    hresult_succeeded
    (
      textLayout->GetMetrics(&textMetrics)
    );
 
    D2D1_RECT_F rc;
    rc.left = 0;
    rc.top = 0;
    rc.right = textMetrics.width;
    rc.bottom = textMetrics.height;
  
    com_ptr<ID2D1SolidColorBrush> fillBrush;
    hresult_succeeded
    (
      renderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::White), &fillBrush)
    );
     renderTarget->FillRectangle(&rc, fillBrush.get());

    WacomGSS::Win32::com_ptr<ID2D1SolidColorBrush>  textBrush;
    hresult_succeeded
    (
      renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &textBrush)
    );

    renderTarget->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_ALIASED);
    renderTarget->DrawTextLayout(D2D1::Point2F(0.0f,0.0f), textLayout.get(), textBrush.get(), D2D1_DRAW_TEXT_OPTIONS_CLIP);

    m_bitmapWhyArea.upperLeftXpixel  = 0;
    m_bitmapWhyArea.upperLeftYpixel  = 0;
    m_bitmapWhyArea.lowerRightXpixel = textMetrics.width-1;
    m_bitmapWhyArea.lowerRightYpixel = textMetrics.height-1;
  }



  void renderWhy_Client(ID2D1RenderTarget * renderTarget)
  {
    using namespace WacomGSS::Win32;

    auto textFormat = createWhyFont(&Button::boundsClient);

    auto b = D2D1::RectF(m_why.boundsClient.left+0.5f, m_why.boundsClient.top+0.5f, m_why.boundsClient.right+0.5f, m_why.boundsClient.bottom+0.5f);
    auto w = b.right - b.left +1;
    auto h = b.bottom - b.top +1;

    com_ptr<IDWriteTextLayout> textLayout;
    hresult_succeeded
    (
      m_pIDWriteFactory->CreateTextLayout(
        m_why.text.data(),      // The string to be laid out and formatted.
        m_why.text.size(),  // The length of the string.
        textFormat.get(),  // The text format to apply to the string (contains font information, etc).
        w,         // The width of the layout box.
        h,        // The height of the layout box.
        &textLayout  // The IDWriteTextLayout interface pointer.
      )
    );

    DWRITE_TEXT_METRICS textMetrics;
    hresult_succeeded
    (
      textLayout->GetMetrics(&textMetrics)
    );
  
    D2D1_RECT_F rc;
    rc.left = 0;
    rc.top = 0;
    rc.right = textMetrics.width;
    rc.bottom = textMetrics.height;
  
    com_ptr<ID2D1SolidColorBrush> fillBrush;
    hresult_succeeded
    (
      renderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::White), &fillBrush)
    );
    renderTarget->FillRectangle(&rc, fillBrush.get());

    WacomGSS::Win32::com_ptr<ID2D1SolidColorBrush>  textBrush;
    hresult_succeeded
    (
      renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &textBrush)
    );

    renderTarget->FillRectangle(&rc, fillBrush.get());

    renderTarget->DrawTextLayout(D2D1::Point2F(0,0), textLayout.get(), textBrush.get(), D2D1_DRAW_TEXT_OPTIONS_CLIP);
  }




  void renderBackground_Screen(ID2D1RenderTarget * renderTarget)
  {
    bool useColor = m_encodingMode != WacomGSS::STU::Protocol::EncodingMode_1bit;
    
    using namespace WacomGSS::Win32;

    auto textFormat = createButtonFont(&Button::boundsScreen);

    com_ptr<ID2D1SolidColorBrush> fillBrush;
    if (useColor)
    {
      hresult_succeeded
      (
        renderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::LightGray), &fillBrush)
      );
    }

    com_ptr<ID2D1SolidColorBrush> blackBrush;
    hresult_succeeded
    (
      renderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::Black), &blackBrush)
    );  

    renderTarget->Clear(D2D1::ColorF(D2D1::ColorF::White));
    if (!useColor)
    {
      renderTarget->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_ALIASED);
    }

    for (auto i = m_btns.begin(); i != m_btns.end(); ++i)
    {   
      auto b = D2D1::RectF(i->boundsScreen.left+0.5f, i->boundsScreen.top+0.5f, i->boundsScreen.right+0.5f, i->boundsScreen.bottom+0.5f);
      
      if (useColor)
      {        
        renderTarget->FillRectangle(b, fillBrush.get());
      }
      renderTarget->DrawRectangle(b, blackBrush.get());        
      renderTarget->DrawText(i->text.data(), i->text.length(), textFormat.get(), b, blackBrush.get(), D2D1_DRAW_TEXT_OPTIONS_CLIP, DWRITE_MEASURING_MODE_NATURAL);      
    }    
  }
  

  void renderBackground_Client(ID2D1RenderTarget * renderTarget)
  {
    using namespace WacomGSS::Win32;

    auto textFormat = createButtonFont(&Button::boundsClient);

    if (!m_useSigMode)
    {
      com_ptr<ID2D1SolidColorBrush> fillBrush;
      hresult_succeeded
      (
        renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::LightGray), &fillBrush)
      );

      renderTarget->Clear(D2D1::ColorF(D2D1::ColorF::White));

      for (auto i = m_btns.begin(); i != m_btns.end(); ++i)
      {
        renderTarget->FillRectangle(i->boundsClient, fillBrush.get());
        renderTarget->DrawRectangle(i->boundsClient, m_textBrush.get());
        renderTarget->DrawText(i->text.data(), i->text.length(), textFormat.get(), i->boundsClient, m_textBrush.get(), D2D1_DRAW_TEXT_OPTIONS_CLIP, DWRITE_MEASURING_MODE_NATURAL);
      }

    }
    else
    {
      WacomGSS::Win32::com_ptr<ID2D1Bitmap> pBmp;
      WacomGSS::Win32::hresult_succeeded(renderTarget->CreateBitmapFromWicBitmap(m_pBitmap540.get(), &pBmp));

      D2D1_SIZE_F size = renderTarget->GetSize();

      renderTarget->DrawBitmap(pBmp.get(), D2D1_RECT_F{ 0, 0, size.width, size.height });
    }


    if (m_sessionId)
    {
      hresult_succeeded
      (
        textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
      );
      hresult_succeeded
      (
        textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_FAR)
      );

      auto size = m_why.boundsClient; //ScreenrenderTarget->GetSize();
      // Unicode LOCK = U+1F512 / UTF-8 = F0 9F 94 92 / UTF-16 = D83D DD12
      renderTarget->DrawText(L"\xd83d\xdd12", 2, textFormat.get(), &size, m_textBrush.get(), D2D1_DRAW_TEXT_OPTIONS_CLIP, DWRITE_MEASURING_MODE_NATURAL);

      hresult_succeeded
      (
        textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
      );
      hresult_succeeded
      (
        textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
      );
    }

  }


  void setRenderDataScale()
  {
    auto s = m_renderTarget->GetSize();
    m_renderDataScale.width  = s.width  / m_capability.tabletMaxX;
    m_renderDataScale.height = s.height / m_capability.tabletMaxY;      
    m_renderInkScale = ((s.width / m_capability.screenWidth) + (s.height / m_capability.screenHeight)) / 2.0f;
  }


  void createBitmap2()
  {
    using namespace WacomGSS;

    calculateWhyPosition<int>(&Button::boundsScreen);

    auto width = m_why.boundsScreen.right- m_why.boundsScreen.left+1;
    auto height = m_why.boundsScreen.bottom- m_why.boundsScreen.top+1;


    Win32::com_ptr<IWICBitmap> pWICBitmap;
    Win32::hresult_succeeded(m_pIWICImagingFactory->CreateBitmap(width, height, GUID_WICPixelFormat32bppBGR, WICBitmapCacheOnDemand, &pWICBitmap));

    Win32::com_ptr<ID2D1RenderTarget> renderTarget;
    {
      auto renderTargetProperties = D2D1::RenderTargetProperties();
      renderTargetProperties.dpiX = renderTargetProperties.dpiY = 96.0f; // force bitmap to be pixel == DIP to ensure 1:1 pixel mapping.
      Win32::hresult_succeeded(m_pID2D1Factory->CreateWicBitmapRenderTarget(pWICBitmap.get(), renderTargetProperties, &renderTarget));
    }

    renderTarget->BeginDraw();
    renderWhy_Screen(renderTarget.get());
    Win32::hresult_succeeded(renderTarget->EndDraw());

    WICRect rc;
    rc.X= 0;
    rc.Y=0;
    rc.Width = m_bitmapWhyArea.lowerRightXpixel + 1;
    rc.Height = m_bitmapWhyArea.lowerRightYpixel + 1;
    Win32::com_ptr<IWICBitmapClipper> pWICBitmapClipper;
    Win32::hresult_succeeded(m_pIWICImagingFactory->CreateBitmapClipper(&pWICBitmapClipper));
    Win32::hresult_succeeded
    (
      pWICBitmapClipper->Initialize(pWICBitmap.get(), &rc)
    );
    m_bitmapWhyEncodingMode = m_forceMonoWhy ? STU::Protocol::EncodingMode_1bit : m_encodingMode;

    m_bitmapWhy = WacomGSS::STU::ProtocolHelper::flatten(m_pIWICImagingFactory.get(), pWICBitmapClipper.get(), rc.Width, rc.Height, m_bitmapWhyEncodingMode);
#ifndef WacomGSS_NoZLIB
    {
      uLongf                compressedLength = 2 * m_bitmapWhy.size();
      std::vector<uint8_t>  compressed(compressedLength);
      int                   err = compress2(&compressed[0], &compressedLength, &m_bitmapWhy[0], m_bitmapWhy.size(), Z_BEST_COMPRESSION);

      if (err != Z_OK) {
        throw std::runtime_error("zlib compression failed");
      }
      compressed.resize(compressedLength);
      std::swap(m_bitmapWhy, compressed);  
      m_bitmapWhyEncodingMode = STU::Protocol::EncodingMode(m_bitmapWhyEncodingMode|STU::Protocol::EncodingMode_Zlib);
    }
#endif

  }

  void createBitmap()
  {
    using namespace WacomGSS;

    Win32::com_ptr<IWICBitmap> pWICBitmap;
    if (!m_useSigMode)
    {
      Win32::hresult_succeeded(m_pIWICImagingFactory->CreateBitmap(m_capability.screenWidth, m_capability.screenHeight, GUID_WICPixelFormat32bppBGR, WICBitmapCacheOnDemand, &pWICBitmap));

      {
        auto encodingFlag = STU::ProtocolHelper::simulateEncodingFlag(m_tablet.getProductId(), m_capability.encodingFlag);

        if (encodingFlag & STU::Protocol::EncodingFlag_24bit)
        {
          m_encodingMode = m_tablet.supportsWrite() ? STU::Protocol::EncodingMode_24bit_Bulk : STU::Protocol::EncodingMode_24bit;
        }
        else if (encodingFlag & STU::Protocol::EncodingFlag_16bit)
        {
          m_encodingMode = m_tablet.supportsWrite() ? STU::Protocol::EncodingMode_16bit_Bulk : STU::Protocol::EncodingMode_16bit;
        }
        else
        {
          m_encodingMode = STU::Protocol::EncodingMode_1bit;
        }
      }
    }
    else
    {
      Win32::com_ptr<IWICStream> pIWICStream = WacomGSS::ut::IWICStreamFromResource(m_hInstance, MAKEINTRESOURCE(IDB_SIGSCRN_DISPLAY), RT_RCDATA, m_pIWICImagingFactory);
      Win32::com_ptr<IWICBitmapDecoder> pIWICBitmapDecoder;
      Win32::hresult_succeeded(m_pIWICImagingFactory->CreateDecoderFromStream(pIWICStream.get(), nullptr, WICDecodeMetadataCacheOnDemand, &pIWICBitmapDecoder));
      Win32::com_ptr<IWICBitmapFrameDecode> pWICBitmapSource;
      Win32::hresult_succeeded(pIWICBitmapDecoder->GetFrame(0, &pWICBitmapSource));
      Win32::com_ptr<IWICFormatConverter> pWICFormatConverter;
      Win32::hresult_succeeded(m_pIWICImagingFactory->CreateFormatConverter(&pWICFormatConverter));      
      Win32::hresult_succeeded(pWICFormatConverter->Initialize(pWICBitmapSource.get(), GUID_WICPixelFormat32bppPBGRA, WICBitmapDitherTypeNone, nullptr, 0, WICBitmapPaletteTypeMedianCut));
      Win32::hresult_succeeded(m_pIWICImagingFactory->CreateBitmapFromSource(pWICFormatConverter.get(), WICBitmapCacheOnDemand, &pWICBitmap));

      m_encodingMode = m_tablet.supportsWrite() ? STU::Protocol::EncodingMode_24bit_Bulk : STU::Protocol::EncodingMode_24bit;
    }

    Win32::com_ptr<ID2D1RenderTarget> renderTarget;
    {
      auto renderTargetProperties = D2D1::RenderTargetProperties();
      renderTargetProperties.dpiX = renderTargetProperties.dpiY = 96.0f; // force bitmap to be pixel == DIP to ensure 1:1 pixel mapping.
      Win32::hresult_succeeded(m_pID2D1Factory->CreateWicBitmapRenderTarget(pWICBitmap.get(), renderTargetProperties, &renderTarget));
    }

    calculateButtonPositions<int>(renderTarget->GetSize(), &Button::boundsScreen);
    
    if (!m_useSigMode)
    {
      renderTarget->BeginDraw();
      renderBackground_Screen(renderTarget.get());
      Win32::hresult_succeeded(renderTarget->EndDraw());

      m_bitmap = WacomGSS::STU::ProtocolHelper::flatten(m_pIWICImagingFactory.get(), pWICBitmap.get(), m_capability.screenWidth, m_capability.screenHeight, m_encodingMode);
    }
    else
    {
      WacomGSS::Win32::com_ptr<ID2D1Bitmap> pBmp;

      Win32::hresult_succeeded(renderTarget->CreateBitmapFromWicBitmap(pWICBitmap.get(), &pBmp));

      D2D1_SIZE_F size = renderTarget->GetSize();
      D2D1_RECT_F rect{ 0, 0, size.width, size.height };

      renderTarget->BeginDraw();
      renderTarget->DrawBitmap(pBmp.get(), rect);
      Win32::hresult_succeeded(renderTarget->EndDraw());
      m_pBitmap540 = pWICBitmap;
    }
  }


  void checkImage(LPCWSTR bmpId, bool pushed)
  {
    uploadImage(m_tablet, m_capability,
                WacomGSS::STU::Protocol::OperationModeType_Signature, pushed, SIG_IMAGE_NUM,
                m_hInstance, bmpId, m_pIWICImagingFactory,
                WacomGSS::STU::Protocol::RomStartImageData::initializeSignature(m_encodingMode, pushed, SIG_IMAGE_NUM, std::array<bool, 3>{ true, true, true}) );
  }


  template<class Rect>
  WacomGSS::Win32::com_ptr<IDWriteTextFormat> createWhyFont(Rect TextArea::* bounds)
  {
    auto fontFamily = L"Arial";
    auto fontSize   = ((m_why.*bounds).bottom - (m_why.*bounds).top)*2.0f/3.0f / 10.0f;
    auto locale     = L"";

    using namespace WacomGSS::Win32;

    com_ptr<IDWriteTextFormat> textFormat;

    hresult_succeeded
    (    
      m_pIDWriteFactory->CreateTextFormat(fontFamily, nullptr, DWRITE_FONT_WEIGHT_NORMAL, DWRITE_FONT_STYLE_NORMAL, DWRITE_FONT_STRETCH_NORMAL, fontSize, locale, &textFormat)
    );
    hresult_succeeded
    (
      textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
    );
    hresult_succeeded
    (
      textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
    );
    return textFormat;
  }


  template<class Rect>
  WacomGSS::Win32::com_ptr<IDWriteTextFormat> createButtonFont(Rect TextArea::* bounds)
  {
    auto fontFamily = L"Arial";
    auto fontSize   = ((m_btns[0].*bounds).bottom - (m_btns[0].*bounds).top)*2.0f/3.0f;
    auto locale     = L"";

    using namespace WacomGSS::Win32;

    com_ptr<IDWriteTextFormat> textFormat;

    hresult_succeeded
    (    
      m_pIDWriteFactory->CreateTextFormat(fontFamily, nullptr, DWRITE_FONT_WEIGHT_NORMAL, DWRITE_FONT_STYLE_NORMAL, DWRITE_FONT_STRETCH_NORMAL, fontSize, locale, &textFormat)
    );
    hresult_succeeded
    (
      textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
    );
    hresult_succeeded
    (
      textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
    );
    return textFormat;
  }


  void createDeviceDependentResources()
  {
    using namespace WacomGSS::Win32;

    RECT clientRect;
    win32api_BOOL(GetClientRect(m_hwnd, &clientRect), "GetClientRect");
    
    auto renderTargetProperties     = D2D1::RenderTargetProperties();
    auto hwndRenderTargetProperties = D2D1::HwndRenderTargetProperties(m_hwnd, D2D1::SizeU(UINT32(clientRect.right-clientRect.left), UINT32(clientRect.bottom-clientRect.top)), D2D1_PRESENT_OPTIONS_RETAIN_CONTENTS|D2D1_PRESENT_OPTIONS_IMMEDIATELY);

    hresult_succeeded
    (
      m_pID2D1Factory->ReloadSystemMetrics()
    );

    hresult_succeeded
    (
      m_pID2D1Factory->CreateHwndRenderTarget(&renderTargetProperties, &hwndRenderTargetProperties, &m_renderTarget)
    );      
    
    hresult_succeeded
    (
      m_renderTarget->CreateSolidColorBrush( D2D1::ColorF(0.0,0,3.0f/5.0f, 0.333f), &m_penInk)
    );

    hresult_succeeded
    (
      m_renderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::Black), &m_textBrush)
    );

    setRenderDataScale();
    calculateButtonPositions<FLOAT>(m_renderTarget->GetSize(), &Button::boundsClient);
    calculateWhyPosition<FLOAT>(&Button::boundsClient);

    //textFormat = createButtonFont(&Button::boundsClient);
  }

  
  void decrypt(uint8_t data[WacomGSS::STU::Protocol::PenDataEncrypted::encryptedSize])
  {
    // forward decryption to tablet handlers.
    m_tablet.decrypt(data);
  }

  void onReport(WacomGSS::STU::Protocol::PenDataTimeCountSequenceEncrypted & v)
  {
    if (v.sessionId == m_sessionId)
    {
      onReport(static_cast<WacomGSS::STU::Protocol::PenData &>(v));      
    }
  }

  void onReport(WacomGSS::STU::Protocol::PenDataEncryptedOption & v)
  {
    if (v.sessionId == m_sessionId)
    {
      onReport(v.penData[0]);      
      onReport(v.penData[1]);
    }
  }

  void onReport(WacomGSS::STU::Protocol::PenDataEncrypted & v)
  {
    if (v.sessionId == m_sessionId)
    {
      onReport(v.penData[0]);
      onReport(v.penData[1]);
    }
  }

  void onReport(WacomGSS::STU::Protocol::PenDataTimeCountSequence & v)
  {
    onReport(static_cast<WacomGSS::STU::Protocol::PenData &>(v));          
  }

  void onReport(WacomGSS::STU::Protocol::PenDataOption & v)
  {
    onReport(static_cast<WacomGSS::STU::Protocol::PenData &>(v));          
  }

  const WPARAM k_user_pendata = 1;
  const WPARAM k_user_signature = 2;

  void onReport(WacomGSS::STU::Protocol::PenData & v)
  {
    WacomGSS::STU::Protocol::PenDataTimeCountSequence penData;
    static_cast<WacomGSS::STU::Protocol::PenData &>(penData) = v;
    penData.timeCount = 0;
    penData.sequence  = 0;
    {
      WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
      m_readerQueue.push_back(penData);
    }

    ::PostMessage(m_hwnd, WM_USER, k_user_pendata, 0);
  }

  void onReport(WacomGSS::STU::Protocol::EventDataSignature & eventDataSignature)
  {
    ::PostMessage(m_hwnd, WM_USER, k_user_signature, eventDataSignature.keyValue);
  }
  void onReport(WacomGSS::STU::Protocol::EventDataSignatureEncrypted & eventDataSignature)
  {
    ::PostMessage(m_hwnd, WM_USER, k_user_signature, eventDataSignature.keyValue);
  }

  void processSignatureEvent(uint8_t keyValue)
  {
    switch (keyValue)
    {
    case 0:
      onCancel();
      break;

    case 1:
      onOK();
      break;

    case 2:
      onClear();
      break;
    }
  }

  void backgroundThread() noexcept
  {
    WacomGSS::setThreadName("SignatureForm::backgroundThread");

    try
    {
      WacomGSS::STU::Report report;
      while (m_queue.wait_getReport_predicate(report))
      {
        handleReport(report.begin(), report.end(), m_isTls);
      }
    }
    catch (...)
    {
      {
        WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
        m_readerException = std::current_exception();
      }
      ::PostMessage(m_hwnd, WM_USER, 0, 0);
    }
  }

  
  void recalc(RenderData & rd) const noexcept
  {
    rd.client.x = rd.x * m_renderDataScale.width;
    rd.client.y = rd.y * m_renderDataScale.height;    
  }


  RenderData renderData(WacomGSS::STU::Protocol::PenDataTimeCountSequence penData, WacomGSS::STU::ProtocolHelper::InkState inkState) const noexcept
  {
    RenderData rd;
    rd.inkState = inkState;
    rd.x        = penData.x;
    rd.y        = penData.y;
    rd.pressure = static_cast<float>(penData.pressure) / m_capability.tabletMaxPressure;    
    recalc(rd);
    return rd;
  }

  void drawScreen()
  {
    renderBackground_Client(m_renderTarget.get());
    renderWhy_Client(m_renderTarget.get());
    m_renderIndex = 0;
    renderInk_Client();
  }

  void drawScreenBeginEnd()
  {
    m_renderTarget->BeginDraw();
    drawScreen();
    HRESULT hr = m_renderTarget->EndDraw();
    if (hr == D2DERR_RECREATE_TARGET)
    {
      discardDeviceDependentResources();
      createDeviceDependentResources();
      m_renderTarget->BeginDraw();
      drawScreen();    
      hr = m_renderTarget->EndDraw();
    }
  }

  void renderInk_Client()
  {
    if (m_penData.empty() || m_renderIndex == m_penData.size())
      return;

    auto p = m_penData.begin() + m_renderIndex;

    for (auto i = p+1; i != m_penData.end(); ++m_renderIndex, ++p, ++i)
    {
      if ((i->inkState & WacomGSS::STU::ProtocolHelper::InkState_isInk) != 0)
      {
        m_renderTarget->DrawLine(p->client, i->client, m_penInk.get(), 4.0f* m_renderInkScale );
      }
    }
  }

  void clearScreen()
  {
    try
    {
      ScopedCursor wait(nullptr, IDC_WAIT);

      if (!m_useSigMode)
      {
        if (m_sessionId)
        {
          m_tablet.endCapture();
        }

        if (!m_bitmap.empty())
        {
          m_tablet.writeImage(m_encodingMode, m_bitmap.data(), m_bitmap.size());
        }
        else
        {
          m_tablet.setClearScreen();
        }

        m_tablet.writeImageArea(m_bitmapWhyEncodingMode, m_bitmapWhyArea, m_bitmapWhy.data(), m_bitmapWhy.size());

        if (m_sessionId)
        {
          m_tablet.startCapture(m_sessionId);
        }
      }
      else
      {
        m_tablet.writeImageArea(m_bitmapWhyEncodingMode, m_bitmapWhyArea, m_bitmapWhy.data(), m_bitmapWhy.size());
      }

      m_penData.clear();
      m_inkingState.clear();
      m_renderIndex = 0;
      m_isDown = 0;
    }
    catch (...)
    {
      m_tablet.getStatus();
      throw;
    }
  }


  void renderBeginEnd()
  {
    m_renderTarget->BeginDraw();
    renderInk_Client();
    HRESULT hr = m_renderTarget->EndDraw();
    if (hr == D2DERR_RECREATE_TARGET)
    {
      discardDeviceDependentResources();
      createDeviceDependentResources();      
      drawScreenBeginEnd();
    }
  }


  D2D1_POINT_2F tabletToScreen(WacomGSS::STU::Protocol::PenData const & penData) const 
  {
    return D2D1::Point2F(static_cast<FLOAT>(penData.x)*m_capability.screenWidth/m_capability.tabletMaxX, 
                         static_cast<FLOAT>(penData.y)*m_capability.screenHeight/m_capability.tabletMaxY);
  }
  

  static bool ptInRect(D2D1_RECT_F const & rc, D2D1_POINT_2F & pt) noexcept
  {
    return pt.x >= rc.left && pt.x <= rc.right && pt.y >= rc.top && pt.y <= rc.bottom;
  }

  void drawWait()                                                  
  {
    if (m_renderTarget)
    {
      auto textFormat = createButtonFont(&Button::boundsClient);

      m_renderTarget->BeginDraw();
      auto size = m_renderTarget->GetSize();
      auto rc = D2D1::RectF(0,0,size.width, size.height);
      // UNICODE U+231B 'Hourglass'
      m_renderTarget->DrawTextW(L"\x231b", 1, textFormat.get(), rc, m_textBrush.get());
      /*HRESULT hr = */ m_renderTarget->EndDraw();
    }
  }
  
  void onOK()
  {
    // process collected data as required here.
    
    ::PostMessage(m_hwnd, WM_CLOSE, 0, 0);
  }

  void onClear()
  {
    drawWait();    
    clearScreen();
    drawScreenBeginEnd();
    {
      WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
      m_readerQueue.clear();
    }
  }

  void onCancel()
  {
    ::PostMessage(m_hwnd, WM_CLOSE, 0, 0);
  }



  void processPenData(WacomGSS::STU::Protocol::PenDataTimeCountSequence & penData, WacomGSS::STU::ProtocolHelper::InkState inkState)
  {
    if (!penData.rdy)
      return;

    auto pt = tabletToScreen(penData);
    
    int btn = 0; // will be +ve if the pen is over a button.
    {
      for (size_t i = 0; i < m_btns.size(); ++i)
      {
        if (ptInRect(m_btns[i].boundsScreen, pt))
        {
          btn = static_cast<int>(i+1);
          break;
        }
      }
    }
    
    using namespace WacomGSS::STU::ProtocolHelper;
    if ((inkState & InkState_isOn) != 0)
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
    }
    else
    {
      if (m_isDown != 0)
      {
        if (btn > 0)
        {
          if (btn == m_isDown)
          {
            (this->*m_btns[static_cast<size_t>(btn-1)].onClick)();
          }
        }
        m_isDown = 0;
      }
    }

    m_penData.push_back(renderData(penData, inkState));
  }

  LRESULT CALLBACK wndProc(UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    LRESULT lResult = 0;
    switch (uMessage)
    {      
      case WM_PAINT:
        {
          PAINTSTRUCT ps;
          BeginPaint(m_hwnd, &ps);
          try
          {
            drawScreenBeginEnd();
          }
          catch (...)
          {
            handleException(m_hwnd);
            DestroyWindow(m_hwnd);
          }
          EndPaint(m_hwnd, &ps);
        }
        break;
        
      case WM_USER:
        try
        {
          if (wParam != k_user_signature)
          {
            decltype(m_readerQueue) queue;
            {
              WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
              if (m_readerException != nullptr)
              {              
                std::rethrow_exception(m_readerException);
              }
              queue.swap(m_readerQueue);
            }

            if (!queue.empty())
            {
            
              for (auto i = queue.begin(); i != queue.end(); ++i)
              {
                processPenData(*i, m_inkingState.nextState(i->rdy, i->pressure));
              }

              renderBeginEnd();
            }
          } 
          else
          {
            processSignatureEvent(static_cast<uint8_t>(lParam & 0xff));
          }
        }
        catch (...)
        {
          handleException(m_hwnd);
          DestroyWindow(m_hwnd);
        }
        break;

      case WM_DESTROY:
        EnableWindow(GetParent(m_hwnd), TRUE);
        try
        {
          m_tablet.disconnect();
        }
        catch (...)
        {
          handleException(m_hwnd);            
        }
        break;
        
      case WM_SIZE:
        try
        {
          if (m_renderTarget)
          {
            using namespace WacomGSS::Win32;

            RECT clientRect;
            win32api_BOOL(GetClientRect(m_hwnd, &clientRect), "GetClientRect");
    
            m_renderTarget->Resize(D2D1::SizeU(UINT32(clientRect.right-clientRect.left), UINT32(clientRect.bottom-clientRect.top)));
                        
            calculateButtonPositions<FLOAT>(m_renderTarget->GetSize(), &Button::boundsClient);
            calculateWhyPosition<FLOAT>(&Button::boundsClient);
            //m_textFormat = createButtonFont(&Button::boundsClient); // m_textFormat is device independent but needs to be resized

            setRenderDataScale();

            for (auto i = m_penData.begin(); i != m_penData.end(); ++i)
            {
              recalc(*i);
            }

            drawScreenBeginEnd();
          }
        }
        catch (...)
        {
          handleException(m_hwnd);
          DestroyWindow(m_hwnd);
        }
        break;

      case WM_CLOSE:
        {
          try
          {
            if (m_tablet.getStatus().statusCode == WacomGSS::STU::Protocol::StatusCode_Capture)
            {
              m_tablet.endCapture();
            }
            drawWait();
            m_tablet.setInkingMode(WacomGSS::STU::Protocol::InkingMode_Off);
            m_tablet.setClearScreen();
          }
          catch (...)
          {
            handleException(m_hwnd);
          }

          EnableWindow(GetParent(m_hwnd), TRUE);
          DestroyWindow(m_hwnd);
        }
        break;

      case WM_DPICHANGED:
        {
          auto rc = (LPRECT)lParam;
          SetWindowPos(m_hwnd, nullptr, rc->left, rc->top, rc->right, rc->bottom, SWP_NOZORDER | SWP_NOACTIVATE); 
        }
        break;

      case WM_CREATE:
        {
          try
          {
            using namespace WacomGSS::Win32;

            FLOAT dpiX, dpiY;
            getDPIForMonitor(dpiX, dpiY);

            // size and position window
            {
              RECT rc;
              float m_tabletWidth  = m_capability.tabletMaxX / 2540.0f * dpiX;
              float m_tabletHeight = m_capability.tabletMaxY / 2540.0f * dpiY;

              rc.right  = static_cast<LONG>(std::ceil(m_tabletWidth));
              rc.bottom = static_cast<LONG>(std::ceil(m_tabletHeight));

              RECT rcParent;
              win32api_BOOL(::GetWindowRect(::GetParent(m_hwnd), &rcParent), "GetWindowRect");
              
              rc.left = rcParent.left + ((rcParent.right - rcParent.left) - rc.right)/2;
              rc.top  = rcParent.top  + ((rcParent.bottom - rcParent.top) - rc.bottom)/2;
              rc.right  += rc.left;
              rc.bottom += rc.top;
              win32api_BOOL(::AdjustWindowRectEx(&rc, WS_CAPTION|WS_SYSMENU|WS_BORDER, FALSE, 0), "AdjustWindowRectEx");
              
              win32api_BOOL(::SetWindowPos(m_hwnd, nullptr, rc.left, rc.top, rc.right-rc.left, rc.bottom-rc.top, SWP_NOACTIVATE|SWP_NOOWNERZORDER|SWP_NOZORDER), "SetWindowPos");
            }

            createDeviceDependentResources();

            createBitmap();
            createBitmap2();

            if (m_useSigMode)
            {
              // ensure that the images have been pre-loaded onto the tablet
              checkImage(MAKEINTRESOURCE(IDB_SIGSCRN_BTNS_UP), false);
              checkImage(MAKEINTRESOURCE(IDB_SIGSCRN_BTNS_DN), true);
            }
            else
            {
              clearScreen();
            }


            bool useColor = m_encodingMode != WacomGSS::STU::Protocol::EncodingMode_1bit;
            if (useColor)
            {              
              WacomGSS::STU::Protocol::HandwritingThicknessColor handwritingThicknessColor;
                
              handwritingThicknessColor.penColor = rgb16_565(blend(D2D1::ColorF(0, 0, 3.0f / 5.0f, 0.333f), D2D1::ColorF(D2D1::ColorF::White))); // tablet doesn't support alphablend, so precompute generic opaque value.

              if (m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_RenderingMode))
              {
                m_tablet.setRenderingMode(WacomGSS::STU::Protocol::RenderingMode_WILL);
                handwritingThicknessColor.penThickness = 2;
              }
              else
              {
                handwritingThicknessColor.penThickness = 1;
              }

              m_tablet.setHandwritingThicknessColor(handwritingThicknessColor);
            }

            m_tablet.setInkingMode(WacomGSS::STU::Protocol::InkingMode_On);
            m_queue  = std::move(m_tablet.interfaceQueue());
            m_reader = std::move(std::thread(std::ref(*this)));     
            
            if (m_useSigMode)
            {
              // Enter signature mode

              // 0=Cancel; 1=Enter; 2=Clear
              WacomGSS::STU::Protocol::OperationMode_Signature sigOpMode{ SIG_IMAGE_NUM,{ 2, 0, 1 }, 0, 0 };
              WacomGSS::STU::Protocol::OperationMode opMode = WacomGSS::STU::Protocol::OperationMode::initializeSignature(sigOpMode);
              m_tablet.setOperationMode(opMode);
              m_tablet.writeImageArea(m_bitmapWhyEncodingMode, m_bitmapWhyArea, m_bitmapWhy.data(), m_bitmapWhy.size());
            }

            bool enableEncryption = false;
            try
            {
              enableEncryption = m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_EncryptionStatus) || WacomGSS::STU::ProtocolHelper::supportsEncryption(m_tablet.getDHprime());
              if (enableEncryption)
              {
                m_sessionId = 0xc0ffee; // recommend to use a random number here.
                m_tablet.startCapture(m_sessionId);
              }
              else
              {
                m_sessionId = 0;
              }
            }
            catch (std::exception const &)
            {
              enableEncryption = false;
              m_sessionId = 0;
            }

            ::EnableWindow(::GetParent(m_hwnd), FALSE);
            // ignore return
          }
          catch (...)
          {
            handleException(m_hwnd);
            ::DestroyWindow(m_hwnd);
          }
        }
        break;

      case WM_CHAR:
        if (wParam == 'c' || wParam == 'C')
        {
          onClear();
        }
        else
        {
          lResult = ::DefWindowProc(m_hwnd, uMessage, wParam, lParam);
        }
        break;

      default:
        lResult = ::DefWindowProc(m_hwnd, uMessage, wParam, lParam);
    }
    return lResult;
  }
  
  static LRESULT CALLBACK wndProc2_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    auto p = reinterpret_cast<SignatureForm *>(GetWindowLongPtr(hWnd, GWLP_USERDATA));
    return p->wndProc(uMessage, wParam, lParam);
  }

  static LRESULT CALLBACK wndProc1_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    if (uMessage == WM_NCCREATE)
    {
      auto p = reinterpret_cast<SignatureForm *>(reinterpret_cast<LPCREATESTRUCT>(lParam)->lpCreateParams);
      ::SetWindowLongPtr(hWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(p));
      ::SetWindowLongPtr(hWnd, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(&wndProc2_s));
      p->m_hwnd = hWnd;
      return p->wndProc(uMessage, wParam, lParam);
    }
    return ::DefWindowProc(hWnd, uMessage, wParam, lParam);
  }


  SignatureForm(std::unique_ptr<WacomGSS::STU::Interface> && intf, Factory const & factory, bool useSigMode, HINSTANCE hInstance)
  :  
    Factory(factory),
    m_hwnd(nullptr),
    m_hInstance(hInstance),
    m_isTls(dynamic_cast<WacomGSS::STU::TlsInterface *>(intf.get())!= 0),
    m_tablet(std::move(intf), std::make_shared<WacomGSS::STU::OpenSSL_EncryptionHandler>(),std::make_shared<WacomGSS::STU::OpenSSL_EncryptionHandler2>()),    
    m_shcore(::LoadLibrary(L"Shcore.dll"), std::nothrow),
    m_pGetDPIForMonitor(nullptr),
    m_btns(3),
    m_readerException(nullptr),
    m_renderIndex(0),
    m_sessionId(0)
    
  {
    m_tablet.endCapture();
    m_capability   = m_tablet.getCapability();
    m_inkingState.setInkThreshold(m_tablet.getInkThreshold());
        
    bool sigModeCapable = m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_OperationMode);
    m_useSigMode = useSigMode && sigModeCapable;

    m_btns[0].text = loadString(hInstance, IDS_SignatureForm_OK);
    m_btns[0].onClick = &SignatureForm::onOK;
    m_btns[1].text = loadString(hInstance, IDS_SignatureForm_Clear);
    m_btns[1].onClick = &SignatureForm::onClear;
    m_btns[2].text = loadString(hInstance, IDS_SignatureForm_Cancel);
    m_btns[2].onClick = &SignatureForm::onCancel;
    if (m_shcore)
    {
      m_pGetDPIForMonitor = reinterpret_cast<pfn_GetDPIForMonitor>(GetProcAddress(m_shcore.get(), "GetDPIForMonitor"));
    }    

    m_why.text = L"I agree to the terms and conditions";
    m_forceMonoWhy = true;
  }


  static const WCHAR k_lpszClassName[];

public:
  ~SignatureForm()
  {
    try
    {
      if (m_reader.joinable())
      {
        m_tablet.queueSetPredicateAll(true);
        m_tablet.queueNotifyAll();
        m_reader.join();
      }
    }
    catch (...)
    {
    }
  }

  static ATOM registerWindow(HINSTANCE hInstance)
  {
    WNDCLASSEX wc = { sizeof(WNDCLASSEX) };
    wc.lpfnWndProc   = &wndProc1_s;
    wc.hInstance     = hInstance;
    wc.hCursor       = LoadCursor(nullptr, IDC_ARROW);
    wc.lpszClassName = k_lpszClassName;

    return ::RegisterClassEx(&wc);
  }

 
  static void dialogBox(HINSTANCE hInstance, HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> && intf, Factory const & factory, bool useSigMode)
  {
    auto cls = std::unique_ptr<SignatureForm>(new SignatureForm(std::move(intf), factory, useSigMode, hInstance));

    HWND hwnd = ::CreateWindowEx(0, k_lpszClassName, loadString(hInstance, IDS_SignatureForm_Title).c_str(), WS_THICKFRAME|WS_BORDER|WS_CAPTION|WS_SYSMENU|WS_POPUP|WS_VISIBLE, 0, 0, 100, 100, hWnd, nullptr, hInstance, cls.get());
    if (hwnd)
    {
      MSG msg;
      while (::IsWindow(hwnd) && ::GetMessage(&msg, nullptr, 0, 0) > 0)
      {
        if (!IsDialogMessage(hWnd, &msg))
        {
          TranslateMessage(&msg);
          DispatchMessage(&msg);
        }
      }
    }
  }

  void operator()() noexcept
  {
    backgroundThread();
  }
};

const WCHAR SignatureForm::k_lpszClassName[] = L"WacomGSS.STU.DemoButtons"; // not localized


class DemoButtons
{  
  Factory m_factory;
  
  DemoButtons()
  {
    D2D1_FACTORY_OPTIONS factoryOptions;
    factoryOptions.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
    WacomGSS::Win32::hresult_succeeded(::D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, __uuidof(m_factory.m_pID2D1Factory), &factoryOptions, &m_factory.m_pID2D1Factory), "D2D1CreateFactory()");
    WacomGSS::Win32::hresult_succeeded(::CoCreateInstance(CLSID_WICImagingFactory1, nullptr, CLSCTX_INPROC_SERVER, __uuidof(m_factory.m_pIWICImagingFactory), &m_factory.m_pIWICImagingFactory), "CoCreateInstance(CLSID_WICImagingFactory1)");
    WacomGSS::Win32::hresult_succeeded(::DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory), &m_factory.m_pIDWriteFactory), "DWriteCreateFactory()");
  }


  INT_PTR CALLBACK dialogProc(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM /*lParam*/) noexcept
  {
    INT_PTR bSuccess = TRUE;
    switch (uMessage)
    {
      case WM_COMMAND:
        try
        {
          switch (LOWORD(wParam))
          {
            case IDC_Dialog_Action:
            {
              std::unique_ptr<WacomGSS::STU::Interface> intf;
              if (doConnect(hWnd, intf))
              {
                auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));

                bool useSigMode = ::IsDlgButtonChecked(hWnd, IDC_Dialog_SigMode) != BST_UNCHECKED;

                SignatureForm::dialogBox(hInstance, hWnd, std::move(intf), m_factory, useSigMode);
              }
            }
            break;

            default:
              bSuccess = handleSerial(hWnd, wParam);
          }
        }
        catch (...)
        {
          handleException(hWnd);
        }
        break;

      case WM_INITDIALOG:
        initSerial(hWnd);
        break;

      case WM_CLOSE:
        EndDialog(hWnd, 0);
        break;

      default:
        bSuccess = FALSE;
    }
    return bSuccess;
  }

  static INT_PTR CALLBACK dialogProc2_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    DemoButtons * p = reinterpret_cast<DemoButtons *>(GetWindowLongPtr(hWnd, DWLP_USER));
    return p->dialogProc(hWnd, uMessage, wParam, lParam);    
  }

  static INT_PTR CALLBACK dialogProc1_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    if (uMessage == WM_INITDIALOG)
    {
      auto p = reinterpret_cast<DemoButtons *>(lParam);
      ::SetWindowLongPtr(hWnd, DWLP_USER, reinterpret_cast<LONG_PTR>(p));
      ::SetWindowLongPtr(hWnd, DWLP_DLGPROC, reinterpret_cast<LONG_PTR>(dialogProc2_s));
      return p->dialogProc(hWnd, uMessage, wParam, lParam);
    }
    return FALSE;
  }

public:
  static INT_PTR dialogBox(HINSTANCE hInstance)
  {
    std::unique_ptr<DemoButtons> cls(new DemoButtons);
    return ::DialogBoxParam(hInstance, MAKEINTRESOURCE(IDD_Dialog), nullptr, dialogProc1_s, reinterpret_cast<LPARAM>(cls.get()));
  }
};




int PASCAL WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR, int)
{
  ::HeapSetInformation(nullptr, HeapEnableTerminationOnCorruption, nullptr, 0);
  //ignore return

  try
  {
    WacomGSS::Win32::ComInitialize coinit(nullptr, COINIT_APARTMENTTHREADED);
    SignatureForm::registerWindow(hInstance);
    
    return DemoButtons::dialogBox(hInstance);
  }
  catch (...)
  {
    handleException(nullptr);
  }
  return 1;
}
