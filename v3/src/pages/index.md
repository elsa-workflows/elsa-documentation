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

## Installation

Elsa can be used in different settings. The most common one is having a server app hosting the workflow engine to which a dashboard app connects. It's also common to serve both the engine as well as the dashboard from the same app. 

Other paradigms include basic workflow execution from a console app, a worker service, or front-end apps like WinForms, WPF, UWP and Xamarin.

For the quick start, we will setup two ASP.NET applications, one representing the workflow server that exposes API endpoints and the other on representing the dashboard that consumes the API endpoints.
We will also look at securing access to the API and dashboard by using [OpenIddict](https://github.com/openiddict/openiddict-core).

{% callout type="warning" title="Protect your endpoints!" %}
When hosting a workflow server that exposes API endpoints to manage & execute workflows, you should always take care of protecting access to them. We will see how in the quickstart.
{% /callout %}

---

## Basic usage

Besides the ability to execute workflows from a host as demonstrated in the quickstart, the most direct way to run a workflow is to use the `IWorkflowRunner` service from your application. For example:

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

In the previous code snippet, we executed an activity (`WriteLine`) directly, but under the hood, a new `Workflow` object was created.
Creating a workflow manually looks like this:

```clike
var workflow = new Workflow(new WriteLine("Hello World!"));
```

Up to this point, we executed only one activity. Real-world workflows, however, typically contain many more activities that are connected in a certain way. 
In Elsa, we use _composite activities_ such as `Sequence` and `Flowchart` to model different kinds of graphs.

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

We looked at simple workflows that are executed manually using the `IWorkflowRunner` service.
For simple workflows that contain no triggers or blocking activities, this is sufficient.
But to run workflows that can be suspended & resumed based on triggers such as timers and other events, we need a system that knows about these workflows.

In a nutshell, Elsa comes with a repository called the **Workflow Definition Store** with which we can register our workflow definitions.
Various components of the system will use this store to lookup workflows to execute.

For example, workflows that start with a trigger such as **Timer** are driven by a hosted service that looks up a list of triggers, which in turn are associated with workflow definitions (by ID).
Knowing a workflow definition ID is enough to find it in the workflow definition store and execute it.

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