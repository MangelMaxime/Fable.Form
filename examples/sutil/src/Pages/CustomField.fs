module Examples.Sutil.Pages.CustomField

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Feliz

// Expose our custom field
// If you have multiple custom fields, you probably want to create a module to expose all of them
// at once instead of opening each field module
// This is up to you to decide on how you want your consumers to use your library
open Examples.Sutil.Fields.SignatureField

/// <summary>
/// Type used to represent the form values
/// </summary>
[<NoComparison>]
type Values =
    {
        UserName: string
        Signature: string
    }

/// <summary>
/// Represents the state of your the component
///
/// In the case of the CustomField example, we just need to keep track of the Form model state
/// </summary>
[<RequireQualifiedAccess; NoComparison>]
type State =
    // The form is being filled
    | Filling of Form.View.Model<Values>
    // The form has been submitted and the files have been printed in the console
    | Filled of string * string

let init =
    {
        UserName = ""
        Signature = ""
    }
    |> Form.View.idle

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let private form: Form<Values, _> =
    let userNameField =
        Form.textField
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
        Form.signatureField
            {
                Parser = Ok
                Value = fun values -> values.Signature
                Update =
                    fun newValue values ->
                        { values with
                            Signature = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "signature"
                        Label = "Signature"
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun userName signature -> (userName, signature)

    Form.succeed onSubmit |> Form.append userNameField |> Form.append signatureField

let private renderFilledView (userName: string) (signature: string) (resetDemo: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                Html.text "Thank you "
                Html.strong userName
                Html.text " for submitting the form"
            ]
        ]

        Html.div "We well received your signature showing below:"

        Html.br []

        Html.img [
            prop.src signature
            prop.alt "Signature"
            prop.style [
                Css.margin length.auto
                Css.displayBlock
                Css.borderRadius (length.px 5)
                Css.border (length.px 1, borderStyle.solid, "hsl(0, 0%, 86%)")
            ]
        ]

        bulma.text.p [
            text.hasTextCentered
            spacing.mt4

            bulma.button.button [
                Ev.onClick (fun _ -> resetDemo ())
                color.isPrimary

                prop.text "Reset the demo"
            ]
        ]

    ]

let Page () =
    let stateStore = init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> Store.set stateStore
                        OnSubmit = State.Filled >> Store.set stateStore
                        Action = Form.View.Action.SubmitOnly "Send"
                        Validation = Form.View.Validation.ValidateOnSubmit
                    }
                    form
                    formValues

            | State.Filled(userName, signature) ->
                renderFilledView
                    userName
                    signature
                    (fun () -> init |> State.Filling |> Store.set stateStore)
    )

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "CustomField"
        Route = Router.Route.CustomField
        Description = "Show case how to create a custom field and use it in a form"
        Remark = None
        Code =
            """
Form.succeed onSubmit
|> Form.append signatureField
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
