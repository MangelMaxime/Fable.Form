module Page.Composability.WithConfiguration.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Examples.Shared.Forms
open Composability.Domain

type Model =
    | FillingForm of Form.View.Model<Composability.WithConfiguration.Values>
    | Submitted of Name * Address

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<Composability.WithConfiguration.Values>
    // Used when the user submit the form
    | Submit of Name * Address
    // Used when the user ask to reset the demo
    | ResetDemo

let init () =
    Composability.WithConfiguration.init |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged formModel ->
        match model with
        | FillingForm _ -> FillingForm formModel, Cmd.none

        | Submitted _ -> model, Cmd.none

    | Submit(name, address) ->
        match model with
        | FillingForm _ -> Submitted(name, address), Cmd.none

        | Submitted _ -> model, Cmd.none

    | ResetDemo -> init ()

// Function used to render a row in the submitted table
let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

// Function used to render the view when the form has been submitted
let private renderSubmittedView (name: Name) (address: Address) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    prop.text "Entry has been created"
                ]
            ]
        ]

        Bulma.table [
            table.isStriped

            prop.children [
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
            Composability.WithConfiguration.form
            values

    | Submitted(name, address) -> renderSubmittedView name address dispatch
