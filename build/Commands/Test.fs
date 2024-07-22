module EasyBuild.Commands.Test

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel
open System.IO
open EasyBuild.Workspace
open EasyBuild.Utils

type TestSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-run the tests")>]
    member val IsWatch: bool = false with get, set

let private mochaCommand =
    CmdLine.empty
    |> CmdLine.appendRaw "mocha"
    |> CmdLine.appendPrefix "-r" "mocha.env.js"
    |> CmdLine.appendPrefix "--reporter" "dot"
    |> CmdLine.appendPrefix "--recursive" "fableBuild"
    |> CmdLine.toString

type TestCommand() =
    inherit Command<TestSettings>()

    override _.Execute(context, settings) =

        let destination = DirectoryInfo(VirtualWorkspace.tests.fableBuild)

        destination.ReCreate()

        let fableCommand =
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            // |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.appendRaw "--verbose"
            |> CmdLine.toString

        if settings.IsWatch then
            [
                Command.RunAsync("dotnet", fableCommand, workingDirectory = Workspace.tests.``.``)
                |> Async.AwaitTask

                Command.RunAsync(
                    "npx",
                    CmdLine.empty
                    |> CmdLine.appendRaw "nodemon"
                    |> CmdLine.appendPrefix "--watch" "fableBuild"
                    |> CmdLine.appendPrefix "--delay" "150ms"
                    |> CmdLine.appendPrefix "--exec" mochaCommand
                    |> CmdLine.toString,
                    workingDirectory = Workspace.tests.``.``
                )
                |> Async.AwaitTask

            ]
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore

        else
            Command.Run("dotnet", fableCommand, workingDirectory = Workspace.tests.``.``)

            Command.Run("npx", mochaCommand, workingDirectory = Workspace.tests.``.``)

        0
