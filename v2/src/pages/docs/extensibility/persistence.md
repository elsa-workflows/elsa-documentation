---
title: Custom persistence providers
---

Elsa comes with a number of persistence providers, such as MongoDB, EntityFramework and YesSQL. But if none of these meet your need, you can implement one yourself.

## Workflow Definition and Workflow Instance Stores

When implementing a custom persistence provider, you can choose to implement a custom provider for any of the following abstractions:

- `IWorkflowDefinitionStore`
- `IWorkflowInstanceStore`
- `IWorkflowExecutionLogStore`
- `IBookmarkStore`

This allows applications to retrieve workflow definitions from file storage for example, while persisting workflow instances to a database.

Checkout [one of the existing implementations](https://github.com/elsa-workflows/elsa-core/tree/master/src/persistence) for complete examples.