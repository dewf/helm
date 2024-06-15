module FSharpQt.MiscTypes

open Org.Whatever.QtTesting

type Alignment =
    | Left
    | Leading
    | Right
    | Trailing
    | HCenter
    | Justify
    | Absolute
    | Top
    | Bottom
    | VCenter
    | Baseline
    | Center
with
    member internal this.QtValue =
        match this with
        | Left -> Common.Alignment.Left
        | Leading -> Common.Alignment.Leading
        | Right -> Common.Alignment.Right
        | Trailing -> Common.Alignment.Trailing
        | HCenter -> Common.Alignment.HCenter
        | Justify -> Common.Alignment.Justify
        | Absolute -> Common.Alignment.Absolute
        | Top -> Common.Alignment.Top
        | Bottom -> Common.Alignment.Bottom
        | VCenter ->Common.Alignment.VCenter
        | Baseline -> Common.Alignment.Baseline
        | Center -> Common.Alignment.Center
        
type Point = {
    X: int
    Y: int
} with
    static member From (x, y) =
        { X = x; Y = y }
    static member internal From(p: Common.Point) =
        { X = p.X; Y = p.Y }
    member internal this.QtValue =
        Common.Point(X = this.X, Y = this.Y)
        
type PointF = {
    X: double
    Y: double
} with
    static member From (x, y) =
        { X = x; Y = y }
    static member From(p: Point) =
        { X = double p.X; Y = p.Y }
    static member internal From(p: Common.Point) =
        { X = p.X; Y = p.Y }
    static member internal From(p: Common.PointF) =
        { X = p.X; Y = p.Y }
    member internal this.QtValue =
        Common.PointF(X = this.X, Y = this.Y)
        
type Size = {
    Width: int
    Height: int
} with
    static member Invalid =
        // useful in some places
        { Width = -1; Height = -1 }
    static member From (w, h) =
        { Width = w; Height = h }
    static member internal From (sz: Common.Size) =
        { Width = sz.Width; Height = sz.Height }
    member internal this.QtValue =
        Common.Size(Width = this.Width, Height = this.Height)
        
type Rect = {
    X: int
    Y: int
    Width: int
    Height: int
} with
    static member From (x, y, width, height) =
        { X = x; Y = y; Width = width; Height = height }
    static member From (size: Size) =
        { X = 0; Y = 0; Width = size.Width; Height = size.Height }
    static member internal From(rect: Common.Rect) =
        { X = rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    member internal this.QtValue =
        Common.Rect(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)

type RectF = {
    X: double
    Y: double
    Width: double
    Height: double
} with
    static member From (x, y, width, height) =
        { X = x; Y = y; Width = width; Height = height }
    static member From (size: Size) =
        { X = 0; Y = 0; Width = size.Width; Height = size.Height }
    static member internal From(rect: Common.Rect) =
        { X = double rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    static member internal From(rect: Common.RectF) =
        { X = rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    member internal this.QtValue =
        Common.RectF(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)

type ToolButtonStyle =
    | IconOnly
    | TextOnly
    | TextBesideIcon
    | TextUnderIcon
    | FollowStyle
with
    static member internal From(style: Common.ToolButtonStyle) =
        match style with
        | Common.ToolButtonStyle.IconOnly -> IconOnly
        | Common.ToolButtonStyle.TextOnly -> TextOnly
        | Common.ToolButtonStyle.TextBesideIcon -> TextBesideIcon
        | Common.ToolButtonStyle.TextUnderIcon -> TextUnderIcon
        | Common.ToolButtonStyle.FollowStyle -> FollowStyle
        | _ -> failwith "ToolButtonStyle.From - unknown enum value"
    member internal this.QtValue =
        match this with
        | IconOnly -> Common.ToolButtonStyle.IconOnly
        | TextOnly -> Common.ToolButtonStyle.TextOnly
        | TextBesideIcon -> Common.ToolButtonStyle.TextBesideIcon
        | TextUnderIcon -> Common.ToolButtonStyle.TextUnderIcon
        | FollowStyle -> Common.ToolButtonStyle.FollowStyle
    
// for anything where we don't want users to be dealing with Org.Whatever.QtTesting namespace (generated C# code)

type WidgetProxy internal(handle: Widget.Handle) =
    // member val widget = widget
    member this.Rect =
        Rect.From(handle.GetRect())

type ActionProxy internal(action: Action.Handle) =
    // not sure what methods/props will be useful yet
    let x = 10

type IconProxy internal(icon: Icon.Handle) =
    let x = 10
    
type DockWidgetProxy internal(widget: DockWidget.Handle) =
    let x = 10
    
// experimenting for extreme cases:
   
type ProxyBase<'handle> internal() =
    member val internal Handle: 'handle = Unchecked.defaultof<'handle> with get, set

type PlainTextEditProxy() =
    inherit ProxyBase<PlainTextEdit.Handle>()
    member this.ToPlainText () =
        this.Handle.ToPlainText()
