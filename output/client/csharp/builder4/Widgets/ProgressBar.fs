module Widgets.ProgressBar

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal = unit

type Attr =
    | Range of min: int * max: int
    | Value of value: int
    | TextVisible of state: bool
    | InnerText of text: string

let private attrKey = function
    | Range _ -> 0
    | Value _ -> 1
    | TextVisible _ -> 2
    | InnerText _ -> 3
    
let diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable progressBar = ProgressBar.Create()
    let mutable innerLabel: Label.Handle = null
    member this.Widget = progressBar
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Range(min, max) ->
                progressBar.SetRange(min, max)
            | Value value ->
                progressBar.SetValue(value)
            | TextVisible state ->
                progressBar.SetTextVisible(state)
            | InnerText text ->
                if innerLabel = null then
                    let layout =
                        BoxLayout.Create()
                    layout.SetDirection(Org.Whatever.QtTesting.BoxLayout.Direction.TopToBottom)
                    layout.SetContentsMargins(0, 0, 0, 0)
                    innerLabel <- Label.Create()
                    innerLabel.SetAlignment(Common.Alignment.Center)
                    layout.AddWidget(innerLabel)
                    progressBar.SetLayout(layout)
                    progressBar.SetTextVisible(false) // you probably want this ...
                innerLabel.SetText(text)
                
    interface IDisposable with
        member this.Dispose() =
            progressBar.Dispose()
            // no need to dispose inner label if it exists, because it's owned by the progress bar

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
    member private this.SignalMap
        with get() = (fun _ -> None)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs this.SignalMap
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
        override this.AttachedToWindow window =
            ()
