open Gtk
open BuilderNode
open Reactor
open Util

type Msg =
    | ToggleEdit
    | EditChanged of string
    | EditActivated
    | AddButton
    | ExtraPush of int
    | ComboChanged of value: (int * string) option
    
type State = {
    EditEnabled: bool
    TextValue: string
    LastActivated: string option
    ExtraButtonCount: int
    ComboSelection: (int * string) option
}

let init() =
    { EditEnabled = true
      TextValue = ""
      LastActivated = None
      ExtraButtonCount = 0
      ComboSelection = None }

let update (state: State) (msg: Msg): State =
    match msg with
    | EditChanged str ->
        { state with TextValue = str }
    | EditActivated ->
        { state with LastActivated = Some state.TextValue }
    | ToggleEdit ->
        { state with EditEnabled = not state.EditEnabled }
    | AddButton ->
        { state with ExtraButtonCount = state.ExtraButtonCount + 1 }
    | ExtraPush i ->
        printfn "hello from extra button %02d" i
        state
    | ComboChanged maybeValue ->
        let nextTextValue =
            match maybeValue with
            | Some (index, str) ->
                sprintf "you selected %d [%s]" index str
            | None ->
                state.TextValue
        { state with ComboSelection = maybeValue; TextValue = nextTextValue }
    
let view (state: State) =
    let edit =
        LineEdit.Node(Attrs = [LineEdit.Value state.TextValue; LineEdit.Enabled state.EditEnabled], OnChanged = Some EditChanged, OnActivated = Some EditActivated)
    let disableButton =
        let label =
            if state.EditEnabled then
                "disable text box"
            else
                "enable text box"
        Button.make label ToggleEdit
    let addButton =
        let enabled =
            state.ExtraButtonCount < 5
        Button.Node(Attrs = [Button.Label "Add"; Button.Enabled enabled], OnClicked = Some AddButton)
    let extraButtons =
        if state.ExtraButtonCount > 0 then
            seq {
                for i in 1..state.ExtraButtonCount do
                    let label =
                        sprintf "extra button %02d" i
                    Button.make label (ExtraPush i) :> WidgetNode<Msg>
            } |> Seq.toList
        else
            []
    let combo =
        let items =
            ["item 01"; "item 02"; "item03"]
        ComboBox.Node(Attrs = [ComboBox.Items items; ComboBox.SelectedIndex (Some 0)], OnSelected = Some ComboChanged)
    let box =
        Box.Node(
            Attrs = [Box.Orientation Box.Vertical; Box.Spacing 10; Box.BorderWidth 20],
            Items = [ edit; disableButton; combo; addButton ] @ extraButtons)
    let title =
        match state.LastActivated with
        | Some value -> sprintf "last activation [%s]" value
        | None -> "..."
    let frame =
        Window.Node(Attrs = [Window.Title title; Window.Size (800, 600); Window.Visible true; Window.ExitOnClose true],
                    Content = box)
    frame :> BuilderNode<Msg>

[<EntryPoint>]
let main argv =
    Application.Init()
    let app = new Application("com.whatever.builder4", GLib.ApplicationFlags.None)
    app.Register(GLib.Cancellable.Current) |> ignore

    let reactor = Reactor(init, update, view)
    reactor.Run()

    0 // return an integer exit code
