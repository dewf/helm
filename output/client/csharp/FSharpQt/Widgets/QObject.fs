module FSharpQt.Widgets.QObject

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.MiscTypes
open FSharpQt.Attrs

type private Signal =
    | Destroyed of object: QObjectProxy
    | ObjectNameChanged of name: string

type private Attr =
    | ObjectName of name: string
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attrOther ->
                this = attrOther
            | _ ->
                false
        override this.Key =
            match this with
            | ObjectName name -> "qobject:objectname"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? QObjectAttrTarget as attrTarget ->
                let qObject =
                    attrTarget.QObject
                match this with
                | ObjectName name ->
                    if attrTarget.SetObjectName(name) then
                        qObject.SetObjectName(name)
            | _ ->
                printfn "warning: QObject.Attr couldn't ApplyTo() unknown target type [%A]" target

type private SignalMapFunc<'msg>(func) =
    inherit SignalMapFuncBase<Signal,'msg>(func)

type Props<'msg>() =
    inherit PropsRoot()
    
    let mutable onDestroyed: (QObjectProxy -> 'msg) option = None
    let mutable onObjectNameChanged: (string -> 'msg) option = None
    
    member internal this.SignalMask = enum<Object.SignalMask> (int this._signalMask)
    
    member this.OnDestroyed with set value =
        onDestroyed <- Some value
        this.AddSignal(int Object.SignalMask.Destroyed)
    
    member this.OnObjectNameChanged with set value =
        onObjectNameChanged <- Some value
        this.AddSignal(int Object.SignalMask.ObjectNameChanged)

    member internal this.SignalMapList =
        let thisFunc = function
            | Destroyed obj ->
                onDestroyed
                |> Option.map (fun f -> f obj)
            | ObjectNameChanged name ->
                onObjectNameChanged
                |> Option.map (fun f -> f name)
        // if we weren't at root level (eg a Widget subclass),
        // we'd do thisFunc :: base.SignalMapFuncs
        [ SignalMapFunc(thisFunc) :> ISignalMapFunc ]

    member this.ObjectName with set value =
        this.PushAttr(ObjectName value)

type ModelCore<'msg>(dispatch: 'msg -> unit) =
    inherit ModelCoreRoot()
    
    let mutable object: Object.Handle = null
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Object.SignalMask> 0
    // binding guards
    let mutable lastObjectName = ""
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.Object
        with get() =
            object
        and set value =
            // no parent class to assign to
            object <- value
            
    member internal this.SignalMaps with set (mapFuncList: ISignalMapFunc list) =
        match mapFuncList with
        | h :: _ ->
            match h with
            | :? SignalMapFunc<'msg> as smf ->
                signalMap <- smf.Func
            | _ ->
                failwith "QObject.ModelCore.SignalMaps: wrong func type"
            // no base class to assign the rest to
            // base.SignalMaps <- etc
        | _ ->
            failwith "QObject.ModelCore: signal map assignment didn't have a head element"
            
    // no this.SignalMask, QObject is abstract (for us)
            
    interface QObjectAttrTarget with
        member this.QObject = object
        member this.SetObjectName name =
            if name <> lastObjectName then
                lastObjectName <- name
                true
            else
                false

    interface Object.SignalHandler with
        member this.Destroyed(obj: Object.Handle) =
            signalDispatch (QObjectProxy(obj) |> Destroyed)
        member this.ObjectNameChanged(name: string) =
            signalDispatch (ObjectNameChanged name)
            
    interface IDisposable with
        member this.Dispose() =
            object.Dispose()
     
            