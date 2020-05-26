/// @file      simple.cpp
/// @copyright Copyright (c) 2011 Wacom Company Limited
/// @author    mholden
/// @date      2012-03-08
/// @brief     simple test harness

/// TODO: Add comments!

#include <WacomGSS/STU/getUsbDevices.hpp>
#include <WacomGSS/STU/UsbInterface.hpp>
#include <WacomGSS/STU/TlsInterface.hpp>
#include <WacomGSS/STU/SerialInterface.hpp>
#include <WacomGSS/STU/ProtocolHelper.hpp>
#include <WacomGSS/STU/ReportHandler.hpp>

#ifdef WacomGSS_WIN32
#include <locale>
#include <codecvt>
#endif

#include <csignal>
#include <iostream>
#include <iomanip>

#include <WacomGSS/STU/TlsProtocol.hpp>


static WacomGSS::atomic<WacomGSS::STU::Interface *> g_intf;


extern "C"
{
  static void signalHandler(int) noexcept
  {
    auto intf = g_intf.load();
    intf->queueSetPredicateAll(true);
    intf->queueNotifyAll(); // notify the interface that the predicate has changed
  }
}


struct as_hex { uint32_t v; as_hex(uint32_t _):v(_){} };
std::ostream & operator << (std::ostream & o, as_hex const & v) { return o << std::setfill('0')<<std::hex<<std::setw(8)<<v.v<<std::dec<<std::setfill(' '); }

std::ostream & operator << (std::ostream & o, WacomGSS::STU::Protocol::PenData const & data)
{
  return o
    << std::setw(3) << static_cast<uint16_t>(data.rdy) << " "
    << std::setw(3) << static_cast<uint16_t>(data.sw)  << " "
    << std::setw(5) << data.pressure << " "
    << std::setw(5) << data.x << " "
    << std::setw(5) << data.y;
}

std::ostream & operator << (std::ostream & o, WacomGSS::STU::Protocol::PenDataTimeCountSequence const & data)
{
  return o
    << std::setw(3) << static_cast<uint16_t>(data.rdy) << " "
    << std::setw(3) << static_cast<uint16_t>(data.sw)  << " "
    << std::setw(5) << data.pressure << " "
    << std::setw(5) << data.x << " "
    << std::setw(5) << data.y << " "
    << std::setw(5) << data.timeCount << " "
    << std::setw(5) << data.sequence
    ;
}

struct Option { uint16_t v; Option(uint16_t _):v(_){} };

std::ostream & operator << (std::ostream & o, Option const & v)
{
  return o
    << " [" << std::setw(5) << v.v << "]";
}



class cout_ReportHandler : public WacomGSS::STU::ProtocolHelper::ReportHandler
{
public:
  void onReport(WacomGSS::STU::Protocol::PenData & data) override
  {
    std::cout << data << std::endl; 
  }

  void onReport(WacomGSS::STU::Protocol::PenDataOption & data) override
  {
    std::cout << data << Option(data.option) << std::endl;
  }

  void decrypt(uint8_t [16])
  {
    // not supported
  }

  void onReport(WacomGSS::STU::Protocol::PenDataEncrypted & data) override
  {
    std::cout
      << "<" << as_hex(data.sessionId) << "> " << data.penData[0] << std::endl
      << "           "                         << data.penData[1] << std::endl;
  }

  void onReport(WacomGSS::STU::Protocol::PenDataEncryptedOption & data) override
  {
    std::cout
      << "<" << as_hex(data.sessionId) << "> " << data.penData[0] << Option(data.option[0]) << std::endl
      << "           "                         << data.penData[1] << Option(data.option[1]) << std::endl;
  }

  void onReport(WacomGSS::STU::Protocol::PenDataTimeCountSequence & data) override
  {
    std::cout << data << std::endl;
  }


  void onReport(WacomGSS::STU::Protocol::DevicePublicKey &) override
  {
    std::cout << "received: DevicePublicKey (ignoring)" << std::endl;
  }

  template<class Iterator>
  void onUnknown(Iterator begin, Iterator end)
  {
    using namespace std;
    auto f = cout.fill('0');
    cout << hex;
    for (; begin != end; ++begin)
    {
      cout << ' ' << setw(2) << static_cast<unsigned>(*begin);
    }
    cout.fill(f);
    cout << dec << endl;
  }
};


void run(WacomGSS::STU::Protocol protocol)
{
  using namespace std;
  using namespace WacomGSS::STU;
  using namespace ProtocolHelper::ostream_operators;

  try
  {
    cout << "getInformation()... ";
    Protocol::Information inf = protocol.getInformation();
    cout << "modelName=" << inf.modelNameNullTerminated << " firmware=" << hex << setw(2) << setfill('0') << (unsigned)inf.firmwareMajorVersion << "." << setw(2) << (unsigned)inf.firmwareMinorVersion << dec << setfill(' ') << "." << (unsigned)inf.secureIcVersion[0] << std::endl;

    try
    {
      cout << "getReportRate()... ";
      auto reportRate = protocol.getReportRate();
      cout << (unsigned) reportRate << std::endl;
    }
    catch (std::system_error &)
    {
      cout << "NOT SUPPORTED" << endl;
    }
    
    
    bool supportedPenDataOptionMode = false;
    uint8_t penDataOptionMode;
    
    try
    {
      

      cout << "getPenDataOptionMode()... ";
      penDataOptionMode = protocol.getPenDataOptionMode();
      cout << "penDataOptionMode=" << static_cast<Protocol::PenDataOptionMode>(penDataOptionMode) << std::endl;
      
      cout << "setPenDataOptionMode(SequenceNumberTimeCount)... ";
      protocol.setPenDataOptionMode(Protocol::PenDataOptionMode_TimeCountSequence);
      cout << "ok!" << endl;;

      penDataOptionMode = protocol.getPenDataOptionMode();

      supportedPenDataOptionMode = true;
    }
    catch (std::system_error &)
    {
      cout << "NOT SUPPORTED" << endl;
    }
    catch (Interface::send_error & e)
    {
      cout << "TLS send error:" << e.what() << endl;
    }
    
    cout << "setClearScreen()... ";
    protocol.setClearScreen();
    cout << "ok!" << std::endl;

    cout << "setInkingMode(On)... ";
    protocol.setInkingMode(Protocol::InkingMode_On);
    cout << "ok!" << std::endl;
    
    cout << "(use stylus, press CTRL-C to quit)" << std::endl << std::endl;

    {
      char const * header;
      switch (penDataOptionMode)
      {
        default:
        case Protocol::PenDataOptionMode_None             : header = "rdy  sw     x     y  pres"; break;
        case Protocol::PenDataOptionMode_TimeCount        : header = "rdy  sw     x     y  pres  time"; break;
        case Protocol::PenDataOptionMode_SequenceNumber   : header = "rdy  sw     x     y  pres   seq"; break;
        case Protocol::PenDataOptionMode_TimeCountSequence: header = "rdy  sw     x     y  pres  time   seq"; break;
      }
      cout << header << std::endl;
    }

    auto interfaceQueue = protocol->interfaceQueue();

    cout_ReportHandler reportHandler;
   
    bool decodeTLSreports = (dynamic_cast<TlsInterface*>(protocol.operator->()) != nullptr);
    Report report;
    while (interfaceQueue.wait_getReport_predicate(report))
    {
      auto r = reportHandler.handleReport(report.begin(), report.end(), decodeTLSreports);
      if (r.first != report.end())
      {
        if (r.second)
        {
          cout << "unknown data in report: ";
        }
        else
        {
          cout << "pending data in report: ";
        }
        reportHandler.onUnknown(r.first, report.end());
      }
    }

    cout << "quitting!" << std::endl;

    cout << "setInkingMode(Off)... ";
    protocol.setInkingMode(Protocol::InkingMode_Off);
    cout << "ok!" << std::endl;

    cout << "setClearScreen()... ";
    protocol.setClearScreen();
    cout << "ok!" << std::endl;

    if (supportedPenDataOptionMode)
    {
      cout << "setPenDataOptionMode(None)... ";
      protocol.setPenDataOptionMode(Protocol::PenDataOptionMode_None);
      cout << "ok!" << endl;;
    }

    protocol->disconnect();
  }
  catch (Interface::device_removed_error const &)
  {
    std::cout << "device removed exception" << std::endl;
  }
  catch (system_error const & e)
  {
    std::cout << "system error: " << e.what() << " code:" << e.code() << " " << e.code().message() << std::endl;
  }
  catch (std::runtime_error const & e)
  {
    std::cout << "runtime_error exception: " << e.what() << std::endl;
  }
}



std::unique_ptr<WacomGSS::STU::TlsInterface> connectTls()
{
  using namespace std;
  using namespace WacomGSS::STU;

  unique_ptr<TlsInterface> intf;
   
  auto tlsDevices = getTlsDevices();

  if (!tlsDevices.empty()) 
  {
    auto const & tlsDevice = tlsDevices.front();

    cout 
      << "Connecting to first TLS device found: ";
#ifdef WacomGSS_WIN32
    wcout << tlsDevice.deviceName;
#else
    /// TODO: display O/S specific here
#endif
    cout << endl;

    cout << "Connecting... "<<endl;

    intf.reset(new TlsInterface);

    auto ec = intf->connect(tlsDevice, TlsInterface::ConnectOption_SSL);
    if (!ec) 
    {
      // success

      TlsProtocol protocol(*intf);
      protocol.sendProtocolVersion(1);
    } 
    else  
    {
      cout << "Failed to connect: " << ec << endl;
      intf = nullptr;
    }
  }
  else 
  {
    cout << "No TLS devices found" << endl;
  }

  return intf;
}


std::unique_ptr<WacomGSS::STU::UsbInterface> connectUsb()
{
  using namespace std;
  using namespace WacomGSS::STU;

  unique_ptr<UsbInterface> intf;
   
  vector<UsbDevice> usbDevices = getUsbDevices();

  if (!usbDevices.empty()) 
  {
    UsbDevice const & usbDevice = usbDevices.front();

    auto f = cout.fill(L'0');
    cout 
      << "Connecting to first device found: "
      << hex
      << setw(4) << usbDevice.idVendor << ':'
      << setw(4) << usbDevice.idProduct << ':'
      << setw(4) << usbDevice.bcdDevice
      << dec
      ;
    /// TODO: display O/S specific here
    cout << endl;
    cout.fill(f);

    cout << "Connecting... "<<endl;

    intf.reset(new UsbInterface);

    auto ec = intf->connect(usbDevice, true);
    if (!ec) 
    {
      // success
    } 
    else  
    {
      cout << "Failed to connect: " << ec << endl;
      intf = nullptr;
    }
  }
  else 
  {
    cout << "No USB devices found" << endl;
  }

  return intf;
}



#ifdef WacomGSS_STU_SerialInterface_hpp
std::unique_ptr<WacomGSS::STU::SerialInterface> connectSerial(char const * comPort, uint32_t baudRate)
{
#ifndef __MACH__
  using namespace std;
  using namespace WacomGSS::STU;
  try
  {
#if defined(WacomGSS_WIN32)
    wstring_convert<codecvt_utf8_utf16<wchar_t, 0x10ffff, little_endian>,wchar_t> conv;
    auto comPort_s = conv.from_bytes(comPort);
#else
    auto comPort_s = comPort;
#endif
    cout << "connecting to " << comPort << "  " << baudRate << endl;

    unique_ptr<SerialInterface> intf(new SerialInterface);
    error_code ec = intf->connect(comPort_s, baudRate, true);
    if (!ec)
    {
      return intf;
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
  return nullptr;
}
#endif


int main(int, char * * argv)
{
  try
  {
    std::cout << "STU simpleInterface sample" << std::endl << std::endl;

    std::unique_ptr<WacomGSS::STU::Interface> intf;

    if (!*++argv)
    {
      intf = connectTls();
      if (!intf)
        intf = connectUsb();
    }
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
        intf = connectSerial(port, baudRate);
      }
      else
      {
        std::cout << "args: serialPort baudRate"  << std::endl;
        
      }
    }

    if (!!intf) 
    { 
      std::cout << "Connected!" << std::endl;

      g_intf.store(intf.get());
      signal(SIGINT, &signalHandler);
      try
      {
        run(*intf);
      }
      catch (...)
      {
        signal(SIGINT, SIG_DFL);
        g_intf.store(nullptr);
        throw;
      }

      signal(SIGINT, SIG_DFL);
      g_intf.store(nullptr);
    }
  }
  catch (std::exception const & ex)
  {
    std::cout << "std::exception: " << ex.what() << std::endl;
  }

  return 0;
}
