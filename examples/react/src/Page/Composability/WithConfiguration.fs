module Page.Composability.WithConfiguration.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma

/// <summary>
/// Represent the form values
/// </summary>
type Values =
    {
        Name: string
        Address: AddressForm.Values
    }

type Model =
    | FillingForm of Form.View.Model<Values>
    | Submitted of User.Name.T * Address.T

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Submit of User.Name.T * Address.T
    // Used when the user ask to reset the demo
    | ResetDemo

let init () =
    {
        Name = ""
        Address = AddressForm.blank
    }
    |> Form.View.idle
    |> FillingForm,
    Cmd.none

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

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let private form: Form<Values, Msg> =
    let nameField =
        Form.textField
            {
                Parser = User.Name.tryParse
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "name"
                        Label = "Name"
                        Placeholder = "Your name"
                        HtmlAttributes = []
                    }
            }

    let onSubmit user address = Submit(user, address)

    Form.succeed onSubmit
    |> Form.append nameField
    |> Form.append (
        AddressForm.form
            {
                Value = fun values -> values.Address
                Update =
                    fun newValue values ->
                        { values with
                            Address = newValue
                        }
            }
    )

// Function used to render a row in the submitted table
let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

// Function used to render the view when the form has been submitted
let private renderSubmittedView (name: User.Name.T) (address: Address.T) dispatch =
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
                    renderRow "Name" (User.Name.toString name)
                    renderRow "Country" (Address.Country.toString address.Country)
                    renderRow "City" (Address.City.toString address.City)
                    renderRow "Postal code" (Address.PostalCode.toString address.PostalCode)
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
                OnSubmit = dispatch
                Action = Form.View.Action.SubmitOnly "Submit"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | Submitted(name, address) -> renderSubmittedView name address dispatch

let information: DemoInformation.T =
    {
        Title = "Composability"
        Remark = None
        Route = Router.Route.Composability Router.ComposabilityRoute.WithConfiguration
        Description = "Demonstrate how you can re-use a form using a 'configuration object'"
        Code =
            """
Form.succeed onSubmit
|> Form.append nameField
|> Form.append (
    AddressForm.form
        {
            Get =
                fun values -> values.Address
            Update =
                fun newValue values ->
                    { values with Address = newValue }
        }
)
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
