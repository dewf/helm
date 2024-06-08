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

let DRAG_THRESH_PIXELS = 5

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
    | EndDrag

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
    | EndDrag ->
        { state with PotentiallyDraggingFrom = None }, Cmd.None
        
let rectContains (r: Common.Rect) (p: Common.Point) =
    p.X >= r.X && p.X < (r.X + r.Width) && p.Y >= r.Y && p.Y < (r.Y + r.Height)
    
type EventDelegate(state: State) =
    inherit EventDelegateBase<Msg,State>(state)
    
    override this.SizeHint = Common.Size (640, 480)
    
    override this.MousePress loc button modifiers =
        if rectContains DRAG_SOURCE_RECT loc then
            Some (BeginPotentialDrag loc)
        else
            None
        
    override this.MouseMove loc buttons modifiers =
        // we don't have tracking enabled so move events will only be received when a button is held
        match state.PotentiallyDraggingFrom with
        | Some p ->
            if Util.dist loc p > DRAG_THRESH_PIXELS then
                printfn "beginning drag!"
                match this.BeginDrag (Text "WOOOOOOOOOOOOOOOOOOOOT") [Widget.DropAction.Copy; Widget.DropAction.Move] Widget.DropAction.Copy with
                | Widget.DropAction.Copy ->
                    printfn "data copied"
                | Widget.DropAction.Move ->
                    printfn "data moved"
                | _ ->
                    printfn "(some other outcome)"
                Some EndDrag
            else
                None
        | None ->
            None
        
    override this.MouseRelease loc button modifiers =
        state.MaybeDropPosition
        |> Option.map (fun _ -> EndDrag)
        
    override this.NeedsPaint prev =
        Everything
        
    override this.DoPaint stack painter widget =
        let darkBlue = stack.Color(DarkBlue)
        let orangeBrush = stack.Brush(stack.Color(1, 0.5, 0.5, 0.25))
        let yellowPen = stack.Pen(stack.Color(Yellow))
        let noPen = stack.Pen(NoPen)
        
        painter.FillRect(widget.Rect.QtValue, darkBlue)
        painter.Pen <- yellowPen
        // painter.Font <- font
        // drag source rect
        painter.DrawRect(DRAG_SOURCE_RECT)
        painter.DrawText(DRAG_SOURCE_RECT, Common.Alignment.Center, "Drag from Me")
        // existing fragments
        for fragment in state.Fragments do
            let rect =
                Common.Rect(fragment.Location.X, fragment.Location.Y, 1000, 1000)
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
            
    // drop stuff -------------------------------------------------
            
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
            EventDelegate(state), [ PaintEvent; DropEvents; SizeHint; MousePressEvent; MouseMoveEvent ],
            Attrs = [ AcceptDrops true ])
    BoxLayout(Items = [
        BoxItem.Create(custom)
    ]) :> ILayoutNode<Msg>
    
type DropTesting<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
