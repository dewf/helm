module FSharpQt.Widgets.FileDialog

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentChanged of path: string
    | CurrentUrlChanged of url: string
    | DirectoryEntered of dir: string
    | DirectoryUrlEntered of url: string
    | FileSelected of file: string
    | FilesSelected of files: string list
    | FilterSelected of filter: string
    | UrlSelected of url: string
    | UrlsSelected of urls: string list

type FileMode =
    | AnyFile
    | ExistingFile
    | Directory
    | ExistingFiles
with
    member internal this.QtValue =
        match this with
        | AnyFile -> FileDialog.FileMode.AnyFile
        | ExistingFile -> FileDialog.FileMode.ExistingFile
        | Directory -> FileDialog.FileMode.Directory
        | ExistingFiles -> FileDialog.FileMode.ExistingFiles

type ViewMode =
    | Detail
    | List
with
    member internal this.QtValue =
        match this with
        | Detail -> FileDialog.ViewMode.Detail
        | List -> FileDialog.ViewMode.List
    
type AcceptMode =
    | Open
    | Save
with
    member internal this.QtValue =
        match this with
        | Open -> FileDialog.AcceptMode.Open
        | Save -> FileDialog.AcceptMode.Save
    
type Attr =
    | WindowTitle of title: string
    | AcceptMode of mode: AcceptMode
    | FileMode of mode: FileMode
    | NameFilter of filter: string
    | NameFilters of filters: string list
    | MimeTypeFilters of filters: string list
    | ViewMode of mode: ViewMode
    | DefaultSuffix of suffix: string
    | Directory of dir: string

let private keyFunc = function
    | WindowTitle _ -> 0
    | AcceptMode _ -> 1
    | FileMode _ -> 2
    | NameFilter _ -> 3
    | NameFilters _ -> 4
    | MimeTypeFilters _ -> 5
    | ViewMode _ -> 6
    | DefaultSuffix _ -> 7
    | Directory _ -> 8
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable dialog = FileDialog.Create(this)
    
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<FileDialog.SignalMask> 0
    
    let dispatcher (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Dialog with get() = dialog
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            dialog.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | WindowTitle title ->
                dialog.SetWindowTitle(title)
            | AcceptMode mode ->
                dialog.SetAcceptMode(mode.QtValue)
            | FileMode mode ->
                dialog.SetFileMode(mode.QtValue)
            | NameFilter filter ->
                dialog.SetNameFilter(filter)
            | NameFilters filters ->
                dialog.SetNameFilters(filters |> Array.ofList)
            | MimeTypeFilters filters ->
                dialog.SetMimeTypeFilters(filters |> Array.ofList)
            | ViewMode mode ->
                dialog.SetViewMode(mode.QtValue)
            | DefaultSuffix suffix ->
                dialog.SetDefaultSuffix(suffix)
            | Directory dir ->
                dialog.SetDirectory(dir)
                
    interface FileDialog.SignalHandler with
        member this.CurrentChanged path =
            dispatcher (CurrentChanged path)
        member this.CurrentUrlChanged url =
            dispatcher (CurrentUrlChanged url)
        member this.DirectoryEntered dir =
            dispatcher (DirectoryEntered dir)
        member this.DirectoryUrlEntered url =
            dispatcher (DirectoryUrlEntered url)
        member this.FileSelected file =
            dispatcher (FileSelected file)
        member this.FilesSelected selected =
            dispatcher (selected |> Array.toList |> FilesSelected)
        member this.FilterSelected filter =
            dispatcher (FilterSelected filter)
        member this.UrlSelected url =
            dispatcher (UrlSelected url)
        member this.UrlsSelected urls =
            dispatcher (urls |> Array.toList |> UrlsSelected)
            
    interface IDisposable with
        member this.Dispose() =
            dialog.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialMask: FileDialog.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- initialMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: FileDialog.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()


type FileDialog<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable signalMask = enum<FileDialog.SignalMask> 0
    
    let mutable onCurrentChanged: (string -> 'msg) option = None
    let mutable onCurrentUrlChanged: (string -> 'msg) option = None
    let mutable onDirectoryEntered: (string -> 'msg) option = None
    let mutable onDirectoryUrlEntered: (string -> 'msg) option = None
    let mutable onFileSelected: (string -> 'msg) option = None
    let mutable onFilesSelected: (string list -> 'msg) option = None
    let mutable onFilterSelected: (string -> 'msg) option = None
    let mutable onUrlSelected: (string -> 'msg) option = None
    let mutable onUrlsSelected: (string list -> 'msg) option = None
    
    member this.OnCurrentChanged with set value =
        onCurrentChanged <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.CurrentChanged
        
    member this.OnCurrentUrlChanged with set value =
        onCurrentUrlChanged <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.CurrentUrlChanged
        
    member this.OnDirectoryEntered with set value =
        onDirectoryEntered <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.DirectoryEntered
        
    member this.OnDirectoryUrlEntered with set value =
        onDirectoryUrlEntered <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.DirectoryUrlEntered
        
    member this.OnFileSelected with set value =
        onFileSelected <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.FileSelected
        
    member this.OnFilesSelected with set value =
        onFilesSelected <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.FilesSelected
        
    member this.OnFilterSelected with set value =
        onFilterSelected <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.FilterSelected
        
    member this.OnUrlSelected with set value =
        onUrlSelected <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.UrlSelected
        
    member this.OnUrlsSelected with set value =
        onUrlsSelected <- Some value
        signalMask <- signalMask ||| FileDialog.SignalMask.UrlsSelected
            
    let signalMap (signal: Signal) =
        match signal with
        | CurrentChanged path ->
            onCurrentChanged
            |> Option.map (fun f -> f path)
        | CurrentUrlChanged url ->
            onCurrentUrlChanged
            |> Option.map (fun f -> f url)
        | DirectoryEntered dir ->
            onDirectoryEntered
            |> Option.map (fun f -> f dir)
        | DirectoryUrlEntered url ->
            onDirectoryUrlEntered
            |> Option.map (fun f -> f url)
        | FileSelected file ->
            onFileSelected
            |> Option.map (fun f -> f file)
        | FilesSelected files ->
            onFilesSelected
            |> Option.map (fun f -> f files)
        | FilterSelected filter ->
            onFilterSelected
            |> Option.map (fun f -> f filter)
        | UrlSelected url ->
            onUrlSelected
            |> Option.map (fun f -> f url)
        | UrlsSelected urls ->
            onUrlsSelected
            |> Option.map (fun f -> f urls)
            
    interface IDialogNode<'msg> with
        override this.Dependencies = []
            
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> FileDialog<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Dialog =
            this.model.Dialog
            
        override this.ContentKey =
            (this :> IDialogNode<'msg>).Dialog
            
        override this.AttachedToWindow window =
            this.model.Dialog.SetParentDialogFlags(window)
