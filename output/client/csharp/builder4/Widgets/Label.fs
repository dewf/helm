module Widgets.Label

open BuilderNode
open System
open Org.Whatever.QtTesting

type Signal = unit

type Align =
    | Left
    | HCenter
    | Right
    | Top
    | VCenter
    | Bottom
    | Center

type Attr =
    | Text of text: string
    | Alignment of align: Align

let private diffAttrs =
    genericDiffAttrs (function
        | Text _ -> 0
        | Alignment _ -> 1)

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable label = Label.Create()
    // no 'do' block currently since no signals
    member this.Widget with get() = label
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                label.SetText(text)
            | Alignment align ->
                let qAlign =
                    match align with
                    | Left -> Common.Alignment.Left
                    | HCenter -> Common.Alignment.HCenter
                    | Right -> Common.Alignment.Right
                    | Top -> Common.Alignment.Top
                    | VCenter -> Common.Alignment.VCenter
                    | Bottom -> Common.Alignment.Bottom
                    | Center -> Common.Alignment.Center
                label.SetAlignment(qAlign)
    interface IDisposable with
        member this.Dispose() =
            label.Dispose()
            
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
    member private this.SignalMap = (fun _ -> None)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies() = []
        
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
