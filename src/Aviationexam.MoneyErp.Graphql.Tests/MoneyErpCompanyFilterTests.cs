using Aviationexam.MoneyErp.Common.Filters;
using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
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
    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData), Explicit = false)]
    public Task QueryCompanyWithHashInFilterValueWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    ) => QueryCompanyWithSpecialCharInFilterValue(authenticationData, "TEST_HASH", "Street 80A # 17-85", '#');

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData), Explicit = false)]
    public Task QueryCompanyWithTildeInFilterValueWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    ) => QueryCompanyWithSpecialCharInFilterValue(authenticationData, "TEST_TILDE", "Street~80A~17", '~');

    [Theory]
    [ClassData(typeof(MoneyErpAuthenticationsClassData), Explicit = false)]
    public Task QueryCompanyWithPipeInFilterValueWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData
    ) => QueryCompanyWithSpecialCharInFilterValue(authenticationData, "TEST_PIPE", "Street 80A|17-85", '|');

    private async Task QueryCompanyWithSpecialCharInFilterValue(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData,
        string testCompanyKod,
        string testCompanyStreet,
        char specialChar
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

        // Step 2: Resolve country ID for CZ and numerical series
        var metadataFilter = new
        {
            countryFilter = FilterFor<Country>.Equal(m => m.Kod, "CZ").ToString(),
            numericalSeriesFilter = FilterFor<NumericalSerie>.Equal(m => m.Kod, "USER_ID").ToString(),
        };
        var metadataResponse = await graphqlClient.Query(
            metadataFilter,
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

        var country = Assert.Single(metadataResponse.Data!.Countries!
            .AsValueEnumerable()
            .Where(c => c!.Deleted is false)
            .ToArray());
        var countryId = country!.ID.AsGuid()!.Value;

        var numericalSerie = Assert.Single(metadataResponse.Data!.NumericalSeries!
            .AsValueEnumerable()
            .Where(c => c!.Deleted is false)
            .ToArray());
        var numericalSerieId = numericalSerie!.ID.AsGuid()!.Value;

        // Step 3: Query company with special char in FaktUlice value
        var companyWithSpecialCharFilter = new
        {
            companyFilter = FilterFor<Company>.And(
                    x => x.StartWith(m => m.Kod, testCompanyKod),
                    x => x.Equal(m => m.FaktUlice, testCompanyStreet)
                )
                .ToString(),
        };
        var companyResponse = await graphqlClient.Query(
            companyWithSpecialCharFilter,
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
            Assert.Equal(testCompanyStreet, existingCompany.FaktUlice);
            return;
        }

        // Step 4: Query without the special char field to distinguish
        // "filter broken by special char" vs "company doesn't exist"
        var fallbackFilter = new
        {
            companyFilter = FilterFor<Company>.StartWith(m => m.Kod, testCompanyKod).ToString(),
        };
        var fallbackResponse = await graphqlClient.Query(
            fallbackFilter,
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
            Assert.Fail(
                $"Company '{fallbackCompany.Kod}' with FaktUlice='{fallbackCompany.FaktUlice}' exists "
                + $"but was NOT found when '{specialChar}' was included in the filter value. "
                + $"This confirms the '{specialChar}' character in filter values is misinterpreted as an operator."
            );
        }

        // Step 5: Company doesn't exist — create it
        var testCompanyName = $"Test Company {specialChar}";
        var companyInput = new
        {
            companyInput = new CompanyInput
            {
                CiselnaRada_ID = numericalSerieId,
                Kod = testCompanyKod,
                Nazev = testCompanyName,
                PlatceDPH = false,
                FaktNazev = testCompanyName,
                FaktUlice = testCompanyStreet,
                FaktMisto = "Prague",
                FaktPsc = "11000",
                FaktStat_ID = countryId,
                ObchNazev = testCompanyName,
                ObchUlice = testCompanyStreet,
                ObchMisto = "Prague",
                ObchPsc = "11000",
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
        Assert.Equal(testCompanyStreet, createResponse.Data!.Company!.FaktUlice);

        // Step 6: Re-query with the special char filter
        var verifyResponse = await graphqlClient.Query(
            companyWithSpecialCharFilter,
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
        Assert.Equal(testCompanyStreet, foundCompany.FaktUlice);
    }
}
