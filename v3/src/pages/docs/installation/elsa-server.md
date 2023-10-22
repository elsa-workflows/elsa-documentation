---
title: Elsa Server
description: Installing Elsa in ASP.NET apps.
---

In this chapter, we'll learn how to setup an Elsa Server.
An Elsa Server is essentially an ASP.NET Core application that hosts the workflow runtime and optionally provides a REST API for managing workflows.
Workflows can be stored to and retrieved from persistence stores such as databases, file systems, and cloud storage, but they can also be hardcoded into the application, as we will see in this chapter. 

## Setup

Create a new empty ASP.NET app using the following command:

```shell
dotnet new web -n "ElsaServer" -f net7.0
```

CD into the project's root directory and add the Elsa package:

```shell
cd ElsaServer
dotnet add package Elsa --prerelease
```

Next, open `Program.cs` file and replace its contents with the following code:

**Program.cs**

```clike
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddElsa();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

With that in place, you can now resolve Elsa services to run workflows. For example, if your app has a controller, you could inject the `IWorkflowRunner` service and run some workflow.

## Trying it out

### Writing to the console

Add a new controller called `RunWorkflowController` with the following code:

**RunWorkflowController.cs**

```clike
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaServer.Controllers;

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
        await _workflowRunner.RunAsync(new WriteLine("Hello ASP.NET world!"));
    }
}
```

Then start the program and navigate to https://localhost:5001/run-workflow using your web browser.
When you look at the application console output, you should see the following message:

```shell
Hello ASP.NET world!
```

### Writing to the HTTP Response

To make this a little bit more interesting, let's update the controller so that instead of writing to the console, the workflow writes directly to the HTTP response.
To do this, we need to make a few small changes:

1. Add the `Elsa.Http` package.
2. Update `Program.cs` to install the Elsa HTTP feature.
3. Update `RunWorkflowController.cs` to use the `WriteHttpResponse` activity instead of the `WriteLine` activity.

Let's take a look at each step.

First, run the following command:

```shell
dotnet add package Elsa.Http --prerelease
```

Update `Program.cs` by replacing the Elsa setup code with the following:

```clike
builder.Services.AddElsa(elsa => elsa.UseHttp());
```

Finally, replace the controller implementation with the following code:

**RunWorkflowController.cs**

```clike
using Elsa.Http;
using Elsa.Workflows.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaServer.Controllers;

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

Notice that we replaced the `WriteLine` activity with the `WriteHttpResponse` activity which comes from the `Elsa.Http` package.

Restart your application and navigate to https://localhost:5001/run-workflow
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
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;

namespace ElsaServer.Workflows;

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

That will effectively register our workflow definition with the workflow runtime.

To try it out, restart the application and navigate to `https://localhost:5001/workflows/hello-world`.

The response should look like this:

![Response](/installation/response-2.png)

## Exposing the REST API

In the previous section, we saw how to create a workflow that exposes itself as an HTTP endpoint.
In this section, we'll look at how to expose the REST API so that we can manage workflows using the designer, which consumes this API.

To enable the REST API, we need to perform the following steps:

1. Add the `Elsa.Workflows.Api` package.
2. Add the `Elsa.Identity` package.
3. Update `Program.cs` to install the API and identity feature.
4. Add a middleware that serves the Elsa API.

Let's take a look at each step.

First, run the following commands:

```shell
dotnet add package Elsa.Workflows.Api --prerelease
dotnet add package Elsa.Identity --prerelease
```

Next, update `Program.cs` by replacing the Elsa setup code with the following:

```clike
builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowRuntime(runtime => runtime.AddWorkflow<HelloWorldHttpWorkflow>());
    elsa.UseHttp();
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options => options.SigningKey = "secret signing key for tokens";
        identity.UseAdminUserProvider();
    });
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
    elsa.UseWorkflowsApi();
});
```

Notice that we added the `UseIdentity` and `UseDefaultAuthentication` methods to the Elsa setup.
This will enable the Elsa identity feature and configure it to use the admin user provider and the admin API key authentication provider.

Next, we need to add a middleware that serves the Elsa API.
To do this, add the following line right after `app.MapControlers();`:

```clike
app.UseWorkflowsApi();
```

With that in place, we can now connect [Elsa Studio](./elsa-studio-blazorwasm) to the Elsa Server and start creating and executing workflows.

## Summary

In this chapter, we learned how to setup an Elsa Server.
We saw how to run workflows programmatically, how to expose workflows as HTTP endpoints, and how to expose the REST API so that we can manage workflows using the designer.

The final result of this chapter can be found [here](