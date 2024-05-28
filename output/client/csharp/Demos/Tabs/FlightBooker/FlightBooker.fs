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
        
let computeStatus (state: State) =
    match state.Mode with
    | OneWay ->
        match state.DepartDate with
        | Some value ->
            if value >= DateTime.Today then
                true, "ready to book"
            else
                false, "departure date must be today or later"
        | None ->
            false, "invalid departure date"
    | RoundTrip ->
        match state.DepartDate, state.ReturnDate with
        | Some depart, Some return_ ->
            if depart >= DateTime.Today then
                if return_ >= depart then
                    true, "ready to book"
                else
                    false, "return date must be on or after departure date"
            else
                false, "departure date must be today or later"
        | _ ->
            false, "roundtrip requires two valid dates"

let view (state: State) =
    let canBook, status =
        computeStatus state
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
    let status =
        Label.Node(Attrs = [ Label.Text status ])
    let bookButton =
        PushButton.Node(Attrs = [ PushButton.Label "Book Trip"; PushButton.Enabled canBook ])
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
        Items = [ combo; edit1; edit2; status; bookButton ])
    :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
