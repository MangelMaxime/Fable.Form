---
layout: navbar-only
---

<div class="container">
    <div class="section">
        <div class="container">
            <div class="has-text-centered has-text-weight-bold mb-3">
                Choose your framework
            </div>
            <div class="tabs is-centered is-toggle">
                <ul>
                    <li class="is-active"><a href="/Fable.Form/examples/react/index.html">React</a></li>
                    <li><a href="/Fable.Form/examples/sutil/index.html">Sutil</a></li>
                    <li><a href="/Fable.Form/examples/lit/index.html">Fable.Lit</a></li>
                </ul>
            </div>
            <div id="root">
            If you see this message, it means that you are probably running the docs locally.
<br/><br/>
Nacara doesn't keep the examples generated files when run locally. But they will be available once the documentation is deployed.
<br/><br/>
If needed you can run <code>./build.sh example react</code> after starting the documentation server to generate the example files.
            </div>
        </div>
    </div>
</div>

<link rel="stylesheet" href="/Fable.Form/examples/daisyui.css">
<script type="module" defer="defer" crossorigin src="dist/index.js"></script>
