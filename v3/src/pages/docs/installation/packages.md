---
title: Packages
description: NuGet packages
---

## Packages

All NuGet packages starting with `Elsa` are part of the Elsa Workflows package library. To get started, however, all you need is the following package:

- Elsa

To install the Elsa package, you can use the `dotnet` CLI from the directory of your project. For example:

```shell
dotnet add package Elsa
```

Additional packages are there to cater to different scenarios.

## Feeds

Elsa Workflows is distributed via NuGet packages from the following feeds:

| Type               | Feed         | URL                                                                                                                    |
|--------------------|--------------|------------------------------------------------------------------------------------------------------------------------|
| Releases           | NuGet        | https://api.nuget.org/v3/index.json                                                                                    |
| Release candidates | NuGet        | https://api.nuget.org/v3/index.json                                                                                    |
| CI/CD              | Azure DevOps | https://pkgs.dev.azure.com/elsa-workflows/3cbdb983-acb6-4ba7-b862-f9e3cbd4e213/_packaging/Packages/nuget/v3/index.json |

---

## Releases and release candidates

Both official releases and release candidates are distributed via NuGet.org.

### Continuous delivery

The latest preview versions are automatically build and deployed to Azure DevOps as soon as commits get pushed to the `v3` branch.
To access packages from the CI/CD feed, you include the feed URL when adding a package using the dotnet CLI, or add the feed to your NuGet.config role (recommended).

Sample NuGet.Config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="NuGet official package source" value="https://api.nuget.org/v3/index.json" />
    <add key="Jint prereleases" value="https://www.myget.org/F/jint/api/v3/index.json" />
    <add key="Elsa 3 CI/CD" value="https://pkgs.dev.azure.com/elsa-workflows/3cbdb983-acb6-4ba7-b862-f9e3cbd4e213/_packaging/Packages/nuget/v3/index.json" />
  </packageSources>
</configuration>
```