module Cmd

open Fable.Core
open Elmish

module OfPromise =

    let exec (func : JS.Promise<'Result>) ofSuccess ofError : Cmd<'Msg> =
        let bind dispatch =
            func
                .``then``(ofSuccess >> dispatch)
                .catch(ofError >> dispatch)
                |> ignore

        [ bind ]
