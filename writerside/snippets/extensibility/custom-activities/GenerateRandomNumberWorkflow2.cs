using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;

public class GenerateRandomNumberWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
            {
                new GenerateRandomNumber
                {
                    Name = "GenerateRandomNumber1"
                },
                new PrintMessage
                {
                    Message = new(context => $"The random number is: {context.GetOutput("GenerateRandomNumber1", "Result")}")
                }
            }
        };
    }
}