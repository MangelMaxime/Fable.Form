
module Form.Base.Core

open Form

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

type Form<'Values, 'Output, 'Field> =
    'Values -> FilledForm<'Output, 'Field>

type FieldConfig<'Attributes, 'Input, 'Values, 'Output> =
    {
        Parser : 'Input -> Result<'Output, string>
        Value : 'Values -> 'Input
        Update : 'Input -> 'Values -> 'Values
        Error : 'Values -> string option
        Attributes : 'Attributes
    }

let succeed (output : 'Output) : Form<'Values, 'Output, 'Field> =
    fun _ ->
        {
            Fields = []
            Result = Ok output
            IsEmpty = true
        }

let fill (form : Form<'Values, 'Output, 'Field>) : 'Values -> FilledForm<'Output, 'Field> =
    form

let append (newForm : Form<'Values, 'A, 'Field>) (currentForm : Form<'Values, 'A -> 'B, 'Field>) : Form<'Values, 'B, 'Field> =
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

        | Error _ ->
            failwith "Not yet implement"


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

    (fun values ->
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
