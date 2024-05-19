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

type Msg =
    | SubmitItem
    | EditChanged of value: string
    | FireSignal
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | ButtonLabel label ->
        { state with ButtonLabel = label }
    
let init () =
    { ButtonLabel = "default"
      EditValue = ""
      AddedItems = [] }, SubCmd.None
    
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
    inherit LayoutNode<'outerMsg>()
    let mutable onSomethingHappened: (string -> 'outerMsg) option = None

    [<DefaultValue>] val mutable reactor: SubReactor<State,Attr,Msg,Signal,LayoutNode<Msg>>
    
    member private this.SignalMap
        with get() = function
            | SomethingHappened str ->
                onSomethingHappened
                |> Option.map (fun f -> f str)
        
    member val Attrs: Attr list = [] with get, set

    member this.OnSomethingHappened with set value = onSomethingHappened <- Some value
    
    override this.Dependencies() = []
    
    override this.Create(dispatch: 'outerMsg -> unit) =
        let rec processCmd (cmd: SubCmd<Msg, Signal>) =
            match cmd with
            | SubCmd.None ->
                ()
            | SubCmd.Signal signal ->
                match this.SignalMap signal with
                | Some outerMsg ->
                    dispatch outerMsg
                | None ->
                    ()
            | SubCmd.Batch commands ->
                commands
                |> List.iter processCmd
        this.reactor <- new SubReactor<State,Attr,Msg,Signal,LayoutNode<Msg>>(init, update, attrUpdate, view, processCmd)
        this.reactor.ApplyAttrs(this.Attrs)

    override this.MigrateFrom(left: BuilderNode<'outerMsg>) =
        let left' = (left :?> Node<'outerMsg>)
        let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
        this.reactor <- left'.reactor
        this.reactor.ApplyAttrs(nextAttrs)

    override this.Dispose() =
        printfn "###### NESTED REACTOR DISPOSING"
        (this.reactor :> IDisposable).Dispose()
        
    override this.Layout =
        this.reactor.Root.Layout
