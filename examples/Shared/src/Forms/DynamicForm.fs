module Examples.Shared.Forms.DynamicForm

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Bulma.Fields
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma.Fields
open Fable.Form.Simple.Fields.Html
#endif

// Domain used by the current exemple, in a real application, this should be in a separate file
// But to make the example easier to understand, we keep it here
//
// If you are not familiar with the domain logic, you can ignore this part
// and focus on the form part
//
// Later on, you can come back to this part to understand how the domain logic
// is used in the form.
//
// The "Designing with types" series from F# for fun and profit is a great resource
// to learn more about this topic: https://fsharpforfunandprofit.com/series/designing-with-types/
[<AutoOpen>]
module Domain =

    [<RequireQualifiedAccess>]
    type UserType =
        | Student
        | Teacher

        interface SelectField.OptionItem with
            member this.Key =
                match this with
                | Student -> "student"
                | Teacher -> "teacher"

            member this.Text =
                match this with
                | Student -> "Student"
                | Teacher -> "Teacher"

type FormResult =
    | NewStudent of name: string
    | NewTeacher of name: string * subject: string

/// <summary>
/// Represent the form values
/// </summary>
[<NoComparison>]
type Values =
    {
        UserType: SelectField.OptionItem option
        Name: string
        Subject: string
    }

let init =
    {
        UserType = None
        Name = ""
        Subject = ""
    }
    |> Form.View.idle

let private studentForm =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes = TextField.create "student-name" |> TextField.withLabel "Name"
            }

    Form.succeed NewStudent |> Form.append nameField |> Form.section "Student"

let private teacherForm =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes = TextField.create "teacher-name" |> TextField.withLabel "Name"
            }

    let subjectField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Subject
                Update =
                    fun newValue values ->
                        { values with
                            Subject = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    TextField.create "teacher-subject"
                    |> TextField.withLabel "Subject"
                    |> TextField.withPlaceholder "Taught subject"
            }

    let onSubmit name subject = NewTeacher(name, subject)

    Form.succeed onSubmit
    |> Form.append nameField
    |> Form.append subjectField
    |> Form.section "Teacher"

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, FormResult> =
    let userTypeField =
        Form.selectField
            {
                Parser =
                    fun value ->
                        match value with
                        | None -> Error "User type is required"
                        | Some value -> value :?> UserType |> Ok
                Value = fun values -> values.UserType
                Update =
                    fun newValue values ->
                        { values with
                            UserType = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    SelectField.create "user-type"
                    |> SelectField.withLabel "Type of user"
                    |> SelectField.withPlaceholder "Choose a user type"
                    |> SelectField.withOptions [
                        UserType.Student
                        UserType.Teacher
                    ]
            }

    userTypeField
    |> Form.andThen (
        function
        | UserType.Student -> studentForm

        | UserType.Teacher -> teacherForm
    )

let information<'R> : DemoInformation<_> =
    {
        Title = "Dynamic form"
        Remark = None
        Route = SharedRouter.Route.DynamicForm
        Description = "A form that changes dynamically based on its own values"
        Code =
            """
userTypeField
|> Form.andThen (
    function
    | Student ->
        studentForm

    | Teacher ->
        teacherForm
)
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
