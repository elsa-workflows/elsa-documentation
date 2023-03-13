---
title: Custom Expression Handlers
---

Besides **JavaScript** and **Liquid**, you can implement your own workflow expression handler.

## IExpressionHandler

To implement your own evaluator, implement `IExpressionHandler`:

```c#
public interface IExpressionHandler
{
    string Syntax { get; }
    Task<object?> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, CancellationToken cancellationToken);
}
```

The `Syntax` property represents a moniker for the syntax you are implementing. For example, the `JavaScriptExpressionHandler` returns `"JavaScript"`, while `LiquidHandler` returns `"Liquid"`.
When the user configures a workflow expression, they will select one of the available syntaxes.
 
Your handler should be able to evaluate the specified expression, and if specified, convert the result to the specified `returnType`.

## Service Registration

To register your expression handler, use the `TryAddProvider<T>` extension method:

```c#
services.TryAddProvider<IExpressionHandler, MyCustomExpressionHandler>(ServiceLifetime.Scoped);
```

> `TryAddProvider<T>` is like `TryAddScoped<T>`, except that it allows for multiple `IExpressionHandlers` registrations as long as the implementation type is different.
