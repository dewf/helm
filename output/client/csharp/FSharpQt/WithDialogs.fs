module FSharpQt.WithDialogs

open BuilderNode
open Org.Whatever.QtTesting

[<AbstractClass>]
type WithDialogs<'msg>(content: IBuilderNode<'msg>, dialogs: (string * IDialogNode<'msg>) list) =
    abstract member RelativeToWidgetAbstract: Widget.Handle option
    default this.RelativeToWidgetAbstract = None
    
    interface IBuilderNode<'msg> with
        override this.Dependencies =
            let dialogs' =
                dialogs
                |> List.map (fun (id, node) -> "dlg_" + id, node :> IBuilderNode<'msg>)
            ("content", content) :: dialogs'
            |> List.map (fun (id, node) -> StrKey id, node)
            
        override this.Create2 dispatch maybeParent =
            ()
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom left depsChanges =
            ()
            
        override this.Dispose() = ()
        
        override this.ContentKey = null // sensible? the dependencies should be compared separately on their own 'merits' ...
        
        override this.ContainingWindowWidget =
            // since this is a roundabout way of parenting dialogs ...
            content.ContainingWindowWidget
        
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
