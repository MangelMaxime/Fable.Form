namespace Fable.Form.Simple

module FormList =

    open Fable.Form

    [<NoComparison; NoEquality>]
    type Form<'Values, 'Field> =
        {
            Fields: Base.FilledField<'Field> list
            Delete: unit -> 'Values
        }

    type Attributes =
        {
            Label: string
            Add: string option
            Delete: string option
        }

    [<NoComparison; NoEquality>]
    type FormList<'Values, 'Field> =
        {
            Forms: Form<'Values, 'Field> list
            Add: unit -> 'Values
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type Config<'Values, 'ElementValues> =
        {
            Value: 'Values -> 'ElementValues list
            Update: 'ElementValues list -> 'Values -> 'Values
            Default: 'ElementValues
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type ElementState<'Values, 'ElementValues> =
        {
            Index: int
            Update: 'ElementValues -> 'Values -> 'Values
            Values: 'Values
            ElementValues: 'ElementValues
        }

    val form:
        tagger: (FormList<'Values, 'Field> -> 'Field) ->
        formConfig: Config<'Values, 'ElementValues> ->
        buildElement: (ElementState<'Values, 'ElementValues> -> Base.FilledForm<'Output, 'Field>) ->
            Base.Form<'Values, 'Output list, 'Field>
