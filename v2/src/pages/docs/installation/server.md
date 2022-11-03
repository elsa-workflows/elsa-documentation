---
title: Server
---

Although the Elsa Core library provides you with a broad range of APIs to execute workflows manually, ultimately you might want the ability to execute your workflows based on events such as timer events, incoming HTTP requests, incoming service bus messages, and anything else that you can conceive of.
For that to work, you will need various background services and middleware installed in your (ASP).NET Core host application.
 
And once you have an Elsa Server setup, you can connect the workflow designer to it directly and start creating and managing workflows visually.

The following steps assume you created a new, blank ASP.NET Core project.

## Add Package



## Startup

The following `Startup` shows the minimum set of calls to make in order to register the most common services required when using **HTTP** and **Timer** activities:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        // Required services for Elsa to work. Registers things like `IWorkflowInvoker`.
        .AddElsa()

        // Registers necessary service to handle HTTP requests.
        .AddHttpActivities()

        // Registers a hosted service that periodically invokes workflows containing time-based activities. 
        .AddTimerActivities();
}
```

Additionally, the following call is required to register the necessary middleware when using with **HTTP** activities:

```csharp
public void Configure(IApplicationBuilder app)
{
    // Register necessary ASP.NET Core middleware that triggers workflows containing HTTP activities. 
    app.UseHttpActivities();
}
```