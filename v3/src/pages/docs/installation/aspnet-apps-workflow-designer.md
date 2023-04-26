---
title: ASP.NET workflow designer
description: Installing the workflow designer in an ASP.NET application. 
---

## Introduction

In the previous chapter, we looked at setting up a workflow server in ASP.NET and looked at creating and executing workflows using the REST API.

In this chapter, we will create a separate ASP.NET app that hosts the workflow designer that connects to the ASP.NET workflow server we created in the previous chapter. 

## Setup

Create a new empty ASP.NET app using the following command:

```shell
dotnet new web -n "WorkflowDesigner.Web" -f net7.0
```

CD into the project's root directory and add the `Elsa` and `WorkflowDesigner.Web` packages:

```shell
cd WokflowDesigner.Web
dotnet add package Elsa.Workflows.Designer
```

Next, open `Program.cs` file and replace its contents with the following code:

**Program.cs**

```clike
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
```

Notice that nothing specific to Elsa wa added here. It's a basic setup for an ASP.NET web app with Razor pages.

In the project, create a new folder called `Pages` and create a new file called `_ViewImports.cshtml` with the following contents:

```razor
@namespace WorkflowDesigner.Web.Pages
@using Elsa.Workflows.Designer
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Create another file called `Index.cshtml` with the following contents:

```html
@page "/"
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

<elsa-shell>
    <elsa-workflow-toolbar></elsa-workflow-toolbar>
    <div class="absolute inset-0" style="top: 64px;">
        <elsa-studio server="https://localhost:5001/elsa/api" monaco-lib-path="/_content/Elsa.Workflows.Designer/monaco-editor/min"></elsa-studio>
        <elsa-custom-activities-manager></elsa-custom-activities-manager>
        <elsa-notifications-manager></elsa-notifications-manager>
    </div>
</elsa-shell>

</body>
</html>
```

This will include the necessary scripts and styles provided by the `Elsa.Workflows.Designer` package and render the `ElsaStudio` razor component using the `component` tag helper.

Make sure that the URL for `param-ServerUrl` contains a valid URL pointing to a workflow server (the one we created in the previous chapter).
When running the workflow server, pay attention to the port number being used - this is likely different than the one used here. 

## Trying it out

When you start the application, you should see the login screen:

![Response](/installation/designer-login.png)

If you configured the workflow server to create a default user, you should be able to login with the default user name and password, which is:

```html
Username: admin
Password: password
```

After having logged in, you will see the home page:

![Response](/installation/designer-home.png)

We can now begin creating workflows via the New button displayed on the top-right:

![Response](/installation/designer-demo.gif)

Up until this point, we have seen how to setup a separate workflow server and designer application. And in case you're wondering, yes, it's also possible to combine the server + designer into a single ASP.NET application.
Which is the topic for the next chapter.