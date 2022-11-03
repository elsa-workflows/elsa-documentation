---
title: Getting started
pageTitle: Elsa Workflows - Add workflow capabilities to any .NET project.
description: Build workflow-driven applications using Elsa Workflows.
---

Learn how to get Elsa Workflows set up in your project in under ten minutes or it's free. {% .lead %}

{% quick-links %}

{% quick-link title="Quickstart" icon="installation" href="/docs/quickstarts/hello-world-console" description="Follow the quickstarts to get started in no time." /%}

{% quick-link title="Installation" icon="presets" href="/docs/installation/package-feeds" description="Setup preview feeds and install packages." /%}

{% quick-link title="Concepts" icon="plugins" href="/docs/concepts/workflows" description="Learn about fundamental Elsa concepts." /%}

{% quick-link title="Guides" icon="theming" href="/docs/guides/recurring-tasks" description="Follow the guides to learn more about how everything works together." /%}

{% /quick-links %}

---

## Craft workflows using C#

Workflows can be defined using plain C# code. In addition to an increasing number of activities that you can choose from, Elsa is designed to be extensible with your own custom activities.

```clike
public class HelloWorldWorkflow : IWorkflow
{
    public void Build(IWorkflowBuilder builder)
    {
        builder
            .StartWith<HelloWorld>()
            .Then<GoodByeWorld>();
    }
}
```

Short-running Workflows can be useful to implement a business rules engine, while long-running workflows greatly simplify the implementation of complex processes that involve coordinating between multiple agents (users & machines).

[Learn how](/docs/quickstarts/hello-world-console)

---

## Design workflows using the Workflow Designer

The Workflow Designer is a 100% client-side web component that can be re-used in any application, and allows you to easily design workflows. Workflows can be exported as JSON files, which can then be executed using the Elsa Core API.

[Learn how](/docs/quickstarts/elsa-dashboard-and-server)

---

## Manage Workflows using the dashboard

Workflows can be defined using plain C# code. In addition to an increasing number of activities that you can choose from, Elsa is designed to be extensible with your own custom activities.

[Learn how](/docs/quickstarts/elsa-dashboard-and-server)