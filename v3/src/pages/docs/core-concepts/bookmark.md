---
title: Bookmark
description: About bookmarks.
---

A bookmark represents a point in a workflow where the workflow can be paused and resumed later.

Bookmarks are created by blocking activities, such as the `Event` activity or the `Delay` activity.

{% callout title="Workflow Execution" %}
Even though a blocking activity waits until its bookmark is resumed, the workflow continues to execute other scheduled activities in other branches.
{% /callout %}