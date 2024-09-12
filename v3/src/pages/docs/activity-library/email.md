---
title: Send Email activities
description: Explanation about the Send Email  activities
---


# How to Use the Send Email Feature in Elsa 3.0

This guide will show you how to configure and use the Send Email feature in Elsa 3.0.

## Step 1: Install Elsa.Email Package

To use the Send Email feature in Elsa 3.0, first, install the `Elsa.Email` package using the following command:

```bash
dotnet add package Elsa.Email --version 3.2.0
```

## Step 2: Configure Email in Program.cs

In your `Program.cs` file, add the following code to configure the Send Email feature in Elsa:

```csharp
// Use email activities.
elsa.UseEmail(email =>
{
    // Get email configuration from appsettings
    EmailOptions kk = builder.Configuration.GetSection("EmailOptions").Get<EmailOptions>();

    // Configure email options
    email.ConfigureOptions = (options) =>
    {
        options.Host = kk.Host;
        options.Port = kk.Port;
        options.UserName = kk.UserName;
        options.Password = kk.Password;
        options.SecureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect;
        options.RequireCredentials = true;
    };
});
```

This code reads email configuration options from the `EmailOptions` section in your `appsettings.json` file and sets them up for the Send Email activity.

## Step 3: Add EmailOptions Configuration to appsettings.json

Next, add the `EmailOptions` configuration section to your `appsettings.json` file. This section will store your email server settings:

```json
{
  "EmailOptions": {
    "Host": "smtp.maildomain.com",
    "Port": 25,
    "UserName": "username",
    "Password": "password"
  }
}
```

Make sure to replace the `Host`, `Port`, `UserName`, and `Password` with your actual email server credentials.

## Step 4: Use Send Email Activity in Elsa Studio

Once you have completed the configuration, open Elsa Studio, and you'll be able to use the **Send Email Activity** in your workflows.
