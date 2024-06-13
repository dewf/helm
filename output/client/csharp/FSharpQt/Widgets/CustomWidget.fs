module FSharpQt.Widgets.CustomWidget

open System.Collections.Generic
open FSharpQt.BuilderNode
open System
open FSharpQt.MiscTypes
open FSharpQt.Painting
open Org.Whatever.QtTesting

// EventDelegate stuff =====================================================

type UpdateArea =
    | NotRequired
    | Everything
    | Rects of Rect list
    
type MouseButton =
    | LeftButton
    | RightButton
    | MiddleButton
    | OtherButton
with
    static member internal From (qtButton: Widget.MouseButton) =
        match qtButton with
        | Widget.MouseButton.None -> failwith "MouseButton.From - .None case - shoudln't happen"
        | Widget.MouseButton.Left -> LeftButton
        | Widget.MouseButton.Right -> RightButton
        | Widget.MouseButton.Middle -> MiddleButton
        | _ -> OtherButton // also handles other enum cases
    member internal this.QtValue =
        match this with
        | LeftButton -> Widget.MouseButton.Left
        | RightButton -> Widget.MouseButton.Right
        | MiddleButton -> Widget.MouseButton.Middle
        | OtherButton -> Widget.MouseButton.Other
    static member internal SetFrom (qtButtonSet: HashSet<Widget.MouseButton>) =
        (set qtButtonSet)
        |> Set.map MouseButton.From
    
type Modifier =
    | Shift
    | Control
    | Alt
    | Meta
with
    static member internal From (qtModifier: Widget.Modifier) =
        match qtModifier with
        | Widget.Modifier.Shift -> Shift
        | Widget.Modifier.Control -> Control
        | Widget.Modifier.Alt -> Alt
        | Widget.Modifier.Meta -> Meta
        | _ -> failwith "Modifier.From - unknown enum value (or .None, which shouldn't happen)"
    static member internal SetFrom (qtModifierSet: HashSet<Widget.Modifier>) =
        (set qtModifierSet)
        |> Set.map Modifier.From
    
type DropAction =
    | Ignore
    | Copy
    | Move
    | Link
with
    static member internal From (qtDropAction: Widget.DropAction) =
        match qtDropAction with
        | Widget.DropAction.Ignore -> Ignore
        | Widget.DropAction.Copy -> Copy
        | Widget.DropAction.Move -> Move
        | Widget.DropAction.Link -> Link
        | _ -> failwith "DropAction.From - unhandled DropAction case (only move/copy/link supported)"
    member internal this.QtValue =
        match this with
        | Ignore -> Widget.DropAction.Ignore
        | Copy -> Widget.DropAction.Copy
        | Move -> Widget.DropAction.Move
        | Link -> Widget.DropAction.Link
    static member internal SetFrom (qtDropActionSet: HashSet<Widget.DropAction>) =
        (set qtDropActionSet)
        |> Set.map DropAction.From
    
type MimeDataProxy internal(qMimeData: Widget.MimeData) =
    member val qMimeData = qMimeData
    member this.HasFormat(mimeType: string) =
        qMimeData.HasFormat(mimeType)
    member this.Text =
        qMimeData.Text()
    member this.Urls =
        qMimeData.Urls()
    
[<AbstractClass>]
type EventDelegateInterface<'msg>() = // obviously it's an abstract class and not a proper interface, but that's mainly because F# doesn't currently support default interface methods / Scala-style traits
    abstract member Widget: Widget.Handle with set

    abstract member SizeHint: Size
    default this.SizeHint = Size.Invalid // invalid size = no recommendation
    
    abstract member NeedsPaintInternal: EventDelegateInterface<'msg> -> UpdateArea
    
    abstract member CreateResourcesInternal: PaintStack -> unit
    abstract member MigrateResources: EventDelegateInterface<'msg> -> unit

    abstract member PaintInternal: PaintStack -> FSharpQt.Painting.Painter -> WidgetProxy -> Rect -> unit

    abstract member MousePress: Point -> MouseButton -> Set<Modifier> -> 'msg option
    default this.MousePress _ _ _ = None

    abstract member MouseMove: Point -> Set<MouseButton> -> Set<Modifier> -> 'msg option
    default this.MouseMove _ _ _ = None

    abstract member MouseRelease: Point -> MouseButton -> Set<Modifier> -> 'msg option
    default this.MouseRelease _ _ _ = None
    
    abstract member Enter: Point -> 'msg option
    default this.Enter _ = None
    
    abstract member Leave: unit -> 'msg option
    default this.Leave() = None
    
    abstract member Resize: Size -> Size -> 'msg option
    default this.Resize _ _ = None
    
    abstract member DragMove: Point -> Set<Modifier> -> MimeDataProxy -> DropAction -> Set<DropAction> -> bool -> (DropAction * 'msg) option
    default this.DragMove _ _ _ _ _ _ = None

    abstract member DragLeave: unit -> 'msg option
    default this.DragLeave () = None

    abstract member Drop: Point -> Set<Modifier> -> MimeDataProxy -> DropAction -> 'msg option
    default this.Drop _ _ _ _ = None
    
type DragPayload =
    | Text of text: string
    | Urls of urls: string list
    
[<AbstractClass>]
type AbstractEventDelegate<'msg,'state when 'state: equality>(state: 'state) =
    inherit EventDelegateInterface<'msg>()

    let mutable widget: Widget.Handle = null
    override this.Widget with set value = widget <- value

    member val private state = state
    
    override this.CreateResourcesInternal stack = ()
    override this.MigrateResources prev = ()

    abstract member NeedsPaint: 'state -> UpdateArea
    default this.NeedsPaint _ = NotRequired
    
    override this.NeedsPaintInternal prevDelegate =
        match prevDelegate with
        | :? AbstractEventDelegate<'msg,'state> as prev' ->
            this.NeedsPaint prev'.state
        | _ ->
            failwith "nope"
            
    member this.BeginDrag (payload: DragPayload) (supported: DropAction list) (defaultAction: DropAction) =
        let drag =
            Widget.CreateDrag(widget)
        let mimeData =
            Widget.CreateMimeData()
        match payload with
        | Text text ->
            mimeData.SetText(text)
        | Urls urls ->
            mimeData.SetUrls(urls |> Array.ofList)
        drag.SetMimeData(mimeData)
        drag.Exec(HashSet(supported |> List.map (_.QtValue)), defaultAction.QtValue)
        |> DropAction.From
        // we're not responsible for either the mimeData nor the drag, as long as the drag was created with an owner
        
[<AbstractClass>]
type EventDelegateBase<'msg,'state when 'state: equality>(state: 'state) =
    inherit AbstractEventDelegate<'msg,'state>(state)
    
    abstract member Paint: PaintStack -> FSharpQt.Painting.Painter -> WidgetProxy -> Rect -> unit
    default this.Paint _ _ _ _ = ()
    
    override this.PaintInternal stack painter widget updateRect =
        this.Paint stack painter widget updateRect
        
[<AbstractClass>]
type EventDelegateBaseWithResources<'msg,'state,'resources when 'state: equality>(state: 'state) =
    inherit AbstractEventDelegate<'msg,'state>(state)
    
    [<DefaultValue>] val mutable private resources: 'resources
    
    abstract member CreateResources: PaintStack -> 'resources
    // no default, must implement

    override this.CreateResourcesInternal stack =
        this.resources <- this.CreateResources stack
        
    override this.MigrateResources prev =
        match prev with
        | :? EventDelegateBaseWithResources<'msg,'state,'resources> as prev' ->
            this.resources <- prev'.resources
        | _ ->
            failwith "nope"

    abstract member Paint: 'resources -> PaintStack -> FSharpQt.Painting.Painter -> WidgetProxy -> Rect -> unit
    default this.Paint _ _ _ _ _ = ()
    
    override this.PaintInternal stack painter widget updateRect =
        this.Paint this.resources stack painter widget updateRect

// begin widget proper =================================================

type Signal = unit
    
type Attr =
    | UpdatesEnabled of enabled: bool
    | MouseTracking of enabled: bool
    | AcceptDrops of enabled: bool
    
let private attrKey = function
    | UpdatesEnabled _ -> 0
    | MouseTracking _ -> 1
    | AcceptDrops _ -> 2
    
let private diffAttrs =
    genericDiffAttrs attrKey

type Model<'msg>(dispatch: 'msg -> unit, methodMask: Widget.MethodMask, eventDelegate: EventDelegateInterface<'msg>) as self =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    
    let widget = Widget.CreateSubclassed(self, methodMask)
    
    // this is for anything the widget needs to create just once at the beginning
    // basically a roundabout way of maintaning auxiliary state just for the EventDelegate, which otherwise isn't supposed to have any (separate from the reactor State that's provided to it)
    let lifetimeResources = new PaintStack()
    
    let mutable eventDelegate = eventDelegate
    
    do
        eventDelegate.CreateResourcesInternal(lifetimeResources)
        
    interface Widget.MethodDelegate with
        override this.PaintEvent(painter: Painter.Handle, updateRect: Common.Rect) =
            use stackResources = new PaintStack() // "stack" (local), vs. the 'lifetimeResources' declared above
            eventDelegate.PaintInternal stackResources (Painter(painter)) (WidgetProxy(widget)) (Rect.From(updateRect))
            
        override this.MousePressEvent(pos: Common.Point, button: Widget.MouseButton, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MousePress (Point.From pos) (MouseButton.From button) (Modifier.SetFrom modifiers)
            |> Option.iter dispatch
            
        override this.MouseMoveEvent(pos: Common.Point, buttons: HashSet<Widget.MouseButton>, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MouseMove (Point.From pos) (MouseButton.SetFrom buttons) (Modifier.SetFrom modifiers)
            |> Option.iter dispatch
                
        override this.MouseReleaseEvent(pos: Common.Point, button: Widget.MouseButton, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MouseRelease (Point.From pos) (MouseButton.From button) (Modifier.SetFrom modifiers)
            |> Option.iter dispatch
                
        override this.EnterEvent(pos: Common.Point) =
            eventDelegate.Enter (Point.From pos)
            |> Option.iter dispatch
                
        override this.LeaveEvent() =
            eventDelegate.Leave()
            |> Option.iter dispatch
                
        override this.ResizeEvent(oldSize: Common.Size, newSize: Common.Size) =
            eventDelegate.Resize (Size.From oldSize) (Size.From newSize)
            |> Option.iter dispatch
            
        override this.SizeHint() =
            eventDelegate.SizeHint.QtValue
                
        override this.DragMoveEvent(pos: Common.Point, modifiers: HashSet<Widget.Modifier>, mimeData: Widget.MimeData, moveEvent: Widget.DragMoveEvent, isEnterEvent: bool) =
            match eventDelegate.DragMove (Point.From pos) (Modifier.SetFrom modifiers) (MimeDataProxy(mimeData)) (moveEvent.ProposedAction() |> DropAction.From) (moveEvent.PossibleActions() |> DropAction.SetFrom) isEnterEvent with
            | Some (dropAction, msg) ->
                moveEvent.AcceptDropAction(dropAction.QtValue)
                dispatch msg
            | None ->
                moveEvent.Ignore()
                
        override this.DragLeaveEvent() =
            eventDelegate.DragLeave()
            |> Option.iter dispatch
            
        override this.DropEvent(pos: Common.Point, modifiers: HashSet<Widget.Modifier>, mimeData: Widget.MimeData, dropAction: Widget.DropAction) =
            eventDelegate.Drop (Point.From pos) (Modifier.SetFrom modifiers) (MimeDataProxy(mimeData)) (DropAction.From dropAction)
            |> Option.iter dispatch
            
        // override this.Dispose() =
        //     // I forget why the generated method delegates have this ...
        //     ()

    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    
    member this.EventDelegate with set (newDelegate: EventDelegateInterface<'msg>) =
        // for now just the widget, maybe 'this' (the entire Model) in the future?
        newDelegate.Widget <- widget
        
        // check if it needs painting (by comparing to previous - for now the implementer will have to extract the previous state themselves, but we'll get to it
        match newDelegate.NeedsPaintInternal(eventDelegate) with
        | NotRequired ->
            ()
        | Everything ->
            widget.Update()
        | Rects rects ->
            for rect in rects do
                widget.Update(rect.QtValue)
                
        // migrate paint resources from previous
        newDelegate.MigrateResources eventDelegate
                
        // update/overwrite value
        eventDelegate <- newDelegate
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | AcceptDrops enabled ->
                widget.SetAcceptDrops(enabled)
            | UpdatesEnabled enabled ->
                widget.SetUpdatesEnabled(enabled)
            | MouseTracking enabled ->
                widget.SetMouseTracking(enabled)
                
    interface IDisposable with
        member this.Dispose() =
            (lifetimeResources :> IDisposable).Dispose()
            widget.Dispose()

let rec private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (methodMask: Widget.MethodMask) (eventDelegate: EventDelegateInterface<'msg>) =
    let model = new Model<'msg>(dispatch, methodMask, eventDelegate)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    // can't assign eventDelegate as simply as signal map, requires different behavior on construction vs. migration
    // hence providing it as Model ctor argument
    // model.EventDelegate <- eventDelegate
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (eventDelegate: EventDelegateInterface<'msg>) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.EventDelegate <- eventDelegate
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type EventMaskItem =
    | MousePressEvent
    | MouseMoveEvent
    | MouseReleaseEvent
    | EnterEvent
    | LeaveEvent
    | PaintEvent
    | SizeHint
    | ResizeEvent
    | DropEvents

type CustomWidget<'msg>(eventDelegate: EventDelegateInterface<'msg>, eventMaskItems: EventMaskItem list) =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = (fun _ -> None) // nothing yet
    
    member private this.MethodMask =
        (enum<Widget.MethodMask> 0, eventMaskItems)
        ||> List.fold (fun acc item ->
            let value =
                match item with
                | MousePressEvent -> Widget.MethodMask.MousePressEvent
                | MouseMoveEvent -> Widget.MethodMask.MouseMoveEvent
                | MouseReleaseEvent -> Widget.MethodMask.MouseReleaseEvent
                | EnterEvent -> Widget.MethodMask.EnterEvent
                | LeaveEvent -> Widget.MethodMask.LeaveEvent
                | PaintEvent -> Widget.MethodMask.PaintEvent
                | SizeHint -> Widget.MethodMask.SizeHint
                | ResizeEvent -> Widget.MethodMask.ResizeEvent
                | DropEvents -> Widget.MethodMask.DropEvents
            acc ||| value)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
            
        override this.Create2 dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch this.MethodMask eventDelegate
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> CustomWidget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap eventDelegate
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
