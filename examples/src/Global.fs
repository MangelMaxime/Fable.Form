[<AutoOpen>]
module Global

let always x _ = x

module Env =

    [<Literal>]
    let commitHash = "4deac2b86aa048545731891ec329fc4dec07fa75"

    let githubBaseUrl = sprintf "https://github.com/MangelMaxime/Warded/blob/%s/" commitHash

    let generateGithubUrl (sourceDirectory : string) (fileName : string) =
        let segments = sourceDirectory.Replace("\\", "/").Split('/')

        let relativeFilePath =
            segments
            |> Array.skipWhile (fun segment ->
                segment <> "examples"
            )
            |> String.concat "/"

        githubBaseUrl + relativeFilePath + "/" + fileName
