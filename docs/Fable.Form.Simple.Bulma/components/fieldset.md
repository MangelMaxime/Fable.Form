---
title: Fieldset / Groups
---

Wraps a list of fields inside of a group with a label on top.

---

<p>
<div class="has-text-centered">

**Showcase**

</div>
</p>

<!-- Wrap your field in a fieldset with the class 'fieldset' -->
<fieldset class="fieldset">
    <!-- Use the the legend element to control what text to show at the top of the group -->
    <legend>Teacher</legend>
    <div class="field"><label class="label">Name</label>
        <div class="control"><input readonly="true" type="text" class="input" placeholder="Teacher name" value=""></div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Subject</label>
        <div class="control"><input readonly="true" type="text" class="input" placeholder="Taught subject" value=""></div>
        <p class="help"></p>
    </div>
</fieldset>

<p>
<div class="has-text-centered">

**Code**

</div>
</p>


```html
<!-- Wrap your field in a fieldset with the class 'fieldset' -->
<fieldset class="fieldset">
    <!-- Use the the legend element to control what text to show at the top of the group -->
    <legend>Teacher</legend>
    <div class="field">
        <label class="label">Name</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="Teacher name" value="">
        </div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Subject</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="Taught subject" value="">
        </div>
        <p class="help"></p>
    </div>
</fieldset>
```


See [dynamic form example](/Fable.Form/examples/index.html#dynamic-form) for an interactive example.

## Variables

<table class="table is-striped">
    <thead>
        <tr>
            <th>Name</th>
            <th>Value</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><code>$fieldset-shadow</code></td>
            <td><code>$box-shadow</code></td>
        </tr>
        <tr>
            <td><code>$fieldset-border-radius</code></td>
            <td><code>$radius-large</code></td>
        </tr>
        <tr>
            <td><code>$fieldset-border</code></td>
            <td><code>1px solid $border</code></td>
        </tr>
        <tr>
            <td><code>$fieldset-padding</code></td>
            <td><code>1.25em</code></td>
        </tr>
        <tr>
            <td><code>$fieldset-legend-color</code></td>
            <td><code>$grey-darker</code></td>
        </tr>
        <tr>
            <td><code>$fieldset-legend-weight</code></td>
            <td><code>$weight-bold</code></td>
        </tr>
    </tbody>
</table>
