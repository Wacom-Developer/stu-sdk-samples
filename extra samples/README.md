
## Wacom Device Kit - STU SDK Extra Samples

The samples included here are primarily for Windows.

### Overview
---
#### HTML

A number of the HTML samples rely on the "canvas" element which is only supported on IE9 or above.  
The HTML samples all rely on the use of ActiveX which, for security reasons, is not supported on browsers other than IE.  
Please see the SigCaptX samples if you want to support browsers other than IE.  

|   Name of sample                   | Description                                                                       |
|------------------------------------|-----------------------------------------------------------------------------------|
|  demobuttons-Save                  |  Demobuttons alternative which converts the signature to a B64 text string        |
|  demobuttons_drawing               |  Draw on the STU with pen colour selections and place small images of houses      |
|  demobuttons_hwarea                |  STU 530 only - sets the handwriting area to a limited section of the pad display |
|  demobuttons_pressure              |  Monitors the pen pressure in a debug window                                      |
|  Diag/GetUsbDevices.html           |  Shows details of connected STU devices                                           |
|  Diag/Identify.html                |  Displays technical information about a connected STU device                      |
|  SendToSTU-setBrightness.html      |  Sets backlight brightness and sends an image to the pad (STU-530 only)           |
|  SignOnImage.html                  |  Displays a background image and allows user to sign on top of it                 |

#### C Sharp

|   Name of sample                | Description                                                                                                    |
|---------------------------------|----------------------------------------------------------------------------------------------------------------|
| DemoButtons_SpeedCalc           | Version of Demobuttons which calculates the average speed of the pen when signing                              |
| DemoButtons_UpsideDown          | Version of DemoButtons which inverts the capture screen display so that the STU can be reversed in orientation |
| DemoButtonsHIDSerial            | Version of DemoButtons allowing user to swap between HID and serial capture                                    |
| DemoButtons-usingEncryption     | Version of DemoButtons using encrypted pen data                                                                |
| DemoButtons-usingInkState       | Version of DemoButtons demonstrating how to use InkingState with pen pressure thresholds                       |
| SaveImage                       | Version of DemoButtons allowing user to save the signature image in JPEG, PNG, GIFF or BMP format              |
| SendToSTU                       | Enable user to select an image and send it to the STU                                                          |
| TestDemoButtons                 | C# equivalent of the standard HTML DemoButtons sample program listing                                          |
| TestDemoButtons-SxS             | DemoButtons using SxS configuration to enable the program to run without registering the DLL (wgssSTU.dll)     |

#### VB.NET

|   Name of sample                | Description                                                                                                    |
|---------------------------------|-----------------------------------------------------------------------------|
| DemoButtonsHIDSerial            | Version of DemoButtons allowing user to swap between HID and serial capture |
| SendToSTU                       | Enable user to select an image and send it to the STU                       |
| TestDemoButtons                 | VB.NET equivalent of the standard HTML DemoButtons sample program           |

