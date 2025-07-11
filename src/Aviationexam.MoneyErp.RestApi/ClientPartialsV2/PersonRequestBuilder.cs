using Microsoft.Kiota.Abstractions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Aviationexam.MoneyErp.RestApi.ClientV2.V20.Person;

public partial class PersonRequestBuilder
{
    public async Task<PersonGetResponse?> GetAsync(
        RequestInformation requestInformation,
        CancellationToken cancellationToken = default
    ) => await RequestAdapter.SendAsync(
        requestInformation,
        PersonGetResponse.CreateFromDiscriminatorValue, null, cancellationToken
    ).ConfigureAwait(false);
}
