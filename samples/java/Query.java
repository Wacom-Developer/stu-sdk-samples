import com.WacomGSS.STU.*;
import com.WacomGSS.STU.Protocol.*;


public class Query
{
  private static final int K_COLUMN = 42;
  private static final char[] g_forceChar = { '-', '!', '=', '=' };

  static final int k_retries = 25;
  static final long k_sleepBetweenRetries = 10;

  int[] g_reportCountLengths;
  boolean g_force;


  private void fmt(int f, String s)
  {
    System.out.format("%-"+K_COLUMN+"s %c ", s, g_forceChar[f]);
  }

  private void fmt(int f, String s, String s2, Object... args)
  {
    fmt(f,s);
    if (args != null && args.length != 0)
      System.out.format(s2, args);
    else
      System.out.println(s2);
  }

  private void fmt(int f, String s, int value)
  {
    fmt(f, s, Integer.toString(value));
  }



  interface Func
  {
    public void func(int f, String n) throws STUException;
  }

  void func(byte reportId, String n, Func fn)
  {
    boolean supported = g_reportCountLengths[reportId & 0xff] != 0;
    int f = (g_force ? 1 : 0) + (supported?2:0);
    try
    {
      if (g_force || supported)
      {
        fn.func(f, n);
      }
      else
      {
        fmt(f, n, "not supported");
      }
    }
    catch (Throwable e)
    {
      fmt(f,n,"exception (Throwable)!");
    }
  }


  void funcTLS(short packetId, String n, Func fn)
  {
    boolean supported = true;
    int f = (g_force ? 1 : 0) + (supported?2:0);
    try
    {
      fn.func(f, n);
    }
    catch (Throwable e)
    {
      fmt(f,n,"exception (Throwable)!");
    }
  }


  String arrhex(byte[] arr)
  {
    StringBuilder sb = new StringBuilder(arr.length * 2);
    java.util.Formatter formatter = new java.util.Formatter(sb);
    for (byte b : arr)
    {
        formatter.format("%02x", b);
    }
    return sb.toString();
  }



  void romImageHash_common(int f, com.WacomGSS.STU.Protocol.Protocol protocol, byte operationModeType, boolean pushed, byte imageNumber, StringBuilder o) 
  {
    try
    {
      com.WacomGSS.STU.Protocol.Status status = protocol.getStatus();
      if (com.WacomGSS.STU.Protocol.ProtocolHelper.statusCanSend(status.getStatusCode(), com.WacomGSS.STU.Protocol.ReportId.RomImageHash, com.WacomGSS.STU.Protocol.ProtocolHelper.OpDirection_Set))
      {
        protocol.setRomImageHash(operationModeType, pushed, imageNumber);

        com.WacomGSS.STU.Protocol.ProtocolHelper.waitForStatusToSend(protocol, com.WacomGSS.STU.Protocol.ReportId.RomImageHash, com.WacomGSS.STU.Protocol.ProtocolHelper.OpDirection_Get, k_retries, k_sleepBetweenRetries);

        com.WacomGSS.STU.Protocol.RomImageHash value = protocol.getRomImageHash();

        if (value.getResult() == 0)
        {
          fmt(f,o.toString(), arrhex(value.getHash()));
        }
        else
        {
          fmt(f,o.toString(), "not stored");
        }
      }
      else
      {
        fmt(f,o.toString(), "not supported in current statusCode %d%n", status.getStatusCode());
      }
    }
    catch (Throwable t)
    {
      fmt(f,o.toString(), "exception!");
    }
  }


  void romImageHash(int f, com.WacomGSS.STU.Protocol.Protocol protocol, byte operationModeType, boolean pushed, byte imageNumber) 
  {
    StringBuilder o = new StringBuilder();
    o.append("RomImageHash[");
    o.append(operationModeType);
    o.append(",");
    o.append(imageNumber);
    o.append(",");
    o.append(pushed?"pushed":"normal");
    o.append("]");

    romImageHash_common(f, protocol, operationModeType, pushed, imageNumber, o);
  }

  void romImageHash(int f, com.WacomGSS.STU.Protocol.Protocol protocol, byte operationModeType, byte imageNumber) 
  {
    StringBuilder o = new StringBuilder();
    o.append("RomImageHash[");
    o.append(operationModeType);
    o.append(",");
    o.append(imageNumber);
    o.append("]");

    romImageHash_common(f, protocol, operationModeType, false, imageNumber, o);
  }


  private void queryOOB(com.WacomGSS.STU.Protocol.TlsProtocolOOB protocolOOB)
  {
    try
    {
      com.WacomGSS.STU.Protocol.TlsProtocolOOB.Status status = protocolOOB.getStatus();

      final int f = 2;

      fmt(f, "[OOB] Status.oobStatus"         , status.getOobStatus());
      fmt(f, "[OOB] Status.oobExtendedStatus" , status.getOobExtendedStatus());


      com.WacomGSS.STU.Protocol.TlsProtocolOOB.Descriptor descriptor = protocolOOB.getDescriptor();
      fmt(f, "[OOB] Descriptor.descriptorFlags"        , descriptor.getDescriptorFlags());
      fmt(f, "[OOB] Descriptor.idVendor"               , "%04x%n", descriptor.getIdVendor());
      fmt(f, "[OOB] Descriptor.idProduct"              , "%04x%n", descriptor.getIdProduct());
      fmt(f, "[OOB] Descriptor.firmwareRevisionMajor"  , "%04x%n", descriptor.getFirmwareRevisionMajor());
      fmt(f, "[OOB] Descriptor.firmwareRevisionMinor"  , "%04x%n", descriptor.getFirmwareRevisionMinor());
      fmt(f, "[OOB] Descriptor.modelName"              , descriptor.getModelName());

      {
        String n = "[OOB] ReportSizeCollection";

        int[] reportSizeCollection = protocolOOB.getReportSizeCollection();
        fmt(f, n, "");
        for (int i = 0; i < 256; ++i)
        {
          if (reportSizeCollection[i] != 0)
          {
            System.out.format("  %3d - %d %n", i, reportSizeCollection[i]);
          }
        }
      }

    }
    catch (Throwable t)
    {
      System.out.println("Exception!");
    }

    System.out.println("");
  }


  private void queryTLS(com.WacomGSS.STU.InterfaceTLS intf)
  {
    com.WacomGSS.STU.Protocol.TlsProtocol protocol = new com.WacomGSS.STU.Protocol.TlsProtocol(intf);

    funcTLS(com.WacomGSS.STU.Protocol.TlsProtocol.PacketId.ProtocolVersion, "ProtocolVersion", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.TlsProtocol.ReturnValue_ProtocolVersion value = protocol.sendProtocolVersion((short)0);

      fmt(f,"ProtocolVersion.returnValueStatus", value.getReturnValueStatus());
      fmt(f,"ProtocolVersion.activeLevel", "%04x%n", value.getActiveLevel());
      short[] supportedLevels = value.getSupportedLevels();
      for(int i = 0; i < supportedLevels.length; ++i)
      {
        short supportedLevel = supportedLevels[i];
        StringBuilder o = new StringBuilder();
        o.append("ProtocolVersion.supportedLevel[");
        o.append(i);
        o.append("]");

        fmt(f, o.toString(), "%04x%n", supportedLevel);
      }
    }});

    System.out.println("");
  }


  private void display(int[] reportSizeCollection)
  {
    for (int i = 0; i < 256; ++i)
    {
      if (reportSizeCollection[i] != 0)
      {
        System.out.format("  %"+K_COLUMN+"d - %d %n", i, reportSizeCollection[i]);
      }
    }
  }


  private void query(com.WacomGSS.STU.Interface intf)
  {
    com.WacomGSS.STU.Protocol.Protocol protocol = new com.WacomGSS.STU.Protocol.Protocol(intf);

    try
    {
      // 0x01 PenData

      // 0x02 -

      // 0x03
      com.WacomGSS.STU.Protocol.Status status = protocol.getStatus();
      fmt(2, "Status.statusCode"     , status.getStatusCode());
      fmt(2, "Status.lastResultCode" , status.getLastResultCode());
      fmt(2, "Status.statusWord"     , "%02x%n", status.getStatusWord());

      g_reportCountLengths = protocol.getInterface().getReportCountLengths();
      if (g_reportCountLengths == null)
      {
        System.out.println("Warning: unable to get reportCountLengths");
        g_force = true;
      }
      else
      {
        g_force = false;
      }
    }
    catch (Throwable t)
    {
      System.out.println("Exception!");
    }

    // 0x04 - Reset
    // 0x05 -

    // 0x06
    func(com.WacomGSS.STU.Protocol.ReportId.HidInformation, "HidInformation", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.HidInformation hidInformation = protocol.getHidInformation();

      fmt(f,n, "%04x:%04x:%04x%n",hidInformation.getIdVendor(),hidInformation.getIdProduct(),hidInformation.getBcdDevice());
    }});


    // 0x07 -

    // 0x08
    func(com.WacomGSS.STU.Protocol.ReportId.Information, "Information", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.Information information = protocol.getInformation();

      fmt(f, "Information.modelName"            , information.getModelName());
      fmt(f, "Information.firmwareMajorVersion" , information.getFirmwareMajorVersion());
      fmt(f, "Information.firmwareMinorVersion" , information.getFirmwareMinorVersion());
    }});

    // 0x09
    func(com.WacomGSS.STU.Protocol.ReportId.Capability, "Capability", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.Capability caps = protocol.getCapability();
    
      fmt(f, "Capability.tabletMaxX"        , caps.getTabletMaxX()        );
      fmt(f, "Capability.tabletMaxY"        , caps.getTabletMaxY()        );
      fmt(f, "Capability.tabletMaxPressure" , caps.getTabletMaxPressure() );
      fmt(f, "Capability.screenWidth"       , caps.getScreenWidth()       );
      fmt(f, "Capability.screenHeight"      , caps.getScreenHeight()      );
      fmt(f, "Capability.maxReportRate"     , caps.getMaxReportRate()     );
      fmt(f, "Capability.resolution"        , caps.getResolution()        );
      fmt(f, "Capability.encodingFlag"      , caps.getEncodingFlag()      );
    }});


    // 0x0A
    func(com.WacomGSS.STU.Protocol.ReportId.Uid, "Uid", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getUid();
      fmt(f,n, value);
    }});


    // 0x0B
    func(com.WacomGSS.STU.Protocol.ReportId.Uid2, "Uid2", new Func() { public void func(int f, String n) throws STUException 
    {
      String value = protocol.getUid2();
      fmt(f,n, value);
    }});


    // 0x0C
    func(com.WacomGSS.STU.Protocol.ReportId.DefaultMode, "DefaultMode", new Func() { public void func(int f, String n) throws STUException 
    {
      byte value = protocol.getDefaultMode();
      fmt(f,n, value);
    }});


    // 0x0D
    func(com.WacomGSS.STU.Protocol.ReportId.ReportRate, "ReportRate", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getReportRate();
      fmt(f,n, value);
    }});


    // 0x0E
    func(com.WacomGSS.STU.Protocol.ReportId.RenderingMode, "RenderingMode", new Func() { public void func(int f, String n) throws STUException 
    {
      byte value = protocol.getRenderingMode();
      fmt(f,n, value);
    }});


    // 0x0F
    func(com.WacomGSS.STU.Protocol.ReportId.Eserial, "Eserial", new Func() { public void func(int f, String n) throws STUException 
    {
      String value = protocol.getEserial();
      fmt(f,n, value);
    }});

    // 0x10 - PenDataEncrypted
    // 0x11 -
    // 0x12 -

    // 0x13
    func(com.WacomGSS.STU.Protocol.ReportId.HostPublicKey, "HostPublicKey", new Func() { public void func(int f, String n) throws STUException 
    {
      PublicKey value = protocol.getHostPublicKey();
      fmt(f,n, arrhex(value.getValue()));
    }});


    // 0x14
    func(com.WacomGSS.STU.Protocol.ReportId.DevicePublicKey, "DevicePublicKey", new Func() { public void func(int f, String n) throws STUException 
    {
      PublicKey value = protocol.getDevicePublicKey();
      fmt(f,n, arrhex(value.getValue()));
    }});


    // 0x15 - StartCapture
    // 0x16 - EndCapture
    // 0x17 -
    // 0x18 -
    // 0x19 -

    // 0x1A
    func(com.WacomGSS.STU.Protocol.ReportId.DHprime, "DHprime", new Func() { public void func(int f, String n) throws STUException 
    {
      DHprime value = protocol.getDHprime();
      fmt(f,n, arrhex(value.getValue()));
    }});


    // 0x1B
    func(com.WacomGSS.STU.Protocol.ReportId.DHbase, "DHbase", new Func() { public void func(int f, String n) throws STUException 
    {
      DHbase value = protocol.getDHbase();
      fmt(f,n, arrhex(value.getValue()));
    }});

    // 0x1C -
    // 0x1D -
    // 0x1E -
    // 0x1F -
    // 0x20 - ClearScreen

    // 0x21
    func(com.WacomGSS.STU.Protocol.ReportId.InkingMode, "InkingMode", new Func() { public void func(int f, String n) throws STUException 
    {
      byte value = protocol.getInkingMode();
      fmt(f,n, value);
    }});


    // 0x22
    func(com.WacomGSS.STU.Protocol.ReportId.InkThreshold, "InkThreshold", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.InkThreshold value = protocol.getInkThreshold();
      fmt(f,"InkThreshold.onPressureMark" , value.getOnPressureMark() );
      fmt(f,"InkThreshold.offPressureMark", value.getOffPressureMark());
    }});

    // 0x23 - ClearScreenArea
    // 0x24 - StartImageDataArea
    // 0x25 - StartImageData
    // 0x26 - ImageDataBlock
    // 0x27 - EndImageData

    // 0x28
    func(com.WacomGSS.STU.Protocol.ReportId.HandwritingThicknessColor, "HandwritingThicknessColor", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.HandwritingThicknessColor value = protocol.getHandwritingThicknessColor();
      fmt(f,"HandwritingThicknessColor.penColor" , "%04x%n", value.getPenColor());
      fmt(f,"HandwritingThicknessColor.penThickness", value.getPenThickness());
    }});


    // 0x29
    func(com.WacomGSS.STU.Protocol.ReportId.BackgroundColor, "BackgroundColor", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getBackgroundColor();
      fmt(f,n, "%04x%n", value);
    }});


    // 0x2A
    func(com.WacomGSS.STU.Protocol.ReportId.HandwritingDisplayArea, "HandwritingDisplayArea", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.Rectangle value = protocol.getHandwritingDisplayArea();
      fmt(f,"HandwritingDisplayArea.upperLeftXpixel"  , value.getUpperLeftXpixel());
      fmt(f,"HandwritingDisplayArea.upperLeftYpixel"  , value.getUpperLeftYpixel());
      fmt(f,"HandwritingDisplayArea.lowerRightXpixel" , value.getLowerRightXpixel());
      fmt(f,"HandwritingDisplayArea.lowerRightYpixel" , value.getLowerRightYpixel());
    }});


    // 0x2B
    func(com.WacomGSS.STU.Protocol.ReportId.BacklightBrightness, "BacklightBrightness", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getBacklightBrightness();
      fmt(f,n, value);
    }});


    // 0x2C
    func(com.WacomGSS.STU.Protocol.ReportId.ScreenContrast, "ScreenContrast", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getScreenContrast();
      fmt(f,n, "%04x%n", value);
    }});


    // 0x2D
    func(com.WacomGSS.STU.Protocol.ReportId.HandwritingThicknessColor24, "HandwritingThicknessColor24", new Func() { public void func(int f, String n) throws STUException 
    {
      com.WacomGSS.STU.Protocol.HandwritingThicknessColor24 value = protocol.getHandwritingThicknessColor24();
      fmt(f,"HandwritingThicknessColor24.penColor" , "%06x%n", value.getPenColor());
      fmt(f,"HandwritingThicknessColor24.penThickness", value.getPenThickness());
    }});


    // 0x2E
    func(com.WacomGSS.STU.Protocol.ReportId.BackgroundColor, "BackgroundColor24", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getBackgroundColor24();
      fmt(f,n, "%06x%n", value);
    }});


    // 0x2F - BootScreen
    // 0x30 - PenDataOption
    // 0x31 - PenDataEncryptedOption

    // 0x32 
    func(com.WacomGSS.STU.Protocol.ReportId.PenDataOptionMode, "PenDataOptionMode", new Func() { public void func(int f, String n) throws STUException 
    {
      int value = protocol.getPenDataOptionMode();
      fmt(f,n, "%04x%n", value);
    }});

    // 0x33 - PenDataTimeCountSequenceEncrypted
    // 0x34 - PenDataTimeCountSequence
    // 0x35..0x3F
    // 0x40 - EncryptionCommand
    // 0x41..0x4F

    // 0x50
    func(com.WacomGSS.STU.Protocol.ReportId.EncryptionStatus, "EncryptionStatus", new Func() { public void func(int f, String n) throws STUException 
    {
      EncryptionStatus encryptionStatus = protocol.getEncryptionStatus();

      fmt(f,"EncryptionStatus.symmetricKeyType     ", encryptionStatus.getSymmetricKeyType());
      fmt(f,"EncryptionStatus.asymmetricPaddingType", encryptionStatus.getAsymmetricPaddingType());
      fmt(f,"EncryptionStatus.asymmetricKeyType    ", encryptionStatus.getAsymmetricKeyType());
      fmt(f,"EncryptionStatus.statusCodeRSAe       ", encryptionStatus.getStatusCodeRSAe());
      fmt(f,"EncryptionStatus.statusCodeRSAn       ", encryptionStatus.getStatusCodeRSAn());
      fmt(f,"EncryptionStatus.statusCodeRSAc       ", encryptionStatus.getStatusCodeRSAc());
      fmt(f,"EncryptionStatus.lastResultCode       ", encryptionStatus.getLastResultCode());
      fmt(f,"EncryptionStatus.rng                  ", encryptionStatus.getRng() ? "1":"0");
      fmt(f,"EncryptionStatus.sha1                 ", encryptionStatus.getSha1()? "1":"0");
      fmt(f,"EncryptionStatus.aes                  ", encryptionStatus.getAes()? "1":"0");
    }});

    // 0x51..0x8F


    // 0x90 - PinPadData
    // 0x91 - PinPadDataEncrypted
    // 0x92 - PinOperationMode

    // 0x93
    func(com.WacomGSS.STU.Protocol.ReportId.OperationMode, "OperationMode", new Func() { public void func(int f, String n) throws STUException 
    {
      OperationMode value = protocol.getOperationMode();
      fmt(f,"OperationMode.OperationModeType", value.getOperationModeType());
    }});

    // 0x94 - StartROMImageData
    // 0x95 -
  
    // 0x96
    for (byte imageNumber = 1; imageNumber <= 3; ++imageNumber)
    {
      byte i = imageNumber;
      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/PinPad", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.PinPad, false, i);
      }});

      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/PinPad", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.PinPad, true, i);
      }});
    }
    for (byte imageNumber = 1; imageNumber <= 10; ++imageNumber)
    {
      byte i = imageNumber;
      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/SlideShow", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.SlideShow, i);
      }});
    }
    for (byte imageNumber = 1; imageNumber <= 3; ++imageNumber)
    {
      byte i = imageNumber;
      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/KeyPad", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.KeyPad, false, i);
      }});

      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/KeyPad", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.KeyPad, true, i);
      }});
    }
    for (byte imageNumber = 1; imageNumber <= 3; ++imageNumber)
    {
      byte i = imageNumber;
      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/Signature", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.Signature, false, i);
      }});

      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/Signature", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.Signature, true, i);
      }});
    }
    for (byte imageNumber = 1; imageNumber <= 6; ++imageNumber)
    {
      byte i = imageNumber;
      func(com.WacomGSS.STU.Protocol.ReportId.RomImageHash, "RomImageHash/MessageBox", new Func() { public void func(int f, String n) throws STUException 
      {
        romImageHash(f, protocol, com.WacomGSS.STU.Protocol.OperationModeType.MessageBox, i);
      }});
    }

    // 0x97 - DeleteROMImage
    // 0x98 - CurrentMessageImageArea
    // 0x99 - UIEventData
    // 0x9A - UIEventDataEncrypted
    // 0x9B - DisplayROMImage
    // 0x9C..0xFE

    // 0xFF
    func(com.WacomGSS.STU.Protocol.ReportId.ReportSizeCollection, "ReportSizeCollection", new Func() { public void func(int f, String n) throws STUException 
    {
      int[] reportSizeCollection = protocol.getReportSizeCollection();
      fmt(f,n);
      display(reportSizeCollection);
    }});



    System.out.println("");
  }



  private void queryUsb()
  {
    com.WacomGSS.STU.UsbDevice[] usbDevices = UsbDevice.getUsbDevices();

    if (usbDevices != null && usbDevices.length > 0)
    {
      com.WacomGSS.STU.UsbInterface usbInterface = new com.WacomGSS.STU.UsbInterface();

      int e = usbInterface.connect(usbDevices[0], true);
      if (e == 0)
      {
        query(usbInterface);
        usbInterface.disconnect();
      }
      else
      {
        System.out.println("failed to connect to USB device");
      }
    }
    else
    {
      System.out.println("no USB devices found");
    }
  }



  private void queryTls()
  {
  
    com.WacomGSS.STU.TlsDevice[] devices = TlsDevice.getTlsDevices();

    if (devices != null && devices.length > 0)
    {
      com.WacomGSS.STU.TlsInterface tlsInterface = new com.WacomGSS.STU.TlsInterface();

      com.WacomGSS.STU.TlsDevice device = devices[0];  

      if (device instanceof com.WacomGSS.STU.TlsDevice_Win32)
      {
        com.WacomGSS.STU.TlsDevice_Win32 d = (com.WacomGSS.STU.TlsDevice_Win32)device;
        System.out.format("TLS Device: %s%n", d.getDeviceName());
      }
      else if (device instanceof com.WacomGSS.STU.TlsDevice_libusb)
      {
        com.WacomGSS.STU.TlsDevice_libusb d = (com.WacomGSS.STU.TlsDevice_libusb)device;
        System.out.format("TLS Device: %04x%04x%04x [%d:%d]%n", d.getIdVendor(), d.getIdProduct(), d.getBcdDevice(), d.getBusNumber()&0xff, d.getDeviceAddress()&0xff);
      }
      else if (device instanceof com.WacomGSS.STU.TlsDevice_OSX)
      {
        com.WacomGSS.STU.TlsDevice_OSX d = (com.WacomGSS.STU.TlsDevice_OSX)device;
        System.out.format("TLS Device: %04x%04x%04x [%d:%d]%n", d.getIdVendor(), d.getIdProduct(), d.getBcdDevice(), d.getBusNumber()&0xff, d.getDeviceAddress()&0xff);
      }
      else
      {
        System.out.format("TLS Device%n");
      }

      int e = tlsInterface.connect(device, com.WacomGSS.STU.TlsInterface.ConnectOption.SSL);
      if (e == 0)
      {
        try
        {
          com.WacomGSS.STU.Protocol.TlsProtocol protocol = new com.WacomGSS.STU.Protocol.TlsProtocol(tlsInterface);
          protocol.sendProtocolVersion((short)1);
        }
        catch (SendException t)
        {
          System.out.println("SendException returnValueStatus="+t.getReturnValueStatus()+" hint{ packetId="+t.getSendHint().getPacketId()+" reportId="+t.getSendHint().getReportId()+" }");
        }
        catch (Throwable t)
        {
          System.out.println("Exception!" + t.toString());
        }

        queryOOB(new com.WacomGSS.STU.Protocol.TlsProtocolOOB(tlsInterface));
        queryTLS(tlsInterface);
        query(tlsInterface);
        tlsInterface.disconnect();
      }
      else
      {
        System.out.println("failed to connect to TLS device");
      }
    }
    else
    {
      System.out.println("no TLS devices found");
    }
  }


  private void querySerial(String serialPort, int baudRate)
  {
    com.WacomGSS.STU.SerialInterface serialInterface = new com.WacomGSS.STU.SerialInterface();

    int e = serialInterface.connect(serialPort, baudRate, true);
    if (e == 0)
    {
      query(serialInterface);
      serialInterface.disconnect();
    }
    else
    {
      System.out.println("failed to connect to serial device");
    }
  }



  public void run(String[] args)
  {
    if (args == null || args.length == 0)
    {
      queryTls();
      queryUsb();
    }
    else if (args.length == 2)
    {
      String serialPort = args[0];
      int baudRate = 3000000;
      querySerial(serialPort, baudRate);
    }
    else
    {
      System.out.println("query [serialPort baudRate]");
    }
  }

  public static void main(String[] args)
  {
    Query program = new Query();
    program.run(args);
  }
}