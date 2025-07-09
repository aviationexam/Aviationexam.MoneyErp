using Microsoft.Kiota.Abstractions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Aviationexam.MoneyErp.RestApi.Client.V10.Connection;

public partial class ConnectionRequestBuilder
{
    public async Task<ConnectionGetResponse?> GetAsync(
        RequestInformation requestInformation,
        CancellationToken cancellationToken = default
    ) => await RequestAdapter.SendAsync(
            requestInformation,
            ConnectionGetResponse.CreateFromDiscriminatorValue, null, cancellationToken
        )
        .ConfigureAwait(false);
}
