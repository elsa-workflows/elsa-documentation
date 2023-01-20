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

**1. Click the "Username" field and enter "admin".**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/a62bd113-a430-4321-a16d-4936cd87b269/ascreenshot.jpeg?tl_px=918,61&amp;br_px=1664,481&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**2. Click the "Password" field and enter "password".**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/4b75004e-0041-4559-a696-e6a2cac36d84/ascreenshot.jpeg?tl_px=874,142&amp;br_px=1620,562&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**3. Click "Sign in"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/13190310-be20-4b45-900f-91d61938095d/ascreenshot.jpeg?tl_px=892,248&amp;br_px=1638,668&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**4. Click "New"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/99532dc7-13e8-47b8-b3bf-8b89db84e730/ascreenshot.jpeg?tl_px=1813,0&amp;br_px=2559,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=448,5)

**5. Click "Workflow Definition"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/c217e703-f37f-4ae5-acb7-3755b850d64e/ascreenshot.jpeg?tl_px=1813,0&amp;br_px=2559,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=403,50)

**6. Drag and drop a new "Http Endpoint" activity onto the canvas.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/2fcf9890-262c-4ba9-a1d5-65b06085ea74/ascreenshot.jpeg?tl_px=0,130&amp;br_px=746,550&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=98,139)

**7. Add the "Get Weather Forecast" activity to the canvas.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/76686c42-d2a3-46f3-9cc4-f5641e581bb7/ascreenshot.jpeg?tl_px=0,52&amp;br_px=746,472&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=85,139)

**8. Add the "HTTP Response" activity to the canvas.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/3762a518-91eb-4548-8655-fef72b01586f/ascreenshot.jpeg?tl_px=0,301&amp;br_px=746,721&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=84,139)

**9. Select the "Variables" tab.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/b6e10df1-2617-4041-803e-29c91e207f0e/ascreenshot.jpeg?tl_px=1766,0&amp;br_px=2512,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,126)

**10. Click "Add variable"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/7d5e0e6a-095b-4358-948e-68f88156f09e/ascreenshot.jpeg?tl_px=1813,46&amp;br_px=2559,466&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=481,139)

**11. Click the "Name" field.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/26421c7e-9837-49aa-8135-6f71f89c1920/ascreenshot.jpeg?tl_px=877,0&amp;br_px=1623,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,89)

**12. Type "WeatherForecasts"**

**13. Click "Save"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/df03605e-9ecb-48ed-81b2-f94bec6b2c37/ascreenshot.jpeg?tl_px=1018,392&amp;br_px=1764,812&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**14. Select the "Http Endpoint" activity.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/fb81dcd1-1bf0-4082-9647-550543c6b734/ascreenshot.jpeg?tl_px=416,152&amp;br_px=1162,572&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**15. Check the "Can start workflow" checkbox.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/a3494371-c740-415b-bd94-d4994d437906/ascreenshot.jpeg?tl_px=189,787&amp;br_px=935,1207&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**16. Select the "Settings" tab.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/a407eb31-1a75-42a2-bb81-9c97d40446d2/ascreenshot.jpeg?tl_px=223,497&amp;br_px=969,917&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**17. Click this text field.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/38bff31b-9429-4e8b-84e6-412fa8b07a6e/ascreenshot.jpeg?tl_px=213,581&amp;br_px=959,1001&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**18. Type "/weatherforecast-designer"**

**19. Check the "GET" method.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/e501d74d-85dd-4054-b22f-6823e814be9d/ascreenshot.jpeg?tl_px=153,717&amp;br_px=899,1137&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**20. Select the "Get Weather Forecast" activity.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/e09b2888-0350-4320-a807-6d20b8d15bae/ascreenshot.jpeg?tl_px=688,159&amp;br_px=1434,579&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**21. Select the "Output" tab.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/a62d4f43-5794-4506-8238-0a20c72c5656/ascreenshot.jpeg?tl_px=305,501&amp;br_px=1051,921&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**22. From the dropdown, select the WeatherForecasts variable created earlier.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/cac6beea-8b8a-4233-8c12-488b5817136b/ascreenshot.jpeg?tl_px=191,582&amp;br_px=937,1002&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**23. Select the HTTP Response activity.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/9042705e-3992-4602-89ca-bdcc8144e57d/ascreenshot.jpeg?tl_px=1007,168&amp;br_px=1753,588&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**24. Switch the syntax for the Content input field to "JavaScript".**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/dd519bf4-4900-446c-a757-ffcb98481abc/ascreenshot.jpeg?tl_px=1446,760&amp;br_px=2192,1180&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**25. Enter the following two lines:

const forecasts = getWeatherforecasts();
return toJson(forecasts);**

**26. Change the content type to "application/json"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/c47cdb31-c3b8-4dad-a7fd-3873deda1af1/user_cropped_screenshot.jpeg?tl_px=164,909&amp;br_px=910,1329&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,159)

**27. Connect each activity.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/56cd6624-67af-431d-b2e9-4967457f1595/ascreenshot.jpeg?tl_px=786,157&amp;br_px=1532,577&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**28. Select the "Properties" tab.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/33d9fd85-9780-4dd3-9ac8-69a040eb43d2/ascreenshot.jpeg?tl_px=1675,0&amp;br_px=2421,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,126)

**29. Click the "Name" field.**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/5839ea4c-4659-423b-8c2a-33967263f420/ascreenshot.jpeg?tl_px=1726,57&amp;br_px=2472,477&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=262,139)

**30. Type "Weather Forecast"**

**31. Click "Publish"**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/858a51a2-5b2e-493e-8352-d77a30702bab/ascreenshot.jpeg?tl_px=1813,0&amp;br_px=2559,420&amp;sharp=0.8&amp;width=560&amp;wat_scale=50&amp;wat=1&amp;wat_opacity=0.7&amp;wat_gravity=northwest&amp;wat_url=https://colony-labs-public.s3.us-east-2.amazonaws.com/images/watermarks/watermark_default.png&amp;wat_pad=424,11)

**32. Try out the workflow by navigating to http://localhost:5085/wf/weatherforecast-designer**

![](https://ajeuwbhvhr.cloudimg.io/colony-recorder.s3.amazonaws.com/files/2023-01-20/7e16716c-26da-443f-b897-62c9cce9c7ea/screenshot.png?tl_px=84,2&amp;br_px=830,422&amp;sharp=0.8&amp;width=560)
#### [Made with Scribe](https://scribehow.com/shared/Workflow__Mj1B4ddoSh27jKmeD3fq8Q)



