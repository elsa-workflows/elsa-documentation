---
title: Installing Packages from NuGet and MyGet
---
## Releases and Pre-Releases
### NuGet
Elsa (pre)releases on the `master` branch **which are tagged** are deployed to [NuGet](https://www.nuget.org/packages/elsa/).

### npm
Elsa Designer (pre)releases are depolyed to [NPM](https://www.npmjs.com/package/@elsa-workflows/elsa-workflows-studio).

## Preview Releases
All commits on the `master` branch are deployed to [Feedz.io](https://feedz.io/).

In order to install Elsa packages from MyGet, add the following package feed to your project:

```
https://f.feedz.io/elsa-workflows/elsa-2/nuget/index.json
```

The easiest way to do that is by adding a `NuGet.config` file to the root of your project/solution folder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="Elsa Preview Feed" value="https://f.feedz.io/elsa-workflows/elsa-2/nuget/index.json" />
    </packageSources>
</configuration>
```
Designer npm preview packages can be obtained from:

```
https://f.feedz.io/elsa-workflows/elsa-2/npm/@elsa-workflows/elsa-workflows-studio
```

