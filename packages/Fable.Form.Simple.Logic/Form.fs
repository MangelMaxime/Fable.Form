namespace Fable.Form.Simple.Logic

open Fable.Form

[<RequireQualifiedAccess>]
module Form =

    // type FilledForm<'Output, 'Field, 'Attributes> =
    //    Base.FilledForm<'Output, FilledField<'Field, 'Attributes>>

    /// <summary>
    /// Fill a form with some <c>'Values</c>
    /// </summary>
    /// <param name="form">The form to fill</param>
    /// <param name="values">The values to give to the form</param>
    /// <returns>
    /// - A list of the fields of the form, with their errors
    /// - The result of the filled form which can be:
    ///     - The correct <c>'Output</c>
    ///     - A non-empty list of validation errors
    /// - Whether the form is empty or not
    /// </returns>
    let fill (form: Base.Form<'Values, 'Output, 'Field>) (values: 'Values) =
        // Work around type system complaining about the 'Field behind forced to a type
        // Revisit? Good enough?
        let filledForm = Base.fill form values

        {|
            Fields = filledForm.Fields
            Result = filledForm.Result
            IsEmpty = filledForm.IsEmpty
        |}

    module View =

        open Elmish
        open Feliz

        type State =
            | Idle
            | Loading
            | Error of string
            | Success of string

        type ErrorTracking =
            | ErrorTracking of
                {|
                    ShowAllErrors: bool
                    ShowFieldError: Set<string>
                |}

        type Model<'Values> =
            {
                Values: 'Values
                State: State
                ErrorTracking: ErrorTracking
            }

        type Validation =
            | ValidateOnBlur
            | ValidateOnSubmit

        [<RequireQualifiedAccess; NoComparison; NoEquality>]
        type Action<'Msg> =
            | SubmitOnly of string
            | Custom of (State -> Elmish.Dispatch<'Msg> -> ReactElement)

        [<NoComparison; NoEquality>]
        type ViewConfig<'Values, 'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: Model<'Values> -> 'Msg
                Action: Action<'Msg>
                Validation: Validation
            }

        [<NoComparison; NoEquality>]
        type FormConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnSubmit: 'Msg option
                State: State
                Action: Action<'Msg>
                Fields: ReactElement list
            }

        let idle (values: 'Values) =
            {
                Values = values
                State = Idle
                ErrorTracking =
                    ErrorTracking
                        {|
                            ShowAllErrors = false
                            ShowFieldError = Set.empty
                        |}
            }

        let setLoading (formModel: Model<'Values>) = { formModel with State = Loading }

        [<NoComparison; NoEquality>]
        type FieldConfig<'Values, 'Msg> =
            {
                OnChange: 'Values -> 'Msg
                OnBlur: (string -> 'Msg) option
                Disabled: bool
                ShowError: string -> bool
            }

        let errorToString (error: Error.Error) =
            match error with
            | Error.RequiredFieldIsEmpty -> "This field is required"

            | Error.ValidationFailed validationError -> validationError

            | Error.External externalError -> externalError

        let custom
            (viewConfig: ViewConfig<'Values, 'Msg>)
            (renderForm: FormConfig<'Msg> -> ReactElement)
            (renderField:
                Dispatch<'Msg>
                    -> FieldConfig<'Values, 'Msg>
                    -> Base.FilledField<'Field>
                    -> ReactElement)
            (form: Base.Form<'Values, 'Msg, 'Field>)
            (model: Model<'Values>)
            =

            let (fields, result) =
                let res = fill form model.Values

                res.Fields, res.Result

            let (ErrorTracking errorTracking) = model.ErrorTracking

            let onSubmit =
                match result with
                | Ok msg ->
                    if model.State = Loading then
                        None

                    else
                        Some msg

                | Result.Error _ ->
                    if errorTracking.ShowAllErrors then
                        None

                    else
                        viewConfig.OnChange
                            { model with
                                ErrorTracking =
                                    ErrorTracking
                                        {| errorTracking with
                                            ShowAllErrors = true
                                        |}
                            }
                        |> Some

            let onBlur =
                match viewConfig.Validation with
                | ValidateOnSubmit -> None

                | ValidateOnBlur ->
                    Some(fun label ->
                        viewConfig.OnChange
                            { model with
                                ErrorTracking =
                                    ErrorTracking
                                        {| errorTracking with
                                            ShowFieldError =
                                                Set.add label errorTracking.ShowFieldError
                                        |}
                            }
                    )

            let showError (label: string) =
                errorTracking.ShowAllErrors || Set.contains label errorTracking.ShowFieldError

            let fieldToElement =
                renderField
                    viewConfig.Dispatch
                    {
                        OnChange = fun values -> viewConfig.OnChange { model with Values = values }
                        OnBlur = onBlur
                        Disabled = model.State = Loading
                        ShowError = showError
                    }

            renderForm
                {
                    Dispatch = viewConfig.Dispatch
                    OnSubmit = onSubmit
                    Action = viewConfig.Action
                    State = model.State
                    Fields = List.map fieldToElement fields
                }
