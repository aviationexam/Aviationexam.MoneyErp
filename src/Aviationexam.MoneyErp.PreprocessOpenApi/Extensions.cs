using System.Collections.Generic;
using System.Linq;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public static class Extensions
{
    public static string Collect(
        this IEnumerable<TreeItem> stack
    ) => $"#/{string.Join('/', stack.Select(x => x.PropertyName).Where(x => x is not null))}";
}
