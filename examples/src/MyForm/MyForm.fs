namespace MyForm

open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Field
open MyForm.Field

module Form =

    type ToggleField<'Values> = ToggleField.ToggleField<'Values>

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Field<'Values, 'Attributes> =
        | Default of Form.Field<'Values, 'Attributes>
        | Toggle of ToggleField<'Values>

    type FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

    /// <summary>
    /// Represents a form using Fable.Form.Simple representation
    /// </summary>
    type Form<'Values, 'Output, 'Attributes> =
        Base.Form<'Values, 'Output, Field<'Values, 'Attributes>>

    let succeed (output: 'Output) : Form<'Values, 'Output, 'Attributes> = Base.succeed output

    let append
        (newForm: Form<'Values, 'A, 'Attributes>)
        (currentForm: Form<'Values, 'A -> 'B, 'Attributes>)
        : Form<'Values, 'B, 'Attributes>
        =

        Base.append newForm currentForm

    let toggleField
        (config: Base.FieldConfig<ToggleField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        ToggleField.form Field.Toggle config

    let textField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form
            (fun x ->
                (Fable.Form.Simple.Form.TextType.TextRaw, x)
                |> Fable.Form.Simple.Form.Field.Text
                |> Field.Default
            )
            config

    module View =

        open Elmish
        open Feliz

        [<NoComparison; NoEquality>]
        type ToggleFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: bool -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: bool
                Error: Error.Error option
                ShowError: bool
                Attributes: ToggleField.Attributes
            }

        [<NoComparison; NoEquality>]
        type CustomConfig<'Msg, 'Attributes> =
            {
                Default: Form.View.CustomConfig<'Msg, 'Attributes>
                Toggle: ToggleFieldConfig<'Msg> -> ReactElement
            }

        let renderField
            (customConfig: CustomConfig<'Msg, 'Attributes>)
            (dispatch: Dispatch<'Msg>)
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (field: FilledField<'Values, 'Attributes>)
            : ReactElement
            =

            let blur label =
                Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

            match field.State with
            | Field.Default defaultField ->
                let filledField: Form.FilledField<_, _> =
                    {
                        State = defaultField
                        Error = field.Error
                        IsDisabled = field.IsDisabled
                    }

                Form.View.renderField customConfig.Default dispatch fieldConfig filledField

            | Field.Toggle info ->
                customConfig.Toggle
                    {
                        Dispatch = dispatch
                        OnChange = info.Update >> fieldConfig.OnChange
                        OnBlur = blur info.Attributes.Label
                        Disabled = fieldConfig.Disabled
                        Value = info.Value
                        Error = field.Error
                        ShowError = fieldConfig.ShowError info.Attributes.Label
                        Attributes = info.Attributes
                    }
