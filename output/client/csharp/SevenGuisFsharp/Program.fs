﻿module Program

open System

open FSharpQt
open BuilderNode
open AppReactor
open Tabs.CircleDrawer
open FSharpQt.Widgets
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

open Org.Whatever.QtTesting

type Msg =
    | OpenDialog
    | AcceptDialog
    | DialogClosed of accepted: bool
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
    | DialogClosed accepted ->
        printfn "dlg accepted: [%A]" accepted
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
                "CircleDrawer", CircleDrawer()
                "Launch", launchButton
            ])
    let dialog =
        let button =
            PushButton(Attrs = [ Text "Accept Me" ], OnClicked = AcceptDialog)
        let button2 =
            PushButton(Attrs = [ Text "Change Window Title" ], OnClicked = ChangeWindowTitle)
        let layout =
            BoxLayout(
                Attrs = [ Direction TopToBottom ],
                Items = [
                    BoxItem.Create(button)
                    BoxItem.Create(button2)
                ])
        Dialog(
            Attrs = [ Dialog.Size (300, 200) ],
            Layout = layout,
            OnClosed = DialogClosed)
    let window =
        MainWindow(
            Attrs = [ Title state.WindowTitle; Size (800, 600) ],
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