module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.InputEnums
open FSharpQt.MiscTypes
open Reactor

open FSharpQt.Widgets
open MainWindow

open FSharpQt.Widgets.Menus
open MenuAction

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
        
    MainWindow(Attrs = [ Title "Wooooot"; Size (640, 480) ], Actions = [ "action01", action ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
