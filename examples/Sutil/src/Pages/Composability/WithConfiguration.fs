module Examples.Sutil.Pages.Composability.WithConfiguration

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms
open Composability.Domain

[<RequireQualifiedAccess; NoComparison>]
type State =
    | Filling of Form.View.Model<Composability.WithConfiguration.Values>
    | Filled of Name * Composability.Domain.Address

// Function used to render a row in the submitted table
let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

// Function used to render the view when the form has been submitted
let private renderSubmittedView (name: Name) (address: Address) (resetDemo: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                prop.text "Entry has been created"
            ]
        ]

        bulma.table [
            table.isStriped

            Html.thead [
                Html.tr [
                    Html.th "Field"
                    Html.th "Value"
                ]
            ]

            Html.tableBody [
                renderRow "Name" (Name.value name)
                renderRow "Country" (Address.Country.value address.Country)
                renderRow "City" (Address.City.value address.City)
                renderRow "Postal code" (Address.PostalCode.value address.PostalCode)
            ]
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
    let stateStore = Composability.WithConfiguration.init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling form ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> (Store.set stateStore)
                        OnSubmit = State.Filled >> Store.set stateStore
                        Action = Form.View.Action.SubmitOnly "Submit"
                        Validation = Form.View.ValidateOnSubmit
                    }
                    Composability.WithConfiguration.form
                    form

            | State.Filled(name, address) ->
                renderSubmittedView
                    name
                    address
                    (fun _ ->
                        Composability.WithConfiguration.init
                        |> State.Filling
                        |> Store.set stateStore
                    )
    )
