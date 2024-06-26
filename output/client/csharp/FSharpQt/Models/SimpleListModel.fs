module FSharpQt.Models.SimpleListModel

open System
open FSharpQt
open Org.Whatever.QtTesting
open MiscTypes

let emptyIndex =
    ModelIndex.Deferred.Empty()

type SimpleListModel<'row>(numColumns: int) as this =
    let mutable rows = [||]
    
    let interior =
        let methodMask =
            if numColumns > 1 then
                AbstractListModel.MethodMask.HeaderData ||| AbstractListModel.MethodMask.ColumnCount
            else
                AbstractListModel.MethodMask.HeaderData
        AbstractListModel
            .CreateSubclassed(this, methodMask)
            .GetInteriorHandle()
            
    let mutable maybeHeaders: string array option = None
    let mutable dataFunc: 'row -> int -> DataRole -> Variant = (fun _ _ _ -> Variant.Empty)
    
    member this.DataFunc with set value =
        dataFunc <- value
        
    member this.Headers with set value =
        let newValue = Some (value |> List.toArray)
        if maybeHeaders <> newValue then
            maybeHeaders <- newValue
            interior.EmitHeaderDataChanged(Enums.Orientation.Horizontal, 0, numColumns - 1)
            
    member this.QtModel =
        interior :> AbstractItemModel.Handle
        
    interface AbstractListModel.MethodDelegate with
        member this.RowCount(parent: ModelIndex.Handle) =
            rows.Length
            
        member this.Data(index: ModelIndex.Handle, role: Enums.ItemDataRole) =
            let rowIndex =
                index.Row()
            let colIndex =
                index.Column()
            if index.IsValid() && rowIndex < rows.Length && colIndex < numColumns then
                let value =
                    let row =
                        rows[rowIndex]
                    dataFunc row colIndex (DataRole.From role)
                value.QtValue
            else
                Variant.Empty.QtValue
                
        // optional depending on mask: ==================================================
        
        member this.HeaderData(section: int, orientation: Enums.Orientation, role: Enums.ItemDataRole) =
            let variant =
                if role = Enums.ItemDataRole.DisplayRole then
                    match maybeHeaders with
                    | Some headers ->
                        if section < headers.Length then
                            Variant.String headers[section]
                        else
                            Variant.String ""
                    | None ->
                        Variant.Empty
                else
                    Variant.Empty
            variant.QtValue
            
        member this.GetFlags(index: ModelIndex.Handle, baseFlags: AbstractListModel.ItemFlags) =
            failwith "not yet implemented"
            
        member this.SetData(index: ModelIndex.Handle, value: Org.Whatever.QtTesting.Variant.Handle, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
        member this.ColumnCount(parent: ModelIndex.Handle) =
            numColumns
            
    interface IDisposable with
        member this.Dispose() =
            interior.Dispose()
            
    member this.AddRowAt(index: int, row: 'row) =
        interior.BeginInsertRows(emptyIndex, index, index)
        rows <- Array.insertAt index row rows
        interior.EndInsertRows()
        
    member this.AddRowsAt(index: int, newRows: 'row list) =
        interior.BeginInsertRows(emptyIndex, index, index + newRows.Length - 1)
        rows <- Array.insertManyAt index newRows rows
        interior.EndInsertRows()
