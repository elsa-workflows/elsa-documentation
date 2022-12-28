---
title: Packages
description: NuGet packages
---

The Elsa library consists of many packages, all starting with the name `Elsa`.
To get started, you only need the `Elsa` package, which is a bundle of the following packages:

- Elsa.Api.Common
- Elsa.Mediator
- Elsa.Workflows.Core
- Elsa.Workflows.Management
- Elsa.Workflows.Runtime

To install the `Elsa` bundle package, you can use the `dotnet` CLI from the directory of your project:

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

## Releases

Releases are distributed via NuGet.org.

## Release Candidates

Releases candidates, too, are distributed via NuGet.org.
The goal of RC packages is to give users a chance to try the upcoming bits and identity potential blocking issue before the final release.

RC packages are usually more stable than preview packages.

## Previews

The latest preview versions are automatically build and deployed to a public feed hosted on Azure DevOps as soon as commits get pushed to the `v3` branch.

Preview packages are useful if you want to take advantage of the most recent additions and fixes, but they are also volatile and can cause additional work due to compile-time breaking changes happening from time to time.

To access packages from this feed, you can include the feed URL when adding a package using the dotnet CLI or add the feed to your NuGet.config role (recommended).

Sample NuGet.Config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="NuGet official package source" value="https://api.nuget.org/v3/index.json" />
    <add key="Elsa 3 preview" value="https://pkgs.dev.azure.com/elsa-workflows/3cbdb983-acb6-4ba7-b862-f9e3cbd4e213/_packaging/Packages/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

{% callout title="Preview packages" type="warning" %}
When looking for preview packages using your NuGet explorer, make sure to tick the "Preview" checkbox. Otherwise, you will not see the preview packages.
{% /callout %}

## Versioning

All **released** packages consist of a major, minor and revision. For example: `3.0.1`.
All **release candidate** packages, in addition, are suffixed with the label `-rc`, followed by the release candidate number. For example: `3.0.2-rc1`
All **preview** packages consist of a major, minor and revision and a suffix with the label `-preview.`, followed by a build number. For example: `3.0.2-preview.128` 

Unless a major overhaul takes place, like what was done between Elsa 2 and 3, the major version number will stay at `3`.
When new features are introduced, the minor version number will be incremented.
For fixes, non-visible changes and/or other small improvements, usually only the revision number will be incremented.