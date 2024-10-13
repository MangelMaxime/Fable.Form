module EasyBuild.Commands.Example

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel
open System.IO
open EasyBuild.Utils
open EasyBuild.Workspace

type ExampleSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-build the examples")>]
    member val IsWatch: bool = false with get, set

type ExampleCommand(destination: string, workingDirectory: string, tailwindWatchDestination: string)
    =
    inherit Command<ExampleSettings>()
    interface ICommandLimiter<ExampleSettings>

    override _.Execute(context, settings) =
        let destination = DirectoryInfo(destination)

        destination.ReCreate()

        Npm.install (workingDirectory = workingDirectory)

        let fableCommand =
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendRaw "--verbose"
            |> CmdLine.toString

        if settings.IsWatch then
            // Clean up the tailwind file if it exists
            let tailwindFile = FileInfo(tailwindWatchDestination)

            if tailwindFile.Exists then
                tailwindFile.Delete()

            [
                Command.RunAsync("dotnet", fableCommand, workingDirectory = workingDirectory)
                |> Async.AwaitTask

                Command.RunAsync("npx", "vite serve", workingDirectory = workingDirectory)
                |> Async.AwaitTask

                Command.RunAsync(
                    "npx",
                    CmdLine.empty
                    |> CmdLine.appendRaw "tailwindcss"
                    |> CmdLine.appendPrefix "-o" tailwindWatchDestination
                    |> CmdLine.appendRaw "--watch"
                    |> CmdLine.toString,
                    workingDirectory = Workspace.examples.Tailwind.``.``
                )
                |> Async.AwaitTask
            ]
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore

        else

            Command.Run("dotnet", fableCommand, workingDirectory = workingDirectory)

        Command.Run("npx", "vite build", workingDirectory = workingDirectory)

        Command.Run(
            "npx",
            CmdLine.empty
            |> CmdLine.appendRaw "tailwindcss"
            |> CmdLine.appendPrefix "-o" VirtualWorkspace.docs_deploy.examples.``daisyui.css``
            |> CmdLine.appendRaw "--minify"
            |> CmdLine.toString,
            workingDirectory = Workspace.examples.Tailwind.``.``
        )

        0

type ReactExampleCommand() =
    inherit
        ExampleCommand(
            VirtualWorkspace.examples.React.fableBuild.``.``,
            Workspace.examples.React.``.``,
            VirtualWorkspace.examples.React.``public``.``daisyui.css``
        )

type SutilExampleCommand() =
    inherit
        ExampleCommand(
            VirtualWorkspace.examples.Sutil.fableBuild.``.``,
            Workspace.examples.Sutil.``.``,
            VirtualWorkspace.examples.Sutil.``public``.``daisyui.css``
        )

type LitExampleCommand() =
    inherit
        ExampleCommand(
            VirtualWorkspace.examples.Lit.fableBuild.``.``,
            Workspace.examples.Lit.``.``,
            VirtualWorkspace.examples.Lit.``public``.``daisyui.css``
        )
