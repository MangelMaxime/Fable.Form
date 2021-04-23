namespace Fable.Form.Simple.Feliz.Bulma

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

                match config.OnBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        config.Dispatch onBlur
                    )

                | None ->
                    ()

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

                match config.OnBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        config.Dispatch onBlur
                    )

                | None ->
                    ()

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
                            match config.OnBlur with
                            | Some onBlur ->
                                prop.onBlur (fun _ ->
                                    config.Dispatch onBlur
                                )

                            | None ->
                                ()
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
                        match config.OnBlur with
                        | Some onBlur ->
                            prop.onBlur (fun _ ->
                                config.Dispatch onBlur
                            )

                        | None ->
                            ()
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

                match config.OnBlur with
                | Some onBlur ->
                    prop.onBlur (fun _ ->
                        config.Dispatch onBlur
                    )

                | None ->
                    ()

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
            (field : Form.FilledField<'Values>)
            : Form.FilledField<'Values> =

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

        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config
