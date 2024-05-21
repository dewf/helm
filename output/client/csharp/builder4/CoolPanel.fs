module CoolPanel

open BuilderNode
open Widgets
open Reactor

type Signal =
    | SomethingHappened
type Attr =
    | WindowTitle of label: string
    | ButtonLabel of label: string
let private keyFunc = function
    | WindowTitle _ -> 0
    | ButtonLabel _ -> 1
let private diffAttrs =
    genericDiffAttrs keyFunc

type State = {
    WindowTitle: string
    ButtonLabel: string
    EditValue: string
    AddedItems: string list
}

type Msg =
    | SubmitItem
    | EditChanged of value: string
    | FireSignal

let init () =
    { WindowTitle = "default Window title"
      ButtonLabel = "default"
      EditValue = ""
      AddedItems = [] }, SubCmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | WindowTitle title ->
        { state with WindowTitle = title }
    | ButtonLabel label ->
        { state with ButtonLabel = label }
        
let update (state: State) (msg: Msg) =
    match msg with
    | FireSignal ->
        state, SubCmd.Signal SomethingHappened
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
    let box =
        BoxLayout.Node(
            Attrs = [BoxLayout.Direction BoxLayout.Vertical],
            Items = [ edit; list; button; fireSignal ])
    MainWindow.Node(
        Attrs = [
            MainWindow.Title state.WindowTitle
            MainWindow.Size (800, 600)
            MainWindow.Visible true
        ], Content = box)
    :> IWindowNode<Msg>
    
// for future experiments:
// type MenuBarWrapper<'outerMsg>(wrapped: IMenuBarNode<'outerMsg>, outerDispatch: 'outerMsg -> unit) =
//     member val private Wrapped = wrapped
//     interface IMenuBarNode<Msg> with
//         member this.Dependencies() = []
//         member this.Create(_: Msg -> unit) = // notice we ignore the provided dispatch and use the one from the ctor
//             wrapped.Create(outerDispatch)
//         member this.MigrateFrom(leftNode: IBuilderNode<Msg>) =
//             let leftWrapperNode =
//                 leftNode :?> MenuBarWrapper<'outerMsg>
//             wrapped.MigrateFrom(leftWrapperNode.Wrapped)
//         member this.Dispose() =
//             // safe/correct?
//             wrapped.Dispose()
//         member this.ContentKey =
//             (this :> IMenuBarNode<Msg>).MenuBar
//         member this.MenuBar =
//             wrapped.MenuBar
//

type Node<'outerMsg>() =
    inherit WindowReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, diffAttrs)
    let mutable onSomethingHappened: 'outerMsg option = None
    member this.OnSomethingHappened with set value = onSomethingHappened <- Some value
    override this.SignalMap (s: Signal) =
        match s with
        | SomethingHappened ->
            onSomethingHappened
