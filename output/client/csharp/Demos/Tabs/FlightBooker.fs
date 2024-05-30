module Tabs.FlightBooker

open System
open BuilderNode
open HelperControls
open SubReactor
open Widgets
open HelperControls.DatePicker

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
        ComboBox.Node(Attrs = [ ComboBox.Items items ], OnSelected = indexToMsg)
    let labeledPicker labelText value changeMsg enabled =
        let label =
            Label.Node(Attrs = [ Label.Text labelText ])
        let picker =
            DatePicker(
                Attrs = [ Value value; Enabled enabled; DialogTitle $"Select '{labelText}' Date" ],
                OnValueChanged = changeMsg)
        BoxLayout.Node(Attrs = [
            BoxLayout.Direction BoxLayout.Horizontal
            BoxLayout.ContentsMargins (0, 0, 0, 0)
            BoxLayout.Spacing 10
        ], Items = [ label; picker ])
    let depart =
        labeledPicker "Depart" state.DepartDate DepartChanged true
    let return_ =
        labeledPicker "Return" state.ReturnDate ReturnChanged (state.Mode = RoundTrip)
    let status =
        Label.Node(Attrs = [ Label.Text status ])
    let bookButton =
        PushButton.Node(Attrs = [ PushButton.Label "Book Trip"; PushButton.Enabled canBook ])
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
        Items = [ combo; depart; return_; status; bookButton ])
    :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
