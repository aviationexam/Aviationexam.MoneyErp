using Aviationexam.MoneyErp.Common.Oidc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Aviationexam.MoneyErp.Common.Extensions;

public static class CommonDependencyInjectionExtensions
{
    public const string MoneyErpServiceKey = "MoneyErp";
    public const string MoneyErpHttpTokenClient = "MoneyErp.HttpTokenClient";

    public static MoneyErpBuilder AddMoneyErp(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpAuthenticationOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var httpTokenClientBuilder = serviceCollection.AddHttpClient(MoneyErpHttpTokenClient)
            .AddDefaultLogger();

#if NET9_0_OR_GREATER
        httpTokenClientBuilder.AddAsKeyed();
#else
        serviceCollection.AddKeyedTransient<HttpClient>(httpTokenClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        );
#endif


        if (shouldRedactHeaderValue is false)
        {
            serviceCollection.Configure<HttpClientFactoryOptions>(httpTokenClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
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

        serviceCollection.TryAddKeyedSingleton<IMoneyErpAccessTokenProvider, MoneyErpAccessTokenProvider>(MoneyErpServiceKey);

        return new MoneyErpBuilder(serviceCollection);
    }
}
