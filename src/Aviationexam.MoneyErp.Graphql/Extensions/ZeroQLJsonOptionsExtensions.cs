using Aviationexam.MoneyErp.Graphql.JsonConverters;
using System.Text.Json;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class ZeroqlJsonOptionsExtensions
{
    public static void ConfigureMoneyErpZeroqlJsonOptions(
        this JsonSerializerOptions jsonSerializerOptions
    )
    {
        jsonSerializerOptions.Converters.Add(new MoneyErpGraphQueryErrorConverter());
    }
}
