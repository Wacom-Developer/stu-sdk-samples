# Wacom Device Kit - STU SDK Samples

The samples included here are primarily for Windows.

The Java samples are also used by Linux and additional build tools and scripts are included in the SDK installation.

## Overview

### HTML

Internet Explorer demo project (IE9 or above is required for canvas support).
Other browsers are not supported - please see the SigCaptX samples instead.
The HTML samples rely on the use of ActiveX which, for security reasons, is not supported on current browsers.
However the legacy Internet Explorer product is usually included in a Windows installation and can be used as a convenient platform to explore the SDK.


**DemoButtons**

    DemoButtons.html    - The sample demonstrates a signature capture application.
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture
                            
                          The sample generates the tablet display image dynamically.
                          Encryption handlers are included. 

**demoTandC**

    demoTandC.html    - The sample demonstrates a signature capture application from a colour STU such as the 530 or 540.
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture
                            
                          The sample generates the tablet display from predefined images (PNG files).
                          Encryption handlers are included.


**SendToSTU**

    SendToSTU.html      - The sample demonstrates sending an image file to the STU display
    SendToSTU-Area.html - The sample demonstrates updating an area of the STU-530 display

### Java

Java demo projects

**DemoButtons**

    DemoButtons.java    - The sample demonstrates a signature capture application.
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture
                            
    Run-DemoButtons.bat - The batch file can be used to build and run the demo with JDK 7
                          from a Dos command prompt

**DemoButtonsSerial**

    DemoButtonsSerial.java  - The sample demonstrates a signature capture application using a serial connection to the STU
                              The demo displays a signature capture window with buttons on the PC and the tablet display:
                                OK     - accepts the signature
                                Clear  - clears the display
                                Cancel - cancels signature capture
                            
    Run-DemoButtonsSerial.bat - The batch file can be used to build and run the demo with JDK
                                 from a Dos command prompt


### csharp

MS Visual Studio C# demo projects
NB: before compiling the project check that Interop.wgssSTU.dll matches the installed SDK  version

**DemoButtons**

    DemoButtons.sln     - The sample demonstrates a signature capture application.
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture
                            
    
**DemoButtons (save option)**

    DemoButtons.sln     - The sample includes code to save the signature
    

### delphi

Embarcadero demo project: Delphi 7

**DemoButtons**

    DemoButtons.dpr     - The sample demonstrates a signature capture application.
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture
                            
**DemoButtons_serial**

    DemoButtons.dpr     - The sample demonstrates a signature capture application using a serial STU (e.g. 430V)
                          The demo displays a signature capture window with buttons on the PC and the tablet display:
                            OK     - accepts the signature
                            Clear  - clears the display
                            Cancel - cancels signature capture




---


