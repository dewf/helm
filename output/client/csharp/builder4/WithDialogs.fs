module WithDialogs

open BuilderNode
open Org.Whatever.QtTesting

[<AbstractClass>]
type WithDialogs<'msg>(content: IBuilderNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    abstract member CreatedWithDialogs: IDialogNode<'msg> list -> unit
    default this.CreatedWithDialogs dialogs = ()
    
    interface IBuilderNode<'msg> with
        member this.Dependencies =
            let dialogs' =
                dialogs
                |> List.map (fun (id, node) -> "dlg_" + id, node :> IBuilderNode<'msg>)
            ("content", content) :: dialogs'
            |> List.map (fun (id, node) -> StrKey id, node)
        member this.Create dispatch =
            // in some cases (eg LayoutWithDialogs) the dialogs would be attached when this node itself receives .AttachedToWindow invocation initiated from a higher level
            // however if the 'content' of this node is the top-level window (eg WindowWithDialogs), there is no ancestral window to trigger .AttachedToWindow down the hierarchy
            // so call the virtual method and let window-based subclasses (eg WindowWithDialogs) perform the attachment themselves
            this.CreatedWithDialogs (dialogs |> List.map snd)
        member this.MigrateFrom left depsChanges =
            let depsMap =
                depsChanges |> Map.ofList
            dialogs
            |> List.choose (fun (id, dlg) ->
                let key = "dlg_" + id
                match depsMap[StrKey key] with
                | Added | Swapped -> Some dlg
                | _ -> None)
            |> this.CreatedWithDialogs
        member this.Dispose() = ()
        member this.ContentKey = null // sensible? the dependencies should take care of themselves ...
        member this.AttachedToWindow window =
            content.AttachedToWindow window
            for _, dlg in dialogs do
                dlg.AttachedToWindow window
        
    interface IDialogParent<'msg> with
        member this.AttachedDialogs = dialogs

type WindowWithDialogs<'msg>(window: IWindowNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(window, dialogs)
    
    override this.CreatedWithDialogs dialogs =
        // internal/nested dialogs are taken care of by MainWindow itself (triggering .AttachedToWindow)
        // this is only for external/peer dialogs at a parallel level, that wouldn't otherwise be 'seen' by MainWindow
        for dlg in dialogs do
            dlg.AttachedToWindow window.WindowWidget
    
    interface IWindowNode<'msg> with
        member this.WindowWidget = window.WindowWidget

type LayoutWithDialogs<'msg>(layout: ILayoutNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(layout, dialogs)
    let mutable maybeSyntheticParent: Widget.Handle option = None
    interface ILayoutNode<'msg> with
        member this.Layout = layout.Layout
        member this.Widget =
            match maybeSyntheticParent with
            | Some widget ->
                widget
            | None ->
                let widget = Widget.Create()
                widget.SetLayout(layout.Layout)
                maybeSyntheticParent <- Some widget
                widget
