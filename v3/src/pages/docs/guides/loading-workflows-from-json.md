---
title: Loading workflows from JSON
description: Loading & running workflow from JSON
---

Loading workflows from JSON is a great way to store workflows in a database or file system. This guide will show you how to load workflows from JSON files.

## Loading workflows from JSON files

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