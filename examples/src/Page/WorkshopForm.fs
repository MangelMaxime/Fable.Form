module Page.WorkshopForm.Component

open Elmish
// Load the API representing how a form should behave and be displayed
open Fable.Form.Simple.View
// Load the different modules coming from our custom implementation
open Fable.Form.WorkshopForm.Base // Give access to the Form composition API
open Fable.Form.WorkshopForm.View // Give access to the Form view API

open Feliz
open Feliz.Bulma

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        UserName: string
        Signature: string
    }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the Login example, we just need to keep track of the Form model state
/// </summary>
type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | Filled of string * string

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Submit of string * string
    // Message sent when the user ask to reset the demo
    | ResetTheDemo

let init () =
    {
        UserName = ""
        Signature = ""
    }
    |> Form.View.idle
    |> FillingForm,
    Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none
        | _ -> model, Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    | Submit(userName, signature) ->
        match model with
        | FillingForm _ -> Filled(userName, signature), Cmd.none
        | _ -> model, Cmd.none

    | ResetTheDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form.Form<Values, Msg, _> =
    let userNameField =
        Form.myInputField
            {
                Parser = Ok
                Value = _.UserName
                Update =
                    fun newValue values ->
                        { values with
                            UserName = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "User name"
                    }
            }

    let signatureField =
        Form.textField
            {
                Parser = Ok
                Value = _.Signature
                Update =
                    fun newValue values ->
                        { values with
                            Signature = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Sign here"
                        Placeholder = "Example: john.doe"
                        HtmlAttributes = []
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun userName signature -> Submit(userName, signature)

    Form.succeed onSubmit |> Form.append userNameField |> Form.append signatureField

let private renderFilledView (userName: string) (signature: string) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [ prop.text $"Thank you %s{userName} for submitting the form" ]
            ]
        ]

        Html.div "We well received your signature showing below:"

        Html.br []

        Html.img [
            prop.src signature
            prop.alt "Signature"
            prop.style [
                style.margin.auto
                style.display.block
                style.borderRadius (length.px 5)
                style.border (1, borderStyle.solid, "hsl(0, 0%, 86%)")
            ]
        ]

        Bulma.text.p [
            text.hasTextCentered
            spacing.mt4

            prop.children [
                Bulma.button.button [
                    prop.onClick (fun _ -> dispatch ResetTheDemo)
                    color.isPrimary

                    prop.text "Reset the demo"
                ]
            ]
        ]

    ]

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        printfn "FillingForm Sign: %A" values.Values

        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Submit"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | Filled(userName, signature) -> renderFilledView userName signature dispatch

let information: DemoInformation.T =
    {
        Title = "WorkshopForm Field"
        Route = Router.Route.WorkshopForm
        Description =
            "This example shows how to create your own custom field if the default fields are not enough for your needs."
        Remark = None
        Code =
            """
Learn more by looking at the source code of the custom field component.
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
