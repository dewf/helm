module Program

open System
open BuilderNode
open Org.Whatever.QtTesting
open AppReactor
open Widgets
open WithDialogs
open BoxLayout
open PushButton
open Dialog
open MainWindow
open TabWidget
open Tabs.Counter
open Tabs.TempConverter
open Tabs.FlightBooker
open Tabs.TimerPage
open Tabs.CRUD

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
        PushButton(Attrs = [Text "Launch Dialog"], OnClicked = OpenDialog)
    let tabWidget =
        TabWidget(
            Pages = [
                "Counter", Counter()
                "TempConv", TempConverter()
                "FlightBooker", FlightBooker()
                "Timer", TimerPage()
                "CRUD", CRUDPage()
                "Launch", launchButton
                // "COMBO", combo
            ])
    let dialog =
        let button =
            PushButton(Attrs = [ Text "Accept Me" ], OnClicked = AcceptDialog)
        let button2 =
            PushButton(Attrs = [ Text "Change Window Title" ], OnClicked = ChangeWindowTitle)
        let layout =
            BoxLayout(
                Attrs = [ Direction Vertical ],
                Items = [
                    BoxItem.Create(button)
                    BoxItem.Create(button2)
                ])
        Dialog(
            Attrs = [ Dialog.Size (300, 200) ],
            Layout = layout,
            OnAccepted = DlgStatus "accepted",
            OnRejected = DlgStatus "rejected")
    let window =
        MainWindow(
            Attrs = [ Title state.WindowTitle ],
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
