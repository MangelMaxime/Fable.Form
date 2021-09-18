namespace Fable.Form.Simple.Bulma

[<RequireQualifiedAccess>]
module Form =

    module View =

        open Feliz
        open Feliz.Bulma
        open Fable.Form
        open Fable.Form.Simple
        open Fable.Form.Simple.Form.View

        let fieldLabel (label : string) =
            Bulma.label [
                prop.text label
            ]

        let errorMessage (message : string) =
            Bulma.help [
                color.isDanger
                prop.text message
            ]

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
                    Bulma.help [ ]

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

        let inputField
            (typ : InputType)
            (
                {
                    Dispatch = dispatch
                    OnChange = onChange
                    OnBlur = onBlur
                    Disabled = disabled
                    Value = value
                    Error = error
                    ShowError = showError
                    Attributes = attributes
                } : TextFieldConfig<'Msg, IReactProperty>
            ) =

            let inputFunc =
                match typ with
                | Text ->
                    Bulma.input.text

                | Password ->
                    Bulma.input.password

                | Email ->
                    Bulma.input.email

                | Color ->
                    Bulma.input.color

                | Date ->
                    Bulma.input.date

                | DateTimeLocal ->
                    Bulma.input.datetimeLocal

                | Number ->
                    Bulma.input.number

                | Search ->
                    Bulma.input.search

                | Tel ->
                    Bulma.input.tel

                | Time ->
                    Bulma.input.time


            inputFunc [
                prop.onChange (onChange >> dispatch)

                match onBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        dispatch onBlur
                    )

                | None ->
                    ()

                prop.disabled disabled
                prop.value value
                prop.placeholder attributes.Placeholder
                if showError && error.IsSome then
                    color.isDanger

                yield! attributes.HtmlAttributes
            ]
            |> withLabelAndError attributes.Label showError error

        let textareaField
            (
                {
                    Dispatch = dispatch
                    OnChange = onChange
                    OnBlur = onBlur
                    Disabled = disabled
                    Value = value
                    Error = error
                    ShowError = showError
                    Attributes = attributes
                } : TextFieldConfig<'Msg, IReactProperty>
            ) =

            Bulma.textarea [
                prop.onChange (onChange >> dispatch)

                match onBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        dispatch onBlur
                    )

                | None ->
                    ()

                prop.disabled disabled
                prop.value value
                prop.placeholder attributes.Placeholder
                if showError && error.IsSome then
                    color.isDanger

                yield! attributes.HtmlAttributes
            ]
            |> withLabelAndError attributes.Label showError error

        let checkboxField
            (
                {
                    Dispatch = dispatch
                    OnChange = onChange
                    OnBlur = onBlur
                    Disabled = disabled
                    Value = value
                    Attributes = attributes
                } : CheckboxFieldConfig<'Msg>
            ) =

            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            prop.onChange (onChange >> dispatch)
                            match onBlur with
                            | Some onBlur ->
                                prop.onBlur (fun _ ->
                                    dispatch onBlur
                                )

                            | None ->
                                ()
                            prop.disabled disabled
                            prop.isChecked value
                        ]

                        Html.text attributes.Text
                    ]
                ]
            ]
            |> (fun x -> [ x ])
            |> wrapInFieldContainer

        let radioField
            (
                {
                    Dispatch = dispatch
                    OnChange = onChange
                    OnBlur = onBlur
                    Disabled = disabled
                    Value = value
                    Error = error
                    ShowError = showError
                    Attributes = attributes
                } : RadioFieldConfig<'Msg>
            ) =

            let radio (key : string, label : string) =
                Bulma.input.labels.radio [
                    Bulma.input.radio [
                        prop.name attributes.Label
                        prop.isChecked (key = value : bool)
                        prop.disabled disabled
                        prop.onChange (fun (_ : bool) -> onChange key |> dispatch)
                        match onBlur with
                        | Some onBlur ->
                            prop.onBlur (fun _ ->
                                dispatch onBlur
                            )

                        | None ->
                            ()
                    ]

                    Html.text label
                ]


            Bulma.control.div [
                attributes.Options
                |> List.map radio
                |> prop.children
            ]
            |> withLabelAndError attributes.Label showError error

        let selectField
            (
                {
                    Dispatch = dispatch
                    OnChange = onChange
                    OnBlur = onBlur
                    Disabled = disabled
                    Value = value
                    Error = error
                    ShowError = showError
                    Attributes = attributes
                } : SelectFieldConfig<'Msg>
            ) =

            let toOption (key : string, label : string) =
                Html.option [
                    prop.value key
                    prop.text label
                ]

            let placeholderOption =
                Html.option [
                    prop.disabled true
                    prop.value ""

                    prop.text ("-- " + attributes.Placeholder + " --")
                ]

            Bulma.select [
                prop.disabled disabled
                prop.onChange (onChange >> dispatch
                )

                match onBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        dispatch onBlur
                    )

                | None ->
                    ()

                prop.value value

                prop.children [
                    placeholderOption

                    yield! attributes.Options
                    |> List.map toOption
                ]
            ]
            |> withLabelAndError attributes.Label showError error

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
            (field : Form.FilledField<'Values, IReactProperty>)
            : Form.FilledField<'Values, IReactProperty> =

            match parentError with
            | Some _ ->
                field

            | None ->
                { field with Error = None }

        let formList
            (
                {
                    Dispatch = dispatch
                    Forms = forms
                    Label = label
                    Add = add
                    Disabled = disabled
                } : FormListConfig<'Msg>
            ) =

            let addButton =
                match disabled, add with
                | (false, Some add) ->
                    Bulma.button.a [
                        prop.onClick (fun _ ->
                            add.Action() |> dispatch
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
                    fieldLabel label

                    yield! forms

                    addButton
                ]
            ]

        let formListItem
            (
                {
                    Dispatch = dispatch
                    Fields = fields
                    Delete = delete
                    Disabled = disabled
                } : FormListItemConfig<'Msg>
            ) =

            let removeButton =
                match disabled, delete with
                | (false, Some delete) ->
                    Bulma.button.a [
                        prop.onClick (fun _ ->
                            delete.Action() |> dispatch
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
                    yield! fields

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

        let form
            (
                {
                    Dispatch = dispatch
                    OnSubmit = onSubmit
                    State = state
                    Action = action
                    CancelPolicy = cancelPolicy
                    Fields = fields
                } : FormConfig<'Msg>
            ) =

            Html.form [
                prop.onSubmit (fun ev ->
                    ev.stopPropagation()
                    ev.preventDefault()

                    onSubmit
                    |> Option.map dispatch
                    |> Option.defaultWith ignore
                )
                
                match cancelPolicy with
                    | CancelPolicy.DoNothing -> ()
                    | CancelPolicy.Action action ->
                        prop.onCancel (fun _ -> action())
                        

                prop.children [
                    yield! fields

                    match state with
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
                                Bulma.button.button [
                                    color.isPrimary
                                    prop.text action
                                    // If the form is loading animate the submit button with the loading animation
                                    if state = Loading then
                                        button.isLoading
                                ]
                                match cancelPolicy with
                                    | CancelPolicy.DoNothing -> Html.none
                                    | CancelPolicy.Action action ->
                                        Bulma.button.button [
                                            color.isLight
                                            prop.text "Cancel"
                                        ]                                
                            ]
                        ]
                    ]
                ]
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg, IReactProperty> =
            {
                Form = form
                TextField = inputField Text
                PasswordField = inputField Password
                EmailField = inputField Email
                TextAreaField = textareaField
                ColorField = inputField Color
                DateField = inputField  Date
                DateTimeLocalField = inputField DateTimeLocal
                NumberField = inputField Number
                SearchField = inputField Search
                TelField = inputField Tel
                TimeField = inputField Time
                CheckboxField = checkboxField
                RadioField = radioField
                SelectField = selectField
                Group = group
                Section = section
                FormList = formList
                FormListItem = formListItem
            }

        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config
