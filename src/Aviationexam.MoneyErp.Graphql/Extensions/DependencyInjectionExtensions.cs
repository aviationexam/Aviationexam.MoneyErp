using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.Graphql.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class DependencyInjectionExtensions
{
    public static MoneyErpBuilder AddGraphQlClient(
        this MoneyErpBuilder builder,
        Action<OptionsBuilder<MoneyErpGraphqlOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var serviceCollection = builder.Services;

        var httpClientBuilder = serviceCollection
            .AddHttpClient<MoneyErpGraphqlClient>()
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpGraphqlOptions>>();
                httpClient.BaseAddress = options.Value.GraphqlEndpoint;
            })
            .AddDefaultLogger();

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        optionsBuilder(serviceCollection
            .AddOptions<MoneyErpGraphqlOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<MoneyErpGraphqlOptions>, MoneyErpGraphqlOptionsPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<MoneyErpGraphqlOptions>, MoneyErpGraphqlOptionsValidate>()
        );

        return builder;
    }
}
