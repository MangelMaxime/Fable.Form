module Fable.Form.Base

/// <summary>
/// Represents a filled field
/// </summary>
type FilledField<'Field> =
    {
        State : 'Field
        Error : Error.Error option
        IsDisabled : bool
    }

/// <summary>
/// Represents a filled form
///
/// You can obtain this by using <see cref="fill"/>
/// </summary>
type FilledForm<'Output, 'Field> =
    {
        Fields : FilledField<'Field> list
        Result : Result<'Output, (Error.Error * Error.Error list)>
        IsEmpty : bool
    }

/// <summary>
/// A <see cref="T:Form"/> which can contain any type of 'field'
/// </summary>
type Form<'Values, 'Output, 'Field> =
    Form of ('Values -> FilledForm<'Output, 'Field>)

/// <summary>
/// <see cref="T:FieldConfig"/> is a contract allowing you to describe how a field will behave
/// </summary>
type FieldConfig<'Attributes, 'Input, 'Values, 'Output> =
    {
        /// <summary>
        /// Function that valides the <c>'Input</c> value and produce an <c>Ok 'Ouput</c> on success or an <c>Error</c> describing the problem
        /// </summary>
        Parser : 'Input -> Result<'Output, string>
        /// <summary>
        /// Function which defined how to access the <c>'Input</c> from <c>'Value</c> type
        /// </summary>
        Value : 'Values -> 'Input
        /// <summary>
        /// Function which defined how the current form <c>'Values</c> should be update with the new <c>'Input</c>
        /// </summary>
        Update : 'Input -> 'Values -> 'Values
        /// <summary>
        /// Defube how to obtain a potential external error. Useful when dealing with Server-side validation for example
        /// </summary>
        Error : 'Values -> string option
        /// <summary>
        /// Type used to represents data specific to the field. For example, you can use it to ask the user to provide a label and placeholder.
        /// </summary>
        Attributes : 'Attributes
    }

/// <summary>
/// Represents a custom field on a form that has been filled with some values.
/// </summary>
type CustomField<'Output, 'Field> =
    {
        /// <summary>
        /// The field
        /// </summary>
        State : 'Field
        /// <summary>
        /// The result of the field
        /// </summary>
        Result : Result<'Output, (Error.Error * Error.Error list)>
        /// <summary>
        /// Whether the field is empty or not.
        ///
        /// <para>
        /// <c>True</c>, if it is empty
        /// </para>
        ///
        /// <para>
        /// <c/>False<c/>, otherwise
        /// </para>
        /// </summary>
        IsEmpty : bool
    }

/// <summary>
/// Create a form that always succeeds when filled.
/// </summary>
/// <param name="output">The value to return when the form is filled</param>
/// <returns>The given <c>Output</c></returns>
let succeed (output : 'Output) : Form<'Values, 'Output, 'Field> =
    Form (
        fun _ ->
            {
                Fields = []
                Result = Ok output
                IsEmpty = true
            }
    )

/// <summary>
/// Fill a form with some <c>'Values</c>
/// </summary>
/// <returns>
/// - A list of the fields of the form, with their errors
/// - The result of the filled form which can be:
///     - The correct <c>'Output</c>
///     - A non-empty list of validation errors
/// - Whether the form is empty or not
/// </returns>
let fill<'Values, 'Output, 'Field> (Form form : Form<'Values, 'Output, 'Field>) : 'Values -> FilledForm<'Output, 'Field> =
    form

/// <summary>
/// Create a custom field
/// </summary>
/// <param name="fillField">A function given some <c>'Values</c> that produce a <see cref="T:FilledField"/></param>
/// <returns>
/// A form build by applying <c>fillField</c> to a provided <c>'Values</c>
/// </returns>
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


/// <summary>
/// Build a form that depends on its own <c>'Values</c>
///
/// This is useful when a field need to checks it's value against another field value.
/// </summary>
/// <example>
/// The classic example for using <c>Base.meta</c> is when dealing with a repeat password field.
/// <code lang="fsharp">
/// Form.meta
///     (fun values ->
///         Form.passwordField
///             {
///                 Parser =
///                     fun value ->
///                         if value = values.Password then
///                             Ok ()
///
///                         else
///                             Error "The passwords do not match"
///                 Value = fun values -> values.RepeatPassword
///                 Update =
///                     fun newValue values_ ->
///                         { values_ with RepeatPassword = newValue }
///                 Error =
///                     fun _ -> None
///                 Attributes =
///                     {
///                         Label = "Repeat password"
///                         Placeholder = "Your password again..."
///                     }
///             }
///     )
/// </code>
/// </example>
/// <param name="fn"></param>
/// <returns></returns>
let meta (fn : 'Values -> Form<'Values, 'Output, 'Field>) : Form<'Values, 'Output, 'Field> =
    Form (
        fun values ->
            fill (fn values) values
    )

/// <summary>
/// Transform the values of a form.
///
/// This function is useful when you want to re-use existing form or nest them.
/// </summary>
/// <param name="fn">Function to apply transform the data</param>
/// <param name="form">The form to which we want to pass the result of the transformation</param>
/// <returns>
/// A new form resulting of <c>fn >> fill form</c>
/// </returns>
/// <example>
/// The classic example for using <c>Base.meta</c> is when dealing with a repeat password field.
/// <code lang="fsharp">
/// // Module contains a re-usable form
/// module AddressForm =
///     type Values =
///         {
///             Country : string
///             City : string
///             PostalCode : string
///         }
///
/// // Module in which we want to re-use the AddressForm
/// module ParentForm =
///     type Values =
///         {
///             Name : string
///             Address : AddressForm.Values
///         }
///
///     Form.succeed formOutput
///         |> Form.append nameField
///         |> Form.append (
///             // Here we are mapping how the parent form should use the property Address to apply the AddressForm.form function
///             Form.mapValues
///                 {
///                     Value =
///                         fun values -> values.Address
///                     Update =
///                         fun newValue values ->
///                             { values with Address = newValue }
///                 }
///                 AddressForm.form
///         )
/// </code>
/// </example>
let mapValues
    (fn : 'A -> 'B)
    (form : Form<'B, 'Output, 'Field>)
    : Form<'A, 'Output, 'Field> =

    Form (fn >> fill form)

/// <summary>
///  Apply the given function to all the field
/// </summary>
/// <param name="fn">Function to apply</param>
/// <param name="form">Form containing the list of fields to which we want to apply the function</param>
/// <returns>A new form, when <c>fn</c> has been apply to all the field of <c>form</c></returns>
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


/// <summary>
/// Append a form to another one while <b>capturing</b> the output of the first one
/// </summary>
/// <param name="newForm">Form to append</param>
/// <param name="currentForm">Form to append to</param>
/// <returns>A new form resulting in the combination of <c>newForm</c> and <c>currentForm</c></returns>
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


/// <summary>
/// Fill a form <c>andThen</c> fill another one.
///
/// This type of form is useful when some part of your form can dynamically change based on the value of another field.
/// </summary>
/// <param name="child">The child form</param>
/// <param name="parent">The parent form which is filled first</param>
/// <returns>A new form which is the result of filling the <c>parent</c> and then filling the <c>child</c> form</returns>

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

/// <summary>
/// Transform the 'output' of a form
///
/// You can use it to keep your forms decoupled from your specific view messages:
///
/// <code lang="fsharp">
/// Base.map SignUp signupForm
/// </code>
/// </summary>
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

/// <summary>
/// Create function which is used to created a single form field
///
/// This functions is meant to be used when you want to design your own View layer.
///
/// See how it is use for Fable.Form.Simple <a href="https://github.com/MangelMaxime/Fable.Form/blob/91c70b9504706fd3d65fd0bbcad97d865b18284a/packages/Fable.Form.Simple/TextField.fs#L15-L17">TextField</a>
/// </summary>
/// <param name="isEmpty">Function used to detect if the field is empty</param>
/// <param name="build">Field builder configuration</param>
/// <param name="config">Field configuration</param>
/// <returns>A form containing a single field</returns>
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

/// <summary>
/// Make a form optional.
///
/// An optional form succeeds when:
/// - All of its fields are empty, in this case it returns <c>None</c>
/// - All of its fields are correct, in this case it returns <c>Some 'Output</c>
/// </summary>
/// <param name="form">The form to make optional</param>
/// <returns>A form producing an optional 'Output</returns>
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
