module Page.DynamicForm.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Feliz.Bulma

/// <summary>
/// Represent the form values
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

module UserType =
    
    let tryParse (text : string) =
        match text with
        | "student" ->
            Ok Student

        | "teacher" ->
            Ok Teacher

        | _ ->
            Error "Invalid user type"

type Model =
    // The form is being filled
    | FillingForm of Form.View.Model<Values>
    // The form has been submitted and a teacher has been created
    | CreatedATeacher of string * string
    // The form has been submitted and a student has been created
    | CreatedAStudent of string

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<Values>
    // The user is submitting the form and is asking to create a student
    | NewStudent of string
    // The user is submitting the form and is asking to create a teacher
    | NewTeacher of string * string
    // Sent when the user ask to reset the demo
    | ResetDemo

let init () =
    {
        UserType = ""
        Name = ""
        Subject = ""
    }
    |> Form.View.idle
    |> FillingForm
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ ->
            FillingForm newModel
            , Cmd.none

        | CreatedAStudent _ 
        | CreatedATeacher _ ->
            model
            , Cmd.none

    | NewStudent name ->
        match model with
        | FillingForm _ ->
            CreatedAStudent name
            , Cmd.none

        | CreatedAStudent _ 
        | CreatedATeacher _ ->
            model
            , Cmd.none

    | NewTeacher (name, subject) ->
        match model with
        | FillingForm _ ->
            CreatedATeacher (name, subject)
            , Cmd.none

        | CreatedAStudent _ 
        | CreatedATeacher _ ->
            model
            , Cmd.none

    | ResetDemo ->
        init ()


let private studentForm =
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

let private teacherForm =
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

    let onSubmit name subject =
        NewTeacher (name, subject)

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
let private form : Form.Form<Values, Msg> =
    let userTypeField =
        Form.selectField
            {
                Parser = 
                    UserType.tryParse
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

// Function used to render the result of the form (when submitted)
let private renderResultView (messageBody : ReactElement) dispatch =
    Bulma.content [
    
        Bulma.message [
            color.isSuccess

            prop.children [
                messageBody
            ]

        ]
            
        Bulma.text.p [
            text.hasTextCentered

            prop.children [
                Bulma.button.button [
                    prop.onClick (fun _ -> dispatch ResetDemo)
                    color.isPrimary

                    prop.text "Reset the demo"
                ]
            ]
        ]

    ]

// Function used to render the view when a student has been created
let private renderStudentView (name : string) dispatch =
    let messageBody =
        Bulma.messageBody [
            Html.text "A new student has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
        ]

    renderResultView messageBody dispatch

// Function used to render the view when a teacher has been created
let private renderTeacherView (name : string) (subject : string) dispatch = 
    let messageBody =
        Bulma.messageBody [
            Html.text "A new teacher has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
            Html.br [ ]
            Html.text "He is teaching: "
            Html.b subject
        ]

    renderResultView messageBody dispatch

let view (model : Model) (dispatch : Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        let actionText =
            match UserType.tryParse values.Values.UserType with
            | Ok Student ->
                "Create student"

            | Ok Teacher ->
                "Create teacher"

            | Error _ ->
                "Create"

        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = actionText
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | CreatedAStudent name ->
        renderStudentView name dispatch

    | CreatedATeacher (name, subject) ->
        renderTeacherView name subject dispatch

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

let title =
    "Dynamic form"

let remark =
    None
