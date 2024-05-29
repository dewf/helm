module Tabs.TimerPage

open BuilderNode
open SubReactor
open Widgets

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
    | TimerTick

let init() =
    { Duration = 3000
      Accumulated = 0 }, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | Reset ->
        { state with Accumulated = 0 }, Cmd.None
    | SetDuration value ->
        let nextState =
            { Duration = value; Accumulated = min state.Accumulated value }
        nextState, Cmd.None
    | TimerTick ->
        let nextState =
            { state with Accumulated = state.Accumulated + TIMER_INTERVAL }
        nextState, Cmd.None
       
let view (state: State) =
    let progress =
        ProgressBar.Node(Attrs = [
            ProgressBar.Range (0, 100)
            ProgressBar.Value 56
            ProgressBar.InnerText "Woot"
        ])
    let slider =
        Slider.Node(Attrs = [
            Slider.Orientation Slider.Horizontal
            Slider.Range (100, 10_000)
            Slider.Value 1000
        ], OnValueChanged = SetDuration)
    let button =
        PushButton.Node(Attrs = [ PushButton.Label "Reset" ], OnClicked = Reset)
    let layout =
        BoxLayout.Node(Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
                       Items = [ progress; slider; button ])
    layout :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
    
