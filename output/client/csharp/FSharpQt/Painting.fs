module FSharpQt.Painting

open Org.Whatever.QtTesting

type Color private(qtColor: PaintResources.Color) =
    member val internal qtColor = qtColor
    new (r: int, g: int, b: int) = Color(PaintResources.Color.Create(r, g, b))
    new (r: int, g: int, b: int, a: int) = Color(PaintResources.Color.Create(r, g, b, a))
    new (r: float, g: float, b: float) = Color(PaintResources.Color.Create(float32 r, float32 g, float32 b))
    new (r: float, g: float, b: float, a: float) = Color(PaintResources.Color.Create(float32 r, float32 g, float32 b, float32 a))
    static member Black = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Black))
    static member White = Color(PaintResources.Color.Create(PaintResources.Color.Constant.White))
    static member DarkGray = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkGray))
    static member Gray = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Gray))
    static member LightGray = Color(PaintResources.Color.Create(PaintResources.Color.Constant.LightGray))
    static member Red = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Red))
    static member Green = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Green))
    static member Blue = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Blue))
    static member Cyan = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Cyan))
    static member Magenta = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Magenta))
    static member Yellow = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Yellow))
    static member DarkRed = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkRed))
    static member DarkGreen = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkGreen))
    static member DarkBlue = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkBlue))
    static member DarkCyan = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkCyan))
    static member DarkMagenta = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkMagenta))
    static member DarkYellow = Color(PaintResources.Color.Create(PaintResources.Color.Constant.DarkYellow))
    static member Transparent = Color(PaintResources.Color.Create(PaintResources.Color.Constant.Transparent))
    override this.Finalize() =
        qtColor.Dispose()

type Brush private(qtBrush: PaintResources.Brush) =
    member val internal qtBrush = qtBrush
    new (color: Color) = Brush(PaintResources.Brush.Create(color.qtColor))
    static member NoBrush = Brush(PaintResources.Brush.Create(PaintResources.Brush.Style.NoBrush))
    override this.Finalize() =
        qtBrush.Dispose()

type Style =
    | NoPen
    | SolidLine
    | DashLine
    | DotLine
    | DashDotLine
    | DashDotDotLine
    | CustomDashLine
with
    member internal this.QtValue =
        match this with
        | NoPen -> PaintResources.Pen.Style.NoPen
        | SolidLine -> PaintResources.Pen.Style.SolidLine
        | DashLine -> PaintResources.Pen.Style.DashLine
        | DotLine -> PaintResources.Pen.Style.DotLine
        | DashDotLine -> PaintResources.Pen.Style.DashDotLine
        | DashDotDotLine -> PaintResources.Pen.Style.DashDotDotLine
        | CustomDashLine -> PaintResources.Pen.Style.CustomDashLine
    
type CapStyle =
    | Flat
    | Square
    | Round
with
    member internal this.QtValue =
        match this with
        | Flat -> PaintResources.Pen.CapStyle.Flat
        | Square -> PaintResources.Pen.CapStyle.Square
        | Round -> PaintResources.Pen.CapStyle.Round
    
type JoinStyle =
    | Miter
    | Bevel
    | Round
    | SvgMiter
with
    member internal this.QtValue =
        match this with
        | Miter -> PaintResources.Pen.JoinStyle.Miter
        | Bevel -> PaintResources.Pen.JoinStyle.Bevel
        | Round -> PaintResources.Pen.JoinStyle.Round
        | SvgMiter -> PaintResources.Pen.JoinStyle.SvgMiter
        

type Pen private(qtPen: PaintResources.Pen) =
    member val internal qtPen = qtPen
    new () = Pen(PaintResources.Pen.Create())
    new (style: Style) =
        Pen(PaintResources.Pen.Create(style.QtValue))
    new (color: Color) =
        Pen(PaintResources.Pen.Create(color.qtColor))
    new (brush: Brush, width: double, style: Style, cap: CapStyle, join: JoinStyle) =
        Pen(PaintResources.Pen.Create(brush.qtBrush, width, style.QtValue, cap.QtValue, join.QtValue))
    override this.Finalize() =
        qtPen.Dispose()
       
type Weight =
    | Thin
    | ExtraLight
    | Light
    | Normal
    | Medium
    | DemiBold
    | Bold
    | ExtraBold
    | Black
with
    member internal this.QtValue =
        match this with
        | Thin -> PaintResources.Font.Weight.Thin
        | ExtraLight -> PaintResources.Font.Weight.ExtraLight
        | Light -> PaintResources.Font.Weight.Light
        | Normal -> PaintResources.Font.Weight.Normal
        | Medium -> PaintResources.Font.Weight.Medium
        | DemiBold -> PaintResources.Font.Weight.DemiBold
        | Bold -> PaintResources.Font.Weight.Bold
        | ExtraBold -> PaintResources.Font.Weight.ExtraBold
        | Black -> PaintResources.Font.Weight.Black
    
type Font private(qtFont: PaintResources.Font) =
    member val internal qtFont = qtFont
    new (family: string, pointSize: int) = Font(PaintResources.Font.Create(family, pointSize))
    new (family: string, pointSize: int, weight: Weight) = Font(PaintResources.Font.Create(family, pointSize, weight.QtValue))
    new (family: string, pointSize: int, weight: Weight, italic: bool) = Font(PaintResources.Font.Create(family, pointSize, weight.QtValue, italic))
    override this.Finalize() =
        qtFont.Dispose()

type Painter internal(qtPainter: Org.Whatever.QtTesting.Painter.Handle) =
    member val qtPainter = qtPainter
    // no finalizer, we never own the qt painter
    member this.Pen with set (value: Pen) = qtPainter.SetPen(value.qtPen)
    member this.Brush with set (value: Brush) = qtPainter.SetBrush(value.qtBrush)
    member this.Font with set (value: Font) = qtPainter.SetFont(value.qtFont)
    member this.DrawText(rect: Common.Rect, align: Common.Alignment, text: string) =
        qtPainter.DrawText(rect, align, text)
        
    member this.FillRect(rect: Common.Rect, brush: Brush) =
        qtPainter.FillRect(rect, brush.qtBrush)
        
    member this.FillRect(rect: Common.Rect, color: Color) =
        qtPainter.FillRect(rect, color.qtColor)

    member this.DrawRect(rect: Common.Rect) =
        qtPainter.DrawRect(rect)
        
    member this.DrawRect(rect: Common.RectF) =
        qtPainter.DrawRect(rect)
        
    member this.DrawRect(x: int, y: int, width: int, height: int) =
        qtPainter.DrawRect(x, y, width, height)
        
    member this.DrawEllipse(rect: Common.RectF) =
        qtPainter.DrawEllipse(rect)

    member this.DrawEllipse(rect: Common.Rect) =
        qtPainter.DrawEllipse(rect)
        
    member this.DrawEllipse(x: int, y: int, width: int, height: int) =
        qtPainter.DrawEllipse(x, y, width, height)
        
    member this.DrawEllipse(center: Common.PointF, rx: double, ry: double) =
        qtPainter.DrawEllipse(center, rx, ry)
        
    member this.DrawEllipse(center: Common.Point, rx: int, ry: int) =
        qtPainter.DrawEllipse(center, rx, ry)
    
