---
title: Extending Liquid
---

The Liquid expression evaluator supports a number of filters one can use when creating workflow expressions using the Liquid syntax.
There might be occasions where you would like to extend the set of filters with your own.
Additionally, when your workflow operates on certain object models, you might want to be able to access these models. This is not allowed by default by the Liquid evaluator.
Instead, one must *whitelist* these object models' types.  

It's those times that you will need to extend the liquid syntax.

## EvaluatingLiquidExpression Notification

Every time Elsa is about to evaluate a Liquid expression, it will publish a `EvaluatingLiquidExpression` notification. Handling this notification allows your application or module to configure the Liquid Template Context.

For example, the following handler whitelists access to a type called `Customer` and adds a custom filter to turn a string into all caps:

```c#
public class MyLiquidHandler : INotificationHandler<EvaluatingLiquidExpression>
{
    public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
    {
        var context = notification.TemplateContext;
        context.MemberAccessStrategy.Register<Customer>();
        context.Filters.AddFilter("all_caps", AllCaps);
        return Task.CompletedTask;
    }
    
    private static FluidValue AllCaps(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        return new StringValue(input.ToStringValue().ToUpper());
    }
}
```

Assuming there is a workflow variable called `Customer` of type `Customer, this allows you to use the following Liquid expression: `Hello, {{ Customer.FullName }}!`, which will evaluate to `"Hello John!"`

## Service Registration.
To register any and all notification handler implementations in a given assembly, use the following extension method on `IServiceCollection`:

`services.AddNotificationHandlers(typeof(SomeMarkerTypeFromMyAssembly))`.

This will register all `INotificationHandler<T>` implementations from the assembly containing `SomeMarkerTypeFromMyAssembly`. 

## Custom Liquid Filters

Instead of adding filters directly to a TemplateContext, you can also implement `ILiquidFilter` and register it with the IoC service container.
For example, the following is a custom filter that transforms the input string to all caps:

```c#
public class AllCapsFilter : ILiquidFilter
{
    public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        return new ValueTask<FluidValue>(input.ToStringValue().ToUpper());
    }
} 
```

Then register the filter as follows:

```c#
services.AddLiquidFilter<AllCapsFilter>("all_caps");
```

The filter can then be used in Liquid expressions: 

Liquid: `Hello {{ 'John' | all_caps }}`

Result: `Hello JOHN`

## Fluid

Elsa uses [Fluid](https://github.com/sebastienros/fluid), an open source Liquid interpreter. For more information on how to further customize liquid, checkout their documentation. 