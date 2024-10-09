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

    /// <summary>
    /// Represents the type of a TextField
    /// </summary>
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

    /// <summary>
    /// DUs used to represents the different of Field supported by Fable.Form.Simple
    /// </summary>
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

    /// <summary>
    /// Represents a FilledField using Fable.Form.Simple representation
    /// </summary>
    and FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

    /// <summary>
    /// Represents a form using Fable.Form.Simple representation
    /// </summary>
    type Form<'Values, 'Output, 'Attributes> =
        Base.Form<'Values, 'Output, Field<'Values, 'Attributes>>

    // Redefined some function from the Base module so the user can access them transparently
    // and they are also specifically typed for the Fable.Form.Simple absttraction

    /// <summary>
    /// Create a form that always succeeds when filled.
    /// </summary>
    /// <param name="output">The value to return when the form is filled</param>
    /// <returns>The given <c>Output</c></returns>
    let succeed (output: 'Output) : Form<'Values, 'Output, 'Attributes> = Base.succeed output

    /// <summary>
    /// Append a form to another one while <b>capturing</b> the output of the first one
    /// </summary>
    /// <param name="newForm">Form to append</param>
    /// <param name="currentForm">Form to append to</param>
    /// <returns>A new form resulting in the combination of <c>newForm</c> and <c>currentForm</c></returns>
    /// <example>
    /// <code lang="fsharp">
    /// let emailField =
    ///     Form.emailField
    ///         {
    ///             // ...
    ///         }
    ///
    /// let passwordField =
    ///     Form.passwordField
    ///         {
    ///             // ...
    ///         }
    ///
    /// let onSubmit =
    ///     fun email password ->
    ///         LogIn (email, password)
    ///
    /// Form.succeed onSubmit
    ///     |> Form.append emailField
    ///     |> Form.append passwordField
    /// </code>
    ///
    /// In this example, <c>append</c> is used to feed <c>onSubmit</c> function and combine it into a <c>Login</c> message when submitted.
    /// </example>
    let append
        (newForm: Form<'Values, 'A, 'Attributes>)
        (currentForm: Form<'Values, 'A -> 'B, 'Attributes>)
        : Form<'Values, 'B, 'Attributes>
        =

        Base.append newForm currentForm

    /// <summary>
    /// Disable a form
    ///
    /// You can combine this with meta to disable parts of a form based on its own values.
    /// </summary>
    /// <param name="form">The form to disable</param>
    /// <returns>A new form which has been marked as disabled</returns>
    let disable (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A, 'Attributes> =

        Base.disable form

    /// <summary>
    /// Fill a form <c>andThen</c> fill another one.
    ///
    /// This type of form is useful when some part of your form can dynamically change based on the value of another field.
    /// </summary>
    /// <param name="child">The child form</param>
    /// <param name="parent">The parent form which is filled first</param>
    /// <returns>A new form which is the result of filling the <c>parent</c> and then filling the <c>child</c> form</returns>
    /// <example>
    /// <para>Imagine you have a form to create a student or a teacher. Based on the type of user selected you can show the student form or the teacher form.</para>
    /// <code lang="fsharp">
    /// Form.selectField
    ///     {
    ///         Parser = function
    ///             | "student" ->
    ///                 Ok Student
    ///
    ///             | "teacher" ->
    ///                 Ok Teacher
    ///
    ///             | _ ->
    ///                 Error "Invalid user type"
    ///         Value =
    ///             fun values -> values.UserType
    ///         Update =
    ///             fun newValue values ->
    ///                 { values with UserType = newValue }
    ///         Error =
    ///             fun _ -> None
    ///         Attributes =
    ///             {
    ///                 Label = "Type of user"
    ///                 Placeholder = "Choose a user type"
    ///                 Options =
    ///                     [
    ///                         "student", "Student"
    ///                         "teacher", "Teacher"
    ///                     ]
    ///             }
    ///     }
    /// |> Form.andThen (
    ///     function
    ///     | Student ->
    ///         let nameField =
    ///             Form.textField
    ///                 {
    ///                     // ...
    ///                 }
    ///
    ///         Form.succeed NewStudent
    ///             |> Form.append nameField
    ///
    ///     | Teacher ->
    ///         let nameField =
    ///             Form.textField
    ///                 {
    ///                     // ...
    ///                 }
    ///
    ///         let subjectField =
    ///             Form.textField
    ///                 {
    ///                     // ...
    ///                 }
    ///
    ///         let onSubmit name subject =
    ///             NewTeacher (name, subject)
    ///
    ///         Form.succeed onSubmit
    ///             |> Form.append nameField
    ///             |> Form.append subjectField
    /// )
    /// </code>
    /// </example>
    let andThen
        (child: 'A -> Form<'Values, 'B, 'Attributes>)
        (parent: Form<'Values, 'A, 'Attributes>)
        : Form<'Values, 'B, 'Attributes>
        =

        Base.andThen child parent

    /// <summary>
    /// Make a form be <b>optional</b>
    ///
    /// If the form has values set, it will return <c>Some 'Value</c>.
    ///
    /// Otherwise, it returns <c>None</c>.
    /// </summary>
    /// <param name="form">Form to make optional</param>
    /// <example>
    /// <code lang="fsharp">
    /// let emailField =
    ///     Form.emailField
    ///         {
    ///             // ...
    ///         }
    ///
    /// let ageField =
    ///     Form.numberField
    ///         {
    ///             // ...
    ///         }
    ///
    /// let onSubmit =
    ///     fun email ageOpt ->
    ///         LogIn (email, ageOpt)
    ///
    /// Form.succeed onSubmit
    ///     |> Form.append emailField
    ///     |> Form.append (Form.optional ageField)
    /// </code>
    /// </example>
    /// <returns>An optional form</returns>
    let optional (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A option, 'Attributes> =

        Base.optional form

    /// <summary>
    /// Create a form that contains a single text field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a text field</returns>
    let textField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextRaw, x)) config

    /// <summary>
    /// Create a form that contains a single password field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a password field</returns>
    let passwordField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextPassword, x)) config

    /// <summary>
    /// Create a form that contains a single color field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a color field</returns>
    let colorField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextColor, x)) config

    /// <summary>
    /// Create a form that contains a single date field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a date field</returns>
    let dateField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextDate, x)) config

    /// <summary>
    /// Create a form that contains a single datetime-local field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a datetime-local field</returns>
    let dateTimeLocalField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextDateTimeLocal, x)) config

    /// <summary>
    /// Create a form that contains a single number field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a number field</returns>
    let numberField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextNumber, x)) config

    /// <summary>
    /// Create a form that contains a single search field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a search field</returns>
    let searchField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextSearch, x)) config

    /// <summary>
    /// Create a form that contains a single tel field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a tel field</returns>
    let telField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextTel, x)) config

    /// <summary>
    /// Create a form that contains a single time field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a time field</returns>
    let timeField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextTime, x)) config

    /// <summary>
    /// Create a form that contains a single email field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a email field</returns>
    let emailField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextEmail, x)) config

    /// <summary>
    /// Create a form that contains a single textarea field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a textarea field</returns>
    let textareaField
        (config: Base.FieldConfig<TextField.Attributes<'Attributes>, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun x -> Field.Text(TextArea, x)) config

    /// <summary>
    /// Create a form that contains a single checkbox field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a checkbox field</returns>
    let checkboxField
        (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        CheckboxField.form Field.Checkbox config

    /// <summary>
    /// Create a form that contains a single radio field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a radio field</returns>
    let radioField
        (config: Base.FieldConfig<RadioField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        RadioField.form Field.Radio config

    /// <summary>
    /// Create a form that contains a single select field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a select field</returns>
    let selectField
        (config: Base.FieldConfig<SelectField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        SelectField.form Field.Select config

    /// <summary>
    /// Create a form that contains a file field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a file field</returns>
    let fileField
        (config: Base.FieldConfig<FileField.Attributes, Browser.Types.File array, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        FileField.form Field.File config

    /// <summary>
    /// Wrap a form in a group
    ///
    /// The behaviour of the form is not altered but it can be rendered differently.
    ///
    /// For example, Fable.Form.Simple.Bulma will render the groups horizontally.
    ///
    /// See the fields password and repeat password <a href="https://mangelmaxime.github.io/Fable.Form/#sign-up">on this page</a>
    /// </summary>
    /// <param name="form">The form to group</param>
    /// <returns>A form marked as a <c>Group</c> field</returns>
    let group (form: Form<'Values, 'Output, 'Attributes>) : Form<'Values, 'Output, 'Attributes> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Field.Group res.Fields
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

    /// <summary>
    /// Wrap a form in a section
    ///
    /// The behaviour of the form is not altered but it can be rendered differently.
    ///
    /// For example, Fable.Form.Simple.Bulma will the form in section with a border and the title display above.
    ///
    /// An example is available <a href="https://mangelmaxime.github.io/Fable.Form/#dynamic-form">on this page</a>
    /// </summary>
    /// <param name="title">The title to display on the section</param>
    /// <param name="form">The form to group</param>
    /// <returns>A form marked as a <c>Section</c> field</returns>
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

    // type FilledForm<'Output, 'Field, 'Attributes> =
    //    Base.FilledForm<'Output, FilledField<'Field, 'Attributes>>

    /// <summary>
    /// Fill a form with some <c>'Values</c>
    /// </summary>
    /// <param name="form">The form to fill</param>
    /// <param name="values">The values to give to the form</param>
    /// <returns>
    /// - A list of the fields of the form, with their errors
    /// - The result of the filled form which can be:
    ///     - The correct <c>'Output</c>
    ///     - A non-empty list of validation errors
    /// - Whether the form is empty or not
    /// </returns>
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

    /// <summary>
    /// Build a variable list of forms
    ///
    /// An example is available <a href="https://mangelmaxime.github.io/Fable.Form/#form-list">on this page</a>
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FormList.Config"/> for more informations
    /// </para>
    /// </param>
    /// <param name="elementForIndex">A function taking an index and returning a new form</param>
    /// <returns>A form representing the list of form as a single form</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let bookForm (index : int) : Form.Form&gt;BookValues,Book&lt; =
    ///     // ...
    ///
    /// Form.succeed onSubmit
    ///     |> Form.append (
    ///         Form.list
    ///             {
    ///                 Default =
    ///                     {
    ///                         Title = ""
    ///                         Author = ""
    ///                         Summary = ""
    ///                     }
    ///                 Value =
    ///                     fun values -> values.Books
    ///                 Update =
    ///                     fun newValue values ->
    ///                         { values with Books = newValue }
    ///                 Attributes =
    ///                     {
    ///                         Label = "Books"
    ///                         Add = Some "Add book"
    ///                         Delete = Some "Remove book"
    ///                     }
    ///             }
    ///             bookForm
    ///     )
    /// </code>
    ///
    /// In this example, <c>append</c> is used to feed <c>onSubmit</c> function and combine it into a <c>Login</c> message when submitted.
    /// </example>
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

    /// <summary>
    /// Build a form that depends on its own <c>'Values</c>
    ///
    /// This is useful when a field need to checks it's value against another field value.
    ///
    /// The classic example for using <c>meta</c> is when dealing with a repeat password field.
    /// </summary>
    /// <param name="fn">Function to apply to transform the form values</param>
    /// <returns>A new form resulting of the application of <c>fn</c> when filling it</returns>
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

    /// <summary>
    /// Transform the values of a form.
    ///
    /// This function is useful when you want to re-use existing form or nest them.
    /// </summary>
    /// <param name="config">A record used to configure the mapping behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Simple.Form.MapValuesConfig`2"/> for more informations
    /// </para>
    /// </param>
    /// <param name="form">The form to which we want to pass the result of the transformation</param>
    /// <returns>
    /// A new form resulting of <c>fn >> fill form</c>
    /// </returns>
    let mapValues
        ({
             Value = value
             Update = update
         }: MapValuesConfig<'A, 'B>)
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

        let initialize<'T when 'T : (new : unit -> 'T)> () : 'T =
            let instance = new 'T()
            let properties = typeof<'T>.GetProperties()
            for prop in properties do
                if prop.PropertyType = typeof<string> then
                    prop.SetValue(instance, "")
            instance

        let setLoading (formModel: Model<'Values>) =
            { formModel with
                State = Loading
            }

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

            | None ->
                { field with
                    Error = None
                }

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
                            |> List.map (fun
                                             {
                                                 Fields = fields
                                                 Delete = delete
                                             } ->
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
                        OnChange =
                            fun values ->
                                viewConfig.OnChange
                                    { model with
                                        Values = values
                                    }
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
