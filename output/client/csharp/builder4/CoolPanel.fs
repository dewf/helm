module CoolPanel

open System
open BuilderNode
open Widgets
open Reactor

type Signal =
    | SomethingHappened of value: string
type Attr =
    | ButtonLabel of label: string
let private keyFunc = function
    | ButtonLabel _ -> 0
let private diffAttrs =
    genericDiffAttrs keyFunc

type State = {
    ButtonLabel: string
    EditValue: string
    AddedItems: string list
}

let init () =
    { ButtonLabel = "default"
      EditValue = ""
      AddedItems = [] }, SubCmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | ButtonLabel label ->
        { state with ButtonLabel = label }

type Msg =
    | SubmitItem
    | EditChanged of value: string
    | FireSignal
    
let update (state: State) (msg: Msg) =
    match msg with
    | FireSignal ->
        state, SubCmd.Signal (SomethingHappened "yazooo")
    | SubmitItem ->
        let toAdd =
            if state.EditValue <> "" then
                state.EditValue
            else
                sprintf "Added Item %02d" (state.AddedItems.Length + 1)
        let nextItems =
            toAdd :: state.AddedItems
        let nextState =
            { state with
                AddedItems = nextItems
                EditValue = "" }
        nextState, SubCmd.None
    | EditChanged value ->
        printfn "edit changed: %s" value
        let nextState =
            { state with EditValue = value }
        nextState, SubCmd.None

let view (state: State) =
    let edit =
        LineEdit.Node(
            Attrs = [LineEdit.Value state.EditValue],
            OnChanged = EditChanged,
            OnReturnPressed = SubmitItem)
    let list =
        let allItems =
            ["One"; "Two"; "Three";"Four"] @ (state.AddedItems |> List.rev)
        ListWidget.Node(Attrs = [ListWidget.Items allItems])
    let button =
        PushButton.Node(Attrs = [PushButton.Label state.ButtonLabel], OnClicked = SubmitItem)
    let fireSignal =
        PushButton.Node(Attrs = [PushButton.Label "Fire Signal!"], OnClicked = FireSignal)
    BoxLayout.Node(
        Attrs = [BoxLayout.Direction BoxLayout.Vertical],
        Items = [ edit; list; button; fireSignal ])
    :> LayoutNode<Msg>

type Node<'outerMsg>() =
    inherit ReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, diffAttrs)
    let mutable onSomethingHappened: (string -> 'outerMsg) option = None
    member this.OnSomethingHappened with set value = onSomethingHappened <- Some value
    override this.SignalMap (s: Signal) =
        match s with
        | SomethingHappened str ->
            onSomethingHappened
            |> Option.map (fun f -> f str)
