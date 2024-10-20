namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

module DateTimeLocalField =

    type Field<'Values>(innerField: DateTimeLocalField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, DateTimeLocalField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField
            (config: StandardRenderFieldConfig<string, DateTimeLocalField.Attributes>)
            =
            Bulma.input.datetimeLocal [
                prop.onChange config.OnChange

                match config.OnBlur with
                | Some onBlur -> prop.onBlur (fun _ -> onBlur ())
                | None -> ()

                prop.disabled config.Disabled
                prop.readOnly config.IsReadOnly

                // For date field we need to use uncontrolled input, otherwise it will not work
                // properly when using the keyboard to input the date
                prop.defaultValue config.Value

                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                match config.Attributes.Max with
                | Some max -> prop.max max
                | None -> ()

                match config.Attributes.Min with
                | Some min -> prop.min min
                | None -> ()

                match config.Attributes.Step with
                | Some step -> prop.step step
                | None -> ()

                prop.autoFocus config.Attributes.AutoFocus
            ]
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
