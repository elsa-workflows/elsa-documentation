---
title: HTTP workflows
description: Working with HTTP activities
---

In this guide, we'll take a look at a workflow that can receive HTTP requests, send HTTP requests and write output to the HTTP response object.

---

## Handling inbound HTTP requests

When creating ASP.NET web applications, we typically create one or more API endpoints. These endpoints may receive a payload via the HTTP request body, perform operations like data access, and finally return a response.
To implement an API endpoint, you have a number of options at your disposal. For example:

- [API controllers](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/)
- [FastEndpoints](https://fast-endpoints.com/)

With Elsa, you have yet another option to implement APIs by creating workflows that start with the **HTTP Endpoint** activity.  

### HTTP Endpoint

The HTTP endpoint activity effectively makes your workflow invokable via HTTP requests. It lets you specify a request path to handle, one or more HTTP verbs, and optionally lets you specify which .NET type to parse inbound JSON payloads into.

To see how it works, we are going to create a new ASP.NET project using the `webapi` template. This template provides a `WeatherForecastController` that returns a random weather forecast to the caller. We will replace this controller with a workflow.
We will first create this workflow in code, and then using the visual designer.

### Create the project

To create the project, run the following command:

```shell
dotnet new webapi -n WorkflowApp.Web --no-openapi -f net7.0
```

Run the following commands to go into the created project directory and run the project:

```shell
cd WorkflowApp.Web
dotnet run
```

When the app is running, take note of the URL it is listening on. For example: `http://localhost:5085`.
To invoke the weather forecast controller, navigate to `http://localhost:5085/weatherforecast`, which should produce output similar to the following:

```json
[
  {
    "date": "2023-01-20",
    "temperatureC": 54,
    "temperatureF": 129,
    "summary": "Hot"
  },
  {
    "date": "2023-01-21",
    "temperatureC": 48,
    "temperatureF": 118,
    "summary": "Balmy"
  },
  {
    "date": "2023-01-22",
    "temperatureC": 49,
    "temperatureF": 120,
    "summary": "Warm"
  },
  {
    "date": "2023-01-23",
    "temperatureC": -15,
    "temperatureF": 6,
    "summary": "Cool"
  },
  {
    "date": "2023-01-24",
    "temperatureC": 51,
    "temperatureF": 123,
    "summary": "Balmy"
  }
]
```

Let's take a look at the contents of `WeatherForecastController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace WorkflowApp.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```

All it does is the following:

- For the next 5 days, generate a new weather forecast for each day.
- Use a shared randomizer instance to produce a random temperature
- Use a shared randomizer instance to produce a randomly selected summary
- Return the weather forecasts, which will be written to the HTTP response as a JSON response.

### Setup Elsa

Let's see what it takes to replace the controller entirely with a workflow.

First, install the necessary packages:

```shell
dotnet add package Elsa
dotnet add package Elsa.Http
```

Update `Program.cs` with the following code:

```clike
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Elsa services.
builder.Services.AddElsa(elsa =>
{
    elsa.UseHttp();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map HTTP workflows.
app.UseWorkflows();

app.Run();

```

### Workflow in code

Before we create the workflow using the designer, we will first see how to create it in code.
Create a new folder called `Workflows` and add the following class:

```clike
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Elsa.Http;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;

namespace WorkflowApp.Web.Workflows;

public class WeatherForecastWorkflow : WorkflowBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/WeatherForecast"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },
                new WriteHttpResponse
                {
                    ContentType = new(MediaTypeNames.Application.Json),
                    StatusCode = new(HttpStatusCode.OK),
                    Content = new(() =>
                    {
                        var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                            {
                                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                TemperatureC = Random.Shared.Next(-20, 55),
                                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                            })
                            .ToArray();

                        return JsonSerializer.Serialize(weatherForecasts);
                    })
                }
            }
        };
    }
}
```

{% callout title="CanStartWorkflow" type="warning" %}
Notice that we set the `CanStartWorkflow` property of the `HttpEndpoint` activity to `true`.
This is a signal to the workflow runtime to extract a trigger from the activity.

Without this property set, the workflow will not be triggered automatically in response to inbound HTTP requests.
{% /callout %}

To register the workflow, go back to `Program.cs` and update Elsa like this:

```clike
// Add Elsa services.
builder.Services.AddElsa(elsa =>
{
    elsa.UseHttp();
    elsa.AddWorkflow<WeatherForecastWorkflow>(); // <-- Add this line to register the workflow.
});
```

Restart the application, and this time navigate to `http://localhost:5085/workflows/weatherforecast`.

The result should be similar to that of `http://localhost:5085/weatherforecast`.

{% callout title="Path prefix" type="note" %}
By default, HTTP workflows are prefixed with `/workflows`. This is an optimization to prevent **every** inbound HTTP request from passing through the accompanying middleware component responsible for invoking workflows.

You can change the prefix to something else. For example, the following code changes the prefix to `"/wf"`:

`elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");`

To remove the prefix entirely, provide an empty string instead:

`elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "");`

As mentioned before, this will cause any inbound HTTP request to be matched against a workflow, which can potentially decrease overall performance of the app, so use with care.  
{% /callout %}

There you have it - we exposed another endpoint using a workflow.

{% callout title="OpenAPI support" type="note" %}
HTTP workflows in Elsa 3 currently don't integrate with Swashbuckle, but there's [an issue for that](https://github.com/elsa-workflows/elsa-core/issues/3644).    
{% /callout %}

### Workflow from designer

To implement the same workflow from the designer, we need to do a bit more prepwork:

1. Setup the designer (or setup a [separate project that hosts the designer](../installation/aspnet-apps-workflow-designer))
2. Create and register a custom activity called `GetWeatherForecast`.

The reason we need to create a custom activity is because the designer does not support running arbitrary C# logic, like we did in the `WeatherForecastWorkflow` class.

#### GetWeatherForecast activity

Create a new folder called `Activities` and add the following class to it:

```clike
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;

namespace WorkflowApp.Web.Activities;

[Activity("Demo", "Returns a list of weather forecasts for the next few days.")]
public class GetWeatherForecast : CodeActivity<ICollection<WeatherForecast>>
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    protected override void Execute(ActivityExecutionContext context)
    {
        var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        context.Set(Result, weatherForecasts);
    }
}
```

Notice that the activity returns a collection of weather forecast objects, and is completely decoupled from any HTTP related context.
This allows us to reuse the activity in other places outside of any HTTP context if we wanted to.

To register the activity with the runtime, update `Program.cs` as follows:

```clike
// Add Elsa services.
builder.Services.AddElsa(elsa =>
{
    elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");
    elsa.AddWorkflow<WeatherForecastWorkflow>();
    elsa.AddActivity<GetWeatherForecast>(); // <-- Add this line to register the custom activity.
});
```

Before updating the application to host the designer, let's quickly update the coded workflow first to use this new activity.

Update `WeatherForecastWorkflow.cs` file with the following:

```clike
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Elsa.Http;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;
using WorkflowApp.Web.Activities;

namespace WorkflowApp.Web.Workflows;

public class WeatherForecastWorkflow : WorkflowBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    protected override void Build(IWorkflowBuilder builder)
    {
        // Declare a workflow variable to capture the result of the new GetWeatherForecast activity.
        var weatherForecasts = builder.WithVariable<ICollection<WeatherForecast>>();
        
        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/WeatherForecast"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },
                
                // Our new activity
                new GetWeatherForecast
                {
                    // Set Result property to our variable in order to capture the output.
                    Result = new (weatherForecasts)
                },
                new WriteHttpResponse
                {
                    ContentType = new(MediaTypeNames.Application.Json),
                    StatusCode = new(HttpStatusCode.OK),
                    
                    // Update the Content property to access the weatherForecasts variable and convert it to a JSON string. 
                    Content = new(context => JsonSerializer.Serialize(weatherForecasts.Get(context)))
                }
            }
        };
    }
}
```

#### Installing the designer

A complete description of configuring the designer in an ASP.NET project can be found [here](../installation/aspnet-apps-workflow-server-and-designer), but here are the basic steps:

1. Add the `Elsa.Workflows.Api` and `Elsa.Workflows.Designer` packages.
2. Update `Program.cs` with the necessary services and middleware components for Razor pages, Elsa identity and Elsa REST API.
3. Add a Razor page to render the designer.

In other words:

```shell
dotnet add package Elsa.Identity
dotnet add package Elsa.Workflows.Api
dotnet add package Elsa.Workflows.Designer
```

**Program.cs**

```clike
using Elsa.Extensions;
using WorkflowApp.Web.Activities;
using WorkflowApp.Web.Workflows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Add Elsa services.
builder.Services.AddElsa(elsa =>
{
    elsa.UseIdentity(identity =>
    {
        identity.IdentityOptions.CreateDefaultAdmin = true;
        identity.TokenOptions.SigningKey = "my-secret-signing-key";
    });
    elsa.UseDefaultAuthentication();
    elsa.UseJavaScript();
    elsa.UseWorkflowsApi();
    elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");
    elsa.AddWorkflow<WeatherForecastWorkflow>();
    elsa.AddActivity<GetWeatherForecast>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapControllers();
app.MapRazorPages();
app.Run();
```

**Pages/_ViewImports.cshtml**

```clike
@namespace WorkflowApp.Web.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

**Pages/Index.cshtml**

```clike
@page
@using Elsa.Workflows.Designer
@using Microsoft.AspNetCore.Mvc.TagHelpers
@{
    var serverUrl = Url.Content("elsa/api");
}

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Elsa Workflows 3.0</title>
    <link rel="stylesheet" href="https://rsms.me/inter/inter.css">
    <link rel="stylesheet" href="_content/Elsa.Workflows.Designer/elsa-workflows-designer/elsa-workflows-designer.css">
    <script src="_content/Elsa.Workflows.Designer/monaco-editor/min/vs/loader.js"></script>
    <script type="module" src="_content/Elsa.Workflows.Designer/elsa-workflows-designer/elsa-workflows-designer.esm.js"></script>
</head>
<body>

<component type="typeof(ElsaStudio)" render-mode="ServerPrerendered" param-ServerUrl="@serverUrl"/>

</body>
</html>
```

Restart the application and navigate to `http://localhost:5085/`, which should present you with a login screen:

![Login screen](/guides/http-workflows/login.png)

The following takes you through the creation of the workflow step-by-step [using Scribe](https://scribehow.com/shared/Workflow__Mj1B4ddoSh27jKmeD3fq8Q)

{% scribe-how id="Workflow__Mj1B4ddoSh27jKmeD3fq8Q" %} 
{% /scribe-how %}

You can also watch the video to re-create the workflow using the designer:

{% youtube id="N-H6UcQ-ZfM" %}
{% /youtube %}
