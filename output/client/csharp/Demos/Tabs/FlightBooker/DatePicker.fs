module Tabs.FlightBooker.DatePicker

open System
open BuilderNode
open SubReactor
open Widgets

type Value =
    | Invalid
    | Valid of dt: DateTime
    
type Signal =
    | ValueChanged of value: Value
    
type Attr =
    | Value of dt: DateTime
    | Enabled of value: bool
    
let keyFunc = function
    | Value _ -> 0
    | Enabled _ -> 1
    
type State = {
    Enabled: bool
    Raw: string
    MaybeParsed: DateTime option
}

type Msg =
    | EditChanged of str: string
    | EditSubmitted
    | ShowCalendar

let init () =
    let dt =
        DateTime.Now
    let state = {
        Enabled = true
        MaybeParsed = Some dt
        Raw = string dt
    }
    state, Cmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Value dt ->
        { state with
            MaybeParsed = Some dt
            Raw = string dt }
    | Enabled value ->
        { state with Enabled = value }
    
let update (state: State) (msg: Msg) =
    // attempt to parse raw text into date
    // update background color of edit if bad
    state, Cmd.None

let view (state: State) =
    let edit =
        LineEdit.Node(
            Attrs = [ LineEdit.Value state.Raw; LineEdit.Enabled state.Enabled ], OnChanged = EditChanged, OnReturnPressed = EditSubmitted)
    let button =
        PushButton.Node(Attrs = [ PushButton.Label "📅"; PushButton.Enabled state.Enabled ], OnClicked = ShowCalendar)
    let hbox =
        BoxLayout.Node(
            Attrs = [
                BoxLayout.Direction BoxLayout.Horizontal
                BoxLayout.Spacing 4
                BoxLayout.ContentsMargins (0, 0, 0, 0)
            ],
            Items = [ edit; button ])
    hbox :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, genericDiffAttrs keyFunc)
