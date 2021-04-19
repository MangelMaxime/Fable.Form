#!/usr/bin/env node

const yargs = require('yargs')
const { hideBin } = require('yargs/helpers')
const concurrently = require('concurrently')
const chalk = require("chalk")
var shell = require("shelljs")
const path = require("path")
const fs = require("fs").promises
const parseChangelog = require('changelog-parser')
const { Octokit } = require("@octokit/rest");
const ghpages = require("gh-pages")

const info = chalk.blueBright
const warn = chalk.yellow
const error = chalk.red
const success = chalk.green
const log = console.log
const resolve = (...args) => path.resolve(__dirname, ...args)

const examplesProjectPath = resolve("examples")
const packageName = "Thoth.Elmish.Toast"

// Make shellsjs throw if there is an error in a command
// It makes it easy to stop the build script when an error occured without having to try/catch or test each invocation
shell.config.fatal = true

const getEnvVariable = function (varName) {
    const value = process.env[varName];
    if (value === undefined) {
        log(error(`Missing environnement variable ${varName}`))
        process.exit(1)
    } else {
        return value;
    }
}

const shellExecInExampleProject = (command) => {
    shell.exec(
        command,
        {
            cwd: examplesProjectPath
        }
    )
}

const ghpagesPublishPromise = (path, config) => {
    return new Promise((resolve, reject) => {
        ghpages.publish(path, config, (err) => {
            if (err) {
                reject(err)
            } else {
                resolve()
            }
        })
    })
}

const examplesClean = () => {
    shell.rm("-rf", resolve("examples", "fableBuild"))
    shell.rm("-rf", resolve("examples", "src", "obj"))
    shell.rm("-rf", resolve("examples", "src", "bin"))
    shell.rm("-rf", resolve("examples", "output"))
}

const examplesWatchHandler = async () => {
    examplesClean()

    shellExecInExampleProject("npm install")

    await concurrently(
        [
            {
                command: "npx webpack serve --mode development",
                cwd: examplesProjectPath
            },
            {
                command: "dotnet fable --outDir fableBuild --watch",
                cwd: examplesProjectPath
            }
        ],
        {
            prefix: "none" // Disable name prefix
        }
    )
}

const examplesBuildHandler = async () => {
    examplesClean()

    shellExecInExampleProject("npm install")

    shellExecInExampleProject("dotnet fable --outDir fableBuild")

    shellExecInExampleProject("npx webpack --mode production")
}

const generateNugetPackageAndPublishIt = async () => {
    const NUGET_KEY = getEnvVariable("NUGET_KEY")

    // Clean files
    shell.rm("-rf", resolve("src", "bin"))
    shell.rm("-rf", resolve("src", "obj"))

    // Prepare the library package
    const projectFsprojPath = resolve("src", `${packageName}.fsproj`)
    const projectFsprojConent = (await fs.readFile(projectFsprojPath)).toString()

    const changelogPath = resolve("CHANGELOG.md")
    // Normalize the new lines otherwise parseChangelog isn't able to parse the file correctly
    const changelogContent = (await fs.readFile(changelogPath)).toString().replace("\r\n", "\n")
    const changelog = await parseChangelog({ text: changelogContent })

    // Check if the changelog has at least 2 versions in it
    // Unreleased & X.Y.Z
    if (changelog.versions.length < 2) {
        log(error(`No version to publish for ${project}`))
        process.exit(1)
    }

    const unreleased = changelog.versions[0];

    // Check malformed changelog
    if (unreleased.title !== "Unreleased") {
        log(error(`Malformed CHANGELOG.md file`))
        log(error("The changelog should first version should be 'Unreleased'"))
        process.exit(1)
    }

    // Access via index is ok we checked the length before
    const versionInfo = changelog.versions[1]
    const newVersion = versionInfo.version

    if (newVersion == null) {
        log(error(`Malformed CHANGELOG.md file`))
        log(error("Please verify the last version format, it should be SEMVER compliant"))
        process.exit(1)
    }

    const fsprojVersionRegex = /<Version>(.*)<\/Version>/gmi
    const m = fsprojVersionRegex.exec(projectFsprojConent)

    if (m === null) {
        log(error(`Missing <Version>..</Version> tag in ${projectFsprojPath}`))
        process.exit(1)
    }

    const lastPublishedVersion = m[1];

    if (lastPublishedVersion === newVersion) {
        log(`Last version has already been published. Skipping...`)
        process.exit(0)
    }

    log(`New version detected, starting publishing`)

    const newFsprojContent = projectFsprojConent.replace(fsprojVersionRegex, `<Version>${newVersion}</Version>`)

    // Start a try-catch here, because we modfied the file on the disk
    // This allows to revert the changes made to the file is something goes wrong
    try {
        // Update fsproj file on the disk
        await fs.writeFile(projectFsprojPath, newFsprojContent)

        shell.exec(
            "dotnet pack -c Release",
            {
                cwd: resolve("src")
            }
        )

        const nugetPackagePath = resolve("src", "bin", "Release", `${packageName}.${newVersion}.nupkg`)

        shell.exec(
            `dotnet nuget push -s nuget.org -k ${NUGET_KEY} ${nugetPackagePath}`
        )

        log(success(`Nuget package published successfully`))

        const GITHUB_TOKEN = getEnvVariable("GITHUB_TOKEN")

        // Stupid isPrerelease detection, if the version contains "-" then we consider it a pre-release
        // Example:
        //  - 1.0.0-beta-001
        //  - 1.0.0-alpha-002
        const isPrerelease = newVersion.indexOf("-") === - 1 ? false : true;

        const octokit = new Octokit({
            auth : GITHUB_TOKEN,
            userAgent : 'Thoth.Elmish.Toast deployment script'
        })

        await octokit.rest.repos.createRelease({
            owner: "thoth-org",
            repo: "Thoth.Elmish.Toast",
            tag_name: newVersion,
            target_commitish: "main",
            name: newVersion,
            body: versionInfo.body,
            prerelease: isPrerelease
        })

    } catch (e) {
        log(error(`Something went wrong while publishing`))
        log("Reverting changes made to the files")
        await fs.writeFile(projectFsprojPath, projectFsprojConent)
        log("Revert done")
        log(e)
        process.exit(1)
    }
}


const publishHandler = async (argv) => {
    throw "Not ready yet"
    examplesBuildHandler()

    // If user didn't ask only for the demo to be published
    // if (!argv.demoOnly) {
    //     await generateNugetPackageAndPublishIt()
    // }

    // // Publish the demo on Github Pages
    // await ghpagesPublishPromise(resolve("demo", "output"))

}

yargs(hideBin(process.argv))
    .completion()
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
                        () => {},
                        examplesWatchHandler
                    )
                    .command(
                        "build",
                        "Build Example project using production mode",
                        () => {},
                        examplesBuildHandler
                    )
        }
    )
    .command(
        "publish",
        "Use this command when you want to release a new version of the library and update the demo project on Github pages",
        (argv) => {
            argv
                .options(
                    "demo-only",
                    {
                        description: "When active, the publish task will only publish the demo project on Github pages",
                        type: "boolean",
                        default: false
                    }
                )
        },
        publishHandler
    )
    .version(false)
    .argv
