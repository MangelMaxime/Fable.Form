module Result

let andThen
    (callback : 'ValueA -> Result<'ValueB, 'Error>)
    (result : Result<'ValueA, 'Error>) : Result<'ValueB, 'Error> =

    match result with
    | Ok value ->
        callback value

    | Error message ->
        Error message
