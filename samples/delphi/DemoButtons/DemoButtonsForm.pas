{
 DemoButtonsForm.pas

 Controlling program for the DemoButtons program which allow user to input a signature on an STU
 and reproduces it on a Window on the PC.  Allows signature to be saved to file on disk

 Copyright (c) 2015 Wacom GmbH. All rights reserved.

}
unit DemoButtonsForm;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, wgssSTU_TLB, SignatureForm;

type
  TForm1 = class(TForm)
    Button1: TButton;
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1 : TForm1;

implementation

{$R *.dfm}

procedure TForm1.Button1Click(Sender: TObject);
var
  usbDevices : IUsbDevices;
  usbDevice  : IUsbDevice;
begin
    usbDevices := CoUsbDevices.Create();
    if (usbDevices.Count > 0) then
    begin
      try
        begin
          usbDevice := usbDevices.Item[0]; //select the first device
          Form2.connect(usbDevice);
          Form2.ShowModal();
        end;
      except
        on e : Exception do
          ShowMessage(e.Message);
      end;
    end
    else
    begin
      ShowMessage('No STU devices attached');
    end;
end;

end.
