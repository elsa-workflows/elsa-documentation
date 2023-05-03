---
title: Liquid expressions
description: A glossary of Liquid expressions.
---

When working with Elsa, you'll sometimes want to write Liquid expressions. This page provides a glossary of various filters and tags you can use, in addition to the standard set that you can find in the [Liquid documentation](https://shopify.github.io/liquid/).

Elsa uses the [Fluid library](https://github.com/sebastienros/fluid) to implement Liquid. More tags and filters can be found there, as well as details on providing your own tags and filters.

## Utility

These are built-in Liquid filters that are common to most Liquid implementations.

| Filter | Description                 | Example                     |
|--------|-----------------------------|-----------------------------|
| `json` | Formats the input into JSON | `{{ some_object \| json }}` |

