---
id: designer-using-the-designer
title: Enabling horizontal layout
sidebar_label: Enabling Horizontal Layout
---

## Setup

To enable this feature you must create a JSON file, we have used **designer.config.json** as the default file name. This file is used to store the existing features.

For now only the layout direction feature is supported.

File sample:

```json
{
  "workflowLayout": {
    "enabled": true,
    "ui": true,
    "defaultValue": "topbottom"
  }
}

```

Properties:
- **enabled**: determines if the feature is enabled and will be applied
- **ui**: determines if the feature is displayed in the UI or not.
- **defaultValue**: the initial value. For the layout feature the options are: "topbottom", "bottomtop", "leftright" & "rightleft".

You can also take a look at the elsa-core/src/designer/elsa-workflows-studio/src/assets/designer.config.json file which is used for the example.

Next you must pass the file location to the root component. 

**Important** The location is not the location of the file in the project, but rather the URL or relative path from the host. It will try do a request to that location to get the file so make sure it's accessible.

```jsx
<elsa-studio-root server-url="https://localhost:11000" monaco-lib-path="build/assets/js/monaco-editor/min" culture="en-US" config="build/assets/designer.config.json">
  <!-- The root dashboard component -->
  <elsa-studio-dashboard></elsa-studio-dashboard>
</elsa-studio-root>
```

## Using in the application

Now that the settings are enabled you should have an extra tab in the designer:

{% figure src="/assets/designer/designer-horizontal-layout-tab.png" /%}

Any changes done to the layout are also applied to instance editor. The layout cannot be changed from the instance editor.
