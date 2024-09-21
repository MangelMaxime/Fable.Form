module rec User

let checkEmailAddress (email: EmailAddress.T) =
    promise {
        // Add some delay to simulate a Server request
        do! Promise.sleep 1000

        if (EmailAddress.toString email) = "warded@mail.com" then
            return Ok(ValidEmail.create email)
        else
            return Error "The e-mail address is taken. Try this one: warded@mail.com"
    }

module ValidEmail =

    type T = private ValidEmail of EmailAddress.T

    let toString (ValidEmail email) = EmailAddress.toString email

    let validateEmailAddress (email: string) =
        match EmailAddress.tryParse email with
        | Ok email -> checkEmailAddress email

        | Error error -> Promise.reject (System.Exception error)

    let create (email: EmailAddress.T) : T = ValidEmail email

module Name =

    type T = private Name of string

    let toString (Name name) = name

    let create (text: string) = Name text

    let tryParse (text: string) =
        if text.Length < 2 then
            Error "The name must have at least 2 characters"

        else
            Ok(Name text)

module Password =

    type T = private Password of string

    let toString (Password password) = password

    let create (text: string) = Password text

    let tryParse (text: string) =
        if text.Length < 4 then
            Error "The password must have at least 4 characters"

        else
            Ok(Password text)

type T =
    {
        Email: ValidEmail.T
        Name: Name.T
        Password: Password.T
        IsProfilePublic: bool
    }

let signUp (email: EmailAddress.T) (name: Name.T) (password: Password.T) (makePublic: bool) =

    checkEmailAddress email
    |> Promise.mapResult (fun validEmail ->
        {
            Email = validEmail
            Name = name
            Password = password
            IsProfilePublic = makePublic
        }
    )
