# Fable.Form

## Development

This project use the `build.js` file as a build system. You can see the different task available by running:

| OS | Command |
|---|---|
| Unix | `./build.js --help` |
| Windows | `build.cmd --help` |

The `build.js` file is a generated file, if you need to modify the build system please edit the `build.ts` file and generate a new `build.js` by running `npx tsc`.

*Tips: When working on the build system, you can open a new terminal and `npx tsc --watch` in it so it will generate a new `build.js` each time you make a change.*

I choosed to write the build system using TypeScript because it allows for a better type checking than JavaScript.

## The secret pun

Naming library is super annoying.

To make it easy to discover, in general, you need to choose a simple and clear name. Especially in a "small" ecosystem like Fable. The problem is that these names are rarely fun neither exciting.

But, it is possible spice thing up for this reason `Fable.Form` stands for `Fable.Formidable`.

Special thanks to [Urs Enzler](https://twitter.com/ursenzler) who came up with [this idea](https://twitter.com/ursenzler/status/1385159595526610945)

## Original implementation

Fable.Form has been inspired by [hecrj/composable-form](https://github.com/hecrj/composable-form) library. The code has been ported from Elm to F# and then I adapted it to my vision of how a form should behave.
