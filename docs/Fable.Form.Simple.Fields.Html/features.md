---
layout: standard
title: Features
---

`Fable.Form.Simple.Fields.Html` provides a set of common HTML fields that can be used by libraries targetting the browser.

Thanks to it, libraries like `Fable.Form.Simple.Bulma` and `Fable.Form.Simple.Sutil.Bulma` can focus on the layout and styling of the fields.

## Available Fields

- [RangeField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/RangeField.fs)
- [CheckboxField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/CheckboxField.fs)
- [SearchField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/SearchField.fs)
- [ColorField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/ColorField.fs)
- [SelectField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/SelectField.fs)
- [DateField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/DateField.fs)
- [TelField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/TelField.fs)
- [DateTimeLocalField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/DateTimeLocalField.fs)
- [TextField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/TextField.fs)
- [EmailField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/EmailField.fs)
- [TextareaField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/TextareaField.fs)
- [TimeField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/TimeField.fs)
- [NumberField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/NumberField.fs)
- [PasswordField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/PasswordField.fs)
- [RadioField](https://github.com/MangelMaxime/Fable.Form/blob/main/packages/Fable.Form.Simple.Fields.Html/RadioField.fs)

## Field Structure

All the fields are following the same definition pattern and can be used in the same way.

<ul class="textual-steps">
<li>

Create a module with name of the field.

```fsharp
module ColorField =
```

</li>

<li>

Define the `Attributes` type which contains all the information that can be used to customize the field, and implement the `Field.IAttributes` interface.

```fsharp
    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// List of predefined colors to display
            /// </summary>
            Suggestions: string list option
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId
```

</li>

<li>

Create a specialized version of the `Field.Field` type where we specify the `Attributes` type and the type used to represent the field value in the View layer.

Example:

- `ColorField` use `string` to represent the color value
- `SelectField` use `OptionItem option` to represent the selected item (`None` if nothing is selected)

```fsharp
type InnerField<'Values> = Field.Field<Attributes, string, 'Values>
```

</li>

<li>

A `form` function where we specify how to detect if the field `isEmpty`.

```fsharp
let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty
```

</li>

<li>

A Pipeline builder API making it easier to create an `Attributes` instance.

This is helpful, when your `Attributes` type has a lot of fields, instead of having to specify all of them, you can use the builder API to specify only the fields you need.

```fsharp
type ColorField =

    static member create(fieldId: string) : ColorField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Suggestions = None
        }

    static member withLabel (label: string) (attributes: ColorField.Attributes) =
        { attributes with
            Label = label
        }

    static member withSuggestions (suggestions: string list) (attributes: ColorField.Attributes) =
        { attributes with
            Suggestions = Some suggestions
        }
```

</li>

</ul>
