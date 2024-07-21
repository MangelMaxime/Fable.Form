module Page.Composability.WithConfiguration.AddressForm

open Fable.Form.Simple

type AddressForm<'Values, 'Attributes> = Form.Form<'Values, Address.T, 'Attributes>

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

let form ({ Value = getValue; Update = update }: Config<'A>) : AddressForm<'A, 'Attributes> =

    let updateField fn newValue values =
        update (fn newValue (getValue values)) values

    let countryField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser = Address.Country.tryParse
                Value = getValue >> fun values -> values.Country
                Update = updateField (fun newValue values -> { values with Country = newValue })
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Country"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let cityField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser = Address.City.tryParse
                Value = getValue >> fun values -> values.City
                Update = updateField (fun newValue values -> { values with City = newValue })
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "City"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let postalCodeField =
        Form.textField
            {
                Parser = Address.PostalCode.tryParse
                Value = getValue >> fun values -> values.PostalCode
                Update = updateField (fun newValue values -> { values with PostalCode = newValue })
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "PostalCode"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    Form.succeed Address.create
    |> Form.append countryField
    |> Form.append cityField
    |> Form.append postalCodeField
