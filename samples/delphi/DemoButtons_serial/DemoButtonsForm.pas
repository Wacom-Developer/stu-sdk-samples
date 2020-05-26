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
    Label1: TLabel;
    portEdit: TEdit;
    Label2: TLabel;
    rateEdit: TEdit;
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
begin
  try
    begin
      Form2.connect(portEdit.Text, rateEdit.Text);
      Form2.ShowModal();
    end;
  except
    on e : Exception do
      ShowMessage(e.Message);
    end;
end;

end.
