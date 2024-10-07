namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma

module RadioField =

    type OptionItem =
        abstract member Key: string
        abstract member Text: string

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            Label: string
            Options: OptionItem list
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, OptionItem option, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, OptionItem option, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field _.IsNone

    type Field<'Values>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, OptionItem option, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<OptionItem option, Attributes>) =

            let radio (optionItem: OptionItem) =
                Bulma.input.labels.radio [
                    Bulma.input.radio [
                        prop.name config.Attributes.Label
                        prop.isChecked (Some optionItem = config.Value: bool)
                        prop.disabled config.Disabled

                        // RadioField can't really be set to readonly in HTML
                        // So we need to not listen to the onChange event
                        prop.readOnly config.IsReadOnly
                        if not config.IsReadOnly then
                            prop.onChange (fun (_: bool) -> config.OnChange(Some optionItem))

                        match config.OnBlur with
                        | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

                        | None -> ()
                    ]

                    Html.text optionItem.Text
                ]

            Bulma.control.div [
                config.Attributes.Options |> List.map radio |> prop.children
            ]
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
