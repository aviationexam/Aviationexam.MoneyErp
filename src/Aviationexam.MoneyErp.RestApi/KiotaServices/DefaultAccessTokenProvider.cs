using Aviationexam.MoneyErp.Common.Oidc;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp.RestApi.KiotaServices;

public sealed class DefaultAccessTokenProvider(
    IMoneyErpAccessTokenProvider accessTokenProvider,
    IOptions<MoneyErpRestApiOptions> moneyErpRestApiOptions
) : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(
        Uri uri, Dictionary<string, object>? additionalAuthenticationContext, CancellationToken cancellationToken
    ) => accessTokenProvider.GetAuthorizationTokenAsync(cancellationToken).AsTask();

    public AllowedHostsValidator AllowedHostsValidator { get; } = new(moneyErpRestApiOptions.Value.AllowedHosts);
}
