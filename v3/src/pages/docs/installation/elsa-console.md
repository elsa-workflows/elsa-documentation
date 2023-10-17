---
title: Elsa Console Application
description: A step-by-step guide to integrating Elsa into a console application.
---

Elsa Workflows can be seamlessly integrated into console applications, offering a lightweight approach to run workflows. This guide will walk you through setting up Elsa in a console application and running a simple workflow.

## **Initial Setup**

### **1. Create a New Console Application**

Start by creating a new console application targeting .NET 7.0:

```shell
dotnet new console -n "ElsaConsole" -f net7.0
```

Navigate to your newly created project's root directory:

```shell
cd ElsaConsole
```

### **2. Install Elsa NuGet Package**

To integrate Elsa into your console application, add the Elsa NuGet package:

```shell
dotnet add package Elsa
```

## **Configuring Elsa**

### **1. Update Program.cs**

Open `Program.cs` and replace its contents with the following code to set up Elsa:

```clike
using Elsa.Extensions;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;
using Microsoft.Extensions.DependencyInjection;

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services to the container.
services.AddElsa();

// Build the service container.
var serviceProvider = services.BuildServiceProvider();
```

This code sets up a service container and adds Elsa services to it. The `serviceProvider` will allow us to resolve Elsa services and run workflows.

### **2. Define and Run a Workflow**

To test Elsa in action, append the following code to `Program.cs`:

```clike
// Define a simple workflow that writes a message to the console.
var workflow = new WriteLine("Hello world!");

// Resolve a workflow runner to execute the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Execute the workflow.
await workflowRunner.RunAsync(workflow);
```

Running the program should display:

```shell
Hello world!
```

### **3. Experimenting with Workflows**

Elsa's flexibility allows you to define more complex workflows. For instance, replace the `workflow` variable with the following sequence of activities:

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

Executing the program now should produce:

```shell
Hello World!
Goodbye cruel world...
```

This example demonstrates how you can chain multiple activities in a sequence. As you delve deeper into Elsa, you'll discover a plethora of activities and configurations to tailor workflows to your needs.
