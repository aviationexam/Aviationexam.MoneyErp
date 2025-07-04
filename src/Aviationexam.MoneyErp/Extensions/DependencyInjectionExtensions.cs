using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.KiotaServices;
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

namespace Aviationexam.MoneyErp.Extensions;

public static class DependencyInjectionExtensions
{
    public const string MoneyErpHttpClient = "MoneyErp.HttpClient";
    public const string MoneyErpHttpTokenClient = "MoneyErp.HttpTokenClient";
    public const string MoneyErpServiceKey = "MoneyErp";

    public static IServiceCollection AddMoneyErpApiClient(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpAuthenticationOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        serviceCollection.AddKeyedScoped<LoggingHandler>(
            MoneyErpHttpClient,
            static (serviceProvider, key) => new LoggingHandler(serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(key!.ToString()!))
        );

        var httpClientBuilder = serviceCollection.AddHttpClient(MoneyErpHttpClient)
            .AttachKiotaHandlers()
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpAuthenticationOptions>>();
                httpClient.BaseAddress = options.Value.Endpoint;
            })
            .AddDefaultLogger();
        var httpTokenClientBuilder = serviceCollection.AddHttpClient(MoneyErpHttpTokenClient)
            .AddDefaultLogger();

        serviceCollection.Configure<HttpClientFactoryOptions>(
            httpClientBuilder.Name,
            options => options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(b.Services.GetRequiredKeyedService<LoggingHandler>(httpClientBuilder.Name)))
        );

#if NET9_0_OR_GREATER
        httpClientBuilder.AddAsKeyed();
        httpTokenClientBuilder.AddAsKeyed();
#else
        serviceCollection.AddKeyedTransient<HttpClient>(httpClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        ).AddKeyedTransient<HttpClient>(httpTokenClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        );
#endif

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false)
                .Configure<HttpClientFactoryOptions>(httpTokenClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        optionsBuilder(serviceCollection
            .AddOptions<MoneyErpAuthenticationOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<MoneyErpAuthenticationOptions>, MoneyErpAuthenticationPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<MoneyErpAuthenticationOptions>, MoneyErpAuthenticationOptionsValidate>()
        );

        serviceCollection.TryAddKeyedTransient<IRequestAdapter, DefaultHttpClientRequestAdapter>(MoneyErpServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAuthenticationProvider, DefaultAuthenticationProvider>(MoneyErpServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAccessTokenProvider, DefaultAccessTokenProvider>(MoneyErpServiceKey);

        serviceCollection.AddTransient<MoneyErpApiClient>(serviceProvider => new MoneyErpApiClient(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(MoneyErpServiceKey)
        ));

        return serviceCollection;
    }
}
