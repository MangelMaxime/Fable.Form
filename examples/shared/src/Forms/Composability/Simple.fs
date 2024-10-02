module Examples.Shared.Forms.Composability.Simple

open Fable.Form.Simple
open Examples.Shared.Forms.Composability.Domain
open Examples.Shared.Forms.Composability.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
#endif

/// <summary>
/// Represent the form values
/// </summary>
type Values =
    {
        Name: string
        Address: AddressForm.Values
    }

let init =
    {
        Name = ""
        Address = AddressForm.blank
    }
    |> Form.View.idle

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, _> =
    let nameField =
        Form.textField
            {
                Parser = Name.tryParse
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "name"
                        Label = "Name"
                        Placeholder = "Your name"
                        AutoComplete = None
                    }
            }

    let onSubmit user address = (user, address)

    Form.succeed onSubmit
    |> Form.append nameField
    |> Form.append (
        Form.mapValues
            {
                Value = fun values -> values.Address
                Update =
                    fun newValue values ->
                        { values with
                            Address = newValue
                        }
            }
            Simple.AddressForm.form
    )

let information<'R> : DemoInformation<_> =
    {
        Title = "Composability"
        Remark = None
        Route = SharedRouter.Route.Composability SharedRouter.ComposabilityRoute.Simple
        Description = "Demonstrate how you can re-use a form the 'simple way'"
        Code =
            """
Form.succeed onSubmit
|> Form.append nameField
|> Form.append (
    Form.mapValues
        {
            Value =
                fun values -> values.Address
            Update =
                fun newValue values ->
                    { values with Address = newValue }
        }
        AddressForm.form
)
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
