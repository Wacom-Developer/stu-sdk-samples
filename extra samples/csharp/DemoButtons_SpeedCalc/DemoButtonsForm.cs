/*
 DemoButtonsForm.cs

 Controlling program for the DemoButtons program which allow user to input a signature on an STU
 and reproduces it on a Window on the PC.  Allows signature to be saved to file on disk

 Copyright (c) 2015 Wacom GmbH. All rights reserved.

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
      wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
      if (usbDevices.Count != 0)
      {
        try
        {
          wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

          SignatureForm demo = new SignatureForm(usbDevice);
          demo.ShowDialog();
          List<wgssSTU.IPenDataTimeCountSequence> penData = demo.getPenData();          
          if (penData != null)
          {            
            // process penData here!

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
