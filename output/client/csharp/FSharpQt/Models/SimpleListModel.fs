module FSharpQt.Models.SimpleListModel

open Org.Whatever.QtTesting

type QtModel =
    interface
        abstract member QtValue: AbstractItemModel.Handle
    end

type SimpleListModel() as this =
    let interior =
        AbstractListModel
            .CreateSubclassed(this, enum<AbstractListModel.MethodMask> 0)
            .GetInteriorHandle()
            
    interface QtModel with
        member this.QtValue = interior :> AbstractItemModel.Handle
        
    interface AbstractListModel.MethodDelegate with
        member this.RowCount(parent: ModelIndex.Handle)=
            failwith "not yet implemented"
            
        member this.Data(index: ModelIndex.Handle, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
        member this.HeaderData(section: int, orientation: Enums.Orientation, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
        member this.GetFlags(index: ModelIndex.Handle, baseFlags: AbstractListModel.ItemFlags) =
            failwith "not yet implemented"
            
        member this.SetData(index: ModelIndex.Handle, value: Variant.Handle, role: Enums.ItemDataRole) =
            failwith "not yet implemented"
            
        member this.Dispose() =
            interior.Dispose()
