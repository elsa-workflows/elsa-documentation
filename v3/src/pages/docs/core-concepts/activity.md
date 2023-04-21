---
title: Activity
description: A description of activities.
---

An activity is a unit of work that can be executed by a workflow. Activities can be chained together to form a workflow.
In Elsa, activities are represented by classes that implement the `IActivity` interface.

## Activity creation

The following is an example of creating a custom activity:

```clike
public class HelloWorld : Activity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello World!");
        await CompleteAsync();
    }
}
```

In the above example, the `HelloWorld` activity is a simple activity that writes "Hello World!" to the console and then completes.

Instead of inheriting from the `Activity` class, you can also inherit from the `CodeActivity` class, which contains an `AutoCompleteBehavior` behavior that automatically completes the activity when the `ExecuteAsync` method completes: 

```clike
public class HelloWorld : CodeActivity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello World!");
    }
}
```

---

## Activity metadata

Activities can be annotated with metadata using the `ActivityAttribute` attribute. The following is an example of annotating an activity with metadata:

```clike
[Activity(
    Namespace = "MyCompany",
    Category = "My Domain",
    Description = "A simple activity that writes \"Hello World!\" to the console."
)]
```

The complete code would look like this:

```clike
[Activity(
    Namespace = "MyCompany",
    Category = "My Domain",
    Description = "A simple activity that writes \"Hello World!\" to the console."
)]
public class HelloWorld : CodeActivity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello World!");
    }
}
```

---

## Composite activities

Activities can be composed of other activities. This is useful when you want to group a set of activities together and execute them as a single unit. Or perhaps your activity contains some logic that determines which activities to execute next.

For example, here is a custom activity that models a simple `if` statement:

```clike
public class If : Activity
{
    public Input<bool> Condition { get; set; } = default!;
    public IActivity? Then { get; set; }
    public IActivity? Else { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        // Get the result from the evaluated condition.
        var result = context.Get(Condition);
        
        // Determine which activity to execute next.
        var nextActivity = result ? Then : Else;

        // Schedule the next activity for execution.
        await context.ScheduleActivityAsync(nextActivity, OnChildCompleted);
    }

    private async ValueTask OnChildCompleted(ActivityExecutionContext context, ActivityExecutionContext childContext)
    {
        // Complete the activity once this child activity completes.
        await context.CompleteActivityAsync();
    }
}
```

Notice that when your activity schedules child activities, it must also provide a callback that will be invoked when the child activity completes.
This is necessary because the activity itself is not completed until all of its child activities have completed.

The following example shows how to use the `If` activity:

```clike
var workflow = new Workflow
{
    Root = new If
    {
        Condition = new(true),
        Then = new WriteLine("Hello World!"),
        Else = new WriteLine("Goodbye cruel world!")
    }
};
```

{% callout title="Workflow Root" %}
Notice that in the above example, we assigned the `If` activity to the `Root` property of the workflow. This works because the `If` activity implements the `IActivity` interface.
{% /callout %}

---

## Blocking activities

A blocking activity is an activity that creates a bookmark and waits for an external event to resume execution. This is useful when you want to wait for some external event to occur before continuing execution.
Examples of blocking activities include the `Event` activity and the `Delay` activity.

When creating a bookmark, you can provide a callback that will be invoked when the bookmark is resumed. From this callback, you can then resume execution of the activity, which may or may not complete the activity.

The following is an example of a blocking activity that waits for an external event to occur:

```clike
public class Event : Activity<object>
{
    public Input<string> Name { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        // Get the event name.
        var eventName = context.Get(Name);

        // Create a bookmark that will be resumed when the event occurs.
        context.CreateBookmark(eventName, OnEventOccurred);
        
        // This activity does not complete until the event occurs.
    }

    private async ValueTask OnEventOccurred(ActivityExecutionContext context)
    {
        // Get the event data, if any.
        var eventData = context.GetInput<object>("Payload");
        
        // Set the event data as the output of the activity.
        context.Set(Reult, eventData);

        // Complete the activity.
        await context.CompleteActivityAsync();
    }
}
```

The following is an example of using the `Event` activity:

```clike
var workflow = new Workflow
{
    Root = new Sequence
    {
        Activities =
        {
            new WriteLine("Starting workflow..."),
            new Event
            {
                Name = new("MyEvent")
            },
            new WriteLine("Event occurred!")
        }
    }
};
```

When we run this workflow, it will print "Starting workflow..." to the console and then wait for an event named "MyEvent" to occur.

```clike
var result = await workflowRunner.RunAsync(workflow);
```

The `result` variable will contain the workflow state and any created bookmarks that we might persist.

To resume the workflow, we need to provide the bookmark and the event data:

```clike 
var workflowState = result.WorkflowState;
var bookmark = workflowState.Bookmarks.Single();
var instanceId = workflowState.Id;

var input = new Dictionary<string, object>
{
    ["Payload"] = "Event occurred!"
}

var runOptions = new RunWorkflowOptions
{
    WorkflowInstanceId = instanceId,
    BookmarkId = bookmark.Id,
    Input = input
};

await workflowRunner.ResumeAsync(bookmark, options);
```

When the workflow resumes, it will print "Event occurred!" to the console and then complete.