using Microsoft.Kiota.Abstractions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace

namespace Aviationexam.MoneyErp.RestApi.ClientV1.V10.Company;

public partial class CompanyRequestBuilder
{
    public async Task<CompanyGetResponse?> GetAsync(
        RequestInformation requestInformation,
        CancellationToken cancellationToken = default
    ) => await RequestAdapter.SendAsync(
        requestInformation,
        CompanyGetResponse.CreateFromDiscriminatorValue, null, cancellationToken
    ).ConfigureAwait(false);
}
