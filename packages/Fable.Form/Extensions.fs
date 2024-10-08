namespace Fable.Form.Extensions

module Result =

    let andThen
        (callback: 'ValueA -> Result<'ValueB, 'Error>)
        (result: Result<'ValueA, 'Error>)
        : Result<'ValueB, 'Error>
        =

        match result with
        | Ok value -> callback value

        | Error error -> Error error

module List =

    // Copied from https://github.com/fsprojects/FSharpPlus/blob/35fa2a97e0c3f93c7cf36c172314e7f2d921d438/src/FSharpPlus/Extensions/List.fs

    /// <summary>Returns a list that drops N elements of the original list and then yields the
    /// remaining elements of the list.</summary>
    /// <remarks>When count exceeds the number of elements in the list it
    /// returns an empty list instead of throwing an exception.</remarks>
    /// <param name="count">The number of items to drop.</param>
    /// <param name="source">The input list.</param>
    ///
    /// <returns>The result list.</returns>
    let drop count source =
        let rec loop i lst =
            match lst, i with
            | [] as x, _
            | x, 0 -> x
            | x, n -> loop (n - 1) (List.tail x)

        if count > 0 then
            loop count source
        else
            source

    /// <summary>Updates the value of an item in a list</summary>
    /// <param name="i">The index of the item to update</param>
    /// <param name="x">The new value of the item</param>
    /// <param name="lst">The input list</param>
    ///
    /// <returns>A new list with the updated element</returns>
    let setAt i x lst =
        if List.length lst > i && i >= 0 then
            lst.[0 .. i - 1] @ x :: lst.[i + 1 ..]
        else
            lst
