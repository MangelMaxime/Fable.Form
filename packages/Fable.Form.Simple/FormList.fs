module Fable.Form.Simple.FormList

open Fable.Form
open Fable.Form.Extensions

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
type FormList<'Values, 'Field> =
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
    (tagger: FormList<'Values, 'Field> -> 'Field)
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
                (fun state (element: Base.FilledForm<'Output, 'Field>) -> element.IsEmpty && state)
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
                                    (listOfElementValues @ [ formConfig.Default ])
                                    values
                        Attributes = formConfig.Attributes
                    }
            Result = result
            IsEmpty = isEmpty
        }
    )
