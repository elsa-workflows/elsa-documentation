---
title: HTTP activities
description: Explanation about the http activities
---

Here we explain the http activities

## HTTP Response
With this activity you return a response from your workflow to the requester. 
You can specify the statuscode, content, contenttype and response headers.

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




