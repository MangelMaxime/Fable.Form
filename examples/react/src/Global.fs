[<AutoOpen>]
module Global

let always x _ = x

module Env =

    [<Literal>]
    let commitHash = "master"

    let githubBaseUrl =
        sprintf "https://github.com/MangelMaxime/Fable.Form/blob/%s/" commitHash

    let generateGithubUrl (sourceDirectory: string) (fileName: string) =
        let segments = sourceDirectory.Replace("\\", "/").Split('/')

        let relativeFilePath =
            segments
            |> Array.skipWhile (fun segment -> segment <> "examples")
            |> String.concat "/"

        githubBaseUrl + relativeFilePath + "/" + fileName
