using Aviationexam.MoneyErp.Common.Oidc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Aviationexam.MoneyErp.Common.Extensions;

public static class CommonDependencyInjectionExtensions
{
    public const string MoneyErpServiceKey = "MoneyErp";
    public const string MoneyErpHttpTokenClient = "MoneyErp.HttpTokenClient";

    public static HttpMessageHandler CreateHttpMessageHandler(
        this IServiceProvider serviceProvider
    )
    {
        var endpointCertificate = serviceProvider.GetRequiredService<IOptions<MoneyErpAuthenticationOptions>>().Value.EndpointCertificate;

        var httpClientHandler = new HttpClientHandler();

        if (endpointCertificate is not null)
        {
            httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            httpClientHandler.ClientCertificates.Add(endpointCertificate);

#pragma warning disable MA0039
            httpClientHandler.ServerCertificateCustomValidationCallback =
#pragma warning restore MA0039
#pragma warning disable format
                [SuppressMessage("ReSharper", "UnusedParameter.Local")]
                (sender, cert, chain, sslPolicyErrors) =>
#pragma warning restore format
                {
                    var chain2 = new X509Chain();
                    chain2.ChainPolicy.ExtraStore.Add(endpointCertificate);
                    chain2.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    chain2.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    return chain2.Build(cert ?? throw new ArgumentNullException(nameof(cert)));
                };
        }

        return httpClientHandler;
    }

    public static MoneyErpBuilder AddMoneyErp(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<MoneyErpAuthenticationOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var httpTokenClientBuilder = serviceCollection.AddHttpClient(MoneyErpHttpTokenClient)
            .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
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
