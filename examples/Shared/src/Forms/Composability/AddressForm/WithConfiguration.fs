module Examples.Shared.Forms.Composability.AddressForm.WithConfiguration.AddressForm

open Examples.Shared.Forms.Composability.Domain

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html
#endif

type AddressForm<'Values> = Form<'Values, Address>

type Values =
    {
        Country: string
        City: string
        PostalCode: string
    }

let blank =
    {
        Country = ""
        City = ""
        PostalCode = ""
    }

[<NoComparison; NoEquality>]
type Config<'A> =
    {
        Value: 'A -> Values
        Update: Values -> 'A -> 'A
    }

let form
    ({
         Value = getValue
         Update = update
     }: Config<'A>)
    : AddressForm<'A>
    =

    let updateField fn newValue values =
        update (fn newValue (getValue values)) values

    let countryField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser = Address.Country.tryParse
                Value = getValue >> fun values -> values.Country
                Update =
                    updateField (fun newValue values ->
                        { values with
                            Country = newValue
                        }
                    )
                Error = fun _ -> None
                Attributes = TextField.create "country" |> TextField.withLabel "Country"
            }

    let cityField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser = Address.City.tryParse
                Value = getValue >> fun values -> values.City
                Update =
                    updateField (fun newValue values ->
                        { values with
                            City = newValue
                        }
                    )
                Error = fun _ -> None
                Attributes = TextField.create "city" |> TextField.withLabel "City"
            }

    let postalCodeField =
        Form.textField
            {
                Parser = Address.PostalCode.tryParse
                Value = getValue >> fun values -> values.PostalCode
                Update =
                    updateField (fun newValue values ->
                        { values with
                            PostalCode = newValue
                        }
                    )
                Error = fun _ -> None
                Attributes = TextField.create "postalCode" |> TextField.withLabel "PostalCode"
            }

    Form.succeed Address.Create
    |> Form.append countryField
    |> Form.append cityField
    |> Form.append postalCodeField
