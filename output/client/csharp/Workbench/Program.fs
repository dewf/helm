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
    | AppExit

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
    | AppExit ->
        state, Cmd.Signal QuitApplication
    
let view (state: State) =
    let action1 =
        let icon =
            Icon(ThemeIcon.CallStart)
        let seq =
            KeySequence(Key.K, [ Control ])
        MenuAction(Attrs = [ Text "Happy"; Shortcut seq; IconAttr icon ], OnTriggered = (fun _ -> ActionTriggered))
        
    let action2 =
        let icon =
            Icon(ThemeIcon.Computer)
        let seq =
            KeySequence(Key.D, [ Alt; Shift ])
        MenuAction(Attrs = [ Text "Better!"; Shortcut seq; IconAttr icon ], OnTriggered = (fun _ -> ActionTriggered))
        
    let exitAction =
        let icon =
            Icon(ThemeIcon.ApplicationExit)
        let seq =
            KeySequence(Key.Q, [ Control ])
        MenuAction(Attrs = [ Text "E&xit"; Shortcut seq; IconAttr icon ], OnTriggered = (fun _ -> AppExit))
        
    let toolBar =
        ToolBar(Items = [
            ToolBarItem(action1)
            Separator()
            ToolBarItem(action2)
            Separator()
            ToolBarItem(exitAction)
        ])
        
    let menuBar =
        let menu =
            Menu(Attrs = [ Title "&File" ], Items = [ action1; action2; exitAction ])
        MenuBar(Menus = [ menu ])
        
    MainWindow(
        Attrs = [ MainWindow.Title "Wooooot"; Size (640, 480) ],
        MenuBar = menuBar,
        ToolBar = toolBar
        // Actions = [
        //     "first", action1
        //     "second", action2
        //     "last", exitAction
        // ] // actions are already owned by MainWindow, and once installed via menu I believe they are active anyway - this doesn't hurt, but in our case it's not necessary, either
        )
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
