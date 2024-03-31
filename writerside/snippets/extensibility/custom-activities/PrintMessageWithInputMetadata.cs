using Elsa.Workflows;
using Elsa.Workflows.Attributes;

[Activity("MyCompany", "Print Message")]
public class PrintMessage : CodeActivity
{
    [Input(Description = "The message to print.")]
    public string Message { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        Console.WriteLine(Message);
    }
}