---
id: hosting-distributed-hosting
title: Hosting Elsa on Multiple Nodes
sidebar_label: Distributed Hosting
---

Hosting Elsa in a multi-node environment is 100% supported and can significantly increase throughput and of course offers redundancy in case once node goes down. 

## Distributed Setup

To make sure Elsa operates well in such an environment, there are four aspects to configure:

1. [Service Bus Broker](#service-bus-broker)
2. [Distributed Lock Provider](#distributed-lock-provider)
3. [Distributed Cache Signal Provider](#distributed-cache-signal-provider)
4. [Distributed Temporal Services](#distributed-temporal-services)

## Service Bus Broker

Elsa uses [Rebus](https://github.com/rebus-org/Rebus) for sending messages via service bus brokers.
Out of the box, it uses a memory provider.

The memory provider is suitable for a single-node setup, but when hosting in a cluster we need to configure an actual message broker such as [RabbitMQ](https://github.com/rebus-org/Rebus.RabbitMq) or [Azure Service Bus](https://github.com/rebus-org/Rebus.AzureServiceBus).

One of the most important reasons of running multiple Elsa nodes besides redundancy is to increase throughput. The more nodes you have, the quicker workflow instruction messages (which are posted to a queue) are processed.

The following code snippet demonstrates configuring Elsa to use RabbitMQ as the broker for Rebus:

```c#
services.AddElsa(elsa => elsa.UseRabbitMq("amqp://localhost:5672");
```

> Make sure to add the `Elsa.Rebus.RabbitMq` package and import the `Elsa.Rebus.RabbitMq` namespace.

Elsa currently ships with support for RabbitMq and Azure Service Bus packages for Rebus, but any provider supported by Rebus is also supported by Elsa. The packages mentioned here are there for convenience, but if you wanted to use Rebus' [Rebus.GoogleCloudPubSub](https://github.com/rebus-org/Rebus.GoogleCloudPubSub) for example, you can add that package directly and configure it as follows:

```c#
services.AddElsa(elsa => elsa.UseServiceBus(context => context.Configurer.Transport(t => t.UsePubSub(context.QueueName)));
```

Note: Whatever provider you are going to use, remember to keep the ```context.QueueName```, i.e. **Do not change it with a custom name!**

## Distributed Lock Provider

Elsa uses [DistributedLock](https://github.com/madelson/DistributedLock) to ensure thant only one thread can work on a workflow instance. By default, the [FileSystem](https://github.com/madelson/DistributedLock/blob/master/docs/DistributedLock.FileSystem.md) lock is used, which ensures that no matter how many threads try to load a workflow instance from the store, only one of them will be able to do so at a time until the lock is released.
When multiple threads try to acquire a lock on a given workflow instance, only the first one will succeed. Subsequent threads will simply wait until the lock is released.

When you run multiple Elsa nodes, it is important to configure a distributed lock provider that can access a shared resource.

If you are using SQL Server to store Elsa workflows, you might consider using the [SqlServer](https://github.com/madelson/DistributedLock/blob/master/docs/DistributedLock.SqlServer.md) provider.
And if you are already using [Redis](https://github.com/madelson/DistributedLock/blob/master/docs/DistributedLock.Redis.md) or [Azure](https://github.com/madelson/DistributedLock/blob/master/docs/DistributedLock.Azure.md), you can use any of those providers as well.

The following snippet shows how to configure Elsa with the SqlServer distributed lock provider:

```c#
services.AddElsa(elsa => elsa.ConfigureDistributedLockProvider(options => options.UseSqlServerLockProvider("Server=localhost;Database=Elsa;Integrated Security=True;")));
```

> Make sure to add the the `Elsa.DistributedLocking.SqlServer` package.

Elsa currently ships with support for SqlServer and Azure Blob Storage, but any provider supported by DistributedLock can be used. To use the [Redis](https://github.com/madelson/DistributedLock/blob/master/docs/DistributedLock.Redis.md) provider for example, you can configure Elsa to use it as follows:

```c#
services.AddRedis("localhost:6379,abortConnect=false"); // Provided by the Elsa.Providers.Redis package. This is optional; you are free to construct your own connection multiplexer from the following factory code.

services.AddElsa(elsa => elsa.ConfigureDistributedLockProvider(options => options.UseProviderFactory(sp => name =>
{
    var connection = sp.GetRequiredService<IConnectionMultiplexer>(); // `services.AddRedis` registers an `IConnectionMultiplexer` as a singleton. 
    return new RedisDistributedLock(name, connection.GetDatabase());
})));
```

## Distributed Cache Signal Provider

Elsa uses a local memory cache to store things like [Workflow Blueprints](#). However, when using a local memory cache in a multi-node environment, the caches need to be synchronized to avoid caches from becoming stale.

When one is dealing with just one node, invalidating local cache entries is easy, because we can listen for domain events to know when it is time to evict a cache entry.

For example, whenever you make changes to a workflow definition, Elsa publishes a domain event called `WorkflowDefinitionSaved`, which is handled by the `CachingWorkflowRegistry` decorator type and clears the cache using a service called `ICacheSignal`.

### ICacheSignal

`ICacheSignal` is a relatively simple service that produces [IChangeToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.primitives.ichangetoken) objects that are used by [IMemoryCache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory).
Other parts of Elsa can then **trigger** a signal that is being observed by the cache in order to invalidate that cache entry.

The default implementation of `ICacheSignal` then triggers these tokens when you call `TriggerToken`.

Elsa provides two additional implementations of `ICacheSignal`, which are:

* RebusCacheSignal
* RedisCacheSignal

### RebusCacheSignal

This implementation uses Elsa's Rebus configuration to **publish a message to all nodes in the cluster**.
Each node receiving this message will then trigger the appropriate change token.

For this to work, you need to configure Rebus with a message broker other than the default memory provider as described in the [Service Bus Broker](#service-bus-broker) section.

The following snippet demonstrates enabling the Rebus provider:

```c#
services.AddElsa(elsa => elsa.UseRebusCacheSignal());
```

No further configuration is necessary since you will already have configured Rebus itself. 

### RedisCacheSignal

This implementation uses Redis' pub/sub mechanism to publish and subscribe to messages and can be enabled as follows:

```c#
services.AddElsa(elsa => elsa.UseRedisCacheSignal());
```

Similar to setting up Redis as the [Distributed Lock Provider](#distributed-lock-provider), you need to register a Redis Connection Multiplexer as a singleton, which can be done with this call:

```c#
services.AddRedis("localhost:6379,abortConnect=false"); // Provided by the Elsa.Providers.Redis package.
```

## Distributed Temporal Services

A temporal service provides functionality to schedule a workflow to execute at a specific time and/or on a recurring interval.
Elsa uses these services to implement the **Timer**, **Cron** and **StartAt** activities.

Elsa comes with the following temporal services:

* [Quartz.NET](https://www.quartz-scheduler.net/)
* [Hangfire](https://www.hangfire.io/)

To register the temporal activities using Quartz.NET as the provider, you would do so as follows:

```c#
servives.AddElsa(elsa => elsa.AddQuartzTemporalActivities());
```

And to use Hangfire instead, you do so as follows:

```c#
servives.AddElsa(elsa => elsa.AddHangfireTemporalActivities());
```

By default, both Quartz.NET and Hangfire are configured to use an in-memory storage provider, which works well for single-node Elsa Server applications.
But when you run multiple Elsa nodes, this also means that each node will begin executing workflows that start with a temporal activity such as **Timer**.
This may or may not be what you want.

In most typical scenarios, you will probably want to run a given time-driven workflow only once per event.
For example, if you have a workflow that sends out newsletters once per day from an Elsa Server cluster consisting of 3 nodes, you probably don't want each node to be sending the newsletter.
Instead, one node should schedule the job, and when the time interval is reached, only one node (which may be another node in the cluster) should execute the job.

To enable this "cluster" mode of operation, you must configure the temporal provider with a persistent storage such as SQL Server. The types of storage providers available depend on the actual temporal provider.

Let's take a look at how to configure both providers.

> Both providers are provided as separate NuGet packages:
> * `Elsa.Activities.Temporal.Quartz`
> * `Elsa.Activities.Temporal.Hangfire`

### Quartz.NET

To setup Quartz.NET to operate in a cluster, we need to configure three aspects:

1. A persistent [job store](https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#datasources-ado-net-jobstores).
2. A [serializer](https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/json-serialization.html#installation).
3. Enable [cluster mode](https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#clustering).

The following snippet demonstrates setting up Quartz.NET:

```c#
services.AddElsa(elsa => elsa
    .AddQuartzTemporalActivities(configureQuartz: quartz => quartz.UsePersistentStore(store =>
    {
        store.UseJsonSerializer();
        store.UseSqlServer("Server=local;Database=Elsa;");
        store.UseClustering();
    }));
```

### Hangfire

Hangfire [supports operation within a cluster automatically](https://docs.hangfire.io/en/latest/background-processing/running-multiple-server-instances.html), provided that you configured it to use persistent storage such as SQL Server.

The following snippet demonstrates setting Hangfire with Elsa:

```c#
services.AddElsa(elsa => elsa.AddHangfireTemporalActivities(hangfire => hangfire.UseSqlServerStorage("Server=local;Database=Elsa;")));
```
