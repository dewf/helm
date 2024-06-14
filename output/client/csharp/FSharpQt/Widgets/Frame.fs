module FSharpQt.Widgets.Frame

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal = unit

type Shape =
    | NoFrame
    | Box
    | Panel
    | StyledPanel
    | HLine
    | VLine
    | WinPanel
with
    member this.QtShape =
        match this with
        | NoFrame -> Frame.Shape.NoFrame
        | Box -> Frame.Shape.Box
        | Panel -> Frame.Shape.Panel
        | StyledPanel -> Frame.Shape.StyledPanel
        | HLine -> Frame.Shape.HLine
        | VLine -> Frame.Shape.VLine
        | WinPanel -> Frame.Shape.WinPanel
    
type Shadow =
    | Plain
    | Raised
    | Sunken
with
    member this.QtShadow =
        match this with
        | Plain -> Frame.Shadow.Plain
        | Raised -> Frame.Shadow.Raised
        | Sunken -> Frame.Shadow.Sunken

type Attr =
    | Shape of shape: Shape
    | Shadow of shadow: Shadow
    | Style of shape: Shape * shadow: Shadow
    
let private diffAttrs =
    genericDiffAttrs (function
        | Shape _ -> 0
        | Shadow _ -> 1
        | Style _ -> 2)
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable frame = Frame.Create()
    
    // no 'do' block currently since no signals
    member this.Widget with get() = frame
    member this.SignalMap with set value = signalMap <- value
    
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Shape shape ->
                frame.SetFrameShape(shape.QtShape)
            | Shadow shadow ->
                frame.SetFrameShadow(shadow.QtShadow)
            | Style(shape, shadow) ->
                frame.SetFrameStyle(shape.QtShape, shadow.QtShadow)
                
    interface IDisposable with
        member this.Dispose() =
            frame.Dispose()

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

type Frame<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = (fun _ -> None)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Frame<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
