module NonVisual

open BuilderNode
open Org.Whatever.QtTesting

type INonVisualNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
        // could have a QObject property, but do we really need it yet? (might be a premature restriction to require these to be QObjects)
    end
    
type WithNonVisual<'msg>(nonVisualItems: INonVisualNode<'msg> list, content: IBuilderNode<'msg>) =
    interface IBuilderNode<'msg> with
        member this.Dependencies() =
            let trayAsBuilderNodes =
                nonVisualItems
                |> List.map (fun x -> x :> IBuilderNode<'msg>)
            (content :: trayAsBuilderNodes)
            |> List.mapi (fun i node -> IntKey i, node)
        member this.Create dispatch = ()
        member this.MigrateFrom left = ()
        member this.Dispose() = ()
        member this.ContentKey = null // sensible? the dependencies should take care of themselves ...

type WidgetWithNonVisual<'msg>(nonVisualItems: INonVisualNode<'msg> list, content: IWidgetNode<'msg>) =
    inherit WithNonVisual<'msg>(nonVisualItems, content)
    interface IWidgetNode<'msg> with
        override this.Widget = content.Widget

type LayoutWithNonVisual<'msg>(nonVisualItems: INonVisualNode<'msg> list, content: ILayoutNode<'msg>) =
    inherit WithNonVisual<'msg>(nonVisualItems, content)
    let mutable maybeSyntheticParent: Widget.Handle option = None
    interface ILayoutNode<'msg> with
        override this.Layout = content.Layout
        override this.Widget =
            match maybeSyntheticParent with
            | Some widget ->
                widget
            | None ->
                let widget = Widget.Create()
                widget.SetLayout((this :> ILayoutNode<'msg>).Layout)
                maybeSyntheticParent <- Some widget
                widget

type TopLevelWithNonVisual<'msg>(nonVisualItems: INonVisualNode<'msg> list, content: ITopLevelNode<'msg>) =
    inherit WithNonVisual<'msg>(nonVisualItems, content)
    interface ITopLevelNode<'msg>
        // ITopLevel doesn't define anything ...
        
type WindowWithNonVisual<'msg>(nonVisualItems: INonVisualNode<'msg> list, content: IWindowNode<'msg>) =
    inherit WithNonVisual<'msg>(nonVisualItems, content)
    interface IWindowNode<'msg> with
        override this.WindowWidget = content.WindowWidget
