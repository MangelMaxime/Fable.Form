module Examples.Sutil.Pages.CustomView

open Sutil
open Sutil.Bulma

open Fable.Form.Simple
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Sutil.Bulma

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

let init =
    {
        UserName = ""
        Email = ""
    }
    |> Form.View.idle

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the CustomView example, we just need to keep track of the Form model state
/// </summary>
[<RequireQualifiedAccess; NoComparison>]
type State =
    // The form is being filled
    | Filling of Form.View.Model<Values>
    // The form has been submitted and the files have been printed in the console
    | Filled of string * string

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, _> =
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
                Attributes = TextField.create "username" |> TextField.withLabel "Username"
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
                Attributes = TextField.create "email" |> TextField.withLabel "Email"
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
                Html.text "Thank you for submitting the form"
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

let daisySubmit (state: Form.View.State) =
    Html.div [
        prop.className "tw-float-right"
        Html.button [
            prop.type' "submit"
            prop.className "tw-daisy-btn tw-daisy-btn-primary"
            prop.disabled (state = Form.View.Loading: bool)
            if state = Form.View.Loading then
                Html.span [
                    prop.className "tw-daisy-loading tw-daisy-loading-spinner"
                ]

            prop.text "Submit"
        ]
    ]

let Page () =
    let stateStore = init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            Html.div [
                // Scope DaisyUI/Tailwind CSS to this component
                prop.className "daisyui"

                match state with
                | State.Filling formModel ->
                    Form.View.asHtml
                        {
                            OnChange = State.Filling >> Store.set stateStore
                            OnSubmit = State.Filled >> Store.set stateStore
                            Action = Form.View.Action.Custom daisySubmit
                            Validation = Form.View.ValidateOnSubmit
                        }
                        form
                        formModel

                | State.Filled(userName, signature) ->
                    renderFilledView
                        userName
                        signature
                        (fun () -> init |> State.Filling |> Store.set stateStore)
            ]
    )

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Custom View"
        Route = Router.Route.FrameworkSpecific Router.SutilRoute.CustomView
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
