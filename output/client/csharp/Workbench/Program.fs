module Program

open System

open FSharpQt
open BuilderNode
open Reactor
open InputEnums

open FSharpQt.Widgets
open MainWindow
open ToolBar

open FSharpQt.Widgets.Menus
open Menu
open MenuAction
open MenuBar

open MiscTypes

type State = {
    NothingYet: int
}

type Msg =
    | ActionTriggered

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | ActionTriggered ->
        printfn "wooot!"
        state, Cmd.None
    
let view (state: State) =
    let action =
        let icon =
            Icon(ThemeIcon.CallStart)
        let seq =
            KeySequence(Key.K, set [Modifier.Control])
        MenuAction(Attrs = [ Text "Happy"; Shortcut seq; IconAttr icon ], OnTriggered = (fun _ -> ActionTriggered))
        
    let toolBar =
        // crashes when we have an action in here
        // so ... is an action in a toolbar (which would be disposed before it) a problem?
        // once again we're back to:
        //   we REALLY need to allow top-level windows to be responsible for all disposal under them, period
        //   the real problem was stuff slipping through and not being destroyed, right?
        ToolBar(Items = [
            // ToolBarItem(action)
        ])
        
    let menuBar =
        let menu =
            Menu(Attrs = [ Title "&File" ], Items = [ action ]) // 
        MenuBar(Menus = [ menu ])
        
    MainWindow(
        Attrs = [ MainWindow.Title "Wooooot"; Size (640, 480) ],
        MenuBar = menuBar,
        ToolBar = toolBar,
        Actions = [ "action01", action ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
