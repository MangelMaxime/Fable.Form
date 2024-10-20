---
layout: standard
title: Installation
---

<div class="notification is-warning has-text-black">
    This content is for v4. <a href="/Fable.Form/index.html">Click here to see the latest version</a>.
</div>

[Fable.Form.Simple](/Fable.Form/Fable.Form.Simple/features.html) offers custom fields that are not supported by default by [Bulma](https://bulma.io/).

In order, to support them, Fable.Form.Simple.Bulma comes with an npm package [fable-form-simple-bulma]() that you need to install.

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

Open the library modules

```fsharp
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
```

</li>

<li>

Add the NPM package to your project

```
npm add fable-form-simple-bulma
```

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

</ul>

You are ready to go 🎉
