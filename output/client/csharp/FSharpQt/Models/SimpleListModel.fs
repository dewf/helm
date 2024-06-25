module FSharpQt.Models.SimpleListModel

open System
open FSharpQt
open Org.Whatever.QtTesting
open MiscTypes

type SimpleListModel<'row>(initialRows: 'row seq, rowFunc: 'row -> DataRole -> Variant) as this =
    let mutable rows = initialRows |> Seq.toArray
    
    let interior =
        AbstractListModel
            .CreateSubclassed(this, enum<AbstractListModel.MethodMask> 0) // no extra methods yet
            .GetInteriorHandle()
            
    member this.QtModel =
        interior :> AbstractItemModel.Handle
        
    interface AbstractListModel.MethodDelegate with
        member this.RowCount(parent: ModelIndex.Handle) =
            rows.Length
            
        member this.Data(index: ModelIndex.Handle, role: Enums.ItemDataRole) =
            if index.IsValid() && index.Row() < rows.Length then
                let value =
                    let row =
                        rows[index.Row()]
                    rowFunc row (DataRole.From role)
                value.QtValue
            else
                Variant.Empty.QtValue
            
        member this.HeaderData(section: int, orientation: Enums.Orientation, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
        member this.GetFlags(index: ModelIndex.Handle, baseFlags: AbstractListModel.ItemFlags) =
            failwith "not yet implemented"
            
        member this.SetData(index: ModelIndex.Handle, value: Org.Whatever.QtTesting.Variant.Handle, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
    interface IDisposable with
        member this.Dispose() =
            interior.Dispose()
