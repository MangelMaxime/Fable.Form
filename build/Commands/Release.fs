module EasyBuild.Commands.Release

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel

type ReleaseSettings() =
    inherit CommandSettings()

type ReleaseCommand() =
    inherit Command<ReleaseSettings>()

    override _.Execute(context, settings) =

        0
