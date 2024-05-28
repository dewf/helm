module Tabs.FlightBooker.FlightBooker

open System
open BuilderNode
open SubReactor
open Tabs.FlightBooker
open Widgets

type Signal = unit
type Attr = unit

type Mode =
    | OneWay
    | RoundTrip

type State = {
    Mode: Mode
    DepartDate: DateTime option
    ReturnDate: DateTime option
}

type WhichPicker =
    | Depart
    | Return

type Msg =
    | ComboChanged of maybeIndex: int option
    | PickerChanged of which: WhichPicker * value: DatePicker.Value

let init() =
    { Mode = OneWay
      DepartDate = Some DateTime.Now
      ReturnDate = None }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | PickerChanged (which, value) ->
        let optValue =
            match value with
            | DatePicker.Valid dt -> Some dt
            | DatePicker.Invalid -> None
        match which with
        | Depart ->
            { state with DepartDate = optValue }, Cmd.None
        | Return ->
            { state with ReturnDate = optValue }, Cmd.None
    | ComboChanged maybeIndex ->
        let nextMode = 
            match maybeIndex with
            | Some 0 -> OneWay
            | Some 1 -> RoundTrip
            | _ -> failwith "nope"
        { state with Mode = nextMode }, Cmd.None

let view (state: State) =
    let canBook =
        match state.Mode with
        | OneWay ->
            match state.DepartDate with
            | Some value ->
                value >= DateTime.Today
            | None ->
                false
        | RoundTrip ->
            match state.DepartDate, state.ReturnDate with
            | Some depart, Some return_ ->
                depart >= DateTime.Today && return_ >= depart
            | _ ->
                false
    let combo =
        let selectedIndex =
            match state.Mode with
            | OneWay -> 0
            | RoundTrip -> 1
        let items = [
            "One Way"
            "Round Trip"
        ]
        // PushButton.Node(Attrs = [ PushButton.Label "just testing" ])
        ComboBox.Node(Attrs = [ ComboBox.Items items; ComboBox.SelectedIndex (Some selectedIndex) ], OnSelected = ComboChanged)
    let edit1 =
        DatePicker.Node(OnValueChanged = (fun value -> PickerChanged (Depart, value)))
    let edit2 =
        DatePicker.Node(Attrs = [ DatePicker.Enabled (state.Mode = RoundTrip) ], OnValueChanged = (fun value -> PickerChanged (Return, value)))
    let bookButton =
        PushButton.Node(Attrs = [ PushButton.Label "Book Trip"; PushButton.Enabled canBook ])
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
        Items = [ combo; edit1; edit2; bookButton ])
    :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
