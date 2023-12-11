---
title: Introduction
pageTitle: Unleash the Power of Workflows in Your .NET Projects with Elsa.
description: Discover the potential of your .NET applications with Elsa Workflows. Create, modify, and manage workflows with ease, both programmatically and visually.
---

Elsa Workflows is a powerful and flexible **execution** engine, encapsulated as a set of open-source .NET libraries designed to infuse .NET applications with workflow capabilities.
With Elsa, developers can weave **logic** directly into their **systems**, enhancing functionality and **automation** and align seamlessly with the application’s core functionality.

Workflows in Elsa can be defined in two ways:

- **Programmatically**: By writing .NET code, developers can define complex workflows tailored to specific business needs.
- **Visually**: Using the built-in designer, non-developers or those who prefer a visual approach can create and modify workflows with ease.

Elsa's versatility makes it suitable for a wide range of applications, from simple task automation to complex business process management. 
Its modular architecture allows for easy integration with existing systems, making it a top choice for developers looking to add workflow functionality to their projects.

---

## Getting Started

Embark on your Elsa Workflows journey! Whether you're new to workflows or an experienced developer, we've got the resources to help you integrate Elsa seamlessly into your project.

{% quick-links %}

{% quick-link title="Docker" icon="docker" href="/docs/installation/docker" description="Get up and running with Elsa in no time using our Docker image." /%}

{% quick-link title="Known Issues and Limitations" icon="warning" href="/docs/getting-started/limitations" description="Learn about the known issues and limitations of Elsa." /%}

{% quick-link title="Installation" icon="installation" href="/docs/installation/packages" description="Begin with our step-by-step guides to set up your system and install the necessary libraries." /%}

{% quick-link title="Activity Library" icon="presets" href="/docs/activity-library/branching" description="Dive into the diverse activity library and discover the building blocks of your workflows." /%}

{% quick-link title="Concepts" icon="plugins" href="/docs/core-concepts/programmatic-vs-designer" description="Grasp the foundational concepts behind Elsa and understand its powerful capabilities." /%}

{% quick-link title="Guides" icon="theming" href="/docs/guides/http-workflows" description="Roll up your sleeves and learn through hands-on guides and tutorials." /%}

{% /quick-links %}


---


### Overview of Elsa Workflows

Elsa Workflows is not just another workflow engine; it's a robust and flexible solution designed to breathe life into your .NET applications. With Elsa, you can model both simple and complex business processes, ensuring that your applications are not only functional but also efficient and user-friendly.

#### **Why Elsa?**
- **Flexibility**: Whether you're looking to automate a simple task or design a complex, multi-step business process, Elsa's got you covered. Its modular design ensures that you can tailor it to fit your exact needs.
  
- **Visual & Programmatic Design**: With Elsa, you don't have to choose between coding and designing. You can define workflows programmatically for precision or use the intuitive visual designer for rapid development and collaboration.
  
- **Integration Ready**: Elsa is designed to play well with others. Whether you're integrating with third-party services or your existing systems, Elsa makes it a breeze.
  
- **Open Source**: Being open source, Elsa benefits from a community of contributors who continuously enhance its capabilities. You get a tool that's not only powerful but also up-to-date with the latest industry standards.

#### **Key Features**
- **Activity Library**: A rich set of out-of-the-box activities that serve as the building blocks for your workflows.
  
- **Triggers**: Kick off workflows automatically based on specific events or conditions.
  
- **Long & Short Running Workflows**: Whether you need a workflow that runs over days, waiting for user input, or one that completes in milliseconds, Elsa can handle it.
  
- **Extensibility**: Need something unique? Elsa is designed to be extensible, allowing you to add custom activities or integrate with other systems seamlessly.

- **Dynamic Expressions**: Evaluate values dynamically during runtime using C#, JavaScript or Liquid expressions.

#### **Potential Use Cases**
- **Business Process Automation**: Streamline business processes like order processing, HR onboarding, or content approval.
  
- **Task Automation**: Automate repetitive tasks, from data entry to report generation.
  
- **Integration Workflows**: Connect disparate systems and ensure smooth data flow between them.
  
- **Alerts & Monitoring**: Set up workflows to monitor systems and send alerts or take corrective actions automatically.

Dive deeper into the documentation to discover all that Elsa Workflows has to offer and start your journey towards creating dynamic, workflow-driven applications!

---

### Benefits of using Elsa Workflows

Elsa Workflows brings a plethora of advantages to the table, making it a top choice for developers and businesses alike. Here's why you should consider Elsa for your workflow needs:

- **Rapid Development**: With Elsa's visual designer and extensive activity library, you can design and deploy workflows in record time, accelerating your development cycle.

- **Scalability**: Built on .NET, Elsa is inherently scalable, ensuring that as your business grows, your workflows can handle the increased load with ease.

- **Cost-Efficient**: Automating processes with Elsa can lead to significant cost savings by reducing manual effort, minimizing errors, and optimizing resource usage.

- **Flexibility**: Elsa's dual approach of programmatic and visual design means you can craft workflows that fit your exact requirements, no matter how intricate.

- **Open-Source Advantage**: Being open-source, Elsa is continuously improved by a community of developers. This ensures you always have access to the latest features and best practices.

- **Seamless Integration**: Elsa is designed to integrate smoothly with other systems, be it databases, third-party services, or custom applications, ensuring a cohesive ecosystem.

- **Enhanced User Experience**: Automated and optimized processes lead to faster response times, consistent outcomes, and a better overall user experience.

- **Robust Security**: With built-in features for authentication and authorization, Elsa ensures that your workflows and data remain secure.

- **Transparent Monitoring & Logging**: Keep an eye on your workflows with Elsa's monitoring capabilities. Track progress, diagnose issues, and ensure everything runs smoothly.

- **Community & Support**: Join a thriving community of Elsa enthusiasts. Whether you have a question, need guidance, or want to share your expertise, the Elsa community is there for you.

Harness the power of Elsa Workflows and elevate your applications to new heights of efficiency and user satisfaction!

---

### Dive into Elsa's Programmatic API

Elsa Workflows offers a programmatic API that draws inspiration from the Windows Workflows Foundation 4. This API provides developers with a powerful toolset to define workflows with precision and flexibility.

#### **Sequential Workflows**

Sequential workflows allow you to define a series of activities that execute in a specific order. Here's a simple example that prompts the user for their name and then displays it:

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

![Output](./sample-workflow-console.gif)

#### Flowchart Workflows

For more complex workflows, Elsa supports `Flowchart` activities. 
These allow you to define workflows as a graph of interconnected activities, offering greater flexibility:

```clike
public class BraidedWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder workflow)
    {
        var writeLine1 = new WriteLine("WriteLine1");
        var writeLine2 = new WriteLine("WriteLine2");
        var writeLine3 = new WriteLine("WriteLine3");
        var writeLine4 = new WriteLine("WriteLine4");
        var writeLine5 = new WriteLine("WriteLine5");
        var writeLine6 = new WriteLine("WriteLine6");
        var writeLine7 = new WriteLine("WriteLine7");

        workflow.Root = new Flowchart
        {
            Start = writeLine1,
            
            Activities =
            {
                writeLine1,
                writeLine2,
                writeLine3,
                writeLine4,
                writeLine5,
                writeLine6,
                writeLine7,
            },

            Connections =
            {
                new Connection(writeLine1, writeLine2),
                new Connection(writeLine1, writeLine3),

                new Connection(writeLine2, writeLine4),
                new Connection(writeLine2, writeLine5),

                new Connection(writeLine3, writeLine5),
                new Connection(writeLine3, writeLine6),

                new Connection(writeLine4, writeLine7),
                new Connection(writeLine5, writeLine7),
                new Connection(writeLine6, writeLine7),
            }
        };
    }
}
```

When visualized, the flowchart workflow appears as:

![Designer](./introduction/braided-flowchart-workflow.png)

#### Visual Designer

For those who prefer a visual approach or want to collaborate with non-developers, Elsa offers an intuitive designer:

![Designer](./sample-workflow-designer.gif)

#### Unleash the Power of Workflows in Your .NET Projects

By integrating workflows into your application, you open the door to a myriad of possibilities. 
From easily updating business logic and orchestrating microservices to handling recurring tasks, data processing, and message processing, the potential is vast. 
With Elsa, you're limited only by your imagination!

---

## What's New

Elsa 3 represents a monumental leap from its predecessor, Elsa 2. This version is not just an upgrade; it's a complete rewrite, bringing forth a plethora of improvements, new features, and architectural changes. Here's a comprehensive overview of what Elsa 3 brings to the table:

### **Core Enhancements**

- **.NET 6 Targeting**: Elsa 3 is designed for the future, targeting .NET 6 and beyond.
  
- **Architectural Clarity**: A clear distinction has been made between the core library, the management library, and the runtime library. This separation ensures enhanced flexibility and integration capabilities.
  
- **Reduced Dependencies**: With fewer dependencies, integrating Elsa into your existing applications is now more seamless than ever.

### **Designer & Programming Model**

- **Revamped Visual Designer**: Experience a state-of-the-art designer equipped with features like drag & drop, multi-select, undo, redo, copy & paste, and more.
  
- **Innovative Programming Model**: Crafting workflows from code and developing custom activities have never been easier, thanks to the new programming model.

- **Diverse Diagram Support**: Beyond the traditional Flowchart and Sequence diagrams, Elsa 3 paves the way for future support for State Machine and BPMN 2.0 diagrams.

### **Execution & Runtime**

- **Queue-Based Scheduler**: Elsa 3 adopts a queue-based workflow scheduler, transitioning from the stack-based approach of previous versions. This change promotes breadth-first execution.
  
- **Parallel Activity Execution**: Activities can now run concurrently using the `Task` and `Job` activity kinds.
  
- **Versatile Workflow Runtimes**: While the default runtime is database-based, Elsa 3 introduces support for a distributed runtime using Proto.Actor, enabling lock-free workflow execution across multiple nodes.

- **Middleware Pipeline**: A middleware pipeline architecture has been implemented for both workflow and activity execution.

### **C# Expressions**

- **C# Expressions**: Elsa 3 introduces support for C# expressions, allowing you to evaluate values dynamically during runtime.

### **Persistence & Security**

- **Flexible Persistence**: The simplified persistence abstraction in Elsa 3 allows you to choose from a range of persistence technologies. Whether it's SQL Server, MongoDB, or Elasticsearch, Elsa 3 has got you covered.
  
- **Secured API Endpoints**: By default, API endpoints are now secured. You have the flexibility to configure authentication using JWT tokens, API keys, or even opt for no authentication.

- **Non-Persistent Activity Data**: Activity input & output are non-persistent by default, ensuring data security. However, if persistence is required, workflow variables can be used to capture and store the data.

### **Workflow Context**

- **Enhanced Workflow Context**: Elsa 3 introduces versatile Workflow Context support, allowing the configuration of multiple workflow context providers for each workflow.

---

## Features

Elsa Workflows is a robust platform designed to simplify the creation and management of workflow-driven applications. Here's a comprehensive overview of its feature set:

### **Workflow Creation**

- **Programmatic Workflows**: Craft workflows programmatically using code. This approach offers strong typing and the ability to reuse workflows across different scenarios.

- **Visual Designer**: Use Elsa's intuitive visual designer to create workflows. This graphical approach is perfect for those who prefer a visual representation and offers the same reusability benefits.

### **Workflow Execution Types**

- **Short-Running Workflows**: These workflows execute seamlessly from start to finish without any suspension. They're ideal for tasks like sending emails or executing a series of steps.

- **Long-Running Workflows**: Designed for tasks that require suspension and resumption. Examples include workflows that await external events or those that orchestrate lengthy tasks.

### **Activity Management**

- **Composite Activities**: Activities like `Sequence` and `Flowchart` that can encapsulate other activities. You can design your own composite activities either programmatically or using the visual designer. These can be reused across different workflows.

- **Triggers**: Activities designed to initiate a workflow. Examples include `HttpEndpoint` and `Timer`.

- **Activities**: The fundamental building blocks of workflows. Elsa comes with a rich set of activities like `WriteLine`, `SendEmail`, `HttpRequest`, and more. The platform's extensibility allows you to add custom activities as needed.

- **Activity Providers**: These providers supply activity types to Elsa. For instance, `TypedActivityProvider` offers activity types based on `IActivity` implementations. Elsa's flexibility will soon support activity types from GraphQL, OpenAPI, JavaScript functions, and more.

### **Dynamic Expressions**

- **Expressions**: Dynamically evaluate values during runtime. Elsa natively supports JavaScript and Liquid expressions, but it's designed to accommodate custom expression evaluators.

    - **C# Example**:
      ```clike
      $"The current date and time is: {DateTime.Now}"
      ```

    - **JavaScript Example**:
      ```javascript
      `The current date and time is: ${new Date()}`
      ```

    - **Liquid Example**:
      ```liquid
      The current date and time is: {{ "now" | date: "%Y-%m-%d %H:%M:%S" }}
      ```

### **Persistence & Hosting**

- **Persistence**: Elsa allows workflows to be saved to a database, enabling their resumption even after application restarts. The persistence mechanism is abstract, allowing for custom persistence providers.

- **Workflow Hosting**: Integrate and execute workflows directly within your application, offering a seamless user experience.

### **Integration Capabilities**

- **External Application Integration**: Elsa workflows can be triggered from any external application capable of making HTTP requests. Conversely, Elsa workflows can interact with external applications through HTTP requests, webhooks, service bus messages, gRPC, and more.


## Join the community!

There's a friendly community around Elsa, and you're invited!

- [Github](https://github.com/elsa-workflows/elsa-core)
- [Discord](https://discord.com/invite/hhChk5H472)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/elsa-workflows)

