---
title: Running Alterations
description: A description of how to run alterations.
---

Instead of submitting alteration plans for asynchronous execution, you can apply alterations immediately using the `IAlterationRunner` service. For example:

```clike
var alterations = new List<IAlteration>
{
    new ModifyVariable("MyVariable", "MyValue")
},

var workflowInstanceIds = new[] { "26cf02e60d4a4be7b99a8588b7ac3bb9" };
var runner = serviceProvider.GetRequiredService<IAlterationRunner>();
var results = await runner.RunAsync(plan, cancellationToken);
```

When an alteration plan is executed immediately, the alterations are applied synchronously and the results are returned.
You will have to manually schedule affected workflow instances to resume execution. Use the `IAlteredWorkflowDispatcher`:

```clike
var dispatcher = serviceProvider.GetRequiredService<IAlteredWorkflowDispatcher>();
await dispatcher.DispatchAsync(results, cancellationToken);
```

This will tell the workflow engine to pickup the altered workflow instances and execute them.