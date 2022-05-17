'
' DemoButtonsForm.vb
'
' Allow user to input a signature
'
' Copyright (c) 2015 Wacom GmbH. All rights reserved.
'
'
Public Class DemoButtonsForm

    Private Sub DemoButtonsForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
      print("Ready")
      radHID.Checked = True
   End Sub

   Private Sub btnSign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSign.Click

      If (radHID.Checked) Then
         captureSignatureHID()
      Else
         captureSignatureSerial()
      End If

   End Sub

   Private Sub captureSignatureHID()
      Dim usbDevices = New wgssSTU.UsbDevices()
      Dim serialInterface = New wgssSTU.SerialInterface()

      If (usbDevices.Count <> 0) Then
         Try
            print("Found " + usbDevices.Count.ToString() + " devices")
            Dim usbDevice = usbDevices(0)
            Dim demo = New SignatureForm(Me, usbDevice, serialInterface, txtCOM.Text, txtBaudRate.Text, True)
            Dim res = demo.ShowDialog()
            print("SignatureForm returned: " + res.ToString())
            If (res = DialogResult.OK) Then
               DisplaySignature(demo)
            End If

         Catch ex As Exception
            MsgBox("Exception: " + ex.Message)
         End Try

      Else
         print("No devices found")
      End If
   End Sub

   Private Sub captureSignatureSerial()

      Dim baudRate As Int32
      Dim fileNameCOMPort As String

      Dim usbDevice As wgssSTU.IUsbDevice
      Dim serialInterface = New wgssSTU.SerialInterface()

      fileNameCOMPort = txtCOM.Text
      baudRate = Int32.Parse(txtBaudRate.Text)

      Try
         Dim demo = New SignatureForm(Me, usbDevice, serialInterface, txtCOM.Text, txtBaudRate.Text, False)
         Dim Res As New DialogResult
         Res = demo.ShowDialog()
         print("SignatureForm returned: " + Res.ToString())
         If (Res = DialogResult.OK) Then
            DisplaySignature(demo)
            demo.Dispose()
         End If

      Catch ex As Exception
         MessageBox.Show("No STU devices attached: " + ex.Message)
      End Try

   End Sub

   Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
    Public Sub print(ByVal txt As String)
        txtDisplay.Text += txt + vbCrLf
        txtDisplay.SelectionStart = txtDisplay.Text.Length ' scroll to end
        txtDisplay.ScrollToCaret()
    End Sub
    Private Sub DisplaySignature(ByVal demo As SignatureForm)
      Dim bitmap As Bitmap

      bitmap = demo.GetSigImage()
      ' resize the image to fit the screen
      Dim scale = 1       ' halve the image size if needed
      If bitmap.Width > 400 Then
         scale = 2
      End If

      PictureBox1.Size = New Size(bitmap.Width / scale, bitmap.Height / scale)
      PictureBox1.Image = bitmap
      'centre the image in the panel
      Dim x, y As Integer
      x = Panel1.Location.X + ((Panel1.Width - Panel1.Width) / 2)
      y = Panel1.Location.Y + ((Panel1.Height - Panel1.Height) / 2)
      Me.Panel1.Location = New Point(x, y)

      Panel1.BringToFront()

      'bitmap.Save("C:\\temp\\sig.png", Imaging.ImageFormat.Png) ' to save the image to disk
   End Sub

   Private Sub radHID_CheckedChanged(sender As Object, e As EventArgs) Handles radHID.CheckedChanged
      radioSelectionCheck()
   End Sub

   Private Sub radSerial_CheckedChanged(sender As Object, e As EventArgs) Handles radSerial.CheckedChanged
      radioSelectionCheck()
   End Sub

   Private Sub radioSelectionCheck()

      If (radHID.Checked = True) Then
         txtCOM.Enabled = False
         txtBaudRate.Enabled = False
      Else
         txtCOM.Enabled = True
         For Each serialPort As String In My.Computer.Ports.SerialPortNames
            txtCOM.Text = serialPort
         Next

         txtBaudRate.Enabled = True
         txtBaudRate.Text = "128000"
         txtCOM.Focus()
      End If
   End Sub
End Class
