using Elsa.Extensions;
using Elsa.Workflows;

public class PrintMessage : Activity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello world!");
        await context.CompleteActivityAsync();
    }
}