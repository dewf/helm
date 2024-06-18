module FSharpQt.Widgets.Dialog

open System
open FSharpQt
open BuilderNode
open MiscTypes
open Reactor
open Org.Whatever.QtTesting

type Signal =
    | Accepted
    | Finished of result: int
    | Rejected
    
type Modality =
    | WindowModal
    | AppModal
    
type Attr =
    | Size of width: int * height: int
    | Title of title: string
    | Modality of modality: Modality
    
let private keyFunc = function
    | Size _ -> 0
    | Title _ -> 1
    | Modality _ -> 2

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeParent: Widget.Handle option) as this =
    let mutable dialog =
        let parentHandle =
            maybeParent
            |> Option.defaultValue null
        Dialog.Create(parentHandle, this)
    
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Dialog.SignalMask> 0
    
    let dispatcher (s: Signal) =
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
    
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Size (w, h) ->
                dialog.Resize(w, h)
            | Title title ->
                dialog.SetWindowTitle title
            | Modality modality ->
                let qModality =
                    match modality with
                    | WindowModal -> Enums.WindowModality.WindowModal
                    | AppModal -> Enums.WindowModality.ApplicationModal
                dialog.SetWindowModality(qModality)
                
    interface Dialog.SignalHandler with
        member this.Accepted() =
            dispatcher Accepted
        member this.Finished result =
            // I think this is from .done(result) method which we don't currently use
            dispatcher (Finished result)
        member this.Rejected() =
            dispatcher Rejected
    
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

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialMask: Dialog.SignalMask) (maybeParent: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeParent)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- initialMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: Dialog.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Dialog<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set

    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member private this.MaybeLayout = maybeLayout
    member this.Layout with set value = maybeLayout <- Some value

    let mutable signalMask = enum<Dialog.SignalMask> 0
        
    let mutable onAccepted: 'msg option = None
    let mutable onFinished: (int -> 'msg) option = None
    let mutable onRejected: 'msg option = None

    member this.OnAccepted with set value =
        onAccepted <- Some value
        signalMask <- signalMask ||| Dialog.SignalMask.Accepted
    
    member this.OnFinished with set value =
        onFinished <- Some value
        signalMask <- signalMask ||| Dialog.SignalMask.Finished
    
    member this.OnRejected with set value =
        onRejected <- Some value
        signalMask <- signalMask ||| Dialog.SignalMask.Rejected
        
    let signalMap (signal: Signal) =
        match signal with
        | Accepted ->
            onAccepted
        | Finished result ->
            onFinished
            |> Option.map (fun f -> f result)
        | Rejected ->
            onRejected
    
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
            this.model <- create this.Attrs signalMap dispatch signalMask buildContext.ContainingWindow
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun layout -> this.model.AddLayout layout.Layout)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Dialog<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
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
