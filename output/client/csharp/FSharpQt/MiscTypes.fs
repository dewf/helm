module FSharpQt.MiscTypes

open System.Collections.Generic
open Org.Whatever.QtTesting
open FSharpQt.InputEnums

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
        | Left -> Enums.Alignment.AlignLeft
        | Leading -> Enums.Alignment.AlignLeading
        | Right -> Enums.Alignment.AlignRight
        | Trailing -> Enums.Alignment.AlignTrailing
        | HCenter -> Enums.Alignment.AlignHCenter
        | Justify -> Enums.Alignment.AlignJustify
        | Absolute -> Enums.Alignment.AlignAbsolute
        | Top -> Enums.Alignment.AlignTop
        | Bottom -> Enums.Alignment.AlignBottom
        | VCenter ->Enums.Alignment.AlignVCenter
        | Baseline -> Enums.Alignment.AlignBaseline
        | Center -> Enums.Alignment.AlignCenter
        
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
    static member internal From(style: Enums.ToolButtonStyle) =
        match style with
        | Enums.ToolButtonStyle.IconOnly -> IconOnly
        | Enums.ToolButtonStyle.TextOnly -> TextOnly
        | Enums.ToolButtonStyle.TextBesideIcon -> TextBesideIcon
        | Enums.ToolButtonStyle.TextUnderIcon -> TextUnderIcon
        | Enums.ToolButtonStyle.FollowStyle -> FollowStyle
        | _ -> failwith "ToolButtonStyle.From - unknown enum value"
    member internal this.QtValue =
        match this with
        | IconOnly -> Enums.ToolButtonStyle.IconOnly
        | TextOnly -> Enums.ToolButtonStyle.TextOnly
        | TextBesideIcon -> Enums.ToolButtonStyle.TextBesideIcon
        | TextUnderIcon -> Enums.ToolButtonStyle.TextUnderIcon
        | FollowStyle -> Enums.ToolButtonStyle.FollowStyle
        
// for utility widgets (synthetic layout widgets etc)

type internal NullWidgetHandler() =
    interface Widget.SignalHandler with
        member this.CustomContextMenuRequested pos =
            ()
        member this.WindowIconChanged icon =
            ()
        member this.WindowTitleChanged title =
            ()
        member this.Dispose() =
            ()
    
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
    member internal this.Handle = icon
    
type DockWidgetProxy internal(widget: DockWidget.Handle) =
    let x = 10
    
type MimeDataProxy internal(qMimeData: Widget.MimeData) =
    member val qMimeData = qMimeData
    member this.HasFormat(mimeType: string) =
        qMimeData.HasFormat(mimeType)
    member this.Text =
        qMimeData.Text()
    member this.Urls =
        qMimeData.Urls()
    
// experimenting for extreme cases:
   
type ProxyBase<'handle> internal() =
    member val internal Handle: 'handle = Unchecked.defaultof<'handle> with get, set

type PlainTextEditProxy() =
    inherit ProxyBase<PlainTextEdit.Handle>()
    internal new(handle: PlainTextEdit.Handle) =
        base.Handle <- handle
        PlainTextEditProxy()
    member this.ToPlainText () =
        this.Handle.ToPlainText()

// other =========================

type KeySequenceProxy(seq: KeySequence.Handle) =
    new(standardKey: StandardKey) =
        let handle =
            KeySequence.Create(standardKey.QtValue)
        KeySequenceProxy(handle)
    new(key: Key) =
        let handle =
            KeySequence.Create(key.QtValue, HashSet<Enums.Modifier>())
        KeySequenceProxy(handle)
    new(key: Key, modifiers: Set<Modifier>) =
        let handle =
            KeySequence.Create(key.QtValue, Modifier.QtSetFrom(modifiers))
        KeySequenceProxy(handle)
    member internal this.Handle = seq
