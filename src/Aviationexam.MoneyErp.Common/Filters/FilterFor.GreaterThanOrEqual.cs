using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static string GreaterThanOrEqual<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value, format, provider
    );

    public static string GreaterThanOrEqual<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value, format, provider
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );

    public static string GreaterThanOrEqual(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.GreaterThanOrEqual, GetPropertyName(property), value
    );
}
