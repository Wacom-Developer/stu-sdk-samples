/// @file      simpleTablet.cpp
/// @copyright Copyright (c) 2012 Wacom Company Limited
/// @author    mholden
/// @date      2012-03-08
/// @brief     simple test harness

/// TODO: Add comments!

#include <WacomGSS/STU/getUsbDevices.hpp>
#include <WacomGSS/STU/UsbInterface.hpp>
#include <WacomGSS/STU/Tablet.hpp>
#include <WacomGSS/STU/ProtocolHelper.hpp>
#include <WacomGSS/STU/ReportHandler.hpp>


#ifndef WacomGSS_NoEncryption
#include <WacomGSS/STU/Tablet_OpenSSL.hpp>
#endif


#include <csignal>
#include <iostream>
#include <iomanip>



#ifdef WacomGSS_WIN32
static const int  k_sigExit = SIGBREAK; // CTRL-BREAK
static const char k_sigExit_s[] = "press CTRL-BREAK to quit";
#else
static const int k_sigExit = SIGTERM; // kill 
static const char k_sigExit_s[] = "use 'kill' to make process exit cleanly";
#endif



// Variables shared across threads
static WacomGSS::atomic<bool>                    g_quitFlag;
static WacomGSS::atomic<WacomGSS::STU::Tablet *> g_tablet;



extern "C" 
{
static void signalHandler(int type) noexcept
{
  if (type == k_sigExit) 
  {
    g_quitFlag.store(true);
  }
  else if (type == SIGINT)
  {
    std::signal(type, signalHandler); // continue to monitor signal

    g_quitFlag.store(false);
  }
  g_tablet.load()->queueSetPredicateAll(true);
  g_tablet.load()->queueNotifyAll(); // notify the interface that the predicate has changed
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
  o
    << static_cast<WacomGSS::STU::Protocol::PenData const &>(data) << " "
    << std::setw(5) << data.timeCount << " "
    << std::setw(5) << data.sequence;
  static uint16_t sequence;
  if (((data.sequence-1u)&0xffff) != sequence)
    o << " *** ";
  sequence =data.sequence;
  return o;
}

struct Option { uint16_t v; Option(uint16_t _):v(_){} };
std::ostream & operator << (std::ostream & o, Option const & v)
{
  return o
    << " [" << std::setw(5) << v.v << "]";
}


class cout_ReportHandler : public WacomGSS::STU::ProtocolHelper::ReportHandler
{
  WacomGSS::STU::Tablet const & m_tablet; // required for pass encryption through

public:
  cout_ReportHandler(WacomGSS::STU::Tablet const & tablet)
  :
    m_tablet(tablet)
  {
  }

  void onReport(WacomGSS::STU::Protocol::PenData & data) override
  {
    std::cout << '\r' << data << std::endl; 
  }

  void onReport(WacomGSS::STU::Protocol::PenDataOption & data) override
  {
    std::cout << data << Option(data.option) << std::endl;
  }

  void onReport(WacomGSS::STU::Protocol::PenDataTimeCountSequence & data) override
  {
    std::cout << data << std::endl;
  }

  void decrypt(uint8_t data[16])
  {
    m_tablet.decrypt(data); // forward to tablet
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
  
  void onReport(WacomGSS::STU::Protocol::PenDataTimeCountSequenceEncrypted & data) override
  {
    std::cout
      << "<" << as_hex(data.sessionId) << "> " << static_cast<WacomGSS::STU::Protocol::PenDataTimeCountSequence const &>(data) << std::endl;
  }

  void onReport(WacomGSS::STU::Protocol::EncryptionStatus &) override
  {
    std::cout << "received: EncryptionStatus (ignoring)" << std::endl;
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


static void run(WacomGSS::STU::Tablet & tablet)
{
  try
  {
    using namespace std;
    using namespace WacomGSS::STU;
    using namespace ProtocolHelper::ostream_operators;

    cout << "getInformation()... ";
    Protocol::Information inf = tablet.getInformation();
    cout << "modelName=" << inf.modelNameNullTerminated << std::endl;

    cout << "setClearScreen()... ";
    tablet.setClearScreen();
    cout << "ok!" << std::endl;

    // Enable best data collection.
    Protocol::PenDataOptionMode penDataOptionMode = Protocol::PenDataOptionMode_None;
    if (tablet.isSupported(Protocol::ReportId_PenDataOptionMode))
    {
      switch (tablet.getProductId())
      {
        case WacomGSS::STU::ProductId_520A:
          penDataOptionMode = Protocol::PenDataOptionMode_TimeCount;
          break;

        case WacomGSS::STU::ProductId_430:
        case WacomGSS::STU::ProductId_530:
        case WacomGSS::STU::ProductId_540:
          penDataOptionMode = Protocol::PenDataOptionMode_TimeCountSequence;
          break;
        
        default:
          cout << "Unknown tablet supporting PenDataOptionMode, setting to None." << endl;
      }
      
      cout << "setPenDataOptionMode(" << penDataOptionMode << ")... ";
      tablet.setPenDataOptionMode(penDataOptionMode);
      cout << "ok!" << std::endl;
    }  
    
    bool useEncryption = tablet.isSupported(Protocol::ReportId_EncryptionStatus) || ProtocolHelper::supportsEncryption(tablet.getDHprime());

    cout << "encryption available: " << (useEncryption ? "yes":"no") << endl;

    if (useEncryption)
    {
#if defined(WacomGSS_STU_Tablet_OpenSSL_hpp)
      cout << "startCapture()... ";
      tablet.startCapture(0xc0ffee);
      cout << "ok!" << endl;
#else
      cout << "(not compiled with OpenSSL support, encryption will not be used)" << endl;
#endif
    }

    cout << "setInkingMode(On)... ";
    tablet.setInkingMode(Protocol::InkingMode_On);
    cout << "ok!" << std::endl;
    
    cout << "(use stylus, press CTRL-C to clear screen, " << k_sigExit_s << ")" << std::endl;
    
    {
      auto interfaceQueue = tablet.interfaceQueue();
   
      cout_ReportHandler reportHandler(tablet);

      Report report;
      while (!g_quitFlag)
      {
        if (interfaceQueue.wait_getReport_predicate(report))
        {
          auto r = reportHandler.handleReport(report.begin(), report.end());
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
        else if (!g_quitFlag)
        {
          // clear
          cout << "setClearScreen()... ";        
          tablet.setClearScreen();        
          cout << "ok!" << std::endl;
          tablet.queueSetPredicateAll(false);
        }
      }
    
    } // end scope of interfaceQueue

    cout << "quitting!" << std::endl;

    if (useEncryption)
    {
      cout << "endCapture()... ";
      tablet.endCapture();
      cout << "ok!" << std::endl;
    }

    cout << "setInkingMode(Off)... ";
    tablet.setInkingMode(Protocol::InkingMode_Off);
    cout << "ok!" << std::endl;

    cout << "setClearScreen()... ";
    tablet.setClearScreen();
    cout << "ok!" << std::endl;

    if (penDataOptionMode != Protocol::PenDataOptionMode_None)
    {
      // Play nice to other applications that are not aware of PenDataOptionMode.
      cout << "setPenDataOptionMode(" << Protocol::PenDataOptionMode_None << ")... ";
      tablet.setPenDataOptionMode(Protocol::PenDataOptionMode_None);
      cout << "ok!" << std::endl;
    }

    tablet.disconnect();
  }
  catch (WacomGSS::STU::Interface::device_removed_error const &)
  {
    std::cout << "device removed exception" << std::endl;
  }
  catch (std::system_error const & e)
  {
    std::cout << "system error: " << e.what() << " code:" << e.code() << " " << e.code().message() << std::endl;
  }
  catch (std::runtime_error const & e)
  {
    std::cout << "runtime_error exception: " << e.what() << std::endl;
  }
}



static std::unique_ptr<WacomGSS::STU::UsbInterface> connectUsb()
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
static std::unique_ptr<WacomGSS::STU::SerialInterface> connectSerial()
{
  using namespace std;
  using namespace WacomGSS::STU;
  
  unique_ptr<SerialInterface> intf(new SerialInterface);

  char const * fn = "COM1";

  cout << "Connecting to '" << fn << "'... ";
  auto ec = intf->connect(fn, true);
  if (!ec) 
  {
    // success
  } 
  else  
  {
    cout << "Failed to connect: " << ec << std::endl;
    intf = nullptr;
  }

  return intf;
}
#endif



int main()
{
  using namespace std;
  
  try
  {
    cout << "STU simple sample" << endl << endl;

    unique_ptr<WacomGSS::STU::Interface> intf;
  
    intf = connectUsb();

#ifdef WacomGSS_STU_SerialInterface_hpp
    if (!intf)  
    {
      intf = connectSerial();      
    }
#endif
    
    if (intf) 
    { 
      cout << "Connected!" << endl;

#if defined(WacomGSS_STU_Tablet_OpenSSL_hpp)
      WacomGSS::STU::Tablet tablet(std::move(intf), std::make_shared<WacomGSS::STU::OpenSSL_EncryptionHandler>(), std::make_shared<WacomGSS::STU::OpenSSL_EncryptionHandler2>() );
#else
      WacomGSS::STU::Tablet tablet(std::move(intf));
#endif

      g_tablet.store(&tablet);
      std::signal(SIGINT, &signalHandler);
      std::signal(k_sigExit, &signalHandler);
      try
      {
        run(tablet);
      }
      catch (...)
      {
        std::signal(SIGINT, SIG_DFL);
        std::signal(k_sigExit, SIG_DFL);
        g_tablet.store(nullptr);
        throw;
      }

      signal(SIGINT, SIG_DFL);
      signal(k_sigExit, SIG_DFL);
      g_tablet.store(nullptr);
    }
    
  }
  catch (std::system_error const & e)
  {
    std::cout << "system error: " << e.what() << " code:" << e.code() << " " << e.code().message() << std::endl;
  }
  catch (std::exception const & ex)
  {
    std::cout << "std::exception: " << ex.what() << std::endl;
  }

  return 0;
}
