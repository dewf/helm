module FSharpQt.Widgets.RadioButton

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal =
    | Clicked
    | ClickedWithState of state: bool // 'checked' state

type Attr =
    | Text of text: string
    | Enabled of enabled: bool

let private keyFunc = function
    | Text _ -> 0
    | Enabled _ -> 1

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable radioButton = RadioButton.Create()
    do
        let dispatcher (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        radioButton.OnClicked (fun state ->
            dispatcher (ClickedWithState state)
            dispatcher Clicked)
        
    member this.Widget with get() = radioButton
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                radioButton.SetText(text)
            | Enabled state ->
                radioButton.SetEnabled(state)
                
    interface IDisposable with
        member this.Dispose() =
            radioButton.Dispose()

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

type RadioButton<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable onClicked: 'msg option = None
    let mutable onClickedWithState: (bool -> 'msg) option = None
    
    member this.OnClicked with set value = onClicked <- Some value
    member this.OnClickedWithState with set value = onClickedWithState <- Some value
        
    member private this.SignalMap
        with get() = function
            | Clicked -> onClicked
            | ClickedWithState state ->
                onClickedWithState
                |> Option.map (fun f -> f state)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []

        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> RadioButton<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.AttachedToWindow window =
            ()
