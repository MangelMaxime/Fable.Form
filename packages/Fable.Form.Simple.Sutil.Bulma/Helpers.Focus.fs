namespace Fable.Form.Simple.Sutil.Bulma.Helpers

open Sutil
open Sutil.Core
open Sutil.Bulma
open Fable.Form
open Fable.Form.Simple
open Fable.Form.Simple.Form.View
open Fable.Form.Simple.Sutil.Bulma

module Focus =

    // Sutil loses focus when the user types in a field
    // See: https://github.com/davedawkins/Sutil/issues/92
    // To workaround, this limitation Fable.Form keeps track of the focused field on unmount
    // and restores focus when the field is mounted again
    //
    // Once the original issue is fixed, this workaround should be removed

    type FocusedField private () =
        [<DefaultValueAttribute>]
        val mutable fieldId: string

        [<DefaultValueAttribute>]
        val mutable selection: (int * int) option

        static let instance =
            let instance = FocusedField()
            instance.fieldId <- ""
            instance.selection <- None
            instance

        static member Instance = instance

        static member SaveFocused(fieldId: string, ?selectionStart: int, ?selectionEnd: int) =
            instance.fieldId <- fieldId

            match selectionStart, selectionEnd with
            | Some selectionStart, Some selectionEnd ->
                instance.selection <- Some(selectionStart, selectionEnd)
            | _ -> instance.selection <- None
