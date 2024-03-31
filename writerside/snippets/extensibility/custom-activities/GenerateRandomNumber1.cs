using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Models;

public class GenerateRandomNumber : CodeActivity
{
    public Output<decimal> Result { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var randomNumber = Random.Shared.Next(1, 100);
        Result.Set(context, randomNumber);
    }
}