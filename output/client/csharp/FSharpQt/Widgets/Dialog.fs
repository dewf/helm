module FSharpQt.Widgets.Dialog

open System
open FSharpQt.BuilderNode
open FSharpQt.Reactor
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.MiscTypes

type Signal =
    // inherited
    | WidgetSignal of signal: Widget.Signal
    // ours
    | Accepted
    | Finished of result: int
    | Rejected

type private Attr =
    | Modal of state: bool
    | SizeGripEnabled of enabled: bool
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as otherAttr ->
                this = otherAttr
            | _ ->
                false
        override this.Key =
            match this with
            | Modal _ -> "dialog:modal"
            | SizeGripEnabled _ -> "dialog:sizegripenabled"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? DialogAttrTarget as attrTarget ->
                let dialog =
                    attrTarget.Dialog
                match this with
                | Modal state ->
                    dialog.SetModal(state)
                | SizeGripEnabled enabled ->
                    dialog.SetSizeGripEnabled(enabled)
            | _ ->
                printfn "warning: PushButton.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    let mutable onAccepted: 'msg option = None
    let mutable onFinished: (int -> 'msg) option = None
    let mutable onRejected: 'msg option = None
    
    member this.SignalMask = enum<Dialog.SignalMask> (int this._signalMask)
        
    member this.OnAccepted with set value =
        onAccepted <- Some value
        this.AddSignal(int Dialog.SignalMask.Accepted)
    
    member this.OnFinished with set value =
        onFinished <- Some value
        this.AddSignal(int Dialog.SignalMask.Finished)
    
    member this.OnRejected with set value =
        onRejected <- Some value
        this.AddSignal(int Dialog.SignalMask.Rejected)

    member this.SignalMap = function
        | WidgetSignal signal ->
            (this :> Widget.Props<'msg>).SignalMap signal
        | Accepted ->
            onAccepted
        | Finished result ->
            onFinished
            |> Option.map (fun f -> f result)
        | Rejected ->
            onRejected
    
    member this.Modal with set value =
        this.PushAttr(Modal value)
        
    member this.SizeGripEnabled with set value =
        this.PushAttr(SizeGripEnabled value)
    
type private Model<'msg>(dispatch: 'msg -> unit, maybeParent: Widget.Handle option) as this =
    let mutable dialog =
        let parentHandle =
            maybeParent
            |> Option.defaultValue null
        Dialog.Create(parentHandle, this)
    
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Dialog.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
    
    member this.Dialog with get() = dialog
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            dialog.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs (attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface DialogAttrTarget with
        member this.Widget = dialog
        member this.Dialog = dialog
        
    interface Dialog.SignalHandler with
        // Widget:  
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> Widget.CustomContextMenuRequested |> WidgetSignal)
        member this.WindowIconChanged icon =
            signalDispatch (IconProxy(icon) |> Widget.WindowIconChanged |> WidgetSignal)
        member this.WindowTitleChanged title =
            signalDispatch (Widget.WindowTitleChanged title |> WidgetSignal)
        // Dialog:
        member this.Accepted() =
            signalDispatch Accepted
        member this.Finished result =
            // I think this is from .done(result) method which we don't currently use
            signalDispatch (Finished result)
        member this.Rejected() =
            signalDispatch Rejected
    
    interface IDisposable with
        member this.Dispose() =
            dialog.Dispose()
    
    member this.RemoveLayout() =
        let existing =
            dialog.GetLayout()
        existing.RemoveAll()
        dialog.SetLayout(null)
        
    member this.AddLayout (layout: Layout.Handle) =
        dialog.SetLayout(layout)

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialMask: Dialog.SignalMask) (maybeParent: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeParent)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- initialMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: Dialog.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Dialog<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>

    member val Attachments: (string * Attachment<'msg>) list = [] with get, set

    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member private this.MaybeLayout = maybeLayout
    member this.Layout with set value = maybeLayout <- Some value

    member private this.MigrateContent (chamgeMap: Map<DepsKey, DepsChange>) =
        match chamgeMap.TryFind (StrKey "layout") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddLayout(maybeLayout.Value.Layout)
            | Removed ->
                this.model.RemoveLayout()
            | Swapped ->
                this.model.RemoveLayout()
                this.model.AddLayout(maybeLayout.Value.Layout)
        | None ->
            // neither side had layout
            ()
        // layout only - parent can't be changed after creation
    
    interface IDialogNode<'msg> with
        override this.Dependencies =
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask buildContext.ContainingWindow
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun layout -> this.model.AddLayout layout.Layout)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Dialog<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask
            this.MigrateContent (depsChanges |> Map.ofList)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Dialog =
            this.model.Dialog
            
        override this.ContentKey =
            (this :> IDialogNode<'msg>).Dialog
            
        override this.Attachments = this.Attachments

// some utility stuff for Cmd.Dialog

let execDialog (id: string) (msgFunc: bool -> 'msg) =
    let msgFunc2 intValue =
        match intValue with
        | 1 -> true
        | _ -> false
        |> msgFunc
    id, DialogOp.ExecWithResult msgFunc2
    
let execDialogAtPoint (id: string) (p: Point) (msgFunc: bool -> 'msg) =
    let msgFunc2 intValue =
        match intValue with
        | 1 -> true
        | _ -> false
        |> msgFunc
    id, DialogOp.ExecAtPointWithResult (p, msgFunc2)
