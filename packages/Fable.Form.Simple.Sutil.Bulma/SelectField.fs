namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma

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
            let toOption (selectedKey: string) (key: string, label: string) =
                let isSelected = (selectedKey = key)

                Html.option [
                    if config.IsReadOnly then
                        prop.style [
                            Css.displayNone
                        ]
                    prop.value key
                    prop.text label
                    if isSelected then
                        prop.selected true
                ]

            let placeholderOption =
                Html.option [
                    if config.IsReadOnly then
                        prop.style [
                            Css.displayNone
                        ]
                    prop.disabled true
                    if config.Value = "" then
                        prop.selected true

                    prop.text ("-- " + config.Attributes.Placeholder + " --")
                ]

            bulma.select [
                prop.disabled config.Disabled
                Ev.onChange config.OnChange

                match config.OnBlur with
                | Some onBlur -> Ev.onBlur (fun _ -> onBlur ())

                | None -> ()

                prop.value config.Value

                placeholderOption

                yield! config.Attributes.Options |> List.map (toOption config.Value)
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
