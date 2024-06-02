module FSharpQt.NonVisual

open BuilderNode
open Org.Whatever.QtTesting

type INonVisualNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
        // could have a QObject property, but do we really need it yet? (might be a premature restriction to require these to be QObjects)
    end
    
type WithNonVisual<'msg>(content: IBuilderNode<'msg>, nonVisualItems: (string * INonVisualNode<'msg>) list) =
    interface IBuilderNode<'msg> with
        member this.Dependencies =
            let trayAsBuilderNodes =
                nonVisualItems
                |> List.map (fun (id, node) -> $"nv_{id}", node :> IBuilderNode<'msg>)
            ("content", content) :: trayAsBuilderNodes
            |> List.map (fun (id, node) -> StrKey id, node)
        member this.Create dispatch = ()
        member this.MigrateFrom left depsChanges = ()
        member this.Dispose() = ()
        member this.ContentKey = null // sensible? the dependencies should take care of themselves ...
        member this.AttachedToWindow window =
            content.AttachedToWindow window
            for _, node in nonVisualItems do
                node.AttachedToWindow window

type WidgetWithNonVisual<'msg>(content: IWidgetNode<'msg>, nonVisualItems: (string * INonVisualNode<'msg>) list) =
    inherit WithNonVisual<'msg>(content, nonVisualItems)
    interface IWidgetNode<'msg> with
        override this.Widget = content.Widget

type LayoutWithNonVisual<'msg>(content: ILayoutNode<'msg>, nonVisualItems: (string * INonVisualNode<'msg>) list) =
    inherit WithNonVisual<'msg>(content, nonVisualItems)
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

type TopLevelWithNonVisual<'msg>(content: ITopLevelNode<'msg>, nonVisualItems: (string * INonVisualNode<'msg>) list) =
    inherit WithNonVisual<'msg>(content, nonVisualItems)
    interface ITopLevelNode<'msg>
        // ITopLevel doesn't define anything ...
        
type WindowWithNonVisual<'msg>(content: IWindowNode<'msg>, nonVisualItems: (string * INonVisualNode<'msg>) list) =
    inherit WithNonVisual<'msg>(content, nonVisualItems)
    interface IWindowNode<'msg> with
        override this.WindowWidget = content.WindowWidget
