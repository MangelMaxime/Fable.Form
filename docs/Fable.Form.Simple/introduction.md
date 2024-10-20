---
layout: standard
title: Introduction
---

`Fable.Form.Simple` is a UI-agnostic library that provides a simple way to manage forms in Fable applications.

It defines common form behaviors, such as:

- State tracking:
    - `Idle` - The form is being filled, this is the default state
    - `Loading` - The form is being submitted
    - `ReadOnly` - The form is being displayed in read-only mode
    - `Error of string` - The form is in error state, with a global error message
    - `Success of string` - The form has been successfully submitted, with a global success message
- Error tracking - keeps track of errors in the form
- Values tracking - keeps track of the values in the form (values are what the user has entered in the form)

`Fable.Form.Simple` can be used with any UI library, currently we provides supports for:

<br/>

<div class="columns is-centered">
    <div class="column is-narrow">
        <a href="/Fable.Form/Fable.Form.Simple.Bulma/installation.html" class="button is-primary">Bulma</a>
    </div>
    <div class="column is-narrow">
        <a href="/Fable.Form/Fable.Form.Simple.Sutil.Bulma/installation.html" class="button is-primary">Sutil</a>
    </div>
</div>
