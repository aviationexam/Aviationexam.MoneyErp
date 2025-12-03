using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static string LessThanOrEqual<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value, format, provider
    );

    public static string LessThanOrEqual<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value, format, provider
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );

    public static string LessThanOrEqual(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.LessThanOrEqual, GetPropertyName(property), value
    );
}
