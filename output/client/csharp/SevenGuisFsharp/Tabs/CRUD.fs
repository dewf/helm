module Tabs.CRUD

open FSharpQt
open BuilderNode
open Reactor

open FSharpQt.Widgets
open Label
open BoxLayout
open GridLayout
open PushButton
open LineEdit
open ListWidget

open Org.Whatever.QtTesting
open Extensions

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
    
let filterNames (names: Name list) (filter: string) =
    names
    |> List.mapi (fun i name -> i, name)
    |> List.choose (fun (index, name) ->
        if name.First.ToLower().StartsWith filter || name.Last.ToLower().StartsWith filter then
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
    let filterLabel = Label(Attrs = [ Label.Text "Filter prefix:" ])
    let filterEdit =
        let value =
            match state.Mode with
            | Unfiltered -> ""
            | Filtered(filter, _) -> filter
        LineEdit(Attrs = [ Value value ], OnChanged = SetFilter)

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
        ListWidget(
            Attrs = [
                Items items
                SelectionMode Single
                CurrentRow state.SelectedIndex
            ],
            OnCurrentRowChanged = SelectItem)

    let firstLabel = Label(Attrs = [ Label.Text "First:" ])
    let firstEdit = LineEdit(Attrs = [ Value state.FirstEdit ], OnChanged = SetFirst)

    let lastLabel = Label(Attrs = [ Label.Text "Last:" ])
    let lastEdit = LineEdit(Attrs = [ Value state.LastEdit ], OnChanged = SetLast)

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
            GridItem(listBox, 1, 0, 4, 2)
            GridItem(firstLabel, 1, 2, align = Common.Alignment.Right)
            GridItem(firstEdit, 1, 3)
            GridItem(lastLabel, 2, 2, align = Common.Alignment.Right)
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
