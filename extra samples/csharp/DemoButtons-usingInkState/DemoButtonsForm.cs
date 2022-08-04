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

          SignatureForm demo = new SignatureForm(this, usbDevice);
          demo.ShowDialog();
          List<wgssSTU.IPenData> penData = demo.getPenData();          
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
    public void print(string txt)
    {
        txtDisplay.Text += txt + "\r\n";
        txtDisplay.SelectionStart = txtDisplay.Text.Length; // scroll to end
        txtDisplay.ScrollToCaret();

    }
  }
}
