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
import ghpages from 'gh-pages';

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


class GHPages {

    static publishPromise (path : string, config?: ghpages.PublishOptions) {
        if (config === undefined) {
            return new Promise<void>((resolve, reject) => {
                ghpages.publish(path, (err) => {
                    if (err) {
                        reject(err)
                    } else {
                        resolve();
                    }
                })
            })
        } else {
            return new Promise<void>((resolve, reject) => {
                ghpages.publish(path, config, (err) => {
                    if (err) {
                        reject(err)
                    } else {
                        resolve();
                    }
                })
            })
        }
    }

}

class Examples {

    static get root() {
        return resolve("examples")
    }

    static resolve(...args: string[]) {
        return resolve(Examples.root, ...args)
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
        shell.rm("-rf", Examples.resolve("fableBuild"))
        shell.rm("-rf", Examples.resolve("src", "obj"))
        shell.rm("-rf", Examples.resolve("src", "bin"))
        shell.rm("-rf", Examples.resolve("output"))
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

    static build() {
        Examples.clean();
        Examples.shellExec("npm install");
        Examples.shellExec("dotnet fable --outDir fableBuild");
        Examples.shellExec("npx webpack --mode production");
    }

    static async publish() {
        Examples.build();
        await GHPages.publishPromise(Examples.resolve("output"));
    }

}

class Tests {

    static get root () {
        return resolve("tests");
    }

    static resolve(...args: string[]) {
        return resolve(Tests.root, ...args)
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
        shell.rm("-rf", Tests.resolve("bin"));
        shell.rm("-rf", Tests.resolve("obj"));
        shell.rm("-rf", Tests.resolve("fableBuild"));
    }

    static async watch() {
        Tests.clean();

        // We need to create the fableBuild directory in order to get nodemon / mocha to watch in it
        shell.mkdir(Tests.resolve("fableBuild"))

        await concurrently(
            [
                {
                    command: 'npx nodemon --watch fableBuild --delay 150ms --exec "npx mocha -r esm -r mocha.env.js --reporter dot --recursive fableBuild"',
                    cwd: Tests.root
                },
                {
                    command: "dotnet fable --watch --outDir fableBuild",
                    cwd: Tests.root
                }
            ],
            {
                prefix: "none"
            });
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
    .command(
        "tests",
        "Commands related to the tests",
        (argv) => {
            return argv
                .command(
                    "watch",
                    "Start the Test in watch mode re-compiling and re-running them on file change",
                    () => {},
                    Tests.watch
                )
        }
    )
    .command(
        "publish",
        "Command used to released a new version, update Github pages, etc.",
        (argv) => {
            return argv
                .command(
                    "ghpages",
                    "Publish the example project on Github pages",
                    () => {},
                    Examples.publish
                )
        }
    )
    .version(false)
    .argv
