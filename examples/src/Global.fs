module Env

[<Literal>]
let gitTagName = ""

let githubBaseUrl = sprintf "https://github.com/MangelMaxime/Warded/blob/%s/" gitTagName

let generateGithubUrl (sourceDirectory : string) (fileName : string) =
    let segments = sourceDirectory.Replace("\\", "/").Split('/')

    let relativeFilePath =
        segments
        |> Array.skipWhile (fun segment ->
            segment <> "examples"
        )
        |> String.concat "/"

    githubBaseUrl + relativeFilePath + "/" + fileName
