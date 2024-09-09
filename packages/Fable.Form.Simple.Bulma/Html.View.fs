namespace Fable.Form.Simple.Bulma.Html

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.Form.View
open Fable.Form.Simple.Bulma

module View =

    let fieldLabel (label: string) =
        Bulma.label [
            prop.text label
        ]

    let errorMessage (message: string) =
        Bulma.help [
            color.isDanger
            prop.text message
        ]

    let errorMessageAsHtml (showError: bool) (error: Error.Error option) =
        match error with
        | Some(Error.External externalError) -> errorMessage externalError

        | _ ->
            if showError then
                error
                |> Option.map errorToString
                |> Option.map errorMessage
                |> Option.defaultValue (Bulma.help [])

            else
                Bulma.help []

    let wrapInFieldContainer (children: ReactElement list) =
        Bulma.field.div [
            prop.children children
        ]

    let withLabelAndError
        (label: string)
        (showError: bool)
        (error: Error.Error option)
        (fieldAsHtml: ReactElement)
        : ReactElement
        =
        [
            fieldLabel label
            Bulma.control.div [
                fieldAsHtml
            ]
            errorMessageAsHtml showError error
        ]
        |> wrapInFieldContainer

    let form
        ({
             Dispatch = dispatch
             OnSubmit = onSubmit
             State = state
             Action = action
             Fields = fields
         }: FormConfig<'Msg>)
        =

        let innerForm =
            Html.form [
                prop.onSubmit (fun ev ->
                    ev.stopPropagation ()
                    ev.preventDefault ()

                    onSubmit |> Option.map dispatch |> Option.defaultWith ignore
                )

                prop.children [
                    yield! fields

                    match state with
                    | Error error -> errorMessage error

                    | Success success ->
                        Bulma.field.div [
                            Bulma.control.div [
                                text.hasTextCentered
                                color.hasTextSuccess
                                text.hasTextWeightBold

                                prop.text success
                            ]
                        ]

                    | Loading
                    | ReadOnly
                    | Idle -> Html.none

                    match action with
                    | Action.SubmitOnly submitLabel ->
                        Bulma.field.div [
                            field.isGrouped
                            field.isGroupedRight

                            prop.children [
                                Bulma.control.div [
                                    Bulma.button.button [
                                        prop.type'.submit
                                        color.isPrimary
                                        prop.text submitLabel
                                        // If the form is loading animate the submit button with the loading animation
                                        if state = Loading then
                                            button.isLoading
                                    ]
                                ]

                            ]
                        ]

                    | Action.Custom func -> func state dispatch
                ]
            ]

        Html.div [
            prop.className "form-designer"
            prop.children [
                innerForm

                Html.div [
                    prop.id "field-properties-portal"
                ]
            ]
        ]

    let rec renderField<'Value, 'Attributes, 'Msg, 'Values when 'Attributes :> Field.IAttributes>
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (field: FilledField<'Values>)
        : ReactElement
        =

        (***

            It would be cleaner to use a match expression here.

            ```fsharp
            match field.State with
            | :? IRendererField as rendererField ->
            ```

            However, by not using a match expression we make it easier to use TypeScript for creating fields.

            ```typescript
            export function renderField(dispatch, fieldConfig, field) {
                let f1, copyOfStruct_1, value;
                const matchValue = field.State;
                if (matchValue instanceof IRendererField) {
                    const rendererField = matchValue;
                    const matchValue_1 = IRendererField__get_FieldRendererType(rendererField);
                    if (matchValue_1.tag === 1) {
                        // This is generic field
                    }
                    else {
                        // This is standard field
                    }
                }
                else {
                    // This is an invalid field
                }
            }
            ```

            This is because, the match with test the instance of the object, which means that we
            need to expose the domain to TypeScript (not easy to do, and outside of what I would
            like to maintain).

            However, by using a dynamic cast we by pass that and only work based on the shape of the object.

            ```typescript
            export function renderField(dispatch, fieldConfig, field) {
                let value, f1, copyOfStruct_1;
                try {
                    const rendererField = field.State;
                    const matchValue = IRendererField__get_FieldRendererType(rendererField);
                    if (matchValue.tag === 1) {
                        // This is generic field
                    }
                    else {
                        // This is standard field
                    }
                }
                catch (matchValue_1) {
                    // This is an invalid field
                }
            }
            ```

            See how, we don't have a test against the instance of the object.

            This means we could create an NPM package, to share the shape of the domain and then use that in TypeScript.
            *)

        try
            let rendererField = field.State :?> IRendererField

            match rendererField.FieldRendererType with
            | FieldRendererType.Standard ->

                let standardField = field.State :?> IStandardField<'Values, 'Value, 'Attributes>

                let attributes = standardField.InnerField.Attributes

                let config =
                    {
                        Dispatch = dispatch
                        OnChange = standardField.InnerField.Update >> fieldConfig.OnChange
                        OnBlur =
                            fieldConfig.OnBlur
                            |> Option.map (fun onBlurEvent -> onBlurEvent (attributes.GetFieldId()))
                        Disabled = field.IsDisabled || fieldConfig.Disabled
                        IsReadOnly = field.IsReadOnly || fieldConfig.IsReadOnly
                        Value = standardField.InnerField.Value
                        Error = field.Error
                        ShowError = fieldConfig.ShowError(attributes.GetFieldId())
                        Attributes = attributes
                    }

                standardField.RenderField config

            | FieldRendererType.Generic ->
                let genericField = field.State :?> IGenericField<'Values>

                genericField.RenderField dispatch fieldConfig field

        with error ->
            Fable.Core.JS.console.error error
#if DEBUG
            Html.div [
                prop.text
                    "Field not implemented, please implement the field `IStandardField<'Values>` or `IGenericField<'Values>`"
            ]
#else
            Html.none
#endif

    let ignoreChildError
        (parentError: Error.Error option)
        (field: FilledField<'Values>)
        : FilledField<'Values>
        =

        match parentError with
        | Some _ -> field

        | None ->
            { field with
                Error = None
            }
