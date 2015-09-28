Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Drawing.Text

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
        Return RoundRect(New Rectangle(X, Y, Width, Height), Curve)
    End Function
    Public Function RoundGradient(ByVal gfx As Graphics, ByVal rect As Rectangle, ByVal Curve As Integer, ByVal C1 As Color, ByVal C2 As Color) As GraphicsPath
        Dim rr As GraphicsPath = RoundRect(rect, Curve)
        Dim pgb As New PathGradientBrush(rr)
        pgb.SurroundColors = New Color() {C1}
        pgb.CenterColor = C2
        gfx.FillRectangle(pgb, rect)
        Return rr
    End Function
    Public Function RoundGradient(ByVal gfx As Graphics, ByVal X As Integer, ByVal Y As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal Curve As Integer, ByVal C1 As Color, ByVal C2 As Color) As GraphicsPath
        Return RoundGradient(gfx, New Rectangle(X, Y, Width, Height), Curve, C1, C2)
    End Function
    Public Function CircleGradient(ByVal gfx As Graphics, ByVal rect As Rectangle, ByVal centercolor As Color, ByVal outercolor As Color)
        Dim pth As New GraphicsPath()
        pth.AddEllipse(rect)
        Dim pgb As New PathGradientBrush(pth)
        pgb.SurroundColors = New Color() {outercolor}
        pgb.CenterColor = centercolor
        gfx.FillRectangle(pgb, rect)
        Return pth
    End Function
End Module

Class SteamChatTheme
    Inherits ContainerControl
#Region "Draw"
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim gfx = e.Graphics
        gfx.Clear(Color.FromArgb(30, 30, 30))
        'Dim bgBrush As New LinearGradientBrush(New Point(0, 0), New Point(0, Height), Color.FromArgb(26, 32, 55), Color.Transparent) ' - ((Height / 100) * 10)
        Dim bgBrush As New LinearGradientBrush(New Point(0, 0), New Point(0, Height), Color.FromArgb(30, 30, 50), Color.Transparent) ' - ((Height / 100) * 10)
        'change color of gradient if window/control is active
        gfx.FillRectangle(bgBrush, New Rectangle(0, 0, Width, Height))
    End Sub
#End Region
#Region "Initializers"
    Sub New()
        Font = New Font("Arial", 8)
        SetStyle(ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        DoubleBuffered = True
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        ParentForm.FormBorderStyle = FormBorderStyle.None
        ParentForm.TransparencyKey = Color.Fuchsia
        Dock = DockStyle.Fill
    End Sub
#End Region
#Region "Header/Move Window"
    Private MouseP As Point = New Point(0, 0)
    Private Cap As Boolean = False
    Private MoveHeight% = 30 : Private pos% = 0
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
#End Region
End Class

Class SteamChatSpecials
    Inherits Control
#Region "Draw"
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        'e.Graphics.Clear(Color.Transparent)
        e.Graphics.DrawImage(_buffer, New Point(0, 0))
    End Sub
#End Region
#Region "Initializers"
    Private _buffer As Bitmap
    Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        DoubleBuffered = True
        BackColor = Color.Transparent
        Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Size = New Size(46, 11)
        Using ms As New MemoryStream
            Dim data As Byte() = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAC4AAAALCAYAAAAA2L+yAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAAJtSURBVEhLjVQ9axRRFH27ayFGCy2VCVY7+Qem8x/4F8TWxipoIVhZpRVkERVEuwVFELOZ2YyoGMliIhISMIVpNjb6G9Zz7rt35u7LJLhwuB/vvPvOefPYsHjwp5PlMwHzkDc1c7/uEfJrrX0inTmHwaxLhPVJa1/gesLTfnZ31iPYC4v5sqDtYB8tJ8yQN1kV67//B7XgVKT2Ppdvn4E3/TZ+sTTHA7Y/DS5U5WiyWQ5XQzbjzYWOGAgx4teRHKhzNSjr4BNWc0Yq8CTIeQ7p3KPx7XMfitEWuNOd6umScGBgb7JynqLZ3y8eXQlZP3Q9sLkrg1LBSZ0FDEQtxlBT1ByfHC8SomrhappCRZhybR9FmvgfxfPc12aGgrqCAwwDZKjmBI3Uppx44ZlQRBGuB1vPTBE8TIRjn8ykAULFg0sdcSZwuHFvwcQCeyba5vEnwimMkbXlEmlGhRM8VL5KYspEmeg2kMMoZ9IM9tkc9uUr6n5yeNsqfPqlfHPT5kThejgax0XzgATse44AgkWUfiVZZzzpq6gA8urzda9dys/tW3gea/HGy9E+I43YXmgNPQE21NHyeYhAHzFAzDKKKF1zMYrTmhzk8XnounCSPL7pKHq3HOgbb2oxyKHTdw8uEr827l9Kc0ZwRLjG2ohFEyWCeHvoGdhT7jFz7CHWgom/xfJZ3PBrYnf8+KrxRHz5/hXe/ehw7eHlcPTxxgKdnAZysNmLbr6QDgav9e8vhTeCGXwasda4OR7eAW/6vXrSZ88M0YDd/NdyuAJuOLNVvbx+GsCJYvsa09pHQ8OLIpva99I132sMNVBu6P0DdZwvbyzP9UgAAAAASUVORK5CYII=")
            ms.Write(data, 0, data.Length)
            ms.Flush()
            data = Nothing
            _buffer = Bitmap.FromStream(ms)
        End Using
    End Sub
#End Region
#Region "Deconstructor"
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        _buffer.Dispose()
        _buffer = Nothing
    End Sub
#End Region
#Region "ClickDetection"

#End Region
#Region "Fixed size"
    Private Sub SteamChatSpecials_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Size = New Size(46, 11)
    End Sub
#End Region
End Class

Class SteamChatTabControl
    Inherits TabControl
#Region "Draw"

    Protected Overrides Sub OnControlAdded(ByVal e As ControlEventArgs)
        MyBase.OnControlAdded(e)
        e.Control.BackColor = Color.Transparent
        e.Control.ForeColor = Color.FromArgb(150, 150, 150)
        e.Control.Font = New Font("Arial", 8)
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        Dim gfx As Graphics = e.Graphics
        gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
        gfx.Clear(Color.FromArgb(27, 32, 45))
        gfx.FillRectangle(New SolidBrush(Color.FromArgb(38, 38, 38)), 0, 23, Width, Height - 23)
        gfx.DrawLine(New Pen(Color.FromArgb(89, 89, 89), 1), 0, 23, Width, 23)
        For i As Integer = 0 To TabPages.Count - 1
            Dim r As Rectangle = GetTabRect(i)
            If String.IsNullOrEmpty(TabPages(i).Tag) Then
                gfx.DrawRectangle(New Pen(Color.FromArgb(30, 30, 40), 1), r) ' equal to clear, except tabthing only
                Dim bgBrush As New LinearGradientBrush(New Point(0, 0), New Point(0, r.Height + 5), Color.FromArgb(86, 86, 86), Color.FromArgb(40, 40, 40)) '2

                If Not SelectedIndex = i Then
                    gfx.FillRectangle(New SolidBrush(Color.FromArgb(58, 58, 58)), r.X, r.Y, r.Width - 2, r.Height)
                    gfx.FillRectangle(New SolidBrush(Color.FromArgb(30, 30, 40)), r.X, r.Y - 22, r.Width, r.Height + 4)
                Else
                    gfx.DrawLine(New Pen(Color.FromArgb(40, 40, 40), 1), r.X, 23, r.X + r.Width - 2, 23)
                    gfx.FillRectangle(bgBrush, r.X, r.Y, r.Width - 2, r.Height)
                End If
                gfx.SmoothingMode = SmoothingMode.AntiAlias
                gfx.DrawImage(_buffer, r.X, r.Y)
                gfx.DrawString(TabPages(i).Text, Font, New SolidBrush(Color.FromArgb(226, 226, 226)), r.X + 17, 23 / 2 - gfx.MeasureString(TabPages(i).Text, Font).Height / 2)
                gfx.SmoothingMode = SmoothingMode.Default
            End If
        Next
    End Sub
    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        ' Do not paint background.
    End Sub
#End Region
#Region "Initializers"
    Private _buffer As Bitmap
    Sub New()
        Font = New Font("Arial", 8)
        SetStyle(ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        DoubleBuffered = True
        BackColor = Color.Transparent
        'Alignment = TabAlignment.Left
        SizeMode = TabSizeMode.Fixed
        ItemSize = New Size(100, 20)
        Using ms As New MemoryStream
            Dim data As Byte() = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAADpSURBVDhPrc49CgIxEAXgBcFCsBAsBAvBQrAQBHMObb2RtxEVxdKflWQRwc7GTkRWbDyC83ASxl0XUqT4yG4y83iRMWYQCsL6oSCsFwrCutbRHMZ0vsg748ZSnnE7EsI6wpM8SFoAwTjljoOwtnD1hFnb0u0jrCWcPWHWtnT7CGsKMdnyucuItTEJf2P2TtDS7SOsYXHtGVuQJVl/6TmdK57B/IWgpdtHWL2I1skG8m96Klq6e4TVinDQhP9F05+Wbh5h1SI0PLLfomVuzkJYxYdo+fcdEFb2cdL74b97CWGlUCKlVCAq+gDTYfsynuRUtgAAAABJRU5ErkJggg==")
            ms.Write(data, 0, data.Length)
            ms.Flush()
            data = Nothing
            _buffer = Bitmap.FromStream(ms)
        End Using
    End Sub
#End Region
#Region "Deconstructors"
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        _buffer.Dispose()
        _buffer = Nothing
    End Sub
#End Region
End Class