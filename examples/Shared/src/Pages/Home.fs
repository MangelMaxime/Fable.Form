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

let htmlContent
    (extraBasics: DemoInformation<'R> list)
    (customFieldInformation: DemoInformation<'R>)
    (customViewInformation: DemoInformation<'R>)
    =
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
        {renderLink Examples.Shared.Forms.SignUp.information}
        {renderLink Examples.Shared.Forms.File.information}
        {renderLink Examples.Shared.Forms.DynamicForm.information}
        {renderLink Examples.Shared.Forms.FormList.information}
        {renderLink Examples.Shared.Forms.Disable.information}
        {renderLink Examples.Shared.Forms.Composability.Simple.information}
        {renderLink Examples.Shared.Forms.Composability.WithConfiguration.information}
        {extraBasics |> List.map renderLink |> String.concat ""}
    </ul>

    <p class="subtitle is-5">Advanced</p>

    <p>The features demonstrated in this section depends on the library which provides the view implementation</p>
    <p>The goal here is to demonstrate advanced usage that you could need when implementing your own view</p>

    <ul>
        {renderLink Examples.Shared.Forms.ValidationStrategies.information}
        {renderLink Examples.Shared.Forms.CustomActions.information}
        {renderLink customFieldInformation}
        {renderLink customViewInformation}
    </ul>
</div>
    """
