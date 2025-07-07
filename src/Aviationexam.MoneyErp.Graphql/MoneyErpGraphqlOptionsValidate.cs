using Microsoft.Extensions.Options;

namespace Aviationexam.MoneyErp.Graphql;

public sealed class MoneyErpGraphqlOptionsValidate : IValidateOptions<MoneyErpGraphqlOptions>
{
    public ValidateOptionsResult Validate(
        string? name, MoneyErpGraphqlOptions options
    ) => ValidateOptionsResult.Success;
}
