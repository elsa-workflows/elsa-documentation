using Elsa.Workflows;
using Elsa.Workflows.Attributes;

[Activity("MyCompany", "Print Message")]
public class PrintMessage : CodeActivity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello world!");
    }
}