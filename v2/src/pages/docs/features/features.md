---
id: features
title: Features
sidebar_label: Features
---

Elsa Workflows is packed with features that help you implement workflow-driven, lo-code applications with ease.

## Visual Workflow Designer

Elsa comes with a standalone, reusable **HTML5 workflow designer** web component that you can embed in your own HTML web pages.
The designer operates completely client-side, and has a rich JavaScript API that allows you to define custom plugins in order to extend its functionality.

## Elsa Dashboard

The Elsa Dashboard enables you to define workflow definitions and inspect executed workflows. 

## Short and Long Running Workflows

Elsa is designed to efficiently execute both short-running and long-running workflows.

Short-running workflows are useful when you need to implement business rules engines for example, where you can invoke the workflow from your application is if it were a function that receives input and returns a result.

Long-running workflows enable you to implement complicated process that involve humans and machines with ease.
A typical example of a long-running process is a document approval workflow, where multiple parties are involved in the review process of some document. Such a workflow might involve timers, emails, reminders, HTTP requests, user actions, and much more.

## Correlation

In order to support long-running workflows, **correlation** is a crucial feature. Correlation allows you to associate a workflow instance with an identifier of your choosing. For example, when implementing a document approval workflow, you might correlate the workflow with your document based on its ID.

## Activities

An Activity represents a single step in the workflow. 
Out of the box, the Elsa suite of NuGet packages provides you with a decent set of activities to start with:

## Versioning

Each workflow definition is versioned. When you publish a new version of your workflow, its version number is incremented. Existing workflow instances will still use the previous versions of your workflow definition, but new workflows will use the latest one.

## Persistence

Elsa ships with the following persistence providers:

* InMemory (specifically useful for unit testing and short-running workflows that complete in a single run)
* Entity Framework Core
* MongoDB
* YesSQL

## Workflow Expressions

Most activities expose properties that can be set to an **expression**, which are evaluated at runtime.
Expressions allow you to reference values produced by other activities for example.
Elsa ships with three expression evaluators:

### Literal

The **Literal** evaluator is not really an interpreter, and is used only when you need to set a value on an activity property that requires no runtime evaluation.

### JavaScript

The **JavaScript** evaluator is typically used when you need to compute some value or read a value from the workflow.

### Liquid

The **Liquid** evaluator is typically used when you need to create an HTTP request, HTTP response, or when sending an email for example where the body is marked up using liquid.

## Extensibility

One of the most important and powerful features of Elsa is its extensibility.

### Activities

Many process are domain specific, and being able to create workflows using a set of activities that represent the domain's ubiquitous language is a powerful feature.

Extending Elsa with domain-specific activities could not be easier. Simply implement a C# class that inherits from `Activity`, register it with the service container, and you're good to go.
Activities become automatically available in the workflow designer, and all of its public properties are editable by default.

###  Persistence

Elsa ships with a number of persistence providers, such as Memory, Entity Framework Core, MongoDB, YesSQL and CosmosDB.
Although these providers should cover most common needs, you are certainly not limited to them. Implement `IWorkflowDefinitionStore` to provide a custom store for workflow definitions, and implement `IWorkflowInstanceStore` to provide a custom store for workflow instances.
Note that you can mix and match, which means that you can for example use the `EntityFrameworkCoreWorkflowDefinitionStore` for workflow definitions and ``.    

### JavaScript Functions

The JavaScript evaluator ships with a handful of useful JavaScript functions that you're likely to use often. For example, it allows you to read variables from the workflow and reference activity output values.
However, if you ever find yourself in need of additional functions, it's easy to extend the set of functions from your own application.

### Liquid Filters

The Liquid evaluator too ships with a handful of useful filters that you're likely to use often. For example, it allows you to read variables from the workflow and reference activity output values.
However, if you ever find yourself in need of additional functions, it's easy to extend the set of filters from your own application.

### Expression Evaluators

And maybe JavaScript and Liquid isn't good enough for you, and you want to write expressions in C#, VBScript or Python. No problem! Just implement your own version of `IExpressionEvaluator`, register it with the service container, and now you can use your custom evaluator syntax from any activity.