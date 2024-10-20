# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: beb30c0222a9fe45c5e6f69caa4851c895a949cc -->
<!-- EasyBuild: END -->

## 5.0.0

### 🚀 Features

* Mutualise Field definition via `Fable.Form.Simple.Fields.Html` + Introduce pipeline builder API ([ef6c8fb](https://github.com/glutinum-org/cli/commit/ef6c8fb419b022961cfa608aadbdf9afdd44fddf))
* Add supports for concrete type in Select/Radio fields ([08d3531](https://github.com/glutinum-org/cli/commit/08d3531e9808be9c574de1fb578311469657d14b))
* Make Fable.Form.Simple renderer agnostic ([5431f94](https://github.com/glutinum-org/cli/commit/5431f9411ca130ee8e0f8f7c4d40b5d32b0bdbe5))
* Remove Elmish dependency ([b9f869c](https://github.com/glutinum-org/cli/commit/b9f869cda9f384e2849ba401557a93aba286a4a0))

### 🐞 Bug Fixes

* Clean up non needed generics declared on Field classes ([e2c13b9](https://github.com/glutinum-org/cli/commit/e2c13b9ef6dbfc3d7e7c10ce9e520b2872079e1a))

## 5.0.0-beta-003 - 2024-09-09

### 🐞 Bug Fixes

* Fix `Fable.Form` dependency version

## 5.0.0-beta-002 - 2024-09-09

### 🚀 Features

* Add support for `ReadOnly` form/field ([02e31e6](https://github.com/glutinum-org/cli/commit/02e31e6fa32f3722da8868ae0b18d34fa1ea68f7))

    1. Set it at the field level

        ```fsharp
        Form.textField
            // ...
            |> Form.readOnly

        // or

        Form.textField
            // ...
            |> Form.readOnlyIf myCondition
        ```

    2. Set it at the form level

        ```fsharp
        let formValue : Form.View.Model<Values> = // ...

        { formValue with State = Form.View.State.Loading }
        ```

* Add `Form.disableIf` ([28337d9](https://github.com/glutinum-org/cli/commit/28337d90c3cd7b686f210db5ab5bde79b371bb66))

## 5.0.0-beta-001 - 2024-09-08

### 🚀 Features

* Make it easier to add custom fields ([533c062](https://github.com/glutinum-org/cli/commit/533c0626ab634267a3d5b3187410d4b4eaf68fd2))

    It is now easy to create custom form fields.

    The drawback right now is that customising only the view requires a little more work than before. But I think the trade-off is worth it.

    Before, people needed to fork `Fable.Form.Simple` and `Fable.Form.Simple.Bulma` to add custom fields. Now, they just need to implements `IField` API and it is done.

    * Define how fields are represented thanks to the `IField`, `StandardRenderFieldConfig`, `IStandardField`, `IGenericField` and more.
    * Add `FieldId` to most of field attributes because using only the label to detect field error don't guarantee a unique result.

        For example, you can have two fields with a label "FirstName". Thanks to the field id you do "firstname-student" and "firstname-teacher"

    * Export removed `Form.xxx` functions from `Fable.Form.Simple` making transition to 2.0 easy
        * `Form.succeed`
        * `Form.append`
        * `Form.disable`
        * `Form.andThen`
        * `Form.optional`
        * `Form.textField`
        * `Form.passwordField`
        * `Form.colorField`
        * `Form.dateField`
        * `Form.dateTimeLocalField`
        * `Form.numberField`
        * `Form.searchField`
        * `Form.telField`
        * `Form.timeField`
        * `Form.emailField`
        * `Form.textareaField`
        * `Form.checkboxField`
        * `Form.radioField`
        * `Form.selectField`
        * `Form.fileField`
        * `Form.group`
        * `Form.section`
        * `Form.fill`
        * `Form.rec mapFieldValues`
        * `Form.list`
        * `Form.meta`
        * `Form.mapValues`

## 4.1.0 - 2024-02-03

### Added

* Add `FileField` (by @amine-mejaouel) ([GH-43](https://github.com/MangelMaxime/Fable.Form/pull/43))

## 4.0.0 - 2022-06-23

### Changed

* Upgrade to Fable 4 and Feliz 2

## 3.0.0 - 2022-03-28

### Added

* Fix #24: Allows to customise the actions of the form.

## 2.0.0 - 2021-10-15

### Added

* Add support for more field type:
    * `Color`
    * `Date`
    * `DateTimeLocal`
    * `Email`
    * `Number`
    * `Search`
    * `Tel`
    * `Time`

* Allow to pass any attributes to an input field using the new `HtmlAttributes` property

## 1.1.0 - 2021-06-07

### Changed

* Lower FSharp.Core requirement

## 1.0.1 - 2021-05-11

### Fixed

* Publish the `*.fsi` fiels inside `fable` folder

## 1.0.0 - 2021-05-11

### Added

* Initial release
