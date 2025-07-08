using Aviationexam.MoneyErp.Common;
using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.Graphql.Client;
using Aviationexam.MoneyErp.Graphql.JsonConverters;
using Aviationexam.MoneyErp.Graphql.ZeroQLServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using ZeroQL.Json;
using ZeroQL.Pipelines;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class DependencyInjectionExtensions
{
    public const string MoneyErpGraphqlHttpClient = "MoneyErp.GraphqlHttpClient";

    public static MoneyErpBuilder AddGraphQlClient(
        this MoneyErpBuilder builder,
        Action<OptionsBuilder<MoneyErpGraphqlOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var serviceCollection = builder.Services;

        ZeroQLJsonOptions.Configure(x => x.Converters.Add(new MoneyErpGraphQueryErrorConverter()));

        serviceCollection.AddKeyedScoped<LoggingHandler>(
            MoneyErpGraphqlHttpClient,
            static (serviceProvider, key) => new LoggingHandler(
                serviceProvider.GetRequiredService<TimeProvider>(),
                serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(key!.ToString()!)
            )
        );

        var httpClientBuilder = serviceCollection
            .AddHttpClient<AuthenticatedHttpHandler>(MoneyErpGraphqlHttpClient)
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MoneyErpGraphqlOptions>>();
                httpClient.BaseAddress = options.Value.GraphqlEndpoint;
            })
            .AddDefaultLogger();

        serviceCollection.Configure<HttpClientFactoryOptions>(
            httpClientBuilder.Name,
            options => options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(b.Services.GetRequiredKeyedService<LoggingHandler>(httpClientBuilder.Name)))
        );

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        serviceCollection.TryAddKeyedScoped<IGraphQLQueryPipeline, FullQueryPipeline>(CommonDependencyInjectionExtensions.MoneyErpServiceKey);
        serviceCollection.AddScoped<MoneyErpGraphqlClient>(serviceProvider => new MoneyErpGraphqlClient(
            serviceProvider.GetRequiredService<AuthenticatedHttpHandler>(),
            serviceProvider.GetRequiredKeyedService<IGraphQLQueryPipeline>(CommonDependencyInjectionExtensions.MoneyErpServiceKey)
        ));

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
