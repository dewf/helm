module FSharpQt.WithDialogs

open BuilderNode
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

[<AbstractClass>]
type WithDialogs<'msg>(content: IBuilderNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    abstract member RelativeToWidgetAbstract: Widget.Handle option
    default this.RelativeToWidgetAbstract = None
    
    abstract member RewriteDialogContext: BuilderContext<'msg> -> BuilderContext<'msg>
    default this.RewriteDialogContext context = context
    
    interface IContextRewriter<'msg> with
        override this.ContextFor key buildContext =
            match key with
            | StrKey "content" -> buildContext
            | _ -> this.RewriteDialogContext(buildContext)
    
    interface IBuilderNode<'msg> with
        override this.Dependencies =
            let dialogs' =
                dialogs
                |> List.map (fun (id, node) -> "dlg_" + id, node :> IBuilderNode<'msg>)
            ("content", content) :: dialogs'
            |> List.map (fun (id, node) -> StrKey id, node)
            
        override this.Create2 dispatch buildContext =
            ()
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom left depsChanges =
            ()
            
        override this.Dispose() = ()
        
        override this.ContentKey = null // sensible? the dependencies should be compared separately on their own 'merits' ...
        
    interface IDialogParent<'msg> with
        member this.RelativeToWidget = this.RelativeToWidgetAbstract
        member this.AttachedDialogs = dialogs

type WindowWithDialogs<'msg>(window: IWindowNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    inherit WithDialogs<'msg>(window, dialogs)
        
    override this.RewriteDialogContext context =
        { context with ContainingWindow = Some window }
    
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
    
    override this.RewriteDialogContext context =
        { context with ContainingWindow = Some dialog }
    
    override this.RelativeToWidgetAbstract =
        Some (dialog.Dialog :> Widget.Handle)
    
    interface IDialogNode<'msg> with
        member this.Dialog = dialog.Dialog
