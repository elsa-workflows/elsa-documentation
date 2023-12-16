---
title: Elsa Studio Blazor Webassembly
description: Setting up Elsa Studio Blazor Webassembly. 
---

## Introduction

In the previous chapter, we looked at setting up a workflow server in ASP.NET and looked at creating and executing workflows using the REST API.

In this chapter, we will create a separate ASP.NET Blazor Webassembly app that hosts Elsa Studio that connects to the workflow server.

## Setup

To setup Elsa Studio, we'll go through the following steps:

1. Create a new Blazor Webassembly application.
2. Add the Elsa Studio packages.
3. Modify the Program.cs. file.
4. Remove any superfluous files.
5. Generate a `wwwroot/appsettings.json` file.
6. Update the `wwwroot/index.html` file.

Let's delve into each step in detail:

### 1. Creating a New Blazor Webassembly App

Kickstart your project by generating a new Blazor Webassembly application using the following command:

```shell
dotnet new blazorwasm-empty -n "ElsaStudioBlazorWasm" -f net7.0
```

### 2. Adding Elsa Studio Packages

Navigate to the root directory of your project and integrate the following Elsa Studio packages:

```shell
cd ElsaStudioBlazorWasm
dotnet add package Elsa.Studio --prerelease
dotnet add package Elsa.Studio.Core.BlazorWasm --prerelease
dotnet add package Elsa.Studio.Login.BlazorWasm --prerelease
```

### 3. Modifying Program.cs

Open the Program.cs file and replace its existing content with the code provided below:

**Program.cs**

```clike
using Elsa.Studio.Backend.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Shell;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Login.BlazorWasm.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Build the host.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;

// Register root components.
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.RegisterCustomElsaStudioElements();

// Register shell services and modules.
builder.Services.AddCore();
builder.Services.AddShell();
builder.Services.AddRemoteBackendModule(options => configuration.GetSection("Backend").Bind(options));
builder.Services.AddLoginModule();
builder.Services.AddDashboardModule();
builder.Services.AddWorkflowsModule();

// Build the application.
var app = builder.Build();

// Run each startup task.
var startupTaskRunner = app.Services.GetRequiredService<IStartupTaskRunner>();
await startupTaskRunner.RunStartupTasksAsync();

// Run the application.
await app.RunAsync();
```

### 4. Removing Unnecessary Files

For a cleaner project structure, eliminate the following directories and files:

- `wwwroot/css`

### 5. Generating wwwroot/appsettings.json

Within the `wwwroot` directory, craft a new `appsettings.json` file and populate it with the subsequent content:

**appsettings.json**

```json
{
  "Backend": {
    "Url": "https://localhost:5001/elsa/api"
  }
}
```

### 6. Updating index.html

To conclude the setup, open the `wwwroot/index.html` file and replace its content with the code showcased below:

**index.html**

```html
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
    <title>MyApplication</title>
    <base href="/"/>
    <link rel="apple-touch-icon" sizes="180x180" href="_content/Elsa.Studio.Shell/apple-touch-icon.png">
    <link rel="icon" type="image/png" sizes="32x32" href="_content/Elsa.Studio.Shell/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="16x16" href="_content/Elsa.Studio.Shell/favicon-16x16.png">
    <link rel="manifest" href="_content/Elsa.Studio.Shell/site.webmanifest">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Ubuntu:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Grandstander:wght@100&display=swap" rel="stylesheet">
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
    <link href="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.css" rel="stylesheet" />
    <link href="_content/Radzen.Blazor/css/material-base.css" rel="stylesheet" >
    <link href="_content/Elsa.Studio.Shell/css/shell.css" rel="stylesheet">
    <link href="Elsa.Studio.Host.Wasm.styles.css" rel="stylesheet">
</head>

<body>
<div id="app">
    <div class="loading-splash mud-container mud-container-maxwidth-false">
        <h5 class="mud-typography mud-typography-h5 mud-primary-text my-6">Loading...</h5>
    </div>
</div>

<div id="blazor-error-ui">
    An unhandled error has occurred.
    <a href="" class="reload">Reload</a>
    <a class="dismiss">🗙</a>
</div>
<script src="_content/BlazorMonaco/jsInterop.js"></script>
<script src="_content/BlazorMonaco/lib/monaco-editor/min/vs/loader.js"></script>
<script src="_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.js"></script>
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
<script src="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.js"></script>
<script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
<script src="_framework/blazor.webassembly.js"></script>
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
The source code for this chapter can be found [here](https://github.com/elsa-workflows/elsa-guides/tree/main/src/installation/elsa-studio/ElsaStudioWasm)
{% /callout %}

## What's Next?
In our upcoming chapter, we will explore the customization possibilities of the Elsa Studio application. Stay tuned!