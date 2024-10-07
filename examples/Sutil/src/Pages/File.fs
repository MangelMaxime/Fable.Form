module Examples.Sutil.Pages.File

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms

[<RequireQualifiedAccess; NoComparison>]
type private State =
    // The form is being filled
    | Filling of Form.View.Model<File.Values>
    // The form has been submitted and the files have been printed in the console
    | FileUploaded of string list

let private renderFilling (stateStore: IStore<State>) (formValues: Form.View.Model<File.Values>) =

    let onSubmit (files: Browser.Types.File array) =
        files
        |> Array.map (fun file -> file.name)
        |> Array.toList
        |> State.FileUploaded
        |> Store.set stateStore

    Html.div [
        bulma.message [
            color.isInfo

            bulma.messageBody [
                Html.text "Files are not uploaded to the server, we are faking it for the demo"
            ]
        ]

        Form.View.asHtml
            {
                OnChange = State.Filling >> (Store.set stateStore)
                OnSubmit = onSubmit
                Action = Form.View.Action.SubmitOnly "Send"
                Validation = Form.View.ValidateOnSubmit
            }
            File.form
            formValues
    ]

let private renderFileUploaded (files: string list) (resetDemo: unit -> unit) =
    bulma.content [
        bulma.text.div [
            size.isSize6
            text.hasTextWeightBold
            prop.text "List of files uploaded"
        ]

        Html.div [
            Html.ul (
                files
                |> List.map (fun file ->
                    Html.li [
                        Html.text file
                    ]
                )
            )
        ]

        Html.br []

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
    let stateStore = File.init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues -> renderFilling stateStore formValues

            | State.FileUploaded files ->
                renderFileUploaded
                    files
                    (fun () -> File.init |> State.Filling |> Store.set stateStore)
    )
