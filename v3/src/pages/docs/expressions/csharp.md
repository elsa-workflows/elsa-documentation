---
title: C# expressions
description: A glossary of C# expressions.
---

When working with Elsa, you'll often need to write dynamic expressions. This page provides a glossary of C# expressions that you can use.

Elsa uses [Roslyn](https://github.com/dotnet/roslyn) to implement the C# expression evaluator..

## Common

Out of the box, all types are registered from the System and System.Linq namespace. 

## Activity output

The following functions are specific to handling activity output.

| Function                                | Description                                       | Example                          |
|-----------------------------------------|---------------------------------------------------|----------------------------------|
| `Output.Get(string, string?): object?` | Gets the output of the activity by activity name. | `Output.Get("HttpEndpoint1")`   |
| `Output.LastResult: object?`            | Gets the last result.                             | `Output.LastResult`              |

## Workflow variables and input

The following functions are specific to handling workflow variables and input.

| Function                            | Description                                | Example                                 |
|-------------------------------------|--------------------------------------------|-----------------------------------------|
| `Variables.Get(string): object?`    | Gets a variable from the workflow.         | `Variable.Get("MyVariable")`            |
| `Variables.Set(string, object?)`    | Sets a variable in the workflow.           | `Variable.Set("MyVariable", "myValue")` |
| `Variables.{VariableName}: object?` | Gets or sets a variable on the workflow.   | `Variable.MyVariable`                   |
| `Input.Get(string): object?`        | Gets the input of the workflow.            | `Input.Get('name')`                     |

### Workflow variables

There are two ways to get variables from the workflow:

- `Variables.Get("MyVariable")`
- `Variables.MyVariable`

The first method is useful when you want to get a variable whose name is not known at build-time.
The second method is useful when you know the name of the variable at build-time, which has the added benefit of providing intellisense.

Similarly, there are two ways to set variables in the workflow:

- `Variables.Set('MyVariable', "myValue");`
- `Variables.MyVariable = "myValue";`

### Workflow input

To get the input of the workflow, use the `Input.Get(string)` function.

For example, if you run a workflow providing the following input:

```json
{
  "input": {
    "name": "Alice"
  }
}
```

You can get the `name` field using the following expression:

```clike
Input.Get("name")
```

## Workflow

These functions and properties are specific to working with workflows. 

| Function or Property         | Description                                                     | Example                                             |
|------------------------------|-----------------------------------------------------------------|-----------------------------------------------------|
| `WorkflowInstanceId: string` | Gets the ID of the workflow instance.                           | `WorkflowInstanceId`                                |
| `CorrelationId: string`      | Gets or sets the correlation ID of the workflow instance.       | `CorrelationId`                                     |
