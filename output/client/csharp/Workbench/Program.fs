module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Models.ListModelNode
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

let SECONDARY_VALUES = [|
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
    Color: Color
    Second: string
}

type State = {
    ListData: TrackedRows<ListRow>
}

type Msg =
    | AddRow
    | AppExit

let init () =
    let initRows = TrackedRows.Init([
        { First = "One"; Color = Color(ColorConstant.Green); Second = "Woot" }
        { First = "Two"; Color = Color(ColorConstant.Magenta); Second = "Doot" }
        { First = "Three"; Color = Color(ColorConstant.Red); Second = "McGroot" }
    ])
    let nextState = {
        ListData = initRows
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
                SECONDARY_VALUES[state.ListData.RowCount % SECONDARY_VALUES.Length]
            state.ListData
                .BeginChanges()
                .AddRow({ First = text; Color = color; Second = secondaryText })
        let nextState =
            { state with ListData = nextData }
        nextState, Cmd.None
    | AppExit ->
        state, Cmd.Signal QuitApplication
    
let view (state: State) =
    let exitAction =
        let seq =
            KeySequence(Key.Q, [ Control ])
        MenuAction(Attrs = [ Text "E&xit"; Shortcut seq ], OnTriggered = (fun _ -> AppExit))

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
            | _ -> Variant.Empty
        ListModelNode(dataFunc, 2, Attrs = [ Rows state.ListData ])
        
    let treeView =
        TreeView(TreeModel = listModel)
        
    let button =
        PushButton(Attrs = [ PushButton.Text "Add One" ], OnClicked = AddRow)
        
    let vbox =
        VBoxLayout(Items = [ BoxItem(treeView); BoxItem(button) ])
        
    MainWindow(
        Attrs = [ MainWindow.Title "Wooooot"; Size (640, 480) ],
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
