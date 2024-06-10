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
    SleepComplete: bool
}

type Msg =
    | DoneSleeping of value: int
    | ButtonClicked
    | PromptResult of button: MessageBoxButton

let init () =
    let nextState = {
        SleepComplete = false
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | DoneSleeping value ->
        { state with SleepComplete = true }, Cmd.None
    | ButtonClicked ->
        let func =
            async {
                printfn "wait 1"
                do! Async.Sleep 1000
                printfn "wait 2"
                do! Async.Sleep 1000
                printfn "wait 3"
                do! Async.Sleep 1000
                printfn "wait 4"
                do! Async.Sleep 1000
                printfn "wait 5"
                do! Async.Sleep 1000
                printfn "done!"
                return 1234
            }
        state, Cmd.OfAsync (asyncPerform func DoneSleeping)
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
                let text, enabled =
                    match state.SleepComplete with
                    | true -> "Done", false
                    | false -> "Do Something", true
                PushButton(
                    Attrs = [
                        PushButton.Text text
                        Enabled enabled
                        MinWidth 200
                    ], OnClicked = ButtonClicked)
            BoxLayout(Attrs = [ Direction TopToBottom; ContentsMargins (10, 10, 10, 10) ],
                      Items = [ BoxItem.Create(quitButton) ])
        MainWindow(Attrs = [ Title "Hello" ], Content = layout)
        
    let mb =
        MessageBox(
            Attrs = [
                WindowTitle "!! Important !!"
                Text "Quit the application?"
                Buttons [Ok; Cancel]
            ])
        
    WindowWithDialogs(mainWindow, [ "prompt", mb ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
