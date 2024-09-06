namespace Fable.Form.Simple.Bulma.Fields.SignatureField

open System
open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma
open Fable.Core.JsInterop
open Glutinum.SignaturePad
open Glutinum.Feliz.ReactSignaturePadWrapper

open type Glutinum.Feliz.ReactSignaturePadWrapper.Exports

module SignatureField =

    type Attributes =
        {
            FieldId: string
            Label: string
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field String.IsNullOrEmpty

    [<ReactComponent>]
    let SignatureFieldComponent (config: StandardRenderFieldConfig<'Msg, string, Attributes>) =
        let signaturePadRef = React.useRef<SignaturePad option> None

        // Register event listener to send back the signature data when the user finish drawing
        React.useEffect (
            fun _ ->
                let callback =
                    fun _ ->
                        let signaturePad = signaturePadRef.current.Value
                        signaturePad.toDataURL () |> config.OnChange |> config.Dispatch

                match signaturePadRef.current with
                | Some signaturePad ->
                    signaturePad?signaturePad?addEventListener ("endStroke", unbox callback)
                | None -> ()

                { new IDisposable with
                    member _.Dispose() =
                        match signaturePadRef.current with
                        | Some signaturePad ->
                            signaturePad?signaturePad?removeEventListener (
                                "endStroke",
                                unbox callback
                            )
                        | None -> ()
                }
            , [|
                box signaturePadRef
                // We need to re-register the event listener when onChange or dispatch change
                // otherwise, we will have a closure on the old values and will lose some state
                // when updating the signature value
                box config.Dispatch
                box config.OnChange
            |]
        )

        // Set the signature data when the value change
        // This allows to have an initial signature for example or reload a form with a signature
        React.useEffect (
            fun _ ->
                match signaturePadRef.current with
                | Some signaturePad ->
                    signaturePad.clear ()
                    signaturePad.fromDataURL (config.Value) |> ignore
                | None -> ()

            , [|
                box config.Value
            |]
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
                        if config.Disabled then
                            style.backgroundColor "hsl(0, 0%, 96%)"
                            style.pointerEvents.none
                    ]
                ]
                signaturePad.height 200
                signaturePad.width 600
            ]
        ]
        |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error

    type Field<'Values, 'Output, 'Value>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, string, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<'Msg, string, Attributes>) =
            // We need to create a "real" React component to be able to use hooks
            // But if in your case you don't need hooks, you can directly render your HTML here
            // This is how it is done for the standard fields Fable.Form.Simple.Bulma
            SignatureFieldComponent config

// Expose the function that the user will use to create a signature field
// It follows the same pattern as the other fields in Fable.Form.Simple.Bulma (Form.xxxField)
[<RequireQualifiedAccess>]
module Form =

    let signatureField
        (config: Base.FieldConfig<SignatureField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        SignatureField.form (fun field -> SignatureField.Field field) config
