using System.Text.Json;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public record TreeItem(
    JsonTokenType JsonTokenType,
    string? PropertyName
);
