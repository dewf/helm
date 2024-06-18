module FSharpQt.EventDelegate

open System.Collections.Generic
open Org.Whatever.QtTesting

open FSharpQt
open InputEnums
open MiscTypes
open Painting

type UpdateArea =
    | NotRequired
    | Everything
    | Rects of Rect list
    
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
    
[<AbstractClass>]
type EventDelegateInterface<'msg>() = // obviously it's an abstract class and not a proper interface, but that's mainly because F# doesn't currently support default interface methods / Scala-style traits
    abstract member Widget: Widget.Handle with set

    abstract member SizeHint: Size
    default this.SizeHint = Size.Invalid // invalid size = no recommendation
    
    abstract member NeedsPaintInternal: EventDelegateInterface<'msg> -> UpdateArea
    
    abstract member CreateResourcesInternal: PaintStack -> unit
    abstract member MigrateResources: EventDelegateInterface<'msg> -> unit

    abstract member PaintInternal: PaintStack -> Painter -> WidgetProxy -> Rect -> unit

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

