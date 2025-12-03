using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static string Empty(
        Expression<Func<T, string>> property
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), string.Empty
    );
}
