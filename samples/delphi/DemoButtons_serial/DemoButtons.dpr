program DemoButtons;

uses
  Forms,
  DemoButtonsForm in 'DemoButtonsForm.pas' {Form1},
  SignatureForm in 'SignatureForm.pas' {Form2},
  wgssSTU_TLB in 'wgssSTU_TLB.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.CreateForm(TForm2, Form2);
  Application.Run;
end.
