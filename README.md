[![Build Status](https://github.com/aviationexam/Aviationexam.MoneyErp/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/aviationexam/Aviationexam.MoneyErp/actions/workflows/build.yml)
[![Build Status - DevOps](https://dev.azure.com/aviationexam/Web%20app/_apis/build/status%2FWeb%20V3%2FPR%20Build%2FAviationexam.MoneyErp%20-%20Build?branchName=main)](https://github.com/aviationexam/Aviationexam.MoneyErp/actions/workflows/build.yml)

### Aviationexam.MoneyErp.Common
[![NuGet](https://img.shields.io/nuget/v/Aviationexam.MoneyErp.Common.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Aviationexam.MoneyErp.Common/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Faviationexam%2Fmoney-erp%2Fshield%2FAviationexam.MoneyErp.Common%2Flatest&label=Aviationexam.MoneyErp.Common)](https://f.feedz.io/aviationexam/money-erp/packages/Aviationexam.MoneyErp.Common/latest/download)

### Aviationexam.MoneyErp.Graphql
[![NuGet](https://img.shields.io/nuget/v/Aviationexam.MoneyErp.Graphql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Aviationexam.MoneyErp.Graphql/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Faviationexam%2Fmoney-erp%2Fshield%2FAviationexam.MoneyErp.Graphql%2Flatest&label=Aviationexam.MoneyErp.Graphql)](https://f.feedz.io/aviationexam/money-erp/packages/Aviationexam.MoneyErp.Graphql/latest/download)

# Aviationexam.MoneyErp Clients

This repository contains .NET clients for interacting with the Money ERP system, a popular accounting and business management software in the Czech Republic and Slovakia. The clients provide convenient access to the GraphQL API of Money ERP.

## Getting Started

To get started, you need to add the desired client library to your project and configure the necessary services in your dependency injection container.

First, add the core package to your project:

```bash
dotnet add package Aviationexam.MoneyErp.Common
```

Then, in your `Program.cs` or `Startup.cs`, configure the Money ERP services:

```csharp
using Aviationexam.MoneyErp.Common.Extensions;

// ...

builder.Services.AddMoneyErp(
    options => options.Configure(builder.Configuration.GetSection("MoneyErp"))
);
```

This will configure the basic services required for authentication and communication with the Money ERP API. The configuration section in your `appsettings.json` should look like this:

```json
{
  "MoneyErp": {
    "Endpoint": "https://your-money-erp-endpoint",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

## GraphQL Client

To use the GraphQL client, you need to add the `Aviationexam.MoneyErp.Graphql` package to your project:

```bash
dotnet add package Aviationexam.MoneyErp.Graphql
```

Then, chain the `AddGraphQlClient` method to the `AddMoneyErp` call:

```csharp
using Aviationexam.MoneyErp.Graphql.Extensions;

// ...

builder.Services.AddMoneyErp(
    options => options.Configure(builder.Configuration.GetSection("MoneyErp"))
)
.AddGraphQlClient(
    options => options.Configure(builder.Configuration.GetSection("MoneyErp"))
);
```

Now you can inject `MoneyErpGraphqlClient` into your services and use it to make GraphQL queries:

```csharp
public class MyService(MoneyErpGraphqlClient graphqlClient)
{
    public async Task<string> GetMoneyErpVersion()
    {
        var version = await graphqlClient.Query(x => x.Version);
        return version.Data;
    }
}
```