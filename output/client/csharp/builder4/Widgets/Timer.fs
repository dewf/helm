module Widgets.Timer

open System
open BuilderNode
open NonVisual
open Org.Whatever.QtTesting

type Signal =
    | Timeout
    
type Attr =
    | Interval of millis: int
    | SingleShot of state: bool
    | Running of state: bool
    
let private diffAttrs =
    genericDiffAttrs (function
        | Interval _ -> 0
        | SingleShot _ -> 1
        | Running _ -> 2)
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable timer = Timer.Create()
    do
        let signalDispatch (s: Signal) =
            signalMap s
            |> Option.iter dispatch
        timer.OnTimeout(fun _ -> signalDispatch Timeout)
    member this.QObject with get() = timer
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Interval millis ->
                timer.SetInterval(millis)
            | SingleShot state ->
                timer.SetSingleShot(state)
            | Running state ->
                if state then
                    timer.Start()
                else
                    timer.Stop()
    interface IDisposable with
        member this.Dispose() =
            timer.Dispose()
            
let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Node<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    let mutable onTimeout: 'msg option = None
    member this.OnTimeout
        with set value = onTimeout <- Some value
    member private this.SignalMap
        with get() = function
            | Timeout -> onTimeout

    interface INonVisualNode<'msg> with
        override this.Dependencies() = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.ContentKey =
            // if we ever change NonVisualNode to explicitly represent QObjects, change this to:
            // (this :> INonVisualNode<'msg>).QObject
            this.model.QObject
        override this.AttachedToWindow window =
            ()
        
        
          
            
            
            
            
        
