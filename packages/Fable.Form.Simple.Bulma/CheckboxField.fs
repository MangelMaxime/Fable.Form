namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma

module CheckboxField =

    type Attributes =
        {
            FieldId: string
            Text: string
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, bool, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, bool, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun _ -> false)

    type Field<'Values, 'Output, 'Value>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, bool, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<'Msg, bool, Attributes>) =

            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            // Checkbox can't really be set to readonly in HTML
                            // So we need to not listen to the onChange event
                            prop.readOnly config.IsReadOnly
                            if not config.IsReadOnly then
                                prop.onChange (config.OnChange >> config.Dispatch)

                            match config.OnBlur with
                            | Some onBlur -> prop.onBlur (fun _ -> config.Dispatch onBlur)

                            | None -> ()
                            prop.disabled config.Disabled

                            prop.isChecked config.Value
                        ]

                        Html.text config.Attributes.Text
                    ]
                ]
            ]
            |> List.singleton
            |> Html.View.wrapInFieldContainer
