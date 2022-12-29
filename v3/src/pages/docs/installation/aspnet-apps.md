---
title: ASP.NET apps
description: Installing Elsa in ASP.NET apps.
---

## Setup

In your ASP.NET project's root directory, run the following command:

```shell
dotnet add package Elsa
```

Next, open your Program.cs file and add the following code:

**Program.cs**

```clike
using Elsa.Extensions;

// Add Elsa services.
builder.Services.AddElsa();
```

With that in place, you can now resolve Elsa services to run workflows. For example, if your app has a controller, you could inject the `IWorkflowRunner` service and run some workflow.

## Trying it out

### Writing to the console

Assuming your ASP.NET project is configured to use API controllers, you can try out running a workflow from a new controller using the following code:

**RunWorkflowController.cs**

```clike
using System.Threading.Tasks;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Samples.MyBackendApi.Controllers;

[ApiController]
[Route("run-workflow")]
public class RunWorkflowController : ControllerBase
{
    private readonly IWorkflowRunner _workflowRunner;

    public RunWorkflowController(IWorkflowRunner workflowRunner)
    {
        _workflowRunner = workflowRunner;
    }

    [HttpPost]
    public async Task Post()
    {
        await _workflowRunner.RunAsync(new WriteLine("Hello ASP.NET world!"));
    }
}
```

Next, start the program and send the following HTTP request to it:

```http request
POST https://localhost:7242/run-workflow
```

When you look at the console, you should see the following message:

```shell
Hello ASP.NET world!
```

### Writing to the HTTP Response

To make this a little bit more interesting, let's update the controller so that instead of writing to the console, Elsa writes directly to the HTTP response.
To do this, we need to make a few small changes:

1. Add the `Elsa.Http` package.
2. Update `Program.cs` to install the Elsa HTTP feature.
3. Update `RunWorkflowController.cs` to use the `WriteHttpResponse` activity instead of the `WriteLine` activity.

Let's take a look at each step.

First, run the following command:

```shell
dotnet add package Elsa.Http
```

Update Program.cs by replacing the Elsa setup with the following:

```clike
builder.Services.AddElsa(elsa => elsa.UseHttp());
```

Finally, replace the controller implementation with the following code:

**RunWorkflowController.cs**

```clike
using System.Threading.Tasks;
using Elsa.Http;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Samples.MyBackendApi.Controllers;

[ApiController]
[Route("run-workflow")]
public class RunWorkflowController : ControllerBase
{
    private readonly IWorkflowRunner _workflowRunner;

    public RunWorkflowController(IWorkflowRunner workflowRunner)
    {
        _workflowRunner = workflowRunner;
    }

    [HttpGet]
    public async Task Post()
    {
        await _workflowRunner.RunAsync(new WriteHttpResponse
        {
            Content = new("Hello ASP.NET world!")
        });
    }
}
```

Restart your application, start a web browser and navigate to https://localhost:7242/run-workflow
This time around, you should see the following response:

![Response](/installation/response.png)

### Exposing workflows as endpoints

In addition to programmatically invoking workflows, you can also create workflows that themselves are routable via HTTP.
In other words, instead of creating a controller, you can create a workflow that itself acts like a controller in the sense that it can handle HTTP requests and provide an HTTP response.

To enable this, we need to add the `WorkflowsMiddleware` ASP.NET middleware component to the request pipeline. To do so, add the following line right before `app.Run();`:

```clike
app.UseWorkflows();
```

Now we can create workflows that expose themselves as endpoints so that we can trigger them directly over HTTP.
Let's look at an example.

First, create a new workflow class using the **workflow builder API** that starts with the `HttpEndpoint` activity (which acts as a workflow trigger) and ends with the `WriteHttpResponse` activity:

```clike
using Elsa.Http;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;

namespace Elsa.Samples.MyBackendApi.Workflows;

public class HelloWorldHttpWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/hello-world"),
                    CanStartWorkflow = true
                },
                new WriteHttpResponse
                {
                    Content = new("Hello world of HTTP workflows!")
                }
            }
        };
    }
}
```

{% callout title="Workflow Builder API" type="note" %}
Notice that this workflow definition is different from what we have seen so far. Up to this point, we instantiated an activity such as `WriteLine` directly and sent it to the workflow runner to run the activity.
When we want to add workflows to the system, however, we need to define and register them with the *workflow runtime* so that components like the `WorkflowsMiddleware` can find and execute them.

There are different ways to define workflows, and one of them is to use the *workflow builder API*.

To use the workflow builder API, create a class that implements `IWorkflow`, or the abstract base class `WorkflowBase`, which in turn implements `IWorkflow`. 
{% /callout %}

{% callout title="Workflow Triggers" type="warning" %}
In order for the workflow runtime to be able to trigger workflows automatically, you need to set the activity's `CanStartWorkflow` property to `true`.
This is easy to forget, so whenever you are wondering why a workflow isn't running even though you are sure you triggered it, the first thing to check is to see if this property is set correctly.
{% /callout %}

Finally, we need to register the workflow with the runtime. To do this, update `Program.cs` by replacing the call to `builder.Services.AddElsa` with the following:

```clike
builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowRuntime(runtime => runtime.AddWorkflow<HelloWorldHttpWorkflow>());
    elsa.UseHttp();
});
```

That will effectively register our workflow definition with the runtime.

To try it out, start the application, open a browser, and navigate to `https://localhost:7242/workflows/hello-world`.

The response should look like this:

![Response](/installation/response-2.png)