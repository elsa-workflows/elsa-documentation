---
title: Elsa Studio Blazor Server
description: Setting up Elsa Studio Blazor Server. 
---

## Introduction

In the previous chapter, we looked at setting up Ela Studio in a Blazor Webassembly project.

In this chapter, we will create the same application, but this time using Blazor Server.

## Setup

To setup Elsa Studio, we'll go through the following steps:

1. Create a new Blazor Server application.
2. Add the Elsa Studio packages.
3. Modify the Program.cs. file.
4. Remove any superfluous files.
5. Update the `appsettings.json` file.
6. Update the `Pages/_Hosts.cshtml` file.

Let's delve into each step in detail:

### 1. Creating a New Blazor Server App

Kickstart your project by generating a new Blazor Server application using the following command:

```shell
dotnet new blazorserver-empty -n "ElsaStudioBlazorServer" -f net7.0
```

### 2. Adding Elsa Studio Packages

Navigate to the root directory of your project and integrate the following Elsa Studio packages:

```shell
cd ElsaStudioBlazorServer
dotnet add package Elsa.Studio --prerelease
dotnet add package Elsa.Studio.Core.BlazorServer --prerelease
dotnet add package Elsa.Studio.Login.BlazorServer --prerelease
```

### 3. Modifying Program.cs

Open the Program.cs file and replace its existing content with the code provided below:

**Program.cs**

```clike
using Elsa.Studio.Backend.Extensions;
using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;

// Build the host.
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Register Razor services.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    // Register the root components.
    options.RootComponents.RegisterCustomElsaStudioElements();
});

// Register shell services and modules.
builder.Services.AddCore();
builder.Services.AddShell(options => configuration.GetSection("Shell").Bind(options));
builder.Services.AddRemoteBackendModule(options => configuration.GetSection("Backend").Bind(options));
builder.Services.AddLoginModule();
builder.Services.AddDashboardModule();
builder.Services.AddWorkflowsModule();

// Configure SignalR.
builder.Services.AddSignalR(options =>
{
    // Set MaximumReceiveMessageSize:
    options.MaximumReceiveMessageSize = 5 * 1024 * 1000; // 5MB
});

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Run the application.
app.Run();
```

### 4. Removing Unnecessary Files

For a cleaner project structure, eliminate the following directories and files:

- `wwwroot/css`
- `Pages/Index.razor`
- `App.razor`
- `MainLayout.razor`

### 5. Updating appsettings.json

Update the `appsettings.json` file with the following content:

```json
{
  "Backend": {
    "Url": "https://localhost:5001/elsa/api"
  }
}
```

### 6. Updating _Host.cshtml

To conclude the setup, open the `Pages/_Host.cshtml` file and replace its content with the code showcased below:

**_Host.cshtml**

```html
@page "/"
@using Elsa.Studio.Shell
@using Microsoft.AspNetCore.Components.Web
@namespace Elsa.Studio.Host.Server.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <base href="~/"/>
    <link rel="apple-touch-icon" sizes="180x180" href="_content/Elsa.Studio.Shell/apple-touch-icon.png">
    <link rel="icon" type="image/png" sizes="32x32" href="_content/Elsa.Studio.Shell/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="16x16" href="_content/Elsa.Studio.Shell/favicon-16x16.png">
    <link rel="manifest" href="_content/Elsa.Studio.Shell/site.webmanifest">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Ubuntu:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Grandstander:wght@100&display=swap" rel="stylesheet">
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet"/>
    <link href="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.css" rel="stylesheet"/>
    <link href="_content/Radzen.Blazor/css/material-base.css" rel="stylesheet">
    <link href="_content/Elsa.Studio.Shell/css/shell.css" rel="stylesheet">
    <link href="Elsa.Studio.Host.Server.styles.css" rel="stylesheet">
    <component type="typeof(HeadOutlet)" render-mode="ServerPrerendered"/>
</head>
<body>
<component type="typeof(App)" render-mode="ServerPrerendered" />

<div id="blazor-error-ui">
    <environment include="Staging,Production">
        An error has occurred. This application may no longer respond until reloaded.
    </environment>
    <environment include="Development">
        An unhandled exception has occurred. See browser dev tools for details.
    </environment>
    <a href="" class="reload">Reload</a>
    <a class="dismiss">🗙</a>
</div>
<script src="_content/BlazorMonaco/jsInterop.js"></script>
<script src="_content/BlazorMonaco/lib/monaco-editor/min/vs/loader.js"></script>
<script src="_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.js"></script>
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
<script src="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.js"></script>
<script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
<script src="_framework/blazor.server.js"></script>
</body>
</html>
```

## Launching the Application

To witness your application in action, execute the following command:

```shell
dotnet run
```

Your application should now be accessible at https://localhost:5001. The port number might vary based on your configuration. By default, you can log in using:

```html
Username: admin
Password: password
```

{% callout title="Source code" %}
The source code for this chapter can be found [here](https://github.com/elsa-workflows/elsa-guides/tree/v3/src/installation/elsa-studio/ElsaStudioBlazorServer)
{% /callout %}

## What's Next?
In our upcoming chapter, we will explore the customization possibilities of the Elsa Studio application. Stay tuned!
