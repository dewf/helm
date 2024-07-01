module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Models
open FSharpQt.Widgets.LineEdit
open FakeThing
open ListModelNode
open SortFilterProxyModel

open FSharpQt.Widgets.BoxLayout
open Reactor
open InputEnums

open FSharpQt.Widgets
open MainWindow
open PushButton
open TreeView

open FSharpQt.Widgets.Menus
open Menu
open MenuAction
open MenuBar

open Models.TrackedRows

let wordList = [|
    "argument"
    "paste"
    "store"
    "spectacular"
    "befitting"
    "sweep"
    "macho"
    "explain"
    "encourage"
    "hush"
    "probable"
    "admire"
    "uninterested"
    "incredible"
    "opposite"
    "pushy"
    "efficient"
    "spotty"
    "connection"
    "account"
    "abnormal"
    "dusty"
    "apparel"
    "infuse"
    "subdued"
    "impolite"
    "roomy"
    "understand"
    "majestic"
    "observant"
    "scissors"
    "cumbersome"
    "trip"
|]

type ListRow = {
    First: string
    Second: string
    Color: Color
    IntValue: int
}

type State = {
    ListData: TrackedRows<ListRow>
    HeadersToggled: bool
    CurrentFilter: string option
    NextIntValue: int
}

type Msg =
    | AddRow
    | AppExit
    | ToggleHeaders
    | FilterTextEdited of text: string

let init () =
    let initRows = TrackedRows.Init([
        { First = "One"; Second = "Woot"; Color = Color(ColorConstant.Green); IntValue = 1 }
        { First = "Two"; Second = "Doot"; Color = Color(ColorConstant.Magenta); IntValue = 2 }
        { First = "Three"; Second = "McGroot"; Color = Color(ColorConstant.Red); IntValue = 3 }
    ])
    let nextState = {
        ListData = initRows
        HeadersToggled = false
        CurrentFilter = None
        NextIntValue = 4 
    }
    nextState, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | AddRow ->
        let nextData =
            let text =
                sprintf "new row %d" (state.ListData.RowCount + 1)
            let color =
                let steps = 15
                let interp start end' current =
                    ((end' - start) * current) / steps + start
                let r = 40
                let g = interp 80 160 state.ListData.RowCount |> min 255
                let b = interp 120 145 state.ListData.RowCount |> min 255
                Color(r, g, b)
            let secondaryText =
                wordList[state.ListData.RowCount % wordList.Length]
            state.ListData
                .BeginChanges()
                .AddRow({ First = text; Color = color; Second = secondaryText; IntValue = state.NextIntValue })
        let nextState =
            { state with ListData = nextData; NextIntValue = state.NextIntValue + 1 }
        nextState, Cmd.None
    | AppExit ->
        state, Cmd.Signal QuitApplication
    | ToggleHeaders ->
        { state with HeadersToggled = not state.HeadersToggled }, Cmd.None
    | FilterTextEdited text ->
        let nextFilter =
            if text = "" then None else Some text
        { state with CurrentFilter = nextFilter }, Cmd.None
    
let view (state: State) =
    let exitAction =
        let seq =
            KeySequence(Key.Q, [ Control ])
        MenuAction(Text = "E&xit", Shortcut = seq, OnTriggered = (fun _ -> AppExit))

    let menuBar =
        let menu =
            Menu(Attrs = [ Title "&File" ],
                 Items = [
                     MenuItem(exitAction)
                 ])
        MenuBar(Menus = [ menu ])
        
    let listModel =
        let dataFunc row col role =
            match col, role with
            | 0, DisplayRole -> Variant.String row.First
            | 0, DecorationRole -> Variant.Color row.Color
            | 1, DisplayRole -> Variant.String row.Second
            | 2, DisplayRole -> Variant.Int row.IntValue
            | _ -> Variant.Empty
        let headers =
            if state.HeadersToggled then
                [ "CHANGED 01"; "CHANGED 02"; "CHANGED 03" ]
            else
                [ "Primary"; "Secondary"; "Sequence" ]
        ListModelNode(dataFunc, 3, Attrs = [ Rows state.ListData; Headers headers ])
        
    let proxyModel =
        let regex =
            match state.CurrentFilter with
            | Some filter -> Regex(filter, [RegexOption.CaseInsensitive])
            | None -> Regex()
        SortFilterProxyModel(
            Attrs = [
                FilterRegex regex
                FilterKeyColumn (Some 1)
            ],
            SourceModel = listModel)
        
    let treeView =
        TreeView(Attrs = [ SortingEnabled true ], TreeModel = proxyModel)
        
    let button =
        PushButton(Text = "Add One", OnClicked = AddRow)
        
    let toggleButton =
        PushButton(Text = "Toggle Headers", OnClicked = ToggleHeaders)
        
    let filterEdit =
        LineEdit(ClearButtonEnabled = true, OnTextEdited = FilterTextEdited)
        
    let fakeButton =
        FakeThing(Attrs2 = [Suffix "whoaaaa!!"])
        
    let vbox =
        VBoxLayout(Items = [
            BoxItem(treeView)
            BoxItem(button)
            BoxItem(toggleButton)
            BoxItem(filterEdit)
            BoxItem(fakeButton)
        ])
        
    MainWindow(
        Attrs = [ MainWindow.Title "Hmmm wut?"; Size (640, 480) ],
        CentralLayout = vbox,
        MenuBar = menuBar
        // actions are already owned by MainWindow, and once installed via menu I believe they are active anyway - this doesn't hurt, but in our case it's not necessary, either
        // Actions = [
        //     "first", action1
        //     // "second", action2
        //     // "last", exitAction
        // ]
        )
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.SetStyle(Windows)
    app.Run argv
