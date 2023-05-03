---
title: HTTP activities
description: Explanation about the HTTP activities
---

These are the activities in the HTTP category:

| Display name        | .NET type             | Kind    | Description                                                                    |
|---------------------|-----------------------|---------|--------------------------------------------------------------------------------|
| HTTP Response       | `WriteHttpResponse`   | Action  | Writes a response to the HTTP response object.                                 |
| HTTP Request        | `SendHttpRequest`     | Action  | Sends an HTTP request to a given URL. Provides status codes as embedded ports. |
| HTTP Request (flow) | `FlowSendHttpRequest` | Action  | Sends an HTTP request to a given URL. Provides status codes as out ports.      |
| HTTP Endpoint       | `HttpEndpoint`        | Trigger | Waits for an inbound HTTP request on a given path and HTTP verb.               |

Additional details about each activity can be found below.

## HTTP Response

This activity lets you write a response to the current HTTP response object.

| Property          | Type                   | Description                                                                                                                                                    | Example                                    |
|-------------------|------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------|
| `StatusCode`      | `HttpStatusCode`       | The status code to write.                                                                                                                                      | `HttpStatusCode.OK`                        |
| `Content`         | `object?`              | The content to write back. String values will be sent as-is, while objects will be serialized to a JSON string. Byte arrays and streams will be sent as files. | `"Hello World!"`                           |
| `ContentType`     | `string?`              | The content type to write when sending the response.                                                                                                           | `"text/plain"`                             |
| `ResponseHeaders` | `HttpResponseHeaders?` | The headers to send along with the response.                                                                                                                   | `new { "x-generator" = "Elsa Workflows" }` |

{% callout title="HTTP context limitations" type="warning" %}
You should only use this activity when you are inside an HTTP context. For example, when thr workflow is started by an HTTP endpoint, or when invoked via an API endpoint.
{% /callout %}

### Example

```clike
using System.Net;
using Elsa.Http;
using Elsa.Http.Models;
using Elsa.Workflows.Core.Abstractions;
using Elsa.Workflows.Core.Contracts;

namespace Samples;

public class SampleWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new WriteHttpResponse
        {
            StatusCode = new(HttpStatusCode.OK),
            Content = new("Hello World!"),
            ContentType = new("text/plain"),
            ResponseHeaders = new(new HttpResponseHeaders { ["x-generator"] = new[] { "Elsa Workflows" } })
        };
    }
}
```

## HTTP Request

This activity lets you call a http endpoint. You can specify the expected returned statuscodes, url, method, content, contenttype, authorization and headers.
It returns the parsed content and the HttpResponse.

## Flow HTTP Request
???

## HTTP Endpoint
This activity lets you create a http endpoint that can be called via the web.

To be able to call it you need to check the checkbox 'Can start workflow' under general settings.
Also you under Settings you need to add a path and a method to call this endpoint. 

It is possible to use route variables in your path for example: '/orders/{number}'.
The url to call is the base url of your elsa instance + '/workflows' + path

If needed you can add authorization to your endpoint but that needs to be configured first.

In the output tab you can connect the incoming data like the content, route variables, querystring data and headers to variables.

If you connect for example the routevariables to a variable named 'routes' (of type object or dictionary string object) you can use it in JavaScript like this:  
getRoutes().number or like this getRoutes()["number"]

The whole http request is available in result.




