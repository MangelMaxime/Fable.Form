namespace Fable.Form.Extensions

module Result =

    val andThen:
        callback: ('ValueA -> Result<'ValueB, 'Error>) -> result: Result<'ValueA, 'Error> -> Result<'ValueB, 'Error>

module List =

    /// <summary>Returns a list that drops N elements of the original list and then yields the
    /// remaining elements of the list.</summary>
    /// <remarks>When count exceeds the number of elements in the list it
    /// returns an empty list instead of throwing an exception.</remarks>
    /// <param name="count">The number of items to drop.</param>
    /// <param name="source">The input list.</param>
    ///
    /// <returns>The result list.</returns>
    val drop: count: int -> source: 'A list -> 'A list

    /// <summary>Updates the value of an item in a list</summary>
    /// <param name="i">The index of the item to update</param>
    /// <param name="x">The new value of the item</param>
    /// <param name="lst">The input list</param>
    ///
    /// <returns>A new list with the updated element</returns>
    val setAt: i: int -> x: 'A -> lst: 'A list -> 'A list
