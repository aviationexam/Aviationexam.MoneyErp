using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public class MoneyErpGetVersionTests(
    ITestOutputHelper testOutputHelper
)
{
    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData))]
    public async Task GetVersionWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(
            testOutputHelper, authenticationData!, shouldRedactHeaderValue: true
        );

        var graphqlClient = serviceProvider.GetRequiredService<MoneyErpGraphqlClient>();

        var version = await graphqlClient.Query(x => x.Version, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(version.Data);
        Assert.NotEmpty(version.Data);
    }
}
