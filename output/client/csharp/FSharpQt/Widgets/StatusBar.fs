module FSharpQt.Widgets.StatusBar

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal =
    | MessageChanged of text: string
    
type Attr =
    | Message of maybeText: string option // no timeout supported yet, need to figure out the best "re-triggering" mechanism there

let private keyFunc = function
    | Message _ -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable statusBar = StatusBar.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<StatusBar.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.StatusBar with get() = statusBar
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            statusBar.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Message maybeText ->
                match maybeText with
                | Some text ->
                    statusBar.ShowMessage(text, 0)
                | None ->
                    statusBar.ClearMessage()
                
    interface StatusBar.SignalHandler with
        member this.MessageChanged message =
            signalDispatch (MessageChanged message)
            
    interface IDisposable with
        member this.Dispose() =
            statusBar.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: StatusBar.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: StatusBar.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type StatusBar<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<StatusBar.SignalMask> 0
    
    let mutable onMessageChanged: (string -> 'msg) option = None
    member this.OnMessageChanged with set value =
        onMessageChanged <- Some value
        signalMask <- signalMask ||| StatusBar.SignalMask.MessageChanged
        
    let signalMap = function
        | MessageChanged text ->
            onMessageChanged
            |> Option.map (fun f -> f text)
                
    interface IStatusBarNode<'msg> with
        override this.Dependencies = []

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> StatusBar<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.StatusBar =
            this.model.StatusBar
            
        override this.ContentKey =
            this.model.StatusBar
            
        override this.Attachments =
            this.Attachments
