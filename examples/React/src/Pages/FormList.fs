module Page.FormList.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Examples.Shared.Forms

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<FormList.Values>
    // Used when the form has been submitted with success
    | FormFilled of string * FormList.Book list

type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<FormList.Values>
    // Message sent when the form is submitted
    | Submit of string * FormList.Book list
    // Message sent when the user ask to reset the demo
    | ResetDemo

let init () = FormList.init |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | Submit(name, books) ->
        match model with
        | FillingForm _ -> FormFilled(name, books), Cmd.none

        | FormFilled _ -> model, Cmd.none

    | ResetDemo -> init ()

// Function used to render a book when the form has been submitted
let private renderBook (rank: int) (book: FormList.Book) =
    Html.tr [
        Html.td [
            Html.b (string (rank + 1))
        ]
        Html.td book.Title
        Html.td book.Author
        Html.td book.Summary
    ]

// Function used to render the filled view (when the form has been submitted)
let private renderFilledView (name: string) (books: FormList.Book list) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    Html.text "Thank you "
                    Html.b name
                    Html.text " for creating those "
                    Html.b (string (List.length books))
                    Html.text " book(s)"
                ]
            ]

        ]

        Bulma.table [
            table.isStriped
            prop.className "is-vcentered-cells"

            prop.children [
                Html.thead [
                    Html.tr [
                        Html.th "#"
                        Html.th "Title"
                        Html.th "Author"
                        Html.th "Description"
                    ]
                ]

                Html.tableBody (List.mapi renderBook books)
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

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        Form.View.asHtml
            {
                OnChange = FormChanged >> dispatch
                OnSubmit = Submit >> dispatch
                Action = Form.View.Action.SubmitOnly "Submit"
                Validation = Form.View.ValidateOnSubmit
            }
            FormList.form
            values

    | FormFilled(name, books) -> renderFilledView name books dispatch
