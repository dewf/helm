open System

open FSharpQt
open FSharpQt.BuilderNode
open Reactor
open FSharpQt.Widgets
open MainWindow


type State = unit
type Msg = unit

let init() =
    (), Cmd.None
    
let update (state: State) (msg: Msg) =
    state, Cmd.None
    
let view (state: State) =
    MainWindow(Attrs = [ Title "Woot"; Size (800, 600) ])
    :> IBuilderNode<Msg>

[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
