using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using ZLinq;

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
                            FirmaStatNazevEn: "Germany",
                            ZpusobDopravyKod: "D",
                            ZpusobDopravyNazev: "Download",
                            ZpusobPlatbyKod: "PE",
                            ZpusobPlatbyNazev: "Online by Card",
                            Polozky:
                            [
                                new(
                                    Nazev: "PPL Fragenkatalog (DAeC) 12 Monate",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 1,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "PLU00167",
                                    SkladKod: "01",
                                    CelkovaCena: 1715.68m,
                                    CelkovaCenaCm: 69m,
                                    DphCelkem: 1715.68m,
                                    DphZaklad: 1441.67m,
                                    DphDan: 274.01m,
                                    DphCelkemCm: 69m,
                                    DphZakladCm: 57.98m,
                                    DphDanCm: 11.02m,
                                    DphSazba: 19
                                ),
                            ]
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
                            FirmaStatNazevEn: "Switzerland",
                            ZpusobDopravyKod: "D",
                            ZpusobDopravyNazev: "Download",
                            ZpusobPlatbyKod: "PE",
                            ZpusobPlatbyNazev: "Online by Card",
                            Polozky:
                            [
                                new(
                                    Nazev: "EASA -1 month subscription",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 2,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 1,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř26",
                                    TypCeny: 0,
                                    ArtiklPlu: "PLU00167",
                                    SkladKod: "01",
                                    CelkovaCena: 721.08m,
                                    CelkovaCenaCm: 29m,
                                    DphCelkem: 721.08m,
                                    DphZaklad: 721.08m,
                                    DphDan: 0,
                                    DphCelkemCm: 29m,
                                    DphZakladCm: 29m,
                                    DphDanCm: 0,
                                    DphSazba: 0
                                ),
                            ]
                        ),
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
                            FirmaStatNazevEn: "Hungary",
                            ZpusobDopravyKod: "D",
                            ZpusobDopravyNazev: "Download",
                            ZpusobPlatbyKod: "PE",
                            ZpusobPlatbyNazev: "Online by Card",
                            Polozky:
                            [
                                new(
                                    Nazev: "LMS Tariff fee-EASA-EN-3 months",
                                    Mnozstvi: 156,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 2,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 1,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř21",
                                    TypCeny: 0,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 83629.95m,
                                    CelkovaCenaCm: 3363.36m,
                                    DphCelkem: 83629.95m,
                                    DphZaklad: 83629.95m,
                                    DphDan: 0,
                                    DphCelkemCm: 3363.36m,
                                    DphZakladCm: 3363.36m,
                                    DphDanCm: 0,
                                    DphSazba: 0
                                ),
                            ]
                        ),
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
                            FirmaStatNazevEn: "Lithuania",
                            ZpusobDopravyKod: "D",
                            ZpusobDopravyNazev: "Download",
                            ZpusobPlatbyKod: "PE",
                            ZpusobPlatbyNazev: "Online by Card",
                            Polozky:
                            [
                                new(
                                    Nazev: "010 - Air Law eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 1,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 820.54m,
                                    CelkovaCenaCm: 33m,
                                    DphCelkem: 820.54m,
                                    DphZaklad: 678.07m,
                                    DphDan: 142.47m,
                                    DphCelkemCm: 33m,
                                    DphZakladCm: 27.27m,
                                    DphDanCm: 5.73m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "031 - Mass & Balance eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 2,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 820.54m,
                                    CelkovaCenaCm: 33m,
                                    DphCelkem: 820.54m,
                                    DphZaklad: 678.07m,
                                    DphDan: 142.47m,
                                    DphCelkemCm: 33m,
                                    DphZakladCm: 27.27m,
                                    DphDanCm: 5.73m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "040 - Human Performance & Limitations eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 3,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 969.73m,
                                    CelkovaCenaCm: 39m,
                                    DphCelkem: 969.73m,
                                    DphZaklad: 801.4m,
                                    DphDan: 168.33m,
                                    DphCelkemCm: 39m,
                                    DphZakladCm: 32.23m,
                                    DphDanCm: 6.77m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "050 - Meteorology eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 4,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 969.73m,
                                    CelkovaCenaCm: 39m,
                                    DphCelkem: 969.73m,
                                    DphZaklad: 801.4m,
                                    DphDan: 168.33m,
                                    DphCelkemCm: 39m,
                                    DphZakladCm: 32.23m,
                                    DphDanCm: 6.77m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "061 - General Navigation eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 5,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 969.73m,
                                    CelkovaCenaCm: 39m,
                                    DphCelkem: 969.73m,
                                    DphZaklad: 801.4m,
                                    DphDan: 168.33m,
                                    DphCelkemCm: 39m,
                                    DphZakladCm: 32.23m,
                                    DphDanCm: 6.77m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "062 - Radio Navigation eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 6,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 969.73m,
                                    CelkovaCenaCm: 39m,
                                    DphCelkem: 969.73m,
                                    DphZaklad: 801.4m,
                                    DphDan: 168.33m,
                                    DphCelkemCm: 39m,
                                    DphZakladCm: 32.23m,
                                    DphDanCm: 6.77m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "070 - Operational Procedures eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 7,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 820.54m,
                                    CelkovaCenaCm: 33m,
                                    DphCelkem: 820.54m,
                                    DphZaklad: 678.07m,
                                    DphDan: 142.47m,
                                    DphCelkemCm: 33m,
                                    DphZakladCm: 27.27m,
                                    DphDanCm: 5.73m,
                                    DphSazba: 21
                                ),
                                new(
                                    Nazev: "090 - Communications eTextbook",
                                    Mnozstvi: 1,
                                    DphEditovanoRucne: true,
                                    DruhSazbyDph: 1,
                                    PredkontaceKod: "FV002",
                                    Jednotka: "pcs",
                                    CisloPolozky: 8,
                                    TypObsahu: 1,
                                    CleneniDphKod: "19Ř24OSS_S",
                                    TypCeny: 1,
                                    ArtiklPlu: "ART00001",
                                    SkladKod: "01",
                                    CelkovaCena: 820.54m,
                                    CelkovaCenaCm: 33m,
                                    DphCelkem: 820.54m,
                                    DphZaklad: 678.07m,
                                    DphDan: 142.47m,
                                    DphCelkemCm: 33m,
                                    DphZakladCm: 27.27m,
                                    DphDanCm: 5.73m,
                                    DphSazba: 21
                                ),
                            ]
                        ),
                    ]
                )
                {
                    Skip = authentication.Skip,
                };
            }
        }
    }

    public sealed record InvoiceItemData(
        string Nazev,
        decimal Mnozstvi,
        bool DphEditovanoRucne,
        int DruhSazbyDph,
        string PredkontaceKod,
        string Jednotka,
        int CisloPolozky,
        int TypObsahu,
        string CleneniDphKod,
        int TypCeny,
        string ArtiklPlu,
        string SkladKod,
        decimal CelkovaCena,
        decimal CelkovaCenaCm,
        decimal DphCelkem,
        decimal DphZaklad,
        decimal DphDan,
        decimal DphCelkemCm,
        decimal DphZakladCm,
        decimal DphDanCm,
        decimal DphSazba
    );

    public sealed record InvoiceAddressData(
        string? Email,
        string? Telefon,
        string Ulice,
        string Misto,
        string Psc,
        string Stat,
        string Nazev
    );

    public sealed record InvoiceData(
        // Basic identifiers
        string CisloDokladu,
        string OdkazNaDoklad,
        string VariabilniSymbol,

        // Dates
        DateTimeOffset DatumVystaveni,
        DateTimeOffset DatumUcetnihoPripadu,
        DateTimeOffset DatumPlneni,
        DateTimeOffset DatumSplatnosti,

        // General info
        string Vystavil,
        string Nazev,
        string? Dic,

        // Money & currency
        decimal CelkovaCastkaCm,
        decimal Kurz,
        int KurzMnozstvi,
        string MenaKod,

        // VAT classification
        string CleneniDphKod,

        // NEW — <Group Kod="…"/>
        string GroupKod, // <<< added

        // Supplier (Adresa/Firma)
        string FirmaKod,
        bool FirmaPlatceDph,
        string? FirmaDic,
        string FirmaNazev,
        string FirmaUlice,
        string FirmaMisto,
        string FirmaKodPsc,
        string FirmaNazevStatu,
        string FirmaStatKod,
        string FirmaStatNazevEn,

        // NEW – recipient addresses
        InvoiceAddressData AdresaPrijemceFaktury, // <<< added
        InvoiceAddressData AdresaKoncovehoPrijemce, // <<< added

        // Transport & payment
        string ZpusobDopravyKod,
        string ZpusobDopravyNazev,
        string ZpusobPlatbyKod,
        string ZpusobPlatbyNazev,

        // Items
        InvoiceItemData[] Polozky
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
            FirmaStatNazevEn,
            ZpusobDopravyKod,
            ZpusobDopravyNazev,
            ZpusobPlatbyKod,
            ZpusobPlatbyNazev,
            new JsonArray(Polozky.AsValueEnumerable().Select(JsonNode (x) => new JsonObject
            {
                [nameof(x.Nazev)] = x.Nazev,
                [nameof(x.Mnozstvi)] = x.Mnozstvi,
                [nameof(x.DphEditovanoRucne)] = x.DphEditovanoRucne,
                [nameof(x.DruhSazbyDph)] = x.DruhSazbyDph,
                [nameof(x.PredkontaceKod)] = x.PredkontaceKod,
                [nameof(x.Jednotka)] = x.Jednotka,
                [nameof(x.CisloPolozky)] = x.CisloPolozky,
                [nameof(x.TypObsahu)] = x.TypObsahu,
                [nameof(x.CleneniDphKod)] = x.CleneniDphKod,
                [nameof(x.TypCeny)] = x.TypCeny,
                [nameof(x.ArtiklPlu)] = x.ArtiklPlu,
                [nameof(x.SkladKod)] = x.SkladKod,
                [nameof(x.CelkovaCena)] = x.CelkovaCena,
                [nameof(x.CelkovaCenaCm)] = x.CelkovaCenaCm,
                [nameof(x.DphCelkem)] = x.DphCelkem,
                [nameof(x.DphZaklad)] = x.DphZaklad,
                [nameof(x.DphDan)] = x.DphDan,
                [nameof(x.DphCelkemCm)] = x.DphCelkemCm,
                [nameof(x.DphZakladCm)] = x.DphZakladCm,
                [nameof(x.DphDanCm)] = x.DphDanCm,
                [nameof(x.DphSazba)] = x.DphSazba,
            }).ToArray())
        ).ToString();

        public static InvoiceData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 30 } arr)
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
                FirmaStatNazevEn: arr[24]?.GetValue<string>() ?? throw new FormatException("FirmaStatNazevEN missing."),
                ZpusobDopravyKod: arr[25]?.GetValue<string>() ?? throw new FormatException("ZpusobDopravyKod missing."),
                ZpusobDopravyNazev: arr[26]?.GetValue<string>() ?? throw new FormatException("ZpusobDopravyNazev missing."),
                ZpusobPlatbyKod: arr[27]?.GetValue<string>() ?? throw new FormatException("ZpusobPlatbyKod missing."),
                ZpusobPlatbyNazev: arr[28]?.GetValue<string>() ?? throw new FormatException("ZpusobPlatbyNazev missing."),
                Polozky: arr[29]?.AsArray().Select(x => new InvoiceItemData(
                    Nazev: x[nameof(InvoiceItemData.Nazev)]?.GetValue<string>() ?? throw new FormatException("Nazev missing."),
                    Mnozstvi: x[nameof(InvoiceItemData.Mnozstvi)]?.GetValue<decimal>() ?? throw new FormatException("Mnozstvi missing."),
                    DphEditovanoRucne: x[nameof(InvoiceItemData.DphEditovanoRucne)]?.GetValue<bool>() ?? throw new FormatException("DphEditovanoRucne missing."),
                    DruhSazbyDph: x[nameof(InvoiceItemData.DruhSazbyDph)]?.GetValue<int>() ?? throw new FormatException("DruhSazbyDph missing."),
                    PredkontaceKod: x[nameof(InvoiceItemData.PredkontaceKod)]?.GetValue<string>() ?? throw new FormatException("PredkontaceKod missing."),
                    Jednotka: x[nameof(InvoiceItemData.Jednotka)]?.GetValue<string>() ?? throw new FormatException("Jednotka missing."),
                    CisloPolozky: x[nameof(InvoiceItemData.CisloPolozky)]?.GetValue<int>() ?? throw new FormatException("CisloPolozky missing."),
                    TypObsahu: x[nameof(InvoiceItemData.TypObsahu)]?.GetValue<int>() ?? throw new FormatException("TypObsahu missing."),
                    CleneniDphKod: x[nameof(InvoiceItemData.CleneniDphKod)]?.GetValue<string>() ?? throw new FormatException("CleneniDphKod missing."),
                    TypCeny: x[nameof(InvoiceItemData.TypCeny)]?.GetValue<int>() ?? throw new FormatException("TypCeny missing."),
                    ArtiklPlu: x[nameof(InvoiceItemData.ArtiklPlu)]?.GetValue<string>() ?? throw new FormatException("ArtiklPlu missing."),
                    SkladKod: x[nameof(InvoiceItemData.SkladKod)]?.GetValue<string>() ?? throw new FormatException("SkladKod missing."),
                    CelkovaCena: x[nameof(InvoiceItemData.CelkovaCena)]?.GetValue<decimal>() ?? throw new FormatException("CelkovaCena missing."),
                    CelkovaCenaCm: x[nameof(InvoiceItemData.CelkovaCenaCm)]?.GetValue<decimal>() ?? throw new FormatException("CelkovaCenaCm missing."),
                    DphCelkem: x[nameof(InvoiceItemData.DphCelkem)]?.GetValue<decimal>() ?? throw new FormatException("DphCelkem missing."),
                    DphZaklad: x[nameof(InvoiceItemData.DphZaklad)]?.GetValue<decimal>() ?? throw new FormatException("DphZaklad missing."),
                    DphDan: x[nameof(InvoiceItemData.DphDan)]?.GetValue<decimal>() ?? throw new FormatException("DphDan missing."),
                    DphCelkemCm: x[nameof(InvoiceItemData.DphCelkemCm)]?.GetValue<decimal>() ?? throw new FormatException("DphCelkemCm missing."),
                    DphZakladCm: x[nameof(InvoiceItemData.DphZakladCm)]?.GetValue<decimal>() ?? throw new FormatException("DphZakladCm missing."),
                    DphDanCm: x[nameof(InvoiceItemData.DphDanCm)]?.GetValue<decimal>() ?? throw new FormatException("DphDanCm missing."),
                    DphSazba: x[nameof(InvoiceItemData.DphSazba)]?.GetValue<decimal>() ?? throw new FormatException("DphSazba missing.")
                )).ToArray() ?? throw new FormatException("Polozky missing.")
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
