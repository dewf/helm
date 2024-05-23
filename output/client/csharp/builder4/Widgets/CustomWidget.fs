module Widgets.CustomWidget

open BuilderNode
open System
open Org.Whatever.QtTesting


// PaintState stuff -- required for custom drawing ====================
type UpdateArea =
    | NotRequired
    | Everything
    | Rects of Common.Rect list
    
[<AbstractClass>]
type AbstractPaintState() =
    abstract member ComputeUpdateArea: AbstractPaintState -> UpdateArea
    abstract member DoPaint: Widget.Handle -> Painter.Handle -> Common.Rect -> unit
    abstract member IsEqualTo: AbstractPaintState -> bool
    abstract member CustomHashCode: int
    override this.Equals(other: Object) =
        match other with
        | :? AbstractPaintState as otherPS -> this.IsEqualTo(otherPS)
        | _ -> false
    override this.GetHashCode() =
        this.CustomHashCode
        
// begin widget proper =================================================

type Signal =
    | MousePress of ev: Widget.MouseEvent
    
type Attr =
    | PaintState of ps: AbstractPaintState
    | UpdatesEnabled of state: bool
    
let private attrKey = function
    | PaintState _ -> 0
    | UpdatesEnabled _ -> 1
    
let private diffAttrs =
    genericDiffAttrs attrKey

type Model<'msg>(dispatch: 'msg -> unit, methodMask: Widget.MethodMask) as self =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let widget = Widget.CreateSubclassed(self, methodMask)
    let signalDispatch s =
        signalMap s
        |> Option.iter dispatch
        
    let mutable lastPaintState: AbstractPaintState option = None
        
    interface Widget.MethodDelegate with
        override this.PaintEvent(painter: Painter.Handle, rect: Common.Rect) =
            lastPaintState
            |> Option.iter (fun ps -> ps.DoPaint widget painter rect)
        override this.MousePressEvent(mouseEvent: Widget.MouseEvent) =
            signalDispatch (MousePress mouseEvent)

        // override this.Dispose() =
        //     // I forget why the generated method delegates have this ...
        //     ()

    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | PaintState ps ->
                // raw paintstate changed, but do we actually need a redraw?
                // the provided PaintState can perform more precise checking
                let updateArea =
                    match lastPaintState with
                    | Some last ->
                        ps.ComputeUpdateArea last
                    | None ->
                        // first incoming paintstate, must draw!
                        Everything
                // assign now that we've checked
                lastPaintState <- Some ps
                // then perform any invalidations requested
                match updateArea with
                | NotRequired ->
                    ()
                | Everything ->
                    widget.Update()
                | Rects rects ->
                    for rect in rects do
                        widget.Update(rect)
            | UpdatesEnabled state ->
                widget.SetUpdatesEnabled(state)

    interface IDisposable with
        member this.Dispose() =
            widget.Dispose()

let rec private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (methodMask: Widget.MethodMask) =
    let model = new Model<'msg>(dispatch, methodMask)
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
    let mutable onMousePress: (Widget.MouseEvent -> 'msg) option = None
    member this.OnMousePress with set value = onMousePress <- Some value
    member private this.SignalMap
        with get() = function
            | MousePress ev ->
                onMousePress
                |> Option.map (fun f -> f ev)
    
    member private this.MethodMask
        with get() = 
            let mousePressValue =
                match onMousePress with
                | Some _ -> Widget.MethodMask.MousePressEvent
                | None -> Widget.MethodMask.None
            // always with PaintEvent, for now (else what's the point?)
            mousePressValue ||| Widget.MethodMask.PaintEvent
                
    interface IWidgetNode<'msg> with
        override this.Dependencies() = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch this.MethodMask
        override this.MigrateFrom(left: IBuilderNode<'msg>) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs this.SignalMap
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Widget =
            this.model.Widget
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
