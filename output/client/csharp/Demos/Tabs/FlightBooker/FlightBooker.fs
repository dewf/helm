module Tabs.FlightBooker.FlightBooker

open System
open BuilderNode
open SubReactor
open Tabs.FlightBooker
open Tabs.FlightBooker.DatePicker
open Widgets

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

type WhichPicker =
    | Depart
    | Return

type Msg =
    | ComboChanged of maybeIndex: int option
    | PickerChanged of which: WhichPicker * value: DatePicker.Value

let init() =
    { Mode = OneWay
      DepartDate = Valid DateTime.Today
      ReturnDate = Empty }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | PickerChanged (which, value) ->
        match which with
        | Depart ->
            { state with DepartDate = value }, Cmd.None
        | Return ->
            { state with ReturnDate = value }, Cmd.None
    | ComboChanged maybeIndex ->
        let nextMode, nextReturn = 
            match maybeIndex with
            | Some 0 ->
                let nextReturn =
                    // clear out return date if it was invalid
                    match state.ReturnDate with
                    | Valid dt -> Valid dt
                    | _ -> Empty
                OneWay, nextReturn
            | Some 1 ->
                let nextReturn =
                    match state.ReturnDate with
                    | Valid dt ->
                        // keep existing since it's OK
                        Valid dt
                    | _ ->
                        // needs a suitable default - try copying from depart date
                        match state.DepartDate with
                        | Valid dt -> Valid dt
                        | _ -> Empty
                RoundTrip, nextReturn
            | _ -> failwith "nope"
        { state with Mode = nextMode; ReturnDate = nextReturn }, Cmd.None
        
let computeStatus (state: State) =
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
        computeStatus state
    let combo =
        let items = [
            "One Way"
            "Round Trip"
        ]
        ComboBox.Node(Attrs = [ ComboBox.Items items ], OnSelected = ComboChanged)
    let edit1 =
        DatePicker(
            Attrs = [ Value state.DepartDate ],
            OnValueChanged = (fun value -> PickerChanged (Depart, value)))
    let edit2 =
        DatePicker(
            Attrs = [
                Value state.ReturnDate
                Enabled (state.Mode = RoundTrip)
            ],
            OnValueChanged = (fun value -> PickerChanged (Return, value)))
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
