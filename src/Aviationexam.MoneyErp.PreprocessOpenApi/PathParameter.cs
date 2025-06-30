namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed record PathParameter(
    string TemplatePath,
    string Path,
    int ParameterId,
    string Name,
    string Type,
    string? Format
);
