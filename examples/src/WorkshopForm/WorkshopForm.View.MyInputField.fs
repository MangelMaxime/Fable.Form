module Fable.Form.WorkshopForm.View.MyInputField

open Fable.Form.Simple
open Fable.Form.WorkshopForm

open Feliz
open Feliz.Bulma

open System
open Fable.Core.JsInterop

let render
    ({
         Dispatch = dispatch
         OnChange = onChange
         OnBlur = onBlur
         Disabled = disabled
         Value = value
         Error = error
         ShowError = showError
         Attributes = attributes
     }: Base.Form.View.MyInputFieldConfig<'Msg>)
    =

    Bulma.control.div [
        Html.div "Hello form custom workfshop form"
        Bulma.input.text [
            prop.onChange (onChange >> dispatch)
            prop.value value
            prop.disabled disabled

            match onBlur with
            | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

            | None -> ()

            if showError && error.IsSome then
                color.isDanger
        ]
    ]
    |> Bulma.Form.View.withLabelAndError attributes.Label showError error
