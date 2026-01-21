using Aviationexam.MoneyErp.Common.Filters;
using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using ZLinq;
using Guid = System.Guid;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public class MoneyErpImportInvoiceTests(
    ITestOutputHelper testOutputHelper
)
{
    [Theory]
    [ClassData(typeof(MoneyErpInvoiceClassData), Explicit = true)]
    public async Task ImportInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData,
        InvoiceData[] invoiceData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(
            testOutputHelper, authenticationData!, shouldRedactHeaderValue: true
        );

        var graphqlClient = serviceProvider.GetRequiredService<MoneyErpGraphqlClient>();

        var version = await graphqlClient.Query(x => x.Version, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(version.Data);
        Assert.NotEmpty(version.Data);

        ICollection<(
            IReadOnlyDictionary<string, Guid> numericalSeries,
            Guid? companyGuid,
            Guid? companyInvoiceReceiverGuid,
            Guid? companyTerminalReceiverGuid,
            Guid? currencyId,
            IReadOnlyDictionary<string, Guid> countryIds,
            Guid? vatClassificationId,
            Guid? invoiceGroupId,
            Guid? transportId,
            Guid? paymentId,
            IReadOnlyDictionary<string, Guid> accountAssignmentIds,
            IReadOnlyDictionary<string, Guid> vatClassificationIds,
            IReadOnlyDictionary<string, Guid> articles,
            IReadOnlyDictionary<string, Guid> warehouses
            )> ids = [];
        foreach (var data in invoiceData)
        {
            var filters = new
            {
                numericalSeriesFilter = FilterFor<NumericalSerie>.Or(
                        x => x.Equal(m => m.Kod, data.CisloDokladuNumericSerieKod),
                        x => x.Equal(m => m.Kod, data.FirmaKodNumericSerieKod)
                    )
                    .ToString(),
                currencyFilter = FilterFor<Currency>.Equal(m => m.Kod, data.MenaKod).ToString(),
                countriesFilter = FilterFor<Country>.Or(
                        EFilterOperator.Equal,
                        m => m.Kod,
                        new[]
                            {
                                data.FirmaStatKod,
                                data.AdresaPrijemceFaktury.StatKod,
                                data.AdresaKoncovehoPrijemce.StatKod,
                            }.AsValueEnumerable()
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct()
                            .ToList()
                    )
                    .ToString(),
                varClassificationFilter = FilterFor<VATClassification>.Equal(m => m.Kod, data.CleneniDphKod).ToString(),
                invoiceGroupFilter = FilterFor<MerpGroup>.Equal(m => m.Kod, data.GroupKod).ToString(),
                transportFilter = FilterFor<TransportType>.Equal(m => m.Kod, data.ZpusobDopravyKod).ToString(),
                paymentFilter = FilterFor<PaymentType>.Equal(m => m.Kod, data.ZpusobPlatbyKod).ToString(),
                accountAssignmentsFilter = FilterFor<AccountAssignment>.Or(
                        EFilterOperator.Equal,
                        m => m.Kod,
                        data.Polozky.AsValueEnumerable()
                            .Select(x => x.PredkontaceKod)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct()
                            .ToList()
                    )
                    .ToString(),
                varClassificationsFilter = FilterFor<VATClassification>.Or(
                        EFilterOperator.Equal,
                        m => m.Kod,
                        data.Polozky.AsValueEnumerable()
                            .Select(x => x.CleneniDphKod)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct()
                            .ToList()
                    )
                    .ToString(),
                artiklPluFilter = FilterFor<Article>.Or(
                        EFilterOperator.Equal,
                        m => m.PLU,
                        data.Polozky.AsValueEnumerable()
                            .Select(x => x.ArtiklPlu)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct()
                            .ToList()
                    )
                    .ToString(),
                warehouseFilter = FilterFor<Warehouse>.Or(
                        EFilterOperator.Equal,
                        m => m.Kod,
                        data.Polozky.AsValueEnumerable()
                            .Select(x => x.SkladKod)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct()
                            .ToList()
                    )
                    .ToString(),
                connectionsTypesFilter = FilterFor<ConnectionsType>.Or(
                        x => x.Equal(m => m.Kod, "E-mail"),
                        x => x.Equal(m => m.Kod, "Mob"),
                        x => x.Equal(m => m.Kod, "Tel")
                    )
                    .ToString(),
                // invoiceReceiverEmailFilter = FilterFor<Connection>.Equal(m => m.Vychozi, data.AdresaPrijemceFaktury.Email).ToString(),
                // receiverEmailFilter = FilterFor<Connection>.Equal(m => m.Vychozi, data.AdresaKoncovehoPrijemce.Email).ToString(),
                // invoiceReceiverPhoneFilter = FilterFor<Connection>.Equal(m => m.Vychozi, data.AdresaPrijemceFaktury.Telefon).ToString(),
                // receiverPhoneFilter = FilterFor<Connection>.Equal(m => m.Vychozi, data.AdresaKoncovehoPrijemce.Telefon).ToString(),
                // invoiceReceiverAddressFilter = FilterFor<Adresar_Firma>.And(
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Email),
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Telefon),
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Ulice),
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Misto),
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Psc),
                //     x => x.Equal(m => m.Kod, data.AdresaPrijemceFaktury.Nazev)
                // ),
                // receiverAddressFilter = FilterFor<PaymentType>.And(
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Email),
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Telefon),
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Ulice),
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Misto),
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Psc),
                //     x => x.Equal(m => m.Kod, data.AdresaKoncovehoPrijemce.Nazev)
                // ),
            };

            var graphResponse = await graphqlClient.Query(
                filters,
                static (f, x) => new
                {
                    NumericalSeries = x.NumericalSeries(Filter: f.numericalSeriesFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev, c.Prefix }),
                    Currency = x.Currencies(Filter: f.currencyFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    Countries = x.Countries(Filter: f.countriesFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    VATClassification = x.VATClassifications(Filter: f.varClassificationFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    InvoiceGroup = x.MerpGroups(
                        Filter: f.invoiceGroupFilter,
                        ObjectName: "FakturaVydana",
                        // ObjednavkaPrijata, ObjednavkaVydana, FakturaPrijata, FakturaVydana, Zasoba, PolozkaCeniku, Artikl, DodaciListVydany, DodaciListPrijaty, Firma, PokladniDoklad, BankovniVypis, ProdejkaVydana, ProdejkaPrijata, SkladovyDoklad
                        selector: c => new
                        {
                            c.ID,
                            c.Kod,
                            c.Nazev,
                        }
                    ),
                    Transport = x.TransportTypes(Filter: f.transportFilter, selector: c => new { c.ID, c.Kod, c.Nazev }),
                    Payment = x.PaymentTypes(Filter: f.paymentFilter, selector: c => new { c.ID, c.Kod, c.Nazev }),
                    AccountAssignments = x.AccountAssignments(Filter: f.accountAssignmentsFilter, selector: c => new { c.ID, c.Kod, c.Nazev }),
                    VATClassifications = x.VATClassifications(Filter: f.varClassificationsFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    Articles = x.Articles(Filter: f.artiklPluFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.PLU, c.Nazev }),
                    Warehouses = x.Warehouses(Filter: f.warehouseFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    ConnectionsTypes = x.ConnectionsTypes(Filter: f.connectionsTypesFilter, selector: c => new { c.ID, c.Kod, c.Nazev }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );
            Assert.Empty(graphResponse.Errors ?? []);

            var numericalSeries = graphResponse.Data!.NumericalSeries!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );
            var currencyId = graphResponse.Data!.Currency!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var countryIds = graphResponse.Data!.Countries!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );
            var vatClassificationId = graphResponse.Data!.VATClassification!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var invoiceGroupId = graphResponse.Data!.InvoiceGroup!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var transportId = graphResponse.Data!.Transport!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var paymentId = graphResponse.Data!.Payment!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var accountAssignmentIds = graphResponse.Data!.AccountAssignments!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );
            var vatClassificationIds = graphResponse.Data!.VATClassifications!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );
            var articles = graphResponse.Data!.Articles!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.PLU!,
                    x => x!.ID.AsGuid()!.Value
                );
            var warehouses = graphResponse.Data!.Warehouses!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );
            var connectionsTypes = graphResponse.Data!.ConnectionsTypes!.AsValueEnumerable()
                .ToDictionary(
                    x => x!.Kod!,
                    x => x!.ID.AsGuid()!.Value
                );

            Assert.Contains(data.CisloDokladuNumericSerieKod, numericalSeries);
            Assert.Contains(data.FirmaKodNumericSerieKod, numericalSeries);
            Assert.All(data.Polozky, x =>
            {
                Assert.Contains(x.PredkontaceKod, accountAssignmentIds);
                Assert.Contains(x.CleneniDphKod, vatClassificationIds);
                Assert.Contains(x.ArtiklPlu, articles);
                Assert.Contains(x.SkladKod, warehouses);
            });

            var secondaryFilters = new
            {
                companyFilter = FilterFor<Company>.And(
                        x => x.Equal(m => m.Kod, data.FirmaKod),
                        x => x.Equal(m => m.FaktNazev, data.FirmaNazev),
                        x => x.Equal(m => m.FaktUlice, data.FirmaUlice),
                        x => x.Equal(m => m.FaktMisto, data.FirmaMisto),
                        x => x.Equal(m => m.FaktPsc, data.FirmaKodPsc),
                        x => x.Equal(m => m.PlatceDPH, data.FirmaPlatceDph),
                        x => x.Equal(m => m.FaktStat_ID, countryIds.GetValueOrDefault(data.FirmaStatKod)),
                        x => x.Equal(m => m.DIC, data.FirmaDic ?? string.Empty)
                    )
                    .ToString(),
                companyInvoiceReceiverFilter = FilterFor<Company>.And(
                        x => x.Equal(m => m.FaktNazev, data.AdresaPrijemceFaktury.Nazev),
                        x => x.Equal(m => m.FaktUlice, data.AdresaPrijemceFaktury.Ulice),
                        x => x.Equal(m => m.FaktMisto, data.AdresaPrijemceFaktury.Misto),
                        x => x.Equal(m => m.FaktPsc, data.AdresaPrijemceFaktury.Psc),
                        x => x.Equal(m => m.FaktStat_ID, countryIds.GetValueOrDefault(data.AdresaPrijemceFaktury.StatKod))
                    )
                    .ToString(),
                companyTerminalReceiverFilter = FilterFor<Company>.And(
                        x => x.Equal(m => m.FaktNazev, data.AdresaKoncovehoPrijemce.Nazev),
                        x => x.Equal(m => m.FaktUlice, data.AdresaKoncovehoPrijemce.Ulice),
                        x => x.Equal(m => m.FaktMisto, data.AdresaKoncovehoPrijemce.Misto),
                        x => x.Equal(m => m.FaktPsc, data.AdresaKoncovehoPrijemce.Psc),
                        x => x.Equal(m => m.FaktStat_ID, countryIds.GetValueOrDefault(data.AdresaKoncovehoPrijemce.StatKod))
                    )
                    .ToString(),
            };
            var secondaryGraphResponse = await graphqlClient.Query(
                secondaryFilters,
                static (f, x) => new
                {
                    Company = x.Companies(Filter: f.companyFilter, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Kod,
                        c.Nazev,
                        c.Create_Date,
                        c.HlavniOsoba_ID,
                        SeznamSpojeni = c.SeznamSpojeni(s => new
                        {
                            s.ID,
                            s.TypSpojeni_ID,
                            s.SpojeniCislo,
                            s.SpojeniMistniPredvolba,
                            s.Stat_ID,
                        }),
                    }),
                    CompanyInvoiceReceiver = x.Companies(Filter: f.companyInvoiceReceiverFilter, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Kod,
                        c.Nazev,
                        c.Create_Date,
                        c.HlavniOsoba_ID,
                        SeznamSpojeni = c.SeznamSpojeni(s => new
                        {
                            s.ID,
                            s.TypSpojeni_ID,
                            s.SpojeniCislo,
                            s.SpojeniMistniPredvolba,
                            s.Stat_ID,
                        }),
                    }),
                    CompanyTerminalReceiver = x.Companies(Filter: f.companyTerminalReceiverFilter, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Kod,
                        c.Nazev,
                        c.Create_Date,
                        c.HlavniOsoba_ID,
                        SeznamSpojeni = c.SeznamSpojeni(s => new
                        {
                            s.ID,
                            s.TypSpojeni_ID,
                            s.SpojeniCislo,
                            s.SpojeniMistniPredvolba,
                            s.Stat_ID,
                        }),
                    }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );
            Assert.Empty(secondaryGraphResponse.Errors ?? []);

            bool adresaPrijemceFakturyMatch = false;
            bool adresaKoncovehoPrijemceMatch = false;
            string? companyEmail = null;
            string? companyPhone = null;
            if (
                data.AdresaPrijemceFaktury.Nazev == data.FirmaNazev
                && data.AdresaPrijemceFaktury.Ulice == data.FirmaUlice
                && data.AdresaPrijemceFaktury.Misto == data.FirmaMisto
                && data.AdresaPrijemceFaktury.Psc == data.FirmaKodPsc
                && data.AdresaPrijemceFaktury.StatKod == data.FirmaStatKod
            )
            {
                adresaPrijemceFakturyMatch = true;
                companyEmail = data.AdresaPrijemceFaktury.Email;
                companyPhone = data.AdresaPrijemceFaktury.Telefon;
            }

            if (
                data.AdresaKoncovehoPrijemce.Nazev == data.FirmaNazev
                && data.AdresaKoncovehoPrijemce.Ulice == data.FirmaUlice
                && data.AdresaKoncovehoPrijemce.Misto == data.FirmaMisto
                && data.AdresaKoncovehoPrijemce.Psc == data.FirmaKodPsc
                && data.AdresaKoncovehoPrijemce.StatKod == data.FirmaStatKod
            )
            {
                adresaKoncovehoPrijemceMatch = true;
                if (adresaPrijemceFakturyMatch is false)
                {
                    companyEmail = data.AdresaKoncovehoPrijemce.Email;
                    companyPhone = data.AdresaKoncovehoPrijemce.Telefon;
                }
            }

            _ = adresaKoncovehoPrijemceMatch;

            var company = secondaryGraphResponse.Data?.Company?.AsValueEnumerable()
                .Where(x => x!.Deleted is false)
                .OrderBy(x => x!.Create_Date)
                .Select(x => new
                {
                    ID = x!.ID.AsGuid(),
                    x.SeznamSpojeni,
                })
                .FirstOrDefault();

            if (company is null)
            {
                ICollection<ConnectionInput> seznamSpojeni = [];
                if (!string.IsNullOrEmpty(companyEmail))
                {
                    seznamSpojeni.Add(new ConnectionInput
                    {
                        TypSpojeni_ID = connectionsTypes.GetValueOrDefault("E-mail"),
                        SpojeniCislo = companyEmail,
                        Vychozi = true,
                    });
                }

                if (!string.IsNullOrEmpty(companyPhone))
                {
                    seznamSpojeni.Add(new ConnectionInput
                    {
                        TypSpojeni_ID = connectionsTypes.GetValueOrDefault("Tel"),
                        SpojeniCislo = companyPhone,
                        Stat_ID = countryIds.GetValueOrDefault(data.FirmaStatKod),
                        Vychozi = true,
                    });
                }

                var companyInput = new
                {
                    companyInput = new CompanyInput
                    {
                        CiselnaRada_ID = numericalSeries.GetValueOrDefault(data.FirmaKodNumericSerieKod),
                        Kod = data.FirmaKod,
                        Nazev = data.FirmaNazev,
                        DIC = data.FirmaDic,
                        FaktNazev = data.FirmaNazev,
                        FaktUlice = data.FirmaUlice,
                        FaktMisto = data.FirmaMisto,
                        FaktPsc = data.FirmaKodPsc,
                        FaktStat_ID = countryIds.GetValueOrDefault(data.FirmaStatKod),
                        ObchNazev = data.FirmaNazev,
                        ObchUlice = data.FirmaUlice,
                        ObchMisto = data.FirmaMisto,
                        ObchPsc = data.FirmaKodPsc,
                        ObchStat_ID = countryIds.GetValueOrDefault(data.FirmaStatKod),
                        ProvNazev = data.FirmaNazev,
                        ProvUlice = data.FirmaUlice,
                        ProvMisto = data.FirmaMisto,
                        ProvPsc = data.FirmaKodPsc,
                        ProvStat_ID = countryIds.GetValueOrDefault(data.FirmaStatKod),
                        PlatceDPH = data.FirmaPlatceDph,
                        SeznamSpojeni = seznamSpojeni.ToArray(),
                    },
                };
                var companyResponse = await graphqlClient.Mutation(
                    companyInput,
                    static (f, x) => new
                    {
                        CompanyGuid = x.CreateCompany(f.companyInput, selector: c => new
                        {
                            c.ID,
                            c.Deleted,
                            c.Kod,
                            c.Nazev,
                            c.Create_Date,
                            c.HlavniOsoba_ID,
                            SeznamSpojeni = c.SeznamSpojeni(s => new
                            {
                                s.ID,
                                s.TypSpojeni_ID,
                                s.SpojeniCislo,
                                s.SpojeniMistniPredvolba,
                                s.Stat_ID,
                            }),
                        }),
                    },
                    cancellationToken: TestContext.Current.CancellationToken
                );
                Assert.Empty(companyResponse.Errors ?? []);
                company = new
                {
                    ID = companyResponse.Data?.CompanyGuid?.ID.AsGuid(),
                    companyResponse.Data?.CompanyGuid?.SeznamSpojeni,
                };
                companyInput.companyInput.ID = company.ID;
                foreach (var connectionInput in companyInput.companyInput.SeznamSpojeni)
                {
                    var contact = company.SeznamSpojeni?.AsValueEnumerable()
                        .Where(x =>
                            x!.TypSpojeni_ID == connectionInput!.TypSpojeni_ID
                            && x.SpojeniCislo == connectionInput.SpojeniCislo
                            && x.Stat_ID == connectionInput.Stat_ID
                        )
                        .FirstOrDefault();

                    if (
                        contact is null
                        && connectionInput!.TypSpojeni_ID.AsGuid() == connectionsTypes.GetValueOrDefault("Tel")
                    )
                    {
                        contact = company.SeznamSpojeni?.AsValueEnumerable()
                            .Where(x =>
                                x!.TypSpojeni_ID == connectionInput.TypSpojeni_ID
                                && x.Stat_ID == connectionInput.Stat_ID
                            )
                            .FirstOrDefault();

                        var phoneNumber = contact?.SpojeniCislo?.Split(' ', 2, StringSplitOptions.TrimEntries);

                        if (
                            phoneNumber?.Length == 2
                            && phoneNumber[0].Replace("+", "00") is { } phonePrefix
                            && connectionInput.SpojeniCislo?.StartsWith(phonePrefix) is true
                        )
                        {
                            var numberAfterPrefix = connectionInput.SpojeniCislo.Substring(phonePrefix.Length);
                            connectionInput.SpojeniCislo = $"{phoneNumber[0]} {numberAfterPrefix}";
                        }
                        else if (
                            phoneNumber?.Length == 2
                            && connectionInput.SpojeniCislo?.StartsWith("00") is false
                            && connectionInput.SpojeniCislo == phoneNumber[1]
                        )
                        {
                            connectionInput.SpojeniCislo = contact?.SpojeniCislo;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    connectionInput!.ID = contact?.ID;
                }

                companyResponse = await graphqlClient.Mutation(
                    companyInput,
                    static (f, x) => new
                    {
                        CompanyGuid = x.EditCompany(f.companyInput, selector: c => new
                        {
                            c.ID,
                            c.Deleted,
                            c.Kod,
                            c.Nazev,
                            c.Create_Date,
                            c.HlavniOsoba_ID,
                            SeznamSpojeni = c.SeznamSpojeni(s => new
                            {
                                s.ID,
                                s.TypSpojeni_ID,
                                s.SpojeniCislo,
                                s.SpojeniMistniPredvolba,
                                s.Stat_ID,
                            }),
                        }),
                    },
                    cancellationToken: TestContext.Current.CancellationToken
                );

                Assert.Empty(companyResponse.Errors ?? []);
                company = new
                {
                    ID = companyResponse.Data?.CompanyGuid?.ID.AsGuid(),
                    companyResponse.Data?.CompanyGuid?.SeznamSpojeni,
                };
            }

            var companyInvoiceReceiver = company;
            var companyTerminalReceiver = company;

            ids.Add((
                numericalSeries,
                company.ID,
                companyInvoiceReceiver.ID,
                companyTerminalReceiver.ID,
                currencyId,
                countryIds,
                vatClassificationId,
                invoiceGroupId,
                transportId,
                paymentId,
                accountAssignmentIds,
                vatClassificationIds,
                articles,
                warehouses
            ));
        }

        foreach (var (invoice, resolvedIds) in invoiceData.Zip(ids))
        {
            var importInvoice = new
            {
                companyInput = new IssuedInvoiceInput
                {
                    ZaokrouhleniCelkovaCastka_ID = Guid.Parse("186cfec6-7203-4c8c-acc5-715c78591ee3"), // TODO where to get this from?
                    ZaokrouhleniPrevazujiciSazbaDPH = false,
                    CiselnaRada_ID = resolvedIds.numericalSeries.GetValueOrDefault(invoice.CisloDokladuNumericSerieKod),
                    CisloDokladu = invoice.CisloDokladu,
                    OdkazNaDoklad = invoice.OdkazNaDoklad,
                    VariabilniSymbol = invoice.VariabilniSymbol,
                    DatumVystaveni = invoice.DatumVystaveni,
                    DatumUcetnihoPripadu = invoice.DatumUcetnihoPripadu,
                    DatumPlneni = invoice.DatumPlneni,
                    DatumSplatnosti = invoice.DatumSplatnosti,
                    Vystavil = invoice.Vystavil,
                    Nazev = invoice.Nazev,
                    DIC = invoice.Dic,
                    CelkovaCastkaCM = invoice.CelkovaCastkaCm,
                    UcetniKurzKurz = invoice.Kurz,
                    //invoice.KurzMnozstvi,
                    Mena_ID = resolvedIds.currencyId,
                    CleneniDPH_ID = resolvedIds.vatClassificationId,
                    Group_ID = resolvedIds.invoiceGroupId,
                    Firma_ID = resolvedIds.companyGuid,
                    FakturacniAdresaFirma_ID = resolvedIds.companyInvoiceReceiverGuid,
                    FakturacniAdresaNazev = invoice.FirmaNazev,
                    FakturacniAdresaUlice = invoice.FirmaUlice,
                    FakturacniAdresaMisto = invoice.FirmaMisto,
                    FakturacniAdresaPSC = invoice.FirmaKodPsc,
                    FakturacniAdresaStat = invoice.FirmaStatKod,
                    DodaciAdresaFirma_ID = resolvedIds.companyTerminalReceiverGuid,
                    DodaciAdresaNazev = invoice.FirmaNazev,
                    DodaciAdresaUlice = invoice.FirmaUlice,
                    DodaciAdresaMisto = invoice.FirmaMisto,
                    DodaciAdresaPSC = invoice.FirmaKodPsc,
                    DodaciAdresaStat = invoice.FirmaStatKod,

                    AdresaPrijemceFakturyStat_ID = resolvedIds.countryIds.GetValueOrDefault(invoice.AdresaPrijemceFaktury.StatKod),
                    //AdresaPrijemceFakturyKontaktniOsoba_ID = resolvedIds.companyInvoiceReceiverGuid,

                    //invoice.AdresaPrijemceFaktury.Email,
                    //invoice.AdresaPrijemceFaktury.Telefon,
                    //invoice.AdresaPrijemceFaktury.Ulice,
                    //invoice.AdresaPrijemceFaktury.Misto,
                    //invoice.AdresaPrijemceFaktury.Psc,
                    //invoice.AdresaPrijemceFaktury.Stat,
                    //invoice.AdresaPrijemceFaktury.Nazev,
                    AdresaKoncovehoPrijemceEmail = invoice.AdresaKoncovehoPrijemce.Email,
                    AdresaKoncovehoPrijemceTelefon = invoice.AdresaKoncovehoPrijemce.Telefon,
                    //AdresaKoncovehoPrijemceKontaktniOsoba_ID = resolvedIds.companyTerminalReceiverGuid,
                    AdresaKoncovehoPrijemceStat_ID = resolvedIds.countryIds.GetValueOrDefault(invoice.AdresaKoncovehoPrijemce.StatKod),
                    //invoice.AdresaKoncovehoPrijemce.Ulice,
                    //invoice.AdresaKoncovehoPrijemce.Misto,
                    //invoice.AdresaKoncovehoPrijemce.Psc,
                    //invoice.AdresaKoncovehoPrijemce.Stat,
                    ZpusobDopravy_ID = resolvedIds.transportId,
                    ZpusobPlatby_ID = resolvedIds.paymentId,
                    Predkontace_ID = resolvedIds.accountAssignmentIds.AsValueEnumerable().FirstOrDefault().Value,
                    Polozky =
                    [
                        .. invoice.Polozky.AsValueEnumerable()
                            .Select(x => new IssuedInvoiceItemInput
                            {
                                Nazev = x.Nazev,
                                Mnozstvi = x.Mnozstvi,
                                DPHEditovanoRucne = x.DphEditovanoRucne,
                                DruhSazbyDPH = x.DruhSazbyDph,
                                Predkontace_ID = resolvedIds.accountAssignmentIds.GetValueOrDefault(x.PredkontaceKod),
                                Jednotka = x.Jednotka,
                                CisloPolozky = x.CisloPolozky,
                                TypObsahu = x.TypObsahu,
                                CleneniDPH_ID = resolvedIds.vatClassificationIds.GetValueOrDefault(x.CleneniDphKod),
                                TypCeny = x.TypCeny,
                                ObsahPolozky = new ContentOfItemWithArticleInput
                                {
                                    Artikl_ID = resolvedIds.articles.GetValueOrDefault(x.ArtiklPlu),
                                    Sklad_ID = resolvedIds.warehouses.GetValueOrDefault(x.SkladKod),
                                },
                                CelkovaCena = x.CelkovaCena,
                                CelkovaCenaCM = x.CelkovaCenaCm,
                                DphCelkem = x.DphCelkem,
                                DphZaklad = x.DphZaklad,
                                DphDan = x.DphDan,
                                DphCelkemCM = x.DphCelkemCm,
                                DphZakladCM = x.DphZakladCm,
                                DphDanCM = x.DphDanCm,
                                DphSazba = x.DphSazba,
                            }),
                    ],
                    Poznamka = invoice.Poznamka,
                },
            };
            var mutationResponse = await graphqlClient.Mutation(
                importInvoice,
                static (f, x) => new
                {
                    CreatedInvoice = x.CreateIssuedInvoice(f.companyInput, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Nazev,
                        c.Create_Date,
                        c.CisloDokladu,
                    }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

            Assert.Empty(mutationResponse.Errors ?? []);

            var updateInvoice = new
            {
                companyInput = new IssuedInvoiceInput
                {
                    ID = mutationResponse.Data!.CreatedInvoice!.ID,
                    CisloDokladu = invoice.CisloDokladu,
                }
            };
            mutationResponse = await graphqlClient.Mutation(
                updateInvoice,
                static (f, x) => new
                {
                    CreatedInvoice = x.EditIssuedInvoice(f.companyInput, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Nazev,
                        c.Create_Date,
                        c.CisloDokladu,
                    }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );
            Assert.Empty(mutationResponse.Errors ?? []);
        }
    }

    private sealed class MoneyErpInvoiceClassData() : TheoryData<MoneyErpAuthenticationsClassData.AuthenticationData?, InvoiceData[]>(
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
                            CisloDokladuNumericSerieKod: "FAKT_VIN",
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
                            GroupKod: "DE",
                            FirmaKodNumericSerieKod: "USER_ID",
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
                            AdresaPrijemceFaktury: new(
                                Email: "x@x.com",
                                Telefon: "0049123456789",
                                Ulice: "Street 1",
                                Misto: "Roding Strahlfeld",
                                Psc: "93426",
                                StatKod: "DE",
                                Stat: "Germany",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: "004917611762621",
                                Ulice: "Street 1",
                                Misto: "Roding Strahlfeld",
                                Psc: "93426",
                                StatKod: "DE",
                                Stat: "Germany",
                                Nazev: "John Doe"
                            ),
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
                                    ArtiklPlu: "PLU00001",
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
                            ],
                            Poznamka: null
                        ),
                        new InvoiceData(
                            CisloDokladuNumericSerieKod: "FAKT_VIN",
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
                            GroupKod: "CZ",
                            FirmaKodNumericSerieKod: "USER_ID",
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
                            AdresaPrijemceFaktury: new(
                                Email: "x@x.com",
                                Telefon: "0793123456",
                                Ulice: "Street 1",
                                Misto: "Arbaz",
                                Psc: "1974",
                                StatKod: "CH",
                                Stat: "Switzerland",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: "0793123456",
                                Ulice: "Street 1",
                                Misto: "Arbaz",
                                Psc: "1974",
                                StatKod: "CH",
                                Stat: "Switzerland",
                                Nazev: "John Doe"
                            ),
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
                                    ArtiklPlu: "PLU00001",
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
                            ],
                            Poznamka: null
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
                            CisloDokladuNumericSerieKod: "FAKT_VIN",
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
                            GroupKod: "CZ",
                            FirmaKodNumericSerieKod: "LMS_ID",
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
                            AdresaPrijemceFaktury: new(
                                Email: "x@x.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Csomad",
                                Psc: "2161",
                                StatKod: "HU",
                                Stat: "Hungary",
                                Nazev: "Easy"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@ex.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Csomad",
                                Psc: "2161",
                                StatKod: "HU",
                                Stat: "Hungary",
                                Nazev: "Easy"
                            ),
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
                                    ArtiklPlu: "PLU00001",
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
                            ],
                            Poznamka: "c207975c-4ae5-488e-ac6b-b9acb87d1a62"
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
                            CisloDokladuNumericSerieKod: "FAKT_VIN",
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
                            GroupKod: "LT",
                            FirmaKodNumericSerieKod: "USER_ID",
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
                            AdresaPrijemceFaktury: new(
                                Email: "x@x.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Vilnius",
                                Psc: "11000",
                                StatKod: "LT",
                                Stat: "Lithuania",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Vilnius",
                                Psc: "11000",
                                StatKod: "LT",
                                Stat: "Lithuania",
                                Nazev: "John Doe"
                            ),
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                                    ArtiklPlu: "PLU00001",
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
                            ],
                            Poznamka: null
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
        string StatKod,
        string Stat,
        string Nazev
    );

    public sealed record InvoiceData(
        // Basic identifiers
        string CisloDokladuNumericSerieKod,
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
        string GroupKod,

        // Supplier (Adresa/Firma)
        string FirmaKodNumericSerieKod,
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
        InvoiceAddressData AdresaPrijemceFaktury,
        InvoiceAddressData AdresaKoncovehoPrijemce,

        // Transport & payment
        string ZpusobDopravyKod,
        string ZpusobDopravyNazev,
        string ZpusobPlatbyKod,
        string ZpusobPlatbyNazev,

        // Items
        InvoiceItemData[] Polozky,
        string? Poznamka
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
            GroupKod,
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
            new JsonObject
            {
                [nameof(AdresaPrijemceFaktury.Email)] = AdresaPrijemceFaktury.Email,
                [nameof(AdresaPrijemceFaktury.Telefon)] = AdresaPrijemceFaktury.Telefon,
                [nameof(AdresaPrijemceFaktury.Ulice)] = AdresaPrijemceFaktury.Ulice,
                [nameof(AdresaPrijemceFaktury.Misto)] = AdresaPrijemceFaktury.Misto,
                [nameof(AdresaPrijemceFaktury.Psc)] = AdresaPrijemceFaktury.Psc,
                [nameof(AdresaPrijemceFaktury.Stat)] = AdresaPrijemceFaktury.Stat,
                [nameof(AdresaPrijemceFaktury.Nazev)] = AdresaPrijemceFaktury.Nazev,
            },
            new JsonObject
            {
                [nameof(AdresaKoncovehoPrijemce.Email)] = AdresaKoncovehoPrijemce.Email,
                [nameof(AdresaKoncovehoPrijemce.Telefon)] = AdresaKoncovehoPrijemce.Telefon,
                [nameof(AdresaKoncovehoPrijemce.Ulice)] = AdresaKoncovehoPrijemce.Ulice,
                [nameof(AdresaKoncovehoPrijemce.Misto)] = AdresaKoncovehoPrijemce.Misto,
                [nameof(AdresaKoncovehoPrijemce.Psc)] = AdresaKoncovehoPrijemce.Psc,
                [nameof(AdresaKoncovehoPrijemce.Stat)] = AdresaKoncovehoPrijemce.Stat,
                [nameof(AdresaKoncovehoPrijemce.Nazev)] = AdresaKoncovehoPrijemce.Nazev,
            },
            ZpusobDopravyKod,
            ZpusobDopravyNazev,
            ZpusobPlatbyKod,
            ZpusobPlatbyNazev,
            new JsonArray(Polozky.AsValueEnumerable()
                .Select(JsonNode (x) => new JsonObject
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
                })
                .ToArray()),
            Poznamka,
            CisloDokladuNumericSerieKod,
            FirmaKodNumericSerieKod
        ).ToString();

        public static InvoiceData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 36 } arr)
            {
                throw new FormatException("Input string is not a valid InvoiceData JSON array.");
            }

            return new InvoiceData(
                CisloDokladuNumericSerieKod: arr[34]?.GetValue<string>() ?? throw new FormatException("CisloDokladuKod missing."),
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
                GroupKod: arr[15]?.GetValue<string>() ?? throw new FormatException("GroupKod missing."),
                FirmaKodNumericSerieKod: arr[35]?.GetValue<string>() ?? throw new FormatException("FirmaKodNumericSerieKod missing."),
                FirmaKod: arr[16]?.GetValue<string>() ?? throw new FormatException("FirmaKod missing."),
                FirmaPlatceDph: arr[17]?.GetValue<bool>() ?? throw new FormatException("FirmaPlatceDPH missing."),
                FirmaDic: arr[18]?.GetValue<string?>(),
                FirmaNazev: arr[19]?.GetValue<string>() ?? throw new FormatException("FirmaNazev missing."),
                FirmaUlice: arr[20]?.GetValue<string>() ?? throw new FormatException("FirmaUlice missing."),
                FirmaMisto: arr[21]?.GetValue<string>() ?? throw new FormatException("FirmaMisto missing."),
                FirmaKodPsc: arr[22]?.GetValue<string>() ?? throw new FormatException("FirmaKodPsc missing."),
                FirmaNazevStatu: arr[23]?.GetValue<string>() ?? throw new FormatException("FirmaNazevStatu missing."),
                FirmaStatKod: arr[24]?.GetValue<string>() ?? throw new FormatException("FirmaStatKod missing."),
                FirmaStatNazevEn: arr[25]?.GetValue<string>() ?? throw new FormatException("FirmaStatNazevEN missing."),
                AdresaPrijemceFaktury: new InvoiceAddressData(
                    Email: arr[26]?[nameof(InvoiceAddressData.Email)]?.GetValue<string?>(),
                    Telefon: arr[26]?[nameof(InvoiceAddressData.Telefon)]?.GetValue<string?>(),
                    Ulice: arr[26]?[nameof(InvoiceAddressData.Ulice)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Ulice missing."),
                    Misto: arr[26]?[nameof(InvoiceAddressData.Misto)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Misto missing."),
                    Psc: arr[26]?[nameof(InvoiceAddressData.Psc)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Psc missing."),
                    StatKod: arr[26]?[nameof(InvoiceAddressData.StatKod)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.StatKod missing."),
                    Stat: arr[26]?[nameof(InvoiceAddressData.Stat)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Stat missing."),
                    Nazev: arr[26]?[nameof(InvoiceAddressData.Nazev)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Nazev missing.")
                ),
                AdresaKoncovehoPrijemce: new InvoiceAddressData(
                    Email: arr[27]?[nameof(InvoiceAddressData.Email)]?.GetValue<string?>(),
                    Telefon: arr[27]?[nameof(InvoiceAddressData.Telefon)]?.GetValue<string?>(),
                    Ulice: arr[27]?[nameof(InvoiceAddressData.Ulice)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Ulice missing."),
                    Misto: arr[27]?[nameof(InvoiceAddressData.Misto)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Misto missing."),
                    Psc: arr[27]?[nameof(InvoiceAddressData.Psc)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Psc missing."),
                    StatKod: arr[27]?[nameof(InvoiceAddressData.StatKod)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.StatKod missing."),
                    Stat: arr[27]?[nameof(InvoiceAddressData.Stat)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Stat missing."),
                    Nazev: arr[27]?[nameof(InvoiceAddressData.Nazev)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Nazev missing.")
                ),
                ZpusobDopravyKod: arr[28]?.GetValue<string>() ?? throw new FormatException("ZpusobDopravyKod missing."),
                ZpusobDopravyNazev: arr[29]?.GetValue<string>() ?? throw new FormatException("ZpusobDopravyNazev missing."),
                ZpusobPlatbyKod: arr[30]?.GetValue<string>() ?? throw new FormatException("ZpusobPlatbyKod missing."),
                ZpusobPlatbyNazev: arr[31]?.GetValue<string>() ?? throw new FormatException("ZpusobPlatbyNazev missing."),
                Polozky: arr[32]
                    ?.AsArray()
                    .AsValueEnumerable()
                    .Select(static x => new InvoiceItemData(
                        Nazev: x![nameof(InvoiceItemData.Nazev)]?.GetValue<string>() ?? throw new FormatException("Nazev missing."),
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
                    ))
                    .ToArray() ?? throw new FormatException("Polozky missing."),
                Poznamka: arr[33]?.GetValue<string?>()
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
