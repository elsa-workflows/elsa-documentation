---
title: Extending JavaScript
---

The JavaScript expression evaluator supports a number of functions one can use when creating workflow expressions using the JavaScript syntax, but there will be times where this simply isn't enough.

It's those times that you will need to extend the available set of functions with that of your own.

## EvaluatingJavaScriptExpression Notification

very time Elsa is about to evaluate a JavaScript expression, it will publish a `EvaluatingJavaScriptExpression` notification. Handling this notification allows your application or module to configure the JavaScript engine.

For example:

```c#
public class SayHelloJavaScriptHandler : INotificationHandler<EvaluatingJavaScriptExpression>
{
    public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
    {
        var engine = notification.Engine;
        engine.SetValue("sayHello", (Func<string, object>) (message => $"Hello {message}!");
        return Task.CompletedTask;
    }
}
```

This allows the user to use the following JavaScript expression: `sayHello("John")`, which will evaluate to `"Hello John!"`

## Service Registration.
To register any and all notification handler implementations in a given assembly, use the following extension method on `IServiceCollection`:

`services.AddNotificationHandlers(typeof(SomeMarkerTypeFromMyAssembly))`.

This will register all `INotificationHandler<T>` implementations from the assembly containing `SomeMarkerTypeFromMyAssembly`. 

## Jint

Elsa uses [Jint](https://github.com/sebastienros/jint), an open source JavaScript interpreter. For more information on how to further customize the engine, checkout their documentation. 
  