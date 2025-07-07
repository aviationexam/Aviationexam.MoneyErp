using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Net.Http;

namespace Aviationexam.MoneyErp.KiotaServices;

public class DefaultHttpClientRequestAdapter(
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    IAuthenticationProvider authenticationProvider,
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    IParseNodeFactory? parseNodeFactory = null,
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    ISerializationWriterFactory? serializationWriterFactory = null,
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpRestApiHttpClient)]
    HttpClient? httpClient = null,
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    ObservabilityOptions? observabilityOptions = null
) : HttpClientRequestAdapter(
    authenticationProvider, parseNodeFactory, serializationWriterFactory, httpClient, observabilityOptions
);
