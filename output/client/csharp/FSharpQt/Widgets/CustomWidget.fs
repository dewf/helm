module FSharpQt.Widgets.CustomWidget

open System.Collections.Generic
open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

// PaintState stuff -- required for custom drawing ====================
type UpdateArea =
    | NotRequired
    | Everything
    | Rects of Common.Rect list
    
[<AbstractClass>]
type PaintState() =
    // implementing all this is a lot of annoying boilerplate,
    // ... so use PaintStateBase below instead.
    abstract member DoPaintInternal: Widget.Handle -> Painter.Handle -> Common.Rect -> unit
    abstract member CreateResourcesInternal: unit -> unit                           // only called once
    abstract member MigrateResources: PaintState -> unit
    abstract member DestroyResourcesInternal: unit -> unit                          // called once on disposal
    abstract member ComputeAreaInternal: PaintState -> UpdateArea
    abstract member InternalEquals: PaintState -> bool
    abstract member InternalHashCode: int
    override this.Equals(other: Object) =
        match other with
        | :? PaintState as otherPS -> this.InternalEquals(otherPS)
        | _ -> false
    override this.GetHashCode() =
        this.InternalHashCode
        
[<AbstractClass>]
type PaintStateBase<'state, 'resources when 'state: equality>(state: 'state) =
    inherit PaintState()
    [<DefaultValue>] val mutable private resources: 'resources
    member val state = state
    abstract member CreateResources: unit -> 'resources
    abstract member DestroyResources: 'resources -> unit
    default this.DestroyResources(resources: 'resources) = ()
    abstract member DoPaint: 'resources -> Widget.Handle -> FSharpQt.Painting.Painter -> Common.Rect -> unit
    override this.DoPaintInternal widget qtPainter rect =
        let painter =
            FSharpQt.Painting.Painter(qtPainter)
        this.DoPaint this.resources widget painter rect
    override this.CreateResourcesInternal() =
        this.resources <- this.CreateResources()
    override this.MigrateResources prev =
        let prev' =
            (prev :?> PaintStateBase<'state,'resources>)
        this.resources <- prev'.resources
    override this.DestroyResourcesInternal() =
        this.DestroyResources(this.resources)
    abstract member ComputeUpdateArea: 'state -> UpdateArea // optional
    default this.ComputeUpdateArea _ =
        Everything
    override this.ComputeAreaInternal prevRaw =
        let prev = prevRaw :?> PaintStateBase<'state,'resources>
        this.ComputeUpdateArea prev.state
    abstract member StateEquals: 'state -> bool             // optional
    default this.StateEquals otherState =
        state = otherState
    override this.InternalEquals (other: PaintState) =
        let other' = other :?> PaintStateBase<'state,'resources>
        this.StateEquals other'.state
    override this.InternalHashCode =
        state.GetHashCode()
        
// EventDelegate stuff =====================================================

[<AbstractClass>]
type EventDelegate<'msg>() =
    abstract member SizeHint: Common.Size
    default this.SizeHint = Common.Size(-1, -1) // invalid size = no recommendation
    abstract member MousePress: Common.Point -> Widget.MouseButton -> Set<Widget.Modifier> -> 'msg option
    default this.MousePress _ _ _ = None
    abstract member MouseMove: Common.Point -> Set<Widget.MouseButton> -> Set<Widget.Modifier> -> 'msg option
    default this.MouseMove _ _ _ = None
    abstract member DragMove: Common.Point -> Set<Widget.Modifier> -> Widget.MimeData -> Widget.DropAction -> Set<Widget.DropAction> -> bool -> (Widget.DropAction * 'msg) option
    default this.DragMove _ _ _ _ _ _ = None
    abstract member DragLeave: unit -> 'msg option
    default this.DragLeave () = None
    abstract member Drop: Common.Point -> Set<Widget.Modifier> -> Widget.MimeData -> Widget.DropAction -> 'msg option
    default this.Drop _ _ _ _ = None

// begin widget proper =================================================

type Signal = unit
    
type Attr =
    | PaintState of ps: PaintState
    | UpdatesEnabled of enabled: bool
    | MouseTracking of enabled: bool
    | AcceptDrops of enabled: bool
    
let private attrKey = function
    | PaintState _ -> 0
    | UpdatesEnabled _ -> 1
    | MouseTracking _ -> 2
    | AcceptDrops _ -> 3
    
let private diffAttrs =
    genericDiffAttrs attrKey

type Model<'msg>(dispatch: 'msg -> unit, methodMask: Widget.MethodMask) as self =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    
    let widget = Widget.CreateSubclassed(self, methodMask)
    
    let signalDispatch s =
        signalMap s
        |> Option.iter dispatch
        
    let mutable maybePaintState: PaintState option = None
    let mutable eventDelegate = Unchecked.defaultof<EventDelegate<'msg>> // will always be set
        
    interface Widget.MethodDelegate with
        override this.PaintEvent(painter: Painter.Handle, rect: Common.Rect) =
            maybePaintState
            |> Option.iter (fun ps -> ps.DoPaintInternal widget painter rect)
            
        override this.MousePressEvent(pos: Common.Point, button: Widget.MouseButton, modifiers: HashSet<Widget.Modifier>) =
            match eventDelegate.MousePress pos button (set modifiers) with
            | Some msg ->
                dispatch msg
            | None ->
                ()
            
        override this.MouseMoveEvent(pos: Common.Point, buttons: HashSet<Widget.MouseButton>, modifiers: HashSet<Widget.Modifier>) =
            match eventDelegate.MouseMove pos (set buttons) (set modifiers) with
            | Some msg ->
                dispatch msg
            | None ->
                ()
            
        override this.SizeHint() =
            eventDelegate.SizeHint
                
        override this.DragMoveEvent(pos: Common.Point, modifiers: HashSet<Widget.Modifier>, mimeData: Widget.MimeData, moveEvent: Widget.DragMoveEvent, isEnterEvent: bool) =
            match eventDelegate.DragMove pos (set modifiers) mimeData (moveEvent.ProposedAction()) (moveEvent.PossibleActions() |> set) isEnterEvent with
            | Some (dropAction, msg) ->
                moveEvent.AcceptDropAction(dropAction)
                dispatch msg
            | None ->
                moveEvent.Ignore()
                
        override this.DragLeaveEvent() =
            match eventDelegate.DragLeave() with
            | Some msg ->
                dispatch msg
            | None ->
                ()
            
        override this.DropEvent(pos: Common.Point, modifiers: HashSet<Widget.Modifier>, mimeData: Widget.MimeData, dropAction: Widget.DropAction) =
            match eventDelegate.Drop pos (set modifiers) mimeData dropAction with
            | Some msg ->
                dispatch msg
            | None ->
                ()
            
        // override this.Dispose() =
        //     // I forget why the generated method delegates have this ...
        //     ()

    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    member this.EventDelegate with set value = eventDelegate <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | PaintState ps ->
                // raw paintstate changed, but do we actually need a redraw?
                // the provided PaintState can perform more precise checking
                let updateArea =
                    match maybePaintState with
                    | Some last ->
                        // in addition to binding 'updateArea', create/migrate resources while we're at it
                        ps.MigrateResources(last)
                        ps.ComputeAreaInternal last
                    | None ->
                        ps.CreateResourcesInternal()
                        // first incoming paintstate, must draw!
                        Everything
                // assign now that we've checked
                maybePaintState <- Some ps
                // then perform any invalidations requested
                match updateArea with
                | NotRequired ->
                    ()
                | Everything ->
                    widget.Update()
                | Rects rects ->
                    for rect in rects do
                        widget.Update(rect)
            | AcceptDrops enabled ->
                widget.SetAcceptDrops(enabled)
            | UpdatesEnabled enabled ->
                widget.SetUpdatesEnabled(enabled)
            | MouseTracking enabled ->
                widget.SetMouseTracking(enabled)
                
    interface IDisposable with
        member this.Dispose() =
            maybePaintState
            |> Option.iter (_.DestroyResourcesInternal())
            widget.Dispose()

let rec private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (methodMask: Widget.MethodMask) (eventDelegate: EventDelegate<'msg>) =
    let model = new Model<'msg>(dispatch, methodMask)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.EventDelegate <- eventDelegate
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (eventDelegate: EventDelegate<'msg>) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.EventDelegate <- eventDelegate
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type EventMaskItem =
    | MousePressEvent
    | MouseMoveEvent
    | PaintEvent
    | SizeHint
    | DropEvents

type CustomWidget<'msg>(eventDelegate: EventDelegate<'msg>, eventMaskItems: EventMaskItem list) =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable menus: (string * IMenuNode<'msg>) list = []
    member this.Menus with set value = menus <- value
    
    member private this.SignalMap = (fun _ -> None) // nothing yet
    
    member private this.MethodMask =
        (Widget.MethodMask.None, eventMaskItems)
        ||> List.fold (fun acc item ->
            let value =
                match item with
                | MousePressEvent -> Widget.MethodMask.MousePressEvent
                | MouseMoveEvent -> Widget.MethodMask.MouseMoveEvent
                | PaintEvent -> Widget.MethodMask.PaintEvent
                | SizeHint -> Widget.MethodMask.SizeHint
                | DropEvents -> Widget.MethodMask.DropEvents
            acc ||| value)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            menus
            |> List.map (fun (id, menu) -> StrKey ("menu_"+id), menu :> IBuilderNode<'msg>)
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch this.MethodMask eventDelegate
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> CustomWidget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap eventDelegate
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Widget =
            this.model.Widget
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
        override this.AttachedToWindow window =
            ()
            
    interface IPopupMenuParent<'msg> with
        override this.RelativeToWidget = this.model.Widget
        override this.AttachedPopups = menus
