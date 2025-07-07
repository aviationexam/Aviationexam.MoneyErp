using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.MoneyErp.Common;

public sealed class MoneyErpAuthenticationOptionsValidate : IValidateOptions<MoneyErpAuthenticationOptions>
{
    public ValidateOptionsResult Validate(string? name, MoneyErpAuthenticationOptions options)
    {
        if (options.JwtEarlyExpirationOffset <= TimeSpan.Zero)
        {
            return ValidateOptionsResult.Fail(
                $"The '{nameof(options.JwtEarlyExpirationOffset)}' option must be a positive value, '{options.JwtEarlyExpirationOffset}' given."
            );
        }

        if (options.JwtEarlyExpirationOffset > TimeSpan.FromMinutes(20))
        {
            return ValidateOptionsResult.Fail(
                $"The '{nameof(options.JwtEarlyExpirationOffset)}' option must not be bigger than 20 minutes, '{options.JwtEarlyExpirationOffset}' given."
            );
        }

        return ValidateOptionsResult.Success;
    }
}
