using Aviationexam.MoneyErp.Graphql.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMoneyErpApiGraphQlClient(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpGraphQlAuthenticationOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        optionsBuilder(serviceCollection
            .AddOptions<MoneyErpGraphQlAuthenticationOptions>()
        );

        var httpClientBuilder = serviceCollection
            .AddHttpClient<MoneyErpGraphqlClient>()
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpGraphQlAuthenticationOptions>>();
                httpClient.BaseAddress = options.Value.GraphQlEndpoint;
            })
            .AddDefaultLogger();

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        return serviceCollection;
    }
}
