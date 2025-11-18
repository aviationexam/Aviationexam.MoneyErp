using Aviationexam.MoneyErp.Common;
using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.RestApi.ClientV1;
using Aviationexam.MoneyErp.RestApi.ClientV2;
using Aviationexam.MoneyErp.RestApi.KiotaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
#if NET8_0
using System.Net.Http;
#endif

namespace Aviationexam.MoneyErp.RestApi.Extensions;

public static class DependencyInjectionExtensions
{
    public const string MoneyErpRestApiHttpClient = "MoneyErp.RestApiHttpClient";

    public static MoneyErpBuilder AddRestApiClient(
        this MoneyErpBuilder builder,
        Action<OptionsBuilder<MoneyErpRestApiOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var serviceCollection = builder.Services;
        serviceCollection.AddKeyedScoped<LoggingHandler>(
            MoneyErpRestApiHttpClient,
            static (serviceProvider, key) => new LoggingHandler(
                serviceProvider.GetRequiredService<TimeProvider>(),
                serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(key!.ToString()!)
            )
        );

        var httpClientBuilder = serviceCollection.AddHttpClient(MoneyErpRestApiHttpClient)
            .AttachKiotaHandlers()
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpAuthenticationOptions>>();
                httpClient.BaseAddress = options.Value.Endpoint;
            })
            .ConfigurePrimaryHttpMessageHandler(CommonDependencyInjectionExtensions.CreateHttpMessageHandler)
            .AddDefaultLogger();

        serviceCollection.Configure<HttpClientFactoryOptions>(
            httpClientBuilder.Name,
            options => options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(b.Services.GetRequiredKeyedService<LoggingHandler>(httpClientBuilder.Name)))
        );

#if NET9_0_OR_GREATER
        httpClientBuilder.AddAsKeyed();
#else
        serviceCollection.AddKeyedTransient<HttpClient>(httpClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        );
#endif

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        optionsBuilder(serviceCollection
            .AddOptions<MoneyErpRestApiOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<MoneyErpRestApiOptions>, MoneyErpRestApiOptionsPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<MoneyErpRestApiOptions>, MoneyErpRestApiOptionsValidate>()
        );

        serviceCollection.TryAddKeyedTransient<IRequestAdapter, DefaultHttpClientRequestAdapter>(CommonDependencyInjectionExtensions.MoneyErpServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAuthenticationProvider, DefaultAuthenticationProvider>(CommonDependencyInjectionExtensions.MoneyErpServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAccessTokenProvider, DefaultAccessTokenProvider>(CommonDependencyInjectionExtensions.MoneyErpServiceKey);

        serviceCollection.AddTransient<MoneyErpApiV1Client>(serviceProvider => new MoneyErpApiV1Client(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(CommonDependencyInjectionExtensions.MoneyErpServiceKey)
        ));
        serviceCollection.AddTransient<MoneyErpApiV2Client>(serviceProvider => new MoneyErpApiV2Client(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(CommonDependencyInjectionExtensions.MoneyErpServiceKey)
        ));

        return builder;
    }
}
