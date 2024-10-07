namespace Fable.Form.Simple.Bulma

open Fable.Form
open Fable.Form.Simple.Bulma.Fields

[<RequireQualifiedAccess>]
module Form =

    module View =

        open Fable.Form.Simple

        let asHtml
            (config: Form.View.ViewConfig<'Values, 'Output, Feliz.ReactElement>)
            : Form<'Values, 'Output> -> Form.View.Model<'Values> -> Feliz.ReactElement
            =
            Form.View.custom config Html.View.form Html.View.renderField

    (*
        Combinators functions are used to combine multiple forms together to create a more complex form.

        Below, we redefine some functions from the Base module so the user can access them transparently,
        and they are also specifically typed for the Fable.Form.Simple.Bulma abstraction
    *)

    /// <summary>
    /// Create a form that always succeeds when filled.
    /// </summary>
    /// <param name="output">The value to return when the form is filled</param>
    /// <returns>The given <c>Output</c></returns>
    let succeed (output: 'Output) : Form<'Values, 'Output> = Base.succeed output

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
        (newForm: Form<'Values, 'A>)
        (currentForm: Form<'Values, 'A -> 'B>)
        : Form<'Values, 'B>
        =

        Base.append newForm currentForm

    /// <summary>
    /// Disable a form
    ///
    /// You can combine this with meta to disable parts of a form based on its own values.
    /// </summary>
    /// <param name="form">The form to disable</param>
    /// <returns>A new form which has been marked as disabled</returns>
    let disable (form: Form<'Values, 'A>) : Form<'Values, 'A> =

        Base.disable form

    /// <summary>
    /// Disable a form based on a condition
    ///
    /// You can combine this with meta to disable parts of a form based on its own values.
    /// </summary>
    /// <param name="condition">The condition to check</param>
    /// <param name="form">The form to disable</param>
    /// <returns>A new form which has been marked as disabled if the condition is <c>true</c></returns>
    let disableIf (condition: bool) (form: Form<'Values, 'A>) : Form<'Values, 'A> =
        Base.disableIf condition form

    /// <summary>
    /// Make a form read-only
    ///
    /// You can combine this with meta to make parts of a form read-only based on its own values.
    /// </summary>
    /// <param name="form">The form to make read-only</param>
    /// <returns>A new form which has been marked as read-only</returns>
    let readOnly (form: Form<'Values, 'A>) : Form<'Values, 'A> = Base.readOnly form

    /// <summary>
    /// Make a form read-only based on a condition
    ///
    /// You can combine this with meta to make parts of a form read-only based on its own values.
    /// </summary>
    /// <param name="condition">The condition to check</param>
    /// <param name="form">The form to make read-only</param>
    /// <returns>A new form which has been marked as read-only if the condition is <c>true</c></returns>
    let readOnlyIf (condition: bool) (form: Form<'Values, 'A>) : Form<'Values, 'A> =
        Base.readOnlyIf condition form

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
    let andThen (child: 'A -> Form<'Values, 'B>) (parent: Form<'Values, 'A>) : Form<'Values, 'B> =

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
    let optional (form: Form<'Values, 'A>) : Form<'Values, 'A option> =

        Base.optional form

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
    let meta (fn: 'Values -> Form<'Values, 'Output>) : Form<'Values, 'Output> =

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
    /// See <see cref="T:Fable.Form.Simple.Bulma.Form.MapValuesConfig`2"/> for more informations
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
        (form: Form<'B, 'Output>)
        : Form<'A, 'Output>
        =

        Base.meta (fun values ->
            form
            |> Base.mapValues value
            |> Base.mapField (fun field ->
                let newUpdate oldValues = update oldValues values

                field.MapFieldValues newUpdate
            )
        )

    (*
        Field functions are used to create fields that can be used in a form.
    *)

    let textField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextRaw, field)) config

    let colorField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextColor, field)) config

    let dateField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextDate, field)) config

    let dateTimeLocalField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextDateTimeLocal, field)) config

    let emailField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextEmail, field)) config

    let numberField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextNumber, field)) config

    let passwordField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextPassword, field)) config

    let searchField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextSearch, field)) config

    let telField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextTel, field)) config

    let timeField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextTime, field)) config

    let textareaField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextArea, field)) config

    let checkboxField
        (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        CheckboxField.form (fun field -> CheckboxField.Field field) config

    let radioField
        (config: Base.FieldConfig<RadioField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        RadioField.form (fun field -> RadioField.Field field) config

    let selectField
        (config: Base.FieldConfig<SelectField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        SelectField.form (fun field -> SelectField.Field field) config

    let fileField
        (config: Base.FieldConfig<FileField.Attributes, Browser.Types.File array, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        FileField.form (fun field -> FileField.Field field) config

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
        (elementForIndex: int -> Form<'ElementValues, 'Output>)
        : Form<'Values, 'Output list>
        =

        let fillElement
            (elementState: FormList.ElementState<'Values, 'ElementValues>)
            : Base.FilledForm<'Output, IField<'Values>>
            =
            let filledElement =
                Base.fill (elementForIndex elementState.Index) elementState.ElementValues

            {
                Fields =
                    filledElement.Fields
                    |> List.map (fun filledField ->
                        {
                            State =
                                filledField.State.MapFieldValues(fun oldValues ->
                                    elementState.Update oldValues elementState.Values
                                )
                            Error = filledField.Error
                            IsDisabled = filledField.IsDisabled
                            IsReadOnly = filledField.IsReadOnly
                        }
                    )
                Result = filledElement.Result
                IsEmpty = filledElement.IsEmpty
            }

        let tagger (formList: FormList.InnerField<'Values, IField<'Values>>) : IField<'Values> =
            FormList.Field formList

        FormList.form tagger config fillElement

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
    let section (title: string) (form: Form<'Values, 'Output>) : Form<'Values, 'Output> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = SectionField.Field(title, res.Fields)
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )

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
    let group (form: Form<'Values, 'Output>) : Form<'Values, 'Output> =
        Base.custom (fun values ->
            let res = Base.fill form values

            {
                State = Group.Field res.Fields
                Result = res.Result
                IsEmpty = res.IsEmpty
            }
        )
