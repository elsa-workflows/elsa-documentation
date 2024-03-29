---
title: Outcome
description: About outcome.
---

An **outcome** represents the result of an [activity](../core-concepts/activity) or [workflow](../core-concepts/workflow) execution.
The key difference between an outcome and an [output](../core-concepts/output) is that an outcome is a discrete string value that represents the result of an activity or workflow execution, whereas an output is a data value that is produced by an activity or workflow.

Outcomes are used to determine the next activity to execute in a flowchart.
An example of an outcome is `Done`, which is used to indicate that an activity has completed successfully.
Another example is `Faulted`, which is used to indicate that an activity has failed.
Many activities have multiple outcomes, such as `Done` and `Faulted`, or `True` and `False` in the case of the Decision activity.

## When to use Outcomes and when to use Outputs

As mentioned above, activities generate **outputs** and **outcomes**, each serving distinct purposes.

**Outcomes** are particularly valuable in flowcharts, where they enable the linking of an activity to a specific result without delving into the underlying details that led to this result. A prime example is the "Decision" activity, which yields either a True or False outcome. These are represented as distinct ports on the activity within a flowchart, simplifying the visualization of decision paths.

On the other hand, **outputs** are crucial when subsequent activities require the data generated by a preceding activity. For instance, the "LoadFile" activity produces a file stream as its output. This output can then be either stored in a variable or directly utilized by other activities, facilitating data manipulation and transfer.

The decision of whether to use outputs or outcomes hinges on the specific use cases:

- If the goal is to facilitate the connection of activities in a flowchart to specific outcomes, then it's advisable to make these outcomes explicitly available from your activity.
- Conversely, if the objective is to provide data for potential use by other activities — either for further processing or to trigger certain actions based on data values — then it's more appropriate to provide this data as an output. Framing it as an outcome would necessitate simplifying the information to a basic string value, representing the outcome.

To illustrate, let's take a hypothetical "Navigate" activity, designed to execute some internal logic and produce one of four directions: "North", "East", "South", or "West". There are three ways to expose this information from the activity:

1. **As Output**: Here, the activity merely presents a "Done" outcome. To act upon the direction provided, an additional activity like "Decision" would be needed to evaluate the direction and generate a True or False result, mirroring the binary outcome structure of the Decision activity.

2. **As Outcome**: In this scenario, the activity would present four distinct outcomes: North, East, South, and West. This setup streamlines the process of linking activities based on the direction, as one can directly connect the desired activity to the appropriate outcome.

3. **Both Output and Outcome**: Offering the direction information as both an output and an outcome provides the greatest flexibility. This dual approach caters to diverse workflow requirements and allows for a more nuanced and adaptable process design.
