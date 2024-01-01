---
title: Elsa Web
description: Installing Elsa in ASP.NET apps.
---

In this guide, we'll demonstrate how to integrate Elsa Workflows into an ASP.NET Core application. 
This setup is ideal for executing long-running workflows in the background, supporting various activities like Timer, Cron, Event, and others that necessitate a background service.

## Initial Setup

Create a new empty ASP.NET app using the following command:

```shell
dotnet new web -n "ElsaWeb" -f net8.0
```

Navigate to your project's root directory and install the Elsa package:

```shell
cd ElsaWeb
dotnet add package Elsa
```

Open the `Program.cs` file and replace its contents with:

**Program.cs:**

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

This configuration allows your application to use Elsa services for running workflows.

## Trying it out

### Example 1: Console Output

Add a new controller named `RunWorkflowController`:

**Controllers/RunWorkflowController.cs:**

```clike
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaWeb.Controllers;

[ApiController]
[Route("run-workflow")]
public class RunWorkflowController(IWorkflowRunner workflowRunner) : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        await workflowRunner.RunAsync(new WriteLine("Hello ASP.NET world!"));
    }
}
```

Start the application using the following command:

```bash
dotnet run --urls=https://localhost:5001
```

Then visit https://localhost:5001/run-workflow in your browser.
The console should display "Hello ASP.NET world!"

### Example 2: HTTP Response

Install the `Elsa.Http` Package

```shell
dotnet add package Elsa.Http
```

Modify the Elsa setup in `Program.cs`:

```clike
builder.Services.AddElsa(elsa => elsa.UseHttp());
```

Adjust the controller to use the `WriteHttpResponse` activity:

**Controllers/RunWorkflowController.cs:**

```clike
using Elsa.Http;
using Elsa.Workflows.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaWeb.Controllers;

[ApiController]
[Route("run-workflow")]
public class RunWorkflowController(IWorkflowRunner workflowRunner) : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        await workflowRunner.RunAsync(new WriteHttpResponse
        {
            Content = new("Hello ASP.NET world!")
        });
    }
}
```

Restart your app and navigate to https://localhost:5001/run-workflow. The browser should now display the message.

![Response](/installation/response.png)

### Example 3: Workflows as HTTP Endpoints

In addition to programmatically invoking workflows, you can also create workflows that themselves are routable via HTTP.
To enable this, we need to add the `WorkflowsMiddleware` ASP.NET middleware component to the request pipeline.

In `Program.cs`, add the following line before app.Run();:

```clike
app.UseWorkflows();
```

Create a new workflow class with HTTP capabilities:

**Workflows/HttpHelloWorld.cs:**

```clike
using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;

namespace ElsaWeb.Workflows;

public class HttpHelloWorld : WorkflowBase
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

{% callout title="Workflow Triggers" type="warning" %}
In order for the workflow runtime to be able to trigger workflows automatically, you need to set the activity's `CanStartWorkflow` property to `true`.
This is easy to forget, so whenever you are wondering why a workflow isn't running even though you are sure you triggered it, the first thing to check is to see if this property is set correctly.
{% /callout %}

Update `Program.cs` to register this workflow:

```clike
using ElsaWeb.Workflows; // Add this line.

builder.Services.AddElsa(elsa =>
{
    elsa.AddWorkflow<HttpHelloWorld>();
    elsa.UseHttp();
});
```

Restart the app and visit https://localhost:5001/workflows/hello-world. You should see the corresponding message.

![Response](/installation/response-2.png)

{% callout title="HTTP Endpoint Prefix Path" type="note" %}
Notice that the URL path is `/workflows/hello-world` and not `/hello-world` as we defined in the workflow definition.
This is because the `WorkflowsMiddleware` component is configured to use `/workflows` as the prefix path by default.
You can change this by passing a different prefix path to the `UseHttp` method. For example, if you want to use `/api`, you can do so by calling `elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/api");`.

{% /callout %}

## Summary

We've covered how to set up Elsa Workflows in an ASP.NET Core application, demonstrating various methods of workflow execution and integration. This approach allows for versatile and efficient handling of background processes and activities within your ASP.NET applications.

The complete code and additional resources are available [here](https://github.com/elsa-workflows/elsa-guides/tree/main/src/installation/elsa-web/ElsaWeb).