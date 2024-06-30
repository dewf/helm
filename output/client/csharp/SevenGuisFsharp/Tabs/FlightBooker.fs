module Tabs.FlightBooker

open System

open FSharpQt
open BuilderNode
open HelperControls
open Reactor
open FSharpQt.Widgets
open HelperControls.DatePicker
open BoxLayout
open Label
open PushButton
open ComboBox

type Signal = unit
type Attr = unit

type Mode =
    | OneWay
    | RoundTrip
    
type State = {
    Mode: Mode
    DepartDate: DatePicker.Value
    ReturnDate: DatePicker.Value
}

type Msg =
    | ModeChanged of mode: Mode
    | DepartChanged of value: DatePicker.Value
    | ReturnChanged of value: DatePicker.Value

let init() =
    { Mode = OneWay
      DepartDate = Empty
      ReturnDate = Empty }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | DepartChanged value ->
        { state with DepartDate = value }, Cmd.None
    | ReturnChanged value ->
        { state with ReturnDate = value }, Cmd.None
    | ModeChanged mode ->
        let nextMode, nextReturn = 
            match mode with
            | OneWay ->
                let nextReturn =
                    // clear out return date if it was invalid
                    match state.ReturnDate with
                    | Valid dt -> Valid dt
                    | _ -> Empty
                OneWay, nextReturn
            | RoundTrip ->
                let nextReturn =
                    match state.ReturnDate with
                    | Valid dt ->
                        // keep existing since it's OK
                        Valid dt
                    | _ ->
                        // for demonstration purposes, force the user to enter something
                        // but a suitable default would be copying from DepartDate
                        Empty
                RoundTrip, nextReturn
        { state with Mode = nextMode; ReturnDate = nextReturn }, Cmd.None
        
let canBookAndStatus (state: State) =
    match state.Mode with
    | OneWay ->
        match state.DepartDate with
        | Valid value ->
            if value >= DateTime.Today then
                true, "ready to book"
            else
                false, "departure date must be today or later"
        | Invalid ->
            false, "invalid departure date"
        | Empty ->
            false, "departure date required"
    | RoundTrip ->
        match state.DepartDate, state.ReturnDate with
        | Valid depart, Valid return_ ->
            if depart >= DateTime.Today then
                if return_ >= depart then
                    true, "ready to book"
                else
                    false, "return date must be on or after departure date"
            else
                false, "departure date must be today or later"
        | Empty, _ ->
            false, "departure date required"
        | Invalid, _ ->
            false, "invalid departure date"
        | _, Empty ->
            false, "return date required"
        | _, Invalid ->
            false, "invalid return date"

let view (state: State) =
    let canBook, status =
        canBookAndStatus state
    let combo =
        let items = [
            "One Way"
            "Round Trip"
        ]
        let indexToMsg maybeIndex =
            match maybeIndex with
            | Some 0 -> ModeChanged OneWay
            | Some 1 -> ModeChanged RoundTrip
            | _ -> failwith "whoops"
        ComboBox(StringItems = items, OnCurrentIndexChanged = indexToMsg)
    let labeledPicker labelText value changeMsg enabled =
        let label =
            Label(Attrs = [ Label.Text labelText ])
        let picker =
            DatePicker(
                Attrs = [ Value value; DatePicker.Enabled enabled; DialogTitle $"Select '{labelText}' Date" ],
                OnValueChanged = changeMsg)
        HBoxLayout(
            ContentsMargins = (0, 0, 0, 0),
            Spacing = 10,
            Items = [
                BoxItem(label)
                BoxItem(picker)
            ])
    let depart =
        labeledPicker "Depart" state.DepartDate DepartChanged true
    let return_ =
        labeledPicker "Return" state.ReturnDate ReturnChanged (state.Mode = RoundTrip)
    let status =
        Label(Attrs = [ Label.Text status ])
    let bookButton =
        PushButton(Text = "Book Trip", Enabled = canBook)
    VBoxLayout(
        Items = [
            BoxItem(combo)
            BoxItem(depart)
            BoxItem(return_)
            BoxItem(status)
            BoxItem(bookButton)
        ])
    :> ILayoutNode<Msg>
    
type FlightBooker<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
