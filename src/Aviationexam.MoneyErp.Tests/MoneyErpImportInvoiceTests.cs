using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.MoneyErp.Tests;

public class MoneyErpImportInvoiceTests
{
    [Theory]
    [ClassData(typeof(MoneyErpInvoiceClassData))]
    public async Task ImportInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData,
        InvoiceData invoiceData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!);

        var client = serviceProvider.GetRequiredService<MoneyErpApiClient>();

        var responses = await client.V10.Article.GetAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responses);
        Assert.Equal(1, responses.Status);
        Assert.NotNull(responses.Data);
        Assert.NotEmpty(responses.Data);

        Assert.Empty(responses.AdditionalData);
        Assert.All(responses.Data, x => Assert.Empty(x.AdditionalData));
    }

    private class MoneyErpInvoiceClassData() : TheoryData<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData>(
        GetData()
    )
    {
        private static IEnumerable<TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData>> GetData()
        {
            foreach (var authentication in MoneyErpAuthenticationsClassData.GetData())
            {
                yield return new TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData>(
                    authentication.Data,
                    new InvoiceData()
                )
                {
                    Skip = authentication.Skip,
                };
            }
        }
    }

    public sealed record InvoiceData(
    ) : IFormattable, IParsable<InvoiceData>
    {
        public string ToString(string? format, IFormatProvider? formatProvider) => new JsonArray(
        ).ToString();

        public static InvoiceData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 3 } arr)
            {
                throw new FormatException("Input string is not a valid AuthenticationData JSON array.");
            }

            return new InvoiceData(
            );
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out InvoiceData result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
