---
title: Console apps
description: Installing Elsa in console apps.
---

## Setup

Create a new console application using the following command:

```shell
dotnet new console -n "HelloWorldConsole"
```

In your console project's root directory, run the following command:

```shell
cd HelloWorldConsole
dotnet add package Elsa
```

Next, open `Program.cs` and replace its contents with the following code:

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

At this point, the application has a service container (the `serviceProvider`) from which we can resolve Elsa services to run workflows.

## Trying it out

To try out Elsa, add the following code to `Program.cs`:

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

Although the workflow used in this example consists of just a single activity, it works the same for any activity that you give the workflow runner.
For example, replace thw `workflow` variable with the following activity structure:

```clike
var workflow = new Sequence
{
    Activities =
    {
        new WriteLine("Hello World!"), 
        new WriteLine("Goodbye cruel world...")
    }
};
```

When you now run the program, you should see the following console output:

```shell
Hello World!
Goodbye cruel world...
```