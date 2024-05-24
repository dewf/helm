module Program

open System
open BuilderNode
open Org.Whatever.QtTesting
open AppReactor
open Tabs
open Widgets

type Msg =
    | NothingYet

type State = {
    Reserved: int
}

let init () =
    let nextState = {
        Reserved = 0
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    state, Cmd.None
    
let view (state: State) =
    let tabWidget =
        TabWidget.Node(
            Pages = [
                "Counter", Counter.Node()
                "TempConv", TempConverter.Node()
            ])
    MainWindow.Node(
        Attrs = [ MainWindow.Title "7GUIs: F#/Qt edition" ],
        Content = tabWidget)
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    // initialize NativeImpl library
    Library.Init()
    // begin F#Qt proper
    let exitCode =
        // notice the 'use' inside of here, because we want it to .Dispose the AppReactor when leaving this block/binding
        use app =
            createApplication init update view
        app.Run argv
    // NativeImpl shutdown
    Library.DumpTables()
    Library.Shutdown()
    exitCode
