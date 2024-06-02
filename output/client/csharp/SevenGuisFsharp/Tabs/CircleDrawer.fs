module Tabs.CircleDrawer

open System

open FSharpQt
open BuilderNode
open SubReactor
open FSharpQt.Widgets
open CustomWidget
open Widgets.BoxLayout
open Widgets.Dialog
open Widgets.Menus.MenuAction
open Widgets.Menus.Menu
open Widgets.PushButton
open Widgets.Slider
open WithDialogs

open Extensions

open Org.Whatever.QtTesting
open type PaintResources

type Signal = unit
type Attr = unit

type Circle = {
    Location: Common.Point
    Radius: int
}

type UndoItem =
    | CircleAdded of circle: Circle
    | RadiusChanged of index: int * oldRadius: int * newRadius: int
    
type State = {
    Circles: Circle list
    MaybeHoverIndex: int option
    NowEditing: bool
    EditingRadius: int
    UndoStack: UndoItem list
    RedoStack: UndoItem list
}

let circleAtIndex (index: int) (state: State) =
    state.Circles
    |> List.item index
        
type Msg =
    | NoOp
    | AddCircle of loc: Common.Point
    | ShowContext of loc: Common.Point
    | ShowDialog
    | MouseMove of loc: Common.Point
    | SetRadius of radius: int
    | ApplyEdit
    | CancelEdit
    | DialogClosed of accepted: bool
    | Undo
    | Redo
    
let init() =
    let state =
        { Circles = []
          MaybeHoverIndex = None
          NowEditing = false 
          EditingRadius = 0
          UndoStack = []
          RedoStack = [] }
    state, Cmd.None
    
let update (state: State) = function
    | NoOp ->
        state, Cmd.None
    | AddCircle loc ->
        let circle =
            { Location = loc; Radius = 35 }
        let nextUndoStack =
            CircleAdded circle :: state.UndoStack
        let nextState =
            { state with
                Circles = circle :: state.Circles
                UndoStack = nextUndoStack
                RedoStack = []
                MaybeHoverIndex = Some 0 }
        nextState, Cmd.None
    | ShowContext loc ->
        match state.MaybeHoverIndex with
        | Some _ ->
            state, Cmd.PopMenu ("context", loc)
        | None ->
            state, Cmd.None
    | ShowDialog ->
        match state.MaybeHoverIndex with
        | Some index ->
            let circle =
                state |> circleAtIndex index
            { state with NowEditing = true; EditingRadius = circle.Radius }, Cmd.DialogOp ("edit", Exec)
        | None ->
            state, Cmd.None
    | SetRadius value ->
        { state with EditingRadius = value }, Cmd.None
    | MouseMove loc ->
        let dist (p1: Common.Point) (p2: Common.Point) =
            let a = Math.Pow(float (p1.X - p2.X), 2.0)
            let b = Math.Pow(float (p1.Y - p2.Y), 2.0)
            sqrt (a + b)
        let nextHoverIndex =
            state.Circles
            |> List.tryFindIndex (fun circle -> dist circle.Location loc < circle.Radius)
        { state with MaybeHoverIndex = nextHoverIndex }, Cmd.None
    | ApplyEdit ->
        state, Cmd.DialogOp ("edit", Accept)
    | CancelEdit ->
        state, Cmd.DialogOp ("edit", Reject)
    | DialogClosed accepted ->
        // this also catches the case where the dialog is closed with the [X] and not via the cancel button
        // hence not changing any state in the CancelEdit handler
        // the other option would be to paramterize the CancelEdit msg to indicate its origin (cancel button vs. dialog [X]),
        // to avoid invoking a Cmd.Dialog:Reject which would create a feedback loop
        let nextState =
            match accepted, state.MaybeHoverIndex with
            | true, Some index ->
                // apply change ================
                let nextCircles, nextUndoStack =
                    let mutable oldRadius: int = -1 // 😮
                    let nextCircles =
                        state.Circles
                        |> List.replaceAtIndex index (fun cir ->
                            oldRadius <- cir.Radius
                            { cir with Radius = state.EditingRadius })
                    let nextUndoStack =
                        RadiusChanged (index, oldRadius, state.EditingRadius) :: state.UndoStack
                    nextCircles, nextUndoStack
                { state with
                    Circles = nextCircles
                    UndoStack = nextUndoStack
                    RedoStack = []
                    NowEditing = false }
            | _ ->
                // ignore/revert ================
                { state with NowEditing = false }
        nextState, Cmd.None
    | Undo ->
        let nextState =
            match state.UndoStack with
            | item :: etc ->
                match item with
                | CircleAdded _ ->
                    let nextCircles =
                        state.Circles |> List.skip 1
                    let nextRedoStack =
                        item :: state.RedoStack
                    { state with Circles = nextCircles; UndoStack = etc; RedoStack = nextRedoStack }
                | RadiusChanged (index, oldRadius, _) ->
                    let nextCircles =
                        state.Circles
                        |> List.replaceAtIndex index (fun cir -> { cir with Radius = oldRadius })
                    let nextRedoStack =
                        item :: state.RedoStack
                    { state with Circles = nextCircles; UndoStack = etc; RedoStack = nextRedoStack }
            | [] ->
                // nothing to undo 
                state
        nextState, Cmd.None
    | Redo ->
        let nextState =
            match state.RedoStack with
            | item :: etc ->
                match item with
                | CircleAdded circle ->
                    let nextCircles =
                        circle :: state.Circles
                    let nextUndoStack =
                        item :: state.UndoStack
                    { state with Circles = nextCircles; UndoStack = nextUndoStack; RedoStack = etc }
                | RadiusChanged (index, _, newRadius) ->
                    let nextCircles =
                        state.Circles
                        |> List.replaceAtIndex index (fun cir -> { cir with Radius = newRadius })
                    let nextUndoStack =
                        item :: state.UndoStack
                    { state with Circles = nextCircles; UndoStack = nextUndoStack; RedoStack = etc }
            | [] ->
                // nothing to redo
                state
        nextState, Cmd.None
        
type PaintResources = {
    BgColor: Color
    FgColor: Color
    HoverColor: Color
    BgBrush: Brush
    ClearBrush: Brush
    HoverBrush: Brush
    Pen: Pen
}

type DrawerPaintState(state: State) =
    inherit PaintStateBase<State, PaintResources>(state)
    override this.CreateResources() =
        let bgColor = Color.Create(Color.Constant.DarkBlue)
        let fgColor = Color.Create(Color.Constant.Yellow)
        let hoverColor = Color.Create(Color.Constant.Magenta)
        let bgBrush = Brush.Create(bgColor)
        let clearBrush = Brush.Create(Brush.Style.NoBrush)
        let hoverBrush = Brush.Create(hoverColor)
        let pen = Pen.Create(fgColor)
        { BgColor = bgColor
          FgColor = fgColor
          HoverColor = hoverColor
          BgBrush = bgBrush
          ClearBrush = clearBrush
          HoverBrush = hoverBrush
          Pen = pen }
    override this.DestroyResources(res: PaintResources) =
        res.BgColor.Dispose()
        res.FgColor.Dispose()
        res.HoverColor.Dispose()
        res.BgBrush.Dispose()
        res.ClearBrush.Dispose()
        res.HoverBrush.Dispose()
        res.Pen.Dispose()

    // by default, without overriding .StateEquals, ANY change in our state from previous will trigger a redraw
    // .StateEquals gives you the ability to define a subset of state which is pertinent for repaints
        
    override this.DoPaint resources widget painter paintRect =
        painter.FillRect(widget.GetRect(), resources.BgColor)
        painter.SetPen(resources.Pen)
        for i, circle in state.Circles |> List.zipWithIndex |> List.rev do
            let brush, radius =
                match state.MaybeHoverIndex with
                | Some index when i = index ->
                    let radius =
                        if state.NowEditing then
                            state.EditingRadius
                        else
                            circle.Radius
                    resources.HoverBrush, radius
                | _ ->
                    resources.BgBrush, circle.Radius
            painter.SetBrush(brush)
            painter.DrawEllipse(circle.Location, radius, radius)

let view (state: State) =
    let undoRedoButtons =
        let undo =
            let enabled =
                not state.UndoStack.IsEmpty
            PushButton(Attrs = [ Text "Undo"; Enabled enabled ], OnClicked = Undo)
        let redo =
            let enabled =
                not state.RedoStack.IsEmpty
            PushButton(Attrs = [ Text "Redo"; Enabled enabled ], OnClicked = Redo)
        BoxLayout(
            Attrs = [ Direction LeftToRight ],
            Items = [
                BoxItem.Stretch 1
                BoxItem.Create(undo)
                BoxItem.Create(redo)
                BoxItem.Stretch 1
            ])
    let canvas =
        let contextMenu =
            let action =
                MenuAction(Attrs = [ Widgets.Menus.MenuAction.Text "Edit Radius" ], OnTriggered = ShowDialog) // no idea why we have to fully qualify .Text attribute. why isn't MenuAction.Text sufficient?
            Menu(Items = [ action ])
        let moveFunc info =
            MouseMove info.Position
        let pressFunc (info: MousePressInfo) =
            match info.Button with
            | Widget.MouseButton.Left ->
                AddCircle info.Position
            | Widget.MouseButton.Right ->
                ShowContext info.Position
            | _ ->
                NoOp
        CustomWidget(
            Attrs = [ PaintState(DrawerPaintState(state)); MouseTracking true ],
            Menus = [ "context", contextMenu ],
            OnMousePress = pressFunc,
            OnMouseMove = moveFunc) // tracking needed for move events without mouse down
    let dialog =
        let slider =
            Slider(Attrs = [
                Orientation Horizontal
                Range(5, 100)
                Value state.EditingRadius
            ], OnValueChanged = SetRadius )
        let cancel =
            PushButton(Attrs = [ Text "Cancel" ], OnClicked = CancelEdit)
        let apply =
            PushButton(Attrs = [ Text "OK" ], OnClicked = ApplyEdit)
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
                BoxItem.Create(slider)
                BoxItem.Spacer 10
                BoxItem.Create(hbox)
            ])
        Dialog(Attrs = [ Dialog.Title "Edit Radius" ], Layout = vbox, OnClosed = DialogClosed)
    let vbox =
        BoxLayout(Items = [
            BoxItem.Create(undoRedoButtons)
            BoxItem.Create(canvas)
        ])
    LayoutWithDialogs(vbox, [ "edit", dialog ])
    :> ILayoutNode<Msg>
    
type CircleDrawer<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
