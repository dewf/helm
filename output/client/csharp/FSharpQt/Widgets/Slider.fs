module FSharpQt.Widgets.Slider

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type SliderAction =
    | NoAction
    | SingleStepAdd
    | SingleStepSub
    | PageStepAdd
    | PageStepSub
    | ToMinimum
    | ToMaximum
    | Move
with
    static member From(value: Slider.SliderAction) =
        match value with
        | Slider.SliderAction.NoAction -> NoAction
        | Slider.SliderAction.SingleStepAdd -> SingleStepAdd
        | Slider.SliderAction.SingleStepSub -> SingleStepSub
        | Slider.SliderAction.PageStepAdd -> PageStepAdd
        | Slider.SliderAction.PageStepSub -> PageStepSub
        | Slider.SliderAction.ToMinimum -> ToMinimum
        | Slider.SliderAction.ToMaximum -> ToMaximum
        | Slider.SliderAction.Move -> Move
        | _ -> failwith "SliderAction.From: wut"

type Signal =
    | ActionTriggered of action: SliderAction
    | RangeChanged of min: int * max: int
    | SliderMoved of value: int
    | SliderPressed
    | SliderReleased
    | ValueChanged of value: int
    
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
    | MinimumWidth of width: int
    | MinimumHeight of height: int
    
let private attrKey = function
    | Orientation _ -> 0
    | Range _ -> 1
    | Value _ -> 2
    | SingleStep _ -> 3
    | PageStep _ -> 4
    | Tracking _ -> 5
    | TickPosition _ -> 6
    | TickInterval _ -> 7
    | MinimumWidth _ -> 8
    | MinimumHeight _ -> 9
    
let diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable slider = Slider.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Slider.SignalMask> 0
    let mutable lastValue = 0
    
    let dispatchSignal (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
            
    member this.Widget = slider
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            slider.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Orientation orient ->
                let orient' =
                    match orient with
                    | Horizontal -> Enums.Orientation.Horizontal
                    | Vertical -> Enums.Orientation.Vertical
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
            | MinimumWidth width ->
                slider.SetMinimumWidth(width)
            | MinimumHeight height ->
                slider.SetMaximumHeight(height)
                
    interface Slider.SignalHandler with
        override this.ActionTriggered action =
            dispatchSignal (SliderAction.From action |> ActionTriggered)
        override this.RangeChanged (min, max) =
            dispatchSignal (RangeChanged (min, max))
        override this.SliderMoved value =
            dispatchSignal (SliderMoved value)
        override this.SliderPressed () =
            dispatchSignal SliderPressed
        override this.SliderReleased () =
            dispatchSignal SliderReleased
        override this.ValueChanged value =
            dispatchSignal (ValueChanged value)
                
    interface IDisposable with
        member this.Dispose() =
            slider.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Slider.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: Slider.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Slider<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
        
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<Slider.SignalMask> 0
    
    let mutable onActionTriggered: (SliderAction -> 'msg) option = None
    let mutable onRangeChanged: (int * int -> 'msg) option = None
    let mutable onSliderMoved: (int -> 'msg) option = None
    let mutable onSliderPressed: 'msg option = None
    let mutable onSliderReleased: 'msg option = None
    let mutable onValueChanged: (int -> 'msg) option = None
    
    member this.OnActionTriggered with set value =
        onActionTriggered <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.ActionTriggered
    member this.OnRangeChanged with set value =
        onRangeChanged <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.RangeChanged
    member this.OnSliderMoved with set value =
        onSliderMoved <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.SliderMoved
    member this.OnSliderPressed with set value =
        onSliderPressed <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.SliderPressed
    member this.OnSliderReleased with set value =
        onSliderReleased <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.SliderReleased
    member this.OnValueChanged with set value =
        onValueChanged <- Some value
        signalMask <- signalMask ||| Slider.SignalMask.ValueChanged
    
    let signalMap = function
        | ActionTriggered action ->
            onActionTriggered
            |> Option.map (fun f -> f action)
        | RangeChanged (min, max) ->
            onRangeChanged
            |> Option.map (fun f -> f (min, max))
        | SliderMoved value ->
            onSliderMoved
            |> Option.map (fun f -> f value)
        | SliderPressed ->
            onSliderPressed
        | SliderReleased ->
            onSliderReleased
        | ValueChanged value ->
            onValueChanged
            |> Option.map (fun f -> f value)
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Slider<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
