#include <WacomGSS/STU/getUsbDevices.hpp>

#include <algorithm>
#include <iostream>
#include <iomanip>

static void write_line(WacomGSS::STU::UsbDevice const & usbDevice)
{
  using namespace std;
  cout 
    << hex << setfill('0') 
    << setw(4) << usbDevice.idVendor  << ':'
    << setw(4) << usbDevice.idProduct << ':'
    << setw(4) << usbDevice.bcdDevice << ' '
  ;

#ifdef WacomGSS_WIN32
  wcout
    << usbDevice.fileName;
 
  if (!usbDevice.bulkFileName.empty()) 
  {
    wcout << endl << L" + bulk driver " << usbDevice.bulkFileName;
  }
#endif

  cout << endl;
}

int main()
{
  try
  {
    using namespace WacomGSS::STU;

    std::vector<UsbDevice> usbDevices = getUsbDevices();

    if (!usbDevices.empty()) 
    {
      std::for_each(usbDevices.begin(), usbDevices.end(), write_line);   
    }
    else 
    {
      std::cout << "none found" << std::endl;
    }
  }
  catch (std::exception const & ex)
  {
    std::cout << "Exception:" << ex.what() << std::endl;
  }
  return 0;
}
