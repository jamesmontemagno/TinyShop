dotnet run --publisher manifest --output-path aspire-manifest.json

azd init
azd infra synth

Let's add a admin section to this application. I want everything to be on one page. I want a way for the user to log in with a simple password. Then on that page I would like a way to see all the items in the product catalog and edit, delete or add a new one.

@rendermode InteractiveServer