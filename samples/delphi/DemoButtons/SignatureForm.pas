{
 SignatureForm.pas

 Allow user to input a signature on an STU and reproduce it on a Window on the PC.  
 Saves signature to a file on disk

 Copyright (c) 2015 Wacom GmbH. All rights reserved.
}
unit SignatureForm;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, wgssSTU_TLB, ExtCtrls, ActiveX, Math;

type TMyPenData = class
  public
    rdy: WordBool;
    sw: Byte;
    pressure: Word;
    x: Word;
    y: Word;
    Constructor Create(penData: IPenData);
end;

type
    TForm2 = class(TForm, ITabletEncryptionHandler2)
    Image1: TImage;
  procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormDestroy(Sender: TObject);
    procedure Image1Click(Sender: TObject);
  private
    procedure onPenData(ASender: TObject; const pPenData: IPenData);
    procedure onPenDataTimeCountSequence(ASender: TObject; const pPenData: IPenDataTimeCountSequence);
    procedure onGetReportException(ASender: TObject; const pException: ITabletEventsException);
    procedure clearScreen();
    function tabletToScreen(penData: TMyPenData) : TPoint;
    function contains(bounds: TRect; point: TPoint) : boolean;
    procedure clickAccept();
    procedure clickClear();
    procedure clickCancel();
    procedure DrawAntialisedLine(Canvas: TCanvas; const AX1, AY1, AX2, AY2: real; const LineColor: TColor);
    procedure saveImage();
    procedure freePoints();
    function getPenDataOptionMode() : Integer;
    procedure setPenDataOptionMode(currentPenDataOptionMode: Integer);

    // ITabletEncryptionHandler2
    procedure reset; safecall;
    procedure clearKeys; safecall;
    function getSymmetricKeyType: Byte; safecall;
    function getAsymmetricPaddingType: Byte; safecall;
    function getAsymmetricKeyType: Byte; safecall;
    function getPublicExponent: PSafeArray; safecall;
    function generatePublicKey: PSafeArray; safecall;
    procedure computeSessionKey(data: PSafeArray); safecall;
    function decrypt(data: PSafeArray): PSafeArray; safecall;
  public
    procedure connect(usbDevice: IUsbDevice);
end;

TOnClickButton = procedure() of object;

type TMyButton = class
  public
    Bounds : TRect;
    Text   : String;
    Click  : TOnClickButton;
end;

var
  Form2: TForm2;
  Tablet: TTablet;
  Capability: ICapability;
  Information: IInformation;
  screenBmp : TBitmap;
  bitmapData : PSafeArray;
  Buttons: Array of TMyButton;
  encodingMode1 : byte;
  m_isDown : Integer;
  points : TList;
  m_penDataOptionMode: Integer;
  m_penData: TList;
  m_encodingMode : encodingMode;

implementation

{$R *.dfm}

procedure TForm2.connect(usbDevice: IUsbDevice);
var
  errorCode  : IErrorCode;
  w1, w2, w3 : Integer;
  x, y, h, w : Integer;
  h1, h2, h3 : Integer;
  i : Integer;
  bArray : Array of Byte;
  ms : TMemoryStream;
  useColor : boolean;
  fontSize : Integer;
  currentPenDataOptionMode : Integer;
  idP : Word;
  eFlag : encodingFlag;
  ProtocolHelper: TProtocolHelper;

begin
  errorCode := Tablet.usbConnect(usbDevice, True);
  if (errorCode.value = 0) then
  begin
    Capability := Tablet.getCapability();
    Information := Tablet.getInformation();

    // First find out if the pad supports the pen data option mode (the 300 doesn't)
    currentPenDataOptionMode := getPenDataOptionMode();

    // Set up the tablet to return time stamp with the pen data or just basic data
    setPenDataOptionMode(currentPenDataOptionMode);

    Form2.AutoSize := true;
    Image1.Width  := Capability.screenWidth;
    Image1.Height := Capability.screenHeight;

    // Add the delagate that receives pen data.
    Tablet.OnonPenData := onPenData;
    Tablet.OnonPenDataTimeCountSequence := onPenDataTimeCountSequence;
    Tablet.OnonGetReportException := onGetReportException;

    SetLength(Buttons, 3);
    for i := 0 to 2 do
      Buttons[i] := TMyButton.Create;

    if (usbDevice.idProduct <> $000a2) then
    begin
      w2 := Capability.screenWidth div 3;
      w3 := Capability.screenWidth div 3;
      w1 := Capability.screenWidth - w2 - w3;
      y  := Capability.screenHeight * 6 div 7;
      h  := Capability.screenHeight - y;

      Buttons[0].Bounds := Rect(0, y, w1, y + h);
      Buttons[1].Bounds := Rect(w1, y, w1 + w2, y + h);
      Buttons[2].Bounds := Rect(w1 + w2, y, w1 + w2 + w3, y + h);

      fontSize := 25;
    end
    else
    begin
      // The STU-300 is very shallow, so it is better to utilise
      // the buttons to the side of the display instead.

      x := Capability.screenWidth * 3 div 4;
      w := Capability.screenWidth - x;

      h2 := Capability.screenHeight div 3;
      h3 := Capability.screenHeight div 3;
      h1 := Capability.screenHeight - h2 - h3;

      Buttons[0].Bounds := Rect(x, 0, x + w, h1);
      Buttons[1].Bounds := Rect(x, h1, x + w, h1 + h2);
      Buttons[2].Bounds := Rect(x, h1 + h2, x + w, h1 + h2 + h3);

      fontSize := 15;
    end;

    Buttons[0].Text := 'OK';
    Buttons[1].Text := 'Clear';
    Buttons[2].Text := 'Cancel';

    Buttons[0].Click := clickAccept;
    Buttons[1].Click := clickClear;
    Buttons[2].Click := clickCancel;

    // Disable color if the STU-520 bulk driver isn't installed.
    // This isn't necessary, but uploading colour images with out the driver
    // is very slow.
    idP := Tablet.getProductId;
    ProtocolHelper := TProtocolHelper.Create(self);
    ProtocolHelper.simulateEncodingFlag(idP,0);
    eFlag := ProtocolHelper.simulateEncodingFlag(idP, 0);
    useColor := false;

    if ((eFlag and (EncodingFlag_16bit or EncodingFlag_24bit)) <> 0) then
      begin
        if (Tablet.supportsWrite) then
          useColor := true;
      end;

    if ((eFlag and EncodingFlag_24bit) <> 0) then
        if (Tablet.supportsWrite) then
            m_encodingMode := EncodingMode_24bit_Bulk
        else
            m_encodingMode := EncodingMode_24bit
    else if ((eFlag and EncodingFlag_16bit) <> 0) then
        if (Tablet.supportsWrite) then
            m_encodingMode := EncodingMode_16bit_Bulk
        else
            m_encodingMode := EncodingMode_16bit
    else
      m_encodingMode := EncodingMode_1bit;

    screenBmp := TBitmap.Create;
    screenBmp.PixelFormat := pf24bit;
    screenBmp.Width := Capability.screenWidth;
    screenBmp.Height := Capability.screenHeight;

    screenBmp.Canvas.Font.Name := 'Courier New';
    screenBmp.Canvas.Font.Color := clBlack;
    screenBmp.Canvas.Font.Size := fontSize;

    with screenBmp.Canvas do
      begin
        Brush.Color := clWhite;
        FillRect(Rect(0, 0, Capability.screenWidth, Capability.screenHeight));

        for i := 0 to 2 do
          begin
            if (useColor) then
            begin
              Brush.Color := clSilver;
            end;  
            rectangle(Buttons[i].Bounds);
            DrawText(Handle, PChar(Buttons[i].Text), Length(Buttons[i].Text), Buttons[i].Bounds, DT_SINGLELINE or DT_CENTER or DT_VCENTER);
          end;
    end;

    // Finally, use this bitmap for the window background.
    Image1.Picture.Bitmap := screenBmp;

    // Now the bitmap has been created, it needs to be converted to device-native
    // format.

    // Unfortunately it is not possible for the native COM component to
    // understand Delphi bitmaps. We have therefore convert the Delphi bitmap
    // into a memory blob that will be understood by COM.
    ms := TMemoryStream.Create;
    try
      screenBmp.SaveToStream(ms);
      ms.Seek(0, 0);
      SetLength(bArray, ms.Size);
      ms.ReadBuffer(bArray[0], Length(bArray));
    finally
      ms.Free;
    end;

    bitmapData := ProtocolHelper.resizeAndFlatten(bArray, 0, 0, screenBmp.Width, screenBmp.Height, Capability.screenWidth, Capability.screenHeight, m_encodingMode, Scale_Fit, 0, 0);
    protocolHelper.Destroy;

    // Initialize the screen
    clearScreen();

    // Enable the pen data on the screen (if not already)
    Tablet.setInkingMode(1);

  end
  else
    raise Exception.Create(errorCode.message);
end;

procedure TForm2.FormCreate(Sender: TObject);
begin
  DoubleBuffered := true;
  Tablet := TTablet.Create(self);
  m_isDown := 0;
  points := TList.Create;
end;

procedure TForm2.onPenData(ASender: TObject; const pPenData: IPenData);
var
  pt     : TPoint;
  prev   : TPoint;
  btn    : Integer;
  i      : Integer;
  isDown : boolean;
begin
  btn := 0; // will be +ve if the pen is over a button.
  pt   := tabletToScreen(TMyPenData.Create(pPenData));

  for i := 0 to 2 do
  begin
    if (contains(Buttons[i].Bounds, pt)) then
    begin
      btn := i+1;
      Break;
    end;
  end;

  isDown := (pPenData.sw <> 0);

  // This code uses a model of four states the pen can be in:
  // down or up, and whether this is the first sample of that state.

  if (isDown) then
  begin
    if (m_isDown = 0) then
    begin
      // transition to down
      if (btn > 0) then
      begin
        // We have put the pen down on a button.
        // Track the pen without inking on the client.
        m_isDown := btn;
      end
      else
      begin
        m_isDown := -1;
      end;
    end
    else
    begin
      // already down, keep doing what we're doing!
    end;

    // draw
    if ((points.Count > 0) and (m_isDown = -1)) then
    begin
      // Draw a line from the previous down point to this down point.
      // This is the simplist thing you can do; a more sophisticated program
      // can perform higher quality rendering than this!

      if (pPenData.sw > 0) then
      begin
        prev := tabletToScreen(TMyPenData(points.Last()));
        DrawAntialisedLine(Image1.Canvas, prev.X, prev.Y, pt.X, pt.Y, clBlack);
      end;
    end;

    // The pen is down, store it for use later.
    if (m_isDown = -1) then
    begin
      points.Add(TMyPenData.Create(pPenData));
    end;
  end
  else
  begin
    if (m_isDown <> 0) then
    begin
      // transition to up
      if (btn > 0) then
      begin
        // The pen is over a button
        if (btn = m_isDown) then
        begin
          // The pen was pressed down over the same button as is was lifted now.
          // Consider that as a click!
          Buttons[btn - 1].Click();
        end;
      end;
      m_isDown := 0;
    end
    else
    begin
      // still up
    end;

    // Add up data once we have collected some down data.
    if (points.Count > 0) then
      points.Add(TMyPenData.Create(pPenData));
  end;

end;

procedure TForm2.onPenDataTimeCountSequence(ASender: TObject; const pPenData: IPenDataTimeCountSequence);
var
  pt     : TPoint;
  prev   : TPoint;
  btn    : Integer;
  i      : Integer;
  isDown : boolean;
begin
  btn := 0; // will be +ve if the pen is over a button.
  pt   := tabletToScreen(TMyPenData.Create(pPenData));

  for i := 0 to 2 do
  begin
    if (contains(Buttons[i].Bounds, pt)) then
    begin
      btn := i+1;
      Break;
    end;
  end;

  isDown := (pPenData.sw <> 0);

  // This code uses a model of four states the pen can be in:
  // down or up, and whether this is the first sample of that state.

  if (isDown) then
  begin
    if (m_isDown = 0) then
    begin
      // transition to down
      if (btn > 0) then
      begin
        // We have put the pen down on a button.
        // Track the pen without inking on the client.
        m_isDown := btn;
      end
      else
      begin
        m_isDown := -1;
      end;
    end
    else
    begin
      // already down, keep doing what we're doing!
    end;

    // draw
    if ((points.Count > 0) and (m_isDown = -1)) then
    begin
      // Draw a line from the previous down point to this down point.
      // This is the simplist thing you can do; a more sophisticated program
      // can perform higher quality rendering than this!

      if (pPenData.sw > 0) then
      begin
        prev := tabletToScreen(TMyPenData(points.Last()));
        DrawAntialisedLine(Image1.Canvas, prev.X, prev.Y, pt.X, pt.Y, clBlack);
      end;
    end;

    // The pen is down, store it for use later.
    if (m_isDown = -1) then
    begin
      points.Add(TMyPenData.Create(pPenData));
    end;
  end
  else
  begin
    if (m_isDown <> 0) then
    begin
      // transition to up
      if (btn > 0) then
      begin
        // The pen is over a button
        if (btn = m_isDown) then
        begin
          // The pen was pressed down over the same button as is was lifted now.
          // Consider that as a click!
          Buttons[btn - 1].Click();
        end;
      end;
      m_isDown := 0;
    end
    else
    begin
      // still up
    end;

    // Add up data once we have collected some down data.
    if (points.Count > 0) then
      points.Add(TMyPenData.Create(pPenData));
  end;

end;

procedure TForm2.clearScreen();
begin
  // note: There is no need to clear the tablet screen prior to writing an image.
  Tablet.writeImage(m_encodingMode, bitmapData);
  Image1.Picture.Bitmap := screenBmp;
  freePoints();
end;

function TForm2.tabletToScreen(penData: TMyPenData) : TPoint;
begin
  Result := Point(penData.x * Capability.screenWidth div Capability.tabletMaxX, penData.y * Capability.screenHeight div Capability.tabletMaxY);
end;

function TForm2.contains(bounds: TRect; point: TPoint) : boolean;
begin
  if (((point.X >= bounds.Left) and (point.X <= bounds.Right)) and ((point.Y >= bounds.Top) and (point.Y <= bounds.Bottom))) then
  begin
      Result := true;
  end
  else
  begin
    Result := false;
  end
end;

procedure TForm2.clickAccept();
begin
  // You probably want to add additional processing here.
  if (points.Count > 0) then
  begin
    saveImage();
    Form2.Close;
  end;  
end;

procedure TForm2.clickClear();
begin
  if (points.Count > 0) then
  begin
    clearScreen();
  end;
end;

procedure TForm2.clickCancel();
begin
  // You probably want to add additional processing here.
  Form2.Close;
end;

procedure TForm2.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  screenBmp.Free;
  freePoints();

  try
  begin
    tablet.setClearScreen;
    tablet.disconnect1;
  end;
  except
    on e : Exception do
      ShowMessage(e.Message);
  end;
end;

procedure TForm2.onGetReportException(ASender: TObject; const pException: ITabletEventsException);
begin
  try
    pException.getException;
  except
    on e : Exception do
    begin
      Form2.Close();
    end;
  end;
end;

Constructor TMyPenData.Create(penData: IPenData);
begin
  self.rdy      := penData.rdy;
  self.sw       := penData.sw;
  self.pressure := penData.pressure;
  self.x        := penData.x;
  self.y        := penData.y;
end;


procedure TForm2.DrawAntialisedLine(Canvas: TCanvas; const AX1, AY1, AX2, AY2: real; const LineColor: TColor);

var
  swapped: boolean;

  procedure plot(const x, y, c: real);
  var
    resclr: TColor;
  begin
    if swapped then
      resclr := Canvas.Pixels[round(y), round(x)]
    else
      resclr := Canvas.Pixels[round(x), round(y)];
      resclr := RGB(round(GetRValue(resclr) * (1-c) + GetRValue(LineColor) * c),
                    round(GetGValue(resclr) * (1-c) + GetGValue(LineColor) * c),
                    round(GetBValue(resclr) * (1-c) + GetBValue(LineColor) * c));
    if swapped then
      Canvas.Pixels[round(y), round(x)] := resclr
    else
      Canvas.Pixels[round(x), round(y)] := resclr;
  end;

  function rfrac(const x: real): real;
  begin
    rfrac := 1 - frac(x);
  end;

  procedure swap(var a, b: real);
  var
    tmp: real;
  begin
    tmp := a;
    a := b;
    b := tmp;
  end;

var
  x1, x2, y1, y2, dx, dy, gradient, xend, yend, xgap, xpxl1, ypxl1,
  xpxl2, ypxl2, intery: real;
  x: integer;

begin

  x1 := AX1;
  x2 := AX2;
  y1 := AY1;
  y2 := AY2;

  dx := x2 - x1;
  dy := y2 - y1;
  swapped := abs(dx) < abs(dy);
  if swapped then
  begin
    swap(x1, y1);
    swap(x2, y2);
    swap(dx, dy);
  end;
  if x2 < x1 then
  begin
    swap(x1, x2);
    swap(y1, y2);
  end;

  if (dx <> 0) then
  begin
    gradient := dy / dx;
  end
  else
  begin
    gradient := 0;
  end;

  xend := round(x1);
  yend := y1 + gradient * (xend - x1);
  xgap := rfrac(x1 + 0.5);
  xpxl1 := xend;
  ypxl1 := floor(yend);
  plot(xpxl1, ypxl1, rfrac(yend) * xgap);
  plot(xpxl1, ypxl1 + 1, frac(yend) * xgap);
  intery := yend + gradient;

  xend := round(x2);
  yend := y2 + gradient * (xend - x2);
  xgap := frac(x2 + 0.5);
  xpxl2 := xend;
  ypxl2 := floor(yend);
  plot(xpxl2, ypxl2, rfrac(yend) * xgap);
  plot(xpxl2, ypxl2 + 1, frac(yend) * xgap);

  for x := round(xpxl1) + 1 to round(xpxl2) - 1 do
  begin
    plot(x, floor(intery), rfrac(intery));
    plot(x, floor(intery) + 1, frac(intery));
    intery := intery + gradient;
  end;

end;

procedure TForm2.saveImage();
var
  i: Integer;
  act, prev: TMyPenData;
  bmp: TBitmap;
  point1, point2: TPoint;
begin

  with bmp do
  begin
    bmp := TBitmap.Create;
    bmp.PixelFormat := pf24bit;
    bmp.Width := Capability.screenWidth;
    bmp.Height := Capability.screenHeight;
    bmp.Canvas.Font.Color := clBlack;
  end;

  with bmp.Canvas do
  begin
    Brush.Color := clWhite;
    FillRect(Rect(0, 0, Capability.screenWidth, Capability.screenHeight));
  end;

  for i := 1 to points.Count do
  begin
    prev := TMyPenData(points[i-1]);
    if (prev.sw <> 0) then
    begin
      act  := TMyPenData(points[i]);
      point1 := tabletToScreen(prev);
      point2 := tabletToScreen(act);
      DrawAntialisedLine(bmp.Canvas, point1.x, point1.y, point2.x, point2.y, clBlack);
    end;
  end;

  bmp.SaveToFile('signature.bmp');
  bmp.Free;
end;

procedure TForm2.freePoints();
var
  i: Integer;
begin
for i := 1 to points.Count do
  begin
    TMyPenData(points[i-1]).Free;
  end;
  points.Clear();
end;

procedure TForm2.FormDestroy(Sender: TObject);
begin
  tablet.Free;
  points.Free;
end;

procedure TForm2.Image1Click(Sender: TObject);
var
  pt1, pt2 : TPoint;
  i        : Integer;
begin
  pt1 := Mouse.CursorPos;
  pt2 := Point(pt1.x - Image1.ClientOrigin.x,pt1.y - Image1.ClientOrigin.y);

  for i := 0 to 2 do
  begin
    if (contains(Buttons[i].Bounds, pt2)) then
    begin
      Buttons[i].Click();
      Break;
    end;
  end;
end;

function TForm2.getPenDataOptionMode() : Integer;
begin
  try
    begin
      getPenDataOptionMode := Tablet.getPenDataOptionMode();
    end;
    except
      on e : Exception do
        getPenDataOptionMode := -1;
    end;
end;

procedure TForm2.setPenDataOptionMode(currentPenDataOptionMode: Integer);
begin
  // If the current option mode is TimeCount then this is a 520 so we must reset the mode
  // to basic data only as there is no handler for TimeCount

  case currentPenDataOptionMode of
    -1:
        // This must be teh 300 which doesn't support getPenDataOptionMode at all so only basic data
        m_penDataOptionMode := PenDataOptionMode_None;
    PenDataOptionMode_None:
        // If the current option mode is "none" then it could be any pad so try setting the full option
        // and if it fails or ends up s TimeCount then set it to none
        try
          Tablet.setPenDataOptionMode(PenDataOptionMode_TimeCountSequence);
          m_penDataOptionMode := Tablet.getPenDataOptionMode();
          if (m_penDataOptionMode = PenDataOptionMode_TimeCount) then
            begin
              Tablet.setPenDataOptionMode(PenDataOptionMode_None);
              m_penDataOptionMode := PenDataOptionMode_None;
            end
          else
            begin
              m_penDataOptionMode := PenDataOptionMode_TimeCountSequence;
            end
        except
          on e : Exception do
            // This shouldn't happen but jus in case...
            m_penDataOptionMode := PenDataOptionMode_None;
        end;
    PenDataOptionMode_TimeCount:
        begin
          Tablet.setPenDataOptionMode(PenDataOptionMode_None);
          m_penDataOptionMode := PenDataOptionMode_None;
        end;
    PenDataOptionMode_TimeCountSequence:
        // If the current mode is timecountsequence then leave it at that
        m_penDataOptionMode := currentPenDataOptionMode;
  end;

end;

procedure TForm2.reset;
begin
end;

procedure TForm2.clearKeys;
begin
end;

function TForm2.getSymmetricKeyType: Byte;
begin
end;

function TForm2.getAsymmetricPaddingType: Byte;
begin
end;

function TForm2.getAsymmetricKeyType: Byte;
begin
end;

function TForm2.getPublicExponent: PSafeArray;
begin
end;

function TForm2.generatePublicKey: PSafeArray;
begin
end;

procedure TForm2.computeSessionKey(data: PSafeArray);
begin
end;

function TForm2.decrypt(data: PSafeArray): PSafeArray;
begin
end;

end.
