module FSharpQt.Widgets.MessageBox

open System
open FSharpQt
open BuilderNode
open Reactor
open Org.Whatever.QtTesting

type Signal = unit

type MessageBoxButton =
    | Ok
    | Save
    | SaveAll
    | Open
    | Yes
    | YesToAll
    | No
    | NoToAll
    | Abort
    | Retry
    | Ignore
    | Close
    | Cancel
    | Discard
    | Help
    | Apply
    | Reset
    | RestoreDefaults
with
    static member internal FromQtValue(raw: MessageBox.StandardButton) =
        match raw with
        // NoButton is currently not supported, it appears to be OK
        | MessageBox.StandardButton.Ok -> Ok
        | MessageBox.StandardButton.Save -> Save
        | MessageBox.StandardButton.SaveAll -> SaveAll
        | MessageBox.StandardButton.Open -> Open
        | MessageBox.StandardButton.Yes -> Yes
        | MessageBox.StandardButton.YesToAll -> YesToAll
        | MessageBox.StandardButton.No -> No
        | MessageBox.StandardButton.NoToAll -> NoToAll
        | MessageBox.StandardButton.Abort -> Abort
        | MessageBox.StandardButton.Retry -> Retry
        | MessageBox.StandardButton.Ignore -> Ignore
        | MessageBox.StandardButton.Close -> Close
        | MessageBox.StandardButton.Cancel -> Cancel
        | MessageBox.StandardButton.Discard -> Discard
        | MessageBox.StandardButton.Help -> Help
        | MessageBox.StandardButton.Apply -> Apply
        | MessageBox.StandardButton.Reset -> Reset
        | MessageBox.StandardButton.RestoreDefaults -> RestoreDefaults
        | _ -> failwithf "MessageBox.StandardButton.FromQtValue - unknown input %A" raw
        
    member internal this.QtValue =
        match this with
        | Ok -> MessageBox.StandardButton.Ok
        | Save -> MessageBox.StandardButton.Save
        | SaveAll -> MessageBox.StandardButton.SaveAll
        | Open -> MessageBox.StandardButton.Open
        | Yes -> MessageBox.StandardButton.Yes
        | YesToAll -> MessageBox.StandardButton.YesToAll
        | No -> MessageBox.StandardButton.No
        | NoToAll -> MessageBox.StandardButton.NoToAll
        | Abort -> MessageBox.StandardButton.Abort
        | Retry -> MessageBox.StandardButton.Retry
        | Ignore -> MessageBox.StandardButton.Ignore
        | Close -> MessageBox.StandardButton.Close
        | Cancel -> MessageBox.StandardButton.Cancel
        | Discard -> MessageBox.StandardButton.Discard
        | Help -> MessageBox.StandardButton.Help
        | Apply -> MessageBox.StandardButton.Apply
        | Reset -> MessageBox.StandardButton.Reset
        | RestoreDefaults -> MessageBox.StandardButton.RestoreDefaults
        
type Icon =
    | Information
    | Warning
    | Critical
    | Question
with
    member internal this.QtValue =
        match this with
        | Information -> MessageBox.Icon.Information
        | Warning -> MessageBox.Icon.Warning
        | Critical -> MessageBox.Icon.Critical
        | Question -> MessageBox.Icon.Question

type Attr =
    | Text of text: string
    | InformativeText of text: string
    | Buttons of buttons: MessageBoxButton list
    | DefaultButton of button: MessageBoxButton
    | Icon of icon: Icon
    | WindowTitle of title: string
    
let private keyFunc = function
    | Text _ -> 0
    | InformativeText _ -> 1
    | Buttons _ -> 2
    | DefaultButton _ -> 3
    | Icon _ -> 4
    | WindowTitle _ -> 5

let diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable messageBox = MessageBox.Create()
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    // no 'do' block currently since no signals
    
    member this.MessageBox with get() = messageBox
    member this.SignalMap with set value = signalMap <- value
    
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                messageBox.SetText(text)
            | InformativeText text ->
                messageBox.SetInformativeText(text)
            | Buttons buttons ->
                let mask =
                    ((enum<MessageBox.StandardButton> 0), buttons)
                    ||> List.fold (fun acc b ->
                        acc ||| b.QtValue)
                messageBox.SetStandardButtons(mask)
            | DefaultButton button ->
                messageBox.SetDefaultButton(button.QtValue)
            | Icon icon ->
                messageBox.SetIcon(icon.QtValue)
            | WindowTitle title ->
                messageBox.SetWindowTitle(title)
                
    interface IDisposable with
        member this.Dispose() =
            messageBox.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type MessageBox<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap = (fun _ -> None)
            
    interface IDialogNode<'msg> with
        override this.Dependencies = []
        
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> MessageBox<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Dialog =
            (this.model.MessageBox :> Dialog.Handle)
            
        override this.ContentKey =
            (this :> IDialogNode<'msg>).Dialog
            
        override this.AttachedToWindow window =
            this.model.MessageBox.SetParentDialogFlags(window)


// use this in Cmd.Dialog invocation, it maps the raw int return value
// this keeps us from having to declare the MessageBoxButton in Reactor which didn't feel right -
//   maybe there will be other dialog types with different meanings to the .exec() return value
let execMessageBox<'msg> (id: string) (msgFunc: MessageBoxButton -> 'msg) =
    let msgFunc2 intValue =
        intValue
        |> enum<MessageBox.StandardButton>
        |> MessageBoxButton.FromQtValue
        |> msgFunc
    id, DialogOp.ExecWithResult msgFunc2
