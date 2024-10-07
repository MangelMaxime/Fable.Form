module Sutil.Router

open Elmish.UrlParser
open Browser.Types

module Parser =

    open Elmish.UrlParser

    // Code copied from Fable.Elmish.Browser

    let parsePath (parser: Parser<_, _>) (location: Location) =
        parse parser location.pathname (parseParams location.search)

    let parseHash (parser: Parser<_, _>) (location: Location) =
        let hash, queryParams =
            let hash =
                if location.hash.Length > 1 then
                    location.hash.Substring 1
                else
                    ""

            let pos = hash.IndexOf "?"

            if pos >= 0 then
                let path = hash.Substring(0, pos)
                let search = hash.Substring(pos + 1)
                path, parseParams search
            else
                hash, Map.empty

        parse parser hash queryParams
