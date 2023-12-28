---
title: Python expressions
description: A glossary of Python expressions.
---

When working with Elsa, you'll often need to write dynamic expressions. This page provides a glossary of Python 3 expressions that you can use.

Elsa uses [IronPython](https://github.com/IronLanguages/ironpython3) to implement the Python expression evaluator.

## Activity output

The following functions are specific to handling activity output.

| Function                               | Description                                       | Example                       |
|----------------------------------------|---------------------------------------------------|-------------------------------|
| `output.get(string, string?): object?` | Gets the output of the activity by activity name. | `output.get('HttpEndpoint1')` |
| `output.last_result: object?`          | Gets the last result.                             | `output.last_result`          |

## Workflow variables and input

The following functions are specific to handling workflow variables and input.

| Function                            | Description                                | Example                                  |
|-------------------------------------|--------------------------------------------|------------------------------------------|
| `variables.get(string): object?`    | Gets a variable from the workflow.         | `variables.get('MyVariable')`            |
| `variables.set(string, object?)`    | Sets a variable in the workflow.           | `variables.Set('MyVariable', 'myValue')` |
| `variables.{VariableName}: object?` | Gets or sets a variable on the workflow.   | `variables.MyVariable`                   |
| `input.get(string): object?`        | Gets the input of the workflow.            | `input.get('name')`                      |

### Workflow variables

There are two ways to get variables from the workflow:

- `variables.get('MyVariable')`
- `variables.MyVariable`

The first method is useful when you want to get a variable whose name is not known at build-time.
The second method is useful when you know the name of the variable at build-time, which has the added benefit of providing intellisense.

Similarly, there are two ways to set variables in the workflow:

- `variables.set('MyVariable', "myValue");`
- `variables.MyVariable = "myValue";`

### Workflow input

To get the input of the workflow, use the `input.get(string)` function.

For example, if you run a workflow providing the following input:

```json
{
  "input": {
    "name": "Alice"
  }
}
```

You can get the `name` field using the following expression:

```python
input.get("name")
```

## Workflow

These functions and properties are specific to working with workflows.

| Function or Property                             | Description                                               | Example                                  |
|--------------------------------------------------|-----------------------------------------------------------|------------------------------------------|
| `execution_context.workflow_instance_id: string` | Gets the ID of the workflow instance.                     | `execution_context.workflow_instance_id` |
| `execution_context.correlation_id: string`       | Gets or sets the correlation ID of the workflow instance. | `execution_context.correlation_id`       |
