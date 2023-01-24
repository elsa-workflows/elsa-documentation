---
title: HTTP workflows
description: Working with HTTP activities
---

In this guide, we'll take a look at a workflow that can receive HTTP requests, send HTTP requests and write output to the HTTP response object.

We will rely on the `webapi` project template that comes with a demo controller called **WeatherForecastController**.

Our workflow will handle inbound HTTP requests, invoke the weather forecast API using an HTTP call, and write back the response to the HTTP response.

As a result, we will learn how to use the following HTTP activities:

- HttpEndpoint
- SendHttpRequest
- WriteHttpResponse

---

## Project setup

To create the project, run the following command:

```shell
dotnet new webapi -n WorkflowApp.Web --no-openapi -f net7.0
```

### Run project

Run the following commands to go into the created project directory and run the project:

```shell
cd WorkflowApp.Web
dotnet run
```

When the app is running, take note of the URL it is listening on. For example: `http://localhost:5085`.
To invoke the weather forecast controller, navigate to `http://localhost:5085/weatherforecast`, which should produce output similar to the following (simplified for clarity):

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
  }
]
```

## Elsa integration

Next, let's install and configure Elsa.

### Packages

Install the following packages:

```shell
dotnet add package Elsa
dotnet add package Elsa.EntityFrameworkCore.Sqlite
dotnet add package Elsa.Http
dotnet add package Elsa.Identity
dotnet add package Elsa.Liquid
dotnet add package Elsa.Workflows.Api
dotnet add package Elsa.Workflows.Designer
```

### Program

Update `Program.cs` with the following code:

```clike
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.Extensions;
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
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(m => m.UseSqlite()));
    elsa.UseJavaScript();
    elsa.UseLiquid();
    elsa.UseWorkflowsApi();
    elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");
    elsa.AddWorkflow<WeatherForecastWorkflow>();
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

{% callout title="Path prefix" type="note" %}
By default, HTTP workflow paths are prefixed with `/workflows`. This is an optimization to prevent *every* inbound HTTP request from passing through the accompanying middleware component responsible for invoking workflows.

You can change the prefix to something else. For example, the following code changes the prefix to `"/wf"`:

`elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");`

To remove the prefix entirely, provide an empty string instead:

`elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "");`

As mentioned before, this will cause any inbound HTTP request to be matched against a workflow, which can potentially decrease overall performance of the app, so use with care.  
{% /callout %}

### Designer

A complete description of configuring the designer in an ASP.NET project can be found [here](../installation/aspnet-apps-workflow-server-and-designer), but we'll repeat the steps here inline for completeness' sake.

We already installed the necessary packages and added the necessary services, but we also need to create a couple of Razor files:

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

With that out of the way, let's continue and create the workflow, first in code.

## Workflow from code

The workflow we'll be creating will be able to do the following:

- Handle inbound HTTP requests
- Send HTTP requests to the WeatherForecast API endpoint.
- Write the weather forecast results back to the HTTP response.

Let's see how to create the workflow in code.

Create a new folder called `Workflows` and add the following class:

```clike
using System.Net;
using System.Net.Mime;
using System.Text;
using Elsa.Http;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Services;

namespace WorkflowApp.Web.Workflows;

public class WeatherForecastWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var serverAddress = new Uri(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(';').First());
        var weatherForecastApiUrl = new Uri(serverAddress, "weatherforecast");
        var weatherForecastResponseVariable = builder.WithVariable<ICollection<WeatherForecast>>();

        builder.Root = new Sequence
        {
            Activities =
            {
                // Expose this workflow as an HTTP endpoint.
                new HttpEndpoint
                {
                    Path = new("/weatherforecast"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                // Invoke another API endpoint. Could be a remote server, but here we are invoking an API hosted in the same app.
                new SendHttpRequest
                {
                    Url = new(weatherForecastApiUrl),
                    Method = new(HttpMethods.Get),
                    ParsedContent = new(weatherForecastResponseVariable)
                },

                // Write back the weather forecast.
                new WriteHttpResponse
                {
                    ContentType = new(MediaTypeNames.Text.Html),
                    StatusCode = new(HttpStatusCode.OK),
                    Content = new(context =>
                    {
                        var weatherForecasts = weatherForecastResponseVariable.Get(context)!;
                        var sb = new StringBuilder();

                        sb.AppendLine(
                            """
<!doctype html>
    <html>
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <script src="https://cdn.tailwindcss.com"></script>
    </head>
    <body>
        <div class="px-4 sm:px-6 lg:px-8">
        <div class="mt-8 flex flex-col">
        <div class="-my-2 -mx-4 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div class="inline-block min-w-full py-2 align-middle md:px-6 lg:px-8">
            <div class="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
              <table class="min-w-full divide-y divide-gray-300">
                <thead class="bg-gray-50">
                  <tr>
                    <th scope="col" class="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">Date</th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Temperature (C/F)</th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Summary</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 bg-white">
""");
                        foreach (var weatherForecast in weatherForecasts)
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine($"""<td class="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">{weatherForecast.Date}</td>""");
                            sb.AppendLine($"""<td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{weatherForecast.TemperatureC}/{weatherForecast.TemperatureF}</td>""");
                            sb.AppendLine($"""<td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{weatherForecast.Summary}</td>""");
                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine(
"""
                </tbody>
            </table>
        </div>
        </div>
        </div>
        </div>
    </body>
    </html>
""");
                        return sb.ToString();
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

To register the workflow with the workflow runtime, go back to `Program.cs` and update the Elsa setup code by adding the following line:

```clike
elsa.AddWorkflow<WeatherForecastWorkflow>();
```

Restart the application, and this time navigate to `http://localhost:5085/wf/weatherforecast`.

The result should look similar to this:

![Weather forecast response](/guides/http-workflows/weatherforecast-response-html.png)

## Workflow from designer

Now that we have seen how to create the workflow from code, let's take a look at creating the same workflow using the designer.

The following takes you through the creation of the workflow step-by-step [using Scribe](https://scribehow.com/shared/Workflow__pTWktzWTQXWQ5FEfPWLvtw).

The liquid template that is used can be downloaded from [here](/guides/http-workflows/weatherforecast-template.liquid)

{% scribe-how id="Workflow__pTWktzWTQXWQ5FEfPWLvtw" %} 
{% /scribe-how %}

You can also watch the video to re-create the workflow using the designer:

{% youtube id="0w-gDOnG7Uo" %}
{% /youtube %}