namespace Fable.Form.Simple.Lit.Bulma.Fields

open Fable.Form
open Lit
open Fable.Form.Simple.Lit.Bulma

module RadioField =

    type Attributes =
        {
            FieldId: string
            Label: string
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

            // let radio (key: string, label: string) =
            //     Bulma.input.labels.radio [
            //         Bulma.input.radio [
            //             prop.name config.Attributes.Label
            //             prop.isChecked (key = config.Value: bool)
            //             prop.disabled config.Disabled

            //             // RadioField can't really be set to readonly in HTML
            //             // So we need to not listen to the onChange event
            //             prop.readOnly config.IsReadOnly
            //             if not config.IsReadOnly then
            //                 prop.onChange (fun (_: bool) -> config.OnChange key)

            //             match config.OnBlur with
            //             | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

            //             | None -> ()
            //         ]

            //         Html.text label
            //     ]

            // Bulma.control.div [
            //     config.Attributes.Options |> List.map radio |> prop.children
            // ]
            // |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error

            html $""" """
