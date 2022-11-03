---
title: Primitives
---

The following activities belong to the *primitives* category:

## Set Context ID

Sets the workflow instance's `ContextId` property using the specified value.

This property works in conjunction with the configured Workflow Context of the workflow definition.
When set, Elsa will try and load the workflow context using a custom workflow context provider defined by the application with  the specified context ID.

A context ID typically represents a unique identifier associated with a custom domain model.

More details about workflow context can be found [here](concepts/concepts-workflow-context.md).

#### Properties

| Name         	| Description                         	|
|------------	|-------------------------------------	|
| Context Id 	| The value to set as the Context ID. 	|

## Set Name

Sets the `Name` property of the workflow instance to the specified value.
Setting a workflow name can be helpful when trying to identify workflow instances that are associated with domain models.

For example, if you have a workflow that handles a customer, you might set the workflow instance's `Name` property to the customer's name.
You can then search for workflow instances associated with this name.

#### Properties

| Name         	| Description                         	                |
|------------	|-------------------------------------	                |
| Value      	| The value to set as the workflow instance's name. 	|

## Set Transient Variable

Sets a workflow variable that is stored **transiently**. This means that the variable will only exist for the duration of the current [burst of execution](concepts/concepts-workflows.md#burst-of-execution).

This is in contrast to the [Set Variable](#set-variable) activity, which will store the variable as part of the workflow instance.

#### Properties

| Property      	| Description                         	|
|---------------	|-------------------------------------	|
| Variable Name 	| The name of the variable to set.    	|
| Value         	| The value to store in the variable. 	|

## Set Variable

Sets a workflow variable that is stored **persistently** as part of the workflow instance.

This is in contrast to the [Set Transient Variable](#set-transient-variable) activity, which will store the variable **transiently**.

#### Properties

| Property      	| Description                         	|
|---------------	|-------------------------------------	|
| Variable Name 	| The name of the variable to set.    	|
| Value         	| The value to store in the variable. 	|