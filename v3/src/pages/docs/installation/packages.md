---
title: Elsa NuGet Packages
description: Understand the Elsa package ecosystem and how to integrate them into your projects.
---

Elsa Workflows is modular and distributed via several NuGet packages. This allows you to pick and choose the components you need, ensuring a lightweight integration into your projects.

## **Core Elsa Package**

The primary package you'll need to get started with Elsa is the `Elsa` package. It's a bundle that includes the following essential packages:

- Elsa.Api.Common
- Elsa.Mediator
- Elsa.Workflows.Core
- Elsa.Workflows.Management
- Elsa.Workflows.Runtime

To install the core `Elsa` package, use the `dotnet` CLI:

```shell
dotnet add package Elsa
```

While the core package provides a solid foundation, Elsa offers additional packages for specialized scenarios.

## **Elsa Package Feeds**

Elsa Workflows packages are distributed through various feeds based on their stability and release phase:

| Type               | Feed  | URL                                                       |
|--------------------|-------|-----------------------------------------------------------|
| Releases           | NuGet | https://api.nuget.org/v3/index.json                       |
| Release candidates | NuGet | https://api.nuget.org/v3/index.json                       |
| Previews           | Feedz | https://f.feedz.io/elsa-workflows/elsa-3/nuget/index.json |

### **Releases**

Stable versions of Elsa are distributed via NuGet.org.

### **Release Candidates (RC)**

RC packages are also available on NuGet.org. They offer a sneak peek into upcoming features, allowing users to test and provide feedback before the final release. While RC packages are generally stable, they might still undergo changes before the final release.

### **Previews**

Preview versions represent the cutting-edge developments in Elsa. They are automatically built and deployed to a public feed on Feedz whenever changes are pushed to the `v3` branch. While they provide the latest features and fixes, they might introduce breaking changes.

To access preview packages, include the feed URL when using the dotnet CLI or add it to your `NuGet.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="NuGet official package source" value="https://api.nuget.org/v3/index.json" />
    <add key="Elsa 3 preview" value="https://f.feedz.io/elsa-workflows/elsa-3/nuget/index.json" />
  </packageSources>
</configuration>
```

{% callout title="Preview packages" type="warning" %}
Ensure the "Preview" checkbox is ticked in your NuGet explorer to view the preview packages.
{% /callout %}

## **Versioning Strategy**

Elsa adheres to a clear versioning strategy:

- **Released** packages: Major.Minor.Revision (e.g., `3.0.1`)
- **Release Candidate** packages: Major.Minor.Revision-preview.X (e.g., `3.0.2-preview.64`)
- **Preview** packages: Major.Minor.Revision-preview.X (e.g., `3.0.2-preview.128`)

The major version remains consistent unless significant changes occur. New features increment the minor version, while fixes or minor improvements bump the revision number.