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

type TextFragment = {
    Location: Common.Point
    Text: string
}

type State = {
    MaybeDropPosition: Common.Point option
    Fragments: TextFragment list
}

type Msg =
    | DragMove of info: DragMoveInfo * canDrop: CanDropFunc
    | DragLeave
    | Drop of info: DropInfo

let init() =
    { MaybeDropPosition = None
      Fragments =  []
    }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | DragMove(info, canDrop) ->
        let nextState =
            if info.MimeTypes |> List.contains "text/plain" then
                canDrop (Some info.ProposedAction)
                { state with MaybeDropPosition = Some info.Position }
            else
                canDrop None
                { state with MaybeDropPosition = None }
        nextState, Cmd.None
    | DragLeave ->
        { state with MaybeDropPosition = None }, Cmd.None
    | Drop info ->
        let fragment = {
            Location = info.Position
            Text = info.MimeData.Text() 
        }
        let nextFragments =
            fragment :: state.Fragments
        { state with MaybeDropPosition = None; Fragments = nextFragments }, Cmd.None
        
let private orangeBrush = Brush(Color(1.0, 0.5, 0))
let private yellowPen = Pen(Color.Yellow)
let private font = Font("Helvetica", 10)

type DrawerPaintState(state: State) =
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
                Common.Rect(X = fragment.Location.X, Y = fragment.Location.Y, Width = 150, Height = 400)
            painter.DrawText(rect, Common.Alignment.Left, fragment.Text)
        // drop preview pos
        match state.MaybeDropPosition with
        | Some pos ->
            painter.Brush <- orangeBrush
            painter.DrawEllipse(pos, 20, 20)
        | None ->
            ()
            
let view (state: State) =
    let custom =
        CustomWidget(
            Attrs = [
                PaintState(DrawerPaintState(state))
                AcceptDrops true
                SizeHint (640, 480)
            ], OnDragMove = DragMove, OnDragLeave = DragLeave, OnDrop = Drop)
    BoxLayout(Items = [
        BoxItem.Create(custom)
    ]) :> ILayoutNode<Msg>
    
type DropTesting<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
