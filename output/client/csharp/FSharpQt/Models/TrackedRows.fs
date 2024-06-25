module FSharpQt.Models.TrackedRows

type RowChangeItem<'row> =
    | Added of index: int * row: 'row
    | RangeAdded of index: int * rows: 'row list

[<CustomEquality>]
[<NoComparison>]
type TrackedRows<'row> = {
    Step: int                           // for attr diffing
    Rows: 'row list
    RowCount: int
    Changes: RowChangeItem<'row> list
} with
    override this.Equals other =
        match other with
        | :? TrackedRows<'row> as other' ->
            this.Step = other'.Step
        | _ ->
            false
            
    override this.GetHashCode() =
        this.Step.GetHashCode() // uhhhh
        
    member this.BeginChanges() =
        { this with Step = this.Step + 1; Changes = [] } // 1 step per group of changes, I guess
        
    member this.AddRow(row: 'row) =
        let index =
            this.Rows.Length
        let nextRows =
            this.Rows @ [ row ]
        let nextCount =
            this.RowCount + 1
        let nextChanges =
            this.Changes @ [ Added (index, row) ]
        { this with Rows = nextRows; RowCount = nextCount; Changes = nextChanges }
        
    member this.AddRows(rows: 'row list) =
        let index =
            this.Rows.Length
        let nextRows =
            this.Rows @ rows
        let nextCount =
            this.RowCount + rows.Length
        let nextChanges =
            this.Changes @ [ RangeAdded(index, rows) ]
        { this with Rows = nextRows; RowCount = nextCount; Changes = nextChanges }
        
    static member Init(rows: 'row list) =
        { Step = 0; Rows = []; RowCount = 0; Changes = [] }.AddRows(rows)
