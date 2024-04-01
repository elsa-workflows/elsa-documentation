public class MyEventWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
       {
           Activities =
           {
               new MyEvent
               {
                    CanStartWorkflow = true // Enable this activity to start this workflow when triggered.
               },
               new WriteLine("Event occurred!")
           }
       };
    }
}