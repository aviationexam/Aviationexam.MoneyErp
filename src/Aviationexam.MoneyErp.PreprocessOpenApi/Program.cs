using Aviationexam.MoneyErp.PreprocessOpenApi;
using System;

if (args is [{ } source, { } target])
{
    new OpenApiPreprocessor(
        source, target
    ).Preprocess();
}
else
{
    Console.WriteLine("Usage: <source> <target>");
}
