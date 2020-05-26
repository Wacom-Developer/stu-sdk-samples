#include "DemoPinPad.h"
#include <WacomGSS/STU/ReportHandler.hpp>
#include <WacomGSS/setThreadName.hpp>
#include <strsafe.h>

#include <sstream>

#define WM_PINPAD_EXCEPTION  (WM_APP + 0)
#define WM_PINPAD_EVENT     (WM_APP + 1)

#define PINPAD_IMAGE_NUM (uint8_t)1
#define PINPAD_TYPE      1  // Numbers only
#define PINPAD_LAYOUT    3  // 1-3 along the top, 0 at the bottom

//==============================================================================


class PinPad : WacomGSS::STU::ProtocolHelper::ReportHandler
{
public:
  enum : uint8_t {
    InputCancel   = 0,
    InputEnter    = 1,
    InputMinDigit = 2,
    InputMaxDigit = 3
  };
  static const int MaxMaxDigits = 12;

  PinPad(HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> && intf, uint8_t minDigits, uint8_t maxDigits, bool hideNumbers)
  : m_hWnd(hWnd)
  , m_hInstance(reinterpret_cast<HINSTANCE>(::GetWindowLongPtr(hWnd, GWLP_HINSTANCE)))
  , m_isTls(dynamic_cast<WacomGSS::STU::TlsInterface*>(intf.get())!=nullptr)
  , m_tablet(std::move(intf))
  , m_minDigits(minDigits)
  , m_maxDigits(maxDigits)
  , m_hideNumbers(hideNumbers)
  , m_queue(std::move(m_tablet.interfaceQueue()))
  {
    WacomGSS::Win32::hresult_succeeded(::CoCreateInstance(CLSID_WICImagingFactory1, nullptr, CLSCTX_INPROC_SERVER, __uuidof(m_pIWICImagingFactory), &m_pIWICImagingFactory));

    m_capability = m_tablet.getCapability();
    m_thread = std::move(std::thread(std::ref(*this)));
  }

  ~PinPad()
  {
    try
    {
      if (m_thread.joinable())
      {
        m_tablet.queueSetPredicateAll(true);
        m_tablet.queueNotifyAll();
        m_thread.join();
      }
    }
    catch (...)
    {
    }
  }

  uint8_t       minDigits() const   { return m_minDigits; }
  uint8_t       maxDigits() const   { return m_maxDigits; }
  bool          hideNumbers() const { return m_hideNumbers; }
  std::wstring  PIN() const         { return m_PIN; }

  void operator()() noexcept
  {
    try
    {
      bool pinPadSupport = m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_OperationMode);

      if (!pinPadSupport)
        throw loadString(m_hInstance, IDS_NotSupported);

      initPinPadMode();

      WacomGSS::STU::Report report;
      while (m_queue.wait_getReport_predicate(report))
      {
        handleReport(report.begin(), report.end(), m_isTls);
      }
      m_tablet.setClearScreen();
      m_tablet.disconnect();
    }
    catch (...)
    {
      {
        WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
        m_readerException = std::current_exception();
      }
      ::PostMessage(m_hWnd, WM_PINPAD_EXCEPTION, 0, 0);
    }
  }

  void handleException()
  {
    try
    {
      {
        WacomGSS::lock_guard<decltype(m_mutex)> lock(m_mutex);
        if (m_readerException != nullptr)
        {
          std::rethrow_exception(m_readerException);
        }
      }

    }
    catch (...)
    {
      ::handleException(m_hWnd);
    }
  }

  void test_RomImageHash()
  {
    std::wostringstream os;
    try
    {
      m_tablet.setRomImageHash(WacomGSS::STU::Protocol::OperationModeType_PinPad, false, PINPAD_IMAGE_NUM);
      WacomGSS::STU::Protocol::RomImageHash romImgHash = m_tablet.getRomImageHash();
      os << "Got hash for " << (romImgHash.imageType ? L"pushed" : L"non-pushed") << L" image number " << romImgHash.imageNumber;
      MessageBox(m_hWnd, os.str().c_str(), NULL, MB_OK);
    }
    catch (std::exception & ex)
    {
      os << L"Exception " << ex.what();
      MessageBox(m_hWnd, os.str().c_str(), NULL, MB_OK);
    }
  }

private:
  WacomGSS::mutex                     m_mutex;
  HWND                                m_hWnd;
  HINSTANCE                           m_hInstance;
  WacomGSS::Win32::com_ptr<IWICImagingFactory> m_pIWICImagingFactory; // Factory for creating WIC bitmaps
  bool                                m_isTls;
  WacomGSS::STU::Tablet               m_tablet;
  WacomGSS::STU::Protocol::Capability m_capability;
  WacomGSS::STU::InterfaceQueue       m_queue; // buffer between Tablet class and this application
  std::thread                         m_thread;
  std::exception_ptr                  m_readerException;

  uint8_t   m_minDigits;
  uint8_t   m_maxDigits;
  bool      m_hideNumbers;
  WCHAR     m_PIN[MaxMaxDigits + 1];

  void onReport(WacomGSS::STU::Protocol::EventDataPinPad & e)
  {
    try
    {
      switch (e.keyInput)
      {
      case InputEnter:
        for (unsigned i = 0; i < e.pin.size(); ++i)
        {
          switch (e.pin[i])
          {
          case 0x0F:
            m_PIN[i] = L'\0';
            break;

          case 0xA:
            m_PIN[i] = L'*';
            break;


          case 0x0B:
            m_PIN[i] = L'#';
            break;

          case 0x0C:
            m_PIN[i] = L'.';
            break;

          default:
            if (0 <= e.pin[i] && e.pin[i] <= 9)
              m_PIN[i] = L'0' + e.pin[i];
            break;
          }
        }
        // fall through...
      case InputCancel:
      case InputMinDigit:
      case InputMaxDigit:
        ::PostMessage(m_hWnd, WM_PINPAD_EVENT, e.keyInput, 0);
        break;
      }
    }
    catch (...)
    {
      {
        std::lock_guard<decltype(m_mutex)> lock(m_mutex);
        m_readerException = std::current_exception();
      }
      ::PostMessage(m_hWnd, WM_PINPAD_EXCEPTION, 0, 0);
    }
  }

  void checkImage(LPCWSTR bmpId, bool pushed)
  {
    WacomGSS::STU::Protocol::EncodingMode      encodingMode = m_tablet.supportsWrite() ? WacomGSS::STU::Protocol::EncodingMode_24bit_Bulk : WacomGSS::STU::Protocol::EncodingMode_24bit;
    WacomGSS::STU::Protocol::RomStartImageData romStartImageData = WacomGSS::STU::Protocol::RomStartImageData::initializePinPad(encodingMode, pushed, PINPAD_IMAGE_NUM, PINPAD_TYPE, PINPAD_LAYOUT);

    uploadImage(m_tablet, m_capability,
                WacomGSS::STU::Protocol::OperationModeType_PinPad, pushed, PINPAD_IMAGE_NUM,
                m_hInstance, bmpId, m_pIWICImagingFactory,
                romStartImageData);
  }

  void initPinPadMode()
  {
    using namespace WacomGSS::STU;

    checkImage(MAKEINTRESOURCE(IDB_PINPAD_BTNS_UP), false);
    checkImage(MAKEINTRESOURCE(IDB_PINPAD_BTNS_DN), true);

    Protocol::OperationMode_PinPad pinPadMode{ PINPAD_IMAGE_NUM,
      0,
      m_minDigits,
      m_maxDigits,
      m_hideNumbers ? 1u : 0u,
      0, // message after
      0 };
    Protocol::OperationMode opMode = Protocol::OperationMode::initializePinPad(pinPadMode);
    m_tablet.setOperationMode(opMode);

  }

};

struct WndData
{
  HWND                      m_hWnd;
  std::mutex                m_mutex;
  std::unique_ptr<PinPad>   m_pinPad;

  WndData(HWND hWnd) : m_hWnd(hWnd) {}

  ~WndData()
  {
    OutputDebugString(!m_pinPad ? L"~WndData NO pin pad\n" : L"~WndData pin pad\n");
  }

  void handleException()
  {
    if (!!m_pinPad)
    {
      m_pinPad->handleException();
      onStop();
    }
  }

  void onStart()
  {
    auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(m_hWnd, GWLP_HINSTANCE));

    BOOL minTranslated = FALSE;
    BOOL maxTranslated = FALSE;
    auto minDigits = ::GetDlgItemInt(m_hWnd, IDC_DemoPinPad_MinDigit, &minTranslated, FALSE);
    auto maxDigits = ::GetDlgItemInt(m_hWnd, IDC_DemoPinPad_MaxDigit, &maxTranslated, FALSE);
    bool hideNumbers = ::SendDlgItemMessage(m_hWnd, IDC_DemoPinPad_HideNum, BM_GETCHECK, 0, 0) == BST_CHECKED;

    if (minTranslated && maxTranslated
      && minDigits > 0
      && minDigits <= maxDigits
      && maxDigits <= PinPad::MaxMaxDigits)
    {
      std::unique_ptr<WacomGSS::STU::Interface> intf;
      if (doConnect(m_hWnd, intf))
      {
        m_pinPad.reset(new PinPad(m_hWnd, std::move(intf), minDigits, maxDigits, hideNumbers));
        enableCtrls(FALSE);
      }
    }
    else
    {
      MessageBox(m_hWnd, loadString(hInstance, IDS_DemoPinPad_InvalidDigits).c_str(), nullptr, MB_ICONEXCLAMATION | MB_OK);
    }
  }

  void onStop()
  {
//    m_pinPad->test_RomImageHash();
    m_pinPad.reset();
    enableCtrls(TRUE);
  }

  void enableCtrls(BOOL bEnable)
  {
    std::array<UINT, 6>  toEnable{
      IDC_DemoPinPad_HideNum,
      IDC_DemoPinPad_MinDigit_Lbl,
      IDC_DemoPinPad_MinDigit,
      IDC_DemoPinPad_MaxDigit_Lbl,
      IDC_DemoPinPad_MaxDigit,
      IDC_DemoPinPad_Start
    };
    std::array<UINT, 1>  toDisable{
      IDC_DemoPinPad_Stop
    };

    for (auto e : toEnable)
    {
      ::EnableWindow(::GetDlgItem(m_hWnd, e), bEnable);
    }
    for (auto e : toDisable)
    {
      ::EnableWindow(::GetDlgItem(m_hWnd, e), !bEnable);
    }
  }
};

WndData * GetWndData(HWND hWnd)
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
            case IDC_DemoPinPad_Start:
              GetWndData(hWnd)->onStart();
            break;

            case IDC_DemoPinPad_Stop:
              GetWndData(hWnd)->onStop();
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
        {
          WndData * wd = new WndData(hWnd);
          SetWindowLongPtr(hWnd, DWLP_USER, reinterpret_cast<LONG_PTR>(wd));
          initSerial(hWnd);
        }
        break;

      case WM_CLOSE:
        EndDialog(hWnd, 0);
        break;

      case WM_DESTROY:
        delete GetWndData(hWnd);
        break;

      case WM_PINPAD_EVENT:
        switch (wParam)
        {
        case PinPad::InputEnter:
          MessageBox(hWnd, GetWndData(hWnd)->m_pinPad->PIN().c_str(), L"PIN", MB_OK);
          GetWndData(hWnd)->onStop();
          break;

        case PinPad::InputCancel:
          {
            auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));
            MessageBox(hWnd, loadString(hInstance, IDS_DemoPinPad_Cancelled).c_str(), L"PIN", MB_OK);
            GetWndData(hWnd)->onStop();
          }
          break;

        case PinPad::InputMinDigit:
          {
            auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));
            WCHAR msg[128];

            StringCbPrintf(msg, sizeof(msg), 
                           loadString(hInstance, IDS_DemoPinPad_MinDigitsError).c_str(), 
                           GetWndData(hWnd)->m_pinPad->minDigits());
            MessageBox(hWnd, msg, NULL, MB_ICONINFORMATION | MB_OK);
          }
          break;

        case PinPad::InputMaxDigit:
          {
            auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));

            WCHAR msg[128];

            StringCbPrintf(msg, sizeof(msg),
                          loadString(hInstance, IDS_DemoPinPad_MaxDigitsError).c_str(),
                          GetWndData(hWnd)->m_pinPad->maxDigits());
            MessageBox(hWnd, msg, NULL, MB_ICONINFORMATION | MB_OK);
          }
          break;
        }
        break;

      case WM_PINPAD_EXCEPTION:
        GetWndData(hWnd)->handleException();
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
