#include "DeleteRomImages.h"
#include <WacomGSS/STU/Tablet.hpp>

//==============================================================================

struct Factory
{
};


class DeleteRomImages : Factory
{
public:
  static void action(HINSTANCE hInstance, HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> && intf, Factory const & )
  {
    try 
    {
      WacomGSS::STU::Tablet m_tablet(std::move(intf));

      if (m_tablet.isSupported(WacomGSS::STU::Protocol::ReportId_RomImageDelete))
      {
        m_tablet.setRomImageDelete(WacomGSS::STU::Protocol::RomImageDeleteMode_All, false, 0); 
        MessageBox(hWnd, loadString(hInstance, IDS_DeleteRomImages_Done).c_str(), loadString(hInstance, IDS_Title).c_str(), MB_ICONEXCLAMATION|MB_OK);
      }
      else
      {
        MessageBox(hWnd, loadString(hInstance, IDS_NotSupported).c_str(), loadString(hInstance, IDS_Title).c_str(), MB_ICONEXCLAMATION|MB_OK);
      }
    }
    catch (...)
    {
      handleException(hWnd);
    }
  }

  //static void registerWindow(HINSTANCE) { }
};



class Dialog
{  
  Factory m_factory;

  Dialog()
  {
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
                DeleteRomImages::action(hInstance, hWnd, std::move(intf), m_factory);
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
    std::unique_ptr<Dialog> cls(new Dialog);
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
    //DeleteRomImages::registerWindow(hInstance);
    
    return Dialog::dialogBox(hInstance);
  }
  catch (...)
  {
    handleException(nullptr);
  }
  return 1;
}
