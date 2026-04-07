using Aviationexam.MoneyErp.Common.Filters;
using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using ZLinq;

namespace Aviationexam.MoneyErp.Graphql.Tests;

/// <summary>
/// Integration test verifying that company filter queries work correctly
/// when filter values contain special characters (#, ~, |) that are also
/// used as operators in the MoneyERP filter DSL.
/// </summary>
public class MoneyErpCompanyFilterTests(
    ITestOutputHelper testOutputHelper
)
{
    private const string TestCompanyKod = "TEST_SPECIAL_CHAR";
    private const string TestCompanyStreet = "Street 80A # 17-85";
    private const string TestCompanyName = "Test Company Special Chars";
    private const string TestCompanyTown = "Prague";
    private const string TestCompanyZip = "11000";

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData), Explicit = false)]
    public async Task QueryCompanyWithHashInFilterValueWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(
            authenticationData!, testOutputHelper, shouldRedactHeaderValue: true
        );

        var graphqlClient = serviceProvider.GetRequiredService<MoneyErpGraphqlClient>();

        // Step 1: Verify connectivity
        var version = await graphqlClient.Query(
            x => x.Version,
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.NotNull(version.Data);
        Assert.NotEmpty(version.Data);

        // Step 2: Resolve country ID for CZ
        var countryFilter = new
        {
            countryFilter = FilterFor<Country>.Equal(m => m.Kod, "CZ").ToString(),
            numericalSeriesFilter = FilterFor<NumericalSerie>.Equal(m => m.Kod, "USER_ID").ToString(),
        };
        var metadataResponse = await graphqlClient.Query(
            countryFilter,
            static (f, x) => new
            {
                Countries = x.Countries(
                    Filter: f.countryFilter,
                    selector: c => new { c.ID, c.Deleted, c.Kod }
                ),
                NumericalSeries = x.NumericalSeries(
                    Filter: f.numericalSeriesFilter,
                    selector: c => new { c.ID, c.Deleted, c.Kod }
                ),
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Empty(metadataResponse.Errors ?? []);

        var countryId = Assert.Single(
            metadataResponse.Data!.Countries!,
            c => c!.Deleted is false
        )!.ID.AsGuid()!.Value;

        var numericalSerieId = Assert.Single(
            metadataResponse.Data!.NumericalSeries!, c => c!.Deleted is false
        )!.ID.AsGuid()!.Value;

        // Step 3: Query company with '#' in FaktUlice value
        var companyWithHashFilter = new
        {
            companyFilter = FilterFor<Company>.And(
                    x => x.StartWith(m => m.Kod, TestCompanyKod),
                    x => x.Equal(m => m.FaktUlice, TestCompanyStreet),
                    x => x.Equal(m => m.FaktMisto, TestCompanyTown)
                )
                .ToString(),
        };
        var companyResponse = await graphqlClient.Query(
            companyWithHashFilter,
            static (f, x) => new
            {
                Companies = x.Companies(
                    Filter: f.companyFilter,
                    selector: c => new { c.ID, c.Deleted, c.Kod, c.FaktUlice }
                ),
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Empty(companyResponse.Errors ?? []);

        var existingCompany = companyResponse.Data?.Companies?
            .AsValueEnumerable()
            .Where(c => c!.Deleted is false)
            .FirstOrDefault();

        if (existingCompany is not null)
        {
            // Company already exists with '#' in street - the filter query works
            Assert.Equal(TestCompanyStreet, existingCompany.FaktUlice);
            return;
        }

        // Step 4: Verify the filter isn't silently broken — query without '#' to distinguish
        // "filter returned nothing because of escaping bug" vs "company doesn't exist"
        var companyWithoutHashFilter = new
        {
            companyFilter = FilterFor<Company>.And(
                    x => x.StartWith(m => m.Kod, TestCompanyKod),
                    x => x.Equal(m => m.FaktMisto, TestCompanyTown)
                )
                .ToString(),
        };
        var fallbackResponse = await graphqlClient.Query(
            companyWithoutHashFilter,
            static (f, x) => new
            {
                Companies = x.Companies(
                    Filter: f.companyFilter,
                    selector: c => new { c.ID, c.Deleted, c.Kod, c.FaktUlice }
                ),
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Empty(fallbackResponse.Errors ?? []);

        var fallbackCompany = fallbackResponse.Data?.Companies?
            .AsValueEnumerable()
            .Where(c => c!.Deleted is false)
            .FirstOrDefault();

        if (fallbackCompany is not null)
        {
            // Company exists but the '#' filter didn't find it — escaping bug confirmed
            Assert.Fail(
                $"Company '{fallbackCompany.Kod}' with FaktUlice='{fallbackCompany.FaktUlice}' exists "
                + "but was NOT found when '#' was included in the filter value. "
                + "This confirms the '#' character in filter values is misinterpreted as the AND operator."
            );
        }

        // Step 5: Company doesn't exist — create it with '#' in street
        var companyInput = new
        {
            companyInput = new CompanyInput
            {
                CiselnaRada_ID = numericalSerieId,
                Kod = TestCompanyKod,
                Nazev = TestCompanyName,
                PlatceDPH = false,
                FaktNazev = TestCompanyName,
                FaktUlice = TestCompanyStreet,
                FaktMisto = TestCompanyTown,
                FaktPsc = TestCompanyZip,
                FaktStat_ID = countryId,
                ObchNazev = TestCompanyName,
                ObchUlice = TestCompanyStreet,
                ObchMisto = TestCompanyTown,
                ObchPsc = TestCompanyZip,
                ObchStat_ID = countryId,
            },
        };
        var createResponse = await graphqlClient.Mutation(
            companyInput,
            static (f, x) => new
            {
                Company = x.CreateCompany(
                    f.companyInput,
                    selector: c => new { c.ID, c.Deleted, c.Kod, c.FaktUlice }
                ),
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Empty(createResponse.Errors ?? []);
        Assert.NotNull(createResponse.Data?.Company);
        Assert.Equal(TestCompanyStreet, createResponse.Data!.Company!.FaktUlice);

        // Step 6: Re-query with the '#' filter — this is the real test
        var verifyResponse = await graphqlClient.Query(
            companyWithHashFilter,
            static (f, x) => new
            {
                Companies = x.Companies(
                    Filter: f.companyFilter,
                    selector: c => new { c.ID, c.Deleted, c.Kod, c.FaktUlice }
                ),
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Empty(verifyResponse.Errors ?? []);

        var foundCompany = verifyResponse.Data?.Companies?
            .AsValueEnumerable()
            .Where(c => c!.Deleted is false)
            .FirstOrDefault();

        Assert.NotNull(foundCompany);
        Assert.Equal(TestCompanyStreet, foundCompany.FaktUlice);
    }
}
