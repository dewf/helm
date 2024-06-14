module FSharpQt.Widgets.Label

open FSharpQt.BuilderNode
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
    | FrameShape of shape: FSharpQt.Widgets.Frame.Shape
    | FrameShadow of shadow: FSharpQt.Widgets.Frame.Shadow
    | FrameStyle of shape: FSharpQt.Widgets.Frame.Shape * shadow: FSharpQt.Widgets.Frame.Shadow
    | FrameLineWidth of width: int
    | FrameMidLineWidth of midWidth: int
    | Text of text: string
    | Alignment of align: Align
    
let keyFunc = function
    | FrameShape _ -> 0
    | FrameShadow _ -> 1
    | FrameStyle _ -> 2
    | FrameLineWidth _ -> 3
    | FrameMidLineWidth _ -> 4
    | Text _ -> 5
    | Alignment _ -> 6

let private diffAttrs =
    genericDiffAttrs keyFunc

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
            | FrameShape shape ->
                label.SetFrameShape(shape.QtShape)
            | FrameShadow shadow ->
                label.SetFrameShadow(shadow.QtShadow)
            | FrameStyle(shape, shadow) ->
                label.SetFrameStyle(shape.QtShape, shadow.QtShadow)
            | FrameLineWidth width ->
                label.SetLineWidth(width)
            | FrameMidLineWidth midWidth ->
                label.SetMidLineWidth(midWidth)
                
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

type Label<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = (fun _ -> None)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps() =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Label<'msg>)
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
