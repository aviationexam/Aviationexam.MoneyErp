using Aviationexam.MoneyErp.Extensions;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Aviationexam.MoneyErp.Tests.Infrastructure;

public static class ServiceProviderFactory
{
    public static ServiceProvider Create(
        MoneyErpAuthenticationsClassData.AuthenticationData authenticationData,
        bool shouldRedactHeaderValue = true
    ) => new ServiceCollection()
        .AddLogging(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddProvider(new XUnitLoggerProvider(TestContext.Current.TestOutputHelper, appendScope: false))
        )
        .AddSingleton<TimeProvider>(_ => TimeProvider.System)
        .AddMoneyErpApiClient(builder => builder.Configure(x =>
        {
            x.ClientId = authenticationData.ClientId;
            x.ClientSecret = authenticationData.ClientSecret;
            x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
            x.Endpoint = new Uri(authenticationData.ServerAddress, UriKind.RelativeOrAbsolute);
        }), shouldRedactHeaderValue)
        .AddMoneyErpApiGraphQlClient(builder => builder.Configure(x =>
        {
            x.GraphQlEndpoint = new Uri(authenticationData.ServerAddress, UriKind.RelativeOrAbsolute);
        }), shouldRedactHeaderValue)
        .BuildServiceProvider();
}
