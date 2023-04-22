---
title: ASP.NET workflow server and designer
description: Installing Elsa in ASP.NET apps to act as a workflow server and designer.
---

## Introduction

In the previous two chapters, we looked at setting up a workflow server and designer separately,

In this chapter, we will setup an ASP.NET app that plays the role of a workflow server as well as a host for the designer.

The main difference in terms of setup is the lack of setting up a CORS policy, which is no longer necessary since the designer will call out to the same origin. 

## Setup

Create a new empty ASP.NET app using the following command:

```shell
dotnet new web -n "WorkflowsApp" -f net7.0
```

CD into the project's root directory and add the following packages:

```shell
cd WorkflowsApp
dotnet add package Elsa --prerelease
dotnet add package Elsa.EntityFrameworkCore --prerelease
dotnet add package Elsa.EntityFrameworkCore.Sqlite --prerelease
dotnet add package Elsa.Http --prerelease
dotnet add package Elsa.Identity --prerelease
dotnet add package Elsa.Workflows.Api --prerelease
dotnet add package Elsa.Workflows.Designer --prerelease
```

Next, open `Program.cs` file and replace its contents with the following code:

**Program.cs**

```clike
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.Extensions;
using Elsa.Identity.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddElsa(elsa =>
{
    // Configure management feature to use EF Core.
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    
    // Expose API endpoints.
    elsa.UseWorkflowsApi();

    // Add services for HTTP activities and workflow middleware.
    elsa.UseHttp();
    
    // Configure identity so that we can create a default admin user.
    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions.SigningKey = "secret-token-signing-key";
    });
    
    // Use default authentication (JWT + API Key).
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
});

// Add Razor pages.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapRazorPages();
app.Run();
```

{% callout title="Your application is at risk!" type="warning" %}
As you may have noticed, we configured the system to use a default admin user when running the application in development mode.
Internally, this will provide a user with admin privileges with the user name *"admin"* and password *"password"*.

Never deploy applications to a production environment using this default username and password. We are using it here for demo purposes only.

To configure the application with a different user name and password, see the the [Users and applications](./aspnet-apps-workflow-server#users-and-applications) section of the [ASP.NET workflow server](./aspnet-apps-workflow-server) chapter. 

{% /callout %}

In the project, create a new folder called `Pages` and create a new file called `_ViewImports.cshtml` with the following contents:

```razor
@namespace Elsa.Samples.WorkflowServerAndDesigner.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Create another file called `Index.cshtml` with the following contents:

```html
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

This will include the necessary scripts and styles provided by the `Elsa.Workflows.Designer` package and render the `ElsaStudio` razor component using the `component` tag helper.

Notice that we don't have to provide an absolute URL pointing to the workflow server, since the designer is hosted by the same application.
All we need to do is resolve a relative path to the backend root API and use that value for the `ServerUrl` attribute of the `ElsaStudio` Razor component.

## Trying it out

When you start the application, you should see the login screen:

![Response](/installation/designer-login.png)

Since we configured the application to create a default user, we can login with the default user name and password, which is:

```html
Username: admin
Password: password
```

After having logged in, you will see the home page:

![Response](/installation/designer-home.png)

We can now begin creating workflows via the New button displayed on the top-right:

![Response](/installation/designer-demo.gif)
