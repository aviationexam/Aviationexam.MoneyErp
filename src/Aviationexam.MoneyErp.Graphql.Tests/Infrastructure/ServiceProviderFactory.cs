using Aviationexam.MoneyErp.Common;
using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;

public static class ServiceProviderFactory
{
    public static ServiceProvider Create(
        MoneyErpAuthenticationsClassData.AuthenticationData authenticationData,
        ITestOutputHelper testOutputHelper,
        bool shouldRedactHeaderValue = true
    ) => new ServiceCollection()
        .AddLogging(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddXUnit(testOutputHelper)
        )
        .AddSingleton<TimeProvider>(_ => TimeProvider.System)
        .AddScoped<IEndpointCertificateProvider>(_ => new PemEndpointCertificateProvider(authenticationData.EndpointCertificatePem))
        .AddMoneyErp(builder => builder.Configure(x =>
        {
            x.ClientId = authenticationData.ClientId;
            x.ClientSecret = authenticationData.ClientSecret;
            x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
            x.Endpoint = new Uri(authenticationData.ServerAddress, UriKind.RelativeOrAbsolute);
        }))
        .AddGraphQlClient(_ => { }, shouldRedactHeaderValue)
        .Services
        .BuildServiceProvider();
}
