---
layout: standard
title: Features
---

<div class="notification is-warning has-text-black">
    This content is for v4. <a href="/Fable.Form/index.html">Click here to see the latest version</a>.
</div>

Here is a list of all the fields supported by `Fable.Form.Simple`:

## Standard HTML fields

- Text input
    - Password
    - Email
    - Raw
- Textarea
- Radio button
- Checkbox
- Select

## Custom fields

### Group

Wraps a form in a group.

It can result in rendering the fields differently. For example, `Fable.Form.Simple.Feliz` renders groups of field horizontally

See [sign-up example](/Fable.Form/examples/index.html#sign-up)

### Section

Wraps a form in a section an area with a title.

See [dynamic example](/Fable.Form/examples/index.html#dynamic-form)

### List of form

Build a variable list of forms, which support adding and removing form.

See [form list](/Fable.Form/examples/index.html#form-list)

## Custom action

Fable.Form.Simple supports to type of `Action`:

- `SubmitOnly of string`: which generates a submit button with the provided label
- `Custom of (State -> Elmish.Dispatch<'Msg> -> ReactElement)`: which allows you full control over the action area.

    This makes it possible for you to add a submit button and a cancel button for example.

    You can also use this feature to customize the look of the action area if the default is not good for you.

    When using, the custom option make sure to have a button of type `<button type="submit"></button>` so the form can capture the submit event.

    See [Custom actions example](/Fable.Form/examples/index.html#custom-actions) for a demo.
