---
title: External application integration
description: Integration with external applications.
---

A common architecture is to have a workflow engine that is responsible for orchestrating the execution of workflows, and a separate application that is responsible for executing the tasks that make up the workflow.
These tasks can be implemented in any programming language, and can be hosted in any application.

To see how this works, we will create two applications that communicate with each other using webhooks.
The application represents an employee onboarding process, where the workflow engine is responsible for orchestrating the process, and the task executor is responsible for executing the tasks that make up the process.

These tasks will be completed by humans, but the same approach can be used to run automated tasks as well.
Ultimately, it is about communicating back to the workflow server once the task has been completed.

## RunTask activity

To request a task to be execute, we will use the `RunTask` activity.
When this activity executes, it performs two steps:

1. It publishes a domain event called `RunTaskRequest`.
2. It creates a bookmark and waits for the system to resume it.

Out of the box, Elsa does not provide any way to handle the `RunTaskRequest` domain event.
You can choose to handle this yourself, or you can use the `Elsa.Webhooks` package to handle this, and is what we will do in this example.

## Elsa.Webhooks package

The `Elsa.Webhooks` package provides a way to handle the `RunTaskRequest` domain event by firing HTTP requests to all registered webhooks matching this event.
Webhook endpoints are configured in your workflow server application and point to the external application that will execute the task.
The external application will then execute the task, and send a response back to the workflow server application once it has completed.

---

## Example

To see how this works, we will create two applications:

1. A workflow server application that hosts the workflow engine.
2. A task executor application that executes the tasks that make up the workflow.

The onboarding workflow will orchestrate the following tasks:

1. Receive the name and the email address for the new employee as input.
2. Run a task called **Create Email Account**.
3. Run a task called **Create Slack Account**.
4. Run a task called **Create GitHub Account**.
5. Run a task called **Add to HR System**.
6. Run a task called **Add to Payroll System**.
7. Send an email to the new employee to welcome them to the company. 

We will have the workflow run the first task, and once it completes, the remaining tasks will be run in parallel, since they are independent of each other.

### Workflow server

The workflow server application is a simple ASP.NET Core application that hosts the workflow engine.
It uses the `Elsa.Webhooks` package to handle the `RunTaskRequest` domain event.

To setup this application, please follow the steps in the [Elsa Server](../installation/elsa-server) guide and add the following packages:

```bash
dotnet add package Elsa.Webhooks
dotnet add package Elsa.JavaScript
dotnet add package Elsa.Email
dotnet add package Elsa.Identity
dotnet add package Elsa.EntityFrameworkCore
dotnet add package Elsa.EntityFrameworkCore.Sqlite
dotnet add package Elsa.Workflows.Api
```

Next, update the `Program.cs` file as follows:

**Program.cs**

```clike
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Webhooks.Extensions;

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

    // Enable JavaScript workflow expressions
    elsa.UseJavaScript(options => options.AllowClrAccess = true);

    // Use email activities.
    elsa.UseEmail(email =>
    {
        email.ConfigureOptions = options =>
        {
            options.Host = "localhost";
            options.Port = 2525;
        };
    });

    // Register custom webhook definitions from the application, if any.
    elsa.UseWebhooks(webhooks => webhooks.WebhookOptions = options => builder.Configuration.GetSection("Webhooks").Bind(options));
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

Update `appsettings.json` and add the following sections:

```json
{
  "Webhooks": {
    "Endpoints": [
      {
        "EventTypes": [
          "RunTask"
        ],
        "Url": "https://localhost:5002/api/webhooks/run-task"
      }
    ]
  }
}
```

This configuration tells the workflow server to send a webhook request to the task executor application whenever a `RunTaskRequest` domain event is published.

Run the workflow server application and create [the following workflow](/guides/external-app-integration/employee-onboarding.json):

![](/guides/external-app-integration/employee-onboarding.png)

The above workflow models the process as described above:

1. Receive the name and the email address for the new employee as input.
2. Run a task called **Create Email Account**.
3. Run a task called **Create Slack Account**.
4. Run a task called **Create GitHub Account**.
5. Run a task called **Add to HR System**.
6. Run a task called **Add to Payroll System**.
7. Send an email to the new employee to welcome them to the company.

All but the first and last tasks are executed by the external application.

Let's go over each activity in the workflow:

#### Set Employee from input

This is the `SetVariable` activity that sets the `Employee` variable to the value of the `Employee` input using the following JavaScript expression:

```javascript
getInput("Employee")
```

![](/guides/external-app-integration/employee-onboarding-set-variable-1.png)

Notice that this activity sets a workflow variable called `Employee`.
Make sure to create this variable first from the **Variables** tab as seen in above screenshot (1).

#### Create Email Account

This is the `RunTask` activity that requests the external application to execute the **Create Email Account** task:

![](/guides/external-app-integration/employee-onboarding-run-task-1.png)

#### Create Slack Account

This is the same as the previous activity, but for the **Create Slack Account** task, and uses the following expression to set the `Payload` input:

```javascript
return {
    employee: getEmployee(),
    description: "Create a Slack account for the new employee."
}
```

#### Create GitHub Account

This is the same as the previous activity, but for the **Create GitHub Account** task, and uses the following expression to set the `Payload` input:

```javascript
return {
    employee: getEmployee(),
    description: "Create a GitHub account for the new employee."
}
```

#### Add to HR System

This is the same as the previous activity, but for the **Add to HR System** task, and uses the following expression to set the `Payload` input:

```javascript
return {
    employee: getEmployee(),
    description: "Add the new employee to the HR system."
}
```

#### Send Welcome Email

This is the `SendEmail` activity that sends a welcome email to the new employee and contains the following settings:

##### From

```text
hr@acme.com
```

##### To

```javascript
getEmployee().Email
```

##### Subject

```javascript
`Welcome onboard, ${getEmployee().Name}!`
```

##### Body

```javascript
`Hi ${getEmployee().Name},<br><br>All of your accounts have been setup. Welcome onboard!`
```

---

### External Application

The external application is an ASP.NET Core MVC application that executes the tasks that make up the workflow.
The application contains a single view to display the list of tasks the user should complete, and a controller that handles the webhook requests from the workflow server.

#### Setup

Run the following CLI command to scaffold the new project:

```bash
dotnet new mvc -o EmployeeOnboarding.Web -f net8.0
```

Add the following NuGet packages:

```bash
cd EmployeeOnboarding.Web
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Sqlite.Design
dotnet add package Elsa.EntityFrameworkCore
```

For this application, we'll use Entity Framework Core to store the onboarding tasks in a SQLite database.
First, let's model the onboarding task entity like this:

**Entities/OnboardingTask.cs**

```clike
namespace EmployeeOnboarding.Web.Entities;

/// <summary>
/// A task that needs to be completed by the user.
/// </summary>
public class OnboardingTask
{
    /// <summary>
    /// The ID of the task.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// An external ID that can be used to reference the task.
    /// </summary>
    public string ExternalId { get; set; } = default!;

    /// <summary>
    /// The ID of the onboarding process that the task belongs to.
    /// </summary>
    public string ProcessId { get; set; } = default!;

    /// <summary>
    /// The name of the task.
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// The task description.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// The name of the employee being onboarded.
    /// </summary>
    public string EmployeeName { get; set; } = default!;

    /// <summary>
    /// The email address of the employee being onboarded.
    /// </summary>
    public string EmployeeEmail { get; set; } = default!;

    /// <summary>
    /// Whether the task has been completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// The date and time when the task was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the task was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
}
```

Next, let's create the database context:

**Data/OnboardingDbContext.cs**

```clike
using EmployeeOnboarding.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding.Web.Data;

public class OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : DbContext(options)
{
    public DbSet<OnboardingTask> Tasks { get; set; } = default!;
}
```

Finally, let's configure the database context in `Program.cs`:

**Program.cs**

```clike
builder.Services.AddDbContextFactory<OnboardingDbContext>(options => options.UseSqlite("Data Source=onboarding.db"));
```

Notice that we are using a `DbContextFactory` to create the database context. This enables us to run migrations automatically from a hosted service, which we'll create next.

#### Migrations

Run the following CLI command to generate the initial migration:

```bash
dotnet ef migrations add Initial
```

Let's create a hosted service that will run migrations automatically when the application starts:

**HostedServices/MigrationsHostedService.cs**

```clike
using EmployeeOnboarding.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding.Web.HostedServices;

public class MigrationsHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OnboardingDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

Register the hosted service in `Program.cs`:

**Program.cs**

```clike
builder.Services.AddHostedService<MigrationsHostedService>();
```

#### Task List

Now that we have our database access layer setup, let's update the Home controller to display the list of tasks that need to be completed.
For that, we will introduce a view model called `IndexViewModel` for the `Index` action of the `HomeController`:

**Views/Home/IndexViewModel.cs**

```clike
using EmployeeOnboarding.Web.Entities;

namespace EmployeeOnboarding.Web.Views.Home;

public class IndexViewModel(ICollection<OnboardingTask> tasks)
{
    public ICollection<OnboardingTask> Tasks { get; set; } = tasks;
}
```

Then update the `Index` action of the `HomeController` to use the view model:

**Controllers/HomeController.cs**

```csharp
using EmployeeOnboarding.Web.Data;
using EmployeeOnboarding.Web.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding.Web.Controllers;

public class HomeController(OnboardingDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var tasks = await dbContext.Tasks.Where(x => !x.IsCompleted).ToListAsync(cancellationToken: cancellationToken);
        var model = new IndexViewModel(tasks);
        return View(model);
    }
}
```

Finally, let's update the `Index.cshtml` view to display the list of tasks:

**Views/Home/Index.cshtml**

```html
@model EmployeeOnboarding.Web.Views.Home.IndexViewModel
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Tasks</h1>
    <p>Please complete the following tasks.</p>
</div>

<div class="container">
    <table class="table table-bordered table-hover">
        <thead class="table-light">
        <tr>
            <th scope="col">Task ID</th>
            <th scope="col">Name</th>
            <th scope="col">Description</th>
            <th scope="col">Employee</th>
            <th scope="col"></th>
        </tr>
        </thead>
        <tbody>
        @foreach (var task in Model.Tasks)
        {
            <tr>

                <th scope="row">@task.Id</th>
                <td>@task.Name</td>
                <td>@task.Description</td>
                <td>@($"{task.EmployeeName} <{task.EmployeeEmail}>")</td>
                <td>
                    <form asp-action="CompleteTask">
                        <input type="hidden" name="TaskId" value="@task.Id"/>
                        <button type="submit" class="btn btn-primary">Complete</button>
                    </form>
                </td>
            </tr>
        }
        </tbody>
    </table>
</div>
```

#### Receiving Tasks

Now that we have a way to display the list of task, let's setup a webhook controller that can receive tasks from the workflow server application.

First, let's create a new controller called `WebhookController`:

**Controllers/WebhookController.cs**

```clike
using EmployeeOnboarding.Web.Data;
using EmployeeOnboarding.Web.Entities;
using EmployeeOnboarding.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding.Web.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhookController(OnboardingDbContext dbContext) : Controller
{
    [HttpPost("run-task")]
    public async Task<IActionResult> RunTask(WebhookEvent webhookEvent)
    {
        var payload = webhookEvent.Payload;
        var taskPayload = payload.TaskPayload;
        var employee = taskPayload.Employee;
        
        var task = new OnboardingTask
        {
            ProcessId = payload.WorkflowInstanceId,
            ExternalId = payload.TaskId,
            Name = payload.TaskName,
            Description = taskPayload.Description,
            EmployeeEmail = employee.Email,
            EmployeeName = employee.Name,
            CreatedAt = DateTimeOffset.Now
        };

        await dbContext.Tasks.AddAsync(task);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}
```

The above listing uses the `WebhookEvent` model to deserialize the webhook payload. The `WebhookEvent` and related models are defined as follows:

**Models/WebhookEvent.cs**

```clike
namespace EmployeeOnboarding.Web.Models;

public record WebhookEvent(string EventType, RunTaskWebhook Payload, DateTimeOffset Timestamp);
```

**Models/RunTaskWebhook.cs**

```clike
namespace EmployeeOnboarding.Web.Models;

public record RunTaskWebhook(string WorkflowInstanceId, string TaskId, string TaskName, TaskPayload TaskPayload);
```

**Models/TaskPayload.cs**

```clike
namespace EmployeeOnboarding.Web.Models;

public record TaskPayload(Employee Employee, string Description);
```

**Models/Employee.cs**

```clike
namespace EmployeeOnboarding.Web.Models;

public record Employee(string Name, string Email);
```

#### Completing Tasks

Let's add the `CompleteTask` action to the `HomeController`. The following shows the complete `HomeController`:

**Controllers/HomeController.cs**

```clike
using EmployeeOnboarding.Web.Data;
using EmployeeOnboarding.Web.Services;
using EmployeeOnboarding.Web.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding.Web.Controllers;

public class HomeController(OnboardingDbContext dbContext, ElsaClient elsaClient) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var tasks = await dbContext.Tasks.Where(x => !x.IsCompleted).ToListAsync(cancellationToken: cancellationToken);
        var model = new IndexViewModel(tasks);
        return View(model);
    }
    
    public async Task<IActionResult> CompleteTask(int taskId, CancellationToken cancellationToken)
    {
        var task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskId);
    
        if (task == null)
            return NotFound();
    
        await elsaClient.ReportTaskCompletedAsync(task.ExternalId, cancellationToken: cancellationToken);
    
        task.IsCompleted = true;
        task.CompletedAt = DateTimeOffset.Now;
    
        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync(cancellationToken);
    
        return RedirectToAction("Index");
    }
}
```

The above listing uses the `ElsaClient` to report the task as completed, which is defined as follows:

**Services/ElsaClient.cs**

```clike
namespace EmployeeOnboarding.Web.Services;

/// <summary>
/// A client for the Elsa API.
/// </summary>
public class ElsaClient(HttpClient httpClient)
{
    /// <summary>
    /// Reports a task as completed.
    /// </summary>
    /// <param name="taskId">The ID of the task to complete.</param>
    /// <param name="result">The result of the task.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    public async Task ReportTaskCompletedAsync(string taskId, object? result = default, CancellationToken cancellationToken = default)
    {
        var url = new Uri($"tasks/{taskId}/complete", UriKind.Relative);
        var request = new { Result = result };
        await httpClient.PostAsJsonAsync(url, request, cancellationToken);
    }
}
```

The HttpClient is configured from `Program.cs` as follows:

**Program.cs**

```clike
// Configure Elsa API client.
var configuration = builder.Configuration;

builder.Services.AddHttpClient<ElsaClient>(httpClient =>
{
    var url = configuration["Elsa:ServerUrl"]!.TrimEnd('/') + '/';
    var apiKey = configuration["Elsa:ApiKey"]!;
    httpClient.BaseAddress = new Uri(url);
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", apiKey);
});
```

The `Elsa` section in `appsettings.json` is defined as follows:

**appsettings.json**

```json
{
  "Elsa": {
    "ServerUrl": "https://localhost:5001/elsa/api",
    "ApiKey": "00000000-0000-0000-0000-000000000000"
  }
}
```

Run the following CLI command to run the application:

```bash
dotnet run --urls=https://localhost:5002
```

#### Running the Workflow

Now that we have a way to display the list of tasks and to complete tasks, let's run the workflow.

Send the following request to run the workflow:

```bash
curl --location 'https://localhost:5001/elsa/api/workflow-definitions/{workflow_definition_id}/execute' \
--header 'Content-Type: application/json' \
--header 'Authorization: ApiKey 00000000-0000-0000-0000-000000000000' \
--data-raw '{
    "input": {
        "Employee": {
            "Name": "Alice Smith",
            "Email": "alice.smith@acme.com"
        }
    }
}'
``` 

Make sure to replace `{workflow_definition_id}` with the actual workflow definition ID we created earlier.

The effect of the above request is that a new task will be created in the database, which will be displayed in the web application.

![](/guides/external-app-integration/employee-onboarding-task-list-1.png)

When you click the `Complete` button, the task will be marked as completed in the database and the workflow will continue asynchronously in the other application.
when you refresh the Task list page, the task will be gone, but 4 new tasks will be created in the database:

![](/guides/external-app-integration/employee-onboarding-task-list-2.png)

Once you complete all tasks, the workflow will send the Welcome email to the employee and the workflow will be completed.

![](/guides/external-app-integration/employee-onboarding-email.png)

You can find the final source code for this guide [here](https://github.com/elsa-workflows/elsa-guides/tree/main/src/guides/external-app-integration/EmployeeOnboarding.Web).

## Conclusion

In this guide, we have seen how to integrate an external application with Elsa by using webhooks. We have seen how to create a workflow that sends tasks to an external application and how to receive tasks from the external application and report them as completed.