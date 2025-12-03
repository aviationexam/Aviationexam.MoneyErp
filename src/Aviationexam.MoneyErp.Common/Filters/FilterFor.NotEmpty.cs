using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T>
{
    public static ReadOnlySpan<char> NotEmpty(
        Expression<Func<T, string?>> property
    ) => GetFilterClause(
        EFilterOperator.NotEqual, GetPropertyName(property), string.Empty
    );
}
