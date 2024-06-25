module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.Models.ListModelNode
open FSharpQt.Widgets.BoxLayout
open Reactor
open InputEnums

open FSharpQt.Widgets
open MainWindow
open PushButton
open ListView

open FSharpQt.Widgets.Menus
open Menu
open MenuAction
open MenuBar

open MiscTypes
open Models.TrackedRows

type ListRow = {
    Content: string
}

type State = {
    ListData: TrackedRows<ListRow>
}

type Msg =
    | AddRow
    | AppExit

let init () =
    let initRows = TrackedRows.Init([
        { Content = "One" }
        { Content = "Two" }
        { Content = "Three" }
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
            state.ListData
                .BeginChanges()
                .AddRow({ Content = text })
        let nextState =
            { state with ListData = nextData }
        nextState, Cmd.None
    | AppExit ->
        state, Cmd.Signal QuitApplication
    
let view (state: State) =
    let exitAction =
        // let icon =
        //     Icon(ThemeIcon.ApplicationExit)
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
        let rowFunc (row: ListRow) (role: DataRole) =
            match role with
            | DisplayRole -> Variant.String row.Content
            | _ -> Variant.Empty
        ListModelNode(rowFunc, Attrs = [ Rows state.ListData ])
        
    let listView =
        ListView(ListModel = listModel)
        
    let button =
        PushButton(Attrs = [ PushButton.Text "Add One" ], OnClicked = AddRow)
        
    let vbox =
        VBoxLayout(Items = [ BoxItem(listView); BoxItem(button) ])
        
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
