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
        InvoiceData[] invoiceData
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

    private class MoneyErpInvoiceClassData() : TheoryData<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>(
        GetData()
    )
    {
        private static IEnumerable<TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>> GetData()
        {
            foreach (var authentication in MoneyErpAuthenticationsClassData.GetData())
            {
                // Invoice 1: EX_20250629_0245_OR2516216_OR2516222.xml, first FakturaVydana
                yield return new TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>(
                    authentication.Data,
                    [
                        new InvoiceData(
                            CisloDokladu: "IN2504259",
                            OdkazNaDoklad: "OR2516216",
                            VariabilniSymbol: "0002516216",
                            DatumVystaveni: DateTimeOffset.Parse("2025-07-01T17:12:18"),
                            DatumUcetnihoPripadu: DateTimeOffset.Parse("2025-07-01T17:12:18"),
                            DatumPlneni: DateTimeOffset.Parse("2025-07-01T17:12:18"),
                            DatumSplatnosti: DateTimeOffset.Parse("2025-07-01T17:12:18"),
                            Vystavil: "Aviationexam",
                            Nazev: "eShop import",
                            Dic: null,
                            CelkovaCastkaCm: 69m,
                            Kurz: 24.865m,
                            KurzMnozstvi: 1,
                            MenaKod: "EUR",
                            CleneniDphKod: "19Ř24OSS_S",
                            FirmaKod: "UID786464",
                            FirmaPlatceDph: false,
                            FirmaDic: null,
                            FirmaNazev: "John Doe",
                            FirmaUlice: "Street 1",
                            FirmaMisto: "Roding Strahlfeld",
                            FirmaKodPsc: "93426",
                            FirmaNazevStatu: "Germany",
                            FirmaStatKod: "DE",
                            FirmaStatNazevEn: "Germany"
                        ),
                        new InvoiceData(
                            CisloDokladu: "IN2504261",
                            OdkazNaDoklad: "OR2516222",
                            VariabilniSymbol: "0002516222",
                            DatumVystaveni: DateTimeOffset.Parse("2025-07-01T22:01:16"),
                            DatumUcetnihoPripadu: DateTimeOffset.Parse("2025-07-01T22:01:16"),
                            DatumPlneni: DateTimeOffset.Parse("2025-07-01T22:01:16"),
                            DatumSplatnosti: DateTimeOffset.Parse("2025-07-01T22:01:16"),
                            Vystavil: "Aviationexam",
                            Nazev: "eShop import",
                            Dic: null,
                            CelkovaCastkaCm: 29m,
                            Kurz: 24.865m,
                            KurzMnozstvi: 1,
                            MenaKod: "EUR",
                            CleneniDphKod: "19Ř26",
                            FirmaKod: "UID819953",
                            FirmaPlatceDph: false,
                            FirmaDic: null,
                            FirmaNazev: "John Doe",
                            FirmaUlice: "Street 1",
                            FirmaMisto: "Arbaz",
                            FirmaKodPsc: "1974",
                            FirmaNazevStatu: "Switzerland",
                            FirmaStatKod: "CH",
                            FirmaStatNazevEn: "Switzerland"
                        )
                    ]
                )
                {
                    Skip = authentication.Skip,
                };

                // Invoice 3: EX_20250630_0245_OR2516237.xml
                yield return new TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>(
                    authentication.Data,
                    [
                        new InvoiceData(
                            CisloDokladu: "IN2504269",
                            OdkazNaDoklad: "OR2516237",
                            VariabilniSymbol: "0002516237",
                            DatumVystaveni: DateTimeOffset.Parse("2025-07-01T17:39:10"),
                            DatumUcetnihoPripadu: DateTimeOffset.Parse("2025-07-01T17:39:10"),
                            DatumPlneni: DateTimeOffset.Parse("2025-07-01T17:39:10"),
                            DatumSplatnosti: DateTimeOffset.Parse("2025-07-01T17:39:10"),
                            Vystavil: "Aviationexam",
                            Nazev: "eShop import",
                            Dic: "HU26524162",
                            CelkovaCastkaCm: 3363.36m,
                            Kurz: 24.865m,
                            KurzMnozstvi: 1,
                            MenaKod: "EUR",
                            CleneniDphKod: "19Ř21",
                            FirmaKod: "LMS28119",
                            FirmaPlatceDph: true,
                            FirmaDic: "HU26524162",
                            FirmaNazev: "Easy",
                            FirmaUlice: "Street 1",
                            FirmaMisto: "Csomad",
                            FirmaKodPsc: "2161",
                            FirmaNazevStatu: "Hungary",
                            FirmaStatKod: "HU",
                            FirmaStatNazevEn: "Hungary"
                        )
                    ]
                )
                {
                    Skip = authentication.Skip,
                };

                // Invoice 4: EX_20250701_0245_ OR2516263.xml
                yield return new TheoryDataRow<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>(
                    authentication.Data,
                    [
                        new InvoiceData(
                            CisloDokladu: "IN2504279",
                            OdkazNaDoklad: "OR2516263",
                            VariabilniSymbol: "0002516263",
                            DatumVystaveni: DateTimeOffset.Parse("2025-07-02T09:52:28"),
                            DatumUcetnihoPripadu: DateTimeOffset.Parse("2025-07-02T09:52:28"),
                            DatumPlneni: DateTimeOffset.Parse("2025-07-02T09:52:28"),
                            DatumSplatnosti: DateTimeOffset.Parse("2025-07-02T09:52:28"),
                            Vystavil: "Aviationexam",
                            Nazev: "eShop import",
                            Dic: null,
                            CelkovaCastkaCm: 288m,
                            Kurz: 24.865m,
                            KurzMnozstvi: 1,
                            MenaKod: "EUR",
                            CleneniDphKod: "19Ř24OSS_S",
                            FirmaKod: "UID233969",
                            FirmaPlatceDph: false,
                            FirmaDic: null,
                            FirmaNazev: "John Doe",
                            FirmaUlice: "Street 1",
                            FirmaMisto: "Vilnius",
                            FirmaKodPsc: "11000",
                            FirmaNazevStatu: "Lithuania",
                            FirmaStatKod: "LT",
                            FirmaStatNazevEn: "Lithuania"
                        )
                    ]
                )
                {
                    Skip = authentication.Skip,
                };
            }
        }
    }

    public sealed record InvoiceData(
        string CisloDokladu,
        string OdkazNaDoklad,
        string VariabilniSymbol,
        DateTimeOffset DatumVystaveni,
        DateTimeOffset DatumUcetnihoPripadu,
        DateTimeOffset DatumPlneni,
        DateTimeOffset DatumSplatnosti,
        string Vystavil,
        string Nazev,
        string? Dic,
        decimal CelkovaCastkaCm,
        decimal Kurz,
        int KurzMnozstvi,
        string MenaKod,
        string CleneniDphKod,
        string FirmaKod,
        bool FirmaPlatceDph,
        string? FirmaDic,
        string FirmaNazev,
        string FirmaUlice,
        string FirmaMisto,
        string FirmaKodPsc,
        string FirmaNazevStatu,
        string FirmaStatKod,
        string FirmaStatNazevEn
    ) : IFormattable, IParsable<InvoiceData>
    {
        public string ToString(string? format, IFormatProvider? formatProvider) => new JsonArray(
            CisloDokladu,
            OdkazNaDoklad,
            VariabilniSymbol,
            DatumVystaveni,
            DatumUcetnihoPripadu,
            DatumPlneni,
            DatumSplatnosti,
            Vystavil,
            Nazev,
            Dic,
            CelkovaCastkaCm,
            Kurz,
            KurzMnozstvi,
            MenaKod,
            CleneniDphKod,
            FirmaKod,
            FirmaPlatceDph,
            FirmaDic,
            FirmaNazev,
            FirmaUlice,
            FirmaMisto,
            FirmaKodPsc,
            FirmaNazevStatu,
            FirmaStatKod,
            FirmaStatNazevEn
        ).ToString();

        public static InvoiceData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 24 } arr)
            {
                throw new FormatException("Input string is not a valid InvoiceData JSON array.");
            }

            return new InvoiceData(
                CisloDokladu: arr[0]?.GetValue<string>() ?? throw new FormatException("CisloDokladu missing."),
                OdkazNaDoklad: arr[1]?.GetValue<string>() ?? throw new FormatException("OdkazNaDoklad missing."),
                VariabilniSymbol: arr[2]?.GetValue<string>() ?? throw new FormatException("VariabilniSymbol missing."),
                DatumVystaveni: arr[3]?.GetValue<DateTimeOffset>() ?? throw new FormatException("DatumVystaveni missing."),
                DatumUcetnihoPripadu: arr[4]?.GetValue<DateTimeOffset>() ?? throw new FormatException("DatumUcetnihoPripadu missing."),
                DatumPlneni: arr[5]?.GetValue<DateTimeOffset>() ?? throw new FormatException("DatumPlneni missing."),
                DatumSplatnosti: arr[6]?.GetValue<DateTimeOffset>() ?? throw new FormatException("DatumSplatnosti missing."),
                Vystavil: arr[7]?.GetValue<string>() ?? throw new FormatException("Vystavil missing."),
                Nazev: arr[8]?.GetValue<string>() ?? throw new FormatException("Nazev missing."),
                Dic: arr[9]?.GetValue<string?>(),
                CelkovaCastkaCm: arr[10]?.GetValue<decimal>() ?? throw new FormatException("CelkovaCastkaCM missing."),
                Kurz: arr[11]?.GetValue<decimal>() ?? throw new FormatException("Kurz missing."),
                KurzMnozstvi: arr[12]?.GetValue<int>() ?? throw new FormatException("KurzMnozstvi missing."),
                MenaKod: arr[13]?.GetValue<string>() ?? throw new FormatException("MenaKod missing."),
                CleneniDphKod: arr[14]?.GetValue<string>() ?? throw new FormatException("CleneniDPHKod missing."),
                FirmaKod: arr[15]?.GetValue<string>() ?? throw new FormatException("FirmaKod missing."),
                FirmaPlatceDph: arr[16]?.GetValue<bool>() ?? throw new FormatException("FirmaPlatceDPH missing."),
                FirmaDic: arr[17]?.GetValue<string?>(),
                FirmaNazev: arr[18]?.GetValue<string>() ?? throw new FormatException("FirmaNazev missing."),
                FirmaUlice: arr[19]?.GetValue<string>() ?? throw new FormatException("FirmaUlice missing."),
                FirmaMisto: arr[20]?.GetValue<string>() ?? throw new FormatException("FirmaMisto missing."),
                FirmaKodPsc: arr[21]?.GetValue<string>() ?? throw new FormatException("FirmaKodPsc missing."),
                FirmaNazevStatu: arr[22]?.GetValue<string>() ?? throw new FormatException("FirmaNazevStatu missing."),
                FirmaStatKod: arr[23]?.GetValue<string>() ?? throw new FormatException("FirmaStatKod missing."),
                FirmaStatNazevEn: arr[23]?.GetValue<string>() ?? throw new FormatException("FirmaStatNazevEN missing.")
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
