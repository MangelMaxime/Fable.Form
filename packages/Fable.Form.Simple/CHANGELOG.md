# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: beb30c0222a9fe45c5e6f69caa4851c895a949cc -->
<!-- EasyBuild: END -->

## 5.0.0

### 🐞 Bug Fixes

* Clean `PackageTags` ([4063ed2](https://github.com/glutinum-org/cli/commit/4063ed2cb5d201ae1c9b31f3f005a1780b913265))

### 🚀 Features

* Make Fable.Form.Simple renderer agnostic ([5431f94](https://github.com/glutinum-org/cli/commit/5431f9411ca130ee8e0f8f7c4d40b5d32b0bdbe5))
* Remove Elmish dependency ([b9f869c](https://github.com/glutinum-org/cli/commit/b9f869cda9f384e2849ba401557a93aba286a4a0))

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

## 5.0.0-beta-001 - 2024-09-08

### 🚀 Features

* Make it easier to add custom fields ([e4b8ea8](https://github.com/glutinum-org/cli/commit/e4b8ea8bb4b814c932a9ad3996cd0f554435373c))

    `Fable.Form.Simple` is now field agnostic. It only contains logic on how a Form should be represented and how it behaves.

    * Change `Form.View.custom` to take an additional `renderForm` and `renderField` functions
    * Remove all `Form.xxx` functions (they moved to Fable.Form.Simple.Bulma)
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

## 3.1.0 - 2022-07-12

### Fixed

* Fix #32: Add `Form.disable`

## 3.0.0 - 2022-03-28

### Changed

* Fix #24: Allows to customise the actions of the form.

    Here is how to migrate your old code:

    ```diff
        {
            Dispatch = dispatch
            OnChange = FormChanged
    -       Action = "Sign in"
    +       Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
    ```

## 2.1.0 - 2021-11-16

### Added

* Fix #28: Add `Form.optional`

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
