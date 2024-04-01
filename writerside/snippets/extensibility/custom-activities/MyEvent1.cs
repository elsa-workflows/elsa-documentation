using Elsa.Workflows;

public class MyEvent : Activity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        context.CreateBookmark("MyEvent");
    }
}