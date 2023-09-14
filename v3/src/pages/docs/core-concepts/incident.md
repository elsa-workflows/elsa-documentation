---
title: Incident
description: A description of incidents.
---

An incident is a record of an error that occurred in the workflow. It contains the following information:

* **ActivityId**: The activity id of the activity that caused the incident.
* **ActivityType**: The type of the activity that caused the incident.
* **Exception**: The exception that caused the incident.
* **Message**: The error message.
* **Timestamp**: The time the incident occurred.

Incidents are stored in the `Incidents` collection of the `WorkflowExecutionContext`, which is ultimately persisted as a `WorkflowInstance` record in the database.

## More information

For more information, see the [Incidents](/docs/incidents/introduction) documentation.