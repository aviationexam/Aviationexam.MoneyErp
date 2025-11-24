using Aviationexam.MoneyErp.Common.Oidc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.ZeroQLServices;

public class AuthenticatedHttpHandler(
    HttpClient client,
    IMoneyErpAccessTokenProvider accessTokenProvider,
    bool disposeClient = false
) : IHttpHandler
{
    public void Dispose()
    {
        if (!disposeClient)
        {
            return;
        }

        client.Dispose();
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var accessToken = await accessTokenProvider.GetAuthorizationTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await client.SendAsync(request, cancellationToken);
    }
}
