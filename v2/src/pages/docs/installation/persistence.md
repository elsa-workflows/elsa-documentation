---
title: Persistence
---

By default, in-memory stores are registered with the DI service container.
But, to make sure your workflows are stored in a more permanent fashion, you will want to add one of the available **persistence providers**.

## Persistence Providers

Elsa has abstracted away its data access logic, enabling the persistence layer to be pluggable.
Out of the box, Elsa currently ships with the following providers:

* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
* [MongoDB](https://www.mongodb.com/)
* [YesSQL](https://github.com/sebastienros/yessql/blob/dev/README.md)

## Entity Framework Core
For example, to use the [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) persistence provider and Sqlite as the database engine, add the following packages:

```bash
dotnet add package Elsa
dotnet add package Elsa.Persistence.EntityFramework
dotnet add package Elsa.Persistence.EntityFramework.Sqlite
```

When installed, update your `Startup` class as follows to configure Elsa to use the EF Core provider with SQLite:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseSqlite()));
}
```

By default, Elsa will use the following connection string when using SQLite: `"Data Source=elsa.sqlite.db;Cache=Shared;"`.
You can override this by importing the `Microsoft.EntityFrameworkCore` namespace to make available the default `UseSqlite()` extension method that accepts a connection string. Example:

```c#
using Microsoft.EntityFrameworkCore;

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseSqlite("Data Source=my-db-name.db;Cache=Shared;")));
}
```

> You can find an [example using Entity Framework Core](https://github.com/elsa-workflows/elsa-core/blob/master/src/samples/persistence/Elsa.Samples.Persistence.EntityFramework/Program.cs#L21) and [YesSQL](https://github.com/elsa-workflows/elsa-core/blob/master/src/samples/persistence/Elsa.Samples.Persistence.YesSql/Program.cs) in the samples folder on GitHub as well.


### SQL Server

To use SQL Server as the EF Core provider, add the `Elsa.Persistence.EntityFramework.SqlServer` package:

```bash
dotnet add package Elsa.Persistence.EntityFramework.SqlServer
```

Then update your Startup class as follows:

```c#
using Elsa.Persistence.EntityFramework.SqlServer

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseSqlServer("Server=local;Database=Elsa")));
}
```

### MySQL

To use MySQL as the EF Core provider, add the `Elsa.Persistence.EntityFramework.MySql` package:

```bash
dotnet add package Elsa.Persistence.EntityFramework.MySql
```

Then update your Startup class as follows:

```c#
using Elsa.Persistence.EntityFramework.MySql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseMySql("Server=localhost;Port=3306;Database=elsa;User=myuser;Password=mypassword;")));
}
```

### Postgres

To use PostgreSQL as the EF Core provider, add the `Elsa.Persistence.EntityFramework.PostgreSql` package:

```bash
dotnet add package Elsa.Persistence.EntityFramework.PostgreSql
```

Then update your Startup class as follows:

```c#
using Elsa.Persistence.EntityFramework.PostgreSql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UsePostgreSql("Server=127.0.0.1;Port=5432;Database=elsa;User Id=postgres;Password=password;")));
}
```

### EF Core Migrations

The EF Core provider for Elsa ships with migrations that you can run either manually or automatically (the default for pooled db contexts).

> Auto-running migrations is convenient to get up and running quickly, but in real world production apps, it is recommend that you maintain your own migrations instead so that you are in control over what happens to the DB schemas.

You can control whether or not to execute migrations automatically using a second parameter on `UseEntityFrameworkPersistence` called `autoRunMigrations`. For example:

```c#
// Run migrations automatically:
services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseSqlite(), true));

// Disable auto-migrations:
services.AddElsa(options => options.UseEntityFrameworkPersistence(ef => ef.UseSqlite(), false));
```

Elsa ships with migrations for:

* MySql
* PostgreSql
* Sqlite
* SqlServer

Each set of migrations are stored in a separate package. For example, the migrations for PostgreSql are stored in the `Elsa.Persistence.EntityFramework.PostgreSql` package.

### Running Migrations Manually

To run the existing migrations provided by Elsa manually, you will need to create a class that implements `IDesignTimeDbContextFactory<ElsaContext>` in your startup project or another executable project (e.g. a console application) in order to be able to use the `dotnet ef database update` command. 

Follow these steps to apply the migrations on a new *Sqlite* database for example:

1. Add the `Microsoft.EntityFrameworkCore.Design` package to your startup project.
2. Add `Elsa.Persistence.EntityFramework.Core`
3. Create a class called e.g. `SqliteElsaContextFactory` (implementation below).
4. Run the following command: `dotnet ef database update -- ConnectionStrings:Elsa="Data Source=elsa.sqlite.db;Cache=Shared;"`

The `SqliteElsaContextFactory` class looks like this:

```c#
using System.IO;
using Elsa.Persistence.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Data
{
    public class SqliteElsaContextFactory : IDesignTimeDbContextFactory<ElsaContext>
    {
        public ElsaContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder();
            var connectionString = configuration.GetConnectionString("Elsa");

            dbContextBuilder.UseSqlite(connectionString, sqlite => sqlite.MigrationsAssembly(typeof(Elsa.Persistence.EntityFramework.Sqlite.SqliteElsaContextFactory).Assembly.FullName));

            return new ElsaContext(dbContextBuilder.Options);
        }
    }
}
```

Notice that the factory accepts command-line arguments, which conveniently allows us to invoke the `dotnet ef database update` command by passing in arguments that get parsed into a `Configuration` object, making it convenient from the factory class to access the specified connection string.

## MongoDB

To configure Elsa with MongoDB, do the following:

Add the `Elsa.Persistence.MongoDb` package:

```bash
dotnet add package Elsa.Persistence.MongoDb
```

And update your Startup class as follows:

```c#
using Elsa.Persistence.MongoDb

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseMongoDbPersistence(options => options.ConnectionString = "mongodb://localhost:27017/Elsa"));
}
```

## YesSQL

To configure Elsa with YesSQL, do the following:

Add the `Elsa.Persistence.YesSql` package:

```bash
dotnet add package Elsa.Persistence.YesSql
```

And update your Startup class as follows:

```c#
using Elsa.Persistence.YesSql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseYesSqlPersistence());
}
```

If you don't configure YesSql with a specific database provider, Elsa will us SQLite by default.

### SQL Server

To configure YesSQL with SQL Server, add the `YesSql.Provider.SqlServer` package:

```bash
dotnet add package YesSql.Provider.SqlServer
```

And update your Startup class as follows:

```c#
using Elsa.Persistence.YesSql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseYesSqlPersistence(config => config.UseSqlServer("Server=local;Database=Elsa")));
}
```

### Postgres

To configure YesSQL with PostgreSql, add the `YesSql.Provider.PostgreSql` package:

```bash
dotnet add package YesSql.Provider.PostgreSql
```

And update your Startup class as follows:

```c#
using Elsa.Persistence.YesSql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseYesSqlPersistence(config => config.UsePostgreSql("Server=127.0.0.1;Port=5432;Database=elsa;User Id=postgres;Password=password;")));
}
```

### MySql

To configure YesSQL with MySql, add the `YesSql.Provider.MySql` package:

```bash
dotnet add package YesSql.Provider.MySql
```

And update your Startup class as follows:

```c#
using Elsa.Persistence.YesSql

public void ConfigureServices(IServiceCollection services)
{
    services.AddElsa(options => options.UseYesSqlPersistence(config => config.UseMySql("Server=localhost;Port=3306;Database=elsa;User=myuser;Password=mypassword;")));
}
```

## Custom Providers

If none of the providers meet your need, you can always implement your own. Look at the source code of [one of the existing providers](https://github.com/elsa-workflows/elsa-core/tree/master/src/persistence) for an existing example.
