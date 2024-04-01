using Elsa.Extensions;
using Elsa.Workflows;

namespace Elsa.Server.Web.Activities;

public class MyEvent : Trigger
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        if (context.IsTriggerOfWorkflow())
        {
            await context.CompleteActivityAsync();
            return;
        }

        context.CreateBookmark("MyEvent");
    }

    protected override object GetTriggerPayload(TriggerIndexingContext context)
    {
        return "MyEvent";
    }
}