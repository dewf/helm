module Program

open System
open BuilderNode
open Org.Whatever.QtTesting
open AppReactor
open Tabs
open Widgets
open WithDialogs
open BoxLayout

type Msg =
    | OpenDialog
    | AcceptDialog
    | DlgStatus of string
    | ChangeWindowTitle

type State = {
    WindowTitle: string
}

let init () =
    let nextState = {
        WindowTitle = "7GUIs: F#/Qt edition"
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | OpenDialog ->
        state, Cmd.DialogOp ("testing", Exec)
    | AcceptDialog ->
        state, Cmd.DialogOp ("testing", Accept)
    | DlgStatus status ->
        printfn "dlg status: [%A]" status
        state, Cmd.None
    | ChangeWindowTitle ->
        let nextState =
            { state with WindowTitle = "changed!!!" }
        nextState, Cmd.None
    
let view (state: State) =
    let launchButton =
        PushButton.Node(Attrs = [PushButton.Label "Launch Dialog"], OnClicked = OpenDialog)
    let tabWidget =
        TabWidget.Node(
            Pages = [
                "Counter", Counter.Node()
                "TempConv", TempConverter.Node()
                "FlightBooker", FlightBooker.Node()
                "Timer", TimerPage.Node()
                "CRUD", CRUD.Node()
                "Launch", launchButton
                // "COMBO", combo
            ])
    let dialog =
        let button =
            PushButton.Node(Attrs = [ PushButton.Label "Accept Me" ], OnClicked = AcceptDialog)
        let button2 =
            PushButton.Node(Attrs = [ PushButton.Label "Change Window Title" ], OnClicked = ChangeWindowTitle)
        let layout =
            BoxLayout(
                Attrs = [ Direction Vertical ],
                Items = [
                    BoxItem.Create(button)
                    BoxItem.Create(button2)
                ])
        Dialog.Node(
            Attrs = [ Dialog.Size (300, 200) ],
            Layout = layout,
            OnAccepted = DlgStatus "accepted",
            OnRejected = DlgStatus "rejected")
    let window =
        MainWindow.Node(
            Attrs = [ MainWindow.Title state.WindowTitle ],
            Content = tabWidget)
    WindowWithDialogs(window, [ "testing", dialog ])
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
