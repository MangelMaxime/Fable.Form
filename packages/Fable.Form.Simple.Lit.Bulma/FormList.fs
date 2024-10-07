namespace Fable.Form.Simple.Lit.Bulma.Fields

open Fable.Form
open Lit
open Fable.Form.Simple
open Fable.Form.Simple.Lit.Bulma
open Fable.Form.Extensions

module FormList =

    [<NoComparison; NoEquality>]
    type Form<'Values, 'Field> =
        {
            Fields: Base.FilledField<'Field> list
            Delete: unit -> 'Values
        }

    type Attributes =
        {
            Label: string
            Add: string option
            Delete: string option
        }

    [<NoComparison; NoEquality>]
    type InnerField<'Values, 'Field> =
        {
            Forms: Form<'Values, 'Field> list
            Add: unit -> 'Values
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type Config<'Values, 'ElementValues> =
        {
            Value: 'Values -> 'ElementValues list
            Update: 'ElementValues list -> 'Values -> 'Values
            Default: 'ElementValues
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type ElementState<'Values, 'ElementValues> =
        {
            Index: int
            Update: 'ElementValues -> 'Values -> 'Values
            Values: 'Values
            ElementValues: 'ElementValues
        }

    let form<'Values, 'Field, 'ElementValues, 'Output>
        (tagger: InnerField<'Values, 'Field> -> 'Field)
        (formConfig: Config<'Values, 'ElementValues>)
        (buildElement: ElementState<'Values, 'ElementValues> -> Base.FilledForm<'Output, 'Field>)
        : Base.Form<'Values, 'Output list, 'Field>
        =

        Base.custom (fun values ->
            let listOfElementValues = formConfig.Value values

            let elementForIndex index elementValues =
                buildElement
                    {
                        Update =
                            fun newElementValues values ->
                                let newList = List.setAt index newElementValues listOfElementValues

                                formConfig.Update newList values
                        Index = index
                        Values = values
                        ElementValues = elementValues
                    }

            let filledElements = List.mapi elementForIndex listOfElementValues

            let toForm (index: int) (form: Base.FilledForm<'Output, 'Field>) =
                {
                    Fields = form.Fields
                    Delete =
                        fun () ->
                            let previousForms = List.take index listOfElementValues

                            let nextForms = List.drop (index + 1) listOfElementValues

                            formConfig.Update (previousForms @ nextForms) values
                }

            let gatherResults
                (next: Base.FilledForm<'Output, 'Field>)
                (current: Result<'Output list, Error.Error * Error.Error list>)
                : Result<'Output list, Error.Error * Error.Error list>
                =

                match next.Result with
                | Ok output -> Result.map (fun x -> output :: x) current

                | Error(head, errors) ->
                    match current with
                    | Ok _ -> Error(head, errors)

                    | Error(currentHead, currentErrors) ->
                        Error(head, errors @ (currentHead :: currentErrors))

            let result = List.foldBack gatherResults filledElements (Ok [])

            let isEmpty =
                List.fold
                    (fun state (element: Base.FilledForm<'Output, 'Field>) ->
                        element.IsEmpty && state
                    )
                    false
                    filledElements

            {
                State =
                    tagger
                        {
                            Forms = List.mapi toForm filledElements
                            Add =
                                fun _ ->
                                    formConfig.Update
                                        (listOfElementValues
                                         @ [
                                             formConfig.Default
                                         ])
                                        values
                            Attributes = formConfig.Attributes
                        }
                Result = result
                IsEmpty = isEmpty
            }
        )

    module View =

        [<NoComparison; NoEquality>]
        type FormListConfig =
            {
                Forms: TemplateResult list
                Label: string
                Add:
                    {|
                        Action: unit -> unit
                        Label: string
                    |} option
                Disabled: bool
            }

        [<NoComparison; NoEquality>]
        type FormListItemConfig =
            {
                Fields: TemplateResult list
                Delete:
                    {|
                        Action: unit -> unit
                        Label: string
                    |} option
                Disabled: bool
            }

    // let formList
    //     ({
    //          Forms = forms
    //          Label = label
    //          Add = add
    //          Disabled = disabled
    //      }: FormListConfig)
    //     =

    //     let addButton =
    //         match disabled, add with
    //         | (false, Some add) ->
    //             Bulma.button.a [
    //                 prop.onClick (fun _ -> add.Action())

    //                 prop.children [
    //                     Bulma.icon [
    //                         icon.isSmall

    //                         prop.children [
    //                             Html.i [
    //                                 prop.className "fas fa-plus"
    //                             ]
    //                         ]
    //                     ]

    //                     Html.span add.Label
    //                 ]
    //             ]

    //         | _ -> Html.none

    //     Bulma.field.div [
    //         Bulma.control.div [
    //             Html.View.fieldLabel label

    //             yield! forms

    //             addButton
    //         ]
    //     ]

    // let formListItem
    //     ({
    //          Fields = fields
    //          Delete = delete
    //          Disabled = disabled
    //      }: FormListItemConfig)
    //     =

    //     let removeButton =
    //         match disabled, delete with
    //         | (false, Some delete) ->
    //             Bulma.button.a [
    //                 prop.onClick (fun _ -> delete.Action())

    //                 prop.children [
    //                     Bulma.icon [
    //                         icon.isSmall

    //                         prop.children [
    //                             Html.i [
    //                                 prop.className "fas fa-times"
    //                             ]
    //                         ]
    //                     ]

    //                     if delete.Label <> "" then
    //                         Html.span delete.Label
    //                 ]
    //             ]

    //         | _ -> Html.none

    //     Html.div [
    //         prop.className "form-list"

    //         prop.children [
    //             yield! fields

    //             Bulma.field.div [
    //                 field.isGrouped
    //                 field.isGroupedRight

    //                 prop.children [
    //                     Bulma.control.div [
    //                         removeButton
    //                     ]
    //                 ]
    //             ]
    //         ]
    //     ]

    type Field<'Values, 'Field, 'Output, 'Value>(innerField: InnerField<'Values, IField<'Values>>) =

        inherit IGenericField<'Values>()

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =
                Field
                    {
                        Forms =
                            List.map
                                (fun (form: Form<'Values, IField<'Values>>) ->
                                    {
                                        Fields =
                                            List.map
                                                (fun
                                                    (filledField: Base.FilledField<IField<'Values>>) ->
                                                    {
                                                        State =
                                                            filledField.State.MapFieldValues
                                                                update
                                                        Error = filledField.Error
                                                        IsDisabled = filledField.IsDisabled
                                                        IsReadOnly = filledField.IsReadOnly
                                                    }
                                                )
                                                form.Fields
                                        Delete = fun _ -> update (form.Delete())
                                    }
                                )
                                innerField.Forms
                        Add = fun _ -> update (innerField.Add())
                        Attributes = innerField.Attributes
                    }

        override _.RenderField
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (filledField: FilledField<'Values>)
            =
            // View.formList
            //     {
            //         Forms =
            //             innerField.Forms
            //             |> List.map (fun
            //                              {
            //                                  Fields = fields
            //                                  Delete = delete
            //                              } ->
            //                 View.formListItem
            //                     {
            //                         Fields = List.map (Html.View.renderField fieldConfig) fields
            //                         Delete =
            //                             innerField.Attributes.Delete
            //                             |> Option.map (fun deleteLabel ->
            //                                 {|
            //                                     Action = delete >> fieldConfig.OnChange
            //                                     Label = deleteLabel
            //                                 |}
            //                             )
            //                         Disabled = filledField.IsDisabled || fieldConfig.Disabled
            //                     }
            //             )
            //         Label = innerField.Attributes.Label
            //         Add =
            //             innerField.Attributes.Add
            //             |> Option.map (fun addLabel ->
            //                 {|
            //                     Action = innerField.Add >> fieldConfig.OnChange
            //                     Label = addLabel
            //                 |}
            //             )
            //         Disabled = filledField.IsDisabled || fieldConfig.Disabled
            //     }

            html $""" """
