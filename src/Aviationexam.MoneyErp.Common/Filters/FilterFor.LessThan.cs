using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T>
{
    public string LessThan<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        EFilterOperator.LessThan, GetPropertyName(property), value.ToString(format, provider)
    );

    public string LessThan(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThan, GetPropertyName(property), value.ToString("yyyy-MM-dd")
    );

    public string LessThan(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThan, GetPropertyName(property), value.ToString("yyyy-MM-dd")
    );

    public string LessThan(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value.ToString("yyyy-MM-dd")
    );
}
