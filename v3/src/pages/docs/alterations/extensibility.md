---
title: Extensibility
description: A description of the Alterations module's extensibility points.
---

Elsa Workflows supports custom alteration types, allowing developers to define their own types and utilize them as
alterations.

To define a custom alteration type, implement the `IAlteration` interface.

```clike
public interface IAlteration
{
}
```

Next, implement an **alteration handler** that handles the alteration type.

```clike
public interface IAlterationHandler where T : IAlteration
{
    bool CanHandle(IAlteration alteration);
    ValueTask HandleAsync(AlterationHandlerContext context);
}
```

Or, derive from the `AlterationHandlerBase<T>` base class to simplify the implementation.

Finally, register the alteration handler with the service collection.

```clike
services.AddElsa(elsa => 
{
    elsa.UseAlterations(alterations => 
    {
        alterations.AddAlteration<MyAlteration, MyAlterationHandler>();
    })
});
```

## Example

The following example demonstrates how to define a custom alteration type and handler.

```clike
public class MyAlteration : IAlteration
{
    public string Message { get; set; }
}

public class MyAlterationHandler : AlterationHandlerBase<MyAlteration>
{
    public override async ValueTask HandleAsync(AlterationHandlerContext<MyAlteration> context, CancellationToken cancellationToken = default)
    {
        context.WorkflowExecutionContext.Output.Add("Message", context.Alteration.Message);
    }
}
```