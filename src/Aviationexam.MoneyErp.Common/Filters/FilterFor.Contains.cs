using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T>
{
    public static ReadOnlySpan<char> Contains(
        Expression<Func<T, string?>> property, string value
    ) => GetFilterClause(
        EFilterOperator.Contains, GetPropertyName(property), value
    );
}
