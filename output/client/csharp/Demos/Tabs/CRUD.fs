module Tabs.CRUD

open BuilderNode
open Org.Whatever.QtTesting
open SubReactor
open Widgets
open BoxLayout

type Attr = unit
type Signal = unit

type Name = {
    First: string
    Last: string
}

type FilteredName = {
    OriginalIndex: int
    Name: Name
}

type Mode =
    | Unfiltered
    | Filtered of filter: string * names: FilteredName list

type State = {
    AllNames: Name list
    Mode: Mode
    SelectedIndex: int option
    FirstEdit: string
    LastEdit: string
}

type Msg =
    | SetFilter of filter: string
    | SelectItem of maybeIndex: int option
    | SetFirst of string
    | SetLast of string
    | Create
    | Update
    | Delete

let init () =
    let state = {
        AllNames = [
            { First = "Hans"; Last = "Emil" }
            { First = "Max"; Last = "MusterMann" }
            { First = "Roman"; Last = "Tisch" } ]
        Mode = Unfiltered
        SelectedIndex = None
        FirstEdit = ""
        LastEdit = ""
    }
    state, Cmd.None
    
    
module List =
    let replaceAtIndex (index: int) (replaceFunc: 'a -> 'a) (xs: 'a list) =
        let before, after =
            List.splitAt index xs
        match after with
        | h :: etc ->
            before @ replaceFunc h :: etc
        | _ ->
            failwith "replaceAtIndex fail"
            
            
let filterNames (names: Name list) (filter: string) =
    names
    |> List.mapi (fun i name -> i, name)
    |> List.choose (fun (index, name) ->
        if name.First.ToLowerInvariant().StartsWith filter || name.Last.ToLowerInvariant().StartsWith filter then
            Some { OriginalIndex = index; Name = name }
        else
            None)
    
let update (state: State) (msg: Msg) =
    match msg with
    | SetFilter filter ->
        let nextState =
            if filter = "" then
                { state with Mode = Unfiltered; SelectedIndex = None; FirstEdit = ""; LastEdit = "" }
            else
                { state with Mode = Filtered (filter, filterNames state.AllNames filter); SelectedIndex = None; FirstEdit = ""; LastEdit = "" }
        nextState, Cmd.None
    | SelectItem maybeIndex ->
        let nextFirstEdit, nextLastEdit =
            match state.Mode with
            | Unfiltered ->
                match maybeIndex with
                | Some index ->
                    state.AllNames
                    |> List.item index
                    |> (fun name -> name.First, name.Last)
                | None ->
                    "", ""
            | Filtered(_, filteredNames) ->
                match maybeIndex with
                | Some index ->
                    filteredNames
                    |> List.item index
                    |> (fun fname -> fname.Name.First, fname.Name.Last)
                | None ->
                    "", ""
        { state with
            FirstEdit = nextFirstEdit
            LastEdit = nextLastEdit 
            SelectedIndex = maybeIndex }, Cmd.None
    | SetFirst text ->
        { state with FirstEdit = text }, Cmd.None
    | SetLast text ->
        { state with LastEdit = text }, Cmd.None
    | Create ->
        let nextState =
            if state.FirstEdit.Length > 0 && state.LastEdit.Length > 0 then
                let name = { First = state.FirstEdit; Last = state.LastEdit }
                let nextNames =
                    state.AllNames @ [ name ]
                let nextMode =
                    match state.Mode with
                    | Unfiltered -> Unfiltered
                    | Filtered(filter, _) ->
                        Filtered(filter, filterNames nextNames filter)
                { state with
                    Mode = nextMode
                    AllNames = nextNames
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
                match state.Mode with
                | Unfiltered ->
                    if index < state.AllNames.Length then
                        let nextNames =
                            state.AllNames
                            |> List.replaceAtIndex index (fun _ -> { First = state.FirstEdit; Last = state.LastEdit })
                        { state with AllNames = nextNames; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
                    else
                        state
                | Filtered(filter, filteredNames) ->
                    if index < filteredNames.Length then
                        let originalIndex =
                            filteredNames
                            |> List.item index
                            |> (_.OriginalIndex)
                        let nextNames =
                            state.AllNames
                            |> List.replaceAtIndex originalIndex (fun _ -> { First = state.FirstEdit; Last = state.LastEdit })
                        let nextMode =
                            Filtered(filter, filterNames nextNames filter)
                        { state with AllNames = nextNames; Mode = nextMode; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
                    else
                        state
            | _ ->
                state
        nextState, Cmd.None
    | Delete ->
        let nextState =
            match state.SelectedIndex with
            | Some index ->
                match state.Mode with
                | Unfiltered ->
                    if index < state.AllNames.Length then
                        let nextNames =
                            state.AllNames
                            |> List.removeAt index
                        { state with AllNames = nextNames; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
                    else
                        state
                | Filtered(filter, filteredNames) ->
                    if index < filteredNames.Length then
                        let originalIndex =
                            filteredNames
                            |> List.item index
                            |> (_.OriginalIndex)
                        let nextNames =
                            state.AllNames
                            |> List.removeAt originalIndex
                        let nextMode =
                            Filtered(filter, filterNames nextNames filter)
                        { state with AllNames = nextNames; Mode = nextMode; FirstEdit = ""; LastEdit = ""; SelectedIndex = None }
                    else
                        state
            | None ->
                state
        nextState, Cmd.None
        
let view (state: State) =
    let filterLabel = Label.Node(Attrs = [ Label.Text "Filter prefix:" ])
    let filterEdit =
        let value =
            match state.Mode with
            | Unfiltered -> ""
            | Filtered(filter, _) -> filter
        LineEdit.Node(Attrs = [ LineEdit.Value value ], OnChanged = SetFilter)

    let items =
        match state.Mode with
        | Unfiltered ->
            state.AllNames
            |> List.map (fun name -> sprintf "%s, %s" name.Last name.First)
        | Filtered(_, names) ->
            names
            |> List.map (fun fname ->
                let name = fname.Name
                sprintf "%s, %s" name.Last name.First)
    let listBox =
        ListWidget.Node(
            Attrs = [
                ListWidget.Items items
                ListWidget.SelectionMode ListWidget.Single
                ListWidget.CurrentRow state.SelectedIndex
            ],
            OnCurrentRowChanged = SelectItem)

    let firstLabel = Label.Node(Attrs = [ Label.Text "First:" ])
    let firstEdit = LineEdit.Node(Attrs = [ LineEdit.Value state.FirstEdit ], OnChanged = SetFirst)

    let lastLabel = Label.Node(Attrs = [ Label.Text "Last:" ])
    let lastEdit = LineEdit.Node(Attrs = [ LineEdit.Value state.LastEdit ], OnChanged = SetLast)

    let createButton =
        let enabled =
            state.FirstEdit.Length > 0 && state.LastEdit.Length > 0
        PushButton.Node(Attrs = [ PushButton.Label "Create"; PushButton.Enabled enabled ], OnClicked = Create)

    let updateButton =
        let enabled =
            match state.SelectedIndex, state.FirstEdit.Length, state.LastEdit.Length with
            | Some _, firstLen, lastLen when firstLen > 0 && lastLen > 0 -> true
            | _ -> false
        PushButton.Node(Attrs = [ PushButton.Label "Update"; PushButton.Enabled enabled ], OnClicked = Update)
        
    let deleteButton =
        let enabled =
            match state.SelectedIndex with
            | Some _ -> true
            | None -> false
        PushButton.Node(Attrs = [ PushButton.Label "Delete"; PushButton.Enabled enabled ], OnClicked = Delete)
    
    GridLayout.Node(
        Attrs = [
            GridLayout.ColumnMinimumWidth (3, 120)
        ],
        Items = [
            GridLayout.WidgetItem (filterLabel, GridLayout.Location.Create(0, 0))
            GridLayout.WidgetItem (filterEdit, GridLayout.Location.Create(0, 1))
            GridLayout.WidgetItem (listBox, GridLayout.Location.Create(1, 0, 4, 2))
            GridLayout.WidgetItem (firstLabel, GridLayout.Location.Create(1, 2, align = Common.Alignment.Right))
            GridLayout.WidgetItem (firstEdit, GridLayout.Location.Create(1, 3))
            GridLayout.WidgetItem (lastLabel, GridLayout.Location.Create(2, 2, align = Common.Alignment.Right))
            GridLayout.WidgetItem (lastEdit, GridLayout.Location.Create(2, 3))
            // buttons:
            let hbox =
                BoxLayout(
                    Attrs = [
                        Direction Horizontal
                        ContentsMargins (0, 0, 0, 0)
                    ],
                    Items = [
                        BoxItem.Create(createButton, stretch = 1)
                        BoxItem.Create(updateButton, stretch = 1)
                        BoxItem.Create(deleteButton, stretch = 1)
                        BoxItem.Stretch 1
                    ])
            GridLayout.LayoutItem (hbox, GridLayout.Location.Create(5, 0, 1, 4))
        ])
    :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
