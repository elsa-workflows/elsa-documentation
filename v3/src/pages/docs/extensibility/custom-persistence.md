---
title: Custom persistence
description: Writing custom persistence providers.
---

In this chapter, we will discuss writing custom persistence providers.

## Persistence providers

Each Elsa module might expose it own persistence abstractions.
Out of the box, the following persistence abstractions re provided by the following modules:

| **Module**                | **Store abstractions**                                                                 |
|---------------------------|----------------------------------------------------------------------------------------|
| Elsa.Workflows.Management | `IWorkflowDefinitionStore`, `IWorkflowInstanceStore`                                   |
| Elsa.Workflows.Runtime    | `IWorkflowStateStore`, `ITriggerStore`, `IBookmarkStore`, `IWorkflowExecutionLogStore` |
| Elsa.Identity             | `IApplicationStore`, `IRoleStore`, `IUserStore`                                        |

When writing a custom persistence provider, you need to implement the corresponding store abstractions and ideally expose them as a *feature* for ease of use.

{% callout %}
You only need to implement the store abstractions for the modules you want to use. For example, if you only want to use the `Elsa.Workflows.Runtime` module, you only need to implement the store abstractions for that module.
{% /callout %}

## Default implementations

Each module provides a default **in-memory** implementation of the store abstractions it exposes.

Because of this modularity, your application can mix & match persistence providers.

For example, you could use the default in-memory implementation of the `IWorkflowDefinitionStore` and a custom implementation of the `IWorkflowInstanceStore`.

Elsa ships with the following default implementations:

### Memory

| **Module**                | **Store abstractions**                                                                                     |
|---------------------------|------------------------------------------------------------------------------------------------------------|
| Elsa.Workflows.Management | `MemoryWorkflowDefinitionStore`, `MemoryWorkflowInstanceStore`                                             |
| Elsa.Workflows.Runtime    | `MemoryWorkflowStateStore`, `MemoryTriggerStore`, `MemoryBookmarkStore`, `MemoryWorkflowExecutionLogStore` |
| Elsa.Identity             | `MemoryApplicationStore`, `MemoryRoleStore`, `MemoryUserStore`                                             |

### Entity Framework Core

| **Module**                | **Store abstractions**                                                                                     |
|---------------------------|------------------------------------------------------------------------------------------------------------|
| Elsa.Workflows.Management | `EFCoreWorkflowDefinitionStore`, `EFCoreWorkflowInstanceStore`                                             |
| Elsa.Workflows.Runtime    | `EFCoreWorkflowStateStore`, `EFCoreTriggerStore`, `EFCoreBookmarkStore`, `EFCoreWorkflowExecutionLogStore` |
| Elsa.Identity             | `EFCoreApplicationStore`, `EFCoreRoleStore`, `EFCoreCoreUserStore`                                         |

### Dapper

| **Module**                | **Store abstractions**                                                                                     |
|---------------------------|------------------------------------------------------------------------------------------------------------|
| Elsa.Workflows.Management | `DapperWorkflowDefinitionStore`, `DapperWorkflowInstanceStore`                                             |
| Elsa.Workflows.Runtime    | `DapperWorkflowStateStore`, `DapperTriggerStore`, `DapperBookmarkStore`, `DapperWorkflowExecutionLogStore` |
| Elsa.Identity             | `DapperApplicationStore`, `DapperRoleStore`, `DapperCoreUserStore`                                         |

### MongoDB (WIP)

** in progress **

## Writing custom persistence providers

To implement a custom provider for a given module, you need to implement the corresponding store abstractions.
For example, if you want to implement a custom persistence provider for the `Elsa.Workflows.Runtime` module, you need to implement the following store abstractions:

- `IWorkflowStateStore`
- `ITriggerStore`
- `IBookmarkStore`
- `IWorkflowExecutionLogStore`

Next, you need to register your custom store implementations with the workflow runtime.

To do so, open `Startup.cs` and add the following line to the elsa services configuration:

```clike
services.AddElsa(elsa => elsa.Configure<DefaultWorkflowRuntimeFeature>(feature => 
{ 
    feature.WorkflowStateStore = sp => sp.GetRequiredService<DapperWorkflowStateStore>();
    feature.TriggerStore = sp => sp.GetRequiredService<DapperTriggerStore>();
    feature.BookmarkStore = sp => sp.GetRequiredService<DapperBookmarkStore>();
    feature.WorkflowExecutionLogStore = sp => sp.GetRequiredService<DapperWorkflowExecutionLogStore>(); 
});

services.AddSingleton<DapperWorkflowStateStore>();
services.AddSingleton<DapperTriggerStore>();
services.AddSingleton<DapperBookmarkStore>();
services.AddSingleton<DapperWorkflowExecutionLogStore>();
```

For complete examples, see the following projects:

- Elsa.Dapper
- Elsa.EntityFrameworkCore
- Elsa.MongoDb (WIP)