namespace Fable.Form.Simple

open Fable.Form

[<RequireQualifiedAccess>]
module Form =

    module View =

        open Feliz

        [<NoEquality; NoComparison>]
        type OnSubmit = OnSubmit of (unit -> unit)

        type State =
            | Idle
            | Loading
            | ReadOnly
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
        type Action =
            | SubmitOnly of string
            | Custom of (State -> ReactElement)

        [<NoComparison; NoEquality>]
        type ViewConfig<'Values, 'Output> =
            {
                OnChange: Model<'Values> -> unit
                OnSubmit: 'Output -> unit
                Action: Action
                Validation: Validation
            }

        [<NoComparison; NoEquality>]
        type FormConfig<'Output> =
            {
                OnSubmit: OnSubmit option
                State: State
                Action: Action
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

        let setLoading (formModel: Model<'Values>) =
            { formModel with
                State = Loading
            }

        [<NoComparison; NoEquality>]
        type FieldConfig<'Values, 'Value> =
            {
                OnChange: 'Values -> unit
                OnBlur: (string -> unit) option
                Disabled: bool
                IsReadOnly: bool
                ShowError: string -> bool
            }

        let errorToString (error: Error.Error) =
            match error with
            | Error.RequiredFieldIsEmpty -> "This field is required"

            | Error.ValidationFailed validationError -> validationError

            | Error.External externalError -> externalError

        let custom
            (viewConfig: ViewConfig<'Values, 'Output>)
            (renderForm: FormConfig<'Output> -> ReactElement)
            (renderField: FieldConfig<'Values, 'Output> -> Base.FilledField<'Field> -> ReactElement)
            (form: Base.Form<'Values, 'Output, 'Field>)
            (model: Model<'Values>)
            =

            let (fields, result) =
                let res = Base.fill form model.Values

                res.Fields, res.Result

            let (ErrorTracking errorTracking) = model.ErrorTracking

            let onSubmit =
                match result with
                | Ok msg ->
                    if model.State = Loading then
                        None

                    else
                        (fun () -> viewConfig.OnSubmit msg) |> OnSubmit |> Some

                | Result.Error _ ->
                    if errorTracking.ShowAllErrors then
                        None

                    else
                        fun () ->
                            viewConfig.OnChange
                                { model with
                                    ErrorTracking =
                                        ErrorTracking
                                            {| errorTracking with
                                                ShowAllErrors = true
                                            |}
                                }
                        |> OnSubmit
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
                    {
                        OnChange =
                            fun values ->
                                viewConfig.OnChange
                                    { model with
                                        Values = values
                                    }
                        OnBlur = onBlur
                        Disabled = model.State = Loading
                        IsReadOnly = model.State = ReadOnly
                        ShowError = showError
                    }

            renderForm
                {
                    OnSubmit = onSubmit
                    Action = viewConfig.Action
                    State = model.State
                    Fields = List.map fieldToElement fields
                }
