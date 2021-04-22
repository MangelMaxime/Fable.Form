module Page.Composability.Simple.AddressForm

open Fable.Form.Simple

type Values =
    {
        Country : string
        City : string
        PostalCode : string
    }

let blank =
    {
        Country = ""
        City = ""
        PostalCode = ""
    }

let form : Form.Form<Values, Address.T> =
    let countryField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser =
                    Address.Country.tryParse
                Value =
                    fun values -> values.Country
                Update =
                    fun newValue values ->
                        { values with Country = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Country"
                        Placeholder = ""
                    }
            }

    let cityField =
        Form.textField
            {
                // Use a custom EmailAddress parser and map the result back into a string
                // as the email field is represented using a string in the Form
                Parser =
                    Address.City.tryParse
                Value =
                    fun values -> values.City
                Update =
                    fun newValue values ->
                        { values with City = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "City"
                        Placeholder = ""
                    }
            }

    let postalCodeField =
        Form.textField
            {
                Parser =
                    Address.PostalCode.tryParse
                Value =
                    fun values -> values.PostalCode
                Update =
                    fun newValue values ->
                        { values with PostalCode = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "PostalCode"
                        Placeholder = ""
                    }
            }

    Form.succeed Address.create
        |> Form.append countryField
        |> Form.append cityField
        |> Form.append postalCodeField
