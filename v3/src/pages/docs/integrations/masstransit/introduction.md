---
title: MassTransit
description: Configuring MassTransit Integration
---

[MassTransit](https://masstransit.io/) is a free, open-source distributed application framework for .NET.
It enables applications to communicate with each other using message-based communication over a variety of transports.

Elsa provides integration with MassTransit to allow you to dispatch workflow execution requests to a queue for
asynchronous execution.
Additionally, Elsa provides a MassTransit activity that allows you to send messages to a queue from within a workflow.

---

## Installing MassTransit

To use MassTransit with Elsa, you must first install the following NuGet package:

```shell
dotnet add package Elsa.MassTransit --prerelease
```

## Configuring MassTransit

To configure MassTransit, use the `UseMassTransit` extension method on the `IModule` argument:

```clike
services.AddElsa(elsa => 
{
    elsa.UseMassTransit();
});
```

MassTransit requires a transport provider to be installed.
See [MassTransit Transports](https://masstransit-project.com/usage/transports.html) for more information.
If you do not specify a transport provider, MassTransit will use the InMemory transport provider by default.

Elsa provides two optional but convenient packages that install the required transport provider and MassTransit:

- `Elsa.MassTransit.AzureServiceBus`: Installs the Azure Service Bus transport provider and MassTransit.
- `Elsa.MassTransit.RabbitMq`: Installs the RabbitMQ transport provider and MassTransit.

### Azure Service Bus

To use the Azure Service Bus transport provider, install the `Elsa.MassTransit.AzureServiceBus` package:

```shell
dotnet add package Elsa.MassTransit.AzureServiceBus --prerelease
```

Next, configure the Azure Service Bus transport provider:

```clike
services.AddElsa(elsa => 
{
    elsa.UseMassTransit(massTransit => 
    {
        massTransit.UseAzureServiceBus(
            "connection-string", 
            serviceBusFeature => serviceBusFeature.ConfigureServiceBus = bus => 
            { 
                bus.PrefetchCount = 4;
                bus.LockDuration = TimeSpan.FromMinutes(5);
                bus.MaxConcurrentCalls = 32;
                bus.MaxDeliveryCount = 8;
                // etc.
            }
        );
    });
});
```

The `ConfigureServiceBus` delegate allows you to configure the `ServiceBusBusFactoryConfigurator` instance.

### RabbitMQ

To use the RabbitMQ transport provider, install the `Elsa.MassTransit.RabbitMq` package:

```shell
dotnet add package Elsa.MassTransit.RabbitMq --prerelease
```

Next, configure the RabbitMQ transport provider:

```clike
services.AddElsa(elsa => 
{
    elsa.UseMassTransit(massTransit => 
    {
        massTransit.UseRabbitMq(
            "amqp://guest:guest@localhost:5672/elsa", 
            rabbitMqFeature => rabbitMqFeature.ConfigureRabbitMq = bus => 
            { 
                bus.PrefetchCount = 4;
                bus.Durable = true;
                bus.AutoDelete = false;
                bus.ConcurrentMessageLimit = 32;
                // etc. 
            }
        );
    });
});
```

The `ConfigureRabbitMq` delegate allows you to configure the `RabbitMqBusFactoryConfigurator` instance.