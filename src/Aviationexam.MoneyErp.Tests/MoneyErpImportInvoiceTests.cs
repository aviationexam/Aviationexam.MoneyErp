using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.Extensions;
using Aviationexam.MoneyErp.RestApi.ClientV2;
using Aviationexam.MoneyErp.RestApi.ClientV2.Models.ApiCore.Services.Person;
using Aviationexam.MoneyErp.RestApi.Extensions;
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
using Guid = System.Guid;

namespace Aviationexam.MoneyErp.Tests;

public class MoneyErpImportInvoiceTests
{
    [Theory]
    [ClassData(typeof(MoneyErpInvoiceClassData), Explicit = true)]
    public async Task ImportInvoiceWorks(
        MoneyErpAuthenticationsClassData.AuthenticationData? authenticationData,
        InvoiceData[] invoiceData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(authenticationData!, shouldRedactHeaderValue: false);

        var restApiClient = serviceProvider.GetRequiredService<MoneyErpApiV2Client>();
        var graphqlClient = serviceProvider.GetRequiredService<MoneyErpGraphqlClient>();

        var version = await graphqlClient.Query(x => x.Version, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(version.Data);
        Assert.NotEmpty(version.Data);

        ICollection<(
            Guid? companyGuid,
            Guid? currencyId,
            Guid? companyCountryId,
            Guid? vatClassificationId,
            Guid? invoiceGroupId,
            Guid? transportId,
            Guid? paymentId,
            Guid? invoiceReceiverPersonId,
            IReadOnlyDictionary<string, Guid> accountAssignmentIds,
            IReadOnlyDictionary<string, Guid> vatClassificationIds,
            IReadOnlyDictionary<string, Guid> articles,
            IReadOnlyDictionary<string, Guid> warehouses
            )> ids = [];
        foreach (var data in invoiceData)
        {
            var filters = new
            {
                currencyFilter = $"{nameof(Currency.Kod)}~eq~{data.MenaKod}",
                companyCountryFilter = $"{nameof(Country.Kod)}~eq~{data.FirmaStatKod}",
                invoiceReceiverAddressCountryFilter = $"{nameof(Country.Kod)}~eq~{data.AdresaPrijemceFaktury.Stat}",
                receiverAddressCountryFilter = $"{nameof(Country.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Stat}",
                varClassificationFilter = $"{nameof(VATClassification.Kod)}~eq~{data.CleneniDphKod}",
                invoiceGroupFilter = $"{nameof(MerpGroup.Kod)}~eq~{data.GroupKod}",
                transportFilter = $"{nameof(TransportType.Kod)}~eq~{data.ZpusobDopravyKod}",
                paymentFilter = $"{nameof(PaymentType.Kod)}~eq~{data.ZpusobPlatbyKod}",
                accountAssignmentsFilter = data.Polozky.AsValueEnumerable().Select(x => x.PredkontaceKod).Distinct().Select(x => $"{nameof(AccountAssignment.Kod)}~eq~{x}").JoinToString('|'),
                varClassificationsFilter = data.Polozky.AsValueEnumerable().Select(x => x.CleneniDphKod).Distinct().Select(x => $"{nameof(VATClassification.Kod)}~eq~{x}").JoinToString('|'),
                artiklPluFilter = data.Polozky.AsValueEnumerable().Select(x => x.ArtiklPlu).Distinct().Select(x => $"{nameof(Article.PLU)}~eq~{x}").JoinToString('|'),
                warehouseFilter = data.Polozky.AsValueEnumerable().Select(x => x.SkladKod).Distinct().Select(x => $"{nameof(Warehouse.Kod)}~eq~{x}").JoinToString('|'),
                connectionsTypesFilter = string.Join('|',
                    $"{nameof(ConnectionsType.Kod)}~eq~E-mail",
                    $"{nameof(ConnectionsType.Kod)}~eq~Mob",
                    $"{nameof(ConnectionsType.Kod)}~eq~Tel"
                ),
                //invoiceReceiverEmailFilter = $"{nameof(Connection.Vychozi)}~eq~{data.AdresaPrijemceFaktury.Email}",
                //receiverEmailFilter = $"{nameof(Connection.Vychozi)}~eq~{data.AdresaKoncovehoPrijemce.Email}",
                //invoiceReceiverPhoneFilter = $"{nameof(Connection.Vychozi)}~eq~{data.AdresaPrijemceFaktury.Telefon}",
                //receiverPhoneFilter = $"{nameof(Connection.Vychozi)}~eq~{data.AdresaKoncovehoPrijemce.Telefon}",
                //invoiceReceiverAddressFilter = string.Join('#',
                //    $"{nameof(Adresar_Firma.Kod)}~eq~{data.AdresaPrijemceFaktury.Email}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaPrijemceFaktury.Telefon}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaPrijemceFaktury.Ulice}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaPrijemceFaktury.Misto}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaPrijemceFaktury.Psc}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaPrijemceFaktury.Nazev}"
                //),
                //receiverAddressFilter = string.Join('#',
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Email}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Telefon}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Ulice}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Misto}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Psc}",
                //    $"{nameof(PaymentType.Kod)}~eq~{data.AdresaKoncovehoPrijemce.Nazev}"
                //),
            };

            var graphResponse = await graphqlClient.Query(
                filters,
                static (f, x) => new
                {
                    Currency = x.Currencies(Filter: f.currencyFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    CompanyCountry = x.Countries(Filter: f.companyCountryFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    InvoiceReceiverAddressCountry = x.Countries(Filter: f.invoiceReceiverAddressCountryFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
                    ReceiverAddressCountry = x.Countries(Filter: f.receiverAddressCountryFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev }),
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

            var currencyId = graphResponse.Data!.Currency!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var companyCountryId = graphResponse.Data!.CompanyCountry!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var invoiceReceiverAddressCountryId = graphResponse.Data!.InvoiceReceiverAddressCountry!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
            var receiverAddressCountryId = graphResponse.Data!.ReceiverAddressCountry!.AsValueEnumerable().FirstOrDefault()?.ID.AsGuid();
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

            var secondaryFilters = new
            {
                companyFilter = string.Join('#',
                    $"{nameof(Company.FaktNazev)}~eq~{data.FirmaNazev}",
                    $"{nameof(Company.FaktUlice)}~eq~{data.FirmaUlice}",
                    $"{nameof(Company.FaktMisto)}~eq~{data.FirmaMisto}",
                    $"{nameof(Company.FaktPsc)}~eq~{data.FirmaKodPsc}",
                    $"{nameof(Company.PlatceDPH)}~eq~{(data.FirmaPlatceDph ? "true" : "false")}",
                    $"{nameof(Company.FaktStat_ID)}~eq~{companyCountryId}",
                    $"{nameof(Company.DIC)}~eq~{data.FirmaDic}"
                ),
            };
            var secondaryGraphResponse = await graphqlClient.Query(
                secondaryFilters,
                static (f, x) => new
                {
                    Company = x.Companies(Filter: f.companyFilter, selector: c => new { c.ID, c.Deleted, c.Kod, c.Nazev, c.Create_Date }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );
            Assert.Empty(secondaryGraphResponse.Errors ?? []);

            var companyGuid = secondaryGraphResponse.Data?.Company?.AsValueEnumerable()
                .Where(x => x!.Deleted is false)
                .OrderBy(x => x!.Create_Date)
                .FirstOrDefault()
                ?.ID.AsGuid();

            if (companyGuid is null)
            {
                var companyInput = new CompanyInput
                {
                    Kod = data.FirmaKod,
                    Nazev = data.FirmaNazev,
                    DIC = data.FirmaDic,
                    FaktNazev = data.FirmaNazev,
                    FaktUlice = data.FirmaUlice,
                    FaktMisto = data.FirmaMisto,
                    FaktPsc = data.FirmaKodPsc,
                    FaktStat_ID = companyCountryId,
                    ObchNazev = data.FirmaNazev,
                    ObchUlice = data.FirmaUlice,
                    ObchMisto = data.FirmaMisto,
                    ObchPsc = data.FirmaKodPsc,
                    ObchStat_ID = companyCountryId,
                    ProvNazev = data.FirmaNazev,
                    ProvUlice = data.FirmaUlice,
                    ProvMisto = data.FirmaMisto,
                    ProvPsc = data.FirmaKodPsc,
                    ProvStat_ID = companyCountryId,
                    PlatceDPH = data.FirmaPlatceDph,
                };
                var companyResponse = await graphqlClient.Mutation(
                    companyInput,
                    static (f, x) => new
                    {
                        CompanyGuid = x.CreateCompany(f, selector: c => new
                        {
                            c.ID,
                            c.Deleted,
                            c.Kod,
                            c.Nazev,
                            c.Create_Date,
                        }),
                    },
                    cancellationToken: TestContext.Current.CancellationToken
                );
                Assert.Empty(companyResponse.Errors ?? []);
                companyGuid = companyResponse.Data?.CompanyGuid?.ID.AsGuid();
            }

            IReadOnlyCollection<ConnectionRequestBuilderExtensions.Connection> invoiceReceiverPhones = [];
            IReadOnlyCollection<ConnectionRequestBuilderExtensions.Connection> invoiceReceiverEmails = [];
            if (connectionsTypes.TryGetValue("Tel", out var connectionTypeTelId))
            {
                if (data.AdresaPrijemceFaktury.Telefon is { } invoiceReceiverPhone)
                {
                    invoiceReceiverPhones = await restApiClient.GetConnectionAsync(connectionTypeTelId, invoiceReceiverPhone, TestContext.Current.CancellationToken);
                }
            }

            if (connectionsTypes.TryGetValue("E-mail", out var connectionTypeEmailId))
            {
                if (data.AdresaPrijemceFaktury.Email is { } invoiceReceiverEmail)
                {
                    invoiceReceiverEmails = await restApiClient.GetConnectionAsync(connectionTypeEmailId, invoiceReceiverEmail, TestContext.Current.CancellationToken);
                }
            }

            var personRequestInformation = restApiClient.AddCustomQueryParameters(
                restApiClient.V20.Person.ToGetRequestInformation(),
                x => { }
            );

            var persons = await restApiClient.V20.Person.GetAsync(personRequestInformation, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(persons);
            Assert.Empty(persons.AdditionalData);
            Assert.NotNull(persons.Data);
            Assert.All(persons.Data, x => Assert.Empty(x.AdditionalData));
            var invoiceReceiverPersonId = persons.Data.AsValueEnumerable().FirstOrDefault()?.ID;

            if (invoiceReceiverPersonId is null)
            {
                var personInputDto = new PersonInputDto
                {
                    Nazev = data.AdresaPrijemceFaktury.Nazev,
                    Email = data.AdresaPrijemceFaktury.Email,
                    Tel2Cislo = data.AdresaPrijemceFaktury.Telefon,
                    //TelefonSpojeni1ID = invoiceReceiverPhoneId,
                    //EmailSpojeniID = invoiceReceiverEmailId,
                    AdresaNazev = data.AdresaPrijemceFaktury.Nazev,
                    AdresaMisto = data.AdresaPrijemceFaktury.Misto,
                    AdresaUlice = data.AdresaPrijemceFaktury.Ulice,
                    AdresaPsc = data.AdresaPrijemceFaktury.Psc,
                    AdresaPscID = null,
                    AdresaStat = null,
                    AdresaStatID = invoiceReceiverAddressCountryId,
                    Attachments = null,
                    CisloOsoby = null,
                    CisloS3 = null,
                    CreateDate = null,
                    CreateID = null,
                    DatumPosty = null,
                    Deleted = null,
                    FaxCislo = null,
                    FaxKlapka = null,
                    FaxMistniCislo = null,
                    FaxPredvolba = null,
                    FaxPredvolbaStat = null,
                    FaxSpojeniID = null,
                    FaxStatID = null,
                    Funkce = null,
                    GroupID = null,
                    Hidden = null,
                    ID = null,
                    IsNew = null,
                    Jmeno = null,
                    Kod = null,
                    KrestniJmeno = null,
                    Locked = null,
                    ModifyDate = null,
                    ModifyID = null,
                    Osloveni = null,
                    ParentID = null,
                    Pohlavi = null,
                    PosilatPostu = null,
                    Poznamka = null,
                    Prijmeni = null,
                    RootID = null,
                    Spojeni = null,
                    TitulPred = null,
                    TitulZa = null,
                };
                var createPersonResponse = await restApiClient.V20.Person.PostAsync([
                    personInputDto,
                ], cancellationToken: TestContext.Current.CancellationToken);

                Assert.NotNull(createPersonResponse);
                Assert.Empty(createPersonResponse.AdditionalData);
                Assert.NotNull(createPersonResponse.Data);

                personInputDto.ID = invoiceReceiverPersonId = createPersonResponse.Data.AsValueEnumerable().FirstOrDefault();

                if (data.AdresaPrijemceFaktury.Telefon is { } invoiceReceiverPhone)
                {
                    personInputDto.TelefonSpojeni2ID = await restApiClient.CreateConnectionAsync(connectionTypeTelId, invoiceReceiverPhone, TestContext.Current.CancellationToken);
                }

                if (data.AdresaPrijemceFaktury.Email is { } invoiceReceiverEmail)
                {
                    personInputDto.EmailSpojeniID = await restApiClient.CreateConnectionAsync(connectionTypeEmailId, invoiceReceiverEmail, TestContext.Current.CancellationToken);
                }

                await restApiClient.V20.Person.PutAsync([
                    personInputDto,
                ], cancellationToken: TestContext.Current.CancellationToken);
            }

            ids.Add((
                companyGuid,
                currencyId,
                companyCountryId,
                vatClassificationId,
                invoiceGroupId,
                transportId,
                paymentId,
                invoiceReceiverPersonId,
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

                    AdresaPrijemceFakturyKontaktniOsoba_ID = resolvedIds.invoiceReceiverPersonId,

                    //invoice.AdresaPrijemceFaktury.Email,
                    //invoice.AdresaPrijemceFaktury.Telefon,
                    //invoice.AdresaPrijemceFaktury.Ulice,
                    //invoice.AdresaPrijemceFaktury.Misto,
                    //invoice.AdresaPrijemceFaktury.Psc,
                    //invoice.AdresaPrijemceFaktury.Stat,
                    //invoice.AdresaPrijemceFaktury.Nazev,
                    AdresaKoncovehoPrijemceEmail = invoice.AdresaKoncovehoPrijemce.Email,
                    AdresaKoncovehoPrijemceTelefon = invoice.AdresaKoncovehoPrijemce.Telefon,
                    AdresaKoncovehoPrijemceKontaktniOsoba_ID = resolvedIds.invoiceReceiverPersonId, // TODO
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
                    CreatedInvoiceGuid = x.CreateIssuedInvoice(f.companyInput, selector: c => new
                    {
                        c.ID,
                        c.Deleted,
                        c.Nazev,
                        c.Create_Date,
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
                                Stat: "Germany",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: "004917611762621",
                                Ulice: "Street 1",
                                Misto: "Roding Strahlfeld",
                                Psc: "93426",
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
                                    ArtiklPlu: "ART00001",
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
                                Stat: "Switzerland",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: "0793123456",
                                Ulice: "Street 1",
                                Misto: "Arbaz",
                                Psc: "1974",
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
                                    ArtiklPlu: "ART00001",
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
                                Stat: "Hungary",
                                Nazev: "Easy"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@ex.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Csomad",
                                Psc: "2161",
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
                                Stat: "Lithuania",
                                Nazev: "John Doe"
                            ),
                            AdresaKoncovehoPrijemce: new(
                                Email: "x@x.com",
                                Telefon: null,
                                Ulice: "Street 1",
                                Misto: "Vilnius",
                                Psc: "11000",
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
        string GroupKod,

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
                .Select(x => (JsonNode) new JsonObject
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
            Poznamka
        ).ToString();

        public static InvoiceData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 34 } arr)
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
                GroupKod: arr[15]?.GetValue<string>() ?? throw new FormatException("GroupKod missing."),
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
                    Stat: arr[26]?[nameof(InvoiceAddressData.Stat)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Stat missing."),
                    Nazev: arr[26]?[nameof(InvoiceAddressData.Nazev)]?.GetValue<string>() ?? throw new FormatException("AdresaPrijemceFaktury.Nazev missing.")
                ),
                AdresaKoncovehoPrijemce: new InvoiceAddressData(
                    Email: arr[27]?[nameof(InvoiceAddressData.Email)]?.GetValue<string?>(),
                    Telefon: arr[27]?[nameof(InvoiceAddressData.Telefon)]?.GetValue<string?>(),
                    Ulice: arr[27]?[nameof(InvoiceAddressData.Ulice)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Ulice missing."),
                    Misto: arr[27]?[nameof(InvoiceAddressData.Misto)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Misto missing."),
                    Psc: arr[27]?[nameof(InvoiceAddressData.Psc)]?.GetValue<string>() ?? throw new FormatException("AdresaKoncovehoPrijemce.Psc missing."),
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
