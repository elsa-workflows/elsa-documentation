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
dotnet add package Elsa.Identity
dotnet add package Elsa.Workflows.Api
```

Next, open `Program.cs` file and replace its contents with the following code:

**Program.cs**

```clike
using Elsa.Extensions;
using Elsa.Http.Extensions;
using Elsa.Identity.Extensions;
using Elsa.Identity.Features;

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
        identity.UseAdminUserProvider();
        identity.TokenOptions = options => options.SigningKey = "secret-token-signing-key";
    });
    
    // Use default authentication (JWT + API Key).
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
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

### Security

The API endpoints exposed by Elsa are protected by default, and require authenticated requests.

In the code above, we configured the system to use the default authentication scheme, which is JWT + API Key.
This means that we need to either provide an **access token** or **API key** in the `Authorization` header of our HTTP requests.

### Admin API key

In this example, we configured the default authentication feature to install an API key provider that will provide an API key for the default admin user.
The admin API key is: `00000000-0000-0000-0000-000000000000`. 

To make authenticated requests to the API endpoints using an API key, we can include the API key as follows:

```shell
curl --location 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: ApiKey 00000000-0000-0000-0000-000000000000'
````

### Admin access token

To request an access token, we can send the following request to the `/identity/login` endpoint:

```shell
curl --location --request POST 'https://localhost:5001/elsa/api/identity/login' \
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
    "accessToken": "{access_token}",
    "refreshToken": "{refresh_token}"
}
```

To make authenticated requests to the API endpoints using an access token, we can include the access token as follows:

```shell
curl --location 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: Bearer {access_token}'
````

When we login to the designer, the designer will request an access token and use it to make authenticated requests to the API endpoints.
When we build external applications, we can communicate with the workflow server API using API keys or access tokens.

{% callout title="Your application is at risk!" type="warning" %}
We configured the system to use a default admin user and admin API key.
Internally, this will provide a user and application with admin privileges with the user name *"admin"* and password *"password"*.

Never deploy applications to a production environment using this default username and password. We are using it here for demo purposes only.

{% /callout %}

### Users and applications

Instead of using the default admin user & API key, we can configure the system to use custom users and applications.
We will use the configuration-based providers for this.

{% callout title="Other providers" %}
Out of the box, Elsa comes with configuration-based providers for users, applications and roles.
You can also use the database-based providers from _Elsa.EntityFrameworkCore_, or write your own providers.
{% /callout %}

First, let's update the code in `Program.cs` to use the configuration-based identity providers:

```clike
.UseIdentity(identity =>
{
    var configuration = builder.Configuration;
    var identitySection = configuration.GetSection("Identity");
    var identityTokenSection = identitySection.GetSection("Tokens");
    
    identity.IdentityOptions = options => identitySection.Bind(options);
    identity.TokenOptions = options => identityTokenSection.Bind(options);
    identity.UseConfigurationBasedUserProvider(options => identitySection.Bind(options));
    identity.UseConfigurationBasedApplicationProvider(options => identitySection.Bind(options));
    identity.UseConfigurationBasedRoleProvider(options => identitySection.Bind(options));
})
```

Next, add the following configuration section to appsettings.json:

```json
{
  "Identity": {
    "Tokens": {
      "SigningKey": "secret-signing-key",
      "AccessTokenLifetime": "1:00:00:00",
      "RefreshTokenLifetime": "1:00:10:00"
    },
    "Roles": [{
      "Id": "admin",
      "Name": "Administrator",
      "Permissions": ["*"]
    }],
    "Users": [],
    "Applications": []
  }
}
```

In order to create a new user using the API, we first need an API key that has the right permission to do so.
Although we could use the default admin API key, let's create a new API key instead.

Send the following request to the `/identity/applications` endpoint:

```shell
curl --location 'https://localhost:5001/elsa/api/identity/applications' \
--header 'Content-Type: application/json' \
--data '{
    "name": "Postman",
    "roles": ["admin"]
}'
````

{% callout title="Applications" %}
By default, creating applications is allowed for anonymous requests from the localhost.
{% /callout %}

When we send the request, we receive a response similar to the following:

```json
{
    "id": "35dfe2518227454699a6febe27ae673f",
    "name": "Postman",
    "roles": [
        "admin"
    ],
    "clientId": "ytUOVpe0NnDaRC9f",
    "clientSecret": "LES7gpF61.|umSs1S1@Ft*H#<J,()i8y",
    "apiKey": "7974554F567065304E6E446152433966-e0931498-0e8e-4c1b-9eed-49bf78b6e1e6",
    "hashedApiKey": "DplTmSOg7dAqHqnW/FzDysjqd6pFh8EiN/IzZ2n+pgc=",
    "hashedApiKeySalt": "6dH9bxJmi5QLDvICV2MONhQxul9TKPA7dG+XiobQ8kQ=",
    "hashedClientSecret": "pW/ek5PxrjgsvoZKoxidfgnSRh4rfPIOPt22fYL4bBg=",
    "hashedClientSecretSalt": "c6FQ+sHJDWJjnPwOkxEjk1Ie4HXYDGeAycKCkscD1uM="
}
```

Copy and paste the entire response, **except for the `apiKey` property**, into the `Applications` section of the `Identity` configuration section in `appsettings.json`:

```json
{
  "Identity": {
    "Tokens": {
      "SigningKey": "secret-signing-key",
      "AccessTokenLifetime": "1:00:00:00",
      "RefreshTokenLifetime": "1:00:10:00"
    },
    "Roles": [{
      "Id": "admin",
      "Name": "Administrator",
      "Permissions": ["*"]
    }],
    "Users": [],
    "Applications": [{
      "id": "35dfe2518227454699a6febe27ae673f",
      "name": "Postman",
      "roles": [
        "admin"
      ],
      "clientId": "ytUOVpe0NnDaRC9f",
      "clientSecret": "LES7gpF61.|umSs1S1@Ft*H#<J,()i8y",
      "hashedApiKey": "DplTmSOg7dAqHqnW/FzDysjqd6pFh8EiN/IzZ2n+pgc=",
      "hashedApiKeySalt": "6dH9bxJmi5QLDvICV2MONhQxul9TKPA7dG+XiobQ8kQ=",
      "hashedClientSecret": "pW/ek5PxrjgsvoZKoxidfgnSRh4rfPIOPt22fYL4bBg=",
      "hashedClientSecretSalt": "c6FQ+sHJDWJjnPwOkxEjk1Ie4HXYDGeAycKCkscD1uM="
    }]
  }
}
```

Restart the application and send the following request to the `/identity/users` endpoint using the API key we just created:

```shell
curl --location 'https://localhost:5001/elsa/api/identity/users' \
--header 'Content-Type: application/json' \
--header 'Authorization: ApiKey 7974554F567065304E6E446152433966-e0931498-0e8e-4c1b-9eed-49bf78b6e1e6' \
--data '{
    "name": "admin",
    "password": "password",
    "roles": ["admin"]
}'
```

This should give a response similar to the following:

```json
{
  "id": "c641abef691448608da8be497704dacb",
  "name": "admin",
  "password": "password",
  "roles": [
    "admin"
  ],
  "hashedPassword": "TcRbxSqIDui2hQfINxVvXgSyxVw4oqfm2g6S8zNW84I=",
  "hashedPasswordSalt": "HWAodoIPNWm/6kmIrrkTnKUs1nxt0Fc6cMMztV7pK3M="
}
```

Copy and paste the entire response **except for the password property** into the `Users` field in appsettings.json:

```json
{
  "Identity": {
    "Tokens": {
      "SigningKey": "secret-signing-key",
      "AccessTokenLifetime": "1:00:00:00",
      "RefreshTokenLifetime": "1:00:10:00"
    },
    "Roles": [{
      "Id": "admin",
      "Name": "Administrator",
      "Permissions": ["*"]
    }],
    "Users": [{
      "id": "c641abef691448608da8be497704dacb",
      "name": "admin",
      "roles": [
        "admin"
      ],
      "hashedPassword": "TcRbxSqIDui2hQfINxVvXgSyxVw4oqfm2g6S8zNW84I=",
      "hashedPasswordSalt": "HWAodoIPNWm/6kmIrrkTnKUs1nxt0Fc6cMMztV7pK3M="
    }],
    "Applications": [{
      "id": "35dfe2518227454699a6febe27ae673f",
      "name": "Postman",
      "roles": [
        "admin"
      ],
      "clientId": "ytUOVpe0NnDaRC9f",
      "clientSecret": "LES7gpF61.|umSs1S1@Ft*H#<J,()i8y",
      "hashedApiKey": "DplTmSOg7dAqHqnW/FzDysjqd6pFh8EiN/IzZ2n+pgc=",
      "hashedApiKeySalt": "6dH9bxJmi5QLDvICV2MONhQxul9TKPA7dG+XiobQ8kQ=",
      "hashedClientSecret": "pW/ek5PxrjgsvoZKoxidfgnSRh4rfPIOPt22fYL4bBg=",
      "hashedClientSecretSalt": "c6FQ+sHJDWJjnPwOkxEjk1Ie4HXYDGeAycKCkscD1uM="
    }]
  }
}
```

Restart the application and send the following request to the `/identity/tokens` endpoint using the API key we just created:

```shell
curl --location 'https://localhost:5001/elsa/api/identity/login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "admin",
    "password": "password"
}'
````

The response will include a new access token as well as a refresh token:

```json
{
    "isAuthenticated": true,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJwZXJtaXNzaW9ucyI6IioiLCJuYmYiOjE2ODIxNzEwODgsImV4cCI6MTY4MjI1NzQ4OCwiaWF0IjoxNjgyMTcxMDg4LCJpc3MiOiJodHRwOi8vZWxzYS5hcGkiLCJhdWQiOiJodHRwOi8vZWxzYS5hcGkifQ.vzf6e7ZzzbD-12PWD8ootbd91Vqz4Vf8aFqV36cFY8M",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJwZXJtaXNzaW9ucyI6IioiLCJuYmYiOjE2ODIxNzEwODgsImV4cCI6MTY4MjI1ODA4OCwiaWF0IjoxNjgyMTcxMDg4LCJpc3MiOiJodHRwOi8vZWxzYS5hcGkiLCJhdWQiOiJodHRwOi8vZWxzYS5hcGkifQ.3C3zS1ByDZcXTaKXuF-3lQ7sA5k6SVDsoYwhcMEhzcQ"
}
```

When you decode the access token, you'll see that it contains the user's name and permissions:

```json
{
  "name": "admin",
  "permissions": "*",
  "nbf": 1682171088,
  "exp": 1682257488,
  "iat": 1682171088,
  "iss": "http://elsa.api",
  "aud": "http://elsa.api"
}
```

We can now use the access token we just received to access the REST API. For example, we can send the following request to retrieve a list of all workflow definitions:

```shell
curl --location 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJwZXJtaXNzaW9ucyI6IioiLCJuYmYiOjE2ODIxNzEwODgsImV4cCI6MTY4MjI1NzQ4OCwiaWF0IjoxNjgyMTcxMDg4LCJpc3MiOiJodHRwOi8vZWxzYS5hcGkiLCJhdWQiOiJodHRwOi8vZWxzYS5hcGkifQ.vzf6e7ZzzbD-12PWD8ootbd91Vqz4Vf8aFqV36cFY8M'
```

Instead of using the access token, we can use the API key instead:

```shell
curl --location 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: ApiKey 7974554F567065304E6E446152433966-e0931498-0e8e-4c1b-9eed-49bf78b6e1e6'
```

{% callout title="Access Tokens vs API Keys" %}
For most scenarios, you'll probably want to use API keys instead of access tokens. API keys are easier to use and don't require you to manage access tokens.
{% /callout %}

---

## Trying it out

Let's try out the REST API by creating a workflow definition and then invoking it.

### Creating a workflow via REST API.

With the application running, we're going to send the following JSON payload to the `/workflow-definitions` endpoint:

```json
{
  "model": {
    "name": "Hello World",
    "root": {
      "type": "Elsa.WriteLine",
      "text": "Hello World!"
    }
  },
  "publish": true
}
```

The payload represents a workflow with the name "Hello World" and a **root** activity of type **Elsa.WriteLine** which is mapped to the `WriteLine` activity type we've seen before.
The `publish` field will tell the endpoint to immediately publish the workflow so that it can be invoked.

The following is the curl representation oif the request to send:

```shell
curl --location --request POST 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: ApiKey {api_key}' \
--header 'Content-Type: application/json' \
--data-raw '{
  "model": {
    "name": "Hello World",
    "root": {
      "type": "Elsa.WriteLine",
      "text": "Hello World!"
    }
  },
  "publish": true
}'
```

The response should look something like this:

```json
{
  "id": "c4f4ed765a864492af1c976039e474f0",
  "definitionId": "8633e947391a4038bd59fd601169480c",
  "name": "Hello World",
  "createdAt": "2023-05-30T17:45:18.944564+00:00",
  "version": 1,
  "variables": [],
  "inputs": [],
  "outputs": [],
  "outcomes": [],
  "customProperties": {},
  "isLatest": true,
  "isPublished": true,
  "root": {
    "text": {
      "typeName": "String",
      "expression": {
        "type": "Literal",
        "value": "Hello World!"
      },
      "memoryReference": {
        "id": "WriteLine1:input-1"
      }
    },
    "id": "WriteLine1",
    "type": "Elsa.WriteLine",
    "version": 1,
    "customProperties": {
      "CanStartWorkflow": false,
      "RunAsynchronously": false
    },
    "metadata": {}
  }
}
```

### Listing workflow definitions

To list all workflow definitions, we can send a GET request to the `/workflow-definitions` endpoint. For example:

```shell
curl --location --request GET 'https://localhost:5001/elsa/api/workflow-definitions' \
--header 'Authorization: ApiKey {api_key}'
```

That will result in a response similar to the following:

```json
{
  "items": [
    {
      "id": "c4f4ed765a864492af1c976039e474f0",
      "definitionId": "8633e947391a4038bd59fd601169480c",
      "name": "Hello World",
      "version": 1,
      "isLatest": true,
      "isPublished": true,
      "materializerName": "Json",
      "createdAt": "2023-05-30T17:45:18.944564+00:00"
    }
  ],
  "totalCount": 1
}
```

### Executing workflow definitions

To execute a workflow, send a POST request to the `/workflow-definitions/{definition_id}/execute` endpoint. For example:

```shell
curl --location --request POST 'https://localhost:5001/elsa/api/workflow-definitions/a84e91cfee7644d7a977e78494be5c5a/execute' \
--header 'Authorization: ApiKey {api_key}'
```

The response will return the workflow instance ID, and to see that the workflow really executed, take a look at your application's console window to read the text that was written by the WriteLine activity:

![Response](/installation/console-output-1.png)

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

### MongoDB

{% callout title="MongoDB" type="note" %}
The MongoDB provider is coming soon.
{% /callout %}

[//]: # (Elsa also ships with a MongoDB provider. To use it, add the `Elsa.MongoDb` package:)

[//]: # ()
[//]: # (```shell)

[//]: # (dotnet add package Elsa.MongoDb)

[//]: # (```)

[//]: # ()
[//]: # (Update `Program.cs` to configure the Elsa Management feature to use the MongoDB provider:)

[//]: # ()
[//]: # (```clike)

[//]: # (// Configure management feature to use MongoDB.)

[//]: # (elsa.UseWorkflowManagement&#40;management => management.UseMongoDb&#40;"localhost"&#41;&#41;;)

[//]: # (```)

### Custom persistence

If you want to use a different persistence provider, you can implement your own persistence provider. For more information, see [Implementing a custom persistence provider](../extensibility/custom-persistence).
