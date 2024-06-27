module Tabs.CRUD

open FSharpQt
open BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Models.TrackedRows
open Reactor

open FSharpQt.Widgets
open Label
open BoxLayout
open GridLayout
open PushButton
open LineEdit

open Models.ListModelNode
open Models.SortFilterProxyModel
open TreeView

type Attr = unit
type Signal = unit

type Name = {
    First: string
    Last: string
}

type State = {
    Names: TrackedRows<Name>
    FilterPattern: string
    SelectedIndex: int option
    FirstEdit: string
    LastEdit: string
}

type Msg =
    | SetFilter of filter: string
    | SelectItem of modelIndex: ModelIndexProxy
    | SetFirst of string
    | SetLast of string
    | Create
    | Update
    | Delete

let init () =
    let state = {
        Names = TrackedRows.Init([
            { First = "Hans"; Last = "Emil" }
            { First = "Max"; Last = "MusterMann" }
            { First = "Roman"; Last = "Tisch" }             
        ])
        FilterPattern = ""
        SelectedIndex = None
        FirstEdit = ""
        LastEdit = ""
    }
    state, Cmd.None
    
// because we need to invoke .MapToSource to convert the selected proxy indices, to something we can use
// maybe these should be called "bindings" instead - AbstactProxyModelBinding in this case
let modelProxy =
    AbstractProxyModelProxy()
    
let update (state: State) (msg: Msg) =
    match msg with
    | SetFilter filter ->
        let nextState =
            { state with
                FilterPattern = filter
                FirstEdit = ""
                LastEdit = "" }
        nextState, Cmd.None
    | SelectItem index ->
        // note 'converted' would be safely GC'ed even if we didn't know it was disposable
        use converted =
            modelProxy.MapToSource(index)
        let selectedIndex, nextFirstEdit, nextLastEdit =
            if converted.IsValid then
                state.Names.Rows
                |> List.item converted.Row
                |> (fun name -> Some converted.Row, name.First, name.Last)
            else
                None, "", ""
        let nextState =
            { state with
                FirstEdit = nextFirstEdit
                LastEdit = nextLastEdit
                SelectedIndex = selectedIndex }
        nextState, Cmd.None
    | SetFirst text ->
        { state with FirstEdit = text }, Cmd.None
    | SetLast text ->
        { state with LastEdit = text }, Cmd.None
    | Create ->
        let nextState =
            if state.FirstEdit.Length > 0 && state.LastEdit.Length > 0 then
                let name = { First = state.FirstEdit; Last = state.LastEdit }
                let nextNames =
                    state.Names
                        .BeginChanges()
                        .AddRow(name)
                { state with
                    Names = nextNames
                    FirstEdit = ""
                    LastEdit = ""
                    SelectedIndex = None }
            else
                state
        nextState, Cmd.None
    | Update ->
        let nextState =
            match state.SelectedIndex, state.FirstEdit.Length, state.LastEdit.Length with
            | Some index, firstLen, lastLen when firstLen > 0 && lastLen > 0 ->
                if index < state.Names.Rows.Length then
                    let nextNames =
                        state.Names
                            .BeginChanges()
                            .ReplaceAtIndex(index, { First = state.FirstEdit; Last = state.LastEdit })
                    { state with Names = nextNames; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
                else
                    state
            | _ ->
                state
        nextState, Cmd.None
    | Delete ->
        state, Cmd.None
        // let nextState =
        //     match state.SelectedIndex with
        //     | Some index ->
        //         match state.Mode with
        //         | Unfiltered ->
        //             if index < state.AllNames.Length then
        //                 let nextNames =
        //                     state.AllNames
        //                     |> List.removeAt index
        //                 { state with AllNames = nextNames; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
        //             else
        //                 state
        //         | Filtered(filter, filteredNames) ->
        //             if index < filteredNames.Length then
        //                 let originalIndex =
        //                     filteredNames
        //                     |> List.item index
        //                     |> (_.OriginalIndex)
        //                 let nextNames =
        //                     state.AllNames
        //                     |> List.removeAt originalIndex
        //                 let nextMode =
        //                     Filtered(filter, filterNames nextNames filter)
        //                 { state with AllNames = nextNames; Mode = nextMode; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
        //             else
        //                 state
        //     | None ->
        //         state
        // nextState, Cmd.None
        
let view (state: State) =
    let filterLabel = Label(Attrs = [ Label.Text "Filter prefix:" ])
    let filterEdit =
        LineEdit(Attrs = [ Value state.FilterPattern ], OnTextChanged = SetFilter)

    let model =
        let dataFunc row col role =
            match col, role with
            | 0, DisplayRole -> Variant.String row.Last
            | 1, DisplayRole -> Variant.String row.First
            | _ -> Variant.Empty
        ListModelNode(dataFunc, 2, Attrs = [ Rows state.Names; Headers [ "Last"; "First" ] ])
        
    let filterModel =
        let regex =
            match state.FilterPattern with
            | "" -> Regex()
            | value -> Regex(value, [ RegexOption.CaseInsensitive ])
        SortFilterProxyModel(SourceModel = model, Attrs = [ FilterRegex regex; FilterKeyColumn None ], MethodProxy = modelProxy)
        
    let treeView =
        TreeView(Attrs = [ SortingEnabled true ], TreeModel = filterModel, OnClicked = SelectItem)

    let firstLabel = Label(Attrs = [ Label.Text "First:" ])
    let firstEdit = LineEdit(Attrs = [ Value state.FirstEdit ], OnTextChanged = SetFirst)

    let lastLabel = Label(Attrs = [ Label.Text "Last:" ])
    let lastEdit = LineEdit(Attrs = [ Value state.LastEdit ], OnTextChanged = SetLast)

    let createButton =
        let enabled =
            state.FirstEdit.Length > 0 && state.LastEdit.Length > 0
        PushButton(Attrs = [ Text "Create"; PushButton.Enabled enabled ], OnClicked = Create)

    let updateButton =
        let enabled =
            match state.SelectedIndex, state.FirstEdit.Length, state.LastEdit.Length with
            | Some _, firstLen, lastLen when firstLen > 0 && lastLen > 0 -> true
            | _ -> false
        PushButton(Attrs = [ Text "Update"; PushButton.Enabled enabled ], OnClicked = Update)
        
    let deleteButton =
        let enabled =
            match state.SelectedIndex with
            | Some _ -> true
            | None -> false
        PushButton(Attrs = [ Text "Delete"; PushButton.Enabled enabled ], OnClicked = Delete)
    
    GridLayout(
        Attrs = [
            ColumnMinimumWidth (3, 120)
        ],
        Items = [
            GridItem(filterLabel, 0, 0)
            GridItem(filterEdit, 0, 1)
            GridItem(treeView, 1, 0, 4, 2)
            GridItem(firstLabel, 1, 2, align = Alignment.Right)
            GridItem(firstEdit, 1, 3)
            GridItem(lastLabel, 2, 2, align = Alignment.Right)
            GridItem(lastEdit, 2, 3)
            // buttons:
            let hbox =
                HBoxLayout(
                    Attrs = [ BoxLayout.ContentsMargins (0, 0, 0, 0) ],
                    Items = [
                        BoxItem(createButton, stretch = 1)
                        BoxItem(updateButton, stretch = 1)
                        BoxItem(deleteButton, stretch = 1)
                        BoxItem(stretch = 1)
                    ])
            GridItem(hbox, 5, 0, 1, 4)
        ])
    :> ILayoutNode<Msg>

type CRUDPage<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
