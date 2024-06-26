module FSharpQt.Models.SimpleListModel

open System
open FSharpQt
open Org.Whatever.QtTesting
open MiscTypes

let emptyIndex =
    ModelIndex.Deferred.Empty()

type SimpleListModel<'row>(dataFunc: 'row -> int -> DataRole -> Variant, numColumns: int) as this =
    let mutable rows = [||]
    
    let interior =
        let methodMask =
            if numColumns > 1 then
                AbstractListModel.MethodMask.ColumnCount
            else
                enum<AbstractListModel.MethodMask> 0
        AbstractListModel
            .CreateSubclassed(this, methodMask)
            .GetInteriorHandle()
            
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
            failwith "not yet implemented"
            
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
