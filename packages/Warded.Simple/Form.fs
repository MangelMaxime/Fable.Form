namespace Warded.Simple

open Warded
open Warded.Simple.Field

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

    let mapValues
        (config : {| Value : 'A -> 'B; Update : 'B -> 'A -> 'A |})
        (form : Form<'B, 'Output>)
        : Form<'A, 'Output> =

        Base.meta (fun values ->
            form
            |> Base.mapValues config.Value
            |> Base.mapField (mapFieldValues config.Update values)
        )


    module View =

        open Feliz
        open Feliz.Bulma
        open Elmish

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

        let fieldLabel (label : string) =
            Bulma.label [
                prop.text label
            ]

        let errorMessage (message : string) =
            Bulma.help [
                color.isDanger
                prop.text message
            ]

        let errorToString (error : Error.Error) =
            match error with
            | Error.RequiredFieldIsEmpty ->
                "This field is required"

            | Error.ValidationFailed validationError ->
                validationError

            | Error.External externalError ->
                externalError

        let errorMessageAsHtml (showError : bool) (error : Error.Error option) =
            match error with
            | Some (Error.External externalError) ->
                errorMessage externalError

            | _ ->
                if showError then
                    error
                    |> Option.map errorToString
                    |> Option.map errorMessage
                    |> Option.defaultValue (Bulma.help [ ])

                else
                    Bulma.help [

                    ]

        let wrapInFieldContainer (children : ReactElement list) =
            Bulma.field.div [
                prop.children children
            ]

        let withLabelAndError
            (label : string)
            (showError : bool)
            (error : Error.Error option)
            (fieldAsHtml : ReactElement)
            : ReactElement =
            [
                fieldLabel label
                Bulma.control.div [
                    fieldAsHtml
                ]
                errorMessageAsHtml showError error
            ]
            |> wrapInFieldContainer

        let inputField (typ : InputType) (config : TextFieldConfig<'Msg>) =
            let inputFunc =
                match typ with
                | InputType.Text ->
                    Bulma.input.text

                | InputType.Password ->
                    Bulma.input.password

            inputFunc [
                prop.onChange (fun (text : string) -> config.OnChange text |> config.Dispatch)
                prop.disabled config.Disabled
                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder
                if config.ShowError && config.Error.IsSome then
                    color.isDanger
            ]
            |> withLabelAndError config.Attributes.Label config.ShowError config.Error

        let textareaField (config : TextFieldConfig<'Msg>) =
            Bulma.textarea [
                prop.onChange (fun (text : string) -> config.OnChange text |> config.Dispatch)
                prop.disabled config.Disabled
                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder
                if config.ShowError && config.Error.IsSome then
                    color.isDanger
            ]
            |> withLabelAndError config.Attributes.Label config.ShowError config.Error

        let checkboxField (config : CheckboxFieldConfig<'Msg>) =
            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            prop.onChange (fun (isChecked : bool) -> config.OnChange isChecked |> config.Dispatch )
                            prop.disabled config.Disabled
                            prop.isChecked config.Value
                        ]

                        Html.text config.Attributes.Text
                    ]
                ]
            ]
            |> (fun x -> [ x ])
            |> wrapInFieldContainer

        let radioField (config : RadioFieldConfig<'Msg>) =
            let radio (key : string, label : string) =
                Bulma.input.labels.radio [
                    Bulma.input.radio [
                        prop.name config.Attributes.Label
                        prop.isChecked (config.Value = key)
                        prop.disabled config.Disabled
                        prop.onChange (fun (_ : bool) -> config.OnChange key |> config.Dispatch)
                    ]

                    Html.text label
                ]


            Bulma.control.div [
                config.Attributes.Options
                |> List.map (fun option ->
                    radio option
                )
                |> prop.children
            ]
            |> withLabelAndError config.Attributes.Label config.ShowError config.Error

        let selectField (config : SelectFieldConfig<'Msg>) =
            let toOption (key : string, label : string) =
                Html.option [
                    prop.value key
                    prop.text label
                ]

            let placeholderOption =
                Html.option [
                    prop.disabled true
                    prop.value ""

                    prop.text ("-- " + config.Attributes.Placeholder + " --")
                ]

            Bulma.select [
                prop.disabled config.Disabled
                prop.onChange (fun (value : string) ->
                    config.OnChange value |> config.Dispatch
                )

                prop.value config.Value

                prop.children [
                    placeholderOption

                    yield! config.Attributes.Options
                    |> List.map toOption
                ]
            ]
            |> withLabelAndError config.Attributes.Label config.ShowError config.Error

        let group (fields : ReactElement list) =
            Bulma.field.div [
                Bulma.columns [
                    fields
                    |> List.map Bulma.column
                    |> prop.children
                ]
            ]

        let section (title : string) (fields : ReactElement list) =
            Html.fieldSet [
                prop.className "fieldset"

                prop.children [
                    Html.legend [
                        prop.text title
                    ]

                    yield! fields
                ]
            ]


        let ignoreChildError
            (parentError : Error.Error option)
            (field : FilledField<'Values>)
            : FilledField<'Values> =

            match parentError with
            | Some _ ->
                field

            | None ->
                { field with Error = None }

        let formList (formConfig : FormListConfig<'Msg>) : ReactElement =
            let addButton =
                match formConfig.Disabled, formConfig.Add with
                | (false, Some add) ->
                    Bulma.button.a [
                        prop.onClick (fun _ ->
                            add.Action() |> formConfig.Dispatch
                        )

                        prop.children [
                            Bulma.icon [
                                icon.isSmall

                                prop.children [
                                    Html.i [
                                        prop.className "fas fa-plus"
                                    ]
                                ]
                            ]

                            Html.span add.Label
                        ]
                    ]

                | _ ->
                    Html.none

            Bulma.field.div [
                Bulma.control.div [
                    fieldLabel formConfig.Label

                    yield! formConfig.Forms

                    addButton
                ]
            ]

        let formListItem (config : FormListItemConfig<'Msg>) : ReactElement =
            let removeButton =
                match config.Disabled, config.Delete with
                | (false, Some delete) ->
                    Bulma.button.a [
                        prop.onClick (fun _ ->
                            delete.Action() |> config.Dispatch
                        )

                        prop.children [
                            Bulma.icon [
                                icon.isSmall

                                prop.children [
                                    Html.i [
                                        prop.className "fas fa-times"
                                    ]
                                ]
                            ]

                            if delete.Label <> "" then
                                Html.span delete.Label
                        ]
                    ]

                | _ ->
                    Html.none

            Html.div [
                prop.className "form-list"

                prop.children [
                    yield! config.Fields

                    Bulma.field.div [
                        field.isGrouped
                        field.isGroupedRight

                        prop.children [
                            Bulma.control.div [
                                removeButton
                            ]
                        ]
                    ]
                ]
            ]


        //let form<'Element when 'Element :> ReactElement> (config : FormConfig<'Msg, 'Element>) =
        let form (config : FormConfig<'Msg>) =
            Html.form [
                prop.onSubmit (fun ev ->
                    ev.stopPropagation()
                    ev.preventDefault()

                    config.OnSubmit
                    |> Option.map config.Dispatch
                    |> Option.defaultWith ignore
                )
                prop.children [
                    yield! config.Fields

                    match config.State with
                    | Error error ->
                        errorMessage error

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
                    | Idle ->
                        Html.none

                    Bulma.field.div [
                        field.isGrouped
                        field.isGroupedRight

                        prop.children [
                            Bulma.control.div [
                                Bulma.button.submit [
                                     color.isPrimary
                                     if config.State = Loading then
                                        prop.value config.Loading
                                    else
                                        prop.value config.Action
                                ]
                            ]
                        ]
                    ]
                ]
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg> =
            {
                Form = form
                TextField = inputField Text
                PasswordField = inputField Password
                TextAreaField = textareaField
                CheckboxField = checkboxField
                RadioField = radioField
                SelectField = selectField
                Group = group
                Section = section
                FormList = formList
                FormListItem = formListItem
            }

        let rec renderField
            (dispatch : Dispatch<'Msg>)
            (customConfig : CustomConfig<'Msg>)
            (fieldConfig : FieldConfig<'Values, 'Msg>)
            (field : FilledField<'Values>)
            : ReactElement =

            let blur = None // TODO:

            match field.State with
            | Field.Text (typ, info) ->
                let config : TextFieldConfig<'Msg> =
                    {
                        Dispatch = dispatch
                        OnChange = info.Update >> fieldConfig.OnChange
                        OnBlur = blur // TODO:
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
                        OnBlur = blur // TODO:
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
                        OnBlur = blur // TODO:
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
                        OnBlur = blur // TODO:
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
                                    ErrorTracking = ErrorTracking {| errorTracking with ShowAllErrors = true |}
                            }
                        |> Some

            let showError (label : string) =
                errorTracking.ShowAllErrors || Set.contains label errorTracking.ShowFieldError

            let fieldToElement =
                renderField
                    viewConfig.Dispatch
                    config
                    {
                        OnChange = fun values -> viewConfig.OnChange { model with Values = values }
                        OnBlur = None // TODO
                        Disabled = model.State = Loading
                        ShowError = showError
                    }

            let onBlur = None // TODO

            config.Form
                {
                    Dispatch = viewConfig.Dispatch
                    OnSubmit = onSubmit
                    Action = viewConfig.Action
                    Loading = viewConfig.Loading
                    State = model.State
                    Fields = List.map fieldToElement fields
                }


        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config
