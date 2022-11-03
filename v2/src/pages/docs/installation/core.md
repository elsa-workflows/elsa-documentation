---
title: Installing Elsa
---

The **Elsa** NuGet package provides APIs to build and execute workflows. In this section, we'll see how to install the package into a .NET project and register the appropriate services with the DI container.

## Add Package

```bash
dotnet add package Elsa
```

## Register Services

To add Elsa services to your application, add the following code to `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa();
}
```

You can now resolve workflow services to build, load and execute workflows.
For example, here's a complete Hello World console application:

```c#
using System.Threading.Tasks;
using Elsa.Activities.Console;
using Elsa.Builders;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Samples.HelloWorldConsole
{
    class Program
    {
        private static async Task Main()
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection()
                .AddElsa(options => options
                    .AddConsoleActivities()
                    .AddWorkflow<HelloWorld>()) // Defined a little bit below.
                .BuildServiceProvider();
            
            // Get a workflow runner.
            var workflowRunner = services.GetRequiredService<IBuildsAndStartsWorkflow>();

            // Run the workflow.
            await workflowRunner.BuildAndStartWorkflowAsync<HelloWorld>();
        }
    }
    
    /// <summary>
    /// A basic workflow with just one WriteLine activity.
    /// </summary>
    public class HelloWorld : IWorkflow
    {
        public void Build(IWorkflowBuilder builder) => builder.WriteLine("Hello World!");
    }
}
```

When you run the above program, you will see the following output:

```bash
Hello World!
```

See the [Guides](guides-hello-world-console.md) section for more examples.