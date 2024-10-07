namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma

module SelectField =

    [<AllowNullLiteral>]
    type OptionItem =
        abstract member Key: string
        abstract member Text: string

    type Value = OptionItem option

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            Label: string
            Placeholder: string
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

            let toOption (optionItem: OptionItem) =
                Html.option [
                    if config.IsReadOnly then
                        prop.style [
                            style.display.none
                        ]
                    prop.value optionItem.Key
                    prop.text optionItem.Text
                ]

            let placeholderOption =
                Html.option [
                    if config.IsReadOnly then
                        prop.style [
                            style.display.none
                        ]
                    prop.disabled true
                    prop.value ""

                    prop.text ("-- " + config.Attributes.Placeholder + " --")
                ]

            Bulma.select [
                prop.disabled config.Disabled
                prop.onChange (fun value ->
                    config.OnChange(
                        config.Attributes.Options
                        |> List.find (fun optionItem -> optionItem.Key = value)
                        |> Some
                    )
                )

                match config.OnBlur with
                | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

                | None -> ()

                match config.Value with
                | Some optionItem -> prop.value optionItem.Key
                | None -> prop.value ""

                prop.children [
                    placeholderOption

                    yield! config.Attributes.Options |> List.map toOption
                ]
            ]
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
