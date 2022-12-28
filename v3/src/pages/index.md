---
title: Getting started
pageTitle: Add workflow capabilities to any .NET project.
description: Build workflow-driven applications using Elsa Workflows.
---

Learn how to get Elsa Workflows set up in your project.

{% quick-links %}

{% quick-link title="Installation" icon="installation" href="/" description="Step-by-step guides to setting up your system and installing the library." /%}

{% quick-link title="Architecture guide" icon="presets" href="/" description="Learn how the internals work and contribute." /%}

{% quick-link title="Modules" icon="plugins" href="/" description="Extend the library with third-party modules or write your own." /%}

{% quick-link title="API reference" icon="theming" href="/" description="Learn to easily customize and extend your workflow application." /%}

{% /quick-links %}

---

## Introduction

Elsa Workflows is a set of open source .NET libraries that add workflow capabilities to any .NET application.
Workflows can be created either from code or using the designer.

The programmatic API to create workflows is loosely inspired on that of Windows Workflows Foundation 4, where you can easily define sequential workflows.
For example, the following workflow prompts the user for their name and prints it out to the console:

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

![Output](./sample-workflow-console.gif)

Alternatively, you can create workflows using the designer, which stores the workflow definition on the server via REST API endpoints:

![Designer](./sample-workflow-designer.gif)

Workflows can be run from within your own application that references the Elsa packages, but they can also be invoked externally using a simple to use REST API that you can optionally expose from your app.

This makes it easy to have, for example, a separate workflow server and backend service, conductor style.

Adding workflows to your application unlocks powerful capabilities, such as easy to update business logic, microservice orchestration, recurring tasks, data processing, message processing, and more.

## Join the community!

There's a friendly community around Elsa, and you're invited!

- [Github](https://github.com/elsa-workflows/elsa-core)
- [Discord](https://discord.com/invite/hhChk5H472)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/elsa-workflows)

