'******************************************************* 
'
'  Form1.cs
'  
'  Allow user to select an image and send it to the STU
'
'  Copyright (c) 2015 Wacom GmbH. All rights reserved.
'  
'*******************************************************
Public Class Form1

    Dim tablet As wgssSTU.Tablet
    Dim capability As wgssSTU.ICapability
    Dim capability2 As wgssSTU.ICapability2
    Dim information As wgssSTU.IInformation

    Private Sub btnSendToSTU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendToSTU.Click
        print("SendToSTU")
        SendToSTU()
    End Sub
    Private Function Connect()
        Dim connected = False
        Try
            Dim usbDevices = New wgssSTU.UsbDevices()
            If (usbDevices.Count = 0) Then
                print("No devices found")
            Else
                connected = True
                tablet = New wgssSTU.Tablet()
                Dim ec = tablet.usbConnect(usbDevices(0), True)
                If (ec.value = 0) Then
                    capability = tablet.getCapability()
                    capability2 = tablet.getCapability_2()
                    information = tablet.getInformation()
                    print("Tablet: " + information.modelName)
                Else
                    Throw New Exception(ec.message)
                End If
            End If
        Catch ex As Exception
            print("Exception: " + ex.Message)
        End Try
        Return connected
    End Function
    Private Sub Disconnect()
        Try
            If Not (tablet Is Nothing) Then
                tablet.disconnect()
            End If
        Catch ex As Exception
            print("Exception: " + ex.Message)
        End Try
    End Sub
    Private Sub SendToSTU()
        Try
            If Connect() <> True Then
                Return
            End If

            print("Connected: " + information.modelName)
            Dim protocolHelper = New wgssSTU.ProtocolHelper()
            Dim encodingFlag As wgssSTU.encodingFlag = 0
            Dim encodingMode As wgssSTU.encodingMode = 0

            Dim idP = tablet.getProductId()

            encodingFlag = capability2.encodingFlag
            encodingFlag = protocolHelper.simulateEncodingFlag(idP, encodingFlag)

            print("Encoding flag: " + encodingFlag.ToString())
            If (encodingFlag And wgssSTU.encodingFlag.EncodingFlag_24bit) Then
                If (tablet.supportsWrite()) Then
                    encodingMode = wgssSTU.encodingMode.EncodingMode_24bit_Bulk
                Else
                    encodingMode = wgssSTU.encodingMode.EncodingMode_24bit
                End If

            ElseIf (encodingFlag And wgssSTU.encodingFlag.EncodingFlag_16bit) Then
                If (tablet.supportsWrite()) Then
                    encodingMode = wgssSTU.encodingMode.EncodingMode_16bit_Bulk
                Else
                    encodingMode = wgssSTU.encodingMode.EncodingMode_16bit
                End If
            Else
                ' assumes 1bit is available
                encodingMode = wgssSTU.encodingMode.EncodingMode_1bit
            End If
            print("encodingMode: " + encodingMode.ToString())

            Dim bitmapData  '// This is the flattened data of the bitmap that we send to the device.
            Dim stream As New System.IO.MemoryStream()
            PictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png)

            bitmapData = protocolHelper.resizeAndFlatten(stream.ToArray(),
                            0, 0, 0, 0, capability.screenWidth, capability.screenHeight, CByte(encodingMode), wgssSTU.Scale.Scale_Fit, False, 0)
            tablet.writeImage(encodingMode, bitmapData)

            Disconnect()
        Catch ex As Exception
            print("Exception: " + ex.Message)
        End Try

    End Sub

    Private Sub print(ByVal txt)
        txtDisplay.Text += txt + vbCrLf
        txtDisplay.SelectionStart = txtDisplay.Text.Length ' scroll to end
        txtDisplay.ScrollToCaret()
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            If Connect() <> True Then
                Return
            End If
            tablet.setClearScreen()
            print("Cleared " + information.modelName)

            Disconnect()
        Catch ex As Exception
            print("Exception: " + ex.Message)
        End Try

    End Sub
    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim res
        res = OpenFileDialog1.ShowDialog()
        If (res = DialogResult.OK) Then
            txtFilename.Text = OpenFileDialog1.FileName
            DisplayImage(Image.FromFile(OpenFileDialog1.FileName))
        End If

    End Sub
    Private Sub DisplayImage(ByVal img As Image)
        ' resize the image to fit the panel
        ' STU displays: 300:396x100 430:320x200 500:640x480 520:800x480 530:800x480
        ' 300/420 scale by 2, else scale by 4. Also handle unexpected size.
        Dim scale = 1
        If (img.Width > 400) Then
            scale = 4
        ElseIf (img.Width > Panel1.Width) Then
            scale = 2
        End If

        PictureBox1.Size = New Size(img.Width / scale, img.Height / scale)
        ' don't exceed the panel size:
        If (PictureBox1.Size.Width > Panel1.Size.Width Or PictureBox1.Size.Height > Panel1.Size.Height) Then
            PictureBox1.Size = Panel1.Size
        End If

        ' centre the image in the panel
        Dim x, y
        x = Panel1.Location.X + ((Panel1.Width - PictureBox1.Width) / 2)
        y = Panel1.Location.Y + ((Panel1.Height - PictureBox1.Height) / 2)
        PictureBox1.Location = New Point(x, y)

        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox1.Image = img
        PictureBox1.Parent = Me
        PictureBox1.BringToFront()

    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DisplayImage(My.Resources._640x480) ' use resource to simplify and avoid locating a folder
        print("Ready")
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class
