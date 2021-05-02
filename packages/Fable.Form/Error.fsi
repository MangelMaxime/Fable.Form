module Fable.Form.Error

type Error =
    /// <summary>
    /// Used to represents an empty field
    /// </summary>
    | RequiredFieldIsEmpty
    /// <summary>
    /// Used to represents an error due a validation failed like when parsing a field value.
    /// </summary>
    | ValidationFailed of string
    /// <summary>
    /// Used to represents an external error
    /// <para>For example, it is used when doing server side validation</para>
    /// </summary>
    | External of string
