module FSharpQt.Widgets.CustomWidget

open System.Collections.Generic
open FSharpQt.BuilderNode
open System
open FSharpQt.Painting
open Org.Whatever.QtTesting

// EventDelegate stuff =====================================================

type UpdateArea =
    | NotRequired
    | Everything
    | Rects of Common.Rect list
    
[<AbstractClass>]
type EventDelegate<'msg>() =
    abstract member Widget: Widget.Handle with set
    abstract member SizeHint: Common.Size

    default this.SizeHint = Common.Size(-1, -1) // invalid size = no recommendation
    abstract member NeedsPaintInternal: EventDelegate<'msg> -> UpdateArea

    abstract member DoPaint: PaintStack -> Widget.Handle -> FSharpQt.Painting.Painter -> Common.Rect -> unit
    default this.DoPaint _ _ _ _ = ()

    abstract member MousePress: Common.Point -> Widget.MouseButton -> Set<Widget.Modifier> -> 'msg option
    default this.MousePress _ _ _ = None

    abstract member MouseMove: Common.Point -> Set<Widget.MouseButton> -> Set<Widget.Modifier> -> 'msg option
    default this.MouseMove _ _ _ = None

    abstract member MouseRelease: Common.Point -> Widget.MouseButton -> Set<Widget.Modifier> -> 'msg option
    default this.MouseRelease _ _ _ = None
    
    abstract member Enter: Common.Point -> 'msg option
    default this.Enter _ = None
    
    abstract member Leave: unit -> 'msg option
    default this.Leave() = None
    
    abstract member Resize: Common.Size -> Common.Size -> 'msg option
    default this.Resize _ _ = None
    
    abstract member DragMove: Common.Point -> Set<Widget.Modifier> -> Widget.MimeData -> Widget.DropAction -> Set<Widget.DropAction> -> bool -> (Widget.DropAction * 'msg) option
    default this.DragMove _ _ _ _ _ _ = None

    abstract member DragLeave: unit -> 'msg option
    default this.DragLeave () = None

    abstract member Drop: Common.Point -> Set<Widget.Modifier> -> Widget.MimeData -> Widget.DropAction -> 'msg option
    default this.Drop _ _ _ _ = None
    
type DragPayload =
    | Text of text: string
    | Urls of urls: string list

[<AbstractClass>]
type EventDelegateBase<'msg,'state when 'state: equality>(state: 'state) =
    inherit EventDelegate<'msg>()
    
    let mutable widget: Widget.Handle = null
    member val private state = state
    
    override this.Widget with set value = widget <- value

    abstract member NeedsPaint: 'state -> UpdateArea
    default this.NeedsPaint _ = NotRequired
    
    override this.NeedsPaintInternal prevDelegate =
        match prevDelegate with
        | :? EventDelegateBase<'msg,'state> as prev' ->
            this.NeedsPaint prev'.state
        | _ ->
            failwith "nope"
            
    member this.BeginDrag (payload: DragPayload) (supported: Widget.DropAction list) (defaultAction: Widget.DropAction) =
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
        drag.Exec(HashSet(supported), defaultAction)
        // we're not responsible for either the mimeData nor the drag, as long as the drag was created with an owner
            

// begin widget proper =================================================

type Signal = unit
    
type Attr =
    // | PaintState of ps: PaintState
    | UpdatesEnabled of enabled: bool
    | MouseTracking of enabled: bool
    | AcceptDrops of enabled: bool
    
let private attrKey = function
    // | PaintState _ -> 0
    | UpdatesEnabled _ -> 1
    | MouseTracking _ -> 2
    | AcceptDrops _ -> 3
    
let private diffAttrs =
    genericDiffAttrs attrKey

type Model<'msg>(dispatch: 'msg -> unit, methodMask: Widget.MethodMask, eventDelegate: EventDelegate<'msg>) as self =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    
    let widget = Widget.CreateSubclassed(self, methodMask)
    
    let signalDispatch s =
        signalMap s
        |> Option.iter dispatch
        
    let mutable eventDelegate = eventDelegate
        
    interface Widget.MethodDelegate with
        override this.PaintEvent(painter: Painter.Handle, rect: Common.Rect) =
            use stack = new PaintStack()
            eventDelegate.DoPaint stack widget (Painter(painter)) rect
            
        override this.MousePressEvent(pos: Common.Point, button: Widget.MouseButton, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MousePress pos button (set modifiers)
            |> Option.iter dispatch
            
        override this.MouseMoveEvent(pos: Common.Point, buttons: HashSet<Widget.MouseButton>, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MouseMove pos (set buttons) (set modifiers)
            |> Option.iter dispatch
                
        override this.MouseReleaseEvent(pos: Common.Point, button: Widget.MouseButton, modifiers: HashSet<Widget.Modifier>) =
            eventDelegate.MouseRelease pos button (set modifiers)
            |> Option.iter dispatch
                
        override this.EnterEvent(pos: Common.Point) =
            eventDelegate.Enter pos
            |> Option.iter dispatch
                
        override this.LeaveEvent() =
            eventDelegate.Leave()
            |> Option.iter dispatch
                
        override this.ResizeEvent(oldSize: Common.Size, newSize: Common.Size) =
            eventDelegate.Resize oldSize newSize
            |> Option.iter dispatch
            
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
            eventDelegate.DragLeave()
            |> Option.iter dispatch
            
        override this.DropEvent(pos: Common.Point, modifiers: HashSet<Widget.Modifier>, mimeData: Widget.MimeData, dropAction: Widget.DropAction) =
            eventDelegate.Drop pos (set modifiers) mimeData dropAction
            |> Option.iter dispatch
            
        // override this.Dispose() =
        //     // I forget why the generated method delegates have this ...
        //     ()

    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    
    member this.EventDelegate with set (newDelegate: EventDelegate<'msg>) =
        // for now just the widget, maybe 'this' (the Model) in the future?
        newDelegate.Widget <- widget
        
        // check if it needs painting (by comparing to previous - for now the implementer will have to extract the previous state themselves, but we'll get to it
        match newDelegate.NeedsPaintInternal(eventDelegate) with
        | NotRequired ->
            ()
        | Everything ->
            widget.Update()
        | Rects rects ->
            for rect in rects do
                widget.Update(rect)
        eventDelegate <- newDelegate
        // if we ever decide to add back in the 'paintresources', we will need to migrate it here (and create them in the ctor/do block of the model)
    
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
            // maybePaintState
            // |> Option.iter (_.DestroyResourcesInternal())
            widget.Dispose()

let rec private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (methodMask: Widget.MethodMask) (eventDelegate: EventDelegate<'msg>) =
    let model = new Model<'msg>(dispatch, methodMask, eventDelegate)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    // model.EventDelegate <- eventDelegate
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
    | MouseReleaseEvent
    | EnterEvent
    | LeaveEvent
    | PaintEvent
    | SizeHint
    | ResizeEvent
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
                | MouseReleaseEvent -> Widget.MethodMask.MouseReleaseEvent
                | EnterEvent -> Widget.MethodMask.EnterEvent
                | LeaveEvent -> Widget.MethodMask.LeaveEvent
                | PaintEvent -> Widget.MethodMask.PaintEvent
                | SizeHint -> Widget.MethodMask.SizeHint
                | ResizeEvent -> Widget.MethodMask.ResizeEvent
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
