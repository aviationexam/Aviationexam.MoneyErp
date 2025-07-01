using Aviationexam.MoneyErp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Aviationexam.MoneyErp.KiotaServices;

public sealed class DefaultAccessTokenProvider(
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpHttpTokenClient)]
    HttpClient httpClient,
    IOptions<MoneyErpAuthenticationOptions> authenticationOptions
) : IAccessTokenProvider
{
    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri, Dictionary<string, object>? additionalAuthenticationContext, CancellationToken cancellationToken
    )
    {
        var options = authenticationOptions.Value;

        using var request = new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint);
        request.Content = new FormUrlEncodedContent([
            KeyValuePair.Create("grant_type", "client_credentials"),
            KeyValuePair.Create("client_id", options.ClientId),
            KeyValuePair.Create("client_secret", options.ClientSecret),
            KeyValuePair.Create("scope", options.TokenScope),
        ]);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var tokenResponse = await JsonSerializer.DeserializeAsync(jsonStream, TokenResponseJsonContext.Default.TokenResponse, cancellationToken);

        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Failed to deserialize token response.");
        }

        return tokenResponse.AccessToken;
    }

    public AllowedHostsValidator AllowedHostsValidator { get; } = new(authenticationOptions.Value.AllowedHosts);
}
