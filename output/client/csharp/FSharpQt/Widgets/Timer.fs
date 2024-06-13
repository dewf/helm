module FSharpQt.Widgets.Timer

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Timeout of elapsed: double // millis
    
type Attr =
    | Interval of millis: int
    | SingleShot of state: bool
    | Running of state: bool
    
let attrKey = function
    | Interval _ -> 0
    | SingleShot _ -> 1
    | Running _ -> 2    
    
let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable timer = Timer.Create()
    let mutable lastTicks = 0L
    do
        let signalDispatch (s: Signal) =
            signalMap s
            |> Option.iter dispatch
            
        timer.OnTimeout(fun _ ->
            let ticks =
                DateTime.Now.Ticks
            let elapsed =
                double (ticks - lastTicks) / double TimeSpan.TicksPerMillisecond
            lastTicks <- ticks
            signalDispatch (Timeout elapsed))
        
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
                    lastTicks <- DateTime.Now.Ticks
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

type Timer<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>

    let mutable onTimeout: (double -> 'msg) option = None
    member this.OnTimeout with set value = onTimeout <- Some value
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = function
        | Timeout elapsed ->
            onTimeout
            |> Option.map (fun f -> f elapsed)
                
    interface INonVisualNode<'msg> with
        override this.Dependencies = []
        
        override this.Create2 dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Timer<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.ContentKey =
            this.model.QObject
            
        override this.Attachments =
            this.Attachments
