using Microsoft.Extensions.Options;

namespace Aviationexam.MoneyErp;

public sealed class MoneyErpAuthenticationPostConfigure : IPostConfigureOptions<MoneyErpAuthenticationOptions>
{
    public void PostConfigure(string? name, MoneyErpAuthenticationOptions options)
    {
    }
}
