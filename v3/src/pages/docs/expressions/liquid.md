---
title: Liquid expressions
description: A glossary of Liquid expressions.
---

When working with Elsa, you'll often want to write dynamic expressions. This page provides a glossary of various filters and tags you can use, in addition to the standard set that you can find in the [Liquid documentation](https://shopify.github.io/liquid/).

Elsa uses the [Fluid library](https://github.com/sebastienros/fluid) to implement Liquid. More tags and filters can be found there, as well as details on providing your own tags and filters.

## Utility

These are built-in Liquid filters that are common to most Liquid implementations.

| Filter   | Description                              | Example                      |
|----------|------------------------------------------|------------------------------|
| `json`   | Serializes the input into JSON.          | `{{ some_value \| json }}`   |
| `base64` | Converts the input into a base64 string. | `{{ some_value \| base64 }}` |

## Workflow

| Object          | Description                                  | Example                      |
|-----------------|----------------------------------------------|------------------------------|
| `Variables`     | Provides access to workflow variable values. | `{{ Variables.MyVariable }}` |
| `CorrelationId` | The correlation ID of the workflow.          | `{{ CorrelationId }}`        |
