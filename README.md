# Warden.Integrations.Seq
Seq integration for Warden

For information on how to setup your Warden please refer to the [Warden documentation](https://github.com/warden-stack/Warden)

Installation:
---
Available as a [NuGet package](https://www.nuget.org/packages/Warden.Integrations.Seq/).
```
Install-Package Warden.Integrations.Seq
```

Configuration:
---
The integration will push an event for each result in an iteration. The levels are `Debug` for valid checks and `Error` for invalid checks (this will be configurable in the future).
```
var wardenConfiguration = WardenConfiguration
  .Create();
  .IntegrateWithSeq("http://seq.example.com", "APIKEY")
  .SetHooks((hooks, integrations) =>
  {
      hooks.OnIterationCompletedAsync(
          iteration => integrations.Seq().PostIterationToSeqAsync(iteration));
      hooks.OnCompletedAsync(check => integrations.Seq().PostCheckToSeqAsync(check));
  })
  .Build();
```
