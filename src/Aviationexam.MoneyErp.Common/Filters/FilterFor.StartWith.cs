using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static ReadOnlySpan<char> StartWith(
        Expression<Func<T, string?>> property, string value
    ) => GetFilterClause(
        EFilterOperator.StartWith, GetPropertyName(property), value
    );
}
