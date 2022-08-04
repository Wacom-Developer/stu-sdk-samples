<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DemoButtonsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Me.btnSign = New System.Windows.Forms.Button()
      Me.btnClose = New System.Windows.Forms.Button()
      Me.txtDisplay = New System.Windows.Forms.TextBox()
      Me.groupBox1 = New System.Windows.Forms.GroupBox()
      Me.txtBaudRate = New System.Windows.Forms.TextBox()
      Me.txtCOM = New System.Windows.Forms.TextBox()
      Me.label2 = New System.Windows.Forms.Label()
      Me.label1 = New System.Windows.Forms.Label()
      Me.radSerial = New System.Windows.Forms.RadioButton()
      Me.radHID = New System.Windows.Forms.RadioButton()
      Me.Panel1 = New System.Windows.Forms.Panel()
      Me.PictureBox1 = New System.Windows.Forms.PictureBox()
      Me.groupBox1.SuspendLayout()
      Me.Panel1.SuspendLayout()
      CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'btnSign
      '
      Me.btnSign.Location = New System.Drawing.Point(56, 256)
      Me.btnSign.Name = "btnSign"
      Me.btnSign.Size = New System.Drawing.Size(75, 23)
      Me.btnSign.TabIndex = 0
      Me.btnSign.Text = "Sign"
      Me.btnSign.UseVisualStyleBackColor = True
      '
      'btnClose
      '
      Me.btnClose.Location = New System.Drawing.Point(246, 256)
      Me.btnClose.Name = "btnClose"
      Me.btnClose.Size = New System.Drawing.Size(75, 23)
      Me.btnClose.TabIndex = 2
      Me.btnClose.Text = "Close"
      Me.btnClose.UseVisualStyleBackColor = True
      '
      'txtDisplay
      '
      Me.txtDisplay.Location = New System.Drawing.Point(470, 12)
      Me.txtDisplay.Multiline = True
      Me.txtDisplay.Name = "txtDisplay"
      Me.txtDisplay.Size = New System.Drawing.Size(311, 588)
      Me.txtDisplay.TabIndex = 4
      '
      'groupBox1
      '
      Me.groupBox1.Controls.Add(Me.txtBaudRate)
      Me.groupBox1.Controls.Add(Me.txtCOM)
      Me.groupBox1.Controls.Add(Me.label2)
      Me.groupBox1.Controls.Add(Me.label1)
      Me.groupBox1.Location = New System.Drawing.Point(169, 94)
      Me.groupBox1.Name = "groupBox1"
      Me.groupBox1.Size = New System.Drawing.Size(192, 98)
      Me.groupBox1.TabIndex = 11
      Me.groupBox1.TabStop = False
      '
      'txtBaudRate
      '
      Me.txtBaudRate.Enabled = False
      Me.txtBaudRate.Location = New System.Drawing.Point(104, 56)
      Me.txtBaudRate.Name = "txtBaudRate"
      Me.txtBaudRate.Size = New System.Drawing.Size(72, 20)
      Me.txtBaudRate.TabIndex = 7
      '
      'txtCOM
      '
      Me.txtCOM.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
      Me.txtCOM.Enabled = False
      Me.txtCOM.Location = New System.Drawing.Point(105, 19)
      Me.txtCOM.Name = "txtCOM"
      Me.txtCOM.Size = New System.Drawing.Size(72, 20)
      Me.txtCOM.TabIndex = 6
      '
      'label2
      '
      Me.label2.AutoSize = True
      Me.label2.Location = New System.Drawing.Point(11, 59)
      Me.label2.Name = "label2"
      Me.label2.Size = New System.Drawing.Size(56, 13)
      Me.label2.TabIndex = 5
      Me.label2.Text = "Baud rate:"
      '
      'label1
      '
      Me.label1.AutoSize = True
      Me.label1.Location = New System.Drawing.Point(11, 19)
      Me.label1.Name = "label1"
      Me.label1.Size = New System.Drawing.Size(72, 13)
      Me.label1.TabIndex = 4
      Me.label1.Text = "COM number:"
      '
      'radSerial
      '
      Me.radSerial.AutoSize = True
      Me.radSerial.Location = New System.Drawing.Point(66, 128)
      Me.radSerial.Name = "radSerial"
      Me.radSerial.Size = New System.Drawing.Size(51, 17)
      Me.radSerial.TabIndex = 10
      Me.radSerial.TabStop = True
      Me.radSerial.Text = "Serial"
      Me.radSerial.UseVisualStyleBackColor = True
      '
      'radHID
      '
      Me.radHID.AutoSize = True
      Me.radHID.Location = New System.Drawing.Point(68, 66)
      Me.radHID.Name = "radHID"
      Me.radHID.Size = New System.Drawing.Size(44, 17)
      Me.radHID.TabIndex = 9
      Me.radHID.TabStop = True
      Me.radHID.Text = "HID"
      Me.radHID.UseVisualStyleBackColor = True
      '
      'Panel1
      '
      Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.Panel1.Controls.Add(Me.PictureBox1)
      Me.Panel1.Location = New System.Drawing.Point(9, 310)
      Me.Panel1.Name = "Panel1"
      Me.Panel1.Size = New System.Drawing.Size(442, 274)
      Me.Panel1.TabIndex = 12
      '
      'PictureBox1
      '
      Me.PictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight
      Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.PictureBox1.Location = New System.Drawing.Point(15, 12)
      Me.PictureBox1.Name = "PictureBox1"
      Me.PictureBox1.Size = New System.Drawing.Size(404, 247)
      Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
      Me.PictureBox1.TabIndex = 2
      Me.PictureBox1.TabStop = False
      '
      'DemoButtonsForm
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(790, 612)
      Me.Controls.Add(Me.Panel1)
      Me.Controls.Add(Me.groupBox1)
      Me.Controls.Add(Me.radSerial)
      Me.Controls.Add(Me.radHID)
      Me.Controls.Add(Me.txtDisplay)
      Me.Controls.Add(Me.btnClose)
      Me.Controls.Add(Me.btnSign)
      Me.Name = "DemoButtonsForm"
      Me.Text = "DemoButtons"
      Me.groupBox1.ResumeLayout(False)
      Me.groupBox1.PerformLayout()
      Me.Panel1.ResumeLayout(False)
      CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents btnSign As System.Windows.Forms.Button
   Friend WithEvents btnClose As System.Windows.Forms.Button
   Friend WithEvents txtDisplay As System.Windows.Forms.TextBox
   Private WithEvents groupBox1 As GroupBox
   Private WithEvents txtBaudRate As TextBox
   Private WithEvents txtCOM As TextBox
   Private WithEvents label2 As Label
   Private WithEvents label1 As Label
   Private WithEvents radSerial As RadioButton
   Private WithEvents radHID As RadioButton
   Friend WithEvents Panel1 As Panel
   Friend WithEvents PictureBox1 As PictureBox
End Class
