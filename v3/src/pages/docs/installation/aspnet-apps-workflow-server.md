---
title: ASP.NET workflow server
description: Installing Elsa in ASP.NET apps to act as a workflow server.
---

## Introduction

In the previous chapter, we looked at a basic ASP.NET application and learned how to install Elsa packages and execute workflows.

In this chapter, we will go one step further and configure expose REST API endpoints to allow external applications to define and run workflows.
This is a prerequisite for using the **workflows designer component** which consumes said API endpoints. 

## Setup

Create a new empty ASP.NET app using the following command:

```shell
dotnet new web -n "WorkflowServer.Api" -f net7.0
```

CD into the project's root directory and add the following packages:

```shell
cd WorkflowServer.Api
dotnet add package Elsa
dotnet add package Elsa.Http
dotnet add package Elsa.Workflows.Api
```

Next, open `Program.cs` file and replace its contents with the following code:

**Program.cs**

```clike
using Elsa.Extensions;
using Elsa.Http.Extensions;
using Elsa.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddElsa(elsa =>
{
    // Expose API endpoints.
    elsa.UseWorkflowsApi();

    // Add services for HTTP activities and workflow middleware.
    elsa.UseHttp();
    
    // Configure identity so that we can create a default admin user.
    elsa.UseIdentity(identity =>
    {
        identity.IdentityOptions.CreateDefaultAdmin = builder.Environment.IsDevelopment();
        identity.TokenOptions.SigningKey = "secret-token-signing-key";
    });
    
    // Use default authentication (JWT).
    elsa.UseDefaultAuthentication();
});

// Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();
```

The API endpoints exposed by Elsa are protected by default, and require authenticated requests.
In order to send authenticated requests, we need a JWT bearer token, aka an access token.

## Getting an access token

Start the application and send the following HTTP request:

```shell
curl --location --request POST 'https://localhost:7248/elsa/api/identity/login' \
--header 'Content-Type: application/json' \
--data-raw '{
    "username": "admin",
    "password": "password"
}'
```

The response should look something like this:

```json
{
    "isAuthenticated": true,
    "accessToken": "{your_access_token}"
}
```

Now that we have an access token with admin privileges, we can create and execute workflows.

{% callout title="Cyber security warning" type="warning" %}
As you may have noticed, we configured the system to create a default user when running the application in development mode.
Internally, this will create a user with admin privileges with the user name *"admin"* and password *"password"*.

Never deploy applications to a production environment using this default username and password. We are using it here for demo purposes only.
Later on, we will see how we can use the API to create new users from our local machine.

{% /callout %}

## Trying it out

### Creating a workflow via REST API.

With the application running, we're going to send the following JSON payload to the `/workflow-definitions` endpoint:

```json
{
    "name": "Hello World",
    "root": {
        "type": "Elsa.WriteLine",
        "text": "Hello World!"
    },
    "publish": true
}
```

The payload represents a workflow with the name "Hello World" and a **root** activity of type **Elsa.WriteLine** which is mapped to the `WriteLine` activity type we've seen before.
The `publish` field will tell the endpoint to immediately publish the workflow so that it can be invoked.

The following is the curl representation oif the request to send:

```shell
curl --location --request POST 'https://localhost:7248/elsa/api/workflow-definitions' \
--header 'Authorization: Bearer {your_access_token}' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Hello World",
    "root": {
        "type": "Elsa.WriteLine",
        "text": "Hello World!"
    },
    "publish": true
}'
```

The response should look something like this:

```json
{
    "id": "de9ea6acaa774d04aafdad86248cc29d",
    "definitionId": "a84e91cfee7644d7a977e78494be5c5a",
    "name": "Hello World",
    "createdAt": "2022-12-29T18:36:37.279474+00:00",
    "version": 1,
    "variables": [],
    "metadata": {},
    "isLatest": true,
    "isPublished": true,
    "root": {
        "text": {
            "typeName": "String",
            "expression": {
                "type": "Literal",
                "value": "Hello World!"
            },
            "memoryReference": {}
        },
        "type": "Elsa.WriteLine",
        "version": 1,
        "canStartWorkflow": false,
        "runAsynchronously": false,
        "customProperties": {},
        "metadata": {}
    }
}
```

### Listing workflow definitions

To list all workflow definitions, we can send a GET request to the `/workflow-definitions` endpoint. For example:

```shell
curl --location --request GET 'https://localhost:7248/elsa/api/workflow-definitions' \
--header 'Authorization: Bearer {your_access_token}'
```

That will result in a response similar to the following:

```json
{
    "items": [
        {
            "id": "de9ea6acaa774d04aafdad86248cc29d",
            "definitionId": "a84e91cfee7644d7a977e78494be5c5a",
            "name": "Hello World",
            "version": 1,
            "isLatest": true,
            "isPublished": true,
            "materializerName": "Json",
            "createdAt": "2022-12-29T18:36:37.279474+00:00"
        }
    ],
    "totalCount": 1
}
```

### Executing workflow definitions

To execute a workflow, send a POST request to the `/workflow-definitions/{definition_id}/execute` endpoint. For example:

```shell
curl --location --request POST 'https://localhost:7248/elsa/api/workflow-definitions/a84e91cfee7644d7a977e78494be5c5a/execute' \
--header 'Authorization: Bearer {your_access_token}'
```

The response will return the workflow instance ID, and to see that the workflow really executed, take a look at your application's console window to read the text that was written by the WriteLine activity:

![Response](/installation/console-output-1.png)

Although being able to control your workflow server via a REST API is awesome, handcrafting workflow JSON probably not so much ;)

Fortunately, we don't have to, thanks to the designer tool. Which is the topic of the next chapter.

## Persistence

When you restart the application, you will notice that your workflow will not have been persisted anywhere. This is because by default, Elsa uses an in-memory store for storing workflow definitions.
To change this, we need to configure an actual persistence store.

Out of the box, Elsa ships with an Entity Framework Core and a MongoDB implementation.

Let's take a look at setting up the EF Core provider and how to configure it to use SQLite.

### Entity Framework Core

First, add the `Elsa.EntityFrameworkCore` and `Elsa.EntityFrameworkCore.Sqlite` packages:

```shell
dotnet add package Elsa.EntityFrameworkCore
dotnet add package Elsa.EntityFrameworkCore.Sqlite
```

Next, update `Program.cs` to configure the Elsa Management feature to use the EF Core provider, which we in turn configure to use SQLite:

```clike
// Configure management feature to use EF Core.
elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));
```

When no connection string is provided for SQLite, like in our case, the following connection string is used by default: `"Data Source=elsa.sqlite.db;Cache=Shared;"`.

This time around, workflows will be persisted even after you restart the application. 
