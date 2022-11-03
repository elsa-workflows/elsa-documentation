---
title: Dashboard
---

In this quickstart, we will take a look at a minimum ASP.NET Core application that hosts the Elsa Dashboard component and connect it to an [Elsa Server](quickstarts-aspnetcore-server-api-endpoints.md).

> **ElsaDashboard + Docker**
>
> Although it is useful to be able to setup your own Elsa Dashboard project in order to customize certain aspects, on many occasions you may find that the basic dashboard is al you need in order to connect to an Elsa Server.
>
> Elsa ships with a pre-made project called [ElsaDashboard.Web](https://github.com/elsa-workflows/elsa-core/tree/master/src/dashboards/aspnetcore/ElsaDashboard) which you can configure to talk to an existing Elsa Server.
> In addition, there is a Dockerfile that you can build and run as well.
>
> For more information, checkout the [ElsaDashboard + Docker](quickstarts/quickstarts-elsa-dashboard-docker) quickstart.

We will:

* Create an ASP.NET Core application.
* Install the Elsa Dashboard component.

## The Project

Create a new, empty ASP.NET Core project called `ElsaQuickstarts.Server.Dashboard`:

```bash
dotnet new web -n "ElsaQuickstarts.Server.Dashboard"
```

CD into the created project folder:

```bash
cd ElsaQuickstarts.Server.Dashboard
```

Add the following packages:

```bash
dotnet add package Elsa.Designer.Components.Web
```

## Startup

Open `Startup.cs` and replace its contents with the following:

```clike
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ElsaQuickstarts.Server.Dashboard
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapFallbackToPage("/_Host"); });
        }
    }
}
```

## _Host.cshtml + _ViewImports.cshtml

Notice that the application will always serve the _Host.cshtml page, which we will create next.

1. Create a new folder called `Pages`.
2. Inside `Pages`, create a new file called `_ViewImports.cshtml`.
2. Inside `Pages`, create a new file called `_Host.cshtml`.

Add the following content to `_ViewImports.cshtml`:

```html
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

And add the following content to `_Host.cshtml`:

```html
@page "/"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Elsa Workflows</title>
    <link rel="icon" type="image/png" sizes="32x32" href="/_content/Elsa.Designer.Components.Web/elsa-workflows-studio/assets/images/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="16x16" href="/_content/Elsa.Designer.Components.Web/elsa-workflows-studio/assets/images/favicon-16x16.png">
    <link rel="stylesheet" href="/_content/Elsa.Designer.Components.Web/elsa-workflows-studio/assets/fonts/inter/inter.css">
    <link rel="stylesheet" href="/_content/Elsa.Designer.Components.Web/elsa-workflows-studio/elsa-workflows-studio.css">
    <script src="/_content/Elsa.Designer.Components.Web/monaco-editor/min/vs/loader.js"></script>
    <script type="module" src="/_content/Elsa.Designer.Components.Web/elsa-workflows-studio/elsa-workflows-studio.esm.js"></script>
</head>
<body>
<elsa-studio-root server-url="https://your-elsa-server-url" monaco-lib-path="_content/Elsa.Designer.Components.Web/monaco-editor/min">
    <elsa-studio-dashboard></elsa-studio-dashboard>
</elsa-studio-root>
</body>
</html>
```

## Run

Run the program and open a web browser to the home page (usually happens automatically if you don't change `launchSettings.json`):

{% figure src="/assets/installation/installing-elsa-dashboard-figure-1.png" /%}

None of the menu items will function correctly until you made sure to point the component to a running Elsa server.

## Next Steps

In this guide, we've seen how to setup an Elsa Dashboard that can connect to an Elsa Server. We haven't covered setting up a server in this guide, but is covered [here](quickstarts-aspnetcore-server-api-endpoints.md)).

Now that you've seen how to setup an ASP.NET Core server with Elsa workflows support, you might want to learn more about the following:

* [How to setup an ASP.NET Core host with Elsa API Endpoints.](quickstarts-aspnetcore-server-api-endpoints.md)
* [How to setup an ASP.NET Core host with Elsa Dashboard + API Endpoints.](quickstarts-aspnetcore-server-dashboard-and-api-endpoints.md)
