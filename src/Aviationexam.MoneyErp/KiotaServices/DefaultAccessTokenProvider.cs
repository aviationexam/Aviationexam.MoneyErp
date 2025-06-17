using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp.KiotaServices;

public sealed class DefaultAccessTokenProvider(
    IOptions<MoneyErpAuthenticationOptions> authenticationOptions
) : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(
        Uri uri, Dictionary<string, object>? additionalAuthenticationContext, CancellationToken cancellationToken
    ) => Task.FromResult<string>(null!);

    public AllowedHostsValidator AllowedHostsValidator { get; } = new(authenticationOptions.Value.AllowedHosts);
}