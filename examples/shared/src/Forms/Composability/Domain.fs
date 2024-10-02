module Examples.Shared.Forms.Composability.Domain

type Name = private Name of string

module Name =

    let value (Name name) = name

    let create (text: string) = Name text

    let tryParse (text: string) =
        if text.Length < 2 then
            Error "The name must have at least 2 characters"

        else
            Ok(Name text)

[<RequireQualifiedAccess>]
module Address =

    type Country = private Country of string
    type City = private City of string
    type PostalCode = private PostalCode of string

    module Country =

        let create text = Country text

        let value (Country country) = country

        let tryParse (text: string) = Ok(create text)

    module City =

        let value (City city) = city

        let create text = City text

        let tryParse (text: string) = Ok(create text)

    module PostalCode =

        let value (PostalCode postalCode) = postalCode

        let create text = PostalCode text

        let tryParse (text: string) = Ok(create text)

type Address =
    {
        Country: Address.Country
        City: Address.City
        PostalCode: Address.PostalCode
    }

    static member Create country city postalCode =
        {
            Country = country
            City = city
            PostalCode = postalCode
        }
