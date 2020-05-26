#include "DemoKeyPad.h"

#include <WacomGSS/STU/Tablet_OpenSSL.hpp>
#include <WacomGSS/STU/ReportHandler.hpp>
#include <WacomGSS/Win32/gdiplus.hpp>
#include <strsafe.h>

#define WM_KEYPAD_EXCEPTION  (WM_APP + 0)
#define WM_KEYPAD_RESULT     (WM_APP + 1)


//==============================================================================


class KeyPad : WacomGSS::STU::ProtocolHelper::ReportHandler
{
public:
  enum : uint8_t {
    InputCancel   = 0,
    InputEnter    = 1,
    InputMinDigit = 2,
    InputMaxDigit = 3
  };
  static const int MaxMaxDigits = 12;

  KeyPad(HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> && intf, int layoutNum)
  : m_hWnd(hWnd)
  , m_hInstance(reinterpret_cast<HINSTANCE>(::GetWindowLongPtr(hWnd, GWLP_HINSTANCE)))
  , m_isTls(dynamic_cast<WacomGSS::STU::TlsInterface*>(intf.get())!=nullptr)
  , m_tablet(std::move(intf))
  , m_layoutNum(layoutNum)
  , m_queue(std::move(m_tablet.interfaceQueue()))
  {

    WacomGSS::Win32::hresult_succeeded(::CoCreateInstance(CLSID_WICImagingFactory1, nullptr, CLSCTX_INPROC_SERVER, __uuidof(m_pIWICImagingFactory), &m_pIWICImagingFactory));

    m_capability = m_tablet.getCapability();
    m_thread = std::move(std::thread(std::ref(*this)));
  }

  ~KeyPad()
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

  void operator()() noexcept
  {
    try
    {
      bool KeyPadSupport = m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_OperationMode);

      if (!KeyPadSupport)
        throw loadString(m_hInstance, IDS_NotSupported);

      initKeyPadMode();

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
        std::lock_guard<decltype(m_mutex)> lock(m_mutex);
        m_readerException = std::current_exception();
      }
      ::PostMessage(m_hWnd, WM_KEYPAD_EXCEPTION, 0, 0);
    }
  }

  void handleException()
  {
    try
    {
      {
        std::lock_guard<decltype(m_mutex)> lock(m_mutex);
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

private:
  static const std::array<std::pair<LPCWSTR, LPCWSTR>, 3> m_bitmapId;
  static std::array<std::array<bool, 9>, 3>       m_btnEnabled;
  std::mutex                          m_mutex;
  HWND                                m_hWnd;
  HINSTANCE                           m_hInstance;
  uint8_t                             m_layoutNum;
  WacomGSS::Win32::com_ptr<IWICImagingFactory> m_pIWICImagingFactory; // Factory for creating WIC bitmaps
  bool                                m_isTls;
  WacomGSS::STU::Tablet               m_tablet;
  WacomGSS::STU::Protocol::Capability m_capability;
  WacomGSS::STU::InterfaceQueue       m_queue; // buffer between Tablet class and this application
  std::thread                         m_thread;
  std::exception_ptr                  m_readerException;


  void onReport(WacomGSS::STU::Protocol::EventDataKeyPad & e)
  {
    ::PostMessage(m_hWnd, WM_KEYPAD_RESULT, MAKEWORD(e.keyNumber, e.screenSelected), 0);
  }

  void checkImage(LPCWSTR bmpId, bool pushed)
  {
    WacomGSS::STU::Protocol::EncodingMode      encodingMode = m_tablet.supportsWrite() ? WacomGSS::STU::Protocol::EncodingMode_24bit_Bulk : WacomGSS::STU::Protocol::EncodingMode_24bit;
    WacomGSS::STU::Protocol::RomStartImageData romStartImageData = WacomGSS::STU::Protocol::RomStartImageData::initializeKeyPad(encodingMode, pushed, m_layoutNum, m_layoutNum, m_btnEnabled[m_layoutNum-1]);

    uploadImage(m_tablet, m_capability,
                WacomGSS::STU::Protocol::OperationModeType_KeyPad, pushed, m_layoutNum,
                m_hInstance, bmpId, m_pIWICImagingFactory,
                romStartImageData);
  }

  void initKeyPadMode()
  {
    using namespace WacomGSS::STU;

    checkImage(m_bitmapId[m_layoutNum-1].first, false);
    checkImage(m_bitmapId[m_layoutNum-1].second, true);

    Protocol::OperationMode_KeyPad KeyPadMode{ m_layoutNum, 0 };
    Protocol::OperationMode opMode = Protocol::OperationMode::initializeKeyPad(KeyPadMode);
    m_tablet.setOperationMode(opMode);
  }


};

const std::array<std::pair<LPCWSTR, LPCWSTR>, 3> KeyPad::m_bitmapId = { {
    { MAKEINTRESOURCE(IDB_KEYPAD1_BTNS_UP), MAKEINTRESOURCE(IDB_KEYPAD1_BTNS_DN) },
    { MAKEINTRESOURCE(IDB_KEYPAD2_BTNS_UP), MAKEINTRESOURCE(IDB_KEYPAD2_BTNS_DN) },
    { MAKEINTRESOURCE(IDB_KEYPAD3_BTNS_UP), MAKEINTRESOURCE(IDB_KEYPAD3_BTNS_DN) }
  } };
std::array<std::array<bool, 9>, 3> KeyPad::m_btnEnabled = { {
    { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
    { 1, 1, 1, 1, 1, 1, 0, 0, 0 },
    { 1, 1, 1, 1, 0, 0, 0, 0, 0 }
  } };

struct WndData
{
  HWND                      m_hWnd;
  std::mutex                m_mutex;
  std::unique_ptr<KeyPad>   m_KeyPad;

  WndData(HWND hWnd) : m_hWnd(hWnd) {}

  void handleException()
  {
    if (!!m_KeyPad)
    {
      m_KeyPad->handleException();
      onStop();
    }
  }

  void onInitDialog()
  {
    ::SendDlgItemMessage(m_hWnd, IDC_Dialog_Layout1, BM_SETCHECK, BST_CHECKED, 0);
    initSerial(m_hWnd);
  }

  void onStart()
  {
    std::unique_ptr<WacomGSS::STU::Interface> intf;
    if (doConnect(m_hWnd, intf))
    {
      auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(m_hWnd, GWLP_HINSTANCE));

      int layout = 0;
      if (::SendDlgItemMessage(m_hWnd, IDC_Dialog_Layout1, BM_GETCHECK, 0, 0) == BST_CHECKED)
      {
        layout = 1;
      }
      else if (::SendDlgItemMessage(m_hWnd, IDC_Dialog_Layout2, BM_GETCHECK, 0, 0) == BST_CHECKED)
      {
        layout = 2;
      }
      else
      {
        layout = 3;
      }

      enableCtrls(FALSE);
      m_KeyPad.reset(new KeyPad(m_hWnd, std::move(intf), layout));
    }
  }

  void onStop()
  {
    m_KeyPad.reset();
    enableCtrls(TRUE);
  }

  void enableCtrls(BOOL bEnable)
  {
    std::array<UINT, 6>  toEnable{
      IDC_Dialog_Layout1,
      IDC_Dialog_Layout2,
      IDC_Dialog_Layout3,
      IDC_Dialog_Start
    };
    std::array<UINT, 1>  toDisable{
      IDC_Dialog_Stop
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
            case IDC_Dialog_Start:
              GetWndData(hWnd)->onStart();
            break;

            case IDC_Dialog_Stop:
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
          wd->onInitDialog();
        }
        break;

      case WM_CLOSE:
        EndDialog(hWnd, 0);
        break;

      case WM_DESTROY:
        delete GetWndData(hWnd);
        break;

      case WM_KEYPAD_RESULT:
      {
        GetWndData(hWnd)->onStop();

        auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));
        uint8_t keyNumber = LOBYTE(wParam);
        uint8_t screenSelected = HIBYTE(wParam);
        WCHAR msg[256];

        StringCbPrintf(msg, sizeof(msg),
                      loadString(hInstance, IDS_DemoKeyPad_Result).c_str(), screenSelected, keyNumber);
        MessageBox(hWnd, msg, NULL, MB_ICONINFORMATION | MB_OK);

      }
      break;

      case WM_KEYPAD_EXCEPTION:
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
