/*
 DemoButtonsForm.cs

 Controlling program for the DemoButtons program which allow user to input a signature on an STU
 and reproduces it on a Window on the PC

 Copyright (c) 2015 Wacom GmbH. All rights reserved.

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

enum PenDataOptionMode
{
   PenDataOptionMode_None,
   PenDataOptionMode_TimeCount,
   PenDataOptionMode_SequenceNumber,
   PenDataOptionMode_TimeCountSequence
};

namespace DemoButtons
{
  public partial class DemoButtonsForm : Form
  {
    public DemoButtonsForm()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int penDataType;
      List<wgssSTU.IPenDataTimeCountSequence> penTimeData = null;
      List<wgssSTU.IPenData> penData = null;

      wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
      if (usbDevices.Count != 0)
      {
        try
        {
          wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

          SignatureForm demo = new SignatureForm(usbDevice, chkUseEncryption.Checked);
          demo.ShowDialog();
          penDataType = demo.penDataType;

          if (penDataType == (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence )
             penTimeData = demo.getPenTimeData();          
          else
             penData = demo.getPenData();

          if (penData != null || penTimeData != null)
          {
             // process penData here!

             if (penData != null)
                txtPenDataCount.Text = penData.Count.ToString();
             else
                txtPenDataCount.Text = penTimeData.Count.ToString();

            wgssSTU.IInformation information = demo.getInformation();
            wgssSTU.ICapability capability = demo.getCapability();
          }
          demo.Dispose();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
      else
      {
        MessageBox.Show("No STU devices attached");       
      }      
    }
  }
}
