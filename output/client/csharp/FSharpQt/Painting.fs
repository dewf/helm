module FSharpQt.Painting

open System.Collections.Generic
open Org.Whatever.QtTesting

type Color private(qtColor: PaintResources.Color) =
    member val internal qtColor = qtColor
    override this.Finalize() =
        qtColor.Dispose()
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

type Brush private(qtBrush: PaintResources.Brush) =
    member val internal qtBrush = qtBrush
    override this.Finalize() =
        qtBrush.Dispose()
    new (color: Color) = Brush(PaintResources.Brush.Create(color.qtColor))
    static member NoBrush = Brush(PaintResources.Brush.Create(PaintResources.Brush.Style.NoBrush))

type PenStyle =
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
    override this.Finalize() =
        qtPen.Dispose()
    new () = Pen(PaintResources.Pen.Create())
    new (style: PenStyle) =
        Pen(PaintResources.Pen.Create(style.QtValue))
    new (color: Color) =
        Pen(PaintResources.Pen.Create(color.qtColor))
    new (brush: Brush, width: double, ?style: PenStyle, ?cap: CapStyle, ?join: JoinStyle) =
        let useStyle =
            defaultArg style SolidLine
        let useCap =
            defaultArg cap Square
        let useJoin =
            defaultArg join Bevel
        Pen(PaintResources.Pen.Create(brush.qtBrush, width, useStyle.QtValue, useCap.QtValue, useJoin.QtValue))
    new (color: Color, width: double, ?style: PenStyle, ?cap: CapStyle, ?join: JoinStyle) =
        let useStyle =
            defaultArg style SolidLine
        let useCap =
            defaultArg cap Square
        let useJoin =
            defaultArg join Bevel
        Pen(Brush(color), width, useStyle, useCap, useJoin)
    
    member this.Width with set (value: int) = this.qtPen.SetWidth(value)
    member this.Width with set (value: double) = this.qtPen.SetWidth(value)
        
       
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
    override this.Finalize() =
        qtFont.Dispose()
    new (family: string, pointSize: int) = Font(PaintResources.Font.Create(family, pointSize))
    new (family: string, pointSize: int, weight: Weight) = Font(PaintResources.Font.Create(family, pointSize, weight.QtValue))
    new (family: string, pointSize: int, weight: Weight, italic: bool) = Font(PaintResources.Font.Create(family, pointSize, weight.QtValue, italic))
        
        
type PainterPath internal(qtPainterPath: PaintResources.PainterPath) =
    member val qtPainterPath = qtPainterPath
    override this.Finalize() =
        qtPainterPath.Dispose()
    new() = PainterPath(PaintResources.PainterPath.Create())
    member this.MoveTo(p: Common.PointF) =
        qtPainterPath.MoveTo(p)
    member this.LineTo(p: Common.PointF) =
        qtPainterPath.Lineto(p)
    member this.CubicTo(c1: Common.PointF, c2: Common.PointF, endPoint: Common.PointF) =
        qtPainterPath.CubicTo(c1, c2, endPoint)
        
type PainterPathStroker internal(qtStroker: PaintResources.PainterPathStroker) =
    member val qtStroker = qtStroker
    override this.Finalize() =
        qtStroker.Dispose()
    new() = PainterPathStroker(PaintResources.PainterPathStroker.Create())
    member this.Width with set value = qtStroker.SetWidth(value)
    member this.JoinStyle with set (value: JoinStyle) = qtStroker.SetJoinStyle(value.QtValue)
    member this.CapStyle with set (value: CapStyle) = qtStroker.SetCapStyle(value.QtValue)
    member this.DashPattern with set (value: double array) = qtStroker.SetDashPattern(value)
    member this.CreateStroke(path: PainterPath) =
        PainterPath(qtStroker.CreateStroke(path.qtPainterPath))
        
type RenderHint =
    | Antialiasing
    | TextAntialiasing
    | SmoothPixmapTransform
    | VerticalSubpixelPositioning
    | LosslessImageRendering
    | NonCosmeticBrushPatterns
with
    member this.QtValue =
        match this with
        | Antialiasing -> Painter.RenderHint.Antialiasing
        | TextAntialiasing -> Painter.RenderHint.TextAntialiasing
        | SmoothPixmapTransform -> Painter.RenderHint.SmoothPixmapTransform
        | VerticalSubpixelPositioning -> Painter.RenderHint.VerticalSubpixelPositioning
        | LosslessImageRendering -> Painter.RenderHint.LosslessImageRendering
        | NonCosmeticBrushPatterns -> Painter.RenderHint.NonCosmeticBrushPatterns

type Painter internal(qtPainter: Org.Whatever.QtTesting.Painter.Handle) =
    member val qtPainter = qtPainter
    // no finalizer, we never own the qt painter
    member this.Pen with set (value: Pen) = qtPainter.SetPen(value.qtPen)
    member this.Brush with set (value: Brush) = qtPainter.SetBrush(value.qtBrush)
    member this.Font with set (value: Font) = qtPainter.SetFont(value.qtFont)
    
    member this.SetRenderHint (hint: RenderHint) (state: bool) =
        qtPainter.SetRenderHint(hint.QtValue, state)
        
    member this.SetRenderHints (hints: Set<RenderHint>) (state: bool) =
        let qHints =
            hints |> Set.map (_.QtValue)
        qtPainter.SetRenderHints(HashSet(qHints), state)
    
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
        
    member this.FillPath(path: PainterPath, brush: Brush) =
        qtPainter.FillPath(path.qtPainterPath, brush.qtBrush)
        
    member this.StrokePath(path: PainterPath, pen: Pen) =
        qtPainter.StrokePath(path.qtPainterPath, pen.qtPen)
        
    member this.DrawPolyline(points: Common.PointF array) =
        qtPainter.DrawPolyline(points)
