namespace Fable.Form.Simple

open Fable.Form
open Fable.Form.Simple.Field

[<RequireQualifiedAccess>]
module Form =

    type TextField<'Values, 'Attributes> = TextField.TextField<'Values, 'Attributes>
    type RadioField<'Values> = RadioField.RadioField<'Values>
    type CheckboxField<'Values> = CheckboxField.CheckboxField<'Values>
    type SelectField<'Values> = SelectField.SelectField<'Values>
    type FileField<'Values> = FileField.FileField<'Values>

    type TextType =
        | TextColor
        | TextDate
        | TextDateTimeLocal
        | TextEmail
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextMonth
        | TextNumber
        | TextPassword
        // TODO:
        // | TextRange
        | TextSearch
        | TextTel
        // Match for input="text"
        | TextRaw
        | TextTime
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextWeek
        | TextArea

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Field<'Values, 'Attributes> =
        | Text of TextType * TextField<'Values, 'Attributes>
        | Radio of RadioField<'Values>
        | Checkbox of CheckboxField<'Values>
        | Select of SelectField<'Values>
        | File of FileField<'Values>
        | Group of FilledField<'Values, 'Attributes> list
        | Section of title: string * FilledField<'Values, 'Attributes> list
        | List of FormList.FormList<'Values, Field<'Values, 'Attributes>>

    and FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

    type Form<'Values, 'Output, 'Attributes> =
        Base.Form<'Values, 'Output, Field<'Values, 'Attributes>>

    // Redefined some function from the Base module so the user can access them transparently and they are also specifically type for the Fable.Form.Simple absttraction

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
        TextField.form (fun x -> Field.Text(TextRaw, x)) config

    let passwordField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextPassword, x)) config

    let colorField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextColor, x)) config

    let dateField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextDate, x)) config

    let dateTimeLocalField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextDateTimeLocal, x)) config

    let numberField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextNumber, x)) config

    let searchField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextSearch, x)) config

    let telField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextTel, x)) config

    let timeField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextTime, x)) config

    let emailField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextEmail, x)) config

    let textareaField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextArea, x)) config

    let checkboxField
        (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        CheckboxField.form Field.Checkbox config

    let radioField
        (config: Base.FieldConfig<RadioField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        RadioField.form Field.Radio config

    let selectField
        (config: Base.FieldConfig<SelectField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        SelectField.form Field.Select config

    let fileField
        (config: Base.FieldConfig<FileField.Attributes, Browser.Types.File array, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        FileField.form Field.File config

    let group (form: Form<'Values, 'Output, 'Attributes>) : Form<'Values, 'Output, 'Attributes> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Field.Group res.Fields
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

    let section
        (title: string)
        (form: Form<'Values, 'Output, 'Attributes>)
        : Form<'Values, 'Output, 'Attributes>
        =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Field.Section(title, res.Fields)
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

    let fill (form: Form<'Values, 'Output, 'Attributes>) (values: 'Values) =
        // Work around type system complaining about the 'Field behind forced to a type
        // Revisit? Good enough?
        let filledForm = Base.fill form values

        {|
            Fields = filledForm.Fields
            Result = filledForm.Result
            IsEmpty = filledForm.IsEmpty
        |}

    let rec private mapFieldValues
        (update: 'A -> 'B -> 'B)
        (values: 'B)
        (field: Field<'A, 'Attributes>)
        : Field<'B, 'Attributes>
        =

        let newUpdate oldValues = update oldValues values

        match field with
        | Field.Text(textType, textField) ->
            Field.Text(textType, Field.mapValues newUpdate textField)

        | Field.Radio radioField -> Field.Radio(Field.mapValues newUpdate radioField)

        | Field.Checkbox checkboxField -> Field.Checkbox(Field.mapValues newUpdate checkboxField)

        | Field.Select selectField -> Field.Select(Field.mapValues newUpdate selectField)

        | Field.File fileField -> Field.File(Field.mapValues newUpdate fileField)

        | Field.Group fields ->
            fields
            |> List.map (fun filledField ->
                {
                    State = mapFieldValues update values filledField.State
                    Error = filledField.Error
                    IsDisabled = filledField.IsDisabled
                }
                : FilledField<'B, 'Attributes>
            )
            |> Field.Group

        | Field.Section(title, fields) ->
            let newFields =
                fields
                |> List.map (fun filledField ->
                    {
                        State = mapFieldValues update values filledField.State
                        Error = filledField.Error
                        IsDisabled = filledField.IsDisabled
                    }
                    : FilledField<'B, 'Attributes>
                )

            Field.Section(title, newFields)

        | Field.List formList ->
            Field.List
                {
                    Forms =
                        List.map
                            (fun (form: FormList.Form<'A, Field<'A, 'Attributes>>) ->
                                {
                                    Fields =
                                        List.map
                                            (fun
                                                (filledField:
                                                    Base.FilledField<Field<'A, 'Attributes>>) ->
                                                {
                                                    State =
                                                        mapFieldValues
                                                            update
                                                            values
                                                            filledField.State
                                                    Error = filledField.Error
                                                    IsDisabled = filledField.IsDisabled
                                                }
                                            )
                                            form.Fields
                                    Delete = fun _ -> update (form.Delete()) values
                                }
                            )
                            formList.Forms
                    Add = fun _ -> update (formList.Add()) values
                    Attributes = formList.Attributes
                }

    let list
        (config: FormList.Config<'Values, 'ElementValues>)
        (elementForIndex: int -> Form<'ElementValues, 'Output, 'Attributes>)
        : Form<'Values, 'Output list, 'Attributes>
        =

        let fillElement
            (elementState: FormList.ElementState<'Values, 'ElementValues>)
            : Base.FilledForm<'Output, Field<'Values, 'Attributes>>
            =
            let filledElement =
                fill (elementForIndex elementState.Index) elementState.ElementValues

            {
                Fields =
                    filledElement.Fields
                    |> List.map (fun filledField ->
                        {
                            State =
                                mapFieldValues
                                    elementState.Update
                                    elementState.Values
                                    filledField.State
                            Error = filledField.Error
                            IsDisabled = filledField.IsDisabled
                        }
                    )
                Result = filledElement.Result
                IsEmpty = filledElement.IsEmpty
            }

        let tagger formList = Field.List formList

        FormList.form tagger config fillElement

    let meta
        (fn: 'Values -> Form<'Values, 'Output, 'Attributes>)
        : Form<'Values, 'Output, 'Attributes>
        =

        Base.meta fn

    [<NoComparison; NoEquality>]
    type MapValuesConfig<'A, 'B> =
        {
            Value: 'A -> 'B
            Update: 'B -> 'A -> 'A
        }

    let mapValues
        ({ Value = value; Update = update }: MapValuesConfig<'A, 'B>)
        (form: Form<'B, 'Output, 'Attributes>)
        : Form<'A, 'Output, 'Attributes>
        =

        Base.meta (fun values ->
            form |> Base.mapValues value |> Base.mapField (mapFieldValues update values)
        )

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

        [<NoComparison; NoEquality>]
        type TextFieldConfig<'Msg, 'Attributes> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: TextField.Attributes<'Attributes>
            }

        [<NoComparison; NoEquality>]
        type CheckboxFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: bool -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: bool
                Error: Error.Error option
                ShowError: bool
                Attributes: CheckboxField.Attributes
            }

        [<NoComparison; NoEquality>]
        type RadioFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: RadioField.Attributes
            }

        [<NoComparison; NoEquality>]
        type SelectFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: SelectField.Attributes
            }

        [<NoComparison; NoEquality>]
        type FileFieldConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                OnChange: Browser.Types.File array -> 'Msg
                Disabled: bool
                Value: Browser.Types.File array
                Error: Error.Error option
                ShowError: bool
                Attributes: FileField.Attributes
            }

        [<NoComparison; NoEquality>]
        type FormListConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                Forms: ReactElement list
                Label: string
                Add:
                    {|
                        Action: unit -> 'Msg
                        Label: string
                    |} option
                Disabled: bool
            }

        [<NoComparison; NoEquality>]
        type FormListItemConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                Fields: ReactElement list
                Delete:
                    {|
                        Action: unit -> 'Msg
                        Label: string
                    |} option
                Disabled: bool
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
        type CustomConfig<'Msg, 'Attributes> =
            {
                Form: FormConfig<'Msg> -> ReactElement
                TextField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                PasswordField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                EmailField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                ColorField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                DateField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                DateTimeLocalField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                NumberField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                SearchField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                TelField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                TimeField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                TextAreaField: TextFieldConfig<'Msg, 'Attributes> -> ReactElement
                CheckboxField: CheckboxFieldConfig<'Msg> -> ReactElement
                RadioField: RadioFieldConfig<'Msg> -> ReactElement
                SelectField: SelectFieldConfig<'Msg> -> ReactElement
                FileField: FileFieldConfig<'Msg> -> ReactElement
                Group: ReactElement list -> ReactElement
                Section: string -> ReactElement list -> ReactElement
                FormList: FormListConfig<'Msg> -> ReactElement
                FormListItem: FormListItemConfig<'Msg> -> ReactElement
            }

        [<NoComparison; NoEquality>]
        type FieldConfig<'Values, 'Msg> =
            {
                OnChange: 'Values -> 'Msg
                OnBlur: (string -> 'Msg) option
                Disabled: bool
                ShowError: string -> bool
            }

        type InputType =
            | Text
            | Password
            | Email
            | Color
            | Date
            | DateTimeLocal
            | Number
            | Search
            | Tel
            | Time

        let errorToString (error: Error.Error) =
            match error with
            | Error.RequiredFieldIsEmpty -> "This field is required"

            | Error.ValidationFailed validationError -> validationError

            | Error.External externalError -> externalError

        let ignoreChildError
            (parentError: Error.Error option)
            (field: FilledField<'Values, 'Attributes>)
            : FilledField<'Values, 'Attributes>
            =

            match parentError with
            | Some _ -> field

            | None -> { field with Error = None }

        let rec renderField
            (dispatch: Dispatch<'Msg>)
            (customConfig: CustomConfig<'Msg, 'Attributes>)
            (fieldConfig: FieldConfig<'Values, 'Msg>)
            (field: FilledField<'Values, 'Attributes>)
            : ReactElement
            =

            let blur label =
                Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

            match field.State with
            | Field.Text(typ, info) ->
                let config: TextFieldConfig<'Msg, 'Attributes> =
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

                match typ with
                | TextRaw -> customConfig.TextField config

                | TextPassword -> customConfig.PasswordField config

                | TextArea -> customConfig.TextAreaField config

                | TextEmail -> customConfig.EmailField config

                | TextColor -> customConfig.ColorField config

                | TextDate -> customConfig.DateField config

                | TextDateTimeLocal -> customConfig.DateTimeLocalField config

                | TextNumber -> customConfig.NumberField config

                | TextSearch -> customConfig.SearchField config

                | TextTel -> customConfig.TelField config

                | TextTime -> customConfig.TimeField config

            | Field.Checkbox info ->
                let config: CheckboxFieldConfig<'Msg> =
                    {
                        Dispatch = dispatch
                        OnChange = info.Update >> fieldConfig.OnChange
                        OnBlur = blur info.Attributes.Text
                        Disabled = field.IsDisabled || fieldConfig.Disabled
                        Value = info.Value
                        Error = field.Error
                        ShowError = fieldConfig.ShowError info.Attributes.Text
                        Attributes = info.Attributes
                    }

                customConfig.CheckboxField config

            | Field.Radio info ->
                let config: RadioFieldConfig<'Msg> =
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

                customConfig.RadioField config

            | Field.Select info ->
                let config: SelectFieldConfig<'Msg> =
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

                customConfig.SelectField config

            | Field.File info ->
                let config: FileFieldConfig<'Msg> =
                    {
                        Dispatch = dispatch
                        OnChange = info.Update >> fieldConfig.OnChange
                        Disabled = field.IsDisabled || fieldConfig.Disabled
                        Value = info.Value
                        Error = field.Error
                        ShowError = fieldConfig.ShowError info.Attributes.Label
                        Attributes = info.Attributes
                    }

                customConfig.FileField config

            | Field.Group fields ->
                fields
                |> List.map (fun field ->
                    (ignoreChildError field.Error
                     >> renderField
                         dispatch
                         customConfig
                         { fieldConfig with
                             Disabled = field.IsDisabled || fieldConfig.Disabled
                         })
                        field
                )
                |> customConfig.Group

            | Field.Section(title, fields) ->
                fields
                |> List.map (fun field ->
                    (ignoreChildError field.Error
                     >> renderField
                         dispatch
                         customConfig
                         { fieldConfig with
                             Disabled = field.IsDisabled || fieldConfig.Disabled
                         })
                        field
                )
                |> customConfig.Section title

            | Field.List {
                             Forms = forms
                             Add = add
                             Attributes = attributes
                         } ->
                customConfig.FormList
                    {
                        Dispatch = dispatch
                        Forms =
                            forms
                            |> List.map (fun { Fields = fields; Delete = delete } ->
                                customConfig.FormListItem
                                    {
                                        Dispatch = dispatch
                                        Fields =
                                            List.map
                                                (renderField dispatch customConfig fieldConfig)
                                                fields
                                        Delete =
                                            attributes.Delete
                                            |> Option.map (fun deleteLabel ->
                                                {|
                                                    Action = delete >> fieldConfig.OnChange
                                                    Label = deleteLabel
                                                |}
                                            )
                                        Disabled = field.IsDisabled || fieldConfig.Disabled
                                    }
                            )
                        Label = attributes.Label
                        Add =
                            attributes.Add
                            |> Option.map (fun addLabel ->
                                {|
                                    Action = add >> fieldConfig.OnChange
                                    Label = addLabel
                                |}
                            )
                        Disabled = field.IsDisabled || fieldConfig.Disabled
                    }

        let custom
            (config: CustomConfig<'Msg, 'Attributes>)
            (viewConfig: ViewConfig<'Values, 'Msg>)
            (form: Form<'Values, 'Msg, 'Attributes>)
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
                    config
                    {
                        OnChange = fun values -> viewConfig.OnChange { model with Values = values }
                        OnBlur = onBlur
                        Disabled = model.State = Loading
                        ShowError = showError
                    }

            config.Form
                {
                    Dispatch = viewConfig.Dispatch
                    OnSubmit = onSubmit
                    Action = viewConfig.Action
                    State = model.State
                    Fields = List.map fieldToElement fields
                }
