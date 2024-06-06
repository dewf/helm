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

type CapStyle =
    | Flat
    | Square
    | Round
    
type JoinStyle =
    | Bevel
    | Miter
    | SvgMiter
    | Round
    
type PenStyle =
    | Solid
    | Dash
    | Dot
    | DashDot
    | DashDotDot
    | CustomDash
    
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
    
type LineAnchor = {
    Pos: PointF
    Vector: PointF
}

type State = {
    Anchors: LineAnchor list
    CapStyle: CapStyle
    JoinStyle: JoinStyle
    PenStyle: PenStyle
    PenWidth: int
    LineStyle: LineStyle
    Animating: bool
}

type Msg =
    | TimerTick of elapsed: double

let init() =
    let points = [
        (250.0, 453.0)
        (171.81, 415.34)
        (152.50, 330.74)
        (206.61, 262.90)
        (293.38, 262.90)
        (347.49, 330.74)
        (328.18, 415.34) ] |> List.map PointF.From
    let vectors = [
        (1.800, 0.449)
        (1.005, 1.101)
        (-0.546, 0.923)
        (-1.686, 0.050)
        (-1.556, -0.861)
        (-0.254, -1.123)
        (1.239, -0.540) ] |> List.map PointF.From
    let anchors =
        List.zip points vectors
        |> List.map (fun (pos, vec) -> { Pos = pos; Vector = vec })
    let state = {
        Anchors = anchors
        CapStyle = Flat
        JoinStyle = Bevel
        PenStyle = Solid
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
    
let stepPoint elapsedMillis left right top bottom { Pos = p; Vector = v } =
    let xDeltaAdjusted =
        (v.X * elapsedMillis) / (double TIMER_INTERVAL)
    let yDeltaAdjusted =
        (v.Y * elapsedMillis) / (double TIMER_INTERVAL)
    let projected =
        { X = p.X + xDeltaAdjusted
          Y = p.Y + yDeltaAdjusted }
    let nextPoint, nextVector =
        if projected.X < left then
            { projected with X = left }, { v with X = -v.X }
        elif projected.X > right then
            { projected with X = right }, { v with X = -v.X }
        elif projected.Y < top then
            { projected with Y = top }, { v with Y = -v.Y }
        elif projected.Y > bottom then
            { projected with Y = bottom }, { v with Y = -v.Y }
        else
            projected, v
    { Pos = nextPoint; Vector = nextVector }
    
    
let stepAnchors (elapsed: double) (anchors: LineAnchor list) (bounds: RectF) =
    let pad = float POINT_SIZE
    let left = pad
    let right = bounds.Width - pad
    let top = pad
    let bottom = bounds.Height - pad

    anchors
    |> List.map (stepPoint elapsed left right top bottom)

let update (state: State) (msg: Msg) =
    match msg with
    | TimerTick elapsed ->
        let nextState =
            if state.Animating then
                let nextAnchors =
                    stepAnchors elapsed state.Anchors (RectF.From(0, 0, 600, 600)) // hmm, how can we get the actual widget rect here? need to handle a size event I think
                { state with Anchors = nextAnchors }
            else
                state
        nextState, Cmd.None
        
let private bgColor = Color.DarkGray
let private controlPointPen = Pen(Color(50, 100, 120, 200))
let private controlPointBrush = Brush(Color(200, 200, 210, 120))
        
type EventDelegate(state: State) =
    inherit EventDelegateBase<Msg,State>(state)
    
    override this.SizeHint = Common.Size (600, 600)
    
    override this.NeedsPaint prev =
        Everything
    
    override this.DoPaint widget painter widgetRect =
        painter.SetRenderHint Antialiasing true
        painter.FillRect(widgetRect, bgColor)
        
        // draw control points
        painter.Pen <- controlPointPen
        painter.Brush <- controlPointBrush
        for anchor in state.Anchors do
            painter.DrawEllipse(anchor.Pos.QtValue, POINT_SIZE, POINT_SIZE)
        
        // painter->setPen(QPen(Qt::lightGray, 0, Qt::SolidLine));
        // painter->setBrush(Qt::NoBrush);
        // painter->drawPolyline(m_points);
            
        
let view (state: State) =
    let custom =
        CustomWidget(EventDelegate(state), [ PaintEvent; SizeHint ])
    let timer =
        Timer(Attrs = [ Interval TIMER_INTERVAL; Running true ], OnTimeout = TimerTick)
    WidgetWithNonVisual(custom, [ "timer", timer ])
    :> IWidgetNode<Msg>

type PathStrokeRenderer<'outerMsg>() =
    inherit WidgetReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, diffAttrs)
