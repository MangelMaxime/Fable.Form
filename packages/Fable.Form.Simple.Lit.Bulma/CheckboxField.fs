namespace Fable.Form.Simple.Lit.Bulma.Fields

open Fable.Form
open Lit
open Fable.Form.Simple.Lit.Bulma

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

    type Field<'Values>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, bool, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<bool, Attributes>) =

            let onChange =
                // Checkbox can't really be set to readonly in HTML
                // So we need to not listen to the onChange event
                if config.IsReadOnly then
                    ignore
                else
                    fun (ev: Browser.Types.Event) ->
                        (ev.target :?> Browser.Types.HTMLInputElement).``checked``
                        |> config.OnChange

            let onBlur =
                match config.OnBlur with
                | Some onBlur -> fun _ -> onBlur ()
                | None -> ignore

            html
                $"""
                <div class="control">
                    <label class="checkbox">
                        <input
                            type="checkbox"
                            class="checkbox"
                            .disabled={config.Disabled}
                            .readOnly={config.IsReadOnly}
                            .checked={config.Value}

                            @change={onChange}
                            @blur={onBlur}
                            >
                        {config.Attributes.Text}
                    </label>
                </div>
            """
            |> List.singleton
            |> Html.View.wrapInFieldContainer
