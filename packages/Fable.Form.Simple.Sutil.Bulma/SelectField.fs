namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html

module SelectField =

    type Field<'Values>(innerField: SelectField.InnerField<'Values>) =

        inherit
            IStandardField<'Values, SelectField.OptionItem option, SelectField.Attributes>(
                innerField
            )

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField
            (config:
                StandardRenderFieldConfig<SelectField.OptionItem option, SelectField.Attributes>)
            =

            let toOption (optionItem: SelectField.OptionItem) =
                Html.option [
                    if config.IsReadOnly then
                        prop.style [
                            Css.displayNone
                        ]
                    prop.value optionItem.Key
                    prop.text optionItem.Text
                ]

            let placeholderOption =
                match config.Attributes.Placeholder with
                | Some placeholder ->
                    Html.option [
                        if config.IsReadOnly then
                            prop.style [
                                Css.displayNone
                            ]
                        prop.disabled true
                        prop.value ""

                        prop.text placeholder
                    ]
                | None -> Html.none

            bulma.select [
                prop.disabled config.Disabled
                Ev.onChange (fun value ->
                    config.OnChange(
                        config.Attributes.Options
                        |> List.find (fun optionItem -> optionItem.Key = value)
                        |> Some
                    )
                )

                match config.OnBlur with
                | Some onBlur -> Ev.onBlur (fun _ -> onBlur ())

                | None -> ()

                match config.Value with
                | Some optionItem -> prop.value optionItem.Key
                | None -> prop.value ""

                placeholderOption

                yield! config.Attributes.Options |> List.map toOption
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
