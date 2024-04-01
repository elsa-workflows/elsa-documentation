using Elsa.Workflows;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Models;

public class If : Activity
{
    public Input<bool> Condition { get; set; } = default!;
    public IActivity? Then { get; set; }
    public IActivity? Else { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var result = context.Get(Condition);
        var nextActivity = result ? Then : Else;
        await context.ScheduleActivityAsync(nextActivity, OnChildCompleted);
    }

    private async ValueTask OnChildCompleted(ActivityCompletedContext context)
    {
        await context.CompleteActivityAsync();
    }
}