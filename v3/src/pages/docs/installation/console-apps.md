---
title: Console apps
description: Installing Elsa in console apps.
---

## Setup

In your console project's root directory, run the following command:

```shell
dotnet add package Elsa
```

Next, open your Program.cs file and replace its contents with the following:

```clike
using Elsa.Extensions;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;
using Microsoft.Extensions.DependencyInjection;

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();
```

## Trying it out

To try out Elsa, add the following:

```clike
// Define a simple workflow, which in this case is a very simple activity that writes something to the console:
var workflow = new WriteLine("Hello world!");

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Run the workflow.
await workflowRunner.RunAsync(workflow);
```

When you run the program, you should see the following output in the console:

```shell
Hello world!
```

Which is evidence that Elsa is working! 🎉