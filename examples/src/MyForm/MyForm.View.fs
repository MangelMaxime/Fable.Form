namespace MyForm

open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Field
open MyForm.Field

module Form =

    module View =

        open Elmish
        open Feliz
        open Feliz.Bulma

        let toggleField
            ({
                 Dispatch = dispatch
                 OnChange = onChange
                 OnBlur = onBlur
                 Disabled = disabled
                 Value = value
                 Attributes = attributes
             }: Form.View.ToggleFieldConfig<'Msg>)
            =

            Bulma.control.div
                [
                    Bulma.input.labels.checkbox
                        [
                            prop.children
                                [
                                    Html.text "I am a toggle field (trust me 😉)"

                                    Bulma.input.checkbox
                                        [
                                            prop.onChange (onChange >> dispatch)
                                            match onBlur with
                                            | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                                            | None -> ()
                                            prop.disabled disabled
                                            prop.isChecked value
                                        ]

                                    Html.text attributes.Label
                                ]
                        ]
                ]
            |> (fun x -> [ x ])
            |> Bulma.Form.View.wrapInFieldContainer

        let htmlViewConfig<'Msg> : MyForm.Form.View.CustomConfig<'Msg, IReactProperty> =
            {
                Default = Bulma.Form.View.htmlViewConfig
                Toggle = toggleField
            }

        let asHtml (config: Fable.Form.Simple.View.Form.View.ViewConfig<'Values, 'Msg>) =
            Fable.Form.Simple.View.Form.View.custom
                config
                Bulma.Form.View.form
                (MyForm.Form.View.renderField htmlViewConfig)
