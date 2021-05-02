module Fable.Form.Base

open Fable.Form.Extensions

type FilledField<'Field> =
    {
        State : 'Field
        Error : Error.Error option
        IsDisabled : bool
    }

type FilledForm<'Output, 'Field> =
    {
        Fields : FilledField<'Field> list
        Result : Result<'Output, (Error.Error * Error.Error list)>
        IsEmpty : bool
    }

[<NoComparison; NoEquality>]
type Form<'Values, 'Output, 'Field> =
    Form of ('Values -> FilledForm<'Output, 'Field>)

[<NoComparison; NoEquality>]
type FieldConfig<'Attributes, 'Input, 'Values, 'Output> =
    {
        Parser : 'Input -> Result<'Output, string>
        Value : 'Values -> 'Input
        Update : 'Input -> 'Values -> 'Values
        Error : 'Values -> string option
        Attributes : 'Attributes
    }

type CustomField<'Output, 'Field> =
    {
        State : 'Field
        Result : Result<'Output, (Error.Error * Error.Error list)>
        IsEmpty : bool
    }

let succeed (output : 'Output) : Form<'Values, 'Output, 'Field> =
    Form (
        fun _ ->
            {
                Fields = []
                Result = Ok output
                IsEmpty = true
            }
    )

let fill<'Values, 'Output, 'Field> (Form form : Form<'Values, 'Output, 'Field>) : 'Values -> FilledForm<'Output, 'Field> =
    form

let custom (fillField : 'Values -> CustomField<'Output, 'Field>) : Form<'Values, 'Output, 'Field> =
    Form (
        fun values ->
            let filled =
                fillField values

            {
                Fields =
                    [
                        {
                            State = filled.State
                            Error =
                                if filled.IsEmpty then
                                    Some Error.RequiredFieldIsEmpty

                                else
                                    match filled.Result with
                                    | Ok _ ->
                                        None

                                    | Error (firstError, _) ->
                                        Some firstError
                            IsDisabled = false
                        }
                    ]
                Result = filled.Result
                IsEmpty = filled.IsEmpty
            }
    )


let meta (fn : 'Values -> Form<'Values, 'Output, 'Field>) : Form<'Values, 'Output, 'Field> =
    Form (
        fun values ->
            fill (fn values) values
    )

let mapValues
    (fn : 'A -> 'B)
    (form : Form<'B, 'Output, 'Field>)
    : Form<'A, 'Output, 'Field> =

    Form (fn >> fill form)

let mapField
    (fn : 'A -> 'B)
    (form : Form<'Values, 'Output, 'A>)
    : Form<'Values, 'Output, 'B> =
    Form (
        fun values ->
            let filled =
                fill form values

            {
                Fields =
                    filled.Fields
                    |> List.map (fun filledField ->
                        {
                            State = fn filledField.State
                            Error = filledField.Error
                            IsDisabled = filledField.IsDisabled
                        }
                    )
                Result = filled.Result
                IsEmpty = filled.IsEmpty
            }
    )


let append (newForm : Form<'Values, 'A, 'Field>) (currentForm : Form<'Values, 'A -> 'B, 'Field>) : Form<'Values, 'B, 'Field> =
    Form (
        fun values ->
            let filledNew =
                fill newForm values

            let filledCurrent =
                fill currentForm values

            let fields =
                filledCurrent.Fields @ filledNew.Fields

            let isEmpty =
                filledCurrent.IsEmpty && filledNew.IsEmpty

            match filledCurrent.Result with
            | Ok fn ->
                {
                    Fields = fields
                    Result = Result.map fn filledNew.Result
                    IsEmpty = isEmpty
                }

            | Error (firstError, otherErrors) ->
                match filledNew.Result with
                | Ok _ ->
                    {
                        Fields = fields
                        Result = Error (firstError, otherErrors)
                        IsEmpty = isEmpty
                    }

                | Error (newFirstError, newOtherErrors) ->
                    {
                        Fields = fields
                        Result =
                            Error (
                                firstError,
                                otherErrors @ (newFirstError :: newOtherErrors)
                            )
                        IsEmpty = isEmpty
                    }
    )


let andThen
    (child : 'A -> Form<'Values, 'B, 'Field>)
    (parent : Form<'Values, 'A, 'Field>)
    : Form<'Values, 'B, 'Field> =

    Form (
        fun values ->
            let filled =
                fill parent values

            match filled.Result with
            | Ok output ->
                let childFilled =
                    fill (child output) values

                {
                    Fields = filled.Fields @ childFilled.Fields
                    Result = childFilled.Result
                    IsEmpty = filled.IsEmpty && childFilled.IsEmpty
                }

            | Error errors ->
                {
                    Fields = filled.Fields
                    Result = Error errors
                    IsEmpty = filled.IsEmpty
                }
    )

let map
    (fn : 'A -> 'B)
    (form : Form<'Values, 'A, 'Field>)
    : Form<'Values, 'B, 'Field> =

    Form (
        fun values ->
            let filled =
                fill form values

            {
                Fields = filled.Fields
                Result = Result.map fn filled.Result
                IsEmpty = filled.IsEmpty
            }
    )

let field
    (isEmpty : 'Input -> bool)
    (build : Field.Field<'Attributes, 'Input, 'Values> -> 'Field)
    (config : FieldConfig<'Attributes, 'Input, 'Values, 'Output>)
    : Form<'Values, 'Output, 'Field> =

    let requiredParser value =
        if isEmpty value then
            Error (Error.RequiredFieldIsEmpty, [])
        else
            config.Parser value
            |> Result.mapError (fun error ->
                (Error.ValidationFailed error, [])
            )

    let parse values =
        requiredParser (config.Value values)
        |> Result.andThen (fun output ->
            config.Error values
            |> Option.map (fun error ->
                Error (Error.External error, [])
            )
            |> Option.defaultValue (Ok output)
        )

    let field_ values =
        let value =
            config.Value values

        let update newValue =
            config.Update newValue values

        build
            {
                Value = value
                Update = update
                Attributes = config.Attributes
            }

    Form (
        fun values ->
            let result =
                parse values

            let (error, isEmpty_) =
                match result with
                | Ok _ ->
                    (None, false)

                | Error (firstError, _) ->
                    Some firstError, firstError = Error.RequiredFieldIsEmpty
            {
                Fields =
                    [ { State = field_ values; Error = error; IsDisabled = false } ]
                Result = result
                IsEmpty = isEmpty_
            }
    )

let optional
    (form : Form<'Values, 'Output, 'Field>)
    : Form<'Values, 'Output option, 'Field> =

    Form (
        fun values ->
            let filled =
                fill form values

            match filled.Result with
            | Ok value ->
                {
                    Fields = filled.Fields
                    Result = Ok (Some value)
                    IsEmpty = filled.IsEmpty
                }

            | Error (firstError, otherErrors) ->
                if filled.IsEmpty then
                    {
                        Fields =
                            filled.Fields
                            |> List.map (fun field ->
                                { field with Error = None }
                            )
                        Result = Ok None
                        IsEmpty = filled.IsEmpty
                    }
                else
                    {
                        Fields = filled.Fields
                        Result = Error (firstError, otherErrors)
                        IsEmpty = false
                    }
    )
