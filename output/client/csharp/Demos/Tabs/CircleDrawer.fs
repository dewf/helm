module Tabs.CircleDrawer

open BuilderNode
open SubReactor
open Widgets
open Org.Whatever.QtTesting
open CustomWidget
open type PaintResources
open Widgets.BoxLayout
open Widgets.PushButton

type Signal = unit
type Attr = unit

type Circle = {
    Location: Common.Point
    Radius: int
}

type State = {
    Circles: Circle list
    MaybeEditingIndex: int option
    EditingRadius: int
} with
    static member Init =
        { Circles = []; MaybeEditingIndex = None; EditingRadius = 0 }
        
type Msg =
    | NoOp
    | AddCircle of loc: Common.Point
    | ShowDialog of loc: Common.Point
        
let init() =
    State.Init, Cmd.None
    
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
        state, Cmd.None
        
type Woot(state: State) =
    inherit PaintStateBase<State>(state)
    // by default, without overriding .StateEquals, ANY change in our state from previous will trigger a redraw
    // .StateEquals gives you the ability to define a subset of state which is pertinent for repaints
    override this.DoPaint widget painter paintRect =
        use bgColor = Color.Create(Color.Constant.DarkBlue)
        use fgColor = Color.Create(Color.Constant.Yellow)
        use pen = Pen.Create(fgColor)
        painter.FillRect(widget.GetRect(), bgColor)
        painter.SetPen(pen)
        for circle in state.Circles do
            let rect = Common.Rect(X = circle.Location.X - circle.Radius, Y = circle.Location.Y - circle.Radius, Width = circle.Radius * 2, Height = circle.Radius * 2)
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
        let pressFunc (info: MousePressInfo) =
            match info.Button with
            | Widget.MouseButton.Left ->
                AddCircle info.Position
            | Widget.MouseButton.Right ->
                ShowDialog info.Position
            | _ ->
                NoOp
        CustomWidget(
            Attrs = [ PaintState(Woot(state)) ], OnMousePress = pressFunc)
    let vbox =
        BoxLayout(Items = [
            BoxItem.Create(hbox)
            BoxItem.Create(canvas)
        ])
    vbox :> ILayoutNode<Msg>
    
type CircleDrawer<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
