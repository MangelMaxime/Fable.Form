module Examples.Sutil.Pages.DynamicForm

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<DynamicForm.Values>
    | Filled of DynamicForm.FormResult

// Function used to render the result of the form (when submitted)
let private renderResultView (messageBody: Core.SutilElement) (resetDemo: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess
            messageBody
        ]

        bulma.text.p [
            text.hasTextCentered

            bulma.button.button [
                Ev.onClick (fun _ -> resetDemo ())
                color.isPrimary

                prop.text "Reset the demo"
            ]
        ]

    ]

// Function used to render the view when a student has been created
let private renderStudentView (name: string) dispatch =
    let messageBody =
        bulma.messageBody [
            Html.text "A new student has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
        ]

    renderResultView messageBody dispatch

// Function used to render the view when a teacher has been created
let private renderTeacherView (name: string) (subject: string) dispatch =
    let messageBody =
        bulma.messageBody [
            Html.text "A new teacher has been created"
            Html.br []
            Html.text "His name is: "
            Html.b name
            Html.br []
            Html.text "He is teaching: "
            Html.b subject
        ]

    renderResultView messageBody dispatch

let Page () =
    let stateStore = DynamicForm.init |> State.Filling |> Store.make

    let resetDemo () =
        DynamicForm.init |> State.Filling |> Store.set stateStore

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> (Store.set stateStore)
                        OnSubmit = State.Filled >> Store.set stateStore
                        Action = Form.View.Action.SubmitOnly "Sign in"
                        Validation = Form.View.ValidateOnSubmit
                    }
                    DynamicForm.form
                    formValues

            | State.Filled formResult ->
                match formResult with
                | DynamicForm.NewStudent name -> renderStudentView name resetDemo
                | DynamicForm.NewTeacher(name, subject) -> renderTeacherView name subject resetDemo
    )
