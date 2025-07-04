using System.Text.Json;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed record SchemaTreeItem(
    JsonTokenType JsonTokenType,
    string? PropertyName
) : TreeItem(
    JsonTokenType,
    PropertyName
)
{
    public bool IsArray { get; set; }

    public string? Type { get; set; }

    public string? Format { get; set; }
}
