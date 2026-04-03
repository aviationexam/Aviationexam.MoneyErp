using OpenTelemetry.Trace;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddMoneyErpGraphqlInstrumentation(
        this TracerProviderBuilder builder
    ) => builder.AddSource(InstrumentationConstants.ActivitySourceName);
}
