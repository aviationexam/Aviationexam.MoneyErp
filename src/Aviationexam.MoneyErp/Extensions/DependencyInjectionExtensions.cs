using Aviationexam.MoneyErp.Client;
using Aviationexam.MoneyErp.KiotaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Net.Http;

namespace Aviationexam.MoneyErp.Extensions;

public static class DependencyInjectionExtensions
{
    public const string MoneyErpHttpClient = "MoneyErp.Client";
    public const string MoneyErpServiceKey = "MoneyErp";

    public static IServiceCollection AddMoneyErpApiClient(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpAuthenticationOptions>> optionsBuilder
    )
    {
        serviceCollection.AddHttpClient(MoneyErpHttpClient).AttachKiotaHandlers();

        optionsBuilder(serviceCollection
            .AddOptions<MoneyErpAuthenticationOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<MoneyErpAuthenticationOptions>, MoneyErpAuthenticationPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<MoneyErpAuthenticationOptions>, MoneyErpAuthenticationOptionsValidate>()
        );

        serviceCollection.AddKeyedTransient<HttpClient>(
            MoneyErpServiceKey,
            (serviceProvider, _) => serviceProvider
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(MoneyErpHttpClient)
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
