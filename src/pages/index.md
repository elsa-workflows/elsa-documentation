---
title: Getting started
pageTitle: Elsa Workflows - Add workflow capabilities to any .NET project.
description: Build workflow-driven applications using Elsa Workflows.
---

Learn how to get Elsa Workflows set up in your project in under ten minutes or it's free. {% .lead %}

{% quick-links %}

{% quick-link title="Installation" icon="installation" href="/" description="Step-by-step guides to setting up your system and installing the library." /%}

{% quick-link title="Architecture guide" icon="presets" href="/" description="Learn how the internals work and contribute." /%}

{% quick-link title="Modules" icon="plugins" href="/" description="Extend the library with third-party modules or write your own." /%}

{% quick-link title="API reference" icon="theming" href="/" description="Learn to easily customize and extend your workflow application." /%}

{% /quick-links %}

---

## Quick start

Elsa can be used in different settings. The most common one is having a dashboard app hosting the designer and a server app hosting the workflow engine, but you can also serve both from the same app. 

Other settings include basic workflow execution from a console app, a worker service, or front-end apps like WinForms, WPF, UWP and Xamarin.

For the quick start, we will setup two ASP.NET applications, one representing the workflow server that exposes API endpoints and the other on representing the dashboard that consumes the API endpoints.
We will also look at securing access to the API and dashboard by using [OpenIddict](https://github.com/openiddict/openiddict-core).

{% callout type="warning" title="Protect your endpoints!" %}
When hosting a workflow server that exposes API endpoints to manage & execute workflows, you should always take care of protecting access to them. We will see how in the quickstart.
{% /callout %}

---

## Basic usage

Besides the ability to execute workflows from a host as demonstarted in the quickstart, the most direct way to run a workflow is to use the `IWorkflowRunner` service from your application. For example:

```clike
using Elsa.Extensions;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;
using Microsoft.Extensions.DependencyInjection;

// Setup a service container from which we can resolve Elsa services.
var services = new ServiceCollection();

// Add Elsa services to the service collection.
services.AddElsa();

// Build a service provider.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Define a workflow.
var workflow = new WriteLine("Hello World!");

// Execute the workflow.
workflowRunner.RunAsync(workflow);
```

### Workflows

In Elsa, a workflow is a type that contains roughly three types of information:

- Metadata about the workflow (name, version, etc.)
- Configuration for the workflow (variables)
- Main activity (the root node of the graph to start executing)

In the previous example, we seemed to execute an activity (`WriteLine`) directly, but under the hood, a new `Workflow` object was created.
Creating a workflow manually looks like this:

```clike
var workflow = new Workflow(new WriteLine("Hello World!"));
```


You may be wondering: how does one create a workflow with more than just one step?

Simple! just use a _composite_ activity such as `Sequence`, `Flowchart` or even a custom one.

{% callout title="Composite Activities" %}
A composite activity is an activity that itself contains one ore more activities that it will execute as part of its operation.
Some examples of composite activities are `Sequence`, `Flowchart`,`If`, `While` and `Switch`.
Even `Workflow` is a composite activity!
{% /callout %}

For example, the following workflow will execute a series of steps:

```clike
var workflow = new Workflow
{
    Root = new Sequence
    {
        Activities =
        {
            new WriteLine("Hello World!"),
            new WriteLine("Goodbye cruel world..."),
        }
    }
};
```

### Workflow Hosting

So far we only looked at simple workflows that are executed manually using the `IWorkflowRunner` service.
However, to run workflows that can be suspended & resumed based on triggers such as timers and other events, we need a system that knows about these workflows.

This system is called the `IWorkflowDefinitionStore`, and effectively represents a repository of **workflow definitions** available to the system.

Checkout the Workflow Hosting page to learn everything there is to know about hosting workflows.

## Getting help

Documentation can go only so far. You may encounter scenarios that has not been covered. You may have questions. Perhaps an idea for a new feature or an improvement of an existing feature. And believe it or not, there may be bugs!
Whatever the case may be, we are here to help!

### Submit an issue

Whenever you encounter an issue or have a great idea, please submit an issue on GitHub.

### Ask questions

There are different places where you can ask questions.

- Github Discussions
- Stack Overflow
- Discord

### Join the community

We have a friendly community on Discord, and you're invited!

We also host a weekly community meeting where we talk about anything & everything, though most commonly about Elsa features, ideas and demos.
The meeting takes place on Discord every Tuesday 19:00 UTC.

Everyone is welcome!