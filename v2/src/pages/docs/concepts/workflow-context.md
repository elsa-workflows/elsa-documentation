---
title: Workflow Context
---

Every workflow can have a specific **Workflow Context** around which the workflow operates.
A workflow context is always application specific, and can be anything you want.
Examples of workflow contexts are documents, time sheets, users, employees, products, shopping carts, leave requests, change requests, and so forth.

These type of entities are typically stored in your own database to which your workflow application has direct access or perhaps indirectly via some API endpoint.

When your workflow executes, you can of course manually (perhaps through a custom activity) load the domain entity that the workflow should operate on.
But when you configure your workflow with a workflow context and implement the necessary **workflow context provider**, the workflow engine will leverage this provider and automatically load (and optionally persist) your business domain entity.  

This is especially useful when your workflow needs to regularly access and/or even modify values on your domain model.

## Workflow Context Provider

A class that you implement that will load (and optionally save) your business domain entity.

## Workflow Context

When the workflow engine loads your business domain entity through the configured workflow context provider, it is stored in the workflow's `WorkflowContext` property.
This property is always available to you from the **Activity Execution Context**, which means it is accessible from custom activities as well as workflow expressions.

## Additional Resources

To see how to use workflow contexts in practice, check out the following guide: [Working with Workflow Context](guides/workflow-contexts.md)