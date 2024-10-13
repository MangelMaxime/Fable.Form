module Examples.Sutil.Fields.SignatureField

open System
open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Core.JsInterop
open Glutinum.SignaturePad
open Browser
open Feliz

open type Glutinum.SignaturePad.Exports

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

    type Field<'Values>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, string, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =
                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, Attributes>) =
            let signaturePadStore: IStore<SignaturePad option> = Store.make None

            Html.canvas [
                prop.width 600
                prop.height 200
                prop.style [
                    Css.margin length.auto
                    Css.displayBlock
                    Css.borderRadius (length.px 5)
                    Css.border (length.px 1, borderStyle.solid, "hsl(0, 0%, 86%)")
                    // If the field is disabled, we can disable pointer events to
                    // make the canvas not interactable
                    if config.Disabled || config.IsReadOnly then
                        Css.backgroundColor "hsl(0, 0%, 96%)"
                        Css.cursorNone
                ]

                Ev.onMount (fun ev ->
                    let canvas = (ev.target :?> Types.HTMLCanvasElement)

                    let signaturePadInstance = SignaturePad canvas

                    signaturePadInstance |> Some |> Store.set signaturePadStore

                    let callback _ =
                        signaturePadInstance.toDataURL () |> config.OnChange

                    signaturePadInstance?addEventListener ("endStroke", unbox callback)

                    signaturePadInstance.clear ()

                    if config.Value <> "" then
                        signaturePadInstance.fromDataURL config.Value |> ignore
                )
            ]
            |> bulma.control.div
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error

// Expose the function that the user will use to create a signature field
// It follows the same pattern as the other fields in Fable.Form.Simple.Bulma (Form.xxxField)
[<RequireQualifiedAccess>]
module Form =

    let signatureField
        (config: Base.FieldConfig<SignatureField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        SignatureField.form (fun field -> SignatureField.Field field) config
