[<RequireQualifiedAccess>]
module EmailAddress

type T =
    private EmailAddress of string

let create (text : string) =
    EmailAddress text

let toString (EmailAddress email) =
    email

let tryParse (text : string) =
    if text.Contains("@") then
        Ok (EmailAddress text)
    else

        Error "The e-mail address must contain a '@' symbol"
