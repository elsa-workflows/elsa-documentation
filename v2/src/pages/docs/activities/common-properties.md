---
title: Common Properties
description: Common Properties
---

All activities share a common set of properties that you can set as listed below:

## Name

When you give an activity a name, you can reference its output properties from other activities using expressions.
When doing so, you need to ensure that activity name's are valid JavaScript symbols. This means no spaces, dots, hyphens, and so forth.
See [JavaScript Expressions](expressions/expressions-javascript.md#activity-output-elsa-20) and [Liquid Expressions](expressions/expressions-liquid.md#activity-output) for more details.

## Display Name

When you specify a Display Name, it will be used when displaying the activity on the designer. Providing a custom display name is oftentimes helpful to clarify the purpose of a specific activity.

For example, if you have a workflow with **Write Line** activities, you might want to give them custom display names such as **Write Current Status** if the activity writes out some status information to standard out.

## Description

Similar to [Display Name](#display-name), the **Description** property lets you provide a custom description that is displayed in the body area of an activity on the designer. This can further enhance the comprehensibility of what a given activity is doing.

Many activities display a default description. When you provide a description yourself, that one is displayed instead.

## Load Workflow Context

Allows workflow context load behavior on a per-activity level.

Enabling this option instructs the workflow runner to load the [Workflow Context](concepts/concepts-workflow-context.md) from the provider **before this activity executes**.

This is useful in scenarios where you need to ensure you have a fresh copy of the workflow context in memory before performing any operations on it.

In most cases, you do not need this option, since the workflow runner automatically loads the workflow context (when configured) into memory before executing.

## Save Workflow Context

Allows workflow context save behavior on a per-activity level.

Enabling this option instructs the workflow runner to save the [Workflow Context](concepts/concepts-workflow-context.md) back into storage using the provider **after this activity executed**.

This is useful in scenarios where you need to ensure that any changes made to the workflow context are persisted back into storage before executing any additional activities.

In most cases, you do not need this option, since the workflow runner automatically saves the workflow context (when configured) back into storage after executing.

## Save Workflow Instance

Controls if a workflow instance should be persisted into storage on a per-activity level.

This is useful in case for example when the workflow was configured to save workflow instances after each [burst of execution](concepts/concepts-workflows.md#burst-of-execution), but for certain critical areas you want to ensure the workflow gets persisted right after the activity completes execution.

## Storage

Many activities have an additional properties in the **Storage** category.
These properties are directly related to an activity's **input** and **output** properties, allowing you to control **where to persist these properties' values**.

By default, all activity property values are persisted within the workflow instance itself.

For some scenarios however, this may be an issue. For example, the **Send HTTP Request** activity might be configured to download a file, which is made available through its `OutputContent` property.
By default, the file would be stored as a base64 string value inline within the workflow instance, which means that every time the workflow instance is loaded into memory, the entire file will be loaded as well.

A better configuration is to change the workflow storage provider to **Transient** or **Blob Storage**, depending on whether or not you need access to the file later on from the workflow.

An example when **Transient** might be appropriate is when you immediately (within the current burst of execution) send the downloaded file via email as an attachment.
The **Blob Storage** provider might be appropriate if you need access to the file later one.

For most scenarios, you don't need to configure the storage provider for a given activity's property, but you can if you want to.