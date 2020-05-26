unit wgssSTU_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// $Rev: 52393 $
// File generated on 05/03/2018 9:29:41 from Type Library described below.

// ************************************************************************  //
// Type Lib: C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll (1)
// LIBID: {20007867-2524-410F-A904-CF6A6A976D89}
// LCID: 0
// Helpfile: 
// HelpString: Wacom STU SDK 2.4 Type Library
// DepndLst: 
//   (1) v2.0 stdole, (C:\Windows\SysWOW64\stdole2.tlb)
// SYS_KIND: SYS_WIN32
// Errors:
//   Hint: Member 'Interface' of 'IProtocol' changed to 'Interface_'
//   Hint: Member 'set' of 'IInterface' changed to 'set_'
//   Hint: Member 'Interface' of 'IProtocol2' changed to 'Interface__'
//   Hint: Member 'getCapability' of 'IProtocol2' changed to 'getCapability_'
//   Hint: Member 'setEndImageData' of 'IProtocol2' changed to 'setEndImageData_'
//   Hint: Member 'Protocol' of 'IInterface2' changed to 'Protocol_'
//   Hint: Member 'Interface' of 'IProtocol3' changed to 'Interface___'
//   Hint: Member 'interfaceQueue' of 'IInterface3' changed to 'interfaceQueue_'
//   Hint: Member 'Protocol' of 'IInterface3' changed to 'Protocol__'
//   Hint: Member 'Interface' of 'IProtocol4' changed to 'Interface____'
//   Hint: Member 'Protocol' of 'IInterface4' changed to 'Protocol___'
//   Hint: Member 'flatten' of 'IProtocolHelper2' changed to 'flatten_'
//   Hint: Member 'resizeAndFlatten' of 'IProtocolHelper2' changed to 'resizeAndFlatten_'
//   Hint: Member 'getNextInkState' of 'IProtocolHelper4' changed to 'getNextInkState_'
//   Hint: Member 'writeImage' of 'IProtocolHelper5' changed to 'writeImage_'
//   Hint: Member 'writeImageArea' of 'IProtocolHelper5' changed to 'writeImageArea_'
//   Hint: Member 'nextState' of 'IInkingState2' changed to 'nextState_'
//   Hint: Member 'Protocol' of 'ITablet2' changed to 'Protocol_'
//   Hint: Member 'getCapability' of 'ITablet2' changed to 'getCapability_'
//   Hint: Member 'Protocol' of 'ITablet3' changed to 'Protocol__'
//   Hint: Member 'serialConnect' of 'ITablet4' changed to 'serialConnect_'
//   Hint: Member 'Protocol' of 'ITablet4' changed to 'Protocol___'
//   Error creating palette bitmap of (TUsbInterface) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TSerialInterface) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TUsbDevices) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TJScript) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TProtocolHelper) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TReportHandler) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TTablet) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TComponent) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TencryptionCommand) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TRectangle) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TinkThreshold) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (ThandwritingThicknessColor) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (THandwritingThicknessColor24) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
//   Error creating palette bitmap of (TInkingState) : Server C:\Program Files (x86)\Wacom STU SDK\COM\bin\Win32\wgssSTU.dll contains no icons
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
{$WARN SYMBOL_PLATFORM OFF}
{$WRITEABLECONST ON}
{$VARPROPSETTER ON}
{$ALIGN 4}

interface

uses Winapi.Windows, System.Classes, System.Variants, System.Win.StdVCL, Vcl.Graphics, Vcl.OleServer, Winapi.ActiveX;
  

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  wgssSTUMajorVersion = 2;
  wgssSTUMinorVersion = 4;

  LIBID_wgssSTU: TGUID = '{20007867-2524-410F-A904-CF6A6A976D89}';

  IID_IProtocol: TGUID = '{20003973-823A-4431-A479-3AA86D58A633}';
  IID_IInterface: TGUID = '{200016D1-CBE2-4625-9F64-9685A2C09838}';
  IID_IInterfaceQueue: TGUID = '{20001FFA-3449-4592-8772-C7C00D46499B}';
  IID_IPredicate: TGUID = '{20008863-5B2A-40BE-8E69-4D2F20303732}';
  IID_IStatus: TGUID = '{2000F706-6451-4F84-BA53-EA90D9BD52DB}';
  IID_IInformation: TGUID = '{200021D5-E224-4C70-A8CB-A62E90183DA9}';
  IID_ICapability: TGUID = '{200028BD-1A2E-4C77-BEBB-EBEF8989EFEA}';
  IID_IInkThreshold: TGUID = '{20003425-E218-45D6-B3E2-961C74364617}';
  IID_IHandwritingThicknessColor: TGUID = '{20004323-9CF7-4375-B7C3-DEFDF8588FAD}';
  IID_IRectangle: TGUID = '{2000518C-05F0-4D88-B867-C5C9D58C04C5}';
  IID_IProtocol2: TGUID = '{2000C3D3-538B-485D-ABD8-7D6C89FC0039}';
  IID_IInterface2: TGUID = '{2000D611-504A-4908-B3F4-F740A0348CEA}';
  IID_ICapability2: TGUID = '{2000DFE5-698E-43D2-B9F2-457127B7243A}';
  IID_IHandwritingThicknessColor24: TGUID = '{2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}';
  IID_IEncryptionStatus: TGUID = '{2000F49B-ABAE-4DBA-81D7-829E3E57C748}';
  IID_IEncryptionCommand: TGUID = '{2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}';
  IID_IProtocol3: TGUID = '{200036DA-950F-4B0E-9B0B-D4108B546A15}';
  IID_IInterface3: TGUID = '{20006055-7BEB-4179-A160-303B81FFA7A5}';
  IID_IInterfaceQueue2: TGUID = '{200029E5-EB8C-48F7-9195-FB7A3F30D89A}';
  IID_IProtocol4: TGUID = '{20005D24-F631-4B2F-9BEA-47022CA72BD5}';
  IID_IInterface4: TGUID = '{200016AC-AF72-452E-9B8A-CCE043C68249}';
  IID_IHidInformation: TGUID = '{200021B3-3E32-4FB4-8D85-B8D9E23FCB32}';
  IID_IUsbDevice: TGUID = '{20006935-F639-4B1E-845F-5B6A6E590E7F}';
  IID_IUsbDevice2: TGUID = '{2000602F-B95F-4C9F-9DC1-517CDB247CD0}';
  IID_IUsbDevices: TGUID = '{20008CC2-6576-4665-8411-99291CF18387}';
  IID__IInterfaceInternal: TGUID = '{20000000-CBF2-4E35-9F92-858A76D8D2B3}';
  IID_IErrorCode: TGUID = '{20001A79-3839-4E67-9855-075A7312AA0D}';
  IID_IUsbInterface: TGUID = '{2000C1D7-8270-490B-A8F3-766258E6BAB9}';
  IID_IUsbInterface2: TGUID = '{2000D919-BE93-453F-B0F2-DEB98AA09B7C}';
  IID_IUsbInterface3: TGUID = '{200024FB-F324-47CF-8909-4CB7129D88EB}';
  IID_IUsbInterface4: TGUID = '{2000249E-4DC7-40EA-BE44-A963011493EC}';
  IID_ISerialInterface: TGUID = '{20003D0C-A047-4487-8FE0-0CF9AB05E48C}';
  IID_ISerialInterface2: TGUID = '{2000EE39-B43B-4A2E-AA71-82D44C6C1A44}';
  IID_ISerialInterface3: TGUID = '{20003335-233C-4842-9C8A-75C7191B34A3}';
  IID_ISerialInterface4: TGUID = '{2000C576-80D3-4C78-A3FC-80C203080A6B}';
  IID_IPenData: TGUID = '{20003BCE-8DD9-43AD-819B-F746171CA1EC}';
  IID_IPenDataOption: TGUID = '{200041E8-CBEC-4414-A64D-B6600A4D3FF6}';
  IID_IPenDataEncrypted: TGUID = '{20007929-8A2A-49DF-8B6D-38B71F8991AB}';
  IID_IPenDataEncryptedOption: TGUID = '{200088A2-DC76-4716-8879-6E6115A1BBE8}';
  IID_IPenDataTimeCountSequence: TGUID = '{20009A29-0C7E-44D5-9A06-960958F032FB}';
  IID_IPenDataTimeCountSequenceEncrypted: TGUID = '{20006841-7756-4F96-AF19-A8DD03BE5C3C}';
  IID_IProtocolHelper: TGUID = '{2000550B-EFC5-4D5F-8969-45C9A67CC93C}';
  IID_IProtocolHelper2: TGUID = '{2000954F-FB11-4414-A4EC-80A92AFC8664}';
  IID_IProtocolHelper3: TGUID = '{200012E8-04DC-44D9-8EEE-872792C85A53}';
  IID_IProtocolHelper4: TGUID = '{20002A23-ACB3-47B8-A7A5-4A4F0C791D1A}';
  IID_IProtocolHelper5: TGUID = '{200014AC-4DBF-40A9-9312-090B8A8EE289}';
  IID_IInkingState: TGUID = '{20001A3F-7165-45C1-BEF9-8196DAB7D039}';
  IID_IInkingState2: TGUID = '{200026D4-2949-411F-BD21-8D01E5DF518C}';
  IID_IReport: TGUID = '{2000122F-B55F-40DE-9284-277D32AEE77F}';
  IID_IDecrypt: TGUID = '{20007FDA-B1C5-44F2-A8F8-AFCD4E6A76C2}';
  IID_IReportHandler: TGUID = '{2000446E-736E-4513-9751-A9B69648863E}';
  IID_ITabletEncryptionHandler: TGUID = '{2000B2ED-AEDA-4E86-8A69-BB987FA3B238}';
  IID_ITabletEncryptionHandler2: TGUID = '{2000AEB7-19C4-42C3-B01E-CE669482FDF4}';
  IID_ITablet: TGUID = '{20005C27-7729-492F-8106-540F2E04DC66}';
  IID_ITablet2: TGUID = '{2000574B-751B-4D52-B59A-2336DB19C5C6}';
  IID_ITablet3: TGUID = '{20001D15-E374-4012-A0C2-C8895B5CFB75}';
  IID_ITablet4: TGUID = '{20001376-687D-4684-AA8B-ADB60A929216}';
  IID_IJScript: TGUID = '{200072FE-6F48-44D0-8C0A-4ABAE07EDFF6}';
  IID_IJScript2: TGUID = '{2000ED38-870A-49CD-A576-64940283D155}';
  IID_IComponentFile: TGUID = '{20005C8A-5280-4C4D-8565-DED24A3672F5}';
  IID_IComponentFiles: TGUID = '{2000A6E2-D773-4829-9B61-78DAC3CCAD82}';
  IID_IComponent: TGUID = '{20001D33-3D2C-4E4D-AFE8-5E5D446637FD}';
  DIID_IInterfaceEvents: TGUID = '{2000A1EE-8F0B-4027-B84E-A8172950CF0C}';
  DIID_IReportHandlerEvents: TGUID = '{20004D65-E291-4B63-8062-7E99EBD21BDF}';
  DIID_IReportHandlerEvents2: TGUID = '{2000595F-4D16-4988-8D37-9FE46084E26F}';
  IID_ITabletEventsException: TGUID = '{20005380-DA13-492A-A97F-EE5811B49B1C}';
  DIID_ITabletEvents: TGUID = '{2000C72A-E338-477E-B359-D6D00006D29E}';
  DIID_ITabletEvents2: TGUID = '{2000CDC6-621B-453D-89E6-46C2BEB326C6}';
  CLASS_UsbInterface: TGUID = '{2000BC66-5735-45C7-9521-93FAAFE8C3BB}';
  CLASS_SerialInterface: TGUID = '{20001CFF-184E-4E32-9FC4-A617533D342E}';
  CLASS_UsbDevices: TGUID = '{2000D7A5-64F7-4826-B56E-85ACC618E4D6}';
  CLASS_JScript: TGUID = '{2000D408-6AFE-4CBA-BDB1-DA087DA66B05}';
  CLASS_ProtocolHelper: TGUID = '{2000CF89-E182-40AD-A8C2-5694987EA5DF}';
  CLASS_ReportHandler: TGUID = '{20008BA5-8133-45BA-91A5-AB54A1F70E13}';
  CLASS_Tablet: TGUID = '{20002178-1165-4D38-A7F5-B169DE2045C1}';
  CLASS_Component: TGUID = '{20000000-5D96-46B0-895F-B0CC7295E44D}';
  CLASS_encryptionCommand: TGUID = '{2000B45D-344F-4FA7-8FDD-6C957FC499F2}';
  CLASS_Rectangle: TGUID = '{200077CB-B6D7-4C0E-97EA-EBBB35051EF4}';
  CLASS_inkThreshold: TGUID = '{20002981-FEBA-4438-BE91-043FFA45CC94}';
  CLASS_handwritingThicknessColor: TGUID = '{20003105-1669-49A9-8DE9-80E1792D6D6A}';
  CLASS_HandwritingThicknessColor24: TGUID = '{20006BFE-4AF1-4F88-8AE0-D87CBBA30CD0}';
  CLASS_InkingState: TGUID = '{2000A253-30B8-4B7D-87CF-623FBF39B0FE}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum ReportId
type
  ReportId = TOleEnum;
const
  ReportId_PenData = $00000001;
  ReportId_Status = $00000003;
  ReportId_Reset = $00000004;
  ReportId_HidInformation = $00000006;
  ReportId_Information = $00000008;
  ReportId_Capability = $00000009;
  ReportId_Uid = $0000000A;
  ReportId_Uid2 = $0000000B;
  ReportId_DefaultMode = $0000000C;
  ReportId_ReportRate = $0000000D;
  ReportId_PenDataEncrypted = $00000010;
  ReportId_HostPublicKey = $00000013;
  ReportId_DevicePublicKey = $00000014;
  ReportId_StartCapture = $00000015;
  ReportId_EndCapture = $00000016;
  ReportId_DHprime = $0000001A;
  ReportId_DHbase = $0000001B;
  ReportId_ClearScreen = $00000020;
  ReportId_InkingMode = $00000021;
  ReportId_InkThreshold = $00000022;
  ReportId_ClearScreenArea = $00000023;
  ReportId_StartImageDataArea = $00000024;
  ReportId_StartImageData = $00000025;
  ReportId_ImageDataBlock = $00000026;
  ReportId_EndImageData = $00000027;
  ReportId_HandwritingThicknessColor = $00000028;
  ReportId_BackgroundColor = $00000029;
  ReportId_HandwritingDisplayArea = $0000002A;
  ReportId_BacklightBrightness = $0000002B;
  ReportId_ScreenContrast = $0000002C;
  ReportId_HandwritingThicknessColor24 = $0000002D;
  ReportId_BackgroundColor24 = $0000002E;
  ReportId_BootScreen = $0000002F;
  ReportId_PenDataOption = $00000030;
  ReportId_PenDataEncryptedOption = $00000031;
  ReportId_PenDataOptionMode = $00000032;
  ReportId_PenDataTimeCountSequenceEncrypted = $00000033;
  ReportId_PenDataTimeCountSequence = $00000034;
  ReportId_EncryptionCommand = $00000040;
  ReportId_EncryptionStatus = $00000050;
  ReportId_GetReport = $00000080;
  ReportId_SetResult = $00000081;
  ReportId_ReportSizeCollection = $000000FF;

// Constants for enum statusCode
type
  statusCode = TOleEnum;
const
  StatusCode_Ready = $00000000;
  StatusCode_Image = $00000001;
  StatusCode_Capture = $00000002;
  StatusCode_Calculation = $00000003;
  StatusCode_Image_Boot = $00000004;
  StatusCode_SystemReset = $000000FF;

// Constants for enum ErrorCode
type
  ErrorCode = TOleEnum;
const
  ErrorCode_None = $00000000;
  ErrorCode_WrongReportId = $00000001;
  ErrorCode_WrongState = $00000002;
  ErrorCode_CRC = $00000003;
  ErrorCode_GraphicsWrongEncodingType = $00000010;
  ErrorCode_GraphicsImageTooLong = $00000011;
  ErrorCode_GraphicsZlibError = $00000012;
  ErrorCode_GraphicsWrongParameters = $00000015;

// Constants for enum ImageDataBlock_
type
  ImageDataBlock_ = TOleEnum;
const
  ImageDataBlock_maxLengthHID = $000000FD;
  ImageDataBlock_maxLengthSerial = $000009FD;

// Constants for enum resetFlag
type
  resetFlag = TOleEnum;
const
  ResetFlag_Software = $00000000;
  ResetFlag_Hardware = $00000001;

// Constants for enum defaultMode
type
  defaultMode = TOleEnum;
const
  DefaultMode_HID = $00000001;
  DefaultMode_Serial = $00000002;

// Constants for enum inkingMode
type
  inkingMode = TOleEnum;
const
  InkingMode_Off = $00000000;
  InkingMode_On = $00000001;

// Constants for enum encodingFlag
type
  encodingFlag = TOleEnum;
const
  EncodingFlag_Zlib = $00000001;
  EncodingFlag_1bit = $00000002;
  EncodingFlag_16bit = $00000004;
  EncodingFlag_24bit = $00000008;

// Constants for enum encodingMode
type
  encodingMode = TOleEnum;
const
  EncodingMode_1bit = $00000000;
  EncodingMode_1bit_Zlib = $00000001;
  EncodingMode_16bit = $00000002;
  EncodingMode_24bit = $00000004;
  EncodingMode_1bit_Bulk = $00000010;
  EncodingMode_16bit_Bulk = $00000012;
  EncodingMode_24bit_Bulk = $00000014;
  EncodingMode_Raw = $00000000;
  EncodingMode_Zlib = $00000001;
  EncodingMode_Bulk = $00000010;
  EncodingMode_16bit_565 = $00000002;

// Constants for enum endImageDataFlag
type
  endImageDataFlag = TOleEnum;
const
  EndImageDataFlag_Commit = $00000000;
  EndImageDataFlag_Abandon = $00000001;

// Constants for enum StatusCodeRSA
type
  StatusCodeRSA = TOleEnum;
const
  StatusCodeRSA_Ready = $00000000;
  StatusCodeRSA_Calculating = $000000FA;
  StatusCodeRSA_Even = $000000FB;
  StatusCodeRSA_Long = $000000FC;
  StatusCodeRSA_Short = $000000FD;
  StatusCodeRSA_Invalid = $000000FE;
  StatusCodeRSA_NotReady = $000000FF;

// Constants for enum ErrorCodeRSA
type
  ErrorCodeRSA = TOleEnum;
const
  ErrorCodeRSA_None = $00000000;
  ErrorCodeRSA_BadParameter = $00000001;
  ErrorCodeRSA_ParameterTooLong = $00000002;
  ErrorCodeRSA_PublicKeyNotReady = $00000003;
  ErrorCodeRSA_PublicExponentNotReady = $00000004;
  ErrorCodeRSA_SpecifiedKeyInUse = $00000005;
  ErrorCodeRSA_SpecifiedKeyNotInUse = $00000006;
  ErrorCodeRSA_BadCommandCode = $00000007;
  ErrorCodeRSA_CommandPending = $00000008;
  ErrorCodeRSA_SpecifiedKeyExists = $00000009;
  ErrorCodeRSA_SpecifiedKeyNotExist = $0000000A;
  ErrorCodeRSA_NotInitialized = $0000000B;

// Constants for enum symmetricKeyType
type
  symmetricKeyType = TOleEnum;
const
  SymmetricKeyType_AES128 = $00000000;
  SymmetricKeyType_AES192 = $00000001;
  SymmetricKeyType_AES256 = $00000002;

// Constants for enum asymmetricKeyType
type
  asymmetricKeyType = TOleEnum;
const
  AsymmetricKeyType_RSA1024 = $00000000;
  AsymmetricKeyType_RSA1536 = $00000001;
  AsymmetricKeyType_RSA2048 = $00000002;

// Constants for enum asymmetricPaddingType
type
  asymmetricPaddingType = TOleEnum;
const
  AsymmetricPaddingType_None = $00000000;
  AsymmetricPaddingType_PKCS1 = $00000001;
  AsymmetricPaddingType_OAEP = $00000002;

// Constants for enum encryptionCommandNumber
type
  encryptionCommandNumber = TOleEnum;
const
  EncryptionCommandNumber_SetEncryptionType = $00000001;
  EncryptionCommandNumber_SetParameterBlock = $00000002;
  EncryptionCommandNumber_GetStatusBlock = $00000003;
  EncryptionCommandNumber_GetParameterBlock = $00000004;
  EncryptionCommandNumber_GenerateSymmetricKey = $00000005;

// Constants for enum encryptionCommandParameterBlockIndex
type
  encryptionCommandParameterBlockIndex = TOleEnum;
const
  EncryptionCommandParameterBlockIndex_RSAe = $00000000;
  EncryptionCommandParameterBlockIndex_RSAn = $00000001;
  EncryptionCommandParameterBlockIndex_RSAc = $00000002;
  EncryptionCommandParameterBlockIndex_RSAm = $00000003;

// Constants for enum penDataOptionMode
type
  penDataOptionMode = TOleEnum;
const
  PenDataOptionMode_None = $00000000;
  PenDataOptionMode_TimeCount = $00000001;
  PenDataOptionMode_SequenceNumber = $00000002;
  PenDataOptionMode_TimeCountSequence = $00000003;

// Constants for enum BaudRate
type
  BaudRate = TOleEnum;
const
  BaudRate_STU500 = $0001C200;
  BaudRate_STU430 = $0001F400;

// Constants for enum InkState_
type
  InkState_ = TOleEnum;
const
  InkState_isOff = $00000001;
  InkState_isOn = $00000002;
  InkState_isInk = $00000004;
  InkState_isFirst = $00000008;
  InkState_isLast = $00000010;

// Constants for enum InkState
type
  InkState = TOleEnum;
const
  InkState_Up = $00000001;
  InkState_Down = $00000002;
  InkState_Inking = $00000006;
  InkState_First = $0000000E;
  InkState_Last = $00000011;

// Constants for enum Scale
type
  Scale = TOleEnum;
const
  Scale_Stretch = $00000000;
  Scale_Fit = $00000001;
  Scale_Clip = $00000002;

// Constants for enum Clip
type
  Clip = TOleEnum;
const
  Clip_Left = $00000000;
  Clip_Center = $00000001;
  Clip_Right = $00000002;
  Clip_Top = $00000000;
  Clip_Middle = $00000010;
  Clip_Bottom = $00000020;

// Constants for enum OpDirection
type
  OpDirection = TOleEnum;
const
  OpDirection_Get = $00000100;
  OpDirection_Set = $00000200;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IProtocol = interface;
  IProtocolDisp = dispinterface;
  IInterface = interface;
  IInterfaceDisp = dispinterface;
  IInterfaceQueue = interface;
  IInterfaceQueueDisp = dispinterface;
  IPredicate = interface;
  IPredicateDisp = dispinterface;
  IStatus = interface;
  IStatusDisp = dispinterface;
  IInformation = interface;
  IInformationDisp = dispinterface;
  ICapability = interface;
  ICapabilityDisp = dispinterface;
  IInkThreshold = interface;
  IInkThresholdDisp = dispinterface;
  IHandwritingThicknessColor = interface;
  IHandwritingThicknessColorDisp = dispinterface;
  IRectangle = interface;
  IRectangleDisp = dispinterface;
  IProtocol2 = interface;
  IProtocol2Disp = dispinterface;
  IInterface2 = interface;
  IInterface2Disp = dispinterface;
  ICapability2 = interface;
  ICapability2Disp = dispinterface;
  IHandwritingThicknessColor24 = interface;
  IHandwritingThicknessColor24Disp = dispinterface;
  IEncryptionStatus = interface;
  IEncryptionStatusDisp = dispinterface;
  IEncryptionCommand = interface;
  IEncryptionCommandDisp = dispinterface;
  IProtocol3 = interface;
  IProtocol3Disp = dispinterface;
  IInterface3 = interface;
  IInterface3Disp = dispinterface;
  IInterfaceQueue2 = interface;
  IInterfaceQueue2Disp = dispinterface;
  IProtocol4 = interface;
  IProtocol4Disp = dispinterface;
  IInterface4 = interface;
  IInterface4Disp = dispinterface;
  IHidInformation = interface;
  IHidInformationDisp = dispinterface;
  IUsbDevice = interface;
  IUsbDeviceDisp = dispinterface;
  IUsbDevice2 = interface;
  IUsbDevice2Disp = dispinterface;
  IUsbDevices = interface;
  IUsbDevicesDisp = dispinterface;
  _IInterfaceInternal = interface;
  IErrorCode = interface;
  IErrorCodeDisp = dispinterface;
  IUsbInterface = interface;
  IUsbInterfaceDisp = dispinterface;
  IUsbInterface2 = interface;
  IUsbInterface2Disp = dispinterface;
  IUsbInterface3 = interface;
  IUsbInterface3Disp = dispinterface;
  IUsbInterface4 = interface;
  IUsbInterface4Disp = dispinterface;
  ISerialInterface = interface;
  ISerialInterfaceDisp = dispinterface;
  ISerialInterface2 = interface;
  ISerialInterface2Disp = dispinterface;
  ISerialInterface3 = interface;
  ISerialInterface3Disp = dispinterface;
  ISerialInterface4 = interface;
  ISerialInterface4Disp = dispinterface;
  IPenData = interface;
  IPenDataDisp = dispinterface;
  IPenDataOption = interface;
  IPenDataOptionDisp = dispinterface;
  IPenDataEncrypted = interface;
  IPenDataEncryptedDisp = dispinterface;
  IPenDataEncryptedOption = interface;
  IPenDataEncryptedOptionDisp = dispinterface;
  IPenDataTimeCountSequence = interface;
  IPenDataTimeCountSequenceDisp = dispinterface;
  IPenDataTimeCountSequenceEncrypted = interface;
  IPenDataTimeCountSequenceEncryptedDisp = dispinterface;
  IProtocolHelper = interface;
  IProtocolHelperDisp = dispinterface;
  IProtocolHelper2 = interface;
  IProtocolHelper2Disp = dispinterface;
  IProtocolHelper3 = interface;
  IProtocolHelper3Disp = dispinterface;
  IProtocolHelper4 = interface;
  IProtocolHelper4Disp = dispinterface;
  IProtocolHelper5 = interface;
  IProtocolHelper5Disp = dispinterface;
  IInkingState = interface;
  IInkingStateDisp = dispinterface;
  IInkingState2 = interface;
  IInkingState2Disp = dispinterface;
  IReport = interface;
  IReportDisp = dispinterface;
  IDecrypt = interface;
  IDecryptDisp = dispinterface;
  IReportHandler = interface;
  IReportHandlerDisp = dispinterface;
  ITabletEncryptionHandler = interface;
  ITabletEncryptionHandlerDisp = dispinterface;
  ITabletEncryptionHandler2 = interface;
  ITabletEncryptionHandler2Disp = dispinterface;
  ITablet = interface;
  ITabletDisp = dispinterface;
  ITablet2 = interface;
  ITablet2Disp = dispinterface;
  ITablet3 = interface;
  ITablet3Disp = dispinterface;
  ITablet4 = interface;
  ITablet4Disp = dispinterface;
  IJScript = interface;
  IJScriptDisp = dispinterface;
  IJScript2 = interface;
  IJScript2Disp = dispinterface;
  IComponentFile = interface;
  IComponentFileDisp = dispinterface;
  IComponentFiles = interface;
  IComponentFilesDisp = dispinterface;
  IComponent = interface;
  IComponentDisp = dispinterface;
  IInterfaceEvents = dispinterface;
  IReportHandlerEvents = dispinterface;
  IReportHandlerEvents2 = dispinterface;
  ITabletEventsException = interface;
  ITabletEventsExceptionDisp = dispinterface;
  ITabletEvents = dispinterface;
  ITabletEvents2 = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  UsbInterface = IUsbInterface4;
  SerialInterface = ISerialInterface4;
  UsbDevices = IUsbDevices;
  JScript = IJScript2;
  ProtocolHelper = IProtocolHelper5;
  ReportHandler = IReportHandler;
  Tablet = ITablet4;
  Component = IComponent;
  encryptionCommand = IEncryptionCommand;
  Rectangle = IRectangle;
  inkThreshold = IInkThreshold;
  handwritingThicknessColor = IHandwritingThicknessColor;
  HandwritingThicknessColor24 = IHandwritingThicknessColor24;
  InkingState = IInkingState2;


// *********************************************************************//
// Interface: IProtocol
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003973-823A-4431-A479-3AA86D58A633}
// *********************************************************************//
  IProtocol = interface(IDispatch)
    ['{20003973-823A-4431-A479-3AA86D58A633}']
    function Get_Interface_: IInterface; safecall;
    function getStatus: IStatus; safecall;
    procedure setReset(resetFlag: Byte); safecall;
    function getInformation: IInformation; safecall;
    function getCapability: ICapability; safecall;
    function getUid: SYSUINT; safecall;
    procedure setUid(uid: SYSUINT); safecall;
    function getHostPublicKey: PSafeArray; safecall;
    procedure setHostPublicKey(publicKey: PSafeArray); safecall;
    function getDevicePublicKey: PSafeArray; safecall;
    procedure setStartCapture(sessionId: SYSUINT); safecall;
    procedure setEndCapture; safecall;
    function getDHprime: PSafeArray; safecall;
    procedure setDHprime(dhPrime: PSafeArray); safecall;
    function getDHbase: PSafeArray; safecall;
    procedure setDHbase(dhBase: PSafeArray); safecall;
    procedure setClearScreen; safecall;
    function getInkingMode: Byte; safecall;
    procedure setInkingMode(inkingMode: Byte); safecall;
    function getInkThreshold: IInkThreshold; safecall;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); safecall;
    procedure setStartImageData(encodingMode: Byte); safecall;
    procedure setImageDataBlock(imageDataBlock: PSafeArray); safecall;
    procedure setEndImageData; safecall;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; safecall;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); safecall;
    function getBackgroundColor: Word; safecall;
    procedure setBackgroundColor(backgroundColor: Word); safecall;
    function getHandwritingDisplayArea: IRectangle; safecall;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); safecall;
    function getBacklightBrightness: Word; safecall;
    procedure setBacklightBrightness(backlightBrightness: Word); safecall;
    function getPenDataOptionMode: Byte; safecall;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); safecall;
    property Interface_: IInterface read Get_Interface_;
  end;

// *********************************************************************//
// DispIntf:  IProtocolDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003973-823A-4431-A479-3AA86D58A633}
// *********************************************************************//
  IProtocolDisp = dispinterface
    ['{20003973-823A-4431-A479-3AA86D58A633}']
    property Interface_: IInterface readonly dispid 2305;
    function getStatus: IStatus; dispid 2306;
    procedure setReset(resetFlag: Byte); dispid 2307;
    function getInformation: IInformation; dispid 2308;
    function getCapability: ICapability; dispid 2309;
    function getUid: SYSUINT; dispid 2310;
    procedure setUid(uid: SYSUINT); dispid 2311;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2312;
    procedure setHostPublicKey(publicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2313;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2314;
    procedure setStartCapture(sessionId: SYSUINT); dispid 2315;
    procedure setEndCapture; dispid 2316;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2317;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2318;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2319;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2320;
    procedure setClearScreen; dispid 2321;
    function getInkingMode: Byte; dispid 2322;
    procedure setInkingMode(inkingMode: Byte); dispid 2323;
    function getInkThreshold: IInkThreshold; dispid 2324;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 2325;
    procedure setStartImageData(encodingMode: Byte); dispid 2326;
    procedure setImageDataBlock(imageDataBlock: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2327;
    procedure setEndImageData; dispid 2328;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 2329;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 2330;
    function getBackgroundColor: Word; dispid 2331;
    procedure setBackgroundColor(backgroundColor: Word); dispid 2332;
    function getHandwritingDisplayArea: IRectangle; dispid 2333;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 2334;
    function getBacklightBrightness: Word; dispid 2335;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 2336;
    function getPenDataOptionMode: Byte; dispid 2337;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 2338;
  end;

// *********************************************************************//
// Interface: IInterface
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200016D1-CBE2-4625-9F64-9685A2C09838}
// *********************************************************************//
  IInterface = interface(IDispatch)
    ['{200016D1-CBE2-4625-9F64-9685A2C09838}']
    procedure disconnect; safecall;
    function isConnected: WordBool; safecall;
    procedure get(data: PSafeArray); safecall;
    procedure set_(data: PSafeArray); safecall;
    function supportsWrite: WordBool; safecall;
    procedure write(data: PSafeArray); safecall;
    function interfaceQueue: IInterfaceQueue; safecall;
    procedure queueNotifyAll; safecall;
    function getReportCountLengths: PSafeArray; safecall;
    function Get_Protocol: IProtocol; safecall;
    property Protocol: IProtocol read Get_Protocol;
  end;

// *********************************************************************//
// DispIntf:  IInterfaceDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200016D1-CBE2-4625-9F64-9685A2C09838}
// *********************************************************************//
  IInterfaceDisp = dispinterface
    ['{200016D1-CBE2-4625-9F64-9685A2C09838}']
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IInterfaceQueue
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001FFA-3449-4592-8772-C7C00D46499B}
// *********************************************************************//
  IInterfaceQueue = interface(IDispatch)
    ['{20001FFA-3449-4592-8772-C7C00D46499B}']
    procedure clear; safecall;
    function empty: WordBool; safecall;
    procedure notify; safecall;
    procedure notifyAll; safecall;
    function tryGetReport(out report: PSafeArray): WordBool; safecall;
    function waitGetReportPredicate(const predicate: IPredicate): PSafeArray; safecall;
  end;

// *********************************************************************//
// DispIntf:  IInterfaceQueueDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001FFA-3449-4592-8772-C7C00D46499B}
// *********************************************************************//
  IInterfaceQueueDisp = dispinterface
    ['{20001FFA-3449-4592-8772-C7C00D46499B}']
    procedure clear; dispid 513;
    function empty: WordBool; dispid 514;
    procedure notify; dispid 515;
    procedure notifyAll; dispid 516;
    function tryGetReport(out report: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 517;
    function waitGetReportPredicate(const predicate: IPredicate): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 518;
  end;

// *********************************************************************//
// Interface: IPredicate
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20008863-5B2A-40BE-8E69-4D2F20303732}
// *********************************************************************//
  IPredicate = interface(IDispatch)
    ['{20008863-5B2A-40BE-8E69-4D2F20303732}']
    function predicate: WordBool; safecall;
  end;

// *********************************************************************//
// DispIntf:  IPredicateDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20008863-5B2A-40BE-8E69-4D2F20303732}
// *********************************************************************//
  IPredicateDisp = dispinterface
    ['{20008863-5B2A-40BE-8E69-4D2F20303732}']
    function predicate: WordBool; dispid 257;
  end;

// *********************************************************************//
// Interface: IStatus
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000F706-6451-4F84-BA53-EA90D9BD52DB}
// *********************************************************************//
  IStatus = interface(IDispatch)
    ['{2000F706-6451-4F84-BA53-EA90D9BD52DB}']
    function Get_statusCode: Byte; safecall;
    function Get_lastResultCode: Byte; safecall;
    function Get_statusWord: Word; safecall;
    property statusCode: Byte read Get_statusCode;
    property lastResultCode: Byte read Get_lastResultCode;
    property statusWord: Word read Get_statusWord;
  end;

// *********************************************************************//
// DispIntf:  IStatusDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000F706-6451-4F84-BA53-EA90D9BD52DB}
// *********************************************************************//
  IStatusDisp = dispinterface
    ['{2000F706-6451-4F84-BA53-EA90D9BD52DB}']
    property statusCode: Byte readonly dispid 2561;
    property lastResultCode: Byte readonly dispid 2562;
    property statusWord: Word readonly dispid 2563;
  end;

// *********************************************************************//
// Interface: IInformation
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200021D5-E224-4C70-A8CB-A62E90183DA9}
// *********************************************************************//
  IInformation = interface(IDispatch)
    ['{200021D5-E224-4C70-A8CB-A62E90183DA9}']
    function Get_modelName: WideString; safecall;
    function Get_firmwareMajorVersion: Byte; safecall;
    function Get_firmwareMinorVersion: Byte; safecall;
    function Get_secureIc: Byte; safecall;
    function Get_secureIcVersion: SYSUINT; safecall;
    property modelName: WideString read Get_modelName;
    property firmwareMajorVersion: Byte read Get_firmwareMajorVersion;
    property firmwareMinorVersion: Byte read Get_firmwareMinorVersion;
    property secureIc: Byte read Get_secureIc;
    property secureIcVersion: SYSUINT read Get_secureIcVersion;
  end;

// *********************************************************************//
// DispIntf:  IInformationDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200021D5-E224-4C70-A8CB-A62E90183DA9}
// *********************************************************************//
  IInformationDisp = dispinterface
    ['{200021D5-E224-4C70-A8CB-A62E90183DA9}']
    property modelName: WideString readonly dispid 2817;
    property firmwareMajorVersion: Byte readonly dispid 2818;
    property firmwareMinorVersion: Byte readonly dispid 2819;
    property secureIc: Byte readonly dispid 2820;
    property secureIcVersion: SYSUINT readonly dispid 2821;
  end;

// *********************************************************************//
// Interface: ICapability
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200028BD-1A2E-4C77-BEBB-EBEF8989EFEA}
// *********************************************************************//
  ICapability = interface(IDispatch)
    ['{200028BD-1A2E-4C77-BEBB-EBEF8989EFEA}']
    function Get_tabletMaxX: Word; safecall;
    function Get_tabletMaxY: Word; safecall;
    function Get_tabletMaxPressure: Word; safecall;
    function Get_screenWidth: Word; safecall;
    function Get_screenHeight: Word; safecall;
    function Get_maxReportRate: Byte; safecall;
    function Get_resolution: Word; safecall;
    function Get_zlibColorSupport: Byte; safecall;
    property tabletMaxX: Word read Get_tabletMaxX;
    property tabletMaxY: Word read Get_tabletMaxY;
    property tabletMaxPressure: Word read Get_tabletMaxPressure;
    property screenWidth: Word read Get_screenWidth;
    property screenHeight: Word read Get_screenHeight;
    property maxReportRate: Byte read Get_maxReportRate;
    property resolution: Word read Get_resolution;
    property zlibColorSupport: Byte read Get_zlibColorSupport;
  end;

// *********************************************************************//
// DispIntf:  ICapabilityDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200028BD-1A2E-4C77-BEBB-EBEF8989EFEA}
// *********************************************************************//
  ICapabilityDisp = dispinterface
    ['{200028BD-1A2E-4C77-BEBB-EBEF8989EFEA}']
    property tabletMaxX: Word readonly dispid 3073;
    property tabletMaxY: Word readonly dispid 3074;
    property tabletMaxPressure: Word readonly dispid 3075;
    property screenWidth: Word readonly dispid 3076;
    property screenHeight: Word readonly dispid 3077;
    property maxReportRate: Byte readonly dispid 3078;
    property resolution: Word readonly dispid 3079;
    property zlibColorSupport: Byte readonly dispid 3080;
  end;

// *********************************************************************//
// Interface: IInkThreshold
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003425-E218-45D6-B3E2-961C74364617}
// *********************************************************************//
  IInkThreshold = interface(IDispatch)
    ['{20003425-E218-45D6-B3E2-961C74364617}']
    procedure Set_onPressureMark(pRetVal: Word); safecall;
    function Get_onPressureMark: Word; safecall;
    procedure Set_offPressureMark(pRetVal: Word); safecall;
    function Get_offPressureMark: Word; safecall;
    property onPressureMark: Word read Get_onPressureMark write Set_onPressureMark;
    property offPressureMark: Word read Get_offPressureMark write Set_offPressureMark;
  end;

// *********************************************************************//
// DispIntf:  IInkThresholdDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003425-E218-45D6-B3E2-961C74364617}
// *********************************************************************//
  IInkThresholdDisp = dispinterface
    ['{20003425-E218-45D6-B3E2-961C74364617}']
    property onPressureMark: Word dispid 4353;
    property offPressureMark: Word dispid 4354;
  end;

// *********************************************************************//
// Interface: IHandwritingThicknessColor
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20004323-9CF7-4375-B7C3-DEFDF8588FAD}
// *********************************************************************//
  IHandwritingThicknessColor = interface(IDispatch)
    ['{20004323-9CF7-4375-B7C3-DEFDF8588FAD}']
    procedure Set_penColor(pRetVal: Word); safecall;
    function Get_penColor: Word; safecall;
    procedure Set_penThickness(pRetVal: Byte); safecall;
    function Get_penThickness: Byte; safecall;
    property penColor: Word read Get_penColor write Set_penColor;
    property penThickness: Byte read Get_penThickness write Set_penThickness;
  end;

// *********************************************************************//
// DispIntf:  IHandwritingThicknessColorDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20004323-9CF7-4375-B7C3-DEFDF8588FAD}
// *********************************************************************//
  IHandwritingThicknessColorDisp = dispinterface
    ['{20004323-9CF7-4375-B7C3-DEFDF8588FAD}']
    property penColor: Word dispid 4609;
    property penThickness: Byte dispid 4610;
  end;

// *********************************************************************//
// Interface: IRectangle
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000518C-05F0-4D88-B867-C5C9D58C04C5}
// *********************************************************************//
  IRectangle = interface(IDispatch)
    ['{2000518C-05F0-4D88-B867-C5C9D58C04C5}']
    procedure Set_upperLeftXPixel(pRetVal: Word); safecall;
    function Get_upperLeftXPixel: Word; safecall;
    procedure Set_upperLeftYPixel(pRetVal: Word); safecall;
    function Get_upperLeftYPixel: Word; safecall;
    procedure Set_lowerRightXPixel(pRetVal: Word); safecall;
    function Get_lowerRightXPixel: Word; safecall;
    procedure Set_lowerRightYPixel(pRetVal: Word); safecall;
    function Get_lowerRightYPixel: Word; safecall;
    property upperLeftXPixel: Word read Get_upperLeftXPixel write Set_upperLeftXPixel;
    property upperLeftYPixel: Word read Get_upperLeftYPixel write Set_upperLeftYPixel;
    property lowerRightXPixel: Word read Get_lowerRightXPixel write Set_lowerRightXPixel;
    property lowerRightYPixel: Word read Get_lowerRightYPixel write Set_lowerRightYPixel;
  end;

// *********************************************************************//
// DispIntf:  IRectangleDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000518C-05F0-4D88-B867-C5C9D58C04C5}
// *********************************************************************//
  IRectangleDisp = dispinterface
    ['{2000518C-05F0-4D88-B867-C5C9D58C04C5}']
    property upperLeftXPixel: Word dispid 4865;
    property upperLeftYPixel: Word dispid 4866;
    property lowerRightXPixel: Word dispid 4867;
    property lowerRightYPixel: Word dispid 4868;
  end;

// *********************************************************************//
// Interface: IProtocol2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C3D3-538B-485D-ABD8-7D6C89FC0039}
// *********************************************************************//
  IProtocol2 = interface(IProtocol)
    ['{2000C3D3-538B-485D-ABD8-7D6C89FC0039}']
    function Get_Interface__: IInterface2; safecall;
    function getCapability_: ICapability2; safecall;
    function getUid2: WideString; safecall;
    procedure setClearScreenArea(const area: IRectangle); safecall;
    procedure setStartImageDataArea(encodingMode: Byte; const area: IRectangle); safecall;
    procedure setEndImageData_(endImageDataFlag: Byte); safecall;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; safecall;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); safecall;
    function getBackgroundColor24: SYSUINT; safecall;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); safecall;
    function getScreenContrast: Word; safecall;
    procedure setScreenContrast(screenContrast: Word); safecall;
    function getEncryptionStatus: IEncryptionStatus; safecall;
    function getEncryptionCommand(encryptionCommandNumber: Byte): IEncryptionCommand; safecall;
    procedure setEncryptionCommand(const encryptionCommand: IEncryptionCommand); safecall;
    property Interface__: IInterface2 read Get_Interface__;
  end;

// *********************************************************************//
// DispIntf:  IProtocol2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C3D3-538B-485D-ABD8-7D6C89FC0039}
// *********************************************************************//
  IProtocol2Disp = dispinterface
    ['{2000C3D3-538B-485D-ABD8-7D6C89FC0039}']
    property Interface__: IInterface2 readonly dispid 2339;
    function getCapability_: ICapability2; dispid 2340;
    function getUid2: WideString; dispid 2341;
    procedure setClearScreenArea(const area: IRectangle); dispid 2342;
    procedure setStartImageDataArea(encodingMode: Byte; const area: IRectangle); dispid 2343;
    procedure setEndImageData_(endImageDataFlag: Byte); dispid 2344;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 2345;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 2346;
    function getBackgroundColor24: SYSUINT; dispid 2347;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 2348;
    function getScreenContrast: Word; dispid 2349;
    procedure setScreenContrast(screenContrast: Word); dispid 2350;
    function getEncryptionStatus: IEncryptionStatus; dispid 2351;
    function getEncryptionCommand(encryptionCommandNumber: Byte): IEncryptionCommand; dispid 2352;
    procedure setEncryptionCommand(const encryptionCommand: IEncryptionCommand); dispid 2353;
    property Interface_: IInterface readonly dispid 2305;
    function getStatus: IStatus; dispid 2306;
    procedure setReset(resetFlag: Byte); dispid 2307;
    function getInformation: IInformation; dispid 2308;
    function getCapability: ICapability; dispid 2309;
    function getUid: SYSUINT; dispid 2310;
    procedure setUid(uid: SYSUINT); dispid 2311;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2312;
    procedure setHostPublicKey(publicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2313;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2314;
    procedure setStartCapture(sessionId: SYSUINT); dispid 2315;
    procedure setEndCapture; dispid 2316;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2317;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2318;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2319;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2320;
    procedure setClearScreen; dispid 2321;
    function getInkingMode: Byte; dispid 2322;
    procedure setInkingMode(inkingMode: Byte); dispid 2323;
    function getInkThreshold: IInkThreshold; dispid 2324;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 2325;
    procedure setStartImageData(encodingMode: Byte); dispid 2326;
    procedure setImageDataBlock(imageDataBlock: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2327;
    procedure setEndImageData; dispid 2328;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 2329;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 2330;
    function getBackgroundColor: Word; dispid 2331;
    procedure setBackgroundColor(backgroundColor: Word); dispid 2332;
    function getHandwritingDisplayArea: IRectangle; dispid 2333;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 2334;
    function getBacklightBrightness: Word; dispid 2335;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 2336;
    function getPenDataOptionMode: Byte; dispid 2337;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 2338;
  end;

// *********************************************************************//
// Interface: IInterface2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D611-504A-4908-B3F4-F740A0348CEA}
// *********************************************************************//
  IInterface2 = interface(IInterface)
    ['{2000D611-504A-4908-B3F4-F740A0348CEA}']
    function getProductId: Word; safecall;
    function Get_Protocol_: IProtocol2; safecall;
    property Protocol_: IProtocol2 read Get_Protocol_;
  end;

// *********************************************************************//
// DispIntf:  IInterface2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D611-504A-4908-B3F4-F740A0348CEA}
// *********************************************************************//
  IInterface2Disp = dispinterface
    ['{2000D611-504A-4908-B3F4-F740A0348CEA}']
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: ICapability2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000DFE5-698E-43D2-B9F2-457127B7243A}
// *********************************************************************//
  ICapability2 = interface(ICapability)
    ['{2000DFE5-698E-43D2-B9F2-457127B7243A}']
    function Get_encodingFlag: Byte; safecall;
    property encodingFlag: Byte read Get_encodingFlag;
  end;

// *********************************************************************//
// DispIntf:  ICapability2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000DFE5-698E-43D2-B9F2-457127B7243A}
// *********************************************************************//
  ICapability2Disp = dispinterface
    ['{2000DFE5-698E-43D2-B9F2-457127B7243A}']
    property encodingFlag: Byte readonly dispid 3081;
    property tabletMaxX: Word readonly dispid 3073;
    property tabletMaxY: Word readonly dispid 3074;
    property tabletMaxPressure: Word readonly dispid 3075;
    property screenWidth: Word readonly dispid 3076;
    property screenHeight: Word readonly dispid 3077;
    property maxReportRate: Byte readonly dispid 3078;
    property resolution: Word readonly dispid 3079;
    property zlibColorSupport: Byte readonly dispid 3080;
  end;

// *********************************************************************//
// Interface: IHandwritingThicknessColor24
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}
// *********************************************************************//
  IHandwritingThicknessColor24 = interface(IDispatch)
    ['{2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}']
    procedure Set_penColor(pRetVal: SYSUINT); safecall;
    function Get_penColor: SYSUINT; safecall;
    procedure Set_penThickness(pRetVal: Byte); safecall;
    function Get_penThickness: Byte; safecall;
    property penColor: SYSUINT read Get_penColor write Set_penColor;
    property penThickness: Byte read Get_penThickness write Set_penThickness;
  end;

// *********************************************************************//
// DispIntf:  IHandwritingThicknessColor24Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}
// *********************************************************************//
  IHandwritingThicknessColor24Disp = dispinterface
    ['{2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}']
    property penColor: SYSUINT dispid 9217;
    property penThickness: Byte dispid 9218;
  end;

// *********************************************************************//
// Interface: IEncryptionStatus
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000F49B-ABAE-4DBA-81D7-829E3E57C748}
// *********************************************************************//
  IEncryptionStatus = interface(IDispatch)
    ['{2000F49B-ABAE-4DBA-81D7-829E3E57C748}']
    function Get_symmetricKeyType: Byte; safecall;
    function Get_asymmetricPaddingType: Byte; safecall;
    function Get_asymmetricKeyType: Byte; safecall;
    function Get_statusCodeRSAe: Byte; safecall;
    function Get_statusCodeRSAn: Byte; safecall;
    function Get_statusCodeRSAc: Byte; safecall;
    function Get_lastResultCode: Byte; safecall;
    function Get_rng: WordBool; safecall;
    function Get_sha1: WordBool; safecall;
    function Get_aes: WordBool; safecall;
    property symmetricKeyType: Byte read Get_symmetricKeyType;
    property asymmetricPaddingType: Byte read Get_asymmetricPaddingType;
    property asymmetricKeyType: Byte read Get_asymmetricKeyType;
    property statusCodeRSAe: Byte read Get_statusCodeRSAe;
    property statusCodeRSAn: Byte read Get_statusCodeRSAn;
    property statusCodeRSAc: Byte read Get_statusCodeRSAc;
    property lastResultCode: Byte read Get_lastResultCode;
    property rng: WordBool read Get_rng;
    property sha1: WordBool read Get_sha1;
    property aes: WordBool read Get_aes;
  end;

// *********************************************************************//
// DispIntf:  IEncryptionStatusDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000F49B-ABAE-4DBA-81D7-829E3E57C748}
// *********************************************************************//
  IEncryptionStatusDisp = dispinterface
    ['{2000F49B-ABAE-4DBA-81D7-829E3E57C748}']
    property symmetricKeyType: Byte readonly dispid 8705;
    property asymmetricPaddingType: Byte readonly dispid 8706;
    property asymmetricKeyType: Byte readonly dispid 8707;
    property statusCodeRSAe: Byte readonly dispid 8708;
    property statusCodeRSAn: Byte readonly dispid 8709;
    property statusCodeRSAc: Byte readonly dispid 8710;
    property lastResultCode: Byte readonly dispid 8711;
    property rng: WordBool readonly dispid 8712;
    property sha1: WordBool readonly dispid 8713;
    property aes: WordBool readonly dispid 8714;
  end;

// *********************************************************************//
// Interface: IEncryptionCommand
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}
// *********************************************************************//
  IEncryptionCommand = interface(IDispatch)
    ['{2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}']
    procedure Set_encryptionCommandNumber(pRetVal: Byte); safecall;
    function Get_encryptionCommandNumber: Byte; safecall;
    procedure Set_parameter(pRetVal: Byte); safecall;
    function Get_parameter: Byte; safecall;
    procedure Set_lengthOrIndex(pRetVal: Byte); safecall;
    function Get_lengthOrIndex: Byte; safecall;
    procedure Set_data(pRetVal: PSafeArray); safecall;
    function Get_data: PSafeArray; safecall;
    procedure initializeSetEncryptionType(symmetricKeyType: Byte; asymmetricPaddingType: Byte; 
                                          asymmetricKeyType: Byte); safecall;
    procedure initializeSetParameterBlock(encryptionCommandParameterBlockIndex: Byte; 
                                          data: PSafeArray); safecall;
    procedure initializeGenerateSymmetricKey; safecall;
    procedure initializeGetParameterBlock(encryptionCommandParameterBlockIndex: Byte; offset: Byte); safecall;
    property encryptionCommandNumber: Byte read Get_encryptionCommandNumber write Set_encryptionCommandNumber;
    property parameter: Byte read Get_parameter write Set_parameter;
    property lengthOrIndex: Byte read Get_lengthOrIndex write Set_lengthOrIndex;
    property data: PSafeArray read Get_data write Set_data;
  end;

// *********************************************************************//
// DispIntf:  IEncryptionCommandDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}
// *********************************************************************//
  IEncryptionCommandDisp = dispinterface
    ['{2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}']
    property encryptionCommandNumber: Byte dispid 9473;
    property parameter: Byte dispid 9474;
    property lengthOrIndex: Byte dispid 9475;
    property data: {NOT_OLEAUTO(PSafeArray)}OleVariant dispid 9476;
    procedure initializeSetEncryptionType(symmetricKeyType: Byte; asymmetricPaddingType: Byte; 
                                          asymmetricKeyType: Byte); dispid 9477;
    procedure initializeSetParameterBlock(encryptionCommandParameterBlockIndex: Byte; 
                                          data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 9478;
    procedure initializeGenerateSymmetricKey; dispid 9479;
    procedure initializeGetParameterBlock(encryptionCommandParameterBlockIndex: Byte; offset: Byte); dispid 9480;
  end;

// *********************************************************************//
// Interface: IProtocol3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200036DA-950F-4B0E-9B0B-D4108B546A15}
// *********************************************************************//
  IProtocol3 = interface(IProtocol2)
    ['{200036DA-950F-4B0E-9B0B-D4108B546A15}']
    function Get_Interface___: IInterface3; safecall;
    property Interface___: IInterface3 read Get_Interface___;
  end;

// *********************************************************************//
// DispIntf:  IProtocol3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200036DA-950F-4B0E-9B0B-D4108B546A15}
// *********************************************************************//
  IProtocol3Disp = dispinterface
    ['{200036DA-950F-4B0E-9B0B-D4108B546A15}']
    property Interface___: IInterface3 readonly dispid 2354;
    property Interface__: IInterface2 readonly dispid 2339;
    function getCapability_: ICapability2; dispid 2340;
    function getUid2: WideString; dispid 2341;
    procedure setClearScreenArea(const area: IRectangle); dispid 2342;
    procedure setStartImageDataArea(encodingMode: Byte; const area: IRectangle); dispid 2343;
    procedure setEndImageData_(endImageDataFlag: Byte); dispid 2344;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 2345;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 2346;
    function getBackgroundColor24: SYSUINT; dispid 2347;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 2348;
    function getScreenContrast: Word; dispid 2349;
    procedure setScreenContrast(screenContrast: Word); dispid 2350;
    function getEncryptionStatus: IEncryptionStatus; dispid 2351;
    function getEncryptionCommand(encryptionCommandNumber: Byte): IEncryptionCommand; dispid 2352;
    procedure setEncryptionCommand(const encryptionCommand: IEncryptionCommand); dispid 2353;
    property Interface_: IInterface readonly dispid 2305;
    function getStatus: IStatus; dispid 2306;
    procedure setReset(resetFlag: Byte); dispid 2307;
    function getInformation: IInformation; dispid 2308;
    function getCapability: ICapability; dispid 2309;
    function getUid: SYSUINT; dispid 2310;
    procedure setUid(uid: SYSUINT); dispid 2311;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2312;
    procedure setHostPublicKey(publicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2313;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2314;
    procedure setStartCapture(sessionId: SYSUINT); dispid 2315;
    procedure setEndCapture; dispid 2316;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2317;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2318;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2319;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2320;
    procedure setClearScreen; dispid 2321;
    function getInkingMode: Byte; dispid 2322;
    procedure setInkingMode(inkingMode: Byte); dispid 2323;
    function getInkThreshold: IInkThreshold; dispid 2324;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 2325;
    procedure setStartImageData(encodingMode: Byte); dispid 2326;
    procedure setImageDataBlock(imageDataBlock: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2327;
    procedure setEndImageData; dispid 2328;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 2329;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 2330;
    function getBackgroundColor: Word; dispid 2331;
    procedure setBackgroundColor(backgroundColor: Word); dispid 2332;
    function getHandwritingDisplayArea: IRectangle; dispid 2333;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 2334;
    function getBacklightBrightness: Word; dispid 2335;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 2336;
    function getPenDataOptionMode: Byte; dispid 2337;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 2338;
  end;

// *********************************************************************//
// Interface: IInterface3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006055-7BEB-4179-A160-303B81FFA7A5}
// *********************************************************************//
  IInterface3 = interface(IInterface2)
    ['{20006055-7BEB-4179-A160-303B81FFA7A5}']
    procedure queueSetPredicateAll(predicate: WordBool); safecall;
    function interfaceQueue_: IInterfaceQueue2; safecall;
    function Get_Protocol__: IProtocol3; safecall;
    property Protocol__: IProtocol3 read Get_Protocol__;
  end;

// *********************************************************************//
// DispIntf:  IInterface3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006055-7BEB-4179-A160-303B81FFA7A5}
// *********************************************************************//
  IInterface3Disp = dispinterface
    ['{20006055-7BEB-4179-A160-303B81FFA7A5}']
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IInterfaceQueue2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200029E5-EB8C-48F7-9195-FB7A3F30D89A}
// *********************************************************************//
  IInterfaceQueue2 = interface(IDispatch)
    ['{200029E5-EB8C-48F7-9195-FB7A3F30D89A}']
    procedure Set_predicate(pRetVal: WordBool); safecall;
    function Get_predicate: WordBool; safecall;
    procedure clear; safecall;
    function empty: WordBool; safecall;
    procedure notify; safecall;
    procedure notifyAll; safecall;
    function tryGetReport: PSafeArray; safecall;
    function waitGetReportPredicate: PSafeArray; safecall;
    property predicate: WordBool read Get_predicate write Set_predicate;
  end;

// *********************************************************************//
// DispIntf:  IInterfaceQueue2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200029E5-EB8C-48F7-9195-FB7A3F30D89A}
// *********************************************************************//
  IInterfaceQueue2Disp = dispinterface
    ['{200029E5-EB8C-48F7-9195-FB7A3F30D89A}']
    property predicate: WordBool dispid 535;
    procedure clear; dispid 529;
    function empty: WordBool; dispid 530;
    procedure notify; dispid 531;
    procedure notifyAll; dispid 532;
    function tryGetReport: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 533;
    function waitGetReportPredicate: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 534;
  end;

// *********************************************************************//
// Interface: IProtocol4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005D24-F631-4B2F-9BEA-47022CA72BD5}
// *********************************************************************//
  IProtocol4 = interface(IProtocol3)
    ['{20005D24-F631-4B2F-9BEA-47022CA72BD5}']
    function Get_Interface____: IInterface4; safecall;
    function getHidInformation: IHidInformation; safecall;
    function getDefaultMode: Byte; safecall;
    procedure setDefaultMode(defaultMode: Byte); safecall;
    function getReportRate: Byte; safecall;
    procedure setReportRate(defaultMode: Byte); safecall;
    function getReportSizeCollection: PSafeArray; safecall;
    property Interface____: IInterface4 read Get_Interface____;
  end;

// *********************************************************************//
// DispIntf:  IProtocol4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005D24-F631-4B2F-9BEA-47022CA72BD5}
// *********************************************************************//
  IProtocol4Disp = dispinterface
    ['{20005D24-F631-4B2F-9BEA-47022CA72BD5}']
    property Interface____: IInterface4 readonly dispid 2355;
    function getHidInformation: IHidInformation; dispid 2356;
    function getDefaultMode: Byte; dispid 2357;
    procedure setDefaultMode(defaultMode: Byte); dispid 2358;
    function getReportRate: Byte; dispid 2359;
    procedure setReportRate(defaultMode: Byte); dispid 2360;
    function getReportSizeCollection: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2361;
    property Interface___: IInterface3 readonly dispid 2354;
    property Interface__: IInterface2 readonly dispid 2339;
    function getCapability_: ICapability2; dispid 2340;
    function getUid2: WideString; dispid 2341;
    procedure setClearScreenArea(const area: IRectangle); dispid 2342;
    procedure setStartImageDataArea(encodingMode: Byte; const area: IRectangle); dispid 2343;
    procedure setEndImageData_(endImageDataFlag: Byte); dispid 2344;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 2345;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 2346;
    function getBackgroundColor24: SYSUINT; dispid 2347;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 2348;
    function getScreenContrast: Word; dispid 2349;
    procedure setScreenContrast(screenContrast: Word); dispid 2350;
    function getEncryptionStatus: IEncryptionStatus; dispid 2351;
    function getEncryptionCommand(encryptionCommandNumber: Byte): IEncryptionCommand; dispid 2352;
    procedure setEncryptionCommand(const encryptionCommand: IEncryptionCommand); dispid 2353;
    property Interface_: IInterface readonly dispid 2305;
    function getStatus: IStatus; dispid 2306;
    procedure setReset(resetFlag: Byte); dispid 2307;
    function getInformation: IInformation; dispid 2308;
    function getCapability: ICapability; dispid 2309;
    function getUid: SYSUINT; dispid 2310;
    procedure setUid(uid: SYSUINT); dispid 2311;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2312;
    procedure setHostPublicKey(publicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2313;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2314;
    procedure setStartCapture(sessionId: SYSUINT); dispid 2315;
    procedure setEndCapture; dispid 2316;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2317;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2318;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 2319;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2320;
    procedure setClearScreen; dispid 2321;
    function getInkingMode: Byte; dispid 2322;
    procedure setInkingMode(inkingMode: Byte); dispid 2323;
    function getInkThreshold: IInkThreshold; dispid 2324;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 2325;
    procedure setStartImageData(encodingMode: Byte); dispid 2326;
    procedure setImageDataBlock(imageDataBlock: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 2327;
    procedure setEndImageData; dispid 2328;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 2329;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 2330;
    function getBackgroundColor: Word; dispid 2331;
    procedure setBackgroundColor(backgroundColor: Word); dispid 2332;
    function getHandwritingDisplayArea: IRectangle; dispid 2333;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 2334;
    function getBacklightBrightness: Word; dispid 2335;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 2336;
    function getPenDataOptionMode: Byte; dispid 2337;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 2338;
  end;

// *********************************************************************//
// Interface: IInterface4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200016AC-AF72-452E-9B8A-CCE043C68249}
// *********************************************************************//
  IInterface4 = interface(IInterface3)
    ['{200016AC-AF72-452E-9B8A-CCE043C68249}']
    function Get_Protocol___: IProtocol4; safecall;
    property Protocol___: IProtocol4 read Get_Protocol___;
  end;

// *********************************************************************//
// DispIntf:  IInterface4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200016AC-AF72-452E-9B8A-CCE043C68249}
// *********************************************************************//
  IInterface4Disp = dispinterface
    ['{200016AC-AF72-452E-9B8A-CCE043C68249}']
    property Protocol___: IProtocol4 readonly dispid 784;
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IHidInformation
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200021B3-3E32-4FB4-8D85-B8D9E23FCB32}
// *********************************************************************//
  IHidInformation = interface(IDispatch)
    ['{200021B3-3E32-4FB4-8D85-B8D9E23FCB32}']
    function Get_idVendor: Word; safecall;
    function Get_idProduct: Word; safecall;
    function Get_bcdDevice: Word; safecall;
    property idVendor: Word read Get_idVendor;
    property idProduct: Word read Get_idProduct;
    property bcdDevice: Word read Get_bcdDevice;
  end;

// *********************************************************************//
// DispIntf:  IHidInformationDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200021B3-3E32-4FB4-8D85-B8D9E23FCB32}
// *********************************************************************//
  IHidInformationDisp = dispinterface
    ['{200021B3-3E32-4FB4-8D85-B8D9E23FCB32}']
    property idVendor: Word readonly dispid 10497;
    property idProduct: Word readonly dispid 10498;
    property bcdDevice: Word readonly dispid 10499;
  end;

// *********************************************************************//
// Interface: IUsbDevice
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006935-F639-4B1E-845F-5B6A6E590E7F}
// *********************************************************************//
  IUsbDevice = interface(IDispatch)
    ['{20006935-F639-4B1E-845F-5B6A6E590E7F}']
    function Get_idVendor: Word; safecall;
    function Get_idProduct: Word; safecall;
    function Get_bcdDevice: Word; safecall;
    function Get_fileName: WideString; safecall;
    function Get_bulkFileName: WideString; safecall;
    property idVendor: Word read Get_idVendor;
    property idProduct: Word read Get_idProduct;
    property bcdDevice: Word read Get_bcdDevice;
    property fileName: WideString read Get_fileName;
    property bulkFileName: WideString read Get_bulkFileName;
  end;

// *********************************************************************//
// DispIntf:  IUsbDeviceDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006935-F639-4B1E-845F-5B6A6E590E7F}
// *********************************************************************//
  IUsbDeviceDisp = dispinterface
    ['{20006935-F639-4B1E-845F-5B6A6E590E7F}']
    property idVendor: Word readonly dispid 2049;
    property idProduct: Word readonly dispid 2050;
    property bcdDevice: Word readonly dispid 2051;
    property fileName: WideString readonly dispid 2052;
    property bulkFileName: WideString readonly dispid 2053;
  end;

// *********************************************************************//
// Interface: IUsbDevice2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000602F-B95F-4C9F-9DC1-517CDB247CD0}
// *********************************************************************//
  IUsbDevice2 = interface(IUsbDevice)
    ['{2000602F-B95F-4C9F-9DC1-517CDB247CD0}']
    function Get_devInst: SYSUINT; safecall;
    property devInst: SYSUINT read Get_devInst;
  end;

// *********************************************************************//
// DispIntf:  IUsbDevice2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000602F-B95F-4C9F-9DC1-517CDB247CD0}
// *********************************************************************//
  IUsbDevice2Disp = dispinterface
    ['{2000602F-B95F-4C9F-9DC1-517CDB247CD0}']
    property devInst: SYSUINT readonly dispid 2054;
    property idVendor: Word readonly dispid 2049;
    property idProduct: Word readonly dispid 2050;
    property bcdDevice: Word readonly dispid 2051;
    property fileName: WideString readonly dispid 2052;
    property bulkFileName: WideString readonly dispid 2053;
  end;

// *********************************************************************//
// Interface: IUsbDevices
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20008CC2-6576-4665-8411-99291CF18387}
// *********************************************************************//
  IUsbDevices = interface(IDispatch)
    ['{20008CC2-6576-4665-8411-99291CF18387}']
    function Get__NewEnum: IUnknown; safecall;
    function Get_Item(index: OleVariant): IUsbDevice; safecall;
    function Get_Count: Integer; safecall;
    property _NewEnum: IUnknown read Get__NewEnum;
    property Item[index: OleVariant]: IUsbDevice read Get_Item; default;
    property Count: Integer read Get_Count;
  end;

// *********************************************************************//
// DispIntf:  IUsbDevicesDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20008CC2-6576-4665-8411-99291CF18387}
// *********************************************************************//
  IUsbDevicesDisp = dispinterface
    ['{20008CC2-6576-4665-8411-99291CF18387}']
    property _NewEnum: IUnknown readonly dispid -4;
    property Item[index: OleVariant]: IUsbDevice readonly dispid 0; default;
    property Count: Integer readonly dispid 1793;
  end;

// *********************************************************************//
// Interface: _IInterfaceInternal
// Flags:     (400) Hidden NonExtensible OleAutomation
// GUID:      {20000000-CBF2-4E35-9F92-858A76D8D2B3}
// *********************************************************************//
  _IInterfaceInternal = interface(IUnknown)
    ['{20000000-CBF2-4E35-9F92-858A76D8D2B3}']
    function suspendReporting(suspend: WordBool; post: WordBool): HResult; stdcall;
  end;

// *********************************************************************//
// Interface: IErrorCode
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001A79-3839-4E67-9855-075A7312AA0D}
// *********************************************************************//
  IErrorCode = interface(IDispatch)
    ['{20001A79-3839-4E67-9855-075A7312AA0D}']
    function Get_value: SYSINT; safecall;
    function Get_category: WideString; safecall;
    function Get_message: WideString; safecall;
    property value: SYSINT read Get_value;
    property category: WideString read Get_category;
    property message: WideString read Get_message;
  end;

// *********************************************************************//
// DispIntf:  IErrorCodeDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001A79-3839-4E67-9855-075A7312AA0D}
// *********************************************************************//
  IErrorCodeDisp = dispinterface
    ['{20001A79-3839-4E67-9855-075A7312AA0D}']
    property value: SYSINT readonly dispid 0;
    property category: WideString readonly dispid 7681;
    property message: WideString readonly dispid 7682;
  end;

// *********************************************************************//
// Interface: IUsbInterface
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C1D7-8270-490B-A8F3-766258E6BAB9}
// *********************************************************************//
  IUsbInterface = interface(IInterface)
    ['{2000C1D7-8270-490B-A8F3-766258E6BAB9}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; safecall;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  IUsbInterfaceDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C1D7-8270-490B-A8F3-766258E6BAB9}
// *********************************************************************//
  IUsbInterfaceDisp = dispinterface
    ['{2000C1D7-8270-490B-A8F3-766258E6BAB9}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 1025;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; dispid 1026;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IUsbInterface2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D919-BE93-453F-B0F2-DEB98AA09B7C}
// *********************************************************************//
  IUsbInterface2 = interface(IInterface2)
    ['{2000D919-BE93-453F-B0F2-DEB98AA09B7C}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; safecall;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  IUsbInterface2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000D919-BE93-453F-B0F2-DEB98AA09B7C}
// *********************************************************************//
  IUsbInterface2Disp = dispinterface
    ['{2000D919-BE93-453F-B0F2-DEB98AA09B7C}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 1025;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; dispid 1026;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IUsbInterface3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200024FB-F324-47CF-8909-4CB7129D88EB}
// *********************************************************************//
  IUsbInterface3 = interface(IInterface3)
    ['{200024FB-F324-47CF-8909-4CB7129D88EB}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; safecall;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  IUsbInterface3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200024FB-F324-47CF-8909-4CB7129D88EB}
// *********************************************************************//
  IUsbInterface3Disp = dispinterface
    ['{200024FB-F324-47CF-8909-4CB7129D88EB}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 1025;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; dispid 1026;
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IUsbInterface4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000249E-4DC7-40EA-BE44-A963011493EC}
// *********************************************************************//
  IUsbInterface4 = interface(IInterface4)
    ['{2000249E-4DC7-40EA-BE44-A963011493EC}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; safecall;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  IUsbInterface4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000249E-4DC7-40EA-BE44-A963011493EC}
// *********************************************************************//
  IUsbInterface4Disp = dispinterface
    ['{2000249E-4DC7-40EA-BE44-A963011493EC}']
    function connect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 1025;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode; dispid 1026;
    property Protocol___: IProtocol4 readonly dispid 784;
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: ISerialInterface
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003D0C-A047-4487-8FE0-0CF9AB05E48C}
// *********************************************************************//
  ISerialInterface = interface(IInterface)
    ['{20003D0C-A047-4487-8FE0-0CF9AB05E48C}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  ISerialInterfaceDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003D0C-A047-4487-8FE0-0CF9AB05E48C}
// *********************************************************************//
  ISerialInterfaceDisp = dispinterface
    ['{20003D0C-A047-4487-8FE0-0CF9AB05E48C}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 5889;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: ISerialInterface2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000EE39-B43B-4A2E-AA71-82D44C6C1A44}
// *********************************************************************//
  ISerialInterface2 = interface(IInterface2)
    ['{2000EE39-B43B-4A2E-AA71-82D44C6C1A44}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  ISerialInterface2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000EE39-B43B-4A2E-AA71-82D44C6C1A44}
// *********************************************************************//
  ISerialInterface2Disp = dispinterface
    ['{2000EE39-B43B-4A2E-AA71-82D44C6C1A44}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 5889;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: ISerialInterface3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003335-233C-4842-9C8A-75C7191B34A3}
// *********************************************************************//
  ISerialInterface3 = interface(IInterface3)
    ['{20003335-233C-4842-9C8A-75C7191B34A3}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  ISerialInterface3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003335-233C-4842-9C8A-75C7191B34A3}
// *********************************************************************//
  ISerialInterface3Disp = dispinterface
    ['{20003335-233C-4842-9C8A-75C7191B34A3}']
    function connect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 5889;
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: ISerialInterface4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C576-80D3-4C78-A3FC-80C203080A6B}
// *********************************************************************//
  ISerialInterface4 = interface(IInterface4)
    ['{2000C576-80D3-4C78-A3FC-80C203080A6B}']
    function connect(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode; safecall;
  end;

// *********************************************************************//
// DispIntf:  ISerialInterface4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000C576-80D3-4C78-A3FC-80C203080A6B}
// *********************************************************************//
  ISerialInterface4Disp = dispinterface
    ['{2000C576-80D3-4C78-A3FC-80C203080A6B}']
    function connect(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode; dispid 5890;
    property Protocol___: IProtocol4 readonly dispid 784;
    procedure queueSetPredicateAll(predicate: WordBool); dispid 781;
    function interfaceQueue_: IInterfaceQueue2; dispid 782;
    property Protocol__: IProtocol3 readonly dispid 783;
    function getProductId: Word; dispid 779;
    property Protocol_: IProtocol2 readonly dispid 780;
    procedure disconnect; dispid 769;
    function isConnected: WordBool; dispid 770;
    procedure get(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 771;
    procedure set_(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 772;
    function supportsWrite: WordBool; dispid 773;
    procedure write(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 774;
    function interfaceQueue: IInterfaceQueue; dispid 775;
    procedure queueNotifyAll; dispid 776;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 777;
    property Protocol: IProtocol readonly dispid 778;
  end;

// *********************************************************************//
// Interface: IPenData
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003BCE-8DD9-43AD-819B-F746171CA1EC}
// *********************************************************************//
  IPenData = interface(IDispatch)
    ['{20003BCE-8DD9-43AD-819B-F746171CA1EC}']
    function Get_rdy: WordBool; safecall;
    function Get_sw: Byte; safecall;
    function Get_pressure: Word; safecall;
    function Get_x: Word; safecall;
    function Get_y: Word; safecall;
    property rdy: WordBool read Get_rdy;
    property sw: Byte read Get_sw;
    property pressure: Word read Get_pressure;
    property x: Word read Get_x;
    property y: Word read Get_y;
  end;

// *********************************************************************//
// DispIntf:  IPenDataDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20003BCE-8DD9-43AD-819B-F746171CA1EC}
// *********************************************************************//
  IPenDataDisp = dispinterface
    ['{20003BCE-8DD9-43AD-819B-F746171CA1EC}']
    property rdy: WordBool readonly dispid 3329;
    property sw: Byte readonly dispid 3330;
    property pressure: Word readonly dispid 3331;
    property x: Word readonly dispid 3332;
    property y: Word readonly dispid 3333;
  end;

// *********************************************************************//
// Interface: IPenDataOption
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200041E8-CBEC-4414-A64D-B6600A4D3FF6}
// *********************************************************************//
  IPenDataOption = interface(IPenData)
    ['{200041E8-CBEC-4414-A64D-B6600A4D3FF6}']
    function Get_option: Word; safecall;
    property option: Word read Get_option;
  end;

// *********************************************************************//
// DispIntf:  IPenDataOptionDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200041E8-CBEC-4414-A64D-B6600A4D3FF6}
// *********************************************************************//
  IPenDataOptionDisp = dispinterface
    ['{200041E8-CBEC-4414-A64D-B6600A4D3FF6}']
    property option: Word readonly dispid 3585;
    property rdy: WordBool readonly dispid 3329;
    property sw: Byte readonly dispid 3330;
    property pressure: Word readonly dispid 3331;
    property x: Word readonly dispid 3332;
    property y: Word readonly dispid 3333;
  end;

// *********************************************************************//
// Interface: IPenDataEncrypted
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20007929-8A2A-49DF-8B6D-38B71F8991AB}
// *********************************************************************//
  IPenDataEncrypted = interface(IDispatch)
    ['{20007929-8A2A-49DF-8B6D-38B71F8991AB}']
    function Get_sessionId: SYSUINT; safecall;
    function Get_penData1: IPenData; safecall;
    function Get_penData2: IPenData; safecall;
    property sessionId: SYSUINT read Get_sessionId;
    property penData1: IPenData read Get_penData1;
    property penData2: IPenData read Get_penData2;
  end;

// *********************************************************************//
// DispIntf:  IPenDataEncryptedDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20007929-8A2A-49DF-8B6D-38B71F8991AB}
// *********************************************************************//
  IPenDataEncryptedDisp = dispinterface
    ['{20007929-8A2A-49DF-8B6D-38B71F8991AB}']
    property sessionId: SYSUINT readonly dispid 3841;
    property penData1: IPenData readonly dispid 3842;
    property penData2: IPenData readonly dispid 3843;
  end;

// *********************************************************************//
// Interface: IPenDataEncryptedOption
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200088A2-DC76-4716-8879-6E6115A1BBE8}
// *********************************************************************//
  IPenDataEncryptedOption = interface(IPenDataEncrypted)
    ['{200088A2-DC76-4716-8879-6E6115A1BBE8}']
    function Get_option1: Word; safecall;
    function Get_option2: Word; safecall;
    property option1: Word read Get_option1;
    property option2: Word read Get_option2;
  end;

// *********************************************************************//
// DispIntf:  IPenDataEncryptedOptionDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200088A2-DC76-4716-8879-6E6115A1BBE8}
// *********************************************************************//
  IPenDataEncryptedOptionDisp = dispinterface
    ['{200088A2-DC76-4716-8879-6E6115A1BBE8}']
    property option1: Word readonly dispid 4097;
    property option2: Word readonly dispid 4098;
    property sessionId: SYSUINT readonly dispid 3841;
    property penData1: IPenData readonly dispid 3842;
    property penData2: IPenData readonly dispid 3843;
  end;

// *********************************************************************//
// Interface: IPenDataTimeCountSequence
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20009A29-0C7E-44D5-9A06-960958F032FB}
// *********************************************************************//
  IPenDataTimeCountSequence = interface(IPenData)
    ['{20009A29-0C7E-44D5-9A06-960958F032FB}']
    function Get_timeCount: Word; safecall;
    function Get_sequence: Word; safecall;
    property timeCount: Word read Get_timeCount;
    property sequence: Word read Get_sequence;
  end;

// *********************************************************************//
// DispIntf:  IPenDataTimeCountSequenceDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20009A29-0C7E-44D5-9A06-960958F032FB}
// *********************************************************************//
  IPenDataTimeCountSequenceDisp = dispinterface
    ['{20009A29-0C7E-44D5-9A06-960958F032FB}']
    property timeCount: Word readonly dispid 9729;
    property sequence: Word readonly dispid 9730;
    property rdy: WordBool readonly dispid 3329;
    property sw: Byte readonly dispid 3330;
    property pressure: Word readonly dispid 3331;
    property x: Word readonly dispid 3332;
    property y: Word readonly dispid 3333;
  end;

// *********************************************************************//
// Interface: IPenDataTimeCountSequenceEncrypted
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006841-7756-4F96-AF19-A8DD03BE5C3C}
// *********************************************************************//
  IPenDataTimeCountSequenceEncrypted = interface(IPenDataTimeCountSequence)
    ['{20006841-7756-4F96-AF19-A8DD03BE5C3C}']
    function Get_sessionId: SYSUINT; safecall;
    property sessionId: SYSUINT read Get_sessionId;
  end;

// *********************************************************************//
// DispIntf:  IPenDataTimeCountSequenceEncryptedDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20006841-7756-4F96-AF19-A8DD03BE5C3C}
// *********************************************************************//
  IPenDataTimeCountSequenceEncryptedDisp = dispinterface
    ['{20006841-7756-4F96-AF19-A8DD03BE5C3C}']
    property sessionId: SYSUINT readonly dispid 9985;
    property timeCount: Word readonly dispid 9729;
    property sequence: Word readonly dispid 9730;
    property rdy: WordBool readonly dispid 3329;
    property sw: Byte readonly dispid 3330;
    property pressure: Word readonly dispid 3331;
    property x: Word readonly dispid 3332;
    property y: Word readonly dispid 3333;
  end;

// *********************************************************************//
// Interface: IProtocolHelper
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000550B-EFC5-4D5F-8969-45C9A67CC93C}
// *********************************************************************//
  IProtocolHelper = interface(IDispatch)
    ['{2000550B-EFC5-4D5F-8969-45C9A67CC93C}']
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; safecall;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); safecall;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); safecall;
    function supportsEncryption(const Protocol: IProtocol): WordBool; safecall;
    function supportsEncryption_DHprime(dhPrime: PSafeArray): WordBool; safecall;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: PSafeArray; retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): PSafeArray; safecall;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; imageData: PSafeArray; 
                         retries: SYSUINT; sleepBetweenRetries: SYSUINT); safecall;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): PSafeArray; safecall;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): PSafeArray; safecall;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): PSafeArray; safecall;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): PSafeArray; safecall;
  end;

// *********************************************************************//
// DispIntf:  IProtocolHelperDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000550B-EFC5-4D5F-8969-45C9A67CC93C}
// *********************************************************************//
  IProtocolHelperDisp = dispinterface
    ['{2000550B-EFC5-4D5F-8969-45C9A67CC93C}']
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; dispid 6145;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); dispid 6146;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); dispid 6147;
    function supportsEncryption(const Protocol: IProtocol): WordBool; dispid 6148;
    function supportsEncryption_DHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 6149;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                                                       retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6150;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; 
                         imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                         sleepBetweenRetries: SYSUINT); dispid 6152;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6153;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6154;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6155;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6156;
  end;

// *********************************************************************//
// Interface: IProtocolHelper2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000954F-FB11-4414-A4EC-80A92AFC8664}
// *********************************************************************//
  IProtocolHelper2 = interface(IProtocolHelper)
    ['{2000954F-FB11-4414-A4EC-80A92AFC8664}']
    function generateSymmetricKeyAndWaitForEncryptionStatus(const Protocol: IProtocol2; 
                                                            retries: SYSUINT; 
                                                            sleepBetweenRetries: SYSUINT; 
                                                            timeout: SYSUINT): IEncryptionStatus; safecall;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; safecall;
    function encodingFlagSupportsColor(encodingFlag: Byte): WordBool; safecall;
    function flattenColor16(image: OleVariant; screenWidth: Word; screenHeight: Word): PSafeArray; safecall;
    function flattenColor24(image: OleVariant; screenWidth: Word; screenHeight: Word): PSafeArray; safecall;
    function flatten_(image: OleVariant; screenWidth: Word; screenHeight: Word; encodingMode: Byte): PSafeArray; safecall;
    function resizeAndFlatten_(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                               screenHeight: Word; encodingMode: Byte; Scale: Scale; 
                               backgroundColor: OleVariant; Clip: Byte): PSafeArray; safecall;
    procedure writeImageArea(const Protocol: IProtocol2; encodingMode: Byte; 
                             const area: IRectangle; data: PSafeArray; retries: SYSUINT; 
                             sleepBetweenRetries: SYSUINT); safecall;
  end;

// *********************************************************************//
// DispIntf:  IProtocolHelper2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000954F-FB11-4414-A4EC-80A92AFC8664}
// *********************************************************************//
  IProtocolHelper2Disp = dispinterface
    ['{2000954F-FB11-4414-A4EC-80A92AFC8664}']
    function generateSymmetricKeyAndWaitForEncryptionStatus(const Protocol: IProtocol2; 
                                                            retries: SYSUINT; 
                                                            sleepBetweenRetries: SYSUINT; 
                                                            timeout: SYSUINT): IEncryptionStatus; dispid 6157;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; dispid 6158;
    function encodingFlagSupportsColor(encodingFlag: Byte): WordBool; dispid 6159;
    function flattenColor16(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6160;
    function flattenColor24(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6161;
    function flatten_(image: OleVariant; screenWidth: Word; screenHeight: Word; encodingMode: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6162;
    function resizeAndFlatten_(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                               screenHeight: Word; encodingMode: Byte; Scale: Scale; 
                               backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6163;
    procedure writeImageArea(const Protocol: IProtocol2; encodingMode: Byte; 
                             const area: IRectangle; data: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                             retries: SYSUINT; sleepBetweenRetries: SYSUINT); dispid 6164;
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; dispid 6145;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); dispid 6146;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); dispid 6147;
    function supportsEncryption(const Protocol: IProtocol): WordBool; dispid 6148;
    function supportsEncryption_DHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 6149;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                                                       retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6150;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; 
                         imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                         sleepBetweenRetries: SYSUINT); dispid 6152;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6153;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6154;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6155;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6156;
  end;

// *********************************************************************//
// Interface: IProtocolHelper3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200012E8-04DC-44D9-8EEE-872792C85A53}
// *********************************************************************//
  IProtocolHelper3 = interface(IProtocolHelper2)
    ['{200012E8-04DC-44D9-8EEE-872792C85A53}']
    function getNextInkState(InkState: InkState; pressure: Word; const inkThreshold: IInkThreshold): InkState; safecall;
  end;

// *********************************************************************//
// DispIntf:  IProtocolHelper3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200012E8-04DC-44D9-8EEE-872792C85A53}
// *********************************************************************//
  IProtocolHelper3Disp = dispinterface
    ['{200012E8-04DC-44D9-8EEE-872792C85A53}']
    function getNextInkState(InkState: InkState; pressure: Word; const inkThreshold: IInkThreshold): InkState; dispid 6165;
    function generateSymmetricKeyAndWaitForEncryptionStatus(const Protocol: IProtocol2; 
                                                            retries: SYSUINT; 
                                                            sleepBetweenRetries: SYSUINT; 
                                                            timeout: SYSUINT): IEncryptionStatus; dispid 6157;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; dispid 6158;
    function encodingFlagSupportsColor(encodingFlag: Byte): WordBool; dispid 6159;
    function flattenColor16(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6160;
    function flattenColor24(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6161;
    function flatten_(image: OleVariant; screenWidth: Word; screenHeight: Word; encodingMode: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6162;
    function resizeAndFlatten_(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                               screenHeight: Word; encodingMode: Byte; Scale: Scale; 
                               backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6163;
    procedure writeImageArea(const Protocol: IProtocol2; encodingMode: Byte; 
                             const area: IRectangle; data: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                             retries: SYSUINT; sleepBetweenRetries: SYSUINT); dispid 6164;
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; dispid 6145;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); dispid 6146;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); dispid 6147;
    function supportsEncryption(const Protocol: IProtocol): WordBool; dispid 6148;
    function supportsEncryption_DHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 6149;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                                                       retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6150;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; 
                         imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                         sleepBetweenRetries: SYSUINT); dispid 6152;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6153;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6154;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6155;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6156;
  end;

// *********************************************************************//
// Interface: IProtocolHelper4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20002A23-ACB3-47B8-A7A5-4A4F0C791D1A}
// *********************************************************************//
  IProtocolHelper4 = interface(IProtocolHelper3)
    ['{20002A23-ACB3-47B8-A7A5-4A4F0C791D1A}']
    function getNextInkState_(InkState: InkState; rdy: WordBool; pressure: Word; 
                              const inkThreshold: IInkThreshold): InkState; safecall;
  end;

// *********************************************************************//
// DispIntf:  IProtocolHelper4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20002A23-ACB3-47B8-A7A5-4A4F0C791D1A}
// *********************************************************************//
  IProtocolHelper4Disp = dispinterface
    ['{20002A23-ACB3-47B8-A7A5-4A4F0C791D1A}']
    function getNextInkState_(InkState: InkState; rdy: WordBool; pressure: Word; 
                              const inkThreshold: IInkThreshold): InkState; dispid 6166;
    function getNextInkState(InkState: InkState; pressure: Word; const inkThreshold: IInkThreshold): InkState; dispid 6165;
    function generateSymmetricKeyAndWaitForEncryptionStatus(const Protocol: IProtocol2; 
                                                            retries: SYSUINT; 
                                                            sleepBetweenRetries: SYSUINT; 
                                                            timeout: SYSUINT): IEncryptionStatus; dispid 6157;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; dispid 6158;
    function encodingFlagSupportsColor(encodingFlag: Byte): WordBool; dispid 6159;
    function flattenColor16(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6160;
    function flattenColor24(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6161;
    function flatten_(image: OleVariant; screenWidth: Word; screenHeight: Word; encodingMode: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6162;
    function resizeAndFlatten_(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                               screenHeight: Word; encodingMode: Byte; Scale: Scale; 
                               backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6163;
    procedure writeImageArea(const Protocol: IProtocol2; encodingMode: Byte; 
                             const area: IRectangle; data: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                             retries: SYSUINT; sleepBetweenRetries: SYSUINT); dispid 6164;
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; dispid 6145;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); dispid 6146;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); dispid 6147;
    function supportsEncryption(const Protocol: IProtocol): WordBool; dispid 6148;
    function supportsEncryption_DHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 6149;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                                                       retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6150;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; 
                         imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                         sleepBetweenRetries: SYSUINT); dispid 6152;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6153;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6154;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6155;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6156;
  end;

// *********************************************************************//
// Interface: IProtocolHelper5
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200014AC-4DBF-40A9-9312-090B8A8EE289}
// *********************************************************************//
  IProtocolHelper5 = interface(IProtocolHelper4)
    ['{200014AC-4DBF-40A9-9312-090B8A8EE289}']
    procedure writeImage_(const Protocol: IProtocol; encodingMode: Byte; maxImageBlockSize: Word; 
                          imageData: PSafeArray; retries: SYSUINT; sleepBetweenRetries: SYSUINT); safecall;
    procedure writeImageArea_(const Protocol: IProtocol2; encodingMode: Byte; 
                              const area: IRectangle; maxImageBlockSize: Word; data: PSafeArray; 
                              retries: SYSUINT; sleepBetweenRetries: SYSUINT); safecall;
  end;

// *********************************************************************//
// DispIntf:  IProtocolHelper5Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200014AC-4DBF-40A9-9312-090B8A8EE289}
// *********************************************************************//
  IProtocolHelper5Disp = dispinterface
    ['{200014AC-4DBF-40A9-9312-090B8A8EE289}']
    procedure writeImage_(const Protocol: IProtocol; encodingMode: Byte; maxImageBlockSize: Word; 
                          imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                          sleepBetweenRetries: SYSUINT); dispid 6167;
    procedure writeImageArea_(const Protocol: IProtocol2; encodingMode: Byte; 
                              const area: IRectangle; maxImageBlockSize: Word; 
                              data: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                              sleepBetweenRetries: SYSUINT); dispid 6168;
    function getNextInkState_(InkState: InkState; rdy: WordBool; pressure: Word; 
                              const inkThreshold: IInkThreshold): InkState; dispid 6166;
    function getNextInkState(InkState: InkState; pressure: Word; const inkThreshold: IInkThreshold): InkState; dispid 6165;
    function generateSymmetricKeyAndWaitForEncryptionStatus(const Protocol: IProtocol2; 
                                                            retries: SYSUINT; 
                                                            sleepBetweenRetries: SYSUINT; 
                                                            timeout: SYSUINT): IEncryptionStatus; dispid 6157;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; dispid 6158;
    function encodingFlagSupportsColor(encodingFlag: Byte): WordBool; dispid 6159;
    function flattenColor16(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6160;
    function flattenColor24(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6161;
    function flatten_(image: OleVariant; screenWidth: Word; screenHeight: Word; encodingMode: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6162;
    function resizeAndFlatten_(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                               screenHeight: Word; encodingMode: Byte; Scale: Scale; 
                               backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6163;
    procedure writeImageArea(const Protocol: IProtocol2; encodingMode: Byte; 
                             const area: IRectangle; data: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                             retries: SYSUINT; sleepBetweenRetries: SYSUINT); dispid 6164;
    function statusCanSend(statusCode: Byte; ReportId: Byte; OpDirection: OpDirection): WordBool; dispid 6145;
    procedure waitForStatusToSend(const Protocol: IProtocol; ReportId: Byte; 
                                  OpDirection: OpDirection; retries: SYSUINT; 
                                  sleepBetweenRetries: SYSUINT); dispid 6146;
    procedure waitForStatus(const Protocol: IProtocol; statusCode: Byte; retries: SYSUINT; 
                            sleepBetweenRetries: SYSUINT); dispid 6147;
    function supportsEncryption(const Protocol: IProtocol): WordBool; dispid 6148;
    function supportsEncryption_DHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 6149;
    function setHostPublicKeyAndPollForDevicePublicKey(const Protocol: IProtocol; 
                                                       hostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                                                       retries: SYSUINT; 
                                                       sleepBetweenRetries: SYSUINT): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6150;
    procedure writeImage(const Protocol: IProtocol; encodingMode: Byte; 
                         imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant; retries: SYSUINT; 
                         sleepBetweenRetries: SYSUINT); dispid 6152;
    function flattenMonochrome(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6153;
    function flattenColor16_565(image: OleVariant; screenWidth: Word; screenHeight: Word): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6154;
    function flatten(image: OleVariant; screenWidth: Word; screenHeight: Word; isColor: WordBool): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6155;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT; 
                              bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word; 
                              screenHeight: Word; isColor: WordBool; Scale: Scale; 
                              backgroundColor: OleVariant; Clip: Byte): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6156;
  end;

// *********************************************************************//
// Interface: IInkingState
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001A3F-7165-45C1-BEF9-8196DAB7D039}
// *********************************************************************//
  IInkingState = interface(IDispatch)
    ['{20001A3F-7165-45C1-BEF9-8196DAB7D039}']
    procedure setInkThreshold(const inkThreshold: IInkThreshold); safecall;
    function getInkThreshold: IInkThreshold; safecall;
    procedure setState(InkState: InkState); safecall;
    procedure clear; safecall;
    function currentState: InkState; safecall;
    function nextState(pressure: Word): InkState; safecall;
  end;

// *********************************************************************//
// DispIntf:  IInkingStateDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001A3F-7165-45C1-BEF9-8196DAB7D039}
// *********************************************************************//
  IInkingStateDisp = dispinterface
    ['{20001A3F-7165-45C1-BEF9-8196DAB7D039}']
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 10241;
    function getInkThreshold: IInkThreshold; dispid 10242;
    procedure setState(InkState: InkState); dispid 10243;
    procedure clear; dispid 10244;
    function currentState: InkState; dispid 10245;
    function nextState(pressure: Word): InkState; dispid 10246;
  end;

// *********************************************************************//
// Interface: IInkingState2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200026D4-2949-411F-BD21-8D01E5DF518C}
// *********************************************************************//
  IInkingState2 = interface(IInkingState)
    ['{200026D4-2949-411F-BD21-8D01E5DF518C}']
    function nextState_(rdy: WordBool; pressure: Word): InkState; safecall;
  end;

// *********************************************************************//
// DispIntf:  IInkingState2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200026D4-2949-411F-BD21-8D01E5DF518C}
// *********************************************************************//
  IInkingState2Disp = dispinterface
    ['{200026D4-2949-411F-BD21-8D01E5DF518C}']
    function nextState_(rdy: WordBool; pressure: Word): InkState; dispid 10247;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 10241;
    function getInkThreshold: IInkThreshold; dispid 10242;
    procedure setState(InkState: InkState); dispid 10243;
    procedure clear; dispid 10244;
    function currentState: InkState; dispid 10245;
    function nextState(pressure: Word): InkState; dispid 10246;
  end;

// *********************************************************************//
// Interface: IReport
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000122F-B55F-40DE-9284-277D32AEE77F}
// *********************************************************************//
  IReport = interface(IDispatch)
    ['{2000122F-B55F-40DE-9284-277D32AEE77F}']
    function Get_data: PSafeArray; safecall;
    property data: PSafeArray read Get_data;
  end;

// *********************************************************************//
// DispIntf:  IReportDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000122F-B55F-40DE-9284-277D32AEE77F}
// *********************************************************************//
  IReportDisp = dispinterface
    ['{2000122F-B55F-40DE-9284-277D32AEE77F}']
    property data: {NOT_OLEAUTO(PSafeArray)}OleVariant readonly dispid 1281;
  end;

// *********************************************************************//
// Interface: IDecrypt
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20007FDA-B1C5-44F2-A8F8-AFCD4E6A76C2}
// *********************************************************************//
  IDecrypt = interface(IDispatch)
    ['{20007FDA-B1C5-44F2-A8F8-AFCD4E6A76C2}']
    function Get_data: PSafeArray; safecall;
    procedure Set_data(pRetVal: PSafeArray); safecall;
    property data: PSafeArray read Get_data write Set_data;
  end;

// *********************************************************************//
// DispIntf:  IDecryptDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20007FDA-B1C5-44F2-A8F8-AFCD4E6A76C2}
// *********************************************************************//
  IDecryptDisp = dispinterface
    ['{20007FDA-B1C5-44F2-A8F8-AFCD4E6A76C2}']
    property data: {NOT_OLEAUTO(PSafeArray)}OleVariant dispid 5633;
  end;

// *********************************************************************//
// Interface: IReportHandler
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000446E-736E-4513-9751-A9B69648863E}
// *********************************************************************//
  IReportHandler = interface(IDispatch)
    ['{2000446E-736E-4513-9751-A9B69648863E}']
    function handleReport(data: PSafeArray): WordBool; safecall;
  end;

// *********************************************************************//
// DispIntf:  IReportHandlerDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000446E-736E-4513-9751-A9B69648863E}
// *********************************************************************//
  IReportHandlerDisp = dispinterface
    ['{2000446E-736E-4513-9751-A9B69648863E}']
    function handleReport(data: {NOT_OLEAUTO(PSafeArray)}OleVariant): WordBool; dispid 5121;
  end;

// *********************************************************************//
// Interface: ITabletEncryptionHandler
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000B2ED-AEDA-4E86-8A69-BB987FA3B238}
// *********************************************************************//
  ITabletEncryptionHandler = interface(IDispatch)
    ['{2000B2ED-AEDA-4E86-8A69-BB987FA3B238}']
    procedure reset; safecall;
    procedure clearKeys; safecall;
    function requireDH: WordBool; safecall;
    procedure setDH(dhPrime: PSafeArray; dhBase: PSafeArray); safecall;
    function generateHostPublicKey: PSafeArray; safecall;
    procedure computeSharedKey(devicePublicKey: PSafeArray); safecall;
    function decrypt(data: PSafeArray): PSafeArray; safecall;
  end;

// *********************************************************************//
// DispIntf:  ITabletEncryptionHandlerDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000B2ED-AEDA-4E86-8A69-BB987FA3B238}
// *********************************************************************//
  ITabletEncryptionHandlerDisp = dispinterface
    ['{2000B2ED-AEDA-4E86-8A69-BB987FA3B238}']
    procedure reset; dispid 7169;
    procedure clearKeys; dispid 7170;
    function requireDH: WordBool; dispid 7171;
    procedure setDH(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant; 
                    dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 7172;
    function generateHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 7173;
    procedure computeSharedKey(devicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 7174;
    function decrypt(data: {NOT_OLEAUTO(PSafeArray)}OleVariant): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 7175;
  end;

// *********************************************************************//
// Interface: ITabletEncryptionHandler2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000AEB7-19C4-42C3-B01E-CE669482FDF4}
// *********************************************************************//
  ITabletEncryptionHandler2 = interface(IDispatch)
    ['{2000AEB7-19C4-42C3-B01E-CE669482FDF4}']
    procedure reset; safecall;
    procedure clearKeys; safecall;
    function getSymmetricKeyType: Byte; safecall;
    function getAsymmetricPaddingType: Byte; safecall;
    function getAsymmetricKeyType: Byte; safecall;
    function getPublicExponent: PSafeArray; safecall;
    function generatePublicKey: PSafeArray; safecall;
    procedure computeSessionKey(data: PSafeArray); safecall;
    function decrypt(data: PSafeArray): PSafeArray; safecall;
  end;

// *********************************************************************//
// DispIntf:  ITabletEncryptionHandler2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000AEB7-19C4-42C3-B01E-CE669482FDF4}
// *********************************************************************//
  ITabletEncryptionHandler2Disp = dispinterface
    ['{2000AEB7-19C4-42C3-B01E-CE669482FDF4}']
    procedure reset; dispid 8961;
    procedure clearKeys; dispid 8962;
    function getSymmetricKeyType: Byte; dispid 8963;
    function getAsymmetricPaddingType: Byte; dispid 8964;
    function getAsymmetricKeyType: Byte; dispid 8965;
    function getPublicExponent: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 8966;
    function generatePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 8967;
    procedure computeSessionKey(data: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 8968;
    function decrypt(data: {NOT_OLEAUTO(PSafeArray)}OleVariant): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 8969;
  end;

// *********************************************************************//
// Interface: ITablet
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005C27-7729-492F-8106-540F2E04DC66}
// *********************************************************************//
  ITablet = interface(IDispatch)
    ['{20005C27-7729-492F-8106-540F2E04DC66}']
    function Get_encryptionHandler: ITabletEncryptionHandler; safecall;
    procedure Set_encryptionHandler(const pRetVal: ITabletEncryptionHandler); safecall;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; safecall;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString; 
                         exclusiveLock: WordBool): IErrorCode; safecall;
    function serialConnect(const fileName: WideString; useCrc: WordBool): IErrorCode; safecall;
    function Get_Protocol: IProtocol; safecall;
    function isEmpty: WordBool; safecall;
    function isConnected: WordBool; safecall;
    procedure disconnect; safecall;
    function interfaceQueue: IInterfaceQueue; safecall;
    procedure queueNotifyAll; safecall;
    function supportsWrite: WordBool; safecall;
    function getReportCountLengths: PSafeArray; safecall;
    function isSupported(ReportId: Byte): WordBool; safecall;
    function getStatus: IStatus; safecall;
    procedure reset; safecall;
    function getInformation: IInformation; safecall;
    function getCapability: ICapability; safecall;
    function getUid: SYSUINT; safecall;
    procedure setUid(uid: SYSUINT); safecall;
    function getHostPublicKey: PSafeArray; safecall;
    function getDevicePublicKey: PSafeArray; safecall;
    procedure startCapture(sessionId: SYSUINT); safecall;
    procedure endCapture; safecall;
    function getDHprime: PSafeArray; safecall;
    procedure setDHprime(dhPrime: PSafeArray); safecall;
    function getDHbase: PSafeArray; safecall;
    procedure setDHbase(dhBase: PSafeArray); safecall;
    procedure setClearScreen; safecall;
    function getInkingMode: Byte; safecall;
    procedure setInkingMode(inkingMode: Byte); safecall;
    function getInkThreshold: IInkThreshold; safecall;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); safecall;
    procedure writeImage(encodingMode: Byte; imageData: PSafeArray); safecall;
    procedure endImageData; safecall;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; safecall;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); safecall;
    function getBackgroundColor: Word; safecall;
    procedure setBackgroundColor(backgroundColor: Word); safecall;
    function getHandwritingDisplayArea: IRectangle; safecall;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); safecall;
    function getBacklightBrightness: Word; safecall;
    procedure setBacklightBrightness(backlightBrightness: Word); safecall;
    function getPenDataOptionMode: Byte; safecall;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); safecall;
    property encryptionHandler: ITabletEncryptionHandler read Get_encryptionHandler write Set_encryptionHandler;
    property Protocol: IProtocol read Get_Protocol;
  end;

// *********************************************************************//
// DispIntf:  ITabletDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005C27-7729-492F-8106-540F2E04DC66}
// *********************************************************************//
  ITabletDisp = dispinterface
    ['{20005C27-7729-492F-8106-540F2E04DC66}']
    property encryptionHandler: ITabletEncryptionHandler dispid 6657;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 6658;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString; 
                         exclusiveLock: WordBool): IErrorCode; dispid 6659;
    function serialConnect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 6660;
    property Protocol: IProtocol readonly dispid 6661;
    function isEmpty: WordBool; dispid 6662;
    function isConnected: WordBool; dispid 6663;
    procedure disconnect; dispid 6664;
    function interfaceQueue: IInterfaceQueue; dispid 6665;
    procedure queueNotifyAll; dispid 6666;
    function supportsWrite: WordBool; dispid 6667;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6668;
    function isSupported(ReportId: Byte): WordBool; dispid 6669;
    function getStatus: IStatus; dispid 6670;
    procedure reset; dispid 6671;
    function getInformation: IInformation; dispid 6672;
    function getCapability: ICapability; dispid 6673;
    function getUid: SYSUINT; dispid 6674;
    procedure setUid(uid: SYSUINT); dispid 6675;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6676;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6677;
    procedure startCapture(sessionId: SYSUINT); dispid 6678;
    procedure endCapture; dispid 6679;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6680;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6681;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6682;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6683;
    procedure setClearScreen; dispid 6684;
    function getInkingMode: Byte; dispid 6685;
    procedure setInkingMode(inkingMode: Byte); dispid 6686;
    function getInkThreshold: IInkThreshold; dispid 6687;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 6688;
    procedure writeImage(encodingMode: Byte; imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6689;
    procedure endImageData; dispid 6690;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 6691;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 6692;
    function getBackgroundColor: Word; dispid 6693;
    procedure setBackgroundColor(backgroundColor: Word); dispid 6694;
    function getHandwritingDisplayArea: IRectangle; dispid 6695;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 6696;
    function getBacklightBrightness: Word; dispid 6697;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 6698;
    function getPenDataOptionMode: Byte; dispid 6699;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 6700;
  end;

// *********************************************************************//
// Interface: ITablet2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000574B-751B-4D52-B59A-2336DB19C5C6}
// *********************************************************************//
  ITablet2 = interface(ITablet)
    ['{2000574B-751B-4D52-B59A-2336DB19C5C6}']
    function Get_encryptionHandler2: ITabletEncryptionHandler2; safecall;
    procedure Set_encryptionHandler2(const pRetVal: ITabletEncryptionHandler2); safecall;
    function getProductId: Word; safecall;
    function Get_Protocol_: IProtocol2; safecall;
    function getCapability_: ICapability2; safecall;
    function getUid2: WideString; safecall;
    procedure setClearScreenArea(const area: IRectangle); safecall;
    procedure writeImageArea(encodingMode: Byte; const area: IRectangle; imageData: PSafeArray); safecall;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; safecall;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); safecall;
    function getBackgroundColor24: SYSUINT; safecall;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); safecall;
    function getScreenContrast: Word; safecall;
    procedure setScreenContrast(screenContrast: Word); safecall;
    function getEncryptionStatus: IEncryptionStatus; safecall;
    property encryptionHandler2: ITabletEncryptionHandler2 read Get_encryptionHandler2 write Set_encryptionHandler2;
    property Protocol_: IProtocol2 read Get_Protocol_;
  end;

// *********************************************************************//
// DispIntf:  ITablet2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000574B-751B-4D52-B59A-2336DB19C5C6}
// *********************************************************************//
  ITablet2Disp = dispinterface
    ['{2000574B-751B-4D52-B59A-2336DB19C5C6}']
    property encryptionHandler2: ITabletEncryptionHandler2 dispid 6701;
    function getProductId: Word; dispid 6702;
    property Protocol_: IProtocol2 readonly dispid 6703;
    function getCapability_: ICapability2; dispid 6704;
    function getUid2: WideString; dispid 6705;
    procedure setClearScreenArea(const area: IRectangle); dispid 6706;
    procedure writeImageArea(encodingMode: Byte; const area: IRectangle; 
                             imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6707;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 6708;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 6709;
    function getBackgroundColor24: SYSUINT; dispid 6710;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 6711;
    function getScreenContrast: Word; dispid 6712;
    procedure setScreenContrast(screenContrast: Word); dispid 6714;
    function getEncryptionStatus: IEncryptionStatus; dispid 6715;
    property encryptionHandler: ITabletEncryptionHandler dispid 6657;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 6658;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString; 
                         exclusiveLock: WordBool): IErrorCode; dispid 6659;
    function serialConnect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 6660;
    property Protocol: IProtocol readonly dispid 6661;
    function isEmpty: WordBool; dispid 6662;
    function isConnected: WordBool; dispid 6663;
    procedure disconnect; dispid 6664;
    function interfaceQueue: IInterfaceQueue; dispid 6665;
    procedure queueNotifyAll; dispid 6666;
    function supportsWrite: WordBool; dispid 6667;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6668;
    function isSupported(ReportId: Byte): WordBool; dispid 6669;
    function getStatus: IStatus; dispid 6670;
    procedure reset; dispid 6671;
    function getInformation: IInformation; dispid 6672;
    function getCapability: ICapability; dispid 6673;
    function getUid: SYSUINT; dispid 6674;
    procedure setUid(uid: SYSUINT); dispid 6675;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6676;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6677;
    procedure startCapture(sessionId: SYSUINT); dispid 6678;
    procedure endCapture; dispid 6679;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6680;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6681;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6682;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6683;
    procedure setClearScreen; dispid 6684;
    function getInkingMode: Byte; dispid 6685;
    procedure setInkingMode(inkingMode: Byte); dispid 6686;
    function getInkThreshold: IInkThreshold; dispid 6687;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 6688;
    procedure writeImage(encodingMode: Byte; imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6689;
    procedure endImageData; dispid 6690;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 6691;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 6692;
    function getBackgroundColor: Word; dispid 6693;
    procedure setBackgroundColor(backgroundColor: Word); dispid 6694;
    function getHandwritingDisplayArea: IRectangle; dispid 6695;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 6696;
    function getBacklightBrightness: Word; dispid 6697;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 6698;
    function getPenDataOptionMode: Byte; dispid 6699;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 6700;
  end;

// *********************************************************************//
// Interface: ITablet3
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001D15-E374-4012-A0C2-C8895B5CFB75}
// *********************************************************************//
  ITablet3 = interface(ITablet2)
    ['{20001D15-E374-4012-A0C2-C8895B5CFB75}']
    function Get_Protocol__: IProtocol3; safecall;
    property Protocol__: IProtocol3 read Get_Protocol__;
  end;

// *********************************************************************//
// DispIntf:  ITablet3Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001D15-E374-4012-A0C2-C8895B5CFB75}
// *********************************************************************//
  ITablet3Disp = dispinterface
    ['{20001D15-E374-4012-A0C2-C8895B5CFB75}']
    property Protocol__: IProtocol3 readonly dispid 6716;
    property encryptionHandler2: ITabletEncryptionHandler2 dispid 6701;
    function getProductId: Word; dispid 6702;
    property Protocol_: IProtocol2 readonly dispid 6703;
    function getCapability_: ICapability2; dispid 6704;
    function getUid2: WideString; dispid 6705;
    procedure setClearScreenArea(const area: IRectangle); dispid 6706;
    procedure writeImageArea(encodingMode: Byte; const area: IRectangle; 
                             imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6707;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 6708;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 6709;
    function getBackgroundColor24: SYSUINT; dispid 6710;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 6711;
    function getScreenContrast: Word; dispid 6712;
    procedure setScreenContrast(screenContrast: Word); dispid 6714;
    function getEncryptionStatus: IEncryptionStatus; dispid 6715;
    property encryptionHandler: ITabletEncryptionHandler dispid 6657;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 6658;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString; 
                         exclusiveLock: WordBool): IErrorCode; dispid 6659;
    function serialConnect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 6660;
    property Protocol: IProtocol readonly dispid 6661;
    function isEmpty: WordBool; dispid 6662;
    function isConnected: WordBool; dispid 6663;
    procedure disconnect; dispid 6664;
    function interfaceQueue: IInterfaceQueue; dispid 6665;
    procedure queueNotifyAll; dispid 6666;
    function supportsWrite: WordBool; dispid 6667;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6668;
    function isSupported(ReportId: Byte): WordBool; dispid 6669;
    function getStatus: IStatus; dispid 6670;
    procedure reset; dispid 6671;
    function getInformation: IInformation; dispid 6672;
    function getCapability: ICapability; dispid 6673;
    function getUid: SYSUINT; dispid 6674;
    procedure setUid(uid: SYSUINT); dispid 6675;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6676;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6677;
    procedure startCapture(sessionId: SYSUINT); dispid 6678;
    procedure endCapture; dispid 6679;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6680;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6681;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6682;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6683;
    procedure setClearScreen; dispid 6684;
    function getInkingMode: Byte; dispid 6685;
    procedure setInkingMode(inkingMode: Byte); dispid 6686;
    function getInkThreshold: IInkThreshold; dispid 6687;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 6688;
    procedure writeImage(encodingMode: Byte; imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6689;
    procedure endImageData; dispid 6690;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 6691;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 6692;
    function getBackgroundColor: Word; dispid 6693;
    procedure setBackgroundColor(backgroundColor: Word); dispid 6694;
    function getHandwritingDisplayArea: IRectangle; dispid 6695;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 6696;
    function getBacklightBrightness: Word; dispid 6697;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 6698;
    function getPenDataOptionMode: Byte; dispid 6699;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 6700;
  end;

// *********************************************************************//
// Interface: ITablet4
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001376-687D-4684-AA8B-ADB60A929216}
// *********************************************************************//
  ITablet4 = interface(ITablet3)
    ['{20001376-687D-4684-AA8B-ADB60A929216}']
    function serialConnect_(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode; safecall;
    function Get_Protocol___: IProtocol4; safecall;
    function getHidInformation: IHidInformation; safecall;
    function getDefaultMode: Byte; safecall;
    procedure setDefaultMode(defaultMode: Byte); safecall;
    function getReportRate: Byte; safecall;
    procedure setReportRate(reportRate: Byte); safecall;
    function getReportSizeCollection: PSafeArray; safecall;
    property Protocol___: IProtocol4 read Get_Protocol___;
  end;

// *********************************************************************//
// DispIntf:  ITablet4Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001376-687D-4684-AA8B-ADB60A929216}
// *********************************************************************//
  ITablet4Disp = dispinterface
    ['{20001376-687D-4684-AA8B-ADB60A929216}']
    function serialConnect_(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode; dispid 6717;
    property Protocol___: IProtocol4 readonly dispid 6718;
    function getHidInformation: IHidInformation; dispid 6719;
    function getDefaultMode: Byte; dispid 6720;
    procedure setDefaultMode(defaultMode: Byte); dispid 6721;
    function getReportRate: Byte; dispid 6722;
    procedure setReportRate(reportRate: Byte); dispid 6723;
    function getReportSizeCollection: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6724;
    property Protocol__: IProtocol3 readonly dispid 6716;
    property encryptionHandler2: ITabletEncryptionHandler2 dispid 6701;
    function getProductId: Word; dispid 6702;
    property Protocol_: IProtocol2 readonly dispid 6703;
    function getCapability_: ICapability2; dispid 6704;
    function getUid2: WideString; dispid 6705;
    procedure setClearScreenArea(const area: IRectangle); dispid 6706;
    procedure writeImageArea(encodingMode: Byte; const area: IRectangle; 
                             imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6707;
    function getHandwritingThicknessColor24: IHandwritingThicknessColor24; dispid 6708;
    procedure setHandwritingThicknessColor24(const handwritingThicknessColor: IHandwritingThicknessColor24); dispid 6709;
    function getBackgroundColor24: SYSUINT; dispid 6710;
    procedure setBackgroundColor24(backgroundColor: SYSUINT); dispid 6711;
    function getScreenContrast: Word; dispid 6712;
    procedure setScreenContrast(screenContrast: Word); dispid 6714;
    function getEncryptionStatus: IEncryptionStatus; dispid 6715;
    property encryptionHandler: ITabletEncryptionHandler dispid 6657;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode; dispid 6658;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString; 
                         exclusiveLock: WordBool): IErrorCode; dispid 6659;
    function serialConnect(const fileName: WideString; useCrc: WordBool): IErrorCode; dispid 6660;
    property Protocol: IProtocol readonly dispid 6661;
    function isEmpty: WordBool; dispid 6662;
    function isConnected: WordBool; dispid 6663;
    procedure disconnect; dispid 6664;
    function interfaceQueue: IInterfaceQueue; dispid 6665;
    procedure queueNotifyAll; dispid 6666;
    function supportsWrite: WordBool; dispid 6667;
    function getReportCountLengths: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6668;
    function isSupported(ReportId: Byte): WordBool; dispid 6669;
    function getStatus: IStatus; dispid 6670;
    procedure reset; dispid 6671;
    function getInformation: IInformation; dispid 6672;
    function getCapability: ICapability; dispid 6673;
    function getUid: SYSUINT; dispid 6674;
    procedure setUid(uid: SYSUINT); dispid 6675;
    function getHostPublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6676;
    function getDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6677;
    procedure startCapture(sessionId: SYSUINT); dispid 6678;
    procedure endCapture; dispid 6679;
    function getDHprime: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6680;
    procedure setDHprime(dhPrime: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6681;
    function getDHbase: {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6682;
    procedure setDHbase(dhBase: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6683;
    procedure setClearScreen; dispid 6684;
    function getInkingMode: Byte; dispid 6685;
    procedure setInkingMode(inkingMode: Byte); dispid 6686;
    function getInkThreshold: IInkThreshold; dispid 6687;
    procedure setInkThreshold(const inkThreshold: IInkThreshold); dispid 6688;
    procedure writeImage(encodingMode: Byte; imageData: {NOT_OLEAUTO(PSafeArray)}OleVariant); dispid 6689;
    procedure endImageData; dispid 6690;
    function getHandwritingThicknessColor: IHandwritingThicknessColor; dispid 6691;
    procedure setHandwritingThicknessColor(const handwritingThicknessColor: IHandwritingThicknessColor); dispid 6692;
    function getBackgroundColor: Word; dispid 6693;
    procedure setBackgroundColor(backgroundColor: Word); dispid 6694;
    function getHandwritingDisplayArea: IRectangle; dispid 6695;
    procedure setHandwritingDisplayArea(const handwritingDisplayArea: IRectangle); dispid 6696;
    function getBacklightBrightness: Word; dispid 6697;
    procedure setBacklightBrightness(backlightBrightness: Word); dispid 6698;
    function getPenDataOptionMode: Byte; dispid 6699;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); dispid 6700;
  end;

// *********************************************************************//
// Interface: IJScript
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200072FE-6F48-44D0-8C0A-4ABAE07EDFF6}
// *********************************************************************//
  IJScript = interface(IDispatch)
    ['{200072FE-6F48-44D0-8C0A-4ABAE07EDFF6}']
    function toArray(value: PSafeArray): IDispatch; safecall;
    function toVBArray(const value: IDispatch): PSafeArray; safecall;
    function toTabletEncryptionHandler(const value: IDispatch): ITabletEncryptionHandler; safecall;
  end;

// *********************************************************************//
// DispIntf:  IJScriptDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {200072FE-6F48-44D0-8C0A-4ABAE07EDFF6}
// *********************************************************************//
  IJScriptDisp = dispinterface
    ['{200072FE-6F48-44D0-8C0A-4ABAE07EDFF6}']
    function toArray(value: {NOT_OLEAUTO(PSafeArray)}OleVariant): IDispatch; dispid 6401;
    function toVBArray(const value: IDispatch): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6402;
    function toTabletEncryptionHandler(const value: IDispatch): ITabletEncryptionHandler; dispid 6403;
  end;

// *********************************************************************//
// Interface: IJScript2
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000ED38-870A-49CD-A576-64940283D155}
// *********************************************************************//
  IJScript2 = interface(IJScript)
    ['{2000ED38-870A-49CD-A576-64940283D155}']
    function toTabletEncryptionHandler2(const value: IDispatch): ITabletEncryptionHandler2; safecall;
  end;

// *********************************************************************//
// DispIntf:  IJScript2Disp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000ED38-870A-49CD-A576-64940283D155}
// *********************************************************************//
  IJScript2Disp = dispinterface
    ['{2000ED38-870A-49CD-A576-64940283D155}']
    function toTabletEncryptionHandler2(const value: IDispatch): ITabletEncryptionHandler2; dispid 6404;
    function toArray(value: {NOT_OLEAUTO(PSafeArray)}OleVariant): IDispatch; dispid 6401;
    function toVBArray(const value: IDispatch): {NOT_OLEAUTO(PSafeArray)}OleVariant; dispid 6402;
    function toTabletEncryptionHandler(const value: IDispatch): ITabletEncryptionHandler; dispid 6403;
  end;

// *********************************************************************//
// Interface: IComponentFile
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005C8A-5280-4C4D-8565-DED24A3672F5}
// *********************************************************************//
  IComponentFile = interface(IDispatch)
    ['{20005C8A-5280-4C4D-8565-DED24A3672F5}']
    function Get_name: WideString; safecall;
    function Get_version: WideString; safecall;
    property name: WideString read Get_name;
    property version: WideString read Get_version;
  end;

// *********************************************************************//
// DispIntf:  IComponentFileDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005C8A-5280-4C4D-8565-DED24A3672F5}
// *********************************************************************//
  IComponentFileDisp = dispinterface
    ['{20005C8A-5280-4C4D-8565-DED24A3672F5}']
    property name: WideString readonly dispid 8449;
    property version: WideString readonly dispid 8450;
  end;

// *********************************************************************//
// Interface: IComponentFiles
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000A6E2-D773-4829-9B61-78DAC3CCAD82}
// *********************************************************************//
  IComponentFiles = interface(IDispatch)
    ['{2000A6E2-D773-4829-9B61-78DAC3CCAD82}']
    function Get__NewEnum: IUnknown; safecall;
    function Get_Item(index: OleVariant): IComponentFile; safecall;
    function Get_Count: Integer; safecall;
    property _NewEnum: IUnknown read Get__NewEnum;
    property Item[index: OleVariant]: IComponentFile read Get_Item; default;
    property Count: Integer read Get_Count;
  end;

// *********************************************************************//
// DispIntf:  IComponentFilesDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {2000A6E2-D773-4829-9B61-78DAC3CCAD82}
// *********************************************************************//
  IComponentFilesDisp = dispinterface
    ['{2000A6E2-D773-4829-9B61-78DAC3CCAD82}']
    property _NewEnum: IUnknown readonly dispid -4;
    property Item[index: OleVariant]: IComponentFile readonly dispid 0; default;
    property Count: Integer readonly dispid 8193;
  end;

// *********************************************************************//
// Interface: IComponent
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001D33-3D2C-4E4D-AFE8-5E5D446637FD}
// *********************************************************************//
  IComponent = interface(IDispatch)
    ['{20001D33-3D2C-4E4D-AFE8-5E5D446637FD}']
    function getProperty(const name: WideString): OleVariant; safecall;
    procedure setProperty(const name: WideString; value: OleVariant); safecall;
    function componentFiles: IComponentFiles; safecall;
    function diagnosticInformation(flag: SYSUINT): WideString; safecall;
  end;

// *********************************************************************//
// DispIntf:  IComponentDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20001D33-3D2C-4E4D-AFE8-5E5D446637FD}
// *********************************************************************//
  IComponentDisp = dispinterface
    ['{20001D33-3D2C-4E4D-AFE8-5E5D446637FD}']
    function getProperty(const name: WideString): OleVariant; dispid 7937;
    procedure setProperty(const name: WideString; value: OleVariant); dispid 7938;
    function componentFiles: IComponentFiles; dispid 7939;
    function diagnosticInformation(flag: SYSUINT): WideString; dispid 7940;
  end;

// *********************************************************************//
// DispIntf:  IInterfaceEvents
// Flags:     (4224) NonExtensible Dispatchable
// GUID:      {2000A1EE-8F0B-4027-B84E-A8172950CF0C}
// *********************************************************************//
  IInterfaceEvents = dispinterface
    ['{2000A1EE-8F0B-4027-B84E-A8172950CF0C}']
    function onReport(const report: IReport): HResult; dispid 1537;
  end;

// *********************************************************************//
// DispIntf:  IReportHandlerEvents
// Flags:     (4224) NonExtensible Dispatchable
// GUID:      {20004D65-E291-4B63-8062-7E99EBD21BDF}
// *********************************************************************//
  IReportHandlerEvents = dispinterface
    ['{20004D65-E291-4B63-8062-7E99EBD21BDF}']
    function onPenData(const penData: IPenData): HResult; dispid 5377;
    function onPenDataOption(const penDataOption: IPenDataOption): HResult; dispid 5378;
    function onPenDataEncrypted(const penDataEncrypted: IPenDataEncrypted): HResult; dispid 5379;
    function onPenDataEncryptedOption(const penDataEncryptedOption: IPenDataEncryptedOption): HResult; dispid 5380;
    function onDevicePublicKey(devicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 5381;
    function onDecrypt(const data: IDecrypt): HResult; dispid 5382;
  end;

// *********************************************************************//
// DispIntf:  IReportHandlerEvents2
// Flags:     (4224) NonExtensible Dispatchable
// GUID:      {2000595F-4D16-4988-8D37-9FE46084E26F}
// *********************************************************************//
  IReportHandlerEvents2 = dispinterface
    ['{2000595F-4D16-4988-8D37-9FE46084E26F}']
    function onPenData(const penData: IPenData): HResult; dispid 5377;
    function onPenDataOption(const penDataOption: IPenDataOption): HResult; dispid 5378;
    function onPenDataEncrypted(const penDataEncrypted: IPenDataEncrypted): HResult; dispid 5379;
    function onPenDataEncryptedOption(const penDataEncryptedOption: IPenDataEncryptedOption): HResult; dispid 5380;
    function onDevicePublicKey(devicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 5381;
    function onDecrypt(const data: IDecrypt): HResult; dispid 5382;
    function onPenDataTimeCountSequence(const penDataTimeCountSequence: IPenDataTimeCountSequence): HResult; dispid 5383;
    function onPenDataTimeCountSequenceEncrypted(const penDataTimeCountSequenceEncrypted: IPenDataTimeCountSequenceEncrypted): HResult; dispid 5384;
    function onEncryptionStatus(const encryptionStatus: IEncryptionStatus): HResult; dispid 5385;
  end;

// *********************************************************************//
// Interface: ITabletEventsException
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005380-DA13-492A-A97F-EE5811B49B1C}
// *********************************************************************//
  ITabletEventsException = interface(IDispatch)
    ['{20005380-DA13-492A-A97F-EE5811B49B1C}']
    procedure getException; safecall;
  end;

// *********************************************************************//
// DispIntf:  ITabletEventsExceptionDisp
// Flags:     (4544) Dual NonExtensible OleAutomation Dispatchable
// GUID:      {20005380-DA13-492A-A97F-EE5811B49B1C}
// *********************************************************************//
  ITabletEventsExceptionDisp = dispinterface
    ['{20005380-DA13-492A-A97F-EE5811B49B1C}']
    procedure getException; dispid 7425;
  end;

// *********************************************************************//
// DispIntf:  ITabletEvents
// Flags:     (4224) NonExtensible Dispatchable
// GUID:      {2000C72A-E338-477E-B359-D6D00006D29E}
// *********************************************************************//
  ITabletEvents = dispinterface
    ['{2000C72A-E338-477E-B359-D6D00006D29E}']
    function onGetReportException(const pException: ITabletEventsException): HResult; dispid 6913;
    function onUnhandledReportData(pData: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 6914;
    function onPenData(const pPenData: IPenData): HResult; dispid 6915;
    function onPenDataOption(const pPenDataOption: IPenDataOption): HResult; dispid 6916;
    function onPenDataEncrypted(const pPenDataEncrypted: IPenDataEncrypted): HResult; dispid 6917;
    function onPenDataEncryptedOption(const pPenDataEncryptedOption: IPenDataEncryptedOption): HResult; dispid 6918;
    function onDevicePublicKey(pDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 6919;
  end;

// *********************************************************************//
// DispIntf:  ITabletEvents2
// Flags:     (4224) NonExtensible Dispatchable
// GUID:      {2000CDC6-621B-453D-89E6-46C2BEB326C6}
// *********************************************************************//
  ITabletEvents2 = dispinterface
    ['{2000CDC6-621B-453D-89E6-46C2BEB326C6}']
    function onGetReportException(const pException: ITabletEventsException): HResult; dispid 6913;
    function onUnhandledReportData(pData: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 6914;
    function onPenData(const pPenData: IPenData): HResult; dispid 6915;
    function onPenDataOption(const pPenDataOption: IPenDataOption): HResult; dispid 6916;
    function onPenDataEncrypted(const pPenDataEncrypted: IPenDataEncrypted): HResult; dispid 6917;
    function onPenDataEncryptedOption(const pPenDataEncryptedOption: IPenDataEncryptedOption): HResult; dispid 6918;
    function onDevicePublicKey(pDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant): HResult; dispid 6919;
    function onPenDataTimeCountSequence(const pPenDataTimeCountSequence: IPenDataTimeCountSequence): HResult; dispid 6921;
    function onPenDataTimeCountSequenceEncrypted(const pPenDataTimeCountSequenceEncrypted: IPenDataTimeCountSequenceEncrypted): HResult; dispid 6922;
    function onEncryptionStatus(const pEncryptionStatus: IEncryptionStatus): HResult; dispid 6923;
  end;

// *********************************************************************//
// The Class CoUsbInterface provides a Create and CreateRemote method to          
// create instances of the default interface IUsbInterface4 exposed by              
// the CoClass UsbInterface. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUsbInterface = class
    class function Create: IUsbInterface4;
    class function CreateRemote(const MachineName: string): IUsbInterface4;
  end;

  TUsbInterfaceonReport = procedure(ASender: TObject; const report: IReport) of object;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TUsbInterface
// Help String      : 
// Default Interface: IUsbInterface4
// Def. Intf. DISP? : No
// Event   Interface: IInterfaceEvents
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TUsbInterface = class(TOleServer)
  private
    FOnonReport: TUsbInterfaceonReport;
    FIntf: IUsbInterface4;
    function GetDefaultInterface: IUsbInterface4;
  protected
    procedure InitServerData; override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IUsbInterface4);
    procedure Disconnect; override;
    function connect1(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode;
    function connect2(const fileName: WideString; const bulkFileName: WideString; 
                      exclusiveLock: WordBool): IErrorCode;
    property DefaultInterface: IUsbInterface4 read GetDefaultInterface;
  published
    property OnonReport: TUsbInterfaceonReport read FOnonReport write FOnonReport;
  end;

// *********************************************************************//
// The Class CoSerialInterface provides a Create and CreateRemote method to          
// create instances of the default interface ISerialInterface4 exposed by              
// the CoClass SerialInterface. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoSerialInterface = class
    class function Create: ISerialInterface4;
    class function CreateRemote(const MachineName: string): ISerialInterface4;
  end;

  TSerialInterfaceonReport = procedure(ASender: TObject; const report: IReport) of object;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TSerialInterface
// Help String      : 
// Default Interface: ISerialInterface4
// Def. Intf. DISP? : No
// Event   Interface: IInterfaceEvents
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TSerialInterface = class(TOleServer)
  private
    FOnonReport: TSerialInterfaceonReport;
    FIntf: ISerialInterface4;
    function GetDefaultInterface: ISerialInterface4;
  protected
    procedure InitServerData; override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: ISerialInterface4);
    procedure Disconnect; override;
    function connect1(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode;
    property DefaultInterface: ISerialInterface4 read GetDefaultInterface;
  published
    property OnonReport: TSerialInterfaceonReport read FOnonReport write FOnonReport;
  end;

// *********************************************************************//
// The Class CoUsbDevices provides a Create and CreateRemote method to          
// create instances of the default interface IUsbDevices exposed by              
// the CoClass UsbDevices. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUsbDevices = class
    class function Create: IUsbDevices;
    class function CreateRemote(const MachineName: string): IUsbDevices;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TUsbDevices
// Help String      : 
// Default Interface: IUsbDevices
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TUsbDevices = class(TOleServer)
  private
    FIntf: IUsbDevices;
    function GetDefaultInterface: IUsbDevices;
  protected
    procedure InitServerData; override;
    function Get__NewEnum: IUnknown;
    function Get_Item(index: OleVariant): IUsbDevice;
    function Get_Count: Integer;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IUsbDevices);
    procedure Disconnect; override;
    property DefaultInterface: IUsbDevices read GetDefaultInterface;
    property _NewEnum: IUnknown read Get__NewEnum;
    property Item[index: OleVariant]: IUsbDevice read Get_Item; default;
    property Count: Integer read Get_Count;
  published
  end;

// *********************************************************************//
// The Class CoJScript provides a Create and CreateRemote method to          
// create instances of the default interface IJScript2 exposed by              
// the CoClass JScript. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoJScript = class
    class function Create: IJScript2;
    class function CreateRemote(const MachineName: string): IJScript2;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TJScript
// Help String      : 
// Default Interface: IJScript2
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TJScript = class(TOleServer)
  private
    FIntf: IJScript2;
    function GetDefaultInterface: IJScript2;
  protected
    procedure InitServerData; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IJScript2);
    procedure Disconnect; override;
    function toTabletEncryptionHandler2(const value: IDispatch): ITabletEncryptionHandler2;
    property DefaultInterface: IJScript2 read GetDefaultInterface;
  published
  end;

// *********************************************************************//
// The Class CoProtocolHelper provides a Create and CreateRemote method to          
// create instances of the default interface IProtocolHelper5 exposed by              
// the CoClass ProtocolHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoProtocolHelper = class
    class function Create: IProtocolHelper5;
    class function CreateRemote(const MachineName: string): IProtocolHelper5;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TProtocolHelper
// Help String      : 
// Default Interface: IProtocolHelper5
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TProtocolHelper = class(TOleServer)
  private
    FIntf: IProtocolHelper5;
    function GetDefaultInterface: IProtocolHelper5;
  protected
    procedure InitServerData; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IProtocolHelper5);
    procedure Disconnect; override;
    procedure writeImage_(const Protocol: IProtocol; encodingMode: Byte; maxImageBlockSize: Word; 
                          imageData: PSafeArray; retries: SYSUINT; sleepBetweenRetries: SYSUINT);
    procedure writeImageArea_(const Protocol: IProtocol2; encodingMode: Byte; 
                              const area: IRectangle; maxImageBlockSize: Word; data: PSafeArray; 
                              retries: SYSUINT; sleepBetweenRetries: SYSUINT);
    property DefaultInterface: IProtocolHelper5 read GetDefaultInterface;
    function simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; safecall;
    function resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT;
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word;
                               screenHeight: Word; encodingMode: Byte; Scale: Scale;
                               backgroundColor: OleVariant; Clip: Byte): PSafeArray;
  published
  end;

// *********************************************************************//
// The Class CoReportHandler provides a Create and CreateRemote method to          
// create instances of the default interface IReportHandler exposed by              
// the CoClass ReportHandler. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoReportHandler = class
    class function Create: IReportHandler;
    class function CreateRemote(const MachineName: string): IReportHandler;
  end;

  TReportHandleronPenData = procedure(ASender: TObject; const penData: IPenData) of object;
  TReportHandleronPenDataOption = procedure(ASender: TObject; const penDataOption: IPenDataOption) of object;
  TReportHandleronPenDataEncrypted = procedure(ASender: TObject; const penDataEncrypted: IPenDataEncrypted) of object;
  TReportHandleronPenDataEncryptedOption = procedure(ASender: TObject; const penDataEncryptedOption: IPenDataEncryptedOption) of object;
  TReportHandleronDevicePublicKey = procedure(ASender: TObject; devicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant) of object;
  TReportHandleronDecrypt = procedure(ASender: TObject; const data: IDecrypt) of object;
  TReportHandleronPenDataTimeCountSequence = procedure(ASender: TObject; const penDataTimeCountSequence: IPenDataTimeCountSequence) of object;
  TReportHandleronPenDataTimeCountSequenceEncrypted = procedure(ASender: TObject; const penDataTimeCountSequenceEncrypted: IPenDataTimeCountSequenceEncrypted) of object;
  TReportHandleronEncryptionStatus = procedure(ASender: TObject; const encryptionStatus: IEncryptionStatus) of object;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TReportHandler
// Help String      : 
// Default Interface: IReportHandler
// Def. Intf. DISP? : No
// Event   Interface: IReportHandlerEvents2
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TReportHandler = class(TOleServer)
  private
    FOnonPenData: TReportHandleronPenData;
    FOnonPenDataOption: TReportHandleronPenDataOption;
    FOnonPenDataEncrypted: TReportHandleronPenDataEncrypted;
    FOnonPenDataEncryptedOption: TReportHandleronPenDataEncryptedOption;
    FOnonDevicePublicKey: TReportHandleronDevicePublicKey;
    FOnonDecrypt: TReportHandleronDecrypt;
    FOnonPenDataTimeCountSequence: TReportHandleronPenDataTimeCountSequence;
    FOnonPenDataTimeCountSequenceEncrypted: TReportHandleronPenDataTimeCountSequenceEncrypted;
    FOnonEncryptionStatus: TReportHandleronEncryptionStatus;
    FIntf: IReportHandler;
    function GetDefaultInterface: IReportHandler;
  protected
    procedure InitServerData; override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IReportHandler);
    procedure Disconnect; override;
    function handleReport(data: PSafeArray): WordBool;
    property DefaultInterface: IReportHandler read GetDefaultInterface;
  published
    property OnonPenData: TReportHandleronPenData read FOnonPenData write FOnonPenData;
    property OnonPenDataOption: TReportHandleronPenDataOption read FOnonPenDataOption write FOnonPenDataOption;
    property OnonPenDataEncrypted: TReportHandleronPenDataEncrypted read FOnonPenDataEncrypted write FOnonPenDataEncrypted;
    property OnonPenDataEncryptedOption: TReportHandleronPenDataEncryptedOption read FOnonPenDataEncryptedOption write FOnonPenDataEncryptedOption;
    property OnonDevicePublicKey: TReportHandleronDevicePublicKey read FOnonDevicePublicKey write FOnonDevicePublicKey;
    property OnonDecrypt: TReportHandleronDecrypt read FOnonDecrypt write FOnonDecrypt;
    property OnonPenDataTimeCountSequence: TReportHandleronPenDataTimeCountSequence read FOnonPenDataTimeCountSequence write FOnonPenDataTimeCountSequence;
    property OnonPenDataTimeCountSequenceEncrypted: TReportHandleronPenDataTimeCountSequenceEncrypted read FOnonPenDataTimeCountSequenceEncrypted write FOnonPenDataTimeCountSequenceEncrypted;
    property OnonEncryptionStatus: TReportHandleronEncryptionStatus read FOnonEncryptionStatus write FOnonEncryptionStatus;
  end;

// *********************************************************************//
// The Class CoTablet provides a Create and CreateRemote method to
// create instances of the default interface ITablet4 exposed by              
// the CoClass Tablet. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoTablet = class
    class function Create: ITablet4;
    class function CreateRemote(const MachineName: string): ITablet4;
  end;

  TTabletonGetReportException = procedure(ASender: TObject; const pException: ITabletEventsException) of object;
  TTabletonUnhandledReportData = procedure(ASender: TObject; pData: {NOT_OLEAUTO(PSafeArray)}OleVariant) of object;
  TTabletonPenData = procedure(ASender: TObject; const pPenData: IPenData) of object;
  TTabletonPenDataOption = procedure(ASender: TObject; const pPenDataOption: IPenDataOption) of object;
  TTabletonPenDataEncrypted = procedure(ASender: TObject; const pPenDataEncrypted: IPenDataEncrypted) of object;
  TTabletonPenDataEncryptedOption = procedure(ASender: TObject; const pPenDataEncryptedOption: IPenDataEncryptedOption) of object;
  TTabletonDevicePublicKey = procedure(ASender: TObject; pDevicePublicKey: {NOT_OLEAUTO(PSafeArray)}OleVariant) of object;
  TTabletonPenDataTimeCountSequence = procedure(ASender: TObject; const pPenDataTimeCountSequence: IPenDataTimeCountSequence) of object;
  TTabletonPenDataTimeCountSequenceEncrypted = procedure(ASender: TObject; const pPenDataTimeCountSequenceEncrypted: IPenDataTimeCountSequenceEncrypted) of object;
  TTabletonEncryptionStatus = procedure(ASender: TObject; const pEncryptionStatus: IEncryptionStatus) of object;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TTablet
// Help String      : 
// Default Interface: ITablet4
// Def. Intf. DISP? : No
// Event   Interface: ITabletEvents2
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TTablet = class(TOleServer)
  private
    FOnonGetReportException: TTabletonGetReportException;
    FOnonUnhandledReportData: TTabletonUnhandledReportData;
    FOnonPenData: TTabletonPenData;
    FOnonPenDataOption: TTabletonPenDataOption;
    FOnonPenDataEncrypted: TTabletonPenDataEncrypted;
    FOnonPenDataEncryptedOption: TTabletonPenDataEncryptedOption;
    FOnonDevicePublicKey: TTabletonDevicePublicKey;
    FOnonPenDataTimeCountSequence: TTabletonPenDataTimeCountSequence;
    FOnonPenDataTimeCountSequenceEncrypted: TTabletonPenDataTimeCountSequenceEncrypted;
    FOnonEncryptionStatus: TTabletonEncryptionStatus;
    FIntf: ITablet4;
    function GetDefaultInterface: ITablet4;
  protected
    procedure InitServerData; override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
    function Get_Protocol___: IProtocol4;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: ITablet4);
    procedure Disconnect; override;
    function usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode;
    function usbConnect2(const fileName: WideString; const bulkFileName: WideString;
                         exclusiveLock: WordBool): IErrorCode;
    function serialConnect_(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode;
    function isEmpty: WordBool;
    function isConnected: WordBool;
    procedure disconnect1;
    function interfaceQueue: IInterfaceQueue;
    procedure queueNotifyAll;
    function supportsWrite: WordBool;
    function getInformation: IInformation;
    function getCapability: ICapability;
    procedure setInkingMode(inkingMode: Byte);
    procedure writeImage(encodingMode: Byte; imageData: PSafeArray);
    procedure setClearScreen;
    function getHidInformation: IHidInformation;
    function getDefaultMode: Byte;
    procedure setDefaultMode(defaultMode: Byte);
    function getReportRate: Byte;
    procedure setReportRate(reportRate: Byte);
    function getReportSizeCollection: PSafeArray;
    property DefaultInterface: ITablet4 read GetDefaultInterface;
    property Protocol___: IProtocol4 read Get_Protocol___;
    function getPenDataOptionMode: Byte; safecall;
    procedure setPenDataOptionMode(penDataOptionMode: Byte); safecall;
    function getProductId: Word;
  published
    property OnonGetReportException: TTabletonGetReportException read FOnonGetReportException write FOnonGetReportException;
    property OnonUnhandledReportData: TTabletonUnhandledReportData read FOnonUnhandledReportData write FOnonUnhandledReportData;
    property OnonPenData: TTabletonPenData read FOnonPenData write FOnonPenData;
    property OnonPenDataOption: TTabletonPenDataOption read FOnonPenDataOption write FOnonPenDataOption;
    property OnonPenDataEncrypted: TTabletonPenDataEncrypted read FOnonPenDataEncrypted write FOnonPenDataEncrypted;
    property OnonPenDataEncryptedOption: TTabletonPenDataEncryptedOption read FOnonPenDataEncryptedOption write FOnonPenDataEncryptedOption;
    property OnonDevicePublicKey: TTabletonDevicePublicKey read FOnonDevicePublicKey write FOnonDevicePublicKey;
    property OnonPenDataTimeCountSequence: TTabletonPenDataTimeCountSequence read FOnonPenDataTimeCountSequence write FOnonPenDataTimeCountSequence;
    property OnonPenDataTimeCountSequenceEncrypted: TTabletonPenDataTimeCountSequenceEncrypted read FOnonPenDataTimeCountSequenceEncrypted write FOnonPenDataTimeCountSequenceEncrypted;
    property OnonEncryptionStatus: TTabletonEncryptionStatus read FOnonEncryptionStatus write FOnonEncryptionStatus;
  end;

// *********************************************************************//
// The Class CoComponent provides a Create and CreateRemote method to          
// create instances of the default interface IComponent exposed by              
// the CoClass Component. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoComponent = class
    class function Create: IComponent;
    class function CreateRemote(const MachineName: string): IComponent;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TComponent
// Help String      : 
// Default Interface: IComponent
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TTabletComponent = class(TOleServer)
  private
    FIntf: IComponent;
    function GetDefaultInterface: IComponent;
  protected
    procedure InitServerData; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IComponent);
    procedure Disconnect; override;
    function getProperty(const name: WideString): OleVariant;
    procedure setProperty(const name: WideString; value: OleVariant);
    function componentFiles: IComponentFiles;
    function diagnosticInformation(flag: SYSUINT): WideString;
    property DefaultInterface: IComponent read GetDefaultInterface;
  published
  end;

// *********************************************************************//
// The Class CoencryptionCommand provides a Create and CreateRemote method to          
// create instances of the default interface IEncryptionCommand exposed by              
// the CoClass encryptionCommand. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoencryptionCommand = class
    class function Create: IEncryptionCommand;
    class function CreateRemote(const MachineName: string): IEncryptionCommand;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TencryptionCommand
// Help String      : 
// Default Interface: IEncryptionCommand
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TencryptionCommand = class(TOleServer)
  private
    FIntf: IEncryptionCommand;
    function GetDefaultInterface: IEncryptionCommand;
  protected
    procedure InitServerData; override;
    procedure Set_encryptionCommandNumber(pRetVal: Byte);
    function Get_encryptionCommandNumber: Byte;
    procedure Set_parameter(pRetVal: Byte);
    function Get_parameter: Byte;
    procedure Set_lengthOrIndex(pRetVal: Byte);
    function Get_lengthOrIndex: Byte;
    procedure Set_data(pRetVal: PSafeArray);
    function Get_data: PSafeArray;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IEncryptionCommand);
    procedure Disconnect; override;
    procedure initializeSetEncryptionType(symmetricKeyType: Byte; asymmetricPaddingType: Byte; 
                                          asymmetricKeyType: Byte);
    procedure initializeSetParameterBlock(encryptionCommandParameterBlockIndex: Byte; 
                                          data: PSafeArray);
    procedure initializeGenerateSymmetricKey;
    procedure initializeGetParameterBlock(encryptionCommandParameterBlockIndex: Byte; offset: Byte);
    property DefaultInterface: IEncryptionCommand read GetDefaultInterface;
    property encryptionCommandNumber: Byte read Get_encryptionCommandNumber write Set_encryptionCommandNumber;
    property parameter: Byte read Get_parameter write Set_parameter;
    property lengthOrIndex: Byte read Get_lengthOrIndex write Set_lengthOrIndex;
    property data: PSafeArray read Get_data write Set_data;
  published
  end;

// *********************************************************************//
// The Class CoRectangle provides a Create and CreateRemote method to          
// create instances of the default interface IRectangle exposed by              
// the CoClass Rectangle. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoRectangle = class
    class function Create: IRectangle;
    class function CreateRemote(const MachineName: string): IRectangle;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TRectangle
// Help String      : 
// Default Interface: IRectangle
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TRectangle = class(TOleServer)
  private
    FIntf: IRectangle;
    function GetDefaultInterface: IRectangle;
  protected
    procedure InitServerData; override;
    procedure Set_upperLeftXPixel(pRetVal: Word);
    function Get_upperLeftXPixel: Word;
    procedure Set_upperLeftYPixel(pRetVal: Word);
    function Get_upperLeftYPixel: Word;
    procedure Set_lowerRightXPixel(pRetVal: Word);
    function Get_lowerRightXPixel: Word;
    procedure Set_lowerRightYPixel(pRetVal: Word);
    function Get_lowerRightYPixel: Word;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IRectangle);
    procedure Disconnect; override;
    property DefaultInterface: IRectangle read GetDefaultInterface;
    property upperLeftXPixel: Word read Get_upperLeftXPixel write Set_upperLeftXPixel;
    property upperLeftYPixel: Word read Get_upperLeftYPixel write Set_upperLeftYPixel;
    property lowerRightXPixel: Word read Get_lowerRightXPixel write Set_lowerRightXPixel;
    property lowerRightYPixel: Word read Get_lowerRightYPixel write Set_lowerRightYPixel;
  published
  end;

// *********************************************************************//
// The Class CoinkThreshold provides a Create and CreateRemote method to          
// create instances of the default interface IInkThreshold exposed by              
// the CoClass inkThreshold. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoinkThreshold = class
    class function Create: IInkThreshold;
    class function CreateRemote(const MachineName: string): IInkThreshold;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TinkThreshold
// Help String      : 
// Default Interface: IInkThreshold
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TinkThreshold = class(TOleServer)
  private
    FIntf: IInkThreshold;
    function GetDefaultInterface: IInkThreshold;
  protected
    procedure InitServerData; override;
    procedure Set_onPressureMark(pRetVal: Word);
    function Get_onPressureMark: Word;
    procedure Set_offPressureMark(pRetVal: Word);
    function Get_offPressureMark: Word;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IInkThreshold);
    procedure Disconnect; override;
    property DefaultInterface: IInkThreshold read GetDefaultInterface;
    property onPressureMark: Word read Get_onPressureMark write Set_onPressureMark;
    property offPressureMark: Word read Get_offPressureMark write Set_offPressureMark;
  published
  end;

// *********************************************************************//
// The Class CohandwritingThicknessColor provides a Create and CreateRemote method to          
// create instances of the default interface IHandwritingThicknessColor exposed by              
// the CoClass handwritingThicknessColor. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CohandwritingThicknessColor = class
    class function Create: IHandwritingThicknessColor;
    class function CreateRemote(const MachineName: string): IHandwritingThicknessColor;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : ThandwritingThicknessColor
// Help String      : 
// Default Interface: IHandwritingThicknessColor
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  ThandwritingThicknessColor = class(TOleServer)
  private
    FIntf: IHandwritingThicknessColor;
    function GetDefaultInterface: IHandwritingThicknessColor;
  protected
    procedure InitServerData; override;
    procedure Set_penColor(pRetVal: Word);
    function Get_penColor: Word;
    procedure Set_penThickness(pRetVal: Byte);
    function Get_penThickness: Byte;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IHandwritingThicknessColor);
    procedure Disconnect; override;
    property DefaultInterface: IHandwritingThicknessColor read GetDefaultInterface;
    property penColor: Word read Get_penColor write Set_penColor;
    property penThickness: Byte read Get_penThickness write Set_penThickness;
  published
  end;

// *********************************************************************//
// The Class CoHandwritingThicknessColor24 provides a Create and CreateRemote method to          
// create instances of the default interface IHandwritingThicknessColor24 exposed by              
// the CoClass HandwritingThicknessColor24. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoHandwritingThicknessColor24 = class
    class function Create: IHandwritingThicknessColor24;
    class function CreateRemote(const MachineName: string): IHandwritingThicknessColor24;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : THandwritingThicknessColor24
// Help String      : 
// Default Interface: IHandwritingThicknessColor24
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  THandwritingThicknessColor24 = class(TOleServer)
  private
    FIntf: IHandwritingThicknessColor24;
    function GetDefaultInterface: IHandwritingThicknessColor24;
  protected
    procedure InitServerData; override;
    procedure Set_penColor(pRetVal: SYSUINT);
    function Get_penColor: SYSUINT;
    procedure Set_penThickness(pRetVal: Byte);
    function Get_penThickness: Byte;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IHandwritingThicknessColor24);
    procedure Disconnect; override;
    property DefaultInterface: IHandwritingThicknessColor24 read GetDefaultInterface;
    property penColor: SYSUINT read Get_penColor write Set_penColor;
    property penThickness: Byte read Get_penThickness write Set_penThickness;
  published
  end;

// *********************************************************************//
// The Class CoInkingState provides a Create and CreateRemote method to          
// create instances of the default interface IInkingState2 exposed by              
// the CoClass InkingState. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoInkingState = class
    class function Create: IInkingState2;
    class function CreateRemote(const MachineName: string): IInkingState2;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TInkingState
// Help String      : 
// Default Interface: IInkingState2
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
  TInkingState = class(TOleServer)
  private
    FIntf: IInkingState2;
    function GetDefaultInterface: IInkingState2;
  protected
    procedure InitServerData; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IInkingState2);
    procedure Disconnect; override;
    function nextState_(rdy: WordBool; pressure: Word): InkState;
    property DefaultInterface: IInkingState2 read GetDefaultInterface;
  published
  end;

procedure Register;

resourcestring
  dtlServerPage = 'ActiveX';

  dtlOcxPage = 'ActiveX';

implementation

uses System.Win.ComObj;

class function CoUsbInterface.Create: IUsbInterface4;
begin
  Result := CreateComObject(CLASS_UsbInterface) as IUsbInterface4;
end;

class function CoUsbInterface.CreateRemote(const MachineName: string): IUsbInterface4;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UsbInterface) as IUsbInterface4;
end;

procedure TUsbInterface.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000BC66-5735-45C7-9521-93FAAFE8C3BB}';
    IntfIID:   '{2000249E-4DC7-40EA-BE44-A963011493EC}';
    EventIID:  '{2000A1EE-8F0B-4027-B84E-A8172950CF0C}';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TUsbInterface.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    ConnectEvents(punk);
    Fintf:= punk as IUsbInterface4;
  end;
end;

procedure TUsbInterface.ConnectTo(svrIntf: IUsbInterface4);
begin
  Disconnect;
  FIntf := svrIntf;
  ConnectEvents(FIntf);
end;

procedure TUsbInterface.DisConnect;
begin
  if Fintf <> nil then
  begin
    DisconnectEvents(FIntf);
    FIntf := nil;
  end;
end;

function TUsbInterface.GetDefaultInterface: IUsbInterface4;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TUsbInterface.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TUsbInterface.Destroy;
begin
  inherited Destroy;
end;

procedure TUsbInterface.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
begin
  case DispID of
    -1: Exit;  // DISPID_UNKNOWN
    1537: if Assigned(FOnonReport) then
         FOnonReport(Self, IUnknown(TVarData(Params[0]).VPointer) as IReport {const IReport});
  end; {case DispID}
end;

function TUsbInterface.connect1(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode;
begin
  Result := DefaultInterface.connect(usbDevice, exclusiveLock);
end;

function TUsbInterface.connect2(const fileName: WideString; const bulkFileName: WideString; 
                                exclusiveLock: WordBool): IErrorCode;
begin
  Result := DefaultInterface.connect2(fileName, bulkFileName, exclusiveLock);
end;

class function CoSerialInterface.Create: ISerialInterface4;
begin
  Result := CreateComObject(CLASS_SerialInterface) as ISerialInterface4;
end;

class function CoSerialInterface.CreateRemote(const MachineName: string): ISerialInterface4;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_SerialInterface) as ISerialInterface4;
end;

procedure TSerialInterface.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20001CFF-184E-4E32-9FC4-A617533D342E}';
    IntfIID:   '{2000C576-80D3-4C78-A3FC-80C203080A6B}';
    EventIID:  '{2000A1EE-8F0B-4027-B84E-A8172950CF0C}';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TSerialInterface.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    ConnectEvents(punk);
    Fintf:= punk as ISerialInterface4;
  end;
end;

procedure TSerialInterface.ConnectTo(svrIntf: ISerialInterface4);
begin
  Disconnect;
  FIntf := svrIntf;
  ConnectEvents(FIntf);
end;

procedure TSerialInterface.DisConnect;
begin
  if Fintf <> nil then
  begin
    DisconnectEvents(FIntf);
    FIntf := nil;
  end;
end;

function TSerialInterface.GetDefaultInterface: ISerialInterface4;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TSerialInterface.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TSerialInterface.Destroy;
begin
  inherited Destroy;
end;

procedure TSerialInterface.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
begin
  case DispID of
    -1: Exit;  // DISPID_UNKNOWN
    1537: if Assigned(FOnonReport) then
         FOnonReport(Self, IUnknown(TVarData(Params[0]).VPointer) as IReport {const IReport});
  end; {case DispID}
end;

function TSerialInterface.connect1(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode;
begin
  Result := DefaultInterface.connect(fileName, BaudRate, useCrc);
end;

class function CoUsbDevices.Create: IUsbDevices;
begin
  Result := CreateComObject(CLASS_UsbDevices) as IUsbDevices;
end;

class function CoUsbDevices.CreateRemote(const MachineName: string): IUsbDevices;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UsbDevices) as IUsbDevices;
end;

procedure TUsbDevices.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000D7A5-64F7-4826-B56E-85ACC618E4D6}';
    IntfIID:   '{20008CC2-6576-4665-8411-99291CF18387}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TUsbDevices.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IUsbDevices;
  end;
end;

procedure TUsbDevices.ConnectTo(svrIntf: IUsbDevices);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TUsbDevices.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TUsbDevices.GetDefaultInterface: IUsbDevices;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TUsbDevices.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TUsbDevices.Destroy;
begin
  inherited Destroy;
end;

function TUsbDevices.Get__NewEnum: IUnknown;
begin
  Result := DefaultInterface._NewEnum;
end;

function TUsbDevices.Get_Item(index: OleVariant): IUsbDevice;
begin
  Result := DefaultInterface.Item[index];
end;

function TUsbDevices.Get_Count: Integer;
begin
  Result := DefaultInterface.Count;
end;

class function CoJScript.Create: IJScript2;
begin
  Result := CreateComObject(CLASS_JScript) as IJScript2;
end;

class function CoJScript.CreateRemote(const MachineName: string): IJScript2;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_JScript) as IJScript2;
end;

procedure TJScript.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000D408-6AFE-4CBA-BDB1-DA087DA66B05}';
    IntfIID:   '{2000ED38-870A-49CD-A576-64940283D155}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TJScript.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IJScript2;
  end;
end;

procedure TJScript.ConnectTo(svrIntf: IJScript2);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TJScript.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TJScript.GetDefaultInterface: IJScript2;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TJScript.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TJScript.Destroy;
begin
  inherited Destroy;
end;

function TJScript.toTabletEncryptionHandler2(const value: IDispatch): ITabletEncryptionHandler2;
begin
  Result := DefaultInterface.toTabletEncryptionHandler2(value);
end;

class function CoProtocolHelper.Create: IProtocolHelper5;
begin
  Result := CreateComObject(CLASS_ProtocolHelper) as IProtocolHelper5;
end;

class function CoProtocolHelper.CreateRemote(const MachineName: string): IProtocolHelper5;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ProtocolHelper) as IProtocolHelper5;
end;

procedure TProtocolHelper.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000CF89-E182-40AD-A8C2-5694987EA5DF}';
    IntfIID:   '{200014AC-4DBF-40A9-9312-090B8A8EE289}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TProtocolHelper.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IProtocolHelper5;
  end;
end;

procedure TProtocolHelper.ConnectTo(svrIntf: IProtocolHelper5);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TProtocolHelper.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TProtocolHelper.GetDefaultInterface: IProtocolHelper5;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TProtocolHelper.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TProtocolHelper.Destroy;
begin
  inherited Destroy;
end;

procedure TProtocolHelper.writeImage_(const Protocol: IProtocol; encodingMode: Byte; 
                                      maxImageBlockSize: Word; imageData: PSafeArray; 
                                      retries: SYSUINT; sleepBetweenRetries: SYSUINT);
begin
  DefaultInterface.writeImage_(Protocol, encodingMode, maxImageBlockSize, imageData, retries, 
                               sleepBetweenRetries);
end;

procedure TProtocolHelper.writeImageArea_(const Protocol: IProtocol2; encodingMode: Byte; 
                                          const area: IRectangle; maxImageBlockSize: Word; 
                                          data: PSafeArray; retries: SYSUINT; 
                                          sleepBetweenRetries: SYSUINT);
begin
  DefaultInterface.writeImageArea_(Protocol, encodingMode, area, maxImageBlockSize, data, retries, 
                                   sleepBetweenRetries);
end;

function TProtocolHelper.simulateEncodingFlag(idProduct: Word; encodingFlag: Byte): Byte; safecall;
begin
  Result := DefaultInterface.simulateEncodingFlag(idProduct, encodingFlag);
end;

function TProtocolHelper.resizeAndFlatten(image: OleVariant; offsetX: SYSUINT; offsetY: SYSUINT;
                               bitmapWidth: SYSUINT; bitmapHeight: SYSUINT; screenWidth: Word;
                               screenHeight: Word; encodingMode: Byte; Scale: Scale;
                               backgroundColor: OleVariant; Clip: Byte): PSafeArray;
begin
  Result := DefaultInterface.resizeAndFlatten_(image, offsetX, offsetY, bitmapWidth, bitmapHeight, screenWidth, screenHeight, encodingMode, Scale, backgroundColor, Clip);
end;

class function CoReportHandler.Create: IReportHandler;
begin
  Result := CreateComObject(CLASS_ReportHandler) as IReportHandler;
end;

class function CoReportHandler.CreateRemote(const MachineName: string): IReportHandler;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ReportHandler) as IReportHandler;
end;

procedure TReportHandler.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20008BA5-8133-45BA-91A5-AB54A1F70E13}';
    IntfIID:   '{2000446E-736E-4513-9751-A9B69648863E}';
    EventIID:  '{2000595F-4D16-4988-8D37-9FE46084E26F}';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TReportHandler.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    ConnectEvents(punk);
    Fintf:= punk as IReportHandler;
  end;
end;

procedure TReportHandler.ConnectTo(svrIntf: IReportHandler);
begin
  Disconnect;
  FIntf := svrIntf;
  ConnectEvents(FIntf);
end;

procedure TReportHandler.DisConnect;
begin
  if Fintf <> nil then
  begin
    DisconnectEvents(FIntf);
    FIntf := nil;
  end;
end;

function TReportHandler.GetDefaultInterface: IReportHandler;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TReportHandler.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TReportHandler.Destroy;
begin
  inherited Destroy;
end;

procedure TReportHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
begin
  case DispID of
    -1: Exit;  // DISPID_UNKNOWN
    5377: if Assigned(FOnonPenData) then
         FOnonPenData(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenData {const IPenData});
    5378: if Assigned(FOnonPenDataOption) then
         FOnonPenDataOption(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataOption {const IPenDataOption});
    5379: if Assigned(FOnonPenDataEncrypted) then
         FOnonPenDataEncrypted(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataEncrypted {const IPenDataEncrypted});
    5380: if Assigned(FOnonPenDataEncryptedOption) then
         FOnonPenDataEncryptedOption(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataEncryptedOption {const IPenDataEncryptedOption});
    5381: if Assigned(FOnonDevicePublicKey) then
         FOnonDevicePublicKey(Self, Params[0] { NOT_OLEAUTO(PSafeArray) OleVariant});
    5382: if Assigned(FOnonDecrypt) then
         FOnonDecrypt(Self, IUnknown(TVarData(Params[0]).VPointer) as IDecrypt {const IDecrypt});
    5383: if Assigned(FOnonPenDataTimeCountSequence) then
         FOnonPenDataTimeCountSequence(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataTimeCountSequence {const IPenDataTimeCountSequence});
    5384: if Assigned(FOnonPenDataTimeCountSequenceEncrypted) then
         FOnonPenDataTimeCountSequenceEncrypted(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataTimeCountSequenceEncrypted {const IPenDataTimeCountSequenceEncrypted});
    5385: if Assigned(FOnonEncryptionStatus) then
         FOnonEncryptionStatus(Self, IUnknown(TVarData(Params[0]).VPointer) as IEncryptionStatus {const IEncryptionStatus});
  end; {case DispID}
end;

function TReportHandler.handleReport(data: PSafeArray): WordBool;
begin
  Result := DefaultInterface.handleReport(data);
end;

class function CoTablet.Create: ITablet4;
begin
  Result := CreateComObject(CLASS_Tablet) as ITablet4;
end;

class function CoTablet.CreateRemote(const MachineName: string): ITablet4;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Tablet) as ITablet4;
end;

procedure TTablet.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20002178-1165-4D38-A7F5-B169DE2045C1}';
    IntfIID:   '{20001376-687D-4684-AA8B-ADB60A929216}';
    EventIID:  '{2000CDC6-621B-453D-89E6-46C2BEB326C6}';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TTablet.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    ConnectEvents(punk);
    Fintf:= punk as ITablet4;
  end;
end;

procedure TTablet.ConnectTo(svrIntf: ITablet4);
begin
  Disconnect;
  FIntf := svrIntf;
  ConnectEvents(FIntf);
end;

procedure TTablet.DisConnect;
begin
  if Fintf <> nil then
  begin
    DisconnectEvents(FIntf);
    FIntf := nil;
  end;
end;

function TTablet.usbConnect(const usbDevice: IUsbDevice; exclusiveLock: WordBool): IErrorCode;
begin
  Result := DefaultInterface.usbConnect(usbDevice, exclusiveLock);
end;

function TTablet.usbConnect2(const fileName: WideString; const bulkFileName: WideString;
                             exclusiveLock: WordBool): IErrorCode;
begin
  Result := DefaultInterface.usbConnect2(fileName, bulkFileName, exclusiveLock);
end;

function TTablet.isEmpty: WordBool;
begin
  Result := DefaultInterface.isEmpty;
end;

function TTablet.isConnected: WordBool;
begin
  Result := DefaultInterface.isConnected;
end;

procedure TTablet.disconnect1;
begin
  DefaultInterface.disconnect;
end;

function TTablet.interfaceQueue: IInterfaceQueue;
begin
  Result := DefaultInterface.interfaceQueue;
end;

procedure TTablet.queueNotifyAll;
begin
  DefaultInterface.queueNotifyAll;
end;

function TTablet.supportsWrite: WordBool;
begin
  Result := DefaultInterface.supportsWrite;
end;

function TTablet.getInformation: IInformation;
begin
  Result := DefaultInterface.getInformation;
end;

function TTablet.getCapability: ICapability;
begin
  Result := DefaultInterface.getCapability;
end;

procedure TTablet.setInkingMode(inkingMode: Byte);
begin
  DefaultInterface.setInkingMode(inkingMode);
end;

procedure TTablet.writeImage(encodingMode: Byte; imageData: PSafeArray);
begin
  DefaultInterface.writeImage(encodingMode, imageData);
end;

procedure TTablet.setClearScreen;
begin
  DefaultInterface.setClearScreen;
end;

function TTablet.GetDefaultInterface: ITablet4;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TTablet.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TTablet.Destroy;
begin
  inherited Destroy;
end;

procedure TTablet.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
begin
  case DispID of
    -1: Exit;  // DISPID_UNKNOWN
    6913: if Assigned(FOnonGetReportException) then
         FOnonGetReportException(Self, IUnknown(TVarData(Params[0]).VPointer) as ITabletEventsException {const ITabletEventsException});
    6914: if Assigned(FOnonUnhandledReportData) then
         FOnonUnhandledReportData(Self, Params[0] { NOT_OLEAUTO(PSafeArray) OleVariant});
    6915: if Assigned(FOnonPenData) then
         FOnonPenData(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenData {const IPenData});
    6916: if Assigned(FOnonPenDataOption) then
         FOnonPenDataOption(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataOption {const IPenDataOption});
    6917: if Assigned(FOnonPenDataEncrypted) then
         FOnonPenDataEncrypted(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataEncrypted {const IPenDataEncrypted});
    6918: if Assigned(FOnonPenDataEncryptedOption) then
         FOnonPenDataEncryptedOption(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataEncryptedOption {const IPenDataEncryptedOption});
    6919: if Assigned(FOnonDevicePublicKey) then
         FOnonDevicePublicKey(Self, Params[0] { NOT_OLEAUTO(PSafeArray) OleVariant});
    6921: if Assigned(FOnonPenDataTimeCountSequence) then
         FOnonPenDataTimeCountSequence(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataTimeCountSequence {const IPenDataTimeCountSequence});
    6922: if Assigned(FOnonPenDataTimeCountSequenceEncrypted) then
         FOnonPenDataTimeCountSequenceEncrypted(Self, IUnknown(TVarData(Params[0]).VPointer) as IPenDataTimeCountSequenceEncrypted {const IPenDataTimeCountSequenceEncrypted});
    6923: if Assigned(FOnonEncryptionStatus) then
         FOnonEncryptionStatus(Self, IUnknown(TVarData(Params[0]).VPointer) as IEncryptionStatus {const IEncryptionStatus});
  end; {case DispID}
end;

function TTablet.Get_Protocol___: IProtocol4;
begin
  Result := DefaultInterface.Protocol___;
end;

function TTablet.serialConnect_(const fileName: WideString; BaudRate: SYSUINT; useCrc: WordBool): IErrorCode;
begin
  Result := DefaultInterface.serialConnect_(fileName, BaudRate, useCrc);
end;

function TTablet.getHidInformation: IHidInformation;
begin
  Result := DefaultInterface.getHidInformation;
end;

function TTablet.getDefaultMode: Byte;
begin
  Result := DefaultInterface.getDefaultMode;
end;

procedure TTablet.setDefaultMode(defaultMode: Byte);
begin
  DefaultInterface.setDefaultMode(defaultMode);
end;

function TTablet.getReportRate: Byte;
begin
  Result := DefaultInterface.getReportRate;
end;

procedure TTablet.setReportRate(reportRate: Byte);
begin
  DefaultInterface.setReportRate(reportRate);
end;

function TTablet.getReportSizeCollection: PSafeArray;
begin
  Result := DefaultInterface.getReportSizeCollection;
end;

function TTablet.getPenDataOptionMode: Byte;
begin
  Result := DefaultInterface.getPenDataOptionMode;
end;

procedure TTablet.setPenDataOptionMode(penDataOptionMode: Byte); safecall;
begin
  DefaultInterface.setPenDataOptionMode(penDataOptionMode);
end;

function TTablet.getProductId: Word;
begin
  Result := DefaultInterface.getProductId;
end;

class function CoComponent.Create: IComponent;
begin
  Result := CreateComObject(CLASS_Component) as IComponent;
end;

class function CoComponent.CreateRemote(const MachineName: string): IComponent;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Component) as IComponent;
end;

procedure TTabletComponent.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20000000-5D96-46B0-895F-B0CC7295E44D}';
    IntfIID:   '{20001D33-3D2C-4E4D-AFE8-5E5D446637FD}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TTabletComponent.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IComponent;
  end;
end;

procedure TTabletComponent.ConnectTo(svrIntf: IComponent);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TTabletComponent.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TTabletComponent.GetDefaultInterface: IComponent;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TTabletComponent.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TTabletComponent.Destroy;
begin
  inherited Destroy;
end;

function TTabletComponent.getProperty(const name: WideString): OleVariant;
begin
  Result := DefaultInterface.getProperty(name);
end;

procedure TTabletComponent.setProperty(const name: WideString; value: OleVariant);
begin
  DefaultInterface.setProperty(name, value);
end;

function TTabletComponent.componentFiles: IComponentFiles;
begin
  Result := DefaultInterface.componentFiles;
end;

function TTabletComponent.diagnosticInformation(flag: SYSUINT): WideString;
begin
  Result := DefaultInterface.diagnosticInformation(flag);
end;

class function CoencryptionCommand.Create: IEncryptionCommand;
begin
  Result := CreateComObject(CLASS_encryptionCommand) as IEncryptionCommand;
end;

class function CoencryptionCommand.CreateRemote(const MachineName: string): IEncryptionCommand;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_encryptionCommand) as IEncryptionCommand;
end;

procedure TencryptionCommand.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000B45D-344F-4FA7-8FDD-6C957FC499F2}';
    IntfIID:   '{2000D7D6-AF25-4A06-ACDE-2CCAC21149D0}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TencryptionCommand.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IEncryptionCommand;
  end;
end;

procedure TencryptionCommand.ConnectTo(svrIntf: IEncryptionCommand);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TencryptionCommand.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TencryptionCommand.GetDefaultInterface: IEncryptionCommand;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TencryptionCommand.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TencryptionCommand.Destroy;
begin
  inherited Destroy;
end;

procedure TencryptionCommand.Set_encryptionCommandNumber(pRetVal: Byte);
begin
  DefaultInterface.encryptionCommandNumber := pRetVal;
end;

function TencryptionCommand.Get_encryptionCommandNumber: Byte;
begin
  Result := DefaultInterface.encryptionCommandNumber;
end;

procedure TencryptionCommand.Set_parameter(pRetVal: Byte);
begin
  DefaultInterface.parameter := pRetVal;
end;

function TencryptionCommand.Get_parameter: Byte;
begin
  Result := DefaultInterface.parameter;
end;

procedure TencryptionCommand.Set_lengthOrIndex(pRetVal: Byte);
begin
  DefaultInterface.lengthOrIndex := pRetVal;
end;

function TencryptionCommand.Get_lengthOrIndex: Byte;
begin
  Result := DefaultInterface.lengthOrIndex;
end;

procedure TencryptionCommand.Set_data(pRetVal: PSafeArray);
begin
  DefaultInterface.data := pRetVal;
end;

function TencryptionCommand.Get_data: PSafeArray;
begin
  Result := DefaultInterface.data;
end;

procedure TencryptionCommand.initializeSetEncryptionType(symmetricKeyType: Byte; 
                                                         asymmetricPaddingType: Byte; 
                                                         asymmetricKeyType: Byte);
begin
  DefaultInterface.initializeSetEncryptionType(symmetricKeyType, asymmetricPaddingType, 
                                               asymmetricKeyType);
end;

procedure TencryptionCommand.initializeSetParameterBlock(encryptionCommandParameterBlockIndex: Byte; 
                                                         data: PSafeArray);
begin
  DefaultInterface.initializeSetParameterBlock(encryptionCommandParameterBlockIndex, data);
end;

procedure TencryptionCommand.initializeGenerateSymmetricKey;
begin
  DefaultInterface.initializeGenerateSymmetricKey;
end;

procedure TencryptionCommand.initializeGetParameterBlock(encryptionCommandParameterBlockIndex: Byte; 
                                                         offset: Byte);
begin
  DefaultInterface.initializeGetParameterBlock(encryptionCommandParameterBlockIndex, offset);
end;

class function CoRectangle.Create: IRectangle;
begin
  Result := CreateComObject(CLASS_Rectangle) as IRectangle;
end;

class function CoRectangle.CreateRemote(const MachineName: string): IRectangle;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Rectangle) as IRectangle;
end;

procedure TRectangle.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{200077CB-B6D7-4C0E-97EA-EBBB35051EF4}';
    IntfIID:   '{2000518C-05F0-4D88-B867-C5C9D58C04C5}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TRectangle.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IRectangle;
  end;
end;

procedure TRectangle.ConnectTo(svrIntf: IRectangle);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TRectangle.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TRectangle.GetDefaultInterface: IRectangle;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TRectangle.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TRectangle.Destroy;
begin
  inherited Destroy;
end;

procedure TRectangle.Set_upperLeftXPixel(pRetVal: Word);
begin
  DefaultInterface.upperLeftXPixel := pRetVal;
end;

function TRectangle.Get_upperLeftXPixel: Word;
begin
  Result := DefaultInterface.upperLeftXPixel;
end;

procedure TRectangle.Set_upperLeftYPixel(pRetVal: Word);
begin
  DefaultInterface.upperLeftYPixel := pRetVal;
end;

function TRectangle.Get_upperLeftYPixel: Word;
begin
  Result := DefaultInterface.upperLeftYPixel;
end;

procedure TRectangle.Set_lowerRightXPixel(pRetVal: Word);
begin
  DefaultInterface.lowerRightXPixel := pRetVal;
end;

function TRectangle.Get_lowerRightXPixel: Word;
begin
  Result := DefaultInterface.lowerRightXPixel;
end;

procedure TRectangle.Set_lowerRightYPixel(pRetVal: Word);
begin
  DefaultInterface.lowerRightYPixel := pRetVal;
end;

function TRectangle.Get_lowerRightYPixel: Word;
begin
  Result := DefaultInterface.lowerRightYPixel;
end;

class function CoinkThreshold.Create: IInkThreshold;
begin
  Result := CreateComObject(CLASS_inkThreshold) as IInkThreshold;
end;

class function CoinkThreshold.CreateRemote(const MachineName: string): IInkThreshold;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_inkThreshold) as IInkThreshold;
end;

procedure TinkThreshold.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20002981-FEBA-4438-BE91-043FFA45CC94}';
    IntfIID:   '{20003425-E218-45D6-B3E2-961C74364617}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TinkThreshold.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IInkThreshold;
  end;
end;

procedure TinkThreshold.ConnectTo(svrIntf: IInkThreshold);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TinkThreshold.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TinkThreshold.GetDefaultInterface: IInkThreshold;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TinkThreshold.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TinkThreshold.Destroy;
begin
  inherited Destroy;
end;

procedure TinkThreshold.Set_onPressureMark(pRetVal: Word);
begin
  DefaultInterface.onPressureMark := pRetVal;
end;

function TinkThreshold.Get_onPressureMark: Word;
begin
  Result := DefaultInterface.onPressureMark;
end;

procedure TinkThreshold.Set_offPressureMark(pRetVal: Word);
begin
  DefaultInterface.offPressureMark := pRetVal;
end;

function TinkThreshold.Get_offPressureMark: Word;
begin
  Result := DefaultInterface.offPressureMark;
end;

class function CohandwritingThicknessColor.Create: IHandwritingThicknessColor;
begin
  Result := CreateComObject(CLASS_handwritingThicknessColor) as IHandwritingThicknessColor;
end;

class function CohandwritingThicknessColor.CreateRemote(const MachineName: string): IHandwritingThicknessColor;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_handwritingThicknessColor) as IHandwritingThicknessColor;
end;

procedure ThandwritingThicknessColor.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20003105-1669-49A9-8DE9-80E1792D6D6A}';
    IntfIID:   '{20004323-9CF7-4375-B7C3-DEFDF8588FAD}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure ThandwritingThicknessColor.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IHandwritingThicknessColor;
  end;
end;

procedure ThandwritingThicknessColor.ConnectTo(svrIntf: IHandwritingThicknessColor);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure ThandwritingThicknessColor.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function ThandwritingThicknessColor.GetDefaultInterface: IHandwritingThicknessColor;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor ThandwritingThicknessColor.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor ThandwritingThicknessColor.Destroy;
begin
  inherited Destroy;
end;

procedure ThandwritingThicknessColor.Set_penColor(pRetVal: Word);
begin
  DefaultInterface.penColor := pRetVal;
end;

function ThandwritingThicknessColor.Get_penColor: Word;
begin
  Result := DefaultInterface.penColor;
end;

procedure ThandwritingThicknessColor.Set_penThickness(pRetVal: Byte);
begin
  DefaultInterface.penThickness := pRetVal;
end;

function ThandwritingThicknessColor.Get_penThickness: Byte;
begin
  Result := DefaultInterface.penThickness;
end;

class function CoHandwritingThicknessColor24.Create: IHandwritingThicknessColor24;
begin
  Result := CreateComObject(CLASS_HandwritingThicknessColor24) as IHandwritingThicknessColor24;
end;

class function CoHandwritingThicknessColor24.CreateRemote(const MachineName: string): IHandwritingThicknessColor24;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_HandwritingThicknessColor24) as IHandwritingThicknessColor24;
end;

procedure THandwritingThicknessColor24.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{20006BFE-4AF1-4F88-8AE0-D87CBBA30CD0}';
    IntfIID:   '{2000FBAB-36A0-479B-B264-AAF5DCB9CCFA}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure THandwritingThicknessColor24.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IHandwritingThicknessColor24;
  end;
end;

procedure THandwritingThicknessColor24.ConnectTo(svrIntf: IHandwritingThicknessColor24);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure THandwritingThicknessColor24.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function THandwritingThicknessColor24.GetDefaultInterface: IHandwritingThicknessColor24;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor THandwritingThicknessColor24.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor THandwritingThicknessColor24.Destroy;
begin
  inherited Destroy;
end;

procedure THandwritingThicknessColor24.Set_penColor(pRetVal: SYSUINT);
begin
  DefaultInterface.penColor := pRetVal;
end;

function THandwritingThicknessColor24.Get_penColor: SYSUINT;
begin
  Result := DefaultInterface.penColor;
end;

procedure THandwritingThicknessColor24.Set_penThickness(pRetVal: Byte);
begin
  DefaultInterface.penThickness := pRetVal;
end;

function THandwritingThicknessColor24.Get_penThickness: Byte;
begin
  Result := DefaultInterface.penThickness;
end;

class function CoInkingState.Create: IInkingState2;
begin
  Result := CreateComObject(CLASS_InkingState) as IInkingState2;
end;

class function CoInkingState.CreateRemote(const MachineName: string): IInkingState2;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_InkingState) as IInkingState2;
end;

procedure TInkingState.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{2000A253-30B8-4B7D-87CF-623FBF39B0FE}';
    IntfIID:   '{200026D4-2949-411F-BD21-8D01E5DF518C}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TInkingState.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IInkingState2;
  end;
end;

procedure TInkingState.ConnectTo(svrIntf: IInkingState2);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TInkingState.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TInkingState.GetDefaultInterface: IInkingState2;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call "Connect" or "ConnectTo" before this operation');
  Result := FIntf;
end;

constructor TInkingState.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
end;

destructor TInkingState.Destroy;
begin
  inherited Destroy;
end;

function TInkingState.nextState_(rdy: WordBool; pressure: Word): InkState;
begin
  Result := DefaultInterface.nextState_(rdy, pressure);
end;

procedure Register;
begin
  RegisterComponents(dtlServerPage, [TUsbInterface, TSerialInterface, TUsbDevices, TJScript, 
    TProtocolHelper, TReportHandler, TTablet, TComponent, TencryptionCommand, 
    TRectangle, TinkThreshold, ThandwritingThicknessColor, THandwritingThicknessColor24, TInkingState]);
end;

end.
