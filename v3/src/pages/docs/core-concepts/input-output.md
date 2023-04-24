---
title: Input and output
description: About input and output.
---

In Elsa, input can refer to two things:

- Input to an activity.
- Input to a workflow.

Similarly, output can refer to two things:

- Output from an activity.
- Output from a workflow.

## Activity input and output

Activities can receive input and return output.

Input is provided via an activity's <c>ActivityExecutionContext</c> object, which is passed to the <c>ExecuteAsync</c> method.

Output can be set via the <c>ActivityExecutionContext</c> object.

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
        var a = A.Get(context);
        var b = B.Get(context);
        var result = a + b;
        context.SetResul(result);
    }
```

The above activity receives two inputs, <c>A</c> and <c>B</c>, and returns a single output, <c>Result</c>.

We have seen this activity in action in the [previous chapter](./activity#input-and-output).

## Workflow input and output

Workflows can receive input and return output as well.

### Input

For example, imagine we have a workflow, and we want the workflow to echo back the input it received:

```clike
var workflow = new WriteLine(context => $"Echo: {context.GetInput<string>("Message")}!");

// Create an input dictionary.
var input = new Dictionary<string, object>
{
    ["Message"] = "Hello World!"
};

// Run the workflow and pass the input.
await workflowRunner.RunAsync(workflow, new RunWorkflowOptions(input: input));
```

The above workflow receives a single input, `Message`, and prints it to the console.

### Output

The following examples demonstrate how to return output from a workflow:

```clike
// Create a workflow that returns some output.
var workflow = new Inline(context => context.WorkflowExecutionContext.Output["Message"] = "Hello from workflow!");

// Run the workflow and hold on to its workflow state.
var result = await workflowRunner.RunAsync(workflow);

// Get the output.
var output = result.WorkflowState.Output["Message"];

Console.WriteLine($"Output: {output}");
```

The key point here is that, in order to provide output from the workflow to the calling application, we need to set the output on the `WorkflowExecutionContext` object's `Output` property.
Every activity has access to the `WorkflowExecutionContext` object via the `ActivityExecutionContext` parameter that is passed to the `ExecuteAsync` method.

## Designer

The designer provides a convenient way to define input and output for workflows via the Input/Output tab.

![Workflow input and output](/core-concepts/workflow-input-output-1.png)

To access the inputs, we can use JavaScript expressions. For example, to access the `A` and `B` input and calculate their sum, we can use the following expression:

```javascript
getA() + getB()
```

To return the output to the code that invokes the workflow, we can use the the `Set Output` activity:

![Workflow input and output](/core-concepts/workflow-input-output-2.png)

### Invoking workflows

If we publish this workflow and invoke it using the `IWorkflowRuntime` service, we can access the output via the `WorkflowExecutionContext` object's `Output` property:

```clike
var input = new Dictionary<string, object>
{
    ["A"] = 1,
    ["B"] = 2
};
var definitionId = "be1e175cce4147d0beaa13ea15f5741c";
var startWorkflowOptions = new StartWorkflowRuntimeOptions(input: input);
var result = await _workflowRuntime.StartWorkflowAsync(definitionId, startWorkflowOptions);
var sum = (int)result.WorkflowExecutionContext.Output["Sum"];
```

{% callout title="Invoking Workflows" %}
The above example shows how to invoke a workflow using the `IWorkflowRuntime` service.
You can also invoke workflows using the HTTP API. This is useful if you want to invoke workflows from another application.
For more information, see the [Invoking Workflows](../guides/invoking-workflows) chapter.
{% /callout %}