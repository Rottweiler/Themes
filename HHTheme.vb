'           `':,;+####;`           
'         #################        
'          ###########@#;+##'      
'          #@##########'####;      
'         :###@#++######'''++';#,  
'         ######+;;##;;;;;+';;;+#  
'        #####++####'';;;+';'''#'  
'       @######++#+++#+####++''#   
'       ######+#;'''++#+#;;+.      
'      ########;+;'+'++#           
'   .##########'+'''++++           
'  ##############+'+'++#           
'##################+####           
'#########@#########+@##           
'###########@##########@           
'############@##########,          
'
'Rottweiler @ HackHound.org

Imports System.Drawing.Drawing2D
Imports System.ComponentModel

Enum MouseState As Byte
    None = 0
    Over = 1
    Down = 2
    Block = 3
End Enum

Module Draw
    Public Function RoundRect(ByVal Rectangle As Rectangle, ByVal Curve As Integer) As GraphicsPath
        Dim P As GraphicsPath = New GraphicsPath()
        Dim ArcRectangleWidth As Integer = Curve * 2
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90)
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90)
        P.AddLine(New Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), New Point(Rectangle.X, Curve + Rectangle.Y))
        Return P
    End Function
    Public Function RoundRect(ByVal X As Integer, ByVal Y As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal Curve As Integer) As GraphicsPath
        Dim Rectangle As Rectangle = New Rectangle(X, Y, Width, Height)
        Dim P As GraphicsPath = New GraphicsPath()
        Dim ArcRectangleWidth As Integer = Curve * 2
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90)
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90)
        P.AddLine(New Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), New Point(Rectangle.X, Curve + Rectangle.Y))
        Return P
    End Function
    Public Function RoundGradient(ByVal gfx As Graphics, ByVal rect As Rectangle, ByVal centercolor As Color, ByVal outercolor As Color)
        Dim pth As New GraphicsPath()
        pth.AddEllipse(rect)
        Dim pgb As New PathGradientBrush(pth)
        pgb.SurroundColors = New Color() {outercolor} 'New Color() {Color.Transparent}
        pgb.CenterColor = centercolor 'Color.FromArgb(130, 130, 130)
        gfx.FillRectangle(pgb, rect)
        Return pth
    End Function
End Module

Class HSLColor
    ' Private data members below are on scale 0-1
    ' They are scaled for use externally based on scale
    Private m_hue As Double = 1.0
    Private m_saturation As Double = 1.0
    Private m_luminosity As Double = 1.0

    Private Const scale As Double = 240.0

    Public Property Hue() As Double
        Get
            Return m_hue * scale
        End Get
        Set(ByVal value As Double)
            m_hue = CheckRange(value / scale)
        End Set
    End Property
    Public Property Saturation() As Double
        Get
            Return m_saturation * scale
        End Get
        Set(ByVal value As Double)
            m_saturation = CheckRange(value / scale)
        End Set
    End Property
    Public Property Luminosity() As Double
        Get
            Return m_luminosity * scale
        End Get
        Set(ByVal value As Double)
            m_luminosity = CheckRange(value / scale)
        End Set
    End Property

    Private Function CheckRange(ByVal value As Double) As Double
        If value < 0.0 Then
            value = 0.0
        ElseIf value > 1.0 Then
            value = 1.0
        End If
        Return value
    End Function

    Public Overrides Function ToString() As String
        Return [String].Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity)
    End Function

    Public Function ToRGBString() As String
        Dim color As Color = CType(Me, Color)
        Return [String].Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B)
    End Function

#Region "Casts to/from System.Drawing.Color"
    Public Shared Widening Operator CType(ByVal hslColor As HSLColor) As Color
        Dim r As Double = 0, g As Double = 0, b As Double = 0
        If hslColor.Luminosity <> 0 Then
            If hslColor.Saturation = 0 Then
                r = InlineAssignHelper(g, InlineAssignHelper(b, hslColor.Luminosity))
            Else
                Dim temp2 As Double = GetTemp2(hslColor)
                Dim temp1 As Double = 2.0 * hslColor.Luminosity - temp2

                r = GetColorComponent(temp1, temp2, hslColor.Hue + 1.0 / 3.0)
                g = GetColorComponent(temp1, temp2, hslColor.Hue)
                b = GetColorComponent(temp1, temp2, hslColor.Hue - 1.0 / 3.0)
            End If
        End If
        Return Color.FromArgb(CInt(255 * Clamp(r, 0, 1)), CInt(255 * Clamp(g, 0, 1)), CInt(255 * Clamp(b, 0, 1)))
    End Operator

    Private Shared Function Clamp(ByVal value As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
        Return If((value < min), min, If((value > max), max, value))
    End Function

    Private Shared Function GetColorComponent(ByVal temp1 As Double, ByVal temp2 As Double, ByVal temp3 As Double) As Double
        temp3 = MoveIntoRange(temp3)
        If temp3 < 1.0 / 6.0 Then
            Return temp1 + (temp2 - temp1) * 6.0 * temp3
        ElseIf temp3 < 0.5 Then
            Return temp2
        ElseIf temp3 < 2.0 / 3.0 Then
            Return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0)
        Else
            Return temp1
        End If
    End Function
    Private Shared Function MoveIntoRange(ByVal temp3 As Double) As Double
        If temp3 < 0.0 Then
            temp3 += 1.0
        ElseIf temp3 > 1.0 Then
            temp3 -= 1.0
        End If
        Return temp3
    End Function
    Private Shared Function GetTemp2(ByVal hslColor As HSLColor) As Double
        Dim temp2 As Double
        If hslColor.Luminosity < 0.5 Then
            '<=??
            temp2 = hslColor.Luminosity * (1.0 + hslColor.Saturation)
        Else
            temp2 = hslColor.Luminosity + hslColor.Saturation - (hslColor.Luminosity * hslColor.Saturation)
        End If
        Return temp2
    End Function

    Public Shared Widening Operator CType(ByVal color As Color) As HSLColor
        Dim hslColor As New HSLColor()
        hslColor.Hue = color.GetHue() / 360.0
        ' we store hue as 0-1 as opposed to 0-360 
        hslColor.Luminosity = color.GetBrightness()
        hslColor.Saturation = color.GetSaturation()
        Return hslColor
    End Operator
#End Region

    Public Sub SetRGB(ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer)
        Dim hslColor As HSLColor = CType(Color.FromArgb(red, green, blue), HSLColor)
        Me.m_hue = hslColor.Hue
        Me.m_saturation = hslColor.Saturation
        Me.m_luminosity = hslColor.Luminosity
    End Sub

    Public Sub New()
    End Sub

    Public Sub New(ByVal color As Color)
        SetRGB(color.R, color.G, color.B)
    End Sub
    Public Sub New(ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer)
        SetRGB(red, green, blue)
    End Sub
    Public Sub New(ByVal hue As Double, ByVal saturation As Double, ByVal luminosity As Double)
        Me.Hue = hue
        Me.Saturation = saturation
        Me.Luminosity = luminosity
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

End Class

Class HHTheme
    Inherits ContainerControl
    Private _icon As Image
    Public Property Icon() As Image
        Get
            Return _icon
        End Get
        Set(ByVal value As Image)
            _icon = value
        End Set
    End Property
    Private headerfont As Font = New Font("helvetica", 16, FontStyle.Bold, GraphicsUnit.Pixel)
    Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        DoubleBuffered = True
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        Dim gfx = e.Graphics
        'gfx.SmoothingMode = SmoothingMode.AntiAlias
        gfx.Clear(Color.FromArgb(22, 22, 22))
        RoundGradient(gfx, New Rectangle(0, -25, Width - 1, 50 - 1), Color.WhiteSmoke, Color.Transparent) 'Color.FromArgb(130, 130, 130)
        RoundGradient(gfx, New Rectangle(0, 0, Width, 39), Color.Black, Color.Transparent)
        gfx.FillRectangle(New SolidBrush(Color.FromArgb(17, 17, 17)), 0, 36, Width, 3)
        gfx.DrawRectangle(Pens.Black, New Rectangle(0, 0, Width - 1, Height - 1))

        Dim fz As SizeF = gfx.MeasureString(Me.Text, headerfont)
        RoundGradient(gfx, New Rectangle(5, 10, fz.Width + 5, fz.Height), Color.FromArgb(100, 255, 145, 0), Color.Transparent)
        gfx.DrawString(Me.Text, New Font("helvetica", 16, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(255, 145, 0)), 10, 10)

        If Icon IsNot Nothing Then
            Dim x As New Bitmap(Icon)
            Dim xd As New Bitmap(16, 16)
            Dim grd As Graphics = Graphics.FromImage(xd)
            grd.DrawImage(x, 0, 0, xd.Width, xd.Height)
            grd.Dispose()
            gfx.DrawImage(xd, fz.Width + 15, 13)
        End If

        gfx.FillRectangle(New SolidBrush(Color.FromArgb(29, 29, 29)), New Rectangle(10, 50, Width - 21, Height - 61))
        gfx.DrawRectangle(New Pen(Color.FromArgb(20, 20, 20), 1), New Rectangle(11, 51, Width - 24, Height - 64))
        gfx.DrawRectangle(New Pen(Color.FromArgb(39, 39, 39), 1), New Rectangle(12, 52, Width - 26, Height - 66))
        '36 ^
        '3 ^v
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        Me.ParentForm.FormBorderStyle = FormBorderStyle.None
        Me.ParentForm.TransparencyKey = Color.Fuchsia
        Dock = DockStyle.Fill
    End Sub
    Private MouseP As Point = New Point(0, 0)
    Private Cap As Boolean = False
    Private MoveHeight% = 50 : Private pos% = 0
    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = Windows.Forms.MouseButtons.Left And New Rectangle(0, 0, Width, MoveHeight).Contains(e.Location) Then
            Cap = True : MouseP = e.Location
        End If
    End Sub
    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseUp(e) : Cap = False
    End Sub
    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)
        If Cap Then
            Parent.Location = MousePosition - MouseP
        End If
    End Sub
End Class

Class HHButton
    Inherits Control
#Region " MouseStates "
    Dim State As MouseState = MouseState.None
    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)
        State = MouseState.Down : Invalidate()
    End Sub
    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseUp(e)
        State = MouseState.Over : Invalidate()
    End Sub
    Protected Overrides Sub OnMouseEnter(ByVal e As System.EventArgs)
        MyBase.OnMouseEnter(e)
        State = MouseState.Over : Invalidate()
    End Sub
    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        MyBase.OnMouseLeave(e)
        State = MouseState.None : Invalidate()
    End Sub
#End Region
    Private _inverted As Boolean = False
    Public Property Inverted() As Boolean
        Get
            Return _inverted
        End Get
        Set(ByVal value As Boolean)
            _inverted = value
        End Set
    End Property
    Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        BackColor = Color.Transparent
        DoubleBuffered = True
        Font = New Font("helvetica", 13, FontStyle.Regular, GraphicsUnit.Pixel)
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        Dim gfx = e.Graphics
        gfx.SmoothingMode = SmoothingMode.AntiAlias
        Dim cbehind, cafter, cline, ctxt As Color

        cbehind = IIf(Inverted, Color.FromArgb(Color.FromArgb(18, 18, 18).ToArgb Xor &HFFFFFF), Color.FromArgb(18, 18, 18))
        cafter = IIf(Inverted, Color.FromArgb(Color.FromArgb(23, 23, 23).ToArgb Xor &HFFFFFF), Color.FromArgb(23, 23, 23))
        cline = IIf(Inverted, Color.FromArgb(Color.FromArgb(29, 29, 29).ToArgb Xor &HFFFFFF), Color.FromArgb(29, 29, 29))
        ctxt = IIf(Inverted, Color.FromArgb(Color.FromArgb(191, 182, 170).ToArgb Xor &HFFFFFF), Color.FromArgb(191, 182, 170))

        If Inverted Then
            cbehind = ControlPaint.Dark(cbehind, 0.01F)
            cafter = ControlPaint.Dark(cafter, 0.01F)
            cline = ControlPaint.Dark(cline, 0.01F)
            ctxt = Color.FromArgb(29, 29, 29)
        End If

        Select Case State
            Case MouseState.None
            Case MouseState.Over
                cbehind = ControlPaint.Dark(cbehind, 0.01F)
                cafter = ControlPaint.Dark(cafter, 0.01F)
                cline = ControlPaint.Dark(cline, 0.01F)
            Case MouseState.Down
                cafter = ControlPaint.Dark(cafter, 0.01F)
                cline = ControlPaint.Dark(cline, 0.01F)
                cbehind = Color.FromArgb(0, 148, 255)
        End Select

        gfx.FillPath(New SolidBrush(cbehind), Draw.RoundRect(ClientRectangle, 3))

        gfx.FillPath(New SolidBrush(cafter), Draw.RoundRect(New Rectangle(1, 1, Width - 2, Height - 2), 3))

        gfx.DrawLine(New Pen(DirectCast(cline, Color), 1), 3, 1, Width - 3, 1)

        Dim mz As SizeF = gfx.MeasureString(Text, Font)
        gfx.DrawString(Text, Font, New SolidBrush(ctxt), (Me.Width / 2) - (mz.Width / 2), (Me.Height / 2) - (mz.Height / 2))
    End Sub

    Public Function Darken(ByVal color As Color, ByVal darkenAmount As Double) As Color
        Dim hslColor As New HSLColor(color)
        hslColor.Luminosity *= darkenAmount
        Return hslColor
    End Function
End Class
