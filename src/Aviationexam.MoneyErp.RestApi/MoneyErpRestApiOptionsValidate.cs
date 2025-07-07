using Microsoft.Extensions.Options;

namespace Aviationexam.MoneyErp.RestApi;

public sealed class MoneyErpRestApiOptionsValidate : IValidateOptions<MoneyErpRestApiOptions>
{
    public ValidateOptionsResult Validate(
        string? name, MoneyErpRestApiOptions options
    ) => ValidateOptionsResult.Success;
}
