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
}

type Msg =
    | DropPreview of loc: Common.Point
    | PerformDrop of fragment: Fragment
    | DropCanceled

let init() =
    { MaybeDropPosition = None
      Fragments =  []
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
        
type DropDelegate(state: State) =
    inherit EventDelegate<Msg>()
    override this.SizeHint = Common.Size (640, 480)
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
        
let private orangeBrush = Brush(Color(1.0, 0.5, 0.5))
let private yellowPen = Pen(Color.Yellow)
let private font = Font("Helvetica", 10)
let private noPen = Pen(NoPen)

type MyPaintState(state: State) =
    inherit PaintStateBase<State, int>(state)
    override this.CreateResources() = 0 // we're just using 'int' as a placeholder since we're not using paintresources right now 

    // by default, without overriding .StateEquals, ANY change in our state from previous will trigger a redraw
    // .StateEquals gives you the ability to define a subset of state which is pertinent for repaints
        
    override this.DoPaint resources widget painter paintRect =
        painter.FillRect(widget.GetRect(), Color.DarkBlue)
        painter.Pen <- yellowPen
        painter.Font <- font
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
            
let view (state: State) =
    let custom =
        CustomWidget(
            DropDelegate(state), [ PaintEvent; DropEvents; SizeHint ],
            Attrs = [
                PaintState(MyPaintState(state))
                AcceptDrops true
            ])
    BoxLayout(Items = [
        BoxItem.Create(custom)
    ]) :> ILayoutNode<Msg>
    
type DropTesting<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
