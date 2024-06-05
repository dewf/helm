module Tabs.DropTesting

open FSharpQt.BuilderNode
open FSharpQt.Painting
open FSharpQt.Reactor
open FSharpQt.Widgets
open BoxLayout
open CustomWidget

open Org.Whatever.QtTesting

type Signal = unit
type Attr = unit

let DRAG_SOURCE_RECT =
    Common.Rect(20, 20, 100, 100)

[<RequireQualifiedAccess>]
type Payload =
    | Text of string
    | Files of string list
    
type Fragment = {
    Location: Common.Point
    Payload: Payload
}

type State = {
    MaybeDropPosition: Common.Point option
    Fragments: Fragment list
    PotentiallyDraggingFrom: Common.Point option
}

type Msg =
    | DropPreview of loc: Common.Point
    | PerformDrop of fragment: Fragment
    | DropCanceled
    | BeginPotentialDrag of loc: Common.Point
    | EndPotentialDrag

let init() =
    { MaybeDropPosition = None
      Fragments =  []
      PotentiallyDraggingFrom = None
    }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | DropPreview loc ->
        { state with MaybeDropPosition = Some loc }, Cmd.None
    | PerformDrop fragment ->
        let nextFragments =
            fragment :: state.Fragments
        { state with Fragments = nextFragments; MaybeDropPosition = None }, Cmd.None
    | DropCanceled ->
        { state with MaybeDropPosition = None }, Cmd.None
    | BeginPotentialDrag loc ->
        { state with PotentiallyDraggingFrom = Some loc }, Cmd.None
    | EndPotentialDrag ->
        { state with PotentiallyDraggingFrom = None }, Cmd.None
        
let private orangeBrush = Brush(Color(1.0, 0.5, 0.5, 0.25))
let private yellowPen = Pen(Color.Yellow)
// let private font = Font("Helvetica", 10)
let private noPen = Pen(NoPen)

let rectContains (r: Common.Rect) (p: Common.Point) =
    p.X >= r.X && p.X < (r.X + r.Width) && p.Y >= r.Y && p.Y < (r.Y + r.Height)
    
let dist (p1: Common.Point) (p2: Common.Point) =
    let dx = p1.X - p2.X
    let dy = p1.Y - p2.Y
    (dx * dx + dy * dy) |> float |> sqrt

type DropDelegate(state: State) =
    inherit EventDelegateBase<Msg,State>(state)
    override this.SizeHint = Common.Size (640, 480)
    
    override this.MousePress loc button modifiers =
        if rectContains DRAG_SOURCE_RECT loc then
            Some (BeginPotentialDrag loc)
        else
            None
        
    override this.MouseMove loc buttons modifiers =
        match state.PotentiallyDraggingFrom with
        | Some p ->
            if dist loc p > 5 then
                printfn "beginning drag!"
                match this.BeginDrag (Text "WOOOOOOOOOOOOOOOOOOOOT") [Widget.DropAction.Copy; Widget.DropAction.Move] Widget.DropAction.Copy with
                | Widget.DropAction.Copy ->
                    printfn "data copied"
                | Widget.DropAction.Move ->
                    printfn "data moved"
                | _ ->
                    printfn "(some other outcome)"
                Some EndPotentialDrag
            else
                None
        | None ->
            None
        
    override this.MouseRelease loc button modifiers =
        Some EndPotentialDrag
        
    override this.NeedsPaint prev =
        Everything
        
    override this.DoPaint widget painter paintRect =
        painter.FillRect(widget.GetRect(), Color.DarkBlue)
        painter.Pen <- yellowPen
        // painter.Font <- font
        // drag source rect
        painter.DrawRect(DRAG_SOURCE_RECT)
        painter.DrawText(DRAG_SOURCE_RECT, Common.Alignment.Center, "Drag from Me")
        // existing fragments
        for fragment in state.Fragments do
            let rect =
                Common.Rect(X = fragment.Location.X, Y = fragment.Location.Y, Width = 1000, Height = 1000)
            match fragment.Payload with
            | Payload.Text text ->
                painter.DrawText(rect, Common.Alignment.Left, text)
            | Payload.Files files ->
                let text =
                    files
                    |> String.concat "\n"
                painter.DrawText(rect, Common.Alignment.Leading, text)
        // preview pos
        match state.MaybeDropPosition with
        | Some pos ->
            painter.Pen <- noPen
            painter.Brush <- orangeBrush
            painter.DrawEllipse(pos, 20, 20)
        | None ->
            ()
            
    override this.DragMove loc modifiers mimeData proposedAction possibleActions isEnterEvent =
        if mimeData.HasFormat("text/plain") && possibleActions.Contains(Widget.DropAction.Copy) then
            Some (Widget.DropAction.Copy, DropPreview loc)
        elif mimeData.HasFormat("text/uri-list") && possibleActions.Contains(Widget.DropAction.Copy) then
            Some (Widget.DropAction.Copy, DropPreview loc)
        else
            None
            
    override this.DragLeave() =
        Some DropCanceled
        
    override this.Drop loc modifiers mimeData dropAction =
        let payload =
            if mimeData.HasFormat("text/plain") then
                mimeData.Text() |> Payload.Text 
            else
                mimeData.Urls() |> Array.toList |> Payload.Files
        let fragment =
            { Location = loc; Payload = payload }
        Some (PerformDrop fragment)
            
let view (state: State) =
    let custom =
        CustomWidget(
            DropDelegate(state), [ PaintEvent; DropEvents; SizeHint; MousePressEvent; MouseMoveEvent ],
            Attrs = [ AcceptDrops true ])
    BoxLayout(Items = [
        BoxItem.Create(custom)
    ]) :> ILayoutNode<Msg>
    
type DropTesting<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
