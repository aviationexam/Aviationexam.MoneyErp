using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp.Common.Oidc;

public interface IMoneyErpAccessTokenProvider
{
    ValueTask<string> GetAuthorizationTokenAsync(
        CancellationToken cancellationToken
    );
}
