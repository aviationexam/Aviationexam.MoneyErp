using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T> where T : class
{
    public const char AndOperator = '#';
    public const char OrOperator = '|';

    private static ReadOnlySpan<char> CombineExpressions(
        char joiningCharacter,
        IReadOnlyCollection<FilterForBuilder<T>.Filter> filters
    )
    {
        var builder = new FilterForBuilder<T>();

        var resultBuilder = new StringBuilder();
        var isFirst = true;
        foreach (var filter in filters)
        {
            if (!isFirst)
            {
                resultBuilder.Append(joiningCharacter);
            }

            isFirst = false;

            resultBuilder.Append(filter(builder));
        }

        return resultBuilder.ToString();
    }

    public static ReadOnlySpan<char> CombineExpressions<TValue>(
        char joiningCharacter,
        IReadOnlyCollection<TValue> values, FilterForBuilder<T>.Filter<TValue> filter
    )
    {
        var resultBuilder = new StringBuilder();
        var isFirst = true;
        foreach (var value in values)
        {
            if (!isFirst)
            {
                resultBuilder.Append(joiningCharacter);
            }

            isFirst = false;

            resultBuilder.Append(filter(value));
        }

        return resultBuilder.ToString();
    }

    public static ReadOnlySpan<char> And(
        params IReadOnlyCollection<FilterForBuilder<T>.Filter> filters
    ) => CombineExpressions(AndOperator, filters);

    public static ReadOnlySpan<char> And(
        EFilterOperator filterOperator,
        Expression<Func<T, string?>> property,
        IReadOnlyCollection<string> values
    ) => CombineExpressions(AndOperator, values, value => GetFilterClause(filterOperator, GetPropertyName(property), value));

    public static ReadOnlySpan<char> Or(
        params IReadOnlyCollection<FilterForBuilder<T>.Filter> filters
    ) => CombineExpressions(OrOperator, filters);

    public static ReadOnlySpan<char> Or(
        EFilterOperator filterOperator,
        Expression<Func<T, string?>> property,
        IReadOnlyCollection<string> values
    ) => CombineExpressions(OrOperator, values, value => GetFilterClause(filterOperator, GetPropertyName(property), value));


    public static ReadOnlySpan<char> Or(
        EFilterOperator filterOperator,
        Expression<Func<T, Guid?>> property,
        IReadOnlyCollection<Guid> values
    ) => CombineExpressions(
        OrOperator,
        values,
        value => GetFilterClause(filterOperator, GetPropertyName(property), value)
    );


    public static ReadOnlySpan<char> GetFilterClause(
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

    public static ReadOnlySpan<char> GetFilterClause<TP>(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        TP value,
        string? format = null,
        IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => GetFilterClause(
        filterOperator, property, value.ToString(format, provider)
    );

    public static ReadOnlySpan<char> GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        Guid value
    ) => GetFilterClause(
        filterOperator, property, value.ToString()
    );

    public static ReadOnlySpan<char> GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        bool value
    ) => GetFilterClause(
        filterOperator, property, value ? "true" : "false"
    );

    public static ReadOnlySpan<char> GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateTimeOffset value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    public static ReadOnlySpan<char> GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateTime value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    public static ReadOnlySpan<char> GetFilterClause(
        EFilterOperator filterOperator,
        ReadOnlySpan<char> property,
        DateOnly value
    ) => GetFilterClause(
        filterOperator, property, value.ToString("yyyy-MM-dd")
    );

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, string?>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName<TP>(
        Expression<Func<T, TP>> property
    ) where TP : INumberBase<TP> => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName<TP>(
        Expression<Func<T, TP?>> property
    ) where TP : struct, INumberBase<TP> => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, Guid>> property
    ) => GetPropertyName(property.Body);

    internal static ReadOnlySpan<char> GetPropertyName(
        Expression<Func<T, Guid?>> property
    ) => GetPropertyName(property.Body);

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

    public static ReadOnlySpan<char> GetPropertyName(
        Expression expressionBody
    ) => expressionBody switch
    {
        MemberExpression { Member: { MemberType: MemberTypes.Property, Name: { } propertyName } } => propertyName,
        _ => throw new ArgumentOutOfRangeException(nameof(expressionBody), expressionBody, null),
    };
}
