module Tabs.CRUD

open BuilderNode
open Org.Whatever.QtTesting
open SubReactor
open Widgets

type Attr = unit
type Signal = unit

type State = {
    Count: int
}

type Msg =
    | NothingYet

let init () =
    { Count = 0 }, Cmd.None
    
let update (state: State) (msg: Msg) =
    state, Cmd.None
        
let view (state: State) =
    let filterLabel = Label.Node(Attrs = [ Label.Text "Filter prefix:" ])
    let filterEdit = LineEdit.Node()
    let listBox = ListWidget.Node()
    let nameLabel = Label.Node(Attrs = [ Label.Text "Name:" ])
    let nameEdit = LineEdit.Node()
    let surnameLabel = Label.Node(Attrs = [ Label.Text "Surname:" ])
    let surnameEdit = LineEdit.Node()
    let createButton = PushButton.Node(Attrs = [ PushButton.Label "Create" ])
    let updateButton = PushButton.Node(Attrs = [ PushButton.Label "Update" ])
    let deleteButton = PushButton.Node(Attrs = [ PushButton.Label "Delete" ])
    let hbox =
        BoxLayout.Node(
            Attrs = [
                BoxLayout.Direction BoxLayout.Horizontal
                BoxLayout.ContentsMargins (0, 0, 0, 0)
            ],
            Items = [
                createButton
                updateButton
                deleteButton
            ])
    GridLayout.Node(
        Attrs = [
            GridLayout.ColumnMinimumWidth (3, 120)
        ],
        Items = [
            GridLayout.WidgetItem (filterLabel, GridLayout.Location.Create(0, 0))
            GridLayout.WidgetItem (filterEdit, GridLayout.Location.Create(0, 1))
            GridLayout.WidgetItem (listBox, GridLayout.Location.Create(1, 0, 4, 2))
            GridLayout.WidgetItem (nameLabel, GridLayout.Location.Create(1, 2, align = Common.Alignment.Right))
            GridLayout.WidgetItem (nameEdit, GridLayout.Location.Create(1, 3))
            GridLayout.WidgetItem (surnameLabel, GridLayout.Location.Create(2, 2, align = Common.Alignment.Right))
            GridLayout.WidgetItem (surnameEdit, GridLayout.Location.Create(2, 3))
            // buttons:
            GridLayout.LayoutItem (hbox, GridLayout.Location.Create(5, 0, 1, 4))
        ])
    :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)


