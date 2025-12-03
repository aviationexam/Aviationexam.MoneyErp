using Aviationexam.MoneyErp.RestApi.ClientV1;
using Aviationexam.MoneyErp.RestApi.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.MoneyErp.RestApi.Tests;

public class MoneyErpApiClientsTests
{
    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetArticleWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

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
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

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
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.Centre.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetCompanyWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

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
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

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
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

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
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.IssuedOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetJobOrderWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.JobOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetOperationWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.Operation.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPersonWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.Person.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPrepaymentInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.PrepaymentInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetPrepaymentIssuedInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.PrepaymentIssuedInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetReceivedInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.ReceivedInvoice.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetReceivedOrderWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.ReceivedOrder.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetStaffWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.Staff.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetTypeOfActivityWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.TypeOfActivity.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetActivityWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiV1Client>();

        var responses = await client.V10.Activity.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.Empty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }
}
