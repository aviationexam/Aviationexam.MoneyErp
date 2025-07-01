using Aviationexam.MoneyErp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Net.Http;

namespace Aviationexam.MoneyErp.KiotaServices;

public class DefaultHttpClientRequestAdapter(
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpServiceKey)]
    IAuthenticationProvider authenticationProvider,
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpServiceKey)]
    IParseNodeFactory? parseNodeFactory = null,
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpServiceKey)]
    ISerializationWriterFactory? serializationWriterFactory = null,
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpHttpClient)]
    HttpClient? httpClient = null,
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpServiceKey)]
    ObservabilityOptions? observabilityOptions = null
) : HttpClientRequestAdapter(
    authenticationProvider, parseNodeFactory, serializationWriterFactory, httpClient, observabilityOptions
);
