/// @file      query.cpp
/// @copyright Copyright (c) 2011 Wacom Company Limited
/// @author    mholden
/// @date      2012-03-22
/// @brief     simple test harness

/// TODO: Add comments!

#include <WacomGSS/STU/getUsbDevices.hpp>
#include <WacomGSS/STU/UsbInterface.hpp>
#include <WacomGSS/STU/TlsInterface.hpp>
#include <WacomGSS/STU/TlsProtocol.hpp>
#include <WacomGSS/STU/SerialInterface.hpp>
#include <WacomGSS/STU/ProtocolHelper.hpp>

#if defined(WacomGSS_WIN32)
#include <codecvt>
#include <locale>
#endif

#include <iostream>
#include <sstream>
#include <iomanip>


using namespace WacomGSS::STU::ProtocolHelper::ostream_operators;


const int k_column=42;
bool g_force;
                           // 0 - = no force, not supported
                           // 1 ! = forced, not supported
                           // 2 = = supported
                           // 3 = = forced, supported

const char g_forceChar[] = { '-', '!', '=', '=' };
std::array<uint16_t,256> g_reportCountLengths;


template<typename InputIterator> struct ArrHex
{
  InputIterator begin, end;
  ArrHex(InputIterator const & b, InputIterator const & e) : begin(b), end(e) {}
};
template<typename InputIterator> std::ostream & operator << (std::ostream & o, ArrHex<InputIterator> const & arr)
{
  o << std::hex << std::setfill('0');
  for (auto i = arr.begin; i != arr.end; ++i)
    o << std::setw(2) << (unsigned)*i;
  o << std::dec << std::setfill(' ');
  return o;
}
template<typename InputIterator> auto arrhex(InputIterator const & b, InputIterator const & e) -> ArrHex<InputIterator>               { return ArrHex<InputIterator>(b, e); }
template<typename Container>     auto arrhex(Container const & c)                              -> decltype(arrhex(c.begin(),c.end())) { return arrhex(c.begin(), c.end()); }







std::ostream & operator << (std::ostream & o, WacomGSS::STU::Protocol::EncodingFlag eF)
{
  std::uint8_t encodingFlag(eF);
  bool flag = false;

#define WacomGSS_EncodingFlag_Entry(Name) \
  if (encodingFlag & WacomGSS::STU::Protocol::EncodingFlag_##Name)   \
  {                                         \
    flag = true;                            \
    o << #Name;                             \
    encodingFlag &= ~WacomGSS::STU::Protocol::EncodingFlag_##Name; \
  }

  WacomGSS_EncodingFlag_Entry(24bit)
  WacomGSS_EncodingFlag_Entry(16bit)
  WacomGSS_EncodingFlag_Entry(1bit)
  WacomGSS_EncodingFlag_Entry(Zlib)
  if (encodingFlag || !flag)
  {
    if (flag)
      o << '|';
    o << "0x" << std::hex << std::setfill('0') << std::setw(2) << (unsigned)encodingFlag;
  }
  return o;
}



std::ostream & fmt(int f, char const * n)
{
  using namespace std;
  cout << setw(k_column) << left << n << g_forceChar[f] << " " << right;
  return cout;
}



void romImageHash_common(int f, WacomGSS::STU::Protocol & protocol, WacomGSS::STU::Protocol::OperationModeType operationModeType, bool pushed, uint8_t imageNumber, std::stringstream & o) 
{
  using namespace std;
  using namespace WacomGSS::STU;


//  ProtocolHelper::waitForStatusToSend(protocol, Protocol::ReportId_RomImageHash, ProtocolHelper::OpDirectionSet);

  auto status = protocol.getStatus();
  if (ProtocolHelper::statusCanSend(status.statusCode, Protocol::ReportId_RomImageHash, ProtocolHelper::OpDirection_Set))
  {
    protocol.setRomImageHash(operationModeType, pushed, imageNumber);

    ProtocolHelper::waitForStatusToSend(protocol, Protocol::ReportId_RomImageHash, ProtocolHelper::OpDirection_Get);
    Protocol::RomImageHash hash = protocol.getRomImageHash();

    if (hash.result == 0)
    {
      fmt(f,o.str().c_str()) << arrhex(hash.hash);
    }
    else
    {
      fmt(f,o.str().c_str()) << "not stored";
    }
  }
  else
  {
    fmt(f,o.str().c_str()) << "not supported in current statusCode " << status.statusCode;
  }
  cout << endl;
}



void romImageHash(int f, WacomGSS::STU::Protocol & protocol, WacomGSS::STU::Protocol::OperationModeType operationModeType, uint8_t imageNumber) 
{
  using namespace std;
  
  stringstream o;
  o << "RomImageHash[" << operationModeType << "," << (unsigned)imageNumber << "]";

  romImageHash_common(f, protocol, operationModeType, false, imageNumber, o);
}



void romImageHash(int f, WacomGSS::STU::Protocol & protocol, WacomGSS::STU::Protocol::OperationModeType operationModeType, bool pushed, uint8_t imageNumber) 
{
  using namespace std;

  stringstream o;
  o << "RomImageHash[" << operationModeType << "," << (unsigned)imageNumber <<"," << (pushed?"pushed":"normal") << "]";

  romImageHash_common(f, protocol, operationModeType, pushed, imageNumber, o);
}


void funcTls(std::uint16_t reportId, char const * n, std::function<void(int f, char const * n)> fn)
{
  using namespace std;
  const bool supported = true;
  int  f = g_force*1 + supported*2;

  try
  {
    fn(f,n);
  }
  catch (WacomGSS::STU::Interface::not_connected_error const & ex)
  {
    fmt(f,n) << "not_connected_error" << endl;
    throw;
  }
  catch (WacomGSS::STU::Interface::io_error const & ex)
  {
    fmt(f,n) << "io_error: " << ex.what() << endl;
  }
  catch (WacomGSS::STU::Interface::send_error const & ex)
  {
    fmt(f,n) << "send_error: " << hex << setw(4) << setfill('0') << ex.value() << dec << setfill(' ') << endl;
  }
  catch (std::system_error const & ex)
  {
    fmt(f,n) << "system_error: " << ex.what() << " " << ex.code() << " " << ex.code().message() << endl;
  }
  catch (std::runtime_error const & ex)
  {
    fmt(f,n) << "runtime_error: " << ex.what() << endl;
  }
  catch (std::exception const & ex)
  {
    fmt(f,n) << "exception: " << ex.what() << endl;
  }
}

void func(std::uint8_t reportId, char const * n, std::function<void(int f, char const * n)> fn)
{
  using namespace std;

  bool supported = g_reportCountLengths[reportId] != 0;
  int  f = g_force*1 + supported*2;
  try
  {
    if (g_force || supported)
    {
      fn(f,n);
      // Win32  = ERROR_GEN_FAILURE
      // libusb = ::LIBUSB_ERROR_PIPE
      // serial = timeout_error
    }
    else
    {
      fmt(f, n) << "not supported" << std::endl;
    }
  }
  catch (WacomGSS::STU::Interface::not_connected_error const & ex)
  {
    fmt(f,n) << "not_connected_error" << endl;
    throw;
  }
  catch (WacomGSS::STU::Interface::io_error const & ex)
  {
    fmt(f,n) << "io_error: " << ex.what() << endl;
  }
  catch (WacomGSS::STU::Interface::send_error const & ex)
  {
    fmt(f,n) << "send_error: " << hex << setw(4) << setfill('0') << ex.value() << dec << setfill(' ') << endl;
  }
  catch (std::system_error const & ex)
  {
    fmt(f,n) << "system_error: " << ex.what() << " " << ex.code() << " " << ex.code().message() << endl;
  }
  catch (std::runtime_error const & ex)
  {
    fmt(f,n) << "runtime_error: " << ex.what() << endl;
  }
  catch (std::exception const & ex)
  {
    fmt(f,n) << "exception: " << ex.what() << endl;
  }
}

# define ASN1_STRFLGS_ESC_2253           1
# define ASN1_STRFLGS_ESC_CTRL           2
# define ASN1_STRFLGS_ESC_MSB            4
# define ASN1_STRFLGS_UTF8_CONVERT       0x10
# define ASN1_STRFLGS_DUMP_UNKNOWN       0x100
# define ASN1_STRFLGS_DUMP_DER           0x200

# define ASN1_STRFLGS_RFC2253    (ASN1_STRFLGS_ESC_2253 | \
                                ASN1_STRFLGS_ESC_CTRL | \
                                ASN1_STRFLGS_ESC_MSB | \
                                ASN1_STRFLGS_UTF8_CONVERT | \
                                ASN1_STRFLGS_DUMP_UNKNOWN | \
                                ASN1_STRFLGS_DUMP_DER)

# define XN_FLAG_SEP_COMMA_PLUS  (1 << 16)/* RFC2253 ,+ */
# define XN_FLAG_DN_REV          (1 << 20)/* Reverse DN order */
# define XN_FLAG_FN_SN           0/* Object short name */
# define XN_FLAG_DUMP_UNKNOWN_FIELDS (1 << 24)

# define XN_FLAG_RFC2253 (ASN1_STRFLGS_RFC2253 | \
                        XN_FLAG_SEP_COMMA_PLUS | \
                        XN_FLAG_DN_REV | \
                        XN_FLAG_FN_SN | \
                        XN_FLAG_DUMP_UNKNOWN_FIELDS)


extern "C" int X509_NAME_print_ex_fp(void*, void*, int, int);
void queryCert(WacomGSS::STU::TlsInterface & intf)
{
  using namespace std;
  using namespace WacomGSS::STU;
  using namespace WacomGSS::OpenSSL;

  auto cert = intf.getPeerCertificate();
  auto name = X509_get_subject_name(cert);

  cout << "Peer Certificate Name =" << endl;
  int indent = 2;
//  int e = X509_NAME_print_ex_fp(stdout, name, indent, XN_FLAG_RFC2253);
  cout << endl;

}


void queryOOB(WacomGSS::STU::TlsProtocolOOB protocolOOB)
{
  using namespace std;
  using namespace WacomGSS::STU;
  
  int f = 2;

  {
    TlsProtocolOOB::Status status = protocolOOB.getStatus();
    fmt(f,"[OOB] Status.oobStatus") << static_cast<TlsProtocolOOB::OobStatus>(status.oobStatus) << endl;
    fmt(f,"[OOB] Status.oobExtendedStatus") << hex << setw(8) << setfill('0') << status.oobExtendedStatus << setfill(' ') << dec << endl;
  }

  {
    TlsProtocolOOB::Descriptor descriptor = protocolOOB.getDescriptor();
    fmt(f,"[OOB] Descriptor.descriptorFlags"       ) << descriptor.descriptorFlags          << endl;
    fmt(f,"[OOB] Descriptor.idVendor"              ) << hex << setw(4) << setfill('0') << descriptor.idVendor              << setfill(' ') << dec << endl;
    fmt(f,"[OOB] Descriptor.idProduct"             ) << hex << setw(4) << setfill('0') << descriptor.idProduct             << setfill(' ') << dec << endl;
    fmt(f,"[OOB] Descriptor.firmwareRevisionMajor" ) << hex << setw(4) << setfill('0') << descriptor.firmwareRevisionMajor << setfill(' ') << dec << endl;
    fmt(f,"[OOB] Descriptor.firmwareRevisionMinor" ) << hex << setw(4) << setfill('0') << descriptor.firmwareRevisionMinor << setfill(' ') << dec << endl;
    fmt(f,"[OOB] Descriptor.modelName"             ) << descriptor.modelNameNullTerminated  << endl;
  }


  {
    char const * n = "[OOB] ReportSizeCollection";

    TlsProtocolOOB::ReportSizeCollection reportSizeCollection = protocolOOB.getReportSizeCollection();
    fmt(f, n) << endl;
    for (unsigned int i = 0; i < 256; ++i)
    {
      stringstream s;
      s << static_cast<TlsProtocolOOB::ReportId>(i);
      if (s.str().size() > 3 || reportSizeCollection[i] != 0)
      {
        cout << "  " << setw(k_column) << static_cast<TlsProtocolOOB::ReportId>(i) << " ";
        if (reportSizeCollection[i])
        {
          cout << (unsigned)reportSizeCollection[i];
        }
        else
        {
          cout << "-";
        }
        cout << endl;
      }
    }
  }
  cout << endl;
}


void queryTLS(WacomGSS::STU::TlsProtocol protocol)
{
  using namespace std;
  using namespace WacomGSS::STU;
  
  funcTls(TlsProtocol::PacketId_ProtocolVersion, "ProtocolVersion", [&](int f, char const * n) 
  {
    TlsProtocol::ReturnValue_ProtocolVersion protocolVersion = protocol.sendProtocolVersion(0);

    fmt(2,"ProtocolVersion.returnValueStatus" ) << static_cast<TlsProtocol::ReturnValueStatus>(protocolVersion.returnValueStatus) << endl;
    fmt(2,"ProtocolVersion.activeLevel" ) << hex << protocolVersion.activeLevel << endl;
    for(size_t i = 0; i < protocolVersion.supportedLevels.size(); ++i)
    {
      uint16_t supportedLevel = protocolVersion.supportedLevels[i];
      stringstream o;
      o << "ProtocolVersion.supportedLevel[" << i << "]";
      fmt(2, o.str().c_str()) << hex << supportedLevel << endl;
    }
  });

}


void display(std::array<uint16_t,256> const & reportSizeCollection)
{
  using namespace std;
  using namespace WacomGSS::STU;

  for (unsigned int i = 0; i < 256; ++i)
  {
    stringstream s;
    s << static_cast<Protocol::ReportId>(i);
    if (s.str().size() > 3 || reportSizeCollection[i] != 0)
    {
      cout <<  "  " << setw(k_column) << static_cast<Protocol::ReportId>(i) << " "; 
      if (reportSizeCollection[i])
      {
        cout << (unsigned)reportSizeCollection[i]; 
      }
      else
      {
        cout << "-"; 
      }
      cout << endl;
    }
  }
}


void query(WacomGSS::STU::Protocol protocol)
{
  using namespace std;
  using namespace WacomGSS::STU;

  // 0x01 PenData

  // 0x02 -

  // 0x03
  Protocol::Status status = protocol.getStatus();
  fmt(2, "Status.statusCode    ") << status.statusCode << endl;
  fmt(2, "Status.lastResultCode") << status.lastResultCode << endl;
  fmt(2, "Status.statusWord    ") << hex << setw(2) << setfill('0') << status.statusWord << dec << setfill(' ') << endl;

  bool rcl = protocol->getReportCountLengths(g_reportCountLengths);
  if (!rcl)
  {
#ifdef WacomGSS_WIN32
    cout << "unable to get reportCountLengths" << endl;
    if (!g_force)
      return;
#else
    cout << "Warning: unable to get reportCountLengths" << endl;
    g_force = true;
#endif
  }
  else
  {
//    fmt(2, "getReportCountLengths") << endl;
//    display(g_reportCountLengths);
  }

  // 0x04 - Reset
  // 0x05 -

  // 0x06
  func(Protocol::ReportId_HidInformation, "HidInformation", [&](int f, char const * n) 
  {
    Protocol::HidInformation hidInformation = protocol.getHidInformation();
    fmt(f,n) << hex << setfill('0') << setw(4) << hidInformation.idVendor << ':' << setw(4) << hidInformation.idProduct << ':' << setw(4) << hidInformation.bcdDevice << dec << setfill(' ') << endl;
  });

  // 0x07 -

  // 0x08
  func(Protocol::ReportId_Information, "Information", [&](int f, char const * n) 
  {
    Protocol::Information inf = protocol.getInformation();
    fmt(f,"Information.modelName"            )<< inf.modelNameNullTerminated        << endl;
    fmt(f,"Information.firmwareMajorVersion" )<< hex << setw(2) << setfill('0') << (unsigned)inf.firmwareMajorVersion << dec << setfill(' ') << endl;
    fmt(f,"Information.firmwareMinorVersion" )<< hex << setw(2) << setfill('0') << (unsigned)inf.firmwareMinorVersion << dec << setfill(' ') << endl;
    fmt(f,"Information.secureIc"             )<< (unsigned)inf.secureIc             << endl;
    fmt(f,"Information.secureVersion"        )<< hex << (unsigned)inf.secureIcVersion[0] << '.'
                                                 << (unsigned)inf.secureIcVersion[1] << '.'
                                                 << (unsigned)inf.secureIcVersion[2] << '.'
                                                     << (unsigned)inf.secureIcVersion[3] <<
                                                 dec << endl;
  });


  // 0x09
  func(Protocol::ReportId_Capability, "Capability", [&](int f, char const * n) 
  {
    Protocol::Capability caps = protocol.getCapability();
    
    fmt(f, "Capability.tabletMaxX       ") << caps.tabletMaxX              << endl;
    fmt(f, "Capability.tabletMaxY       ") << caps.tabletMaxY              << endl;
    fmt(f, "Capability.tabletMaxPressure") << caps.tabletMaxPressure       << endl;
    fmt(f, "Capability.screenWidth      ") << caps.screenWidth             << endl;
    fmt(f, "Capability.screenHeight     ") << caps.screenHeight            << endl;
    fmt(f, "Capability.maxReportRate    ") << (unsigned)caps.maxReportRate << endl;
    fmt(f, "Capability.resolution       ") << caps.resolution              << endl;
    fmt(f, "Capability.encodingFlag     ") << Protocol::EncodingFlag(caps.encodingFlag) << endl;
  });

  // 0x0A
  func(Protocol::ReportId_Uid, "Uid", [&](int f, char const * n) 
  {
    uint32_t uid = protocol.getUid();
    fmt(f,n) << "0x" << hex << setw(8) << setfill('0') << uid << dec << setfill(' ') << endl;
  });

  // 0x0B
  func(Protocol::ReportId_Uid2, "Uid2", [&](int f, char const * n) 
  {
    Protocol::Uid2 uid2 = protocol.getUid2();
    fmt(f,n) << uid2.uid2NullTerminated << endl;
  });


  // 0x0C
  func(Protocol::ReportId_DefaultMode, "DefaultMode", [&](int f, char const * n) 
  {
    uint8_t defaultMode = protocol.getDefaultMode();
    fmt(f,n) << static_cast<Protocol::DefaultMode>(defaultMode) << endl;
  });

  // 0x0D
  func(Protocol::ReportId_ReportRate, "ReportRate", [&](int f, char const * n) 
  {
    uint8_t reportRate = protocol.getReportRate();
    fmt(f,n) << (unsigned)reportRate << endl;
  });


  // 0x0E
  func(Protocol::ReportId_RenderingMode, "RenderingMode", [&](int f, char const * n) 
  {
    uint8_t renderingMode = protocol.getRenderingMode();
    fmt(f,n) << static_cast<Protocol::RenderingMode>(renderingMode) << endl;
  });


  // 0x0F
  func(Protocol::ReportId_Eserial, "Eserial", [&](int f, char const * n) 
  {
    Protocol::Eserial eSerial = protocol.getEserial();
    fmt(f,n) << eSerial.eSerialNullTerminated << endl;
  });

  // 0x10 - PenDataEncrypted
  // 0x11 -
  // 0x12 -

  // 0x13
  func(Protocol::ReportId_HostPublicKey, "HostPublicKey", [&](int f, char const * n) 
  {
    Protocol::PublicKey hostPublicKey = protocol.getHostPublicKey();
    fmt(f,n) << arrhex(hostPublicKey) << endl;
  });


  // 0x14
  func(Protocol::ReportId_DevicePublicKey, "DevicePublicKey", [&](int f, char const * n) 
  {
    Protocol::PublicKey devicePublicKey = protocol.getDevicePublicKey();
    fmt(f,n) << arrhex(devicePublicKey) << endl;
  });

  // 0x15 - StartCapture
  // 0x16 - EndCapture
  // 0x17 -
  // 0x18 -
  // 0x19 -

  // 0x1A
  func(Protocol::ReportId_DHprime, "DHprime", [&](int f, char const * n) 
  {
    Protocol::DHprime dhPrime = protocol.getDHprime();
    fmt(f,n) << arrhex(dhPrime) << endl;
  });


  // 0x1B
  func(Protocol::ReportId_DHprime, "DHbase", [&](int f, char const * n) 
  {
    Protocol::DHbase dhBase = protocol.getDHbase();
    fmt(f,n) << arrhex(dhBase) << endl;
  });

  // 0x1C -
  // 0x1D -
  // 0x1E -
  // 0x1F -
  // 0x20 - ClearScreen

  // 0x21
  func(Protocol::ReportId_InkingMode, "InkingMode", [&](int f, char const * n) 
  {
    uint8_t inkingMode = protocol.getInkingMode();
    fmt(f,n) << static_cast<Protocol::InkingMode>(inkingMode) << endl;
  });


  // 0x22
  func(Protocol::ReportId_InkThreshold, "InkThreshold", [&](int f, char const * n) 
  {
    Protocol::InkThreshold inkThreshold = protocol.getInkThreshold();
    fmt(f,"InkThreshold.onPressureMark  ") << inkThreshold.onPressureMark << endl;
    fmt(f,"InkThreshold.offPressureMark ") << inkThreshold.offPressureMark << endl;
  });

  // 0x23 - ClearScreenArea
  // 0x24 - StartImageDataArea
  // 0x25 - StartImageData
  // 0x26 - ImageDataBlock
  // 0x27 - EndImageData

  // 0x28
  func(Protocol::ReportId_HandwritingThicknessColor, "HandwritingThicknessColor", [&](int f, char const * n) 
  {
    Protocol::HandwritingThicknessColor handwritingThicknessColor = protocol.getHandwritingThicknessColor();
    fmt(f, "HandwritingThicknessColor.penColor    ") << hex << setw(4) << setfill('0') << handwritingThicknessColor.penColor << setfill(' ') << dec << endl;
    fmt(f, "HandwritingThicknessColor.penThickness") << (unsigned)handwritingThicknessColor.penThickness << endl;
  });

  // 0x29
  func(Protocol::ReportId_BackgroundColor, "BackgroundColor", [&](int f, char const * n) 
  {
    uint16_t backgroundColor = protocol.getBackgroundColor();
    fmt(f,n) << hex << setw(4) << setfill('0') << backgroundColor << setfill(' ') << dec << endl;
  });


  // 0x2A
  func(Protocol::ReportId_HandwritingDisplayArea, "HandwritingDisplayArea", [&](int f, char const * n) 
  {
    Protocol::Rectangle area = protocol.getHandwritingDisplayArea();
    fmt(f, "HandwritingDisplayArea.upperLeftXpixel  ") << area.upperLeftXpixel  << endl;
    fmt(f, "HandwritingDisplayArea.upperLeftYpixel  ") << area.upperLeftYpixel  << endl;
    fmt(f, "HandwritingDisplayArea.lowerRightXpixel ") << area.lowerRightXpixel << endl;
    fmt(f, "HandwritingDisplayArea.lowerRightYpixel ") << area.lowerRightYpixel << endl;
  });

  // 0x2B
  func(Protocol::ReportId_BacklightBrightness, "BacklightBrightness", [&](int f, char const * n) 
  {
    uint16_t backlightBrightness = protocol.getBacklightBrightness();
    fmt(f,n) << backlightBrightness << endl;
  });


  // 0x2C
  func(Protocol::ReportId_ScreenContrast, "ScreenContrast", [&](int f, char const * n) 
  {
    uint16_t screenContrast = protocol.getScreenContrast();
    fmt(f,n) << hex << setw(4) << setfill('0') << screenContrast << setfill(' ') << endl;
  });


  // 0x2D
  func(Protocol::ReportId_HandwritingThicknessColor24, "HandwritingThicknessColor24", [&](int f, char const * n) 
  {
    Protocol::HandwritingThicknessColor24 handwritingThicknessColor24 = protocol.getHandwritingThicknessColor24();
    fmt(f,"HandwritingThicknessColor24.penColor    ") << hex << setw(6) << setfill('0') << handwritingThicknessColor24.penColor << setfill(' ') << dec << endl;
    fmt(f,"HandwritingThicknessColor24.penThickness") << (unsigned)handwritingThicknessColor24.penThickness << endl;
  });


  // 0x2E
  func(Protocol::ReportId_BackgroundColor24, "BackgroundColor24", [&](int f, char const * n) 
  {
    uint32_t backgroundColor24 = protocol.getBackgroundColor24();
    fmt(f,n) << hex << setw(6) << setfill('0') << backgroundColor24 << setfill(' ') << dec << endl;
  });

  // 0x2F - BootScreen
  // 0x30 - PenDataOption
  // 0x31 - PenDataEncryptedOption

  // 0x32 
  func(Protocol::ReportId_PenDataOptionMode, "PenDataOptionMode", [&](int f, char const * n) 
  {
    uint8_t penDataOptionMode = protocol.getPenDataOptionMode();
    fmt(f,n) << static_cast<Protocol::PenDataOptionMode>(penDataOptionMode) << endl;
  });

  // 0x33 - PenDataTimeCountSequenceEncrypted
  // 0x34 - PenDataTimeCountSequence
  // 0x35..0x3F
  // 0x40 - EncryptionCommand
  // 0x41..0x4F

  // 0x50
  func(Protocol::ReportId_EncryptionStatus, "EncryptionStatus", [&](int f, char const * n) 
  {
    Protocol::EncryptionStatus encryptionStatus = protocol.getEncryptionStatus();

    fmt(f,"EncryptionStatus.symmetricKeyType     ") << static_cast<Protocol::SymmetricKeyType>(encryptionStatus.symmetricKeyType) << endl;
    fmt(f,"EncryptionStatus.asymmetricPaddingType") << static_cast<Protocol::AsymmetricPaddingType>(encryptionStatus.asymmetricPaddingType) << endl;
    fmt(f,"EncryptionStatus.asymmetricKeyType    ") << static_cast<Protocol::AsymmetricKeyType>(encryptionStatus.asymmetricKeyType) << endl;
    fmt(f,"EncryptionStatus.statusCodeRSAe       ") << static_cast<Protocol::StatusCodeRSA>(encryptionStatus.statusCodeRSAe) << endl;
    fmt(f,"EncryptionStatus.statusCodeRSAn       ") << static_cast<Protocol::StatusCodeRSA>(encryptionStatus.statusCodeRSAn) << endl;
    fmt(f,"EncryptionStatus.statusCodeRSAc       ") << static_cast<Protocol::StatusCodeRSA>(encryptionStatus.statusCodeRSAc) << endl;
    fmt(f,"EncryptionStatus.lastResultCode       ") << static_cast<Protocol::ErrorCodeRSA>(encryptionStatus.lastResultCode) << endl;
    fmt(f,"EncryptionStatus.rng                  ") << (unsigned)encryptionStatus.rng << endl;
    fmt(f,"EncryptionStatus.sha1                 ") << (unsigned)encryptionStatus.sha1 << endl;
    fmt(f,"EncryptionStatus.aes                  ") << (unsigned)encryptionStatus.aes << endl;
  });

  // 0x51..0x8F


  // 0x90 - PinPadData
  // 0x91 - PinPadDataEncrypted
  // 0x92 - PinOperationMode

  // 0x93
  func(Protocol::ReportId_OperationMode, "OperationMode", [&](int f, char const * n) 
  {
    Protocol::OperationMode operationMode = protocol.getOperationMode();
    fmt(f,n) << static_cast<Protocol::OperationModeType>(operationMode.operationMode) << endl;
  });

  // 0x94 - StartROMImageData
  // 0x95 -
  
  // 0x96
  for (uint8_t imageNumber = 1; imageNumber <= 3; ++imageNumber)
  {
    func(Protocol::ReportId_RomImageHash, "RomImageHash/PinPad", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_PinPad, false, imageNumber);
    });

    func(Protocol::ReportId_RomImageHash, "RomImageHash/PinPad", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_PinPad, true, imageNumber);
    });
  }
  for (uint8_t imageNumber = 1; imageNumber <= 10; ++imageNumber)
  {
    func(Protocol::ReportId_RomImageHash, "RomImageHash/SlideShow", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_SlideShow, imageNumber);
    });
  }
  for (uint8_t imageNumber = 1; imageNumber <= 3; ++imageNumber)
  {
    func(Protocol::ReportId_RomImageHash, "RomImageHash/KeyPad", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_KeyPad, false, imageNumber);
    });

    func(Protocol::ReportId_RomImageHash, "RomImageHash/KeyPad", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_KeyPad, true, imageNumber);
    });
  }
  for (uint8_t imageNumber = 1; imageNumber <= 3; ++imageNumber)
  {
    func(Protocol::ReportId_RomImageHash, "RomImageHash/Signature", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_Signature, false, imageNumber);
    });

    func(Protocol::ReportId_RomImageHash, "RomImageHash/Signature", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_Signature, true, imageNumber);
    });
  }
  for (uint8_t imageNumber = 1; imageNumber <= 6; ++imageNumber)
  {
    func(Protocol::ReportId_RomImageHash, "RomImageHash/MessageBox", [&](int f, char const * n) 
    {
      romImageHash(f, protocol, Protocol::OperationModeType_MessageBox, imageNumber);
    });
  }

  // 0x97 - DeleteROMImage
  // 0x98 - CurrentMessageImageArea
  // 0x99 - UIEventData
  // 0x9A - UIEventDataEncrypted
  // 0x9B - DisplayROMImage
  // 0x9C..0xFE

  // 0xFF
  func(Protocol::ReportId_ReportSizeCollection, "ReportSizeCollection", [&](int f, char const * n) 
  {
    Protocol::ReportSizeCollection reportSizeCollection = protocol.getReportSizeCollection();
    fmt(f,n) << endl;

    display(reportSizeCollection);
  });

}



void queryUsb()
{
  using namespace std;
  using namespace WacomGSS::STU;

  {
    auto tlsDevices = getTlsDevices();
    if (!tlsDevices.empty())
    {
      for (auto const & tlsDevice : tlsDevices)
      {
        try
        {
#ifdef WacomGSS_WIN32
          wcout << "Device: " << tlsDevice.deviceName << endl;
#else
          cout << "TLS Device: " << hex << setfill('0') << setw(4) << tlsDevice.idVendor << ':' << setw(4) << tlsDevice.idProduct << ':' << setw(4) << tlsDevice.bcdDevice << dec << setfill(' ') << endl;
#endif

          TlsInterface intf;
          auto ec = intf.connect(tlsDevice, TlsInterface::ConnectOption_SSL);
          if (!ec)
          {
            queryCert(intf);
            queryOOB(intf);
            queryTLS(intf);
            {
              TlsProtocol protocol(intf);
              protocol.sendProtocolVersion(1);
            }
            query(intf);
            intf.disconnect();
          }
          else
          {
            cout << "Failed to connect:" << ec << endl;
          }
          cout << endl;
        }
        catch (system_error const & ex)
        {
          cout << "system error: " << ex.what() << " " << ex.code() << " " << ex.code().message() << endl;
        }
      }
    
      return;
    }
  }


  auto usbDevices = getUsbDevices();

  if (!usbDevices.empty()) 
  {
    for (auto const & usbDevice : usbDevices)
    {
      try
      {
        cout << "Device: " << hex << setfill('0') << setw(4) << usbDevice.idVendor << ':' << setw(4) << usbDevice.idProduct << ':' << setw(4) << usbDevice.bcdDevice << dec << setfill(' ') << endl;

        UsbInterface intf;
        auto ec = intf.connect(usbDevice, true);
        if (!ec)
        {
          query(intf);
          intf.disconnect();
        }
        else
        {
          cout << "Failed to connect:" << ec << endl;
        }
        cout << endl;
      }
      catch (system_error const & ex)
      {
        cout << "system error: " << ex.what() << " " << ex.code() << " " << ex.code().message() << endl;
      }
    }
  }
  else 
  {
    cout << "No USB devices found" << endl;
  }
}


void querySerial(char const * comPort, std::uint32_t baudRate)
{
#ifndef __MACH__
  using namespace std;
  using namespace WacomGSS::STU;
  try
  {
    wstring port;

#if defined(WacomGSS_WIN32)
    wstring_convert<codecvt_utf8_utf16<wchar_t, 0x10ffff, little_endian>,wchar_t> conv;
    auto comPort_s = conv.from_bytes(comPort);
#else
    auto comPort_s = comPort;
#endif
    cout << "connecting to " << comPort << "  " << baudRate << endl;
    
    SerialInterface intf;
    error_code ec = intf.connect(comPort_s, baudRate, true);
    if (!ec)
    {
      query(intf);
      intf.disconnect();
    }
    else
    {
      cout << "Failed to connect:" << ec << endl;
    }
    cout << endl;
  }
  catch (system_error const & ex)
  {
    cout << "system error: " << ex.what() << " " << ex.code() << " " << ex.code().message() << endl;
  }
#endif
}



int main(int, char * * argv)
{
  try
  {
    std::cout << "STU query sample" << std::endl << std::endl;

    if (*++argv && **argv == '-')
    {
      if (argv[0][1] == 'f') 
      {
        std::cout << "***force flag used - ignoring ReportCountLengths ***" << std::endl;
        g_force = true;
      }
      ++argv;
    }

    if (!*argv)
      queryUsb();
    else
    {
      char const * port = *argv;
      if (port)
      {
        char const * rate = *++argv;

        std::uint32_t baudRate;
        if (rate)
        {
          baudRate = std::strtoul(rate, nullptr, 0);
        }
        else
        {
          baudRate = WacomGSS::STU::SerialInterface::BaudRate_STU430;
        }
        querySerial(port, baudRate);
      }
      else
      {
        std::cout << "args: serialPort baudRate"  << std::endl;
        
      }
    }
  }
  catch (std::exception const & ex)
  {
    std::cout << "std::exception: " << ex.what() << std::endl;
  }

  return 0;
}
