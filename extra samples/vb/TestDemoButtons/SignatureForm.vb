'
' SignatureForm.vb
'
' Display signature input form on PC screen and on STU pad and process user input
'
' Copyright (c) 2022 Wacom Ltd. All rights reserved
'
Enum PenDataOptionMode
  PenDataOptionMode_None
  PenDataOptionMode_TimeCount
  PenDataOptionMode_SequenceNumber
  PenDataOptionMode_TimeCountSequence
End Enum

Public Class SignatureForm

    Private m_tablet As wgssSTU.Tablet
    Private m_capability As wgssSTU.ICapability
    Private m_information As wgssSTU.IInformation

    ' In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
    ' Using an array of these makes it easy to add or remove buttons as desired.
    Private Delegate Sub ButtonClick()
    Private Structure Button
        Public Bounds As Rectangle 'in Screen coordinates
        Public Text As String
        Public Click As EventHandler
        Public Sub PerformClick()
            Click(Me, Nothing)
        End Sub
    End Structure

    Private m_penInk As Pen   ' cached object.

    ' The isDown flag is used like this:
    ' 0 = up
    ' +ve = down, pressed on button number
    ' -1 = down, inking
    ' -2 = down, ignoring
    Private m_isDown As Int16

    Private m_penData As List(Of wgssSTU.IPenData) ' Array of data being stored. This can be subsequently used as desired.
    Private m_penTimeData As List(Of wgssSTU.IPenDataTimeCountSequence) ' Array of data being stored. This can be subsequently used as desired.
    Private m_penDataOptionMode As Int16 ' The pen data Option mode flag - basic Or With time And sequence counts
    Private m_btns() As Button  ' The array of buttons that we are emulating.

    Private m_bitmap As Bitmap      ' This bitmap that we display on the screen.
    Private m_encodingMode As Byte  ' How we send the bitmap to the device.
    Private m_bitmapData() As Byte  ' This is the flattened data of the bitmap that we send to the device.

    Private m_ParentForm As DemoButtonsForm ' link to calling form

    ' SignatureForm
    Public Sub New(ByVal ParentForm As DemoButtonsForm, ByVal usbDevice As wgssSTU.IUsbDevice)
    m_ParentForm = ParentForm
    Dim currentPenDataOptionMode As Int16

    Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0F, 96.0F)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi

        InitializeComponent() ' This call is required by the Windows Form Designer.

        m_penData = New List(Of wgssSTU.IPenData)

        m_tablet = New wgssSTU.Tablet()
        Dim protocolHelper = New wgssSTU.ProtocolHelper()

        Dim ec = m_tablet.usbConnect(usbDevice, True)
        If (ec.value = 0) Then
            m_capability = m_tablet.getCapability()
            m_information = m_tablet.getInformation()
            print("Connected " + m_information.modelName)
            
            currentPenDataOptionMode = getPenDataOptionMode(m_tablet, ParentForm)

            ' Set up the tablet to return time stamp with the pen data Or just basic data
            setPenDataOptionMode(currentPenDataOptionMode)
        Else
            Throw New Exception(ec.message)
        End If

        Me.SuspendLayout()
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0F, 96.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi

        ' Set the size of the client window to be actual size, 
        ' based on the reported DPI of the monitor.

        Dim clientSize = New Size((m_capability.tabletMaxX / 2540.0F * 96.0F), (m_capability.tabletMaxY / 2540.0F * 96.0F))
        Me.ClientSize = clientSize
        Me.ResumeLayout()

        ReDim m_btns(3)

        If (usbDevice.idProduct <> &HA2) Then
            ' Place the buttons across the bottom of the screen.
            Dim w2 = m_capability.screenWidth / 3
            Dim w3 = m_capability.screenWidth / 3
            Dim w1 = m_capability.screenWidth - w2 - w3
            Dim y = m_capability.screenHeight * 6 / 7
            Dim h = m_capability.screenHeight - y

            m_btns(0).Bounds = New Rectangle(0, y, w1, h)
            m_btns(1).Bounds = New Rectangle(w1, y, w2, h)
            m_btns(2).Bounds = New Rectangle(w1 + w2, y, w3, h)

        Else
            ' The STU-300 is very shallow, so it is better to utilise
            ' the buttons to the side of the display instead.

            Dim x = m_capability.screenWidth * 3 / 4
            Dim w = m_capability.screenWidth - x

            Dim h2 = m_capability.screenHeight / 3
            Dim h3 = m_capability.screenHeight / 3
            Dim h1 = m_capability.screenHeight - h2 - h3

            m_btns(0).Bounds = New Rectangle(x, 0, w, h1)
            m_btns(1).Bounds = New Rectangle(x, h1, w, h2)
            m_btns(2).Bounds = New Rectangle(x, h1 + h2, w, h3)
        End If
        m_btns(0).Text = "OK"
        m_btns(1).Text = "Clear"
        m_btns(2).Text = "Cancel"
        m_btns(0).Click = New EventHandler(AddressOf btnOK_Click)
        m_btns(1).Click = New EventHandler(AddressOf btnClear_Click)
        m_btns(2).Click = New EventHandler(AddressOf btnCancel_Click)


        ' Disable color if the bulk driver isn't installed.
        ' This isn't necessary, but uploading colour images with out the driver
        ' is very slow.

        ' Calculate the encodingMode that will be used to update the image

        Dim idP = m_tablet.getProductId()
        Dim encodingFlag = protocolHelper.simulateEncodingFlag(idP, 0)
        Dim useColor = False
        If (encodingFlag And (wgssSTU.encodingFlag.EncodingFlag_16bit Or wgssSTU.encodingFlag.EncodingFlag_24bit)) Then
            If (m_tablet.supportsWrite()) Then
                useColor = True
            End If
        End If
        If (encodingFlag And wgssSTU.encodingFlag.EncodingFlag_24bit) Then
            If (m_tablet.supportsWrite()) Then
                m_encodingMode = wgssSTU.encodingMode.EncodingMode_24bit_Bulk
            Else
                m_encodingMode = wgssSTU.encodingMode.EncodingMode_24bit
            End If

        ElseIf (encodingFlag And wgssSTU.encodingFlag.EncodingFlag_16bit) Then
            If (m_tablet.supportsWrite()) Then
                m_encodingMode = wgssSTU.encodingMode.EncodingMode_16bit_Bulk
            Else
                m_encodingMode = wgssSTU.encodingMode.EncodingMode_16bit
            End If
        Else
            ' assumes 1bit is available
            m_encodingMode = wgssSTU.encodingMode.EncodingMode_1bit
        End If


        ' Size the bitmap to the size of the LCD screen.
        ' This application uses the same bitmap for both the screen and client (window).
        ' However, at high DPI, this bitmap will be stretch and it would be better to 
        ' create individual bitmaps for screen and client at native resolutions.
        m_bitmap = New Bitmap(m_capability.screenWidth, m_capability.screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        Dim gfx = Graphics.FromImage(m_bitmap)
        gfx.Clear(Color.White)

        ' Uses pixels for units as DPI won't be accurate for tablet LCD.
        Dim font = New Font(FontFamily.GenericSansSerif, m_btns(0).Bounds.Height / 2.0F, GraphicsUnit.Pixel)
        Dim sf = New StringFormat()
        sf.Alignment = StringAlignment.Center
        sf.LineAlignment = StringAlignment.Center

        If (useColor) Then
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
        Else
            ' Anti-aliasing should be turned off for monochrome devices.
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel
        End If

        ' Draw the buttons
        For i As Integer = 0 To (m_btns.Length - 1)
            If (useColor) Then
                gfx.FillRectangle(Brushes.LightGray, m_btns(i).Bounds)
            End If
            gfx.DrawRectangle(Pens.Black, m_btns(i).Bounds)
            gfx.DrawString(m_btns(i).Text, font, Brushes.Black, m_btns(i).Bounds, sf)
        Next


        gfx.Dispose()
        font.Dispose()

        ' Finally, use this bitmap for the window background.
        Me.BackgroundImage = m_bitmap
        Me.BackgroundImageLayout = ImageLayout.Stretch

        ' Now the bitmap has been created, it needs to be converted to device-native
        ' format.

        ' Unfortunately it is not possible for the native COM component to
        ' understand .NET bitmaps. We have therefore convert the .NET bitmap
        ' into a memory blob that will be understood by COM.

        Dim stream = New System.IO.MemoryStream()
        m_bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png)
        m_bitmapData = protocolHelper.resizeAndFlatten(stream.ToArray(),
                            0, 0, CUInt(m_bitmap.Width), CUInt(m_bitmap.Height),
                            m_capability.screenWidth, m_capability.screenHeight,
                            CByte(m_encodingMode),
                            wgssSTU.Scale.Scale_Fit, 0, 0)

        protocolHelper = Nothing

        stream.Dispose()

        ' If you wish to further optimize image transfer, you can compress the image using 
        ' the Zlib algorithm.

        Dim useZlibCompression = False
        If ((Not useColor) And useZlibCompression) Then
            ' m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
            m_encodingMode = m_encodingMode Or wgssSTU.encodingMode.EncodingMode_Zlib
        End If

        ' Calculate the size and cache the inking pen.

        Dim s = Me.AutoScaleDimensions
        Dim inkWidthMM = 0.7F
        m_penInk = New Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2.0F))
        m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round
        m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round


        ' Add the delagate that receives pen data.
        Select Case m_penDataOptionMode
           Case PenDataOptionMode.PenDataOptionMode_TimeCountSequence
              m_ParentForm.print("Setting up pen timed data handler")
              AddHandler m_tablet.onPenDataTimeCountSequence, New wgssSTU.ITabletEvents2_onPenDataTimeCountSequenceEventHandler(AddressOf onPenDataTimeCountSequence)
           Case PenDataOptionMode.PenDataOptionMode_TimeCount
              m_ParentForm.print("Setting up pen timed data handler")
              AddHandler m_tablet.onPenData, New wgssSTU.ITabletEvents2_onPenDataEventHandler(AddressOf onPenData)
           Case Else
              m_ParentForm.print("Setting up basic pen data handler")
              AddHandler m_tablet.onPenData, New wgssSTU.ITabletEvents2_onPenDataEventHandler(AddressOf onPenData)
        End Select

        AddHandler m_tablet.onGetReportException, New wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(AddressOf onGetReportException)

    ' Initialize the screen
    clearScreen(m_tablet)

    ' Enable the pen data on the screen (if not already)
    m_tablet.setInkingMode(&H1)

    End Sub
    
    Private Function getPenDataOptionMode(ByVal tablet As wgssSTU.Tablet, ByVal parentForm As DemoButtonsForm)
      Dim penDataOptionMode As Int16

      Try
         penDataOptionMode = tablet.getPenDataOptionMode()
      Catch optionModeException As Exception
         m_ParentForm.print("Tablet doesn't support getPenDataOptionMode: " + optionModeException.Message)
         penDataOptionMode = -1
      End Try
      parentForm.print("Pen data option mode: " + m_penDataOptionMode.ToString())

      Return penDataOptionMode
   End Function

   Private Sub setPenDataOptionMode(ByVal currentPenDataOptionMode As Int16)
      ' If the current option mode Is TimeCount then this Is a 520 so we must reset the mode
      ' to basic data only as there Is no handler for TimeCount

      m_ParentForm.print("current mode: " + currentPenDataOptionMode.ToString())

      Select Case (currentPenDataOptionMode)
         Case -1
            ' THis must be the 300 which doesn't support getPenDataOptionMode at all so only basic data
            m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_None

         Case PenDataOptionMode.PenDataOptionMode_None
            ' If the current option mode Is "none" then it could be any pad so try setting the full option
            ' And if it fails Or ends up as TimeCount then set it to none
            Try
               m_tablet.setPenDataOptionMode(wgssSTU.penDataOptionMode.PenDataOptionMode_TimeCountSequence)
               m_penDataOptionMode = m_tablet.getPenDataOptionMode()
               If (m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCount) Then
                  m_tablet.setPenDataOptionMode(wgssSTU.penDataOptionMode.PenDataOptionMode_None)
                  m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_None
               Else
                  m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence
               End If
            Catch ex As Exception
               '  THis shouldn't happen but just in case...
               m_ParentForm.print("Using basic pen data")
               m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_None
            End Try

         Case PenDataOptionMode.PenDataOptionMode_TimeCount
            m_tablet.setPenDataOptionMode(wgssSTU.penDataOptionMode.PenDataOptionMode_None)
            m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_None

         Case PenDataOptionMode.PenDataOptionMode_TimeCountSequence
            ' If the current mode Is timecountsequence then leave it at that
            m_penDataOptionMode = currentPenDataOptionMode
      End Select

      Select Case m_penDataOptionMode
         Case PenDataOptionMode.PenDataOptionMode_None
            m_penData = New List(Of wgssSTU.IPenData)
            m_ParentForm.print("None")
         Case PenDataOptionMode.PenDataOptionMode_TimeCount
            m_penData = New List(Of wgssSTU.IPenData)
            m_ParentForm.print("Time count")
         Case PenDataOptionMode.PenDataOptionMode_SequenceNumber
            m_penData = New List(Of wgssSTU.IPenData)
            m_ParentForm.print("Seq number")
         Case PenDataOptionMode.PenDataOptionMode_TimeCountSequence
            m_penTimeData = New List(Of wgssSTU.IPenDataTimeCountSequence)
            m_ParentForm.print("Time count + seq")
         Case Else
            m_penData = New List(Of wgssSTU.IPenData)
      End Select
   End Sub
   
    Private Function tabletToClient(ByVal penData As wgssSTU.IPenData)

        ' Client means the Windows Form coordinates.
        'Return New PointF(penData.x * Me.ClientSize.Width / m_capability.tabletMaxX, penData.y * Me.ClientSize.Height / m_capability.tabletMaxY)
        Return New PointF(CDbl(penData.x) * CDbl(Me.ClientSize.Width) / CDbl(m_capability.tabletMaxX), CDbl(penData.y) * CDbl(Me.ClientSize.Height) / CDbl(m_capability.tabletMaxY))
    End Function

    Private Function tabletToClientTimed(ByVal penData As wgssSTU.IPenDataTimeCountSequence)

       ' Client means the Windows Form coordinates.
       'Return New PointF(penData.x * Me.ClientSize.Width / m_capability.tabletMaxX, penData.y * Me.ClientSize.Height / m_capability.tabletMaxY)
       Return New PointF(CDbl(penData.x) * CDbl(Me.ClientSize.Width) / CDbl(m_capability.tabletMaxX), CDbl(penData.y) * CDbl(Me.ClientSize.Height) / CDbl(m_capability.tabletMaxY))
    End Function
   
    Private Function tabletToScreen(ByVal penData As wgssSTU.IPenData)
        ' Screen means LCD screen of the tablet.
        Return Point.Round(New PointF(CDbl(penData.x) * CDbl(m_capability.screenWidth) / CDbl(m_capability.tabletMaxX), CDbl(penData.y) * CDbl(m_capability.screenHeight) / CDbl(m_capability.tabletMaxY)))
    End Function

    Private Function clientToScreen(ByVal pt As Point)

        'client (window) coordinates to LCD screen coordinates. 
        ' This is needed for converting mouse coordinates into LCD bitmap coordinates as that's
        ' what this application uses as the coordinate space for buttons.
        Return Point.Round(New PointF(CDbl(pt.X) * CDbl(m_capability.screenWidth) / CDbl(Me.ClientSize.Width), CDbl(pt.Y) * CDbl(m_capability.screenHeight) / CDbl(Me.ClientSize.Height)))
    End Function

  Private Sub clearScreen(ByRef tablet As wgssSTU.Tablet)
    ' note: There is no need to clear the tablet screen prior to writing an image.
    ' m_tablet.writeImage(m_encodingMode, m_bitmapData)

    Try
      If (tablet IsNot Nothing) Then
        tablet.writeImage(m_encodingMode, m_bitmapData)
      End If
    Catch ex As Exception
      MessageBox.Show(ex.Message)
    End Try

    If (m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence) Then
      m_penTimeData.Clear()
    Else
      m_penData.Clear()
    End If
    m_isDown = 0
    Me.Invalidate()
  End Sub


  Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If (m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence) Then
           Me.m_penTimeData.Clear()
        Else
           Me.m_penData.Clear()
        End If
        
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
      If (m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence) Then
         clearSignature(m_penTimeData.Count, m_tablet)
      Else
         clearSignature(m_penData.Count, m_tablet)
      End If
    End Sub

    Private Sub clearSignature(ByVal penDataCount As Int32, ByVal tablet As wgssSTU.Tablet)
       If (penDataCount <> 0) Then
          clearScreen(m_tablet)
       End If
    End Sub
   
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
      If (m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence) Then
         closeProgramOK(m_penTimeData.Count)
      Else
         closeProgramOK(m_penData.Count)
      End If
    End Sub

    Private Sub closeProgramOK(ByVal penDataCount As Int32)
       If (penDataCount > 0) Then
          Me.DialogResult = DialogResult.OK
          Me.Close()
       Else
          MessageBox.Show("No signature provided")
       End If
    End Sub
   
    Private Sub SignatureForm_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseClick
        ' Enable the mouse to click on the simulated buttons that we have displayed.

        ' Note that this can add some tricky logic into processing pen data
        ' if the pen was down at the time of this click, especially if the pen was logically
        ' also 'pressing' a button! This demo however ignores any that.

        Dim pt = clientToScreen(e.Location)
        For Each btn In m_btns
            If (btn.Bounds.Contains(pt)) Then
                btn.PerformClick()
                Exit For
            End If
        Next

    End Sub

    Private Sub SignatureForm_Closed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        ' Ensure that you correctly disconnect from the tablet, otherwise you are 
        ' likely to get errors when wanting to connect a second time.
        If (m_tablet IsNot Nothing) Then
            If m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence Then
               RemoveHandler m_tablet.onPenData, New wgssSTU.ITabletEvents2_onPenDataEventHandler(AddressOf  onPenDataTimeCountSequence)
            Else
               RemoveHandler m_tablet.onPenData, New wgssSTU.ITabletEvents2_onPenDataEventHandler(AddressOf onPenData)
            End If
            RemoveHandler m_tablet.onGetReportException, New wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(AddressOf onGetReportException)

            m_tablet.setInkingMode(&H0)
            m_tablet.setClearScreen()
            m_tablet.disconnect()
        End If

        m_penInk.Dispose()

    End Sub
    Private Sub onGetReportException(ByVal tabletEventsException As wgssSTU.ITabletEventsException)
        Try
            tabletEventsException.getException()
        Catch e As Exception
            print("Error: " + e.Message)
            MessageBox.Show("Error: " + e.Message)
            m_tablet.disconnect()
            m_tablet = Nothing
            m_penData = Nothing
            Me.Close()
        End Try
    End Sub
    Private Sub onPenData(ByVal penData As wgssSTU.IPenData) ' Process incoming pen data

        Dim pt = tabletToScreen(penData)

        Dim btn = 0 ' will be +ve if the pen is over a button.

        For i As Integer = 0 To (m_btns.Length - 1)
            If (m_btns(i).Bounds.Contains(pt)) Then
                btn = i + 1
                Exit For
            End If
        Next


        Dim isDown = (penData.sw <> 0)

        ' This code uses a model of four states the pen can be in:
        ' down or up, and whether this is the first sample of that state.

        If (isDown) Then

            If (m_isDown = 0) Then

                ' transition to down
                If (btn > 0) Then
                    ' We have put the pen down on a button.
                    ' Track the pen without inking on the client.
                    m_isDown = btn
                Else
                    ' We have put the pen down somewhere else.
                    ' Treat it as part of the signature.
                    m_isDown = -1
                End If
            Else
                ' already down, keep doing what we're doing!
            End If

            ' draw
            If (m_penData.Count <> 0 And m_isDown = -1) Then

                ' Draw a line from the previous down point to this down point.
                ' This is the simplist thing you can do; a more sophisticated program
                ' can perform higher quality rendering than this!

                Dim gfx = Me.CreateGraphics()
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality

                Dim prevPenData = m_penData(m_penData.Count - 1)

                Dim prev = tabletToClient(prevPenData)

                gfx.DrawLine(m_penInk, prev, tabletToClient(penData))
                gfx.Dispose()
            End If

            ' The pen is down, store it for use later.
            If (m_isDown = -1) Then
                m_penData.Add(penData)
            End If
        Else
            If (m_isDown <> 0) Then
                ' transition to up
                If (btn > 0) Then
                    ' The pen is over a button
                    If (btn = m_isDown) Then
                        ' The pen was pressed down over the same button as is was lifted now. 
                        ' Consider that as a click!
                        m_btns(btn - 1).PerformClick()
                    End If
                End If
                m_isDown = 0
            Else
                ' still up
            End If

            ' Add up data once we have collected some down data.
            If (m_penData.Count <> 0) Then
                m_penData.Add(penData)
            End If
        End If
    End Sub

   Private Sub onPenDataTimeCountSequence(ByVal penTimeData As wgssSTU.IPenDataTimeCountSequence) ' Process incoming pen data
        Dim penSequence As UInt16
        Dim penTimeStamp As UInt16
        Dim penPressure As UInt16
        Dim x As UInt16
        Dim y As UInt16

        penPressure = penTimeData.pressure
        penTimeStamp = penTimeData.timeCount
        penSequence = penTimeData.sequence
        x = penTimeData.x
        y = penTimeData.y

        Dim pt = tabletToScreen(penTimeData)

        Dim btn = 0 ' will be +ve if the pen is over a button.

        For i As Integer = 0 To (m_btns.Length - 1)
           If (m_btns(i).Bounds.Contains(pt)) Then
              btn = i + 1
              Exit For
           End If
        Next


        Dim isDown = (penTimeData.sw <> 0)

        ' This code uses a model of four states the pen can be in:
        ' down or up, and whether this is the first sample of that state.

        If (isDown) Then

           If (m_isDown = 0) Then

              ' transition to down
              If (btn > 0) Then
                 ' We have put the pen down on a button.
                 ' Track the pen without inking on the client.
                 m_isDown = btn
              Else
                 ' We have put the pen down somewhere else.
                 ' Treat it as part of the signature.
                 m_isDown = -1
              End If
           Else
              ' already down, keep doing what we're doing!
           End If

           ' draw
           If (m_penTimeData.Count <> 0 And m_isDown = -1) Then

              ' Draw a line from the previous down point to this down point.
              ' This is the simplist thing you can do; a more sophisticated program
              ' can perform higher quality rendering than this!

              Dim gfx = Me.CreateGraphics()
              gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
              gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High
              gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
              gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality

              Dim prevPenData = m_penTimeData(m_penTimeData.Count - 1)

              Dim prev = tabletToClientTimed(prevPenData)

              gfx.DrawLine(m_penInk, prev, tabletToClientTimed(penTimeData))
              gfx.Dispose()
           End If

           ' The pen is down, store it for use later.
           If (m_isDown = -1) Then
              m_penTimeData.Add(penTimeData)
           End If
        Else
           If (m_isDown <> 0) Then
              ' transition to up
              If (btn > 0) Then
                 ' The pen is over a button
                 If (btn = m_isDown) Then
                    ' The pen was pressed down over the same button as is was lifted now. 
                    ' Consider that as a click!
                    m_btns(btn - 1).PerformClick()
                 End If
              End If
              m_isDown = 0
           Else
              ' still up
           End If

           ' Add up data once we have collected some down data.
           If (m_penTimeData.Count <> 0) Then
              m_penTimeData.Add(penTimeData)
           End If
        End If
     End Sub

  Private Sub SignatureForm_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
    ' Redraw all the pen data up until now!

    If m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence Then
      RenderPenTimeData(e)
    Else
      RenderPenData(e)
    End If
  End Sub

  Private Sub RenderPenData(ByVal e As System.Windows.Forms.PaintEventArgs)

      If (m_penData.Count <> 0) Then
         Dim gfx = e.Graphics
         gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
         gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High
         gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
         gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
         Dim isDown = False
         Dim prev = New PointF()
         For i As Integer = 0 To (m_penData.Count - 1)
            If (m_penData(i).sw <> 0) Then
               If (Not isDown) Then
                  isDown = True
                  prev = tabletToClient(m_penData(i))
               Else
                  Dim curr = tabletToClient(m_penData(i))
                  gfx.DrawLine(m_penInk, prev, curr)
                  prev = curr
               End If
            Else
               If (isDown) Then
                  isDown = False
               End If
            End If
         Next
      End If
   End Sub

   Private Sub RenderPenTimeData(ByVal e As System.Windows.Forms.PaintEventArgs)

      If (m_penTimeData.Count <> 0) Then
         Dim gfx = e.Graphics
         gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
         gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High
         gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
         gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
         Dim isDown = False
         Dim prev = New PointF()
         For i As Integer = 0 To (m_penTimeData.Count - 1)
            If (m_penTimeData(i).sw <> 0) Then
               If (Not isDown) Then
                  isDown = True
                  prev = tabletToClientTimed(m_penTimeData(i))
               Else
                  Dim curr = tabletToClientTimed(m_penTimeData(i))
                  gfx.DrawLine(m_penInk, prev, curr)
                  prev = curr
               End If
            Else
               If (isDown) Then
                  isDown = False
               End If
            End If
         Next
      End If
   End Sub

  Public Function GetSigImage()

    Dim bitmap As Bitmap
    Dim brush As SolidBrush
    Dim rect As Rectangle
    Dim p1, p2 As Point

    rect.X = rect.Y = 0
    rect.Width = m_capability.screenWidth
    rect.Height = m_capability.screenHeight

    Try
      bitmap = New Bitmap(rect.Width, rect.Height)
      Dim gfx = Graphics.FromImage(bitmap)
      Dim s = Me.AutoScaleDimensions
      '            Dim inkWidthMM = 0.7F
      Dim inkWidthMM = 1.0F
      m_penInk = New Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2.0F))
      m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round
      m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round

      gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
      gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High
      gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
      gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality

      brush = New SolidBrush(Color.White)
      gfx.FillRectangle(brush, 0, 0, rect.Width, rect.Height)

      If m_penDataOptionMode = PenDataOptionMode.PenDataOptionMode_TimeCountSequence Then
        For i As Integer = 1 To (m_penTimeData.Count - 1)
          p1 = tabletToScreen(m_penTimeData(i - 1))
          p2 = tabletToScreen(m_penTimeData(i))

          If (m_penTimeData(i - 1).sw > 0 Or m_penTimeData(i).sw > 0) Then
            gfx.DrawLine(m_penInk, p1, p2)
          End If
        Next
      Else
        For i As Integer = 1 To (m_penData.Count - 1)
          p1 = tabletToScreen(m_penData(i - 1))
          p2 = tabletToScreen(m_penData(i))

          If (m_penData(i - 1).sw > 0 Or m_penData(i).sw > 0) Then
            gfx.DrawLine(m_penInk, p1, p2)
          End If
        Next
      End If

    Catch ex As Exception
      print("Exception: " + ex.Message)
      MsgBox("Exception: " + ex.Message)
      bitmap = Nothing
    End Try

    Return bitmap

  End Function

  Private Sub print(ByVal txt As String)
        m_ParentForm.print(txt)
    End Sub
End Class