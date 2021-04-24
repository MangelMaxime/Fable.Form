#!/usr/bin/env node


//////////////// IMPORTANT ////////////////
// The build.js file is a generated file do not edit it
// If you need to modify the build system please edit the build.ts file
// We commit the generated build.js file because it is much quicker to run and allows to get bash completion from yargs
//////////////////////////////////////////


import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import chalk from "chalk";
// import { Octokit } from "@octokit/rest";
import shell from "shelljs";
import path from 'path';
import concurrently from 'concurrently';

// Ignore TypeScript warning about unused variable for the helpers
/** @ts-ignore */
const info = chalk.blueBright
/** @ts-ignore */
const warn = chalk.yellow
/** @ts-ignore */
const error = chalk.red
/** @ts-ignore */
const success = chalk.green
/** @ts-ignore */
const log = console.log
const resolve = (...args: string[]) => path.resolve(__dirname, ...args)

// Make shellsjs throw if there is an error in a command
// It makes it easy to stop the build script when an error occured without having to try/catch or test each invocation
shell.config.fatal = true

// const getEnvVariable = function (varName: string) {
//     const value = process.env[varName];
//     if (value === undefined) {
//         log(error(`Missing environnement variable ${varName}`))
//         process.exit(1)
//     } else {
//         return value;
//     }
// }

class Examples {

    static get root() {
        return resolve("examples")
    }

    static shellExec(command: string) {
        shell.exec(
            command,
            {
                cwd: Examples.root
            }
        )
    }

    static clean() {
        shell.rm("-rf", resolve("examples", "fableBuild"))
        shell.rm("-rf", resolve("examples", "src", "obj"))
        shell.rm("-rf", resolve("examples", "src", "bin"))
        shell.rm("-rf", resolve("examples", "output"))
    }

    static async watch() {
        Examples.clean();
        Examples.shellExec("npm install");

        await concurrently(
            [
                {
                    command: "npx webpack serve --mode development",
                    cwd: Examples.root
                },
                {
                    command: "dotnet fable --outDir fableBuild --watch",
                    cwd: Examples.root
                }
            ],
            {
                prefix: "none"
            }
        )
    }

    static async build() {
        Examples.clean();
        Examples.shellExec("npm install");
        Examples.shellExec("dotnet fable --outDir fableBuild");
        Examples.shellExec("npx webpack --mode production");
    }

}

yargs(hideBin(process.argv))
    .strict()
    .help()
    .alias("help", "h")
    .command(
        "examples",
        "Commands related to the Example project",
        (argv) => {
            return argv
                .command(
                    "watch",
                    "Start Example in watch mode.",
                    () => { },
                    Examples.watch
                )
                .command(
                    "build",
                    "Build Example project using production mode",
                    () => { },
                    Examples.build
                )
        }
    )
    // .command(
    //     "publish",
    //     "Use this command when you want to release a new version of the library and update the demo project on Github pages",
    //     (argv) => {
    //         argv
    //             .options(
    //                 "demo-only",
    //                 {
    //                     description: "When active, the publish task will only publish the demo project on Github pages",
    //                     type: "boolean",
    //                     default: false
    //                 }
    //             )
    //     },
    //     publishHandler
    // )
    .version(false)
    .argv
