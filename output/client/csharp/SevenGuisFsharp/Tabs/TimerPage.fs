module Tabs.TimerPage

open FSharpQt
open BuilderNode
open NonVisual
open Reactor
open FSharpQt.Widgets
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
}

type Msg =
    | Reset
    | SetDuration of value: int
    | TimerTick of elapsed: double

let init() =
    { Duration = 1000
      Accumulated = 0 }, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | Reset ->
        { state with Accumulated = 0 }, Cmd.None
    | SetDuration value ->
        let nextState =
            { state with Duration = value; Accumulated = min state.Accumulated value }
        nextState, Cmd.None
    | TimerTick elapsed ->
        let nextState =
            { state with Accumulated = min (state.Accumulated + int elapsed) state.Duration }
        nextState, Cmd.None
       
let view (state: State) =
    let layout =
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
        let hbox =
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
                    MinimumWidth 250
                ], OnValueChanged = SetDuration)
            BoxLayout(Attrs = [ Direction LeftToRight ],
                      Items = [
                          BoxItem.Create(label)
                          BoxItem.Create(slider)
                      ])
        let button =
            PushButton(Attrs = [ Text "Reset" ], OnClicked = Reset)
        BoxLayout(Attrs = [ Direction TopToBottom ],
                  Items = [
                      BoxItem.Create(progress)
                      BoxItem.Create(hbox)
                      BoxItem.Create(button)
                  ])
    let timer =
        Timer(Attrs = [ Interval TIMER_INTERVAL; Running true ], OnTimeout = TimerTick)
    LayoutWithNonVisual(layout, [ "timer", timer ]) :> ILayoutNode<Msg>
    

type TimerPage<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
    
