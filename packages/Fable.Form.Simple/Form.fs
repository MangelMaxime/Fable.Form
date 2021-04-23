namespace Fable.Form.Simple

open Fable.Form
open Fable.Form.Simple.Field

[<RequireQualifiedAccess>]
module Form =

    type TextField<'Values> = TextField.TextField<'Values>
    type RadioField<'Values> = RadioField.RadioField<'Values>
    type CheckboxField<'Values> = CheckboxField.CheckboxField<'Values>
    type SelectField<'Values> = SelectField.SelectField<'Values>

    type TextType =
        | TextRaw
        | TextPassword
        | TextArea

    [<RequireQualifiedAccess>]
    type Field<'Values> =
        | Text of TextType * TextField<'Values>
        | Radio of RadioField<'Values>
        | Checkbox of CheckboxField<'Values>
        | Select of SelectField<'Values>
        | Group of FilledField<'Values> list
        | Section of title : string * FilledField<'Values> list
        | List of FormList.FormList<'Values, Field<'Values>>

    and FilledField<'Values> =
        Base.FilledField<Field<'Values>>

    type Form<'Values, 'Output> =
        Base.Form<'Values, 'Output, Field<'Values>>

    let succeed (output : 'Output) : Form<'Values, 'Output> =
        Base.succeed output

    let append : Form<'Values, 'A> -> Form<'Values, 'A -> 'B> -> Form<'Values, 'B> =
        Base.append

    let andThen : ('A -> Form<'Values, 'B>) -> Form<'Values, 'A> -> Form<'Values, 'B> =
        Base.andThen

    let textField
        (config : Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        TextField.form (fun x -> Field.Text (TextRaw, x)) config

    let passwordField
        (config : Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        TextField.form (fun x -> Field.Text (TextPassword, x)) config

    let textareaField
        (config : Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        TextField.form (fun x -> Field.Text (TextArea, x)) config

    let checkboxField
        (config : Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        CheckboxField.form Field.Checkbox config

    let radioField
        (config : Base.FieldConfig<RadioField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        RadioField.form Field.Radio config

    let selectField
        (config : Base.FieldConfig<SelectField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output> =
        SelectField.form Field.Select config

    let group
        (form : Form<'Values, 'Output>)
        : Form<'Values, 'Output> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Field.Group res.Fields
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

    let section
        (title : string)
        (form : Form<'Values, 'Output>)
        : Form<'Values, 'Output> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Field.Section (title, res.Fields)
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

    //type FilledForm<'Output, 'Field> =
    //    Base.FilledForm<'Output, FilledField<'Field>>

    let fill
        (form : Form<'Values, 'Output>)
        (values : 'Values) =
        // Work around type system complaining about the 'Field behind forced to a type
        // Revisit? Good enough?
        let x = Base.fill form values

        {|
            Fields = x.Fields
            Result = x.Result
            IsEmpty = x.IsEmpty
        |}

    let rec mapFieldValues
        (update : 'A -> 'B -> 'B)
        (values : 'B)
        (field : Field<'A>)
        : Field<'B> =

        let newUpdate oldValues =
            update oldValues values

        match field with
        | Field.Text (textType, textField) ->
            Field.Text (textType, Field.mapValues newUpdate textField)

        | Field.Radio radioField ->
            Field.Radio (Field.mapValues newUpdate radioField)

        | Field.Checkbox checkboxField ->
            Field.Checkbox (Field.mapValues newUpdate checkboxField)

        | Field.Select selectField ->
            Field.Select (Field.mapValues newUpdate selectField)

        | Field.Group fields ->
            fields
            |> List.map (fun filledField ->
                {
                    State = mapFieldValues update values filledField.State
                    Error = filledField.Error
                    IsDisabled = filledField.IsDisabled
                } : FilledField<'B>
            )
            |> Field.Group

        | Field.Section (title, fields) ->
            let newFields =
                fields
                |> List.map (fun filledField ->
                    {
                        State = mapFieldValues update values filledField.State
                        Error = filledField.Error
                        IsDisabled = filledField.IsDisabled
                    } : FilledField<'B>
                )

            Field.Section (title, newFields)

        | Field.List formList ->
            Field.List
                {
                    Forms =
                        List.map
                            (fun (form : FormList.Form<'A,Field<'A>>) ->
                                {
                                    Fields =
                                        List.map
                                            (fun (filledField : Base.FilledField<Field<'A>>) ->
                                                {
                                                    State = mapFieldValues update values filledField.State
                                                    Error = filledField.Error
                                                    IsDisabled = filledField.IsDisabled
                                                }
                                            )
                                            form.Fields
                                    Delete =
                                        fun _ -> update (form.Delete ()) values
                                }
                            )
                            formList.Forms
                    Add = fun _ -> update (formList.Add ()) values
                    Attributes = formList.Attributes
                }


    let list
        (config : FormList.Config<'Values, 'ElementValues>)
        (elementForIndex : int -> Form<'ElementValues, 'Output>)
        : Form<'Values, 'Output list> =

        let fillElement (elementState : FormList.ElementState<'Values, 'ElementValues>) : Base.FilledForm<'Output, Field<'Values>> =
            let filledElement =
                fill (elementForIndex elementState.Index) elementState.ElementValues

            {
                Fields =
                    filledElement.Fields
                    |> List.map (fun filledField ->
                        {
                            State = mapFieldValues elementState.Update elementState.Values filledField.State
                            Error = filledField.Error
                            IsDisabled = filledField.IsDisabled
                        }
                    )
                Result = filledElement.Result
                IsEmpty = filledElement.IsEmpty
            }

        let tagger formList =
            Field.List formList

        FormList.form tagger config fillElement

    let meta : ('Values -> Form<'Values, 'Output>) -> Form<'Values, 'Output> =
        Base.meta

    type MapValuesConfig<'A, 'B> =
        {
            Value : 'A -> 'B
            Update : 'B -> 'A -> 'A
        }

    let mapValues
        ({ Value = value; Update = update } : MapValuesConfig<'A, 'B>)
        (form : Form<'B, 'Output>)
        : Form<'A, 'Output> =

        Base.meta (fun values ->
            form
            |> Base.mapValues value
            |> Base.mapField (mapFieldValues update values)
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
            | ErrorTracking of {| ShowAllErrors : bool; ShowFieldError : Set<string> |}
        type Model<'Values> =
            {
                Values : 'Values
                State : State
                ErrorTracking : ErrorTracking
            }


        type Validation =
            | ValidateOnBlur
            | ValidateOnSubmit

        type ViewConfig<'Values, 'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnChange : Model<'Values> -> 'Msg
                Action : string
                Loading : string
                Validation : Validation
            }

        type FormConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnSubmit : 'Msg option
                State : State
                Action : string
                Loading : string
                Fields : ReactElement list
            }

        type TextFieldConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnChange : string -> 'Msg
                OnBlur : 'Msg option
                Disabled : bool
                Value : string
                Error : Error.Error option
                ShowError : bool
                Attributes : TextField.Attributes
            }

        type CheckboxFieldConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnChange : bool -> 'Msg
                OnBlur : 'Msg option
                Disabled : bool
                Value : bool
                Error : Error.Error option
                ShowError : bool
                Attributes : CheckboxField.Attributes
            }

        type RadioFieldConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnChange : string -> 'Msg
                OnBlur : 'Msg option
                Disabled : bool
                Value : string
                Error : Error.Error option
                ShowError : bool
                Attributes : RadioField.Attributes
            }

        type SelectFieldConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                OnChange : string -> 'Msg
                OnBlur : 'Msg option
                Disabled : bool
                Value : string
                Error : Error.Error option
                ShowError : bool
                Attributes : SelectField.Attributes
            }

        type FormListConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                Forms : ReactElement list
                Label : string
                Add : {| Action : unit -> 'Msg; Label : string |} option
                Disabled : bool
            }

        type FormListItemConfig<'Msg> =
            {
                Dispatch : Dispatch<'Msg>
                Fields : ReactElement list
                Delete : {| Action : unit -> 'Msg; Label : string |} option
                Disabled : bool
            }

        let idle (values : 'Values)=
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

        let setLoading (formModel : Model<'Values>) =
            { formModel with
                State = Loading
            }

        type CustomConfig<'Msg> =
            {
                Form : FormConfig<'Msg> -> ReactElement
                TextField : TextFieldConfig<'Msg> -> ReactElement
                PasswordField : TextFieldConfig<'Msg> -> ReactElement
                TextAreaField : TextFieldConfig<'Msg> -> ReactElement
                CheckboxField : CheckboxFieldConfig<'Msg> -> ReactElement
                RadioField : RadioFieldConfig<'Msg> -> ReactElement
                SelectField : SelectFieldConfig<'Msg> -> ReactElement
                Group : ReactElement list -> ReactElement
                Section : string -> ReactElement list -> ReactElement
                FormList : FormListConfig<'Msg> -> ReactElement
                FormListItem : FormListItemConfig<'Msg> -> ReactElement
            }

        type FieldConfig<'Values, 'Msg> =
            {
                OnChange : 'Values -> 'Msg
                OnBlur : (string -> 'Msg) option
                Disabled : bool
                ShowError : string -> bool
            }

        type InputType =
            | Text
            | Password

        let errorToString (error : Error.Error) =
            match error with
            | Error.RequiredFieldIsEmpty ->
                "This field is required"

            | Error.ValidationFailed validationError ->
                validationError

            | Error.External externalError ->
                externalError

        let ignoreChildError
            (parentError : Error.Error option)
            (field : FilledField<'Values>)
            : FilledField<'Values> =

            match parentError with
            | Some _ ->
                field

            | None ->
                { field with Error = None }


        let rec renderField
            (dispatch : Dispatch<'Msg>)
            (customConfig : CustomConfig<'Msg>)
            (fieldConfig : FieldConfig<'Values, 'Msg>)
            (field : FilledField<'Values>)
            : ReactElement =

            let blur label =
                Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

            match field.State with
            | Field.Text (typ, info) ->
                let config : TextFieldConfig<'Msg> =
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
                | TextRaw ->
                    customConfig.TextField config

                | TextType.TextPassword ->
                    customConfig.PasswordField config

                | TextType.TextArea ->
                    customConfig.TextAreaField config

            | Field.Checkbox info ->
                let config : CheckboxFieldConfig<'Msg> =
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
                let config : RadioFieldConfig<'Msg> =
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
                let config : SelectFieldConfig<'Msg> =
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

            | Field.Group fields ->
                fields
                |> List.map (fun field ->
                    (ignoreChildError field.Error >> renderField dispatch customConfig { fieldConfig with Disabled = field.IsDisabled || fieldConfig.Disabled }) field
                )
                |> customConfig.Group

            | Field.Section (title, fields) ->
                fields
                |> List.map (fun field ->
                    (ignoreChildError field.Error >> renderField dispatch customConfig { fieldConfig with Disabled = field.IsDisabled || fieldConfig.Disabled }) field
                )
                |> customConfig.Section title

            | Field.List { Forms = forms; Add = add; Attributes = attributes } ->
                customConfig.FormList
                    {
                        Dispatch = dispatch
                        Forms =
                            forms
                            |> List.map (fun { Fields = fields; Delete = delete } ->
                                customConfig.FormListItem
                                    {
                                        Dispatch = dispatch
                                        Fields = List.map (renderField dispatch customConfig fieldConfig) fields
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
            (config : CustomConfig<'Msg>)
            (viewConfig : ViewConfig<'Values, 'Msg>)
            (form : Form<'Values, 'Msg>)
            (model : Model<'Values>) =

            let (fields, result) =
                let res =
                    fill form model.Values

                res.Fields, res.Result

            let (ErrorTracking errorTracking) =
                model.ErrorTracking

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
                | ValidateOnSubmit ->
                    None

                | ValidateOnBlur ->
                    Some (fun label ->
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

            let showError (label : string) =
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
                    Loading = viewConfig.Loading
                    State = model.State
                    Fields = List.map fieldToElement fields
                }
