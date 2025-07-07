using Microsoft.Extensions.DependencyInjection;

namespace Aviationexam.MoneyErp.Common.Extensions;

public class MoneyErpBuilder(
    IServiceCollection serviceCollection
)
{
    public IServiceCollection Services { get; } = serviceCollection;
}
