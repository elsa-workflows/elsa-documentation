---
title: Loading workflows from JSON
description: Loading & running workflow from JSON
---

Loading workflows from JSON is a great way to store workflows in a database or file system. This guide will show you how to load workflows from JSON files.

## Console application

The most straightforward way to load workflows from JSON files is to simply load the contents of a JSON file, deserialize it and then execute the deserialized workflow.

Here's a complete Program.cs file for a console application that demonstrates how to load a workflow from a JSON file and execute it:

```clike
using Elsa.Extensions;
using Elsa.Testing.Shared;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Models;
using Microsoft.Extensions.DependencyInjection;

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Populate registries. This is only necessary for applications  that are not using hosted services.
await serviceProvider.PopulateRegistriesAsync();

// Import a workflow from a JSON file.
var workflowJson = await File.ReadAllTextAsync("HelloWorld.json");

// Get a serializer to deserialize the workflow.
var serializer = serviceProvider.GetRequiredService<IActivitySerializer>();

// Deserialize the workflow.
var workflow = serializer.Deserialize<Workflow>(workflowJson);

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Run the workflow.
await workflowRunner.RunAsync(workflow);
```

The HelloWorld.json is a file stored in the same directory as Program.cs and is configured to be copied to the output directory if its newer than the file in the output directory.

```json
{
  "id": "HelloWorld-v1",
  "definitionId": "HelloWorld",
  "name": "Hello World",
  "root": {
    "id": "Flowchart1",
    "type": "Elsa.Flowchart",
    "activities": [
      {
        "id": "WriteLine1",
        "type": "Elsa.WriteLine",
        "text": {
          "typeName": "String",
          "expression": {
            "type": "Literal",
            "value": "Hello World!"
          },
          "memoryReference": {
            "id": "WriteLine1:input-1"
          }
        }
      }
    ]
  }
}
```

### ASP.NET application

When running Elsa in an ASP.NET application, you might want to make available your workflows stored in JSON to the workflow runtime.

The workflow runtime relies on `IWorkflowDefinitionProvider` implementations to provide it with workflow definitions.
Out of the box, Elsa comes with the following providers:

- ClrWorkflowDefinitionProvider
- FluentStorageWorkflowDefinitionProvider

The `ClrWorkflowDefinitionProvider` is responsible for providing workflow definitions created as .NET types implementing `IWorkflow`.
The `FluentStorageWorkflowDefinitionProvider` is responsible for providing workflow definitions provided via `IBlobStorage`, which is an API provided by the [FluentStorage](https://github.com/robinrodricks/FluentStorage) library.

In addition to the built-in providers, you can also implement your own `IWorkflowDefinitionProvider` to provide workflow definitions from any source you want.

For this chapter, however, it suffices to use the `FluentStorageWorkflowDefinitionProvider` to provide workflow definitions from JSON files stored in a file system.
To set this up, you need to do the following:

1. Setup a workflow server application as described in the [ASP.NET workflow server](../installation/aspnet-apps-workflow-server) chapter.
2. Add a package reference to `Elsa.WorkflowProviders.FluentStorage` to the workflow server project.
3. Update Program.cs to use the `FluentStorageWorkflowDefinitionProvider`:

```clike
// Add Elsa services.
services.AddElsa(elsa => elsa
    // Add the Fluent Storage workflow definition provider.
    .UseFluentStorageProvider()
    ... // Other configuration.
```

By default, the fluent storage provider will look for workflow definitions in the `Workflows` folder.
To try it out, create the `Workflows` folder in the root of the project and create a new file called `hello-world.jsonz with the following contents:

```json
{
  "id": "hello-world-v1",
  "definitionId": "hello-world",
  "name": "Hello World",
  "isLatest": true,
  "isPublished": true,
  "version": 1,
  "root": {
    "id": "Flowchart1",
    "type": "Elsa.Flowchart",
    "activities": [
      {
        "id": "WriteLine1",
        "type": "Elsa.WriteLine",
        "text": {
          "typeName": "String",
          "expression": {
            "type": "Literal",
            "value": "Hello World!"
          },
          "memoryReference": {
            "id": "WriteLine1:input-1"
          }
        }
      }
    ]
  }
}
```

Take note of the workflow definition ID, which is `hello-world`.
This is the ID you use when invoking the workflow using the REST API, for example.

```
curl --location --request POST 'https://localhost:5001/elsa/api/workflow-definitions/hello-world/execute' \
--header 'Authorization: ApiKey {your_api_key}'
```