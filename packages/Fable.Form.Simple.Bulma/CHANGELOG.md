# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: b4c90120754bc99cd66712e9e240013751f9eff5 -->
<!-- EasyBuild: END -->

## 5.0.0-beta-001

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
