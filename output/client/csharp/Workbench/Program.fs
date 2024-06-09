module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.WithDialogs
open Reactor
open FSharpQt.Widgets

open MainWindow
open PushButton
open MessageBox

type State = {
    Nothing: int
}

let MESSAGEBOX_ID = "prompt"

type Msg =
    | ButtonClicked
    | PromptResult of button: MessageBoxButton

let init () =
    let nextState = {
        Nothing = 0
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | ButtonClicked ->
        state, Cmd.Dialog (execMessageBox MESSAGEBOX_ID PromptResult)
    | PromptResult button ->
        printfn "got dialog button: %A" button
        state, Cmd.None
    
let view (state: State) =
    let showButton =
        PushButton(Attrs = [ PushButton.Text "Nice" ], OnClicked = ButtonClicked)
    let mainWindow =
        MainWindow(Attrs = [ Title "Hello"; Size (800, 600) ], Content = showButton)
    let mb =
        MessageBox(Attrs = [ Text "Yeet"; InformativeText "Better"; Buttons [Ok; Cancel] ])
    WindowWithDialogs(mainWindow, [ MESSAGEBOX_ID, mb ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
