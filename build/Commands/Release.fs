module EasyBuild.Commands.Release

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System
open System.IO
open System.Text.RegularExpressions
open System.ComponentModel
open Thoth.Json.Newtonsoft
open EasyBuild.CommitParser
open EasyBuild.CommitParser.Types
open Semver
open LibGit2Sharp
open EasyBuild.Workspace
open System.Linq
open EasyBuild.Utils.Dotnet

let private capitalizeFirstLetter (text: string) =
    (string text.[0]).ToUpper() + text.[1..]

[<RequireQualifiedAccess>]
type Project =
    | FableForm
    | FableFormSimple
    | FableFormSimpleBulma
    | FableFormSimpleBulmaNpm
    | All

type ProjectTypeConverter() =
    inherit TypeConverter()

    override _.ConvertFrom(_: ITypeDescriptorContext, _, value: obj) =
        match value with
        | :? string as text ->
            match text with
            | "Fable.Form" -> Project.FableForm
            | "Fable.Form.Simple" -> Project.FableFormSimple
            | "Fable.Form.Simple.Bulma" -> Project.FableFormSimpleBulma
            | "fable-form-simple-bulma" -> Project.FableFormSimpleBulmaNpm
            | "all"
            | "All" -> Project.All
            | _ -> raise <| System.InvalidOperationException("Invalid project name")

        | _ -> raise <| System.InvalidOperationException("Invalid project name")

type ReleaseSettings() =
    inherit CommandSettings()

    [<CommandOption("--major")>]
    member val BumpMajor = false with get, set

    [<CommandOption("--minor")>]
    member val BumpMinor = false with get, set

    [<CommandOption("--patch")>]
    member val BumpPatch = false with get, set

    [<CommandOption("--version")>]
    member val Version = None with get, set

    [<CommandOption("--project")>]
    [<TypeConverter(typeof<ProjectTypeConverter>)>]
    [<Description("""Name of the project to release

Possible values:
- Fable.Form
- Fable.Form.Simple
- Fable.Form.Simple.Bulma
- fable-form-simple-bulma

If not specified, all projects will be released.
    """)>]
    member val Project = Project.All with get, set

    [<CommandOption("--allow-dirty")>]
    [<Description("Allow to run in a dirty repository (having not commit changes in your reporitory)")>]
    member val AllowDirty: bool = false with get, set

    [<CommandOption("--allow-branch <VALUES>")>]
    [<Description("List of branches that are allowed to be used to generate the changelog. Default is 'main'")>]
    member val AllowBranch: string array =
        [|
            "main"
        |] with get, set

[<NoComparison>]
type ReleaseContext =
    {
        NewVersion: SemVersion
        ReleaseCommits:
            {|
                OriginalCommit: Commit
                SemanticCommit: CommitMessage
            |} seq
        ChangelogContent: string[]
        LastCommitSha: string
    }

let private getReleaseContext
    (repository: Repository)
    (settings: ReleaseSettings)
    (changelogPath: string)
    =
    let changelogContent =
        File.ReadAllText(changelogPath).Replace("\r\n", "\n").Split('\n')

    let changelogConfigSection =
        changelogContent
        |> Array.skipWhile (fun line -> "<!-- EasyBuild: START -->" <> line)
        |> Array.takeWhile (fun line -> "<!-- EasyBuild: END -->" <> line)

    let lastReleasedCommit =
        let regex = Regex("^<!-- last_commit_released:\s(?'hash'\w*) -->$")

        changelogConfigSection
        |> Array.tryPick (fun line ->
            let m = regex.Match(line)

            if m.Success then
                Some m.Groups.["hash"].Value
            else
                None
        )

    let commitFilter = CommitFilter()

    // If we found a last released commit, use it as the starting point
    // Otherwise, not providing a starting point seems to get all commits
    if lastReleasedCommit.IsSome then
        commitFilter.ExcludeReachableFrom <- lastReleasedCommit.Value

    let commits = repository.Commits.QueryBy(commitFilter).ToList()

    let commitParserConfig =
        Workspace.``commit-linter.json``
        |> File.ReadAllText
        |> Decode.unsafeFromString CommitParserConfig.decoder

    let releaseCommits =
        commits
        // Parse the commit message
        |> Seq.choose (fun commit ->
            match Parser.tryParseCommitMessage commitParserConfig commit.Message, commit with
            | Ok semanticCommit, commit ->
                Some
                    {|
                        OriginalCommit = commit
                        SemanticCommit = semanticCommit
                    |}
            | Error _, _ -> None
        )
        // Only include commits that are feat or fix
        |> Seq.filter (fun commits ->
            match commits.SemanticCommit.Type with
            | "feat"
            | "fix" -> true
            | _ -> false
        )
        // Only keep the commits related to the tool we are releasing
        // Each tool should have its own generation but I am waiting for EasyBuild.ChangelogGen to be ready
        // for implementing this correctly.
        // For now, this is good enough because we only have a single changelog
        |> Seq.filter (fun commits ->
            match commits.SemanticCommit.Tags with
            | Some tags ->
                match settings.Project with
                | Project.FableForm -> List.contains "Fable.Form" tags
                | Project.FableFormSimple -> List.contains "Fable.Form.Simple" tags
                | Project.FableFormSimpleBulma -> List.contains "Fable.Form.Simple.Bulma" tags
                | Project.FableFormSimpleBulmaNpm ->
                    List.contains "Fable.Form.Simple.Bulma.Npm" tags
                | Project.All -> true
            | None -> false
        )

    let lastChangelogVersion = Changelog.tryFindLastVersion changelogPath

    let shouldBumpMajor =
        settings.BumpMajor
        || releaseCommits
           |> Seq.exists (fun commit -> commit.SemanticCommit.BreakingChange)

    let shouldBumpMinor =
        settings.BumpMinor
        || releaseCommits |> Seq.exists (fun commit -> commit.SemanticCommit.Type = "feat")

    let shouldBumpPatch =
        settings.BumpPatch
        || releaseCommits |> Seq.exists (fun commit -> commit.SemanticCommit.Type = "fix")

    let refVersion =
        match lastChangelogVersion with
        | Ok version -> version.Version
        | Error Changelog.NoVersionFound -> SemVersion(0, 0, 0)
        | Error error -> error.ToText() |> failwith

    let newVersion =
        match settings.Version with
        | Some version -> SemVersion.Parse(version, SemVersionStyles.Strict)
        | None ->
            if shouldBumpMajor then
                refVersion.WithMajor(refVersion.Major + 1).WithMinor(0).WithPatch(0)
            elif shouldBumpMinor then
                refVersion.WithMinor(refVersion.Minor + 1).WithPatch(0)
            elif shouldBumpPatch then
                refVersion.WithPatch(refVersion.Patch + 1)
            else
                failwith "No version bump required"

    {
        NewVersion = newVersion
        ReleaseCommits = releaseCommits
        ChangelogContent = changelogContent
        LastCommitSha = commits[0].Sha
    }

let private tryFindAdditionalChangelogContent (text: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n') |> Seq.toList

    let rec apply (acc: string list) (lines: string list) (isInsideChangelogBlock: bool) =
        match lines with
        | [] -> acc
        | line :: rest ->
            if isInsideChangelogBlock then
                if line = "=== changelog ===" then
                    apply acc rest false
                else
                    apply
                        (acc
                         @ [
                             line
                         ])
                        rest
                        true
            else if line = "=== changelog ===" then
                apply acc rest true
            else
                apply acc rest false

    apply [] lines false

let private updateChangelog (releaseContext: ReleaseContext) (changelogPath: string) =
    let newVersionLines = ResizeArray<string>()

    let appendLine (line: string) = newVersionLines.Add(line)

    let newLine () = newVersionLines.Add("")

    appendLine ($"## {releaseContext.NewVersion}")
    newLine ()

    releaseContext.ReleaseCommits
    |> Seq.groupBy (fun commit -> commit.SemanticCommit.Type)
    |> Seq.iter (fun (commitType, commits) ->
        match commitType with
        | "feat" -> appendLine "### 🚀 Features"
        | "fix" -> appendLine "### 🐞 Bug Fixes"
        | _ -> ()

        newLine ()

        for commit in commits do
            let githubCommitUrl sha =
                $"https://github.com/glutinum-org/cli/commit/%s{sha}"

            let shortSha = commit.OriginalCommit.Sha.Substring(0, 7)

            let commitUrl = githubCommitUrl commit.OriginalCommit.Sha

            let description = capitalizeFirstLetter commit.SemanticCommit.Description

            $"* %s{description} ([%s{shortSha}](%s{commitUrl}))" |> appendLine

            let additionalChangelogContent =
                tryFindAdditionalChangelogContent commit.OriginalCommit.Message
                // Indent the additional lines to be under item bullet point
                |> List.map (fun line -> $"    %s{line}")
                // Trim empty lines
                |> List.map (fun line ->
                    if String.IsNullOrWhiteSpace line then
                        ""
                    else
                        line
                )

            if not additionalChangelogContent.IsEmpty then
                appendLine ""
                additionalChangelogContent |> List.iter appendLine
                appendLine ""

        newLine ()
    )

    newLine ()

    // TODO: Add contributors list
    // TODO: Add breaking changes list

    let rec removeConsecutiveEmptyLines
        (previousLineWasBlank: bool)
        (result: string list)
        (lines: string list)
        =
        match lines with
        | [] -> result
        | line :: rest ->
            // printfn $"%A{String.IsNullOrWhiteSpace(line)}"
            if previousLineWasBlank && String.IsNullOrWhiteSpace(line) then
                removeConsecutiveEmptyLines true result rest
            else
                removeConsecutiveEmptyLines
                    (String.IsNullOrWhiteSpace(line))
                    (result
                     @ [
                         line
                     ])
                    rest

    let newChangelogContent =
        [
            // Add title and description of the original changelog
            yield!
                releaseContext.ChangelogContent
                |> Seq.takeWhile (fun line -> "<!-- EasyBuild: START -->" <> line)

            // Ad EasyBuild metadata
            "<!-- EasyBuild: START -->"
            $"<!-- last_commit_released: {releaseContext.LastCommitSha} -->"
            "<!-- EasyBuild: END -->"
            ""

            // New version
            yield! newVersionLines

            // Add the rest of the changelog
            yield!
                releaseContext.ChangelogContent
                |> Seq.skipWhile (fun line -> not (line.StartsWith("##")))
        ]
        |> removeConsecutiveEmptyLines false []
        |> String.concat "\n"

    File.WriteAllText(changelogPath, newChangelogContent)

let private releaseNuGet (projectFile: string) =

    let struct (standardOutput, _) =
        Command.ReadAsync(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "pack"
            |> CmdLine.appendRaw projectFile
            |> CmdLine.appendRaw "-c Release"
            |> CmdLine.toString
        )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let m =
        Regex.Match(standardOutput, "Successfully created package '(?'nupkgPath'.*\.nupkg)'")

    if not m.Success then
        failwith $"Failed to find nupkg path in output:\n{standardOutput}"

    let nugetKey = Environment.GetEnvironmentVariable("NUGET_KEY")

    if isNull nugetKey then
        failwith "NUGET_KEY environment variable is not set"

    printfn $"""Relase: $A{m.Groups.["nupkgPath"].Value}"""

// Nuget.push (m.Groups.["nupkgPath"].Value, Environment.GetEnvironmentVariable("NUGET_KEY"))

let private releaseProject
    (repository: Repository)
    (settings: ReleaseSettings)
    (projectFile: string)
    (changelogPath: string)
    =

    let releaseContext = getReleaseContext repository settings changelogPath

    if Seq.isEmpty releaseContext.ReleaseCommits then
        printfn $"No commits to release for project %A{settings.Project}"

    else
        updateChangelog releaseContext changelogPath

        releaseNuGet projectFile

        printfn $"Changelog updated to version %A{releaseContext.NewVersion}"

let private releaseFableForm (repository: Repository) (settings: ReleaseSettings) =
    releaseProject
        repository
        settings
        Workspace.packages.``Fable.Form``.``Fable.Form.fsproj``
        Workspace.packages.``Fable.Form``.``CHANGELOG.md``

let private releaseFableFormSimple (repository: Repository) (settings: ReleaseSettings) =
    releaseProject
        repository
        settings
        Workspace.packages.``Fable.Form.Simple``.``Fable.Form.Simple.fsproj``
        Workspace.packages.``Fable.Form.Simple``.``CHANGELOG.md``

let private releaseFableFormSimpleBulma (repository: Repository) (settings: ReleaseSettings) =
    releaseProject
        repository
        settings
        Workspace.packages.``Fable.Form.Simple.Bulma``.``Fable.Form.Simple.Bulma.fsproj``
        Workspace.packages.``Fable.Form.Simple.Bulma``.``CHANGELOG.md``

let private releaseFableFormSimpleBulmaNpm (repository: Repository) (settings: ReleaseSettings) =
    failwith "Not implemented"
// releaseProject
//     repository
//     settings
//     (Workspace.packages.``fable-form-simple-bulma``.``CHANGELOG.md``)

type ReleaseCommand() =
    inherit Command<ReleaseSettings>()

    override _.Execute(context, settings) =
        // Test.TestCommand().Execute(context, Test.TestSettings()) |> ignore

        // TODO: Replace libgit2sharp with using CLI directly
        // libgit2sharp seems all nice at first, but I find the API to be a bit cumbersome
        // when manipulating the repository for (commit, stage, etc.)
        // It also doesn't support SSH
        use repository = new Repository(Workspace.``.``)

        if not (Array.contains repository.Head.FriendlyName settings.AllowBranch) then
            failwith $"Branch '{repository.Head.FriendlyName}' is not allowed to make a release"

        if repository.RetrieveStatus().IsDirty && not settings.AllowDirty then
            failwith "You must commit your changes before publishing"

        match settings.Project with
        | Project.FableForm -> releaseFableForm repository settings
        | Project.FableFormSimple -> releaseFableFormSimple repository settings
        | Project.FableFormSimpleBulma -> releaseFableFormSimpleBulma repository settings
        | Project.FableFormSimpleBulmaNpm -> releaseFableFormSimpleBulmaNpm repository settings
        | Project.All ->
            releaseFableForm repository settings
            releaseFableFormSimple repository settings
            releaseFableFormSimpleBulma repository settings
            releaseFableFormSimpleBulmaNpm repository settings

        0
