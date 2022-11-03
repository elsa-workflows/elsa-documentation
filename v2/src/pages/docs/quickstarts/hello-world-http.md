---
title: Hello World - ASP.NET Core
---

In this quickstart, we will take a look at a minimum ASP.NET Core application that executes a workflow.
The workflow will listen for an incoming HTTP request and write back a simple response.

We will:

* Create an ASP.NET Core application.
* Programmatically define a workflow definition using Elsa's Workflow Builder API that executes when an HTTP request comes in at a specified URL.

## The Project

Create a new, empty ASP.NET Core project called `ElsaQuickstarts.WebApp.HelloWorld`:

```bash
dotnet new web -n "ElsaQuickstarts.WebApp.HelloWorld"
```

CD into the created project folder:

```bash
cd ElsaQuickstarts.WebApp.HelloWorld
```

Add the following packages:

```bash
dotnet add package Elsa
dotnet add package Elsa.Activities.Http
```

## The Workflow

Create a new file called `HelloWorld.cs` and add the following code:

```clike
using System.Net;
using Elsa.Activities.Http;
using Elsa.Builders;

namespace ElsaQuickstarts.WebApp.HelloWorld
{
    /// <summary>
    /// A workflow that is triggered when HTTP requests are made to /hello-world and writes a response.
    /// </summary>
    public class HelloWorld : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .HttpEndpoint("/hello-world")
                .WriteHttpResponse(HttpStatusCode.OK, "<h1>Hello World!</h1>", "text/html");
        }
    }
}
```

The above workflow has two activities.
The first activity `HttpEndpoint` represents an HTTP endpoint, which can be invoked using an HTTP client, including a web browser.
The first activity is connected to the second activity `WriteHttpResponse`, which writes a response to the HTTP client.

> When activities are chained as in seen in this example, the second activity is connected to the "Done" outcome of the first activity.
> You can also connect to the "Done" outcome explicitly, like this: 
> ```c#
> builder
>   .HttpEndpoint("/hello-world")
>   .When(OutcomeNames.Done)
>   .WriteHttpResponse(HttpStatusCode.OK, "<h1>Hello World!</h1>", "text/html");
> ``` 
> Explicitly connecting to an activity's outcome is necessary anytime you want to connect to an outcome other than `"Done"`

## Startup

Next, open `Startup.cs` and replace its contents with the following:

```clike
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ElsaQuickstarts.WebApp.HelloWorld
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddElsa(options => options
                    .AddHttpActivities()
                    .AddWorkflow<HelloWorld>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpActivities();
        }
    }
}
``` 

## Run

Run the program and wait until you see the following output:

```text
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

Open a web browser and navigate to `https://localhost:5001/hello-world`.

The result should look like this:

{% figure src="/assets/quickstarts/aspnetcore-hello-world-figure-1.png" /%}

## Next Steps

In this guide, we've seen how to setup a workflow that is triggered when an HTTP request comes in.
Implementing HTTP workflows is a great way to easily implement logic in response to HTTP requests quickly.

Now that you've seen how to setup an ASP.NET Core server with Elsa workflows support, you might want to learn more about the following:

* [How to setup an ASP.NET Core host with Elsa API Endpoints.](quickstarts-aspnetcore-server-api-endpoints.md)
* [How to setup an ASP.NET Core host with Elsa Dashboard.](quickstarts-aspnetcore-server-dashboard.md)
* [How to setup an ASP.NET Core host with Elsa Dashboard + API Endpoints.](quickstarts-aspnetcore-server-dashboard-and-api-endpoints.md)