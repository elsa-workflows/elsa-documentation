---
title: Programmatic workflows vs designer workflows
description: A brief overview of the differences and similarities between programmatic workflows and designer workflows.
---

At its heart, Elsa executes `IActivity` objects. These objects are created either programmatically or using the designer.
When created using the designer, the workflow definition is stored in JSON form and is used to reconstruct an actual `Workflow` object (which implements `IActivity`) at runtime. 

Let's take a look at some of the more important differences and similarities between these two approaches.

## Programmatic workflows

Programmatic workflows are created by instantiating `IActivity` objects and setting their properties. For example, the following code creates a workflow that prompts the user for their name and prints it out to the console:

```clike
// Define a workflow variable to capture the output of the ReadLine activity.
var nameVariable = new Variable<string>();

// Define a simple sequential workflow:
var workflow = new Sequence
{
    // Register the name variable.
    Variables = { nameVariable }, 
    
    // Setup the sequence of activities to run.
    Activities =
    {
        new WriteLine("Please tell me your name:"), 
        new ReadLine(nameVariable),
        new WriteLine(context => $"Nice to meet you, {nameVariable.Get(context)}!")
    }
};
```

Output:
```bash
Please tell me your name:
Elsa
Nice to meet you, Elsa!
```

Of note here is that we used the `Sequence` activity to define a sequential workflow. Besides `Sequence`, Elsa also supports `Flowchart` activities, which allow you to define a workflow as a graph of activities by declaring connections between activities.
Here is an example that reimplements the previous workflow using a `Flowchart`:

```clike
// Define a workflow variable to capture the output of the ReadLine activity.
var nameVariable = new Variable<string>();

// Define the activities to put in the flowchart:
var writeLine1 = new WriteLine("Please tell me your name:");
var writeLine2 = new ReadLine(nameVariable);
var writeLine3 = new WriteLine(context => $"Nice to meet you, {nameVariable.Get(context)}!");

// Define a flowchart workflow:
var workflow = new Flowchart
{
    // Register the name variable.
    Variables = { nameVariable }, 
    
    // Add the activities.
    Activities =
    {
        writeLine1, 
        writeLine2,
        writeLine3
    },
    
    // Setup the connections between activities.
    Connections =
    {
        new Connection(writeLine1, writeLine2),
        new Connection(writeLine2, writeLine3)
    }
};
```

Although the implementation is different, the output will be the same:

```bash
Please tell me your name:
Elsa
Nice to meet you, Elsa!
```

---

## Designer workflows

When using the designer, you define workflows by dragging and dropping activities onto the canvas and connecting them.
The underlying data model is the same as the one used for programmatic workflows, and uses the Flowchart activity.
In other words, when creating workflows using the designer, you are creating workflows whose `Root` property is set to a `Flowchart` activity.

An important difference, however, is that workflows created using the designer do (currently) not support C# lambda expressions.
Instead, we use JavaScript expressions. 

Let's see an example.

The following workflow is created using the designer:

![Designer workflow](/core-concepts/greeting-workflow-1.png)

Let's look at each activity in turn:

### Activity 1

This activity is a `WriteLine` activity. Its `Text` property is set to `Please tell me your name:`.

![Designer workflow](/core-concepts/greeting-workflow-1-activity-1.png)

### Activity 2

This activity is a `ReadLine` activity. this activity has no input, but its `Output` property is set to a workflow variable called `Name`.

![Designer workflow](/core-concepts/greeting-workflow-1-activity-2.png)

This variable is defined in the workflow's `Variables` setting:

![Designer workflow](/core-concepts/greeting-workflow-1-activity-2-variable.png)

### Activity 3

This activity is a `WriteLine` activity. Its `Text` property is set to a JavaScript expression: `Nice to meet you, ${getName()}!\`.

![Designer workflow](/core-concepts/greeting-workflow-1-activity-3.png)

{% callout title="Accessing workflow variables from JavaScript" %}
To access workflow variables from JavaScript, you can use the following naming convention: `get{nameOfVariable}()`. For example, to access the `Name` variable, you can use `getName()`.
{% /callout %}

### JSON representation

Let's export the workflow from the designer and take a look at its JSON structure (formatted and simplified for readability):

```json
{
  "name": "Nice to meet you",
  "variables": [
    {
      "id": "d6d10d5433e646b9bc02f1d05efeb584",
      "name": "Name",
      "typeName": "String"
    }
  ],
  "root": {
    "type": "Elsa.Flowchart",
    "id": "Flowchart1",
    "start": "WriteLine1",
    "activities": [
      {
        "text": {
          "typeName": "String",
          "expression": {
            "type": "Literal",
            "value": "Please tell me your name:"
          },
          "memoryReference": {
            "id": "WriteLine1:input-1"
          }
        },
        "id": "WriteLine1",
        "type": "Elsa.WriteLine"
      },
      {
        "result": {
          "typeName": "String",
          "memoryReference": {
            "id": "d6d10d5433e646b9bc02f1d05efeb584"
          }
        },
        "id": "ReadLine1",
        "type": "Elsa.ReadLine"
      },
      {
        "text": {
          "typeName": "String",
          "expression": {
            "type": "JavaScript",
            "value": "\u0060Nice to meet you, ${getName()}!\u0060"
          },
          "memoryReference": {
            "id": "WriteLine2:input-1"
          }
        },
        "id": "WriteLine2",
        "type": "Elsa.WriteLine"
      }
    ],
    "connections": [
      {
        "source": "WriteLine1",
        "target": "ReadLine1",
        "sourcePort": "Done",
        "targetPort": "In"
      },
      {
        "source": "ReadLine1",
        "target": "WriteLine2",
        "sourcePort": "Done",
        "targetPort": "In"
      }
    ]
  }
}
```

A few key points to note here:

* The `variables` property contains the workflow variables.
* The `root` property contains the workflow's root activity, which is a `Flowchart` activity.
* The `activities` property contains the activities that are part of the workflow.
* The `connections` property contains the connections between activities.
* The `memoryReference` property is used to reference workflow variables.
* The `expression` property is used to define JavaScript expressions.
* The `typeName` property is used to define the type of a property.
* The `type` property is used to define the type of an activity.
* The `id` property is used to uniquely identify an activity or connection.
* The `source` and `target` properties are used to define the source and target activities of a connection.
* The `sourcePort` and `targetPort` properties are used to define the source and target ports of a connection.

The JSON structure is very similar to the C# data model. The main difference is that we use JavaScript expressions instead of C# lambda expressions and that the designer creates a `Workflow` object that then wraps a `Flowchart` object using its `Root` property.

## Summary

In this chapter, we've looked at the core concepts of Elsa workflows. 
We've seen that workflows are made up of activities and that activities can be connected to each other. We've also seen that workflows can be defined programmatically or using the designer.

In the next chapters, we'll explore more concepts. Whenever you see a C# representation of a workflow and activity, keep in mind that the JSON representation created via the designer is very similar.
The key differences are that the designer uses JavaScript expressions instead of C# lambda expressions and that the designer creates a `Workflow` object that then wraps a `Flowchart` object using its `Root` property.