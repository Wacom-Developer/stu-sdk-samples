//
// demobuttons_area.js
//
// Displays a form with 8 buttons on the STU pad and on Windows allowing user to draw a brown, yellow or blue line
// or place images of houses on the screen.  Demonstrates use of image placing by area and setting pen colour/thickness.
//
// Copyright (c) 2015 Wacom GmbH. All rights reserved.
//
// 
                                    // enumerations copied from SDK\COM\doc with reformatted syntax
enumEncodingFlag = {                // encodingFlag reports what the STU device is capable of:
  EncodingFlag_Zlib : 0x01, 
  EncodingFlag_1bit : 0x02,         // mono
  EncodingFlag_16bit : 0x04,        // 16bit colour (520/530)
  EncodingFlag_24bit : 0x08         // 24bit colour (530)
}

enumEncodingMode = {                // selects image transformation
  EncodingMode_1bit : 0x00,         // mono display STU300/430/500
  EncodingMode_1bit_Zli0b : 0x01,    // use zlib compression (not automated by the SDK – the application code has to compress the data)
  EncodingMode_16bit : 0x02,        // colour stu-520 & 530
  EncodingMode_24bit : 0x04,        // colour STU 530
                                    // tablet.supportsWrite() is true if the bulk driver is installed and available
  EncodingMode_1bit_Bulk : 0x10,    // use bulk driver (520/530) 
  EncodingMode_16bit_Bulk : 0x12, 
  EncodingMode_24bit_Bulk : 0x14, 

  EncodingMode_Raw : 0x00, 
  EncodingMode_Zlib : 0x01, 
  EncodingMode_Bulk : 0x10, 
  EncodingMode_16bit_565 : 0x02 
}

enumScale = {
  Scale_Stretch : 0, 
  Scale_Fit : 1, 
  Scale_Clip : 2 
}
enumProductId = {
  STU_500 :  0xa1,
  STU_300 :  0xa2,
  STU_520 :  0xa3,
  STU_430 :  0xa4,
  STU_530 :  0xa5 
}

enumInkingMode = {
  InkingMode_On : 0x01,
  InkingMode_Off : 0x00
}

enumPenMode = {
  PlacingImage : 1,
  DrawingLine  : 2,
  SelectingButton : 3
}

var gDebugMode = 1;
var gImageCount = 0;

// Debug function which outputs to a text field on the HTML form
function print(txt, messageLevel) {
  if (gDebugMode >= messageLevel)
  {
    var txtDisplay = document.getElementById("txtDisplay");
    txtDisplay.value += txt + "\n";
    txtDisplay.scrollTop = txtDisplay.scrollHeight; // scroll to end
  }
}

function dec2hex(i) {
  return( "0x" + ((i<16)?"0":"") + i.toString(16).toUpperCase()); // add leading zero if < 16 e.g. 0x0F
}

function getCurrentDir() {
	var scriptFullName = window.location.pathname; // gets /c:/pathname/file.html
	scriptFullName = scriptFullName.replace(/\//g,"\\"); //convert all '/' to '\'
	var scriptPath = scriptFullName.substring( 1, scriptFullName.lastIndexOf("\\")+1 ); // c:\pathname\
	//scriptPath = unescape(scriptPath); // change %20 back to space
	return( scriptPath );
}

function SignatureForm(preview) {

  // Member variables
  var wgssSTU; //netscape plugin.
  var m_protocolHelper;
  var m_tablet;
  var m_tablet2;
  var m_capability;
  var m_information;
  var m_inkThreshold;
  var m_lastButton = 4;  // Default pen mode is writing a line
  var m_usingEncryption;

  // The isDown flag is used like this:
  // 0 = up
  // +ve = down, pressed on button number
  // -1 = down, inking
  // -2 = down, ignoring
  var m_isDown;

  var m_penData; // Array of data being stored. This can be subsequently used as desired. 
  var m_penColour; // Array of pen colours
  var m_penThickness;  // Array of pen thicknesses
  var m_btns; // The array of buttons that we are emulating.
  var m_bitmap; // This bitmap that we display on the screen.
  var m_encodingMode; // How we send the bitmap to the device.
  var m_bitmapData; // This is the flattened data of the bitmap that we send to the device.
  var m_penMode;  // If true then pen has selected an image to drop onto screen
  var m_imagesPlaced;
  var m_lineColour = 'brown';
  var m_thicknessColour;

  // Detect the browser.
  var useActiveX;
  try {
    var test = new ActiveXObject("WacomGSS.STU.UsbDevices");
    // If the above hasn't thrown, browser is IE and components are installed
    useActiveX = true;
  }
  catch (ex) {
    // Browser isn't IE or components are not installed, either way we can't use ActiveX
    useActiveX = false;
  }

  var modalBackground;
  var formDiv;
  var canvas;
  var ctx;
  var canvasImage;

  function Rectangle(x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;

    this.Contains = function (pt) {
      if (((pt.x >= this.x) && (pt.x <= (this.x + this.width))) &&
       ((pt.y >= this.y) && (pt.y <= (this.y + this.height)))) {
        return true;
      } else {
        return false;
      }
    }
  }

  // In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
  // Using an array of these makes it easy to add or remove buttons as desired.
  //  delegate void ButtonClick();
  function Button() {
    this.Bounds; // in Screen coordinates
    this.Text;
    this.Click;
	this.Image;
	this.ImageWidth;
	this.ImageHeight;
	this.ClickCount;
	this.Activated;
  };

  function ImageDetails()
  {
	this.Filename;
	this.ImageTopLeftX;
	this.ImageTopLeftY;
	this.ImageLowerRightX;
	this.ImageLowerRightY;
  }
  
  function Point(x, y) {
    this.x = x;
    this.y = y;
  }

  // Create an object maintaining the compatibility beetween browsers.
  function createObject(objectName) {
    var object;
    if (useActiveX) {
      object = new ActiveXObject(objectName);
    } else {
      if (!wgssSTU) {
        wgssSTU = document.getElementById("wgssSTU");
      }
      object = wgssSTU.createObject(objectName);

    }

    return object;
  }
  
	// Reset to zero the click counters for the buttons
	function resetButtonClickCounters(btnCounter)
	{
	  var i = 0;
	  print("Resetting button counters where current button is " + btnCounter, 1);
	  print("No of buttons to reset: " + m_btns.length, 1);
	  
	  for (i = 0; i < m_btns.length; i++)
	  {
		 // If the user has just pressed on a button then reset the counters for all the other ones
		 // but not this one. Note that btnCounter counts from 1 - 8, but i from 0 to 7
		 if (btnCounter > 0)
		 {
			if (i + 1 != btnCounter)
			{
			   //print("Resetting counter for button " + i, 1);
			   m_btns[i].ClickCount = 0;
			   m_btns[i].Activated = 0;
			}
		 }
		 else
		 {
			m_btns[i].ClickCount = 0;
		    m_btns[i].Activated = 0;
		 }
	  }
	}

  function tabletToScreen(penData) {
    // Screen means LCD screen of the tablet.
    return new Point(penData.x * m_capability.screenWidth / m_capability.tabletMaxX, penData.y * m_capability.screenHeight / m_capability.tabletMaxY);
  }

  function createModalWindow(width, height) {
    modalBackground = document.createElement('div');
    modalBackground.id = "modal-background";
    modalBackground.className = "active";
    modalBackground.style.width = window.innerWidth;
    modalBackground.style.height = window.innerHeight;
    document.getElementsByTagName('body')[0].appendChild(modalBackground);

    formDiv = document.createElement('div');
    formDiv.id = "signatureWindow";
    formDiv.className = "active";
    formDiv.style.top = (window.innerHeight / 2) - (height / 2) + "px";
    formDiv.style.left = (window.innerWidth / 2) - (width / 2) + "px";
    formDiv.style.width = width + "px";
    formDiv.style.height = height + "px";
    document.getElementsByTagName('body')[0].appendChild(formDiv);

    canvas = document.createElement("canvas");
    canvas.height = formDiv.offsetHeight;
    canvas.width = formDiv.offsetWidth;
    formDiv.appendChild(canvas);

    if (canvas.addEventListener) {
      canvas.addEventListener("click", onCanvasClick, false);
    } else if (canvas.attachEvent) {
      canvas.attachEvent("onClick", onCanvasClick);
    } else {
      canvas["onClick"] = onCanvasClick;
    }
  }

  function stringToByteArray(imageString) {
    if (imageString.indexOf("data:image/") != 0)
      return null;

    var base64Position = imageString.indexOf("base64");
    if (base64Position == -1)
      return null;

    var imageData = imageString.substring(base64Position + 7);

    // window.atob is only available in mozilla browsers and IE 10 and above.
    // if you want to use IE 9 there are differents library
    // for doing the same functionality.
    var raw;
    if (window.atob) {
      raw = window.atob(imageData);
    } else {
      //if there is an exception the browser does not support the atob function
      raw = decode64(imageData);
    }

    var rawLength = raw.length;
    var imageArray = new Array(rawLength);

    for (i = 0; i < rawLength; i++) {
      imageArray[i] = raw.charCodeAt(i);
    }

    var js = new ActiveXObject("WacomGSS.STU.JScript");
    return js.toVBArray(imageArray);
  }

  // Connect to the first device
  this.connect = function (debugMode, encryptionRequired) {

    var usbDevices = createObject("WacomGSS.STU.UsbDevices");
    var txtDisplay = document.getElementById("txtDisplay");
	
	if (debugMode > 0)
	{
	   //console.log("Displaying debug window");
	   txtDisplay.hidden = "";
	   if (gDebugMode == 0)
          gDebugMode = debugMode;
	}
    else
    {
	   gDebugMode = debugMode;
	   txtDisplay.hidden = "true";
	}
	
    if (usbDevices.Count > 0) {

      m_penData = new Array();
	  m_penColour = new Array();
	  m_penThickness = new Array();
	  
      m_tablet = createObject("WacomGSS.STU.Tablet");

      // A more sophisticated applications should cycle for a few times as the connection may only be
      // temporarily unavailable for a second or so. 
      // For example, if a background process such as Wacom STU Display
      // is running, this periodically updates a slideshow of images to the device.


      if (typeof(encryptionHandler) != "undefined")
      {
        if (useActiveX) {
          var js = new ActiveXObject("WacomGSS.STU.JScript");
          m_tablet.encryptionHandler  = js.toTabletEncryptionHandler(encryptionHandler());
          m_tablet.encryptionHandler2 = js.toTabletEncryptionHandler2(encryptionHandler2());
        }
      }

      m_protocolHelper = createObject("WacomGSS.STU.ProtocolHelper");

      var usbDevice = usbDevices.Item(0);
      var ec = m_tablet.usbConnect(usbDevice, true);
      if (ec.value == 0) {
        m_capability = m_tablet.getCapability();
        m_information = m_tablet.getInformation();
        m_inkThreshold = m_tablet.getInkThreshold();
        m_usingEncryption = false;

        if (m_tablet.isSupported(0x50) ||     // v2 encryption
            m_protocolHelper.supportsEncryption_DHprime(m_tablet.getDHprime())) // v1 encryption
			{
				if (encryptionRequired > 0)
                   m_usingEncryption = true;
			}
      } else {
        alert(ec.message);
        return;
      }
	  print ("Creating modal window", 1);
	  
	  if (setup_ButtonProperties(usbDevice))
	  {
	     // Create the signature window.
          createModalWindow(m_capability.screenWidth, m_capability.screenHeight);
	  }
	  else
	  {
	    // Set-up of buttons failed because it's the wrong pad so abandon program
	     btnCancel_Click();
	  }
	  useColor = setEncodingMode();
	  print("useColor: " + useColor, 1);

      // This application uses the same bitmap for both the screen and client (window).

      ctx = canvas.getContext("2d");

      ctx.lineWidth = 1;
      ctx.strokeStyle = 'black';
      ctx.font = "24px Arial";

      ctx.fillStyle = "white";
      ctx.fillRect(0, 0, canvas.width, canvas.height);

      // Draw the buttons
	  
      for (var i = 0; i < m_btns.length; ++i) {
        setup_Button(i, ctx, usbDevice);
	  }
	  
      ctx.stroke();

      // Now the bitmap has been created, it needs to be converted to device-native
      // format.

      canvasImage = canvas.toDataURL("image/jpeg");
      var imageData = stringToByteArray(canvasImage);
      m_bitmapData = m_protocolHelper.resizeAndFlatten(imageData, 0, 0, 0, 0, m_capability.screenWidth, m_capability.screenHeight, m_encodingMode, 1, false, 0);

      // Add the delegate that receives pen data and error.
      if (useActiveX) {
	    print("Setting up pen event trapping", 1);
        eval("function m_tablet::onPenData(data) {return onPenData(data);}");
        eval("function m_tablet::onPenDataEncrypted(data) {return onPenDataEncrypted(data);}");
        eval("function m_tablet::onGetReportException(exception) {return onGetReportException(exception);}");
      } else {
	    printf("setting up onPenData without Active X", 1);
        m_tablet.onPenData = onPenData;
        m_tablet.onPenDataEncrypted = onPenDataEncrypted;
        m_tablet.onGetReportException = onGetReportException;
      }

      // Initialize the screen
      clearScreen();

      try
      {
        if (m_usingEncryption)
        {
		  print("Capturing with encryption", 1);
		  // startCapture() enables encryption
          m_tablet.startCapture(0xc0ffee);
		  m_usingEncryption = false;
        }
      }
      catch (e)
      {
	    print("Capturing without encryption", 1);
        m_usingEncryption = false;
      }
      // Enable the pen data on the screen (if not already)
	  print ("Setting inking mode", 1)
      m_tablet.setInkingMode(0x01);
	  print ("Inking mode set to true", 1);

	  // Set up empty array of images ready for populating later as each one is placed on the tablet
	  m_imagesPlaced = new Array();
	  print("images array initialised", 1);
	  
	  // Don't forget to set the image counter back to zero in case this is not the first time through
	  gImageCount = 0;
	  
	  // Set the inking colour to brown as on the canvas
	  print("Getting colour and thickness values", 1);
	  m_thicknesscolour = m_tablet.getHandwritingThicknessColor();
	  //print("colour/thickness" + m_thicknesscolour.penColor + "/ " + m_thicknesscolour.penThickness);
	  print("Setting pen colour for button " + m_lastButton, 1);
	  setPenColourOnPad(m_lastButton);
	  
    } else {
      alert("There is no tablet present");
    }
  }

  function setPenColourOnPad(lastButton)
  {
    switch (lastButton)
	  {
	     case 4:
		      // Brown road selected
		 	  m_thicknesscolour.penColor = RGB_to_uint(24, 24, 0);
			  print("Brown line selected", 1);
			  m_thicknesscolour.penThickness = 2;
			  break;
		 case 5:
		      // Yellow road selected
		 	  m_thicknesscolour.penColor = RGB_to_uint(31, 63, 0);
			  print("Yellow line selected", 1);
			  m_thicknesscolour.penThickness = 4;
			  break;
		 case 6:
		 	  // Blue river selected
		 	  m_thicknesscolour.penColor = RGB_to_uint(12, 48, 31);
		      print("Blue line selected", 1);
			  m_thicknesscolour.penThickness = 6;
			  break;
	  }

	  print("Calling colour/thickness method for button " + lastButton, 1);
	  m_tablet.setHandwritingThicknessColor(m_thicknesscolour);
	  print("Thickness and colour now set", 1);
  }

  // Convert RGB values to unsigned int 5-6-5 for RGB values
  function RGB_to_uint(red, green, blue)
  {
    var colourValue;

    colourValue = (red << 11 | green << 5 | blue);
	print("colourValue: " + colourValue, 2);
    return colourValue;
  }
 
  // Function to set up the properties of the 6 buttons on the pad
  function setup_ButtonProperties(usbDevice)
  {
      m_btns = new Array(8);
	  for (var j = 0; j < m_btns.length; j++) {
        m_btns[j] = new Button();
	  }
 	
      if (usbDevice.idProduct == enumProductId.STU_530) {
        // Place the buttons across the bottom of the screen.
		print("Calculating width", 1);
		
		var btnWidth = m_capability.screenWidth / 8;
        var y = m_capability.screenHeight * 6 / 7;
        var h = m_capability.screenHeight - y;
        print("btnWidth " + btnWidth, 1);
		
		for (var j = 0; j < m_btns.length; j++) {
		   if (j == 0) {
           m_btns[0].Bounds = new Rectangle(0, y, btnWidth, h);
		   }
		   else  {
		     m_btns[j].Bounds = new Rectangle(btnWidth * j, y, btnWidth, h);
		   }
		   m_btns[j].ClickCount = 0;
		   m_btns[j].Activated = 0;
	    }
		
	  } else 
	  {
         // This program is only designed to work with the 530
		 alert("This sample only works with an STU-530");
		 return(0);
      }

	  // Set up the text, images and functions associated with each button
      m_btns[0].Text = "OK";
      m_btns[1].Text = "Clear";
      m_btns[2].Text = "Cancel";
	  m_btns[3].Text = "Brown road";
	  m_btns[4].Text = "Yellow road";
	  m_btns[5].Text = "River";
	  m_btns[6].Text = "Green house";
	  m_btns[7].Text = "Red house";
	  
      m_btns[0].Click = btnOk_Click;
      m_btns[1].Click = btnClear_Click;
      m_btns[2].Click = btnCancel_Click;
	  m_btns[3].Click = btn4_Click;
	  m_btns[4].Click = btn5_Click;
	  m_btns[5].Click = btn6_Click;
	  m_btns[6].Click = btn7_Click;
	  m_btns[7].Click = btn8_Click;

	  m_btns[0].Image = "None";
	  m_btns[1].Image = "None";
	  m_btns[2].Image = "None";
	  m_btns[3].Image = "BrownRoad";
	  m_btns[4].Image = "YellowRoad";
	  m_btns[5].Image = "River";
	  m_btns[6].Image = "GreenHouse";
	  m_btns[7].Image = "RedHouse";
	  
	  for (j = 0; j <= 7; j++)
	  {
	     m_btns[j].ClickCount = 0;
		 m_btns[j].Activated = 0;
	  }
	  
	  print("Initial click count for button 8: " + m_btns[7].ClickCount, 1);
	  return(1);
  }
  
  function setup_Button( i, ctx, usbDevice )
  {
	if (m_btns[i].Image != "None")
	{
	   print('Setting up image ' + m_btns[i].Image, 1);                 
	   var image = document.getElementById(m_btns[i].Image); 
	   var imageWidth = document.getElementById(m_btns[i].Image).width; 
	   var imageHeight = document.getElementById(m_btns[i].Image).height; 
	   
	   if (image != null)
	   {
	     ctx.fillStyle = "white";
	     print("Filling rectangle for button " + i, 1);
         ctx.fillRect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
		 
		 ctx.fillStyle = "black";
	     ctx.rect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
		 
	     print('Displaying image', 1);
		 var xOffset = (m_btns[i].Bounds.width - imageWidth) / 2;
		 var yOffset = (m_btns[i].Bounds.height - imageHeight) / 2;
		 ctx.drawImage(image, m_btns[i].Bounds.x + xOffset, m_btns[i].Bounds.y + yOffset, imageWidth, imageHeight );   
	   }
    }
    else
	{
      ctx.fillStyle = "white";
	  print("Filling rectangle for button " + i, 1);
      ctx.fillRect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
	  
	  ctx.fillStyle = "black";
	  ctx.rect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
	  
	  var xPos = m_btns[i].Bounds.x + ((m_btns[i].Bounds.width / 2) - (ctx.measureText(m_btns[i].Text).width / 2));
      var yOffset = 40;
	  ctx.fillText(m_btns[i].Text, xPos, m_btns[i].Bounds.y + yOffset);
    }
  }
  
  function setEncodingMode()
  {
      var encodingFlag = m_protocolHelper.simulateEncodingFlag(m_tablet.getProductId(), m_capability.encodingFlag);
      // Disable color if the bulk driver isn't installed (supportsWrite())
      if ((encodingFlag & enumEncodingFlag.EncodingFlag_24bit) != 0)
      {
        m_encodingMode = m_tablet.supportsWrite() ? enumEncodingMode.EncodingMode_24bit_Bulk : enumEncodingMode.EncodingMode_24bit; 
      }
      else if ((encodingFlag & enumEncodingFlag.EncodingFlag_16bit) != 0)
      {
        m_encodingMode = m_tablet.supportsWrite() ? enumEncodingMode.EncodingMode_16bit_Bulk : enumEncodingMode.EncodingMode_16bit; 
      }
      else
      {
        // assumes 1bit is available
        m_encodingMode = enumEncodingMode.EncodingMode_1bit; 
      }
      var useColor = false;
      if((m_encodingMode & (enumEncodingMode.EncodingMode_16bit_Bulk | enumEncodingMode.EncodingMode_24bit_Bulk)) != 0)
        useColor = true;
	  print("Encoding mode is " + m_encodingMode, 1);
	  return useColor;
  }
  
  // Function to send the contents of a bitmap image to the screen
  function clearScreen() {
    // note: There is no need to clear the tablet screen prior to writing an image.
	print("Writing image to pad", 1);
    m_tablet.writeImage(m_encodingMode, m_bitmapData);

    m_penData = new Array();
	m_penColour = new Array();
	m_penThickness = new Array();
	
    m_isDown = 0;
	m_penMode = 0;

    // repaint the background image on the screen.
    var image = new Image();
    image.onload = function () {
      ctx.drawImage(image, 0, 0);

      if (m_usingEncryption)
      {
        ctx.fillStyle = "black";
        ctx.fillText("\uD83D\uDD12", 20, 50);
      }
    }
    image.src = canvasImage;
  }

  function onPenDataEncrypted(penData) { // Process incoming encrypted pen data
    onPenData(penData.penData1);
    onPenData(penData.penData2);
  }

  
  function onPenData(penData) { // Process incoming pen data
    if (!penData.rdy)
	{
	  print("Pen not ready", 1);
      return;
	}

    var pt = tabletToScreen(penData);

	print("Pen point " + pt.x + " " + pt.y, 2);
	
    var btn = 0; // will be +ve if the pen is over a button.
	
    for (var i = 0; i < m_btns.length; ++i) {
      if (m_btns[i].Bounds.Contains(pt)) {
        btn = i + 1;
		m_btns[i].ClickCount = m_btns[i].ClickCount + 1;
		if (m_btns[i].ClickCount == 1)
		{
		   print("Resetting other button counters, btn = " + btn, 1);
	       resetButtonClickCounters(btn);
		}
	    print ("onPenData identified button " + btn + " " + m_btns[i].ClickCount + " times", 2);
		m_lastButton = btn;
		btnFound = btn;
        break;
      }
    }
    print("Pen mode:" + m_penMode + ' m_isDown: ' + m_isDown, 2);
	
	// If the pen isn't over a button then we can reset all the button counters

	if (m_isDown == 0)
    {
	  print('m_isDown == 0, m_penMode == ' + m_penMode , 2);
      var isDown = (penData.pressure > m_inkThreshold.onPressureMark);
      print("pressure " + penData.pressure, 2);
	  print("onPressureMark " + m_inkThreshold.onPressureMark + " isDown " + isDown, 2);
      if (isDown)
      {
	    print("Transition to down", 2);
        // transition to down
        if (btn > 0) 
        {
          // We have put the pen down on a button.
          // Track the pen without inking on the client.

          m_isDown = btn;
		  m_penMode = enumPenMode.SelectingButton;
        }
        else
        {
		  print("Pen placed on drawing area", 2);
          // We have put the pen down somewhere else.
          // Treat it as standard pen stroke
		  if (m_penMode == 0 || m_lastButton >= 4)
		  {
            m_isDown = -1;
			print("Choosing button action for button : " + m_lastButton, 1);
			switch(m_lastButton)
			{
				case 4: 
				case 5: 
				case 6:
					m_penMode = enumPenMode.DrawingLine;
					print("Button " + m_lastButton + " m_penMode is " + m_penMode, 1);
					break;
				case 7:
					placeImage(penData);
					break;
				case 8:
					placeImage(penData);
				    break;
				default:
					m_penMode = enumPenMode.DrawingLine;
					break;
			}
		  }
		  else
		  {
		    print("Button/mode : " + m_lastButton + ' ' + m_penMode, 2);
			if ((m_lastButton == 7 || m_lastButton == 8) && m_penMode != enumPenMode.PlacingImage)
			{
				placeImage(penData);
			}
		  }
        }
      }
      print("m_isdown " + m_isDown, 2);
      if (m_isDown == -1 && m_penMode == enumPenMode.DrawingLine)
	  {
        m_penData.push(penData);
		m_penColour.push(m_lineColour);
		m_penThickness.push(m_thicknesscolour.penThickness);
	  }
    }
    else
    {
      var isDown = !(penData.pressure <= m_inkThreshold.offPressureMark);
      print("isDown, offPressureMark, m_penMode " + isDown + " " + m_inkThreshold.offPressureMark + " " + m_penMode, 2);
      // draw
      if (m_isDown == -1 && (m_penMode == 0 || m_penMode == enumPenMode.DrawingLine)) 
      {
        // Draw a line from the previous down point to this down point.
        // This is the simplest thing you can do; a more sophisticated program
        // can perform higher quality rendering than this!

		print("Drawing line", 2);
		print("Total length : " + m_penData.length, 2);
		if (m_penData.length > 0)
		{
		 var prev = tabletToScreen(m_penData[m_penData.length - 1]);
		}
		else
		{
		 var prev = pt;
		}
       
	    // Reproduce the line on the HTML canvas
		var ctx = canvas.getContext("2d");
		if (ctx != null)
		{
		   print("Drawing to canvas", 2);
           ctx.beginPath();
		   if (m_penMode == 0)
		   {
		     ctx.lineWidth = 1;
             ctx.strokeStyle = 'black';
             ctx.font = "24px Arial";
	       }
	       else
	       {
		     ctx.lineWidth = m_thicknesscolour.penThickness;
		     ctx.strokeStyle = m_lineColour;
		     ctx.font = '30px Arial';
	        }
		   print("From x/y to x/y : " + prev.x + ' ' + prev.y + ' ' + pt.x + ' ' + pt.y, 2);
           ctx.moveTo(prev.x, prev.y);
           ctx.lineTo(pt.x, pt.y);
           ctx.closePath();
           ctx.stroke();
		}
		else
		{
		   print("Unable to obtain canvas object", 1);
		}

        m_penData.push(penData);
		m_penColour.push(m_lineColour);
		m_penThickness.push(m_thicknesscolour.penThickness);
      }
      
      if (!isDown)
      {
		print("Transition to up and pen mode is " + m_penMode, 2);
        // transition to up
        if (btn > 0) {
          // The pen is over a button

          if (btn == m_isDown) {
            // The pen was pressed down over the same button as it has now been lifted from
            // Consider that as a click!
			print("Clicked on button " + btn, 1);
			print("Clicked on button " + btn + " " + m_btns[btn-1].ClickCount + " times", 1);
			if (m_btns[btn-1].Activated == 0)
			{
			   m_btns[btn-1].Activated = 1;
               m_btns[btn-1].Click();
			}
          }
        }
		if (m_penMode != enumPenMode.DrawingLine)
		{
		  print('Resetting m_penMode to 0', 2);
		  if (m_penMode != enumPenMode.PlacingImage);
		  {
			m_penMode = 0;
		  }
		  m_penMode = 0;
		  
		  if (m_lastButton >= 7)
		  {
		     //  Don't enable inking if user has selected a house to drop on the pad
			 print("Disabling inking because user has selected a house(1)", 1);
		     m_tablet.setInkingMode(0x00);
	      }
		  else
		  {
			 print("Enabling inking", 1);
		     m_tablet.setInkingMode(0x01);
		  }
		} 
    	//m_penData = new Array();
        m_isDown = 0;
      }
    }
  }

  // Capture any report exception.
  function onGetReportException(exception) {
    try {
      exception.getException();
    } catch (e) {
      alert(e);
    }
  }

  // Generate the signature image
  function generateImage() {

    var saveImageCanvas = document.createElement("canvas");
    saveImageCanvas.width = canvas.width;
    saveImageCanvas.height = canvas.height;
    print("Height and width of test canvas: " + canvas.width + "/" + canvas.height,1 );
	
    var ctx = saveImageCanvas.getContext("2d");
    ctx.lineWidth = 2;
    ctx.strokeStyle = 'black';
    ctx.font = "30px Arial";

    ctx.fillStyle = "white";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

	print("Generating final image for pen data length: " + m_penData.length, 1);
	
    if (m_penData.length > 1)
    {
      var isDown = false;
      var prev;

      for (var i = 0; i < m_penData.length; i++) {
        // Draw a line from the previous down point to this down point.
        // This is the simplist thing you can do; a more sophisticated program
        // can perform higher quality rendering than this!

        if (!isDown)
        {
          if (m_penData[i].pressure > m_inkThreshold.onPressureMark)
          {
            isDown = true;
            prev = tabletToScreen(m_penData[i]);
			ctx.beginPath();
			ctx.lineWidth = 2;
            ctx.strokeStyle = m_penColour[i];
            ctx.moveTo(prev.x, prev.y);
          }
        }
        else 
        {
          var pd = tabletToScreen(m_penData[i]);
		  
		  print("Thickness/Colour for next line: " + m_penThickness[i] + "/" + m_penColour[i], 2);
		  ctx.lineWidth = 2;
          ctx.strokeStyle = m_penColour[i];
          ctx.lineTo(pd.x, pd.y);
		  ctx.closePath();
		  ctx.stroke();
          prev = pd;
          ctx.moveTo(prev.x, prev.y);
		  
          if (m_penData[i].pressure <= m_inkThreshold.offPressureMark)
          {
		    print("Thickness/Colour: " + m_penThickness[i] + "/" + m_penColour[i], 1);
		    ctx.lineWidth = 2;
            ctx.strokeStyle = m_penColour[i];
			ctx.closePath();
            ctx.stroke();
            isDown = false;
          }
        }
      }
    }
  
    if (isDown)
	{	  
      print("Thickness/Colour: " + m_penThickness[i] + "/" + m_penColour[i], 1);
	  ctx.lineWidth = 2;
      ctx.strokeStyle = m_penColour[i];
	  ctx.closePath();
      ctx.stroke(); 
	}
    //print("Preview width/height " + preview.width + "/" + preview.height, 1);
	
	// Now put the houses on as well
	print("Displaying images: " + gImageCount, 1);
	var houseCount = 0;
	for (houseCount = 0; houseCount < gImageCount; houseCount++)
	{
	   displayImageOnCanvas(houseCount, ctx);
	}
    preview.src = saveImageCanvas.toDataURL("image/png");
  }

  function generateLine() {

    var saveImageCanvas = document.getElementById("CanvasTest");
    saveImageCanvas.width = canvas.width;
    saveImageCanvas.height = canvas.height;

    var ctx = saveImageCanvas.getContext("2d");
    ctx.lineWidth = 3;
    ctx.strokeStyle = 'green';
    ctx.font = "30px Arial";

    ctx.fillStyle = "white";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    ctx.beginPath();
    
	// print ("Canvas height " + canvas.height);
	// print ("Canvas width " + canvas.width);
	
	// Draw left side
	drawline(ctx, 30, 50, 200, 1);
	
	// Draw right side
	drawline(ctx, 230, 50, 200, 1);
   
    // Draw top
    drawline(ctx, 30, 50, 200, 0);	
	
	// Draw bottom
	drawline(ctx, 30, 250, 200, 0);
    ctx.stroke();

    preview.src = saveImageCanvas.toDataURL("image/jpeg");
  }

  function drawline(ctx, StartX, StartY, LineLength, Down)  {
  
  ctx.moveTo(StartX, StartY);
  
  for (var i = 0; i < LineLength; i++) {
      if (Down)
        ctx.lineTo(StartX, StartY+i);
	  else
	    ctx.lineTo(StartX+i, StartY);
    }
  }
  
  function close() {
    document.getElementsByTagName('body')[0].removeChild(modalBackground);
    document.getElementsByTagName('body')[0].removeChild(formDiv);

    // Ensure that you correctly disconnect from the tablet, otherwise you are 
    // likely to get errors when wanting to connect a second time.
    if (m_tablet != null) {
	  print("Disabling inking (2)", 1);
      m_tablet.setInkingMode(0x00);
      m_tablet.setClearScreen();
      m_tablet.disconnect();
    }
  }

  function btnOk_Click() {
    // You probably want to add additional processing here.
    generateImage();
    setTimeout(function(){close();}, 1);
  }

  function btnCancel_Click() {
    // You probably want to add additional processing here.
    // setTimeout(function(){close();}, 1);
	setTimeout(function(){close();}, 1);
 }

  function btnClear_Click() {
    // You probably want to add additional processing here.
    print("Clearing screen", 1);
    clearScreen();
  }
  
  function btn4_Click() {
    setPadInking('brown', 4);
  }
  
  function btn5_Click() {
    setPadInking('yellow', 5);
  }
 
  function btn6_Click() {
    setPadInking('blue', 6);
  }
  
  function btn7_Click() {
	m_penMode = enumPenMode.SelectingButton;
	print("Green house - disabling inking (3)", 1);
	m_tablet.setInkingMode(0x00);
  }
  
  function btn8_Click() {
	m_penMode = enumPenMode.SelectingButton;
	print("Red house - disabling inking (4)", 1);
	m_tablet.setInkingMode(0x00);
  }
  
  // Enable inking on pad and set the pen colour/thickness
  function setPadInking(colour, btn)
  {
    print("Setting up inking for btn " + btn);
  	m_penMode = 4;
	m_lineColour = colour;
    print("Enabling inking", 1);
	m_tablet.setInkingMode(0x01);
	setPenColourOnPad(btn);
  }
  
  function onCanvasClick(event) {
    // Enable the mouse to click on the simulated buttons that we have displayed.

    // Note that this can add some tricky logic into processing pen data
    // if the pen was down at the time of this click, especially if the pen was logically
    // also 'pressing' a button! This demo however ignores any that.

    var posX = event.pageX - formDiv.offsetLeft;
    var posY = event.pageY - formDiv.offsetTop;

    for (var i = 0; i < m_btns.length; i++) {
      if (m_btns[i].Bounds.Contains(new Point(posX, posY))) {
        m_btns[i].Click();
        break;
      }
    }
  }

  // Only necessary for IE9
  function decode64(input) {

    var keyStr = "ABCDEFGHIJKLMNOP" +
                 "QRSTUVWXYZabcdef" +
                 "ghijklmnopqrstuv" +
                 "wxyz0123456789+/" +
                 "=";

    var output = "";
    var chr1, chr2, chr3 = "";
    var enc1, enc2, enc3, enc4 = "";
    var i = 0;

    // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
    var base64test = /[^A-Za-z0-9\+\/\=]/g;

    if (base64test.exec(input)) {
      alert("There were invalid base64 characters in the input text.\n" +
            "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
            "Expect errors in decoding.");
    }

    input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

    do {
      enc1 = keyStr.indexOf(input.charAt(i++));
      enc2 = keyStr.indexOf(input.charAt(i++));
      enc3 = keyStr.indexOf(input.charAt(i++));
      enc4 = keyStr.indexOf(input.charAt(i++));

      chr1 = (enc1 << 2) | (enc2 >> 4);
      chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
      chr3 = ((enc3 & 3) << 6) | enc4;

      output = output + String.fromCharCode(chr1);

      if (enc3 != 64) {
        output = output + String.fromCharCode(chr2);
      }

      if (enc4 != 64) {
        output = output + String.fromCharCode(chr3);
      }

      chr1 = chr2 = chr3 = "";
      enc1 = enc2 = enc3 = enc4 = "";

    } while (i < input.length);

    return unescape(output);
  }

 // Function to draw an image on the HTML canvas
 function drawImageOnCanvas(btnSelected)
 {
    print("Putting image " + btnSelected + " on canvas", 1);
	print("Finding image " + m_btns[btnSelected].Image, 1);
 	var image = document.getElementById(m_btns[btnSelected].Image); 
	if (image != null)
	{
	  print('Displaying image at ' + m_imagesPlaced[gImageCount-1].ImageTopLeftX + ' ' +  m_imagesPlaced[gImageCount-1].ImageTopLeftY, 1);
	  var ctx = canvas.getContext("2d");
      ctx.drawImage(image, m_imagesPlaced[gImageCount-1].ImageTopLeftX, m_imagesPlaced[gImageCount-1].ImageTopLeftY, m_btns[btnSelected].ImageWidth, m_btns[btnSelected].ImageHeight );   
	}
	else
	{
		print("Unable to obtain image " + m_btns[btnSelected].Image, 1);
	}
 }
  
 function displayImageOnCanvas(imageLoop, canvasContext)
 {
    print("Displaying image: " + imageLoop, 1);
	var imageName = m_imagesPlaced[imageLoop].Filename;
	
	var image = document.getElementById(imageName); 
	print("Image to display: " + imageName, 1);
	
	var topLeftX = Math.floor(m_imagesPlaced[imageLoop].ImageTopLeftX);
	var topLeftY = Math.floor(m_imagesPlaced[imageLoop].ImageTopLeftY);
	print("Top X/Y" + topLeftX + " " + topLeftY, 1);
	
	var imageWidth = Math.floor((m_imagesPlaced[imageLoop].ImageLowerRightX - m_imagesPlaced[imageLoop].ImageTopLeftX));
	var imageHeight = Math.floor((m_imagesPlaced[imageLoop].ImageLowerRightY - m_imagesPlaced[imageLoop].ImageTopLeftY));
	
	print("Width/Height: " + imageWidth + " " + imageHeight, 1);
	if (image != null)
	{
	  print("Displaying image", 1);
	  print('Displaying image at ' + topLeftX + ' ' +  topLeftY, 1);
	  print('Width/height' + imageWidth + "/" + imageHeight, 1);
      canvasContext.drawImage(image, topLeftX, topLeftY, imageWidth, imageHeight );
	  print("Image drawn on canvas", 1);
	}
	else
	{
		print("Unable to obtain image " + m_imagesPlaced[imageLoop-1].Filename + ".png", 1);
	}
 }
 
  function calculate_Coordinates(btnCount, imageName, penData)
  {
	  var imageDetails = new ImageDetails();
      print("Calculating co-ords for " + imageName, 1);
      var topLeftX = penData.x * m_capability.screenWidth / m_capability.tabletMaxX
      var topLeftY = penData.y * m_capability.screenHeight / m_capability.tabletMaxY
	  
      var imageWidth = document.getElementById(imageName).width;
	  var imageHeight = document.getElementById(imageName).height;
 
	  print("Image width/height for image : " + imageWidth + ' ' + imageHeight + ' ' + imageName, 1);   
	  print("Original x/y : " + topLeftX + ' ' + topLeftY, 1);
	  
	  /*  topLeftX and topLeftY represent the point on the pad where the pen touched,
	      and we want the centre of the picture to appear here, not its top left point
	  */
	  var centred_TopLeftX = Math.floor(topLeftX - imageWidth / 2);
	  var centred_TopLeftY = Math.floor(topLeftY - imageHeight / 2);
	  
	  print("Recalculated x/y : " + centred_TopLeftX + ' ' + centred_TopLeftY, 1);
	  print("Saving to button " + btnCount, 1);
	  
	  m_btns[btnCount].ImageWidth = imageWidth;
	  m_btns[btnCount].ImageHeight = imageHeight;
	  
	  imageDetails.ImageTopLeftX = centred_TopLeftX;
	  imageDetails.ImageTopLeftY = centred_TopLeftY;
	  imageDetails.ImageLowerRightX = centred_TopLeftX + imageWidth -1;
	  imageDetails.ImageLowerRightY = centred_TopLeftY + imageHeight -1;
	  imageDetails.Filename = imageName;
	  
	  ++gImageCount;
	  print("Adding image to array: " + imageName,1 );
      m_imagesPlaced.push(imageDetails);	  
	  
	  print("Width/height: " + imageWidth + ' ' + imageHeight, 1);
  }

  // Send an image to a defined area of the pad  
  function sdkSendToSTU_Area(filename, btnSelected) 
  {
	var ImageFile = filename + '.png';
    var filepath = getCurrentDir() + ImageFile;
	print("btnSelected: " + btnSelected, 1);
    print("Sending " + filepath + " to area", 1);
 
    try
    {
	  print("Sending file " + filename, 1);
      print("STU model: " + m_information.modelName, 1);
      var pId = m_tablet.getProductId();
      print("Product id: "+ dec2hex(pId), 1);
      print("encodingMode: " + dec2hex(m_encodingMode), 1);
  
      var rect = new ActiveXObject("WacomGSS.STU.Rectangle");
	  
      rect.upperLeftXPixel  = m_imagesPlaced[gImageCount-1].ImageTopLeftX;
	  rect.upperLeftYPixel  = m_imagesPlaced[gImageCount-1].ImageTopLeftY;
      rect.LowerRightXPixel = m_imagesPlaced[gImageCount-1].ImageLowerRightX; 
      rect.LowerRightYPixel = m_imagesPlaced[gImageCount-1].ImageLowerRightY; 

      print("Update image area at X/Y: " + rect.upperLeftXPixel + ' ' + rect.upperLeftYPixel, 1);
        
      var stuImage = m_protocolHelper.resizeAndFlatten(unescape(filepath), 0, 0, 0, 0, m_btns[btnSelected-1].ImageWidth, m_btns[btnSelected-1].ImageHeight, m_encodingMode, enumScale.Scale_Stretch, false, 0);
	  print("Writing image", 1);
      m_tablet.writeImageArea(m_encodingMode, rect, stuImage);
    }
    catch(e)
    {
      print("Exception: " + e.message, 1);
    }
  }
  
  // Controlling function for placing an image on the pad and displaying it on the HTML canvas
  function placeImage(penData)
  {
    print("Placing image for button : " + m_lastButton, 1);
	m_penMode = enumPenMode.PlacingImage;
	print("Disabling inking (5)", 1);
	m_tablet.setInkingMode(0x00);
	m_penMode = enumPenMode.PlacingImage;
	print('Sending picture ' + m_lastButton + ' to x/y ' + penData.x + ' ' + penData.y, 1);
	calculate_Coordinates(m_lastButton-1, m_btns[m_lastButton-1].Image, penData);
	print("Returned from calculation", 2);
	drawImageOnCanvas(m_lastButton-1);
	print("Returned from drawing on canvas", 2);
	print(m_btns[m_lastButton-1].Image,2);
	sdkSendToSTU_Area(m_btns[m_lastButton-1].Image, m_lastButton);
  }
}