module Examples.Sutil.Pages.FormList

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<FormList.Values>
    | Filled of string * FormList.Book list

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
let private renderFilledView (name: string) (books: FormList.Book list) (resetDemo: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                Html.text "Thank you "
                Html.b name
                Html.text " for creating those "
                Html.b (string (List.length books))
                Html.text " book(s)"
            ]

        ]

        bulma.table [
            table.isStriped
            prop.className "is-vcentered-cells"

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

        bulma.text.p [
            text.hasTextCentered

            bulma.button.button [
                Ev.onClick (fun _ -> resetDemo ())
                color.isPrimary

                prop.text "Reset the demo"
            ]
        ]

    ]

let Page () =
    let stateStore = FormList.init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> (Store.set stateStore)
                        OnSubmit = State.Filled >> Store.set stateStore
                        Action = Form.View.Action.SubmitOnly "Submit"
                        Validation = Form.View.ValidateOnSubmit
                    }
                    FormList.form
                    formValues

            | State.Filled(name, books) ->
                renderFilledView
                    name
                    books
                    // Reset the demo
                    (fun () -> FormList.init |> State.Filling |> Store.set stateStore)
    )
