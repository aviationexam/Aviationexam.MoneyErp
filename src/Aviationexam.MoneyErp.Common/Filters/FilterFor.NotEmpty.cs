using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static string NotEmpty(
        Expression<Func<T, string>> property
    ) => GetFilterClause(
        EFilterOperator.NotEqual, GetPropertyName(property), string.Empty
    );
}
