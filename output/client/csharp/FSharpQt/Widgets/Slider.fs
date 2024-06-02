module FSharpQt.Widgets.Slider

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | ValueChanged of int
    
type TickPos =
    | NoTicks
    | Above
    | Left
    | Below
    | Right
    | BothSides
    
type Orientation =
    | Horizontal
    | Vertical
    
type Attr =
    | Orientation of orient: Orientation
    | Range of min: int * max: int
    | Value of value: int
    | SingleStep of step: int
    | PageStep of pageStep: int
    | Tracking of track: bool
    | TickPosition of tickPos: TickPos
    | TickInterval of interval: int
    
let private attrKey = function
    | Orientation _ -> 0
    | Range _ -> 1
    | Value _ -> 2
    | SingleStep _ -> 3
    | PageStep _ -> 4
    | Tracking _ -> 5
    | TickPosition _ -> 6
    | TickInterval _ -> 7
    
let diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable slider = Slider.Create()
    let mutable lastValue = 0
    do
        let dispatchSignal (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        slider.OnValueChanged (fun value ->
            lastValue <- value
            dispatchSignal (ValueChanged value))
    member this.Widget = slider
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Orientation orient ->
                let orient' =
                    match orient with
                    | Horizontal -> Common.Orientation.Horizontal
                    | Vertical -> Common.Orientation.Vertical
                slider.SetOrientation(orient')
            | Range(min, max) ->
                slider.SetRange(min, max)
            | Value value ->
                if value <> lastValue then
                    lastValue <- value
                    slider.SetValue(value)
            | SingleStep step ->
                slider.SetSingleStep(step)
            | PageStep pageStep ->
                slider.SetPageStep(pageStep)
            | Tracking track ->
                slider.SetTracking(track)
            | TickPosition tickPos ->
                let tickPos' =
                    match tickPos with
                    | NoTicks -> Slider.TickPosition.None
                    | Above -> Slider.TickPosition.Above
                    | Left -> Slider.TickPosition.Left
                    | Below -> Slider.TickPosition.Below
                    | Right -> Slider.TickPosition.Right
                    | BothSides -> Slider.TickPosition.BothSides
                slider.SetTickPosition(tickPos')
            | TickInterval interval ->
                slider.SetTickInterval(interval)
    interface IDisposable with
        member this.Dispose() =
            slider.Dispose()

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

type Slider<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    let mutable onValueChanged: (int -> 'msg) option = None
    member this.OnValueChanged with set value = onValueChanged <- Some value
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap
        with get() = function
            | ValueChanged value ->
                onValueChanged
                |> Option.map (fun f -> f value)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Slider<'msg>)
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
