module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.Widgets.BoxLayout
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
    | LaunchDialog
    | PromptResult of button: MessageBoxButton

let init () =
    let nextState = {
        Nothing = 0
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | LaunchDialog ->
        state, Cmd.Dialog (execMessageBox MESSAGEBOX_ID PromptResult)
    | PromptResult button ->
        let cmd =
            match button with
            | Ok -> Cmd.Signal QuitApplication
            | _ -> Cmd.None
        state, cmd
    
let view (state: State) =
    let mainWindow =
        let layout =
            let quitButton =
                PushButton(
                    Attrs = [
                        PushButton.Text "Quit App"
                        MinWidth 200
                    ], OnClicked = LaunchDialog)
            BoxLayout(Attrs = [ Direction TopToBottom; ContentsMargins (10, 10, 10, 10) ],
                      Items = [ BoxItem.Create(quitButton) ])
        MainWindow(Attrs = [ Title "Hello" ], Content = layout)
        
    let mb =
        MessageBox(
            Attrs = [
                Text "Quit the application?"
                Buttons [Ok; Cancel]
                WindowTitle "!! Important !!"
            ])
        
    WindowWithDialogs(mainWindow, [ MESSAGEBOX_ID, mb ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
