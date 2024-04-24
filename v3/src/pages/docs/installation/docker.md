---
title: Elsa Docker images
description: Running the Elsa Docker containers
---

## TLDR

### Elsa Server + Studio

Run the following commands to try the latest Elsa Server + Studio Docker image:

```shell
docker pull elsaworkflows/elsa-server-and-studio-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -e HTTP_PORTS=8080 -e HTTP__BASEURL=http://localhost:13000 -p 13000:8080 elsaworkflows/elsa-server-and-studio-v3:latest
```

Navigate to: http://localhost:13000/

Studio login:
```
Username: admin
Password: password
```

### Elsa Server

Run the following commands to try the latest Elsa Server Docker image:

```shell
docker pull elsaworkflows/elsa-server-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -e HTTP_PORTS=8080 -e HTTP__BASEURL=http://localhost:13000 -p 13000:8080 elsaworkflows/elsa-server-v3:latest
```

Navigate to: http://localhost:13000/

### Elsa Studio

Run the following commands to try the latest Elsa Studio Docker image:

```shell
docker pull elsaworkflows/elsa-studio-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -e HTTP_PORTS=8080 -e ELSASERVER__URL=http://localhost:13000/elsa/api -p 14000:8080 elsaworkflows/elsa-studio-v3:latest
```

Navigate to: http://localhost:14000/

Login:
```
Username: admin
Password: password
```

## Introduction

We are maintaining the following set of reference ASP.NET applications at https://hub.docker.com/repository/docker/elsaworkflows:

- [elsaworkflows/elsa-server-and-studio-v3](https://hub.docker.com/repository/docker/elsaworkflows/elsa-server-and-studio-v3)
- [elsaworkflows/elsa-server-v3](https://hub.docker.com/repository/docker/elsaworkflows/elsa-server-v3)
- [elsaworkflows/elsa-studio-v3](https://hub.docker.com/repository/docker/elsaworkflows/elsa-studio-v3)

This should make it easy to give Elsa a quick spin without having to create an ASP.NET application and setting up Elsa.

Before trying to run an image, make sure you have [Docker](https://docs.docker.com/get-docker/) installed on your machine.

## Elsa Server + Studio

This image hosts an ASP.NET Core application that runs both as an Elsa Server as well as an Elsa Studio application.

To run the container, simply run the following commands from your terminal:

```shell
docker pull elsaworkflows/elsa-server-and-studio-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT=Development -p 13000:80 elsaworkflows/elsa-server-and-studio-v3:latest
```

When the container has started, open a web browser and navigate to http://localhost:13000/.

On the login screen, enter the following credentials:

```
Username: admin
Password: password
```

That's it!

## Elsa Server

This image hosts an ASP.NET Core application that runs as an Elsa Server.

To run the container, simply run the following commands from your terminal:

```shell
docker pull elsaworkflows/elsa-server-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT=Development -p 13000:80 elsaworkflows/elsa-server-v3:latest
```

When the container has started, open a web browser and navigate to http://localhost:13000/.

To view the API endpoints, navigate to http://localhost:13000/swagger.
That's it!

## Elsa Studio

This image hosts an ASP.NET Core application that runs as an Elsa Studio application.

To run the container, simply run the following commands from your terminal:

```shell
docker pull elsaworkflows/elsa-studio-v3:latest
docker run -t -i -e ASPNETCORE_ENVIRONMENT=Development -e ELSASERVER__URL=http://localhost:13000/elsa/api -p 14000:80 elsaworkflows/elsa-studio-v3:latest
```

When the container has started, open a web browser and navigate to http://localhost:14000/.

On the login screen, enter the following credentials:

```
Username: admin
Password: password
```

That's it!