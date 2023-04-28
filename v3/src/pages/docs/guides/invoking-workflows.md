---
title: Invoking workflows
description: Ways to execute workflows
---

There are a few ways to execute workflow:
- Trigger the blocking activity
- Execute the workflow using the Api
- Dispatch the workflow using the Api

---

## Trigger the blocking activity
Blocking activities can be triggered in order to start the workflow. For example, a workflow starting with an **HTTP endpoint** activity can be started by calling the path configured in the activity.

Having a workflow that starts with an Http endpoint activity, with the following configuration:
![Http endpoint configuration](/guides/invoking-workflows/http-endpoint.png)

Also, please make sure that you have the **Can start workflow** set to true:
![Can start workflow](/guides/invoking-workflows/can-start-workflow.png)

This workflow can be executing by the following request:
```shell
curl --location --request POST 'https://localhost:5001/workflows/test' \
--header 'Content-Type: application/json'
```

## Execute the workflow using the Api
The Elsa Api provides an endpoint for executing a workflow synchronously. This endpoint is: `/elsa/api/workflow-definitions/<workflow-definition-id>/execute` endpoint:

```shell
curl --location --request POST 'https://localhost:5001/elsa/api/workflow-definitions/<workflow-definition-id>/execute' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer {access_token}'
--data-raw '{
    "input": {
        "InputData": {
            "dummyProp": "hello"
        }
    }
}'
```

Notice that this endpoint, requires the **workflow definition id**, the authorization header and we have the possibility to provide input variables. 

The authorization token can be obtained as described in [ASP.NET workflow server / security](../installation/aspnet-apps-workflow-server).

You can define inputs and outputs for each workflow from the **Input/Output** tab.
![Workflow input and output](/guides/invoking-workflows/inputs.png)

The result of the call should give you back the result below.
```
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

The id is the workflow instance id, you see data about the execution and the outputs of the workflow under 'output'.

{% callout title="Input / Output" %}
The above example talks about the inputs and outputs of a workflow.
For more information, see the [Input / Output](../core-concepts/input-output) chapter.
{% /callout %}

## Dispatch the workflow using the Api
The Elsa Api also provides an endpoint for executing a workflow asynchronously. This endpoint is: `/elsa/api/workflow-definitions/<workflow-definition-id>/dispatch`.

The request is pretty much the same as the **/execute** endpoint.