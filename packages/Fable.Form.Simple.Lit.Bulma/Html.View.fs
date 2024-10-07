namespace Fable.Form.Simple.Lit.Bulma.Html

open Lit
open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.Form.View
open Fable.Form.Simple.Lit.Bulma

module View =

    let fieldLabel (label: string) =
        html
            $"""
            <label class="label">
                {label}
            </label>
        """

    let errorMessage (message: string) =
        html
            $"""
            <p class="help is-danger">
                {message}
            </p>
        """

    let errorMessageAsHtml (showError: bool) (error: Error.Error option) =
        let empty = html $"""<p class="help"></p>"""

        match error with
        | Some(Error.External externalError) -> errorMessage externalError

        | _ ->
            if showError then
                error
                |> Option.map errorToString
                |> Option.map errorMessage
                |> Option.defaultValue empty

            else
                empty

    let wrapInFieldContainer (children: TemplateResult list) =
        html
            $"""
            <div class="field">
                {children}
            </div>
        """

    let withLabelAndError
        (label: string)
        (showError: bool)
        (error: Error.Error option)
        (fieldAsHtml: TemplateResult)
        : TemplateResult
        =
        [
            fieldLabel label
            html
                $"""
                <div class="control">
                    {fieldAsHtml}
                </div>
            """
            errorMessageAsHtml showError error
        ]
        |> wrapInFieldContainer

    let form
        ({
             OnSubmit = onSubmit
             State = state
             Action = action
             Fields = fields
         }: FormConfig<'Output, TemplateResult>)
        =

        let stateElement =
            match state with
            | Error error -> errorMessage error

            | Success success ->
                html
                    $"""
                    <div class="field">
                        <div class="control has-text-centered has-text-weight-bold has-text-success">
                            {success}
                        </div>
                    </div>
                """

            | Loading
            | ReadOnly
            | Idle -> html $""" """

        let actionElement =
            match action with
            | Action.SubmitOnly submitLabel ->
                let buttonCls =
                    [
                        "button"
                        "is-primary"
                        if state = Loading then
                            "is-loading"
                    ]
                    |> String.concat " "

                html
                    $"""
                    <div class="field is-grouped is-grouped-right">
                        <div class="control">
                            <button class="{buttonCls}" type="submit">
                                {submitLabel}
                            </button>
                        </div>
                    </div>
                """

            | Action.Custom func -> func state

        html
            $"""
            <form
                @submit={fun (ev: Browser.Types.Event) ->
                             ev.stopPropagation ()
                             ev.preventDefault ()

                             match onSubmit with
                             | None -> ()
                             | Some(OnSubmit onSubmit) -> onSubmit ()}
            >

                {fields}
                {stateElement}
                {actionElement}
            </form>
        """

    let rec renderField<'Value, 'Attributes, 'Values when 'Attributes :> Field.IAttributes>
        (fieldConfig: Form.View.FieldConfig<'Values, 'Value>)
        (field: FilledField<'Values>)
        : TemplateResult
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
                        OnChange = standardField.InnerField.Update >> fieldConfig.OnChange
                        OnBlur =
                            fieldConfig.OnBlur
                            |> Option.map (fun onBlurEvent ->
                                fun () -> onBlurEvent (attributes.GetFieldId())
                            )
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

                genericField.RenderField fieldConfig field

        with error ->
            Fable.Core.JS.console.error error
#if DEBUG
            html
                $"""
                <div>
                    Field not implemented, please implement the field `IStandardField<'Values>` or `IGenericField<'Values>`
                </div>
            """
#else
            html $""""""
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
