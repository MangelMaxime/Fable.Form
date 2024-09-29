module Examples.Shared.Pages.Home

let renderLink
    ({
         Title = titleText
         Description = description
         Route = route
     }: DemoInformation<_>)
    =
    $"""
<li>
    <a href=%s{route.HashPart}>%s{titleText}</a>

    <div class="content">
        %s{description}
    </div>
</li>
    """

let htmlContent =
    $"""


<div class="content">
    <br>
        <div class="has-text-centered">
            <h5 class="title is-5 is-5">List of examples</h5>
        </div>

    <hr>

    <p class="subtitle is-5">Basic</p>

    <p>The features demonstrated in this section are available for all the library based on <b>Fable.Form</b></p>

    <ul>
        {renderLink Examples.Shared.Forms.Login.information}
    </ul>

    <p class="subtitle is-5">Advanced</p>

    <p>The features demonstrated in this section depends on the library which provides the view implementation</p>
    <p>The goal here is to demonstrate advanced usage that you could need when implementing your own view</p>

    <ul>

    </ul>
</div>
    """
