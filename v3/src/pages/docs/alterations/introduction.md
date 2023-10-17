---
title: Introduction to Alterations
description: An introduction to alterations.
---

An [alteration](../core-concepts/alteration) represents a change that can be applied to a
given [workflow instance](../core-concepts/workflow#workflow-instance).

Using alterations, you can modify a workflow instance's state, schedule activities, and more.

## Alteration Types

Elsa Workflows supports the following alteration types:

- ModifyVariable: Modifies a variable.
- Migrate: Migrates a workflow instance to a new version.
- ScheduleActivity: Schedules an activity to be executed.
- CancelActivity: Cancels an activity. For example, Delay, Event, MessageReceived, etc.