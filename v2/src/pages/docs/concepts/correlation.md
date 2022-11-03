---
title: Correlation
---

Elsa enables you to _correlate_ workflows with a application-specific values. These values can be anything, but typically represent an identifier of an entity in your domain.

For example, it could represent a customer, a document, a change request, and anything else.

## When to use correlation?

Imagine you have a document approval workflow that requests reviews of a document from users in the system.
Such workflows are typically long-running: they will suspend while awaiting input from a user.

When a user provides input, the application needs to know which workflow instance to send the input to.
That's where correlation comes in: the system sends input **as well as the document ID**.

See the [Correlation Guide](guides/correlation.md) to see correlation in action.