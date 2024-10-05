---
title: Workflow
description: A description of workflows.
---

A workflow is a sequence of activities that can be executed in a variety of ways.
In Elsa, a workflow is represented by an instance of the `Workflow` class, and is itself an activity. To be precise, it is a _composite activity_ and exposes a`Root` property that can be used to define the root activity of the workflow.
The `Root` property is of type `IActivity`, which is the base interface for all activities.

---

## Workflow creation

The following is an example of instantiating a Workflow object and setting its `Root` property to a `Sequence` activity:

```clike
var workflow = new Workflow
{
    Root = new Sequence
    {
        Activities = 
        {
            new WriteLine("Hello World!"),
            new WriteLine("Goodbye cruel world!")
        }
    }
};
```

---

## Workflow execution

When you have an instance of a `Workflow` object, you can execute it using the `WorkflowRunner` class. The `WorkflowRunner` class is responsible for executing a workflow and is the entry point for all workflow execution.:

The following is an example of executing a workflow:

```clike
var workflowRunner = services.GetRequiredService<IWorkflowRunner>(); // Or inject it in your constructor.
await workflowRunner.RunAsync(workflow);
```

## Workflow definition

Workflows can be created either from code or using the designer. When created using the designer, the workflow definition is stored in JSON form and us used to reconstruct an actual `Workflow` object at runtime.

## Workflow instance

A workflow instance is an instance of a workflow that is currently being executed and is represented by an instance of the `WorkflowState` class.
A workflow instance is created when a workflow is executed. In other words, a workflow instance is a _runtime representation_ of a workflow.

## Workflow execution context

A workflow execution context is a data structure that is passed around during workflow execution. It contains the workflow instance, the current activity, the current input and output, and more.
