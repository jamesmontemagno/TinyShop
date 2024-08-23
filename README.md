# TinyShop on .NET Aspire

This is a demo repo for a tiny little commerce website that displays sporting goods. There is an API backend and a Blazor frontend that calls it. In the `main` branch is the starting project where the two projects need to be run separate or configure for multi-deploy in Visual Studio. There is no orchestration, environment variables are used for URL discovery, there is no resiliency, no health checks, and no telemetry in it at all. This is where we go through and add .NET Aspire and make it AWESOME! :)


Demo flow:
- Run app as is (configur multi-deploy)
- Configure .NET Aspire in backend
- Show dashboard
- Show resiliency
- Show health check
- Add .NET Aspire to frontend
- Configure service discovery
- Add redis cache
- AddResisOutputCache
- UseOutputCache
- Add `[OutputCache]` to api
- Back to slides
- Deployment
- Create manifest
- Azd init
- Azd infra synth
- Azd up
- Also show in VS
- Show pre-deployed and talk about the dashboard
