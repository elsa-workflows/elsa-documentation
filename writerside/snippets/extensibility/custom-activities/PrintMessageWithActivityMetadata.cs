using Elsa.Workflows;
using Elsa.Workflows.Attributes;

[Activity("MyCompany", "Print a message to the console")]
public class PrintMessage : CodeActivity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello world!");
    }
}