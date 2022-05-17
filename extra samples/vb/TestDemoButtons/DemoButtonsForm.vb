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
    End Sub

    Private Sub btnSign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSign.Click
        Dim usbDevices = New wgssSTU.UsbDevices()
        If (usbDevices.Count <> 0) Then
            Try
                print("Found " + usbDevices.Count.ToString() + " devices")
                Dim usbDevice = usbDevices(0)
                Dim demo = New SignatureForm(Me, usbDevice)
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
        Dim scale = 2       ' halve or quarter the image size
        If bitmap.Width > 400 Then
            scale = 4
        End If
        PictureBox1.Size = New Size(bitmap.Width / scale, bitmap.Height / scale)
        PictureBox1.Image = bitmap
        'centre the image in the panel
        Dim x, y As Integer
        x = Panel1.Location.X + ((Panel1.Width - PictureBox1.Width) / 2)
        y = Panel1.Location.Y + ((Panel1.Height - PictureBox1.Height) / 2)
        Me.PictureBox1.Location = New Point(x, y)

        PictureBox1.BringToFront()

        'bitmap.Save("C:\\temp\\sig.png", Imaging.ImageFormat.Png) ' to save the image to disk
    End Sub
End Class
