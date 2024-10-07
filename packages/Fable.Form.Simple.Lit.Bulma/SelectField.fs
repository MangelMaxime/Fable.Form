namespace Fable.Form.Simple.Lit.Bulma.Fields

open Fable.Form
open Lit
open Fable.Form.Simple.Lit.Bulma

module SelectField =

    type Attributes =
        {
            FieldId: string
            Label: string
            Placeholder: string
            Options: (string * string) list
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

    type Field<'Values>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, string, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, Attributes>) =

            // let toOption (key: string, label: string) =
            //     Html.option [
            //         if config.IsReadOnly then
            //             prop.style [
            //                 style.display.none
            //             ]
            //         prop.value key
            //         prop.text label
            //     ]

            // let placeholderOption =
            //     Html.option [
            //         if config.IsReadOnly then
            //             prop.style [
            //                 style.display.none
            //             ]
            //         prop.disabled true
            //         prop.value ""

            //         prop.text ("-- " + config.Attributes.Placeholder + " --")
            //     ]

            // Bulma.select [
            //     prop.disabled config.Disabled
            //     prop.onChange config.OnChange

            //     match config.OnBlur with
            //     | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

            //     | None -> ()

            //     prop.value config.Value

            //     prop.children [
            //         placeholderOption

            //         yield! config.Attributes.Options |> List.map toOption
            //     ]
            // ]
            // |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error

            html $""" """
