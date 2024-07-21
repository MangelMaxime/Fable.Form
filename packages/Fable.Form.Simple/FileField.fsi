namespace Fable.Form.Simple.Field

module FileField =

    open Fable.Form

    type FileType =
        | Any
        | Specific of string list

    type FileIconClassName =
        | Default
        | Custom of string

    type Attributes =
        { Label: string
          InputLabel: string
          Accept: FileType
          FileIconClassName: FileIconClassName
          Multiple: bool }

    type FileField<'Values> = Field.Field<Attributes, Browser.Types.File array, 'Values>

    val form<'Values, 'Field, 'Output> :
        ((FileField<'Values> -> 'Field)
            -> Base.FieldConfig<Attributes, Browser.Types.File array, 'Values, 'Output>
            -> Base.Form<'Values, 'Output, 'Field>)
