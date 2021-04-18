module ConsoleTracer

open System
open Fable.Core

// Use it when initializing your Elmish app like this
//|> Program.withTrace ConsoleTracer.withTrace

let getMsgNameAndFields (t: Type) (x: 'Msg): string * obj =
    let rec getCaseName (t: Type) (acc: string list) (x: obj) =
        let caseName = Reflection.getCaseName x
        let uci =
            FSharp.Reflection.FSharpType.GetUnionCases(t)
            |> Array.find (fun uci -> uci.Name = caseName)

        let acc = (Reflection.getCaseName x)::acc
        let fields = Reflection.getCaseFields x
        if fields.Length = 1 && Reflection.isUnion fields.[0] then
            getCaseName (uci.GetFields().[0].PropertyType) acc fields.[0]
        else
            // Case names are intentionally left reverted so we see
            // the most meaningful message first
            let msgName = acc |> String.concat "/"
            let fields =
                (uci.GetFields(), fields)
                ||> Array.zip
                |> Array.map (fun (fi, v) -> fi.Name, v)
                |> JsInterop.createObj
            msgName, fields

    if Reflection.isUnion x then
        getCaseName t [] x
    else
        "Msg", box x

let inline withTrace (msg: 'Msg) (_state: 'State) =
    let msg, fields = getMsgNameAndFields typeof<'Msg> msg
    JS.console.log(msg, fields)
