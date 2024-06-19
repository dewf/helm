module Program

open System

open FSharpQt
open BuilderNode
open Reactor
open InputEnums
open MiscTypes

open FSharpQt.Widgets
open MainWindow

open FSharpQt.Widgets.Menus
open Menu
open MenuAction
open MenuBar

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
        let seq =
            KeySequenceProxy(Key.K, set [Modifier.Control])
        MenuAction(Attrs = [ Text "Happy"; Shortcut seq ], OnTriggered = (fun _ -> ActionTriggered))

    let menuBar =
        let menu =
            Menu(Attrs = [ Title "&File" ], Items = [ action ])
        MenuBar(Menus = [ menu ])
        
    MainWindow(
        Attrs = [ MainWindow.Title "Wooooot"; Size (640, 480) ],
        MenuBar = menuBar,
        Actions = [ "action01", action ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
