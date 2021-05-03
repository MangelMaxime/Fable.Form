[<RequireQualifiedAccess>]
module Address

module Country =

    type T = private Country of string

    let create text = 
        Country text

    let toString (Country country) = 
        country

    let tryParse (text : string) =
        Ok (create text)

module City =

    type T = private City of string

    let toString (City city) = 
        city

    let create text = City text

    let tryParse (text : string) =
        Ok (create text)

module PostalCode =

    type T = private PostalCode of string

    let toString (PostalCode postalCode) =
        postalCode

    let create text = PostalCode text

    let tryParse (text : string) =
        Ok (create text)

type T =
    {
        Country : Country.T
        City : City.T
        PostalCode : PostalCode.T
    }

let create country city postalCode =
    {
        Country = country
        City = city
        PostalCode = postalCode
    }
