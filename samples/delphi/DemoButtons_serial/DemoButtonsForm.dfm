object Form1: TForm1
  Left = 192
  Top = 124
  Caption = 'DemoButtons (Delphi)'
  ClientHeight = 129
  ClientWidth = 305
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 16
    Top = 16
    Width = 50
    Height = 13
    Caption = 'COM port:'
  end
  object Label2: TLabel
    Left = 18
    Top = 56
    Width = 52
    Height = 13
    Caption = 'Baud-rate:'
  end
  object Button1: TButton
    Left = 18
    Top = 96
    Width = 75
    Height = 25
    Caption = 'Demo'
    TabOrder = 0
    OnClick = Button1Click
  end
  object portEdit: TEdit
    Left = 80
    Top = 8
    Width = 121
    Height = 21
    TabOrder = 1
    Text = 'COM1'
  end
  object rateEdit: TEdit
    Left = 80
    Top = 53
    Width = 121
    Height = 21
    TabOrder = 2
    Text = '128000'
  end
end
