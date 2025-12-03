using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Aviationexam.MoneyErp.Common.Filters;

public class FilterFor<T> where T : class
{
    public static string GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        ReadOnlySpan<char> value
    ) => filterOperator switch
    {
        EFilterOperator.Equal => $"{property}~eq~{value}",
        EFilterOperator.NotEqual => $"{property}~ne~{value}",
        EFilterOperator.LessThan => $"{property}~lt~{value}",
        EFilterOperator.LessThanOrEqual => $"{property}~lte~{value}",
        EFilterOperator.GreaterThan => $"{property}~gt~{value}",
        EFilterOperator.GreaterThanOrEqual => $"{property}~gte~{value}",
        EFilterOperator.StartWith => $"{property}~sw~{value}",
        EFilterOperator.Contains => $"{property}~ct~{value}",
        EFilterOperator.EndWith => $"{property}~ew~{value}",
        _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null),
    };

    public static string GetFilterClause<TP>(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        TP value,
        string? format = null,
        IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        filterOperator, property, value.ToString(format, provider)
    );

    public static string GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        bool value
    ) => GetFilterClause(
        filterOperator, property, value ? "true" : "false"
    );

    public static string GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateTimeOffset value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    public static string GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateTime value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    public static string GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateOnly value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, string>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName<TP>(
        Expression<Func<T, TP>> property
    ) where TP : INumberBase<TP> => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName<TP>(
        Expression<Func<T, TP?>> property
    ) where TP : struct, INumberBase<TP> => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, bool>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, bool?>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateTimeOffset>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateTimeOffset?>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateTime>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateTime?>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateOnly>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, DateOnly?>> property
    ) => GetPropertyName(property.Body);

    private static ReadOnlySpan<char> GetPropertyName(
        Expression expressionBody
    ) => expressionBody switch
    {
        MemberExpression { Member: { MemberType: MemberTypes.Property, Name: { } propertyName } } => propertyName,
        _ => throw new ArgumentOutOfRangeException(nameof(expressionBody), expressionBody, null),
    };
}
