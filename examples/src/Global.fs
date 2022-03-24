[<AutoOpen>]
module Global

let always x _ = x

module Env =

    [<Literal>]
    let commitHash = "4cb633cc1b5177632448f19cae70483dbdc8d914"

    let githubBaseUrl = sprintf "https://github.com/MangelMaxime/Fable.Form/blob/%s/" commitHash

    let generateGithubUrl (sourceDirectory : string) (fileName : string) =
        let segments = sourceDirectory.Replace("\\", "/").Split('/')

        let relativeFilePath =
            segments
            |> Array.skipWhile (fun segment ->
                segment <> "examples"
            )
            |> String.concat "/"

        githubBaseUrl + relativeFilePath + "/" + fileName
