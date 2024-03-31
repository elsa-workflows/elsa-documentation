using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;

public class GenerateRandomNumberWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var generateRandomNumber = new GenerateRandomNumber();

        builder.Root = new Sequence
        {
            Activities =
            {
                generateRandomNumber,
                new PrintMessage
                {
                    Message = new(context => $"The random number is: {generateRandomNumber.GetOutput<GenerateRandomNumber, decimal>(context, x => x.Result)}")
                }
            }
        };
    }
}