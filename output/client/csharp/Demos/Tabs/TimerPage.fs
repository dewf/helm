module Tabs.TimerPage

open System
open BuilderNode
open NonVisual
open SubReactor
open Widgets
open BoxLayout
open Label
open PushButton
open Slider
open ProgressBar
open Timer

type Signal = unit
type Attr = unit
let TIMER_INTERVAL = 1000 / 20

type State = {
    Duration: int
    Accumulated: int
    LastTicks: int64
}

type Msg =
    | Reset
    | SetDuration of value: int
    | TimerTick

let init() =
    { Duration = 1000
      Accumulated = 0
      LastTicks = DateTime.Now.Ticks }, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | Reset ->
        { state with Accumulated = 0 }, Cmd.None
    | SetDuration value ->
        let nextState =
            { state with Duration = value; Accumulated = min state.Accumulated value }
        nextState, Cmd.None
    | TimerTick ->
        // default QTimer is ridiculously inaccurate, so we compute our own elapsed time
        let ticks =
            DateTime.Now.Ticks
        let millisSinceLast =
            (ticks - state.LastTicks) / TimeSpan.TicksPerMillisecond
        let nextState =
            { state with
                Accumulated = min (state.Accumulated + int millisSinceLast) state.Duration
                LastTicks = ticks }
        nextState, Cmd.None
       
let view (state: State) =
    let progress =
        let value =
            (state.Accumulated * 1000) / state.Duration
        let text =
            sprintf "%.02fs" (float state.Accumulated / 1000.0)
        ProgressBar(Attrs = [
            Range (0, 1000) // using 1000 divisions ... setting duration directly here seemed to cause flickering
            Value value
            InnerText text
        ])
    let label =
        let text =
            sprintf "%.02fs" (float state.Duration / 1000.0)
        Label(Attrs = [ Label.Text text ])
    let slider =
        Slider(Attrs = [
            Orientation Horizontal
            Slider.Range (100, 10_000)
            TickPosition Below
            TickInterval 1000
            Slider.Value state.Duration
        ], OnValueChanged = SetDuration)
    let hbox =
        BoxLayout(Attrs = [ Direction BoxLayout.Horizontal ],
                  Items = [
                      BoxItem.Create(label)
                      BoxItem.Create(slider)
                  ])
    let button =
        PushButton(Attrs = [ Text "Reset" ], OnClicked = Reset)
    let layout =
        BoxLayout(Attrs = [ Direction BoxLayout.Vertical ],
                  Items = [
                      BoxItem.Create(progress)
                      BoxItem.Create(hbox)
                      BoxItem.Create(button)
                  ])
    let timer =
        Timer(Attrs = [ Interval TIMER_INTERVAL; Running true ], OnTimeout = TimerTick)
    LayoutWithNonVisual(layout, [ "timer", timer ]) :> ILayoutNode<Msg>
    

type TimerPage<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
    
