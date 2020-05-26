#pragma once

#define IDC_Serial_Group          0x8101
#define IDC_Serial_Serial         0x8102
#define IDC_Serial_Port_Label     0x8103
#define IDC_Serial_Port           0x8104
#define IDC_Serial_BaudRate_Label 0x8105
#define IDC_Serial_BaudRate       0x8106

#define Serial_Default_Baud "128000"

#define IDS_Title                  0x01
#define IDS_NoTablet               0x02
#define IDS_Exception              0x03
#define IDS_ConnectFailed          0x04
#define IDS_InvalidPort            0x05
#define IDS_InvalidBaudRate        0x06
#define IDS_NotSupported           0x07


#ifdef __cplusplus

#include <WacomGSS/STU/Tablet.hpp>
#include <WacomGSS/STU/Protocol.hpp>
#include <WacomGSS/STU/TlsInterface.hpp>
#include <WacomGSS/Win32/windows.hpp>
#include <WacomGSS/Win32/com.hpp>
#include <WacomGSS/Win32/wincodec.hpp>

std::wstring loadString(HINSTANCE hInstance, UINT uID);
std::wstring getDlgItemText(HWND hDlg, int nIDDlgItem);

typedef decltype(WacomGSS::STU::Protocol::RomImageHash::hash) ImageHashValue;

ImageHashValue getImageHash(uint8_t const * imageData, ULONG imageSize);

void uploadImage(WacomGSS::STU::Tablet & tablet, WacomGSS::STU::Protocol::Capability const & capability,
                 WacomGSS::STU::Protocol::OperationModeType operationModeType, bool imageType, uint8_t imageNumber, 
                 HINSTANCE hInstance, LPCWSTR bitmapId, WacomGSS::Win32::com_ptr<IWICImagingFactory> & pIWICImagingFactory,
                 WacomGSS::STU::Protocol::RomStartImageData const & romStartImageData);


void handleException(HWND hwnd) noexcept;

bool doConnect(HWND hWnd, std::unique_ptr<WacomGSS::STU::Interface> & retVal);
INT_PTR handleSerial(HWND hWnd, WPARAM wParam);
void initSerial(HWND hWnd);


class ScopedCursor
{
  HCURSOR m_hCursor;
public:
  ScopedCursor(HCURSOR hCursor)
  :
    m_hCursor(::SetCursor(hCursor))
  {
  }
  ScopedCursor(HINSTANCE hInstance, LPCWSTR lpszCursor)
  :
    m_hCursor(::SetCursor(::LoadCursor(hInstance, lpszCursor)))
  {
  }
  ~ScopedCursor()
  {
    ::SetCursor(m_hCursor);
  }
};


namespace WacomGSS
{
  namespace ut
  {
    Win32::com_ptr<IWICStream> IWICStreamFromResource(HMODULE hModule, PCWSTR pszName, PCWSTR pszType, Win32::com_ptr<IWICImagingFactory> const & pIWICImagingFactory);
  }
}


#endif
