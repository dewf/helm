module Program

open System

open FSharpQt
open BuilderNode
open Reactor

open MiscTypes
open FSharpQt.Widgets
open Dialog
open MainWindow
open PushButton
open BoxLayout
open PlainTextEdit

type State = {
    NothingYet: int
}

type Msg =
    | DoSomething
    | ShowNested
    | FetchText

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None
    
// unfortunately we need to be able to call some methods on PlainTextEdit(), for example to get the current text value (which isn't sent along with signals, currently)
// since it's not state-dependent (ie, its existence in our view is unconditional), we can just bind it at global level
let editProxy = PlainTextEditProxy()
    
let update (state: State) (msg: Msg) =
    match msg with
    | DoSomething ->
        state, Cmd.Dialog ("dialog1", Show)
    | ShowNested ->
        state, Cmd.Dialog ("nested", Show)
    | FetchText ->
        printfn "text was: %s" (editProxy.ToPlainText())
        state, Cmd.None
    
let view (state: State) =
    let edit =
        PlainTextEdit(MethodProxy = editProxy)
        
    let button =
        PushButton(Attrs = [ Text "ButtonText" ], OnClicked = DoSomething)
        
    let fetchButton =
        PushButton(Attrs = [ Text "Fetch Value" ], OnClicked = FetchText)

    let layout =
        VBoxLayout(Items = [
            BoxItem(edit, stretch = 1)
            BoxItem(button)
            BoxItem(fetchButton)
        ])

    let dialog =
        let nested =
            Dialog(Attrs = [ Dialog.Title "Smaller!"; Dialog.Size (320, 240) ])
        let layout =
            let button =
                PushButton(Attrs = [ Text "Lauch Nested" ], OnClicked = ShowNested)
            VBoxLayout(Items = [ BoxItem(button) ])
        Dialog(Attrs = [ Dialog.Title "Awhooo"; Dialog.Size (640, 480) ], Layout = layout, Attachments = [ "nested", Attachment.Dialog nested ])

    MainWindow(Attrs = [ Title "Wooooot"; Size (640, 480) ], Content = layout, Attachments = [ "dialog1", Attachment.Dialog dialog ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
