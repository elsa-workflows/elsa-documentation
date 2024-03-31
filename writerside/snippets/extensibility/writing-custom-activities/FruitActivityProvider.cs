using Elsa.Extensions;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Models;

namespace Elsa.Server.Web.Activities;

public class FruitActivityProvider(IActivityFactory activityFactory) : IActivityProvider
{
    public ValueTask<IEnumerable<ActivityDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default)
    {
        var fruits = new[]
        {
            "Apples", "Bananas", "Cherries",
        };

        var activities = fruits.Select(x =>
        {
            var fullTypeName = $"Demo.Buy{x}";
            return new ActivityDescriptor
            {
                TypeName = fullTypeName,
                Name = $"Buy{x}",
                Namespace = "Demo",
                DisplayName = $"Buy {x}",
                Category = "Fruits",
                Description = $"Buy {x} from the store.",
                Constructor = context =>
                {
                    var activity = activityFactory.Create<PrintMessage>(context);

                    activity.Message = new($"Buying {x}...");
                    activity.Type = fullTypeName;
                    return activity;
                }
            };
        }).ToList();

        return new(activities);
    }
}