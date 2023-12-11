---
title: Workflow Dispatcher
description: Configuring the Workflow Dispatcher
---

The Workflow Dispatcher is a service that is responsible for dispatching requests to execute workflows.
It is represented by the [IWorkflowDispatcher](https://github.com/elsa-workflows/elsa-core/blob/main/src/modules/Elsa.Workflows.Runtime/Contracts/IWorkflowDispatcher.cs) interface.

When a request to execute a workflow is dispatched, a consumer will pick up the request and execute the workflow asynchronously in the background.
This allows the caller to continue without waiting for the workflow to complete.

---

## Configuring the Workflow Dispatcher

Out of the box, Elsa provides two implementations of the `IWorkflowDispatcher` interface:

- `BackgroundWorkflowDispatcher`: A simple implementation that queues the specified request for workflow execution on a non-durable background worker.
- `MassTransitWorkflowDispatcher`: An implementation that uses MassTransit to dispatch the specified request to a queue for workflow execution.

### Background Workflow Dispatcher

This is the default implementation that is used when no other implementation is registered.

### MassTransit Workflow Dispatcher

This implementation uses MassTransit to dispatch the specified request to a queue for workflow execution.

To use this implementation, you must first configure MassTransit. See [MassTransit Configuration](/docs/integrations/masstransit.md) for more information.
Next, you need to tell the Workflow Runtime to use the MassTransit Workflow Dispatcher:

```clike
services.AddElsa(elsa => 
{
    elsa.UseWorkflowRuntime(runtime => 
    {
        runtime.UseMassTransitDispatcher();
    });
});
```

The MassTransit implementation of the workflow dispatcher can be further configured using the `MassTransitWorkflowDispatcherOptions` class:

```clike
services.Configure<MassTransitWorkflowDispatcherOptions>(options =>
{
    // The number of concurrent messages to process.
    options.ConcurrentMessageLimit = 32;
});
```