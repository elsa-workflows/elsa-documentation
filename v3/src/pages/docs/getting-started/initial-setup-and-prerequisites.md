---
title: Initial Setup and Prerequisites
description: Begin your journey with Elsa Workflows by setting up the environment tailored to your needs. From a simple console application to a robust server setup, this guide has you covered.
---

Elsa Workflows offers flexibility in how you can set up and run workflows. Whether you're looking for a quick console-based workflow or a full-fledged ASP.NET Core workflow server, Elsa has got you covered. In this guide, we'll walk you through the initial setup for various application types.

## **Prerequisites**

Before diving into the setup, ensure you have the following:

- .NET SDK (Version 6 or higher)
- A code editor (e.g., Visual Studio, Visual Studio Code, Rider)
- Basic knowledge of C# and ASP.NET Core

## **1. Elsa Console Application**

For those who want a quick introduction to Elsa or wish to test workflows without a server setup, the console application is an ideal starting point:

1. **Create a New Console Application**:
   ```bash
   dotnet new console -n "ElsaConsole" -f net7.0
   ```

2. **Install Elsa NuGet Packages**:
   ```bash
   cd ElsaConsole
   dotnet add package Elsa
   ```

3. **Define and Run a Simple Workflow**:
   - Use C# to define a workflow that prompts for a name and greets the user.
   - Execute the workflow within the console application.

[Continue setting up an Elsa Console Application](/docs/installation/elsa-console#update-program-cs)

## **2. Elsa Server Application**

For developers and businesses aiming to leverage the full power of Elsa, setting up a workflow server provides a more comprehensive platform:

1. **Create a New ASP.NET Core Web Application**:
   ```bash
   dotnet new web -n ElsaServer
   ```

2. **Install Required Elsa NuGet Packages**:
   ```bash
   cd ElsaServer
   dotnet add package Elsa
   dotnet add package Elsa.Http
   dotnet add package Elsa.Workflows.Api
   ```

[Continue Setting up an Elsa Server Application](/docs/installation/aspnet-apps-workflow-server#update-program-cs)

## **3. Elsa Studio Application**

For a visual representation and management of your workflows, Elsa Studio offers a user-friendly interface to design, monitor, and manage your workflows:

1. **Create a New Blazor WebAssembly Application**:
   ```bash
   dotnet new blazorwasm -n ElsaStudio
   ```

2. **Install Elsa Studio NuGet Packages**:
   ```bash
   dotnet add package Elsa.Studio
   ```

3. **Connect Elsa Studio to the Workflow Server**:
   - Configure Elsa Studio to connect to the workflow server created in step 2.
   - Launch Elsa Studio and start designing your workflows!

[Link to detailed guide on setting up Elsa Studio Blazor Application]

---

{% callout title="Extensible Platform" %}
As you progress with Elsa, remember that the platform is extensible. You can always add more features, integrate with other systems, and customize it to fit your specific needs.
{% /callout %}