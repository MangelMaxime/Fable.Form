# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: b4c90120754bc99cd66712e9e240013751f9eff5 -->
<!-- EasyBuild: END -->

## 5.0.0-beta-001

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
