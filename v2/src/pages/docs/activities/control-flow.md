---
title: Control Flow
---

The following activities belong to the *control flow* category: 

## Break

Breaks the current iteration of a given [For](#for), [For Each](#for-each) or [While](#while) activity.

For example, if you have a [While](#while) activity with a `Condition` value of `true` (representing an infinite loop), you can break out of this loop using the **Break** activity.

#### Properties

This activity has no additional properties other than the [shared set of activity properties](activities/common-properties.md). 

## Finish

Signals the workflow runner that the **current** composite activity should be stopped. 

Since **workflows are composite activities**, this means that when this activity is used within a workflow, the workflow instance will enter the **Finished** state.
When used in a child composite activity, that activity will stop execution and yield back control to its container. However, **it will not stop workflow execution itself**.

The activity also removes any blocking activities within the scope of the currently executing composite activity **including any child scopes**.

For example, if you have a workflow with one or more custom composite activities and any of them are blocked on some activity, they will all be removed and the workflow will complete.
But if you have a custom composite activity that executes a **Finish** activity, only blocking activities within its immediate and descendant scopes will be removed.

Furthermore, the **Finish** activity provides the means to **send back output** from a custom composite activity **to its parent**.

To see how this works, check out the [Parent/Child Workflows](guides/guides-parent-child-workflows.md) guide.

It also allows you to schedule any additional outcomes other than **Done** on the parent composite activity (which is useful only from custom composite activities since workflows are root composite activities).

| Property        	| Description                                                                                                      	|
|-----------------	|------------------------------------------------------------------------------------------------------------------	|
| Activity Output 	| The object to return as output from the currently executing composite activity to the parent composite activity. 	|
| Outcome Names   	| Zero or more outcomes to schedule on the parent composite activity.                                              	|

| Outcome 	| Description                                                                 	|
|---------	|-----------------------------------------------------------------------------	|
| Done    	| Always scheduled when the activity completed.                               	|
| *       	| Any additional outcomes that are configured in the `Outcome Names` property 	|

## For

Models a looping construct that is similar to C#'s `for` statement where the loop executes from a starting number to a final number using a given step value.

| Property 	| Description                                                                 	|
|----------	|-----------------------------------------------------------------------------	|
| Start    	| The number to start from when performing the loop.                          	|
| End      	| The number to stop at when performing the loop.                             	|
| Step     	| The number to use when incrementing the current value.                      	|
| Operator 	| The operator to use when comparing the current value against the End value. 	|

| Outcome 	| Description                                                                                                                                              	|
|---------	|--------------------------------------------------------------------------------------------------------------------------------------------------------	|
| Iterate 	| Scheduled for each iteration of the loop. When an iteration ends, the activity will automatically determine if another iteration should be scheduled. 	|
| Done    	| Scheduled when the loop has finished.                                                                                                                    	|

## For Each

Models a looping construct that is similar to C#'s `foreach` statement where the loop iterates over a collection of objects.

> There's currently a limitation that prevents the use of the `Literal` and `Liquid` syntaxes when providing a value. Therefore, when using the designer, make sure to use the `Json` or `JavaScript` syntax.

When providing an expression using `Json` or `JavaScript`, make sure that the expression evaluates to an `array`.

| Property 	| Description                              	|
|----------	|------------------------------------------	|
| Items    	| A collection of objects to iterate over. 	|

| Outcome 	| Description                                                                                                                                              	|
|---------	|-------------------------------------------------------------------------------------------------------------------------------------------------------    |
| Iterate 	| Scheduled for each iteration of the loop. When an iteration ends, the activity will automatically determine if another iteration should be scheduled. 	|
| Done    	| Scheduled when the loop has finished.                                                                                                                     |