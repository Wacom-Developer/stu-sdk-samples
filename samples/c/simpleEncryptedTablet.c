/*==============================================================================
simpleEncryptedTablet.c

2015-11-13  CCL     Created

This sample code will setup and run an STU unit in encrypted mode (if supported).
Decrypted Pen Data will be printed to the console. To view encrypted output
you can simply comment out the decrypt step in the encryption handlers.

Enabling encryption in the STU SDK requires several steps:
    1) Providing implementations for both'EncryptionHandler' (v1/v2) interfaces
        a) The implementation is up to the end user, for this sample I used 
           OpenSSL APIs.
        b) These are accessed via function table Structs much like ReportHandler
        c) v1 expects Diffie Hellman key exchange
        d) v2 expects RSA key exchange
        e) The version used is determined at runtime and is hardware dependant.
    2) WacomGSS_Tablet is instantiated with 'create_3', passing in your 
       EncryptionHandler function tables.
        a) This instantiation will check your EncryptionHandler interfaces so 
           they must be present & working.
    3) New ReportHandlers are implemented for Encrypted PenData
    4) startCapture is called with a unique SessionID 

To build this sample please ensure you have OpenSSL/STU-SDK libs installed and 
correct paths etc. set in your build enviroment. 

This code was tested with: STU-530 RSA Keys / AES Encryption
                           STU-430 RSA Keys / AES Encryption
                           STU-500 DH  Keys / AES Encryption
==============================================================================*/

#include <WacomGSS/wgssSTU.h>
#include <openssl/aes.h>
#include <openssl/dh.h>
#include <openssl/engine.h>

#include <stdio.h>
#include <signal.h>

#define WacomGSS_unused_parameter(X) { (void)(X); }

static volatile int     g_quitFlag;
static WacomGSS_Tablet  g_tablet;

struct tagMyEncryptionHandler1
{
  DH* m_DH;
  AES_KEY m_AESKEY;
};
typedef struct tagMyEncryptionHandler1 MyEncryptionHandler1;
enum tagMyEncryptionHandler1_Return
{
  MyEncryptionHandler1_Return_Success = 0, /* success must be 0, all other values indicate error */
  MyEncryptionHandler1_Return_Error = 1
};

struct tagMyEncryptionHandler2
{
  RSA* m_RSA;
  AES_KEY m_AESKEY;
};
typedef struct tagMyEncryptionHandler2 MyEncryptionHandler2;
enum tagMyEncryptionHandler2_Return
{
  MyEncryptionHandler2_Return_Success = 0, /* success must be 0, all other values indicate error */
  MyEncryptionHandler2_Return_Error = 1
};

static void signalHandler(int i)
{
  g_quitFlag = 1;
  /*int e =*/ WacomGSS_Tablet_queueNotifyAll(g_tablet); /* notify the interface that the predicate has changed */

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
    int e2 = WacomGSS_getException(&code, NULL, &message);
    if (!e2)
    {
      printf("error %d: %d %s\n", e, code, message);
      WacomGSS_free(message);
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


/**********************WacomGSS_ReportHandlerFunctionTable********************/

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

static int WacomGSS_DECL onPenDataTimeCountSequence(void * reportHandler, size_t sizeofPenDataTimeCountSequence, WacomGSS_PenDataTimeCountSequence const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenDataTimeCountSequence == sizeof(WacomGSS_PenDataTimeCountSequence))
  {
    printf("%1u %1u %3u %5u %5u %5u %5u\n", penData->rdy, penData->sw, penData->pressure, penData->x, penData->y, penData->timeCount, penData->sequence);
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

static int WacomGSS_DECL onPenDataTimeCountSequenceEncrypted(void * reportHandler, size_t sizeofPenDataTimeCountSequenceEncrypted, WacomGSS_PenDataTimeCountSequenceEncrypted const * penData)
{
  WacomGSS_unused_parameter(reportHandler)
  if (sizeofPenDataTimeCountSequenceEncrypted == sizeof(WacomGSS_PenDataTimeCountSequenceEncrypted))
  {
    printf("<%08x> %1u %1u %3u %5u %5u %5u %5u\n", penData->sessionId, penData->rdy, penData->sw, penData->pressure, penData->x, penData->y, penData->timeCount, penData->sequence);
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

static int WacomGSS_DECL onDecrypt(void* reportHandler, uint8_t data[16])
{
  WacomGSS_unused_parameter(reportHandler);
  int r = WacomGSS_Tablet_decrypt(g_tablet, data);
  return r == WacomGSS_Return_Success ? 0 : 1;
}
/**********************WacomGSS_ReportHandlerFunctionTable********************/

/*****************WacomGSS_TabletEncryptionHandlerFunctionTable***************/

void * MyEncryptionHandler1_new()
{
  MyEncryptionHandler1* p = malloc(sizeof(MyEncryptionHandler1));
  p->m_DH = DH_new();
  memset(&p->m_AESKEY, 0, sizeof(AES_KEY));
  return p;
}

static void WacomGSS_DECL MyEncryptionHandler1_free(void* tabletEncryptionHandler_)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  if (tabletEncryptionHandler->m_DH)
  {
    DH_free(tabletEncryptionHandler->m_DH);
  }
  free(tabletEncryptionHandler);
}

static int WacomGSS_DECL MyEncryptionHandler1_reset(void* tabletEncryptionHandler_)
{
  MyEncryptionHandler1 * p = tabletEncryptionHandler_;
  if (p->m_DH)
  {
    p->m_DH->p = NULL;
    p->m_DH->g = NULL;
    p->m_DH->pub_key = NULL;
    p->m_DH->priv_key = NULL;
  }

  memset(&p->m_AESKEY, 0, sizeof(AES_KEY));

  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_clearKeys(void* tabletEncryptionHandler_)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  BN_clear(tabletEncryptionHandler->m_DH->pub_key);
  BN_clear(tabletEncryptionHandler->m_DH->priv_key);

  memset(&tabletEncryptionHandler->m_AESKEY, 0, sizeof(AES_KEY));

  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_requireDH(void* tabletEncryptionHandler_, WacomGSS_bool * ret)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  *ret = tabletEncryptionHandler->m_DH->g == NULL || tabletEncryptionHandler->m_DH->p == NULL ? WacomGSS_true : WacomGSS_false;

  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_setDH(void* tabletEncryptionHandler_, size_t sizeofDHprime, WacomGSS_DHprime const * dhPrime, size_t sizeofDHbase, WacomGSS_DHbase const * dhBase)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  MyEncryptionHandler1_reset(tabletEncryptionHandler);

  tabletEncryptionHandler->m_DH->p = BN_bin2bn(dhPrime, sizeofDHprime, NULL);
  tabletEncryptionHandler->m_DH->g = BN_bin2bn(dhBase, sizeofDHbase, NULL);

  if (!tabletEncryptionHandler->m_DH->p || !tabletEncryptionHandler->m_DH->g)
  {
    printf("MyEncryptionHandler1_setDH: Failed to set DH\n");
    return MyEncryptionHandler1_Return_Error;
  }

  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_generateHostPublicKey(void * tabletEncryptionHandler_, size_t sizeofHostPublicKey, WacomGSS_PublicKey * hostPublicKey)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;

  if (!DH_generate_key(tabletEncryptionHandler->m_DH) || 
      !tabletEncryptionHandler->m_DH->pub_key || 
      !tabletEncryptionHandler->m_DH->priv_key)
  {
    printf("DH_generate_key Failed\n");
    return MyEncryptionHandler1_Return_Error;
  }

  if (BN_num_bytes(tabletEncryptionHandler->m_DH->pub_key) != sizeofHostPublicKey)
  {
    printf("BN_num_bytes Failed\n");
    return MyEncryptionHandler1_Return_Error;
  }

  if (BN_bn2bin(tabletEncryptionHandler->m_DH->pub_key, hostPublicKey) != sizeofHostPublicKey)
  {
    printf("BN_bn2bin Failed\n");
    return MyEncryptionHandler1_Return_Error;
  }

  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_computeSharedKey(void * tabletEncryptionHandler_, size_t sizeofDevicePublicKey, WacomGSS_PublicKey const * devicePublicKey)
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  BIGNUM * bnDevicePublicKey = BN_bin2bn(devicePublicKey, sizeofDevicePublicKey, NULL); /* convert endian */

  WacomGSS_PublicKey sharedKey;

  size_t r = DH_compute_key(&sharedKey, bnDevicePublicKey, tabletEncryptionHandler->m_DH);

  if (r == SIZE_MAX)
  {
    printf("DH_compute_key error\n");
    return MyEncryptionHandler1_Return_Error;
  }

  if (r != sizeof(sharedKey))
  {
    printf("DH_compute_key() should never return a value larger than DH_size(dh)\n");
    return MyEncryptionHandler1_Return_Error;
  }
    
  AES_set_decrypt_key(&sharedKey, sizeof(sharedKey) * 8, &tabletEncryptionHandler->m_AESKEY);
  BN_free(bnDevicePublicKey);
  return MyEncryptionHandler1_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler1_decrypt(void * tabletEncryptionHandler_, uint8_t data[16])
{
  MyEncryptionHandler1 * tabletEncryptionHandler = tabletEncryptionHandler_;
  AES_decrypt(data, data, &tabletEncryptionHandler->m_AESKEY);
  return MyEncryptionHandler1_Return_Success;
}
/***************WacomGSS_TabletEncryptionHandlerFunctionTable END*************/

/*****************WacomGSS_TabletEncryptionHandlerFunctionTable2**************/

void * MyEncryptionHandler2_new()
{
  MyEncryptionHandler2* tabletEncryptionHandler = malloc(sizeof(MyEncryptionHandler2));
  tabletEncryptionHandler->m_RSA = RSA_new();
  return tabletEncryptionHandler;
}
static void WacomGSS_DECL MyEncryptionHandler2_free(void* tabletEncryptionHandler2)
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  if (tabletEncryptionHandler->m_RSA)
  {
    RSA_free(tabletEncryptionHandler->m_RSA);
  }
  memset(&tabletEncryptionHandler->m_AESKEY, 0, sizeof(AES_KEY));
  free(tabletEncryptionHandler);
}

static void WacomGSS_DECL MyEncryptionHandler2_freeBuffer(void* memory)
{
  free(memory);  
  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_reset(void* tabletEncryptionHandler2)
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  if (tabletEncryptionHandler->m_RSA)
  {
    RSA_free(tabletEncryptionHandler->m_RSA);
    tabletEncryptionHandler->m_RSA = RSA_new();
  }
  memset(&tabletEncryptionHandler->m_AESKEY, 0, sizeof(AES_KEY));
  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_clearKeys(void* tabletEncryptionHandler2)
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  if (tabletEncryptionHandler->m_RSA)
  {
    BN_clear(tabletEncryptionHandler->m_RSA->e);
    BN_clear(tabletEncryptionHandler->m_RSA->n);
  }
  memset(&tabletEncryptionHandler->m_AESKEY, 0, sizeof(AES_KEY));
  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_getParameters(void * tabletEncryptionHandler2, WacomGSS_SymmetricKeyType * symmetricKeyType, WacomGSS_AsymmetricPaddingType * asymmetricPaddingType, WacomGSS_AsymmetricKeyType * asymmetricKeyType)
{
  WacomGSS_unused_parameter(tabletEncryptionHandler2);
  symmetricKeyType = WacomGSS_SymmetricKeyType_AES256;
  asymmetricPaddingType = WacomGSS_AsymmetricPaddingType_OAEP;
  asymmetricKeyType = WacomGSS_AsymmetricKeyType_RSA2048;

  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_getPublicExponent(void * tabletEncryptionHandler2, size_t * length, uint8_t * * publicExponent)
{
  WacomGSS_unused_parameter(tabletEncryptionHandler2);
  BIGNUM* e = BN_new();
        
  if (BN_set_word(e, RSA_F4) == 0)
  {
    printf("BN_set_word(RSA_F4) error \n");
    return MyEncryptionHandler2_Return_Error;
  }

  size_t num_bytes = BN_num_bytes(e);
  uint8_t* public_exponent = malloc(num_bytes); /* This memory is managed by the caller */
    
  *length = num_bytes;

  const unsigned int blockSize = 64; /* See WacomGSS_EncryptionCommand */

  if (num_bytes > blockSize)
  {
    printf("Unexpectedly large size 'e'\n");
    return MyEncryptionHandler2_Return_Error;
  }

  if (BN_bn2bin(e, public_exponent) != num_bytes)
  {
    printf("BN_bn2bin error\n");
    return MyEncryptionHandler2_Return_Error;
  }

  *publicExponent = public_exponent;

  BN_clear_free(e);
  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_generatePublicKey(void * tabletEncryptionHandler2, size_t * length, uint8_t * * publicKey)
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  if (!tabletEncryptionHandler->m_RSA->n)
  {
    BIGNUM* e = BN_new();

    if (BN_set_word(e, RSA_F4) == 0)
    {
      printf("BN_set_word(RSA_F4) error\n");
      return MyEncryptionHandler2_Return_Error;
    }

    int ret = RSA_generate_key_ex(tabletEncryptionHandler->m_RSA, 2048, e, 0);

    if (ret != 1)
    {
      printf("RSA_generate_key_ex error\n");
      return MyEncryptionHandler2_Return_Error;
    }
  }

  int num_bytes = BN_num_bytes(tabletEncryptionHandler->m_RSA->n);
  uint8_t* public_key = malloc(num_bytes); /*This memory is managed by the caller*/
  *length = num_bytes;

  if (BN_bn2bin(tabletEncryptionHandler->m_RSA->n, public_key) != num_bytes)
  {
    printf("BN_bn2bin error\n");
    return MyEncryptionHandler2_Return_Error;
  }

  *publicKey = public_key;

  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_computeSessionKey(void * tabletEncryptionHandler2, size_t length, uint8_t * data)
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  uint8_t* key = malloc(length);

  const size_t keySizeBits = 256;
  const size_t keySizeBytes = keySizeBits / 8;

  size_t r = RSA_private_decrypt(length, data, key, tabletEncryptionHandler->m_RSA, RSA_PKCS1_OAEP_PADDING);
  
  if (r < keySizeBytes || (r == UINT_MAX - 1))
  {
    printf("unexpected size of decrypted key\n");
    free(key);
    return MyEncryptionHandler2_Return_Error;
  }

  int e = AES_set_decrypt_key(key + r - keySizeBytes, keySizeBits, &tabletEncryptionHandler->m_AESKEY);
  if (e != 0)
  {
    printf("AES_set_decrypt_key failed\n");
    free(key);
    return MyEncryptionHandler2_Return_Error;
  }

  free(key);
  return MyEncryptionHandler2_Return_Success;
}

static int WacomGSS_DECL MyEncryptionHandler2_decrypt(void * tabletEncryptionHandler2, uint8_t data[16])
{
  MyEncryptionHandler2* tabletEncryptionHandler = tabletEncryptionHandler2;
  AES_decrypt(data, data, &tabletEncryptionHandler->m_AESKEY);
  return MyEncryptionHandler2_Return_Success;
}
/***************WacomGSS_TabletEncryptionHandlerFunctionTable2 END************/

static void run(WacomGSS_Tablet handle)
{
    int e;
    WacomGSS_InterfaceQueue interfaceQueue = NULL;
    WacomGSS_ReportHandlerFunctionTable reportHandlerFunctionTable = { onPenData,
                                                                       onPenDataOption,
                                                                       onPenDataEncrypted,
                                                                       onPenDataEncryptedOption,
                                                                       NULL,
                                                                       onDecrypt,
                                                                       onPenDataTimeCountSequence,
                                                                       onPenDataTimeCountSequenceEncrypted,
                                                                       NULL };



    WacomGSS_bool penDataOptionModeSupported = WacomGSS_false;
    uint8_t       penDataOptionMode = WacomGSS_PenDataOptionMode_None;

  e = WacomGSS_Tablet_isSupported(handle, WacomGSS_ReportId_PenDataOptionMode, &penDataOptionModeSupported);
  if (penDataOptionModeSupported != WacomGSS_false)
  {
    uint16_t idProduct = 0x0000;
    e = WacomGSS_Tablet_getProductId(handle, &idProduct);
    if (e == 0)
    {
      switch (idProduct)
      {
        case WacomGSS_ProductId_520A:
          penDataOptionMode = WacomGSS_PenDataOptionMode_TimeCount;
          break;
        case WacomGSS_ProductId_430:
        case WacomGSS_ProductId_530:
          penDataOptionMode = WacomGSS_PenDataOptionMode_TimeCountSequence;
          break;
      }

      e = WacomGSS_Tablet_setPenDataOptionMode(handle, penDataOptionMode);
      if (displayError(e)) return;
      printf("penDataOptionMode = %u\n", penDataOptionMode);
    }
  }
  
  printf("setClearScreen()... ");
  e = WacomGSS_Tablet_setClearScreen(handle);
  if (displayError(e)) return;
  printf("Ok!\n");

  WacomGSS_bool useEncryption = WacomGSS_false;

  /* Check if device uses DH Key Exchange */
  WacomGSS_DHprime* dhPrime = 0;
  e = WacomGSS_Tablet_getDHprime(handle, sizeof(WacomGSS_DHprime), &dhPrime);
  if (displayError(e)) return;
  e = WacomGSS_ProtocolHelper_supportsEncryption_DHprime(sizeof(WacomGSS_DHprime), dhPrime, &useEncryption);
  if (displayError(e)) return;
  WacomGSS_free(dhPrime);

  /* If not, check RSA Key Exchange support */
  if (!useEncryption)
  {
    e = WacomGSS_Tablet_isSupported(handle, WacomGSS_ReportId_EncryptionStatus, &useEncryption);
    if (displayError(e)) return;
  }

  if (useEncryption)
  {
      printf("Encryption enabled!\n");
      WacomGSS_Tablet_startCapture(handle, 0xc0ffee);
  }

  e = WacomGSS_Tablet_interfaceQueue(handle, &interfaceQueue);
  if (displayError(e)) return;  

  for (;;)
  {
    WacomGSS_bool ret;
    uint8_t * report;
    size_t    length;

    e = WacomGSS_InterfaceQueue_wait_getReportPredicate(interfaceQueue, NULL, &quitSet, &report, &length, &ret);
    if (displayError(e)) break;

    if (ret)
    {
      uint8_t const * ptr;
      e = WacomGSS_ReportHandler_handleReport(sizeof(reportHandlerFunctionTable), &reportHandlerFunctionTable, NULL, report, length, &ptr, &ret);
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

      WacomGSS_free(report);
    }
    else
    {
        if (useEncryption)
        {
            WacomGSS_Tablet_endCapture(handle);
        }
      // quitSet
      break;
    }
  }

  if (e == 0)
  {
    printf("setClearScreen()... ");
    e = WacomGSS_Tablet_setClearScreen(handle);
    displayError(e);

    if (penDataOptionModeSupported)
    {
      e = WacomGSS_Tablet_setPenDataOptionMode(handle, WacomGSS_PenDataOptionMode_None);
      displayError(e);
    }
  }

  e = WacomGSS_InterfaceQueue_free(interfaceQueue);
  displayError(e);

  e = WacomGSS_Tablet_disconnect(handle);
  displayError(e);
}


static const WacomGSS_TabletEncryptionHandlerFunctionTable myEncryptionHandlerFunctionTable1 = { MyEncryptionHandler1_free,
                                                                                                 MyEncryptionHandler1_reset,
                                                                                                 MyEncryptionHandler1_clearKeys,
                                                                                                 MyEncryptionHandler1_requireDH,
                                                                                                 MyEncryptionHandler1_setDH,
                                                                                                 MyEncryptionHandler1_generateHostPublicKey,
                                                                                                 MyEncryptionHandler1_computeSharedKey,
                                                                                                 MyEncryptionHandler1_decrypt };

static const WacomGSS_TabletEncryptionHandlerFunctionTable2 myEncryptionHandlerFunctionTable2 = { MyEncryptionHandler2_free,
                                                                                                  MyEncryptionHandler2_freeBuffer,
                                                                                                  MyEncryptionHandler2_reset,
                                                                                                  MyEncryptionHandler2_clearKeys,
                                                                                                  MyEncryptionHandler2_getParameters,
                                                                                                  MyEncryptionHandler2_getPublicExponent,
                                                                                                  MyEncryptionHandler2_generatePublicKey,
                                                                                                  MyEncryptionHandler2_computeSessionKey,
                                                                                                  MyEncryptionHandler2_decrypt };


int main(void)
{    
  {
    WacomGSS_UsbDevice * usbDevices;
    size_t count;
    int e = WacomGSS_getUsbDevices(sizeof(WacomGSS_UsbDevice), &count, &usbDevices);
    if (!e)
    {
      if (count)
      { 
        WacomGSS_Interface intf;

        printf("Connecting %04x:%04x:%04x...\n", usbDevices[0].usbDevice.idVendor, usbDevices[0].usbDevice.idProduct, usbDevices[0].usbDevice.bcdDevice);
        e = WacomGSS_UsbInterface_create_1(sizeof(WacomGSS_UsbDevice), &usbDevices[0], WacomGSS_true, &intf);

        if (e == 0)
        {
          WacomGSS_Tablet tablet;

          void* myEncryptionHandler1 = MyEncryptionHandler1_new();
          void* myEncryptionHandler2 = MyEncryptionHandler2_new();

          e = WacomGSS_Tablet_create_3(sizeof(myEncryptionHandlerFunctionTable1),
                                       &myEncryptionHandlerFunctionTable1,
                                       myEncryptionHandler1, 
                                       sizeof(myEncryptionHandlerFunctionTable2),
                                       &myEncryptionHandlerFunctionTable2,
                                       myEncryptionHandler2,
                                       &tablet);
          if (e == 0)
          {
            printf("Connected!\n");

            e = WacomGSS_Tablet_attach(tablet, intf);
            
            if (e == 0)
            {
              intf = NULL;

              g_tablet = tablet;
              signal(SIGINT, &signalHandler);
    
              run(tablet);
    
              signal(SIGINT, SIG_DFL);    
              g_tablet = NULL;
            }
            else
            {
              printf("WacomGSS_Tablet_attach() ");
              displayError(e);
            }
            WacomGSS_Tablet_free(tablet);
          }
          else
          {
            myEncryptionHandlerFunctionTable1.free(myEncryptionHandler1);
            myEncryptionHandlerFunctionTable2.free(myEncryptionHandler2);
            printf("WacomGSS_Tablet_create() ");
            displayError(e);
          }
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


  }
  return 0;
}
