module FSharpQt.WithDialogs

open BuilderNode
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

[<AbstractClass>]
type WithDialogs<'msg>(content: IBuilderNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    let mutable maybeParent: IBuilderNode<'msg> option = None
    
    abstract member RelativeToWidgetAbstract: Widget.Handle option
    default this.RelativeToWidgetAbstract = None
    
    interface IBuilderNode<'msg> with
        override this.Dependencies =
            let dialogs' =
                dialogs
                |> List.map (fun (id, node) -> "dlg_" + id, node :> IBuilderNode<'msg>)
            // content will be realized first, which is necessary since the dialogs will be hitting .ContainingWindowWidget which routes to content (for them)
            ("content", content) :: dialogs'
            |> List.map (fun (id, node) -> StrKey id, node)
            
        override this.Create2 dispatch maybeParentParam =
            maybeParent <- maybeParentParam
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom left depsChanges =
            ()
            
        override this.Dispose() = ()
        
        override this.ContentKey = null // sensible? the dependencies should be compared separately on their own 'merits' ...
        
        override this.ContainingWindowWidget querent =
            if querent = content then
                // only the content is allowed to look upward
                maybeParent
                |> Option.map (fun node -> node.ContainingWindowWidget this)
                |> Option.flatten
            else
                // querent is either the dialogs or something beneath them (although I think we're writing these so that only 'this' is used as a querent param, so deeper nodes shouldn't be bubbled up)
                // treat them AS IF content is their immediate parent
                content.ContainingWindowWidget querent
        
    interface IDialogParent<'msg> with
        member this.RelativeToWidget = this.RelativeToWidgetAbstract
        member this.AttachedDialogs = dialogs

type WindowWithDialogs<'msg>(window: IWindowNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(window, dialogs)
    
    override this.RelativeToWidgetAbstract =
        Some window.WindowWidget
    
    interface IWindowNode<'msg> with
        member this.WindowWidget = window.WindowWidget

type LayoutWithDialogs<'msg>(layout: ILayoutNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(layout, dialogs)
    interface ILayoutNode<'msg> with
        member this.Layout = layout.Layout

type WidgetWithDialogs<'msg>(widget: IWidgetNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(widget, dialogs)
    
    override this.RelativeToWidgetAbstract =
        Some widget.Widget
    
    interface IWidgetNode<'msg> with
        member this.Widget = widget.Widget

type DialogWithDialogs<'msg>(dialog: IDialogNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(dialog, dialogs)
    
    override this.RelativeToWidgetAbstract =
        Some (dialog.Dialog :> Widget.Handle)
    
    interface IDialogNode<'msg> with
        member this.Dialog = dialog.Dialog
