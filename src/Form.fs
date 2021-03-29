module Form.Core

module Base = Form.Base.Core

type TextField<'Values> = Base.TextField.TextField<'Values>
type RadioField<'Values> = Base.RadioField.RadioField<'Values>

open Form.Base

type TextType =
    | TextRaw

type Field<'Values> =
    | Text of TextType * TextField<'Values>
    | Radio of RadioField<'Values>


type Form<'Values, 'Output> =
    Base.Form<'Values, 'Output, Field<'Values>>


let succeed (output : 'Output) : Form<'Values, 'Output> =
    Base.succeed output

let append : Form<'Values, 'A> -> Form<'Values, 'A -> 'B> -> Form<'Values, 'B> =
    Base.append

let textField
    (config : Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
    : Form<'Values, 'Output> =
    TextField.form (fun x -> Text (TextRaw, x)) config
