using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;

[FlowNode("Pass", "Fail")]
public class PerformTask : Activity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        await context.CompleteActivityWithOutcomesAsync("Pass");
    }
}