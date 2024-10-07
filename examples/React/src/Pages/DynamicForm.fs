module Page.DynamicForm.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Examples.Shared.Forms
open DynamicForm.Domain

type Model =
    // The form is being filled
    | FillingForm of Form.View.Model<DynamicForm.Values>
    // The form has been submitted and a teacher has been created
    | CreatedATeacher of string * string
    // The form has been submitted and a student has been created
    | CreatedAStudent of string

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<DynamicForm.Values>
    | FormFilled of DynamicForm.FormResult
    // Sent when the user ask to reset the demo
    | ResetDemo

let init () =
    DynamicForm.init |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | CreatedAStudent _
        | CreatedATeacher _ -> model, Cmd.none

    | FormFilled formResult ->
        match formResult with
        | DynamicForm.FormResult.NewStudent name -> CreatedAStudent name, Cmd.none
        | DynamicForm.FormResult.NewTeacher(name, subject) ->
            CreatedATeacher(name, subject), Cmd.none

    | ResetDemo -> init ()

// Function used to render the result of the form (when submitted)
let private renderResultView (messageBody: ReactElement) dispatch =
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
let private renderStudentView (name: string) dispatch =
    let messageBody =
        Bulma.messageBody [
            Html.text "A new student has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
        ]

    renderResultView messageBody dispatch

// Function used to render the view when a teacher has been created
let private renderTeacherView (name: string) (subject: string) dispatch =
    let messageBody =
        Bulma.messageBody [
            Html.text "A new teacher has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
            Html.br []
            Html.text "He is teaching: "
            Html.b subject
        ]

    renderResultView messageBody dispatch

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        let actionText =
            match UserType.tryParse values.Values.UserType with
            | Ok Student -> "Create student"

            | Ok Teacher -> "Create teacher"

            | Error _ -> "Create"

        Form.View.asHtml
            {
                OnChange = FormChanged >> dispatch
                OnSubmit = FormFilled >> dispatch
                Action = Form.View.Action.SubmitOnly actionText
                Validation = Form.View.ValidateOnSubmit
            }
            DynamicForm.form
            values

    | CreatedAStudent name -> renderStudentView name dispatch

    | CreatedATeacher(name, subject) -> renderTeacherView name subject dispatch
