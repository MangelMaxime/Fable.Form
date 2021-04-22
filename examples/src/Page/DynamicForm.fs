module Page.DynamicForm

open Warded.Simple
open Elmish

// Student
// Teacher

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        UserType : string
        Name : string
        Subject : string
    }

type UserType
    = Student
    | Teacher

/// <summary>
/// Represents the model of your Elmish component
/// </summary>
type Model =
    Form.View.Model<Values>

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occure in the form
    | FormChanged of Model
    // Used when the user submit the form
    | NewStudent of string
    | NewTeacher of string * string

let init () =
    {
        UserType = ""
        Name = ""
        Subject = ""
    }
    |> Form.View.idle
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        newModel
        , Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    // | LogIn (email, password, rememberMe) ->
    //     // For the example, we just set a message in the Form view
    //     { model with
    //         State = Form.View.Success "You have been logged in successfully"
    //     }
    //     , Cmd.none

let studentForm =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with Name = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Name"
                        Placeholder = "Student name"
                    }
            }

    Form.succeed NewStudent
        |> Form.append nameField
        |> Form.section "Student"

let teacherForm =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with Name = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Name"
                        Placeholder = "Teacher name"
                    }
            }

    let subjectField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Subject
                Update =
                    fun newValue values ->
                        { values with Subject = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Subject"
                        Placeholder = "Taught subject"
                    }
            }

    let formOutput name subject =
        NewTeacher (name, subject)

    Form.succeed formOutput
        |> Form.append nameField
        |> Form.append subjectField
        |> Form.section "Teacher"

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form : Form.Form<Values, Msg> =
    let userTypeField =
        Form.selectField
            {
                Parser = function
                    | "student" ->
                        Ok Student

                    | "teacher" ->
                        Ok Teacher

                    | _ ->
                        Error "Invalid user type"
                Value =
                    fun values -> values.UserType
                Update =
                    fun newValue values ->
                        { values with UserType = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Type of user"
                        Placeholder = "Choose a user type"
                        Options =
                            [
                                "student", "Student"
                                "teacher", "Teacher"
                            ]
                    }
            }

    userTypeField
    |> Form.andThen (
        function
        | Student ->
            studentForm

        | Teacher ->
            teacherForm
    )

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = "Submit"
            Loading = "Loading"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model

let code =
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

let githubLink =
    Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
