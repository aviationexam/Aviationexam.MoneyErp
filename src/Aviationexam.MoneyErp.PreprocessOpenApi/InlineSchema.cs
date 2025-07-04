namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed record InlineSchema(
    string Type,
    string? Format
) : ISchema;
