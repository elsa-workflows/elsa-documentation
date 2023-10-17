---
title: Introduction to Incidents
description: An introduction to incidents.
---

An [incident](../core-concepts/incident) is a record of an error that occurred in the workflow.

Errors occur when an unhandled exception is thrown when an activity executes.
The workflow runtime catches the exception and creates an incident record.
The incident record is stored in the `Incidents` collection of the `WorkflowExecutionContext`, which is ultimately persisted as a `WorkflowInstance` record in the database.
