---
layout: standard
title: Installation
---

Fable.Form.Simple.Bulma offers React support using [Bulma](https://bulma.io/) CSS framework.

<ul class="textual-steps">

<li>

Add the NuGet package to your project

```bash
# .NET CLI
dotnet add yourProject.fsproj package Fable.Form.Simple.Bulma

# Paket CLI
paket add Fable.Form.Simple.Bulma
```

</li>

<li>

Add this NPM package to your project

```bash
npm add fable-form-simple-bulma
```

This is required because Fable.Form.Simple.Bulma comes with custom fields that need additional styles.

</li>

<li>

Import the style in your `style.scss` file

```scss
// First import Bulma
@import "~bulma";

// Import
@import "~fable-form-simple-bulma";
```

</li>

<li>

In your F# file, open the required namespaces

```fsharp
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html
```

</li>

</ul>

You are ready to go 🎉

<p class="has-text-centered has-text-weight-semibold">
Choose how you want to host your form
</p>

<br/>

<div class="columns is-centered">
    <div class="column is-narrow">
        <a href="/Fable.Form/Fable.Form.Simple.Bulma/usage-with-elmish.html" class="button is-primary">Elmish</a>
    </div>
    <div class="column is-narrow">
        <a href="/Fable.Form/Fable.Form.Simple.Bulma/usage-with-react.html" class="button
        is-primary">React</a>
    </div>
</div>
