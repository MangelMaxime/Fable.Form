module Page.CustomView.Component

open Elmish
open Feliz
open Feliz.Bulma

open Fable.Form.Simple
open Fable.Form.Simple.Bulma

// Expose our custom field
// If you have multiple custom fields, you probably want to create a module to expose all of them
// at once instead of opening each field module
// This is up to you to decide on how you want your consumers to use your library
open Fable.Form.Simple.Bulma.Fields.DaisyTextInput

/// <summary>
/// Type used to represent the form values
/// </summary>
[<NoComparison>]
type Values =
    {
        UserName: string
        Email: string
    }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the CustomView example, we just need to keep track of the Form model state
/// </summary>
[<NoComparison>]
type Model =
    // The form is being filled
    | FillingForm of Form.View.Model<Values>
    // The form has been submitted and the files have been printed in the console
    | FilledForm of string * string

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
[<NoComparison>]
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Register of string * string
    // Sent when the user ask to reset the demo
    | ResetDemo

let init () =
    {
        UserName = ""
        Email = ""
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

        | FilledForm _ -> model, Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    | Register(userName, signature) -> FilledForm(userName, signature), Cmd.none

    // Reset the demo
    | ResetDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, Msg> =
    let userNameField =
        Form.daysiTextInputField
            {
                Parser = Ok
                Value = fun values -> values.UserName
                Update =
                    fun newValue values ->
                        { values with
                            UserName = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "username"
                        Label = "Username"
                        Placeholder = "Type your username"
                        AutoComplete = None
                    }
            }

    let signatureField =
        Form.daysiTextInputField
            {
                Parser = Ok
                Value = fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with
                            Email = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "email"
                        Label = "Email"
                        Placeholder = "Type your email"
                        AutoComplete = None
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun userName signature -> Register(userName, signature)

    Form.succeed onSubmit |> Form.append userNameField |> Form.append signatureField

let private renderFilledView (userName: string) (signature: string) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    Html.text "Thank you for submitting the form"
                ]
            ]
        ]

        Html.ul [
            Html.li [
                Html.text "Username: "
                Html.strong userName
            ]

            Html.li [
                Html.text "Email: "
                Html.strong signature
            ]
        ]

        Bulma.text.p [
            text.hasTextCentered
            spacing.mt4

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
    Html.div [
        // We need to load DaisyUI CSS for this example
        Html.link [
            prop.rel "stylesheet"
            prop.type' "text/css"
            prop.href "https://cdn.jsdelivr.net/npm/daisyui@4.12.12/dist/full.min.css"
        ]

        match model with
        | FillingForm formModel ->
            Form.View.asHtml
                {
                    OnChange = FormChanged >> dispatch
                    OnSubmit = dispatch
                    Action = Form.View.Action.SubmitOnly "Send"
                    Validation = Form.View.ValidateOnSubmit
                }
                form
                formModel

        | FilledForm(userName, signature) -> renderFilledView userName signature dispatch

    ]

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Custom View"
        Route = Router.Route.FrameworkSpecific Router.ReactRoute.CustomView
        Description =
            "Show case how to customize an existing field to use another CSS library (<b>logic stays the same</b>)"
        Remark = None
        Code =
            """
Form.succeed onSubmit
|> Form.append signatureField
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
