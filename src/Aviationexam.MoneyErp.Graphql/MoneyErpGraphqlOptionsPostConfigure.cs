using Aviationexam.MoneyErp.Common;
using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.MoneyErp.Graphql;

public sealed class MoneyErpGraphqlOptionsPostConfigure(
    IOptions<MoneyErpAuthenticationOptions> authenticationOptions
) : IPostConfigureOptions<MoneyErpGraphqlOptions>
{
    public void PostConfigure(string? name, MoneyErpGraphqlOptions options)
    {
        if (authenticationOptions.Value is { Endpoint: { } endpoint })
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            options.GraphqlEndpoint ??= new Uri(endpoint, options.GraphqlPath);
        }
    }
}
