using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.Extensions;
using Aviationexam.MoneyErp.Tests.Infrastructure;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.MoneyErp.Tests;

public class MoneyErpApiClientTests
{
    [Theory]
    [MemberData(nameof(MoneyErpAuthentications))]
    public async Task TestGetConnectionAsync(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Connection.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [MemberData(nameof(MoneyErpAuthentications))]
    public async Task TestGetIssuedInvoiceAsync(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.IssuedInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [MemberData(nameof(MoneyErpAuthentications))]
    public async Task TestGetCompanyAsync(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Company.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    private static ServiceProvider BuildServiceProvider(
        string clientId, string clientSecret, string serverAddress
    ) => new ServiceCollection()
        .AddLogging(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddProvider(new XUnitLoggerProvider(TestContext.Current.TestOutputHelper, appendScope: false))
        )
        .AddSingleton<TimeProvider>(_ => TimeProvider.System)
        .AddMoneyErpApiClient(builder => builder.Configure(x =>
        {
            x.ClientId = clientId;
            x.ClientSecret = clientSecret;
            x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
            x.Endpoint = new Uri(serverAddress, UriKind.RelativeOrAbsolute);
        }), shouldRedactHeaderValue: true)
        .BuildServiceProvider();

    public static TheoryData<string, string, string> MoneyErpAuthentications()
    {
        Loader.LoadEnvFile(".env.local");

        var clientId = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_ID")?.Trim();
        var clientSecret = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_SECRET")?.Trim();
        var endpoint = Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT")?.Trim();

        if (
            clientId is null
            || clientSecret is null
            || endpoint is null
        )
        {
            return
            [
                new TheoryDataRow<string, string, string>(string.Empty, string.Empty, string.Empty) { Skip = "Authentication is not provided." },
            ];
        }

        return
        [
            new TheoryDataRow<string, string, string>(
                clientId,
                clientSecret,
                endpoint
            ),
        ];
    }
}
