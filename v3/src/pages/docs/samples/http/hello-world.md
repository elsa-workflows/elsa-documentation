---
title: HTTP Hello World
description: A sample workflow that responds to HTTP requests.
---

![HTTP Hello World](/samples/http/hello-world/http-hello-world.png)

This is a simple workflow that listens for HTTP requests and responds with a "Hello World" message.

[http-hello-world.json](/samples/http/hello-world/http-hello-world.json)

Sample CURL:

```shell
curl --location 'https://localhost:5001/workflows/hello-world'
```

Sample response:

```json
{
    "message": "Hello World!"
}
```