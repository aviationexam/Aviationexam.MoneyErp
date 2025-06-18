using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.MoneyErp.Tests;

public class MoneyErpApiClientTests
{
    [Theory]
    [MemberData(nameof(MoneyErpAuthentications))]
    public async Task TestAuthenticationAsync(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = new ServiceCollection()
            .AddMoneyErpApiClient(builder => builder.Configure(x =>
            {
                x.ClientId = clientId;
                x.ClientSecret = clientSecret;
                x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
                x.Endpoint = new Uri(serverAddress, UriKind.RelativeOrAbsolute);
            }))
            .BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        await client.V10.Person.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
    }

    public static TheoryData<string, string, string> MoneyErpAuthentications()
    {
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
