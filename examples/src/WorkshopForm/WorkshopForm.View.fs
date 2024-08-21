module Fable.Form.WorkshopForm.View

open Fable.Form.Simple.View
open Fable.Form.Simple
open Feliz

[<RequireQualifiedAccess>]
module Form =

    module View =

        let htmlViewConfig<'Msg> : Base.Form.View.CustomConfig<'Msg, IReactProperty> =
            {
                Standard = Bulma.Form.View.htmlViewConfig
                MyInputField = View.MyInputField.render
            }

        let asHtml (config: Form.View.ViewConfig<'Values, 'Msg>) =
            Form.View.custom config Bulma.Form.View.form (Base.Form.View.renderField htmlViewConfig)
