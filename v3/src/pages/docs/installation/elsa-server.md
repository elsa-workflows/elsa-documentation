---
title: Elsa Server
description: Setting up an Elsa Server.
---

In this chapter, we'll learn how to setup an Elsa Server.
An Elsa Server is an ASP.NET Core application that hosts the workflow runtime and provides a REST API for managing workflows. This REST API is consumed by the [Elsa Studio](./elsa-studio-blazorwasm.md).
Workflows can be stored to and retrieved from persistence stores such as databases, file systems, and cloud storage, but they can also be hardcoded into the application, as we will see in this chapter. 

## Setup

Create a new empty ASP.NET app using the following commands:

```shell
dotnet new web -n "ElsaServer" -f net8.0
cd ElsaServer
dotnet add package Elsa --prerelease
dotnet add package Elsa.EntityFrameworkCore --prerelease
dotnet add package Elsa.EntityFrameworkCore.Sqlite --prerelease
dotnet add package Elsa.Identity --prerelease
dotnet add package Elsa.Scheduling --prerelease
dotnet add package Elsa.Workflows.Api --prerelease
dotnet add package Elsa.CSharp --prerelease
```

Next, open Program.cs file and replace its contents with the following code:

```csharp
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddElsa(elsa =>
{
    // Configure Management layer to use EF Core.
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore());

    // Configure Runtime layer to use EF Core.
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore());
    
    // Default Identity features for authentication/authorization.
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-signing-key"; //This key needs to be at least 256 bits long.
        identity.UseAdminUserProvider();
    });
    
    // Configure ASP.NET authentication/authorization.
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
    
    // Expose Elsa API endpoints.
    elsa.UseWorkflowsApi();
    
    // Setup a SignalR hub for real-time updates from the server.
    elsa.UseRealTimeWorkflows();
    
    // Enable C# workflow expressions
    elsa.UseCSharp();
    
    // Enable HTTP activities.
    elsa.UseHttp();
    
    // Use timer activities.
    elsa.UseScheduling();
    
    // Register custom activities from the application, if any.
    elsa.AddActivitiesFrom<Program>();
    
    // Register custom workflows from the application, if any.
    elsa.AddWorkflowsFrom<Program>();
});

// Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin() // For demo purposes only. Use a specific origin instead.
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("x-elsa-workflow-instance-id"))); // Required for Elsa Studio in order to support running workflows from the designer. Alternatively, you can use the `*` wildcard to expose all headers.

// Add Health Checks.
builder.Services.AddHealthChecks();

// Build the web application.
var app = builder.Build();

// Configure web application's middleware pipeline.
app.UseCors();
app.UseRouting(); // Required for SignalR.
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi(); // Use Elsa API endpoints.
app.UseWorkflows(); // Use Elsa middleware to handle HTTP requests mapped to HTTP Endpoint activities.
app.UseWorkflowsSignalRHubs(); // Optional SignalR integration. Elsa Studio uses SignalR to receive real-time updates from the server. 

app.Run();
```

To run the application on port 5001, execute the following command:

```shell
dotnet run --urls "https://localhost:5001"
```

The above example demonstrates how to:

- Configure Elsa to use Entity Framework Core for persistence.
- Configure Elsa to use ASP.NET Core Identity for authentication/authorization.
- Configure Elsa to use ASP.NET Core authentication/authorization.
- Expose Elsa API endpoints.
- Setup a SignalR hub for real-time updates from the server.
- Enable C# workflow expressions.
- Enable HTTP activities.
- Enable timer activities.
- Register custom activities from the application, if any.
- Register custom workflows from the application, if any.
- Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
- Add Health Checks.
- Configure ASP.NET's middleware pipeline.
- Use Elsa API endpoints.
- Use Elsa middleware to handle HTTP requests mapped to HTTP Endpoint activities.
- Use SignalR integration. Elsa Studio uses SignalR to receive real-time updates from the server.
- Run the application on port 5001.

Now that we have an Elsa Server running, let's create an Elsa Studio application.

## Summary

In this chapter, we learned how to setup an Elsa Server.

The final result of this chapter can be found [here](https://github.com/elsa-workflows/elsa-guides/tree/main/src/installation/elsa-server/ElsaServer).