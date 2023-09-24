---
title: Variables
description: This section provides an in-depth explanation of variables within Elsa, including their structure, types, and storage options.
---

In Elsa, variables play a crucial role as containers for storing and retrieving data throughout a workflow's lifecycle. They facilitate the transfer of data between activities and serve as storage for data utilized by these activities.

## Structure of Variables

Each variable comprises the following components:

- **Name:** A unique, case-sensitive identifier for the variable within a workflow.
- **Type:** Defines the data type of the variable.
- **Default Value:** An optional default value assigned to the variable upon creation.
- **Storage Provider:** Specifies the location where the variable data is stored.

## Detailed Components Description

### Name

The **name** of a variable is a unique identifier within a workflow, ensuring precise variable access and manipulation.

### Default Value

A variable may possess a **default value**, used at the variable's initiation.

### Types of Variables

Variables in Elsa Workflows can be of any type supported by .NET, enhancing the flexibility and versatility of data handling. Types include but are not limited to:

- **String**
- **Number**
- **Boolean**
- **DateTime**
- **Object**
- **Array**
- **Custom types**

### Storage Providers

**Storage providers** dictate where variables are housed. Elsa Workflows offers two built-in providers:

- **Memory:** Variables are accessible only during the workflow's lifetime. Use this provider for storing temporary data and or data that is non-serializable.
- **Workflow Instance:** Variables are preserved alongside the workflow instance. Use this provider for storing data that is serializable and needs to be preserved for the workflow's lifetime.

{% callout title="Important Information on Variable Storage" type="warning" %}
- *Workflow Instance Storage*: Ensure that the variable type is serializable when using the Workflow Instance storage provider to avoid throwing an exception.
- *Memory Storage:* Remember that values will be lost if your workflow is persisted and resumed later while using the Memory storage provider.
{% /callout %}

## Extensibility

### Custom Types

Elsa Workflows supports custom types, allowing developers to define their own types and utilize them as variables. For example, a custom type named `Person` can be defined as follows:

```clike
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

To use the custom type as a variable, simply define the variable as follows:

```clike
var person = builder.WithVariable<Person>().WithMemoryStorage();
```

To make the type available from the designer, register it with the management service:

**Program.cs**

```clike
services.AddElsa(elsa => 
{
    elsa.UseWorkflowManagament(management => 
    {
        management.AddVariableType<Person>(category: "CRM");
    });
});
```

### Custom Storage Providers

For more advanced storage needs, create custom storage providers by implementing the `IStorageDriver` interface, containing:

- `WriteAsync`: Invoked upon variable creation or update.
- `ReadAsync`: Called when a variable is accessed.
- `DeleteAsync`: Operates when a variable is deleted.

Below is a basic example showcasing the implementation of a custom storage provider:

```clike
public class MyStorageDriver : IStorageDriver
{
    public Task WriteAsync(string key, object value, CancellationToken cancellationToken = default)
    {
        // Implementation for writing the value to a custom storage provider.
    }

    public Task<object?> ReadAsync(string key, CancellationToken cancellationToken = default)
    {
        // Implementation for reading the value from a custom storage provider.
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        // Implementation for deleting the value from a custom storage provider.
    }
}
```

To register the custom storage provider:

**Program.cs**

```clike
services.AddStorageDriver<MyStorageDriver>();
```

Custom storage providers broaden the possibilities, allowing variables storage in various platforms such as databases, files, Redis, cloud storage providers, and more.

## Defining Variables in Workflows

Seamlessly define variables in both programmatic and designed workflows to meet diverse development needs.

### Programmatic Workflows

In programmatic workflows, utilize the `Variable` method to define variables. Below is an illustration of variable definition with Memory storage provider:

```clike
public class Greeter : WorkflowBase
{
    public void Build(IWorkflowBuilder builder)
    {
        // Initialize a variable named "Message" with a default value of "Hello World!".
        var message = builder.WithVariable<string>("Hello World!").WithMemoryStorage();
           
        // Output the message to the console.
        builder.Root = new WriteLine(context => message.Get(context));
    }
}
```

### Designed Workflows

In designed workflows, employ the Variables panel for variable definition, as shown in the example below:

![Variables](/core-concepts/variables/create-variable.png)

## Conclusion

Understanding and effectively utilizing variables is paramount in optimizing the functionality and efficiency of Elsa Workflows. By mastering variable types, storage providers, and definition methods, enhance your workflow processes and ensure robust and reliable data handling throughout your Elsa Workflows.