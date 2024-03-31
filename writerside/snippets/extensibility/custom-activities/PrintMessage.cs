using Elsa.Workflows;

public class PrintMessage : CodeActivity
{
    protected override void Execute(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello world!");
    }
}