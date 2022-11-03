---
title: Liquid Expressions
---

## Accessing Custom Types

Liquid is a secure template language which will only allow a predefined set of members to be accessed, and where model members can't be changed. Property are added to the TemplateOptions.MemberAccessStrategy property. This options object can be reused every time a template is rendered.

Alternatively, the `MemberAccessStrategy` can be assigned an instance of `UnsafeMemberAccessStrategy` which will allow any property to be accessed.

### Allow-listing a specific type

To configure the liquid engine, implement `INotificationHandler<EvaluatingLiquidExpression>`. This notification handler will be invoked every time Elsa is about to evaluate a liquid expression.

The following example demonstrates how to allow access to a custom type called `MyType` from liquid expressions:

```c#
public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
{
    public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
    {
        notification.TemplateContext.Options.MemberAccessStrategy.Register<Mytype>();
    }
}
```

Make sure to register your handler with DI:

```c#
services.AddNotificationHandler<EvaluatingLiquidExpression, ConfigureLiquidEngine>();
```

For more information about stuff you can do with Fluid, the engine used by Elsa for liquid handling, checkout [their documentation](https://github.com/sebastienros/fluid#basic-overview).

## Liquid Expressions

The following Liquid expressions are supported:

## Common Variables

### Workflow Variables

Use the following syntax to access a workflow variable: 

```liquid
{{ Variables.NameOfVariable }}
```

For example, given a workflow variable called `FirstName` with a value of `"Alice"`, the expression`Hello {{ Variables.FirstName }}` will result in `Hello Alice`.  

### Input

Input values can be accessed using the following syntax: 

```liquid
{{ Input }}
```

### Activity Output

To access a named activity's output, use the following syntax: 

```liquid
{{ Activities.SomeActivityName.Output }}
```

### CorrelationId

Returns the correlation ID (if any) of the currently executing workflow.

```liquid
{{ CorrelationId }}
```

### WorkflowInstanceId

Returns the workflow instance ID of the currently executing workflow.

```liquid
{{ WorkflowInstanceId }}
```

### WorkflowDefinitionId

Returns the workflow definition ID of the currently executing workflow.

```liquid
{{ WorkflowDefinitionId }}
```

### WorkflowDefinitionVersion

Returns the workflow definition version of the currently executing workflow.

```liquid
{{ WorkflowDefinitionVersion }}
```

### Configuration

Provides access to a .NET configuration value.

```liquid
{{ Configuration.SomeSection }}
```

As an example, let's say you have the following JSON in `appsettings.json`:

```json
{
  "Elsa": {
    "Smtp": {
      "Host": "localhost",
      "Port": 2525
    }
  }
}
```

You can access the configured `Port` value using the following expression:

```liquid
{{ Configuration.Elsa.Smtp.Port }}
```

## Common Filters

### json

A liquid filter that renders the specified value as a JSON string.

```liquid
{{ Input | json }}
```

Example output:

```json
{
  "SomeDocument": {
    "Title": "About Elsa Workflows"
  }
}
```

### base64

A liquid filter that renders the specified value as a bas64 representation.
The value is first converted to a string. If the value is an object, array, dictionary or datetime, it is first serialized using `JsonConvert.SerializeObject` before being encoded as base64.

```liquid
{{ "Some string value" | base64 }}
```

Example output:

```text
U29tZSBzdHJpbmcgdmFsdWU=
```

## Workflow Filters

### workflow_definition_id

Translates the specified **workflow name** or **workflow tag** into a **workflow ID**.
This is useful for activities such as `RunWorkflow` which require a workflow ID to run.

Usage:

```liquid
{{ "SomeWorkflowName" | workflow_definition_id }}
```

```liquid
{{ "SomeWorkflowTag" | workflow_definition_id: tag }}
```

## HTTP Variables

### Request

Provides access to various properties on the current HTTP Request object:

- `{{ Request.QueryString }}`
- `{{ Request.ContentType }}`
- `{{ Request.ContentLength }}`
- `{{ Request.Form }}`
- `{{ Request.Protocol }}`
- `{{ Request.Path }}`
- `{{ Request.PathBase }}`
- `{{ Request.Host }}`
- `{{ Request.IsHttps }}`
- `{{ Request.Scheme }}`
- `{{ Request.Method }}`

## HTTP Filters

### signal_url

A liquid filter that generates a fully-qualified absolute signal URL that will trigger the workflow instance from which this function is invoked.

Example: 

```liquid
{{ "MySignal" | signal_url }}
```

Example output:

`https://localhost:5001/signals/trigger/{some base64 token}`
