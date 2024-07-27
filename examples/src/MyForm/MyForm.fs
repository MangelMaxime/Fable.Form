namespace MyForm

open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Field
open MyForm.Field

// Make an alias to the "default" form module so it is easier to reference
module DefaultForm = Fable.Form.Simple.Form

module Form =

    // Create our own Form domain defining the list of Field supported

    type SignatureField<'Values> = SignatureField.SignatureField<'Values>

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Field<'Values, 'Attributes> =
        // We want to support the standard fields  defined by Fable.Form.Simple
        | Standard of Form.Field<'Values, 'Attributes>
        // Below are our custom fields
        | Signature of SignatureField<'Values>

    type FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

    /// <summary>
    /// Represents our form type
    /// </summary>
    type Form<'Values, 'Output, 'Attributes> =
        Base.Form<'Values, 'Output, Field<'Values, 'Attributes>>

    ///////////////////////////////////////
    /// Start of combinators redefined ///
    /////////////////////////////////////

    // We redefined the main combinators specialized for our custom field type
    // this will allows us to write `Form.succeed` instead of `Base.succeed`
    // Plus `Form.succeed` is restricted to our Form type defined above

    // Tips: In your own project, I encourage you to look at `Fable.Form.Simple` source code
    // to copy/paste the functions with XML documentation
    // Here they have been stripped to make the example more concise

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

    /////////////////////////////////////
    /// End of combinators redefined ///
    ///////////////////////////////////

    // Redefined the default field functions, so we can wrap them with `Field.Default`
    // Here only `textField` has been redefined, but in your own project you should
    // redefine all the default fields

    let textField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form
            (fun x -> (Form.TextType.TextRaw, x) |> Form.Field.Text |> Field.Standard)
            config

    // Define field functions for our own fields

    let signatureField
        (config: Base.FieldConfig<SignatureField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        SignatureField.form Field.Signature config

    module View =

        open Elmish
        open Feliz

        [<NoComparison; NoEquality>]
        type ToggleFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: SignatureField.Attributes
            }

        [<NoComparison; NoEquality>]
        type CustomConfig<'Msg, 'Attributes> =
            {
                // Custom configuration used by the Default form
                Standard: Form.View.CustomConfig<'Msg, 'Attributes>
                // Render function for our new fields
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
            // If we have a default field, we forward it to the default render function
            | Field.Standard defaultField ->
                let filledField: Form.FilledField<_, _> =
                    {
                        State = defaultField
                        Error = field.Error
                        IsDisabled = field.IsDisabled
                    }

                Form.View.renderField customConfig.Standard dispatch fieldConfig filledField

            | Field.Signature info ->
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
