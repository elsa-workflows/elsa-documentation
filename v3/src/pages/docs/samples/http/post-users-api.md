    ---
title: HTTP Backend API
description: A sample workflow that accepts HTTP POST requests and returns a JSON response.
---

![HTTP Post Users API](/samples/http/post-users-api/http-post-users-api.png)

This is an example of a workflow that demonstrates the following features:

- Responding to inbound HTTP POST requests that include a JSON body.
- Returning the received JSON payload as a JSON response.
- Using variables.
- Using expressions.

[http-post-users-api.json](/samples/http/post-users-api/http-post-users-api.json)

Sample CURL:

```shell
curl --location 'https://localhost:5001/workflows/users' \
--header 'Content-Type: application/json' \
--data-raw '{
"id": 1,
"email": "bob@acme.com",
"name": "Bob Jones"
}'
```

Sample response:

```json
{
    "id": 1,
    "email": "bob@acme.com",
    "name": "Bob Jones"
}
```
