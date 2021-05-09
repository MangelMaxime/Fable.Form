---
title: Custom view templates
---

`Fable.Form.Simple` allows you to adapt the view code to make your form match your application looks.

If you are using [Bulma](https://bulma.io/) in your application you should use [Fable.Form.Simple.Bulma](/Fable.Form/Fable.Form.Simple.Bulma/installation.html).

However, if you need to customize the view code, you should start from this file structure:

```fsharp
// Have the name of your implementation as a suffix of Fable.Form.Simple
namespace Fable.Form.Simple.MyImplementation

// Force the user to use qualified access from here
// This ensure the API usage from the consumer code
[<RequireQualifiedAccess>]
module Form =

    module View =

        // Uncomment, the line corresponding to your preferred DOM library
        // open Feliz
        // open Fable.React

        // Open the differents modules we need to have access to
        open Fable.Form
        open Fable.Form.Simple
        open Fable.Form.Simple.Form.View

        // Contract that we need to implement to support
        // all the feature offered by Fable.Simple.Form
        let htmlViewConfig<'Msg> : CustomConfig<'Msg> =
            {
                Form = failwith "Not implemented yet"
                TextField = failwith "Not implemented yet"
                PasswordField = failwith "Not implemented yet"
                EmailField = failwith "Not implemented yet"
                TextAreaField = failwith "Not implemented yet"
                CheckboxField = failwith "Not implemented yet"
                RadioField = failwith "Not implemented yet"
                SelectField = failwith "Not implemented yet"
                Group = failwith "Not implemented yet"
                Section = failwith "Not implemented yet"
                FormList = failwith "Not implemented yet"
                FormListItem = failwith "Not implemented yet"
            }

        // Function which will be called by the consumer to render the form
        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config
```

You can take a look at [Fable.Form.Simple.Bulma](https://github.com/MangelMaxime/Fable.Form/blob/38a41274940e98a50b30d6991722d780ffe00189/packages/Fable.Form.Simple.Bulma/Form.fs) implementation. It is done in less than 500 lines of code.
