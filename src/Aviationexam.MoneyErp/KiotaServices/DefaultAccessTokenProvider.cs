using Aviationexam.MoneyErp.Common.Extensions;
using Aviationexam.MoneyErp.Common.Oidc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp.KiotaServices;

public sealed class DefaultAccessTokenProvider(
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    IMoneyErpAccessTokenProvider accessTokenProvider,
    IOptions<MoneyErpRestApiOptions> moneyErpRestApiOptions
) : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(
        Uri uri, Dictionary<string, object>? additionalAuthenticationContext, CancellationToken cancellationToken
    ) => accessTokenProvider.GetAuthorizationTokenAsync(cancellationToken).AsTask();

    public AllowedHostsValidator AllowedHostsValidator { get; } = new(moneyErpRestApiOptions.Value.AllowedHosts);
}
