[<AutoOpen>]
module Globals

open Fable.Core

[<Import("*", "assert")>]
let Assert: Node.Assert.IExports = jsNative

let always x _ = x
