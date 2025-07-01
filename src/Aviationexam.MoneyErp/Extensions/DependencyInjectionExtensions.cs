using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.KiotaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;

namespace Aviationexam.MoneyErp.Extensions;

public static class DependencyInjectionExtensions
{
    public const string MoneyErpHttpClient = "MoneyErp.Client";
    public const string MoneyErpHttpTokenClient = "MoneyErp.TokenClient";
    public const string MoneyErpServiceKey = "MoneyErp";

    public static IServiceCollection AddMoneyErpApiClient(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpAuthenticationOptions>> optionsBuilder
    )
    {
        serviceCollection.AddHttpClient(MoneyErpHttpClient)
            .AttachKiotaHandlers()
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpAuthenticationOptions>>();
                httpClient.BaseAddress = options.Value.Endpoint;
            })
            .AddAsKeyed();
        serviceCollection.AddHttpClient(MoneyErpHttpTokenClient)
            .AddAsKeyed();

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
