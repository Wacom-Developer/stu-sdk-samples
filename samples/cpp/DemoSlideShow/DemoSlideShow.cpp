#include "DemoSlideShow.h"
#include <WacomGSS/STU/Tablet.hpp>



struct WndData
{
  HWND m_hWnd;
  HINSTANCE m_hInstance;
  WacomGSS::Win32::com_ptr<IWICImagingFactory> m_pIWICImagingFactory; // Factory for creating WIC bitmaps
  
  WndData(HWND hWnd)
  :
    m_hWnd(hWnd),
    m_hInstance(reinterpret_cast<HINSTANCE>(::GetWindowLongPtr(hWnd, GWLP_HINSTANCE)))
  {
    WacomGSS::Win32::hresult_succeeded(::CoCreateInstance(CLSID_WICImagingFactory1, nullptr, CLSCTX_INPROC_SERVER, __uuidof(m_pIWICImagingFactory), &m_pIWICImagingFactory));
  }


  void checkImage(WacomGSS::STU::Tablet & tablet, WacomGSS::STU::Protocol::Capability const & capability, LPCWSTR bitmapId, uint8_t bitmapNumber)
  {
    WacomGSS::STU::Protocol::EncodingMode      encodingMode = tablet.supportsWrite() ? WacomGSS::STU::Protocol::EncodingMode_24bit_Bulk : WacomGSS::STU::Protocol::EncodingMode_24bit;
    WacomGSS::STU::Protocol::RomStartImageData romStartImageData = WacomGSS::STU::Protocol::RomStartImageData::initializeSlideShow(encodingMode, false, bitmapNumber);
    
    uploadImage(tablet, capability,
                WacomGSS::STU::Protocol::OperationModeType_SlideShow, false, bitmapNumber,
                m_hInstance, bitmapId, m_pIWICImagingFactory,
                romStartImageData);
  }


  void upload()
  {
    std::unique_ptr<WacomGSS::STU::Interface> intf;
    if (doConnect(m_hWnd, intf))
    {
      ScopedCursor waitCursor(nullptr, IDC_WAIT);

      WacomGSS::STU::Tablet tablet(std::move(intf));
      auto capability = tablet.getCapability();
      checkImage(tablet, capability, MAKEINTRESOURCE(1), 1);
      checkImage(tablet, capability, MAKEINTRESOURCE(2), 2);
      checkImage(tablet, capability, MAKEINTRESOURCE(3), 3);
    }
  }


  void start()
  {
    std::unique_ptr<WacomGSS::STU::Interface> intf;
    if (doConnect(m_hWnd, intf))
    {
      ScopedCursor waitCursor(nullptr, IDC_WAIT);

      WacomGSS::STU::Tablet tablet(std::move(intf));
      auto capability = tablet.getCapability();
      checkImage(tablet, capability, MAKEINTRESOURCE(1), 1);
      checkImage(tablet, capability, MAKEINTRESOURCE(2), 2);
      checkImage(tablet, capability, MAKEINTRESOURCE(3), 3);

      WacomGSS::STU::Protocol::OperationMode_SlideShow slideShow;

      slideShow.workingMode = 0;
      slideShow.numberOfSlides = 3;
      slideShow.slideNumber[0] = 1;
      slideShow.slideNumber[1] = 2;
      slideShow.slideNumber[2] = 3;
      slideShow.interval = 2000;

      tablet.setOperationMode( WacomGSS::STU::Protocol::OperationMode::initializeSlideShow(slideShow) );
    }
  }


  void stop()
  {
    std::unique_ptr<WacomGSS::STU::Interface> intf;
    if (doConnect(m_hWnd, intf))
    {
      ScopedCursor waitCursor(nullptr, IDC_WAIT);
    
      WacomGSS::STU::Tablet tablet(std::move(intf));
      tablet.setOperationMode( WacomGSS::STU::Protocol::OperationMode::initializeNormal() );
    }
  }

};



WndData * getWndData(HWND hWnd)
{
  return reinterpret_cast<WndData *>(GetWindowLongPtr(hWnd, DWLP_USER));
}



class Dialog
{ 

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
            case IDC_DemoSlideShow_Upload:
              getWndData(hWnd)->upload();
              break;

            case IDC_DemoSlideShow_Start:
              getWndData(hWnd)->start();
            break;

            case IDC_DemoSlideShow_Stop:
              getWndData(hWnd)->stop();
              break;

            default:
              bSuccess = handleSerial(hWnd, wParam);
              break;
          }
        }
        catch (...)
        {
          handleException(hWnd);
        }
        break;

      case WM_INITDIALOG:
        SetWindowLongPtr(hWnd, DWLP_USER, reinterpret_cast<LONG_PTR>(new WndData(hWnd)));
        initSerial(hWnd);
        break;

      case WM_CLOSE:
        EndDialog(hWnd, 0);
        break;

      case WM_DESTROY:
        delete getWndData(hWnd);
        break;

      default:
        bSuccess = FALSE;
    }
    return bSuccess;
  }

  static INT_PTR CALLBACK dialogProc2_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    Dialog * p = reinterpret_cast<Dialog *>(GetWindowLongPtr(hWnd, DWLP_USER));
    return p->dialogProc(hWnd, uMessage, wParam, lParam);    
  }

  static INT_PTR CALLBACK dialogProc1_s(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam) noexcept
  {
    if (uMessage == WM_INITDIALOG)
    {
      auto p = reinterpret_cast<Dialog *>(lParam);
      ::SetWindowLongPtr(hWnd, DWLP_USER, reinterpret_cast<LONG_PTR>(p));
      ::SetWindowLongPtr(hWnd, DWLP_DLGPROC, reinterpret_cast<LONG_PTR>(dialogProc2_s));
      return p->dialogProc(hWnd, uMessage, wParam, lParam);
    }
    return FALSE;
  }

public:
  static INT_PTR dialogBox(HINSTANCE hInstance)
  {
    std::unique_ptr<Dialog> cls(new Dialog());
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
    return Dialog::dialogBox(hInstance);
  }
  catch (...)
  {
    handleException(nullptr);
  }
  return 1;
}
