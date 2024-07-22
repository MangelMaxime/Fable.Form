namespace EasyBuild.Utils

open System.IO
[<AutoOpen>]
module Extensions =

    type DirectoryInfo with

        member this.ReCreate() =
            if this.Exists then
                this.Delete(true)
            this.Create()
