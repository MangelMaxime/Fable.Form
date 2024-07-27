namespace MyForm

open Fable.Form.Simple
open Fable.Form.Simple.View

module View =

    open Elmish
    open Feliz
    open Feliz.Bulma

    open System
    open Fable.Core.JsInterop
    open Glutinum.SignaturePad
    open Glutinum.Feliz.ReactSignaturePadWrapper

    open type Glutinum.Feliz.ReactSignaturePadWrapper.Exports

    [<ReactComponent>]
    let ToggleField
        ({
             Dispatch = dispatch
             OnChange = onChange
             OnBlur = onBlur
             Disabled = disabled
             Value = value
             Attributes = attributes
         }: Form.View.ToggleFieldConfig<'Msg>)
        =
        let signaturePadRef = React.useRef<SignaturePad option> None

        React.useEffect (fun _ ->
            let callback = fun _ -> printfn "SignaturePad stroke end"

            match signaturePadRef.current with
            | Some signaturePad -> signaturePad.addEventListener ("endStroke", unbox callback)
            | None -> ()

            { new IDisposable with
                member this.Dispose() =
                    match signaturePadRef.current with
                    | Some signaturePad ->
                        signaturePad.removeEventListener ("endStroke", unbox callback)
                    | None -> ()
            }
        )

        Bulma.control.div
            [
                SignaturePad
                    [
                        // signaturePad.ref signaturePadRef
                        signaturePad.redrawOnResize true
                    ]
            ]
        |> List.singleton
        |> Bulma.Form.View.wrapInFieldContainer

    let htmlViewConfig<'Msg> : MyForm.Form.View.CustomConfig<'Msg, IReactProperty> =
        {
            Standard = Bulma.Form.View.htmlViewConfig
            Toggle = ToggleField
        }

    let asHtml (config: Form.View.ViewConfig<'Values, 'Msg>) =
        Form.View.custom config Bulma.Form.View.form (MyForm.Form.View.renderField htmlViewConfig)
