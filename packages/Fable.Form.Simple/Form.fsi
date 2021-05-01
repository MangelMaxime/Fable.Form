namespace Fable.Form.Simple

module Form =

    open Fable.Form

    type TextField<'Values> = Field.TextField.TextField<'Values>
    type RadioField<'Values> = Field.RadioField.RadioField<'Values>
    type CheckboxField<'Values> = Field.CheckboxField.CheckboxField<'Values>
    type SelectField<'Values> = Field.SelectField.SelectField<'Values>

    /// <summary>
    /// Represents the type of a TextField
    /// </summary>
    type TextType =
        | TextRaw
        | TextPassword
        | TextEmail
        | TextArea

    /// <summary>
    /// DUs used to represents the different of Field supported by Fable.Form.Simple
    /// </summary>
    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Field<'Values> =
        | Text of TextType * TextField<'Values>
        | Radio of RadioField<'Values>
        | Checkbox of CheckboxField<'Values>
        | Select of SelectField<'Values>
        | Group of FilledField<'Values> list
        | Section of title: string * FilledField<'Values> list
        | List of FormList.FormList<'Values,Field<'Values>>

    /// <summary>
    /// Represents a FilledField using Fable.Form.Simple representation
    /// </summary>
    and FilledField<'Values> = Base.FilledField<Field<'Values>>

    /// <summary>
    /// Represents a form using Fable.Form.Simple representation
    /// </summary>
    type Form<'Values,'Output> = Base.Form<'Values,'Output,Field<'Values>>

    /// <summary>
    /// Create a form that always succeeds when filled.
    /// </summary>
    /// <param name="output">The value to return when the form is filled</param>
    /// <returns>The given <c>Output</c></returns>
    val succeed :
        output : 'Output ->
        Form<'Values,'Output>


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
    /// let formOutput =
    ///     fun email password ->
    ///         LogIn (email, password)
    ///
    /// Form.succeed formOutput
    ///     |> Form.append emailField
    ///     |> Form.append passwordField
    /// </code>
    ///
    /// In this example, <c>append</c> is used to feed <c>formOutput</c> function and combine it into a <c>Login</c> message when submitted.
    /// </example>
    val append :
        newForm : Form<'Values,'A> ->
        currentForm : Form<'Values,('A -> 'B)> ->
        Form<'Values,'B>


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
    ///         let formOutput name subject =
    ///             NewTeacher (name, subject)
    ///
    ///         Form.succeed formOutput
    ///             |> Form.append nameField
    ///             |> Form.append subjectField
    /// )
    /// </code>
    /// </example>
    val andThen :
        child : ('A -> Form<'Values,'B>) ->
        parent : Form<'Values,'A> -> Form<'Values,'B>

    /// <summary>
    /// Create a form that contains a single text field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a text field</returns>
    val textField :
        config : Base.FieldConfig<Field.TextField.Attributes,string,'Values,'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single password field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a password field</returns>
    val passwordField :
        config : Base.FieldConfig<Field.TextField.Attributes,string,'Values,'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single email field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a email field</returns>
    val emailField :
        config : Base.FieldConfig<Field.TextField.Attributes,string,'Values,'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single textarea field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a textarea field</returns>
    val textareaField :
        config : Base.FieldConfig<Field.TextField.Attributes,string,'Values,'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single checkbox field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a checkbox field</returns>
    val checkboxField :
        config : Base.FieldConfig<Field.CheckboxField.Attributes,bool,'Values, 'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single radio field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a radio field</returns>
    val radioField :
        config : Base.FieldConfig<Field.RadioField.Attributes,string,'Values,'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Create a form that contains a single select field
    /// </summary>
    /// <param name="config">A record used to configure the field behaviour.
    /// <para>
    /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
    /// </para>
    /// </param>
    /// <returns>Returns a form representing a select field</returns>
    val selectField :
        config : Base.FieldConfig<Field.SelectField.Attributes,string,'Values, 'Output> ->
        Form<'Values,'Output>

    /// <summary>
    /// Wrap a form in a group
    ///
    /// The behaviour of the form is not altered but it can be rendered differently.
    ///
    /// For example, Fable.Form.Simple.Feliz.Bulma will render the groups horizontally.
    ///
    /// See the fields password and repeat password <a href="https://mangelmaxime.github.io/Fable.Form/#sign-up">on this page</a>
    /// </summary>
    /// <param name="form">The form to group</param>
    /// <returns>A form marked as a <c>Group</c> field</returns>
    val group :
        form : Form<'Values,'Output> -> Form<'Values,'Output>

    /// <summary>
    /// Wrap a form in a section
    ///
    /// The behaviour of the form is not altered but it can be rendered differently.
    ///
    /// For example, Fable.Form.Simple.Feliz.Bulma will the form in section with a border and the title display above.
    ///
    /// An example is available <a href="https://mangelmaxime.github.io/Fable.Form/#dynamic-form">on this page</a>
    /// </summary>
    /// <param name="title">The title to display on the section</param>
    /// <param name="form">The form to group</param>
    /// <returns>A form marked as a <c>Section</c> field</returns>
    val section :
        title : string ->
        form : Form<'Values,'Output> ->
        Form<'Values,'Output>

    //type FilledForm<'Output, 'Field> =
    //    Base.FilledForm<'Output, FilledField<'Field>>

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
    val fill :
        form : Form<'Values,'Output> ->
        values : 'Values -> {| Fields: Base.FilledField<Field<'Values>> list; IsEmpty: bool; Result: Result<'Output,(Error.Error * Error.Error list)> |}

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
    /// Form.succeed formOutput
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
    /// In this example, <c>append</c> is used to feed <c>formOutput</c> function and combine it into a <c>Login</c> message when submitted.
    /// </example>
    val list :
        config          : FormList.Config<'Values,'ElementValues> ->
        elementForIndex : (int -> Form<'ElementValues,'Output>) ->
        Form<'Values,'Output list>

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
    val meta :
        fn : ('Values -> Form<'Values,'Output>) ->
        Form<'Values,'Output>

    [<NoComparison; NoEquality>]
    type MapValuesConfig<'A,'B> =
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
    val mapValues :
        config : MapValuesConfig<'A,'B> ->
        form : Form<'B,'Output> ->
        Form<'A,'Output>

    module View =
        type State =
            | Idle
            | Loading
            | Error of string
            | Success of string

        type ErrorTracking =
            | ErrorTracking of {| ShowAllErrors: bool; ShowFieldError: Set<string> |}

        type Model<'Values> =
            {
                Values: 'Values
                State: State
                ErrorTracking: ErrorTracking
            }

        type Validation =
            | ValidateOnBlur
            | ValidateOnSubmit

        [<NoComparison; NoEquality>]
        type ViewConfig<'Values,'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnChange: Model<'Values> -> 'Msg
                Action: string
                Loading: string
                Validation: Validation
            }

        [<NoComparison; NoEquality>]
        type FormConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnSubmit: 'Msg option
                State: State
                Action: string
                Loading: string
                Fields: Feliz.ReactElement list
            }

        [<NoComparison; NoEquality>]
        type TextFieldConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: Field.TextField.Attributes
            }

        [<NoComparison; NoEquality>]
        type CheckboxFieldConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnChange: bool -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: bool
                Error: Error.Error option
                ShowError: bool
                Attributes: Field.CheckboxField.Attributes
            }

        [<NoComparison; NoEquality>]
        type RadioFieldConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: Field.RadioField.Attributes
            }

        [<NoComparison; NoEquality>]
        type SelectFieldConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                OnChange: string -> 'Msg
                OnBlur: 'Msg option
                Disabled: bool
                Value: string
                Error: Error.Error option
                ShowError: bool
                Attributes: Field.SelectField.Attributes
            }

        [<NoComparison; NoEquality>]
        type FormListConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                Forms: Feliz.ReactElement list
                Label: string
                Add: {| Action: (unit -> 'Msg); Label: string |} option
                Disabled: bool
            }

        [<NoComparison; NoEquality>]
        type FormListItemConfig<'Msg> =
            {
                Dispatch: Elmish.Dispatch<'Msg>
                Fields: Feliz.ReactElement list
                Delete: {| Action: (unit -> 'Msg); Label: string |} option
                Disabled: bool
            }

        val idle : values:'Values -> Model<'Values>

        val setLoading : formModel:Model<'Values> -> Model<'Values>

        [<NoComparison; NoEquality>]
        type CustomConfig<'Msg> =
            {
                Form: FormConfig<'Msg> -> Feliz.ReactElement
                TextField: TextFieldConfig<'Msg> -> Feliz.ReactElement
                PasswordField: TextFieldConfig<'Msg> -> Feliz.ReactElement
                EmailField: TextFieldConfig<'Msg> -> Feliz.ReactElement
                TextAreaField: TextFieldConfig<'Msg> -> Feliz.ReactElement
                CheckboxField: CheckboxFieldConfig<'Msg> -> Feliz.ReactElement
                RadioField: RadioFieldConfig<'Msg> -> Feliz.ReactElement
                SelectField: SelectFieldConfig<'Msg> -> Feliz.ReactElement
                Group: Feliz.ReactElement list -> Feliz.ReactElement
                Section: string -> Feliz.ReactElement list -> Feliz.ReactElement
                FormList: FormListConfig<'Msg> -> Feliz.ReactElement
                FormListItem: FormListItemConfig<'Msg> -> Feliz.ReactElement
            }

        [<NoComparison; NoEquality>]
        type FieldConfig<'Values,'Msg> =
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

        val errorToString : error:Error.Error -> string

        val ignoreChildError :
            parentError : Error.Error option ->
            field : FilledField<'Values> ->
            FilledField<'Values>

        val renderField :
            dispatch : Elmish.Dispatch<'Msg> ->
            customConfig : CustomConfig<'Msg> ->
            fieldConfig : FieldConfig<'Values,'Msg> ->
            field : FilledField<'Values> ->
            Feliz.ReactElement

        val custom :
            config : CustomConfig<'Msg> ->
            viewConfig : ViewConfig<'Values,'Msg> ->
            form : Form<'Values,'Msg> ->
            model : Model<'Values> ->
            Feliz.ReactElement
