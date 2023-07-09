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

## Input and output

Similar to functions, activities can receive input and return outputs.
Access to input is provided through the `ActivityExecutionContext` object that is passed to the `ExecuteAsync` method.
Similarly, output can be set through the `ActivityExecutionContext` object.

The following is an example of an activity that receives input and returns output:

```clike
public class Sum : CodeActivity<int>
{
    public Sum(Variable<int> a, Variable<int> b, Variable<int> result)
    {
        A = new(a);
        B = new(b);
        Result = new(result);
    }

    public Input<int> A { get; set; } = default!;
    public Input<int> B { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var input1 = A.Get(context);
        var input2 = B.Get(context);
        var result = input1 + input2;
        context.SetResult(result);
    }
}
```

The above activity receives two inputs, `A` and `B`, and returns a single output, `Result`.
Notice how the inputs are accessed using the `Get` method and the output is set using the `SetResult` method.
The `SetResult` method is a convenience method that sets the inherited `Result` property from `CodeActivity<T>`.

We can now use the above activity in a workflow:

```clike
// Declare workflow variables to hold activity output.
var a = new Variable<int>();
var b = new Variable<int>();
var sum = new Variable<int>();

var workflow = new Workflow
{
    Root = new Sequence
    {
        Variables = {a, b, sum},
        Activities =
        {
            new WriteLine("Enter first value"),
            new ReadLine(a),
            new WriteLine("Enter second value"),
            new ReadLine(b),
            new Sum(a, b, sum),
            new WriteLine(context => $"The sum of {a.Get(context)} and {b.Get(context)} is {sum.Get(context)}")
        }
    }
};
```

---



## Metadata

Activities can be annotated with metadata using the `ActivityAttribute` attribute.
This metadata is used by the designer to, for example, group activities into categories and display descriptions.

The following is an example of annotating an activity with metadata:

```clike
[Activity(
    Namespace = "Demo",
    Category = "Demo",
    Description = "A simple activity that writes \"Hello World!\" to the console."
)]
```
Activity inputs can be annotated using the InputAttribute attribute.
Similar to above, this allows the designer to display "friendly names" and descriptions of an input.


 ```clike
[InputAttribute(
            AutoEvaluate = false, 
            DisplayName = "Name of recipient",
            Description = "This is a description of the Name Input")]

        public Input<string> Name { get; set; }

 [InputAttribute(
            AutoEvaluate = false,
            DisplayName = "Message to Send",
            Description = "Write your message to send")]

        public Input<string> Message { get; set; }
```
---

## Outcomes
Custom activity outcomes can be defined by annotating the class with the `FlowNodeAttribute` attribute as follows

```clike
[Activity("Demo", "Demo", "Simple activity ")]
[FlowNode("Pass", "Fail")]
```
This produces two outcomes of the activity. In this case, a "Pass" or "Fail". To produce an outcome, use the `CompleteActivityAsync` method, as follows

```clike

 protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
        {
            var outcome = 2 > 1 ? "Pass" : "Fail";

            await context.CompleteActivityAsync(new Outcomes(outcome));
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
Notice that we assigned the `If` activity to the `Root` property of the workflow. This works because the `If` activity implements the `IActivity` interface.
If you need to run multiple activities, simply install a container activity such as `Sequence` or `Flowchart` as the root activity.
{% /callout %}

---

## Blocking activities

A blocking activity is an activity that creates a bookmark and waits for an external event to resume execution. This is useful when you want to wait for some external event to occur before continuing execution.
Examples of blocking activities include the `Event` activity and the `Delay` activity.

The following is an example of a blocking activity that creates a bookmark, which we can later use to resume the activity.

```clike
public class MyEvent : Activity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        // Create a bookmark. The created bookmark will be stored in the workflow state.
        context.CreateBookmark();
        
        // This activity does not complete until the event occurs.
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
            new MyEvent(), // This will block further execution until the MyEvent's bookmark is resumed. 
            new WriteLine("Event occurred!")
        }
    }
};
```

When we run this workflow, it will print "Starting workflow..." to the console and then wait for an event named "MyEvent" to occur.

```clike
var result = await workflowRunner.RunAsync(workflow);
```

The `result` variable contains the workflow state, which in turn contains the bookmark that was created.

To resume the workflow, we need to provide the bookmark ID.
In this example, we know that the workflow only contains one bookmark, so we can simply grab the first bookmark from the workflow state.

```clike 
var workflowState = result.WorkflowState;
var bookmark = workflowState.Bookmarks.Single(); // Get the bookmark that was created by the MyEvent activity.
var options = new RunWorkflowOptions(BookmarkId: bookmark.Id);

// Resume the workflow.
await workflowRunner.RunAsync(workflow, workflowState, options);
```

When the workflow resumes, it will print "Event occurred!" to the console and then complete.

{% callout title="Bookmark Management" %}
The example above uses low-level services to manually retrieve and resume bookmarks. In practice, you would typically use a higher-level service such as `IWorkflowRuntime` to start and resume workflows when implementing custom blocking activities.
{% /callout %}

---

## Triggers

A trigger is a special type of activity that is used to start workflows in response to some external event, such as a HTTP request or a message from a message queue.

As an example, we can update the `MyEvent` activity from the previous example to be a trigger:

```clike
// Implement the ITrigger interface.
public class MyEvent : Activity, ITrigger
{
    protected override void Execute(ActivityExecutionContext context)
    {
        // Create a bookmark. The created bookmark will be stored in the workflow state.
        context.CreateBookmark();
        
        // This activity does not complete until the event occurs.
    }
    
    // Implement the ITrigger interface.
    ValueTask<IEnumerable<object>> GetTriggerPayloadsAsync(TriggerIndexingContext context)
    {
       // We need no payloads for this example.
       return new(Enumerable.Empty<object>());
    }
}
```

These triggers are a way for higher-level services such as `IWorkflowRuntime` to start workflows in response to some external event.
Under the hood, the workflow runtime calls into lower-level services to run the appropriate workflow.
