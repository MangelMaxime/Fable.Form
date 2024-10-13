namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma

module FileField =

    [<RequireQualifiedAccess>]
    type FileType =
        | Any
        | Specific of string list

    type FileIconClassName =
        | DefaultIcon
        | CustomIcon of string

    type Attributes =
        {
            FieldId: string
            Label: string
            InputLabel: string
            Accept: FileType
            FileIconClassName: FileIconClassName
            Multiple: bool
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, Browser.Types.File array, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, Browser.Types.File array, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field Array.isEmpty

    type Field<'Values>(innerField: InnerField<'Values>) =

        inherit IStandardField<'Values, Browser.Types.File array, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField
            (config: StandardRenderFieldConfig<Browser.Types.File array, Attributes>)
            =

            let fileInput =
                Bulma.file [
                    if not (config.Value |> Array.isEmpty) then
                        file.hasName

                    prop.children [
                        Bulma.fileLabel.label [
                            // File input don't support readonly so we need to prevent the pointer events
                            if config.IsReadOnly then
                                prop.style [
                                    style.pointerEvents.none
                                ]
                            prop.children [
                                Bulma.fileInput [
                                    prop.onInput (fun x ->
                                        let files =
                                            (x.currentTarget :?> Browser.Types.HTMLInputElement)
                                                .files

                                        let files = Array.init files.length (fun i -> files[i])

                                        files |> config.OnChange
                                    )

                                    prop.multiple config.Attributes.Multiple

                                    match config.Attributes.Accept with
                                    | FileType.Any -> ()
                                    | FileType.Specific fileTypes ->
                                        prop.accept (fileTypes |> String.concat ",")

                                    prop.disabled config.Disabled
                                ]
                                Bulma.fileCta [
                                    match config.Attributes.FileIconClassName with
                                    | DefaultIcon ->
                                        Bulma.fileIcon [
                                            prop.innerHtml
                                                """<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-upload">
<path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
<polyline points="17 8 12 3 7 8"/>
<line x1="12" x2="12" y1="3" y2="15"/>
</svg>
<!--
This icon has been taken from Lucide icons project

See: https://lucide.dev/license
-->"""
                                        ]

                                    | CustomIcon className ->
                                        Bulma.fileIcon [
                                            Html.i [
                                                prop.className className
                                            ]
                                        ]

                                    Bulma.fileLabel.span [
                                        prop.text config.Attributes.InputLabel
                                        prop.onClick (fun ev ->
                                            ev.stopPropagation ()
                                            ev.stopImmediatePropagation ()
                                        )
                                    ]
                                ]

                                if not (config.Value |> Array.isEmpty) then
                                    Bulma.fileName [
                                        prop.text (config.Value |> Array.head).name
                                    ]
                            ]
                        ]

                    ]
                ]

            fileInput
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
