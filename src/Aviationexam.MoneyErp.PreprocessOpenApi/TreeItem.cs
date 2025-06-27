using System.Text.Json;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed record TreeItem(
    JsonTokenType JsonTokenType,
    string? PropertyName
);