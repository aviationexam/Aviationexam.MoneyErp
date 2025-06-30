using System.Text.Json;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public record ParameterTreeItem(
    JsonTokenType JsonTokenType,
    string? PropertyName
) : TreeItem(
    JsonTokenType,
    PropertyName
)
{
    public string? Name { get; set; }

    public string? In { get; set; }
}
