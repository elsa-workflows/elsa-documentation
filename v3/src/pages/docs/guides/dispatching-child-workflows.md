---
title: Dispatching Child Workflows
description: Using DispatchWorkflow activity to dispatch child workflows.
---

When building workflows, you might want to reuse existing workflows as part of a larger workflow.

In Elsa, you can reuse workflows by dispatching them from other workflows.
This is done using the `DispatchWorkflow` activity.

## Dispatching a workflow

To dispatch a workflow, you need to add a `DispatchWorkflow` activity to your workflow.
This activity has a `WorkflowDefinitionId` property that you can use to specify the definition ID of the workflow you
want to dispatch.

When the workflow is executed, the `DispatchWorkflow` activity will dispatch the workflow with the specified ID.
The dispatched workflow will run in the background and the parent workflow will continue executing, unless
the `WaitForCompletion` property is set to `true`.

## Dispatching a workflow with input

You can also pass input to the dispatched workflow.
To do this, you need to set the `Input` property to a JSON object that contains the input you want to pass to the
workflow.

When the workflow is executed, the `DispatchWorkflow` activity will dispatch the workflow with the specified input.

## Dispatching a workflow and receiving output

You can also receive output from the dispatched workflow.
To do this, you need to set the `WaitForCmpletion` property to `true` and set the `Output` property to a JSON object
that contains the output you want to receive from the workflow.

## Trying it out
When the workflow is executed, the `DispatchWorkflow` activity will dispatch the workflow and wait for it to complete.
When the workflow completes, the `DispatchWorkflow` activity will receive the output from the workflow and continue
executing.

Each of the following examples assume continuing from the previous workflows.

### Example 1: Dispatching a workflow

In this example, we'll create a workflow that dispatches another workflow with no input and no output.

#### Child Workflow

First, create a new workflow called **Child Workflow** using Elsa Studio with the following activities:

- `WriteLine`
    - `Text`: `Hello from Child`.

Make sure to publish the workflow.

#### Parent Workflow

Then, create a new workflow called **Parent Workflow** using Elsa Studio that contains the following activities:

- `WriteLine`
    - `Text`: `Parent started`.
- `DispatchWorkflow`
    - `WorkflowDefinitionId`: <select the **Child Workflow** workflow> from the dropdown.
    - `WaitForCompletion`: `true`.
- `WriteLine`
    - `Text`: `Parent completed`.

The child workflow should look like this:

Run the parent workflow from the designer and keep an eye out on the console output, which should read:

```
Parent started
Hello from Child
Parent completed
```

To try out the example, you can download the
workflows [here](/guides/dispatching-child-workflows/dispatching-child-workflows-demo1.zip) and import them.

### Example 2: Dispatching a workflow with input

In this example, we'll update the child workflow to receive input from the parent workflow.

#### Child Workflow

First, Add the following input property to the workflow:

- `Message` (string)

Then, update the WriteLine activity as follows:

- `WriteLine`
    - `Text` (JavaScript): `getMessage()`.

#### Parent Workflow

Then, update the `DispatchWorkflow` activity in **Parent Workflow** as follows:

- `DispatchWorkflow`
    - `WorkflowDefinitionId`: <select the **Child Workflow** workflow> from the dropdown.
    - `WaitForCompletion`: `true`.
    - `Input` (JavaScript): `return {"Message": "Hello from Parent"}`.

Make sure to publish the workflows (or at least the child workflow), then run the parent workflow from the designer and keep an eye out on the console output, which should read:

```
Parent started
Hello from Parent
Parent completed
```

To try out the example, you can download the workflows [here](/guides/dispatching-child-workflows/dispatching-child-workflows-demo2.zip) and import them.

### Example 3: Dispatching a workflow and receiving output

In this example, we'll update the child workflow to send output to the parent workflow.

#### Child Workflow

First, Add the following output property to the workflow:

- `Response` (string)

Then, add a `SetOutput` activity to the workflow and set its properties as follows:

- `SetOutput`
    - `Output`: <select the **Response** output> from the dropdown.
    - `Value`: `"Hello from Child"`.

Make sure to connect the `WriteLine` activity to the `SetOutput` activity and publish the workflow.

#### Parent Workflow

Then, update the `DispatchWorkflow` activity in **Parent Workflow** as follows:

- Add a new variable called `ChildOutput` of type `ObjectDictionary`.

Then update the `DispatchWorkflow` activity as follows:

- `DispatchWorkflow`
    - `WorkflowDefinitionId`: <select the **Child Workflow** workflow> from the dropdown.
    - `WaitForCompletion`: `true`.
    - `Output`: <select the **ChildOutput** variable> from the dropdown.

Add a new WriteLine activity in between the DispatchWorkflow and the WriteLine activities and set its properties as follows:

- `WriteLine`
    - `Text` (JavaScript): `getChildOutput().Response`.

Make sure to publish the workflows (or at least the child workflow), then run the parent workflow from the designer and keep an eye out on the console output, which should read:

```
Parent started
Hello from Parent
Hello from Child
Parent completed
```

To try out the example, you can download the workflows [here](/guides/dispatching-child-workflows/dispatching-child-workflows-demo3.zip) and import them.