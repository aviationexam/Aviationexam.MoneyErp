using Aviationexam.MoneyErp.Graphql.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ZeroQL.Json;

namespace Aviationexam.MoneyErp.Graphql;

public class Initializer
{
    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries")]
    public static void Initialize() => ZeroQLJsonOptions.Configure(x => x.ConfigureMoneyErpZeroqlJsonOptions());
}
