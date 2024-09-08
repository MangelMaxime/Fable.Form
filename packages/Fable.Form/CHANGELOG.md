# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: b4c90120754bc99cd66712e9e240013751f9eff5 -->
<!-- EasyBuild: END -->

## 3.0.0-beta-001

### 🚀 Features

* Make it easier to add custom fields ([c99eed9](https://github.com/glutinum-org/cli/commit/c99eed98527d3a0f19b75967434b74af8cb7ca26))

    * Field attributes now needs to inherit from `IAttributes`
    * Refactor `Base.fill` to explicitly take a `values` argument instead of returning a lambda

        ```fsharp
        val fill:
            Form<'Values,'Output,'Field>
            -> 'Values -> FilledForm<'Output,'Field>
        ```

        ```fsharp
        val fill:
            Form<'Values,'Output,'Field> ->
            values: 'Values
            -> FilledForm<'Output,'Field>
        ```

## 2.0.0 - 2022-06-23

### Changed

* Upgrade to Fable 4

## 1.2.0 - 2022-07-12

### Fixed

* Fix #32: Add `Form.disable`

## 1.1.0 - 2021-06-07

### Changed

* Lower FSharp.Core requirement

## 1.0.1 - 2021-05-11

### Fixed

* Publish the `*.fsi` fiels inside `fable` folder

## 1.0.0 - 2021-05-11

### Added

* Initial release
