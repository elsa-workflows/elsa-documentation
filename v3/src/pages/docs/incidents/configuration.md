---
title: Incident Strategies
description: An overview of incident strategies.
---

We can configure what the workflow engine should do in case of an incident through Incident Strategies.

## Global

The default strategy is `Fault`, but we can change it by setting the `IncidentStrategy` property of the `WorkflowOptions` class:

```clike
services.Configure<IncidentOptions>(options =>
{
    options.DefaultIncidentStrategy = typeof(ContinueWithIncidents);
});
```

The default strategy will be used for all workflows that do not have a strategy configured explicitly.

## Workflow Specific

We can configure the incident strategy for a workflow by setting the `WorkflowOptions` property of the `Workflow` class:

```clike
public class MyWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.WorkflowOptions.IncidentStrategyType = typeof(ContinueWithIncidents);
    }
}
```

We can also configure the incident strategy for a workflow via Elsa Studio:

![Incident strategy setting](/incidents/workflow-definition-incident-settings.png)