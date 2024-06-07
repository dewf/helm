module Tabs.PathStroking.Renderer

open FSharpQt.BuilderNode
open FSharpQt.NonVisual
open FSharpQt.Painting
open FSharpQt.Reactor
open FSharpQt.Widgets.CustomWidget
open FSharpQt.Widgets.Timer
open Org.Whatever.QtTesting

open FSharpQt.MiscTypes

let POINT_SIZE = 10.0
let TIMER_INTERVAL = 25 // millis

type LineStyle =
    | Curves
    | Lines

type Signal = unit

type Attr =
    | CapStyle of style: CapStyle
    | JoinStyle of style: JoinStyle
    | PenStyle of style: PenStyle
    | PenWidth of width: int
    | LineStyle of style: LineStyle
    | Animating of value: bool
    
let private diffAttrs =
    let attrKey = function
        | CapStyle _ -> 0
        | JoinStyle _ -> 1
        | PenStyle _ -> 2
        | PenWidth _ -> 3
        | LineStyle _ -> 4
        | Animating _ -> 5
    genericDiffAttrs attrKey
    
type PathPoint = {
    Position: PointF
    Velocity: PointF
}

type State = {
    ViewRect: RectF
    Points: PathPoint array
    CapStyle: CapStyle
    JoinStyle: JoinStyle
    PenStyle: PenStyle
    PenWidth: int
    LineStyle: LineStyle
    Animating: bool
}

type Msg =
    | Resized of rect: RectF
    | TimerTick of elapsed: double

let init() =
    let points = [|
        (250.0, 453.0)
        (171.81, 415.34)
        (152.50, 330.74)
        (206.61, 262.90)
        (293.38, 262.90)
        (347.49, 330.74)
        (328.18, 415.34) |] |> Array.map PointF.From
    let velocities = [|
        (1.800, 0.449)
        (1.005, 1.101)
        (-0.546, 0.923)
        (-1.686, 0.050)
        (-1.556, -0.861)
        (-0.254, -1.123)
        (1.239, -0.540) |] |> Array.map PointF.From
    let anchors =
        Array.zip points velocities
        |> Array.map (fun (pos, vel) -> { Position = pos; Velocity = vel })
    let state = {
        ViewRect = RectF.From(0, 0, 0, 0)
        Points = anchors
        CapStyle = Flat
        JoinStyle = Bevel
        PenStyle = SolidLine
        PenWidth = 5
        LineStyle = Curves
        Animating = true
    }
    state, Cmd.None

let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | CapStyle style -> { state with CapStyle = style }
    | JoinStyle style -> { state with JoinStyle = style }
    | PenStyle style -> { state with PenStyle = style }
    | PenWidth width -> { state with PenWidth = width }
    | LineStyle style -> { state with LineStyle = style }
    | Animating value -> { state with Animating = value }
    
let stepSinglePoint elapsedMillis left right top bottom { Position = pos; Velocity = vel } =
    let xDeltaAdjusted =
        (vel.X * elapsedMillis) / (double TIMER_INTERVAL)
    let yDeltaAdjusted =
        (vel.Y * elapsedMillis) / (double TIMER_INTERVAL)
    let projected =
        { X = pos.X + xDeltaAdjusted
          Y = pos.Y + yDeltaAdjusted }
    let nextPoint, nextVector =
        if projected.X < left then
            { projected with X = left }, { vel with X = -vel.X }
        elif projected.X > right then
            { projected with X = right }, { vel with X = -vel.X }
        elif projected.Y < top then
            { projected with Y = top }, { vel with Y = -vel.Y }
        elif projected.Y > bottom then
            { projected with Y = bottom }, { vel with Y = -vel.Y }
        else
            projected, vel
    { Position = nextPoint; Velocity = nextVector }
    
let stepPoints (elapsed: double) (anchors: PathPoint array) (bounds: RectF) =
    let pad = float POINT_SIZE
    let left = pad
    let right = bounds.Width - pad
    let top = pad
    let bottom = bounds.Height - pad

    anchors
    |> Array.map (stepSinglePoint elapsed left right top bottom)

let update (state: State) (msg: Msg) =
    match msg with
    | Resized rect ->
        { state with ViewRect = rect }, Cmd.None
    | TimerTick elapsed ->
        let nextState =
            if state.Animating then
                let nextAnchors =
                    stepPoints elapsed state.Points state.ViewRect
                { state with Points = nextAnchors }
            else
                state
        nextState, Cmd.None
        
let private bgColor = Color.DarkGray
let private lineColorBrush = Brush(Color.Red)
let private noPen = Pen(NoPen)
let private controlPointPen = Pen(Color(50, 100, 120, 200))
let private controlPointBrush = Brush(Color(200, 200, 210, 120))
        
type EventDelegate(state: State) =
    inherit EventDelegateBase<Msg,State>(state)
    
    override this.SizeHint = Common.Size (600, 600)
    
    override this.NeedsPaint _ =
        Everything

    override this.Resize _ newSize =
        RectF.From(0, 0, newSize.Width, newSize.Height)
        |> Resized
        |> Some
    
    override this.DoPaint _ painter widgetRect =
        painter.SetRenderHint Antialiasing true
        painter.FillRect(widgetRect, bgColor)
        
        painter.Pen <- noPen

        // construct path        
        let path = PainterPath()
        path.MoveTo(state.Points[0].Position.QtValue)
        match state.LineStyle with
        | Lines ->
            seq { 1 .. state.Points.Length - 1 }
            |> Seq.iter (fun i ->
                path.LineTo(state.Points[i].Position.QtValue))
        | Curves ->
            let mutable i = 1
            while i + 2 < state.Points.Length do
                path.CubicTo(state.Points[i].Position.QtValue, state.Points[i+1].Position.QtValue, state.Points[i+2].Position.QtValue)
                i <- i + 3
            while i < state.Points.Length do
                path.LineTo(state.Points[i].Position.QtValue)
                i <- i + 1

        // draw path
        match state.PenStyle with
        | CustomDashLine ->
            let stroker =
                PainterPathStroker(Width = state.PenWidth, JoinStyle = state.JoinStyle, CapStyle = state.CapStyle)
            let dashes =
                let space = 4
                [| 1.0; space; 3; space; 9; space; 27; space; 9; space; 3; space |]
            stroker.DashPattern <- dashes
            let stroke = stroker.CreateStroke(path)
            painter.FillPath(stroke, lineColorBrush)
        | _ ->
            let pen = Pen(lineColorBrush, state.PenWidth, state.PenStyle, state.CapStyle, state.JoinStyle)
            painter.StrokePath(path, pen)
        
        // draw control points
        painter.Pen <- controlPointPen
        painter.Brush <- controlPointBrush
        for anchor in state.Points do
            painter.DrawEllipse(anchor.Position.QtValue, POINT_SIZE, POINT_SIZE)
        painter.Pen <- Pen(Color.LightGray, 0, SolidLine)
        painter.Brush <- Brush.NoBrush
        let points =
            state.Points
            |> Array.map (_.Position.QtValue)
        painter.DrawPolyline(points)

        
let view (state: State) =
    let custom =
        CustomWidget(EventDelegate(state), [ PaintEvent; SizeHint; ResizeEvent ])
    let timer =
        Timer(Attrs = [ Interval TIMER_INTERVAL; Running true ], OnTimeout = TimerTick)
    WidgetWithNonVisual(custom, [ "timer", timer ])
    :> IWidgetNode<Msg>

type PathStrokeRenderer<'outerMsg>() =
    inherit WidgetReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, diffAttrs)
