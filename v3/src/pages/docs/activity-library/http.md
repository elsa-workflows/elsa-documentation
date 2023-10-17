---
title: HTTP activities
description: Explanation about the HTTP activities
---

These are the activities in the HTTP category:

| Display name        | .NET type               | Kind    | Description                                                                    |
|---------------------|-------------------------|---------|--------------------------------------------------------------------------------|
| HTTP Response       | `WriteHttpResponse`     | Action  | Writes a response to the HTTP response.                                        |
| HTTP File Response  | `WriteFileHttpResponse` | Action  | Writes one or more files to the HTTP response.                                 |
| HTTP Request        | `SendHttpRequest`       | Action  | Sends an HTTP request to a given URL. Provides status codes as embedded ports. |
| HTTP Request (flow) | `FlowSendHttpRequest`   | Action  | Sends an HTTP request to a given URL. Provides status codes as out ports.      |
| HTTP Endpoint       | `HttpEndpoint`          | Trigger | Waits for an inbound HTTP request on a given path and HTTP verb.               |

Additional details about each activity can be found below.

## HTTP Response

This activity lets you write a response to the current HTTP response object.

| Property          | Type                   | Description                                                                                                       | Example                                    |
|-------------------|------------------------|-------------------------------------------------------------------------------------------------------------------|--------------------------------------------|
| `StatusCode`      | `HttpStatusCode`       | The status code to write.                                                                                         | `HttpStatusCode.OK`                        |
| `Content`         | `object?`              | The content to write back. String values will be sent as-is, while objects will be serialized to a JSON string.   | `"Hello World!"`                           |
| `ContentType`     | `string?`              | The content type to write when sending the response.                                                              | `"text/plain"`                             |
| `ResponseHeaders` | `HttpResponseHeaders?` | The headers to send along with the response.                                                                      | `new { "x-generator" = "Elsa Workflows" }` |

## HTTP File Response

This activity lets you write one or more files to the current HTTP response object.
Supports resumable/partial downloads.

| Property                      | Type      | Description                                                                                                                                                                           | Example          |
|-------------------------------|-----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------|
| `Content`                     | `object?` | The content to send back as a file. Supports URLs (which will be downloaded first), byte arrays, streams, `Downloadable` objects and arrays of any combination of these types.        | Hello World!     |
| `ContentType`                 | `string?` | The file MIME type to write when sending the response.                                                                                                                                | application/pdf  |
| `Filename`                    | `string?` | The filename to download the file as. If not set, the filename will be determined by the system.                                                                                      | invoice.pdf      |
| `Entity Tag`                  | `string?` | A custom entity tag to use. If not set, the entity tag will be determined by the system.                                                                                              | "abcd1e3"        |
| `Enable Resumeable Downloads` | `bool`    | Whether to enable resumable downloads. When enabled, the client can resume a download if the connection is lost.                                                                      | true             |
| `Download Correlation ID`     | `string?` | When supporting resumable downloads, the system needs to be able to correlate the generated cached file to the request. When left empty, the system will use the "x-elsa-download-id" | "my-download-id" |

{% callout title="HTTP context limitations" type="warning" %}
You should only use these activities when you are inside an HTTP context. For example, when the workflow is started by an HTTP endpoint, or when invoked via an API endpoint.
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

This activity lets you call a http endpoint. You can specify the expected returned status codes, url, method, content, content type, authorization and headers.
It returns the parsed content and the HttpResponse.

## HTTP Request (Flow)

Same as HTTP Request, but the status codes are returned as out ports instead of embedded ports.

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




