using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;

public class GenerateRandomNumber : CodeActivity
{
    [Output(Description = "The generated random number.")]
    public Output<decimal> Result { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var randomNumber = Random.Shared.Next(1, 100);
        Result.Set(context, randomNumber);
    }
}