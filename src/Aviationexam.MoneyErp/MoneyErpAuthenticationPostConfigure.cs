using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.MoneyErp;

public sealed class MoneyErpAuthenticationPostConfigure : IPostConfigureOptions<MoneyErpAuthenticationOptions>
{
    public void PostConfigure(string? name, MoneyErpAuthenticationOptions options)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (options.JwtEarlyExpirationOffset == TimeSpan.Zero)
        {
            options.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
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
