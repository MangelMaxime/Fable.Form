namespace Fable.Form.Simple.Bulma.Fields

open Feliz
open Feliz.Bulma
open Fable.Form
open Fable.Form.Simple.Bulma

module InputField =

    [<NoComparison; NoEquality>]
    type Config =
        {
            OnChange: string -> unit
            OnBlur: (unit -> unit) option
            Disabled: bool
            IsReadOnly: bool
            Value: string
            Error: Error.Error option
            ShowError: bool
            Label: string
            InputFunc: IReactProperty list -> ReactElement
            ExtraInputProps: IReactProperty list
        }

    /// <summary>
    /// Generic input field renderer used to mutualize the rendering of input fields
    ///
    /// ATTENTION: For some input fields, you may not want to use this function.
    ///
    /// For example, date fields needs to use uncontrolled input fields.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    let renderField (config: Config) =

        config.InputFunc [
            prop.onChange config.OnChange

            match config.OnBlur with
            | Some onBlur -> prop.onBlur (fun _ -> onBlur ())
            | None -> ()

            prop.disabled config.Disabled
            prop.readOnly config.IsReadOnly
            prop.value config.Value

            if config.ShowError && config.Error.IsSome then
                color.isDanger

            yield! config.ExtraInputProps
        ]
        |> Html.View.withLabelAndError config.Label config.ShowError config.Error
