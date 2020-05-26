#include <WacomGSS/wgssSTU.h>
#include <stdio.h>

int main(void)
{
  WacomGSS_UsbDevice * usbDevices;
  size_t count;
  int e = WacomGSS_getUsbDevices(sizeof(WacomGSS_UsbDevice), &count, &usbDevices);
  if (e == 0)
  {
    if (count)
    {
      size_t i;
      for (i = 0; i < count; ++i)
      {
#if defined(WacomGSS_WIN32)
        printf("%04x:%04x:%04x %ws [%ws]\n",
            usbDevices[i].usbDevice.idVendor,
            usbDevices[i].usbDevice.idProduct,
            usbDevices[i].usbDevice.bcdDevice,
            usbDevices[i].fileName,
            usbDevices[i].bulkFileName);
#elif defined(WacomGSS_Linux)
        printf("%04x:%04x:%04x %d %d\n",
            usbDevices[i].usbDevice.idVendor,
            usbDevices[i].usbDevice.idProduct,
            usbDevices[i].usbDevice.bcdDevice,
            usbDevices[i].busNumber,
            usbDevices[i].deviceAddress);
#else
#error
#endif
      }
    }
    else
    {
      printf("no usb devices found\n");
    }

    WacomGSS_free(usbDevices);
  }
  else
  {
    printf("WacomGSS_getUsbDevices() failed, %d\n", e);
  }

  return 0;
}
