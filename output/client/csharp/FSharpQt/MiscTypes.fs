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
    member this.QtValue =
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
    member this.QtValue =
        Common.Point(X = this.X, Y = this.Y)
        
type PointF = {
    X: double
    Y: double
} with
    member this.QtValue =
        Common.PointF(X = this.X, Y = this.Y)
        
type Rect = {
    X: int
    Y: int
    Width: int
    Height: int
} with
    member this.QtValue =
        Common.Rect(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)
    static member FromQtRect(rect: Common.Rect) =
        { X = rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }

type RectF = {
    X: double
    Y: double
    Width: double
    Height: double
} with
    member this.QtValue =
        Common.RectF(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)


// for anything where we don't want users to be dealing with Org.Whatever.QtTesting namespace / C# shit
type WidgetProxy internal(widget: Widget.Handle) =
    member val widget = widget
