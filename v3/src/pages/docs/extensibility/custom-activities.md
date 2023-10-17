---
title: Custom activities
description: Writing custom activities.
---

In this chapter, we will look at how to write custom activities.

## Pre-requisites

Before you can write custom activities, you need to have a working Elsa application. If you don't have one yet, please follow the [Installation - Console apps](../installation/console-apps) guide.

## Writing custom activities

There are different ways to provide activities to the application. The easiest way is to create a class that inherits from `CodeActivity`.

Let's look at an example.

### Greeting activity

Let's say we want to create a custom activity that prints a greeting to the console.

First, create a class that inherits from `CodeActivity`:

```clike
using Elsa.Extensions;
using Elsa.Workflows.Core.Models;

namespace Demo;

public class Greeter : CodeActivity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello, world!");
    }
}
```

Next, we need to register the activity with the workflow runtime. To do so, open `Startup.cs` and add the following line to the elsa services configuration:

```clike
services.AddElsa(elsa => elsa.AddActivity<Greeter>());
```

To try out the activity, let's create a workflow that uses it. Create a new `Workflow` object as follows:

```clike
var workflow = new Workflow
{
    Root =
    {
        new Greeter()
    }
};
```

Finally, let's run the workflow:

```clike
await workflowRunner.RunAsync(workflow);
```

If you run the application, you should see the following output:

```
Hello, world!
```

### Passing data to activities

Let's extend the `Greeter` activity to accept a name as input and print a greeting to the console.

First, let's add a property called `Name` to the activity class:

```clike
public class Greeter : CodeActivity
{
    public Input<string> Name { get; set; } = default!;
    
```

Next, let's update the `Execute` method to use the `Name` property:

```clike
protected override void Execute(ActivityExecutionContext context)
{
    Console.WriteLine($"Hello, {Name.Get(context)}!");
}
```

Finally, let's update the workflow to pass a name to the activity:

```clike
var workflow = new Workflow
{
    Root =
    {
        new Greeter
        {
            Name = new("World")
        }
    }
};
```

If you run the application, you should see the following output:

```
Hello, World!
```

### Passing data from activities

Let's extend the `Greeter` activity to return a greeting as output.

First, let's change the activity base type from `CodeActivity` to `CodeActivity<string>`:

```clike
public class Greeter : CodeActivity<string>
{
```

Next, let's update the `Execute` method to return a greeting:

```clike
protected override string Execute(ActivityExecutionContext context)
{
    var name = Name.Get(context);
    var message = $"Hello, {name}!";
    context.SetResult(message);
}
```

Finally, let's update the workflow to use the output of the activity:

```clike

// Create a variable to store the greeting.
var greeting = new Variable<string>();

// Create a workflow.
var workflow = new Workflow
{
    // Add the variable to the workflow.
    Variables = { greeting },

    // Add the activity to the workflow.
    Root = new Sequence
    {
        Activities =
        {
            new Greeter
            {
                Name = new("World"),

                // Set the variable to the output of the activity.
                Result = new(greeting)
            },

            // Print the greeting to the console.
            new WriteLine(context => $"The greeting is: {greeting.Get(context)}")
        }
    }
};
```

Notice that we are using the `Result` property to set the variable to the output of the activity.
Also notice that we changes the workflow's root activity to a `Sequence` activity.
This allows us to add multiple activities to the workflow.

If you run the application, you should see the following output:

```
The greeting is: Hello, World!
```

## Workflow designer

Custom activities can be used in the workflow designer.

### Create the application
To try it out, create a new ASP.NET application by following the [Installation - Elsa Studio](../installation/elsa-studio) guide.

### Create the activity

In the application that you just created, create a new folder called `Activities` and add a new class called `Greeter` to it using the code we have written earlier:

```clike
using Elsa.Extensions;
using Elsa.Workflows.Core.Models;

namespace Demo;

public class Greeter : CodeActivity<string>
{
    public Input<string> Name { get; set; } = default!;
    
    protected override void Execute(ActivityExecutionContext context)
    {
        var name = Name.Get(context);
        var message = $"Hello, {name}!";
        context.SetResult(message);
    }
}
```

### Register the activity

Next, open `Program.cs` and add the following line to the elsa services configuration:

```clike
services
    .AddElsa(elsa => elsa
        .AddActivity<Greeter>()
        ...
```

### Create a workflow

Run the application and create a new Workflow Definition. You should see the `Greeter` activity in the toolbox.

![Activity toolbox and the Greeter activity](/extensibility/custom-activities/activity-toolbox-1.png)

To use it, we need to add it to the workflow. Drag the `Greeter` activity from the toolbox to the workflow designer.

Enter a name for the activity in the properties panel.

![Greeter activity input](/extensibility/custom-activities/greeter-input-1.png)

Next, let's add a `WriteLine` activity to the workflow. Drag the `WriteLine` activity from the toolbox to the workflow designer and connect the `Greeter` activity to it.

![WriteLine activity input](/extensibility/custom-activities/writeline-input-1.png)

In order for the `WriteLine` activity to print the greeting, we need to set its `Text` property to an expression that uses the output of the `Greeter` activity.
Before we can use the output of the `Greeter` activity, we need to capture it into a workflow variable, like we did in the console sample.

When using the designer, we can define workflow variables from the Variables panel.

![Workflow variables panel](/extensibility/custom-activities/variables-panel-1.png)

Click the `Add` button to add a new variable. Enter a name for the variable and select the `string` type.

![Workflow variables panel](/extensibility/custom-activities/variables-panel-2.png)

Next, we need to set the `Result` property of the `Greeter` activity to the variable we just created.

![Greeter activity output](/extensibility/custom-activities/greeter-output-1.png)

Finally, we can set the `Text` property of the `WriteLine` activity to an expression that uses the variable.
Let's use the following JavaScript expression:

```javascript
`The greeting is: ${getMessage()}`
```

![WriteLine activity input](/extensibility/custom-activities/writeline-input-2.png)

### Run the workflow

To run the workflow, **make sure to publish your changes** first.

Then copy the **Workflow definition ID** from the Properties panel:

![Workflow property panel](/extensibility/custom-activities/workflow-property-panel-1.png)

Finally, send the following HTTP request (using Postman, cURL, etc.):

```http
curl --location 'https://localhost:5001/elsa/api/workflow-definitions/{workflow_definition_id}/execute' \
--header 'Content-Type: application/json' \
--header 'Authorization: ApiKey {api_key}' \
--data '{
}'
```

When you look at the application's console output, you should see the following message:

```
The greeting is: Hello, World!
```