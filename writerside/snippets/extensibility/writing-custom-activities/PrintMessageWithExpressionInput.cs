using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Models;

public class PrintMessage : CodeActivity
{
    public Input<string> Message { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var message = Message.Get(context);
        Console.WriteLine(message);
    }
}