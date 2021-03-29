module Form.View

type State =
    | Idle
    | Loading
    | Error of string
    | Success of string

type ErrorTracking =
    | ErrorTracking of {| ShowAllErrors : bool; ShowFieldError : Set<string> |}

type Model<'Values> =
    {
        Values : 'Values
        State : State
        ErrorTracking : ErrorTracking
    }

let idle (values : 'Values)=
    {
        Values = values
        State = Idle
        ErrorTracking =
            ErrorTracking
                {|
                    ShowAllErrors = false
                    ShowFieldError = Set.empty
                |}
    }

type FormConfig<'Msg, 'Element> =
    {
        OnSubmit : 'Msg option
        State : State
        Action : string
        Loading : string
        Fields : 'Element list
    }


// let form (config : FormConfig<'Msg, 'Element>) =

