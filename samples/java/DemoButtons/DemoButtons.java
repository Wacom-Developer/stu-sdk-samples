import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import javax.swing.border.*;
import java.util.List;
import java.util.ArrayList;
import java.awt.geom.Line2D;
import java.awt.geom.Point2D;
import java.awt.image.BufferedImage;
import com.WacomGSS.STU.*;
import com.WacomGSS.STU.Protocol.*;
import java.util.Arrays;

import java.math.BigInteger;
import java.security.Key;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.PublicKey;
import java.security.interfaces.RSAPublicKey;

import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;

import java.io.File;
import javax.imageio.ImageIO;

import java.util.concurrent.Executors;
import java.util.concurrent.Future;

// Notes:
// There are three coordinate spaces to deal with that are named:
//   tablet: the raw tablet coordinate
//   screen: the tablet LCD screen
//   client: the Form window client area

public class DemoButtons extends JFrame
{
  private JPanel image;
  private Future<KeyPair> keyPair;
  
  static class SignatureDialog extends JDialog implements ITabletHandler
  {

    static class MyEncryptionHandler implements Tablet.IEncryptionHandler
    {
      private BigInteger p;
      private BigInteger g;
      private BigInteger privateKey;
      private Cipher     aesCipher;
       
      @Override
      public void reset()
      {
        clearKeys();
        this.p = null;
        this.g = null;
      }

      @Override
      public void clearKeys()
      {
        this.privateKey = null;
        this.aesCipher = null;
      }

      @Override
      public boolean requireDH()
      {
        return this.p == null || this.g == null;
      }

      @Override
      public void setDH(DHprime dhPrime, DHbase dhBase)
      {
        this.p = new BigInteger(1, dhPrime.getValue());
        this.g = new BigInteger(1, dhBase.getValue());
      }

      @Override
      public com.WacomGSS.STU.Protocol.PublicKey generateHostPublicKey()
      {
        this.privateKey = new BigInteger("0F965BC2C949B91938787D5973C94856C", 16); // should be randomly chosen according to DH rules.

        BigInteger publicKey_bi = this.g.modPow(this.privateKey, this.p);
        try
        {
          com.WacomGSS.STU.Protocol.PublicKey publicKey = new com.WacomGSS.STU.Protocol.PublicKey(publicKey_bi.toByteArray());
          return publicKey;
        } 
        catch (Exception e)
        {
        }
        return null;
      }

      @Override
      public void computeSharedKey(com.WacomGSS.STU.Protocol.PublicKey devicePublicKey)
      {
        BigInteger devicePublicKey_bi = new BigInteger(1, devicePublicKey.getValue());
        BigInteger sharedKey = devicePublicKey_bi.modPow(this.privateKey, this.p);

        byte[] array = sharedKey.toByteArray();
        if (array[0] == 0)
        {
          byte[] tmp = new byte[array.length - 1];
          System.arraycopy(array, 1, tmp, 0, tmp.length);
          array = tmp;
        }

        try
        {
          Key aesKey = new SecretKeySpec(array, "AES");
      
          this.aesCipher = Cipher.getInstance("AES/ECB/NoPadding");
          aesCipher.init(Cipher.DECRYPT_MODE, aesKey);
          return;
        }
        catch (Exception e)
        {
        }
        this.aesCipher = null;
      }

      @Override
      public byte[] decrypt(byte[] data)
      {
        try
        {
          byte[] decryptedData = this.aesCipher.doFinal(data);
          return decryptedData;
        }
        catch (Exception e)
        {
        }
        return null;
      }
    }


    static class MyEncryptionHandler2 implements Tablet.IEncryptionHandler2
    {
      private Future<KeyPair> futureKeyPair;
      private Cipher  aesCipher;

      public MyEncryptionHandler2(Future<KeyPair> keyPair)
      {
        futureKeyPair = keyPair;
      }

      @Override
      public void reset()
      {
        clearKeys();
        //this.keyPair = null;
      }

      public void clearKeys()
      {
        this.aesCipher = null;
      }

      @Override
      public SymmetricKeyType getSymmetricKeyType()
      {
        return SymmetricKeyType.AES128;
        //return SymmetricKeyType.AES256; // requires "Java Crypotography Extension (JCE) Unlimited Strength Jurisdiction Policy Files"
      }

      @Override
      public AsymmetricPaddingType getAsymmetricPaddingType()
      {
        return AsymmetricPaddingType.PKCS1;
        //return AsymmetricPaddingType.OAEP;
      }

      @Override
      public AsymmetricKeyType getAsymmetricKeyType()
      {
        return AsymmetricKeyType.RSA2048;
      }

      public String toHex(byte[] arr)
      {
        StringBuilder sb = new StringBuilder(arr.length * 2);
        java.util.Formatter formatter = new java.util.Formatter(sb);  
        for (byte b : arr)
        {  
            formatter.format("%02x", b);  
        }  
        return sb.toString();  
      }

      private int rsaKeySize()
      {
        switch (this.getAsymmetricKeyType())
        {
          case RSA1024: return 1024;
          case RSA1536: return 1536;
          case RSA2048: return 2048;
        }
        return 0;
      }

      private int aesKeySize()
      {
        switch (this.getSymmetricKeyType())
        {
          case AES128: return 128;
          case AES192: return 192;
          case AES256: return 256;
        }
        return 0;
      }


      private KeyPair ensureKeyPair()
      {
        try
        {
          return futureKeyPair.get();
        }
        catch (Exception e)
        {
          System.out.println("retrieving keyPair exception");
        }
        return null;
      }


      @Override
      public byte[] getPublicExponent()
      {
        KeyPair keyPair = this.ensureKeyPair();

        byte[] ret = ((RSAPublicKey)keyPair.getPublic()).getPublicExponent().toByteArray();
        return ret;
      }

      @Override
      public byte[] generatePublicKey()
      {
        KeyPair keyPair = this.ensureKeyPair();

        byte[] modulus = ((RSAPublicKey)keyPair.getPublic()).getModulus().toByteArray();

        byte[]ret = new byte[rsaKeySize()/8];
        System.arraycopy(modulus, modulus.length-ret.length, ret, 0, ret.length);

        return ret;
      }

      @Override
      public void computeSessionKey(byte[] data)
      {
        KeyPair keyPair = this.ensureKeyPair();

        byte[] plaintext = null;
        try
        {
          Cipher rsaCipher = Cipher.getInstance("RSA/ECB/PKCS1Padding");
          rsaCipher.init(Cipher.DECRYPT_MODE, keyPair.getPrivate());
          plaintext = rsaCipher.doFinal(data);
        }
        catch (Exception e)
        {
        }

        int keySizeBytes = this.aesKeySize()/8;

        if (plaintext.length != keySizeBytes)
        {
          byte[] k2 = new byte[keySizeBytes];
          if (plaintext.length > keySizeBytes)
            System.arraycopy(plaintext, plaintext.length-keySizeBytes, k2, 0, k2.length);
          else
            System.arraycopy(plaintext, 0, k2, 1, keySizeBytes -1);
          plaintext = k2;
        }

        Key aesKey = new SecretKeySpec(plaintext, "AES");
      
        try
        {
          this.aesCipher = Cipher.getInstance("AES/ECB/NoPadding");
          this.aesCipher.init(Cipher.DECRYPT_MODE, aesKey);
          return;
        }
        catch (Exception e)
        {
        }
        this.aesCipher = null;
      }

      @Override
      public byte[] decrypt(byte[] data)
      {
        try
        {
          byte[] decryptedData = this.aesCipher.doFinal(data);
          return decryptedData;
        }
        catch (Exception e)
        {
        }
        return null;
      }
    }




    private Tablet        tablet;
    private Capability    capability;
    private Information   information;

    // In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
    // Using an array of these makes it easy to add or remove buttons as desired.
    private static class Button
    {
      java.awt.Rectangle bounds;  // in Screen coordinates
      String             text;
      ActionListener     click;

      void performClick()
      {
        click.actionPerformed(null);
      }
    }

    // The isDown flag is used like this:
    // 0 = up
    // +ve = down, pressed on button number
    // -1 = down, inking
    // -2 = down, ignoring
    private int isDown;

    private List<PenData> penData; // Array of data being stored. This can be subsequently used as desired.

    private Button[] btns; // The array of buttons that we are emulating.

    private JPanel        panel;
    
    private boolean useSigMode;   // use Signature Mode (STU-540 only)
    private BufferedImage bitmap; // This bitmap that we display on the screen.
    private EncodingMode encodingMode;  // How we send the bitmap to the device.
    private byte[] bitmapData;    // This is the flattened data of the bitmap that we send to the device.
    
    private boolean encrypted = false;



    private Point2D.Float tabletToClient(PenData penData)
    {
      // Client means the panel coordinates.
      return new Point2D.Float( (float)penData.getX() * this.panel.getWidth()  / this.capability.getTabletMaxX(), 
                                (float)penData.getY() * this.panel.getHeight() / this.capability.getTabletMaxY() );
    }



    private Point2D.Float tabletToScreen(PenData penData)
    {
      // Screen means LCD screen of the tablet.
      return new Point2D.Float( (float)penData.getX() * this.capability.getScreenWidth() / this.capability.getTabletMaxX(), 
                                (float)penData.getY() * this.capability.getScreenHeight() / this.capability.getTabletMaxY() );
    }



    private Point clientToScreen(Point pt)
    {
      // client (window) coordinates to LCD screen coordinates. 
      // This is needed for converting mouse coordinates into LCD bitmap coordinates as that's
      // what this application uses as the coordinate space for buttons.
      return new Point( Math.round( (float)pt.getX() * this.capability.getScreenWidth() / this.panel.getWidth() ), 
                        Math.round( (float)pt.getY() * this.capability.getScreenHeight() / this.panel.getHeight() ) );
    }



    private void pressOkButton() throws STUException
    {
      this.setVisible(false);
    }



    private void pressClearButton() throws STUException
    {
      clearScreen();
    }



    private void pressCancelButton() throws STUException
    {
      this.setVisible(false);
      this.penData = null;
    }



    private void clearScreen() throws STUException
    {
      if (!this.useSigMode)
      {
        this.tablet.writeImage(this.encodingMode, this.bitmapData);
      }
      this.penData.clear();
      this.isDown = 0;
      this.panel.repaint();
    }



    public void dispose()
    {
      // Ensure that you correctly disconnect from the tablet, otherwise you are 
      // likely to get errors when wanting to connect a second time.
      if (this.tablet != null)
      {
        try
        {
          this.tablet.setInkingMode(InkingMode.Off);

          if (encrypted) {
            this.tablet.endCapture();
            encrypted = false;
          }
      
          this.tablet.setOperationMode(OperationMode.initializeNormal());
          this.tablet.setClearScreen();
        }
        catch (Throwable t)
        {
        }
        this.tablet.disconnect();
        this.tablet = null;
      }
      
      super.dispose();
    }



    private void drawCenteredString(Graphics2D gfx, String text, int x, int y, int width, int height)
    {
      FontMetrics fm   = gfx.getFontMetrics(gfx.getFont());
      int textHeight   = fm.getHeight();
      int textWidth    = fm.stringWidth(text);
   
      int textX = x + (width  - textWidth) / 2;
      int textY = y + (height - textHeight) / 2 + fm.getAscent();

      gfx.drawString(text, textX, textY);
    }



    private void drawInk(Graphics2D gfx, PenData pd0, PenData pd1)
    {
      gfx.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
      gfx.setColor(new Color(0,0,64,255));
      gfx.setStroke(new BasicStroke(3, BasicStroke.CAP_ROUND, BasicStroke.JOIN_ROUND));

      Point2D.Float pt0 = tabletToClient(pd0);
      Point2D.Float pt1 = tabletToClient(pd1);
      Shape l = new Line2D.Float(pt0, pt1);
      gfx.draw(l);
    }



    private void drawInk(Graphics2D gfx)
    {
      PenData[] pd = new PenData[0];
      pd = this.penData.toArray(pd);
      for (int i = 1; i < pd.length; ++i)
      {
        if (pd[i-1].getSw() != 0 && pd[i].getSw() != 0)
        {
          drawInk(gfx, pd[i-1], pd[i]);
        }
      }
    }



    // Pass in the device you want to connect to!
    public SignatureDialog(Frame frame, UsbDevice usbDevice, TlsDevice tlsDevice, boolean useSigMode, Future<KeyPair> keyPair) throws STUException
    {
      super(frame, true);
      this.setLocation(new Point(0, 0));
      this.setLocationRelativeTo(frame);
      this.panel = new JPanel()
        {
          @Override
          public void paintComponent(Graphics gfx)
          {
            super.paintComponent(gfx);
            if (bitmap != null)
            {
               Image rescaled = bitmap.getScaledInstance(panel.getWidth(),panel.getHeight(), Image.SCALE_SMOOTH);
               gfx.drawImage(rescaled, 0,0, null);
               drawInk((Graphics2D)gfx);
            }
          }
        };
      this.panel.addMouseListener(new MouseAdapter()
        { 
          public void mouseClicked(MouseEvent e)
          {
            Point pt = clientToScreen(e.getPoint());
            for (Button btn : SignatureDialog.this.btns)
            {
              if (btn.bounds.contains(pt))
              {
                btn.performClick();
                break;
              }
            }
          }
        });


      this.penData = new ArrayList<PenData>();

      try
      {
        this.tablet = new Tablet();
        // A more sophisticated applications should cycle for a few times as the connection may only be
        // temporarily unavailable for a second or so. 
        // For example, if a background process such as Wacom STU Display
        // is running, this periodically updates a slideshow of images to the device.

        this.tablet.setEncryptionHandler(new MyEncryptionHandler());
        this.tablet.setEncryptionHandler2(new MyEncryptionHandler2(keyPair));
        
        int e;
        if (usbDevice != null)
          e = tablet.usbConnect(usbDevice, true);
        else
          e = tablet.tlsConnect(tlsDevice);
        if (e == 0)
        {
          this.capability = tablet.getCapability();
          this.information = tablet.getInformation();
        }
        else
        {
          throw new RuntimeException("Failed to connect to USB tablet, error " + e);
        }

        if (useSigMode && !tablet.isSupported(ReportId.OperationMode))
        {
          JOptionPane.showMessageDialog(this, 
                                        this.information.getModelName() + " does not support Signature Mode operation, reverting to normal operation", 
                                        "Warning", 
                                        JOptionPane.WARNING_MESSAGE);
          useSigMode = false;
        }
        this.useSigMode = useSigMode;

        // Set the size of the client window to be actual size, 
        // based on the reported DPI of the monitor.

        int screenResolution = this.getToolkit().getScreenResolution();

        Dimension d = new Dimension(this.capability.getTabletMaxX()*screenResolution/2540, this.capability.getTabletMaxY()*screenResolution/2540);
        this.panel.setPreferredSize(d);
        this.setLayout(new BorderLayout());
        this.setResizable(false);
        this.add(this.panel);
        this.pack();

        this.btns = new Button[3];
        this.btns[0] = new Button();
        this.btns[1] = new Button();
        this.btns[2] = new Button();

        if (useSigMode)
        {
          // LCD is 800x480; Button positions and sizes are fixed
          btns[0].bounds = new java.awt.Rectangle(  0, 431, 265, 48);
          btns[1].bounds = new java.awt.Rectangle(266, 431, 265, 48);
          btns[2].bounds = new java.awt.Rectangle(532, 431, 265, 48);
        }
        else if (this.tablet.getProductId() != UsbDevice.ProductId_300)
        {
          // Place the buttons across the bottom of the screen.

          int w2 = this.capability.getScreenWidth() / 3;
          int w3 = this.capability.getScreenWidth() / 3;
          int w1 = this.capability.getScreenWidth() - w2 - w3;
          int y = this.capability.getScreenHeight() * 6 / 7;
          int h = this.capability.getScreenHeight() - y;

          btns[0].bounds = new java.awt.Rectangle(0, y, w1, h);
          btns[1].bounds = new java.awt.Rectangle(w1, y, w2, h);
          btns[2].bounds = new java.awt.Rectangle(w1 + w2, y, w3, h);
        }
        else
        {
          // The STU-300 is very shallow, so it is better to utilise
          // the buttons to the side of the display instead.

          int x = this.capability.getScreenWidth() * 3 / 4;
          int w = this.capability.getScreenWidth() - x;

          int h2 = this.capability.getScreenHeight() / 3;
          int h3 = this.capability.getScreenHeight() / 3;
          int h1 = this.capability.getScreenHeight() - h2 - h3;

          btns[0].bounds = new java.awt.Rectangle(x, 0, w, h1);
          btns[1].bounds = new java.awt.Rectangle(x, h1, w, h2);
          btns[2].bounds = new java.awt.Rectangle(x, h1 + h2, w, h3);
        }

        btns[0].text = "Clear";
        btns[0].click = new ActionListener() 
          {
            public void actionPerformed(ActionEvent evt)
            {
              try
              {
                pressClearButton();
              }
              catch (STUException e)
              {
               // e
              }
            }
          };

        btns[1].text = "Cancel";
        btns[1].click = new ActionListener() 
          {
            public void actionPerformed(ActionEvent evt)
            {
              try
              {
                pressCancelButton();
              }
              catch (STUException e)
              {
               // e
              }
            }
          };

        btns[2].text = "OK";
        btns[2].click = new ActionListener() 
          {
            public void actionPerformed(ActionEvent evt)
            {
              try
              {
                pressOkButton();
              }
              catch (STUException e)
              {
               // e
              }
            }
          };

        byte encodingFlag = ProtocolHelper.simulateEncodingFlag(this.tablet.getProductId(), this.capability.getEncodingFlag());

        if ((encodingFlag & EncodingFlag.EncodingFlag_24bit) != 0)
        {
          this.encodingMode = this.tablet.supportsWrite() ? EncodingMode.EncodingMode_24bit_Bulk : EncodingMode.EncodingMode_24bit;
        }
        else if ((encodingFlag & EncodingFlag.EncodingFlag_16bit) != 0)
        {
          this.encodingMode = this.tablet.supportsWrite() ? EncodingMode.EncodingMode_16bit_Bulk : EncodingMode.EncodingMode_16bit;
        }
        else
        {
          this.encodingMode = EncodingMode.EncodingMode_1bit;
        }


        if (useSigMode && !initializeSigMode())
        {
          JOptionPane.showMessageDialog(this, 
                                        "Exception initializing Signature Mode, reverting to normal operation", 
                                        "Error", 
                                        JOptionPane.ERROR_MESSAGE);
          useSigMode = false;
        }

        if (!useSigMode)
        {
          Color btnColor = (this.encodingMode == EncodingMode.EncodingMode_1bit) ? Color.WHITE : Color.LIGHT_GRAY;
          this.bitmap = createScreenImage(new Color[] { btnColor, btnColor, btnColor}, Color.BLACK, null);

          // Now the bitmap has been created, it needs to be converted to device-native
          // format.
          this.bitmapData = ProtocolHelper.flatten(this.bitmap, this.bitmap.getWidth(), this.bitmap.getHeight(), encodingMode);
        }

        // If you wish to further optimize image transfer, you can compress the image using 
        // the Zlib algorithm.
        boolean useZlibCompression = false;

        if (this.encodingMode == EncodingMode.EncodingMode_1bit && useZlibCompression)
        {
          // m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
          // m_encodingMode = EncodingMode.EncodingMode_1bit_Zlib;
        }

        // Add the delagate that receives pen data.
        this.tablet.addTabletHandler(this);

        // Initialize the screen
        clearScreen();

        if (ProtocolHelper.supportsEncryption(tablet.getProtocol())||tablet.isSupported(ReportId.EncryptionStatus))
        {
          this.tablet.startCapture(0xc0ffee);
          encrypted = true;
        }

        // Enable the pen data on the screen (if not already)
        this.tablet.setInkingMode(InkingMode.On);
      }
      catch (Throwable t)
      {
        if (this.tablet != null)
        {
          this.tablet.disconnect();
          this.tablet = null;
        }
        throw t;
      }
    }



    public void onGetReportException(STUException e)
    {
      JOptionPane.showMessageDialog(this, "Error:" + e, "Error (onGetReportException)", JOptionPane.ERROR_MESSAGE);
      this.tablet.disconnect();
      this.tablet = null;
      this.penData = null;
      this.setVisible(false);
    }



    public void onUnhandledReportData(byte[] data)
    {
    }



    public void onPenData(PenData penData)
    {
      Point2D pt = tabletToScreen(penData);

      int btn = 0; // will be +ve if the pen is over a button.
      {        
        for (int i = 0; i < this.btns.length; ++i)
        {
          if (this.btns[i].bounds.contains(pt))
          {
            btn = i+1;
            break;
          }
        }
      }

      boolean isDown = (penData.getSw() != 0);

      // This code uses a model of four states the pen can be in:
      // down or up, and whether this is the first sample of that state.

      if (isDown)
      {
        if (this.isDown == 0)
        {
          // transition to down
          if (btn > 0)
          {
            // We have put the pen down on a button.
            // Track the pen without inking on the client.

            this.isDown = btn; 
          }
          else
          {
            // We have put the pen down somewhere else.
            // Treat it as part of the signature.

            this.isDown = -1;
          }
        }
        else
        {
          // already down, keep doing what we're doing!
          // draw
          if (!this.penData.isEmpty() && this.isDown == -1)
          {
            // Draw a line from the previous down point to this down point.
            // This is the simplist thing you can do; a more sophisticated program
            // can perform higher quality rendering than this!
            Graphics2D gfx = (Graphics2D)this.panel.getGraphics();
            drawInk(gfx, this.penData.get(this.penData.size()-1), penData);
            gfx.dispose();
          }

        }

        // The pen is down, store it for use later.
        if (this.isDown == -1)
          this.penData.add(penData);
      }
      else
      {
        if (this.isDown != 0)
        {
          // transition to up
          if (btn > 0)
          {
            // The pen is over a button

            if (btn == this.isDown && !this.useSigMode)
            {
              // The pen was pressed down over the same button as is was lifted now. 
              // Consider that as a click.
              // In Signature Mode, click detection is handled by the tablet and
              // generates a EventDataSignature/EventDataSignatureEncrypted event
              this.btns[btn - 1].performClick();
            }
          }
          this.isDown = 0;
        }
        else
        {
           // still up
        }

        // Add up data once we have collected some down data.
        if (!this.penData.isEmpty())
          this.penData.add(penData);
      }
      
    }



    public void onPenDataOption(PenDataOption penDataOption)
    {
      onPenData(penDataOption);
    }



    public void onPenDataEncrypted(PenDataEncrypted penDataEncrypted)
    {
      onPenData(penDataEncrypted.getPenData1());
      onPenData(penDataEncrypted.getPenData2());
    }



    public void onPenDataEncryptedOption(PenDataEncryptedOption penDataEncryptedOption)
    {
      onPenData(penDataEncryptedOption.getPenDataOption1());
      onPenData(penDataEncryptedOption.getPenDataOption2());
    }


    public void onPenDataTimeCountSequence(PenDataTimeCountSequence penDataTimeCountSequence)
    {
      onPenData(penDataTimeCountSequence);
    }


    public void onPenDataTimeCountSequenceEncrypted(PenDataTimeCountSequenceEncrypted penDataTimeCountSequenceEncrypted)
    {
      onPenData(penDataTimeCountSequenceEncrypted);
    }


    public void onEncryptionStatus(EncryptionStatus encryptionStatus)
    {
    }

    public void onDevicePublicKey(DevicePublicKey devicePublicKey)
    {
    }


    public void onEventDataPinPad(EventDataPinPad eventData)
    {
    }


    public void onEventDataKeyPad(EventDataKeyPad eventData)
    {
    }


    public void onEventDataSignature(EventDataSignature eventData)
    {
      onSignatureEvent(eventData.getKeyValue());
    }


    public void onEventDataPinPadEncrypted(EventDataPinPadEncrypted eventData)
    {
    }


    public void onEventDataKeyPadEncrypted(EventDataKeyPadEncrypted eventData)
    {
    }


    public void onEventDataSignatureEncrypted(EventDataSignatureEncrypted eventData)
    {   
      onSignatureEvent(eventData.getKeyValue());
    }


    private void onSignatureEvent(byte keyValue)
    {
      try
      {
        switch (keyValue)
        {
          case (byte)0:
            pressCancelButton();
            break;

          case (byte)1:
            pressOkButton();
            break;

          case (byte)2:
            pressClearButton();
            break;
        }
      }
      catch (Exception ex)
      {       
      }
    }


    public PenData[] getPenData()
    {
      if (this.penData != null)
      {
        PenData[] arrayPenData = new PenData[0];
        return this.penData.toArray(arrayPenData);
      }
      return null;
    }

    public Information getInformation()
    {
      if (this.penData != null)
      {
        return this.information;
      }
      return null;
    }


    public Capability getCapability()
    {
      if (this.penData != null)
      {
        return this.capability;
      }
      return null;
    }

    private static final byte sigScreenImageNum = (byte)2;

    // Check if a Signature Mode screen image is already stored on the tablet. Download it if not.
    private void checkSigModeImage(boolean pushed, byte[] imageData) throws STUException, java.security.NoSuchAlgorithmException
    {
      boolean sigKeyEnabled[] = { true, true, true };
      RomStartImageData romStartImageData = RomStartImageData.initializeSignature(this.encodingMode, pushed, sigScreenImageNum, sigKeyEnabled);

      this.tablet.setRomImageHash(OperationModeType.Signature, pushed, sigScreenImageNum);

      RomImageHash romImgHash = tablet.getRomImageHash();

      boolean writeImage = true;
      if (romImgHash.getResult() == 0)
      {
        // There is already an image stored on the tablet corresponding to this image number and pushed state:
        // compare image hashes to determine if we need to overwrite it.
        java.security.MessageDigest md = java.security.MessageDigest.getInstance("MD5");
        byte[] hash = md.digest(imageData);
        if (Arrays.equals(hash, romImgHash.getHash()))
        {
          // Image hashes match: no need to write image again
          writeImage = false;
        }
      }
      // else - no image on pad, writeImage = true;

      if (writeImage)
      {
        tablet.writeRomImage(romStartImageData, imageData);
      }
    }

    // Create bitmap image for the tablet LCD screen and/or client (window).
    // This application uses the same size bitmap for both the screen and client.
    // However, at high DPI, this bitmap will be stretch and it would be better to 
    // create individual bitmaps for screen and client at native resolutions.
    private BufferedImage createScreenImage(Color[] btnColors, Color txtColor, byte[] btnOrder)
    {
      BufferedImage image = new BufferedImage(this.capability.getScreenWidth(), this.capability.getScreenHeight(), BufferedImage.TYPE_INT_RGB);
      Graphics2D gfx = image.createGraphics();
    
      gfx.setColor(Color.WHITE);
      gfx.fillRect(0, 0, image.getWidth(), image.getHeight());

      double fontSize = (this.btns[0].bounds.getHeight() / 2.0); // pixels
      gfx.setFont(new Font("Arial", Font.PLAIN, (int)fontSize));

      // Draw the buttons
      for (int i = 0; i < this.btns.length; ++i)
      {
        // Button objects are created in the order, left-to-right, Clear / Cancel / OK
        // If reordering for Signature Mode (btnOrder != null), use bounds of another button when drawing
        // for image to be sent to tablet.
        Button              btn = this.btns[i];
        java.awt.Rectangle  bounds = this.btns[(btnOrder == null) ? i : btnOrder[i]].bounds;

        if (this.encodingMode != EncodingMode.EncodingMode_1bit)
        {
          gfx.setColor(btnColors[i]);
          gfx.fillRect((int)bounds.getX(), (int)bounds.getY(), (int)bounds.getWidth(), (int)bounds.getHeight());
        }
        gfx.setColor(txtColor);
        gfx.drawRect((int)bounds.getX(), (int)bounds.getY(), (int)bounds.getWidth(), (int)bounds.getHeight());
        drawCenteredString(gfx, btn.text, (int)bounds.getX(), (int)bounds.getY(), (int)bounds.getWidth(), (int)bounds.getHeight());
      }

      gfx.dispose();

      return image;
    }

    // Initialize Signature Mode (STU-540 only)
    private boolean initializeSigMode()
    {
      try 
      {
        // Buttons on bitmaps sent to the tablet must be in the order Cancel / OK / Clear. The tablet will then 
        // reorder button images displayed according to parameters passed to it in OperationMode_Signature
        // This application uses Clear / Cancel / OK
        byte[]  btnOrder = { (byte)2, (byte)0, (byte)1 };
        Color[] btnsUpColors = new Color[] { new Color(0, 96, 255), Color.RED, Color.GREEN.darker() };
        Color[] btnsDownColors = new Color[] { btnsUpColors[0].darker(), btnsUpColors[1].darker(), btnsUpColors[2].darker() };
        byte[]  bitmapData;

        BufferedImage btnsUp  = createScreenImage(btnsUpColors, Color.BLACK, btnOrder);
        bitmapData = ProtocolHelper.flatten(btnsUp, btnsUp.getWidth(), btnsUp.getHeight(), encodingMode);
        checkSigModeImage(false, bitmapData);


        BufferedImage btnsPushed = createScreenImage(btnsDownColors, Color.WHITE, btnOrder);
        bitmapData = ProtocolHelper.flatten(btnsPushed, btnsPushed.getWidth(), btnsPushed.getHeight(), encodingMode);
        checkSigModeImage(true, bitmapData);

        OperationMode_Signature sigMode = new OperationMode_Signature(sigScreenImageNum, btnOrder, (byte)0, (byte)0 );

        this.tablet.setOperationMode(OperationMode.initializeSignature(sigMode));

        this.bitmap = createScreenImage(btnsUpColors, Color.BLACK, null);

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

  }



  BufferedImage signatureImage;
  private JCheckBox chkUseSigMode;

  private Point2D.Float tabletToClient(PenData penData, Capability capability, JPanel panel)
  {
      // Client means the panel coordinates.
      //return new Point2D.Float( (float)penData.getX() * this.panel.getWidth()  / this.capability.getTabletMaxX(), 
                                //(float)penData.getY() * this.panel.getHeight() / this.capability.getTabletMaxY() );

      
      //System.out.println("tabletToClient X/Y " + penData.getX() + " " + penData.getY());
	   /*
	  System.out.println("Arg 1: " + penData.getX() * panel.getWidth() / capability.getTabletMaxX() );
	  System.out.println("Arg 2: " + penData.getY() * panel.getHeight() / capability.getTabletMaxY() );
	  */
	  return new Point2D.Float((float) penData.getX() * panel.getWidth() / capability.getTabletMaxX(),
				(float) penData.getY() * panel.getHeight() / capability.getTabletMaxY());
  }
	
  private BufferedImage createImage(PenData[] penData, Capability capability, Information information) {
		BufferedImage bi = new BufferedImage(image.getWidth(), image.getHeight(), BufferedImage.TYPE_INT_RGB);
		Graphics2D g = (Graphics2D) bi.getGraphics();
		g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
		g.setColor(Color.WHITE);
		g.fillRect(0, 0, bi.getWidth(), bi.getHeight());
		g.setColor(new Color(0, 0, 64, 255));		
		g.setStroke(new BasicStroke(3, BasicStroke.CAP_ROUND,
				BasicStroke.JOIN_ROUND));						
		
		/*
		System.out.println("Screen width/height: " + capability.getScreenWidth() + " " + capability.getScreenHeight());
		
		System.out.println("Converting pendata into graphics");
		System.out.println("Pendata length: " + penData.length);	
		System.out.println("image width + height: " + image.getWidth() + " " + image.getHeight());
	    System.out.println("Tabletmaxx " + capability.getTabletMaxX());
	    System.out.println("TabletmaxY " + capability.getTabletMaxY());
		*/
		for (int i = 1; i < penData.length; i++)
		{		    
			PenData p1 = penData[i];
			if (p1.getSw() != 0)
			{
				//System.out.println("Drawing point " + i);
				Point2D.Float pt1 = tabletToClient(penData[i - 1], capability, image);
				Point2D.Float pt2 = tabletToClient(penData[i], capability, image);
				//System.out.println("Creating shape");
				Shape l = new Line2D.Float(pt1, pt2);
				g.draw(l);		
			}
		}
		//System.out.println("End of createImage");
		return bi;
  }


  private void onGetSignature()
  {
    try
    {
      com.WacomGSS.STU.UsbDevice[] usbDevices = UsbDevice.getUsbDevices();
      com.WacomGSS.STU.TlsDevice[] tlsDevices = TlsDevice.getTlsDevices();

      com.WacomGSS.STU.UsbDevice usbDevice = null;
      com.WacomGSS.STU.TlsDevice tlsDevice = null;
      if (usbDevices != null && usbDevices.length > 0)
        usbDevice = usbDevices[0];
      if (tlsDevices != null && tlsDevices.length > 0)
        tlsDevice = tlsDevices[0];

      if (usbDevice != null || tlsDevice != null)
      {
        boolean sigMode = chkUseSigMode.isSelected();

        SignatureDialog signatureDialog = new SignatureDialog(this, usbDevice,tlsDevice, sigMode, this.keyPair);

        signatureDialog.setVisible(true);

        PenData[] penData = signatureDialog.getPenData();
        if (penData != null && penData.length > 0)
        {
          // collected data!
		   this.signatureImage = createImage(penData, signatureDialog.getCapability(), signatureDialog.getInformation());
		  //System.out.println("Repainting");
		  image.repaint();
		  try
		  {
			  ImageIO.write(this.signatureImage, "png", new File("sig.png"));
		  }
		  catch (Exception e)
		  {
		  }
        }
        signatureDialog.dispose();
        
      }
      else
      {
        throw new RuntimeException("No tablets attached");
      }
    }
    catch (STUException e)
    {
        JOptionPane.showMessageDialog(this,
              e,
              "Error (STU)",
              JOptionPane.ERROR_MESSAGE);
    }
    catch (RuntimeException e)
    {
        JOptionPane.showMessageDialog(this,
              e,
              "Error (RT)",
              JOptionPane.ERROR_MESSAGE);
    }
    catch (Exception e)
    {
        JOptionPane.showMessageDialog(this,
              e,
              "Error",
              JOptionPane.ERROR_MESSAGE);
    }
  }



  public DemoButtons()
  {
    this.keyPair = Executors.newSingleThreadExecutor().submit(()->
    {
      KeyPair keyPair = null;
      try
      {
        System.out.println("creating RSA keypair...");
        KeyPairGenerator keyGen = KeyPairGenerator.getInstance("RSA");
        keyGen.initialize(2048);
        keyPair = keyGen.genKeyPair();
        System.out.println("creating RSA keypair...done");
      }
      catch (Exception e)
      {
        System.out.println("creating RSA keypair exception");
      }
      return keyPair;
    });

    this.setTitle("Wacom STU SDK - Java Sample");
    this.setLayout(new BorderLayout());
    this.setMinimumSize(new Dimension(350, 100));
    this.setLocationRelativeTo(null); 

    JPanel panel = new JPanel();
    panel.setLayout(new BoxLayout(panel, BoxLayout.PAGE_AXIS));
    panel.setBorder(BorderFactory.createEmptyBorder(6, 6, 6, 6));

    JButton btn = new JButton("Get Signature");
    btn.setAlignmentX(CENTER_ALIGNMENT);
    btn.addActionListener(new ActionListener()
    {
      public void actionPerformed(ActionEvent evt)
      {
        onGetSignature();
      }
    });
    panel.add(btn);

    panel.add(Box.createRigidArea(new Dimension(0,6)));

    chkUseSigMode = new JCheckBox("Use signature mode");
    chkUseSigMode.setAlignmentX(CENTER_ALIGNMENT);
    panel.add(chkUseSigMode);

    image = new JPanel() 
      {
        @Override
        public void paintComponent(Graphics gfx)
        {
          super.paintComponent(gfx);
          if (signatureImage != null)
          {
            double newHeight = ((double) signatureImage.getHeight() / signatureImage.getWidth()) * this.getWidth();
            Image rescaled = signatureImage.getScaledInstance(this.getWidth(), (int) newHeight, Image.SCALE_AREA_AVERAGING);
            gfx.drawImage(rescaled, 0, (int) ((this.getHeight() / 2) - (newHeight / 2)), null);
          }
        }
      };
    //image.setBorder(new TitledBorder(new LineBorder(new Color(0, 0, 0)), "Image", TitledBorder.LEADING, TitledBorder.TOP, null, Color.BLACK));
    image.setPreferredSize(new Dimension(300, 200));


    this.add(panel, BorderLayout.NORTH);
    this.add(image, BorderLayout.SOUTH);
    this.pack();
    this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
  }



  private static void runProgram()
  {
    DemoButtons sample = new DemoButtons();
    sample.setVisible(true);
  }



  public static void main(String[] args)
  {
    EventQueue.invokeLater(new Runnable()
    {
      public void run()
      {
        runProgram();
      }
    });
  }
}
