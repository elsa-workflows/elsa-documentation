---
title: Elsa Docker image
description: Running the Elsa Docker container
---

## TLDR

```shell
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -p 13000:80 elsaworkflows/elsa-v3:latest
```

Designer login:
```
Username: admin
Password: password
```

## Introduction

We are maintaining a reference ASP.NET application that hosts both the workflow server and designer that is distributed as a Docker image from https://hub.docker.com/repository/docker/elsaworkflows/elsa-v3.

This should make it easy to give Elsa a quick spin without having to create an ASP.NET application and setting up Elsa.

## Running the docker image

Before trying to run the image, make sure you have [Docker](https://docs.docker.com/get-docker/) installed on your machine.

To run the image, simply run the following command from your terminal:

```shell
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -p 13000:80 elsaworkflows/elsa-v3:latest
```

When the container has started, open a web browser and navigate to http://localhost:13000/.

On the login screen, enter the following credentials:

```
Username: admin
Password: password
```

That's it!