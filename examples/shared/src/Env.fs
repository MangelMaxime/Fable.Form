[<RequireQualifiedAccess>]
module Env

[<Literal>]
let private commitHash = "master"

let private githubBaseUrl =
    sprintf "https://github.com/MangelMaxime/Fable.Form/blob/%s/" commitHash

let generateGithubUrl (sourceDirectory: string) (fileName: string) =
    let segments = sourceDirectory.Replace("\\", "/").Split('/')

    let relativeFilePath =
        segments
        |> Array.skipWhile (fun segment -> segment <> "examples")
        |> String.concat "/"

    githubBaseUrl + relativeFilePath + "/" + fileName
