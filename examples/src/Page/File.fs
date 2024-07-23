module Page.File.Component

open Elmish
open Fable.Core
open Feliz
open Feliz.Bulma

open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Field
open Fable.Form.Simple.Bulma

/// <summary>
/// Type used to represent the form values
/// </summary>
[<NoComparison>]
type Values = { Files: Browser.Types.File array }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the File example, we just need to keep track of the Form model state
/// </summary>
[<NoComparison>]
type Model =
    // The form is being filled
    | FillingForm of Form.View.Model<Values>
    // The form has been submitted and the files have been printed in the console
    | FileUploaded of string list

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
[<NoComparison>]
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | SendFiles of Browser.Types.File array
    // Sent when the user ask to reset the demo
    | ResetDemo

let init () =
    { Files = Array.empty } |> Form.View.idle |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FileUploaded _ -> model, Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    | SendFiles files ->
        files |> Array.map (fun file -> file.name) |> Array.toList |> FileUploaded, Cmd.none

    // Reset the demo
    | ResetDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form.Form<Values, Msg, _> =
    let fileField =
        Form.fileField
            {
                Parser = Ok
                Value = fun values -> values.Files
                Update = fun newValue values -> { values with Files = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Invoices"
                        InputLabel = "Choose one or more PDF files"
                        Accept = FileField.FileType.Specific [ ".pdf" ]
                        FileIconClassName = FileField.FileIconClassName.Default
                        Multiple = true
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun (files: Browser.Types.File array) -> SendFiles files

    Form.succeed onSubmit |> Form.append fileField

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm formModel ->
        Html.div
            [
                Bulma.message
                    [
                        color.isInfo
                        prop.children
                            [
                                Bulma.messageBody
                                    [
                                        Html.text
                                            "Files are not uploaded to the server, we are faking it for the demo"
                                    ]
                            ]
                    ]

                Form.View.asHtml
                    {
                        Dispatch = dispatch
                        OnChange = FormChanged
                        Action = Form.View.Action.SubmitOnly "Send"
                        Validation = Form.View.ValidateOnSubmit
                    }
                    form
                    formModel
            ]

    | FileUploaded files ->
        Bulma.content
            [
                Bulma.text.div
                    [ size.isSize6; text.hasTextWeightBold; prop.text "List of files uploaded" ]

                Html.div [ Html.ul (files |> List.map (fun file -> Html.li [ Html.text file ])) ]

                Html.br []

                Bulma.text.p
                    [
                        text.hasTextCentered

                        prop.children
                            [
                                Bulma.button.button
                                    [
                                        prop.onClick (fun _ -> dispatch ResetDemo)
                                        color.isPrimary

                                        prop.text "Reset the demo"
                                    ]
                            ]
                    ]
            ]

let information: DemoInformation.T =
    {
        Title = "File upload"
        Route = Router.Route.File
        Description = "A simple file upload form"
        Remark = None
        Code =
            """
Form.succeed onSubmit
|> Form.append fileField
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
