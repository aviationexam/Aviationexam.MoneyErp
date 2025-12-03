using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterFor<T>
{
    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, string>> property, string value
    ) => GetFilterClause(EFilterOperator.Equal, GetPropertyName(property), value);

    public static ReadOnlySpan<char> Equal<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value, format, provider
    );

    public static ReadOnlySpan<char> Equal<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value, format, provider
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, bool>> property, bool value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, bool?>> property, bool value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );

    public static ReadOnlySpan<char> Equal(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => GetFilterClause(
        EFilterOperator.Equal, GetPropertyName(property), value
    );
}
