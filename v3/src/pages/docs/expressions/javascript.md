---
title: JavaScript expressions
description: A glossary of JavaScript expressions.
---

When working with Elsa, you'll often need to write JavaScript expressions. This page provides a glossary of JavaScript expressions that you can use.

## Glossary

| Function                                            | Description                                                                                                                                                                   | Example                    |
|-----------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------|
| **Common JavaScript functions**                     | These are built-in JS functions that are common to most JavaScript implementations.                                                                                           |                            |
| `JSON.parse`                                        | Parse a JavaScript object. Also works with `ExpandoObject` objects.                                                                                                           |                            |
|                                                     |                                                                                                                                                                               |                            |
| **Elsa specific functions**                         |                                                                                                                                                                               |                            |
| `getVariable('myVariable')`                         | Gets a variable from the workflow.                                                                                                                                            |                            |
| `setVariable('myVariable', 'myValue')`              | Sets a variable in the workflow.                                                                                                                                              |                            |
| `get{MyVariable}()`                                 | Gets a variable from the workflow. This is a shorthand for `getVariable('myVariable')`.                                                                                       | `getMyVariable()`          |
| `set{MyVariable}('myValue')`                        | Sets a variable in the workflow. This is a shorthand for `setVariable('myVariable', 'myValue')`                                                                               | `setMyVariable('myValue')` |
| `getInput('name')`                                  | Gets the input of the workflow.                                                                                                                                               |                            |
| `getWorkflowInstanceId()`                           | Gets the ID of the workflow instance.                                                                                                                                         |                            |
| `getCorrelationId()`                                | Gets the correlation ID of the workflow instance.                                                                                                                             |                            |
| `setCorrelationId('myCorrelationId')`               | Sets the correlation ID of the workflow instance.                                                                                                                             |                            |
| `isNullOrWhiteSpace('')`                            | Returns `true` if the string is `null`, empty, or consists only of white-space characters.                                                                                    |                            |
| `isNullOrEmpty('')`                                 | Returns `true` if the string is `null` or empty.                                                                                                                              |                            |
| `parseGuid('00000000-0000-0000-0000-000000000000')` | Parses a string into a GUID.                                                                                                                                                  |                            |
| `newGuid()`                                         | Generates a new GUID.                                                                                                                                                         |                            |
| `newGuidString()`                                   | Generates a new GUID and returns it as a string.                                                                                                                              |                            |
| `newShortGuid()`                                    | Generates a new GUID and returns it as a short string.                                                                                                                        |                            |
| `toJson('{"name": "Alice"}')`                       | Converts an object to JSON. Use this instead of `JSON.stringify()` when you need to serialize a .NET object to JSON. `JSON.stringify()` only works on `ExpandObject` objects. |                            |

## Explanations

Let's take a look at a few examples.

### Workflow variables

There are two ways to get variables from the workflow:

- `getVariable('myVariable')`
- `getMyVariable()`

The first method is useful when you want to get a variable whose name is not known at compile time.
The second method is useful when you know the name of the variable at compile time, which has the added benefit of providing intellisense.

Similarly, there are two ways to set variables in the workflow:

- `setVariable('myVariable', 'myValue')`
- `setMyVariable('myValue')`

### Workflow input

To get the input of the workflow, use the `getInput()` function.

For example, if you run a workflow providing the following input:

```json
{
  "input": {
    "name": "Alice"
  }
}
```

You can get the `name` property using the following expression:

```javascript
getInput('name')
```