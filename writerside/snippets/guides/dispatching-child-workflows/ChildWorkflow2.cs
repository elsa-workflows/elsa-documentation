using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Models;
using JetBrains.Annotations;

namespace ElsaServer.Workflows;

[UsedImplicitly]
public class ChildWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Child Workflow";
        builder.Inputs.Add(new InputDefinition
        {
            Name = "Message",
            DisplayName = "Message",
            Description = "The message to write to the console.",
            Type = typeof(string)
        });
        builder.Root = new WriteLine("Hello from Child");
    }
}