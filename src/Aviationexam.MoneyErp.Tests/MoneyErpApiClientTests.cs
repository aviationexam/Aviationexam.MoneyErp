using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.Extensions;
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
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetArticleWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Article.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetBankStatementWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.BankStatement.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        //Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetCentreWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Centre.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetCompanyWorks(
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

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetConnectionWorks(
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
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetIssuedInvoiceWorks(
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
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetIssuedOrderWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.IssuedOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetJobOrderWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.JobOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetOperationWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Operation.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPersonWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Person.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        //Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPrepaymentInvoiceWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.PrepaymentInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        //Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPrepaymentIssuedInvoiceWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.PrepaymentIssuedInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetReceivedInvoiceWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.ReceivedInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetReceivedOrderWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.ReceivedOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetStaffWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Staff.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetTypeOfActivityWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.TypeOfActivity.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        //Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetActivityWorks(
        string clientId,
        string clientSecret,
        string serverAddress
    )
    {
        await using var serviceProvider = BuildServiceProvider(clientId, clientSecret, serverAddress);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Activity.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
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
        }), shouldRedactHeaderValue: false)
        .BuildServiceProvider();
}
