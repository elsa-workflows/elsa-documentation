---
title: Elsa Server
description: Setting up an Elsa Server.
---

In this chapter, we'll learn how to setup an Elsa Server.

## What is an Elsa Server?

An Elsa Server is an ASP.NET Core web application that lets you manage workflows using a REST API ad execute them. 
You can store your workflows in various places like databases, file systems, or even cloud storage. In this guide, we're going to learn how to set one up! 

## Setup

### 1. Create Your Elsa Server Project

First, we need to create a new ASP.NET project. Open your command line tool and run these commands:

```shell
dotnet new web -n "ElsaServer" -f net8.0
cd ElsaServer
dotnet add package Elsa
dotnet add package Elsa.EntityFrameworkCore
dotnet add package Elsa.EntityFrameworkCore.Sqlite
dotnet add package Elsa.Identity
dotnet add package Elsa.Scheduling
dotnet add package Elsa.Workflows.Api
dotnet add package Elsa.CSharp
```

These commands will set up a new web project and add necessary Elsa packages to it.

### 2. Update Program.cs

Now, we need to add some code to make our server work.
Open the Program.cs file in your project and replace its contents with the code provided below.
This code does a lot of things like setting up database connections, enabling user authentication, and preparing the server to handle workflows.

**Program.cs:**

```clike
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
        identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-signing-key"; // This key needs to be at least 256 bits long.
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

### 3. Launch the Server

To run the application on port 5001, execute the following command:

```shell
dotnet run --urls "https://localhost:5001"
```

## What did we just do?

Here's a quick rundown of what we've set up:

- We made Elsa use Entity Framework Core for storing data.
- We configured user authentication and authorization.
- We made sure our server can communicate with the Elsa Studio app.
- We added capabilities for real-time updates and handling HTTP requests.
- We added health checks to keep an eye on our server's status.
- Finally, we made our server ready to run.

Now that we have an Elsa Server running, let's [create an Elsa Studio application](./elsa-studio-blazorwasm).

## Next Steps

With your Elsa Server up and running, you can now proceed to [create an Elsa Studio application](./elsa-studio-blazorwasm)

## Summary

In this guide, we walked through the steps to set up an Elsa Server. You can see the final result and more details [here](https://github.com/elsa-workflows/elsa-guides/tree/main/src/installation/elsa-server/ElsaServer).