module Tabs.CircleDrawer

open System
open BuilderNode
open SubReactor
open Widgets
open Org.Whatever.QtTesting
open CustomWidget
open type PaintResources
open Widgets.BoxLayout
open Widgets.Dialog
open Widgets.PushButton
open Widgets.Slider
open WithDialogs

type Signal = unit
type Attr = unit

type Circle = {
    Location: Common.Point
    Radius: int
}

type State = {
    Circles: Circle list
    MaybeHoverIndex: int option
    EditingRadius: int
}
        
type Msg =
    | NoOp
    | AddCircle of loc: Common.Point
    | ShowDialog of loc: Common.Point
    | MouseMove of loc: Common.Point
    | SetRadius of radius: int
        
let init() =
    let state =
        { Circles = []
          MaybeHoverIndex = None
          EditingRadius = 0 }
    state, Cmd.None
    
let update (state: State) = function
    | NoOp ->
        state, Cmd.None
    | AddCircle loc ->
        let circle =
            { Location = loc; Radius = 10 }
        let nextState =
            { state with Circles = circle :: state.Circles }
        nextState, Cmd.None
    | ShowDialog loc ->
        printfn "show dialog @ %A" loc
        state, Cmd.DialogOp ("edit", Exec)
    | SetRadius value ->
        printfn "set radius: %d" value
        state, Cmd.None
    | MouseMove loc ->
        let dist (p1: Common.Point) (p2: Common.Point) =
            let a = Math.Pow(float (p1.X - p2.X), 2.0)
            let b = Math.Pow(float (p1.Y - p2.Y), 2.0)
            sqrt (a + b)
        let nextHoverIndex =
            state.Circles
            |> List.tryFindIndex (fun circle -> dist circle.Location loc < circle.Radius)
        { state with MaybeHoverIndex = nextHoverIndex }, Cmd.None
        
type Woot(state: State) =
    inherit PaintStateBase<State>(state)
    // by default, without overriding .StateEquals, ANY change in our state from previous will trigger a redraw
    // .StateEquals gives you the ability to define a subset of state which is pertinent for repaints
    override this.DoPaint widget painter paintRect =
        use bgColor = Color.Create(Color.Constant.DarkBlue)
        use fgColor = Color.Create(Color.Constant.Yellow)
        use hoverColor = Color.Create(Color.Constant.Magenta)
        use pen = Pen.Create(fgColor)
        painter.FillRect(widget.GetRect(), bgColor)
        painter.SetPen(pen)
        for i, circle in state.Circles |> List.zipWithIndex do
            let rect = Common.Rect(X = circle.Location.X - circle.Radius, Y = circle.Location.Y - circle.Radius, Width = circle.Radius * 2, Height = circle.Radius * 2)
            match state.MaybeHoverIndex with
            | Some index when i = index ->
                painter.FillRect(rect, hoverColor)
            | _ ->
                painter.DrawRect(rect)

let view (state: State) =
    let undo =
        PushButton(Attrs = [ Text "Undo" ])
    let redo =
        PushButton(Attrs = [ Text "Redo" ])
    let hbox =
        BoxLayout(
            Attrs = [ Direction LeftToRight ],
            Items = [
                BoxItem.Stretch 1
                BoxItem.Create(undo)
                BoxItem.Create(redo)
                BoxItem.Stretch 1
            ])
    let canvas =
        let moveFunc info =
            MouseMove info.Position
        let pressFunc (info: MousePressInfo) =
            match info.Button with
            | Widget.MouseButton.Left ->
                AddCircle info.Position
            | Widget.MouseButton.Right ->
                ShowDialog info.Position
            | _ ->
                NoOp
        CustomWidget(
            Attrs = [ PaintState(Woot(state)); MouseTracking true ], OnMousePress = pressFunc, OnMouseMove = moveFunc) // tracking needed for move events without mouse down
    let dialog =
        let slider =
            Slider(Attrs = [
                Orientation Horizontal
                Range(5, 100)
                Value 10
            ], OnValueChanged = SetRadius )
        let cancel =
            PushButton(Attrs = [ Text "Cancel" ])
        let apply =
            PushButton(Attrs = [ Text "OK" ])
        let vbox =
            let hbox =
                BoxLayout(
                    Attrs = [ Direction LeftToRight ],
                    Items = [
                        BoxItem.Stretch 1
                        BoxItem.Create(cancel)
                        BoxItem.Create(apply)
                        BoxItem.Stretch 1
                    ])
            BoxLayout(Items = [
                // BoxItem.Create(label)
                BoxItem.Create(slider)
                BoxItem.Create(hbox)
            ])
        Dialog(Attrs = [ Title "Edit Radius" ], Layout = vbox)
    let vbox =
        BoxLayout(Items = [
            BoxItem.Create(hbox)
            BoxItem.Create(canvas)
        ])
    LayoutWithDialogs(vbox, [ "edit", dialog ])
    :> ILayoutNode<Msg>
    
type CircleDrawer<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
