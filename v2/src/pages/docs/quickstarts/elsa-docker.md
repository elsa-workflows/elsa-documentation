---
title: Elsa + Docker
---

If you have [Docker](http://docker.com/) installed, then running the Elsa Dashboard + Server Docker image is the quickest way to run the dashboard:

```bash
docker run -t -i -e ELSA__SERVER__BASEURL=http://localhost:13000 -e ASPNETCORE_ENVIRONMENT=Development -p 13000:80 elsaworkflows/elsa-dashboard-and-server:latest
```

Notice that you can specify the Elsa Server URL by passing in an environment variable called `ELSA__SERVER__BASEURL`.
We also instructed Docker to map container port 80 to port 13000, which means that we can now launch a webbrowser and navigate to:

```
http://localhost:13000/
```

Here's what that looks like:

![Elsa Dashboard + Docker](assets/quickstarts/quickstarts-elsa-dashboard-docker-animation-1.gif" /%}