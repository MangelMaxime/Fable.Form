---
layout: standard
title: Installation
---

Fable.Form.Simple.Sutil.Bulma offers [Sutil](https://sutil.dev/) support using [Bulma](https://bulma.io/) CSS framework.

<ul class="textual-steps">

<li>

Add the NuGet package to your project

```bash
# .NET CLI
dotnet add yourProject.fsproj package Fable.Form.Simple.Sutil.Bulma

# Paket CLI
paket add Fable.Form.Simple.Sutil.Bulma
```

</li>

<li>

Add this NPM package to your project

```bash
npm add fable-form-simple-bulma
```

This is required because Fable.Form.Simple.Sutil.Bulma comes with custom fields that need additional styles.

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
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html
```

</li>

</ul>

You are ready to go 🎉
