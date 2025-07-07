using Aviationexam.MoneyErp.Common;
using Microsoft.Extensions.Options;

namespace Aviationexam.MoneyErp.RestApi;

public sealed class MoneyErpRestApiOptionsPostConfigure(
    IOptions<MoneyErpAuthenticationOptions> authenticationOptions
) : IPostConfigureOptions<MoneyErpRestApiOptions>
{
    public void PostConfigure(string? name, MoneyErpRestApiOptions options)
    {
        if (authenticationOptions.Value is { Endpoint: { } endpoint })
        {
            options.AllowedHosts ??= // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            [
                endpoint.Authority,
            ];
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (options.AllowedHosts is null)
        {
            options.AllowedHosts =
            [
                "demo.moneyerp.cz:82",
            ];
        }
    }
}
