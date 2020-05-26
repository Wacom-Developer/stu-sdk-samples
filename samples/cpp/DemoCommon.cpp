// common routines between sample code
#include "DemoCommon.hpp"
#include <WacomGSS/STU/getUsbDevices.hpp>
#include <WacomGSS/STU/UsbInterface.hpp>
#include <WacomGSS/STU/TlsInterface.hpp>
#include <WacomGSS/STU/TlsInterface_Debug.hpp>
#include <WacomGSS/STU/TlsProtocol.hpp>
#include <WacomGSS/STU/SerialInterface.hpp>
#include <WacomGSS/STU/Win32/getSerialPorts.hpp>
#include <WacomGSS/Win32/ntstatus.hpp>
#include <sstream>
#include <bcrypt.h>


std::wstring loadString(HINSTANCE hInstance, UINT uID)
{
  PWSTR p = nullptr;
  int   l = ::LoadStringW(hInstance, uID, reinterpret_cast<PWSTR>(&p), 0);
  WacomGSS::Win32::win32api_bool(l > 0 && p != nullptr, "LoadString");  
  return std::wstring(p, static_cast<std::wstring::size_type>(l));
}



std::wstring getDlgItemText(HWND hDlg, int nIDDlgItem)
{
  HWND hCtl = ::GetDlgItem(hDlg, nIDDlgItem);
  auto l = SendMessage(hCtl, WM_GETTEXTLENGTH, 0, 0);
  std::unique_ptr<WCHAR[]> b(new WCHAR[l+1u]);
  SendMessage(hCtl, WM_GETTEXT, l+1u, reinterpret_cast<LPARAM>(b.get()));
  return std::wstring(b.get());
}



class BCryptAlgHandle
{
  BCRYPT_ALG_HANDLE handle;

  BCryptAlgHandle(BCryptAlgHandle const &) = delete;
  BCryptAlgHandle & operator = (BCryptAlgHandle const &) = delete;

public:
  BCryptAlgHandle()
  :
    handle(nullptr)
  {
  }


  ~BCryptAlgHandle()
  {
    if (handle != nullptr)
    {
      ::BCryptCloseAlgorithmProvider(handle, 0);
    }
  }


  BCRYPT_ALG_HANDLE * operator & () 
  {
    return &handle;
  }


  operator BCRYPT_ALG_HANDLE () const
  {
    return handle;
  }
};



class BCryptHashHandle
{
  BCRYPT_HASH_HANDLE handle;

  BCryptHashHandle(BCryptHashHandle const &) = delete;
  BCryptHashHandle & operator = (BCryptHashHandle const &) = delete;

public:
  BCryptHashHandle()
  :
    handle(nullptr)
  {
  }


  ~BCryptHashHandle()
  {
    if (handle != nullptr)
    {
      ::BCryptDestroyHash(handle);
    }
  }


  BCRYPT_HASH_HANDLE * operator & () 
  {
    return &handle;
  }


  operator BCRYPT_HASH_HANDLE () const
  {
    return handle;
  }
};



typedef decltype(WacomGSS::STU::Protocol::RomImageHash::hash) ImageHashValue;



ImageHashValue getImageHash(uint8_t const * imageData, ULONG imageSize)
{
  BCryptAlgHandle algHandle;
  WacomGSS::Win32::win32api_NTSTATUS(::BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_MD5_ALGORITHM, nullptr, 0), "BCryptOpenAlgorithmProvider");

  BCryptHashHandle hashHandle;
  WacomGSS::Win32::win32api_NTSTATUS(::BCryptCreateHash(algHandle, &hashHandle, nullptr, 0, nullptr, 0, 0), "BCryptCreateHash");

  WacomGSS::Win32::win32api_NTSTATUS(::BCryptHashData(hashHandle, const_cast<uint8_t *>(imageData), imageSize, 0), "BCryptHashData");

  ImageHashValue hashValue;
  WacomGSS::Win32::win32api_NTSTATUS(::BCryptFinishHash(hashHandle, hashValue.data(), hashValue.size(), 0), "BCryptFinishHash");

  return hashValue;
}


void uploadImage(WacomGSS::STU::Tablet & tablet, WacomGSS::STU::Protocol::Capability const & capability,
                 WacomGSS::STU::Protocol::OperationModeType operationModeType, bool imageType, uint8_t imageNumber, 
                 HINSTANCE hInstance, LPCWSTR bitmapId, WacomGSS::Win32::com_ptr<IWICImagingFactory> & pIWICImagingFactory,
                 WacomGSS::STU::Protocol::RomStartImageData const & romStartImageData)
{
  using namespace WacomGSS;
  using namespace WacomGSS::STU;

  tablet.setRomImageHash(operationModeType, imageType, imageNumber);
  STU::Protocol::RomImageHash romImgHash = tablet.getRomImageHash();

  Win32::com_ptr<IWICStream> pIWICStream = ut::IWICStreamFromResource(hInstance, bitmapId, RT_RCDATA, pIWICImagingFactory);
  Win32::com_ptr<IWICBitmapDecoder> pIWICBitmapDecoder;
  Win32::hresult_succeeded(pIWICImagingFactory->CreateDecoderFromStream(pIWICStream.get(), nullptr, WICDecodeMetadataCacheOnDemand, &pIWICBitmapDecoder));
  Win32::com_ptr<IWICBitmapFrameDecode> pWICBitmapSource;
  Win32::hresult_succeeded(pIWICBitmapDecoder->GetFrame(0, &pWICBitmapSource));

  auto  bitmapData = STU::ProtocolHelper::flatten(pIWICImagingFactory.get(), pWICBitmapSource.get(), capability.screenWidth, capability.screenHeight, Protocol::EncodingMode_24bit);
  bool  writeImage = true;

  if (romImgHash.result == 0)
  {
    // there is already an image on the tablet - check it is the right one
    ImageHashValue  imgHash = getImageHash(bitmapData.data(), bitmapData.size());
    if (imgHash == romImgHash.hash)
    {
      // image matches, no need to write it again
      writeImage = false;
    }
  }
  // else - no image on pad, writeImage = true;

  if (writeImage)
  {
    ScopedCursor waitCursor(nullptr, IDC_WAIT);
    tablet.writeRomImage(romStartImageData, bitmapData.data(), bitmapData.size());
  }
}





void handleException(HWND hwnd) noexcept
{
  try
  {
    std::wstringstream o;
    try 
    {
      throw;
    }
    catch (WacomGSS::STU::Interface::device_removed_error &)
    {
      o << "Device unplugged!";
    }
    catch (WacomGSS::STU::Interface::send_error & e)
    {
      using namespace WacomGSS::STU::ProtocolHelper::ostream_operators;

      char const * what = e.what();
      o << "TLS send error: ";
      if (what)
        o << what << " ";

      {
        std::stringstream oo;
        oo << static_cast<WacomGSS::STU::TlsProtocol::ReturnValueStatus>(e.value());
        o << "- " << oo.str().c_str();
      }

      if (e.packetId() || e.reportId())
      {        
        o << " - ";
        if (e.packetId())
        {
          std::stringstream oo;
          oo << static_cast<WacomGSS::STU::TlsProtocol::PacketId>(e.packetId());
          o << oo.str().c_str() << " ";
        }
        if (e.reportId())
        {
          std::stringstream oo;
          oo << static_cast<WacomGSS::STU::Protocol::ReportId>(e.reportId());
          o << oo.str().c_str();
        }
      }
    }
    catch (std::system_error & e)
    {
      o << "exception: system_error: ";
      char const * what = e.what();
      if (what)
        o << what;
      else
        o << typeid(e).name();

      o << " - ";
      o << e.code();
      o << " \"";
      o << e.code().message().c_str();
      o << "\"";
    }
    catch (std::exception & e)
    {
      o << "exception: ";
      char const * what = e.what();
      if (what)
        o << what;
      else
        o << typeid(e).name();
    }
    catch (std::wstring & wstr)
    {
      o << "exception: " << wstr;
    }
    catch (...)
    {
      o << "unknown exception!";
    }

    //auto s = o.str();

    //std::wstring_convert<std::codecvt_utf8<wchar_t, 0x10ffff, std::little_endian> > conv;
    //auto ws = conv.from_bytes(s);

    HINSTANCE hInstance = reinterpret_cast<HINSTANCE>(::GetWindowLongPtr(hwnd, GWLP_HINSTANCE));

    ::MessageBox(hwnd, o.str().c_str(), loadString(hInstance, IDS_Exception).c_str(), MB_ICONSTOP|MB_OK);          
  }
  catch (...)
  {
  }
}



bool doConnect(HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> & retVal)
{
  auto hInstance = reinterpret_cast<HINSTANCE>(GetWindowLongPtr(hWnd, GWLP_HINSTANCE));

  bool useSerial  = ::IsDlgButtonChecked(hWnd, IDC_Serial_Serial) != BST_UNCHECKED;

  if (!useSerial)
  {
    {
      auto tlsDevices = WacomGSS::STU::getTlsDevices();
      if (!tlsDevices.empty())
      {
        auto const & tlsDevice = tlsDevices.front();
        auto intf = std::make_unique<WacomGSS::STU::TlsInterface>();


        intf->setDebug(WacomGSS::STU::TlsInterface_Debug_OutputDebugString());


        auto ec = intf->connect(tlsDevice, WacomGSS::STU::TlsInterface::ConnectOption_SSL);
        if (!ec) 
        {
          {
            WacomGSS::STU::TlsProtocol protocol(*intf);
            protocol.sendProtocolVersion(1);
          }
          retVal = std::move(intf);
          return true;
        }
        else
        {
          auto msg = loadString(hInstance, IDS_ConnectFailed);
          ::MessageBox(hWnd, msg.c_str(), nullptr, MB_ICONSTOP|MB_OK);
        }
      }
    }

    auto usbDevices = WacomGSS::STU::getUsbDevices();
    if (!usbDevices.empty())
    {
      auto const & usbDevice = usbDevices.front();
      auto intf = std::make_unique<WacomGSS::STU::UsbInterface>();
      auto ec = intf->connect(usbDevice, true);
      if (!ec) 
      {
        retVal = std::move(intf);
        return true;
      }
      else
      {
        auto msg = loadString(hInstance, IDS_ConnectFailed);
        ::MessageBox(hWnd, msg.c_str(), nullptr, MB_ICONSTOP|MB_OK);
      }
    }
    else
    {
      MessageBox(hWnd, loadString(hInstance, IDS_NoTablet).c_str(), nullptr, MB_ICONEXCLAMATION|MB_OK);
    }
  }
  else
  {
    auto portName = getDlgItemText(hWnd, IDC_Serial_Port);
    if (!portName.empty())
    {
      {
        auto pos = portName.find(L'\t');
        if (pos != portName.npos)
        {
          portName.resize(pos);
        }
      }

      BOOL translated = FALSE;
      auto baudRate = ::GetDlgItemInt(hWnd, IDC_Serial_BaudRate, &translated, FALSE);
      if (translated && baudRate >0)
      {
        auto intf = std::make_unique<WacomGSS::STU::SerialInterface>();
        auto ec = intf->connect(portName, baudRate, true);
        if (!ec) 
        {
          retVal = std::move(intf);
          return true;
        }
        else
        {
          auto msg = loadString(hInstance, IDS_ConnectFailed);
          ::MessageBox(hWnd, msg.c_str(), nullptr, MB_ICONSTOP|MB_OK);
        }
      }
      else
      {
        MessageBox(hWnd, loadString(hInstance, IDS_InvalidBaudRate).c_str(), nullptr, MB_ICONEXCLAMATION|MB_OK);
      }
    }
    else
    {
      MessageBox(hWnd, loadString(hInstance, IDS_InvalidPort).c_str(), nullptr, MB_ICONEXCLAMATION|MB_OK);
    }
  }
  return false; 
}


INT_PTR handleSerial(HWND hWnd, WPARAM wParam)
{
  INT_PTR bSuccess;

  if (LOWORD(wParam) == IDC_Serial_Serial && HIWORD(wParam) == BN_CLICKED)
  {
    BOOL useSerial = (::IsDlgButtonChecked(hWnd, IDC_Serial_Serial) != BST_UNCHECKED) ? TRUE : FALSE;
    ::EnableWindow(::GetDlgItem(hWnd, IDC_Serial_Port_Label    ), useSerial);
    ::EnableWindow(::GetDlgItem(hWnd, IDC_Serial_Port          ), useSerial);
    ::EnableWindow(::GetDlgItem(hWnd, IDC_Serial_BaudRate_Label), useSerial);
    ::EnableWindow(::GetDlgItem(hWnd, IDC_Serial_BaudRate      ), useSerial);

    bSuccess = TRUE;
  }
  else
  {
    bSuccess = FALSE;
  }
  return bSuccess;
}



void initSerial(HWND hWnd)
{
  try
  {
    auto serialPorts = WacomGSS::STU::getSerialPorts();
    if (!serialPorts.empty())
    {
      HWND hCtl = ::GetDlgItem(hWnd, IDC_Serial_Port);
      WPARAM wIndex = 0;
      WPARAM counter = 0;
      for (auto const & serialPort : serialPorts)
      {
        std::wstring s(serialPort.name);
        s.append(L"\t");

        switch (serialPort.type)
        {
          default:
          case WacomGSS::STU::SerialPort::Unknown: s.append(L"(unknown)"); break;
          case WacomGSS::STU::SerialPort::Physical: s.append(L"(physical)"); break;
          case WacomGSS::STU::SerialPort::Virtual: 
            s.append(L"(virtual)"); 
            if (wIndex == 0)
              wIndex = counter;
            break;
          case WacomGSS::STU::SerialPort::Remote: 
            s.append(L"(remote)"); 
            if (wIndex == 0)
              wIndex = counter;
            break;
        }
        SendMessage(hCtl, CB_ADDSTRING, 0, reinterpret_cast<LPARAM>(s.c_str()));
        ++counter;
      }
      SendMessage(hCtl, CB_SETCURSEL, 0, 0);
    }
  
  }
  catch(...)
  {
    handleException(hWnd);
  }
}



namespace WacomGSS
{
  namespace ut
  {
    Win32::com_ptr<IWICStream> IWICStreamFromResource(HMODULE hModule, PCWSTR pszName, PCWSTR pszType, Win32::com_ptr<IWICImagingFactory> const & pIWICImagingFactory)
    {
      using namespace Win32;

      HRSRC hRsrc = ::FindResource(hModule, pszName, pszType);
      if (hRsrc) 
      {
        DWORD dwSize = ::SizeofResource(hModule, hRsrc);
        if (dwSize) 
        {
          HGLOBAL hGlobal = ::LoadResource(hModule, hRsrc);
          if (hGlobal) 
          {
            LPVOID pRsrc = ::LockResource(hGlobal);
            if (pRsrc) 
            {
              com_ptr<IWICStream> pIWICStream;
          
              hresult_succeeded(pIWICImagingFactory->CreateStream(&pIWICStream));

              hresult_succeeded(pIWICStream->InitializeFromMemory(reinterpret_cast<LPBYTE>(pRsrc), dwSize));
          
              return std::move(pIWICStream);
            }
            else
            {
              throw_win32api_error(::GetLastError(), "LockResource");
            }
          }
          else
          {
            throw_win32api_error(::GetLastError(), "LoadResource");
          }
        }
        else
        {
          throw_win32api_error(::GetLastError(), "SizeofResource");
        }
      }
      else
      {
        throw_win32api_error(::GetLastError(), "FindResource");
      }

      // cannot get here
    }
  }
}


