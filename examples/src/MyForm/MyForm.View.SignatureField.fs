module Fable.Form.MyForm.View.SignatureField

open Fable.Form.Simple
open Fable.Form.MyForm

open Feliz
open Feliz.Bulma

open System
open Fable.Core.JsInterop
open Glutinum.SignaturePad
open Glutinum.Feliz.ReactSignaturePadWrapper

open type Glutinum.Feliz.ReactSignaturePadWrapper.Exports

[<ReactComponent>]
let SignatureField
    ({
         Dispatch = dispatch
         OnChange = onChange
         OnBlur = _
         Disabled = disabled
         Value = value
         Error = error
         ShowError = showError
         Attributes = attributes
     }: Base.Form.View.SignatureFieldConfig<'Msg>)
    =
    let signaturePadRef = React.useRef<SignaturePad option> None

    // Register event listener to send back the signature data when the user finish drawing
    React.useEffect (
        fun _ ->
            let callback =
                fun _ ->
                    let signaturePad = signaturePadRef.current.Value

                    { value with
                        CurrentSignature = signaturePad.toDataURL ()
                        History = value.History @ [ value.CurrentSignature ]
                    }
                    |> onChange
                    |> dispatch

            match signaturePadRef.current with
            | Some signaturePad ->
                signaturePad?signaturePad?addEventListener ("endStroke", unbox callback)
            | None -> ()

            { new IDisposable with
                member _.Dispose() =
                    match signaturePadRef.current with
                    | Some signaturePad ->
                        signaturePad?signaturePad?removeEventListener ("endStroke", unbox callback)
                    | None -> ()
            }
        , [|
            box signaturePadRef
            box dispatch
        |]
    )

    // Set the signature data when the value change
    // This allows to have an initial signature for example or reload a form with a signature
    React.useEffect (
        fun _ ->
            match signaturePadRef.current with
            | Some signaturePad ->
                signaturePad.clear ()
                signaturePad.fromDataURL (value.CurrentSignature) |> ignore
            | None -> ()

        , [| box value.CurrentSignature |]
    )

    Bulma.control.div [
        SignaturePad [
            signaturePad.ref signaturePadRef
            signaturePad.redrawOnResize true
            signaturePad.canvasProps [
                prop.style [
                    style.margin.auto
                    style.display.block
                    style.borderRadius (length.px 5)
                    style.border (1, borderStyle.solid, "hsl(0, 0%, 86%)")
                    // If the field is disabled, we can disable pointer events to
                    // make the canvas not interactable
                    if disabled then
                        style.backgroundColor "hsl(0, 0%, 96%)"
                        style.pointerEvents.none
                ]
            ]
            signaturePad.height 200
            signaturePad.width 600
        ]

        Bulma.button.a [
            prop.text "Clear"
            prop.onClick (fun _ -> signaturePadRef.current.Value.clear ()
            // signaturePadRef.current.Value.toDataURL () |> onChange |> dispatch
            )
        ]

        Bulma.buttons [
            Bulma.button.span [
                prop.text "Undo"
                prop.onClick (fun _ ->
                    let history = value.History

                    match history with
                    | [] -> ()
                    | _ ->
                        let last = List.last history
                        let newHistory = List.take (List.length history - 1) history

                        let newValue =
                            { value with
                                History = newHistory
                                CurrentSignature = last
                            }

                        newValue |> onChange |> dispatch
                )
            ]
        ]
    ]
    |> Bulma.Form.View.withLabelAndError attributes.Label showError error
