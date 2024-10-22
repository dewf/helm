﻿module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Models
open FSharpQt.Widgets.LineEdit
open FSharpQt.Widgets.StatusBar
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
    Suffix: string
    StatusCounter: int
}

type Msg =
    | AddRow
    | AppExit
    | ToggleHeaders
    | FilterTextEdited of text: string
    | ReplaceSuffix
    | ShowStatus
    
let cmdSetStatus text timeout =
    Cmd.ViewExec (fun bindings ->
        viewexec bindings {
            let! status = StatusBar.bindNode "status"
            status.ShowMessage(text, timeout)
        })

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
        Suffix = "Initial Suffix"
        StatusCounter = 1
    }
    nextState, cmdSetStatus "status bar init" 0
    
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
    | ReplaceSuffix ->
        { state with Suffix = "REPLACED!!" }, Cmd.None
    | ShowStatus ->
        let nextState =
            { state with StatusCounter = state.StatusCounter + 1 }
        let cmd =
            let text =
                sprintf "status bar update %d" state.StatusCounter
            cmdSetStatus text 500
        nextState, cmd
    
let view (state: State) =
    let exitAction =
        let seq =
            KeySequence(Key.Q, [ Control ])
        MenuAction(
            Icon = Icon(ThemeIcon.Computer),
            Text = "E&xit",
            Shortcut = seq,
            OnTriggered = AppExit)

    let menuBar =
        let fileMenu =
            Menu(Title = "&File",
                 Items = [
                     MenuItem(exitAction)
                 ])
        MenuBar(Menus = [ fileMenu ])
        
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
        ListModelNode(dataFunc, 3,
                      Rows = state.ListData,
                      Headers = headers)
        
    let proxyModel =
        let regex =
            match state.CurrentFilter with
            | Some filter -> Regex(filter, [RegexOption.CaseInsensitive])
            | None -> Regex()
        SortFilterProxyModel(
            FilterRegularExpression = regex,
            FilterKeyColumn = Some 1,
            SourceModel = listModel)
        
    let treeView =
        TreeView(SortingEnabled = true, TreeModel = proxyModel)
        
    let button =
        PushButton(Text = "Add One", OnClicked = AddRow)
        
    let toggleButton =
        PushButton(Text = "Toggle Headers", OnClicked = ToggleHeaders)
        
    let filterEdit =
        LineEdit(ClearButtonEnabled = true, OnTextEdited = FilterTextEdited)
        
    let fakeButton =
        FakeThing(Attrs = [Suffix state.Suffix])
        
    let replaceSuffixButton =
        PushButton(Text = "Replace Suffix", OnClicked = ReplaceSuffix)
        
    let showStatusButton =
        PushButton(Text = "Show Status", OnClicked = ShowStatus)
        
    let vbox =
        VBoxLayout(Items = [
            BoxItem(treeView)
            BoxItem(button)
            BoxItem(toggleButton)
            BoxItem(filterEdit)
            BoxItem(fakeButton)
            BoxItem(replaceSuffixButton)
            BoxItem(showStatusButton)
        ])
        
    let status =
        StatusBar(Name = "status")
        
    MainWindow(
        WindowTitle = "Hmmm wut?",
        Size = Size.From (640, 480),
        CentralLayout = vbox,
        MenuBar = menuBar,
        StatusBar = status
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
    app.SetStyle(Fusion)
    app.Run argv
