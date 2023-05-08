---
title: Invoking workflows
description: Ways to execute workflows
---

There are multiple ways to execute workflow:

- Use a trigger.
- Use the REST API.
- Use C# code.

---

## Use a trigger

When a workflow is configured to start with a [trigger](../core-concepts/trigger), that workflow will start whenever the trigger is activated.
How the trigger is activated depends on the trigger type.

For example, a workflow that starts with an **HTTP endpoint** trigger can be started by calling the path configured in the trigger.

![Http endpoint configuration](/guides/invoking-workflows/http-endpoint.png)

{% callout title="The trigger ain't triggering" type="warning" %}

Triggers become active only when their _Can start workflow_ is enabled.

![Can start workflow](/guides/invoking-workflows/can-start-workflow.png)

Without this, the trigger will not start the workflow (but it will still be used as a blocking activity).
{% /callout %}

This workflow can be triggered with the following request:
```shell
curl --location --request POST 'https://localhost:5001/workflows/test' \
--header 'Content-Type: application/json'
```

Other examples of triggers include:

- **Timer** triggers, which start a workflow at a specific time.
- **Event** triggers, which start a workflow when an event is received.
- **Message** triggers, which start a workflow when a message is received.

## Use the REST API
The `Elsa.Workflows.Api` package provides two endpoints for executing a workflow:

- `/workflow-definitions/{workflow_definition_id}/execute`: Executes a workflow definition **synchronously**.
- `/workflow-instances/{workflow_instance_id}/dispatch`: Executes a workflow instance **asynchronously**.

### Synchronous execution

Synchronously in this context doesn't mean that the workflow will be executed in the same thread as the caller. It still uses task-based asynchronous programming.
The difference is that the caller will wait for the workflow to finish before getting a response.

To execute a workflow definition synchronously, you can use the following request:

```shell
curl --location --request POST 'https://localhost:5001/elsa/api/workflow-definitions/<workflow-definition-id>/execute' \
--header 'Content-Type: application/json' \
--header 'Authorization: ApiKey {api_key}'
--data-raw '{
    "input": {
        "InputData": {
            "dummyProp": "hello"
        }
    }
}'
```

{% callout title="Authorization" type="note" %}

The authorization token can be obtained as described in [ASP.NET workflow server / security](../installation/aspnet-apps-workflow-server).

{% /callout %}

You can define inputs and outputs for each workflow from the **Input/Output** tab.
![Workflow input and output](/guides/invoking-workflows/inputs.png)

The response should look something like this:

```json
{
    "workflowState": {
        "definitionId": "2f6ba5802e254082b00bd4dab00e650a",
        "definitionVersion": 1,
        "status": "Finished",
        "subStatus": "Finished",
        "bookmarks": [],
        "completionCallbacks": [],
        "activityExecutionContexts": [],
        "output": {
            "OutputData": "hello there!"
        },
        "properties": {
            "PersistentVariablesDictionary": {
                "f765d2c1ef7843ed862488ae6f191884:Workflow1:subvar": ""
            }
        },
        "id": "f765d2c1ef7843ed862488ae6f191884"
    }
}
```

The `id` field contains the _workflow instance ID_.
The `output` fields contains the outputs of the workflow under.


For more information about input and output, see the [Input / Output](../core-concepts/input-output) chapter.

### Asynchronous execution

The following endpoint executes a workflow _asynchronously_: 

```
/workflow-definitions/{workflow_definition_id}/dispatch`
```

The request is pretty much the same as the `/execute` endpoint, except that it doesn't wait for the workflow to finish.

Instead, the endpoint will **enqueue** the workflow for execution and return the workflow instance ID.

Here is an example of a response:

```json
{
    "workflowInstanceId": "b235209459294b97904eaee89fe7f930"
}
```

{% callout title="Workflow instance ID" type="warning" %}

The workflow instance ID is generated _before_ the workflow instance is actually created. This means that the workflow instance ID can be used to track the workflow instance, even if it doesn't exist yet.

{% /callout %}

## Use C# code

There are two ways to execute a workflow from C# code:

- Using the `IWorkflowRunner` service.
- Using the `IWorkflowRuntime` service.

### Using the `IWorkflowRunner` service

The `IWorkflowRunner` is a low-level service that can execute any `IActivity` object.

For example, the following code executes a workflow called `HelloWorld`:

```csharp
var workflow = new Workflow
{
   Root = new WriteLine("Hello world!")
};

var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

await workflowRunner.RunWorkflowAsync(workflow);
```

{% callout title="Workflow instance ID" type="note" %}
The _Workflow_ class itself is an activity, too!
{% /callout %}

### Using the `IWorkflowRuntime` service

Using `IWorkflowRunner` is easy, but it requires you to know what workflow to execute and process any bookmarks it creates, yourself.
Furthermore, you will have to manage persisting the workflow instance yourself.

The `IWorkflowRuntime` service, on the other hand, is a high-level service that lets you _trigger_ one or more workflows, depending on what input you provide.
The workflow runtime works together with the `IWorkflowDefinitionService` to find workflows from the `IWorkflowDefinitionStore`.

This means that before you can trigger workflows using the `IWorkflowRuntime` service, you will have to register your workflows.

When using the workflow designer (or the REST API directly) to create workflows, the workflows will be registered automatically.
And if you have a custom `IWorkflowDefinitionProvider`, its workflows will be registered automatically, too.
But, if you need to, you can also register workflows manually. For example:

```csharp
public class HelloWorldWorkflow : WorkflowBase
{
   protected override void Build(IWorkflowBuilder builder)
   {
        builder.Root = new WriteLine("Hello world!");
   }
}

services.AddElsa(elsa => elsa.AddWorkflow<MyWorkflow>());
```

The following examples demonstrates invoking an existing workflow using the `IWorkflowRuntime` service:

```csharp

var workflowRuntime = serviceProvider.GetRequiredService<IWorkflowRuntime>();
var result = await workflowRuntime.StartWorkflowAsync("HelloWorldWorkflow");
```

{% callout title="Workflow instance ID" type="note" %}
The _IWorkflowRuntime_ service is often used when you want to trigger a workflow from an event handler. For example, if you implement a custom trigger activity that monitors a directory for file changes, you can use the _IWorkflowRuntime_ service to trigger a workflow when a file is created.
Another example is when you want to trigger a workflow from a message queue. In that case, you can use the _IWorkflowRuntime_ service to trigger a workflow when a message is received.
{% /callout %}