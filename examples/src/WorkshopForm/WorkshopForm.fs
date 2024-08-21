module Fable.Form.WorkshopForm.Base

module Standard = Fable.Form.Simple.Form

open Fable.Form
open Fable.Form.Simple.View
open Fable.Form.Simple.Field

open Fable.Form.WorkshopForm.Field

module Form =

    type MyInputField<'Values> = MyInputField.MyInputField<'Values>

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Field<'Values, 'Attributes> =
        | Standard of Standard.Field<'Values, 'Attributes>
        | MyInputField of MyInputField<'Values>

    type FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

    /// <summary>
    /// Represents our form type
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

    let disable (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A, 'Attributes> =

        Base.disable form

    let andThen
        (child: 'A -> Form<'Values, 'B, 'Attributes>)
        (parent: Form<'Values, 'A, 'Attributes>)
        : Form<'Values, 'B, 'Attributes>
        =

        Base.andThen child parent

    let optional (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A option, 'Attributes> =
        Base.optional form

    let textField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form
            (fun x -> (Standard.TextType.TextRaw, x) |> Standard.Field.Text |> Field.Standard)
            config

    let myInputField
        (config: Base.FieldConfig<MyInputField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        MyInputField.form Field.MyInputField config

    module View =

        open Elmish
        open Feliz

        [<NoComparison; NoEquality>]
        type MyInputFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: MyInputField.Attributes
            }

        [<NoComparison; NoEquality>]
        type CustomConfig<'Msg, 'Attributes> =
            {
                Standard: Standard.View.CustomConfig<'Msg, 'Attributes>
                // Render function for our new fields
                MyInputField: MyInputFieldConfig<'Msg> -> ReactElement
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
            | Field.MyInputField info ->
                customConfig.MyInputField
                    {
                        Dispatch = dispatch
                        OnChange = info.Update >> fieldConfig.OnChange
                        OnBlur = blur info.Attributes.Label
                        Disabled = field.IsDisabled || fieldConfig.Disabled
                        Value = info.Value
                        Error = field.Error
                        ShowError = fieldConfig.ShowError info.Attributes.Label
                        Attributes = info.Attributes
                    }

            | Field.Standard standardField ->
                let filledField: Standard.FilledField<_, _> =
                    {
                        State = standardField
                        Error = field.Error
                        IsDisabled = field.IsDisabled
                    }

                Standard.View.renderField customConfig.Standard dispatch fieldConfig filledField
