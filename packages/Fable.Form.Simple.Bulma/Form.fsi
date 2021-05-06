namespace Fable.Form.Simple.Bulma

[<RequireQualifiedAccess>]
module Form =

    module View =

        open Feliz
        open Fable.Form
        open Fable.Form.Simple

        val fieldLabel :
            label : string ->
            ReactElement

        val errorMessage :
            message:string ->
            ReactElement

        val errorMessageAsHtml :
            showError :bool ->
            error : Error.Error option ->
            ReactElement

        val wrapInFieldContainer :
            children : ReactElement list ->
            ReactElement

        val withLabelAndError :
            label : string ->
            showError : bool ->
            error : Error.Error option ->
            fieldAsHtml : ReactElement ->
            ReactElement

        val inputField :
            typ : Form.View.InputType ->
            config : Form.View.TextFieldConfig<'Msg> ->
            ReactElement

        val textareaField :
            config : Form.View.TextFieldConfig<'Msg> ->
            ReactElement

        val checkboxField :
            config : Form.View.CheckboxFieldConfig<'Msg> ->
            ReactElement

        val radioField :
            config : Form.View.RadioFieldConfig<'Msg> ->
            ReactElement

        val selectField :
            config : Form.View.SelectFieldConfig<'Msg> ->
            ReactElement

        val group :
            fields :  ReactElement list ->
            ReactElement

        val section :
            title : string ->
            fields :  ReactElement list ->
            ReactElement

        val ignoreChildError :
            parentError : Error.Error option ->
            field : Form.FilledField<'Values> ->
            Form.FilledField<'Values>

        val formList :
            formConfig : Form.View.FormListConfig<'Msg> ->
            ReactElement

        val formListItem :
            config : Form.View.FormListItemConfig<'Msg> ->
            ReactElement

        val form :
            config : Form.View.FormConfig<'Msg> ->
            ReactElement

        val htmlViewConfig<'Msg> :
            Form.View.CustomConfig<'Msg>

        val asHtml :
            config : Form.View.ViewConfig<'Values,'Msg> ->
            (Form.Form<'Values,'Msg> -> Form.View.Model<'Values> -> ReactElement)
