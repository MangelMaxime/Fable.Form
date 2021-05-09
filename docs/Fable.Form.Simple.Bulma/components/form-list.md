---
title: Form list
---

Wraps a list of fields inside of a box.

---

<p>
<div class="has-text-centered">

**Showcase**

</div>
</p>

<div class="form-list">
    <div class="field"><label class="label">Name of book #1</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="" value="The warded man">
        </div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Author of book #1</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="" value="Peter V. Brett">
        </div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Summary of book #1</label>
        <div class="control">
            <textarea class="textarea" readonly="true" placeholder="">
            The Painted Man, book one of the Demon Cycle, is a captivating and thrilling fantasy adventure, pulling the reader into a world of demons, darkness and heroes.
            </textarea>
        </div>
        <p class="help"></p>
    </div>
</div>

<p>
<div class="has-text-centered">

**Code**

</div>
</p>


```html

<!-- Wrap your fields in a div with the class 'form-list' -->
<div class="form-list">
    <div class="field"><label class="label">Name of book #1</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="" value="The warded man">
        </div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Author of book #1</label>
        <div class="control">
            <input readonly="true" type="text" class="input" placeholder="" value="Peter V. Brett">
        </div>
        <p class="help"></p>
    </div>
    <div class="field"><label class="label">Summary of book #1</label>
        <div class="control">
            <textarea class="textarea" readonly="true" placeholder="">
            The Painted Man, book one of the Demon Cycle, is a captivating
            and thrilling fantasy adventure, pulling the reader
            into a world of demons, darkness and heroes.
            </textarea>
        </div>
        <p class="help"></p>
    </div>
</div>
```


See [form list example](/Fable.Form/examples/index.html#form-list) for an interactive example.

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
            <td><code>$formlist-shadow</code></td>
            <td><code>$box-shadow</td></code>
        </tr>
        <tr>
            <td><code>$formlist-border-radius</code></td>
            <td><code>$radius-large</td></code>
        </tr>
        <tr>
            <td><code>$formlist-border</code></td>
            <td><code>1px solid $border</td></code>
        </tr>
        <tr>
            <td><code>$formlist-padding</code></td>
            <td><code>1.25em</td></code>
        </tr>
    </tbody>
</table>
