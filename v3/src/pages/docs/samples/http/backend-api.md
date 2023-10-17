    ---
title: HTTP Backend API
description: A sample workflow that responds to HTTP requests by fetching user data from a backend API and returning it as JSON.
---

![HTTP Backend API](/samples/http/backend-api/http-backend-api.png)

This is an example of a workflow that demonstrates the following features:

- Responding to inbound HTTP requests.
- Making HTTP requests to a backend API.
- Returning JSON responses.
- Using variables.
- Using expressions.
- Using route parameters.

[http-backend-api.json](/samples/http/backend-api/http-backend-api.json)

Sample CURL:

```shell
curl --location 'https://localhost:5001/workflows/users/2'
```

Sample response:

```json
{
  "id": 2,
  "email": "janet.weaver@reqres.in",
  "first_name": "Janet",
  "last_name": "Weaver",
  "avatar": "https://reqres.in/img/faces/2-image.jpg"
}
```