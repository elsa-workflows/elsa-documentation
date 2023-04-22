---
title: Getting started
pageTitle: Add workflow capabilities to any .NET project.
description: Build workflow-driven applications using Elsa Workflows.
---

Learn how to get Elsa Workflows set up in your project.

{% quick-links %}

{% quick-link title="Installation" icon="installation" href="/docs/installation/packages" description="Step-by-step guides to setting up your system and installing the library." /%}

{% quick-link title="Activity library" icon="presets" href="/docs/activity-library/branching" description="Explore the activity library." /%}

{% quick-link title="Concepts" icon="plugins" href="/docs/core-concepts/programmatic-vs-designer" description="Learn more about the core concepts." /%}

{% quick-link title="Guides" icon="theming" href="/docs/guides/http-workflows" description="Learn more by doing." /%}

{% /quick-links %}

---

## Introduction

Elsa Workflows is a set of open source .NET libraries that add workflow capabilities to any .NET application.
Workflows can be created either from code or using the designer.

The programmatic API to create workflows is loosely inspired on that of Windows Workflows Foundation 4, where you can define sequential workflows for example.
For example, the following workflow prompts the user for their name and prints it out to the console:

```clike
// Define a workflow variable to capture the output of the ReadLine activity.
var nameVariable = new Variable<string>();

// Define a simple sequential workflow:
var workflow = new Sequence
{
    // Register the name variable.
    Variables = { nameVariable }, 
    
    // Setup the sequence of activities to run.
    Activities =
    {
        new WriteLine("Please tell me your name:"), 
        new ReadLine(nameVariable),
        new WriteLine(context => $"Nice to meet you, {nameVariable.Get(context)}!")
    }
};
```

Output:

![Output](./sample-workflow-console.gif)

Besides `Sequence`, Elsa also supports `Flowchart` activities, which allow you to define a workflow as a graph of activities:

```clike
public class BraidedWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder workflow)
    {
        var writeLine1 = new WriteLine("WriteLine1");
        var writeLine2 = new WriteLine("WriteLine2");
        var writeLine3 = new WriteLine("WriteLine3");
        var writeLine4 = new WriteLine("WriteLine4");
        var writeLine5 = new WriteLine("WriteLine5");
        var writeLine6 = new WriteLine("WriteLine6");
        var writeLine7 = new WriteLine("WriteLine7");

        workflow.Root = new Flowchart
        {
            Start = writeLine1,
            
            Activities =
            {
                writeLine1,
                writeLine2,
                writeLine3,
                writeLine4,
                writeLine5,
                writeLine6,
                writeLine7,
            },

            Connections =
            {
                new Connection(writeLine1, writeLine2),
                new Connection(writeLine1, writeLine3),

                new Connection(writeLine2, writeLine4),
                new Connection(writeLine2, writeLine5),

                new Connection(writeLine3, writeLine5),
                new Connection(writeLine3, writeLine6),

                new Connection(writeLine4, writeLine7),
                new Connection(writeLine5, writeLine7),
                new Connection(writeLine6, writeLine7),
            }
        };
    }
}
```

When visualized, the above graph looks like this:

![Designer](./introduction/braided-flowchart-workflow.png)

In addition to creating them in code, you can create workflows with the designer:

![Designer](./sample-workflow-designer.gif)

Workflows can execute from within your own application as well as from external applications.

Adding workflows to your application unlocks powerful capabilities, such as easy to update business logic, microservice orchestration, recurring tasks, data processing, message processing, and virtually anything else you can think of.

---

## What's new

Elsa 3 is a complete departure from previous versions of Elsa Workflows and was rebuilt from the ground up.
The new version is much more flexible and extensible, and it comes with a new designer.

The following is a list of the most important changes:

- Targets .NET 6 and up.
- Clear distinction between the core library, the management library and the runtime library, allowing for more flexibility.
- Less dependencies, which makes it easier to integrate Elsa into existing applications.
- New visual designer that boosts productivity to new levels. Think drag & drop, multi-select, undo, redo, copy & paste, and more.
- New programming model that makes it easier to create workflows from code and custom activities.
- Support for different kinds of diagrams, such as Flowchart and Sequence, with State Machine and BMPN 2.0 coming later.  
- Workflow scheduler is now queue-based by default, as opposed to stack-based in previous versions for breadth-first execution.
- Easily run activities in parallel using the `Task` and `Job` activity kinds.
- Support for different kinds of workflow runtimes. The default runtime is a simple database-based runtime, but you can also use a distributed runtime that uses Proto.Actor to run workflows lock-free across multiple nodes.
- Middleware pipeline architecture for both workflow & activity execution.
- Simplified persistence abstraction, allowing you to use any persistence technology you want.
- Workflow instances and execution log can be stored in different persistence stores, such as SQL Server and MongoDB, but also Elasticsearch.
- API endpoints are now secured by default, and can be configured to use JWT tokens, API keys, or no authentication at all.
- Activity inout & output is non-persisted by default (as opposed to previous versions of Elsa), but can be configured to be persisted by capturing them using workflow variables.
- Versatile Workflow Context support, allowing you to configure multiple workflow context providers per workflow.

---

## Features

Elsa comes packed with features that make it easy to build workflow-driven applications.

### Programmatic workflows

Workflows can be created from code. This allows you to create workflows in a strongly typed manner, and to reuse them in multiple places.

### Designed workflows

Workflows can be created using the designer. This allows you to create workflows visually, and to reuse them in multiple places.

### Short running workflows

Short running workflows are workflows that execute from start to end without entering suspension states in between. Examples are workflows that involve orchestrating short running tasks, such as sending emails, or workflows that involve executing a series of steps.

### Long running workflows

Long running workflows are workflows that can suspend and resume execution at any point in time. Examples are workflows that involve orchestrating long running tasks, such as human workflows, or workflows that involve waiting for external events to occur.

### Composite activities

Composite activities are activities that can contain other activities. Examples are `Sequence` and `Flowchart`.
You can also model your own composite activities. This can be done using code or using the designer.
These activities can be reused in other workflows.

### Triggers

Triggers are activities that can be used to start a workflow. Examples are `HttpEndpoint` and `Timer`.

### Activities

Activities are the building blocks of workflows. Examples are `WriteLine`, `ReadLine`, `SendEmail`, `HttpRequest`, `Timer`, `Fork`, `Join`, `Switch`, `While`, `DoWhile`, `ForEach`, `Delay`, `SetVariable`.
In addition, Elsa was built to be extensible, so you can easily add your own activities.

### Activity providers

Activity providers are responsible for providing activity types to the system.
For example, the `TypedActivityProvider` provides activity types based on classes that implement `IActivity`.
Another example is the `MassTransitActivityTypeProvider`, which provides activity types based on service bus message types.
Future examples will include activity types based on GraphQL, OpenAPI, JavaScript functions, and more.

### Dynamic expressions

Expressions can be used to dynamically evaluate values at runtime. For example, an expression can be used to dynamically set the value of an activity property based on other values.

### Persistence

Workflows can be persisted to a database. This allows you to resume workflows after your application has restarted.
Persistence is abstracted away, so you can plug in your own persistence provider.

### Workflow hosting

Workflows can be hosted in your own application. This allows you to execute workflows from within your own application.

### Integration with external applications

Workflows can be executed from external applications. This allows you to execute workflows from any application that can make HTTP requests.
And vice versa, workflows can make HTTP requests to external applications, either explicitly via HTTP Request activities, custom code, or implicitly via webhooks, service bus messages and gRPC.

## Join the community!

There's a friendly community around Elsa, and you're invited!

- [Github](https://github.com/elsa-workflows/elsa-core)
- [Discord](https://discord.com/invite/hhChk5H472)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/elsa-workflows)

