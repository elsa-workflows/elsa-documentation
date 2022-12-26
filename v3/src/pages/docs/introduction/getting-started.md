---
title: Getting started
description: Getting started with Elsa Workflows.
---

There are different architectural approaches one might take when building an application with workflow support, and we will look at all of them.
We will tackle this by going through different "recipies", starting with the simplest one.

The recipes are categorized as follows:

## Console applications

These are simple console applications that are the easiest to setup and will demonstrate various kinds of workflows written in C#, how to run them, and also show things like loading and executing workflows from JSON files.

## ASP.NET

Here, we'll be looking at different kinds of setups. For example, we'll setup a basic workflow server that hosts workflows created in C# that we can invoke through a REST API, both via a custom controller as well as endpoints provided from the `Elsa.Workflows.Api` package.
We'll also add a dashboard app to create workflows visually, both using a "monolith" approach where the application acts as both the workflow + dashboard server, as well as hosting the dashboard app separately,

## Docker

We'll provide basic Dockerfile and instructions to help you containerize your workflow applications.