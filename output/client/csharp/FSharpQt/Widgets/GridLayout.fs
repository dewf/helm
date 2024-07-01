module FSharpQt.Widgets.GridLayout

open System
open Org.Whatever.QtTesting
open FSharpQt
open BuilderNode
open MiscTypes

type Signal = unit

type Attr =
    | Spacing of spacing: int
    | ContentsMargins of left: int * top: int * right: int * bottom: int
    | RowMinimumHeight of row: int * minHeight: int
    | ColumnMinimumWidth of col: int * minWidth: int
    | RowStretch of row: int * stretch: int
    | ColumnStretch of col: int * stretch: int
    
let private keyFunc = function
    | Spacing _ -> 0
    | ContentsMargins _ -> 1
    // a little more complicated, to allow for a single Attrs with multiple row/columns:
    | RowMinimumHeight (row, _) -> (row * 4) + 2      // 2, 6, 10 ...
    | ColumnMinimumWidth (col, _) -> (col * 4) + 3    // 3, 7, 11 ...
    | RowStretch (row, _) -> (row * 4) + 4            // 4, 8, 12 ...
    | ColumnStretch (col, _) -> (col * 4) + 5         // 5, 9, 13 ...

let private diffAttrs =
    genericDiffAttrs keyFunc
    
type Location = {
    Row: int
    Col: int
    RowSpan: int option
    ColSpan: int option
    Align: Alignment option
} with
    static member Create(row: int, col: int, ?rowSpan: int, ?colSpan: int, ?align: Alignment) =
        { Row = row
          Col = col
          RowSpan = defaultArg (Some rowSpan) None
          ColSpan = defaultArg (Some colSpan) None
          Align = defaultArg (Some align) None }
    static member Default =
        { Row = 0; Col = 0; RowSpan = None; ColSpan = None; Align = None }

type internal InternalItem<'msg> =
    | WidgetItem of w: IWidgetNode<'msg> * loc: Location
    | LayoutItem of l: ILayoutNode<'msg> * loc: Location
        
type GridItem<'msg> private(item: InternalItem<'msg>) =
    member val internal Item = item
    new(w: IWidgetNode<'msg>, row: int, col: int, ?rowSpan: int, ?colSpan: int, ?align: Alignment) =
        let loc =
            { Row = row
              Col = col
              RowSpan = defaultArg (Some rowSpan) None
              ColSpan = defaultArg (Some colSpan) None
              Align = defaultArg (Some align) None }
        GridItem(WidgetItem (w, loc))
    new(l: ILayoutNode<'msg>, row: int, col: int, ?rowSpan: int, ?colSpan: int, ?align: Alignment) =
        let loc =
            { Row = row
              Col = col
              RowSpan = defaultArg (Some rowSpan) None
              ColSpan = defaultArg (Some colSpan) None
              Align = defaultArg (Some align) None }
        GridItem(LayoutItem (l, loc))
    member this.Node =
        match item with
        | WidgetItem(w, _) -> w :> IBuilderNode<'msg>
        | LayoutItem(l, _) -> l :> IBuilderNode<'msg>
    member this.Key =
        match item with
        | WidgetItem(w, loc) -> w.ContentKey, loc
        | LayoutItem(l, loc) -> l.ContentKey, loc
        
type private Method =
    | Normal
    | WithAlignment of align: Alignment
    | WithSpans of rowSpan: int * colSpan: int
    | WithSpansAlignment of rowSpan: int * colSpan: int * align: Alignment
    
let private whichMethod (loc: Location) =
    match loc.Align with
    | Some align ->
        match loc.RowSpan, loc.ColSpan with
        | None, None ->
            WithAlignment align
        | None, Some colSpan ->
            WithSpansAlignment (1, colSpan, align)
        | Some rowSpan, None ->
            WithSpansAlignment (rowSpan, 1, align)
        | Some rowSpan, Some colSpan ->
            WithSpansAlignment (rowSpan, colSpan, align)
    | None ->
        match loc.RowSpan, loc.ColSpan with
        | None, None ->
            Normal
        | None, Some colSpan ->
            WithSpans (1, colSpan)
        | Some rowSpan, None ->
            WithSpans (rowSpan, 1)
        | Some rowSpan, Some colSpan ->
            WithSpans (rowSpan, colSpan)
    
let private addItem (grid: GridLayout.Handle) = function
    | WidgetItem(w, loc) ->
        match whichMethod loc with
        | Normal ->
            grid.AddWidget(w.Widget, loc.Row, loc.Col)
        | WithAlignment align ->
            grid.AddWidget(w.Widget, loc.Row, loc.Col, align.QtValue)
        | WithSpans(rowSpan, colSpan) ->
            grid.AddWidget(w.Widget, loc.Row, loc.Col, rowSpan, colSpan)
        | WithSpansAlignment(rowSpan, colSpan, align) ->
            grid.AddWidget(w.Widget, loc.Row, loc.Col, rowSpan, colSpan, align.QtValue)
    | LayoutItem(l, loc) ->
        match whichMethod loc with
        | Normal ->
            grid.AddLayout(l.Layout, loc.Row, loc.Col)
        | WithAlignment align ->
            grid.AddLayout(l.Layout, loc.Row, loc.Col, align.QtValue)
        | WithSpans(rowSpan, colSpan) ->
            grid.AddLayout(l.Layout, loc.Row, loc.Col, rowSpan, colSpan)
        | WithSpansAlignment(rowSpan, colSpan, align) ->
            grid.AddLayout(l.Layout, loc.Row, loc.Col, rowSpan, colSpan, align.QtValue)
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    
    let mutable grid = GridLayout.Create()
    
    member this.Layout with get() = grid
    
    member this.SignalMap with set value = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Spacing spacing ->
                grid.SetSpacing(spacing)
            | ContentsMargins (left, top, right, bottom) ->
                grid.SetContentsMargins (left, top, right, bottom)
            | RowMinimumHeight (row, minHeight) ->
                grid.SetRowMinimumHeight(row, minHeight)
            | ColumnMinimumWidth (col, minWidth) ->
                grid.SetColumnMinimumWidth(col, minWidth)
            | RowStretch (row, stretch) ->
                grid.SetRowStretch(row, stretch)
            | ColumnStretch (col, stretch) ->
                grid.SetColumnStretch(col, stretch)
                
    interface IDisposable with
        member this.Dispose() =
            grid.Dispose()
            
    member this.Refill(items: GridItem<'msg> list) =
        grid.RemoveAll()
        for item in items do
            addItem grid item.Item

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

type GridLayout<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Items: GridItem<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = (fun _ -> None)
    
    member private this.MigrateContent(leftBox: GridLayout<'msg>) =
        let leftContents =
            leftBox.Items
            |> List.map (_.Key)
        let thisContents =
            this.Items
            |> List.map (_.Key)
        if leftContents <> thisContents then
            this.model.Refill(this.Items)
        else
            ()
        
    interface ILayoutNode<'msg> with
        override this.Dependencies =
            // because the indices are generated here, based on items order,
            // it prevents the possibility of the "user" (app developer) from being able to reorder existing items without them being destroyed/recreated entirely
            // but I don't think that's a very common use case, to be reordering anything in a vbox/hbox, except maybe adding things at the end (which should work fine)
            // if user-reordering was a common use case, then the user would have to provide item keys / IDs as part of the item list
            // we'll do that for example with top-level windows in the app window order, so that windows can be added/removed without forcing a rebuild of existing windows
            this.Items
            |> List.mapi (fun i item -> IntKey i, item.Node)
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            this.model.Refill(this.Items)
        
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> GridLayout<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged__old
            this.model <- migrate left'.model nextAttrs signalMap
            this.MigrateContent(left')
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Layout =
            (this.model.Layout :> Layout.Handle)
            
        override this.ContentKey =
            (this :> ILayoutNode<'msg>).Layout
            
        override this.Attachments =
            this.Attachments
