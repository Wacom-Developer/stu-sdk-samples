# STU SDK

## Version 2.15.5

## History

###   Release 2.15.5   29-May-2020
    * Rebuilt zip file

###   Release 2.15.4   21-Apr-2020
    * Samples removed from Windows installer
			
###   Release 2.15.3c  29-Oct-2019
    * Added documentation for Fedora 27 build
			
###   Release 2.15.3b  13-Sep-2019
    * Added 2.15.3 release for Linux which includes fix for the 540 and Java
			
###   Release 2.15.3   23-Aug-2019
    * Workaround for failure to initialize STU-540 correctly with Java
    * Rebuild to counteract anti-virus false positives in STU SigCaptX

###   Release 2.15.2   19-Jun-2019
    * Added OpenSSL DLLs (Windows Java folders only) to SDK installer
  
###   Release 2.15.1b  25-Mar-2019
    * Incremented SigCaptX version  
    * Fix for 64-bit Java when loading OpenSSL libraries

###   Release 2.15.1   19-Feb-2019
    * OpenSSL v1.1 
    * Fix to writeImageArea()
      
###   Release 2.14.1 2018-08-21
    * Fixes for 24-bit colour and getUID2()
      
###   Release 2.13.6 2018-02-16
    * Various Linux fixes including report lengths, libusb, makefile

###   Release 2.13.5 2018-01-16
    *  Updated installed DemoButtons sample for STU-540 
    *  Added root certificate for STU-541 certificate exchange validation
    *  Added mutex locking to Tablet class in case of multi-threaded calls

###   Release 2.13.4 2017-10-04
    *  Added Java support for STU-541
    *  Made OpenSLL DLLs load on demand (wgssSTU.dll)

###   Release 2.13.3 2017-07-04
    *  Rebuild for STU-SigCaptX
    *  Added Linux support for STU-540/541
    *  STU-541 C++ only

###   Release 2.13.1  2017-03-27
    *  Added C support for STU-540
    *  Added Java support for STU-540
    *  Fixed issues in Linux: 32-bit build for Ubuntu 12.04.05 LTS added
      
###   Release 2.13.0  2017-02-10
    *  Extended STU-540 C++ interface to C: Same functionality as the original C++ version but with a C interface
    
###   Release 2,12.0
    *  Internal build
    
###   Release 2.11.0
    *  Internal build
    
###   Release 2.10.1  2016-08-09
    *  Improved Linux support
    
###   Release 2.10.0  2016-07-14
    *  Java interface fixed for STU-530
    *  Addition of STU SigCaptX
    *  Linux fixes
    *  Addition of STU-540 interface

###   Release 2.9.0  2016-03-29
    *  Added support for Visual Studio 2015 (C++)

###   Release 2.8.0  2015-12-14
    *  Breaking change: added DefaultMode and ReportRate to all interfaces. This will break existing code written for the previous, beta release
    *  Updated thread local storage use in order to get round compatibility issue in Windows XP
    *  Added support for serial ports larger than COM9
	  
###   Release 2.7.0  2015-11-17
    *  Fixed bug sending COM image to serial STU-430V/G and STU-530V/G
    *  Added new API for C++ Win32: getSerialPorts()
    *  Added WacomGSS_Tablet_decrypt to C interface

###   Release 2.6.0  2015-11-06
    *  Fixed bug sending image to serial STU-430V/G and STU-530V/G
    *  Updated netscape plugin for Firefox 41 compatibility 

###   Release 2.5.0  2015-10-15
    *  Update to support new features of STU-430V/G and STU-530V/G
    *  Changed SerialInterface to accept baudRate parameter in connect(); existing method will default to use STU-500 baud rate
    *  Fixed errors in some C interface functions that incorrectly returned value unspecified (1) when they should have returned value success (0)

###   Release 2.4.1  2015-10-01
    *  Fix for Windows SxS operation
    *  Fix to stop Java threads when no longer needed

###   Release 2.4.0  2015-09-30
    *  Corrections to InkingState for STU-430/530
    *  Breaking change: changed ProtocolHelper InkState to match STU-530 (fw 1.02).
    *  Updated enumUsbDevices to be more robust in the case of a corrupted registry entry for a device (which will now be ignored)
    *  InkState and InkingState added to Java to mirror C++
    *  Added support for serial STU-500 on Linux
    *  Establish Windows 10 compatibility
	  
###   Release 2.3.1  2015-05-05
    *  Fixes for component registration and DLL installation.
	  
###   Release 2.3.0  2015-04-29
    *  New InkingState added to C++ and IDL (COM). Sample code has been updated in STU-SDK for C++, HTML and C#.

###   Release 2.2.1  2015-04-16
    *  STU Utilities link changed (..\support to ../support)

###   Release 2.2.1  2015-03-25
    *  STU SDK Redistribution notes added

###   Release 2.2.1  2014-10-14
    *  Fix to the Linux C++ code to prevent using the usb interface without claiming it first. 

###   Release 2.2.0  2014-10-08
    * Plugin support for browsers other than Internet Explorer, including Firefox and Chrome. (Safari is not supported)
    * Bug fixes and other minor code improvements, including:
      *  In Internet Explorer, automatically disconnects from the tablet if the page is unloaded. 
      *  COM: Fixes to the SxS manifest which prevented its use in the previous release. 
    * Known issue: the 'C' interface language binding cannot be used on Windows XP:
       * This is due to a limitation within the Microsoft Visual Studio compiler and runtime we use.
       * If this use case affects you, contact us. 

###   Release 2.1.3  2014-07-23
    Bug fixes and other minor code improvements, including:
    *  C++: Fixed issues with compilation with Visual Studio 2013 
    *  Fixed Win8.1 check for delaying startup of connect to ensure tablet is fully powered on. 
    *  Windows: Added code for retrieving error strings from ntdll and wininet dlls. 
    *  .NET: Removed digital signing in interop (as incompatible with Strong Name Key) 
    *  COM: Added UnloadHandler to Interfaces when used inside Internet Explorer. 
    *  COM: Fixed SxS manifest 
    *  C: Fixed ReportHandler failing to correctly return data for PenDataTimeCountSequence/Encrypted 
    *  C: Added missing enums from header 


###   Release 2.1.2  2014-04-25
    *  .NET: Added pre-built interop file due to a bug that prevented Visual Studio 2010 from automatically generating one
    *  COM: Fixed Tablet.writeImageArea() 
    *  COM: Fixed a memory leak in Tablet.connect()
    *  Added detection for tablets to be in low (D3) power state and wait for them to power on
    *  C: language binding updated with 2.1 feature set
    *  C++: Fixed a race condition within retrieving reports; see wait_getReport_predicate() to replace wait_getReport(). 
    
###   Release 2.1.1  2013-11-01
    *  Support for STU-430, STU-530
    *  Improvements to the Java interface (note some are breaking changes)

###   Release 2.0.0  2013-07-10
    *  Initial release
 