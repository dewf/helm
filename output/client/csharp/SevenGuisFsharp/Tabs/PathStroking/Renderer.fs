module Tabs.PathStroking.Renderer

open System
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
    
let private attrKey = function
    | CapStyle _ -> 0
    | JoinStyle _ -> 1
    | PenStyle _ -> 2
    | PenWidth _ -> 3
    | LineStyle _ -> 4
    | Animating _ -> 5

let private diffAttrs =
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
        (171.816851753197, 415.34898018587336)
        (152.5072087818176, 330.74790660436855)
        (206.61162608824418, 262.9031132097581)
        (293.3883739117558, 262.9031132097581)
        (347.4927912181824, 330.74790660436855)
        (328.183148246803, 415.34898018587336) ] |> List.map PointF.From
    let vectors = [
        (1.8000000044703484, 0.44999999552965164)
        (1.0050069307528509, 1.1014934681357038)
        (-0.5467768602268507, 0.923539892863587)
        (-1.68682652324041, 0.050141941484603825)
        (-1.5566614092635886, -0.8610139145414915)
        (-0.2542985040053737, -1.123808731434778)
        (1.2395563615130234, -0.5403526520372772) ] |> List.map PointF.From
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
