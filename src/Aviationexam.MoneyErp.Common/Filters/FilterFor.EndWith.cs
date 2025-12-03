using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static string EndWith(
        Expression<Func<T, string>> property, string value
    ) => GetFilterClause(
        EFilterOperator.EndWith, GetPropertyName(property), value
    );
}
