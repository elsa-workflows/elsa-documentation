---
title: Alteration Plans
description: A description of alteration plans.
---

An alteration plan represents a collection of alterations that can be applied to a workflow instance or a set of
workflow instances.

## Creating Alteration Plans

To create an alteration plan, create a new instance of the `NewAlterationPlan` class. For example:

```clike
var plan = new NewAlterationPlan
{
    Alterations = new List<IAlteration>
    {
        new ModifyVariable("MyVariable", "MyValue")
    },
    WorkflowInstanceIds = new[] { "26cf02e60d4a4be7b99a8588b7ac3bb9" } 
};
```

## Submitting Alteration Plans

To submit an alteration plan, use the `IAlterationPlanScheduler` service. For example:

```clike

var scheduler = serviceProvider.GetRequiredService<IAlterationPlanScheduler>();
var planId = await scheduler.SubmitAsync(plan, cancellationToken);
```

When a plan is submitted, an **alteration job** is created for each workflow instance, to which each alteration will be
applied.

Alteration plans are executed asynchronously in the background. To monitor the execution of an alteration plan, use
the `IAlterationPlanStore` service. For example:

```clike
var store = serviceProvider.GetRequiredService<IAlterationPlanStore>();
var plan = await _alterationPlanStore.FindAsync(new AlterationPlanFilter { Id = planId }, cancellationToken);
```

To get the alteration jobs that were created as part of the plan, use the `IAlterationJobStore` service. For example:

```clike
var store = serviceProvider.GetRequiredService<IAlterationJobStore>();
var jobs = (await _alterationJobStore.FindManyAsync(new AlterationJobFilter { PlanId = planId }, cancellationToken)).ToList();
```