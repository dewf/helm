module Widgets.ListWidget

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentRowChanged of index: int option

type SelectionMode =
    | NotAllowed
    | Single
    | Extended
    
type Attr =
    | Items of items: string list
    | SelectionMode of mode: SelectionMode
    
let private attrKey = function
    | Items _ -> 0
    | SelectionMode _ -> 1

let private diffAttrs =
    genericDiffAttrs attrKey
