/* TODO: Add comments! */

#include <WacomGSS/wgssSTU.h>

#include <stdio.h>
#include <signal.h>

#define WacomGSS_unused_parameter(X) { (void)(X); }

static volatile int       g_quitFlag;
static WacomGSS_Interface g_intf;

static struct tagWacomGSS_FunctionTable_v1 * wgss;

void * load_wgssSTU(void);
void   unload_wgssSTU(void * handle);



static void signalHandler(int i)
{
  g_quitFlag = 1;
  /*int e =*/ wgss->Interface_queueNotifyAll(g_intf); /* notify the interface that the predicate has changed */

  WacomGSS_unused_parameter(i)
}



static WacomGSS_bool WacomGSS_DECL quitSet(void * p)
{
  WacomGSS_unused_parameter(p)
  return g_quitFlag;
}



static int displayError(int e)
{
  if (e)
  {
    char * message;
    int code;
    int e2 = wgss->getException(&code, NULL, &message);
    if (!e2)
    {
      printf("error %d: %d %s\n", e, code, message);
      wgss->free(message);
    }
    else
    {
      printf("error %d - getException() failed %d\n", e, e2);
    }
  }
  return e;
}



static void display(uint8_t const * begin, uint8_t const * end)
{
  while (begin != end)
  {
    printf(" %02x", *begin++);
  }
  printf("\n");
}



static int WacomGSS_DECL onPenData(void * reportHandler, size_t sizeofPenData, WacomGSS_PenData const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenData == sizeof(WacomGSS_PenData))
  {
    printf("%1u %1u %3u %5u %5u\n", penData->rdy, penData->sw, penData->pressure, penData->x, penData->y);
    return 0;
  }
  return 1;
}



static int WacomGSS_DECL onPenDataOption(void * reportHandler, size_t sizeofPenDataOption, WacomGSS_PenDataOption const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenDataOption == sizeof(WacomGSS_PenDataOption))
  {
    printf("%1u %1u %3u %5u %5u [%5u]\n", penData->rdy, penData->sw, penData->pressure, penData->x, penData->y, penData->option);
    return 0;
  }
  return 1;
}



static int WacomGSS_DECL onPenDataEncrypted(void * reportHandler, size_t sizeofPenDataEncrypted, WacomGSS_PenDataEncrypted const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenDataEncrypted == sizeof(WacomGSS_PenDataEncrypted))
  {
    printf("<%08x> %1u %1u %3u %5u %5u\n"
       "           %1u %1u %3u %5u %5u\n",
           penData->sessionId,
           penData->penData[0].rdy, penData->penData[0].sw, penData->penData[0].pressure, penData->penData[0].x, penData->penData[0].y,
           penData->penData[1].rdy, penData->penData[1].sw, penData->penData[1].pressure, penData->penData[1].x, penData->penData[1].y);
    return 0;
  }
  return 1;
}



static int WacomGSS_DECL onPenDataEncryptedOption(void * reportHandler, size_t sizeofPenDataEncryptedOption, WacomGSS_PenDataEncryptedOption const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenDataEncryptedOption == sizeof(WacomGSS_PenDataEncryptedOption))
  {
    printf("<%08x> %1u %1u %3u %5u %5u [%5u]\n"
       "           %1u %1u %3u %5u %5u [%5u]\n",
           penData->sessionId,
           penData->penData[0].rdy, penData->penData[0].sw, penData->penData[0].pressure, penData->penData[0].x, penData->penData[0].y, penData->option[0],
           penData->penData[1].rdy, penData->penData[1].sw, penData->penData[1].pressure, penData->penData[1].x, penData->penData[1].y, penData->option[1]);
    return 0;
  }
  return 1;
}



static void run(WacomGSS_Interface handle)
{
  int e; 
  WacomGSS_InterfaceQueue interfaceQueue = NULL;
  WacomGSS_ReportHandlerFunctionTable reportHandlerFunctionTable = { onPenData, onPenDataOption, onPenDataEncrypted, onPenDataEncryptedOption, NULL, NULL };
  uint8_t penDataOptionMode;

  e = wgss->Protocol_getPenDataOptionMode(handle, &penDataOptionMode);
  if (e == 0)
  {
    printf("penDataOptionMode = %u\n", penDataOptionMode);
  }

  printf("setClearScreen()... ");
  e = wgss->Protocol_setClearScreen(handle);
  if (displayError(e)) return;
  printf("Ok!\n");

  e = wgss->Interface_interfaceQueue(handle, &interfaceQueue);
  if (displayError(e)) return;

  for (;;)
  {
    WacomGSS_bool ret;
    uint8_t * report;
    size_t    length;

    e = wgss->InterfaceQueue_wait_getReportPredicate(interfaceQueue, NULL, &quitSet, &report, &length, &ret);
    if (displayError(e)) break;
    
    if (ret)
    {
      uint8_t const * ptr;
      e = wgss->ReportHandler_handleReport(sizeof(reportHandlerFunctionTable), &reportHandlerFunctionTable, NULL, report, length, &ptr, &ret);
      if (displayError(e) == 0)
      {
        if (ptr != report+length)
        {
          if (ret)
          {
            printf("unknown data in report:");
          }
          else
          {
            printf("pending data in report:");
          }
          display(ptr, report+length);
        }
      }

      wgss->free(report);
    }
    else
    {
      // quitSet
      break;
    }
  }

  if (e == 0)
  {
    printf("setClearScreen()... ");
    e = wgss->Protocol_setClearScreen(handle);
    displayError(e);
  }

  e = wgss->InterfaceQueue_free(interfaceQueue);
  displayError(e);

  e = wgss->Interface_disconnect(handle);
  displayError(e);
}



int main(void)
{
  void * handle = load_wgssSTU();
  if (handle)
  {
    WacomGSS_UsbDevice * usbDevices;
    size_t count;
    int e = wgss->getUsbDevices(sizeof(WacomGSS_UsbDevice), &count, &usbDevices);
    if (!e)
    {
      if (count)
      { 
        WacomGSS_Interface intf;

        printf("Connecting %04x:%04x:%04x...\n", usbDevices[0].usbDevice.idVendor, usbDevices[0].usbDevice.idProduct, usbDevices[0].usbDevice.bcdDevice);
        e = wgss->UsbInterface_create_1(sizeof(WacomGSS_UsbDevice), &usbDevices[0], WacomGSS_true, &intf);

        if (e == 0)
        {
          printf("Connected!\n");

          g_intf = intf;
          signal(SIGINT, &signalHandler);
  
          run(intf);

          signal(SIGINT, SIG_DFL);
          g_intf = NULL;

          wgss->Interface_free(intf);
        }
        else
        {
          printf("WacomGSS_UsbInterface_create() ");
          displayError(e);
        }
      }
      else
      {
        printf("no devices found!\n");
      }
    }
    else
    {
      printf("WacomGSS_getUsbDevices() ");
      displayError(e);
    }

    unload_wgssSTU(handle);
  }

  return 0;
}






#if defined(WacomGSS_WIN32)
#pragma warning(push,3)
#include <windows.h>
#pragma warning(pop)

void * load_wgssSTU(void)
{
  HINSTANCE hWgssSTU = LoadLibraryW(L"wgssSTU.DLL");
  if (hWgssSTU)
  {
    WacomGSS_getFunctionTable_fn getFunctionTable = (WacomGSS_getFunctionTable_fn) GetProcAddress(hWgssSTU, "WacomGSS_getFunctionTable");
    if (getFunctionTable)
    {
      int e = getFunctionTable(sizeof(struct tagWacomGSS_FunctionTable_v1), (WacomGSS_FunctionTable**)&wgss);
      if (!e)
      {
        return hWgssSTU;
      }
      else
      {
        printf("getFunctionTable() failed, e=%d\n", e);
      }
    }
    else
    {
      printf("entry point not found\n");
    }

    unload_wgssSTU(hWgssSTU);
  }
  else
  {
    printf("LoadLibrary() failed\n");
  }
  return NULL;
}

void unload_wgssSTU(void * handle)
{
  if (handle)
  {
    if (wgss)
    {
      wgss->free(wgss);
      wgss = NULL;
    }
    FreeLibrary(handle);
  }
}
#elif defined(WacomGSS_Linux)
#include <dlfcn.h>
void * load_wgssSTU(void)
{
  void * lib = dlopen("/usr/local/lib/libwgssSTU.so", RTLD_LAZY);
  if (lib)
  {
    WacomGSS_getFunctionTable_fn fn = dlsym(lib, "WacomGSS_getFunctionTable");
    char * err;
    if ((err = dlerror()) == NULL)
    {
      if (fn)
      {
        int e = fn(sizeof(struct tagWacomGSS_FunctionTable_v1), &wgss);
        if (!e)
        {
          return lib;
        }
        else
        {
          printf("getFunctionTable() failed, e=%d\n", e);
        }
      }
      else
      {
        printf("fn is NULL!\n");
      }
    }
    else
    {
      printf("dlsym error: %s\n", err);
    }
    dlclose(lib);
  }
  else
;;
  }
  else
  {
    printf("dlopen() failed\n");
  }
  return NULL;
}
void unload_wgssSTU(void * lib)
{
  if (lib)
  {
    if (wgss)
    {
      wgss->free(wgss);
      wgss = NULL;
    }
    dlclose(lib);
  }
}
#endif
