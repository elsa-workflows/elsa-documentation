---
title: JavaScript expressions
description: A glossary of JavaScript expressions.
---

When working with Elsa, you'll often need to write JavaScript expressions. This page provides a glossary of JavaScript expressions that you can use.

Elsa uses the [Jint library](https://github.com/sebastienros/jint) to implement JavaScript. More functions can be found there, as well as details on providing your own functions.

## Common

These are built-in JS functions that are common to most JavaScript implementations.

| Function                   | Description                                                                                                                                                                             | Example                             |
|----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------|
| `JSON.parse(Object)`       | Parse a JavaScript object. Also works with `ExpandoObject` objects.                                                                                                                     | `JSON.parse('{"name": "Alice"}')`   |
| `JSON.stringify(string)`   | Stringify a JavaScript object. Only works with JSON objects (which are ExpandoObjects in Jint). Use `toJson()` instead if you need to serialize .NET objects other than `ExpandoObject` | `JSON.stringify({ name: "Alice" })` |
| `parseInt(string): number` | Parses the specified string into a `number`.                                                                                                                                            | `parseInt('42')`                    |

## Activity output

The following functions are specific to handling activity output.

| Function                                 | Description                                     | Example                               |
|------------------------------------------|-------------------------------------------------|---------------------------------------|
| `getOutputFrom(string, string?): any`    | Gets the output of the activity by activity ID. | `getOutputFrom('HttpEndpoint1')`      |
| `get{OutputName}From{ActivityId}(): any` | Gets the output of the activity by activity ID. | `getParsedContentFromHttpEndpoint1()` |
| `getLastResult(): any`                   | Gets the last result.                           | `getLastResult()`                     |

## Workflow variables and input

The following functions are specific to handling workflow variables and input.

| Function                   | Description                        | Example                                |
|----------------------------|------------------------------------|----------------------------------------|
| `getVariable(string): any` | Gets a variable from the workflow. | `getVariable('myVariable')`            |
| `setVariable(string, any)` | Sets a variable in the workflow.   | `setVariable('myVariable', 'myValue')` |
| `get{VariableName}(): any` | Gets a variable from the workflow. | `getMyVariable()`                      |
| `set{VariableName}(any)`   | Sets a variable in the workflow.   | `setMyVariable('myValue')`             |
| `getInput(string): any`    | Gets the input of the workflow.    | `getInput('name')`                     |

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

## Workflow

These functions are specific to working with workflows. 

| Function                              | Description                                                                                                                                                                   | Example                                             |
|---------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|
| `getWorkflowInstanceId(): string`     | Gets the ID of the workflow instance.                                                                                                                                         | `getWorkflowInstanceId()`                           |
| `getCorrelationId(): string`          | Gets the correlation ID of the workflow instance.                                                                                                                             | `getCorrelationId()`                                |
| `setCorrelationId(string)`            | Sets the correlation ID of the workflow instance.                                                                                                                             | `setCorrelationId('myCorrelationId')`               |
| `isNullOrWhiteSpace(string): boolean` | Returns `true` if the string is `null`, empty, or consists only of white-space characters.                                                                                    | `isNullOrWhiteSpace('')`                            |
| `isNullOrEmpty(string): boolean`      | Returns `true` if the string is `null` or empty.                                                                                                                              | `isNullOrEmpty('')`                                 |
| `parseGuid(string): Guid`             | Parses a string into a GUID.                                                                                                                                                  | `parseGuid('00000000-0000-0000-0000-000000000000')` |
| `newGuid(): Guid`                     | Generates a new GUID.                                                                                                                                                         | `newGuid()`                                         |
| `newGuidString(): string`             | Generates a new GUID and returns it as a string.                                                                                                                              | `newGuidString()`                                   |
| `newShortGuid(): string`              | Generates a new GUID and returns it as a short string.                                                                                                                        | `newShortGuid()`                                    |
| `toJson(any)`                         | Converts an object to JSON. Use this instead of `JSON.stringify()` when you need to serialize a .NET object to JSON. `JSON.stringify()` only works on `ExpandObject` objects. | `toJson('{"name": "Alice"}')`                       |

## Utility

These functions are useful for working with strings, numbers, GUIDs, collections and more.

| Function                              | Description                                                                                                                                                                   | Example                                             |
|---------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|
| `isNullOrWhiteSpace(string): boolean` | Returns `true` if the string is `null`, empty, or consists only of white-space characters.                                                                                    | `isNullOrWhiteSpace('')`                            |
| `isNullOrEmpty(string): boolean`      | Returns `true` if the string is `null` or empty.                                                                                                                              | `isNullOrEmpty('')`                                 |
| `parseGuid(string): Guid`             | Parses a string into a GUID.                                                                                                                                                  | `parseGuid('00000000-0000-0000-0000-000000000000')` |
| `newGuid(): Guid`                     | Generates a new GUID.                                                                                                                                                         | `newGuid()`                                         |
| `newGuidString(): string`             | Generates a new GUID and returns it as a string.                                                                                                                              | `newGuidString()`                                   |
| `newShortGuid(): string`              | Generates a new GUID and returns it as a short string.                                                                                                                        | `newShortGuid()`                                    |
| `toJson(any)`                         | Converts an object to JSON. Use this instead of `JSON.stringify()` when you need to serialize a .NET object to JSON. `JSON.stringify()` only works on `ExpandObject` objects. | `toJson('{"name": "Alice"}')`                       |
