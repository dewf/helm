module FSharpQt.Widgets.CustomWidget

open System

open FSharpQt.BuilderNode
open FSharpQt.InputEnums
open FSharpQt.Painting
open Org.Whatever.QtTesting
open FSharpQt.EventDelegate

open FSharpQt.MiscTypes
open FSharpQt.Attrs
open Widget

type private Model<'msg>(dispatch: 'msg -> unit, methodMask: Widget.MethodMask, eventDelegate: EventDelegateInterface<'msg>) as this =
    let widget = Widget.CreateSubclassed(this, methodMask, this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    
    let mutable currentMask = enum<Widget.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    // this is for anything the widget needs to create just once at the beginning
    // basically a roundabout way of maintaning auxiliary state just for the EventDelegate, which otherwise isn't supposed to have any (separate from the reactor State that's provided to it)
    let lifetimeResources = new PaintStack()
    let mutable eventDelegate = eventDelegate
    do
        eventDelegate.CreateResourcesInternal(lifetimeResources)
            
    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            widget.SetSignalMask(value)
            currentMask <- value
            
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
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface WidgetAttrTarget with
        override this.Widget = widget
                
    interface Widget.SignalHandler with
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> CustomContextMenuRequested)
        member this.WindowIconChanged icon =
            signalDispatch (IconProxy(icon) |> WindowIconChanged)
        member this.WindowTitleChanged title =
            signalDispatch (WindowTitleChanged title)
        
    interface Widget.MethodDelegate with
        override this.PaintEvent(painter: Painter.Handle, updateRect: Common.Rect) =
            use stackResources = new PaintStack() // "stack" (local), vs. the 'lifetimeResources' declared above
            eventDelegate.PaintInternal stackResources (Painter(painter)) (WidgetProxy(widget)) (Rect.From(updateRect))
            
        override this.MousePressEvent(pos: Common.Point, button: Enums.MouseButton, modifiers: Enums.Modifiers) =
            eventDelegate.MousePress (Point.From pos) (MouseButton.From button) (Modifier.SetFrom modifiers)
            |> Option.iter dispatch
            
        override this.MouseMoveEvent(pos: Common.Point, buttons: Enums.MouseButtonSet, modifiers: Enums.Modifiers) =
            eventDelegate.MouseMove (Point.From pos) (MouseButton.SetFrom buttons) (Modifier.SetFrom modifiers)
            |> Option.iter dispatch
                
        override this.MouseReleaseEvent(pos: Common.Point, button: Enums.MouseButton, modifiers: Enums.Modifiers) =
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
                
        override this.DragMoveEvent(pos: Common.Point, modifiers: Enums.Modifiers, mimeData: Widget.MimeData, moveEvent: Widget.DragMoveEvent, isEnterEvent: bool) =
            match eventDelegate.DragMove (Point.From pos) (Modifier.SetFrom modifiers) (MimeDataProxy(mimeData)) (moveEvent.ProposedAction() |> DropAction.From) (moveEvent.PossibleActions() |> DropAction.SetFrom) isEnterEvent with
            | Some (dropAction, msg) ->
                moveEvent.AcceptDropAction(dropAction.QtValue)
                dispatch msg
            | None ->
                moveEvent.Ignore()
                
        override this.DragLeaveEvent() =
            eventDelegate.DragLeave()
            |> Option.iter dispatch
            
        override this.DropEvent(pos: Common.Point, modifiers: Enums.Modifiers, mimeData: Widget.MimeData, dropAction: Widget.DropAction) =
            eventDelegate.Drop (Point.From pos) (Modifier.SetFrom modifiers) (MimeDataProxy(mimeData)) (DropAction.From dropAction)
            |> Option.iter dispatch
            
    interface IDisposable with
        member this.Dispose() =
            (lifetimeResources :> IDisposable).Dispose()
            widget.Dispose()

let rec private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (methodMask: Widget.MethodMask) (eventDelegate: EventDelegateInterface<'msg>) (signalMask: Widget.SignalMask) =
    let model = new Model<'msg>(dispatch, methodMask, eventDelegate)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    // can't assign eventDelegate as simply as signal map, requires different behavior on construction vs. migration
    // hence providing it as Model ctor argument
    // model.EventDelegate <- eventDelegate
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (eventDelegate: EventDelegateInterface<'msg>) (signalMask: Widget.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.EventDelegate <- eventDelegate
    model.SignalMask <- signalMask
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
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
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
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.MethodMask eventDelegate this.SignalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> CustomWidget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap eventDelegate this.SignalMask
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
